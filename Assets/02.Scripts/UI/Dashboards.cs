using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Dashboards : MonoBehaviour
    {
        //#region instance
        //private static Dashboards instance;

        //public static Dashboards Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = GameObject.FindObjectOfType<Dashboards>();
        //        }
        //        return instance;
        //    }
        //}
        //#endregion

        public Vector3 min;
        public Vector3 max;

        //public void SetVectorMinMax(float x, float y, float z)
        //{
        //    float minX = min.x > x ? x : min.x;
        //    float minY = min.y > y ? y : min.y;
        //    float minZ = min.z > z ? z : min.z;

        //    float maxX = max.x < x ? x : max.x;
        //    float maxY = max.y < y ? y : max.y;
        //    float maxZ = max.z < z ? z : max.z;
        //    min = new Vector3(minX, minY, minZ);
        //    max = new Vector3(maxX, maxY, maxZ);
        //}

        private void Update()
        {
            min = Environment.GlobalSetting.min;
            max = Environment.GlobalSetting.max;
        }
    }
}
