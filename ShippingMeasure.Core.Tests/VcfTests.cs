using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingMeasure.Core.Models;

namespace ShippingMeasure.Core.Tests
{
    [TestClass]
    public class VcfTests
    {
        [TestMethod]
        public void VerifyThatVcfInterpolationValueShouldBeBetweenUpperAndLower()
        {
            var vcfItems = new[]
            {
                new VolumeCorrectionFactor { Temp = 1m, DensityOfStandard = 10m, Factor = 100m },
                new VolumeCorrectionFactor { Temp = 2m, DensityOfStandard = 20m, Factor = 200m }
            };

            var actual = vcfItems.GetValue(1.3m, 15m);

            Assert.AreEqual(140m, actual);
        }

        [TestMethod]
        public void VerifyThatVcfInterpolationValueShouldBeBetween4UpperAndLowerItems()
        {
            var vcfItems = new[]
            {
                new VolumeCorrectionFactor { Temp = 4m, DensityOfStandard = 40m, Factor = 400m },
                new VolumeCorrectionFactor { Temp = 5m, DensityOfStandard = 40m, Factor = 450m },
                new VolumeCorrectionFactor { Temp = 4m, DensityOfStandard = 50m, Factor = 450m },
                new VolumeCorrectionFactor { Temp = 5m, DensityOfStandard = 50m, Factor = 500m }
            };

            var actual = vcfItems.GetValue(4.2m, 43m);

            Assert.AreEqual(425m, actual);
        }
    }
}
