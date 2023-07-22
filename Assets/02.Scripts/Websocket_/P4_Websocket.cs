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
        /// Manager�� ������ �ؽ�ó ����
        /// </summary>
        /// <param name="tex"></param>
        void Update_SendTextureToP3d(Texture2D tex)
        {
            //main.MainManager.Instance.Websocket_texture2D = tex;
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
                //Debug.Log(e.Data.ToString());
                ImageData data = JsonConvert.DeserializeObject<ImageData>(e.Data);
                data.ConvertImgString_to_byteArray();
                data.ConvertPoseString_to_float3Array();
                //data.stage1_InitComplete = true;

                ////string info = "";
                //Debug.Log($"count: {data.cam_count}");
                //Debug.Log($"pose {data.pose_string}");
                //Debug.Log($"i0 {data.img_0}");
                //Debug.Log($"i1 {data.img_1}");
                //Debug.Log($"i2 {data.img_2}");
                //Debug.Log($"i3 {data.img_3}");

                //// �� ������ �ڵ�� �̹��� ������ ��ť
                Management.MainManager.Instance.EnqueueImageData(data);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing JSON data: " + ex.Message);
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

        #endregion WebSocket events
    }
}