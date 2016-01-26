using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShippingMeasure.Db.Tests
{
    [TestClass]
    public class VcfDbTests
    {
        [TestMethod]
        public void GetAll()
        {
            var all = new VcfDb().GetAll();

            Assert.AreEqual(125893, all.Count());
            Assert.IsTrue(all.Any(f => f.Temp == -18m && f.DensityOfStandard == 654m && f.Factor == 1.0553m));
            Assert.IsTrue(all.Any(f => f.Temp == 150m && f.DensityOfStandard == 1074m && f.Factor == .9184m));
            Assert.IsTrue(all.Any(f => f.Temp == 52.25m && f.DensityOfStandard == 654m && f.Factor == .9517m));
        }
    }
}
