﻿using Microsoft.MixedReality.Toolkit.UI;
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

        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(testObject);
            GameObject.Destroy(targetObject);
        }
    }
}
