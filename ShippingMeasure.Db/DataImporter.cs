using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Db
{
    public class DataImporter
    {
        private class RawData
        {
            public Vessel Vessel { get; set; }
            public List<TankFileRawData> Tanks { get; } = new List<TankFileRawData>();
        }

        private class TankFileRawData
        {
            public string TankName { get; set; }
            public List<decimal> VInclination { get; } = new List<decimal>();
            public List<TankFileTrimmingCorrectionLine> TrimmingCorrectionLines { get; } = new List<TankFileTrimmingCorrectionLine>();
            public List<decimal> HInclination { get; } = new List<decimal>();
            public List<TankFileListingCorrectionLine> ListingCorrectionLines { get; } = new List<TankFileListingCorrectionLine>();
            public List<TankFileVolumeLine> VolumeLines { get; } = new List<TankFileVolumeLine>();
        }

        private class TankFileTrimmingCorrectionLine
        {
            public decimal Height { get; set; }
            public List<decimal> Corrections { get; } = new List<decimal>();
        }

        private class TankFileListingCorrectionLine
        {
            public decimal Height { get; set; }
            public List<decimal> Correction { get; } = new List<decimal>();
        }

        private class TankFileVolumeLine
        {
            public decimal Height { get; set; }
            public decimal Volume { get; set; }
        }
    }
}
