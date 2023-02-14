using UnityEngine;

namespace AzMath {
    [System.Serializable] 
    public class PolarVec3
    {
        [SerializeField] public float r;
        [SerializeField] private float _θ;
        public float θ {
            get {return _θ;}
            set {float tmp = value; 
                while(tmp < 0){tmp += 360;}
                _θ = tmp % 360;}
            }
        [SerializeField] public float h;

        public static PolarVec3 zero {get {return new PolarVec3(0,0,0);}}
        public static PolarVec3 forward {get {return new PolarVec3(1,90,0);}}
        public static PolarVec3 backward {get {return new PolarVec3(1,270,0);}}
        public static PolarVec3 up {get {return new PolarVec3(0,0,1);}}
        public static PolarVec3 right {get {return new PolarVec3(1,0,0);}}
        public static PolarVec3 down {get {return new PolarVec3(0,0,-1);}}
        public static PolarVec3 left {get {return new PolarVec3(1,180,0);}}

        public PolarVec3(){
            this.r = 0;
            this.θ = 0;
            this.h = 0;
        }
        public PolarVec3(float r, float θ, float h){
            this.r = r;
            this.θ = θ;
            this.h = h;
        }

        public static PolarVec3 CartesianToPolar(Vector3 cartesian){
            return CartesianToPolar(cartesian.x, cartesian.y, cartesian.z);
        }

        public static PolarVec3 CartesianToPolar(float x, float y, float z){
            float r = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2));
            float θ = Mathf.Atan2(z,x) * Mathf.Rad2Deg;
            float h = y;

            return new PolarVec3(r, θ, h);
        }

        public static Vector3 PolartoCartesian(PolarVec3 polar){
            return PolartoCartesian(polar.r, polar.θ, polar.h);
        }

        public static Vector3 PolartoCartesian(float r, float θ, float h){
            float x = r * Mathf.Cos(θ * Mathf.Deg2Rad);
            float y = h;
            float z = r * Mathf.Sin(θ * Mathf.Deg2Rad);

            return new Vector3(x,y,z);
        }

        public static PolarVec3 operator *(PolarVec3 v, float s){
            v.r *= s; 
            v.h *= s;
            return v;
        }

        // These work but are not great implementations
        public static PolarVec3 operator +(PolarVec3 v1, PolarVec3 v2){
            return PolarVec3.CartesianToPolar(PolarVec3.PolartoCartesian(v1) + PolarVec3.PolartoCartesian(v2));
        }
        public static PolarVec3 operator -(PolarVec3 v){
            v.r = -v.r;
            v.h = -v.h;
            return v;
        }

        public static PolarVec3 operator -(PolarVec3 v1, PolarVec3 v2){
            return v1 + -v2;
        }

        /*
        public static PolarVec3 operator +(PolarVec3 v1, PolarVec3 v2){
            PolarVec3 result = new PolarVec3();
            // r = Sqrt(r1^2 + r2^2 + 2*r1*r2*cos(θ2 - θ1))
            result.r = Mathf.Sqrt(Mathf.Pow(v1.r, 2) + Mathf.Pow(v2.r,2) + (2*v1.r*v2.r*Mathf.Cos(v2.θ - v1.θ)));
            // θ = θ1 + arctan2(r2*sin(θ2 - θ1), r1 + r2*cos(θ2 - θ1))
            result.θ = v1.θ + Mathf.Atan2(v2.r * Mathf.Sin(v2.θ - v1.θ), v1.r + v2.r*Mathf.Cos(v2.θ - v1.θ));
            result.h = v1.h + v2.h;
            return result;
        }
        */

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            var v2 = (PolarVec3)obj;
            return (this.r == v2.r && this.θ == v2.θ && this.h == v2.h);
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.r.GetHashCode() ^ this.θ.GetHashCode() ^ this.h.GetHashCode();
        }

        public static bool operator ==(PolarVec3 v1, PolarVec3 v2){
            return v1.r == v2.r && v1._θ == v2._θ && v1.h == v2.h;
        }
        public static bool operator !=(PolarVec3 v1, PolarVec3 v2){
            return v1.r != v2.r || v1._θ != v2._θ || v1.h != v2.h;
        }

        override public string ToString(){
            return $"({r.ToString("F2")},{θ.ToString("F2")},{h.ToString("F2")})";
        }

        /*
        Vector3 cartesianToPolar(float x, float y, float z){
            float r = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2));
            float θ;
            float h = y;

            if (x == 0 && y == 0 ){
                θ = 0;
            }
            else {
                if (x >= 0){
                    θ = Mathf.Asin(z/r);
                }
                else {
                    θ = -Mathf.Asin(z/r) + Mathf.PI;
                }
            }
            
            return new Vector3 (r, θ, h);
        }
        */
    }
}
