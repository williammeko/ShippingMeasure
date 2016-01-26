using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShippingMeasure
{
    public static class Extensions
    {

        public static void RecursivelyForEach(this ComboTreeNodeCollection source, Action<ComboTreeNode> action)
        {
            foreach (var n in source)
            {
                action(n);
                n.Nodes.RecursivelyForEach(action);
            }
        }

        public static IEnumerable<T> RecursivelySelect<T>(this ComboTreeNodeCollection source, Func<ComboTreeNode, T> selector)
        {
            foreach (var n in source)
            {
                yield return selector(n);
                n.Nodes.RecursivelySelect(selector);
            }
        }

        public static ComboTreeNode RecursivelyFirstOrDefault(this ComboTreeNodeCollection source, Func<ComboTreeNode, bool> predicate)
        {
            foreach (var n in source)
            {
                if (predicate(n))
                {
                    return n;
                }
                var node = n.Nodes.RecursivelyFirstOrDefault(predicate);
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }

        public static void RecursivelyForEachControl(this Control.ControlCollection source, Action<Control> action)
        {
            foreach (Control c in source)
            {
                action(c);
                c.Controls.RecursivelyForEachControl(action);
            }
        }

        public static void SetCellsEnabled(this DataGridViewColumn source, bool enabled)
        {
            source.ReadOnly = !enabled;

            if (enabled)
            {
                source.DefaultCellStyle.BackColor = SystemColors.Window;
            }
            else
            {
                source.DefaultCellStyle.BackColor = SystemColors.Control;
            }
        }
    }
}