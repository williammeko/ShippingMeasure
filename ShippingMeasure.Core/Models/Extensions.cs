using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public static class Extensions
    {
        public static KindOfGoods FirstOrDefaultRecursively(this IEnumerable<KindOfGoods> source, Func<KindOfGoods, bool> predicate)
        {
            foreach (var k in source)
            {
                if (predicate(k))
                {
                    return k;
                }
                var kind = k.SubKinds.FirstOrDefaultRecursively(predicate);
                if (kind != null)
                {
                    return kind;
                }
            }
            return null;
        }

        public static ReceiptType ToReceiptType(this object type)
        {
            if (type == null || !type.ToString().Trim().Any())
            {
                return ReceiptType.MassOfOil;
            }
            return (ReceiptType)Enum.Parse(typeof(ReceiptType), type.ToString(), true);
        }

        public static string ToLocString(this ReceiptType receiptType)
        {
            switch (receiptType)
            {
                case ReceiptType.DeliveryReceiptLoading:
                    return Strings.ReceiptTypeNameDeliveryReceiptLoading;
                case ReceiptType.DeliveryReceiptDestination:
                    return Strings.ReceiptTypeNameDeliveryReceiptDestination;
                case ReceiptType.MassOfOil:
                default:
                    return Strings.ReceiptTypeNameMassOfOil;
            }
        }
    }
}
