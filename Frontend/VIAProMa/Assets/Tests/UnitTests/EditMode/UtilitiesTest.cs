using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class UtilitiesTest
    {
        /// <summary>
        /// Tests the Utility function which removes special characters from strings
        /// </summary>
        [Test]
        public void StringUtilities_RemoveSpecialCharacters_Test()
        {
            const string input = "Hello\\-+^ World!1~/.,;:";
            const string expected = "Hello World1";
            string result = StringUtilities.RemoveSpecialCharacters(input);
            Assert.AreEqual(expected, result);
        }





        [Test]
        public void StringUtilities_ContainsAny_FittingKeywordTest()
        {
            // Arrange
            string word = "Rindfleischetikettierungsüberwachungsaufgabenübertragungsgesetz";
            string[] keywords = { "fleisch", "Gesetz", "Wachstum" };
            bool expected = true;

            // Act
            bool result = StringUtilities.ContainsAny(word, keywords);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void StringUtilities_ContainsAny_CaseSensitiveTest()
        {
            // Arrange
            string word = "Rindfleischetikettierungsüberwachungsaufgabenübertragungsgesetz";
            string[] keywords = { "Fleisch" };
            bool expected = true; // Technically, the keyword is contained, but not in capital

            // Act
            bool result = StringUtilities.ContainsAny(word, keywords);

            // Assert
            Assert.AreEqual(expected, result);
        }

        public void StringUtilities_ContainsAny_EmptyKeywordTest()
        {
            // Arrange
            string word = "Rindfleischetikettierungsüberwachungsaufgabenübertragungsgesetz";
            string[] keywords = { "" };
            bool expected = true;

            // Act
            bool result = StringUtilities.ContainsAny(word, keywords);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void StringUtilities_ContainsAny_EmptyKeywordsListTest()
        {
            // Arrange
            string word = "Rindfleischetikettierungsüberwachungsaufgabenübertragungsgesetz";
            string[] keywords = { };
            bool expected = false; // /Since there is not one keyword to be contained in the word

            // Act
            bool result = StringUtilities.ContainsAny(word, keywords);

            // Assert
            Assert.AreEqual(expected, result);
        }




        /*[Test]
        public void StringUtilities_SingletonScriptableObject()
        {
            // Arrange
            SingletonScriptableObject<ScriptableObject> first = new SingletonScriptableObject<ScriptableObject>();
            SingletonScriptableObject<ScriptableObject> second = SingletonScriptableObject<ScriptableObject>.CreateInstance();

            // Act

            // Assert
            Assert.IsNotNull(first);
        }*/




        // Colors can even be implicitly converted to and from Vector4, e.g.:
        //  Vector4 newV4 = new Color(0.3f, 0.4f, 0.6f);
        [Test]
        public void ConversionUtilities_Vector3ToColor_White()
        {
            // Arrange
            Vector3 vector = new Vector3(1, 1, 1);
            Color expected = new Color(1, 1, 1);

            // Act
            Color result = ConversionUtilities.Vector3ToColor(vector);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConversionUtilities_ColorToVector4_OutOfRange()
        {
            // Arrange
            Color color = new Color(-4.3f, 317, 0, -182);
            Vector4 expected = new Vector4(-4.3f, 317, 0, -182);

            // Act
            Vector4 result = ConversionUtilities.ColorToVector4(color);

            // Assert
            Assert.AreEqual(expected, result);
        }





        enum Season
        {
            Spring,
            [System.ComponentModel.Description("My favorite season!")] Summer,
            Autumn,
            Winter
        }

        [Test]
        public void EnumExtensions_GetDescription_Text()
        {
            // Arrange
            string expected = "My favorite season!";

            // Act
            string result = EnumExtensions.GetDescription(Season.Summer);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EnumExtensions_GetDescription_Empty()
        {
            // Arrange
            string expected = "";

            // Act
            string result = EnumExtensions.GetDescription(Season.Winter);

            // Assert
            Assert.AreEqual(expected, result);
        }




       /*[Test]
        public void JsonArrayUtility_FromJson_Empty()
        {
            // Arrange
            string json = "";
            string[] expected = { "" };

            // Act
            string[] result = JsonArrayUtility.FromJson<string>(json);
            Debug.Log(result);

            // Assert
            Assert.AreEqual(expected, result);
        }*/
        
    }
}
