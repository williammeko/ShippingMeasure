using ShippingMeasure.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure.Controls
{
    public class DataGridViewCheckedComboBoxColumn : DataGridViewColumn
    {
        public DataGridViewCheckedComboBoxColumn() : base(new DataGridViewCheckedComboBoxCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CheckedComboBoxCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewCheckedComboBoxCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewCheckedComboBoxCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    public class DataGridViewCheckedComboBoxCell : DataGridViewTextBoxCell
    {
        public DataGridViewCheckedComboBoxCell() : base()
        {
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var ctl = DataGridView.EditingControl as DataGridViewCheckedComboBoxEditingControl;
            var checkedItems = initialFormattedValue as List<CheckedComboBoxItem>;
            if (checkedItems != null)
            {
                ctl.Items
                    .Cast<CheckedComboBoxItem>()
                    .ToList()
                    .ForEach(i => i.CheckState = checkedItems.Any(checkedItem => i.Text.Equals(checkedItem.Text) && i.Tag == checkedItem.Tag));
            }
            else
            {
                ctl.Items.Cast<CheckedComboBoxItem>()
                    .ToList()
                    .ForEach(i => i.CheckState = false);
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that DataGridViewCheckedComboBoxCell uses.
                return typeof(DataGridViewCheckedComboBoxEditingControl);
            }
        }

        public new List<CheckedComboBoxItem> Value { get; set; }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that DataGridViewCheckedComboBoxCell contains.
                return typeof(List<CheckedComboBoxItem>);
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                return this.ValueType;
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use empty list as the default value.
                return new List<CheckedComboBoxItem>();
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            var checkedItems = formattedValue as IEnumerable<CheckedComboBoxItem>;
            var formattedText = (checkedItems != null)
                ? checkedItems.Aggregate(new StringBuilder(), (s, i) => s.AppendFormat("{0}, ", i.Text), s => s.Length > 0 ? s.ToString(0, s.Length - 2) : String.Empty)
                : String.Empty;
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedText, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
    }

    public class DataGridViewCheckedComboBoxEditingControl : CheckedComboBox, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public DataGridViewCheckedComboBoxEditingControl()
        {
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property.
        public object EditingControlFormattedValue
        {
            get
            {
                //return this.Items.Cast<CheckedComboBoxItem>().Where(i => i.CheckState).ToList();
                return this.Items
                    .Cast<CheckedComboBoxItem>()
                    .Where(i => i.CheckState)
                    .ToList();
            }
            set
            {
                var checkedItems = (List<CheckedComboBoxItem>)value;
                this.Items.Cast<CheckedComboBoxItem>().Each(i => i.CheckState = checkedItems.Any(checkedItem => i.Text.Equals(checkedItem.Text) && i.Tag == checkedItem.Tag));
            }
        }

        // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            //return (EditingControlFormattedValue as IEnumerable<CheckedComboBoxItem>).Aggregate(new StringBuilder(), (s, i) => s.AppendFormat("{0}, ", i.Text), s => s.Length > 0 ? s.ToString(0, s.Length - 2) : String.Empty);
            return  this.EditingControlFormattedValue;
        }

        // Implements the IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex property.
        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey method.
        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the CheckedListBox handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Space:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl.RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        // Implements the IDataGridViewEditingControl.EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        // Implements the IDataGridViewEditingControl.EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value; }
        }

        // Implements the IDataGridViewEditingControl.EditingPanelCursor property.
        public Cursor EditingPanelCursor
        {
            get { return base.Cursor; }
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            base.OnSelectedValueChanged(e);

            this.valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }
    }
}
