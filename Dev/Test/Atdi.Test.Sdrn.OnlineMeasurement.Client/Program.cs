using Atdi.Contracts.WcfServices.Sdrn.Server;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClientImpl;

namespace Atdi.Test.Sdrn.OnlineMeasurement.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start SDRN Server WCF Service Client test ...");
            Console.ReadLine();

            var sdrnServer = GetServicByEndpoint("SdrnServerNetTcpEndpoint");

            var initResult = sdrnServer.InitOnlineMeasurement(new OnlineMeasurementOptions
            {
                SensorId = 4,
                Period = new TimeSpan(0, 10, 0)
            });

            if (initResult.Allowed)
            {
                Console.WriteLine($"Init Result: Online Measurement is Allowed: ServerToken = '{new Guid(initResult.ServerToken)}'");

                var cancelled = false;
                var status = default(SensorAvailabilityDescriptor);

                while(!cancelled)
                {
                    Thread.Sleep(3000);

                    status = sdrnServer.GetSensorAvailabilityForOnlineMesurement(initResult.ServerToken);

                    cancelled = status.Status != OnlineMeasurementStatus.Initiation && status.Status != OnlineMeasurementStatus.WaitSensor;
                }

                if (status.Status == OnlineMeasurementStatus.SonsorReady)
                {
                    Console.WriteLine($"SENSOR: WebSocket was opened '{status.Status}', URL = '{status.WebSocketUrl}'");

                    var handler = new WebSocketHandler(status.SensorToken);

                    var webSocket = new WebSocketClient(new Uri(status.WebSocketUrl), handler);
                    var context = webSocket.Connect();

                    var message = new OnlineMeasMessage
                    {
                        Kind = OnlineMeasMessageKind.ClientTaskRegistration,
                        Container = new ClientMeasTaskData
                        {
                            SensorToken = status.SensorToken,
                            Att_dB = 21,
                            DetectorType = DetectorType.Auto,
                            FreqStart_MHz = 2164,
                            FreqStop_MHz = 2169,
                            OnlineMeasType = OnlineMeasType.Level,
                            PreAmp_dB = 11,
                            RBW_kHz = 1,
                            RefLevel_dBm = -1,
                            SweepTime_s = 0.001,
                            TraceCount = 10,
                            TraceType = TraceType.Auto
                        }
                    };
                    var json = JsonConvert.SerializeObject(message);
                    context.Send(json);

                    Console.WriteLine($"Send task data to SENSOR: {json}");
                    Console.WriteLine($"Wait data from SENSOR");
                    Console.ReadLine();

                    // Отправляем запрос на остановку потока данных
                    var message2 = new OnlineMeasMessage
                    {
                        Kind = OnlineMeasMessageKind.ClientTaskCancellation,
                        Container = new ClientTaskCancellationData
                        {
                            SensorToken = status.SensorToken
                        }
                    };
                    var json2 = JsonConvert.SerializeObject(message2);
                    context.Send(json2);

                    Console.WriteLine($"Send TaskCancellation message to SENSOR");
                    Console.ReadLine();

                    Console.WriteLine($"Press any key to close WebSocket with SENSOR.");

                    webSocket.Dispose();
                }
                else
                {
                    Console.WriteLine($"SENSOR: {status.Status}, '{status.Message}'");
                }
            }
            else
            {
                Console.WriteLine($"Init Result: Online Measurement is not Allowed: {initResult.Message}");
            }
            Console.WriteLine($"Test was finished. Press any key to exit.");
            Console.ReadLine();
        }

        static ISdrnsController GetServicByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<ISdrnsController>(endpointName);
            return f.CreateChannel();
        }
    }
}
