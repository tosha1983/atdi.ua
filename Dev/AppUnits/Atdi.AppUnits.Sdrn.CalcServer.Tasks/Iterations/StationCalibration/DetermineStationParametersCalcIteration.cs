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
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Contracts.Sdrn.DeepServices.Gis;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// 
    /// </summary>
    public class DetermineStationParametersCalcIteration : IIterationHandler<AllStationCorellationCalcData, CalibrationResult[]>
    {
        private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly IObjectPool<CalcPoint[]> _calcPointArrayPool;
        private readonly IObjectPoolSite _poolSite;
        private readonly ITransformation _transformation;
        private readonly AppServerComponentConfig _appServerComponentConfig;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public DetermineStationParametersCalcIteration(
            IIterationsPool iterationsPool,
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            ITransformation transformation,
            ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _poolSite = poolSite;
            _appServerComponentConfig = appServerComponentConfig;
            _transformation = transformation;
            _calcPointArrayPool = _poolSite.GetPool<CalcPoint[]>(ObjectPools.StationCalibrationCalcPointArrayObjectPool);
            _logger = logger;
        }

        public CalibrationResult[] Run(ITaskContext taskContext, AllStationCorellationCalcData data)
        {
            var calcPointArrayBuffer = default(CalcPoint[]);

            // создаем список для хранения результатов обработки
            var listCalcCorellationResult = new List<CalibrationResult>();
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
                // список для хранения наименований всех стандартов
                var allStandards = new List<string>();
                // получаем набор стандартов из станций
                allStandards.AddRange(Utils.GetUniqueArrayStandardsfromStations(data.GSIDGroupeStation));
                // получаем набор стандартов из драйв тестов
                allStandards.AddRange(Utils.GetUniqueArrayStandardsFromDriveTests(data.GSIDGroupeDriveTests));
                // создаем список неповторяющихся значений стандартов
                var arrStandards = allStandards.Distinct().ToArray();

                // цикл по стандартам
                for (int v = 0; v < arrStandards.Length; v++)
                {
                    // получить очередное значение стандарта
                    var standard = arrStandards[v];

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    ///                     БЛОК
                    ///          "Прямые соответсвия GSID (основная масса)"
                    /// 
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // вызов метода сравнения станций и драйв тестов (с использованием функции CompareGSID)
                    // результатом выполнения данной функции есть три сущности: массив связей между набором станций и драйв тестами (сгруппирован по GSID), а также массив драйв тестов и массив станций (каждый из которых сгруппирован по GSID), которые не удалось связать через функцию CompareGSID
                    var linkDriveTestAndStations = Utils.CompareDriveTestAndStation(data.GSIDGroupeDriveTests, data.GSIDGroupeStation, standard, out DriveTestsResult[][] outDriveTestsResults, out ContextStation[][] outContextStations);

                    // преобразуем в список массив драйв тестов, для которого не найдены соотвествия со станциями (данный список будет пополняться при дальнейшей работе алгоритма)
                    var outListDriveTestsResults = outDriveTestsResults.ToList();

                    // преобразуем в список массив станций , для которого не найдены соотвествия с драйв тестами (данный список будет пополняться при дальнейшей работе алгоритма)
                    var outListContextStations = outContextStations.ToList();

                    // создаем список для хранения результатов обработки по отдельно взятому стандарту и заданой группе GSID
                    var calibrationStationsAndDriveTestsResultByGroup = new List<CalibrationStationsAndDriveTestsResult>();

                    /// обработка по отдельно взятой группе (GSID) пролинкованных массивов станций и драйв тестов
                    for (int z = 0; z < linkDriveTestAndStations.Length; z++)
                    {
                        // получаем массив станций одной группы (связанных с драйв тестами)
                        var station = linkDriveTestAndStations[z].ContextStation;
                        // получаем массив драйв тестов одной группы (связанных со станциями)
                        var driveTest = linkDriveTestAndStations[z].DriveTestsResults;

                        

                        ///  4.2.2. Расчет корреляции weake (схема бл 2)
                        bool StatusCorellationLinkGroup = false;
                        for (int j = 0; j < driveTest.Length; j++)
                        {
                            StatusCorellationLinkGroup = CalcCorellation(taskContext, station, driveTest[j], data);
                            if (StatusCorellationLinkGroup)
                            {
                                break;
                            }
                        }
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        ///
                        /// 
                        /// Если связанные GSIDGroupeStations и  GSIDGroupeDriveTests в блоке 2(Расчет корреляции weake) получили False то они также 
                        /// изымаються из дальнейшего расмотрения в группе блоков
                        /// «Прямые соответсвия GSID(основная масса)» и переноситься в: -массив(или список) массивов станций и массив(или список) 
                        /// массивов драйв тестов для последующего анализа в группе блоков «НДП, изменение GSID, новые РЕЗ»
                        /// 
                        ///////////////////////////////////////////////////////////////////////////////////////////
                        if (StatusCorellationLinkGroup == false)
                        {
                            outListContextStations.Add(station);
                            outListDriveTestsResults.Add(driveTest);
                        }
                        else
                        {
                            // создаем список для хранения результатов обработки по отдельно взятому стандарту и заданой группе GSID
                            var tempResultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();

                            //  4.2.3. Подбор параметров станции по результатам Drive Test (hard)
                            for (int j = 0; j < driveTest.Length; j++)
                            {
                                var сalibrationStationsGSIDGroupeStations = CalibrationStations(taskContext, station, driveTest[j], data);
                                // если результат выполнения метода CalibrationStations не пустой, тогда добавляем его в список tempResultCorrelationGSIDGroupeStations
                                if (сalibrationStationsGSIDGroupeStations.Count > 0)
                                {
                                    tempResultCorrelationGSIDGroupeStations.AddRange(сalibrationStationsGSIDGroupeStations);
                                }
                            }

                            //4.2.4. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов (схема бл 4)
                            var calibrationStationsAndDriveTestsResult = FillCalibrationStationResultFirstBlock(tempResultCorrelationGSIDGroupeStations);

                            if ((calibrationStationsAndDriveTestsResult.ResultCalibrationStation.Length > 0) || (calibrationStationsAndDriveTestsResult.ResultCalibrationDriveTest.Length > 0))
                            {
                                // добавляем промежуточный результат в массив результатов
                                calibrationStationsAndDriveTestsResultByGroup.Add(calibrationStationsAndDriveTestsResult);

                                // убираем из общего списка станций и  драйв тестов  те которые попали в результаты
                                CompressedStationsAndDriveTest(ref outListContextStations, ref outListDriveTestsResults, tempResultCorrelationGSIDGroupeStations);
                            }
                        }
                    }


                   

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    ///                     БЛОК
                    ///         "НДП, изменение GSID, новые РЕЗ"
                    /// 
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    /// 
                    ///     4.2.5. Ранжирование драйв тестов с GSID (по количеству данных и их уровню)
                    ///     
                    ///     Внутри GSIDGroupeDriveTests данные уже ранжированы в данном блоки необходимо ранжировать сам массив (или список) массивов драйв тестов. 
                    ///     Ранжируется на основании количества точек первого DriveTests из GSIDGroupeDriveTests с большим количеством точек идут первые. 
                    ///     При равном количестве точек первым будет идти тот у кого средний уровень по всем точкам выше. 
                    ///     На входи и выходе одинаковое количество элементов у массивов.Ничего не удаляется и не переноситься.
                    ///     
                    /// 
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////

                    for (int i = 0; i < outListDriveTestsResults.Count; i++)
                    {
                        var drvTests = outListDriveTestsResults[i];

                        var driveTestsResults = new DriveTestsResult[drvTests.Length];
                        var keyValueDriveTests = new Dictionary<int, float>();

                        var orderByCountPoints = from z in drvTests orderby z.CountPoints descending select z;
                        var tempDriveTestsByOneGroup = orderByCountPoints.ToArray();
                        for (int j = 0; j < tempDriveTestsByOneGroup.Length; j++)
                        {
                            keyValueDriveTests.Add(tempDriveTestsByOneGroup[j].Num, tempDriveTestsByOneGroup[j].Points.Select(x => x.Level_dBm).Sum());
                        }
                        var orderDriveTests = from z in keyValueDriveTests.ToList() orderby z.Value descending select z;
                        var tempOrderDriveTests = orderDriveTests.ToArray();
                        for (int s = 0; s < tempOrderDriveTests.Length; s++)
                        {
                            var fndDriveTest = tempDriveTestsByOneGroup.First(x => x.Num == tempOrderDriveTests[s].Key);
                            driveTestsResults[s] = fndDriveTest;
                        }

                        outListDriveTestsResults[i] = driveTestsResults;
                    }




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    ///     4.2.6. Ранжирование Stations под данный Drive Test
                    ///     
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////

                    for (int i = 0; i < outListDriveTestsResults.Count; i++)
                    {
                        var arrDriveTests = outListDriveTestsResults[i];

                        for (int w = 0; w < arrDriveTests.Length; w++)
                        {
                           
                            //получаем очередной драйв тест
                            var currentDriveTest = arrDriveTests[w];

                            var coordinatesDrivePoint = currentDriveTest.Points.Select(x => x.Coordinate).ToArray();

                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            ///
                            ///     1. Для GSIDGroupeDriveTests мы считаем центр масс всех координат, всех точек по заданному драйв тесту. 
                            ///     
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            var centerWeightCoordinateOfDriveTest = Utils.CenterWeightAllCoordinates(currentDriveTest.Points);


                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            ///
                            ///    2. Формируем новый массив (или список) массивов станций (GSIDGroupeStations) на основании того перемещаем туда все GSIDGroupeStations
                            ///     из массива (или списка) массивов станций (GSIDGroupeStations) если хотябы одна из станций  GSIDGroupeStations 
                            ///     имеют координаты ближе чем 1км (параметр вынести в файл конфигурации в зависимости от STANDART) к координатам GSIDGroupeDriveTests.
                            ///     
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            var GSIDGroupeStations = new List<ContextStation[]>();

                            for (int j = 0; j < outListContextStations.Count; j++)
                            {
                                var arrStations = outListContextStations[j];
                                for (int z = 0; z < arrStations.Length; z++)
                                {
                                    for (int p = 0; p < coordinatesDrivePoint.Length; p++)
                                    {
                                        var coordinateStation = _transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = arrStations[z].Site.Longitude, Latitude = arrStations[z].Site.Latitude }, _transformation.ConvertProjectionToCode(data.Projection));
                                        if (GeometricСalculations.GetDistance_km(coordinatesDrivePoint[p].X, coordinatesDrivePoint[p].Y, coordinateStation.X, coordinateStation.Y) <= _appServerComponentConfig.MinDistanceBetweenDriveTestAndStation_GSM)
                                        {
                                            // добавляем весь массив станций arrStations в случае если одна из станций, которая входит в arrStations имеет расстояние до одной из точек текущего DrivePoint меньше 1 км (берем с конфигурации)
                                            GSIDGroupeStations.Add(arrStations);
                                            break;
                                        }
                                    }
                                }
                            }

                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            ///
                            ///    3. В новый массив (или список) массивов станций (GSIDGroupeStations) проводим ранжирование 
                            ///    GSIDGroupeStations по критерию минимальное расстояние от центра масс GSIDGroupeDriveTests 
                            ///     
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            for (int q = 0; q < GSIDGroupeStations.Count; q++)
                            {
                                var arrStations = GSIDGroupeStations[q];
                                var stationsResults = new ContextStation[arrStations.Length];
                                var keyValueStations = new Dictionary<long, double>();
                                for (int z = 0; z < arrStations.Length; z++)
                                {
                                    var coordinateStation = _transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = arrStations[z].Site.Longitude, Latitude = arrStations[z].Site.Latitude }, _transformation.ConvertProjectionToCode(data.Projection));
                                    var distance = GeometricСalculations.GetDistance_km(centerWeightCoordinateOfDriveTest.X, centerWeightCoordinateOfDriveTest.Y, coordinateStation.X, coordinateStation.Y);
                                    keyValueStations.Add(arrStations[z].Id, distance);
                                }


                                var orderStations = from z in keyValueStations.ToList() orderby z.Value ascending select z;
                                var tempOrderStations = orderStations.ToArray();
                                for (int s = 0; s < tempOrderStations.Length; s++)
                                {
                                    var fndStations = arrStations.First(x => x.Id == tempOrderStations[s].Key);
                                    stationsResults[s] = fndStations;
                                }
                                GSIDGroupeStations[q] = stationsResults;
                            }

                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            ///
                            ///    4.2.7 Подбор параметров станции по результатам Drive Test (hard) (схема бл 7)
                            ///    
                            ///     
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////

                            for (int m = 0; m < GSIDGroupeStations.Count; m++)
                            {
                                var tempResultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();

                                // проводим анализ по каждой отдельно взятой группе станций отдельно
                                var station = GSIDGroupeStations[m];

                                // расчет корелляции
                                bool statusCorellationLinkGroup = CalcCorellation(taskContext, station, currentDriveTest, data);
                                if (statusCorellationLinkGroup == true)
                                {
                                    var сalibrationStationsGSIDGroupeStations = CalibrationStations(taskContext, station, currentDriveTest, data);
                                    // если результат выполнения метода CalibrationStations не пустой, тогда добавляем его в список tempResultCorrelationGSIDGroupeStations
                                    if (сalibrationStationsGSIDGroupeStations.Count > 0)
                                    {
                                        tempResultCorrelationGSIDGroupeStations.AddRange(сalibrationStationsGSIDGroupeStations);
                                    }
                                }


                                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                ///    4.2.8. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов (схема бл 8)
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                // Производим последовательную обработку массива (ResultCorrelationGSIDGroupeStations) запись за записью.
                                if (tempResultCorrelationGSIDGroupeStations.Count > 0)
                                {
                                    //4.2.4. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов (схема бл 4)
                                    var calibrationStationsAndDriveTestsResult = FillCalibrationStationResultSecondBlock(tempResultCorrelationGSIDGroupeStations);

                                    if ((calibrationStationsAndDriveTestsResult.ResultCalibrationStation.Length > 0) || (calibrationStationsAndDriveTestsResult.ResultCalibrationDriveTest.Length > 0))
                                    {
                                        // добавляем промежуточный результат в массив результатов
                                        calibrationStationsAndDriveTestsResultByGroup.Add(calibrationStationsAndDriveTestsResult);

                                        // убираем из общего списка станций и  драйв тестов  те которые попали в результаты
                                        CompressedStationsAndDriveTest(ref outListContextStations, ref outListDriveTestsResults, tempResultCorrelationGSIDGroupeStations);
                                    }
                                }
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                ///   привязки нет, но блок 2 хоть раз проходили с положительным результатом
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                else if ((tempResultCorrelationGSIDGroupeStations.Count == 0) && (statusCorellationLinkGroup == true))
                                {
                                    var lstCalibrationStationResult = new List<CalibrationStationResult>();
                                    for (int d = 0; d < station.Length; d++)
                                    {
                                        lstCalibrationStationResult.Add(new CalibrationStationResult()
                                        {
                                            ExternalSource = station[d].ExternalSource,
                                            ExternalCode = station[d].ExternalCode,
                                            LicenseGsid = station[d].LicenseGsid,
                                            RealGsid = station[d].RealGsid,
                                            ResultStationStatus = StationStatusResult.UN
                                            //MaxCorellation = ??????????????????????????
                                            //ParametersStationNew=  ??????????????????????????
                                            //ParametersStationOld ??????????????????????????
                                        });
                                    }
                                    // формируем результат с массивом драйв тестов CalibrationDriveTestResult[] = null
                                    calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                                    {
                                        ResultCalibrationDriveTest = null, // ???????????????????? (здесь можно передавать null?)
                                        ResultCalibrationStation = lstCalibrationStationResult.ToArray()
                                    });

                                    // убираем из общего списка станций  те которые попали в результаты
                                    CompressedStations(ref outListContextStations, station);
                                }
                            }


                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                            ///
                            ///   4.2.10. Оценка Drive Test на предмет НДП (схема бл. 9)
                            ///   
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////

                            /// обработка станций со статусом "P" ??????????
                            var arrayStationsInStatusP = new List<ContextStation[]>();
                            for (int j = 0; j < GSIDGroupeStations.Count; j++)
                            {
                                var arrStations = GSIDGroupeStations[j].ToList();
                                var temp = arrStations.FindAll(x => x.Type == ClientContextStationType.P);
                                if (temp != null)
                                {
                                    arrayStationsInStatusP.Add(temp.ToArray());
                                }
                            }

                            for (int m = 0; m < arrayStationsInStatusP.Count; m++)
                            {
                                var tempResultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();

                                // проводим анализ по каждой отдельно взятой группе станций отдельно
                                var station = arrayStationsInStatusP[m];

                                // расчет корелляции
                                bool statusCorellationLinkGroup = CalcCorellation(taskContext, station, currentDriveTest, data);
                                if (statusCorellationLinkGroup == true)
                                {
                                    var сalibrationStationsGSIDGroupeStations = CalibrationStations(taskContext, station, currentDriveTest, data);
                                    // если результат выполнения метода CalibrationStations не пустой, тогда добавляем его в список tempResultCorrelationGSIDGroupeStations
                                    if (сalibrationStationsGSIDGroupeStations.Count > 0)
                                    {
                                        tempResultCorrelationGSIDGroupeStations.AddRange(сalibrationStationsGSIDGroupeStations);
                                    }
                                }

                                // Производим последовательную обработку массива (ResultCorrelationGSIDGroupeStations) запись за записью.
                                if (tempResultCorrelationGSIDGroupeStations.Count > 0)
                                {
                                    // Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов 
                                    var calibrationStationsAndDriveTestsResult = FillCalibrationStationResultThirdBlock(tempResultCorrelationGSIDGroupeStations);

                                    if ((calibrationStationsAndDriveTestsResult.ResultCalibrationStation.Length > 0) || (calibrationStationsAndDriveTestsResult.ResultCalibrationDriveTest.Length > 0))
                                    {
                                        // добавляем промежуточный результат в массив результатов
                                        calibrationStationsAndDriveTestsResultByGroup.Add(calibrationStationsAndDriveTestsResult);

                                        // убираем из общего списка станций и  драйв тестов  те которые попали в результаты
                                        CompressedStationsAndDriveTest(ref outListContextStations, ref outListDriveTestsResults, tempResultCorrelationGSIDGroupeStations);
                                    }
                                }
                                else if ((tempResultCorrelationGSIDGroupeStations.Count == 0) && (statusCorellationLinkGroup == true))
                                {
                                    var lstCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
                                    var lstCalibrationStationResult = new List<CalibrationStationResult>();
                                    for (int d = 0; d < station.Length; d++)
                                    {
                                        lstCalibrationStationResult.Add(new CalibrationStationResult()
                                        {
                                            ExternalSource = station[d].ExternalSource,
                                            ExternalCode = station[d].ExternalCode,
                                            LicenseGsid = station[d].LicenseGsid,
                                            RealGsid = station[d].RealGsid,
                                            ResultStationStatus = StationStatusResult.UN
                                            //MaxCorellation = ??????????????????????????
                                            //ParametersStationNew=  ??????????????????????????
                                            //ParametersStationOld ??????????????????????????
                                        });
                                    }

                                    // формируем результат с массивом драйв тестов CalibrationDriveTestResult[] = null
                                    calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                                    {
                                        ResultCalibrationDriveTest = new CalibrationDriveTestResult[1]
                                        {
                                                 new CalibrationDriveTestResult()
                                                 {
                                                    Gsid = currentDriveTest.GSID,
                                                    CountPointsInDriveTest = currentDriveTest.Points.Length,
                                                    ResultDriveTestStatus = DriveTestStatusResult.IT
                                                    //GsidFromStation
                                                    //MaxPercentCorellation
                                                 }
                                        },
                                        ResultCalibrationStation = lstCalibrationStationResult.ToArray()
                                    });

                                    // убираем из общего списка станций  те которые попали в результаты
                                    CompressedStations(ref outListContextStations, station);
                                }
                            }
                        }
                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    ///   4.2.11. Все не изъятые Stations помечаются как необнаруженные (схема бл. 10)
                    ///   
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////

                    for (int j = 0; j < outListContextStations.Count; j++)
                    {
                        var arrStations = outListContextStations[j];
                        var lstCalibrationStationResult = new List<CalibrationStationResult>();
                        for (int d = 0; d < arrStations.Length; d++)
                        {
                            lstCalibrationStationResult.Add(new CalibrationStationResult()
                            {
                                ExternalSource = arrStations[d].ExternalSource,
                                ExternalCode = arrStations[d].ExternalCode,
                                LicenseGsid = arrStations[d].LicenseGsid,
                                RealGsid = arrStations[d].RealGsid,
                                ResultStationStatus = StationStatusResult.NF
                                //MaxCorellation = ??????????????????????????
                                //ParametersStationNew=  ??????????????????????????
                                //ParametersStationOld ??????????????????????????
                            });
                        }
                        // формируем результат с массивом драйв тестов CalibrationDriveTestResult[] = null
                        calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                        {
                            ResultCalibrationDriveTest = null, // ???????????????????? (здесь можно передавать null?)
                            ResultCalibrationStation = lstCalibrationStationResult.ToArray()
                        });
                    }


                    // до этого момента станции прошли весь алгоритм обработки и если остаются какие-то, то для них устанавиливаем статус NF в результате и очищаем соотвественно список outListContextStations
                    outListContextStations.Clear();


                    // подготовка общего результата

                    var listCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
                    var listCalibrationStationResult = new List<CalibrationStationResult>();
                    for (int m = 0; m < calibrationStationsAndDriveTestsResultByGroup.Count; m++)
                    {
                        var calibrationStationsAndDriveTest = calibrationStationsAndDriveTestsResultByGroup[m];
                        var driveTests = calibrationStationsAndDriveTest.ResultCalibrationDriveTest;
                        var stations = calibrationStationsAndDriveTest.ResultCalibrationStation;
                        for (int z = 0; z < driveTests.Length; z++)
                        {
                            if (driveTests[z] != null)
                            {
                                listCalibrationDriveTestResult.Add(driveTests[z]);
                            }
                        }
                        for (int z = 0; z < stations.Length; z++)
                        {
                            if (stations[z] != null)
                            {
                                listCalibrationStationResult.Add(stations[z]);
                            }
                        }

                    }

                     

                    var calcCorellationResult = new CalibrationResult()
                    {
                        // заполнить все поля
                        Standard = standard,
                        ResultCalibrationStation = listCalibrationStationResult.ToArray(),
                        ResultCalibrationDriveTest = listCalibrationDriveTestResult.ToArray(),
                        CountStation_CS = listCalibrationStationResult.Select(x => x.ResultStationStatus == StationStatusResult.CS).Count(),
                        CountStation_IT = listCalibrationStationResult.Select(x => x.ResultStationStatus == StationStatusResult.IT).Count(),
                        CountStation_NF = listCalibrationStationResult.Select(x => x.ResultStationStatus == StationStatusResult.NF).Count(),
                        CountStation_NS = listCalibrationStationResult.Select(x => x.ResultStationStatus == StationStatusResult.NS).Count(),
                        CountStation_UN = listCalibrationStationResult.Select(x => x.ResultStationStatus == StationStatusResult.UN).Count(),
                        //AreaName= ???????????????????
                        //CountMeasGSID=???????????????????
                        //CountMeasGSID_IT=???????????????????
                        //CountMeasGSID_LS=???????????????????
                        //GeneralParameters=???????????????????
                        //NumberStation= ???????????????????
                        //NumberStationInContour=???????????????????
                        //TimeStart=???????????????????
                    };
                    listCalcCorellationResult.Add(calcCorellationResult);
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
            return listCalcCorellationResult.ToArray();
        }


        /// <summary>
        /// Расчет корреляции weake (схема бл 2)
        /// </summary>
        /// <param name="taskContext"></param>
        /// <param name="stations"></param>
        /// <param name="driveTest"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CalcCorellation(ITaskContext taskContext, ContextStation[] stations, DriveTestsResult driveTest, AllStationCorellationCalcData data)
        {
            bool StatusCorellationLinkGroup = false;
            for (int i = 0; i < stations.Length; i++)
            {
                var fieldStrengthCalcData = new FieldStrengthCalcData
                {
                    Antenna = stations[i].Antenna,
                    PropagationModel = data.PropagationModel,
                    PointCoordinate = stations[i].Coordinate,
                    PointAltitude_m = stations[i].Site.Altitude,
                    MapArea = data.MapData.Area,
                    BuildingContent = data.MapData.BuildingContent,
                    ClutterContent = data.MapData.ClutterContent,
                    ReliefContent = data.MapData.ReliefContent,
                    Transmitter = stations[i].Transmitter,
                    CluttersDesc = data.CluttersDesc
                };

                var stationCorellationCalcData = new StationCorellationCalcData()
                {
                    GSIDGroupeStation = stations[i],
                    CorellationParameters = data.CorellationParameters,
                    GSIDGroupeDriveTests = driveTest,
                    FieldStrengthCalcData = fieldStrengthCalcData,
                    GeneralParameters = data.GeneralParameters
                };


                var iterationCorellationCalc = _iterationsPool.GetIteration<StationCorellationCalcData, ResultCorrelationGSIDGroupeStations>();
                var resultCorellationCalcData = iterationCorellationCalc.Run(taskContext, stationCorellationCalcData);

                // Если максимальная корреляция превысит(или равна) Сorrelation threshold weak то возвращаем True
                if (resultCorellationCalcData.Corellation_factor > stationCorellationCalcData.GeneralParameters.СorrelationThresholdWeak)
                {
                    StatusCorellationLinkGroup = true;
                    break;
                }
            }
            return StatusCorellationLinkGroup;
        }

        /// <summary>
        /// Подбор параметров станции по результатам Drive Test (hard)
        /// </summary>
        /// <param name="taskContext"></param>
        /// <param name="stations"></param>
        /// <param name="driveTest"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<ResultCorrelationGSIDGroupeStations> CalibrationStations(ITaskContext taskContext, ContextStation[] stations, DriveTestsResult driveTest, AllStationCorellationCalcData data)
        {
            var resultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();

            for (int i = 0; i < stations.Length; i++)
            {
                var fieldStrengthCalcData = new FieldStrengthCalcData
                {
                    Antenna = stations[i].Antenna,
                    PropagationModel = data.PropagationModel,
                    PointCoordinate = stations[i].Coordinate,
                    PointAltitude_m = stations[i].Site.Altitude,
                    MapArea = data.MapData.Area,
                    BuildingContent = data.MapData.BuildingContent,
                    ClutterContent = data.MapData.ClutterContent,
                    ReliefContent = data.MapData.ReliefContent,
                    Transmitter = stations[i].Transmitter,
                    CluttersDesc = data.CluttersDesc
                };


                var stationCalibrationCalcData = new StationCalibrationCalcData()
                {
                    CalibrationParameters = data.CalibrationParameters,
                    GSIDGroupeStation = stations[i],
                    CorellationParameters = data.CorellationParameters,
                    GSIDGroupeDriveTests = driveTest,
                    FieldStrengthCalcData = fieldStrengthCalcData,
                    GeneralParameters = data.GeneralParameters
                };

                var iterationCalibrationCalc = _iterationsPool.GetIteration<StationCalibrationCalcData, ResultCorrelationGSIDGroupeStations>();

                var resultCalibrationCalcData = iterationCalibrationCalc.Run(taskContext, stationCalibrationCalcData);

                resultCorrelationGSIDGroupeStations.Add(resultCalibrationCalcData);
            }
            return resultCorrelationGSIDGroupeStations;
        }

        /// <summary>
        /// Заполнение результата для случая описанного в пункте 4.2.4. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов
        /// </summary>
        /// <param name="resultsCorrelationGSIDGroupeStations"></param>
        /// <returns></returns>
        public CalibrationStationsAndDriveTestsResult FillCalibrationStationResultFirstBlock(List<ResultCorrelationGSIDGroupeStations> resultsCorrelationGSIDGroupeStations)
        {
            var listCalibrationStationResults = new List<CalibrationStationResult>();
            var listCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
            for (int g = 0; g < resultsCorrelationGSIDGroupeStations.Count; g++)
            {
                // извлекаем очередной результат, полученный в методе CalibrationStations
                var resGSIDGroupeStations = resultsCorrelationGSIDGroupeStations[g];

                if ((resGSIDGroupeStations.DriveTestsResult != null) && (resGSIDGroupeStations.ClientContextStation != null))
                {
                    // статусы ResultStationStatus и ResultDriveTestStatus определяем из RulesStatusParameters
                    // здесь нужно корректно заполнить параметры ниже
                    bool? corellation = null;//??????????????????????????
                    bool? trustOldResults = null;//??????????????????????????
                    bool? pointForCorrelation = null;//??????????????????????????
                    bool? exceptionParameter = null;//??????????????????????????
                    var status = resGSIDGroupeStations.ClientContextStation.Type;
                    var calibrationStatusParameters = RulesStatusParameters.CalibrationFirstStatusParameters.Find(c => c.Corellation == corellation && c.StatusStation == status && c.TrustOldResults == trustOldResults && c.PointForCorrelation == pointForCorrelation && c.ExceptionParameter == exceptionParameter);
                    if (calibrationStatusParameters != null)
                    {
                        var calibrationStationResult = new CalibrationStationResult()
                        {
                            ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                            ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                            LicenseGsid = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                            RealGsid = resGSIDGroupeStations.ClientContextStation.RealGsid,
                            //calibrationStationResult.MaxCorellation = ??????????????????????????
                            ParametersStationNew = resGSIDGroupeStations.ParametersStationNew,
                            ParametersStationOld = resGSIDGroupeStations.ParametersStationOld,
                            ResultStationStatus = calibrationStatusParameters.ResultStationStatus,
                        };
                        listCalibrationStationResults.Add(calibrationStationResult);

                        var calibrationDriveTestResult = new CalibrationDriveTestResult()
                        {
                            ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                            ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                            Gsid = resGSIDGroupeStations.DriveTestsResult.GSID,
                            GsidFromStation = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                            //MaxPercentCorellation = ??????????????????????????
                            CountPointsInDriveTest = resGSIDGroupeStations.DriveTestsResult.Points.Length,
                            ResultDriveTestStatus = calibrationStatusParameters.ResultDriveTestStatus
                        };
                        listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                    }
                }
                // содержит только драйв тест
                else if ((resGSIDGroupeStations.DriveTestsResult != null) && (resGSIDGroupeStations.ClientContextStation == null))
                {
                    var calibrationDriveTestResult = new CalibrationDriveTestResult()
                    {
                        ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                        ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                        Gsid = resGSIDGroupeStations.DriveTestsResult.GSID,
                        GsidFromStation = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                        //MaxPercentCorellation = ??????????????????????????
                        CountPointsInDriveTest = resGSIDGroupeStations.DriveTestsResult.Points.Length,
                        ResultDriveTestStatus = DriveTestStatusResult.IT
                    };
                    listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                }
                // содержит только станцию
                else if ((resGSIDGroupeStations.DriveTestsResult == null) && (resGSIDGroupeStations.ClientContextStation != null))
                {
                    var calibrationStationResult = new CalibrationStationResult()
                    {
                        ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                        ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                        LicenseGsid = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                        RealGsid = resGSIDGroupeStations.ClientContextStation.RealGsid,
                        //calibrationStationResult.MaxCorellation = ??????????????????????????
                        ParametersStationNew = resGSIDGroupeStations.ParametersStationNew,
                        ParametersStationOld = resGSIDGroupeStations.ParametersStationOld,
                        ResultStationStatus = StationStatusResult.NF
                    };
                    listCalibrationStationResults.Add(calibrationStationResult);
                }
            }
            return new CalibrationStationsAndDriveTestsResult()
            {
                ResultCalibrationDriveTest = listCalibrationDriveTestResult.ToArray(),
                ResultCalibrationStation = listCalibrationStationResults.ToArray()
            };
        }

        /// <summary>
        /// Заполнение результата для случая описанного в пункте 4.2.7 (Подбор параметров станции по результатам Drive Test)
        /// </summary>
        /// <param name="resultsCorrelationGSIDGroupeStations"></param>
        /// <returns></returns>
        public CalibrationStationsAndDriveTestsResult FillCalibrationStationResultSecondBlock(List<ResultCorrelationGSIDGroupeStations> resultsCorrelationGSIDGroupeStations)
        {
            var listCalibrationStationResults = new List<CalibrationStationResult>();
            var listCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
            for (int g = 0; g < resultsCorrelationGSIDGroupeStations.Count; g++)
            {
                // извлекаем очередной результат, полученный в методе CalibrationStations
                var resGSIDGroupeStations = resultsCorrelationGSIDGroupeStations[g];

                if ((resGSIDGroupeStations.DriveTestsResult != null) && (resGSIDGroupeStations.ClientContextStation != null))
                {
                    // статусы ResultStationStatus и ResultDriveTestStatus определяем из RulesStatusParameters
                    // здесь нужно корректно заполнить параметры ниже
                    bool? pointForCorrelation = null;//??????????????????????????
                    bool? exceptionParameter = null;//??????????????????????????
                    var status = resGSIDGroupeStations.ClientContextStation.Type;
                    var calibrationStatusParameters = RulesStatusParameters.CalibrationSecondStatusParameter.Find(c => c.StatusStation == status && c.PointForCorrelation == pointForCorrelation && c.ExceptionParameter == exceptionParameter);
                    if (calibrationStatusParameters != null)
                    {
                        var calibrationStationResult = new CalibrationStationResult()
                        {
                            ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                            ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                            LicenseGsid = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                            RealGsid = resGSIDGroupeStations.ClientContextStation.RealGsid,
                            //calibrationStationResult.MaxCorellation = ??????????????????????????
                            ParametersStationNew = resGSIDGroupeStations.ParametersStationNew,
                            ParametersStationOld = resGSIDGroupeStations.ParametersStationOld,
                            ResultStationStatus = calibrationStatusParameters.ResultStationStatus,
                        };
                        listCalibrationStationResults.Add(calibrationStationResult);

                        var calibrationDriveTestResult = new CalibrationDriveTestResult()
                        {
                            ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                            ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                            Gsid = resGSIDGroupeStations.DriveTestsResult.GSID,
                            GsidFromStation = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                            //MaxPercentCorellation = ??????????????????????????
                            CountPointsInDriveTest = resGSIDGroupeStations.DriveTestsResult.Points.Length,
                            ResultDriveTestStatus = calibrationStatusParameters.ResultDriveTestStatus
                        };
                        listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                    }
                }
                // содержит только драйв тест
                else if ((resGSIDGroupeStations.DriveTestsResult != null) && (resGSIDGroupeStations.ClientContextStation == null))
                {
                    var calibrationDriveTestResult = new CalibrationDriveTestResult()
                    {
                        ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                        ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                        Gsid = resGSIDGroupeStations.DriveTestsResult.GSID,
                        GsidFromStation = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                        //MaxPercentCorellation = ??????????????????????????
                        CountPointsInDriveTest = resGSIDGroupeStations.DriveTestsResult.Points.Length,
                    };

                    bool? pointForCorrelation = null; // ??????????????????????????
                    if (pointForCorrelation.Value)
                    {
                        calibrationDriveTestResult.ResultDriveTestStatus = DriveTestStatusResult.IT;
                    }
                    else
                    {
                        calibrationDriveTestResult.ResultDriveTestStatus = DriveTestStatusResult.UN;
                    }

                    listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                }
                // содержит только станцию
                else if ((resGSIDGroupeStations.DriveTestsResult == null) && (resGSIDGroupeStations.ClientContextStation != null))
                {
                    var calibrationStationResult = new CalibrationStationResult()
                    {
                        ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                        ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                        LicenseGsid = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                        RealGsid = resGSIDGroupeStations.ClientContextStation.RealGsid,
                        //calibrationStationResult.MaxCorellation = ??????????????????????????
                        ParametersStationNew = resGSIDGroupeStations.ParametersStationNew,
                        ParametersStationOld = resGSIDGroupeStations.ParametersStationOld
                    };

                    bool? pointForCorrelation = null; //  ??????????????????????????
                    if (pointForCorrelation.Value)
                    {
                        calibrationStationResult.ResultStationStatus = StationStatusResult.NF;
                    }
                    else
                    {
                        calibrationStationResult.ResultStationStatus = StationStatusResult.UN;
                    }

                    listCalibrationStationResults.Add(calibrationStationResult);
                }
            }
            return new CalibrationStationsAndDriveTestsResult()
            {
                ResultCalibrationDriveTest = listCalibrationDriveTestResult.ToArray(),
                ResultCalibrationStation = listCalibrationStationResults.ToArray()
            };
        }

        /// <summary>
        /// Заполнение результата для случая, когда выполняется поиск станций со статусом P
        /// </summary>
        /// <param name="resultsCorrelationGSIDGroupeStations"></param>
        /// <returns></returns>
        public CalibrationStationsAndDriveTestsResult FillCalibrationStationResultThirdBlock(List<ResultCorrelationGSIDGroupeStations> resultsCorrelationGSIDGroupeStations)
        {
            var listCalibrationStationResults = new List<CalibrationStationResult>();
            var listCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
            for (int g = 0; g < resultsCorrelationGSIDGroupeStations.Count; g++)
            {
                // извлекаем очередной результат, полученный в методе CalibrationStations
                var resGSIDGroupeStations = resultsCorrelationGSIDGroupeStations[g];

                if ((resGSIDGroupeStations.DriveTestsResult != null) && (resGSIDGroupeStations.ClientContextStation != null))
                {
                    // статусы ResultStationStatus и ResultDriveTestStatus определяем из RulesStatusParameters
                    // здесь нужно корректно заполнить параметры ниже
                    bool? pointForCorrelation = null;//??????????????????????????

                    var calibrationStationResult = new CalibrationStationResult()
                    {
                        ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                        ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                        LicenseGsid = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                        RealGsid = resGSIDGroupeStations.ClientContextStation.RealGsid,
                        //calibrationStationResult.MaxCorellation = ??????????????????????????
                        ParametersStationNew = resGSIDGroupeStations.ParametersStationNew,
                        ParametersStationOld = resGSIDGroupeStations.ParametersStationOld
                    };
                    listCalibrationStationResults.Add(calibrationStationResult);

                    var calibrationDriveTestResult = new CalibrationDriveTestResult()
                    {
                        ExternalSource = resGSIDGroupeStations.ClientContextStation.ExternalSource,
                        ExternalCode = resGSIDGroupeStations.ClientContextStation.ExternalCode,
                        Gsid = resGSIDGroupeStations.DriveTestsResult.GSID,
                        GsidFromStation = resGSIDGroupeStations.ClientContextStation.LicenseGsid,
                        //MaxPercentCorellation = ??????????????????????????
                        CountPointsInDriveTest = resGSIDGroupeStations.DriveTestsResult.Points.Length,
                    };

                    if (pointForCorrelation.Value)
                    {
                        calibrationDriveTestResult.ResultDriveTestStatus = DriveTestStatusResult.LS;
                    }
                    else
                    {
                        calibrationDriveTestResult.ResultDriveTestStatus = DriveTestStatusResult.UN;
                    }

                    listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                }
                // содержит только драйв тест
                else if ((resGSIDGroupeStations.DriveTestsResult != null) && (resGSIDGroupeStations.ClientContextStation == null))
                {
                    // здесь нужна реализация алгоритма ???????????????????????
                }
                // содержит только станцию
                else if ((resGSIDGroupeStations.DriveTestsResult == null) && (resGSIDGroupeStations.ClientContextStation != null))
                {
                    // здесь нужна реализация алгоритма ???????????????????????
                }
            }
            return new CalibrationStationsAndDriveTestsResult()
            {
                ResultCalibrationDriveTest = listCalibrationDriveTestResult.ToArray(),
                ResultCalibrationStation = listCalibrationStationResults.ToArray()
            };
        }

        /// <summary>
        /// Удаление из общего перечня станций contextStations тех, для которых привязка была найдена
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="driveTests"></param>
        /// <param name="resultCorrelationGSIDGroupeStations"></param>
        public void CompressedStationsAndDriveTest(ref List<ContextStation[]> contextStations, ref List<DriveTestsResult[]> driveTests, List<ResultCorrelationGSIDGroupeStations> resultCorrelationGSIDGroupeStations)
        {
            // убираем из общего списка станций, те которые попали в результаты
            for (int f = 0; f < contextStations.Count; f++)
            {
                var arrStations = contextStations[f].ToList();
                for (int b = 0; b < resultCorrelationGSIDGroupeStations.Count; b++)
                {
                    arrStations.RemoveAll(x => x.Id == resultCorrelationGSIDGroupeStations[b].ClientContextStation.Id);
                    if (arrStations.Count > 0)
                    {
                        contextStations[f] = arrStations.ToArray();
                    }
                    else
                    {
                        contextStations.RemoveAt(f);
                    }
                }
            }

            // убираем из общего списка драйв тестов, те которые попали в результаты
            for (int f = 0; f < driveTests.Count; f++)
            {
                var arrDriveTests = driveTests[f].ToList();
                for (int b = 0; b < resultCorrelationGSIDGroupeStations.Count; b++)
                {
                    arrDriveTests.RemoveAll(x => x.Num == resultCorrelationGSIDGroupeStations[b].DriveTestsResult.Num);
                    if (arrDriveTests.Count > 0)
                    {
                        driveTests[f] = arrDriveTests.ToArray();
                    }
                    else
                    {
                        driveTests.RemoveAt(f);
                    }
                }
            }
        }

        /// <summary>
        /// Удаление из общего перечня станций contextStations тех, для которых привязка найдена не была
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="gsidGroupeStations"></param>
        public void CompressedStations(ref List<ContextStation[]> contextStations,  ContextStation[] gsidGroupeStations)
        {
            // убираем из общего списка станций, те которые попали в результаты
            for (int f = 0; f < contextStations.Count; f++)
            {
                var arrStations = contextStations[f].ToList();
                for (int b = 0; b < gsidGroupeStations.Length; b++)
                {
                    arrStations.RemoveAll(x => x.Id == gsidGroupeStations[b].Id);
                    if (arrStations.Count > 0)
                    {
                        contextStations[f] = arrStations.ToArray();
                    }
                    else
                    {
                        contextStations.RemoveAt(f);
                    }
                }
            }
        }
    }
}
