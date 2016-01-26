using ShippingMeasure.Core.Models;
using System;

namespace ShippingMeasure
{
    public class DeliveryReceiptDestinationForm : ReceiptForm
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.SetReceiptType(ReceiptType.DeliveryReceiptDestination);
        }
    }
}
