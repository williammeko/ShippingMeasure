using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class ReceiptTankDetail
    {
        public string TankName { get; set; }

        /// <summary>
        /// while property VolumeByHeight is set to true, means that property Heigth is Height value,
        /// otherwise, property Height is Ullage value.
        /// </summary>
        public bool VolumeByHeight { get; set; } = true;

        public decimal? Height { get; set; }

        public decimal? TemperatureOfTank { get; set; }

        public decimal? HeightOfWater { get; set; }

        public decimal? TemperatureMeasured { get; set; }

        public decimal? DensityMeasured { get; set; }

        public decimal? DensityOfStandard { get; set; }

        public decimal? Vcf20 { get; set; }

        public decimal? Volume { get; set; }

        public decimal? VolumeOfWater { get; set; }

        public decimal? VolumeOfStandard { get; set; }

        public decimal? Mass { get; set; }
    }
}
