using System;
using System.Collections.Generic;
using ICSM;
using XICSM.ICSControlClient.Environment;
using Atdi.DataModels.DataConstraint;
using Atdi.Contracts.WcfServices.Sdrn.Server;

namespace XICSM.ICSControlClient.WcfServiceClients
{
    public class SdrnsControllerWcfClient : WcfServiceClientBase<ISdrnsController, SdrnsControllerWcfClient>
    {
        public SdrnsControllerWcfClient() : base("SdrnsController") { }

        #region Getters
        public static ShortSensor[] GetShortSensors()
        {
            var result = Execute(contract => contract.GetShortSensors());

            return result ?? new ShortSensor[] { };
        }
        public static Sensor[] GetSensors(ComplexCondition condition)
        {
            var result = Execute(contract => contract.GetSensors(condition));

            return result ?? new Sensor[] { };
        }

        public static ShortMeasTask[] GetShortMeasTasks()
        {
            var result = Execute(contract => contract.GetShortMeasTasks());

            return result ?? new ShortMeasTask[] { };
        }

        public static ShortMeasurementResults[] GetShortMeasResultsByTask(int taskId)
        {
            var result = Execute(contract => contract.GetShortMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }));

            return result ?? new ShortMeasurementResults[] { };
        }
        public static ShortMeasurementResults[] GetShortMeasResultsByDates(DateTime startDate, DateTime stopDate)
        {
            var dates = new GetShortMeasResultsByDateValue()
            {
                Start = startDate,
                End = stopDate
            };

            var result = Execute(contract => contract.GetShortMeasResultsByDate(dates));

            return result ?? new ShortMeasurementResults[] { };
        }
        public static MeasurementResults[] GetMeasResultsByTask(int taskId)
        {
            var result = Execute(contract => contract.GetMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }));

            return result ?? new MeasurementResults[] { };
        }

        public static Sensor GetSensorById(long sensorId)
        {
            var result = Execute(contract => contract.GetSensor(new SensorIdentifier { Value = sensorId }));

            return result;
        }
        public static ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType)
        {
            var result = Execute(contract => contract.GetShortMeasResultsSpecial(measurementType));

            return result ?? new ShortMeasurementResults[] { };
        }
        public static MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType measurementType)
        {
            var result = Execute(contract => contract.GetMeasResultsHeaderSpecial(measurementType));

            return result ?? new MeasurementResults[] { };
        }
        public static ShortResultsMeasurementsStation[] GetShortMeasResStation(int measResultsId)
        {
            var result = Execute(contract => contract.GetShortMeasResStation(measResultsId));

            return result ?? new ShortResultsMeasurementsStation[] { };
        }
        public static Route[] GetRoutes(long measResultsId)
        {
            var result = Execute(contract => contract.GetRoutes(measResultsId));

            return result ?? new Route[] { };
        }
        public static SensorPoligonPoint[] GetSensorPoligonPoint(long measResultsId)
        {
            var result = Execute(contract => contract.GetSensorPoligonPoint(measResultsId));

            return result ?? new SensorPoligonPoint[] { };
        }
        public static ResultsMeasurementsStation[] GetResMeasStation(long measResultsId, long stationId)
        {
            var result = Execute(contract => contract.GetResMeasStation(measResultsId, stationId));

            return result ?? new ResultsMeasurementsStation[] { };
        }
        public static SOFrequency[] GetSOformMeasResultStation(List<double> frequenciesMHz, double bwKHz, List<int> measResultId, double lonMax, double lonMin, double latMax, double latMin, double trLevelDBm)
        {
            var parameters = new GetSOformMeasResultStationValue()
            {
                BW_kHz = bwKHz,
                Frequencies_MHz = frequenciesMHz,
                LatMax = latMax,
                LatMin = latMin,
                LonMax = lonMax,
                LonMin = lonMin,
                TrLevel_dBm = trLevelDBm,
                MeasResultID = measResultId
            };

            var result = Execute(contract => contract.GetSOformMeasResultStation(parameters));

            return result ?? new SOFrequency[] { };
        }
        public static MeasurementResults[] GetMeasResultsHeaderByTaskId(long taskId)
        {
            var result = Execute(contract => contract.GetMeasResultsHeaderByTaskId(new MeasTaskIdentifier { Value = taskId }));

            return result ?? new MeasurementResults[] { };
        }
        public static ResultsMeasurementsStation GetResMeasStationById(long stationId)
        {
            var result = Execute(contract => contract.GetResMeasStationById(stationId));

            return result;
        }
        public static ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(long resId)
        {
            var result = Execute(contract => contract.GetResMeasStationHeaderByResId(resId));

            return result ?? new ResultsMeasurementsStation[] { };
        }
        public static ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(long resId, ResultsMeasurementsStationFilters filter)
        {
            var result = Execute(contract => contract.GetResMeasStationHeaderByResIdWithFilter(resId, filter));

            return result ?? new ResultsMeasurementsStation[] { };
        }
        public static MeasurementResults GetMeasurementResultByResId(long resId, double? startFrequencyHz, double? stopFrequencyHz)
        {
            return Execute(contract => contract.GetMeasurementResultByResId(resId, true, startFrequencyHz, stopFrequencyHz));
        }

        public static Emitting[] GetEmittingsByIcsmId(long[] stations, string tableName)
        {
            var result = Execute(contract => contract.GetEmittingsByIcsmId(stations, tableName));

            return result ?? new Emitting[] { };
        }
        public static SignalingSysInfo[] GetSignalingSysInfos(long measResultId, double freqMHz)
        {
            var result = Execute(contract => contract.GetSignalingSysInfos(measResultId, freqMHz));
            return result ?? new SignalingSysInfo[] { };
        }

        public static MeasurementResults GetMeasurementResultByResId(long resId)
        {
            double? start = 0;
            double? stop = 0;
            var result1 = Execute(contract => contract.GetMeasurementResultByResId(resId, false, start, stop));

            //isLoadAllData = true;
            //var result2 = Execute(contract => contract.GetMeasurementResultByResId(resId, isLoadAllData, start, stop));
            return result1;
        }

        public static MeasTask GetMeasTaskHeaderById(long taskId)
        {
            var result = Execute(contract => contract.GetMeasTaskHeader(new MeasTaskIdentifier { Value = taskId }));

            return result;
        }
        public static StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(long taskId)
        {
            var result = Execute(contract => contract.GetStationDataForMeasurementsByTaskId(new MeasTaskIdentifier { Value = taskId }));

            if (result == null)
            {
                return new StationDataForMeasurements[] { };
            }
            return result;
        }
        public static StationLevelsByTask[] GetStationLevelsByTask(List<long> measResultId, int measTaskId, int SectorId)
        {
            var parameters = new LevelsByTaskParams()
            {
                MeasResultID = measResultId,
                MeasTaskId = measTaskId,
                SectorId = SectorId
            };
            var result = Execute(contract => contract.GetStationLevelsByTask(parameters));

            return result ?? new StationLevelsByTask[] { };
        }
        public static ShortMeasurementResults[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, int taskId)
        {
            var result = Execute(contract => contract.GetShortMeasResultsByTypeAndTaskId(measurementType, taskId));

            return result ?? new ShortMeasurementResults[] { };
        }
        public static MeasTask GetMeasTaskById(long taskId)
        {
            return Execute(contract => contract.GetMeasTaskById(taskId));
        }

        #endregion

        #region Actions
        public static long CreateMeasTask(MeasTask task)
        {
            var result = Execute(contract => contract.CreateMeasTask(task));

            if (result == null)
            {
                return IM.NullI;
            }
            return result.Value;
        }
        public static void DeleteMeasTaskById(long taskId)
        {
            var result = Execute(contract => contract.DeleteMeasTask(new MeasTaskIdentifier { Value = taskId }));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Delete the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void RunMeasTask(long taskId)
        {
            var result = Execute(contract => contract.RunMeasTask(new MeasTaskIdentifier { Value = taskId }));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Run the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void StopMeasTask(long taskId)
        {
            var result = Execute(contract => contract.StopMeasTask(new MeasTaskIdentifier { Value = taskId }));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Stop the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void DeleteEmittingById(long[] emittingId)
        {
            var result = Execute(contract => contract.DeleteEmitting(emittingId));

            if (!result)
            {
                System.Windows.Forms.MessageBox.Show("Unknown error", "Delete Emittings faild!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void AddAssociationStationByEmitting(long[] emittingsId, long associatedStationId, string AssociatedStationTableName)
        {
            var result = Execute(contract => contract.AddAssociationStationByEmitting(emittingsId, associatedStationId, AssociatedStationTableName));
            if (!result)
            {
                System.Windows.Forms.MessageBox.Show("Unknown error", "Add association station by emitting faild!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static bool UpdateSensorTitle(long id, string title)
        {
            var result = Execute(contract => contract.UpdateSensorTitle(id, title));

            if (!result)
            {
                System.Windows.Forms.MessageBox.Show("Unknown error", $"Update sensor with Id #{id}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            return result;
        }
        public static OnlineMeasurementInitiationResult InitOnlineMeasurement(long sensorId, TimeSpan period)
        {
            var options = new OnlineMeasurementOptions
            {
                Period = period,
                SensorId = sensorId
            };

            var result = Execute(contract => contract.InitOnlineMeasurement(options));

            return result;
        }
        public static SensorAvailabilityDescriptor GetSensorAvailabilityForOnlineMesurement(byte[] serverToken)
        {
            var result = Execute(contract => contract.GetSensorAvailabilityForOnlineMesurement(serverToken));
            return result;
        }
        #endregion
    }
}
