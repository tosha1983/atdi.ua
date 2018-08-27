using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.CommonOperation;

namespace Atdi.Contracts.WcfServices.Sdrn.Device
{
    /// <summary>
    /// The public contract of the WCF-service of the Device Integration Bus
    /// </summary>
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface IMeasTasksBus
    {
        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        Result TryRegister(Sensor sensor, string sdrnServer);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Result<SensorRegistrationResult[]> GetRegistrationResults(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Result<DeviceCommand[]> GetCommands(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Result SendCommandResults(DeviceCommandResult[] results);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Result<MeasTask[]> GetMeasTasks(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Result SendMeasResults(MeasResults[] results);
    }
}
