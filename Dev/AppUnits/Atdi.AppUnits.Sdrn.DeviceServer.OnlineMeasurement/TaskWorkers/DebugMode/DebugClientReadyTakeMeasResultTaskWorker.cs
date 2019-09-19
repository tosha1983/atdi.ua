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
using Atdi.Common;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;
using Atdi.DataModels.Sdrns.Device;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results;



namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.TaskWorkers
{
    public class DebugClientReadyTakeMeasResultTaskWorker : ITaskWorker<DebugClientReadyTakeMeasResultTask, OnlineMeasurementProcess, PerThreadTaskWorkerLifetime>
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly ILogger _logger;

        public DebugClientReadyTakeMeasResultTaskWorker(
           ILogger logger,
           IController controller,
           AppServerComponentConfig config)
        {
            this._logger = logger;
            this._controller = controller;
            this._config = config;
        }


        public void Run(ITaskContext<DebugClientReadyTakeMeasResultTask, OnlineMeasurementProcess> context)
        {
            try
            {
                // тут поток измерений с передачей их на клиента 

                int i = 0;

                // тут зависит от логики - дял теста меряем пока не вылезет.... или клиент этого попросит сам
                while (true) // цыкл измерения - может в реальности быть немного другим но суть есть цыкл
                {

                    // обязательно опрос и выход если отменили - иначе зависним в измерениях на веки вечные!!!! 
                    if (context.Token.IsCancellationRequested)
                    {
                        context.Cancel();
                        return;
                    }

                    var count = context.Process.Parameters.Freq_Hz.Length;

                    var floatData = GenerateLevel(count); //GetFloatArray(count);
                    var r = new Random();
                    var measResult = new DeviceServerResultLevel
                    {
                        SensorToken = Guid.NewGuid().ToByteArray(),
                        Time = DateTime.Now,
                        Level = floatData,
                        Index = ++i,
                        Overload = r.Next(1, 100) > 45
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

        private float[] GenerateLevel(int count)
        {
            var data = new float[count];
            int point1 = count / 4;
            int point2 = count / 2;
            int point3 = point1 + point2;
            var r = new Random();
            for (int i = 0; i < point1; i++)
            {
                data[i] = -100 + (float)-5 * (float)r.NextDouble();
            }
            for (int i = point1; i < point2; i++)
            {
                float k = -100 + (-10 + 100) * (i - point1) / (point2 - point1);
                data[i] = k + (float)-10 * (float)r.NextDouble();
            }
            for (int i = point2; i < point3; i++)
            {
                float k = -10 + (10 - 100) * (i - point2) / (point3 - point2);
                data[i] = k + (float)-15 * (float)r.NextDouble();
            }
            for (int i = point3; i < data.Length; i++)
            {
                data[i] = -100 + (float)-20 * (float)r.NextDouble();
            }
            return data;
        }
    }
}
