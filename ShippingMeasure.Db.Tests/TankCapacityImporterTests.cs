using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ShippingMeasure.Db.Tests
{
    [TestClass]
    public class TankCapacityImporterTests
    {
        [TestMethod]
        public void Import()
        {
            var importer = new TankCapacityImporter { AccessFile = ".\\Data\\Data.mdb" };
            importer.RawDataFiles.AddRange(new[] { ".\\Data\\tank 01 raw data.txt", ".\\Data\\tank 02 raw data.txt" });

            importer.Import();
            var tankDb = new TankDb();
            var vessel = tankDb.GetVessel();
            var tanks = tankDb.GetAllTanks();
            var volItems = tankDb.GetAllOilVolumeItems();

            Assert.AreEqual("imported vessel 01", vessel.Name);
            Assert.AreEqual("imported cert 01", vessel.CertNo);
            Assert.IsTrue(tanks.Any(t => t.Name == "imported tank 01"));
            Assert.IsTrue(tanks.Any(t => t.Name == "imported tank 02"));
            Assert.AreEqual(27 + 8, volItems.Count());
            Assert.IsTrue(volItems.Any(v => v.TankName == "imported tank 01" && v.HInclination == -0.5m && v.VInclination == -0.6m && v.Height == 3m && v.Volume == 100m));
            Assert.IsTrue(volItems.Any(v => v.TankName == "imported tank 01" && v.HInclination == -0.5m && v.VInclination == 0m && v.Height == 1m && v.Volume == 99m));
            Assert.IsTrue(volItems.Any(v => v.TankName == "imported tank 01" && v.HInclination == 0m && v.VInclination == -0.3m && v.Height == 2m && v.Volume == 1099.2m));
            Assert.IsTrue(volItems.Any(v => v.TankName == "imported tank 01" && v.HInclination == 0.5m && v.VInclination == 0m && v.Height == 2m && v.Volume == 2099.3m));
            Assert.IsTrue(volItems.Any(v => v.TankName == "imported tank 02" && v.HInclination == -5m && v.VInclination == -6m && v.Height == 2m && v.Volume == 1m));
            Assert.IsTrue(volItems.Any(v => v.TankName == "imported tank 02" && v.HInclination == 0m && v.VInclination == -3m && v.Height == 1m && v.Volume == 8m));
            Assert.IsTrue(volItems.Any(v => v.TankName == "imported tank 02" && v.HInclination == 0m && v.VInclination == -3m && v.Height == 2m && v.Volume == 6m));
            Assert.IsTrue(!volItems.Any(v => v.TankName == "imported tank 02" && v.HInclination == 0m && v.VInclination == -3m && v.Height == 2m && v.Volume == 8m));
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileVesselNameDifferent()
        {
            this.VerifyThatShouldThrow(
                new[] { ".\\Data\\tank 03 raw data - different vessel name.txt", ".\\Data\\tank 04 raw data - different vessel name.txt" },
                "There are more than one vessel names defined in the raw files.");
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileTankDuplicated()
        {
            this.VerifyThatShouldThrow(
                new[] { ".\\Data\\tank 05 raw data - duplcated tank name.txt", ".\\Data\\tank 06 raw data - duplcated tank name.txt" },
                "Duplicated tank definition.");
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileCertNoDifferent()
        {
            this.VerifyThatShouldThrow(
                new[] { ".\\Data\\tank 07 raw data - different certno.txt", ".\\Data\\tank 08 raw data - different certno.txt" },
                "There are more than one certification numbers defined in the raw files.");
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileTankInfoNot5Digits()
        {
            this.VerifyThatShouldThrow(
                new[] { ".\\Data\\tank 09 raw data - tank info not 5 digits.txt" },
                "Incorrect tank raw data, there should be 5-digit tank information.");
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileVInclinationCountIsNotCorrecct()
        {
            this.VerifyThatShouldThrow(
                new[] { ".\\Data\\tank 10 raw data - incorrect vinclination column count.txt" },
                "Expected column count is 5, but actually 3.");
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileDataColumnCountIsNotCorrecct()
        {
            this.VerifyThatShouldThrow(
                new[] { ".\\Data\\tank 11 raw data - data column count incorrect.txt" },
                "Expected data column count is 5, but actually 4.");
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileHeightAndUllageAreNotCorrecct()
        {
            this.VerifyThatShouldThrow(
                new[] { ".\\Data\\tank 12 raw data - height and ullage incorrect.txt" },
                "Expected height(3) plus ullage(1) is 5");
        }

        private void VerifyThatShouldThrow(IEnumerable<string> rawDataFiles, string expectedPrefix)
        {
            var importer = new TankCapacityImporter { AccessFile = ".\\Data\\Data.mdb" };
            importer.RawDataFiles.AddRange(rawDataFiles);

            Exception actual = null;
            try
            {
                importer.Import();
            }
            catch (Exception ex)
            {
                actual = ex;
            }

            Assert.AreEqual(expectedPrefix, actual.InnerException.Message.Substring(0, expectedPrefix.Length));
        }
    }
}
