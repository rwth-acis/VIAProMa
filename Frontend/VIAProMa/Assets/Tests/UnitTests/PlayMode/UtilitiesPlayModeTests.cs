using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace Tests
{
    public class UtilitiesPlayModeTests
    {
        private GameObject testObject;
        private ConstantRotation constantRotation;

        private BoundingBoxStateController boundingBoxStateController;

        private FloatEffect floatEffect;

        private FollowObject followObject;

        private GameObject targetObject;

        private RandomColor randomColor;

        private GameObject arrayObject;
        private HorizontalObjectArray horizontalObjectArray;
        private GameObject arrayObject1;
        private GameObject arrayObject2;
        private GameObject arrayObject3;
        private GameObject arrayObject4;
        private HorizontalObjectArray horizontalObjectArray1;
        private HorizontalObjectArray horizontalObjectArray2;
        private HorizontalObjectArray horizontalObjectArray3;
        private HorizontalObjectArray horizontalObjectArray4;

        [SetUp]
        public void Setup()
        {
            testObject = GameObject.Instantiate(new GameObject());
            constantRotation = testObject.AddComponent<ConstantRotation>();

            testObject.AddComponent<BoundingBox>();
            testObject.AddComponent<BoxCollider>();
            boundingBoxStateController = testObject.AddComponent<BoundingBoxStateController>();

            floatEffect = testObject.AddComponent<FloatEffect>();

            targetObject = GameObject.Instantiate(new GameObject());
            followObject = testObject.AddComponent<FollowObject>();

            testObject.AddComponent<MeshRenderer>();

            arrayObject = GameObject.Instantiate(new GameObject());
            arrayObject = GameObject.Instantiate(new GameObject());
            horizontalObjectArray = arrayObject.AddComponent<HorizontalObjectArray>();
            arrayObject1 = GameObject.Instantiate(new GameObject());
            arrayObject2 = GameObject.Instantiate(new GameObject());
            arrayObject3 = GameObject.Instantiate(new GameObject());
            arrayObject4 = GameObject.Instantiate(new GameObject());
            arrayObject1.AddComponent<BoundingBox>();
            arrayObject2.AddComponent<BoundingBox>();
            arrayObject3.AddComponent<BoundingBox>();
            arrayObject4.AddComponent<BoundingBox>();
            horizontalObjectArray1 = arrayObject1.AddComponent<HorizontalObjectArray>();
            horizontalObjectArray2 = arrayObject2.AddComponent<HorizontalObjectArray>();
            horizontalObjectArray3 = arrayObject3.AddComponent<HorizontalObjectArray>();
            horizontalObjectArray4 = arrayObject4.AddComponent<HorizontalObjectArray>();
        }

        [UnityTest]
        public IEnumerator ConstantRotation()
        {
            // Arrange
            Vector3 actualRotation = testObject.transform.localEulerAngles;

            // Use yield to skip a frame.
            yield return new WaitForSeconds(Time.deltaTime);

            // Use the Assert class to test conditions.
            Assert.IsTrue(actualRotation.y != 0.0f && actualRotation.x == 0.0f && actualRotation.z == 0.0f); // Check whether a y-axis rotation took place
        }

        [UnityTest]
        public IEnumerator BoundingBoxStateController_BoxCollider_true()
        {
             // Use yield to skip a frame.
            yield return null;

            // Use the Assert class to test conditions.
            Assert.IsTrue(boundingBoxStateController.BoundingBoxActive = true);
        }

        [UnityTest]
        public IEnumerator FloatEffect()
        {
            // Arrange
            testObject.transform.localPosition = new Vector3(0, 0, 0);

            // Use yield to skip a frame.
            yield return new WaitForSeconds(0.1f);

            // Use the Assert class to test conditions.
            Assert.AreNotEqual(floatEffect.StartPosition, floatEffect.transform.localPosition);
        }

        [UnityTest]
        public IEnumerator FollowObject()
        {
            // Arrange
            targetObject.transform.localPosition = new Vector3(4, 5, 6);
            followObject.target = targetObject.transform;

            // Use yield to skip a frame.
            yield return new WaitForSeconds(0.1f);

            // Use the Assert class to test conditions.
            Assert.AreEqual(followObject.target.localPosition, testObject.transform.localPosition);
        }

        [UnityTest]
        public IEnumerator RandomColor()
        {
            // Arrange
            Renderer rend = testObject.GetComponent<Renderer>();
            Color initialColor = rend.material.color;

            // Use yield to skip a frame.
            randomColor = testObject.AddComponent<RandomColor>();
            yield return new WaitForSeconds(0.1f);
            Color finalColor = rend.material.color;

            // Use the Assert class to test conditions.
            Assert.AreNotEqual(initialColor, finalColor);
        }

        [UnityTest]
        public IEnumerator HorizontalObjectArray_SingleObject()
        {
            // Arrange
            Vector3 initialPosition = arrayObject1.transform.position;
            horizontalObjectArray1.Collection = new GameObject[] { arrayObject1 };

            // Use yield to skip a frame.
            yield return new WaitForSeconds(Time.deltaTime);
            Vector3 finalPosition = arrayObject1.transform.position;

            // Use the Assert class to test conditions.
            //Debug.Log("Expected: " + initialPosition.y + "; actual: " + finalPosition.y);
            Assert.AreEqual(initialPosition, finalPosition); // If only one object is passed, its coordinates do not change
        }

        [UnityTest]
        public IEnumerator HorizontalObjectArray_MultipleObjectsOdd_CentalObjectAsArray() // Any other object from the array would not work
        {
            // Arrange
            Vector3 initialPosition1 = arrayObject1.transform.position;
            Vector3 initialPosition2 = arrayObject2.transform.position;
            Vector3 initialPosition3 = arrayObject3.transform.position;
            horizontalObjectArray2.Collection = new GameObject[] { arrayObject1, arrayObject2, arrayObject3 };

            // Use yield to skip a frame.
            yield return new WaitForSeconds(Time.deltaTime);
            Vector3 finalPosition1 = arrayObject1.transform.position;
            Vector3 finalPosition2 = arrayObject2.transform.position;
            Vector3 finalPosition3 = arrayObject3.transform.position;

            // Use the Assert class to test conditions.
            Assert.AreNotEqual(initialPosition1, finalPosition1);
            Assert.AreEqual(initialPosition2, finalPosition2);
            Assert.AreNotEqual(initialPosition3, finalPosition3);
            Assert.AreEqual(finalPosition1.x, -finalPosition3.x);
            //Assert.IsTrue(initialPosition1 == finalPosition1 && initialPosition2 == finalPosition2);
        }

        [UnityTest]
        public IEnumerator HorizontalObjectArray_MultipleObjectsOdd()
        {
            // Arrange
            Vector3 initialPosition1 = arrayObject1.transform.position;
            Vector3 initialPosition2 = arrayObject2.transform.position;
            Vector3 initialPosition3 = arrayObject3.transform.position;
            horizontalObjectArray.Collection = new GameObject[] { arrayObject1, arrayObject2, arrayObject3 };

            // Use yield to skip a frame.
            yield return new WaitForSeconds(Time.deltaTime);
            Vector3 finalPosition1 = arrayObject1.transform.position;
            Vector3 finalPosition2 = arrayObject2.transform.position;
            Vector3 finalPosition3 = arrayObject3.transform.position;

            // Use the Assert class to test conditions.
            Assert.AreNotEqual(initialPosition1, finalPosition1);
            Assert.AreEqual(initialPosition2, finalPosition2);
            Assert.AreNotEqual(initialPosition3, finalPosition3);
            Assert.AreEqual(finalPosition1.x, -finalPosition3.x);
        }

        [UnityTest]
        public IEnumerator HorizontalObjectArray_MultipleObjectsEven()
        {
            // Arrange
            Vector3 initialPosition1 = arrayObject1.transform.position;
            Vector3 initialPosition2 = arrayObject2.transform.position;
            Vector3 initialPosition3 = arrayObject3.transform.position;
            Vector3 initialPosition4 = arrayObject4.transform.position;
            horizontalObjectArray.Collection = new GameObject[] { arrayObject1, arrayObject2, arrayObject3, arrayObject4 };

            // Use yield to skip a frame.
            yield return new WaitForSeconds(Time.deltaTime);
            Vector3 finalPosition1 = arrayObject1.transform.position;
            Vector3 finalPosition2 = arrayObject2.transform.position;
            Vector3 finalPosition3 = arrayObject3.transform.position;
            Vector3 finalPosition4 = arrayObject4.transform.position;

            // Use the Assert class to test conditions.
            Assert.AreNotEqual(initialPosition1, finalPosition1);
            Assert.AreNotEqual(initialPosition2, finalPosition2);
            Assert.AreNotEqual(initialPosition3, finalPosition3);
            Assert.AreNotEqual(initialPosition4, finalPosition4);
            Assert.AreEqual(finalPosition1.x, -finalPosition4.x);
            Assert.AreEqual(finalPosition2.x, -finalPosition3.x);
        }

        /* Template
        [UnityTest]
        public IEnumerator ScriptName_FunctionName_TestName()
        {
            // Arrange

            // Use yield to skip a frame.
            yield return new WaitForSeconds(Time.deltaTime);

            // Use the Assert class to test conditions.
        }
        */

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(testObject);
            GameObject.Destroy(targetObject);
            GameObject.Destroy(arrayObject);
            GameObject.Destroy(arrayObject1);
            GameObject.Destroy(arrayObject2);
            GameObject.Destroy(arrayObject3);
        }
    }
}
