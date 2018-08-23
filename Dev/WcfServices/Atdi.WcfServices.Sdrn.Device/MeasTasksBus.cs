using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.Contracts.WcfServices.Sdrn.Device;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.Sdrns.Device;
using Atdi.Platform.Logging;

namespace Atdi.WcfServices.Sdrn.Device
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MeasTasksBus : WcfServiceBase<IMeasTasksBus>, IMeasTasksBus
    {

        private readonly ILogger _logger;

        public MeasTasksBus(ILogger logger)
        {
            this._logger = logger;
        }


        public Result<DeviceCommand[]> GetCommands(SensorDescriptor sensorDescriptor)
        {
            throw new NotImplementedException();
        }

        public Result<MeasTask[]> GetMeasTasks(SensorDescriptor sensorDescriptor)
        {
            throw new NotImplementedException();
        }

        public Result<SensorRegistrationResult[]> GetRegistrationResults(SensorDescriptor sensorDescriptor)
        {
            throw new NotImplementedException();
        }

        public Result SendCommandResults(DeviceCommandResult[] results)
        {
            throw new NotImplementedException();
        }

        public Result SendMeasResults(MeasResults[] results)
        {
            throw new NotImplementedException();
        }

        public Result TryRegister(Sensor sensor, string sdrnServer)
        {
            throw new NotImplementedException();
        }
    }
}
