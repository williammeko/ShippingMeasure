using ShippingMeasure.Core.Models;
using System;

namespace ShippingMeasure
{
    public class DeliveryReceiptLoadingForm : ReceiptForm
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.SetReceiptType(ReceiptType.DeliveryReceiptLoading);
        }
    }
}
