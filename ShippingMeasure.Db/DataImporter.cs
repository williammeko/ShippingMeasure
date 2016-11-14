using ShippingMeasure.Common;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            public List<decimal> VInclinationColumns { get; } = new List<decimal>();
            public List<TankFileTrimmingCorrectionLine> TrimmingCorrectionLines { get; } = new List<TankFileTrimmingCorrectionLine>();
            public List<decimal> HInclinationColumns { get; } = new List<decimal>();
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
            public List<decimal> Corrections { get; } = new List<decimal>();
        }

        private class TankFileVolumeLine
        {
            public decimal Height { get; set; }
            public decimal Volume { get; set; }
        }

        private enum TankFileDataType
        {
            None,
            TrimmingHeightCorrection,
            ListingHeightCorrection,
            Volume,
        }

        public List<string> RawDataFiles { get; } = new List<string>();
        public string AccessFile { get; set; }
        public event Action<TraceLevel, string> WriteLog;

        private RawData rawData;

        public void Import()
        {
            this.rawData = new RawData();

            this.RawDataFiles.ForEach(f =>
            {
                try
                {
                    this.Read(f);
                }
                catch (Exception ex)
                {
                    this.OnWriteLog(TraceLevel.Error, ex.ToString());
                    throw new FormatException(String.Format("Incorrect raw data file: \"{0}\"", f), ex);
                }
            });

            this.OnWriteLog(TraceLevel.Info, String.Format("Connecting to DB file: {0} ...", this.AccessFile));
            var tankDb = new TankDb() { ConnectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};", this.AccessFile) };
            try
            {
                this.OnWriteLog(TraceLevel.Info, "Removing original tanks from DB ...");
                tankDb.ClearTanks();
                this.OnWriteLog(TraceLevel.Info, "Removing original TrimmingHeightCorrection data from DB ...");
                tankDb.ClearTrimmingHeightCorrectionItems();
                this.OnWriteLog(TraceLevel.Info, "Removing original ListingHeightCorrection data from DB ...");
                tankDb.ClearListingHeightCorrectionItems();
                this.OnWriteLog(TraceLevel.Info, String.Format("Saving vessel information: {0}, Cert No.: {1} ...", this.rawData.Vessel.Name, this.rawData.Vessel.CertNo));
                tankDb.SaveVessel(this.rawData.Vessel);
                tankDb.Add(this.rawData.Tanks.Select(t => new Tank { Name = t.TankName, Height = t.HeightOfTank }));
                this.rawData.Tanks.ForEach(t =>
                {
                    this.OnWriteLog(TraceLevel.Info, String.Format("Adding TrimmingHeightCorrection data of Tank: {0}, Records: {1} ...", t.TankName, t.TrimmingCorrectionLines.Count));
                    tankDb.Add(t.TrimmingCorrectionLines);
                    this.OnWriteLog(TraceLevel.Info, String.Format("Adding ListingHeightCorrection data of Tank: {0}, Records: {1} ...", t.TankName, t.ListingCorrectionLines.Count));
                    tankDb.Add(t.ListingCorrectionLines);
                });
            }
            catch (Exception ex)
            {
                this.OnWriteLog(TraceLevel.Error, ex.ToString());
                throw ex;
            }
        }

        private void Read(string file)
        {
            using (var reader = new StreamReader(file))
            {
                this.OnWriteLog(TraceLevel.Info, String.Format("Reading header from raw data: {0} ...", file));
                var tankRawData = this.ReadHeader(reader, file);

                this.OnWriteLog(TraceLevel.Info, String.Format("Reading detail data from raw data: {0} ...", file));
                this.ReadData(reader, tankRawData);

                this.rawData.Tanks.Add(tankRawData);
            }
        }

        private void ReadData(StreamReader reader, TankFileRawData tankRawData)
        {
            var dataType = TankFileDataType.None;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();

                switch (line)
                {
                    case "1": // "1" means begin of trimming height correction lines
                        dataType = TankFileDataType.TrimmingHeightCorrection;
                        this.ReadTrimmingHeightCorrectionColumns(reader, tankRawData);
                        break;
                    case "2": // "2" means begin of listing height correction lines
                        dataType = TankFileDataType.ListingHeightCorrection;
                        this.ReadListingHeightCorrectionColumns(reader, tankRawData);
                        break;
                    case "3": // "3" means begin of volmne lines
                        dataType = TankFileDataType.Volume;
                        break;
                    default:
                        ReadDataLine(reader, dataType, tankRawData); // read one line of data of specified data type
                        break;
                }
            }
        }

        private void ReadTrimmingHeightCorrectionColumns(StreamReader reader, TankFileRawData tankRawData)
        {
            if (tankRawData.VInclinationColumns.Any())
            {
                throw new FormatException("Duplicating columns of trimming heigh correction");
            }

            if (reader.EndOfStream)
            {
                throw new FormatException("Could not find columns of trimming heigth correction data");
            }

            var line = reader.ReadLine().Trim();
            if (this.IsInvalidLine(line))
            {
                return;
            }

            List<decimal> columns;
            try
            {
                columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.ToDecimal())
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new FormatException("Error parsing columns of trimming height correction", ex);
            }

            tankRawData.VInclinationColumns.AddRange(columns);
        }

        private void ReadListingHeightCorrectionColumns(StreamReader reader, TankFileRawData tankRawData)
        {
            if (tankRawData.HInclinationColumns.Any())
            {
                throw new FormatException("Duplicating columns of listing heigh correction");
            }

            if (reader.EndOfStream)
            {
                throw new FormatException("Could not find columns of listing heigth correction data");
            }

            var line = reader.ReadLine().Trim();
            if (this.IsInvalidLine(line))
            {
                return;
            }

            List<decimal> columns;
            try
            {
                columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.ToDecimal())
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new FormatException("Error parsing columns of listing height correction", ex);
            }

            tankRawData.HInclinationColumns.AddRange(columns);
        }

        private void ReadDataLine(StreamReader reader, TankFileDataType dataType, TankFileRawData tankRawData)
        {
            if (reader.EndOfStream)
            {
                return;
            }

            switch (dataType)
            {
                case TankFileDataType.TrimmingHeightCorrection:
                    this.ReadTrimmingHeightCorrectionLine(reader, tankRawData);
                    break;
                case TankFileDataType.ListingHeightCorrection:
                    this.ReadListingHeightCorrectionLine(reader, tankRawData);
                    break;
                case TankFileDataType.Volume:
                    this.ReadVolumeLine(reader, tankRawData);
                    break;
                default:
                    break;
            }
        }

        private void ReadTrimmingHeightCorrectionLine(StreamReader reader, TankFileRawData tankRawData)
        {
            // todo ...
            var line = reader.ReadLine().Trim();
            if (this.IsInvalidLine(line))
            {
                return;
            }

            decimal d;
            List<decimal> data = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Decimal.TryParse(s, out d))
                .Select(s => s.ToDecimal())
                .ToList();

            if (data.Count != tankRawData.VInclinationColumns.Count + 2)
            {
                throw new FormatException(String.Format("Invalid trimming height correction line: {0}", line));
            }

            var item = new TankFileTrimmingCorrectionLine { Height = data[0], };
            for (int i = 2; i < data.Count; i++)
            {
                item.Corrections.Add(data[i]);
            }
            tankRawData.TrimmingCorrectionLines.Add(item);
        }

        private void ReadListingHeightCorrectionLine(StreamReader reader, TankFileRawData tankRawData)
        {
            var line = reader.ReadLine().Trim();
            if (this.IsInvalidLine(line))
            {
                return;
            }

            decimal d;
            List<decimal> data = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Decimal.TryParse(s, out d))
                .Select(s => s.ToDecimal())
                .ToList();

            if (data.Count != tankRawData.HInclinationColumns.Count + 2)
            {
                throw new FormatException(String.Format("Invalid listing height correction line: {0}", line));
            }

            var item = new TankFileListingCorrectionLine { Height = data[0], };
            for (int i = 2; i < data.Count; i++)
            {
                item.Corrections.Add(data[i]);
            }
            tankRawData.ListingCorrectionLines.Add(item);
        }

        private void ReadVolumeLine(StreamReader reader, TankFileRawData tankRawData)
        {
            // todo ...
            var line = reader.ReadLine().Trim();
            if (this.IsInvalidLine(line))
            {
                return;
            }

            decimal d;
            List<decimal> data = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => Decimal.TryParse(s, out d))
                .Select(s => s.ToDecimal())
                .ToList();

            if (data.Count < 3)
            {
                throw new FormatException(String.Format("Invalid volume line: {0}", line));
            }

            tankRawData.VolumeLines.Add(new TankFileVolumeLine
            {
                Height = data[0],
                Volume = data[2],
            });
        }

        private TankFileRawData ReadHeader(StreamReader reader, string fileName)
        {
            TankFileRawData tankRawData;

            // line 1 = certification number, line 2 = vessel name
            var certNo = reader.ReadLine().Trim();
            var vesselName = reader.ReadLine().Trim();
            if (this.rawData.Vessel.CertNo != null && this.rawData.Vessel.CertNo != certNo)
            {
                var message = String.Format("There are more than one certification numbers defined in the raw files. The one in file \"{0}\" is \"{1}\", however there is another one is \"{2}\"",
                    fileName, certNo, this.rawData.Vessel.CertNo);
                throw new FormatException(message);
            }
            else
            {
                this.rawData.Vessel.CertNo = certNo;
                this.OnWriteLog(TraceLevel.Info, String.Format("Vessel Cert No.: {0}", certNo));
            }
            if (this.rawData.Vessel.Name != null && this.rawData.Vessel.Name != vesselName)
            {
                var message = String.Format("There are more than one vessel names defined in the raw files. The one in file \"{0}\" is \"{1}\", however there is another one is \"{2}\"",
                    fileName, vesselName, this.rawData.Vessel.Name);
                throw new FormatException(message);
            }
            else
            {
                this.rawData.Vessel.Name = vesselName;
                this.OnWriteLog(TraceLevel.Info, String.Format("Vessel name: {0}", vesselName));
            }

            // line 3 = tank name
            var tankName = reader.ReadLine().Trim();
            if (this.rawData.Tanks.Any(t => t.TankName == tankName))
            {
                throw new FormatException(String.Format("Duplicated tank definition. Tank: \"{0}\", File: \"{1}\"", tankName, fileName));
            }
            else
            {
                tankRawData = new TankFileRawData { TankName = tankName };
                this.OnWriteLog(TraceLevel.Info, String.Format("Tank name: {0}", tankName));
            }

            // if vessel is correct, and tank is not duplicated, return the read tank raw data
            return tankRawData;
        }

        private bool IsInvalidLine(string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                return true;
            }

            var whiteChars = new[] { ' ', '\t' };
            var charsWithoutSpace = line.Where(c => !whiteChars.Any(whiteChar => whiteChar == c));
            var isAsteriskLine = charsWithoutSpace.Take(5).All(c => c == '*');

            return isAsteriskLine;
        }

        private void OnWriteLog(TraceLevel level, string message)
        {
            if (this.WriteLog != null)
            {
                this.WriteLog(level, message);
            }
        }
    }
}
