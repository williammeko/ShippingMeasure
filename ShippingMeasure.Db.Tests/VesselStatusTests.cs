using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingMeasure.Core;

namespace ShippingMeasure.Db.Tests
{
    [TestClass]
    public class VesselStatusTests
    {
        [TestMethod]
        public void Parse()
        {
            var actual = VesselStatus.Parse(" p : .1 / -5 - -3 = -2.0 ");

            Assert.AreEqual(HInclinationStatus.P, actual.HStatus);
            Assert.AreEqual(0.1m, actual.HValue);
            Assert.AreEqual(-5m, actual.AftDraft);
            Assert.AreEqual(-3m, actual.ForeDraft);
            Assert.AreEqual(-2m, actual.VValue);
        }

        [TestMethod]
        public void Create()
        {
            var actual = VesselStatus.Create("p", ".1", "-5", "-3", "-2.0");

            Assert.AreEqual(HInclinationStatus.P, actual.HStatus);
            Assert.AreEqual(0.1m, actual.HValue);
            Assert.AreEqual(-5m, actual.AftDraft);
            Assert.AreEqual(-3m, actual.ForeDraft);
            Assert.AreEqual(-2m, actual.VValue);
        }

        [TestMethod]
        public new void ToString()
        {
            var status = new VesselStatus(HInclinationStatus.S, -100000.1m, 2m, 1.99998m);

            var actual = status.ToString();

            Assert.AreEqual("S: -100000.1 / 2 - 1.99998 = 0.00002", actual);
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileFormatInvalid()
        {
            Exception actual = null;

            try
            {
                VesselStatus.Parse("k : .1 / 4 - 2 = 2");
            }
            catch (Exception ex)
            {
                actual = ex;
            }

            Assert.AreEqual(typeof(FormatException), actual.GetType());
            Assert.AreEqual(@"invalid status string to parse, argument: s = k : .1 / 4 - 2 = 2", actual.Message);
        }

        [TestMethod]
        public void VerifyThatShouldThrowWhileEquationIsWrong()
        {
            Exception actual = null;

            try
            {
                VesselStatus.Parse("p : .1 / 4 - 2 = 3");
            }
            catch (Exception ex)
            {
                actual = ex;
            }

            Assert.AreEqual(typeof(FormatException), actual.GetType());
            Assert.AreEqual(@"illegal equation, argument: s = p : .1 / 4 - 2 = 3", actual.Message);
        }
    }
}
