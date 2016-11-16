using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class TrimmingHeightCorrection
    {
        public string TankName { get; set; }
        public decimal Height { get; set; }
        public decimal VInclination { get; set; }
        public decimal Correction { get; set; }
    }
}
