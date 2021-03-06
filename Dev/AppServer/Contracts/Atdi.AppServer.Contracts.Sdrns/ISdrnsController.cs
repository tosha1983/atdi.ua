﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// The public contract of the service of the controller of the SDR Networks 
    /// </summary>
    [ServiceContract(Namespace = ServicesSpecification.Namespace)]
    public interface ISdrnsController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        Sensor GetSensor(SensorIdentifier sensorId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        Sensor[] GetSensors(DataConstraint constraint, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortSensor GetShortSensor(SensorIdentifier sensorId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortSensor[] GetShortSensors(DataConstraint constraint, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasTaskIdentifier CreateMeasTask(MeasTask task, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationResult DeleteMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationResult RunMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationResult StopMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasTask GetShortMeasTask(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasTask[] GetShortMeasTasks(DataConstraint constraint, CommonOperationArguments otherArgs);

                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults[] GetMeasResultsByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResults(DataConstraint constraint, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults GetShortMeasResultsById(MeasurementResultsIdentifier measResultsId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResultsByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        CommonOperationDataResult<int> DeleteMeasResults(MeasurementResultsIdentifier MeasResultsId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType measurementType, CommonOperationArguments otherArgs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortResultsMeasurementsStation[] GetShortMeasResStation(int MeasResultsId, CommonOperationArguments otherArgs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        Route[] GetRoutes(int MeasResultsId, CommonOperationArguments otherArgs);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        SensorPoligonPoint[] GetSensorPoligonPoint(int MeasResultsId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="StationId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultsMeasurementsStation[] GetResMeasStation(int MeasResultsId, int StationId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        SOFrequency[] GetSOformMeasResultStation(GetSOformMeasResultStationValue options, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResultsExtend[] GetShortMeasResultsByDate(GetShortMeasResultsByDateValue constraint, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measResultsId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults[] GetMeasResultsHeaderByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="StationId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultsMeasurementsStation GetResMeasStationById(int StationId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MeasResultsId"></param>
        /// <param name="StationId"></param>
        /// <returns></returns>
        [OperationContract]
        ResultsMeasurementsStationExtended[] GetResMeasStationHeaderByResId(int ResId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasurementResults GetMeasurementResultByResId(int ResId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        MeasTask GetMeasTaskHeader(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(MeasTaskIdentifier taskId, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelParams"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        StationLevelsByTask[] GetStationLevelsByTask(LevelsByTaskParams levelParams, CommonOperationArguments otherArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurementType"></param>
        /// <param name="otherArgs"></param>
        /// <returns></returns>
        [OperationContract]
        ShortMeasurementResultsExtend[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, int taskId, CommonOperationArguments otherArgs);

    }
}
