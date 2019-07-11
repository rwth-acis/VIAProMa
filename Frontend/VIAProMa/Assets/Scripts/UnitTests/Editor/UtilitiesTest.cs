using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class UtilitiesTest
    {
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
