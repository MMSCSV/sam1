using System;
using NUnit.Framework;

namespace CareFusion.Dispensing.Test
{
    [TestFixture(Category = "Unit")]
    public class StringExtensionsFixture
    {
        [Test]
        public void TrimControlCodesFromNull()
        {
            string nullString = null;

            string xmlString = nullString.TrimControlCodes();

            Assert.AreEqual(nullString, xmlString);
        }

        [Test]
        public void TrimControlCodesWithNoControlCodes()
        {
            string noControlCode = "01003094404910111715051510LA13B126AA";

            string result = noControlCode.TrimControlCodes();

            Assert.AreEqual(noControlCode, result);
        }

        [Test]
        public void TrimControlCodesWithASCII29()
        {
            string controlCodeString = "01003094404910111715051510LA13B126AA"; // ASCII 29 (x001D)

            string result = controlCodeString.TrimControlCodes();

            Assert.AreNotEqual(controlCodeString, result);
            Assert.AreEqual("01003094404910111715051510LA13B126AA", result);
        }

        [Test]
        public void TrimControlCodesWithASCII22()
        {
            string controlCodeString = "01003094404910111715051510LA13B126AA" + Convert.ToChar(22); // ASCII 22 (x0016)

            string result = controlCodeString.TrimControlCodes();

            Assert.AreNotEqual(controlCodeString, result);
            Assert.AreEqual("01003094404910111715051510LA13B126AA", result);
        }
    }
}
