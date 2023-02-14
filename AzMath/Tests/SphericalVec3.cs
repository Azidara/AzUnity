using System;
using UnityEngine;

namespace AzMath {
    public class SphericalVec3
    {
        public double r;
        
        public double θ;
        public double φ;
        /*
        private float _θ;
        public float θ {
            get {return _θ;}
            set {float tmp = value; 
                while(tmp < 0){tmp += 360;}
                _θ = tmp % 360;}
            }
        
        private float _φ;
        public float φ {
            get {return _φ;}
            set {float tmp = value;
                while(tmp < 0){tmp += 180;}
                if (tmp > 180){tmp = 180 - (tmp % 180);}
                _φ = tmp;
                }
            }
            //*/

        public static SphericalVec3 zero {get {return new SphericalVec3(0,0,0);}}
        public static SphericalVec3 forward {get {return new SphericalVec3(1,90,0);}}
        public static SphericalVec3 backward {get {return new SphericalVec3(1,270,0);}}
        public static SphericalVec3 up {get {return new SphericalVec3(1,0,90);}}
        public static SphericalVec3 down {get {return new SphericalVec3(1,0,-90);}}
        public static SphericalVec3 right {get {return new SphericalVec3(1,90,0);}}
        public static SphericalVec3 left {get {return new SphericalVec3(1,270,0);}}

        public SphericalVec3(){
            this.r = 0;
            this.θ = 0;
            this.φ = 0;
        }

        public SphericalVec3(double r, double θ, double φ){
            this.r = r;
            this.θ = θ;
            this.φ = φ;
        }

        
        public static SphericalVec3 CartesianToSpherical(Vector3 cartesian){
            return CartesianToSpherical(cartesian.x, cartesian.y, cartesian.z);
        }
        public static SphericalVec3 CartesianToSpherical(double x, double y, double z){
            double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
            //double t = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));
            double φ = Math.Atan2(z,x);
            //double θ = Math.Atan(t/y);
            double θ = Math.Acos(y/r);

            return new SphericalVec3 (r, φ, θ);
        }
        public static Vector3 SphericalToCartesian(SphericalVec3 spherical){
            return SphericalToCartesian(spherical.r, spherical.φ, spherical.θ);
        }
        public static Vector3 SphericalToCartesian(double r, double φ, double θ){
            double x = r * Math.Sin(θ) * Math.Cos(φ);
            double y = r * Math.Cos(θ);
            double z = r * Math.Sin(θ) * Math.Sin(φ);
            return (new Vector3 ((float)x,(float)y,(float)z));
        }
        
        /*
        public static SphericalVec3 CartesianToSpherical(Vector3 cartesian){
            return CartesianToSpherical(cartesian.x, cartesian.y, cartesian.z);
        }

        public static SphericalVec3 CartesianToSpherical(float x, float y, float z){
            // r = Sqrt(x^2 + y^2 + z^2)
            float r = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
            // θ = Atan(z/x)
            float θ = Mathf.Atan2(z,x) * Mathf.Rad2Deg;
            // φ = Acos(y/r)
            float t = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
            float φ = Mathf.Acos(y/t) * Mathf.Rad2Deg;
            if (t == 0){φ=0;}
            return new SphericalVec3(r, θ, φ);
        }
        public static Vector3 SphericalToCartesian(SphericalVec3 spherical){
            return SphericalToCartesian(spherical.r, spherical.θ, spherical.φ);
        }
        public static Vector3 SphericalToCartesian(float r, float θ, float φ){
            θ = θ * Mathf.Deg2Rad;
            φ = φ * Mathf.Deg2Rad;
            float x = r * Mathf.Sin(φ) * Mathf.Cos(θ);
            float y = r * Mathf.Cos(φ);
            float z = r * Mathf.Sin(φ) * Mathf.Sin(θ);
            

            return new Vector3 (x,y,z);
        }
        */
        /*
        public static SphericalVec3 CartesianToSpherical(float x, float y, float z){
            // r = Sqrt(x^2 + y^2 + z^2)
            float r = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
            // θ = Atan(z/x)
            float θ = Mathf.Atan(z/x) * Mathf.Rad2Deg;
            // φ = Acos(y/r)
            float φ = Mathf.Acos(y/r) * Mathf.Rad2Deg;
            
            if (x == 0){ 
                if (z == 0){
                    θ = 0;
                }
                else if(z < 0){
                    θ = -90;
                }
                else {
                    θ = 90;
                }   
            }
            if (r == 0){
                φ = 0;
                θ = 0;
            }

            return new SphericalVec3(r, θ, φ);
        }
        */

        // These work but are not great implementations
        public static SphericalVec3 operator +(SphericalVec3 v1, SphericalVec3 v2){
            return SphericalVec3.CartesianToSpherical(SphericalVec3.SphericalToCartesian(v1) + SphericalVec3.SphericalToCartesian(v2));
        }

        public static SphericalVec3 operator -(SphericalVec3 v){
            v.r = -v.r;
            return v;
        }

        public static SphericalVec3 operator -(SphericalVec3 v1, SphericalVec3 v2){
            return v1 + -v2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            var v2 = (SphericalVec3)obj;
            return (this.r == v2.r && this.θ == v2.θ && this.φ == v2.φ);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.r.GetHashCode() ^ this.θ.GetHashCode() ^ this.φ.GetHashCode();
        }

        public static bool operator ==(SphericalVec3 v1, SphericalVec3 v2){
            return v1.r == v2.r && v1.θ == v2.θ && v1.φ == v2.φ;
        }
        public static bool operator !=(SphericalVec3 v1, SphericalVec3 v2){
            return v1.r != v2.r || v1.θ != v2.θ || v1.φ != v2.φ;
        }

        override public string ToString(){
            return $"({r.ToString("F2")},{θ.ToString("F2")},{φ.ToString("F2")})";
        }
    }
}