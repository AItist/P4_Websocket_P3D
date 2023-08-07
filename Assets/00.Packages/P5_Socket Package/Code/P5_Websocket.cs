using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebSocket_
{
    using Management;
    using Newtonsoft.Json;
    using System;
    using WebSocketSharp;
    using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

    public class P5_Websocket : MonoBehaviour
    {
        private WebSocket _webSocket;
        public string _serverUrl = "ws://localhost:8082"; // replace with your WebSocket URL

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

            try
            {
                string str = System.Text.Encoding.UTF8.GetString(e.RawData);

                string data = JsonConvert.DeserializeObject<string>(str);

                SocketManager.Instance.Enqueue(Convert.FromBase64String(data));
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
