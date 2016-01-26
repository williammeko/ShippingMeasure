using ShippingMeasure.Core.Models;
using System;

namespace ShippingMeasure
{
    public class MassOfOilForm : ReceiptForm
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.SetReceiptType(ReceiptType.MassOfOil);
        }
    }
}
