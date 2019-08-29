using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.WebSocket;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.TaskWorkers
{
    public class ClientReadyTakeMeasResultTaskWorker : ITaskWorker<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess, PerThreadTaskWorkerLifetime>
    {

        

        public void Run(ITaskContext<ClientReadyTakeMeasResultTask, OnlineMeasurementProcess> context)
        {
            try
            {
                // тут поток измерений с передачей их на клиента 
                
                int i = 0;

                // тут зависит от логики - дял теста меряем пока не вылезет.... или клиент этого попросит сам
                while (true)
                {

                    // обязательно опрос и выход если отменили - иначе зависним в измерениях на веки вечные!!!! 
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        return;
                    }

                    var floatData = GetFloatArray(10000);
                    var measResult = new DeviceServerResult
                    {
                        SensorToken = Guid.NewGuid().ToByteArray(),
                        Time = DateTime.Now,
                        Levels_dB = floatData,
                        Index = ++i
                    };

                    // так оборачиваем результат
                    var message = new OnlineMeasMessage
                    {
                        Kind = OnlineMeasMessageKind.DeviceServerMeasResult,
                        Container = measResult
                    };

                    // и  отправляем его
                    context.Process.Publisher.Send(message);
                }

                //context.Finish();
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
            
        }

        public float[] GetFloatArray(int count)
        {
            var r = new Random();
            var data = new float[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = (float)r.NextDouble();
            }
            return data;
        }
    }
}
