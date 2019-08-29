using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebSocketClientImpl
{
    public class WebSocketHandler
    {
        private readonly byte[] sensorToken;

        public WebSocketHandler(byte[] sensorToken)
        {
            this.sensorToken = sensorToken;
        }

        public void Handle(string data, WebSocketContext context)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<OnlineMeasMessage>(data);
                if (msg.Kind == OnlineMeasMessageKind.DeviceServerParameters)
                {
                    Console.WriteLine($"Received data: get measurment result parameters");
                    var readyMsg = new OnlineMeasMessage
                    {
                        Kind = OnlineMeasMessageKind.ClientReadyTakeMeasResult,
                        Container = new ClientRegistrationData
                        {
                            SensorToken = this.sensorToken
                        }
                    };
                    var json = JsonConvert.SerializeObject(readyMsg);
                    context.Send(json);
                }
                else if (msg.Kind == OnlineMeasMessageKind.DeviceServerMeasResult)
                {
                    Console.WriteLine($"Received data: get measurment result");
                    var jObject = msg.Container as Newtonsoft.Json.Linq.JObject;
                    var result = jObject.ToObject<DeviceServerResult>();
                    Console.WriteLine($" {result.Index}  -----: {result.Time}; Length {result.Levels_dB.Length}");
                }
            }
            catch  (Exception e)
            {
                Console.WriteLine(e);
            }

            
        }
    }
}
