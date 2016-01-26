using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Core.Models
{
    public class Receipt
    {
        public string No { get; set; }

        public string VesselName { get; set; }

        public DateTime? Time { get; set; }

        public string ReceiptFor { get; set; }

        public string PortOfShipment { get; set; }

        public string PortOfDestination { get; set; }

        public KindOfGoods KindOfGoods { get; set; }

        public VesselStatus VesselStatus { get; set; }

        public decimal? TotalOfVolumeOfStandard { get; set; }

        public decimal? TotalOfVolume { get; set; }

        public decimal? TotalOfVolumeOfWater { get; set; }

        public decimal? TotalOfMass { get; set; }

        public decimal? TotalOfVolumeOfPipes { get; set; }

        public string OperaterName { get; set; }

        public string AgentName { get; set; }

        public string ShipperName { get; set; }

        public string ConsignerName { get; set; }

        public string ConsigneeName { get; set; }

        public DateTime TimeSaved { get; set; }

        public ReceiptType ReceiptType { get; set; }

        public List<ReceiptTankDetail> ReceiptTankDetails { get; } = new List<ReceiptTankDetail>();
    }
}
