using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Tasks;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.Results;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeviceServer.OnlineMeasurement.TaskWorkers
{
    public class DebugClientTaskRegistrationTaskWorker : ITaskWorker<DebugClientTaskRegistrationTask, OnlineMeasurementProcess, PerThreadTaskWorkerLifetime>
    {
        private readonly AppServerComponentConfig _config;
        private readonly IController _controller;
        private readonly ILogger _logger;

        public DebugClientTaskRegistrationTaskWorker(AppServerComponentConfig config, IController controller, ILogger logger)
        {
            this._config = config;
            this._controller = controller;
            this._logger = logger;
        }

        private double[] GenerateFreqMHz(int count)
        {
            var data = new double[count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 935.554 + 0.00001 * i;
            }
            return data;
        }

        public void Run(ITaskContext<DebugClientTaskRegistrationTask, OnlineMeasurementProcess> context)
        {
            try
            {


                var serverParams = new DeviceServerParametersDataLevel
                {
                    SensorToken = Guid.NewGuid().ToByteArray(),
                    Att_dB = context.Process.MeasTask.Att_dB,
                    PreAmp_dB = context.Process.MeasTask.PreAmp_dB,
                    RefLevel_dBm = context.Process.MeasTask.RefLevel_dBm,
                    RBW_kHz = context.Process.MeasTask.RBW_kHz,
                    Freq_Hz = GenerateFreqMHz(10000),
                    isChanged_Att_dB = false,
                    isChanged_PreAmp_dB = false,
                    isChanged_RBW_kHz = false,
                    isChanged_RefLevel_dBm = false
                };
                context.Process.Parameters = serverParams;



                context.Finish();
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
        }
    }
}
