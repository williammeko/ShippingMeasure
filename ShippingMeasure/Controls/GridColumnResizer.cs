using ShippingMeasure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure.Controls
{
    public class GridColumnResizer
    {
        public List<DataGridViewColumn> FixedColumns { get; } = new List<DataGridViewColumn>();

        private Form form;
        private DataGridView grid;
        private Dictionary<DataGridViewColumn, decimal> resizeCols = new Dictionary<DataGridViewColumn, decimal>();

        public GridColumnResizer(Form form, DataGridView grid)
        {
            this.form = form;
            this.grid = grid;
        }

        public void Setup()
        {
            this.grid.Columns.Cast<DataGridViewColumn>().Except(this.FixedColumns).Each(c => this.resizeCols.Add(c, 0m));

            this.grid.ColumnWidthChanged += (s, a) => this.CalculatePercentages();
            this.form.Resize += this.Resize;
            this.grid.Resize += this.Resize;
        }

        private void Resize(object sender, EventArgs e)
        {
            // todo: problem using resizer
            var availableWidth = this.grid.Width - this.FixedColumns.Sum(c => c.Width) - 1;
            this.resizeCols.Keys.Each(c =>
            {
                c.Width = (int)(this.resizeCols[c] * availableWidth);
            });
        }

        private void CalculatePercentages()
        {
            var totalWidth = this.resizeCols.Keys.Sum(c => c.Width).TryToDecimal();
            this.resizeCols.Keys.Each(c =>
            {
                this.resizeCols[c] = c.Width.TryToDecimal() / totalWidth;
            });
        }
    }
}
