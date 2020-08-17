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

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
	[TaskHandler(CalcTaskType.RefSpectrumByDriveTestsCalcTask)]
	public class RefSpectrumByDriveTestsCalcTask : ITaskHandler
	{
		private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
        private readonly IDataLayer<EntityDataOrm<IC.InfocenterEntityOrmContext>> _infocenterDataLayer;
        private readonly IMapRepository _mapRepository;
        public  readonly ISignalService _signalService;
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
        private RefSpectrumStationAndDriveTest[] _refSpectrumStationAndDriveTest;





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


        public void Run()
        {
            try
            {
                var iterationReceivedPowerCalcResult = _iterationsPool.GetIteration<ReceivedPowerCalcData, ReceivedPowerCalcResult>();
                var iterationPercentTimeForGainCalcDataResult = _iterationsPool.GetIteration<PercentTimeForGainCalcData, double[]>();
                var mapData = _mapRepository.GetMapByName(this._calcDbScope, this._taskContext.ProjectId, this._parameters.MapName);
                var propagationModel = _contextService.GetPropagationModel(this._calcDbScope, this._taskContext.ClientContextId);
                var listResultRefSpectrumByDriveTests = new List<ResultRefSpectrumByDriveTests>();
                var skipDriveTests = new List<ContextStation>();
                var listRefSpectrumStationAndDriveTest = this._refSpectrumStationAndDriveTest.ToList();
                var driveTestParameters = listRefSpectrumStationAndDriveTest.Select(x => x.DriveTestParameters).ToList();
                var sensorIds = driveTestParameters.Select(x => x.SensorId).Distinct().ToArray();
                var freqs_MHz = driveTestParameters.Select(x => x.Freq_MHz).Distinct().ToArray();
                int index = 0;
                int percentComplete = 0;

                for (int i = 0; i < sensorIds.Length; i++)
                {
                    var lstReceivedPowerCalcResult = new List<ReceivedPowerCalcResult>();
                    float sensorAntennaHeight_m = 0;
                    for (int j = 0; j < freqs_MHz.Length; j++)
                    {
                        for (int k = 0; k < this._refSpectrumStationAndDriveTest.Length; k++)
                        {
                            var refSpectrumStation = this._refSpectrumStationAndDriveTest[k];
                            if (((refSpectrumStation.DriveTestParameters.Freq_MHz == freqs_MHz[j])
                                && (refSpectrumStation.DriveTestParameters.SensorId == sensorIds[i])) == false)
                            {
                                continue;
                            }
                            else
                            {
                                var contextStation = _refSpectrumStationAndDriveTest[k].ContextStation;
                                var driveTest = _refSpectrumStationAndDriveTest[k].DriveTestParameters;


                                var receivedPowerCalcData = new ReceivedPowerCalcData();
                                receivedPowerCalcData.BuildingContent = mapData.BuildingContent;
                                receivedPowerCalcData.ClutterContent = mapData.ClutterContent;
                                receivedPowerCalcData.CluttersDesc = _mapRepository.GetCluttersDesc(this._calcDbScope, mapData.Id);
                                receivedPowerCalcData.MapArea = mapData.Area;
                                receivedPowerCalcData.PropagationModel = propagationModel;
                                receivedPowerCalcData.ReliefContent = mapData.ReliefContent;
                                receivedPowerCalcData.Transmitter = contextStation.Transmitter;
                                receivedPowerCalcData.TxAntenna = contextStation.Antenna;
                                var wgs84Coordinate = new Wgs84Coordinate()
                                {
                                    Longitude = contextStation.Site.Longitude,
                                    Latitude = contextStation.Site.Latitude
                                };
                                receivedPowerCalcData.TxCoordinate = _transformation.ConvertCoordinateToAtdi(in wgs84Coordinate, this._parameters.Projection);
                                receivedPowerCalcData.TxAltitude_m = contextStation.Site.Altitude;
                                receivedPowerCalcData.RxAntenna = driveTest.SensorAntenna;
                                receivedPowerCalcData.RxCoordinate = driveTest.Coordinate;
                                receivedPowerCalcData.RxFeederLoss_dB = driveTest.RxFeederLoss_dB;
                                receivedPowerCalcData.TxFreq_Mhz = driveTest.Freq_MHz;
                                receivedPowerCalcData.RxAltitude_m = driveTest.SensorAntennaHeight_m;
                                sensorAntennaHeight_m = driveTest.SensorAntennaHeight_m;

                                var resultRefSpectrumByDriveTests = new ResultRefSpectrumByDriveTests();
                                resultRefSpectrumByDriveTests.OrderId = index;
                                resultRefSpectrumByDriveTests.TableIcsmName = contextStation.ExternalSource;
                                resultRefSpectrumByDriveTests.IdIcsm = Convert.ToInt64(contextStation.ExternalCode);
                                resultRefSpectrumByDriveTests.GlobalCID = driveTest.GSID;
                                resultRefSpectrumByDriveTests.Freq_MHz = driveTest.Freq_MHz;
                                resultRefSpectrumByDriveTests.DateMeas = new DateTimeOffset(driveTest.MeasTime.Value);
                                resultRefSpectrumByDriveTests.IdSensor = sensorIds[i].Value;

                                // вызов итерации определения уровня сигнала Level
                                var resulLevelCalc = iterationReceivedPowerCalcResult.Run(_taskContext, receivedPowerCalcData);
                                //ReceivedPowerCalcResult resulLevelCalc = new ReceivedPowerCalcResult();
                                //resulLevelCalc.Level_dBm = 30.45;
                                //resulLevelCalc.Frequency_Mhz = driveTest.Freq_MHz;
                                //resulLevelCalc.Distance_km = 34;
                                //resulLevelCalc.AntennaHeight_m = 3;
                                if (resulLevelCalc.Level_dBm.Value < this._parameters.PowerThreshold_dBm)
                                {
                                    if (skipDriveTests.Find(x => x.Id == contextStation.Id) == null)
                                    {
                                        skipDriveTests.Add(contextStation);
                                        continue;
                                    }
                                }
                                else
                                {
                                    resultRefSpectrumByDriveTests.Level_dBm = resulLevelCalc.Level_dBm.Value;
                                }

                                index++;
                                lstReceivedPowerCalcResult.Add(resulLevelCalc);
                                listResultRefSpectrumByDriveTests.Add(resultRefSpectrumByDriveTests);
                            }


                            var clc = (double)(100.0 / (double)(freqs_MHz.Length * this._refSpectrumStationAndDriveTest.Length * sensorIds.Length));
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
                        }
                    }
                    var percentTimeForGainCalcData = new PercentTimeForGainCalcData();
                    percentTimeForGainCalcData.SensorAntennaHeight_m = sensorAntennaHeight_m;
                    percentTimeForGainCalcData.SensorId = sensorIds[i].Value;
                    percentTimeForGainCalcData.StationData = lstReceivedPowerCalcResult.ToArray();

                    // вызов итерации определения процента времени P, в течение которого излучение данного сектора доминирует на данном сенсоре
                    var resultPercent = iterationPercentTimeForGainCalcDataResult.Run(_taskContext, percentTimeForGainCalcData);
                    if ((resultPercent != null) && (resultPercent.Length > 0))
                    {
                        for (int n = 0; n < percentTimeForGainCalcData.StationData.Length; n++)
                        {
                            var fndStationData = listResultRefSpectrumByDriveTests.Find(x => x.IdSensor == sensorIds[i].Value && x.Freq_MHz == percentTimeForGainCalcData.StationData[n].Frequency_Mhz.Value);
                            if (fndStationData != null)
                            {
                                fndStationData.Percent = resultPercent[n];
                            }
                        }
                    }
                }


                if (skipDriveTests.Count > 0)
                {
                    // справочник с данными по стандарту и количеству базовых станций, которые были пропущены по причине невыполнения условия (resulLevelCalc.Level_dBm.Value < this._parameters.PowerThreshold_dBm)
                    string baseStations = "";
                    var standards = driveTestParameters.Select(x => x.Standard).Distinct().ToArray();
                    for (int n = 0; n < standards.Length; n++)
                    {
                        var allDriveTestsByStandard = driveTestParameters.FindAll(x => x.Standard == standards[n]);
                        if ((allDriveTestsByStandard != null) && (allDriveTestsByStandard.Count > 0))
                        {
                            var distinctDriveTests = allDriveTestsByStandard.Select(x => x.GSID).Distinct();
                            if ((distinctDriveTests != null) && (distinctDriveTests.Count() > 0))
                            {
                                baseStations += $"{standards[n]}: {distinctDriveTests.Count()} base stations;" + Environment.NewLine;
                            }
                        }
                    }
                    string message = "The following emission sources have too small signal level and are not associated with any sensor:" + Environment.NewLine;
                    message += baseStations;
                    message += "Please check the power threshold value" + Environment.NewLine;
                    this._taskContext.SendEvent(new CalcResultEvent
                    {
                        Level = CalcResultEventLevel.Warning,
                        Context = "RefSpectrumByDriveTestsCalcTask",
                        Message = message
                    });
                }


                ///// запись результатов
                if ((listResultRefSpectrumByDriveTests != null) && (listResultRefSpectrumByDriveTests.Count > 0))
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


                    for (int i = 0; i < listResultRefSpectrumByDriveTests.Count; i++)
                    {
                        SaveTaskResult(listResultRefSpectrumByDriveTests[i], resultId);
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
                                    c => c.ResultId

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
                        ResultId = reader.GetValue(c => c.ResultId)
                    };
                });

                var stationIdsRes = new List<long>();


                var partstationIdsRes = BreakDownElemBlocks.BreakDown(this._parameters.StationIds);
                for (int i = 0; i < partstationIdsRes.Count; i++)
                {
                    var queryStationCalibrationStaResult = _calcServerDataLayer.GetBuilder<IStationCalibrationStaResult>()
                     .From()
                     .Select(
                         c => c.StationMonitoringId
                     )
                    .Where(c => c.Id, ConditionOperator.In, partstationIdsRes[i]);

                    _calcDbScope.Executor.ExecuteAndFetch(queryStationCalibrationStaResult, readerStationCalibrationStaResult =>
                    {
                        while (readerStationCalibrationStaResult.Read())
                        {
                            stationIdsRes.Add(readerStationCalibrationStaResult.GetValue(c => c.StationMonitoringId));
                        }
                        return true;
                    });
                }

                this._parameters.StationIds = stationIdsRes.ToArray();

                // load stations
                List<ContextStation> lstStations = new List<ContextStation>();

                var refSpectrumStationAndDriveTest = new List<RefSpectrumStationAndDriveTest>();

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
                        //.Where(c => c.CONTEXT.Id, ConditionOperator.Equal, _taskContext.ClientContextId)
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

                            lstStations.Add(stationRecord);

                        }

                        return true;
                    });
                }


                for (int i = 0; i < lstStations.Count; i++)
                {
                    var stationRecord = lstStations[i];
                    // поиск связанных драйв тестов
                    DriveTestParameters driveTestParameters = null;
                    var queryStationCalibrationDriveTestResult = _calcServerDataLayer.GetBuilder<IStationCalibrationDriveTestResult>()
                      .From()
                      .Select(
                          c => c.CalibrationResultId,
                          c => c.CountPointsInDriveTest,
                          c => c.DriveTestId,
                          c => c.ExternalCode,
                          c => c.ExternalSource,
                          c => c.Freq_MHz,
                          c => c.Id,
                          c => c.LinkToStationMonitoringId,
                          c => c.MaxPercentCorellation,
                          c => c.MeasGcid,
                          c => c.ResultDriveTestStatus,
                          c => c.Standard,
                          c => c.StationGcid
                      )
                     .Where(c => c.LinkToStationMonitoringId, ConditionOperator.Equal, stationRecord.Id)
                    .Where(c => c.CALCRESULTS_STATION_CALIBRATION.Id, ConditionOperator.Equal, this._parameters.ResultId);

                    _calcDbScope.Executor.ExecuteAndFetch(queryStationCalibrationDriveTestResult, readerStationCalibrationDriveTestResult =>
                    {
                        if (!readerStationCalibrationDriveTestResult.Read())
                        {
                            return false;
                        }
                        driveTestParameters = new DriveTestParameters()
                        {
                            CountPoints = readerStationCalibrationDriveTestResult.GetValue(c => c.CountPointsInDriveTest),
                            Standard = readerStationCalibrationDriveTestResult.GetValue(c => c.Standard),
                            DriveTestId = readerStationCalibrationDriveTestResult.GetValue(c => c.DriveTestId),
                            Freq_MHz = readerStationCalibrationDriveTestResult.GetValue(c => c.Freq_MHz),
                            GSID = readerStationCalibrationDriveTestResult.GetValue(c => c.StationGcid),
                            MaxCorrelation = readerStationCalibrationDriveTestResult.GetValue(c => c.MaxPercentCorellation),
                        };

                        return true;
                    });

                    if (driveTestParameters != null)
                    {
                        var queryDriveTest = _infocenterDataLayer.GetBuilder<IC.SdrnServer.IDriveTest>()
                      .From()
                      .Select(
                          c => c.RESULT.SensorName,
                          c => c.RESULT.SensorTitle,
                          c => c.RESULT.SENSOR.Id,
                          c => c.RESULT.MeasTime,
                          c => c.RESULT.Id
                      )
                      .Where(c => c.Id, ConditionOperator.Equal, driveTestParameters.DriveTestId);

                        _infoDbScope.Executor.ExecuteAndFetch(queryDriveTest, readerStationMonitoring =>
                        {
                            if (!readerStationMonitoring.Read())
                            {
                                return false;
                            }
                            driveTestParameters.SensorId = readerStationMonitoring.GetValue(c => c.RESULT.SENSOR.Id); // ????
                            driveTestParameters.SensorName = readerStationMonitoring.GetValue(c => c.RESULT.SensorName);
                            driveTestParameters.SensorTitle = readerStationMonitoring.GetValue(c => c.RESULT.SensorTitle);
                            driveTestParameters.MeasTime = readerStationMonitoring.GetValue(c => c.RESULT.MeasTime);
                            return true;
                        });


                        if (driveTestParameters.SensorId != null)
                        {

                            var querySensorLocation = _infocenterDataLayer.GetBuilder<IC.SdrnServer.ISensorLocation>()
                           .From()
                           .Select(
                           c => c.Asl,
                           c => c.Lon,
                           c => c.Lat
                           )
                           .Where(c => c.SENSOR.Id, ConditionOperator.Equal, driveTestParameters.SensorId.Value)
                           .OrderByDesc(c=>c.Id);

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
                                    driveTestParameters.Coordinate = _transformation.ConvertCoordinateToAtdi(in wgs84Coordinate, this._parameters.Projection);
                                    driveTestParameters.SensorAntennaHeight_m = 3;//(float)readerSensorLocation.GetValue(c => c.Asl).Value;
                                }
                                return true;
                            });

                            if (isSensorLocation == false)
                            {
                                // если координаты сенсора не найдены это ошибка
                                this._logger.Warning(Contexts.ThisComponent, Events.CoordinateNotFound.With(driveTestParameters.SensorId.Value));
                                continue;
                            }

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
                           c => c.SENSOR_ANTENNA.SENSOR.Elevation
                           )
                           .Where(c => c.SENSOR_ANTENNA.SENSOR.Id, ConditionOperator.Equal, driveTestParameters.SensorId.Value)
                           .OrderByDesc(c => c.Freq);

                            _infoDbScope.Executor.ExecuteAndFetch(queryAntennaPattern, readerAntennaPattern =>
                            {
                                while (readerAntennaPattern.Read())
                                {
                                    if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.AddLoss).HasValue)
                                    {
                                        driveTestParameters.RxFeederLoss_dB = readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.AddLoss).Value;
                                    }

                                    driveTestParameters.SensorAntenna = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntenna();

                                    if (readerAntennaPattern.GetValue(c => c.Gain).HasValue)
                                    {
                                        driveTestParameters.SensorAntenna.Gain_dB = (float)readerAntennaPattern.GetValue(c => c.Gain);
                                    }

                                    if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.Xpd).HasValue)
                                    {
                                        driveTestParameters.SensorAntenna.XPD_dB = (float)readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.Xpd);
                                    }

                                    if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Azimuth).HasValue)
                                    {
                                        driveTestParameters.SensorAntenna.Azimuth_deg = (float)readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Azimuth);
                                    }
                                   
                                    if (readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Elevation).HasValue)
                                    {
                                        driveTestParameters.SensorAntenna.Tilt_deg = (float)readerAntennaPattern.GetValue(c => c.SENSOR_ANTENNA.SENSOR.Elevation);
                                    }




                                    if (readerAntennaPattern.GetValue(c => c.DiagH) != null)
                                    {
                                        // HH
                                        var argsHH = new DiagrammArgs()
                                        {
                                            AntennaPatternType = AntennaPatternType.HH,
                                            Gain = driveTestParameters.SensorAntenna.Gain_dB,
                                            Points = readerAntennaPattern.GetValue(c => c.DiagH)
                                        };
                                        var diagrammPointsResult = new DiagrammPoint[1000];

                                        this._signalService.CalcAntennaPattern(in argsHH, ref diagrammPointsResult);
                                        if (diagrammPointsResult[0].Angle != null)
                                        {
                                            driveTestParameters.SensorAntenna.HhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                            driveTestParameters.SensorAntenna.HhPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                            driveTestParameters.SensorAntenna.HhPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                        };


                                        // HV
                                        var argsHV = new DiagrammArgs()
                                        {
                                            AntennaPatternType = AntennaPatternType.HV,
                                            Gain = driveTestParameters.SensorAntenna.Gain_dB,
                                            Points = readerAntennaPattern.GetValue(c => c.DiagH)
                                        };
                                        diagrammPointsResult = new DiagrammPoint[1000];

                                        this._signalService.CalcAntennaPattern(in argsHV, ref diagrammPointsResult);
                                        if (diagrammPointsResult[0].Angle != null)
                                        {
                                            driveTestParameters.SensorAntenna.HvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                            driveTestParameters.SensorAntenna.HvPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                            driveTestParameters.SensorAntenna.HvPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                        };
                                    }
                                    else
                                    {
                                        // HH

                                        driveTestParameters.SensorAntenna.HhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        driveTestParameters.SensorAntenna.HhPattern.Loss_dB = new float[2] { 0, 0 };
                                        driveTestParameters.SensorAntenna.HhPattern.Angle_deg = new double[2] { 0, 350 };

                                        // HV
                                        driveTestParameters.SensorAntenna.HvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        driveTestParameters.SensorAntenna.HvPattern.Loss_dB = new float[2] { 0, 0 };
                                        driveTestParameters.SensorAntenna.HvPattern.Angle_deg = new double[2] { 0, 350 };

                                    }

                                    if (readerAntennaPattern.GetValue(c => c.DiagV) != null)
                                    {
                                        // VH
                                        var argsVH = new DiagrammArgs()
                                        {
                                            AntennaPatternType = AntennaPatternType.VH,
                                            Gain = driveTestParameters.SensorAntenna.Gain_dB,
                                            Points = readerAntennaPattern.GetValue(c => c.DiagV)
                                        };
                                        var diagrammPointsResult = new DiagrammPoint[1000];

                                        this._signalService.CalcAntennaPattern(in argsVH, ref diagrammPointsResult);
                                        if (diagrammPointsResult[0].Angle != null)
                                        {
                                            driveTestParameters.SensorAntenna.VhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                            driveTestParameters.SensorAntenna.VhPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                            driveTestParameters.SensorAntenna.VhPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                        };


                                        // VV
                                        var argsVV = new DiagrammArgs()
                                        {
                                            AntennaPatternType = AntennaPatternType.VV,
                                            Gain = driveTestParameters.SensorAntenna.Gain_dB,
                                            Points = readerAntennaPattern.GetValue(c => c.DiagV)
                                        };
                                        diagrammPointsResult = new DiagrammPoint[1000];

                                        this._signalService.CalcAntennaPattern(in argsVV, ref diagrammPointsResult);
                                        if (diagrammPointsResult[0].Angle != null)
                                        {
                                            driveTestParameters.SensorAntenna.VvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                            driveTestParameters.SensorAntenna.VvPattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                            driveTestParameters.SensorAntenna.VvPattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                                        };
                                    }
                                    else
                                    {

                                        // VH

                                        driveTestParameters.SensorAntenna.VhPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        driveTestParameters.SensorAntenna.VhPattern.Loss_dB = new float[2] { 0, 0 };
                                        driveTestParameters.SensorAntenna.VhPattern.Angle_deg = new double[2] { -90, 90 };

                                        // VV
                                        driveTestParameters.SensorAntenna.VvPattern = new DataModels.Sdrn.DeepServices.RadioSystem.Stations.StationAntennaPattern();
                                        driveTestParameters.SensorAntenna.VvPattern.Loss_dB = new float[2] { 0, 0 };
                                        driveTestParameters.SensorAntenna.VvPattern.Angle_deg = new double[2] { -90, 90 };


                                    }
                                    break;
                                }
                                return true;
                            });

                        }
                    }

                    if (driveTestParameters != null)
                    {
                        refSpectrumStationAndDriveTest.Add(new RefSpectrumStationAndDriveTest()
                        {
                            ContextStation = stationRecord,
                            DriveTestParameters = driveTestParameters
                        });
                    }

                }

                this._refSpectrumStationAndDriveTest = refSpectrumStationAndDriveTest.ToArray();
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

        private void SaveTaskResult(ResultRefSpectrumByDriveTests result, long resultId)
        {
            var insertQueryRefSpectrumByDriveTestsDetailResult = _calcServerDataLayer.GetBuilder<IRefSpectrumByDriveTestsDetailResult>()
                .Insert()
                .SetValue(c => c.OrderId, result.OrderId)
                .SetValue(c => c.DateMeas, result.DateMeas)
                .SetValue(c => c.Freq_MHz, result.Freq_MHz)
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
