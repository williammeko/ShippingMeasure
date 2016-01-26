using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingMeasure.Common;
using System.Linq;
using ShippingMeasure.Core.Models;
using System.Collections.Generic;

namespace ShippingMeasure.Db.Tests
{
    [TestClass]
    public class TankDbTests
    {
        [TestMethod]
        public void ClearTanks()
        {
            var tankDb = new TankDb();

            tankDb.ClearTanks();

            var actual = tankDb.GetAllTanks();
            Assert.AreEqual(0, actual.Count());
            actual = new TankDb().GetAllTanks();
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void ClearOilVolumeItems()
        {
            var tankDb = new TankDb();

            tankDb.ClearOilVolumeItems();

            var actual = tankDb.GetAllOilVolumeItems();
            Assert.AreEqual(0, actual.Count());
            actual = new TankDb().GetAllOilVolumeItems();
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void SaveVessel()
        {
            // delete vessel info from Vessel table
            new QueryContext(Config.DataConnectionString, "DELETE * FROM Vessel").ExecuteNonQuery();

            // inserting scenario
            var tankDb = new TankDb();
            var vessel = new Vessel { Name = "vessel 01", CertNo = "certno 01 " };
            tankDb.SaveVessel(vessel);

            var actual = tankDb.GetVessel();
            Assert.AreEqual(vessel.Name, actual.Name);
            Assert.AreEqual(vessel.CertNo, actual.CertNo);
            actual = new TankDb().GetVessel();
            Assert.AreEqual(vessel.Name, actual.Name);
            Assert.AreEqual(vessel.CertNo, actual.CertNo);

            // updating scenario
            vessel = new Vessel { Name = "vessel 02", CertNo = "certno 02 " };
            tankDb.SaveVessel(vessel);

            actual = tankDb.GetVessel();
            Assert.AreEqual(vessel.Name, actual.Name);
            Assert.AreEqual(vessel.CertNo, actual.CertNo);
            actual = new TankDb().GetVessel();
            Assert.AreEqual(vessel.Name, actual.Name);
            Assert.AreEqual(vessel.CertNo, actual.CertNo);
        }

        [TestMethod]
        public void SavePipes()
        {
            var pipes = new List<Pipe>
            {
                new Pipe { Name = "pipe 01", Volume = 0.1m },
                new Pipe { Name = "pipe 02", Volume = 2m },
            };

            new TankDb().SaveVessel(new Vessel { Name = "vessel 01", CertNo = "cert 01" });
            new TankDb().SavePipes(pipes);
            var actual = new TankDb().GetVessel();

            Assert.AreEqual(2, actual.Pipes.Count());
            Assert.AreEqual(0.1m, actual.Pipes.First(p => p.Name == "pipe 01").Volume);
            Assert.AreEqual(2m, actual.Pipes.First(p => p.Name == "pipe 02").Volume);
        }

        [TestMethod]
        public void AddTanks()
        {
            var tanks = new[]
            {
                new Tank { Name = "tank 01", Height = 11.1m },
                new Tank { Name = "tank 02", Height = 22.2m }
            };

            var tankDb = new TankDb();
            tankDb.ClearTanks();
            tankDb.Add(tanks);

            var actual = tankDb.GetAllTanks();
            Assert.IsTrue(actual.Any(t => t.Name == "tank 01"));
            Assert.IsTrue(actual.Any(t => t.Name == "tank 02"));
            actual = new TankDb().GetAllTanks();
            Assert.IsTrue(actual.Any(t => t.Name == "tank 01"));
            Assert.IsTrue(actual.Any(t => t.Name == "tank 02"));
            Assert.AreEqual(11.1m, actual.First(t => t.Name == "tank 01").Height);
            Assert.AreEqual(22.2m, actual.First(t => t.Name == "tank 02").Height);

            // add more
            tanks = new[]
            {
                new Tank { Name = "tank 03", Height = 33.3m }
            };
            tankDb.Add(tanks);
            actual = tankDb.GetAllTanks();
            Assert.AreEqual(3, actual.Count());
            Assert.IsTrue(actual.Any(t => t.Name == "tank 01"));
            Assert.IsTrue(actual.Any(t => t.Name == "tank 02"));
            Assert.IsTrue(actual.Any(t => t.Name == "tank 03"));
            Assert.AreEqual(33.3m, actual.First(t => t.Name == "tank 03").Height);
            actual = new TankDb().GetAllTanks();
            Assert.AreEqual(3, actual.Count());
            Assert.IsTrue(actual.Any(t => t.Name == "tank 01"));
            Assert.IsTrue(actual.Any(t => t.Name == "tank 02"));
            Assert.IsTrue(actual.Any(t => t.Name == "tank 03"));
            Assert.AreEqual(33.3m, actual.First(t => t.Name == "tank 03").Height);
        }

        [TestMethod]
        public void AddOilVolumeItems()
        {
            var items = new[]
            {
                new OilVolume { TankName = "tank 01", HInclination = 0.1m, VInclination = 0.2m, Height = 0.3m, Volume = 0.4m },
                new OilVolume { TankName = "tank 02", HInclination = 5m, VInclination = 0.6m, Height = 0.7m, Volume = 0.8m }
            };

            var tankDb = new TankDb();
            tankDb.ClearOilVolumeItems();
            tankDb.Add(items);

            var actual = tankDb.GetAllOilVolumeItems();
            Assert.AreEqual(2, actual.Count());
            var item = actual.First(t => t.TankName.Equals("tank 01"));
            Assert.AreEqual(0.4m, item.Volume);
            Assert.AreEqual(0.3m, item.Height);
            Assert.AreEqual(0.2m, item.VInclination);
            Assert.AreEqual(0.1m, item.HInclination);
            item = actual.First(t => t.TankName.Equals("tank 02"));
            Assert.AreEqual(0.8m, item.Volume);
            Assert.AreEqual(0.7m, item.Height);
            Assert.AreEqual(0.6m, item.VInclination);
            Assert.AreEqual(5m, item.HInclination);

            // add more
            items = new[]
            {
                new OilVolume { TankName = "tank 03", HInclination = 0.09m, VInclination = 10m, Height = 1.1m, Volume = 1200m }
            };
            tankDb.Add(items);
            actual = tankDb.GetAllOilVolumeItems();
            Assert.AreEqual(3, actual.Count());
            item = actual.First(t => t.TankName.Equals("tank 03"));
            Assert.AreEqual(1200m, item.Volume);
            Assert.AreEqual(1.1m, item.Height);
            Assert.AreEqual(10m, item.VInclination);
            Assert.AreEqual(0.09m, item.HInclination);
            actual = new TankDb().GetAllOilVolumeItems();
            Assert.AreEqual(3, actual.Count());
            item = actual.First(t => t.TankName.Equals("tank 03"));
            Assert.AreEqual(1200m, item.Volume);
            Assert.AreEqual(1.1m, item.Height);
            Assert.AreEqual(10m, item.VInclination);
            Assert.AreEqual(0.09m, item.HInclination);
        }
    }
}
