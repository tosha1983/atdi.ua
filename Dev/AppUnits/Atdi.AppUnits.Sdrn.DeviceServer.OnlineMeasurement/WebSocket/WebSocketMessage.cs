using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket
{
    public enum WebSocketMessageKind
    {
        Unknown,

        /// <summary>
        /// a text message.
        /// </summary>
        Text,

        /// <summary>
        /// a binairy message
        /// </summary>
        Binary,

        /// <summary>
        /// a client send a close message.
        /// </summary>
        Closed,

        /// <summary>
        /// message from client is a ping message
        /// </summary>
        Ping,

        /// <summary>
        ///  message from client is a pong message
        /// </summary>
        Pong
    }

    public class WebSocketMessage
    {
        /// <summary>
        /// Kind of the message
        /// </summary>
        public WebSocketMessageKind Kind { get; set; }

        /// <summary>
        /// Contains the data send by a client/server.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Message length in bytes.
        /// </summary>
        public UInt64 Length { get; set; } = 0;

    }
}
