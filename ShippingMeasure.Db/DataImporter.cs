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

        }

    }
}
