using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShippingMeasure.Db.Tests
{
    [TestClass]
    public class StandardDensityDbTests
    {
        [TestMethod]
        public void GetAll()
        {
            var all = new StandardDensityDb().GetAll();

            Assert.AreEqual(120485, all.Count());
            var first = all.First();
            Assert.AreEqual(-18m, first.Temp);
            Assert.AreEqual(1015m, first.DensityMeasured);
            Assert.AreEqual(990.4m, first.DensityOfStandard);
            var last = all.Last();
            Assert.AreEqual(49.25m, last.Temp);
            Assert.AreEqual(1053m, last.DensityMeasured);
            Assert.AreEqual(1071.7m, last.DensityOfStandard);
        }
    }
}
