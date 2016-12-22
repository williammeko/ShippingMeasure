using ShippingMeasure.Common;
using ShippingMeasure.Core;
using ShippingMeasure.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ShippingMeasure.Core.Models;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Globalization;

namespace ShippingMeasure.Controls
{
    public class MeasureGrid : DataGridView
    {
        public TankDb TankDb { get; set; }
        public StandardDensityDb StandardDensityDb { get; set; }
        public VcfDb VcfDb { get; set; }
        public bool IsDirty { get; private set; }
        public bool AutoCalculate { get; set; }
        public string ValidationErrorMessage
        {
            get { return this.validationErrorMessage; }
            set
            {
                this.validationErrorMessage = value;
                this.OnValidationErrorMessageChanged();
            }
        }
        public bool VolumeByHeight
        {
            get { return this.volumeByHeight; }
            set
            {
                if (this.volumeByHeight != value)
                {
                    this.volumeByHeight = value;
                    this.OnVolumeByHeightChanged();
                }
            }
        }

        public decimal? HInclination
        {
            get { return this.hInclination; }
            set
            {
                var oriValue = this.hInclination;
                this.hInclination = value;
                if (value != oriValue)
                {
                    this.OnHInclinationChanged(value);
                }
            }
        }

        public decimal? VInclination
        {
            get { return this.vInclination; }
            set
            {
                var oriValue = this.vInclination;
                this.vInclination = value;
                if (value != oriValue)
                {
                    this.OnVInclinationChanged(value);
                }
            }
        }

        public KindOfGoods SelectedKindOfGoods
        {
            set
            {
                var oriValue = this.selectedKindOfGoods;
                this.selectedKindOfGoods = value;
                if (value != oriValue)
                {
                    this.needInputDensity = Const.NonPetroleumProducts.Any(k => value.UId.Equals(k.UId, StringComparison.OrdinalIgnoreCase));
                    this.UpdateCellsEnabled();
                }
            }
        }

        public event Action<decimal?> TotalOfVolumeOfStandardChanged;
        public event Action<decimal?> TotalOfVolumeChanged;
        public event Action<decimal?> TotalOfVolumeOfWaterChanged;
        public event Action<decimal?> TotalOfMassChanged;
        public event Action Dirty;
        public event Action<string> ValidationErrorMessageChanged;
        public event Action<bool> VolumeByHeightChanged;
        public event Action<string, TraceLevel> NewNotification;

        private DataGridViewTextBoxColumn colTankName = new DataGridViewTextBoxColumn { Name = "TankName", HeaderText = Strings.HeaderTankName, Width = 110 };
        private DataGridViewTextBoxColumn colHeight = new DataGridViewTextBoxColumn { Name = "Height", HeaderText = Strings.HeaderHeight, Width = 90 };
        private DataGridViewTextBoxColumn colTemperatureOfTank = new DataGridViewTextBoxColumn { Name = "TemperatureOfTank", HeaderText = Strings.HeaderTemperatureOfTank, Width = 90 };
        private DataGridViewTextBoxColumn colHeightOfWater = new DataGridViewTextBoxColumn { Name = "HeightOfWater", HeaderText = Strings.HeaderHeightOfWater, Width = 90 };
        private DataGridViewTextBoxColumn colTemperatureMeasured = new DataGridViewTextBoxColumn { Name = "TemperatureMeasured", HeaderText = Strings.HeaderTemperatureMeasured, Width = 90 };
        private DataGridViewTextBoxColumn colDensityMeasured = new DataGridViewTextBoxColumn { Name = "DensityMeasured", HeaderText = Strings.HeaderDensityMeasured, Width = 90 };
        private DataGridViewTextBoxColumn colDensityOfStandard = new DataGridViewTextBoxColumn { Name = "DensityOfStandard", HeaderText = Strings.HeaderDensityOfStandard, Width = 90 };
        private DataGridViewTextBoxColumn colVcf20 = new DataGridViewTextBoxColumn { Name = "Vcf20", HeaderText = Strings.HeaderVcf20, Width = 90 };
        private DataGridViewTextBoxColumn colVolume = new DataGridViewTextBoxColumn { Name = "Volume", HeaderText = Strings.HeaderVolume, Width = 90 };
        private DataGridViewTextBoxColumn colVolumeOfWater = new DataGridViewTextBoxColumn { Name = "VolumeOfWater", HeaderText = Strings.HeaderVolumeOfWater, Width = 90 };
        private DataGridViewTextBoxColumn colVolumeOfStandard = new DataGridViewTextBoxColumn { Name = "VolumeOfStandard", HeaderText = Strings.HeaderVolumeOfStandard, Width = 90 };
        private DataGridViewTextBoxColumn colMass = new DataGridViewTextBoxColumn { Name = "Mass", HeaderText = Strings.HeaderMass, Width = 90 };

        private decimal? hInclination;
        private decimal? vInclination;
        private KindOfGoods selectedKindOfGoods;
        private bool needInputDensity = true;
        private bool ignoreCellValueChangedEvent = false;
        private string validationErrorMessage = String.Empty;
        private List<KeyValuePair<DataGridViewCell, string>> validationErrorMessages = new List<KeyValuePair<DataGridViewCell, string>>();
        private Dictionary<int, Action<DataGridViewCell, DataGridViewCellValidatingEventArgs>> colValidationFuncs;
        private bool volumeByHeight = true;
        private ToolStripMenuItem menuVolumeByHeight;
        private ToolStripMenuItem menuVolumeByUllage;
        private BackgroundWorker calculationWorker = null;
        private Dictionary<string, object> propertyValues = new Dictionary<string, object>() { { "Enabled", true } };
        private CultureInfo currentCulture = Thread.CurrentThread.CurrentUICulture;

        public void LoadReceipt(Receipt receipt)
        {
            this.ignoreCellValueChangedEvent = true;

            try
            {
                this.Rows.Clear();

                receipt.ReceiptTankDetails.ForEach(r =>
                {
                    this.VolumeByHeight = r.VolumeByHeight;

                    var row = new DataGridViewRow();
                    var rowIndex = this.Rows.Add(row);
                    this.Rows[rowIndex].Cells[colTankName.Index].Value = r.TankName;
                    this.Rows[rowIndex].Cells[colHeight.Index].Value = r.Height;
                    this.Rows[rowIndex].Cells[colTemperatureOfTank.Index].Value = r.TemperatureOfTank;
                    this.Rows[rowIndex].Cells[colHeightOfWater.Index].Value = r.HeightOfWater;
                    this.Rows[rowIndex].Cells[colTemperatureMeasured.Index].Value = r.TemperatureMeasured;
                    this.Rows[rowIndex].Cells[colDensityMeasured.Index].Value = r.DensityMeasured;
                    this.Rows[rowIndex].Cells[colDensityOfStandard.Index].Value = r.DensityOfStandard;
                    this.Rows[rowIndex].Cells[colVcf20.Index].Value = r.Vcf20;
                    this.Rows[rowIndex].Cells[colVolume.Index].Value = r.Volume;
                    this.Rows[rowIndex].Cells[colVolumeOfWater.Index].Value = r.VolumeOfWater;
                    this.Rows[rowIndex].Cells[colVolumeOfStandard.Index].Value = r.VolumeOfStandard;
                    this.Rows[rowIndex].Cells[colMass.Index].Value = r.Mass;
                });
            }
            finally
            {
                this.ignoreCellValueChangedEvent = false;
            }

            this.ReportLoaded();
        }

        public void GetReceiptDetails(Receipt receipt, bool validate = true)
        {
            var details = new List<ReceiptTankDetail>();
            this.Rows.Cast<DataGridViewRow>().Each(r =>
            {
                var tankName = r.Cells[colTankName.Index].Value.ToString();

                if (validate)
                {
                    var error = new[]
                    {
                        r.Cells[colHeight.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorHeight, tankName)),
                        r.Cells[colTemperatureOfTank.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorTemperatureOfTank, tankName)),
                        r.Cells[colHeightOfWater.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorHeightOfWater, tankName)),
                        r.Cells[colTemperatureMeasured.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorTemperatureMeasured, tankName)),
                        r.Cells[colDensityMeasured.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorDensityMeasured, tankName)),
                        r.Cells[colDensityOfStandard.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorDensityOfStandard, tankName)),
                        r.Cells[colVcf20.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorVcf20, tankName)),
                        r.Cells[colVolume.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorVolume, tankName)),
                        r.Cells[colVolumeOfWater.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorVolumeOfWater, tankName)),
                        r.Cells[colVolumeOfStandard.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorVolumeOfStandard, tankName)),
                        r.Cells[colMass.Index].Value.ValidateDecimal(String.Format(Strings.MsgValidationErrorMass, tankName)),
                    }
                    .Where(e => e != null)
                    .Aggregate(new StringBuilder(), (sb, e) => sb.AppendLine(e)).ToString();

                    if (error.Length > 0)
                    {
                        throw new ValidationException(error);
                    }
                }

                details.Add(new ReceiptTankDetail
                {
                    TankName = tankName,
                    VolumeByHeight = this.VolumeByHeight,
                    Height = r.Cells[colHeight.Index].Value.TryToNullableDecimal(),
                    TemperatureOfTank = r.Cells[colTemperatureOfTank.Index].Value.TryToNullableDecimal(),
                    HeightOfWater = r.Cells[colHeightOfWater.Index].Value.TryToNullableDecimal(),
                    TemperatureMeasured = r.Cells[colTemperatureMeasured.Index].Value.TryToNullableDecimal(),
                    DensityMeasured = r.Cells[colDensityMeasured.Index].Value.TryToNullableDecimal(),
                    DensityOfStandard = r.Cells[colDensityOfStandard.Index].Value.TryToNullableDecimal(),
                    Vcf20 = r.Cells[colVcf20.Index].Value.TryToNullableDecimal(),
                    Volume = r.Cells[colVolume.Index].Value.TryToNullableDecimal(),
                    VolumeOfWater = r.Cells[colVolumeOfWater.Index].Value.TryToNullableDecimal(),
                    VolumeOfStandard = r.Cells[colVolumeOfStandard.Index].Value.TryToNullableDecimal(),
                    Mass = r.Cells[colMass.Index].Value.TryToNullableDecimal()
                });
            });

            receipt.ReceiptTankDetails.AddRange(details);
        }

        public void NewReceipt()
        {
            this.LoadTanks();
            this.VolumeByHeight = true;
            this.ReportLoaded();
        }

        public void ReportSaved()
        {
            this.IsDirty = false;
        }

        public void Calculate()
        {
            if (this.calculationWorker == null)
            {
                this.FindForm().FormClosing += (s, a) =>
                {
                    if (this.calculationWorker.IsBusy)
                    {
                        a.Cancel = true;
                        return;
                    }
                    this.calculationWorker.Dispose();
                };

                this.calculationWorker = new BackgroundWorker();

                this.calculationWorker.DoWork += (s, a) =>
                {
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = this.currentCulture;

                    for (int i = 0, rowCount = this.Rows.Count; i < rowCount; i++)
                    {
                        if (!this.IsHeightFulfilled(i))
                        {
                            this.EmptyRow(i);
                            continue;
                        }

                        var tankName = this.Rows[i].Cells[this.colTankName.Index].Value.ToString();

                        if (!this.needInputDensity)
                        {
                            this.OnNewNotification(String.Format(Strings.MsgCalcInfoDensityOfStandard, tankName), TraceLevel.Info);
                            this.TryCalculateDensityOfStandard(i);
                            this.OnNewNotification(String.Format(Strings.MsgCalcInfoVcf, tankName), TraceLevel.Info);
                            this.TryCalculateVcf20(i);
                        }
                        this.OnNewNotification(String.Format(Strings.MsgCalcInfoVolumeOfWater, tankName), TraceLevel.Info);
                        this.TryCalculateVolumeOfWater(i);
                        this.OnNewNotification(String.Format(Strings.MsgCalcInfoVolume, tankName), TraceLevel.Info);
                        this.TryCalculateVolume(i);
                        this.OnNewNotification(String.Format(Strings.MsgCalcInfoVolumeOfStandard, tankName), TraceLevel.Info);
                        this.TryCalculateVolumeOfStandard(i);
                        this.OnNewNotification(String.Format(Strings.MsgCalcInfoMass, tankName), TraceLevel.Info);
                        this.TryCalculateMass(i);
                    }

                    this.OnNewNotification(Strings.MsgCalcInfoTotalOfVolume, TraceLevel.Info);
                    this.OnTotalOfVolumeChanged();
                    this.OnNewNotification(Strings.MsgCalcInfoTotalOfVolumeOfStandard, TraceLevel.Info);
                    this.OnTotalOfVolumeOfStandardChanged();
                    this.OnNewNotification(Strings.MsgCalcInfoTotalOfVolumeOfWater, TraceLevel.Info);
                    this.OnTotalOfVolumeOfWaterChanged();
                    this.OnNewNotification(Strings.MsgCalcInfoTotalOfMass, TraceLevel.Info);
                    this.OnTotalOfMassChanged();
                };

                this.calculationWorker.RunWorkerCompleted += (s, a) =>
                {
                    if (a.Error != null)
                    {
                        this.OnNewNotification(String.Format(Strings.MsgCalcErrorFinish, a.Error.Message), TraceLevel.Error);
                    }
                    else
                    {
                        this.OnNewNotification(Strings.MsgCalcFinish, TraceLevel.Off);
                    }

                    this.Enabled = (bool)this.propertyValues["Enabled"];
                };
            }

            this.propertyValues["Enabled"] = this.Enabled;
            this.Enabled = false;
            this.calculationWorker.RunWorkerAsync();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.AllowUserToAddRows = false;
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.TopCenter;

            if (!this.DesignMode)
            {
                this.Columns.Clear();
                this.EnsureColumn(this.colTankName);
                this.EnsureColumn(this.colHeight);
                this.EnsureColumn(this.colTemperatureOfTank);
                this.EnsureColumn(this.colHeightOfWater);
                this.EnsureColumn(this.colTemperatureMeasured);
                this.EnsureColumn(this.colDensityMeasured);
                this.EnsureColumn(this.colDensityOfStandard);
                this.EnsureColumn(this.colVcf20);
                this.EnsureColumn(this.colVolume);
                this.EnsureColumn(this.colVolumeOfWater);
                this.EnsureColumn(this.colVolumeOfStandard);
                this.EnsureColumn(this.colMass);
                this.InitCellsEnabled();

                this.colHeight.ValueType = typeof(decimal);
                this.colTemperatureOfTank.ValueType = typeof(decimal);
                this.colHeightOfWater.ValueType = typeof(decimal);
                this.colTemperatureMeasured.ValueType = typeof(decimal);
                this.colDensityMeasured.ValueType = typeof(decimal);
                this.colDensityOfStandard.ValueType = typeof(decimal);
                this.colVcf20.ValueType = typeof(decimal);
                this.colVolume.ValueType = typeof(decimal);
                this.colVolumeOfWater.ValueType = typeof(decimal);
                this.colVolumeOfStandard.ValueType = typeof(decimal);
                this.colMass.ValueType = typeof(decimal);
                this.colHeight.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colTemperatureOfTank.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colHeightOfWater.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colTemperatureMeasured.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colDensityMeasured.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colDensityOfStandard.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F1" });
                this.colVcf20.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F4" });
                this.colVolume.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colVolumeOfWater.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colVolumeOfStandard.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });
                this.colMass.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3" });

                this.menuVolumeByHeight = new ToolStripMenuItem(Strings.MenuVolumeByHeight, null, (s, a) => this.menuVolumeByHeight.Checked = this.VolumeByHeight = true) { CheckOnClick = true, Checked = true };
                this.menuVolumeByUllage = new ToolStripMenuItem(Strings.MenuVolumeByUllage, null, (s, a) => { this.menuVolumeByUllage.Checked = true; this.VolumeByHeight = false; }) { CheckOnClick = true, Checked = false };
                this.colHeight.ContextMenuStrip = new ContextMenuStrip();
                this.colHeight.ContextMenuStrip.Items.AddRange(new[] { this.menuVolumeByHeight, this.menuVolumeByUllage });

                this.LoadTanks();
                this.SetupValidation();
            }
        }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            base.OnCellValueChanged(e);

            if (this.ignoreCellValueChangedEvent || e.RowIndex < 0)
            {
                return;
            }

            this.ReportDirty();

            if (this.AutoCalculate)
            {
                if (this.IsHeightFulfilled(e.RowIndex))
                {
                    this.TryCalculate(e.RowIndex, e.ColumnIndex);
                }
                else
                {
                    if (e.ColumnIndex == this.colHeight.Index)
                    {
                        this.EmptyRow(e.RowIndex);
                    }
                }
            }
        }

        private void TryCalculate(int rowIndex, int columnIndex)
        {
            var effectedColumns = this.GetEffectedColumns(columnIndex);

            if ((effectedColumns & EffectedColumns.DensityOfStandard) == EffectedColumns.DensityOfStandard && !this.needInputDensity)
            {
                this.TryCalculateDensityOfStandard(rowIndex);
            }
            if ((effectedColumns & EffectedColumns.Vcf20) == EffectedColumns.Vcf20 && !this.needInputDensity)
            {
                this.TryCalculateVcf20(rowIndex);
            }
            if ((effectedColumns & EffectedColumns.Volume) == EffectedColumns.Volume)
            {
                this.TryCalculateVolume(rowIndex);
            }
            if ((effectedColumns & EffectedColumns.VolumeOfWater) == EffectedColumns.VolumeOfWater)
            {
                this.TryCalculateVolumeOfWater(rowIndex);
            }
            if ((effectedColumns & EffectedColumns.VolumeOfStandard) == EffectedColumns.VolumeOfStandard)
            {
                this.TryCalculateVolumeOfStandard(rowIndex);
            }
            if ((effectedColumns & EffectedColumns.Mass) == EffectedColumns.Mass)
            {
                this.TryCalculateMass(rowIndex);
            }
            if ((effectedColumns & EffectedColumns.TotalOfVolume) == EffectedColumns.TotalOfVolume)
            {
                this.OnTotalOfVolumeChanged();
            }
            if ((effectedColumns & EffectedColumns.TotalOfVolumeOfStandard) == EffectedColumns.TotalOfVolumeOfStandard)
            {
                this.OnTotalOfVolumeOfStandardChanged();
            }
            if ((effectedColumns & EffectedColumns.TotalOfVolumeOfWater) == EffectedColumns.TotalOfVolumeOfWater)
            {
                this.OnTotalOfVolumeOfWaterChanged();
            }
            if ((effectedColumns & EffectedColumns.TotalOfMass) == EffectedColumns.TotalOfMass)
            {
                this.OnTotalOfMassChanged();
            }
        }

        private void SetupValidation()
        {
            this.CellValidating += (s, a) =>
            {
                var row = this.Rows[a.RowIndex];
                if (row.IsNewRow)
                {
                    return;
                }

                if (this.colValidationFuncs.Keys.Contains(a.ColumnIndex))
                {
                    this.colValidationFuncs[a.ColumnIndex](row.Cells[a.ColumnIndex], a);
                }
            };

            this.DataError += (s, a) => { };

            var handleValidation = new Action<DataGridViewCell, DataGridViewCellValidatingEventArgs, bool, string>((cell, args, failed, errorMessage) =>
            {
                //args.Cancel = failed;
                this.validationErrorMessages.RemoveAll(p => p.Key == cell);

                if (failed)
                {
                    var pair = this.validationErrorMessages.FirstOrDefault(p => p.Key == cell);
                    this.validationErrorMessages.Add(new KeyValuePair<DataGridViewCell, string>(cell, errorMessage));
                    cell.ErrorText = errorMessage;
                    this.ValidationErrorMessage = errorMessage;
                }
                else
                {
                    cell.ErrorText = null;
                    string error = null;
                    if (this.validationErrorMessages.Count() > 0)
                    {
                        error = this.validationErrorMessages[this.validationErrorMessages.Count() - 1].Value;
                    }
                    this.ValidationErrorMessage = error;
                }
            });

            decimal temp; // used to try parse decimal values.

            this.colValidationFuncs = new Dictionary<int, Action<DataGridViewCell, DataGridViewCellValidatingEventArgs>>
            {
                { this.colHeight.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorHeight) },
                { this.colTemperatureOfTank.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorTemperatureOfTank) },
                { this.colHeightOfWater.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorHeightOfWater) },
                { this.colTemperatureMeasured.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorTemperatureMeasured) },
                { this.colDensityMeasured.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorDensityMeasured) },
                { this.colDensityOfStandard.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorDensityOfStandard) },
                { this.colVcf20.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorVcf) },
                { this.colVolume.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorVolume) },
                { this.colVolumeOfWater.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorVolumeOfWater) },
                { this.colVolumeOfStandard.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorVolumeOfStandard) },
                { this.colMass.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorMass) }
            };
        }

        private void OnValidationErrorMessageChanged()
        {
            if (this.ValidationErrorMessageChanged != null)
            {
                this.ValidationErrorMessageChanged(this.ValidationErrorMessage);
            }
        }

        private void OnNewNotification(string message, TraceLevel level)
        {
            if (this.NewNotification != null)
            {
                this.Invoke(new Action(() => this.NewNotification(message, level)));
            }
        }

        private void OnVolumeByHeightChanged()
        {
            this.colHeight.HeaderText = this.VolumeByHeight ? Strings.HeaderHeight : Strings.HeaderUllage;
            this.menuVolumeByHeight.Checked = this.VolumeByHeight;
            this.menuVolumeByUllage.Checked = !this.VolumeByHeight;
            this.ReportDirty();

            if (this.VolumeByHeightChanged != null)
            {
                this.VolumeByHeightChanged(this.VolumeByHeight);
            }
        }

        private EffectedColumns GetEffectedColumns(int columnIndex)
        {
            var effected = EffectedColumns.None;

            if (columnIndex == this.colHeight.Index)
            {
                effected |= EffectedColumns.Volume;
            }
            else if (columnIndex == this.colTemperatureOfTank.Index)
            {
                effected |= EffectedColumns.Volume;
                effected |= EffectedColumns.VolumeOfWater;
            }
            else if (columnIndex == this.colHeightOfWater.Index)
            {
                effected |= EffectedColumns.VolumeOfWater;
            }
            else if (columnIndex == this.colTemperatureMeasured.Index)
            {
                effected |= EffectedColumns.DensityOfStandard;
                effected |= EffectedColumns.Vcf20;
            }
            else if (columnIndex == this.colDensityMeasured.Index)
            {
                effected |= EffectedColumns.DensityOfStandard;
            }
            else if (columnIndex == this.colDensityOfStandard.Index)
            {
                effected |= EffectedColumns.Vcf20;
                effected |= EffectedColumns.Mass;
            }
            else if (columnIndex == this.colVcf20.Index)
            {
                effected |= EffectedColumns.VolumeOfStandard;
            }
            else if (columnIndex == this.colVolume.Index)
            {
                effected |= EffectedColumns.VolumeOfStandard;
                effected |= EffectedColumns.TotalOfVolume;
            }
            else if (columnIndex == this.colVolumeOfWater.Index)
            {
                effected |= EffectedColumns.Volume;
                effected |= EffectedColumns.TotalOfVolumeOfWater;
            }
            else if (columnIndex == this.colVolumeOfStandard.Index)
            {
                effected |= EffectedColumns.Mass;
                effected |= EffectedColumns.TotalOfVolumeOfStandard;
            }
            else if (columnIndex == this.colMass.Index)
            {
                effected |= EffectedColumns.TotalOfMass;
            }

            return effected;
        }

        private void TryCalculateDensityOfStandard(int rowIndex)
        {
            var row = this.Rows[rowIndex];

            try
            {
                var temperatureMeasured = row.Cells[this.colTemperatureMeasured.Index].Value.TryToDecimal();
                var densityMeasured = row.Cells[this.colDensityMeasured.Index].Value.TryToDecimal();
                row.Cells[this.colDensityOfStandard.Index].Value = this.StandardDensityDb.GetAll().GetValue(temperatureMeasured, densityMeasured);
            }
            catch (Exception ex)
            {
                LogHelper.WriteWarning(String.Format("Exception thrown while calculating value.\r\nException: {0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()));
                row.Cells[this.colDensityOfStandard.Index].Value = null;
            }
        }

        private void TryCalculateVcf20(int rowIndex)
        {
            var row = this.Rows[rowIndex];

            try
            {
                var densityOfStandard = row.Cells[this.colDensityOfStandard.Index].Value.TryToDecimal();
                var temperatureMeasured = row.Cells[this.colTemperatureMeasured.Index].Value.TryToDecimal();
                row.Cells[this.colVcf20.Index].Value = this.VcfDb.GetAll().GetValue(temperatureMeasured, densityOfStandard);
            }
            catch (Exception ex)
            {
                LogHelper.WriteWarning(String.Format("Exception thrown while calculating value.\r\nException: {0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()));
                row.Cells[this.colVcf20.Index].Value = null;
            }
        }

        private void TryCalculateVolume(int rowIndex)
        {
            var row = this.Rows[rowIndex];

            if (!this.HInclination.HasValue || !this.VInclination.HasValue)
            {
                row.Cells[this.colVolume.Index].Value = null;
                return;
            }

            try
            {
                var tankName = row.Cells[this.colTankName.Index].Value.ToString();
                var height = this.VolumeByHeight ? row.Cells[this.colHeight.Index].Value.TryToDecimal() : this.TankDb.GetAllTanks().First(t => t.Name.Equals(tankName)).Height - row.Cells[this.colHeight.Index].Value.TryToDecimal();
                var tempOfTank = row.Cells[this.colTemperatureOfTank.Index].Value.TryToDecimal();
                var volumeOfWater = row.Cells[this.colVolumeOfWater.Index].Value.TryToDecimal();






                // todo ...
                var hCorrection = this.TankDb
                    .GetAllListingHeightCorrectionItems()
                    .GetValue(tankName, height, this.HInclination.Value)
                    .MathRound();
                var vCorrection = this.TankDb
                    .GetAllTrimmingHeightCorrectionItems()
                    .GetValue(tankName, height, this.VInclination.Value)
                    .MathRound();
                var heightQueried = height - hCorrection - vCorrection;
                var volumeQueried = this.TankDb.GetAllVolumeItems().GetValue(tankName, heightQueried);



                //var volumeQueried = this.TankDb.GetAllOilVolumeItems().GetValue(tankName, this.HInclination.Value, this.VInclination.Value, height);

                //decimal a = 0.000012m;
                //decimal volumeMeasured = volumeQueried * (1 + 3 * a * (tempOfTank - 20));

                //row.Cells[this.colVolume.Index].Value = volumeMeasured - volumeOfWater;
            }
            catch (Exception ex)
            {
                LogHelper.WriteWarning(String.Format("Exception thrown while calculating value.\r\nException: {0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()));
                row.Cells[this.colVolume.Index].Value = null;
            }
        }

        private void TryCalculateVolumeOfWater(int rowIndex)
        {
            var row = this.Rows[rowIndex];

            if (!this.HInclination.HasValue || !this.VInclination.HasValue)
            {
                row.Cells[this.colVolumeOfWater.Index].Value = null;
                return;
            }

            try
            {
                var heightOfWater = row.Cells[this.colHeightOfWater.Index].Value.TryToDecimal();

                if (heightOfWater == 0)
                {
                    row.Cells[this.colVolumeOfWater.Index].Value = 0m;
                    return;
                }

                var tankName = row.Cells[this.colTankName.Index].Value.ToString();
                var tempOfTank = row.Cells[this.colTemperatureOfTank.Index].Value.TryToDecimal();
                var volumeQueried = this.TankDb.GetAllOilVolumeItems().GetValue(tankName, this.HInclination.Value, this.VInclination.Value, heightOfWater);

                decimal a = 0.000012m;
                decimal volumeMeasured = volumeQueried * (1 + 3 * a * (tempOfTank - 20));

                row.Cells[this.colVolumeOfWater.Index].Value = volumeMeasured;
            }
            catch (Exception ex)
            {
                LogHelper.WriteWarning(String.Format("Exception thrown while calculating value.\r\nException: {0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()));
                row.Cells[this.colVolumeOfWater.Index].Value = null;
            }
        }

        private void TryCalculateVolumeOfStandard(int rowIndex)
        {
            var row = this.Rows[rowIndex];

            try
            {
                var volume = row.Cells[this.colVolume.Index].Value.TryToDecimal();
                var vcf20 = row.Cells[this.colVcf20.Index].Value.TryToDecimal();

                row.Cells[this.colVolumeOfStandard.Index].Value = volume * vcf20;
            }
            catch (Exception ex)
            {
                LogHelper.WriteWarning(String.Format("Exception thrown while calculating value.\r\nException: {0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()));
                row.Cells[this.colVolumeOfStandard.Index].Value = null;
            }
        }

        private void TryCalculateMass(int rowIndex)
        {
            var row = this.Rows[rowIndex];

            try
            {
                var volumeOfStandard = row.Cells[this.colVolumeOfStandard.Index].Value.TryToDecimal();
                var densityOfStandard = row.Cells[this.colDensityOfStandard.Index].Value.TryToDecimal();

                row.Cells[this.colMass.Index].Value = volumeOfStandard * (densityOfStandard - 1.1m) / 1000m;
            }
            catch (Exception ex)
            {
                LogHelper.WriteWarning(String.Format("Exception thrown while calculating value.\r\nException: {0}\r\nDetails:\r\n{1}", ex.Message, ex.ToString()));
                row.Cells[this.colMass.Index].Value = null;
            }
        }

        private void OnTotalOfVolumeOfStandardChanged()
        {
            if (this.TotalOfVolumeOfStandardChanged != null)
            {
                var total = this.TryGetTotal(this.colVolumeOfStandard.Index);
                this.Invoke(new Action(() => this.TotalOfVolumeOfStandardChanged(total)));
            }
        }

        private void OnTotalOfVolumeChanged()
        {
            if (this.TotalOfVolumeChanged != null)
            {
                var total = this.TryGetTotal(this.colVolume.Index);
                this.Invoke(new Action(() => this.TotalOfVolumeChanged(total)));
            }
        }

        private void OnTotalOfVolumeOfWaterChanged()
        {
            if (this.TotalOfVolumeOfWaterChanged != null)
            {
                var total = this.TryGetTotal(this.colVolumeOfWater.Index);
                this.Invoke(new Action(() => this.TotalOfVolumeOfWaterChanged(total)));
            }
        }

        private void OnTotalOfMassChanged()
        {
            if (this.TotalOfMassChanged != null)
            {
                var total = this.TryGetTotal(this.colMass.Index);
                this.Invoke(new Action(() => this.TotalOfMassChanged(total)));
            }
        }

        private void OnHInclinationChanged(decimal? value)
        {
            this.Rows.Cast<DataGridViewRow>().Each(r =>
            {
                if (this.AutoCalculate && this.IsHeightFulfilled(r.Index))
                {
                    // Volume column should be updated after VolumeOfWater column updated
                    this.TryCalculateVolumeOfWater(r.Index);
                }
            });
        }

        private void OnVInclinationChanged(decimal? value)
        {
            this.Rows.Cast<DataGridViewRow>().Each(r =>
            {
                if (this.AutoCalculate && this.IsHeightFulfilled(r.Index))
                {
                    // Volume column should be updated after VolumeOfWater column updated
                    this.TryCalculateVolumeOfWater(r.Index);
                }
            });
        }

        private void OnVolumeOfPipesChanged(decimal? value)
        {
            this.Rows.Cast<DataGridViewRow>().Each(r =>
            {
                if (this.AutoCalculate && this.IsHeightFulfilled(r.Index))
                {
                    this.TryCalculateVolume(r.Index);
                }
            });
        }

        private void InitCellsEnabled()
        {
            this.colTankName.SetCellsEnabled(false);
            this.colHeight.SetCellsEnabled(true);
            this.colTemperatureOfTank.SetCellsEnabled(true);
            this.colHeightOfWater.SetCellsEnabled(true);
            this.colTemperatureMeasured.SetCellsEnabled(true);
            this.colDensityMeasured.SetCellsEnabled(true);
            this.colDensityOfStandard.SetCellsEnabled(this.needInputDensity);
            this.colVcf20.SetCellsEnabled(this.needInputDensity);
            this.colVolume.SetCellsEnabled(false);
            this.colVolumeOfWater.SetCellsEnabled(false);
            this.colVolumeOfStandard.SetCellsEnabled(false);
            this.colMass.SetCellsEnabled(false);
        }

        private void UpdateCellsEnabled()
        {
            this.colDensityOfStandard.SetCellsEnabled(this.needInputDensity);
            this.colVcf20.SetCellsEnabled(this.needInputDensity);
        }

        private bool IsHeightFulfilled(int rowIndex)
        {
            return (this.Rows[rowIndex].Cells[this.colHeight.Index].Value.TryToNullableDecimal() != null);
        }

        private void EmptyRow(int rowIndex)
        {
            var cells = this.Rows[rowIndex].Cells;

            var originalIgnoreCellValueChangedEvent = this.ignoreCellValueChangedEvent;
            this.ignoreCellValueChangedEvent = true;

            cells[colHeight.Index].Value = null;
            cells[colTemperatureOfTank.Index].Value = null;
            cells[colHeightOfWater.Index].Value = null;
            cells[colTemperatureMeasured.Index].Value = null;
            cells[colDensityMeasured.Index].Value = null;
            cells[colDensityOfStandard.Index].Value = null;
            cells[colVcf20.Index].Value = null;
            cells[colVolume.Index].Value = null;
            cells[colVolumeOfWater.Index].Value = null;
            cells[colVolumeOfStandard.Index].Value = null;
            cells[colMass.Index].Value = null;

            this.ignoreCellValueChangedEvent = originalIgnoreCellValueChangedEvent;
        }

        private decimal? TryGetTotal(int columnIndex)
        {
            decimal total = 0m;

            this.Rows.Cast<DataGridViewRow>().Each(r =>
            {
                var value = r.Cells[columnIndex].Value;
                if (value != null)
                {
                    try { total += value.TryToDecimal(); }
                    catch { }
                }
            });

            return total;
        }

        private void LoadTanks()
        {
            this.ignoreCellValueChangedEvent = true;

            try
            {
                this.Rows.Clear();

                TankDb.GetAllTanks().Each(t =>
                {
                    var row = new DataGridViewRow();
                    var rowIndex = this.Rows.Add(row);
                    this.Rows[rowIndex].Cells[colTankName.Index].Value = t.Name;
                });
            }
            finally
            {
                this.ignoreCellValueChangedEvent = false;
            }
        }

        private void EnsureColumn(DataGridViewColumn column)
        {
            if (!this.Columns.Contains(column.Name))
            {
                this.Columns.Add(column);
            }
        }

        private void ReportDirty()
        {
            if (this.ignoreCellValueChangedEvent)
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

        private enum EffectedColumns
        {
            None = 0,
            DensityOfStandard = 1,
            Vcf20 = 2,
            Volume = 4,
            VolumeOfWater = 8,
            VolumeOfStandard = 16,
            Mass = 32,
            TotalOfVolume = 64,
            TotalOfVolumeOfStandard = 128,
            TotalOfVolumeOfWater = 256,
            TotalOfMass = 512
        }
    }
}
