using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels.DataConstraint;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    /// <summary>
    /// The public contract of the service of the controller of the SDR Networks 
    /// </summary>
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface ISdrnsController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        Sensor GetSensor(SensorIdentifier sensorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        Sensor[] GetSensors(ComplexCondition condition);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortSensor GetShortSensor(SensorIdentifier sensorId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortSensor[] GetShortSensors();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasTaskIdentifier CreateMeasTask(MeasTask task);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationResult DeleteMeasTask(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationResult RunMeasTask(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationResult StopMeasTask(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasTask GetShortMeasTask(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasTask[] GetShortMeasTasks();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults[] GetMeasResultsByTaskId(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResults();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults GetShortMeasResultsById(MeasurementResultsIdentifier measResultsId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResultsByTaskId(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationDataResult<int> DeleteMeasResults(MeasurementResultsIdentifier MeasResultsId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType measurementType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortResultsMeasurementsStation[] GetShortMeasResStation(long MeasResultsId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        Route[] GetRoutes(long MeasResultsId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        SensorPoligonPoint[] GetSensorPoligonPoint(long MeasResultsId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="StationId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultsMeasurementsStation[] GetResMeasStation(long MeasResultsId, long StationId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        SOFrequency[] GetSOformMeasResultStation(GetSOformMeasResultStationValue options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResultsByDate(GetShortMeasResultsByDateValue constraint);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults[] GetMeasResultsHeaderByTaskId(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="StationId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultsMeasurementsStation GetResMeasStationById(long StationId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="StationId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(long ResId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults GetMeasurementResultByResId(long ResId, bool isLoadAllData, double? StartFrequency_Hz, double? StopFrequency_Hz);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasTask GetMeasTaskHeader(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasTask GetMeasTaskById(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(MeasTaskIdentifier taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelParams"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        StationLevelsByTask[] GetStationLevelsByTask(LevelsByTaskParams levelParams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, long taskId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resId"></param>
        /// <param name="StartFrequency_Hz"></param>
        /// <param name="StopFrequency_Hz"></param>
        /// <returns></returns>
        [OperationContract]
        ReferenceLevels GetReferenceLevelsByResultId(long resId, bool isLoadAllData, double? StartFrequency_Hz, double? StopFrequency_Hz);

        /// <summary>
        /// Delete emittings
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        bool DeleteEmitting(long[] emittingsId);

        /// <summary>
        /// Add association station by emitting
        /// </summary>
        /// <param name="emittingId"></param>
        /// <param name="AssociatedStationID"></param>
        /// <param name="AssociatedStationTableName"></param>
        /// <returns></returns>
        [OperationContract]
        bool AddAssociationStationByEmitting(long[] emittingsId, long AssociatedStationID, string AssociatedStationTableName);

        /// <summary>
        /// Get Emittings by IcsmId identifiers
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="icsmTableName"></param>
        /// <returns></returns>
        [OperationContract]
        Emitting[] GetEmittingsByIcsmId(long[] ids, string icsmTableName);


        [OperationContract]

        SignalingSysInfo[] GetSignalingSysInfos(long measResultId, double freq_MHz);
    }
}
