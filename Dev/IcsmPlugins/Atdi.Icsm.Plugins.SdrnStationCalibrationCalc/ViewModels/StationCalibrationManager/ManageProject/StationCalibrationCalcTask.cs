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
        private static readonly string OwnerInstance = "Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.Client";
        private static readonly Guid OwnerProjectId = Guid.Parse("1987d232-9d71-4d46-ac34-fa4a792ce275");
        private static readonly Guid OwnerMapId = Guid.Parse("00efd40f-95c1-4a14-b00d-9f077ddb665a");
        private static readonly string MainProjectMap = "MapSdrnStationCalibrationCalc";

        public static void Run(WebApiDataLayer dataLayer, IQueryExecutor executor, IcsmMobStation[] icsmMobStations, ParamsCalculationModel paramsCalculationModel)
        {
            try
            {
                // определяем проект
                var projectId = DefineProject(dataLayer, executor);

                // определяем карту
                var mapId = DefineMap(dataLayer, executor, projectId);

                //если мы проект менять больше не собираемся, делаем проект доступным
                MakeProjectAvailable(dataLayer, executor, projectId);


                // определяем контекст
                var ownerClientContextId = Guid.NewGuid();
                var clientContextId = DefineClientContext(dataLayer, executor, projectId, ownerClientContextId, icsmMobStations, paramsCalculationModel);




                // станции из контекста
                var stations = GetClientContextStations(dataLayer, executor, clientContextId);

                var calcTaskId = CreateCalcTask(dataLayer, executor, clientContextId, paramsCalculationModel);




                // делаем задачу доступной для расчета
                MakeCalcTaskAvailable(dataLayer, executor, calcTaskId, clientContextId);

                // создаем запись для результатов
                var calcResultId = CreateCalcTaskResult(dataLayer, executor, calcTaskId);

                CreateResult(dataLayer, executor, calcResultId);

                // запускаем расчет
                RunCalcTask(dataLayer, executor, calcTaskId, calcResultId);

                // ожидаем результат
                var calcResultObject = WaitForCalcResult(dataLayer, executor, calcTaskId, calcResultId);


            }
            catch (Exception e)
            {
                
            }

        }



        private static long DefineProject(WebApiDataLayer dataLayer, IQueryExecutor executor)
        {
            var selQuery = dataLayer.GetBuilder<IProject>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.OwnerProjectId, OwnerProjectId);

            var projectId = executor.ExecuteAndFetch(selQuery, reader =>
            {
                if (reader.Count == 0 || !reader.Read())
                {
                    return (long)0;
                }
                var id = reader.GetValue(c => c.Id);
                return id;
            });

            if (projectId == 0)
            {
                var insQuery = dataLayer.GetBuilder<IProject>()
                    .Create()
                    .SetValue(c => c.Name, "Project: SdrnStationCalibrationCalc")
                    .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                    .SetValue(c => c.OwnerInstance, OwnerInstance)
                    .SetValue(c => c.OwnerProjectId, OwnerProjectId)
                    .SetValue(c => c.StatusCode, (byte)ProjectStatusCode.Created)
                    .SetValue(c => c.StatusName, "Created")
                    .SetValue(c => c.StatusNote, "The new project was created")
                    .SetValue(c => c.Projection, "4UTN35")
                    .SetValue(c => c.Note,
                        "A  project was created");
                var projectPk = executor.Execute<IProject_PK>(insQuery);
                projectId = projectPk.Id;
            }
            return projectId;
        }

        private static void MakeProjectAvailable(WebApiDataLayer dataLayer, IQueryExecutor executor, long projectId)
        {
            var updQuery = dataLayer.GetBuilder<IProject>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)ProjectStatusCode.Available)
                .SetValue(c => c.StatusName, "Available")
                .SetValue(c => c.StatusNote, "The project was made available")
                // один вариант фильтра
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.OwnerProjectId, OwnerProjectId)
                // второй вариант фильтра
                .Filter(c => c.Id, projectId);

            var count = executor.Execute(updQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't make the project with ID #{projectId} Available");
            }
        }

        private static long DefineMap(WebApiDataLayer dataLayer, IQueryExecutor executor, long projectId)
        {
            var selQuery = dataLayer.GetBuilder<IProjectMap>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.OwnerMapId, OwnerMapId)
                .Filter(c => c.PROJECT.Id, projectId)
                .Filter(c => c.MapName, MainProjectMap);

            var mapId = executor.ExecuteAndFetch(selQuery, reader =>
            {
                if (reader.Count == 0 || !reader.Read())
                {
                    return (long)0;
                }

                var id = reader.GetValue(c => c.Id);
                return id;
            });

            if (mapId == 0)
            {
                var insQuery = dataLayer.GetBuilder<IProjectMap>()
                    .Create()
                    .SetValue(c => c.MapName, MainProjectMap)
                    .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                    .SetValue(c => c.OwnerInstance, OwnerInstance)
                    .SetValue(c => c.OwnerMapId, OwnerMapId)
                    .SetValue(c => c.StatusCode, (byte)ProjectMapStatusCode.Created)
                    .SetValue(c => c.StatusName, "Created")
                    .SetValue(c => c.StatusNote, "The new map was created")
                    .SetValue(c => c.PROJECT.Id, projectId)
                    .SetValue(c => c.MapNote,
                        "A project map was created")
                    .SetValue(c => c.OwnerUpperLeftX, 276_328)
                    .SetValue(c => c.OwnerUpperLeftY, 5_532_476)
                    .SetValue(c => c.StepUnit, "M")
                    .SetValue(c => c.OwnerAxisXNumber, 4122)
                    .SetValue(c => c.OwnerAxisXStep, 5)
                    .SetValue(c => c.OwnerAxisYNumber, 3340)
                    .SetValue(c => c.OwnerAxisYStep, 5)
                    ;
                var projectMapPk = executor.Execute<IProjectMap_PK>(insQuery);
                mapId = projectMapPk.Id;

                // меняем статус
                var updQuery = dataLayer.GetBuilder<IProjectMap>()
                    .Update()
                    .SetValue(c => c.StatusCode, (byte)ProjectMapStatusCode.Pending)
                    .SetValue(c => c.StatusName, "Pending")
                    .SetValue(c => c.StatusNote, "")
                    .Filter(c => c.Id, mapId);

                executor.Execute(updQuery);

                // ожидаем расчет карты
                var cancel = false;
                while (!cancel)
                {
                    System.Threading.Thread.Sleep(5 * 1000);

                    var checkQuery = dataLayer.GetBuilder<IProjectMap>()
                        .Read()
                        .Select(c => c.StatusCode)
                        .Select(c => c.StatusNote)
                        .Filter(c => c.Id, mapId);

                    cancel = executor.ExecuteAndFetch(checkQuery, reader =>
                    {
                        if (reader.Count == 0 || !reader.Read())
                        {
                            throw new InvalidOperationException($"A map not found by ID #{mapId}");
                        }

                        var status = (ProjectMapStatusCode)reader.GetValue(c => c.StatusCode);
                        var statusNote = reader.GetValue(c => c.StatusNote);

                        if (status == ProjectMapStatusCode.Failed)
                        {
                            throw new InvalidOperationException($"Error preparing map with ID #{mapId}: {statusNote}");
                        }
                        return status == ProjectMapStatusCode.Prepared;
                    });
                }
            }
            return mapId;
        }

        private static long DefineClientContext(WebApiDataLayer dataLayer, IQueryExecutor executor, long projectId, Guid ownerContextId, IcsmMobStation[] icsmMobStations, ParamsCalculationModel paramsCalculationModel)
        {
            var selQuery = dataLayer.GetBuilder<IClientContext>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.OwnerContextId, ownerContextId)
                .Filter(c => c.PROJECT.Id, projectId);

            var contextId = executor.ExecuteAndFetch(selQuery, reader =>
            {
                if (reader.Count == 0 || !reader.Read())
                {
                    return (long)0;
                }

                var id = reader.GetValue(c => c.Id);
                return id;
            });

            if (contextId == 0)
            {
                var insQuery = dataLayer.GetBuilder<IClientContext>()
                    .Create()
                    .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                    .SetValue(c => c.OwnerInstance, OwnerInstance)
                    .SetValue(c => c.OwnerContextId, ownerContextId)
                    .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Created)
                    .SetValue(c => c.StatusName, ClientContextStatusCode.Created.ToString())
                    .SetValue(c => c.StatusNote, "The new context was created")
                    .SetValue(c => c.PROJECT.Id, projectId)
                    .SetValue(c => c.TypeCode, (byte)ClientContextTypeCode.Client)
                    .SetValue(c => c.TypeName, ClientContextTypeCode.Client.ToString())
                    .SetValue(c => c.Name, "Test client context")
                    .SetValue(c => c.Note, "The client context to SdrnStationCalibrationCalc")
                    ;
                var contextPk = executor.Execute<IClientContext_PK>(insQuery);
                contextId = contextPk.Id;

                CreateClientContextPropagationModels(dataLayer, executor, contextId);
                var stations = new long[icsmMobStations.Length];
                for (var i = 0; i < icsmMobStations.Length; i++)
                {
                    stations[i] = CreateClientContextStation(dataLayer, executor, contextId, icsmMobStations[i]);
                    // планируемые задачи для каждой станции
                    CreatePlannedCalcTask(dataLayer, executor, contextId, paramsCalculationModel);
                }

                // меняем статус
                var updQuery = dataLayer.GetBuilder<IClientContext>()
                    .Update()
                    .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Pending)
                    .SetValue(c => c.StatusName, "Pending")
                    .SetValue(c => c.StatusNote, "")
                    .Filter(c => c.Id, contextId);

                executor.Execute(updQuery);

                // ожидаем расчет контекста
                var cancel = false;
                while (!cancel)
                {
                    System.Threading.Thread.Sleep(20 * 1000);

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
            }
            return contextId;
        }

        private static void CreateClientContextPropagationModels(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId)
        {
            var mainQuery = dataLayer.GetBuilder<IClientContextMainBlock>()
                .Apply()
                .Filter(c => c.ContextId, contextId)
                .CreateIfNotExists()
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)MainBlockModelTypeCode.ITU525)
                    .SetValue(c => c.ModelTypeName, MainBlockModelTypeCode.ITU525.ToString())
                .UpdateIfExists()
                    .SetValue(c => c.ModelTypeCode, (byte)MainBlockModelTypeCode.ITU525)
                    .SetValue(c => c.ModelTypeName, MainBlockModelTypeCode.ITU525.ToString())
                ;
            var count = executor.Execute(mainQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of MainBlock in the client context with ID #{contextId}");
            }

            var absorptionQuery = dataLayer.GetBuilder<IClientContextAbsorption>()
                .Apply()
                .Filter(c => c.ContextId, contextId)
                .SetValue(c => c.ContextId, contextId)
                .SetValue(c => c.ModelTypeCode, (byte)AbsorptionModelTypeCode.FlatAndLinear)
                .SetValue(c => c.ModelTypeName, AbsorptionModelTypeCode.FlatAndLinear.ToString())
                .SetValue(c => c.Available, true)
                ;
            count = executor.Execute(absorptionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Absorption in the client context with ID #{contextId}");
            }

            // в случаи не включения блока в расчет запись создавать не обязательно
            // но если нужно зарезервировать ее, то следует создавать так как показано ниже
            var additionalQuery = dataLayer.GetBuilder<IClientContextAdditional>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)AdditionalModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, AdditionalModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(additionalQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Additional in the client context with ID #{contextId}");
            }

            var atmosphericQuery = dataLayer.GetBuilder<IClientContextAtmospheric>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)AtmosphericModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, AtmosphericModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(atmosphericQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Atmospheric in the client context with ID #{contextId}");
            }

            var clutterQuery = dataLayer.GetBuilder<IClientContextClutter>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)ClutterModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, ClutterModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(clutterQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Clutter in the client context with ID #{contextId}");
            }

            var diffractionQuery = dataLayer.GetBuilder<IClientContextDiffraction>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)DiffractionModelTypeCode.Deygout91)
                    .SetValue(c => c.ModelTypeName, DiffractionModelTypeCode.Deygout91.ToString())
                    .SetValue(c => c.Available, true)
                ;
            count = executor.Execute(diffractionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Diffraction in the client context with ID #{contextId}");
            }

            var ductingQuery = dataLayer.GetBuilder<IClientContextDucting>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(ductingQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Ducting in the client context with ID #{contextId}");
            }

            var reflectionQuery = dataLayer.GetBuilder<IClientContextReflection>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(reflectionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Reflection in the client context with ID #{contextId}");
            }

            var subPathDiffractionQuery = dataLayer.GetBuilder<IClientContextSubPathDiffraction>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)SubPathDiffractionModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, SubPathDiffractionModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(subPathDiffractionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of SubPathDiffraction in the client context with ID #{contextId}");
            }

            var tropoQuery = dataLayer.GetBuilder<IClientContextTropo>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)TropoModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, TropoModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(tropoQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Tropo in the client context with ID #{contextId}");
            }

            var paramsQuery = dataLayer.GetBuilder<IClientContextGlobalParams>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.Time_pc, 50)
                    .SetValue(c => c.Location_pc, 50)
                    .SetValue(c => c.EarthRadius_km, 8500)
                ;
            count = executor.Execute(paramsQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of GlobalParams in the client context with ID #{contextId}");
            }
        }

        private static long CreateClientContextStation(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, IcsmMobStation icsmMobStation)
        {
            var insQuery = dataLayer.GetBuilder<IContextStation>()
                .Create()
                .SetValue(c => c.CreatedDate, icsmMobStation.CreatedDate)
                .SetValue(c => c.CONTEXT.Id, contextId)
                .SetValue(c => c.Name, icsmMobStation.Name)
                .SetValue(c => c.StateCode, (byte)StationStateCode.A)
                .SetValue(c => c.StateName, icsmMobStation.StateName)
                .SetValue(c => c.CallSign, icsmMobStation.CallSign)

                .SetValue(c => c.SITE.Longitude_DEC, icsmMobStation.SITE.Longitude_DEC)
                .SetValue(c => c.SITE.Latitude_DEC, icsmMobStation.SITE.Latitude_DEC)
                .SetValue(c => c.SITE.Altitude_m, icsmMobStation.SITE.Altitude_m)

                .SetValue(c => c.ANTENNA.ItuPatternCode, (byte)ItuPattern.None)
                .SetValue(c => c.ANTENNA.ItuPatternName, "None")
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
                .SetValue(c => c.KTBF_dBm , icsmMobStation.RECEIVER.KTBF_dBm)
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
       

        private static long CreateCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, ParamsCalculationModel command)
        {
            var insQuery = dataLayer.GetBuilder<ICalcTask>()
                .Create()
                .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                .SetValue(c => c.CONTEXT.Id, contextId)
                .SetValue(c => c.MapName, MainProjectMap)
                .SetValue(c => c.OwnerInstance, OwnerInstance)
                .SetValue(c => c.OwnerTaskId, Guid.NewGuid())
                .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Created)
                .SetValue(c => c.StatusName, CalcTaskStatusCode.Created.ToString())
                .SetValue(c => c.StatusNote, "The task was created")
                .SetValue(c => c.TypeCode, (byte)CalcTaskType.StationCalibrationCalcTask)
                .SetValue(c => c.TypeName, CalcTaskType.StationCalibrationCalcTask.ToString())
                ;
            var taskPk = executor.Execute<ICalcTask_PK>(insQuery);


            var query = dataLayer.GetBuilder<IStationCalibrationArgs>()
              .Create()
              .SetValue(c => c.AltitudeStation, command.AltitudeStation)
              .SetValue(c => c.AzimuthStation, command.AzimuthStation)
              .SetValue(c => c.CascadeTuning, command.CascadeTuning)
              .SetValue(c => c.CoordinatesStation, command.CoordinatesStation)
              .SetValue(c => c.CorrelationDistance_m, command.CorrelationDistance_m)
              .SetValue(c => c.Delta_dB, command.Delta_dB)
              .SetValue(c => c.Detail, command.Detail)
              .SetValue(c => c.DetailOfCascade, command.DetailOfCascade)
              .SetValue(c => c.DistanceAroundContour_km, command.DistanceAroundContour_km)
              .SetValue(c => c.InfocMeasResults, command.InfocMeasResults)
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
              .SetValue(c => c.ShiftTiltStationStep_deg, command.ShiftTiltStationStep_deg)
              .SetValue(c => c.StationIds, command.StationIds)
              .SetValue(c => c.TaskId, taskPk.Id)
              .SetValue(c => c.TiltStation, command.TiltStation)
              .SetValue(c => c.TrustOldResults, command.TrustOldResults)
              .SetValue(c => c.UseMeasurementSameGSID, command.UseMeasurementSameGSID)
              .SetValue(c => c.CorrelationThresholdHard, command.CorrelationThresholdHard)
              .SetValue(c => c.CorrelationThresholdWeak, command.CorrelationThresholdWeak)
              ;
            var stationCalibrationArgsPk = dataLayer.Executor.Execute<IStationCalibrationArgs_PK>(query);
            return taskPk.Id;
        }

        private static long CreatePlannedCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, ParamsCalculationModel command)
        {
            var rand = new Random();
            var insQuery = dataLayer.GetBuilder<IContextPlannedCalcTask>()
                    .Create()
                    .SetValue(c => c.CONTEXT.Id, contextId)
                    .SetValue(c => c.MapName, MainProjectMap)
                    .SetValue(c => c.TypeCode, (byte)CalcTaskType.StationCalibrationCalcTask)
                    .SetValue(c => c.TypeName, CalcTaskType.StationCalibrationCalcTask.ToString())
                    .SetValue(c => c.StartNumber, (int)1)
                ;
            var plannedTaskPk = executor.Execute<IContextPlannedCalcTask_PK>(insQuery);

            var query = dataLayer.GetBuilder<IStationCalibrationArgsDefault>()
               .Create()
               .SetValue(c => c.AltitudeStation, command.AltitudeStation)
               .SetValue(c => c.AzimuthStation, command.AzimuthStation)
               .SetValue(c => c.CascadeTuning, command.CascadeTuning)
               .SetValue(c => c.CoordinatesStation, command.CoordinatesStation)
               .SetValue(c => c.CorrelationDistance_m, command.CorrelationDistance_m)
               .SetValue(c => c.Delta_dB, command.Delta_dB)
               .SetValue(c => c.Detail, command.Detail)
               .SetValue(c => c.DetailOfCascade, command.DetailOfCascade)
               .SetValue(c => c.DistanceAroundContour_km, command.DistanceAroundContour_km)
               .SetValue(c => c.InfocMeasResults, command.InfocMeasResults)
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
               .SetValue(c => c.ShiftTiltStationStep_deg, command.ShiftTiltStationStep_deg)
               .SetValue(c => c.StationIds, command.StationIds)
               .SetValue(c => c.TaskId, plannedTaskPk.Id)
               .SetValue(c => c.TiltStation, command.TiltStation)
               .SetValue(c => c.TrustOldResults, command.TrustOldResults)
               .SetValue(c => c.UseMeasurementSameGSID, command.UseMeasurementSameGSID)
               .SetValue(c => c.CorrelationThresholdHard, command.CorrelationThresholdHard)
               .SetValue(c => c.CorrelationThresholdWeak, command.CorrelationThresholdWeak)
               ;
            var stationCalibrationArgsPk = dataLayer.Executor.Execute<IStationCalibrationArgsDefault_PK>(query);

            return plannedTaskPk.Id;
        }

        private static long[] GetClientContextStations(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId)
        {
            var readQuery = dataLayer.GetBuilder<IContextStation>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.CONTEXT.Id, contextId);

            var result = executor.ExecuteAndRead(readQuery, r => r.GetValue(c => c.Id));
            return result;
        }

        private static void MakeCalcTaskAvailable(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId, long contextId)
        {
            var updQuery = dataLayer.GetBuilder<ICalcTask>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Available)
                .SetValue(c => c.StatusName, CalcTaskStatusCode.Available.ToString())
                .SetValue(c => c.StatusNote, "The task was was made available")
                // необязательно, но можем усиливать фильтрацию текущим контекстом
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.CONTEXT.Id, contextId)
                // фильтр по ключу
                .Filter(c => c.Id, taskId);

            var count = executor.Execute(updQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't make the task with ID #{taskId} Available");
            }
        }

        private static long CreateCalcTaskResult(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId)
        {
            var insQuery = dataLayer.GetBuilder<ICalcResult>()
                .Create()
                .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                .SetValue(c => c.TASK.Id, taskId)
                .SetValue(c => c.CallerInstance, OwnerInstance)
                .SetValue(c => c.CallerResultId, Guid.NewGuid())
                .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Created)
                .SetValue(c => c.StatusName, CalcResultStatusCode.Created.ToString())
                .SetValue(c => c.StatusNote, "The result was created by the client")
                ;
            var resultPk = executor.Execute<ICalcResult_PK>(insQuery);


            return resultPk.Id;
        }

        private static long CreateResult(WebApiDataLayer dataLayer, IQueryExecutor executor, long resultId)
        {
            var insQuery = dataLayer.GetBuilder<IStationCalibrationResult>()
                .Create()
                .SetValue(c => c.AreaName,"Test")
                .SetValue(c => c.RESULT.Id, resultId)
                .SetValue(c => c.NumberStation, 1)
                ;
            var resultPk = executor.Execute<IStationCalibrationResult_PK>(insQuery);


            var insQueryStationCalibrationDriveTestResult = dataLayer.GetBuilder<IStationCalibrationDriveTestResult>()
               .Create()
               .SetValue(c => c.RealGsid, "Test")
               .SetValue(c => c.LicenseGsid, "Test2")
               .SetValue(c => c.ResultDriveTestStatus, "UN")
               .SetValue(c => c.CalibrationResultId, resultPk.ResultId)
               ;
            var resultPkStationCalibrationDriveTestResult = executor.Execute<IStationCalibrationDriveTestResult_PK>(insQueryStationCalibrationDriveTestResult);

            var insQueryStationCalibrationStaResult = dataLayer.GetBuilder<IStationCalibrationStaResult>()
             .Create()
             .SetValue(c => c.RealGsid, "Test")
             .SetValue(c => c.LicenseGsid, "Test2")
             .SetValue(c => c.CalibrationResultId, resultPk.ResultId)
             ;
            var resultPkIStationCalibrationStaResult = executor.Execute<IStationCalibrationStaResult_PK>(insQueryStationCalibrationStaResult);



            return resultPk.ResultId;
        }

        private static void RunCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId, long resultId)
        {
            var updQuery = dataLayer.GetBuilder<ICalcResult>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Pending)
                .SetValue(c => c.StatusName, CalcResultStatusCode.Pending.ToString())
                .SetValue(c => c.StatusNote, "The result was ran by the client")
                // необязательно, но можем усиливать фильтрацию текущим контекстом
                .Filter(c => c.CallerInstance, OwnerInstance)
                .Filter(c => c.TASK.Id, taskId)
                // фильтр по ключу
                .Filter(c => c.Id, resultId);

            var count = executor.Execute(updQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't run the task with ID #{taskId} (result ID #{resultId})");
            }
        }

        private static IStationCalibrationResult WaitForCalcResult(WebApiDataLayer dataLayer, IQueryExecutor executor, long calcTaskId, long calcResultId)
        {
            var cancel = false;
            var resultObject = dataLayer.ProxyInstanceFactory.Create<IStationCalibrationResult>();
            resultObject.RESULT = dataLayer.ProxyInstanceFactory.Create<ICalcResult>();

            while (!cancel)
            {
                System.Threading.Thread.Sleep(5 * 1000);

                var checkQuery = dataLayer.GetBuilder<ICalcResult>()
                    .Read()
                    .Select(c => c.Id)
                    .Select(c => c.CreatedDate)
                    .Select(c => c.StatusCode)
                    .Select(c => c.StatusName)
                    .Select(c => c.StatusNote)
                    .Select(c => c.StartTime)
                    .Select(c => c.FinishTime)
                    .Filter(c => c.Id, calcResultId);

                cancel = executor.ExecuteAndFetch(checkQuery, reader =>
                {
                    if (reader.Count == 0 || !reader.Read())
                    {
                        throw new InvalidOperationException($"A calc result not found by ID #{calcResultId}");
                    }

                    var status = (CalcResultStatusCode)reader.GetValue(c => c.StatusCode);
                    var statusNote = reader.GetValue(c => c.StatusNote);


                    if (status != CalcResultStatusCode.Pending
                    && status != CalcResultStatusCode.Processing)
                    {
                        resultObject.RESULT.Id = reader.GetValue(c => c.Id);
                        resultObject.RESULT.StatusCode = reader.GetValue(c => c.StatusCode);
                        resultObject.RESULT.StatusName = reader.GetValue(c => c.StatusName);
                        resultObject.RESULT.StatusNote = reader.GetValue(c => c.StatusNote);
                        resultObject.RESULT.CreatedDate = reader.GetValue(c => c.CreatedDate);
                        resultObject.RESULT.StartTime = reader.GetValue(c => c.StartTime);
                        resultObject.RESULT.FinishTime = reader.GetValue(c => c.FinishTime);

                        var resultQuery = dataLayer.GetBuilder<IStationCalibrationResult>()
                            .Read()
                            .Select(c => c.NumberStation)
                            .Select(c => c.NumberStationInContour)
                            .Select(c => c.AreaName)
                            .Select(c => c.CountMeasGSID)
                            .Select(c => c.CountMeasGSID_IT)
                            .Select(c => c.CountMeasGSID_LS)
                            .Select(c => c.CountStation_CS)
                            .Select(c => c.CountStation_IT)
                            .Select(c => c.CountStation_NF)
                            .Select(c => c.CountStation_NS)
                            .Select(c => c.CountStation_UN)
                            .Select(c => c.Standard)
                            .Select(c => c.TimeStart)
                            .Filter(c => c.ResultId, calcResultId);

                        var rs = executor.ExecuteAndRead(resultQuery, r =>
                        {
                            resultObject.Standard = r.GetValue(c => c.Standard);
                            resultObject.TimeStart = r.GetValue(c => c.TimeStart);
                            return resultObject;
                        });

                    }

                    if (status == CalcResultStatusCode.Failed)
                    {
                        return true;
                    }

                    if (status == CalcResultStatusCode.Completed)
                    {
                        return true;
                    }

                    if (status == CalcResultStatusCode.Aborted)
                    {
                        return true;
                    }

                    if (status == CalcResultStatusCode.Canceled)
                    {
                        return true;
                    }

                    return false;
                });
            }

            return resultObject;
        }
    }

    public enum CalcTaskType
    {
        /// <summary>
        /// Расчет профилей покрытия относительно одной произволной точки 
        /// </summary>
        CoverageProfilesCalc = 1,

        /// <summary>
        /// Расчет профилей покрытия относительно одной произволной точки 
        /// </summary>
        PointFieldStrengthCalc = 2,

        /// <summary>
        /// Первая тестовая расчетная задача
        /// </summary>
        FirstExampleTask = 101,

        /// <summary>
        /// Вторая тестовая расчетная задача
        /// </summary>
        SecondExampleTask = 102,

        /// <summary>
        /// Треться тестовая расчетная задачи
        /// </summary>
        ThirdExampleTask = 103,

        /// <summary>
        /// Определение параметров станций по результатам измерений мобильной компоненты
        /// </summary>
        StationCalibrationCalcTask = 104

    }
}
