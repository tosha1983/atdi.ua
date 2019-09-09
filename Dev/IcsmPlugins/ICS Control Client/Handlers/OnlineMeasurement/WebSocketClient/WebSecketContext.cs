﻿using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XICSM.ICSControlClient.OnlineMeasurement
{
    public class WebSocketContext
    {
        private static readonly UTF8Encoding encoder = new UTF8Encoding();
        private readonly ClientWebSocket _webSocket;
        private readonly int _sendTimeout = 5000; // 5 sec

        public WebSocketContext(ClientWebSocket webSocket)
        {
            this._webSocket = webSocket;
        }

        public void SendAsJson(OnlineMeasMessageKind kind, object data)
        {
            var readyMsg = new OnlineMeasMessage
            {
                Kind = kind,
                Container = data
            };
            var json = JsonConvert.SerializeObject(readyMsg);
            this.Send(json);
        }

        private void Send(string data)
        {
            if (this._webSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("The Websocket connection is not opened");
            }

            var buffer = encoder.GetBytes(data);
            

            using (var cts = new CancellationTokenSource(_sendTimeout))
            {
                _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cts.Token).GetAwaiter().GetResult();
                if (cts.IsCancellationRequested)
                {
                    throw new InvalidOperationException("The message is not sent: timeout expired");
                }
            }
        }

        private void Send(byte[] data)
        {
            if (this._webSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("The Websocket connection is not opened");
            }
            using (var cts = new CancellationTokenSource(_sendTimeout))
            {
                _webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, cts.Token).GetAwaiter().GetResult();
                if (cts.IsCancellationRequested)
                {
                    throw new InvalidOperationException("The message is not sent: timeout expired");
                }
            }
        }
    }
}
