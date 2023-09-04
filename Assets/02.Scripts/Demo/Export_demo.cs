using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using UnityEditor;
//using static PaintIn3D.P3dWindow;
using System;
using WebSocket_;

public class Export_demo : MonoBehaviour
{
    public string publicPath;
    private bool check;

    public Base_Websocket sock;

    public P3dPaintable paintable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!check)
            {
                check = true;
                
                Execute(paintable, paintable.Materials, paintable.GetComponents<P3dPaintableTexture>());
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            check = false;
        }
    }

    void Execute(P3dPaintable paintable, Material[] materials, P3dPaintableTexture[] paintableTextures)
    {
        //Debug.Log("Hello");

        for (int i = 0; i < materials.Length; i++)
        {
            var material = materials[i];

            if (material != null && paintableTextures.Length > 0)
            {
                var path = AssetDatabase.GetAssetPath(material);
                //Debug.Log(path); // 
                var dir = string.IsNullOrEmpty(path) == false ? System.IO.Path.GetDirectoryName(path) : "Assets";
                //Debug.Log(dir); // Assets

                //path = EditorUtility.SaveFilePanelInProject("Export Material & Textures", name, "mat", "Export Your Material and Textures", dir);
                path = publicPath;

                if (string.IsNullOrEmpty(path) == false)
                {
                    //Undo.RecordObjects(paintableTextures, "Export Material & Textures");

                    //var clone = Instantiate(material);

                    //AssetDatabase.CreateAsset(clone, path);

                    foreach (var paintableTexture in paintableTextures)
                    {
                        //var finalPath = System.IO.Path.GetDirectoryName(path) + "/" + System.IO.Path.GetFileNameWithoutExtension(path) + paintableTexture.Slot.Name + "." + GetExtension(Settings.DefaultTextureFormat);

                        //System.IO.File.WriteAllBytes(finalPath, GetData(paintableTexture, Settings.DefaultTextureFormat));

                        ////byte[] imgData = GetData(paintableTexture, PaintIn3D.P3dWindow.Settings.DefaultTextureFormat);
                        //string encodeStr = Convert.ToBase64String(imgData);

                        //Debug.Log(encodeStr);

                        //sock.Send_Message(encodeStr);
                        //AssetDatabase.ImportAsset(finalPath);

                        //paintableTexture.Output = AssetDatabase.AssetPathToGUID(finalPath);

                        //clone.SetTexture(paintableTexture.Slot.Name, AssetDatabase.LoadAssetAtPath<Texture>(finalPath));
                    }

                    //EditorUtility.SetDirty(this);
                }
            }
        }
    }

    //private string GetExtension(PaintIn3D.P3dWindow.ExportTextureFormat f)
    //{
    //    switch (PaintIn3D.P3dWindow.Settings.DefaultTextureFormat)
    //    {
    //        case PaintIn3D.P3dWindow.ExportTextureFormat.PNG: return "png";
    //        case PaintIn3D.P3dWindow.ExportTextureFormat.TGA: return "tga";
    //    }

    //    return null;
    //}

    //private byte[] GetData(P3dPaintableTexture t, PaintIn3D.P3dWindow.ExportTextureFormat f)
    //{
    //    switch (PaintIn3D.P3dWindow.Settings.DefaultTextureFormat)
    //    {
    //        case PaintIn3D.P3dWindow.ExportTextureFormat.PNG: return t.GetPngData();
    //        case PaintIn3D.P3dWindow.ExportTextureFormat.TGA: return t.GetTgaData();
    //    }

    //    return null;
    //}
}
