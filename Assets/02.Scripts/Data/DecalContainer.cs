using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public struct DecalContainer
    {
        public PaintIn3D.P3dPaintDecal paintDecal;
        public PaintIn3D.P3dPaintableTexture texture;
        public Camera originCamera;
        public Transform camRoot;
        public Quaternion camAngle;
    }
}
