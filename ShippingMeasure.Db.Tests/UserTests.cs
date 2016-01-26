using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security;
using ShippingMeasure.Common;

namespace ShippingMeasure.Db.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void Authorize()
        {
            var username = "SDARI";
            var password = "02164035681";

            var p = new SecureString();
            password.ToCharArray().Each(p.AppendChar);
            new UserDb().Save(username, p);
            var actual = new UserDb().Authorize(username, p);

            Assert.AreEqual(true, actual);

            password = "incorrect password";
            p = new SecureString();
            password.ToCharArray().Each(p.AppendChar);
            actual = new UserDb().Authorize(username, p);

            Assert.AreEqual(false, actual);
        }
    }
}
