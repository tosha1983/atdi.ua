using Atdi.Common;
using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.Platform.Logging;
using System;
using DM = Atdi.DataModels.Sdrns.Device;
using System.Threading;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.EntityOrm;



namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing
{
    public class SensorActivityWorker : ITaskWorker<ActiveSensorTask, DispatchProcess, SingletonTaskWorkerLifetime>
    {
        private readonly IBusGate _busGate;
        private readonly ILogger _logger;
        private readonly ConfigProcessing _config;

        public SensorActivityWorker(
            ConfigProcessing config,
            ILogger logger,
            IBusGate busGate)
        {

            this._logger = logger;
            this._busGate = busGate;
            this._config = config;
        }


        public void Run(ITaskContext<ActiveSensorTask, DispatchProcess> context)
        {
            try
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(this._config.SleepTimePeriodSendActivitySensor_ms);

                    DM.DeviceCommandResult deviceCommandResult = new DM.DeviceCommandResult();
                    deviceCommandResult.CommandId = "SendActivitySensor";
                    deviceCommandResult.CustDate1 = DateTime.Now;
                    deviceCommandResult.CustTxt1 = "";

                    var publisher = this._busGate.CreatePublisher("main");
                    publisher.Send<DM.DeviceCommandResult>("SendCommandResult", deviceCommandResult);
                    publisher.Dispose();
                }
            }
            catch (Exception e)
            {
                context.Abort(e);
            }
        }

      
    }
}
