using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class VolumeCorrectionFactor
    {
        public decimal Temp { get; set; }

        public decimal DensityOfStandard { get; set; }

        public decimal Factor { get; set; }
    }
}
