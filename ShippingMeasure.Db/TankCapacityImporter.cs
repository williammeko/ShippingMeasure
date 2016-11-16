using ShippingMeasure.Common;
using ShippingMeasure.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ShippingMeasure.Db
{
    [Obsolete]
    public class TankCapacityImporter
    {
        private class RawData
        {
            public Vessel Vessel { get; set; }
            public List<TankRawData> Tanks { get; private set; }

            public RawData()
            {
                this.Vessel = new Vessel();
                this.Tanks = new List<TankRawData>();
            }
        }

        private class TankRawData
        {
            public string TankName { get; set; }
            public int CountOfHeightsPerHInclination { get; set; }
            public int CountOfVInclination { get; set; }
            public int CountOfHInclination { get; set; }
            public decimal HeightOfTank { get; set; }
            public List<OilVolume> VolumeItems { get; set; }

            public TankRawData()
            {
                this.VolumeItems = new List<OilVolume>();
            }
        }

        public List<string> RawDataFiles { get; private set; }
        public string AccessFile { get; set; }
        public event Action<TraceLevel, string> WriteLog;

        private RawData rawData;

        public TankCapacityImporter()
        {
            this.RawDataFiles = new List<string>();
        }

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
                this.OnWriteLog(TraceLevel.Info, "Removing original volume data from DB ...");
                tankDb.ClearOilVolumeItems();
                this.OnWriteLog(TraceLevel.Info, String.Format("Saving vessel information: {0}, Cert No.: {1} ...", this.rawData.Vessel.Name, this.rawData.Vessel.CertNo));
                tankDb.SaveVessel(this.rawData.Vessel);
                tankDb.Add(this.rawData.Tanks.Select(t => new Tank { Name = t.TankName, Height = t.HeightOfTank }));
                this.rawData.Tanks.ForEach(t =>
                {
                    this.OnWriteLog(TraceLevel.Info, String.Format("Adding volume data of Tank: {0}, Records: {1} ...", t.TankName, t.VolumeItems.Count()));
                    tankDb.Add(t.VolumeItems);
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
                this.ReadList(reader, tankRawData);

                this.rawData.Tanks.Add(tankRawData);
            }
        }

        private void ReadList(StreamReader reader, TankRawData tankRawData)
        {
            var lineColCount = 2 + tankRawData.CountOfVInclination;

            for (int hInclinationIndex = 0; hInclinationIndex < tankRawData.CountOfHInclination; hInclinationIndex++)
            {
                // line 1 offset = h-inclination
                var hInclination = reader.ReadLine().Trim().TryToDecimal();
                this.OnWriteLog(TraceLevel.Info, String.Format("H-inclination: {0}", hInclination));

                // line 2 offset = v-inclination columns
                var vInclinationsLine = reader.ReadLine();
                var vInclinationStrings = vInclinationsLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (vInclinationStrings.Length != tankRawData.CountOfVInclination)
                {
                    throw new FormatException(String.Format("Expected column count is {0}, but actually {1}. Line: {2}", tankRawData.CountOfVInclination, vInclinationStrings.Length, vInclinationsLine));
                }
                var vInclinations = vInclinationStrings.Select(s => s.TryToDecimal()).ToArray();
                this.OnWriteLog(TraceLevel.Info, String.Format("V-inclination data: {0}", vInclinationsLine));

                // from line 3 offset = [heigh, ullage, vol(v-inclination 1), ..., vol(v-inclination n)]
                this.OnWriteLog(TraceLevel.Info, String.Format("Reading volume data for H-inclination: {0} ...", hInclination));
                int lineIndex = 0;
                for (; lineIndex < tankRawData.CountOfHeightsPerHInclination; lineIndex++)
                {
                    // from line 3 offset = [heigh, ullage, vol(v-inclination 1), ..., vol(v-inclination n)]
                    var line = reader.ReadLine();
                    var items = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length != lineColCount)
                    {
                        throw new FormatException(String.Format("Expected data column count is {0}, but actually {1}. Line: {2}", lineColCount, items.Length, line));
                    }

                    var height = items[0].TryToDecimal();
                    var ullage = items[1].TryToDecimal();
                    if (height + ullage != tankRawData.HeightOfTank)
                    {
                        throw new FormatException(String.Format("Expected height({0}) plus ullage({1}) is {2}", height, ullage, tankRawData.HeightOfTank));
                    }

                    // add volume record for each colume
                    for (int colIndex = 0; colIndex < tankRawData.CountOfVInclination; colIndex++)
                    {
                        tankRawData.VolumeItems.Add(new OilVolume
                        {
                            TankName = tankRawData.TankName,
                            HInclination = hInclination,
                            VInclination = vInclinations[colIndex],
                            Height = height,
                            Volume = items[colIndex + 2].TryToDecimal()
                        });
                    }
                }
                this.OnWriteLog(TraceLevel.Info, String.Format("{0} lines of volume data has been read", lineIndex));
            }
        }

        private TankRawData ReadHeader(StreamReader reader, string fileName)
        {
            TankRawData tankRawData;

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
                tankRawData = new TankRawData { TankName = tankName };
                this.OnWriteLog(TraceLevel.Info, String.Format("Tank name: {0}", tankName));
            }

            // line 4 = tank raw data (count of height values per h-inclination, unknown value, count of v-inclination, count of h-inclination, height of tank)
            var tankRawDataText = reader.ReadLine();
            var tankRawDataSubItems = tankRawDataText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tankRawDataSubItems.Length != 5)
            {
                throw new FormatException(String.Format("Incorrect tank raw data, there should be 5-digit tank information. File: \"{0}\", Line: 5", fileName));
            }
            tankRawData.CountOfHeightsPerHInclination = tankRawDataSubItems[0].ToInt32();
            tankRawData.CountOfVInclination = tankRawDataSubItems[2].ToInt32();
            tankRawData.CountOfHInclination = tankRawDataSubItems[3].ToInt32();
            tankRawData.HeightOfTank = tankRawDataSubItems[4].TryToDecimal();
            this.OnWriteLog(TraceLevel.Info, String.Format("Tank raw data: CountOfHeightsPerHInclination={0}, CountOfVInclination={1}, CountOfHInclination={2}, HeightOfTank={3}",
                tankRawData.CountOfHeightsPerHInclination,
                tankRawData.CountOfVInclination,
                tankRawData.CountOfHInclination,
                tankRawData.HeightOfTank));

            // if vessel is correct, and tank is not duplicated, return the read tank raw data
            return tankRawData;
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
