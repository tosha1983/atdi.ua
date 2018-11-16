using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSM;
using XICSM.ICSControlClient.Environment;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
using MD = XICSM.ICSControlClient.Metadata;

namespace XICSM.ICSControlClient.WcfServiceClients
{
    public class SdrnsControllerWcfClient : WcfServiceClientBase<ISdrnsController, SdrnsControllerWcfClient>
    {
        public SdrnsControllerWcfClient() : base("SdrnsController") { }

        #region Getters
        public static ShortSensor[] GetShortSensors(DataConstraint constraint)
        {
            var result = Execute(contract => contract.GetShortSensors(constraint, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ShortSensor[] { };
            }
            return result;
        }
        public static Sensor[] GetSensors(DataConstraint constraint)
        {
            var result = Execute(contract => contract.GetSensors(constraint, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new Sensor[] { };
            }
            return result;
        }

        public static ShortMeasTask[] GetShortMeasTasks(DataConstraint constraint)
        {
            var result = Execute(contract => contract.GetShortMeasTasks(constraint, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ShortMeasTask[] { };
            }
            return result;
        }

        public static ShortMeasurementResults[] GetShortMeasResultsByTask(int taskId)
        {
            var result = Execute(contract => contract.GetShortMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ShortMeasurementResults[] { };
            }

            return result;
        }
        public static ShortMeasurementResultsExtend[] GetShortMeasResultsByDates(DateTime startDate, DateTime stopDate)
        {
            var dates = new GetShortMeasResultsByDateValue()
            {
                Start = startDate,
                End = stopDate
            };

            var result = Execute(contract => contract.GetShortMeasResultsByDate(dates, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ShortMeasurementResultsExtend[] { };
            }
            return result;
        }
        public static MeasurementResults[] GetMeasResultsByTask(int taskId)
        {
            var result = Execute(contract => contract.GetMeasResultsByTaskId(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new MeasurementResults[] { };
            }
            return result;
        }
        //public static MeasTask GetMeasTaskById(int taskId)
        //{
        //    var result = Execute(contract => contract.GetMeasTask(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

        //    return result;
        //}
        public static MeasurementResults GetMeasResultsById(int measSdrResultsId, int measTaskId, int subMeasTaskId, int subMeasTaskStationId)
        {
            var id = new MeasurementResultsIdentifier
            {
                MeasSdrResultsId = measSdrResultsId,
                MeasTaskId = new MeasTaskIdentifier { Value = measTaskId },
                SubMeasTaskId = subMeasTaskId,
                SubMeasTaskStationId = subMeasTaskStationId
            };

            var result = Execute(contract => contract.GetMeasResultsById(id, GetDefaultOtherArgs()));

            return result;
        }
        public static Sensor GetSensorById(int sensorId)
        {
            var result = Execute(contract => contract.GetSensor(new SensorIdentifier { Value = sensorId }, GetDefaultOtherArgs()));

            return result;
        }
        public static ShortMeasurementResults[] GetShortMeasResultsSpecial(MeasurementType measurementType)
        {
            var result = Execute(contract => contract.GetShortMeasResultsSpecial(measurementType, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ShortMeasurementResults[] { };
            }
            return result;
        }
        public static MeasurementResults[] GetMeasResultsHeaderSpecial(MeasurementType measurementType)
        {
            var result = Execute(contract => contract.GetMeasResultsHeaderSpecial(measurementType, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new MeasurementResults[] { };
            }
            return result;
        }
        public static ShortResultsMeasurementsStation[] GetShortMeasResStation(int MeasResultsId)
        {
            var result = Execute(contract => contract.GetShortMeasResStation(MeasResultsId, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ShortResultsMeasurementsStation[] { };
            }
            return result;
        }
        public static Route[] GetRoutes(int MeasResultsId)
        {
            var result = Execute(contract => contract.GetRoutes(MeasResultsId, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new Route[] { };
            }
            return result;
        }
        public static SensorPoligonPoint[] GetSensorPoligonPoint(int MeasResultsId)
        {
            var result = Execute(contract => contract.GetSensorPoligonPoint(MeasResultsId, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new SensorPoligonPoint[] { };
            }
            return result;
        }
        public static ResultsMeasurementsStation[] GetResMeasStation(int MeasResultsId, int StationId)
        {
            var result = Execute(contract => contract.GetResMeasStation(MeasResultsId, StationId, GetDefaultOtherArgs()));

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

            var result = Execute(contract => contract.GetSOformMeasResultStation(parameters, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new SOFrequency[] { };
            }
            return result;
        }
        public static MeasurementResults[] GetMeasResultsHeaderByTaskId(int taskId)
        {
            var result = Execute(contract => contract.GetMeasResultsHeaderByTaskId(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new MeasurementResults[] { };
            }
            return result;
        }
        public static ResultsMeasurementsStation GetResMeasStationById(int stationId)
        {
            var result = Execute(contract => contract.GetResMeasStationById(stationId, GetDefaultOtherArgs()));

            return result;
        }
        public static ResultsMeasurementsStationExtended[] GetResMeasStationHeaderByResId(int resId)
        {
            var result = Execute(contract => contract.GetResMeasStationHeaderByResId(resId, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ResultsMeasurementsStationExtended[] { };
            }
            return result;
        }
        public static MeasurementResults GetMeasurementResultByResId(int resId)
        {
            var result = Execute(contract => contract.GetMeasurementResultByResId(resId, GetDefaultOtherArgs()));

            return result;
        }
        public static MeasTask GetMeasTaskHeaderById(int taskId)
        {
            var result = Execute(contract => contract.GetMeasTaskHeader(new MeasTaskIdentifier { Value = taskId }, new CommonOperationArguments() { UserId = MD.Employee.GetCurrentUserId() }));

            return result;
        }
        public static StationDataForMeasurements[] GetStationDataForMeasurementsByTaskId(int taskId)
        {
            var result = Execute(contract => contract.GetStationDataForMeasurementsByTaskId(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new StationDataForMeasurements[] { };
            }
            return result;
        }
        public static StationLevelsByTask[] GetStationLevelsByTask(List<int> MeasResultID, int MeasTaskId, int SectorId)
        {
            var parameters = new LevelsByTaskParams()
            {
                MeasResultID = MeasResultID,
                MeasTaskId = MeasTaskId,
                SectorId = SectorId
            };
            var result = Execute(contract => contract.GetStationLevelsByTask(parameters, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new StationLevelsByTask[] { };
            }
            return result;
        }
        public static ShortMeasurementResultsExtend[] GetShortMeasResultsByTypeAndTaskId(MeasurementType measurementType, int taskId)
        {
            var result = Execute(contract => contract.GetShortMeasResultsByTypeAndTaskId(measurementType, taskId, GetDefaultOtherArgs()));

            if (result == null)
            {
                return new ShortMeasurementResultsExtend[] { };
            }
            return result;
        }

        #endregion

        #region Actions
        public static int CreateMeasTask(MeasTask task)
        {
            var result = Execute(contract => contract.CreateMeasTask(task, GetDefaultOtherArgs()));

            if (result == null)
            {
                return IM.NullI;
            }
            return result.Value;
        }
        public static void DeleteMeasTaskById(int taskId)
        {
            var result = Execute(contract => contract.DeleteMeasTask(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Delete the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void RunMeasTask(int taskId)
        {
            var result = Execute(contract => contract.RunMeasTask(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Run the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        public static void StopMeasTask(int taskId)
        {
            var result = Execute(contract => contract.StopMeasTask(new MeasTaskIdentifier { Value = taskId }, GetDefaultOtherArgs()));

            if (result.State == CommonOperationState.Fault)
            {
                System.Windows.Forms.MessageBox.Show(result.FaultCause ?? "Unknown error", $"Stop the meas task with  Id #{taskId}", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        #endregion
        private static CommonOperationArguments GetDefaultOtherArgs()
        {
            return (new CommonOperationArguments() { UserId = MD.Employee.GetCurrentUserId() });
        }
    }
}
