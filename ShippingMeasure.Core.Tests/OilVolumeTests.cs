using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingMeasure.Core.Models;

namespace ShippingMeasure.Core.Tests
{
    [TestClass]
    public class OilVolumeTests
    {
        [TestMethod]
        public void VerifyThatOilVolumeInterpolationValueShouldBeBetweenUpperAndLower()
        {
            var tankName = "tank name 01";
            var oilVolumeItems = new[]
            {
                new OilVolume { TankName = tankName, HInclination = -3m, VInclination = 1m, Height = 20.06m, Volume = 7846.511m },
                new OilVolume { TankName = tankName, HInclination = -2.5m, VInclination = 2m, Height = 20.07m, Volume = 7859.365m },
            };

            var actual = oilVolumeItems.GetValue(tankName, -2.7m, 1.2m, 20.063m);

            Assert.AreEqual(7851.224m, Math.Round(actual, 3));
        }

        [TestMethod]
        public void VerifyThatOilVolumeInterpolationValueShouldBeBetween4UpperAndLowerItems()
        {
            var tankName = "tank name 01";
            var oilVolumeItems = new[]
            {
                new OilVolume { TankName = tankName, HInclination = -3m, VInclination = 1m, Height = 20.07m, Volume = 7848.782m },
                new OilVolume { TankName = tankName, HInclination = -3m, VInclination = 2m, Height = 20.07m, Volume = 7848.522m },
                new OilVolume { TankName = tankName, HInclination = -3m, VInclination = 1m, Height = 20.06m, Volume = 7846.511m },
                new OilVolume { TankName = tankName, HInclination = -3m, VInclination = 2m, Height = 20.06m, Volume = 7846.248m },

                new OilVolume { TankName = tankName, HInclination = -2.5m, VInclination = 1m, Height = 20.07m, Volume = 7859.675m },
                new OilVolume { TankName = tankName, HInclination = -2.5m, VInclination = 2m, Height = 20.07m, Volume = 7859.365m },
                new OilVolume { TankName = tankName, HInclination = -2.5m, VInclination = 1m, Height = 20.06m, Volume = 7857.445m },
                new OilVolume { TankName = tankName, HInclination = -2.5m, VInclination = 2m, Height = 20.06m, Volume = 7857.133m },
            };

            var actual = oilVolumeItems.GetValue(tankName, -2.7m, 1.2m, 20.063m);

            Assert.AreEqual(7853.687m, Math.Round(actual, 3));
        }
    }
}
