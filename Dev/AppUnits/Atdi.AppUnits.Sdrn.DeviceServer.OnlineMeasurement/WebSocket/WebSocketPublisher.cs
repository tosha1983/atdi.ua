using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket
{
    public class WebSocketPublisher
    {
        private static readonly UTF8Encoding Encoder = new UTF8Encoding();
        private readonly WebSocketContext _context;

        public WebSocketPublisher(WebSocketContext context)
        {
            this._context = context;
        }

        public void Send(OnlineMeasMessage message)
        {
            var messageAsJson = JsonConvert.SerializeObject(message);

            var data = WebSocketPublisher.Encoder.GetBytes(messageAsJson);
            _context.SendMessage(
                new WebSocketMessage
                {
                    Kind = WebSocketMessageKind.Text,
                    Data = data,
                    Length = (ulong)data.Length
                });
        }
    }
}
