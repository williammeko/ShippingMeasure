using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure.Controls
{
    public class DataGridViewCheckedListBoxColumn : DataGridViewColumn
    {
        public DataGridViewCheckedListBoxColumn() : base(new DataGridViewCheckedListBoxCell())
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
                // Ensure that the cell used for the template is a CheckedListBoxCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewCheckedListBoxCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewCheckedListBoxCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    public class DataGridViewCheckedListBoxCell : DataGridViewTextBoxCell
    {
        public DataGridViewCheckedListBoxCell() : base()
        {
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var ctl = DataGridView.EditingControl as DataGridViewCheckedListBoxEditingControl;
            // Use the default row value when Value property is null.
            if (this.Value == null)
            {
                ctl.Value = (List<object>)this.DefaultNewRowValue;
            }
            else
            {
                ctl.Value = (List<object>)this.Value;
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that DataGridViewCheckedListBoxCell uses.
                return typeof(DataGridViewCheckedListBoxEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that DataGridViewCheckedListBoxCell contains.
                return typeof(List<object>);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use empty list as the default value.
                return new List<object>();
            }
        }
    }

    public class DataGridViewCheckedListBoxEditingControl : CheckedListBox, IDataGridViewEditingControl
    {
        public List<object> Value { get; set; }

        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public DataGridViewCheckedListBoxEditingControl()
        {
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property.
        public object EditingControlFormattedValue
        {
            get
            {
                if (this.Value == null)
                {
                    return String.Empty;
                }
                var sb = new StringBuilder();
                this.Value.ForEach(o => sb.AppendFormat("{0}, ", o.ToString()));
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 2, 2);
                }
                return sb.ToString();
            }
            set
            {
                if (value is String)
                {
                    if (value == null)
                    {
                        this.Value = null;
                        return;
                    }
                    var val = new List<object>();
                    val.AddRange(value.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    this.Value = val;
                    return;
                }
                this.Value = (List<object>)value;
            }
        }

        // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
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
