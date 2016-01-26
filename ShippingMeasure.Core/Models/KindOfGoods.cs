using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class KindOfGoods
    {
        public string UId { get; set; }

        public string Name { get; set; }

        public bool BuiltIn { get; set; } = false;

        public bool Customized { get; set; } = false;

        public bool IsContainer { get; set; } = false;

        public List<KindOfGoods> SubKinds { get; } = new List<KindOfGoods>();

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            var hash = this.UId.GetHashCode()
                + this.Name.GetHashCode()
                + this.BuiltIn.GetHashCode()
                + this.Customized.GetHashCode();
            hash = this.SubKinds.Aggregate(hash, (h, k) => hash + k.GetHashCode());

            return hash;
        }

        public override bool Equals(object obj)
        {
            var target = obj as KindOfGoods;
            if (target == null)
            {
                return false;
            }
            return target.GetHashCode() == this.GetHashCode();
        }
    }
}
