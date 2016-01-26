using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Security;

namespace ShippingMeasure.Common.Tests
{
    [TestClass]
    public class CommonHelperTests
    {
        [TestMethod]
        public void TestConsentInfoConfig()
        {
            var consentInfo = "[ConsentInfo]";

            Config.ConsentInfo = consentInfo;
            var actual = Config.ConsentInfo;

            Assert.AreEqual(consentInfo, actual);
        }

        [TestMethod]
        public void VerifyAesEncryption()
        {
            var plainText = "plain text";

            var actualEncryptedBytes = CommonHelper.EncryptStringToBytesWithAes(plainText);
            var actualDecrypedText = CommonHelper.DecryptStringFromBytesWithAes(actualEncryptedBytes);

            Assert.AreEqual("02f2de1f51a2bf286672f41373d7e3ea", actualEncryptedBytes.Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("x2"))).ToString());
            Assert.AreEqual(plainText, actualDecrypedText);
        }

        [TestMethod]
        public void VerifyMd5Encryption()
        {
            var plainText = "plain text".Aggregate(new SecureString(), (s, c) => { s.AppendChar(c); return s; });

            var actual = CommonHelper.SecureStringToMD5(plainText);

            Assert.AreEqual("31bc5c2b8fd4f20cd747347b7504a385", actual);
        }
    }
}
