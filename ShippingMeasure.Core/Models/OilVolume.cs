using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class OilVolume
    {
        public string TankName { get; set; }

        public decimal HInclination { get; set; }

        public decimal VInclination { get; set; }

        public decimal Height { get; set; }

        public decimal Volume { get; set; }
    }
}
