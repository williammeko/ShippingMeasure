using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingMeasure.Core.Models;
using System.Collections.Generic;
using System.Linq;
using ShippingMeasure.Core;
using ShippingMeasure.Common;

namespace ShippingMeasure.Db.Tests
{
    [TestClass]
    public class ReceiptDbTests
    {
        [TestMethod]
        public void Save()
        {
            var receipt = this.GetTestReceipts()[0];
            var receiptDb = new ReceiptDb();

            receiptDb.Save(receipt);

            var actual = receiptDb.Get(receipt.No);
            this.AssertAreEqual(receipt, actual);
            Assert.AreEqual(receipt.ReceiptTankDetails.Count, actual.ReceiptTankDetails.Count);
            receipt.ReceiptTankDetails.ForEach(d => this.AssertAreEqual(d, actual.ReceiptTankDetails.First(actualDetail => actualDetail.TankName.Equals(d.TankName))));
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileReceiptNoIsEmpty()
        {
            // empty.
            var receipt = this.GetTestReceipts()[0];
            receipt.No = " ";
            this.AssertHasException<ArgumentException>("Receipt No. cannot be empty", () => new ReceiptDb().Save(receipt));

            // null
            receipt = this.GetTestReceipts()[0];
            receipt.No = null;
            this.AssertHasException<ArgumentException>("Receipt No. cannot be empty", () => new ReceiptDb().Save(receipt));
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileTankNameIsEmpty()
        {
            // empty.
            var receipt = this.GetTestReceipts()[0];
            receipt.ReceiptTankDetails.Last().TankName = "	 	 	 	";
            this.AssertHasException<ArgumentException>("Receipt tank detail: TankName cannot be empty", () => new ReceiptDb().Save(receipt));

            // null
            receipt = this.GetTestReceipts()[0];
            receipt.ReceiptTankDetails.Last().TankName = null;
            this.AssertHasException<ArgumentException>("Receipt tank detail: TankName cannot be empty", () => new ReceiptDb().Save(receipt));
        }

        [TestMethod]
        public void RemoveReceipt()
        {
            var receipt = this.GetTestReceipts()[0];
            var receiptDb = new ReceiptDb();

            receiptDb.Save(receipt);
            receiptDb.Remove(receipt.No);

            Assert.IsFalse(receiptDb.GetAll().Any(r => r.No.Equals(receipt.No)));
            Assert.IsFalse(new ReceiptDb().GetAll().Any(r => r.No.Equals(receipt.No)));
        }

        [TestMethod]
        public void AddReceipt()
        {
            var receipts = this.GetTestReceipts();
            var kindsOfGoods = new[] { new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = "kind 01", Customized = true }, new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = "kind 02", Customized = true } };
            receipts[0].KindOfGoods = kindsOfGoods[0];
            receipts[1].KindOfGoods = kindsOfGoods[1];
            var receiptDb = new ReceiptDb();

            kindsOfGoods.Each(k => receiptDb.Save(k));
            receiptDb.GetAll().Select(r => r.No).ToList().ForEach(n => receiptDb.Remove(n));

            // add one
            receiptDb.Save(receipts[0]);
            Assert.AreEqual(1, receiptDb.GetAll().Count());
            Assert.AreEqual(1, new ReceiptDb().GetAll().Count());
            var actual = receiptDb.GetAll().First();
            this.AssertAreEqual(receipts[0], actual);
            actual = new ReceiptDb().GetAll().First();
            this.AssertAreEqual(receipts[0], actual);
            actual = new ReceiptDb().Get(receipts[0].No);
            this.AssertAreEqual(receipts[0], actual);
            Assert.AreNotEqual(null, actual.KindOfGoods);
            Assert.AreEqual(receipts[0].ReceiptTankDetails.Count, actual.ReceiptTankDetails.Count);
            receipts[0].ReceiptTankDetails.ForEach(d => this.AssertAreEqual(d, actual.ReceiptTankDetails.First(actualDetail => actualDetail.TankName.Equals(d.TankName))));

            // add another
            receiptDb.Save(receipts[1]);
            Assert.AreEqual(2, receiptDb.GetAll().Count());
            Assert.AreEqual(2, new ReceiptDb().GetAll().Count());
            actual = receiptDb.GetAll().First(r => r.No.Equals(receipts[1].No));
            this.AssertAreEqual(receipts[1], actual);
            actual = new ReceiptDb().GetAll().First(r => r.No.Equals(receipts[1].No));
            this.AssertAreEqual(receipts[1], actual);
            actual = new ReceiptDb().Get(receipts[1].No);
            this.AssertAreEqual(receipts[1], actual);
            Assert.AreNotEqual(null, actual.KindOfGoods);
            Assert.AreEqual(receipts[1].ReceiptTankDetails.Count, actual.ReceiptTankDetails.Count);
            receipts[1].ReceiptTankDetails.ForEach(d => this.AssertAreEqual(d, actual.ReceiptTankDetails.First(actualDetail => actualDetail.TankName.Equals(d.TankName))));
        }

        [TestMethod]
        public void UpdateReceipt()
        {
            var receipt = this.GetTestReceipts()[0];
            var receiptDb = new ReceiptDb();

            receiptDb.Save(receipt);
            receipt.AgentName = "AgentName changed";
            receipt.TotalOfMass = 10101.010101m;
            receiptDb.Save(receipt);

            var actual = receiptDb.GetAll().First(r => r.No.Equals(receipt.No));
            Assert.AreEqual(receipt.AgentName, actual.AgentName);
            Assert.AreEqual(receipt.TotalOfMass, actual.TotalOfMass);
            actual = new ReceiptDb().GetAll().First(r => r.No.Equals(receipt.No));
            Assert.AreEqual(receipt.AgentName, actual.AgentName);
            Assert.AreEqual(receipt.TotalOfMass, actual.TotalOfMass);
            actual = new ReceiptDb().Get(receipt.No);
            Assert.AreEqual(receipt.AgentName, actual.AgentName);
            Assert.AreEqual(receipt.TotalOfMass, actual.TotalOfMass);
        }

        [TestMethod]
        public void Exists()
        {
            var receipt = this.GetTestReceipts()[0];
            var receiptDb = new ReceiptDb();

            // exists
            receiptDb.Save(receipt);
            var actual = new ReceiptDb().Exists(receipt.No);
            Assert.AreEqual(true, actual);

            // not exist
            receiptDb.Remove(receipt.No);
            actual = new ReceiptDb().Exists(receipt.No);
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void GetReceiptTankDetails()
        {
            var receipt = this.GetTestReceipts()[0];
            var receiptDb = new ReceiptDb();

            receiptDb.Save(receipt);
            var actual = new ReceiptDb().GetReceiptTankDetails(receipt.No);

            Assert.AreEqual(receipt.ReceiptTankDetails.Count(), actual.Count());
            this.AssertAreEqual(receipt.ReceiptTankDetails[0], actual.First(d => d.TankName.Equals(receipt.ReceiptTankDetails[0].TankName)));
            this.AssertAreEqual(receipt.ReceiptTankDetails[1], actual.First(d => d.TankName.Equals(receipt.ReceiptTankDetails[1].TankName)));
        }

        [TestMethod]
        public void SaveKindOfGoods()
        {
            var kindOfGoods = new KindOfGoods { UId = "kind 01", Name = "kind 01", Customized = true };
            var receiptDb = new ReceiptDb();

            receiptDb.Save(kindOfGoods);

            var actual = receiptDb.GetAllKindsOfGoods().First(k => k.UId.Equals(kindOfGoods.UId));
            this.AssertAreEqual(kindOfGoods, actual);
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileKindOfGoodsIsEmpty()
        {
            // empty.
            var kindOfGoods = new KindOfGoods { UId = "kind 01", Name = "				", Customized = true };
            this.AssertHasException<ArgumentException>("Kind name/id cannot be empty", () => new ReceiptDb().Save(kindOfGoods));

            // empty.
            kindOfGoods = new KindOfGoods { UId = "				", Name = "kind 01", Customized = true };
            this.AssertHasException<ArgumentException>("Kind name/id cannot be empty", () => new ReceiptDb().Save(kindOfGoods));

            // null
            kindOfGoods = new KindOfGoods { UId = "kind 01", Name = null, Customized = true };
            this.AssertHasException<ArgumentException>("Kind name/id cannot be empty", () => new ReceiptDb().Save(kindOfGoods));

            // null
            kindOfGoods = new KindOfGoods { UId = null, Name = "kind 01", Customized = true };
            this.AssertHasException<ArgumentException>("Kind name/id cannot be empty", () => new ReceiptDb().Save(kindOfGoods));
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileKindOfGoodsIsBuiltIn()
        {
            var kindOfGoods = new KindOfGoods { UId = "kind 01", Name = "kind 01", BuiltIn = true, Customized = true };

            this.AssertHasException<ArgumentException>("Built-in kind cannot be saved", () => new ReceiptDb().Save(kindOfGoods));
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileSavingBuiltInKindOfGoods()
        {
            var kindOfGoods = Const.BuiltInKindsOfGoods.First();

            this.AssertHasException<ArgumentException>(
                String.Format("Built-in kind \"{0}\" cannot be saved", kindOfGoods.Name),
                () => new ReceiptDb().Save(kindOfGoods));
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileKindOfGoodsIsNotCustomized()
        {
            var kindOfGoods = new KindOfGoods { UId = "kind 01", Name = "kind 01", BuiltIn = false, Customized = false };

            this.AssertHasException<ArgumentException>("Only customized kinds can be saved", () => new ReceiptDb().Save(kindOfGoods));
        }

        [TestMethod]
        public void RemoveKindOfGoods()
        {
            var kindOfGoods = new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = "kind 01", Customized = true };
            var receiptDb = new ReceiptDb();

            receiptDb.Save(kindOfGoods);
            receiptDb.RemoveKindOfGoods(kindOfGoods.UId);

            Assert.IsFalse(receiptDb.GetAllKindsOfGoods().Any(k => k.UId.Equals(kindOfGoods.UId)));
            Assert.IsFalse(new ReceiptDb().GetAllKindsOfGoods().Any(k => k.UId.Equals(kindOfGoods.UId)));
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileRemovingAnInUsedKind()
        {
            var kindOfGoods = new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = "kind 01", Customized = true };
            var receiptDb = new ReceiptDb();
            receiptDb.Save(kindOfGoods);
            var receipt = this.GetTestReceipts()[0];
            receipt.KindOfGoods = kindOfGoods;
            receiptDb.Save(receipt);

            this.AssertHasException<InvalidOperationException>(String.Format("The kind [{0}] is currently in used, cannot be removed", kindOfGoods.UId), () => receiptDb.RemoveKindOfGoods(kindOfGoods.UId));
        }

        [TestMethod]
        public void AddKindOfGoods()
        {

            // todo. test case run failed.
            var kindsOfGoods = new[]
            {
                new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = "added kind 01", Customized = true },
                new KindOfGoods { UId = Guid.NewGuid().ToString(), Name = "added kind 02", Customized = true }
            };
            var receiptDb = new ReceiptDb();

            try
            {
                receiptDb.GetAllKindsOfGoods().Where(k => k.Name.StartsWith("added kind ")).Select(k => k.UId).ToList().ForEach(u => receiptDb.RemoveKindOfGoods(u));
            }
            catch
            {
            }

            // add one
            receiptDb.Save(kindsOfGoods[0]);
            Assert.AreEqual(1, receiptDb.GetAllKindsOfGoods().Where(k => k.Name.StartsWith("added kind ")).Count());
            Assert.AreEqual(1, new ReceiptDb().GetAllKindsOfGoods().Where(k => k.Name.StartsWith("added kind ")).Count());
            var actual = receiptDb.GetAllKindsOfGoods().First(k => k.UId.Equals(kindsOfGoods[0].UId));
            this.AssertAreEqual(kindsOfGoods[0], actual);
            actual = new ReceiptDb().GetAllKindsOfGoods().First(k => k.UId.Equals(kindsOfGoods[0].UId));
            this.AssertAreEqual(kindsOfGoods[0], actual);

            // add another
            receiptDb.Save(kindsOfGoods[1]);
            Assert.AreEqual(2, receiptDb.GetAllKindsOfGoods().Where(k => k.Name.StartsWith("added kind ")).Count());
            Assert.AreEqual(2, new ReceiptDb().GetAllKindsOfGoods().Where(k => k.Name.StartsWith("added kind ")).Count());
            actual = receiptDb.GetAllKindsOfGoods().First(k => k.UId.Equals(kindsOfGoods[1].UId));
            this.AssertAreEqual(kindsOfGoods[1], actual);
            actual = new ReceiptDb().GetAllKindsOfGoods().First(k => k.UId.Equals(kindsOfGoods[1].UId));
            this.AssertAreEqual(kindsOfGoods[1], actual);
        }

        [TestMethod]
        public void UpdateKindOfGoods()
        {
            var kindOfGoods = new KindOfGoods { UId = "kind 01", Name = "kind 01", Customized = true };
            var receiptDb = new ReceiptDb();

            receiptDb.Save(kindOfGoods);
            kindOfGoods.Name = "kind 01 changed";
            receiptDb.Save(kindOfGoods);

            var actual = receiptDb.GetAllKindsOfGoods().First(k => k.UId.Equals(kindOfGoods.UId));
            Assert.AreEqual(kindOfGoods.Name, actual.Name);
            actual = new ReceiptDb().GetAllKindsOfGoods().First(k => k.UId.Equals(kindOfGoods.UId));
            Assert.AreEqual(kindOfGoods.Name, actual.Name);
        }

        [TestMethod]
        public void VerifyThatVolumeOfHeightShouldBeSaved()
        {
            var receiptDb = new ReceiptDb();

            // VolumeByHeight is true
            var receipt = this.GetTestReceipts()[0];
            receipt.ReceiptTankDetails.ForEach(d => d.VolumeByHeight = true);
            receiptDb.Save(receipt);
            var actual = new ReceiptDb().GetReceiptTankDetails(receipt.No);
            actual.Each(d => Assert.AreEqual(true, d.VolumeByHeight));

            // VolumeByHeight is false
            receipt = this.GetTestReceipts()[1];
            receipt.ReceiptTankDetails.ForEach(d => d.VolumeByHeight = false);
            receiptDb.Save(receipt);
            actual = new ReceiptDb().GetReceiptTankDetails(receipt.No);
            actual.Each(d => Assert.AreEqual(false, d.VolumeByHeight));
        }

        private List<Receipt> GetTestReceipts()
        {
            var list = new List<Receipt>
            {
                new Receipt
                {
                    No = "receipt 01",
                    VesselName = "vessel 01",
                    Time = DateTime.Now,
                    ReceiptFor = "Load",
                    PortOfShipment = "port of shipment 01",
                    PortOfDestination = "port of destination 01",
                    KindOfGoods = null,
                    VesselStatus = VesselStatus.Parse("p:1/5-3=2"),
                    TotalOfVolumeOfStandard = 0.1m,
                    TotalOfVolume = 0.2m,
                    TotalOfVolumeOfWater = 0.3m,
                    TotalOfMass = 0.4m,
                    TotalOfVolumeOfPipes = 0.5m,
                    OperaterName = "OperaterName 01",
                    AgentName = "AgentName 01",
                    ShipperName = "ShipperName 01",
                    ConsignerName = "ConsignerName 01",
                    ConsigneeName = "ConsigneeName 01",
                    ReceiptType = ReceiptType.DeliveryReceiptDestination
                },
                new Receipt
                {
                    No = "receipt 02",
                    VesselName = "vessel 02",
                    Time = DateTime.Now,
                    ReceiptFor = "Load",
                    PortOfShipment = "port of shipment 02",
                    PortOfDestination = "port of destination 02",
                    KindOfGoods = null,
                    VesselStatus = VesselStatus.Parse("S:-.1/-1--0.8=-.2"),
                    TotalOfVolumeOfStandard = 10.1m,
                    TotalOfVolume = 10.2m,
                    TotalOfVolumeOfWater = 10.3m,
                    TotalOfMass = 10.4m,
                    TotalOfVolumeOfPipes = 10.5m,
                    OperaterName = "OperaterName 02",
                    AgentName = "AgentName 02",
                    ShipperName = "ShipperName 02",
                    ConsignerName = "ConsignerName 02",
                    ConsigneeName = "ConsigneeName 02",
                    ReceiptType = ReceiptType.MassOfOil
                }
            };

            list[0].ReceiptTankDetails.AddRange(this.GetTestReceiptTankDetails());
            list[1].ReceiptTankDetails.AddRange(this.GetTestReceiptTankDetails());

            return list;
        }

        private IEnumerable<ReceiptTankDetail> GetTestReceiptTankDetails()
        {
            return new List<ReceiptTankDetail>
            {
                new ReceiptTankDetail
                {
                    TankName = "tank 01",
                    VolumeByHeight = true,
                    Height = 01m,
                    TemperatureOfTank =  02m,
                    HeightOfWater =  03m,
                    TemperatureMeasured = 04m,
                    DensityMeasured =  05m,
                    DensityOfStandard =  06m,
                    Vcf20 = 07m,
                    Volume = 08m,
                    VolumeOfWater = 09m,
                    VolumeOfStandard = 10m,
                    Mass = 11m,
                },
                new ReceiptTankDetail
                {
                    TankName = "tank 02",
                    VolumeByHeight = true,
                    Height = 101m,
                    TemperatureOfTank =  102m,
                    HeightOfWater =  103m,
                    TemperatureMeasured = 104m,
                    DensityMeasured =  105m,
                    DensityOfStandard =  106m,
                    Vcf20 = 107m,
                    Volume = 108m,
                    VolumeOfWater = 109m,
                    VolumeOfStandard = 110m,
                    Mass = 111m,
                }
            };
        }

        private void AssertAreEqual(Receipt expected, Receipt actual)
        {
            Assert.AreEqual(expected.No, actual.No);
            Assert.AreEqual(expected.VesselName, actual.VesselName);
            Assert.AreEqual(expected.Time.Value.ToString("yyyy-MM-dd HH:mm:ss"), actual.Time.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.AreEqual(expected.ReceiptFor, actual.ReceiptFor);
            Assert.AreEqual(expected.PortOfShipment, actual.PortOfShipment);
            Assert.AreEqual(expected.PortOfDestination, actual.PortOfDestination);
            Assert.AreEqual(expected.KindOfGoods, actual.KindOfGoods);
            Assert.AreEqual(expected.VesselStatus.ToString(), actual.VesselStatus.ToString());
            Assert.AreEqual(expected.TotalOfVolumeOfStandard, actual.TotalOfVolumeOfStandard);
            Assert.AreEqual(expected.TotalOfVolume, actual.TotalOfVolume);
            Assert.AreEqual(expected.TotalOfVolumeOfWater, actual.TotalOfVolumeOfWater);
            Assert.AreEqual(expected.TotalOfMass, actual.TotalOfMass);
            Assert.AreEqual(expected.TotalOfVolumeOfPipes, actual.TotalOfVolumeOfPipes);
            Assert.AreEqual(expected.OperaterName, actual.OperaterName);
            Assert.AreEqual(expected.AgentName, actual.AgentName);
            Assert.AreEqual(expected.ShipperName, actual.ShipperName);
            Assert.AreEqual(expected.ConsignerName, actual.ConsignerName);
            Assert.AreEqual(expected.ConsigneeName, actual.ConsigneeName);
            Assert.AreEqual(expected.ReceiptType, actual.ReceiptType);
        }

        private void AssertAreEqual(ReceiptTankDetail expected, ReceiptTankDetail actual)
        {
            Assert.AreEqual(expected.TankName, actual.TankName);
            Assert.AreEqual(expected.VolumeByHeight, actual.VolumeByHeight);
            Assert.AreEqual(expected.Height, actual.Height);
            Assert.AreEqual(expected.TemperatureOfTank, actual.TemperatureOfTank);
            Assert.AreEqual(expected.HeightOfWater, actual.HeightOfWater);
            Assert.AreEqual(expected.TemperatureMeasured, actual.TemperatureMeasured);
            Assert.AreEqual(expected.DensityMeasured, actual.DensityMeasured);
            Assert.AreEqual(expected.DensityOfStandard, actual.DensityOfStandard);
            Assert.AreEqual(expected.Vcf20, actual.Vcf20);
            Assert.AreEqual(expected.Volume, actual.Volume);
            Assert.AreEqual(expected.VolumeOfWater, actual.VolumeOfWater);
            Assert.AreEqual(expected.VolumeOfStandard, actual.VolumeOfStandard);
            Assert.AreEqual(expected.Mass, actual.Mass);
        }

        private void AssertAreEqual(KindOfGoods expected, KindOfGoods actual)
        {
            Assert.AreEqual(expected.UId, actual.UId);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected, actual);
        }

        private void AssertHasException<TException>(string expectedMessage, Action action)
        {
            Exception actual = null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                actual = ex;
            }

            Assert.AreEqual(typeof(TException), actual.GetType());
            Assert.AreEqual(expectedMessage, actual.Message);
        }
    }
}
