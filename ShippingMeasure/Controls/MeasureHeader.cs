using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShippingMeasure.Core.Models;
using ShippingMeasure.Core;
using ShippingMeasure.Common;
using ShippingMeasure.Db;
using DropDownControls;

namespace ShippingMeasure.Controls
{
    public partial class MeasureHeader : UserControl
    {
        public ReceiptDb ReceiptDb { get; set; }
        public TankDb TankDb { get; set; }
        public string VesselName { get; set; }
        public bool IsDirty { get; private set; }
        public bool IsNewReceipt { get { return !this.txtReceiptNo.ReadOnly; } }
        public string ValidationErrorMessage
        {
            get { return this.validationErrorMessage; }
            set
            {
                this.validationErrorMessage = value;
                this.OnValidationErrorMessageChanged();
            }
        }

        public event Action<decimal?> HInclinationChanged;
        public event Action<decimal?> VInclinationChanged;
        public event Action Dirty;
        public event Action<string> ValidationErrorMessageChanged;
        public event Action<decimal?> TotalOfVolumeOfPipesChanged;
        public event Action<KindOfGoods> SelectedKindOfGoodsChanged; 

        private bool loading = false;
        private ComboTreeNode SelectedKindOfGoodsNode = null;
        private string validationErrorMessage = String.Empty;
        private List<KeyValuePair<Control, string>> validationErrorMessages = new List<KeyValuePair<Control, string>>();

        private string HStatus
        {
            get { return this.rdoListToPort.Checked ? "P" : (this.rdoListToStarboard.Checked ? "S" : String.Empty); }
        }

        public decimal TotalOfVolumeOfPipes
        {
            get
            {
                return cmbPipes.Nodes.Cast<ComboTreeNode>().Aggregate(0m, (v, item) => item.Checked ? v + ((Pipe)item.Tag).Volume : v);
            }
        }

        public MeasureHeader()
        {
            InitializeComponent();
        }

        public void LoadReceipt(Receipt receipt)
        {
            this.loading = true;

            this.txtVesselName.Text = receipt.VesselName;
            this.txtTime.Text = receipt.Time.Value.ToString("yyyy-MM-dd");
            this.rdoLoad.Checked = receipt.ReceiptFor == "Load";
            this.rdoDischarge.Checked = receipt.ReceiptFor == "Discharge";
            this.txtPortOfShipment.Text = receipt.PortOfShipment;
            this.txtPortOfDestination.Text = receipt.PortOfDestination;
            this.cmbKindOfGoods.SelectedNode = this.cmbKindOfGoods.Nodes.RecursivelyFirstOrDefault(n => ((KindOfGoods)n.Tag).Equals(receipt.KindOfGoods));
            this.rdoListToPort.Checked = receipt.VesselStatus.IsEmpty ? false : receipt.VesselStatus.HStatus == HInclinationStatus.P;
            this.rdoListToStarboard.Checked = receipt.VesselStatus.IsEmpty ? false : receipt.VesselStatus.HStatus == HInclinationStatus.S;
            this.txtHInclination.Text = receipt.VesselStatus.IsEmpty ? String.Empty : receipt.VesselStatus.HValue.ToString();
            this.txtVInclination.Text = receipt.VesselStatus.IsEmpty ? String.Empty : receipt.VesselStatus.VValue.ToString();
            this.txtForeDraft.Text = receipt.VesselStatus.IsEmpty ? String.Empty : receipt.VesselStatus.ForeDraft.ToString();
            this.txtAftDraft.Text = receipt.VesselStatus.IsEmpty ? String.Empty : receipt.VesselStatus.AftDraft.ToString();
            this.txtReceiptNo.Text = receipt.No;
            this.txtReceiptNo.ReadOnly = true;

            this.SelectPipes(receipt);

            this.loading = false;
            this.ReportLoaded();
        }

        public void GetReceiptHeader(Receipt receipt, bool validate = true)
        {
            if (validate)
            {
                var error = new[]
                {
                    this.txtReceiptNo.Text.ValidateNotEmpty(lblReceiptNo.Text, true),
                    this.txtVesselName.Text.ValidateNotEmpty(lblVesselName.Text, true),
                    this.txtTime.Text.ValidateDate(Strings.LabelTime),
                    (!this.rdoLoad.Checked && !this.rdoDischarge.Checked) ? String.Format(Strings.MsgValidationErrorCannotBeEmpty, Strings.LabelReceiptFor) : null,
                    this.txtPortOfShipment.Text.ValidateNotEmpty(lblPortOfShipment.Text, true),
                    this.txtPortOfDestination.Text.ValidateNotEmpty(lblPortOfDestination.Text, true),
                    this.cmbKindOfGoods.SelectedNode == null ? String.Format(Strings.MsgValidationErrorCannotBeEmpty, lblKindOfGoods.Text) : null,
                    !VesselStatus.Validate(this.HStatus, this.txtHInclination.Text, this.txtAftDraft.Text, this.txtForeDraft.Text, this.txtVInclination.Text) ? String.Format(Strings.MsgValidationErrorInvalidVesselStatus, lblVesselStatus.Text) : null
                }
                .Where(e => e != null)
                .Aggregate(new StringBuilder(), (sb, e) => sb.AppendLine(e)).ToString();

                if (error.Length > 0)
                {
                    throw new ValidationException(error);
                }
            }

            var r = new Receipt();
            r.No = this.txtReceiptNo.Text.Trim();
            r.VesselName = this.txtVesselName.Text.Trim();
            r.Time = this.txtTime.Text.Trim().TryToNullableDateTime();
            r.ReceiptFor = this.rdoLoad.Checked ? "Load" : (this.rdoDischarge.Checked ? "Discharge" : String.Empty);
            r.PortOfShipment = this.txtPortOfShipment.Text.Trim();
            r.PortOfDestination = this.txtPortOfDestination.Text.Trim();
            r.KindOfGoods = this.cmbKindOfGoods.SelectedNode == null ? null : (KindOfGoods)this.cmbKindOfGoods.SelectedNode.Tag;
            try { r.VesselStatus = VesselStatus.Create(this.HStatus, this.txtHInclination.Text, this.txtAftDraft.Text, this.txtForeDraft.Text, this.txtVInclination.Text); } catch { }
            r.TotalOfVolumeOfPipes = this.TotalOfVolumeOfPipes;

            receipt.No = r.No;
            receipt.VesselName = r.VesselName;
            receipt.Time = r.Time;
            receipt.ReceiptFor = r.ReceiptFor;
            receipt.PortOfShipment = r.PortOfShipment;
            receipt.PortOfDestination = r.PortOfDestination;
            receipt.KindOfGoods = r.KindOfGoods;
            receipt.VesselStatus = r.VesselStatus;
            receipt.TotalOfVolumeOfPipes = r.TotalOfVolumeOfPipes;
        }

        public void NewReceipt()
        {
            this.loading = true;

            this.txtVesselName.Text = this.VesselName;
            this.txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.rdoLoad.Checked = false;
            this.rdoDischarge.Checked = false;
            this.txtPortOfShipment.Text = String.Empty;
            this.txtPortOfDestination.Text = String.Empty;
            this.cmbKindOfGoods.SelectedNode = null;
            this.rdoListToPort.Checked = false;
            this.rdoListToStarboard.Checked = false;
            this.txtHInclination.Text = String.Empty;
            this.txtForeDraft.Text = String.Empty;
            this.txtAftDraft.Text = String.Empty;
            this.txtVInclination.Text = String.Empty;
            this.cmbPipes.Nodes.RecursivelyForEach(n => n.Checked = false);
            this.txtReceiptNo.Text = String.Empty;
            this.txtReceiptNo.ReadOnly = false;

            this.loading = false;
            this.ReportLoaded();
        }

        public void ReportSaved()
        {
            this.txtReceiptNo.ReadOnly = true;
            this.IsDirty = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.DesignMode)
            {
                this.txtVesselName.Text = this.VesselName;
                this.txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                this.LoadKindsOfGoods();
                this.cmbPipes.Nodes.AddRange(this.TankDb.GetVessel().Pipes.Select(p => new ComboTreeNode(p.ToString()) { Tag = p }).ToArray());
            }

            var tryChangeVInlination = new EventHandler((s, a) =>
            {
                decimal foreDraft = 0, aftDraft = 0;
                var parsed = Decimal.TryParse(this.txtForeDraft.Text, out foreDraft) && Decimal.TryParse(this.txtAftDraft.Text, out aftDraft);
                if (parsed)
                {
                    this.txtVInclination.Text = (aftDraft - foreDraft).ToString();
                }
                else
                {
                    this.txtVInclination.Text = String.Empty;
                }
            });
            this.txtForeDraft.TextChanged += tryChangeVInlination;
            this.txtAftDraft.TextChanged += tryChangeVInlination;
            var hInclinationChanged = new Action(() =>
            {
                decimal result;
                var parsed = Decimal.TryParse(this.txtHInclination.Text.Trim(), out result)
                    && (this.rdoListToPort.Checked || this.rdoListToStarboard.Checked);
                if (parsed)
                {
                    if (this.rdoListToPort.Checked)
                    {
                        result = -result;
                    }
                    this.OnHInclinationChanged(result);
                }
                else
                {
                    this.OnHInclinationChanged(null);
                }
            });
            this.txtHInclination.TextChanged += (s, a) => hInclinationChanged();
            this.rdoListToPort.CheckedChanged += (s, a) => hInclinationChanged();
            this.rdoListToStarboard.CheckedChanged += (s, a) => hInclinationChanged();
            this.txtVInclination.TextChanged += (s, a) =>
            {
                decimal result;
                var parsed = Decimal.TryParse(this.txtVInclination.Text.Trim(), out result);
                if (parsed)
                {
                    this.OnVInclinationChanged(result);
                }
                else
                {
                    this.OnVInclinationChanged(null);
                }
            };

            this.txtVesselName.TextChanged += (s, a) => this.ReportDirty();
            this.txtTime.TextChanged += (s, a) => this.ReportDirty();
            this.rdoLoad.CheckedChanged += (s, a) => this.ReportDirty();
            this.rdoDischarge.CheckedChanged += (s, a) => this.ReportDirty();
            this.txtPortOfShipment.TextChanged += (s, a) => this.ReportDirty();
            this.txtPortOfDestination.TextChanged += (s, a) => this.ReportDirty();
            this.cmbKindOfGoods.SelectedNodeChanged += (s, a) => this.ReportDirty();
            this.rdoListToPort.CheckedChanged += (s, a) => this.ReportDirty();
            this.rdoListToStarboard.CheckedChanged += (s, a) => this.ReportDirty();
            this.txtHInclination.TextChanged += (s, a) => this.ReportDirty();
            this.txtForeDraft.TextChanged += (s, a) => this.ReportDirty();
            this.txtAftDraft.TextChanged += (s, a) => this.ReportDirty();
            this.txtVInclination.TextChanged += (s, a) => this.ReportDirty();
            this.cmbPipes.AfterCheck += (s, a) => { this.ReportDirty(); this.OnTotalOfVolumeOfPipesChanged(this.TotalOfVolumeOfPipes); };
            this.txtReceiptNo.TextChanged += (s, a) => this.ReportDirty();
            this.cmbKindOfGoods.SelectedNodeChanged += (s, a) => this.OnSelectedKindChanged();

            this.SetupValidation();
        }

        private void SetupValidation()
        {
            var handleValidation = new Action<object, CancelEventArgs, bool, string>((control, args, failed, errorMessage) =>
            {
                var ctrl = (Control)control;
                //args.Cancel = failed;
                this.validationErrorMessages.RemoveAll(p => p.Key == control);

                if (failed)
                {
                    var pair = this.validationErrorMessages.FirstOrDefault(p => p.Key == control);
                    this.validationErrorMessages.Add(new KeyValuePair<Control, string>(ctrl, errorMessage));
                    this.ValidationErrorMessage = errorMessage;
                }
                else
                {
                    string error = null;
                    if (this.validationErrorMessages.Count() > 0)
                    {
                        error = this.validationErrorMessages[this.validationErrorMessages.Count() - 1].Value;
                    }
                    this.ValidationErrorMessage = error;
                }
            });

            decimal temp; // used to try parse decimal values.

            this.txtVesselName.Validating += (s, a) => handleValidation(s, a, this.txtVesselName.Text.Trim().Length < 1, Strings.MsgValidationErrorVesselName);
            this.rdoLoad.Validating += (s, a) => handleValidation(s, a, !this.rdoLoad.Checked && !this.rdoDischarge.Checked, Strings.MsgValidationErrorLoadDischarge);
            this.rdoDischarge.Validating += (s, a) => handleValidation(s, a, !this.rdoLoad.Checked && !this.rdoDischarge.Checked, Strings.MsgValidationErrorLoadDischarge);
            this.txtPortOfShipment.Validating += (s, a) => handleValidation(s, a, this.txtPortOfShipment.Text.Trim().Length < 1, Strings.MsgValidationErrorPortOfShipment);
            this.txtPortOfDestination.Validating += (s, a) => handleValidation(s, a, this.txtPortOfDestination.Text.Trim().Length < 1, Strings.MsgValidationErrorPortOfDestination);
            this.cmbKindOfGoods.Validating += (s, a) => handleValidation(s, a, this.cmbKindOfGoods.SelectedNode == null || ((KindOfGoods)this.cmbKindOfGoods.SelectedNode.Tag).IsContainer, Strings.MsgValidationErrorKindOfGoods);
            this.rdoListToPort.Validating += (s, a) => handleValidation(s, a, !this.rdoListToPort.Checked && !this.rdoListToStarboard.Checked, Strings.MsgValidationErrorListToPortStarboard);
            this.rdoListToStarboard.Validating += (s, a) => handleValidation(s, a, !this.rdoListToPort.Checked && !this.rdoListToStarboard.Checked, Strings.MsgValidationErrorListToPortStarboard);
            this.txtHInclination.Validating += (s, a) => handleValidation(s, a, this.txtHInclination.Text.Trim().Length < 1 || !Decimal.TryParse(this.txtHInclination.Text.Trim(), out temp), Strings.MsgValidationErrorVesselStatus);
            this.txtForeDraft.Validating += (s, a) => handleValidation(s, a, this.txtForeDraft.Text.Trim().Length < 1 || !Decimal.TryParse(this.txtForeDraft.Text.Trim(), out temp), Strings.MsgValidationErrorForeDraft);
            this.txtAftDraft.Validating += (s, a) => handleValidation(s, a, this.txtAftDraft.Text.Trim().Length < 1 || !Decimal.TryParse(this.txtAftDraft.Text.Trim(), out temp), Strings.MsgValidationErrorAftDraft);
            this.txtVInclination.Validating += (s, a) => handleValidation(s, a, this.txtVInclination.Text.Trim().Length < 1 || !Decimal.TryParse(this.txtVInclination.Text.Trim(), out temp), Strings.MsgValidationErrorTrim);
            this.txtReceiptNo.Validating += (s, a) => handleValidation(s, a, this.txtReceiptNo.Text.Trim().Length < 1, Strings.MsgValidationErrorReceiptNo);
        }

        private void OnValidationErrorMessageChanged()
        {
            if (this.ValidationErrorMessageChanged != null)
            {
                this.ValidationErrorMessageChanged(this.ValidationErrorMessage);
            }
        }

        private void OnHInclinationChanged(decimal? value)
        {
            if (this.HInclinationChanged != null)
            {
                this.HInclinationChanged(value);
            }
        }

        private void OnVInclinationChanged(decimal? value)
        {
            if (this.VInclinationChanged != null)
            {
                this.VInclinationChanged(value);
            }
        }

        private void OnTotalOfVolumeOfPipesChanged(decimal? value)
        {
            if (this.TotalOfVolumeOfPipesChanged != null)
            {
                this.TotalOfVolumeOfPipesChanged(value);
            }
        }

        private void OnSelectedKindChanged()
        {
            if (this.loading)
            {
                return;
            }

            if (this.cmbKindOfGoods.SelectedNode != null)
            {
                var kind = this.cmbKindOfGoods.SelectedNode.Tag as KindOfGoods;
                if (kind != null && kind.IsContainer)
                {
                    this.cmbKindOfGoods.SelectedNode = this.SelectedKindOfGoodsNode;
                }
                if (kind != null && kind.BuiltIn && kind.Customized)
                {
                    this.cmbKindOfGoods.SelectedNode = this.SelectedKindOfGoodsNode;
                    using (var form = new KindsOfGoodsForm(this.ReceiptDb))
                    {
                        var result = form.ShowDialog();
                        this.LoadCustomizedKindsOfGoods();

                        if (result == DialogResult.OK)
                        {
                            this.cmbKindOfGoods.SelectedNode = this.cmbKindOfGoods.Nodes.RecursivelyFirstOrDefault(n => ((KindOfGoods)n.Tag).Equals(form.SelectedKind));
                        }
                    }
                }
            }

            if (this.cmbKindOfGoods.SelectedNode != this.SelectedKindOfGoodsNode && this.SelectedKindOfGoodsChanged != null)
            {
                this.SelectedKindOfGoodsChanged(this.cmbKindOfGoods.SelectedNode.Tag as KindOfGoods);
            }

            this.SelectedKindOfGoodsNode = this.cmbKindOfGoods.SelectedNode;
        }

        private void LoadKindsOfGoods()
        {
            if (this.cmbKindOfGoods.Nodes.Count > 0)
            {
                return;
            }

            this.cmbKindOfGoods.Nodes.Clear();
            Const.BuiltInKindsOfGoods.ForEach(k => this.AddNode(this.cmbKindOfGoods.Nodes, k));
            this.LoadCustomizedKindsOfGoods();
        }

        private void LoadCustomizedKindsOfGoods()
        {
            var containerNode = this.cmbKindOfGoods.Nodes.RecursivelyFirstOrDefault(n =>
            {
                var kind = n.Tag as KindOfGoods;
                return kind != null && kind.BuiltIn && kind.Customized && kind.IsContainer;
            });

            if (containerNode == null)
            {
                return;
            }

            containerNode.Nodes.Clear();
            this.ReceiptDb.GetAllKindsOfGoods().OrderBy(k => k.Name).Each(k => this.AddNode(containerNode.Nodes, k));
            this.AddNode(containerNode.Nodes, new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = Strings.BuiltInKindCustomizedAdd, BuiltIn = true, Customized = true });
        }

        private void AddNode(ComboTreeNodeCollection nodes, KindOfGoods kind)
        {
            var node = new ComboTreeNode(kind.Name) { Tag = kind, Expanded = true };
            nodes.Add(node);
            kind.SubKinds.ForEach(k => this.AddNode(node.Nodes, k));
        }

        private void SelectPipes(Receipt receipt)
        {
            // todo: select pipes
        }

        private void ReportDirty()
        {
            if (this.loading)
            {
                return;
            }

            this.IsDirty = true;
            if (this.Dirty != null)
            {
                this.Dirty();
            }
        }

        private void ReportLoaded()
        {
            this.IsDirty = false;
        }
    }
}
