using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.Platform.Logging;
using Atdi.Platform.Data;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// 
    /// </summary>
    public class DetermineStationParametersCalcIteration : IIterationHandler<AllStationCorellationCalcData, CalibrationResult>
    {
        private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly IObjectPool<CalcPoint[]> _calcPointArrayPool;
        private readonly IObjectPoolSite _poolSite;
        private readonly AppServerComponentConfig _appServerComponentConfig;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public DetermineStationParametersCalcIteration(
            IIterationsPool iterationsPool,
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _poolSite = poolSite;
            _appServerComponentConfig = appServerComponentConfig;
            _calcPointArrayPool = _poolSite.GetPool<CalcPoint[]>(ObjectPools.StationCalibrationCalcPointArrayObjectPool);
            _logger = logger;
        }

        public CalibrationResult Run(ITaskContext taskContext, AllStationCorellationCalcData data)
        {
            var calcCorellationResult = new CalibrationResult();
            var calcPointArrayBuffer = default(CalcPoint[]);
            // вызываем механизм расчета FieldStrengthCalcData на основе переданных данных data.FieldStrengthCalcData
            var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();


            try
            {

                // предварительная обработка
                ////////////////////////////////////////////////////////////////////////////////////////////
                ///
                /// Усредняем результаты измерения внутри пикселя.  Т.е. берем точки из DriveTestsResult группируем по пикселям карты.
                /// Далее внутри каждого пикселя вычисляем среднее значение напряженности поля. 
                /// Т.е. на выходе у нас количество точек в DriveTestsResult может сократиться.
                /// А координаты уже будут центральные координаты пикселя.
                /// Должно также быть ограничено количество точек (уже выходных).
                /// Не более 5000 (данное число в конфигурацию). Если их будет более, то их следует перфорировать.
                /// Логика перфорации следующая. Перфарируем с коэфициентом 2. Если этого окажеться недостаточно, то перфорацию повторим.
                /// Все точки PointFS[] Points сортируем по Lat (если есть одинаковые Lat то внутри по Lon)
                /// Все DriveTests должны пройти это при этом на входе у нас будет большой объём данных, а на выходе контролируемый. 
                ///////////////////////////////////////////////////////////////////////////////////////////
                 Utils.PrepareData(ref data.GSIDGroupeStation, ref data.GSIDGroupeDriveTests);


                ///////////////////////////////////////////////////////////////////////////////////////////
                ///    
                ///     включение механизма перфорации DriveTests
                /// 
                ///////////////////////////////////////////////////////////////////////////////////////////
                Utils.PerforationDriveTestResults(ref data.GSIDGroupeDriveTests);

                for (int i = 0; i < data.GSIDGroupeDriveTests.Length; i++)
                {
                    while (true)
                    {
                        if ((data.GSIDGroupeDriveTests[i].Points != null) && (data.GSIDGroupeDriveTests[i].Points.Length > _appServerComponentConfig.MaxCountPointInDriveTest))
                        {
                            // включение механизма перфорации по Points
                            Utils.PerforationPoints(ref data.GSIDGroupeDriveTests[i].Points);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////
                ///     
                ///     включение механизма перфорации по всем станциям
                /// 
                ///////////////////////////////////////////////////////////////////////////////////////////
                var perforationStations = Utils.PerforationStations(data.GSIDGroupeStation);
                data.GSIDGroupeStation = perforationStations.ToArray();


                ///////////////////////////////////////////////////////////////////////////////////////////
                ///
                ///  4.2.1. Формирование соответствий между станциями и результатами измерений (схема бл 1)
                /// 
                ///////////////////////////////////////////////////////////////////////////////////////////
                var allStandards = new List<string>();
                allStandards.AddRange(Utils.GetUniqueArrayStandardsfromStations(data.GSIDGroupeStation));
                allStandards.AddRange(Utils.GetUniqueArrayStandardsFromDriveTests(data.GSIDGroupeDriveTests));
                var arrStandards = allStandards.Distinct().ToArray();
                for (int v = 0; v < arrStandards.Length; v++)
                {
                    var standard = arrStandards[v];
                    var linkDriveTestAndStations = Utils.CompareDriveTestAndStation(data.GSIDGroupeDriveTests, data.GSIDGroupeStation, standard , out DriveTestsResult[][] outDriveTestsResults, out ContextStation[][] outContextStations);

                    for (int z = 0; z < linkDriveTestAndStations.Length; z++)
                    {
                        for (int i = 0; i < linkDriveTestAndStations[z].ContextStation.Length; i++)
                        {
                            for (int j = 0; j < linkDriveTestAndStations[z].DriveTestsResults.Length; j++)
                            {
                                var station = linkDriveTestAndStations[z].ContextStation;
                                var driveTest = linkDriveTestAndStations[z].DriveTestsResults;
                                for (int h = 0; h < station.Length; h++)
                                {
                                    for (int b = 0; b < driveTest.Length; b++)
                                    {
                                        var fieldStrengthCalcData = new FieldStrengthCalcData
                                        {
                                            Antenna = station[i].Antenna,
                                            PropagationModel = data.PropagationModel,
                                            PointCoordinate = station[i].Coordinate,
                                            PointAltitude_m = station[i].Site.Altitude,
                                            MapArea = data.MapData.Area,
                                            BuildingContent = data.MapData.BuildingContent,
                                            ClutterContent = data.MapData.ClutterContent,
                                            ReliefContent = data.MapData.ReliefContent,
                                            Transmitter = station[i].Transmitter,
                                            CluttersDesc = data.CluttersDesc
                                        };

                                        var stationCorellationCalcData = new StationCorellationCalcData()
                                        {
                                            GSIDGroupeStation = station[i],
                                            CorellationParameters = data.CorellationParameters,
                                            GSIDGroupeDriveTests = driveTest[b],
                                            FieldStrengthCalcData = fieldStrengthCalcData,
                                            GeneralParameters = data.GeneralParameters
                                        };

                                        var stationCalibrationCalcData = new StationCalibrationCalcData()
                                        {
                                            CalibrationParameters = data.CalibrationParameters,
                                            GSIDGroupeStation = station[i],
                                            CorellationParameters = data.CorellationParameters,
                                            GSIDGroupeDriveTests = driveTest[b],
                                            FieldStrengthCalcData = fieldStrengthCalcData,
                                            GeneralParameters = data.GeneralParameters
                                        };

                                        var iterationCorellationCalc = _iterationsPool.GetIteration<StationCorellationCalcData, ResultCorrelationGSIDGroupeStations>();
                                        var resultCorellationCalcData = iterationCorellationCalc.Run(taskContext, stationCorellationCalcData);

                                        var iterationCalibrationCalc = _iterationsPool.GetIteration<StationCalibrationCalcData, ResultCorrelationGSIDGroupeStations>();
                                        var resultCalibrationCalcData = iterationCalibrationCalc.Run(taskContext, stationCalibrationCalcData);
                                    }
                                }
                            }
                        }
                    }
                }

                
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (calcPointArrayBuffer != null)
                {
                    _calcPointArrayPool.Put(calcPointArrayBuffer);
                }
            }
            return calcCorellationResult;
        }
    }
}
