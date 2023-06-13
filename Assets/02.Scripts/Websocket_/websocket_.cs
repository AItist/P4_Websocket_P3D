using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using WebSocketSharp;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;
using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Drawing;

using PaintIn3D;

using Data;

public class websocket_ : MonoBehaviour
{
    private WebSocket _webSocket;
    public string _serverUrl = "ws://localhost:8081"; // replace with your WebSocket URL

    /// <summary>
    /// �����
    /// </summary>
    //public bool debug_resend = false;

    private byte[] decompressedData;

    public void SendDataToServer(string data)
    {
        //Debug.Log($"Data received, length {data}");
        //Debug.Log($"Data received, length {data.Length}");

        WS_SendMessage(data);
    }

    private void Start()
    {
        _webSocket = new WebSocket(_serverUrl);
        _webSocket.OnOpen += OnOpen;
        _webSocket.OnMessage += OnMessage;
        _webSocket.OnClose += OnClose;
        _webSocket.OnError += OnError;
        _webSocket.Connect();
    }

    /// <summary>
    /// �ؽ�ó�� png �̹����� �����Ѵ�.
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="path"></param>
    void SaveTextureAsPNG(Texture2D texture, string path)
    {
        byte[] pngData = texture.EncodeToPNG();

        string appPath = Application.dataPath;
        string combinedPath = $"{Application.dataPath}{path}";

        if (pngData != null)
            File.WriteAllBytes(combinedPath, pngData);
        else
            Debug.Log("Failed to convert Texture2D to PNG");
    }

    #region Update

    /// <summary>
    /// Update �޼��� �ȿ��� Texture2D�� �����ϰ� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="tWidth"></param>
    /// <param name="tHeight"></param>
    /// <param name="tDepth"></param>
    /// <param name="decompressedImg"></param>
    /// <returns> Texture2D </returns>
    Texture2D Update_CreateTexture2D(int tWidth, int tHeight, int tDepth, byte[] decompressedImg)
    {
        if (decompressedData == null) { return null; }

        //int tWidth = 640;
        //int tHeight = 480;
        //int tDepth = 3;

        // Step 3: Create a new Texture2D and load the decompressed data
        Texture2D recoveredTexture = new Texture2D(tWidth, tHeight, TextureFormat.RGB24, false);
        recoveredTexture.LoadRawTextureData(decompressedData);
        recoveredTexture.Apply();

        return recoveredTexture;
    }

    /// <summary>
    /// Manager�� ������ �ؽ�ó ����
    /// </summary>
    /// <param name="tex"></param>
    void Update_SendTextureToP3d(Texture2D tex)
    {
        //main.MainManager.Instance.Websocket_texture2D = tex;
    }

    private void Update()
    {
        int tWidth = 640;
        int tHeight = 480;
        int tDepth = 3;

        Texture2D recoveredTexture = Update_CreateTexture2D(tWidth, tHeight, tDepth, decompressedData);

        //// �����
        //SaveTextureAsPNG(recoveredTexture, "/Hello.png");

        if (recoveredTexture == null) { return; }

        // ������ �ؽ�ó�� ��ǥ�� paintDecal�� ������Ʈ �Ѵ�.
        Update_SendTextureToP3d(recoveredTexture);

    }

    #endregion

    #region WebSocket events

    /// <summary>
    /// ���ڿ� ������ �̹����� byte[] �迭�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="imgData"></param>
    private void ConvertImgData(string camIndex, string imgData)
    {
        // Step 1: Decode the Base64 encoded data
        byte[] decodedData = Convert.FromBase64String(imgData);

        // Step 2: Decompress the decoded data using GZip
        using (MemoryStream compressedStream = new MemoryStream(decodedData))
        {
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(decompressedStream);
                }
                decompressedData = decompressedStream.ToArray();
                // TODO : ConvertData decompressedData�� �����Ҷ�, camIndex�� Ű�� �ؼ� dict�� �����ؾ� �Ѵ�.

                //Debug.Log(decompressedStream.Length.ToString());
            }
        }

    }

    private void OnDestroy()
    {
        _webSocket.Close();
    }

    private void OnOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket opened.");
    }

    /// <summary>
    /// �����Ͽ��� AI ó���� �̹��� �޾ƿͼ� ������ ��ġ ����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMessage(object sender, MessageEventArgs e)
    {
        //Debug.Log($"{_serverUrl} WebSocket message received: " + e.Data);
        //Debug.Log("OnMessage");

        try
        {
            ImageData data = JsonConvert.DeserializeObject<ImageData>(e.Data);

            //Debug.Log($"data.index : {data.index}");
            //Debug.Log($"data.ret : {data.ret}");
            //Debug.Log($"data.frame : {data.frame}");

            // �� ����
            //data.ConvertImgString_to_byteArray();

            ConvertImgData(data.index.ToString(), data.frame);
            Debug.Log("OnMessage");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing JSON data: " + ex.Message);
        }

        //if (debug_resend)
        //{
        //    WS_SendMessage("hello data");
        //}
    }

    private void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket closed.");
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("WebSocket error: " + e.Message);
    }

    public void WS_SendMessage(string message)
    {
        _webSocket.Send(message);
    }

    #endregion
}

//public class ImageData
//{
//    public int index { get; set; }
//    public bool ret { get; set; }
//    public string frame { get; set; }
//}