using DropDownControls;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using ShippingMeasure.Common;

namespace ShippingMeasure.Controls
{
    public class PipesColumn : ComboTreeBoxColumn
    {
        public PipesColumn() : base()
        {
            this.CellTemplate = new PipesCell();
        }
    }


    public class PipesCell : ComboTreeBoxCell
    {
        public override Type EditType
        {
            get
            {
                return typeof(PipesEditingControl);
            }
        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            var nodes = formattedValue as ComboTreeNodeCollection;
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    this.Nodes.Add(node);
                }
            }
            return formattedValue;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            //base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            BeforePaintContent(graphics, cellBounds, cellState, cellStyle, paintParts);

            if (paintParts.HasFlag(DataGridViewPaintParts.ContentForeground))
            {
                ComboTreeBoxColumn column = (ComboTreeBoxColumn)OwningColumn;
                Rectangle contentBounds = cellBounds;
                contentBounds.Width -= 17;
                TextFormatFlags flags = PipesBox.TEXT_FORMAT_FLAGS | TextFormatFlags.PreserveGraphicsClipping;
                Rectangle txtBounds = new Rectangle(contentBounds.Left + 1, contentBounds.Top, contentBounds.Right - 4, contentBounds.Height);

                var sum = 0m;
                var nodes = formattedValue as ComboTreeNodeCollection;
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        if (node.Checked)
                        {
                            sum += ((Pipe)node.Tag).Volume;
                        }
                    }
                }

                TextRenderer.DrawText(graphics, String.Format("{0} m³", sum), cellStyle.Font, txtBounds, cellStyle.ForeColor, flags);
            }

            AfterPaintContent(graphics, clipBounds, cellBounds, rowIndex, cellStyle, advancedBorderStyle, paintParts);
        }
    }


    /// <summary>
    /// Editing control to accompany <see cref="ComboTreeBoxCell"/>.
    /// </summary>
    public class PipesEditingControl : PipesBox, IDataGridViewEditingControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PipesEditingControl()
        {
            TabStop = false;
        }

        #region IDataGridViewEditingControl Members

        void IDataGridViewEditingControl.ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            // font
            Font = dataGridViewCellStyle.Font;

            // background colour
            if (dataGridViewCellStyle.BackColor.A < 255)
            {
                Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
                BackColor = opaqueBackColor;
                ((IDataGridViewEditingControl)this).EditingControlDataGridView.EditingPanel.BackColor = opaqueBackColor;
            }
            else
            {
                BackColor = dataGridViewCellStyle.BackColor;
            }

            // foreground colour
            ForeColor = dataGridViewCellStyle.ForeColor;
        }

        DataGridView IDataGridViewEditingControl.EditingControlDataGridView
        {
            get;
            set;
        }

        object IDataGridViewEditingControl.EditingControlFormattedValue
        {
            get
            {
                return ((IDataGridViewEditingControl)this).GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
            }
            set
            {
            }
        }

        int IDataGridViewEditingControl.EditingControlRowIndex
        {
            get;
            set;
        }

        bool IDataGridViewEditingControl.EditingControlValueChanged
        {
            get;
            set;
        }

        bool IDataGridViewEditingControl.EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            if ((keyData & Keys.KeyCode) == Keys.Down ||
                (keyData & Keys.KeyCode) == Keys.Up ||
                (DroppedDown && ((keyData & Keys.KeyCode) == Keys.Escape) || (keyData & Keys.KeyCode) == Keys.Enter))
            {
                return true;
            }

            return !dataGridViewWantsInputKey;
        }

        Cursor IDataGridViewEditingControl.EditingPanelCursor
        {
            get { return Cursors.Default; }
        }

        object IDataGridViewEditingControl.GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Nodes;
        }

        void IDataGridViewEditingControl.PrepareEditingControlForEdit(bool selectAll)
        {
            DroppedDown = true;
        }

        bool IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        #endregion

        private void NotifyDataGridViewOfValueChange()
        {
            ((IDataGridViewEditingControl)this).EditingControlValueChanged = true;
            ((IDataGridViewEditingControl)this).EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        protected override void OnAfterCheck(ComboTreeNodeEventArgs e)
        {
            base.OnAfterCheck(e);
            NotifyDataGridViewOfValueChange();
        }
    }


    public class PipesBox : ComboTreeBox
    {
        internal const TextFormatFlags TEXT_FORMAT_FLAGS = TextFormatFlags.TextBoxControl | TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.PathEllipsis;

        public PipesBox() : base()
        {
            this.ShowCheckBoxes = true;
        }

        protected override void OnPaintContent(DropDownPaintEventArgs e)
        {
            decimal sum = 0m;
            foreach (var node in this.CheckedNodes)
            {
                var pipe = node.Tag as Pipe;
                if (pipe != null)
                {
                    sum += pipe.Volume;
                }
            }
            Rectangle txtBounds = new Rectangle(1, 0, e.Bounds.Width - 4, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, String.Format("{0} m³", sum), Font, txtBounds, ForeColor, TEXT_FORMAT_FLAGS);
            if (Focused && ShowFocusCues && !DroppedDown) e.DrawFocusRectangle();
        }
    }
}
