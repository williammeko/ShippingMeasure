using ShippingMeasure.Core;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Reports
{
    public class ReceiptReportModel
    {
        public string No { get; set; }

        public string VesselName { get; set; }

        public DateTime? Time { get; set; }

        public string ReceiptFor { get; set; }

        public string PortOfShipment { get; set; }

        public string PortOfDestination { get; set; }

        public string KindOfGoods { get; set; }

        public string HInclinationStatusDescription { get; set; }

        public string VInclinationForeDraftValue { get; set; }

        public string VInclinationAftDraftValue { get; set; }

        public string VInclinationTrimValue { get; set; }

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

        public string HeightHeaderText { get; set; }

        public DateTime TimeSaved { get; set; }

        public List<ReceiptTankDetail> ReceiptTankDetails { get; } = new List<ReceiptTankDetail>();

        public static ReceiptReportModel ConvertFrom(Receipt receipt)
        {
            // todo: use ReceiptReportModel instead of Receipt model, to display a report.
            var model = new ReceiptReportModel
            {
                No = receipt.No,
                VesselName = receipt.VesselName,
                Time = receipt.Time,
                ReceiptFor = receipt.ReceiptFor.Equals("Load", StringComparison.OrdinalIgnoreCase) ? Strings.ReportLabelReceiptForLoad : (receipt.ReceiptFor.Equals("Discharge", StringComparison.OrdinalIgnoreCase) ? Strings.ReportLabelReceiptForDischarge : String.Empty),
                PortOfShipment = receipt.PortOfShipment,
                PortOfDestination = receipt.PortOfDestination,
                KindOfGoods = receipt.KindOfGoods != null ? receipt.KindOfGoods.Name : null,
                HInclinationStatusDescription = GetHStatusDescription(receipt.VesselStatus),
                VInclinationForeDraftValue = receipt.VesselStatus != null ? String.Format("{0} {1}", receipt.VesselStatus.ForeDraft.ToString(), Strings.ReportLabelUnitMetre) : null,
                VInclinationAftDraftValue = receipt.VesselStatus != null ? String.Format("{0} {1}", receipt.VesselStatus.AftDraft.ToString(), Strings.ReportLabelUnitMetre) : null,
                VInclinationTrimValue = receipt.VesselStatus != null ? String.Format("{0} {1}", receipt.VesselStatus.VValue.ToString(), Strings.ReportLabelUnitMetre) : null,
                TotalOfVolumeOfStandard = receipt.TotalOfVolumeOfStandard,
                TotalOfVolume = receipt.TotalOfVolume,
                TotalOfVolumeOfWater = receipt.TotalOfVolumeOfWater,
                TotalOfMass = receipt.TotalOfMass,
                TotalOfVolumeOfPipes = receipt.TotalOfVolumeOfPipes,
                OperaterName = receipt.OperaterName,
                AgentName = receipt.AgentName,
                ShipperName = receipt.ShipperName,
                ConsignerName = receipt.ConsignerName,
                ConsigneeName = receipt.ConsigneeName,
                HeightHeaderText = receipt.ReceiptTankDetails.Count > 0 && !receipt.ReceiptTankDetails.First().VolumeByHeight ? Strings.HeaderUllage : Strings.HeaderHeight,
                TimeSaved = receipt.TimeSaved
            };

            model.ReceiptTankDetails.AddRange(receipt.ReceiptTankDetails);

            return model;
        }

        private static string GetHStatusDescription(VesselStatus vesselStatus)
        {
            if (vesselStatus == null)
            {
                return null;
            }

            return String.Format("{0} {1}{2}",
                vesselStatus.HStatus == HInclinationStatus.P ? Strings.ReportLabelListToPort : Strings.ReportLabelListToStarboard,
                vesselStatus.HValue,
                Strings.ReportLabelUnitDegree);
        }
    }
}
