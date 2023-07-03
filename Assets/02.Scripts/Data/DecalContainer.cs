using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public struct DecalContainer
    {
        public PaintIn3D.P3dPaintDecal paintDecal;
        public Camera originCamera;
    }
}
