using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingMeasure.Core.Models;
using System.Collections.Generic;

namespace ShippingMeasure.Core.Tests
{
    [TestClass]
    public class StandardDensityTests
    {
        [TestMethod]
        public void VerifyThatStandardDensityInterpolationValueShouldBeBetweenUpperAndLower()
        {
            var standardDensities = new []
            {
                new StandardDensity { Temp = 1m, DensityMeasured = 10m, DensityOfStandard = 100m },
                new StandardDensity { Temp = 2m, DensityMeasured = 20m, DensityOfStandard = 200m }
            };

            var actual = standardDensities.GetValue(1.3m, 15m);

            Assert.AreEqual(140m, actual);
        }

        [TestMethod]
        public void VerifyThatStandardDensityInterpolationValueShouldBeBetween4UpperAndLowerItems()
        {
            var standardDensities = new[]
            {
                new StandardDensity { Temp = 4m, DensityMeasured = 40m, DensityOfStandard = 400m },
                new StandardDensity { Temp = 5m, DensityMeasured = 40m, DensityOfStandard = 450m },
                new StandardDensity { Temp = 4m, DensityMeasured = 50m, DensityOfStandard = 450m },
                new StandardDensity { Temp = 5m, DensityMeasured = 50m, DensityOfStandard = 500m }
            };

            var actual = standardDensities.GetValue(4.2m, 43m);

            Assert.AreEqual(425m, actual);
        }
    }
}
