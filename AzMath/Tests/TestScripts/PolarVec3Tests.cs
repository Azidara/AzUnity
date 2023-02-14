using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AzMath;

namespace AzMathUnitTesting {
    public class PolarVec3Tests
    {
        public float tolerance_threshold = 0.001f;

        // A Test behaves as an ordinary method
        [Test]
        public void PolarVec3TestsSimplePasses()
        {
            Assert.Pass();
        }

        [Test]
        public void PolarVec3ConversionTests(){
            test_PolarVec3Conversion(new Vector3(0,0,0));
            test_PolarVec3Conversion(new Vector3(24,54,43));
            test_PolarVec3Conversion(new Vector3(-24, 3423,-23));
            test_PolarVec3Conversion(new Vector3(-23, -12, 46));
        }

        [Test]
        public void PolarVec3AdditionTests(){
            test_PolarVec3Addition(new Vector3 (23,45,-32), new Vector3(-31, -32, 45));
        }

        [Test]
        public void PolarVec3SubtractionTests(){
            test_PolarVec3Subtraction(new Vector3 (23, 45, 43), new Vector3(-31, -32, 23));
        }

        [Test]
        public void PolarVec3DirectionTests(){
            Assert.AreEqual(PolarVec3.zero, PolarVec3.CartesianToPolar(Vector3.zero));
            Assert.AreEqual(PolarVec3.forward, PolarVec3.CartesianToPolar(Vector3.forward));
            Assert.AreEqual(PolarVec3.backward, PolarVec3.CartesianToPolar(Vector3.back));
            Assert.AreEqual(PolarVec3.up, PolarVec3.CartesianToPolar(Vector3.up));
            Assert.AreEqual(PolarVec3.down, PolarVec3.CartesianToPolar(Vector3.down));
            Assert.AreEqual(PolarVec3.right, PolarVec3.CartesianToPolar(Vector3.right));
            Assert.AreEqual(PolarVec3.left, PolarVec3.CartesianToPolar(Vector3.left));

            /*
            Assert.AreEqual(Vector3.zero, PolarVec3.PolartoCartesian(PolarVec3.zero));
            Assert.AreEqual(Vector3.up, PolarVec3.PolartoCartesian(PolarVec3.up));
            Assert.AreEqual(Vector3.down, PolarVec3.PolartoCartesian(PolarVec3.down));
            Assert.AreEqual(Vector3.right, PolarVec3.PolartoCartesian(PolarVec3.right));
            Assert.AreEqual(Vector3.left, PolarVec3.PolartoCartesian(PolarVec3.left));
            */
        }

        #region Individual Test Functions

        public void test_PolarVec3Conversion(Vector3 testVec){
            //Debug.Log($"{testVec.ToString("F32")} : {PolarVec3.PolartoCartesian(PolarVec3.CartesianToPolar(testVec)).ToString("F32")}");
            Vector3 convertedVec = PolarVec3.PolartoCartesian(PolarVec3.CartesianToPolar(testVec));
            //Assert.AreEqual(testVec, convertedVec);
            Utility.VecAreEqual(testVec, convertedVec, tolerance_threshold);
        }
        public void test_PolarVec3Addition(Vector3 v1, Vector3 v2){
            Vector3 addedVec = PolarVec3.PolartoCartesian(PolarVec3.CartesianToPolar(v1) + PolarVec3.CartesianToPolar(v2));
            //Assert.AreEqual(v1 + v2, addedVec);
            Utility.VecAreEqual(v1 + v2, addedVec, tolerance_threshold);
        }
        public void test_PolarVec3Subtraction(Vector3 v1, Vector3 v2){
            Vector3 subtractedVec = PolarVec3.PolartoCartesian(PolarVec3.CartesianToPolar(v1) - PolarVec3.CartesianToPolar(v2));
            //Assert.AreEqual(v1 - v2, subtractedVec);
            Utility.VecAreEqual(v1 - v2, subtractedVec, tolerance_threshold);
        }

        #endregion
    }
}
