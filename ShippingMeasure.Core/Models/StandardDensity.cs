using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class StandardDensity : IComparable<StandardDensity>
    {
        public decimal Temp { get; set; }

        public decimal DensityMeasured { get; set; }

        public decimal DensityOfStandard { get; set; }

        public int CompareTo(StandardDensity other)
        {
            if (this.Temp == other.Temp
                && this.DensityMeasured == other.DensityMeasured
                && this.DensityOfStandard == other.DensityOfStandard)
            {
                return 0;
            }

            if (this.Temp <= other.Temp
                && this.DensityMeasured <= other.DensityMeasured
                && this.DensityOfStandard <= other.DensityOfStandard)
            {
                return -1;
            }

            if (this.Temp >= other.Temp
                && this.DensityMeasured >= other.DensityMeasured
                && this.DensityOfStandard >= other.DensityOfStandard)
            {
                return 1;
            }

            return this.DensityOfStandard.CompareTo(other.DensityOfStandard);
        }
    }
}
