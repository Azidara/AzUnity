using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AzMath;

namespace AzMathUnitTesting {
    public class Utility{
        public static void VecAreEqual(Vector2 v1, Vector2 v2, float tolerance=float.NaN){
            if (tolerance == float.NaN){
                Assert.AreEqual(v1.x, v2.x);
                Assert.AreEqual(v1.y, v2.y);
            }
            else {
                Assert.AreEqual(v1.x, v2.x, tolerance);
                Assert.AreEqual(v1.y, v2.y, tolerance);
            }
        }

        public static void VecAreEqual(Vector3 v1, Vector3 v2, float tolerance=float.NaN){
            if (tolerance == float.NaN){
                Assert.AreEqual(v1.x, v2.x);
                Assert.AreEqual(v1.y, v2.y);
                Assert.AreEqual(v1.z, v2.z);
            }
            else {
                Assert.AreEqual(v1.x, v2.x, tolerance);
                Assert.AreEqual(v1.y, v2.y, tolerance);
                Assert.AreEqual(v1.z, v2.z, tolerance);
            }
        }
    }

    public class SphericalVec3Tests
    {
        public float tolerance_threshold = 0.001f;

        // A Test behaves as an ordinary method
        [Test]
        public void SphericalVec3TestsSimplePasses()
        {
            Assert.Pass();
        }

        [Test]
        public void SphericalVec3ConversionTests(){
            //test_SphericalVec3Conversion(new Vector3(0,0,0));
            test_SphericalVec3Conversion(new Vector3(24,54,12));
            test_SphericalVec3Conversion(new Vector3(-24, 3423, -23));
            test_SphericalVec3Conversion(new Vector3(-23, -12, 67));
        }
        
        [Test]
        public void SphericalVec3AdditionTests(){
            test_SphericalVec3Addition(new Vector3 (23,45, 65), new Vector3(-31, -32, 23));
        }

        [Test]
        public void SphericalVec3SubtractionTests(){
            test_SphericalVec3Subtraction(new Vector3 (23,-45, 23), new Vector3(-31, -32, 42));
        }
        
        [Test]
        public void SphericalVec3DirectionTests(){
            /*
            Debug.Log($"Forward : {SphericalVec3.CartesianToSpherical(Vector3.forward).ToString()}");
            Debug.Log($"Backward : {SphericalVec3.CartesianToSpherical(Vector3.back).ToString()}");
            Debug.Log($"Up : {SphericalVec3.CartesianToSpherical(Vector3.up).ToString()}");
            Debug.Log($"Down : {SphericalVec3.CartesianToSpherical(Vector3.down).ToString()}");
            Debug.Log($"Right : {SphericalVec3.CartesianToSpherical(Vector3.right).ToString()}");
            Debug.Log($"Left : {SphericalVec3.CartesianToSpherical(Vector3.left).ToString()}");
            */

            Assert.AreEqual(SphericalVec3.forward, SphericalVec3.CartesianToSpherical(Vector3.forward));
            Assert.AreEqual(SphericalVec3.backward, SphericalVec3.CartesianToSpherical(Vector3.back));
            Assert.AreEqual(SphericalVec3.up, SphericalVec3.CartesianToSpherical(Vector3.up));
            Assert.AreEqual(SphericalVec3.down, SphericalVec3.CartesianToSpherical(Vector3.down));
            Assert.AreEqual(SphericalVec3.right, SphericalVec3.CartesianToSpherical(Vector3.right));
            Assert.AreEqual(SphericalVec3.left, SphericalVec3.CartesianToSpherical(Vector3.left));
            Assert.AreEqual(SphericalVec3.zero, SphericalVec3.CartesianToSpherical(Vector3.zero));

            /*
            Assert.AreEqual(Vector3.zero, SphericalVec3.PolartoCartesian(SphericalVec3.zero));
            Assert.AreEqual(Vector3.up, SphericalVec3.PolartoCartesian(SphericalVec3.up));
            Assert.AreEqual(Vector3.down, SphericalVec3.PolartoCartesian(SphericalVec3.down));
            Assert.AreEqual(Vector3.right, SphericalVec3.PolartoCartesian(SphericalVec3.right));
            Assert.AreEqual(Vector3.left, SphericalVec3.PolartoCartesian(SphericalVec3.left));
            */
        }

        #region Individual Test Functions

        public void test_SphericalVec3Conversion(Vector3 testVec){
            //Debug.Log($"{testVec.ToString("F32")} : {SphericalVec3.PolartoCartesian(SphericalVec3.CartesianToPolar(testVec)).ToString("F32")}");
            Vector3 convertedVec = SphericalVec3.SphericalToCartesian(SphericalVec3.CartesianToSpherical(testVec));
            Assert.AreEqual(testVec, convertedVec);
            Utility.VecAreEqual(testVec, convertedVec, tolerance_threshold);
        }
        public void test_SphericalVec3Addition(Vector3 v1, Vector3 v2){
            Vector3 AddedVec = SphericalVec3.SphericalToCartesian(SphericalVec3.CartesianToSpherical(v1) + SphericalVec3.CartesianToSpherical(v2));
            Assert.AreEqual(v1 + v2, AddedVec);
            Utility.VecAreEqual(v1 + v2, AddedVec, tolerance_threshold);
        }
        public void test_SphericalVec3Subtraction(Vector3 v1, Vector3 v2){
            Vector3 SubtractedVec = SphericalVec3.SphericalToCartesian(SphericalVec3.CartesianToSpherical(v1) - SphericalVec3.CartesianToSpherical(v2));
            Assert.AreEqual(v1 - v2, SubtractedVec);
            Utility.VecAreEqual(v1 - v2, SubtractedVec, tolerance_threshold);
        }

        #endregion
        
    }
}
