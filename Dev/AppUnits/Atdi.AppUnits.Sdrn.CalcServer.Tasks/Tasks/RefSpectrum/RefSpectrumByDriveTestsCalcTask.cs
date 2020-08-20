using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.Infocenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.CalcServer.Internal;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using IC = Atdi.DataModels.Sdrn.Infocenter.Entities;
using Atdi.Platform.Logging;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Platform;
using Atdi.Common;
using Atdi.Common.Extensions;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations;
using Atdi.Platform.Data;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
    [TaskHandler(CalcTaskType.RefSpectrumByDriveTestsCalcTask)]
    public class RefSpectrumByDriveTestsCalcTask : ITaskHandler
    {
        private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
        private readonly IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> _infocenterDataLayer;
        private readonly IMapRepository _mapRepository;
        public readonly ISignalService _signalService;
        private readonly IClientContextService _contextService;
        private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;
        private readonly ILogger _logger;
        private readonly IObjectPoolSite _poolSite;
        private readonly AppServerComponentConfig _appServerComponentConfig;
        private ITaskContext _taskContext;
        private IDataLayerScope _calcDbScope;
        private IDataLayerScope _infoDbScope;
        private RefSpectrumParameters _parameters;
        private SensorParameters[] _sensorParameters;
        private RefSpectrumStationCalibration[] _refSpectrumStationCalibrations;




        public RefSpectrumByDriveTestsCalcTask(
            AppServerComponentConfig appServerComponentConfig,
            IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
            IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> infocenterDataLayer,
            IMapRepository mapRepository,
            IClientContextService contextService,
            IIterationsPool iterationsPool,
            ITransformation transformation,
            IObjectPoolSite poolSite,
            ISignalService signalService,
            ILogger logger)
        {
            _mapRepository = mapRepository;
            _calcServerDataLayer = calcServerDataLayer;
            _infocenterDataLayer = infocenterDataLayer;
            _contextService = contextService;
            _iterationsPool = iterationsPool;
            _transformation = transformation;
            _appServerComponentConfig = appServerComponentConfig;
            _poolSite = poolSite;
            _logger = logger;
            _signalService = signalService;
        }

        public void Dispose()
        {
            if (_calcDbScope != null)
            {
                _calcDbScope.Dispose();
                _calcDbScope = null;
            }
            if (_infoDbScope != null)
            {
                _infoDbScope.Dispose();
                _infoDbScope = null;
            }
            _taskContext = null;
        }

        public void Load(ITaskContext taskContext)
        {
            this._taskContext = taskContext;
            this._calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();
            this._infoDbScope = this._infocenterDataLayer.CreateScope<InfocenterDataContext>();

            // загрузить параметры задачи
            this.LoadTaskParameters();
        }



        private long? FindSensor(double Freq_MHz, StationAntenna[] stationAntennas)
        {
            long? id = null;
            if ((stationAntennas != null) && (stationAntennas.Length > 0))
            { return 0; }
            else { }
            return id;
        }

        private bool CompareFreqStationForRefLevel(double Freq_MHz, ContextStation contextStation)
        {
            var FreqDT = Freq_MHz;
            var FreqST = contextStation.Transmitter.Freq_MHz;
            var FreqArr = contextStation.Transmitter.Freqs_MHz;
            var BW = contextStation.Transmitter.BW_kHz / 1000.0;
            if ((FreqST - BW <= FreqDT) && (FreqST + BW >= FreqDT)) { return true; }
            //if ((FreqArr != null) && (FreqArr.Length > 0))
            //{
            //    for (int i = 0; FreqArr.Length > i; i++)
            //    {
            //        if ((FreqArr[i] - BW <= FreqDT) && (FreqArr[i] + BW >= FreqDT)) { return true; }
            //    }
            //}
            return false;
        }

        private ReceivedPowerCalcData FillReceivedPowerCalcData(byte[] buildingContent,
                                               byte[] clutterContent,
                                               CluttersDesc cluttersDesc,
                                               AtdiMapArea atdiMapArea,
                                               PropagationModel propagationModel,
                                               short[] reliefContent,
                                               Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationTransmitter stationTransmitter,
                                               Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna stationAntennaTx,

                                               AtdiCoordinate coordinateTx,
                                               double TxAltitude_m,
                                                Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna stationAntennaRx,
                                                AtdiCoordinate coordinateRx,
                                                double rxFeederLoss_dB,
                                                double txFreq_Mhz,
                                               double RxAltitude_m
                                               )
        {
            var receivedPowerCalcData = new ReceivedPowerCalcData();
            receivedPowerCalcData.BuildingContent = buildingContent;
            receivedPowerCalcData.ClutterContent = clutterContent;
            receivedPowerCalcData.CluttersDesc = cluttersDesc;
            receivedPowerCalcData.MapArea = atdiMapArea;
            receivedPowerCalcData.PropagationModel = propagationModel;
            receivedPowerCalcData.ReliefContent = reliefContent;
            receivedPowerCalcData.Transmitter = stationTransmitter;
            receivedPowerCalcData.TxAntenna = stationAntennaTx;
            receivedPowerCalcData.TxCoordinate = coordinateTx;
            receivedPowerCalcData.TxAltitude_m = TxAltitude_m;
            receivedPowerCalcData.RxAntenna = stationAntennaRx;
            receivedPowerCalcData.RxCoordinate = coordinateRx;
            receivedPowerCalcData.RxFeederLoss_dB = rxFeederLoss_dB;
            receivedPowerCalcData.TxFreq_Mhz = txFreq_Mhz;
            receivedPowerCalcData.RxAltitude_m = RxAltitude_m;

            return receivedPowerCalcData;
        }

        public void Run()
        {
            try
            {

                if ((this._sensorParameters == null) || ((this._sensorParameters != null) && (this._sensorParameters.Length == 0)))
                {
                    this._taskContext.SendEvent(new CalcResultEvent
                    {
                        Level = CalcResultEventLevel.Error,
                        Context = "RefSpectrumByDriveTestsCalcTask",
                        Message = $"For task id = '{this._taskContext.TaskId}' no information about sensor parameters"
                    });
                    var except = new Exception($"For task id = '{this._taskContext.TaskId}' no information about sensor parameters");
                    this._logger.Exception(Contexts.ThisComponent, except);
                    throw except;
                }

                if ((this._refSpectrumStationCalibrations == null) || ((this._refSpectrumStationCalibrations != null) && (this._refSpectrumStationCalibrations.Length == 0)))
                {
                    this._taskContext.SendEvent(new CalcResultEvent
                    {
                        Level = CalcResultEventLevel.Error,
                        Context = "RefSpectrumByDriveTestsCalcTask",
                        Message = $"For task id = '{this._taskContext.TaskId}' no information about stations calibration"
                    });
                    var except = new Exception($"For task id = '{this._taskContext.TaskId}' no information about stations calibration");
                    this._logger.Exception(Contexts.ThisComponent, except);
                    throw except;
                }


                var iterationReceivedPowerCalcResult = _iterationsPool.GetIteration<ReceivedPowerCalcData, ReceivedPowerCalcResult>();
                var iterationPercentTimeForGainCalcDataResult = _iterationsPool.GetIteration<PercentTimeForGainCalcData, double[]>();
                var mapData = _mapRepository.GetMapByName(this._calcDbScope, this._taskContext.ProjectId, this._parameters.MapName);
                var propagationModel = _contextService.GetPropagationModel(this._calcDbScope, this._taskContext.ClientContextId);
                var listResultRefSpectrumBySensors = new List<ResultRefSpectrumBySensors>();
                var skipDriveTests = new List<ContextStation>();
                var listRefSpectrumStationCalibrations = this._refSpectrumStationCalibrations.ToList();
                var sensorIds = _sensorParameters.Select(x => x.SensorId).Distinct().ToArray();
                List<double> listFreqs = new List<double>();
                for (int i = 0; i < listRefSpectrumStationCalibrations.Count; i++)
                {
                    if (listRefSpectrumStationCalibrations[i].Freq_MHz > 0)
                    {
                        if (!listFreqs.Contains(Math.Round(listRefSpectrumStationCalibrations[i].Freq_MHz, 6)))
                        {
                            listFreqs.Add(Math.Round(listRefSpectrumStationCalibrations[i].Freq_MHz, 6));
                        }
                    }
                    else
                    {
                        if (listRefSpectrumStationCalibrations[i].Old_Freq_MHz > 0)
                        {
                            if (!listFreqs.Contains(Math.Round(listRefSpectrumStationCalibrations[i].Old_Freq_MHz, 6)))
                            {
                                listFreqs.Add(Math.Round(listRefSpectrumStationCalibrations[i].Old_Freq_MHz, 6));
                            }
                        }
                    }
                }
                var freqs_MHz = listFreqs.ToArray();

                int index = 1;
                int percentComplete = 0;
                int indexForPC = 0;
                for (int i = 0; i < sensorIds.Length; i++)
                {

                    var sensor = _sensorParameters.ToList().Find(x => x.SensorId == sensorIds[i]);
                    if (sensor == null)
                    {
                        this._logger.Info(Contexts.ThisComponent, $"For sensor id = {sensorIds[i]} not find additional parameters!");
                        continue;
                    }
                    if ((sensor.Coordinate.X == 0) && (sensor.Coordinate.Y == 0))
                    {
                        this._logger.Info(Contexts.ThisComponent, $"Sensor id = {sensor.SensorId} contain is null coordinate! ");
                        continue;
                    }

                    //sensor.RxFeederLoss_dB = 0;
                    if (sensor.SensorAntennaHeight_m == 0)
                    {
                        sensor.SensorAntennaHeight_m = 75;
                    }

                    for (int j = 0; j < freqs_MHz.Length; j++)
                    {
                        var idSensor = FindSensor(freqs_MHz[j], sensor.SensorAntennas);
                        if (idSensor == null)
                        {
                            this._logger.Info(Contexts.ThisComponent, $"idSensor not found");
                            continue;
                        }
                        var fndSensorAntennas = sensor.SensorAntennas[idSensor.Value];
                        if (fndSensorAntennas == null)
                        {
                            this._logger.Info(Contexts.ThisComponent, $"SensorAntennas not found for idSensor = {idSensor.Value}");
                            continue;
                        }
                        //fndSensorAntennas.Azimuth_deg = 0;
                        //fndSensorAntennas.Gain_dB = 0;
                        //fndSensorAntennas.Tilt_deg = 0;

                        if (fndSensorAntennas.Freq_MHz == 0)
                        {
                            fndSensorAntennas.Freq_MHz = 1000;
                        }
                        if (fndSensorAntennas.XPD_dB == 0)
                        {
                            fndSensorAntennas.XPD_dB = 25;
                        }

                        var lstReceivedPowerCalcResult = new List<ReceivedPowerCalcResult>();
                        for (int k = 0; k < this._refSpectrumStationCalibrations.Length; k++)
                        {
                            //bool isDeleteStation = false;

                            var refSpectrumStation = this._refSpectrumStationCalibrations[k];
                            if ((CompareFreqStationForRefLevel(freqs_MHz[j], refSpectrumStation.contextStation)) == false)
                            {
                                //k++;
                                continue;
                            }
                            else
                            {
                                var contextStation = refSpectrumStation.contextStation;

                                //contextStation.Antenna.Azimuth_deg = 0;
                                //contextStation.Antenna.Freq_MHz = 1000;
                                //contextStation.Antenna.Gain_dB = 0;
                                //contextStation.Antenna.Tilt_deg = 0;
                                //contextStation.Antenna.XPD_dB = 25;

                                //sensor.Coordinate  - if null - ERROR






                                var wgs84Coordinate = new Wgs84Coordinate()
                                {
                                    Longitude = contextStation.Site.Longitude,
                                    Latitude = contextStation.Site.Latitude
                                };


                                var receivedPowerCalcData = FillReceivedPowerCalcData(mapData.BuildingContent, mapData.ClutterContent,
                                                          _mapRepository.GetCluttersDesc(this._calcDbScope, mapData.Id), mapData.Area,
                                                           propagationModel, mapData.ReliefContent, contextStation.Transmitter,
                                                           contextStation.Antenna, _transformation.ConvertCoordinateToAtdi(in wgs84Coordinate, this._parameters.Projection),
                                                            contextStation.Site.Altitude, fndSensorAntennas, sensor.Coordinate, sensor.RxFeederLoss_dB, refSpectrumStation.Freq_MHz, sensor.SensorAntennaHeight_m);



                                var resultRefSpectrumBySensors = new ResultRefSpectrumBySensors();
                                resultRefSpectrumBySensors.OrderId = index;
                                resultRefSpectrumBySensors.TableIcsmName = contextStation.ExternalSource;
                                resultRefSpectrumBySensors.IdIcsm = Convert.ToInt32(contextStation.ExternalCode);
                                resultRefSpectrumBySensors.GlobalCID = refSpectrumStation.RealGsid;
                                resultRefSpectrumBySensors.Freq_MHz = refSpectrumStation.Freq_MHz;
                                if (refSpectrumStation.DateTimeMeas != null)
                                {
                                    resultRefSpectrumBySensors.DateMeas = refSpectrumStation.DateTimeMeas.Value;
                                }
                                resultRefSpectrumBySensors.IdSensor = sensorIds[i].Value;

                                // вызов итерации определения уровня сигнала Level
                                var resulLevelCalc = iterationReceivedPowerCalcResult.Run(_taskContext, receivedPowerCalcData);

                                if (resulLevelCalc.Level_dBm.Value < this._parameters.PowerThreshold_dBm)
                                {
                                    //if (skipDriveTests.Find(x => x.Id == contextStation.Id) == null)
                                    //{
                                    //skipDriveTests.Add(contextStation);
                                    //k++;
                                    continue;
                                    //}
                                }
                                else
                                {
                                    resultRefSpectrumBySensors.Level_dBm = resulLevelCalc.Level_dBm.Value;
                                }

                                index++;
                                lstReceivedPowerCalcResult.Add(resulLevelCalc);
                                listResultRefSpectrumBySensors.Add(resultRefSpectrumBySensors);

                                //isDeleteStation = true;
                            }


                            var clc = (double)(100.0 / (double)(freqs_MHz.Length * this._refSpectrumStationCalibrations.Length * sensorIds.Length));
                            var newCalcPercent = (int)(((k + 1) * (j + 1) * (i + 1)) * clc);
                            if ((newCalcPercent - percentComplete) > 4)
                            {
                                percentComplete = newCalcPercent;
                                this._taskContext.SendEvent(new CalcResultEvent<CurrentProgress>
                                {
                                    Level = CalcResultEventLevel.Info,
                                    Context = "RefSpectrumByDriveTestsCalcTask",
                                    Message = $"Percent complete",
                                    Data = new CurrentProgress
                                    {
                                        State = newCalcPercent
                                    }
                                });
                            }
                            //if (isDeleteStation)
                            //{
                            //List<RefSpectrumStationCalibration> tmp =_refSpectrumStationCalibrations.ToList();
                            //tmp.RemoveAt(k);
                            //_refSpectrumStationCalibrations = tmp.ToArray();
                            //k = 0;
                            //continue;
                            //}
                            //k++;
                        }

                        var percentTimeForGainCalcData = new PercentTimeForGainCalcData();
                        percentTimeForGainCalcData.SensorAntennaHeight_m = sensor.SensorAntennaHeight_m;
                        percentTimeForGainCalcData.SensorId = sensorIds[i].Value;
                        percentTimeForGainCalcData.StationData = lstReceivedPowerCalcResult.ToArray();

                        // вызов итерации определения процента времени P, в течение которого излучение данного сектора доминирует на данном сенсоре
                        var resultPercent = iterationPercentTimeForGainCalcDataResult.Run(_taskContext, percentTimeForGainCalcData);
                        if ((resultPercent != null) && (resultPercent.Length > 0))
                        {
                            for (int n = 0; n < percentTimeForGainCalcData.StationData.Length; n++)
                            {
                                listResultRefSpectrumBySensors[n + indexForPC].Percent = resultPercent[n];
                            }
                            indexForPC = indexForPC + percentTimeForGainCalcData.StationData.Length;
                        }
                    }

                }


                //if (skipDriveTests.Count > 0)
                //{
                //    // справочник с данными по стандарту и количеству базовых станций, которые были пропущены по причине невыполнения условия (resulLevelCalc.Level_dBm.Value < this._parameters.PowerThreshold_dBm)
                //    string baseStations = "";
                //    var tempContextStations = _refSpectrumStationCalibrations.Select(v => v.contextStation);
                //    if (tempContextStations != null)
                //    {
                //        var standards = tempContextStations.Select(x => x.Standard).Distinct().ToArray();
                //        for (int n = 0; n < standards.Length; n++)
                //        {
                //            var allDriveTestsByStandard = tempContextStations.ToList().FindAll(x => x.Standard == standards[n]);
                //            if ((allDriveTestsByStandard != null) && (allDriveTestsByStandard.Count > 0))
                //            {
                //                var distinctDriveTests = allDriveTestsByStandard.Select(x => x.RealGsid).Distinct();
                //                if ((distinctDriveTests != null) && (distinctDriveTests.Count() > 0))
                //                {
                //                    baseStations += $"{standards[n]}: {distinctDriveTests.Count()} base stations;" + Environment.NewLine;
                //                }
                //            }
                //        }
                //        string message = "The following emission sources have too small signal level and are not associated with any sensor:" + Environment.NewLine;
                //        message += baseStations;
                //        message += "Please check the power threshold value" + Environment.NewLine;
                //        this._taskContext.SendEvent(new CalcResultEvent
                //        {
                //            Level = CalcResultEventLevel.Warning,
                //            Context = "RefSpectrumByDriveTestsCalcTask",
                //            Message = message
                //        });
                //    }
                //}


                ///// запись результатов
                if ((listResultRefSpectrumBySensors != null) && (listResultRefSpectrumBySensors.Count > 0))
                {
                    var resultId = CreateResult();

                    this._taskContext.SendEvent(new CalcResultEvent<CurrentProgress>
                    {
                        Level = CalcResultEventLevel.Info,
                        Context = "RefSpectrumByDriveTestsCalcTask",
                        Message = $"Percent complete",
                        Data = new CurrentProgress
                        {
                            State = 100
                        }
                    });


                    for (int i = 0; i < listResultRefSpectrumBySensors.Count; i++)
                    {
                        SaveTaskResult(listResultRefSpectrumBySensors[i], resultId);
                    }
                }
                else
                {
                    this._taskContext.SendEvent(new CalcResultEvent
                    {
                        Level = CalcResultEventLevel.Warning,
                        Context = "RefSpectrumByDriveTestsCalcTask",
                        Message = $"For task id = '{this._taskContext.TaskId}' no results were generated"
                    });
                }
                // переводим результат в статус "Completed"
                var updQuery = _calcServerDataLayer.GetBuilder<ICalcResult>()
                   .Update()
                   .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Completed)
                   .SetValue(c => c.StatusName, CalcResultStatusCode.Completed.ToString())
                   .SetValue(c => c.StatusNote, "The calc  result completed")
                   .Where(c => c.TASK.Id, ConditionOperator.Equal, _taskContext.TaskId)
                   .Where(c => c.Id, ConditionOperator.Equal, _taskContext.ResultId);
                _calcDbScope.Executor.Execute(updQuery);

            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.RefSpectrumByDriveTestsCalcTask, e);
            }
        }

        private DateTime? GetDateTimeMeas(long[] infocMeasResults)
        {
            DateTime? dateTimeMeas = null;
            var partResultIds = BreakDownElemBlocks.BreakDown(infocMeasResults);
            for (int i = 0; i < partResultIds.Count; i++)
            {
                var driveTests = new List<DriveTestsResult>();
                var queryDriveTest = _infocenterDataLayer.GetBuilder<IDriveTest>()
                .From()
                .Select(
                    c => c.Id,
                    c => c.Freq_MHz,
                    c => c.Gsid,
                    c => c.PointsCount,
                    c => c.Standard,
                    c => c.RESULT.MeasTime
                )
                .Where(c => c.RESULT.Id, ConditionOperator.In, partResultIds[i].ToArray());
                var contextDriveTestsResults = _infoDbScope.Executor.ExecuteAndFetch(queryDriveTest, reader =>
                {
                    while (reader.Read())
                    {
                        dateTimeMeas = reader.GetValue(c => c.RESULT.MeasTime);
                        break;
                    }
                    return true;
                });
            }
            return dateTimeMeas;
        }

        private void LoadTaskParameters()
        {
            try
            {
                // load parameters
                var query = _calcServerDataLayer.GetBuilder<IRefSpectrumByDriveTestsArgs>()
                                .From()
                                .Select(
                                    c => c.Comments,
                                    c => c.PowerThreshold_dBm,
                                    c => c.StationIds,
                                    c => c.TASK.CONTEXT.PROJECT.Projection,
                                    c => c.TASK.MapName,
                                    c => c.ResultId,
                                    c => c.SensorIds
                                )
                                .Where(c => c.TaskId, ConditionOperator.Equal, _taskContext.TaskId);

                this._parameters = _calcDbScope.Executor.ExecuteAndFetch(query, reader =>
                {
                    if (!reader.Read())
                    {
                        return null;
                    }
                    return new RefSpectrumParameters()
                    {
                        Projection = reader.GetValue(c => c.TASK.CONTEXT.PROJECT.Projection),
                        MapName = reader.GetValue(c => c.TASK.MapName),
                        StationIds = reader.GetValue(c => c.StationIds),
                        Comments = reader.GetValue(c => c.Comments),
                        PowerThreshold_dBm = reader.GetValue(c => c.PowerThreshold_dBm),
                        ResultId = reader.GetValue(c => c.ResultId),
                        SensorIds = reader.GetValue(c => c.SensorIds)
                    };
                });

                if ((this._parameters.SensorIds == null) || ((this._parameters.SensorIds != null) && (this._parameters.SensorIds.Length == 0)))
                {
                    this._taskContext.SendEvent(new CalcResultEvent
                    {
                        Level = CalcResultEventLevel.Error,
                        Context = "RefSpectrumByDriveTestsCalcTask",
                        Message = $"For task id = '{this._taskContext.TaskId}' no information about sensors"
                    });
                    var except = new Exception($"For task id = '{this._taskContext.TaskId}' no information about sensors");
                    this._logger.Exception(Contexts.ThisComponent, except);
                    throw except;
                }

                if ((this._parameters.StationIds == null) || ((this._parameters.StationIds != null) && (this._parameters.StationIds.Length == 0)))
                {
                    this._taskContext.SendEvent(new CalcResultEvent
                    {
                        Level = CalcResultEventLevel.Error,
                        Context = "RefSpectrumByDriveTestsCalcTask",
                        Message = $"For task id = '{this._taskContext.TaskId}' no information about stations"
                    });
                    var except = new Exception($"For task id = '{this._taskContext.TaskId}' no information about stations");
                    this._logger.Exception(Contexts.ThisComponent, except);
                    throw except;
                }


                var stationRes = new List<RefSpectrumStationCalibration>();


                var partstationIdsRes = BreakDownElemBlocks.BreakDown(this._parameters.StationIds);
                for (int i = 0; i < partstationIdsRes.Count; i++)
                {
                    var queryStationCalibrationStaResult = _calcServerDataLayer.GetBuilder<IStationCalibrationStaResult>()
                     .From()
                     .Select(
                         c => c.StationMonitoringId,
                         c => c.Freq_MHz,
                         c => c.Old_Freq_MHz,
                         c => c.Standard,
                         c => c.RealGsid,
                         c => c.New_Altitude_m,
                         c => c.Old_Altitude_m,
                         c => c.New_Tilt_deg,
                         c => c.Old_Tilt_deg,
                         c => c.New_Azimuth_deg,
                         c => c.Old_Azimuth_deg,
                         c => c.New_Lon_deg,
                         c => c.Old_Lon_deg,
                         c => c.New_Lat_deg,
                         c => c.Old_Lat_deg,
                         c => c.New_Power_dB,
                         c => c.Old_Power_dB,
                         c => c.CALCRESULTS_STATION_CALIBRATION.PARAMETERS.InfocMeasResults
                     )
                    .Where(c => c.Id, ConditionOperator.In, partstationIdsRes[i]);

                    _calcDbScope.Executor.ExecuteAndFetch(queryStationCalibrationStaResult, readerStationCalibrationStaResult =>
                    {
                        while (readerStationCalibrationStaResult.Read())
                        {
                            RefSpectrumStationCalibration refSpectrumStationCalibration = new RefSpectrumStationCalibration()
                            {
                                StationMonitoringId = readerStationCalibrationStaResult.GetValue(c => c.StationMonitoringId),
                                Freq_MHz = readerStationCalibrationStaResult.GetValue(c => c.Freq_MHz),
                                Old_Freq_MHz = readerStationCalibrationStaResult.GetValue(c => c.Old_Freq_MHz),
                                Standard = readerStationCalibrationStaResult.GetValue(c => c.Standard),
                                RealGsid = readerStationCalibrationStaResult.GetValue(c => c.RealGsid)
                            };

                            if (readerStationCalibrationStaResult.GetValue(c => c.New_Altitude_m).HasValue)
                            {
                                refSpectrumStationCalibration.Altitude_m = readerStationCalibrationStaResult.GetValue(c => c.New_Altitude_m).Value;
                            }
                            else
                            {
                                refSpectrumStationCalibration.Altitude_m = readerStationCalibrationStaResult.GetValue(c => c.Old_Altitude_m);
                            }


                            if (readerStationCalibrationStaResult.GetValue(c => c.New_Tilt_deg).HasValue)
                            {
                                refSpectrumStationCalibration.Tilt_Deg = readerStationCalibrationStaResult.GetValue(c => c.New_Tilt_deg).Value;
                            }
                            else
                            {
                                refSpectrumStationCalibration.Tilt_Deg = readerStationCalibrationStaResult.GetValue(c => c.Old_Tilt_deg);
                            }

                            if (readerStationCalibrationStaResult.GetValue(c => c.New_Azimuth_deg).HasValue)
                            {
                                refSpectrumStationCalibration.Azimuth_deg = readerStationCalibrationStaResult.GetValue(c => c.New_Azimuth_deg).Value;
                            }
                            else
                            {
                                refSpectrumStationCalibration.Azimuth_deg = readerStationCalibrationStaResult.GetValue(c => c.Old_Azimuth_deg);
                            }

                            if (readerStationCalibrationStaResult.GetValue(c => c.New_Lon_deg).HasValue)
                            {
                                refSpectrumStationCalibration.Lon_deg = readerStationCalibrationStaResult.GetValue(c => c.New_Lon_deg).Value;
                            }
                            else
                            {
                                refSpectrumStationCalibration.Lon_deg = readerStationCalibrationStaResult.GetValue(c => c.Old_Lon_deg);
                            }

                            if (readerStationCalibrationStaResult.GetValue(c => c.New_Lat_deg).HasValue)
                            {
                                refSpectrumStationCalibration.Lat_deg = readerStationCalibrationStaResult.GetValue(c => c.New_Lat_deg).Value;
                            }
                            else
                            {
                                refSpectrumStationCalibration.Lat_deg = readerStationCalibrationStaResult.GetValue(c => c.Old_Lat_deg);
                            }

                            if (readerStationCalibrationStaResult.GetValue(c => c.New_Power_dB).HasValue)
                            {
                                refSpectrumStationCalibration.Power_dB = readerStationCalibrationStaResult.GetValue(c => c.New_Power_dB).Value;
                            }
                            else
                            {
                                refSpectrumStationCalibration.Power_dB = readerStationCalibrationStaResult.GetValue(c => c.Old_Power_dB);
                            }

                            refSpectrumStationCalibration.DateTimeMeas = GetDateTimeMeas(readerStationCalibrationStaResult.GetValue(c => c.CALCRESULTS_STATION_CALIBRATION.PARAMETERS.InfocMeasResults));

                            stationRes.Add(refSpectrumStationCalibration);

                        }
                        return true;
                    });
                }




                this._parameters.StationIds = stationRes.Select(x => x.StationMonitoringId).ToArray();

                // load stations
                List<ContextStation> lstStations = new List<ContextStation>();

                var refSpectrumStationAndSensors = new List<RefSpectrumStationAndSensors>();

                var partStationIdsByCalcServer = BreakDownElemBlocks.BreakDown(this._parameters.StationIds);
                for (int i = 0; i < partStationIdsByCalcServer.Count; i++)
                {
                    var queryStation = _calcServerDataLayer.GetBuilder<IContextStation>()
                        .From()
                        .Select(
                            c => c.Id,
                            c => c.ExternalCode,
                            c => c.ExternalSource,
                            c => c.CONTEXT.Id,
                            c => c.StateCode,
                            c => c.CreatedDate,
                            c => c.CallSign,
                            c => c.Name,
                            c => c.Standard,
                            c => c.ModifiedDate,

                            c => c.LicenseGsid,
                            c => c.RealGsid,
                            c => c.RegionCode,


                            c => c.SITE.Latitude_DEC,
                            c => c.SITE.Longitude_DEC,
                            c => c.SITE.Altitude_m,

                            c => c.COORDINATES.AtdiX,
                            c => c.COORDINATES.AtdiY,

                            c => c.ANTENNA.Azimuth_deg,
                            c => c.ANTENNA.Gain_dB,
                            c => c.ANTENNA.ItuPatternCode,
                            c => c.ANTENNA.Tilt_deg,
                            c => c.ANTENNA.XPD_dB,

                            c => c.ANTENNA.HH_PATTERN.StationId,
                            c => c.ANTENNA.HH_PATTERN.Angle_deg,
                            c => c.ANTENNA.HH_PATTERN.Loss_dB,

                            c => c.ANTENNA.HV_PATTERN.StationId,
                            c => c.ANTENNA.HV_PATTERN.Angle_deg,
                            c => c.ANTENNA.HV_PATTERN.Loss_dB,

                            c => c.ANTENNA.VH_PATTERN.StationId,
                            c => c.ANTENNA.VH_PATTERN.Angle_deg,
                            c => c.ANTENNA.VH_PATTERN.Loss_dB,

                            c => c.ANTENNA.VV_PATTERN.StationId,
                            c => c.ANTENNA.VV_PATTERN.Angle_deg,
                            c => c.ANTENNA.VV_PATTERN.Loss_dB,

                            c => c.TRANSMITTER.StationId,
                            c => c.TRANSMITTER.Loss_dB,
                            c => c.TRANSMITTER.Freq_MHz,
                            c => c.TRANSMITTER.BW_kHz,
                            c => c.TRANSMITTER.MaxPower_dBm,
                            c => c.TRANSMITTER.PolarizationCode,
                            c => c.TRANSMITTER.Freqs_MHz,


                            c => c.RECEIVER.StationId,
                            c => c.RECEIVER.Loss_dB,
                            c => c.RECEIVER.Freq_MHz,
                            c => c.RECEIVER.BW_kHz,
                            c => c.RECEIVER.KTBF_dBm,
                            c => c.RECEIVER.Threshold_dBm,
                            c => c.RECEIVER.PolarizationCode,
                            c => c.RECEIVER.Freqs_MHz
                        )
                        .Where(c => c.Id, ConditionOperator.In, partStationIdsByCalcServer[i].ToArray());

                    var contextStation = _calcDbScope.Executor.ExecuteAndFetch(queryStation, reader =>
                    {
                        while (reader.Read())
                        {
                            var stationRecord = new ContextStation
                            {
                                Id = reader.GetValue(c => c.Id),
                                ContextId = _taskContext.ClientContextId,
                                CreatedDate = reader.GetValue(c => c.CreatedDate),
                                Type = (ClientContextStationType)reader.GetValue(c => c.StateCode),
                                CallSign = reader.GetValue(c => c.CallSign),
                                Name = reader.GetValue(c => c.Name),
                                Standard = reader.GetValue(c => c.Standard).GetStandardForDriveTest(),
                                RealStandard = reader.GetValue(c => c.Standard),
                                ExternalCode = reader.GetValue(c => c.ExternalCode),
                                ExternalSource = reader.GetValue(c => c.ExternalSource),
                                ModifiedDate = reader.GetValue(c => c.ModifiedDate),
                                LicenseGsid = reader.GetValue(c => c.LicenseGsid),
                                RealGsid = reader.GetValue(c => c.RealGsid),
                                RegionCode = reader.GetValue(c => c.RegionCode),

                                Site = new Wgs84Site
                                {
                                    Latitude = reader.GetValue(c => c.SITE.Latitude_DEC),
                                    Longitude = reader.GetValue(c => c.SITE.Longitude_DEC),
                                    Altitude = reader.GetValue(c => c.SITE.Altitude_m)
                                },
                                Coordinate = new AtdiCoordinate
                                {
                                    X = reader.GetValue(c => c.COORDINATES.AtdiX),
                                    Y = reader.GetValue(c => c.COORDINATES.AtdiY)
                                },
                                Antenna = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna
                                {
                                    Gain_dB = reader.GetValue(c => c.ANTENNA.Gain_dB),
                                    XPD_dB = reader.GetValue(c => c.ANTENNA.XPD_dB),
                                    Azimuth_deg = reader.GetValue(c => c.ANTENNA.Azimuth_deg),
                                    ItuPattern = (DataModels.Sdrn.DeepServices.RadioSystem.Stations.AntennaItuPattern)reader.GetValue(c => c.ANTENNA.ItuPatternCode),
                                    Tilt_deg = reader.GetValue(c => c.ANTENNA.Tilt_deg)
                                },
                            };

                            if (reader.IsNotNull(c => c.TRANSMITTER.StationId))
                            {
                                stationRecord.Transmitter = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationTransmitter
                                {
                                    Loss_dB = reader.GetValue(c => c.TRANSMITTER.Loss_dB),
                                    Polarization = (DataModels.Sdrn.DeepServices.RadioSystem.Stations.PolarizationType)reader.GetValue(c => c.TRANSMITTER.PolarizationCode),
                                    MaxPower_dBm = reader.GetValue(c => c.TRANSMITTER.MaxPower_dBm),
                                    BW_kHz = reader.GetValue(c => c.TRANSMITTER.BW_kHz),
                                    Freq_MHz = reader.GetValue(c => c.TRANSMITTER.Freq_MHz),
                                    Freqs_MHz = reader.GetValue(c => c.TRANSMITTER.Freqs_MHz)
                                };
                            }

                            if (reader.IsNotNull(c => c.RECEIVER.StationId))
                            {
                                stationRecord.Receiver = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationReceiver
                                {
                                    Loss_dB = reader.GetValue(c => c.RECEIVER.Loss_dB),
                                    Polarization = (DataModels.Sdrn.DeepServices.RadioSystem.Stations.PolarizationType)reader.GetValue(c => c.RECEIVER.PolarizationCode),
                                    KTBF_dBm = reader.GetValue(c => c.RECEIVER.KTBF_dBm),
                                    BW_kHz = reader.GetValue(c => c.RECEIVER.BW_kHz),
                                    Freq_MHz = reader.GetValue(c => c.RECEIVER.Freq_MHz),
                                    Threshold_dBm = reader.GetValue(c => c.RECEIVER.Threshold_dBm),
                                    Freqs_MHz = reader.GetValue(c => c.RECEIVER.Freqs_MHz)
                                };
                            }

                            if (reader.IsNotNull(c => c.ANTENNA.HH_PATTERN.StationId))
                            {
                                stationRecord.Antenna.HhPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.HH_PATTERN.Angle_deg);
                                stationRecord.Antenna.HhPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.HH_PATTERN.Loss_dB);
                            }
                            if (reader.IsNotNull(c => c.ANTENNA.HV_PATTERN.StationId))
                            {
                                stationRecord.Antenna.HvPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.HV_PATTERN.Angle_deg);
                                stationRecord.Antenna.HvPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.HV_PATTERN.Loss_dB);
                            }
                            if (reader.IsNotNull(c => c.ANTENNA.VH_PATTERN.StationId))
                            {
                                stationRecord.Antenna.VhPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.VH_PATTERN.Angle_deg);
                                stationRecord.Antenna.VhPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.VH_PATTERN.Loss_dB);
                            }
                            if (reader.IsNotNull(c => c.ANTENNA.VV_PATTERN.StationId))
                            {
                                stationRecord.Antenna.VvPattern.Angle_deg = reader.GetValue(c => c.ANTENNA.VV_PATTERN.Angle_deg);
                                stationRecord.Antenna.VvPattern.Loss_dB = reader.GetValue(c => c.ANTENNA.VV_PATTERN.Loss_dB);
                            }


                            var refSpectrumStationCalibration = stationRes.Find(x => x.StationMonitoringId == reader.GetValue(c => c.Id));
                            if (refSpectrumStationCalibration != null)
                            {
                                refSpectrumStationCalibration.contextStation = stationRecord;
                                refSpectrumStationCalibration.contextStation.RealGsid = refSpectrumStationCalibration.RealGsid;
                                refSpectrumStationCalibration.contextStation.Antenna.Azimuth_deg = refSpectrumStationCalibration.Azimuth_deg;
                                //refSpectrumStationCalibration.contextStation.Antenna.Freq_MHz = (float)refSpectrumStationCalibration.Freq_MHz;
                                refSpectrumStationCalibration.contextStation.Antenna.Tilt_deg = refSpectrumStationCalibration.Tilt_Deg;
                                refSpectrumStationCalibration.contextStation.Transmitter.Freq_MHz = (float)refSpectrumStationCalibration.Freq_MHz;
                                refSpectrumStationCalibration.contextStation.Transmitter.MaxPower_dBm = refSpectrumStationCalibration.Power_dB;
                                refSpectrumStationCalibration.contextStation.Site = new Wgs84Site();
                                refSpectrumStationCalibration.contextStation.Site.Altitude = refSpectrumStationCalibration.Altitude_m;
                                refSpectrumStationCalibration.contextStation.Site.Longitude = refSpectrumStationCalibration.Lon_deg;
                                refSpectrumStationCalibration.contextStation.Site.Latitude = refSpectrumStationCalibration.Lat_deg;
                            }

                            lstStations.Add(stationRecord);

                        }

                        return true;
                    });
                }

                var sensorParameters = new List<SensorParameters>();

                if (this._parameters.SensorIds != null)
                {
                    for (int i = 0; i < this._parameters.SensorIds.Length; i++)
                    {
                        SensorParameters sensorParameter = new SensorParameters();
                        var sensorId = this._parameters.SensorIds[i];
                        sensorParameter.SensorId = sensorId;
                        var querySensorLocation = _infocenterDataLayer.GetBuilder<IC.SdrnServer.ISensorLocation>()
                       .From()
                       .Select(
                       c => c.Asl,
                       c => c.Lon,
                       c => c.Lat
                       )
                       .Where(c => c.SENSOR.Id, ConditionOperator.Equal, sensorId)
                       .OrderByDesc(c => c.Id);

                        var isSensorLocation = _infoDbScope.Executor.ExecuteAndFetch(querySensorLocation, readerSensorLocation =>
                        {
                            if (!readerSensorLocation.Read())
                            {
                                return false;
                            }
                            if ((readerSensorLocation.GetValue(c => c.Lon) != null)
                                || (readerSensorLocation.GetValue(c => c.Lat) != null)
                                || (readerSensorLocation.GetValue(c => c.Asl) != null))
                            {
                                var wgs84Coordinate = new Wgs84Coordinate()
                                {
                                    Longitude = readerSensorLocation.GetValue(c => c.Lon).Value,
                                    Latitude = readerSensorLocation.GetValue(c => c.Lat).Value
                                };
                                sensorParameter.Coordinate = _transformation.ConvertCoordinateToAtdi(in wgs84Coordinate, this._parameters.Projection);
                            }
                            return true;
                        });

                        if (isSensorLocation == false)
                        {
                            // если координаты сенсора не найдены это ошибка
                            this._logger.Warning(Contexts.ThisComponent, Events.CoordinateNotFound.With(sensorId));
                            continue;
                        }


                        var querySensorAntenna = _infocenterDataLayer.GetBuilder<IC.SdrnServer.ISensorAntenna>()
                    .From()
                    .Select(
                    c => c.AddLoss,
                    c => c.SENSOR.RxLoss,
                    c => c.SENSOR.Agl
                    )
                    .Where(c => c.SENSOR.Id, ConditionOperator.Equal, sensorId);

                        _infoDbScope.Executor.ExecuteAndFetch(querySensorAntenna, readerSensorAntenna =>
                        {
                            while (readerSensorAntenna.Read())
                            {

                                if (readerSensorAntenna.GetValue(c => c.AddLoss).HasValue)
                                {
                                    sensorParameter.RxFeederLoss_dB = readerSensorAntenna.GetValue(c => c.AddLoss).Value;
                                }
                                if (readerSensorAntenna.GetValue(c => c.SENSOR.RxLoss).HasValue)
                                {
                                    sensorParameter.RxFeederLoss_dB += readerSensorAntenna.GetValue(c => c.SENSOR.RxLoss).Value;
                                }

                                if (readerSensorAntenna.GetValue(c => c.SENSOR.Agl).HasValue)
                                {
                                    if ((float)readerSensorAntenna.GetValue(c => c.SENSOR.Agl).Value >= 10)
                                    {
                                        sensorParameter.SensorAntennaHeight_m = (float)readerSensorAntenna.GetValue(c => c.SENSOR.Agl).Value;
                                    }
                                    else
                                    {
                                        sensorParameter.SensorAntennaHeight_m = 10;
                                    }
                                }
                                else
                                {
                                    sensorParameter.SensorAntennaHeight_m = 10;
                                }

                                break;
                            }
                            return true;
                        });

                        var lstSensorStationAntenna = new List<DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna>();

                        var queryAntennaPattern = _infocenterDataLayer.GetBuilder<IC.SdrnServer.ISensorAntennaPattern>()
                       .From()
                       .Select(
                       c => c.DiagA,
                       c => c.DiagH,
                       c => c.DiagV,
                       c => c.Freq,
                       c => c.Gain,
                       c => c.SENSOR_ANTENNA.Xpd,
                       c => c.SENSOR_ANTENNA.AddLoss,
                       c => c.SENSOR_ANTENNA.SENSOR.Azimuth,
                       c => c.SENSOR_ANTENNA.SENSOR.RxLoss,
                       c => c.SENSOR_ANTENNA.SENSOR.Elevation
                       )
                       .Where(c => c.SENSOR_ANTENNA.SENSOR.Id, ConditionOperator.Equal, sensorId)
                       .OrderByDesc(c => c.Freq);

                        _infoDbScope.Executor.ExecuteAndFetch(queryAntennaPattern, readerAntennaPattern =>
                        {
                            while (readerAntennaPattern.Read())
                            {

                                var sensorAntenna = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna();


                                sensorAntenna = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna();

                                if (readerAntennaPattern.GetValue(c => c.Gain).HasValue)
                                {
                                    sensorAntenna.Gain_dB = (float)readerAntennaPattern.GetValue(c => c.Gain);
                                }

                                if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.Xpd).HasValue)
                                {
                                    sensorAntenna.XPD_dB = (float)readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.Xpd);
                                }

                                if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Azimuth).HasValue)
                                {
                                    sensorAntenna.Azimuth_deg = (float)readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Azimuth);
                                }

                                if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Elevation).HasValue)
                                {
                                    sensorAntenna.Tilt_deg = (float)readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Elevation);
                                }

                                if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Elevation).HasValue)
                                {
                                    sensorAntenna.Freq_MHz = (float)readerAntennaPattern.GetValue(c => c.Freq);
                                }


                                if (readerAntennaPattern.GetValue(c => c.DiagH) != null)
                                {
                                    // HH
                                    var argsHH = new DiagrammArgs()
                                    {
                                        AntennaPatternType = AntennaPatternType.HH,
                                        Gain = (float)readerAntennaPattern.GetValue(c => c.Gain),
                                        Points = readerAntennaPattern.GetValue(c => c.DiagH)
                                    };
                                    var diagrammPointsResult = new DiagrammPoint[1000];

                                    this._signalService.CalcAntennaPattern(in argsHH, ref diagrammPointsResult);
                                    if (diagrammPointsResult[0].Angle != null)
                                    {
                                        sensorAntenna.HhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        sensorAntenna.HhPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                        sensorAntenna.HhPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                    };


                                    // HV
                                    var argsHV = new DiagrammArgs()
                                    {
                                        AntennaPatternType = AntennaPatternType.HV,
                                        Gain = (float)readerAntennaPattern.GetValue(c => c.Gain),
                                        Points = readerAntennaPattern.GetValue(c => c.DiagH)
                                    };
                                    diagrammPointsResult = new DiagrammPoint[1000];

                                    this._signalService.CalcAntennaPattern(in argsHV, ref diagrammPointsResult);
                                    if (diagrammPointsResult[0].Angle != null)
                                    {
                                        sensorAntenna.HvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        sensorAntenna.HvPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                        sensorAntenna.HvPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                    };
                                }
                                else
                                {
                                    // HH

                                    sensorAntenna.HhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                    sensorAntenna.HhPattern.Loss_dB = new float[2] { 0, 0 };
                                    sensorAntenna.HhPattern.Angle_deg = new double[2] { 0, 350 };

                                    // HV
                                    sensorAntenna.HvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                    sensorAntenna.HvPattern.Loss_dB = new float[2] { 0, 0 };
                                    sensorAntenna.HvPattern.Angle_deg = new double[2] { 0, 350 };

                                }

                                if (readerAntennaPattern.GetValue(c => c.DiagV) != null)
                                {
                                    // VH
                                    var argsVH = new DiagrammArgs()
                                    {
                                        AntennaPatternType = AntennaPatternType.VH,
                                        Gain = (float)readerAntennaPattern.GetValue(c => c.Gain),
                                        Points = readerAntennaPattern.GetValue(c => c.DiagV)
                                    };
                                    var diagrammPointsResult = new DiagrammPoint[1000];

                                    this._signalService.CalcAntennaPattern(in argsVH, ref diagrammPointsResult);
                                    if (diagrammPointsResult[0].Angle != null)
                                    {
                                        sensorAntenna.VhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        sensorAntenna.VhPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                        sensorAntenna.VhPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                    };


                                    // VV
                                    var argsVV = new DiagrammArgs()
                                    {
                                        AntennaPatternType = AntennaPatternType.VV,
                                        Gain = (float)readerAntennaPattern.GetValue(c => c.Gain),
                                        Points = readerAntennaPattern.GetValue(c => c.DiagV)
                                    };
                                    diagrammPointsResult = new DiagrammPoint[1000];

                                    this._signalService.CalcAntennaPattern(in argsVV, ref diagrammPointsResult);
                                    if (diagrammPointsResult[0].Angle != null)
                                    {
                                        sensorAntenna.VvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        sensorAntenna.VvPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                        sensorAntenna.VvPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                    };
                                }
                                else
                                {

                                    // VH

                                    sensorAntenna.VhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                    sensorAntenna.VhPattern.Loss_dB = new float[2] { 0, 0 };
                                    sensorAntenna.VhPattern.Angle_deg = new double[2] { -90, 90 };

                                    // VV
                                    sensorAntenna.VvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                    sensorAntenna.VvPattern.Loss_dB = new float[2] { 0, 0 };
                                    sensorAntenna.VvPattern.Angle_deg = new double[2] { -90, 90 };

                                }
                                lstSensorStationAntenna.Add(sensorAntenna);
                            }
                            return true;
                        });


                        if (lstSensorStationAntenna.Count == 0)
                        {
                            var sensorAntenna = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna();
                            sensorAntenna = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna();
                            sensorAntenna.Gain_dB = 0;
                            sensorAntenna.XPD_dB = 25;
                            sensorAntenna.Azimuth_deg = 0;
                            sensorAntenna.Tilt_deg = 0;
                            sensorAntenna.Freq_MHz = 1000;

                            // HH
                            sensorAntenna.HhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                            sensorAntenna.HhPattern.Loss_dB = new float[2] { 0, 0 };
                            sensorAntenna.HhPattern.Angle_deg = new double[2] { 0, 350 };

                            // HV
                            sensorAntenna.HvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                            sensorAntenna.HvPattern.Loss_dB = new float[2] { 0, 0 };
                            sensorAntenna.HvPattern.Angle_deg = new double[2] { 0, 350 };

                            // VH

                            sensorAntenna.VhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                            sensorAntenna.VhPattern.Loss_dB = new float[2] { 0, 0 };
                            sensorAntenna.VhPattern.Angle_deg = new double[2] { -90, 90 };

                            // VV
                            sensorAntenna.VvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                            sensorAntenna.VvPattern.Loss_dB = new float[2] { 0, 0 };
                            sensorAntenna.VvPattern.Angle_deg = new double[2] { -90, 90 };


                            lstSensorStationAntenna.Add(sensorAntenna);
                        }

                        sensorParameter.SensorAntennas = lstSensorStationAntenna.ToArray();
                        sensorParameters.Add(sensorParameter);
                    }
                }

                this._sensorParameters = sensorParameters.ToArray();
                this._refSpectrumStationCalibrations = stationRes.ToArray();
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.RefSpectrumByDriveTestsCalcTask, e);
            }
        }

        private long CreateResult()
        {
            var insertQueryRefSpectrumByDriveTestsResult = _calcServerDataLayer.GetBuilder<IRefSpectrumByDriveTestsResult>()
                .Insert()
                .SetValue(c => c.DateCreated, DateTime.Now)
                .SetValue(c => c.PARAMETERS.TaskId, _taskContext.TaskId)
                .SetValue(c => c.RESULT.Id, _taskContext.ResultId)
                ;
            var key = _calcDbScope.Executor.Execute<IRefSpectrumByDriveTestsResult_PK>(insertQueryRefSpectrumByDriveTestsResult);
            return key.Id;
        }

        private void SaveTaskResult(ResultRefSpectrumBySensors result, long resultId)
        {
            var insertQueryRefSpectrumByDriveTestsDetailResult = _calcServerDataLayer.GetBuilder<IRefSpectrumByDriveTestsDetailResult>()
                .Insert()
                .SetValue(c => c.OrderId, result.OrderId);
            if (result.DateMeas != null)
            {
                insertQueryRefSpectrumByDriveTestsDetailResult.SetValue(c => c.DateMeas, result.DateMeas);
            }
            insertQueryRefSpectrumByDriveTestsDetailResult.SetValue(c => c.Freq_MHz, result.Freq_MHz)
            .SetValue(c => c.GlobalCID, result.GlobalCID)
            .SetValue(c => c.IdIcsm, result.IdIcsm)
            .SetValue(c => c.IdSensor, result.IdSensor)
            .SetValue(c => c.Level_dBm, result.Level_dBm)
            .SetValue(c => c.Percent, result.Percent)
            .SetValue(c => c.TableIcsmName, result.TableIcsmName)
            .SetValue(c => c.RESULT_REF_SPECTRUM.Id, resultId)
            ;
            _calcDbScope.Executor.Execute<IRefSpectrumByDriveTestsDetailResult_PK>(insertQueryRefSpectrumByDriveTestsDetailResult);
        }
    }
}
