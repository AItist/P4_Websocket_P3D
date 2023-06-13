using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Drawing;

using PaintIn3D;

using Data;

namespace WebSocket_
{
    using WebSocketSharp;
    using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

    public class P4_Websocket : MonoBehaviour
    {
        private WebSocket _webSocket;
        public string _serverUrl = "ws://localhost:8081"; 

        private byte[] decompressedData;

        public void SendDataToServer(string data)
        {
            //Debug.Log($"Data received, length {data}");
            //Debug.Log($"Data received, length {data.Length}");

            Send_Message(data);
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

        private void OnDestroy()
        {
            _webSocket.Close();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            Debug.Log("WebSocket opened.");
        }

        /// <summary>
        /// �����Ͽ��� AI ó���� �̹��� �޾ƿͼ� ������ ��ġ ����, �� �����ڿ� ������ ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMessage(object sender, MessageEventArgs e)
        {
            //Debug.Log($"{_serverUrl} WebSocket message received: " + e.Data);

            try
            {
                ImageData data = JsonConvert.DeserializeObject<ImageData>(e.Data);
                data.ConvertImgString_to_byteArray();

                // �� ������ �ڵ�� �̹��� ������ ��ť
                Management.MainManager.Instance.EnqueueImageData(data);

                // Debug
                //Debug.Log($"data.index : {data.index}");
                //Debug.Log($"data.ret : {data.ret}");
                //Debug.Log($"data.frame : {data.frame}");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing JSON data: " + ex.Message);
            }

            // P5 debug
            //_webSocket.Send("Hello");
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Debug.Log("WebSocket closed.");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.LogError("WebSocket error: " + e.Message);
        }

        public void Send_Message(string message)
        {
            _webSocket.Send(message);
        }

        #endregion WebSocket events
    }
}