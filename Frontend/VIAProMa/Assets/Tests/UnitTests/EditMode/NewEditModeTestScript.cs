using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System.Threading.Tasks;


namespace Tests
{
    public class NewEditModeTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public async void backendConnectorSaveLoadTest()
        {
            // Arrange
            string saveContent = "Hello, world!";
            string saveName = "BackendConnectorSaveLoadTest";
            string expectedString = saveContent;

            // Act + Assert

            bool saveResult = await BackendConnector.Save(saveName, saveContent);
            if (!saveResult)
            {
                TestContext.WriteLine("Save error ocurred.");
                Assert.Fail();
            }
            else
            {
                ApiResult<string> actualString = await BackendConnector.Load(saveName);
                Assert.AreEqual(expectedString, actualString);
            }
        }
    }
}
