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

    public class Base_Websocket : MonoBehaviour
    {
        private WebSocket _webSocket;
        public string _serverUrl = "ws://localhost:8081";

        //private byte[] decompressedData;

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

        #region WebSocket events

        private void OnDestroy()
        {
            _webSocket.Close();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            Debug.Log("WebSocket opened.");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            Debug.Log($"{_serverUrl} WebSocket message received: " + e.Data);
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