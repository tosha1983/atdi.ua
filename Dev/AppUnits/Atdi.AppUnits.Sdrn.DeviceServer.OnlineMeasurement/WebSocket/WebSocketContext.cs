using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket
{
    public class WebSocketContext
    {
        private readonly TcpClient _client;

        public WebSocketContext(TcpClient client)
        {
            this._client = client;
        }

        public void SendMessage(WebSocketMessage message)
        {
            if (!_client.Connected)
            {
                throw new InvalidOperationException("Web socket connection is closed");
            }
            
            var stream = _client.GetStream();
            if (!stream.CanWrite)
            {
                throw new InvalidOperationException("Web socket can not write data ");
            }

            var messageLength = (ulong)message.Data.Length;

            byte start = 0x00;

            if (message.Kind == WebSocketMessageKind.Text)
            {
                start = 0x81;
            }
            else if (message.Kind == WebSocketMessageKind.Binary)
            {
                start = 0x82;
            }
            else if (message.Kind == WebSocketMessageKind.Closed)
            {
                start = 0x88;
            }
            else if (message.Kind == WebSocketMessageKind.Ping)
            {
                start = 0x89;
            }
            else if (message.Kind == WebSocketMessageKind.Pong)
            {
                start = 0x8A;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported the message kind '{message.Kind}'");
            }

            var frame = new List<byte>
            {
                start
            };

            if (messageLength <= 125)
            {
                frame.Add((byte)messageLength);
                frame.AddRange(message.Data);
            }
            else if (messageLength > 125 && messageLength <= 65535)
            {
                UInt16 messageLength16bit = (UInt16)messageLength;
                byte[] byteArray = BitConverter.GetBytes(messageLength16bit).Reverse<byte>().ToArray();

                frame.Add((byte)126);
                frame.AddRange(byteArray);
                frame.AddRange(message.Data);
            }
            else
            {
                byte[] byteArray = BitConverter.GetBytes(messageLength).Reverse<byte>().ToArray();
                frame.Add((byte)127);
                frame.AddRange(byteArray);
                frame.AddRange(message.Data);
            }

            var frameBytes = frame.ToArray();
            stream.Write(frameBytes, 0, frameBytes.Length);
            stream.Flush();

        }
    }
}
