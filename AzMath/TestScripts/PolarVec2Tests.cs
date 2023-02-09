using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AzMath;

namespace AzMathUnitTesting {
    public class PolarVec2Tests
    {
        public float tolerance_threshold = 0.001f;

        // A Test behaves as an ordinary method
        [Test]
        public void PolarVec2TestsSimplePasses()
        {
            Assert.Pass();
        }

        [Test]
        public void PolarVec2ConversionTests(){
            test_PolarVec2Conversion(new Vector2(0,0));
            test_PolarVec2Conversion(new Vector2(24,54));
            test_PolarVec2Conversion(new Vector2(-24, 3423));
            test_PolarVec2Conversion(new Vector2(-23, -12));
        }

        [Test]
        public void PolarVec2AdditionTests(){
            test_PolarVec2Addition(new Vector2 (23,45), new Vector2(-31, -32));
        }

        [Test]
        public void PolarVec2SubtractionTests(){
            test_PolarVec2Subtraction(new Vector2 (23,45), new Vector2(-31, -32));
        }

        [Test]
        public void PolarVec2DirectionTests(){
            Assert.AreEqual(PolarVec2.zero, PolarVec2.CartesianToPolar(Vector2.zero));
            Assert.AreEqual(PolarVec2.up, PolarVec2.CartesianToPolar(Vector2.up));
            Assert.AreEqual(PolarVec2.down, PolarVec2.CartesianToPolar(Vector2.down));
            Assert.AreEqual(PolarVec2.right, PolarVec2.CartesianToPolar(Vector2.right));
            Assert.AreEqual(PolarVec2.left, PolarVec2.CartesianToPolar(Vector2.left));

            /*
            Assert.AreEqual(Vector2.zero, PolarVec2.PolartoCartesian(PolarVec2.zero));
            Assert.AreEqual(Vector2.up, PolarVec2.PolartoCartesian(PolarVec2.up));
            Assert.AreEqual(Vector2.down, PolarVec2.PolartoCartesian(PolarVec2.down));
            Assert.AreEqual(Vector2.right, PolarVec2.PolartoCartesian(PolarVec2.right));
            Assert.AreEqual(Vector2.left, PolarVec2.PolartoCartesian(PolarVec2.left));
            */
        }

        #region Individual Test Functions

        public void test_PolarVec2Conversion(Vector2 testVec){
            //Debug.Log($"{testVec.ToString("F32")} : {PolarVec2.PolartoCartesian(PolarVec2.CartesianToPolar(testVec)).ToString("F32")}");
            Vector2 convertedVec = PolarVec2.PolartoCartesian(PolarVec2.CartesianToPolar(testVec));
            //Assert.AreEqual(testVec, convertedVec);
            Utility.VecAreEqual(testVec, convertedVec, tolerance_threshold);
        }
        public void test_PolarVec2Addition(Vector2 v1, Vector2 v2){
            Vector2 addedVec = PolarVec2.PolartoCartesian(PolarVec2.CartesianToPolar(v1) + PolarVec2.CartesianToPolar(v2));
            //Assert.AreEqual(v1 + v2, addedVec);
            Utility.VecAreEqual(v1 + v2, addedVec, tolerance_threshold);
        }
        public void test_PolarVec2Subtraction(Vector2 v1, Vector2 v2){
            Vector2 subtractedVec = PolarVec2.PolartoCartesian(PolarVec2.CartesianToPolar(v1) - PolarVec2.CartesianToPolar(v2));
            //Assert.AreEqual(v1 - v2, subtractedVec);
            Utility.VecAreEqual(v1 - v2, subtractedVec, tolerance_threshold);
        }

        #endregion
    }
}
