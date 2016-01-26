using ShippingMeasure.Common;
using ShippingMeasure.Core;
using ShippingMeasure.Core.Models;
using ShippingMeasure.Db;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure.Controls
{
    public class MeasureGridSummary : DataGridView
    {
        public bool IsDirty { get; private set; }
        public string ValidationErrorMessage
        {
            get { return this.validationErrorMessage; }
            set
            {
                this.validationErrorMessage = value;
                this.OnValidationErrorMessageChanged();
            }
        }

        public event Action Dirty;
        public event Action<string> ValidationErrorMessageChanged;

        public decimal? TotalOfVolume
        {
            get { return this.TryGetValue(this.colTotalOfVolume.Index); }
            set { this.TrySetValue(this.colTotalOfVolume.Index, value); }
        }

        public decimal? TotalOfVolumeOfStandard
        {
            get { return this.TryGetValue(this.colTotalOfVolumeOfStandard.Index); }
            set { this.TrySetValue(this.colTotalOfVolumeOfStandard.Index, value); }
        }

        public decimal? TotalOfVolumeOfWater
        {
            get { return this.TryGetValue(this.colTotalOfVolumeOfWater.Index); }
            set { this.TrySetValue(this.colTotalOfVolumeOfWater.Index, value); }
        }

        public decimal? TotalOfMass
        {
            get { return this.TryGetValue(this.colTotalOfMass.Index); }
            set { this.TrySetValue(this.colTotalOfMass.Index, value); }
        }

        private DataGridViewColumn colTotalOfVolumeOfStandardText = new DataGridViewTextBoxColumn { Name = "TotalOfVolumeOfStandardText", HeaderText = Strings.LabelTotalOfVolumeOfStandard };
        private DataGridViewColumn colTotalOfVolumeOfStandard = new DataGridViewTextBoxColumn { Name = "TotalOfVolumeOfStandard", HeaderText = Strings.LabelTotalOfVolumeOfStandard };
        private DataGridViewColumn colTotalOfVolumeText = new DataGridViewTextBoxColumn { Name = "TotalOfVolumeText", HeaderText = Strings.LabelTotalOfVolume };
        private DataGridViewColumn colTotalOfVolume = new DataGridViewTextBoxColumn { Name = "TotalOfVolume", HeaderText = Strings.LabelTotalOfVolume };
        private DataGridViewColumn colTotalOfVolumeOfWaterText = new DataGridViewTextBoxColumn { Name = "TotalOfVolumeOfWaterText", HeaderText = Strings.LabelTotaVlOfolumeOfWater };
        private DataGridViewColumn colTotalOfVolumeOfWater = new DataGridViewTextBoxColumn { Name = "TotaVlOfolumeOfWater", HeaderText = Strings.LabelTotaVlOfolumeOfWater };
        private DataGridViewColumn colTotalOfMassText = new DataGridViewTextBoxColumn { Name = "TotalOfMassText", HeaderText = Strings.LabelTotalOfMass };
        private DataGridViewColumn colTotalOfMass = new DataGridViewTextBoxColumn { Name = "TotalOfMass", HeaderText = Strings.LabelTotalOfMass };

        private DataGridViewRow Row;
        private bool loading = false;
        //private GridColumnResizer resizer;
        private string validationErrorMessage = String.Empty;
        private List<KeyValuePair<DataGridViewCell, string>> validationErrorMessages = new List<KeyValuePair<DataGridViewCell, string>>();
        private Dictionary<int, Action<DataGridViewCell, DataGridViewCellValidatingEventArgs>> colValidationFuncs;

        public void LoadReceipt(Receipt receipt)
        {
            this.loading = true;

            this.TrySetValue(this.colTotalOfVolumeOfStandard.Index, receipt.TotalOfVolumeOfStandard);
            this.TrySetValue(this.colTotalOfVolume.Index, receipt.TotalOfVolume);
            this.TrySetValue(this.colTotalOfVolumeOfWater.Index, receipt.TotalOfVolumeOfWater);
            this.TrySetValue(this.colTotalOfMass.Index, receipt.TotalOfMass);

            this.loading = false;
            this.ReportLoaded();
        }

        public void GetReceiptSummary(Receipt receipt)
        {
            // todo: validation logic on summary

            var r = new Receipt();
            r.TotalOfVolumeOfStandard = this.TotalOfVolumeOfStandard;
            r.TotalOfVolume = this.TotalOfVolume;
            r.TotalOfVolumeOfWater = this.TotalOfVolumeOfWater;
            r.TotalOfMass = this.TotalOfMass;

            receipt.TotalOfVolumeOfStandard = r.TotalOfVolumeOfStandard;
            receipt.TotalOfVolume = r.TotalOfVolume;
            receipt.TotalOfVolumeOfWater = r.TotalOfVolumeOfWater;
            receipt.TotalOfMass = r.TotalOfMass;
        }

        public void NewReceipt()
        {
            this.loading = true;

            this.TrySetValue(this.colTotalOfVolumeOfStandard.Index, null);
            this.TrySetValue(this.colTotalOfVolume.Index, null);
            this.TrySetValue(this.colTotalOfVolumeOfWater.Index, null);
            this.TrySetValue(this.colTotalOfMass.Index, null);

            this.loading = false;
            this.ReportLoaded();
        }

        public void ReportSaved()
        {
            this.IsDirty = false;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            this.AllowUserToAddRows = false;
            this.ColumnHeadersVisible = false;
            this.RowHeadersVisible = false;

            if (!this.DesignMode)
            {
                this.Columns.Clear();
                this.EnsureColumn(this.colTotalOfVolumeOfStandardText);
                this.EnsureColumn(this.colTotalOfVolumeOfStandard);
                this.EnsureColumn(this.colTotalOfVolumeText);
                this.EnsureColumn(this.colTotalOfVolume);
                this.EnsureColumn(this.colTotalOfVolumeOfWaterText);
                this.EnsureColumn(this.colTotalOfVolumeOfWater);
                this.EnsureColumn(this.colTotalOfMassText);
                this.EnsureColumn(this.colTotalOfMass);

                var font = new Font(this.Font.FontFamily, this.Font.Size + 3, FontStyle.Bold);

                this.colTotalOfVolumeOfStandard.ValueType = typeof(decimal);
                this.colTotalOfVolume.ValueType = typeof(decimal);
                this.colTotalOfVolumeOfWater.ValueType = typeof(decimal);
                this.colTotalOfMass.ValueType = typeof(decimal);

                this.colTotalOfVolumeOfStandardText.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Font = font });
                this.colTotalOfVolumeText.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Font = font });
                this.colTotalOfVolumeOfWaterText.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Font = font });
                this.colTotalOfMassText.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Font = font });

                this.colTotalOfVolumeOfStandard.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3", Font = font });
                this.colTotalOfVolume.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3", Font = font });
                this.colTotalOfVolumeOfWater.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3", Font = font });
                this.colTotalOfMass.DefaultCellStyle.ApplyStyle(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "F3", Font = font });

                this.EnsureRow();
                this.SetupValidation();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //if (this.resizer == null)
            //{
            //    var cols = this.Columns.Cast<DataGridViewColumn>();
            //    var resizeCols = cols.Where(c => !c.ReadOnly).ToList();
            //    var width = (this.Width - cols.Where(c => c.ReadOnly).Sum(c => c.Width)) / resizeCols.Count() - 1;
            //    resizeCols.ForEach(c => c.Width = width);
            //    this.resizer = new GridColumnResizer(this.TopLevelControl as Form, this);
            //    this.resizer.FixedColumns.AddRange(this.Columns.Cast<DataGridViewColumn>().Where(c => c.ReadOnly == true));
            //    this.resizer.Setup();
            //}

            if (!this.DesignMode)
            {
                var cols = this.Columns.Cast<DataGridViewColumn>();
                var resizeCols = cols.Where(c => !c.ReadOnly).ToList();
                var width = (this.Width - cols.Where(c => c.ReadOnly).Sum(c => c.Width)) / resizeCols.Count() - 1;
                resizeCols.ForEach(c => c.Width = width);
            }

            base.OnPaint(e);
        }

        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            base.OnCellValueChanged(e);

            if (e.ColumnIndex == this.colTotalOfVolumeOfStandard.Index)
            {
                this.ReportDirty();
            }
            else if (e.ColumnIndex == this.colTotalOfVolume.Index)
            {
                this.ReportDirty();
            }
            else if (e.ColumnIndex == this.colTotalOfVolumeOfWater.Index)
            {
                this.ReportDirty();
            }
            else if (e.ColumnIndex == this.colTotalOfMass.Index)
            {
                this.ReportDirty();
            }
        }

        private decimal? TryGetValue(int columnIndex)
        {
            if (this.Row == null)
            {
                return null;
            }

            var value = this.Row.Cells[columnIndex].Value;
            if (value != null)
            {
                try { return value.TryToDecimal(); }
                catch { }
            }
            return 0m;
        }

        private void TrySetValue(int columnIndex, decimal? value)
        {
            if (this.Row != null)
            {
                this.Row.Cells[columnIndex].Value = value;
            }
        }

        private void EnsureColumn(DataGridViewColumn column)
        {
            if (!this.Columns.Contains(column.Name))
            {
                this.Columns.Add(column);
            }
        }

        private void EnsureRow()
        {
            this.Row = null;
            this.Rows.Clear();

            this.Row = new DataGridViewRow();
            this.Rows.Add(this.Row);
            this.Row.Cells[colTotalOfVolumeOfStandardText.Index].Value = this.colTotalOfVolumeOfStandardText.HeaderText;
            //this.colTotalOfVolumeOfStandardText.ReadOnly = true;
            this.colTotalOfVolumeOfStandardText.SetCellsEnabled(false);
            this.colTotalOfVolumeOfStandardText.Width = 180;

            this.Row.Cells[colTotalOfVolumeText.Index].Value = this.colTotalOfVolumeText.HeaderText;
            //this.colTotalOfVolumeText.ReadOnly = true;
            this.colTotalOfVolumeText.SetCellsEnabled(false);
            this.colTotalOfVolumeText.Width = 180;

            this.Row.Cells[colTotalOfVolumeOfWaterText.Index].Value = this.colTotalOfVolumeOfWaterText.HeaderText;
            //this.colTotalOfVolumeOfWaterText.ReadOnly = true;
            this.colTotalOfVolumeOfWaterText.SetCellsEnabled(false);
            this.colTotalOfVolumeOfWaterText.Width = 180;

            this.Row.Cells[colTotalOfMassText.Index].Value = this.colTotalOfMassText.HeaderText;
            //this.colTotalOfMassText.ReadOnly = true;
            this.colTotalOfMassText.SetCellsEnabled(false);
            this.colTotalOfMassText.Width = 180;

            this.CellEnter += (s, a) =>
            {
                if (this.Columns.Cast<DataGridViewColumn>().Where(c => c.ReadOnly).Select(c => c.Index).Any(i => i.Equals(a.ColumnIndex)))
                {
                    this.Row.Cells[a.ColumnIndex].Selected = false;
                    this.Row.Cells[a.ColumnIndex + 1].Selected = true;
                }
            };

            this.SelectionChanged += (s, a) =>
            {
                if (this.SelectedCells.Count > 0)
                {
                    var selectedCell = this.SelectedCells[0];
                    if (selectedCell.ReadOnly)
                    {
                        if (selectedCell.ColumnIndex < this.ColumnCount - 1)
                        {
                            this.CurrentCell = this.Row.Cells[selectedCell.ColumnIndex + 1];
                        }
                    }
                    else
                    {
                        try
                        {
                            this.CurrentCell = selectedCell;
                        }
                        catch { }
                    }
                }
            };
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
                { this.colTotalOfVolumeOfStandard.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorTotalOfVolumeOfStandard) },
                { this.colTotalOfVolume.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorTotalOfVolume) },
                { this.colTotalOfVolumeOfWater.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorTotalOfVolumeOfWater) },
                { this.colTotalOfMass.Index, (c, a) => handleValidation(c, a, !Decimal.TryParse(a.FormattedValue.ToString(), out temp), Strings.MsgCellErrorTotalOfMass) }
            };
        }

        private void OnValidationErrorMessageChanged()
        {
            if (this.ValidationErrorMessageChanged != null)
            {
                this.ValidationErrorMessageChanged(this.ValidationErrorMessage);
            }
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
