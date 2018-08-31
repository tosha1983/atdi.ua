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
        /// 
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="sdrnServer"></param>
        /// <returns></returns>
        [OperationContract]
        Result<SensorUpdatingResult> UpdateSensor(Sensor sensor, string sdrnServer);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BusResult<DeviceCommand> GetCommand(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckCommand(SensorDescriptor sensorDescriptor, byte[] token);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result SendCommandResult(SensorDescriptor sensorDescriptor, DeviceCommandResult commandResult);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BusResult<MeasTask> GetMeasTask(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckMeasTask(SensorDescriptor sensorDescriptor, byte[] token);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result SendMeasResults(SensorDescriptor sensorDescriptor, MeasResults results);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [OperationContract]
        Result SendEntity(SensorDescriptor sensorDescriptor, Entity entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="entityPart"></param>
        /// <returns></returns>
        [OperationContract]
        Result SendEntityPart(SensorDescriptor sensorDescriptor, EntityPart entityPart);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <returns></returns>
        [OperationContract]
        BusResult<Entity> GetEntity(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <returns></returns>
        [OperationContract]
        BusResult<EntityPart> GetEntityPart(SensorDescriptor sensorDescriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckEntity(SensorDescriptor sensorDescriptor, byte[] token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorDescriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [OperationContract]
        Result AckEntityPart(SensorDescriptor sensorDescriptor, byte[] token);
    }
}
