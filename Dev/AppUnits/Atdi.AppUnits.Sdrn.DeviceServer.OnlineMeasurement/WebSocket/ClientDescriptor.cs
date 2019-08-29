using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket
{
    public class ClientDescriptor
    {
        public MeasurementDispatcher Dispatcher { get; set; }

        public long OnlineMeasId { get; set; }

        public Guid Token { get; set; }

        public ClientReadyTakeMeasResultTask AsyncTask { get; set; }

        public CancellationTokenSource TokenSource { get; set; }

        public OnlineMeasurementProcess Process { get; set; }

        public WebSocketServer Server { get; set; }

        public bool CheckToken(byte[] source)
        {
            try
            {
                var guid = new Guid(source);
                return this.Token == guid;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
