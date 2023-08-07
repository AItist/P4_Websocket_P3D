using Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUser : MonoBehaviour
{
    public MeshRenderer renderer;

    // Update is called once per frame
    void Update()
    {
        if (SocketManager.Instance.IsQueueIsEmpty()) { return; }

        byte[] bytes = SocketManager.Instance.Dequeue_LastOne();

        int tWidth = 512;
        int tHeight = 512;
        int tDepth = 3;

        Texture2D recoveredTexture = Update_CreateTexture2D(tWidth, tHeight, tDepth, bytes);

        if (recoveredTexture == null) { return; }

        renderer.material.SetTexture("_MainTex", recoveredTexture);
    }

    private Texture2D Update_CreateTexture2D(int tWidth, int tHeight, int tDepth, byte[] decompressedData)
    {
        if (decompressedData == null) { return null; }

        Texture2D recovered = new Texture2D(tWidth, tHeight);
        recovered.LoadImage(decompressedData);
        recovered.Apply();

        return recovered;
    }
}
