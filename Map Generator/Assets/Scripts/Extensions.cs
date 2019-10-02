using UnityEngine;

namespace Extensions { 
    public static class Vector2Extensions {
        public static float GetAngle(this Vector2 v) { 
            float angle = Mathf.Atan2(v.y, v.x);
            if(angle < 0 ) angle += Mathf.PI * 2;
            return angle;
        }

        public static float GetAngle(this Vector2 v0, Vector2 v1) { 
            return GetAngle(v1 - v0);
        }

        public static Vector3 ToVector3(this Vector2 v, float z) { 
            return new Vector3(v.x, v.y, z);
        }
        public static Vector3 ToVector3(this Vector2 v) { 
            return new Vector3(v.x, v.y, 0f);
        }
    }
}