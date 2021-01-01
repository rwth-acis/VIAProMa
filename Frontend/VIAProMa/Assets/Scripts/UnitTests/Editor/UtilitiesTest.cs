using i5.VIAProMa.Utilities;
using NUnit.Framework;

namespace i5.VIAProMa.UnitTests.Editor
{
    public class UtilitiesTest
    {
        /// <summary>
        /// Tests the Utility function which removes special characters from strings
        /// </summary>
        [Test]
        public void UtilitiesTestSimplePasses()
        {
            const string input = "Hello\\-+^ World~/.,;:";
            const string expected = "Hello World";
            string result = StringUtilities.RemoveSpecialCharacters(input);
            Assert.AreEqual(expected, result);
        }
    }
}
