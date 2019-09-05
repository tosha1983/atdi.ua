using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks
{
    public class OnlineMeasurementProcess : ProcessBase
    {
        public OnlineMeasurementProcess()
            : base("Online Measurement")
        {
        }

        public OnlineMeasurementProcess(IProcess parentProcess)
            : base("Online Measurement", parentProcess)
        {
        }

        // Первая фаза: объект задачи по которой устройство должно сделать измерения
        //  в ответ устройство в свойство Parameters ложить описание результатов
        public ClientMeasTaskData MeasTask { get; set; }

        public int CountMeasurementDone { get; set; }
      

        public byte[] SensorToken { get; set; }

        // Фаза готовности клиента получать результаты измерения
        public ClientReadyData ReadyData { get; set; }

        public DeviceServerParametersDataLevel Parameters { get; set; }

        public WebSocketPublisher Publisher { get; set; }
    }
}
