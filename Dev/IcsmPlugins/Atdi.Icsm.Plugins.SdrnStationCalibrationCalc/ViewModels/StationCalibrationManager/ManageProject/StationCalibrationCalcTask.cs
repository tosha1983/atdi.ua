using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{
    public static class StationCalibrationCalcTask
    {
        public static void SaveTask(WebApiDataLayer dataLayer, IQueryExecutor executor, IcsmMobStation[] icsmMobStations, ParamsCalculationModel paramsCalculationModel, long contextCalcTaskId)
        {

            var clientContextId = GetClientContextIdByTaskId(dataLayer, executor, contextCalcTaskId);
            if (clientContextId != 0)
            {
                var stations = new long[icsmMobStations.Length];
                for (var i = 0; i < icsmMobStations.Length; i++)
                {
                    stations[i] = CreateClientContextStation(dataLayer, executor, clientContextId, icsmMobStations[i]);
                }
                paramsCalculationModel.StationIds = stations;

                var calcTaskUpdated = UpdateCalcTask(dataLayer, executor, clientContextId, paramsCalculationModel, contextCalcTaskId);

                UpdateClientContext(dataLayer, executor, icsmMobStations, clientContextId);

            }
            else
            {
                throw new Exception("Client context Id is 0!");
            }
        }

        public static void RunTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextCalcTaskId)
        {
            var clientContextId = GetClientContextIdByTaskId(dataLayer, executor, contextCalcTaskId);
            if (clientContextId != 0)
            {

                string OwnerInstance = "AF53CC81-4781-4FF1-8F24-49629480D79C";

                // делаем задачу доступной для расчета
                MakeCalcTaskAvailable(dataLayer, executor, contextCalcTaskId, clientContextId, OwnerInstance);

                // создаем запись для результатов
                var calcResultId = CreateCalcTaskResult(dataLayer, executor, contextCalcTaskId, OwnerInstance);

                // запускаем расчет
                RunCalcTask(dataLayer, executor, contextCalcTaskId, calcResultId, OwnerInstance);

                // ожидаем результат
                //var calcResultObject = WaitForCalcResult(dataLayer, executor, contextCalcTaskId, calcResultId);
            }
            else
            {
                throw new Exception("Client context Id is 0!");
            }

        }


        private static long GetClientContextIdByTaskId(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextTaskId)
        {
            var readQuery = dataLayer.GetBuilder<ICalcTask>()
                    .Read()
                    .Select(c => c.Id)
                    .Select(c => c.CONTEXT.Id)
                    .Filter(c => c.Id, contextTaskId);
            ;
            var clientContextIdBy = executor.ExecuteAndFetch(readQuery, reader =>
            {
                if (reader.Count == 0 || !reader.Read())
                {
                    return (long)0;
                }
                var contextId = reader.GetValue(c => c.CONTEXT.Id);
                return contextId;
            });
            return clientContextIdBy;
        }

        private static long UpdateClientContext(WebApiDataLayer dataLayer, IQueryExecutor executor, IcsmMobStation[] icsmMobStations, long contextId)
        {
            var updQuery = dataLayer.GetBuilder<IClientContext>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Pending)
                .SetValue(c => c.StatusName, "Pending")
                .SetValue(c => c.StatusNote, "")
                .Filter(c => c.Id, contextId);

            executor.Execute(updQuery);

            var cancel = false;
            while (!cancel)
            {
                System.Threading.Thread.Sleep(5 * 1000);

                var checkQuery = dataLayer.GetBuilder<IClientContext>()
                    .Read()
                    .Select(c => c.StatusCode)
                    .Select(c => c.StatusNote)
                    .Filter(c => c.Id, contextId);

                cancel = executor.ExecuteAndFetch(checkQuery, reader =>
                {
                    if (reader.Count == 0 || !reader.Read())
                    {
                        throw new InvalidOperationException($"A client context not found by ID #{contextId}");
                    }

                    var status = (ClientContextStatusCode)reader.GetValue(c => c.StatusCode);
                    var statusNote = reader.GetValue(c => c.StatusNote);


                    if (status == ClientContextStatusCode.Failed)
                    {
                        throw new InvalidOperationException($"Error preparing client context with ID #{contextId}: {statusNote}");
                    }
                    return status == ClientContextStatusCode.Prepared;
                });
            }

            return contextId;
        }


        private static long CreateClientContextStation(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, IcsmMobStation icsmMobStation)
        {
            var enumStateCode = Enum.GetValues(typeof(StationStateCode)).Cast<StationStateCode>().ToList();
            var insQuery = dataLayer.GetBuilder<IContextStation>()
                .Create()
                .SetValue(c => c.CreatedDate, icsmMobStation.CreatedDate)
                .SetValue(c => c.CONTEXT.Id, contextId)
                .SetValue(c => c.Name, icsmMobStation.Name)
                .SetValue(c => c.StateCode, (byte)enumStateCode.Find(x => x.ToString() == icsmMobStation.StateName))
                .SetValue(c => c.StateName, icsmMobStation.StateName)
                .SetValue(c => c.CallSign, icsmMobStation.CallSign)

                .SetValue(c => c.Standard, icsmMobStation.Standard)
                .SetValue(c => c.RealGsid, icsmMobStation.RealGsid)
                .SetValue(c => c.RegionCode, icsmMobStation.RegionCode)
                .SetValue(c => c.ModifiedDate, icsmMobStation.ModifiedDate)
                .SetValue(c => c.ExternalSource, icsmMobStation.ExternalSource)
                .SetValue(c => c.ExternalCode, icsmMobStation.ExternalCode)


                .SetValue(c => c.SITE.Longitude_DEC, icsmMobStation.SITE.Longitude_DEC)
                .SetValue(c => c.SITE.Latitude_DEC, icsmMobStation.SITE.Latitude_DEC)
                .SetValue(c => c.SITE.Altitude_m, icsmMobStation.SITE.Altitude_m)

                .SetValue(c => c.ANTENNA.ItuPatternCode, icsmMobStation.ANTENNA.ItuPatternCode)
                .SetValue(c => c.ANTENNA.ItuPatternName, icsmMobStation.ANTENNA.ItuPatternName)
                .SetValue(c => c.ANTENNA.XPD_dB, icsmMobStation.ANTENNA.XPD_dB)
                .SetValue(c => c.ANTENNA.Gain_dB, icsmMobStation.ANTENNA.Gain_dB)
                .SetValue(c => c.ANTENNA.Tilt_deg, icsmMobStation.ANTENNA.Tilt_deg)
                .SetValue(c => c.ANTENNA.Azimuth_deg, icsmMobStation.ANTENNA.Azimuth_deg);

            var stationPk = executor.Execute<IContextStation_PK>(insQuery);
            var stationId = stationPk.Id;

            // создаем запись о трансмитере
            var transQuery = dataLayer.GetBuilder<IContextStationTransmitter>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.PolarizationCode, icsmMobStation.TRANSMITTER.PolarizationCode)
                .SetValue(c => c.PolarizationName, ((PolarizationCode)(icsmMobStation.TRANSMITTER.PolarizationCode)).ToString())
                .SetValue(c => c.Loss_dB, icsmMobStation.TRANSMITTER.Loss_dB)
                .SetValue(c => c.Freq_MHz, icsmMobStation.TRANSMITTER.Freq_MHz)
                .SetValue(c => c.BW_kHz, icsmMobStation.TRANSMITTER.BW_kHz)
                .SetValue(c => c.MaxPower_dBm, icsmMobStation.TRANSMITTER.MaxPower_dBm);
            executor.Execute(transQuery);


            // создаем запись о приемнике
            var receiveQuery = dataLayer.GetBuilder<IContextStationReceiver>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.PolarizationCode, icsmMobStation.RECEIVER.PolarizationCode)
                .SetValue(c => c.PolarizationName, ((PolarizationCode)(icsmMobStation.RECEIVER.PolarizationCode)).ToString())
                .SetValue(c => c.Loss_dB, icsmMobStation.RECEIVER.Loss_dB)
                .SetValue(c => c.Freq_MHz, icsmMobStation.RECEIVER.Freq_MHz)
                .SetValue(c => c.BW_kHz, icsmMobStation.RECEIVER.BW_kHz)
                .SetValue(c => c.KTBF_dBm, icsmMobStation.RECEIVER.KTBF_dBm)
                .SetValue(c => c.Threshold_dBm, icsmMobStation.RECEIVER.Threshold_dBm)
                ;

            executor.Execute(receiveQuery);


            //  создаем патерн атенты
            var paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "H")
                .SetValue(c => c.WavePlane, "H")

                .SetValue(c => c.Angle_deg, icsmMobStation.ANTENNA.HH_PATTERN.Angle_deg)
                .SetValue(c => c.Loss_dB, icsmMobStation.ANTENNA.HH_PATTERN.Loss_dB);
            executor.Execute(paternQuery);

            paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "H")
                .SetValue(c => c.WavePlane, "V")

                .SetValue(c => c.Angle_deg, icsmMobStation.ANTENNA.HV_PATTERN.Angle_deg)
                .SetValue(c => c.Loss_dB, icsmMobStation.ANTENNA.HV_PATTERN.Loss_dB);
            executor.Execute(paternQuery);

            paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "V")
                .SetValue(c => c.WavePlane, "H")

                .SetValue(c => c.Angle_deg, icsmMobStation.ANTENNA.VH_PATTERN.Angle_deg)
                .SetValue(c => c.Loss_dB, icsmMobStation.ANTENNA.VH_PATTERN.Loss_dB);
            executor.Execute(paternQuery);

            paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "V")
                .SetValue(c => c.WavePlane, "V")

                .SetValue(c => c.Angle_deg, icsmMobStation.ANTENNA.VV_PATTERN.Angle_deg)
                .SetValue(c => c.Loss_dB, icsmMobStation.ANTENNA.VV_PATTERN.Loss_dB);
            executor.Execute(paternQuery);
            return stationId;
        }


        private static bool UpdateCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, ParamsCalculationModel command, long contextCalcTaskId)
        {
            bool isSuccessUpdate = false;
            try
            {
                var updateQueryCalcTask = dataLayer.GetBuilder<ICalcTask>()
                    .Update()
                    .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Modifying)
                    .SetValue(c => c.StatusName, CalcTaskStatusCode.Modifying.ToString())
                    .SetValue(c => c.StatusNote, "The task was modified")
                    .Filter(c => c.Id, contextCalcTaskId)
                    ;
                executor.Execute(updateQueryCalcTask);


                var queryStationCalibrationArgs = dataLayer.GetBuilder<IStationCalibrationArgs>()
                  .Update()
                  .SetValue(c => c.AltitudeStation, command.AltitudeStation)
                  .SetValue(c => c.AzimuthStation, command.AzimuthStation)
                  .SetValue(c => c.CascadeTuning, command.CascadeTuning)
                  .SetValue(c => c.CoordinatesStation, command.CoordinatesStation)
                  .SetValue(c => c.CorrelationDistance_m, command.CorrelationDistance_m)
                  .SetValue(c => c.Delta_dB, command.Delta_dB)
                  .SetValue(c => c.Detail, command.Detail)
                  .SetValue(c => c.DetailOfCascade, command.DetailOfCascade)
                  .SetValue(c => c.DistanceAroundContour_km, command.DistanceAroundContour_km)
                  .SetValue(c => c.MaxAntennasPatternLoss_dB, command.MaxAntennasPatternLoss_dB)
                  .SetValue(c => c.MaxDeviationAltitudeStation_m, command.MaxDeviationAltitudeStation_m)
                  .SetValue(c => c.MaxDeviationAzimuthStation_deg, command.MaxDeviationAzimuthStation_deg)
                  .SetValue(c => c.MaxDeviationCoordinatesStation_m, command.MaxDeviationCoordinatesStation_m)
                  .SetValue(c => c.MaxDeviationTiltStation_deg, command.MaxDeviationTiltStation_deg)
                  .SetValue(c => c.MaxRangeMeasurements_dBmkV, command.MaxRangeMeasurements_dBmkV)
                  .SetValue(c => c.Method, command.Method)
                  .SetValue(c => c.MinNumberPointForCorrelation, command.MinNumberPointForCorrelation)
                  .SetValue(c => c.MinRangeMeasurements_dBmkV, command.MinRangeMeasurements_dBmkV)
                  .SetValue(c => c.NumberCascade, command.NumberCascade)
                  .SetValue(c => c.PowerStation, command.PowerStation)
                  .SetValue(c => c.ShiftAltitudeStationMax_m, command.ShiftAltitudeStationMax_m)
                  .SetValue(c => c.ShiftAltitudeStationMin_m, command.ShiftAltitudeStationMin_m)
                  .SetValue(c => c.ShiftAltitudeStationStep_m, command.ShiftAltitudeStationStep_m)
                  .SetValue(c => c.ShiftAzimuthStationMax_deg, command.ShiftAzimuthStationMax_deg)
                  .SetValue(c => c.ShiftAzimuthStationMin_deg, command.ShiftAzimuthStationMin_deg)
                  .SetValue(c => c.ShiftAzimuthStationStep_deg, command.ShiftAzimuthStationStep_deg)
                  .SetValue(c => c.ShiftCoordinatesStationStep_m, command.ShiftCoordinatesStationStep_m)
                  .SetValue(c => c.ShiftCoordinatesStation_m, command.ShiftCoordinatesStation_m)
                  .SetValue(c => c.ShiftPowerStationMax_dB, command.ShiftPowerStationMax_dB)
                  .SetValue(c => c.ShiftPowerStationMin_dB, command.ShiftPowerStationMin_dB)
                  .SetValue(c => c.ShiftPowerStationStep_dB, command.ShiftPowerStationStep_dB)
                  .SetValue(c => c.ShiftTiltStationMax_deg, command.ShiftTiltStationMax_deg)
                  .SetValue(c => c.ShiftTiltStationMin_deg, command.ShiftTiltStationMin_deg)
                  .SetValue(c => c.ShiftTiltStationStep_deg, command.ShiftTiltStationStep_deg);
                if (command.StationIds != null)
                {
                    queryStationCalibrationArgs.SetValue(c => c.StationIds, command.StationIds);
                }
                if (command.InfocMeasResults != null)
                {
                    queryStationCalibrationArgs.SetValue(c => c.InfocMeasResults, command.InfocMeasResults);
                }
                queryStationCalibrationArgs.SetValue(c => c.TiltStation, command.TiltStation)
                  .SetValue(c => c.TrustOldResults, command.TrustOldResults)
                  .SetValue(c => c.UseMeasurementSameGSID, command.UseMeasurementSameGSID)
                  .SetValue(c => c.CorrelationThresholdHard, command.CorrelationThresholdHard)
                  .SetValue(c => c.CorrelationThresholdWeak, command.CorrelationThresholdWeak)
                  .Filter(c => c.TASK.Id, contextCalcTaskId)
                  ;
                dataLayer.Executor.Execute(queryStationCalibrationArgs);
                isSuccessUpdate = true;
            }
            catch (Exception)
            {
                isSuccessUpdate = false;
            }
            return isSuccessUpdate;
        }

        private static void MakeCalcTaskAvailable(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId, long contextId, string ownerInstance)
        {
            var updQuery = dataLayer.GetBuilder<ICalcTask>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Available)
                .SetValue(c => c.StatusName, CalcTaskStatusCode.Available.ToString())
                .SetValue(c => c.StatusNote, "The task was was made available")
                .Filter(c => c.Id, taskId);

            var count = executor.Execute(updQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't make the task with ID #{taskId} Available");
            }
        }

        private static long CreateCalcTaskResult(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId, string ownerInstance)
        {
            var insQuery = dataLayer.GetBuilder<ICalcResult>()
                .Create()
                .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                .SetValue(c => c.TASK.Id, taskId)
                .SetValue(c => c.CallerInstance, ownerInstance)
                .SetValue(c => c.CallerResultId, Guid.NewGuid())
                .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Created)
                .SetValue(c => c.StatusName, CalcResultStatusCode.Created.ToString())
                .SetValue(c => c.StatusNote, "The result was created by the client")
                ;
            var resultPk = executor.Execute<ICalcResult_PK>(insQuery);
            return resultPk.Id;
        }


        private static void RunCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId, long resultId, string ownerInstance)
        {
            var updQuery = dataLayer.GetBuilder<ICalcResult>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Pending)
                .SetValue(c => c.StatusName, CalcResultStatusCode.Pending.ToString())
                .SetValue(c => c.StatusNote, "The result was ran by the client")
                .Filter(c => c.CallerInstance, ownerInstance)
                .Filter(c => c.TASK.Id, taskId)
                .Filter(c => c.Id, resultId);

            var count = executor.Execute(updQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't run the task with ID #{taskId} (result ID #{resultId})");
            }
        }
    }
}
