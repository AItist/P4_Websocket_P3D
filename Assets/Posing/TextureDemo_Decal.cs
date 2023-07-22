using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureDemo_Decal : MonoBehaviour
{
   

    public Material p3d_Decal;
    public Texture2D tex;



    private void Update()
    {
    
     
        p3d_Decal.SetTexture("_MainTex", tex);


    }



}
