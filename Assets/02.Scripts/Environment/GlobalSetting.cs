using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public static class GlobalSetting
    {
        public static int POSE_RIGGINGPOINTS_COUNT = 40; // 33°³ * x,y,z°ª
        public static int camWidth = 640;
        public static int camHeight = 480;

        public static Vector3 min;
        public static Vector3 max;

        public static void SetVectorMinMax(float x, float y, float z)
        {
            float minX = min.x > x ? x : min.x;
            float minY = min.y > y ? y : min.y;
            float minZ = min.z > z ? z : min.z;

            float maxX = max.x < x ? x : max.x;
            float maxY = max.y < y ? y : max.y;
            float maxZ = max.z < z ? z : max.z;
            min = new Vector3(minX, minY, minZ);
            max = new Vector3(maxX, maxY, maxZ);
        }
    }
}
