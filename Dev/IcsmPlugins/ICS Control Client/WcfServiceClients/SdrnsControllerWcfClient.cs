using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            if (result == null)
            {
                return new ShortSensor[] { };
            }
            return result;
        }
        public static Sensor[] GetSensors(ComplexCondition condition)
        {
            var result = Execute(contract => contract.GetSensors(condition));

            if (result == null)
            {
                return new Sensor[] { };
            }
            return result;
        }

        public static ShortMeasTask[] GetShortMeasTasks()
        {
            var result = Execute(contract => contract.GetShortMeasTasks());

            if (result == null)
            {
                return new ShortMeasTask[] { };
            }
            return result;
        }

        public static ShortMeasurementResults[] GetShortMeasResultsByTask(int taskId)
        {
            var result = Execute(contract => contract.GetShortMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }));

            if (result == null)
            {
                return new ShortMeasurementResults[] { };
            }

            return result;
        }
        public static ShortMeasurementResults[] GetShortMeasResultsByDates(DateTime startDate, DateTime stopDate)
        {
            var dates = new GetShortMeasResultsByDateValue()
            {
                Start = startDate,
                End = stopDate
            };

            var result = Execute(contract => contract.GetShortMeasResultsByDate(dates));

            if (result == null)
            {
                return new ShortMeasurementResults[] { };
            }
            return result;
        }
        public static MeasurementResults[] GetMeasResultsByTask(int taskId)
        {
            var result = Execute(contract => contract.GetMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }));

            if (result == null)
            {
                return new MeasurementResults[] { };
            }
            return result;
        }
      
        public static Sensor GetSensorById(long sensorId)
        {
            var result = Execute(contract => contract.GetSensor(new SensorIdentifier { Value = sensorId }));

            return result;
        }
        public static ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType)
        {
            var result = Execute(contract => contract.GetShortMeasResultsSpecial(measurementType));

            if (result == null)
            {
                return new ShortMeasurementResults[] { };
            }
            return result;
        }
        public static MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType measurementType)
        {
            var result = Execute(contract => contract.GetMeasResultsHeaderSpecial(measurementType));

            if (result == null)
            {
                return new MeasurementResults[] { };
            }
            return result;
        }
        public static ShortResultsMeasurementsStation[] GetShortMeasResStation(int MeasResultsId)
        {
            var result = Execute(contract => contract.GetShortMeasResStation(MeasResultsId));

            if (result == null)
            {
                return new ShortResultsMeasurementsStation[] { };
            }
            return result;
        }
        public static Route[] GetRoutes(long MeasResultsId)
        {
            var result = Execute(contract => contract.GetRoutes(MeasResultsId));

            if (result == null)
            {
                return new Route[] { };
            }
            return result;
        }
        public static SensorPoligonPoint[] GetSensorPoligonPoint(long MeasResultsId)
        {
            var result = Execute(contract => contract.GetSensorPoligonPoint(MeasResultsId));

            if (result == null)
            {
                return new SensorPoligonPoint[] { };
            }
            return result;
        }
        public static ResultsMeasurementsStation[] GetResMeasStation(long MeasResultsId, long StationId)
        {
            var result = Execute(contract => contract.GetResMeasStation(MeasResultsId, StationId));

            if (result == null)
            {
                return new ResultsMeasurementsStation[] { };
            }
            return result;
        }
        public static SOFrequency[] GetSOformMeasResultStation(List<double> Frequencies_MHz, double BW_kHz, List<int> MeasResultID, double LonMax, double LonMin, double LatMax, double LatMin, double TrLevel_dBm)
        {
            var parameters = new GetSOformMeasResultStationValue()
            {
                BW_kHz = BW_kHz,
                Frequencies_MHz = Frequencies_MHz,
                LatMax = LatMax,
                LatMin = LatMin,
                LonMax = LonMax,
                LonMin = LonMin,
                TrLevel_dBm = TrLevel_dBm,
                MeasResultID = MeasResultID
            };

            var result = Execute(contract => contract.GetSOformMeasResultStation(parameters));

            if (result == null)
            {
                return new SOFrequency[] { };
            }
            return result;
        }
        public static MeasurementResults[] GetMeasResultsHeaderByTaskId(long taskId)
        {
            var result = Execute(contract => contract.GetMeasResultsHeaderByTaskId(new MeasTaskIdentifier { Value = taskId }));

            if (result == null)
            {
                return new MeasurementResults[] { };
            }
            return result;
        }
        public static ResultsMeasurementsStation GetResMeasStationById(long stationId)
        {
            var result = Execute(contract => contract.GetResMeasStationById(stationId));

            return result;
        }
        public static ResultsMeasurementsStation[] GetResMeasStationHeaderByResId(long resId)
        {
            var result = Execute(contract => contract.GetResMeasStationHeaderByResId(resId));

            if (result == null)
            {
                return new ResultsMeasurementsStation[] { };
            }
            return result;
        }
        public static MeasurementResults GetMeasurementResultByResId(long resId, double? StartFrequency_Hz, double? StopFrequency_Hz)
        {
            return Execute(contract => contract.GetMeasurementResultByResId(resId, true, StartFrequency_Hz, StopFrequency_Hz));
        }

        public static Emitting[] GetEmittingsByIcsmId(long[] stations, string tableName)
        {
            var result = Execute(contract => contract.GetEmittingsByIcsmId(stations, tableName));

            if (result == null)
            {
                return new Emitting[] { };
            }
            return result;
        }
        public static SignalingSysInfo[] GetSignalingSysInfos(long measResultId, double freq_MHz)
        {
            var result = Execute(contract => contract.GetSignalingSysInfos(measResultId, freq_MHz));
            if (result == null)
            {
                return new SignalingSysInfo[] { };
            }
            return result;
        }

        public static MeasurementResults GetMeasurementResultByResId(long resId)
        {
            var isLoadAllData = false;
            double? start = 0;
            double? stop = 0;
            var result1 = Execute(contract => contract.GetMeasurementResultByResId(resId, isLoadAllData, start, stop));

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
        public static StationLevelsByTask[] GetStationLevelsByTask(List<long> MeasResultID, int MeasTaskId, int SectorId)
        {
            var parameters = new LevelsByTaskParams()
            {
                MeasResultID = MeasResultID,
                MeasTaskId = MeasTaskId,
                SectorId = SectorId
            };
            var result = Execute(contract => contract.GetStationLevelsByTask(parameters));

            if (result == null)
            {
                return new StationLevelsByTask[] { };
            }
            return result;
        }
        public static ShortMeasurementResults[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, int taskId)
        {
            var result = Execute(contract => contract.GetShortMeasResultsByTypeAndTaskId(measurementType, taskId));

            if (result == null)
            {
                return new ShortMeasurementResults[] { };
            }
            return result;
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
        public static void AddAssociationStationByEmitting(long[] emittingsId, long AssociatedStationID, string AssociatedStationTableName)
        {
            var result = Execute(contract => contract.AddAssociationStationByEmitting(emittingsId, AssociatedStationID, AssociatedStationTableName));
            if (!result)
            {
                System.Windows.Forms.MessageBox.Show("Unknown error", "Add association station by emitting faild!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
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
