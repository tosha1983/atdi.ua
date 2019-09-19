using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XICSM.ICSControlClient.OnlineMeasurement
{
    public interface IWebSocketMessageHandler
    {
        void OnDeviceServerParameters(MessageContainer data, WebSocketContext context);
        void OnDeviceServerMeasResult(MessageContainer data, WebSocketContext context);
        void OnDeviceServerCancellation(MessageContainer data, WebSocketContext context);
    }

    public class MessageContainer
    {
        private readonly JObject _data;

        public MessageContainer(JObject data)
        {
            this._data = data;
        }
        public T GetData<T>()
        {
            if (_data == null)
            {
                return default(T);
            }
            var result = _data.ToObject<T>();
            return result;
        }
    }
    public class WebSocketHandler
    {
        private readonly IWebSocketMessageHandler messageHandler;

        public WebSocketHandler(IWebSocketMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public void Handle(string data, WebSocketContext context)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<OnlineMeasMessage>(data);
                if (msg.Kind == OnlineMeasMessageKind.DeviceServerParameters)
                {
                    messageHandler.OnDeviceServerParameters(new MessageContainer((Newtonsoft.Json.Linq.JObject)msg.Container), context);
                    //var jObject = msg.Container as Newtonsoft.Json.Linq.JObject;
                    //var parameters = jObject.ToObject<DeviceServerParametersData>();
                    ////Console.WriteLine($"Received data: get measurment result parameters: Length = #{parameters.Frequencies.Length}");

                    //var readyMsg = new OnlineMeasMessage
                    //{
                    //    Kind = OnlineMeasMessageKind.ClientReadyTakeMeasResult,
                    //    Container = new ClientReadyData
                    //    {
                    //        SensorToken = this.sensorToken
                    //    }
                    //};
                    //var json = JsonConvert.SerializeObject(readyMsg);
                    //context.Send(json);
                }
                else if (msg.Kind == OnlineMeasMessageKind.DeviceServerMeasResult)
                {
                    //Console.WriteLine($"Received data: get measurment result");
                    //var jObject = msg.Container as Newtonsoft.Json.Linq.JObject;
                    //var result = jObject.ToObject<DeviceServerResult>();
                    //Console.WriteLine($" {result.Index}  -----: {result.Time}; Length {result.Levels_dB.Length}");
                    messageHandler.OnDeviceServerMeasResult(new MessageContainer((Newtonsoft.Json.Linq.JObject)msg.Container), context);
                }
                else if (msg.Kind == OnlineMeasMessageKind.DeviceServerCancellation)
                {
                    messageHandler.OnDeviceServerCancellation(new MessageContainer((Newtonsoft.Json.Linq.JObject)msg.Container), context);
                }
            }
            catch  (Exception e)
            {
                Console.WriteLine(e);
            }

            
        }
    }
}
