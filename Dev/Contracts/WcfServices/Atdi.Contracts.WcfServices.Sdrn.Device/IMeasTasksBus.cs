using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.CommonOperation;
using System.Runtime.Serialization;

namespace Atdi.Contracts.WcfServices.Sdrn.Device
{
    /// <summary>
    /// Represents the result of the operation with the returned data of type TReturnedData and with the common state:  Success or Fault
    /// </summary>
    /// <typeparam name="TReturnedData">The type of the returned data of the operation</typeparam>
    [DataContract(Namespace = Specification.Namespace)]
    public class BusResult<TReturnedData> : Result<TReturnedData>
    {
        /// <summary>
        /// The returned data of the operation
        /// </summary>
        [DataMember]
        public byte[] Token { get; set; }
    }

    /// <summary>
    /// The public contract of the WCF-service of the Device Integration Bus
    /// </summary>
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface IMeasTasksBus
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="sdrnServer"></param>
        /// <returns></returns>
        [OperationContract]
        Result<SensorRegistrationResult> RegisterSensor(Sensor sensor, string sdrnServer);

        /// <summary>
        /// Update the Sensor parameters
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="sdrnServer"></param>
        /// <returns></returns>
        [OperationContract]
        Result<SensorUpdatingResult> UpdateSensor(Sensor sensor, string sdrnServer);

        /// <summary>
        /// Requests and obtains the device command from the Server
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BusResult<DeviceCommand> GetCommand(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// Confirms the correspondent answer has been received from the Server
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckCommand(SensorDescriptor sensorDescriptor, byte[] token);

        /// <summary>
        /// Sends the result of the command, previously received from the Server
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result SendCommandResult(SensorDescriptor sensorDescriptor, DeviceCommandResult commandResult);

        /// <summary>
        /// Requests and obtains the measurement task from the Server
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BusResult<MeasTask> GetMeasTask(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// Confirms the correspondent answer has been received from the Server
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckMeasTask(SensorDescriptor sensorDescriptor, byte[] token);

        /// <summary>
        /// Send the measurement results to the Server
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result SendMeasResults(SensorDescriptor sensorDescriptor, MeasResults results);

        /// <summary>
        /// Sends a universal object to the Server
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [OperationContract]
        Result SendEntity(SensorDescriptor sensorDescriptor, Entity entity);

        /// <summary>
        /// Sends a part of universal object
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="entityPart"></param>
        /// <returns></returns>
        [OperationContract]
        Result SendEntityPart(SensorDescriptor sensorDescriptor, EntityPart entityPart);

        /// <summary>
        /// Requests and obtains a universal object from the Server
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <returns></returns>
        [OperationContract]
        BusResult<Entity> GetEntity(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// Requests and obtains a part of universal object, previously obtained from the Server
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <returns></returns>
        [OperationContract]
        BusResult<EntityPart> GetEntityPart(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// Confirms the correspondent answer has been received from the Server
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckEntity(SensorDescriptor sensorDescriptor, byte[] token);

        /// <summary>
        /// Confirms the correspondent answer has been received from the Server
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckEntityPart(SensorDescriptor sensorDescriptor, byte[] token);
    }
}
