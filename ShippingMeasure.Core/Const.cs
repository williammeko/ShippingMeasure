using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core
{
    public class Const
    {
        public static List<KindOfGoods> BuiltInKindsOfGoods { get; }

        public static List<KindOfGoods> NonPetroleumProducts { get; }

        static Const()
        {
            Const.BuiltInKindsOfGoods = new List<KindOfGoods>()
            {
                new KindOfGoods { UId = "044FC8CD-6D85-44B2-BBF8-2DD6C683B875", Name = Strings.BuiltInKindPetroleumProducts, BuiltIn = true, IsContainer = true },
                new KindOfGoods { UId = "6011C252-29DA-4E5F-B96B-D2096ABB99D6", Name = Strings.BuiltInKindLubricant, BuiltIn = true },
                new KindOfGoods { UId = "E80C2BD2-CD58-42A5-B384-C286CB614609", Name = Strings.BuiltInKindCrudeOil, BuiltIn = true },
                new KindOfGoods { UId = "058F93AC-3C23-4F73-8763-D10EB87E2523", Name = Strings.BuiltInKindChemicalProduct, BuiltIn = true }
            };

            Const.BuiltInKindsOfGoods.First(k => k.IsContainer).SubKinds.AddRange(new[]
            {
                new KindOfGoods { UId = "EBD74505-CDF7-4EE7-B230-BA0CB257A5E5", Name = Strings.BuiltInKindDiesel, BuiltIn = true },
                new KindOfGoods { UId = "2DFAD355-1794-41D1-AA24-83CFE857A153", Name = Strings.BuiltInKindPetrol, BuiltIn = true },
                new KindOfGoods { UId = "0788B1E4-DA4F-409C-830F-E50F7B14F562", Name = Strings.BuiltInKindCustomized, BuiltIn = true, Customized = true, IsContainer = true }
            });

            var nonPetroleumProductUIds = new[] { "6011C252-29DA-4E5F-B96B-D2096ABB99D6", "E80C2BD2-CD58-42A5-B384-C286CB614609", "058F93AC-3C23-4F73-8763-D10EB87E2523" };
            Const.NonPetroleumProducts = (
                from k in Const.BuiltInKindsOfGoods
                where nonPetroleumProductUIds.Any(uid => k.UId.Equals(uid, StringComparison.OrdinalIgnoreCase))
                select k
            ).ToList();
        }
    }
}
