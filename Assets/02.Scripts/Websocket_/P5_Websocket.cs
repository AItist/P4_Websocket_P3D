using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebSocket_
{
    using Newtonsoft.Json;
    using System;
    using UnityEditor.PackageManager;
    using UnityEditor.VersionControl;
    using WebSocketSharp;
    using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

    public class P5_Websocket : MonoBehaviour
    {

        byte[] decompressedData;
        public MeshRenderer renderer;

        // ----- temp

        private WebSocket _webSocket;
        public string _serverUrl = "ws://localhost:8082"; // replace with your WebSocket URL

        private void Update()
        {
            int tWidth = 512;
            int tHeight = 512;
            int tDepth = 3;

            Texture2D recoveredTexture = Update_CreateTexture2D(tWidth, tHeight, tDepth, decompressedData);

            if (recoveredTexture == null) { return; }

            renderer.material.SetTexture("_MainTex", recoveredTexture);
        }

        private Texture2D Update_CreateTexture2D(int tWidth, int tHeight, int tDepth, byte[] decompressedData)
        {
            if (decompressedData == null) { return null; }

            //Texture2D recoveredTexture = new Texture2D(tWidth, tHeight, TextureFormat.RGB24, false);
            //recoveredTexture.LoadRawTextureData(decompressedData);
            //recoveredTexture.Apply();

            Texture2D recovered = new Texture2D(tWidth, tHeight);
            recovered.LoadImage(decompressedData);
            recovered.Apply();

            return recovered;

            //return recoveredTexture;
        }

        // Start is called before the first frame update
        void Start()
        {
            _webSocket = new WebSocket(_serverUrl);
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnMessage += OnMessage;
            _webSocket.OnClose += OnClose;
            _webSocket.OnError += OnError;
            _webSocket.Connect();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            Debug.Log("WebSocket opened.");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            //Debug.Log("WebSocket on message.");
            //Debug.Log(e.Data);

            try
            {
                string str = System.Text.Encoding.UTF8.GetString(e.RawData);
                //byte[] test_imgValue = Convert.FromBase64String(str);
                //decompressedData = test_imgValue;

                //return;
                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(str);

                Dictionary<string, byte[]> imgData = new Dictionary<string, byte[]>();
                foreach (string key in data.Keys)
                {
                    string value = data[key];
                    byte[] convertedValue = Convert.FromBase64String(value);

                    imgData.Add(key, convertedValue);

                    decompressedData = convertedValue;  // temp
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
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
    }
}
