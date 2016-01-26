using ShippingMeasure.Common;
using ShippingMeasure.Core;
using ShippingMeasure.Core.Models;
using ShippingMeasure.Db;
using ShippingMeasure.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure
{
    public partial class ReceiptForm : Form
    {
        private TankDb TankDb { get; set; }
        private StandardDensityDb StandardDensityDb { get; set; }
        private VcfDb VcfDb { get; set; }
        private ReceiptDb ReceiptDb { get; set; }

        private ReceiptType receiptType;
        private bool isDirty = false;

        public ReceiptForm()
        {
            InitializeComponent();

            this.TankDb = new TankDb();
            this.StandardDensityDb = new StandardDensityDb();
            this.VcfDb = new VcfDb();
            this.ReceiptDb = new ReceiptDb();

            this.SetupValidation();

            this.header.ReceiptDb = this.ReceiptDb;
            this.header.TankDb = this.TankDb;
            this.header.VesselName = this.TankDb.GetVessel().Name;
            this.header.HInclinationChanged += h => this.grid.HInclination = h;
            this.header.VInclinationChanged += v => this.grid.VInclination = v;
            this.header.SelectedKindOfGoodsChanged += k => this.grid.SelectedKindOfGoods = k;
            this.header.Dirty += this.ReportDirty;
            this.grid.TotalOfVolumeOfStandardChanged += v => this.summary.TotalOfVolumeOfStandard = v;
            this.grid.TotalOfVolumeChanged += v => this.summary.TotalOfVolume = v + this.header.TotalOfVolumeOfPipes;
            this.grid.TotalOfVolumeOfWaterChanged += v => this.summary.TotalOfVolumeOfWater = v;
            this.grid.TotalOfMassChanged += v => this.summary.TotalOfMass = v;
            this.grid.Dirty += this.ReportDirty;
            this.grid.VolumeByHeightChanged += v => { this.menuVolumeByHeight.Checked = v; this.menuVolumeByUllage.Checked = !v; };
            this.grid.NewNotification += (m, l) => this.ReportStatus(m, l);
            this.summary.Dirty += this.ReportDirty;

            this.menuNew.Click += (s, a) => this.NewReceipt();
            this.menuLoad.Click += (s, a) => this.LoadReceipt();
            this.menuSave.Click += (s, a) => this.SaveReceipt();
            this.menuPrint.Click += (s, a) => this.PrintReceipt();
            this.menuVolumeByHeight.Click += (s, a) => this.menuVolumeByHeight.Checked = this.grid.VolumeByHeight = true;
            this.menuVolumeByUllage.Click += (s, a) => { this.menuVolumeByUllage.Checked = true; this.grid.VolumeByHeight = false; };
            this.menuAutoCalculate.Checked = Config.AutoCalculate;
            this.menuAutoCalculate.Click += (s, a) => { Config.AutoCalculate = this.menuAutoCalculate.Checked = !this.menuAutoCalculate.Checked; this.grid.AutoCalculate = this.menuAutoCalculate.Checked; this.menuCalculate.Enabled = !this.menuAutoCalculate.Checked; };
            this.menuCalculate.Enabled = !Config.AutoCalculate;
            this.menuCalculate.Click += (s, a) => this.grid.Calculate();

            this.grid.TankDb = this.TankDb;
            this.grid.StandardDensityDb = this.StandardDensityDb;
            this.grid.VcfDb = this.VcfDb;
            this.grid.AutoCalculate = Config.AutoCalculate;

            this.txtOperatorName.TextChanged += (s, a) => this.ReportDirty();
            this.txtAgentLoading.TextChanged += (s, a) => this.ReportDirty();
            this.txtAgentDestination.TextChanged += (s, a) => this.ReportDirty();
            this.txtShipperLoading.TextChanged += (s, a) => this.ReportDirty();
            this.txtShipperDestination.TextChanged += (s, a) => this.ReportDirty();
            this.txtConsigner.TextChanged += (s, a) => this.ReportDirty();
            this.txtConsignee.TextChanged += (s, a) => this.ReportDirty();

            this.KeyPreview = true;
            this.KeyDown += (s, a) =>
            {
                if (a.Control && !a.Alt && !a.Shift && a.KeyCode == Keys.S) //[ctrl]-[s]
                {
                    this.SaveReceipt();
                }
            };

            this.SetReceiptType(ReceiptType.MassOfOil);
        }

        public void SetReceiptType(ReceiptType type)
        {
            bool isMassOfOil = type == ReceiptType.MassOfOil;
            bool isDeliveryReceiptLoading = type == ReceiptType.DeliveryReceiptLoading;
            bool isDeliveryReceiptDestination = type == ReceiptType.DeliveryReceiptDestination;

            this.Text = isDeliveryReceiptLoading ? Strings.FormTitleDeliveryReceiptLoading : (isDeliveryReceiptDestination ? Strings.FormTitleDeliveryReceiptDestination : Strings.FormTitleMassOfOil);
            this.pnlMassOfOil.Visible = isMassOfOil;
            this.pnlDeliveryReceiptLoading.Visible = isDeliveryReceiptLoading;
            this.pnlDeliverReceiptDestination.Visible = isDeliveryReceiptDestination;

            this.receiptType = type;
        }

        private void SetupValidation()
        {
            this.header.ValidationErrorMessageChanged += this.ReportValidation;
            this.grid.ValidationErrorMessageChanged += this.ReportValidation;
            this.summary.ValidationErrorMessageChanged += this.ReportValidation;
        }

        private void NewReceipt()
        {
            if (this.isDirty)
            {
                if (MessageBox.Show(this, Strings.MsgConfirmationDiscardToNew, Strings.MsgConfirmation, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return;
                }
            }

            this.header.NewReceipt();
            this.grid.NewReceipt();
            this.summary.NewReceipt();

            this.txtOperatorName.Text = String.Empty;
            this.txtAgentLoading.Text = String.Empty;
            this.txtAgentDestination.Text = String.Empty;
            this.txtShipperLoading.Text = String.Empty;
            this.txtShipperDestination.Text = String.Empty;
            this.txtConsigner.Text = String.Empty;
            this.txtConsignee.Text = String.Empty;

            this.ReportLoaded();
            this.ReportStatus(Strings.MsgStatusNotSave, TraceLevel.Info);
        }

        private void LoadReceipt()
        {
            if (this.isDirty)
            {
                if (MessageBox.Show(this, Strings.MsgConfirmationDiscardToLoad, Strings.MsgConfirmation, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return;
                }
            }

            using (var receiptsForm = new ReceiptsForm(ReceiptsForm.For.Load, this.ReceiptDb, this.receiptType))
            {
                if (receiptsForm.ShowDialog(this) == DialogResult.OK)
                {
                    this.LoadReceipt(receiptsForm.Receipt);
                    this.ReportStatus(Strings.MsgStatusReceiptLoaded, TraceLevel.Info);
                }
            }
        }

        private void LoadReceipt(Receipt receipt)
        {
            this.header.LoadReceipt(receipt);
            this.grid.LoadReceipt(receipt);
            this.summary.LoadReceipt(receipt);

            bool isMassOfOil = this.receiptType == ReceiptType.MassOfOil;
            bool isDeliveryReceiptLoading = this.receiptType == ReceiptType.DeliveryReceiptLoading;
            bool isDeliveryReceiptDestination = this.receiptType == ReceiptType.DeliveryReceiptDestination;

            this.txtOperatorName.Text = isMassOfOil ? receipt.OperaterName : String.Empty;
            this.txtAgentLoading.Text = isDeliveryReceiptLoading ? receipt.AgentName : String.Empty;
            this.txtAgentDestination.Text = isDeliveryReceiptDestination ? receipt.AgentName : String.Empty;
            this.txtShipperLoading.Text = isDeliveryReceiptLoading ? receipt.ShipperName : String.Empty;
            this.txtShipperDestination.Text = isDeliveryReceiptDestination ? receipt.ShipperName : String.Empty;
            this.txtConsigner.Text = isDeliveryReceiptLoading ? receipt.ConsignerName : String.Empty;
            this.txtConsignee.Text = isDeliveryReceiptDestination ? receipt.ConsigneeName : String.Empty;

            this.ReportLoaded();
        }

        private void GetReceiptFooter(Receipt receipt)
        {
            bool isMassOfOil = this.receiptType == ReceiptType.MassOfOil;
            bool isDeliveryReceiptLoading = this.receiptType == ReceiptType.DeliveryReceiptLoading;
            bool isDeliveryReceiptDestination = this.receiptType == ReceiptType.DeliveryReceiptDestination;

            receipt.OperaterName = isMassOfOil ? this.txtOperatorName.Text : String.Empty;
            receipt.AgentName = isDeliveryReceiptLoading ? this.txtAgentLoading.Text : (isDeliveryReceiptDestination ? this.txtAgentDestination.Text : String.Empty);
            receipt.ShipperName = isDeliveryReceiptLoading ? this.txtShipperLoading.Text : (isDeliveryReceiptDestination ? this.txtShipperDestination.Text : String.Empty);
            receipt.ConsignerName = isDeliveryReceiptLoading ? this.txtConsigner.Text : String.Empty;
            receipt.ConsigneeName = isDeliveryReceiptDestination ? this.txtConsignee.Text : String.Empty;
        }

        private void SaveReceipt()
        {
            Receipt receipt;

            try
            {
                if (this.isDirty)
                {
                    receipt = new Receipt { ReceiptType = this.receiptType };
                    this.header.GetReceiptHeader(receipt);
                    this.grid.GetReceiptDetails(receipt);
                    this.summary.GetReceiptSummary(receipt);
                    this.GetReceiptFooter(receipt);
                }
            }
            catch (ValidationException ex)
            {
                if (MessageBox.Show(this, String.Format(Strings.MsgErrorValidationFailedToSave, ex.Message), Strings.MsgError, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    this.ReportStatus(Strings.MsgStatusSaveFailed, TraceLevel.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ReportStatus(Strings.MsgStatusSaveFailed, TraceLevel.Error);
                LogHelper.Write(ex);
            }

            try
            {
                receipt = new Receipt { ReceiptType = this.receiptType };
                this.header.GetReceiptHeader(receipt, false);
                this.grid.GetReceiptDetails(receipt, false);
                this.summary.GetReceiptSummary(receipt);
                this.GetReceiptFooter(receipt);

                if (this.ReceiptDb.Exists(receipt.No))
                {
                    var existingReceipt = this.ReceiptDb.Get(receipt.No);
                    if (existingReceipt.ReceiptType != receipt.ReceiptType)
                    {
                        throw new InvalidOperationException(String.Format(Strings.MsgErrorReceiptNoUsedByOtherReceiptType, receipt.No, existingReceipt.ReceiptType.ToLocString() ));
                    }
                    else if (this.header.IsNewReceipt)
                    {
                        if (MessageBox.Show(this, String.Format(Strings.MsgWarnExistingReceipt, receipt.No), Strings.MsgWarn, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                        {
                            this.ReportStatus(Strings.MsgStatusSaveFailed, TraceLevel.Error);
                            return;
                        }
                    }
                }

                this.ReceiptDb.Save(receipt);
                this.ReportSaved();
            }
            catch (Exception ex)
            {
                LogHelper.Write(ex);
                this.ReportStatus(Strings.MsgStatusSaveFailed, TraceLevel.Error);
                MessageBox.Show(this, String.Format(Strings.MsgErrorSaveFailedWithError, ex.Message), Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintReceipt()
        {
            var receipt = new Receipt { ReceiptType = this.receiptType };

            try
            {
                this.header.GetReceiptHeader(receipt, false);
                this.grid.GetReceiptDetails(receipt, false);
                this.summary.GetReceiptSummary(receipt);
                this.GetReceiptFooter(receipt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(Strings.MsgErrorPrintFailedWithError, ex.Message), Strings.MsgError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            new ReceiptReportViewerForm(ReceiptReportModel.ConvertFrom(receipt), this.receiptType).Show();
        }

        private void ReportDirty()
        {
            this.isDirty = true;
            this.menuSave.Enabled = true;
            ReportStatus(Strings.MsgStatusChangedAndNotSaved, TraceLevel.Warning);
        }

        private void ReportLoaded()
        {
            this.isDirty = false;
            this.menuSave.Enabled = false;
        }

        private void ReportSaved()
        {
            this.header.ReportSaved();
            this.grid.ReportSaved();
            this.summary.ReportSaved();
            this.isDirty = false;
            this.menuSave.Enabled = false;
            this.ReportStatus(Strings.MsgStatusSaveSucceeded, TraceLevel.Off);
        }

        private void ReportValidation(string errorMessage)
        {
            if (errorMessage != null)
            {
                this.ReportStatus(errorMessage, TraceLevel.Error);
            }
            else
            {
                this.ReportStatus(String.Empty, TraceLevel.Info);
            }
        }

        private void ReportStatus(string message, TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    this.Invoke(new Action(() => this.lblStatus.ForeColor = Color.Red));
                    break;
                case TraceLevel.Warning:
                    this.Invoke(new Action(() => this.lblStatus.ForeColor = Color.DarkOrange));
                    break;
                case TraceLevel.Off:
                    this.Invoke(new Action(() => this.lblStatus.ForeColor = Color.Green));
                    break;
                default:
                    this.Invoke(new Action(() => this.lblStatus.ForeColor = SystemColors.ControlText));
                    break;
            }
            this.Invoke(new Action(() => this.lblStatus.Text = message));
        }
    }
}
