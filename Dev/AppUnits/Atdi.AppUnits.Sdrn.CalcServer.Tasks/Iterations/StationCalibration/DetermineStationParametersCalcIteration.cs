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
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.DataModels.DataConstraint;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// 
    /// </summary>
    public class DetermineStationParametersCalcIteration : IIterationHandler<AllStationCorellationCalcData, CalibrationResult[]>
    {
        private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly IObjectPool<PointFS[]> _calcPointArrayPool;
        private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
        private readonly IObjectPool<DriveTestsResult[][]> _calcListDriveTestsResultPool;
        private readonly IObjectPool<CalibrationResult[]> _calibrationResultPool;


        private IDataLayerScope _calcDbScope;
        private readonly IObjectPoolSite _poolSite;
        private readonly ITransformation _transformation;
        private readonly AppServerComponentConfig _appServerComponentConfig;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public DetermineStationParametersCalcIteration(
            IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
            IIterationsPool iterationsPool,
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            ITransformation transformation,
            ILogger logger)
        {
            _calcServerDataLayer = calcServerDataLayer;
            _iterationsPool = iterationsPool;
            _poolSite = poolSite;
            _appServerComponentConfig = appServerComponentConfig;
            _transformation = transformation;
            _calcPointArrayPool = _poolSite.GetPool<PointFS[]>(ObjectPools.StationCalibrationPointFSArrayObjectPool);
            _calcListDriveTestsResultPool = _poolSite.GetPool<DriveTestsResult[][]>(ObjectPools.StationCalibrationListDriveTestsResultObjectPool);
            _calibrationResultPool = _poolSite.GetPool<CalibrationResult[]>(ObjectPools.StationCalibrationResultObjectPool);
            _logger = logger;
        }

        /// <summary>
        /// Минимальное расстояние к drive point для заданного в файле конфигурации стандарта
        /// </summary>
        /// <param name="standard"></param>
        /// <returns></returns>
        public int GetMinDistanceFromConfigByStandard(string standard)
        {
            int? minDistance = null;
            if ((standard=="GSM") || (standard == "GSM-900") || (standard == "GSM-1800") || (standard == "E-GSM"))
            {
                minDistance = _appServerComponentConfig.MinDistanceBetweenDriveTestAndStation_GSM;
            }
            else if ((standard == "UMTS") || (standard == "WCDMA"))
            {
                minDistance = _appServerComponentConfig.MinDistanceBetweenDriveTestAndStation_UMTS;
            }
            else if ((standard == "LTE") || (standard == "LTE-1800") || (standard == "LTE-2600") || (standard == "LTE-900"))
            {
                minDistance = _appServerComponentConfig.MinDistanceBetweenDriveTestAndStation_LTE;
            }
            else if ((standard == "CDMA") || (standard == "CDMA-450") || (standard == "CDMA-800") || (standard == "EVDO"))
            {
                minDistance = _appServerComponentConfig.MinDistanceBetweenDriveTestAndStation_CDMA;
            }
            else
            {
                throw new Exception($"The parameter 'minimum distance' in the config is not defined for the '{standard}' standard");
            }
            return minDistance.Value;
        }

        public CalibrationResult[] Run(ITaskContext taskContext, AllStationCorellationCalcData data)
        {
            this._calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();

            if (data.GSIDGroupeDriveTests.Length==0)
            {
                throw new Exception("The count of drive tests is 0!");
            }
            if (data.GSIDGroupeStation.Length == 0)
            {
                throw new Exception("The count of stations is 0!");
            }

            // создаем список для хранения результатов обработки
            //var listCalcCorellationResult = new List<CalibrationResult>();

            try
            {
                int percentComplete = 0;
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
                long countRecordsListDriveTestsResultBuffer = 0;
                var listDriveTestsResultBuffer = default(DriveTestsResult[][]);
                try
                {
                    listDriveTestsResultBuffer = _calcListDriveTestsResultPool.Take();
                    Utils.CompareDriveTestsWithoutStandards(in data.GSIDGroupeDriveTests, listDriveTestsResultBuffer, out countRecordsListDriveTestsResultBuffer);
                    data.GSIDGroupeDriveTests = Utils.PrepareData(ref data, ref listDriveTestsResultBuffer, countRecordsListDriveTestsResultBuffer, this._calcPointArrayPool);
                }
                finally
                {
                    if (listDriveTestsResultBuffer != null)
                    {
                        _calcListDriveTestsResultPool.Put(listDriveTestsResultBuffer);
                    }
                }

                ///////////////////////////////////////////////////////////////////////////////////////////
                ///    
                ///     включение механизма перфорации DriveTests
                /// 
                ///////////////////////////////////////////////////////////////////////////////////////////
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

                var arrayStandardsfromStations = Utils.GetUniqueArrayStandardsfromStations(data.GSIDGroupeStation);
                if (arrayStandardsfromStations != null)
                {
                    allStandards.AddRange(arrayStandardsfromStations);
                }
                // получаем набор стандартов из драйв тестов
                //var arrayStandardsFromDriveTests = Utils.GetUniqueArrayStandardsFromDriveTests(data.GSIDGroupeDriveTests);
                //if (arrayStandardsFromDriveTests != null)
                //{
                //allStandards.AddRange(arrayStandardsFromDriveTests);
                //}

                if (allStandards.Count == 0)
                {
                    throw new Exception("The count of standards is 0!");
                }

                // создаем список неповторяющихся значений стандартов
                var arrStandards = allStandards.Distinct().ToArray();

                var selectDriveTestsByStandards = new List<DriveTestsResult>();
                if (data.GSIDGroupeDriveTests != null)
                {
                    for (int c = 0; c < arrStandards.Length; c++)
                    {
                        var listDriveTests = data.GSIDGroupeDriveTests.ToList();
                        var fndDrivetTests = listDriveTests.FindAll(x => x.Standard == arrStandards[c]);
                        if (fndDrivetTests != null)
                        {
                            selectDriveTestsByStandards.AddRange(fndDrivetTests);
                        }
                    }
                }
                data.GSIDGroupeDriveTests = selectDriveTestsByStandards.ToArray();


                var listCalcCorellationResult = new CalibrationResult[arrStandards.Length];
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
                    var linkDriveTestsAndStations = Utils.CompareDriveTestAndStation(data.GSIDGroupeDriveTests, data.GSIDGroupeStation, standard, out DriveTestsResult[][] outDriveTestsResults, out ContextStation[][] outContextStations, _transformation, 100, data.Projection);

                    // преобразуем в список массив драйв тестов, для которого не найдены соотвествия со станциями (данный список будет пополняться при дальнейшей работе алгоритма)

                    var outListDriveTestsResults = outDriveTestsResults.ToList();

                    // преобразуем в список массив станций , для которого не найдены соотвествия с драйв тестами (данный список будет пополняться при дальнейшей работе алгоритма)
                    var outListContextStations = outContextStations.ToList();

                    // список для храненения станций со статусом P для дальнейшей обработки
                    var outListContextStationsForStatusP = Utils.GroupStationsByStatus(data.GSIDGroupeStation, standard, _transformation, 100, data.Projection);

                    //дополнительная проверка перечня станций, которые будут обрабатываться в блоке НДП на наличие таких, у которых есть статус P
                    if ((outListContextStationsForStatusP != null) && (outListContextStationsForStatusP.Length > 0))
                    {
                        for (int z = 0; z < outListContextStations.Count; z++)
                        {
                            var listStations = outListContextStations[z].ToList();
                            for (int g = 0; g < listStations.Count; g++)
                            {
                                for (int h = 0; h < outListContextStationsForStatusP.Length; h++)
                                {
                                    var findStationInStatusP = outListContextStationsForStatusP[h].ToList();
                                    if (findStationInStatusP.Find(x => x.Id == listStations[g].Id) != null)
                                    {
                                        listStations.RemoveAt(g);
                                    }
                                }
                            }
                            outListContextStations[z] = listStations.ToArray();
                        }
                    }


                    // создаем список для хранения результатов обработки по отдельно взятому стандарту и заданой группе GSID
                    var calibrationStationsAndDriveTestsResultByGroup = new List<CalibrationStationsAndDriveTestsResult>();

                    /// обработка по отдельно взятой группе (GSID) пролинкованных массивов станций и драйв тестов
                    for (int z = 0; z < linkDriveTestsAndStations.Length; z++)
                    {
                        // получаем массив станций одной группы (связанных с драйв тестами)
                        var station = linkDriveTestsAndStations[z].ContextStation;

                        // получаем массив драйв тестов одной группы (связанных со станциями)
                        var driveTest = linkDriveTestsAndStations[z].DriveTestsResults;

                        ///  4.2.2. Расчет корреляции weake (схема бл 2)
                        var StatusCorellationLinkGroup = false;
                        for (int j = 0; j < driveTest.Length; j++)
                        {
                            StatusCorellationLinkGroup = CalcCorellation(taskContext, station, driveTest[j], data, out double? maxCorellation_pc);
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

                            var contextStations = new List<ContextStation>();
                            var driveTestsResult = new List<DriveTestsResult>();

                            for (int j = 0; j < driveTest.Length; j++)
                            {
                                var сalibrationStationsGSIDGroupeStations = CalibrationStations(taskContext, station, driveTest[j], data);
                                // если результат выполнения метода CalibrationStations не пустой, тогда добавляем его в список tempResultCorrelationGSIDGroupeStations
                                if (сalibrationStationsGSIDGroupeStations.Length > 0)
                                {
                                    tempResultCorrelationGSIDGroupeStations.AddRange(/*Atdi.Common.CopyHelper.CreateDeepCopy*/(сalibrationStationsGSIDGroupeStations));
                                }
                            }

                            // По максимуму значения Correlation_pc из п.1 привязываем станции и драйв тесты.
                            var resultCorrelationGSIDGroupeStations = LinkedStationsAndDriveTests(tempResultCorrelationGSIDGroupeStations, out contextStations, out driveTestsResult);

                            //4.2.4. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов (схема бл 4)
                            var calibrationStationsAndDriveTestsResult = FillCalibrationStationResultFirstBlock(resultCorrelationGSIDGroupeStations, data.CalibrationParameters, data.CorellationParameters, data.GeneralParameters);
                            if (calibrationStationsAndDriveTestsResult != null)
                            {
                                if ((calibrationStationsAndDriveTestsResult.ResultCalibrationStation.Length > 0) || (calibrationStationsAndDriveTestsResult.ResultCalibrationDriveTest.Length > 0))
                                {
                                    // добавляем промежуточный результат в массив результатов
                                    calibrationStationsAndDriveTestsResultByGroup.Add(calibrationStationsAndDriveTestsResult);

                                    // убираем из общего списка станций и  драйв тестов  те которые попали в результаты
                                    CompressedStationsAndDriveTest(ref outListContextStations, ref outListDriveTestsResults, resultCorrelationGSIDGroupeStations);
                                }
                            }
                            else
                            {
                                if ((contextStations != null) && (contextStations.Count > 0))
                                {
                                    outListContextStations.Add(contextStations.ToArray());
                                    outListDriveTestsResults.Add(driveTestsResult.ToArray());
                                }
                            }
                        }

                        if (linkDriveTestsAndStations.Length > 0)
                        {
                            percentComplete = (z + 1) * (int)(50.0 / linkDriveTestsAndStations.Length);
                            UpdatePercentComplete(data.resultId, percentComplete);
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
                        var orderByCountPoints = from z in drvTests orderby z.CountPoints descending select z;
                        var duplicateCountPoints = drvTests.GroupBy(x => new { x.CountPoints }).Where(g => g.Count() > 1).SelectMany(g => g.Select(gg => gg));
                        outListDriveTestsResults[i] = orderByCountPoints.ToArray();
                    }

                    for (int i = 0; i < outListDriveTestsResults.Count; i++)
                    {
                        var drvTests = outListDriveTestsResults[i];
                        var fndElems = drvTests.ToList();
                        var keyValueDriveTests = new Dictionary<long, float>();
                        var duplicateCountPoints = drvTests.GroupBy(x => new { x.CountPoints }).Where(g => g.Count() > 1).SelectMany(g => g.Select(gg => gg));
                        if ((duplicateCountPoints != null) && (duplicateCountPoints.Count()) > 0)
                        {

                            var driveTestsResults = new DriveTestsResult[duplicateCountPoints.Count()];
                            var tempDriveTestsByOneGroup = duplicateCountPoints.ToArray();
                            for (int j = 0; j < tempDriveTestsByOneGroup.Length; j++)
                            {
                                keyValueDriveTests.Add(tempDriveTestsByOneGroup[j].Num, (tempDriveTestsByOneGroup[j].Points.Select(x => x.Level_dBm).Sum() / tempDriveTestsByOneGroup[j].Points.Length));
                            }
                            var orderDriveTests = from z in keyValueDriveTests.ToList() orderby z.Value descending select z;
                            var tempOrderDriveTests = orderDriveTests.ToArray();
                            for (int s = 0; s < tempOrderDriveTests.Length; s++)
                            {
                                var fndDriveTest = tempDriveTestsByOneGroup.First(x => x.Num == tempOrderDriveTests[s].Key);
                                driveTestsResults[s] = fndDriveTest;
                            }


                            var arrIndexes = new int[driveTestsResults.Length];
                            for (int k = driveTestsResults.Length - 1, n = 0; k >= 0; k--, n++)
                            {
                                var fnd = fndElems.Find(c => c.Num == driveTestsResults[n].Num);
                                if (fnd != null)
                                {
                                    var index = fndElems.IndexOf(fnd);
                                    fndElems[index] = driveTestsResults[k];
                                }
                            }
                            outListDriveTestsResults[i] = fndElems.ToArray();

                        }
                        else
                        {
                            outListDriveTestsResults[i] = drvTests;
                        }
                    }



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    ///     4.2.6. Ранжирование Stations под данный Drive Test
                    ///     
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////

                    for (int i = 0; i < outListDriveTestsResults.Count; i++)
                    {
                        var arrDriveTests = outListDriveTestsResults[i];
                        if (arrDriveTests != null)
                        {
                            for (int w = 0; w < arrDriveTests.Length; w++)
                            {

                                //получаем очередной драйв тест
                                var currentDriveTest = arrDriveTests[w];
                                if (currentDriveTest != null)
                                {

                                    var coordinatesDrivePoint = currentDriveTest.Points.Select(x => x.Coordinate).ToArray();

                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    ///
                                    ///     1. Для GSIDGroupeDriveTests мы считаем центр масс всех координат, всех точек по заданному драйв тесту. 
                                    ///     
                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    var centerWeightCoordinateOfDriveTest = Utils.CenterWeightAllCoordinates(currentDriveTest.Points);


                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    ///
                                    ///    2. Формируем новый массив (или список) массивов станций (GSIDGroupeStations) на основании того перемещаем туда все станции
                                    ///     из массива (или списка) массивов станций (outListContextStations) если хотябы одна из станций  outListContextStations 
                                    ///     имеют координаты ближе чем 1км (параметр вынести в файл конфигурации в зависимости от STANDART) к координатам GSIDGroupeDriveTests.
                                    ///     
                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    var GSIDGroupeStations = new List<ContextStation[]>();
                                    for (int j = 0; j < outListContextStations.Count; j++)
                                    {
                                        var arrStations = outListContextStations[j];
                                        for (int p = 0; p < coordinatesDrivePoint.Length; p++)
                                        {
                                            var coordinateStation = _transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = arrStations[0].Site.Longitude, Latitude = arrStations[0].Site.Latitude }, _transformation.ConvertProjectionToCode(data.Projection));
                                            if (GeometricСalculations.GetDistance_km(coordinatesDrivePoint[p].X, coordinatesDrivePoint[p].Y, coordinateStation.X, coordinateStation.Y) <= GetMinDistanceFromConfigByStandard(standard))
                                            {
                                                // добавляем весь массив станций arrStations в случае если одна из станций, которая входит в arrStations имеет расстояние до одной из точек текущего DrivePoint меньше 1 км (берем с конфигурации)
                                                GSIDGroupeStations.Add(arrStations);
                                                break;
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

                                    if (GSIDGroupeStations != null)
                                    {
                                        for (int m = 0; m < GSIDGroupeStations.Count; m++)
                                        {

                                            // проводим анализ по каждой отдельно взятой группе станций отдельно
                                            var station = GSIDGroupeStations[m];
                                            var resultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();
                                            var contextStations = new List<ContextStation>();
                                            var driveTestsResult = new List<DriveTestsResult>();


                                            // расчет корелляции
                                            bool statusCorellationLinkGroup = CalcCorellation(taskContext, station, currentDriveTest, data, out double? maxCorellation_pc);
                                            if (statusCorellationLinkGroup == true)
                                            {
                                                var tempResultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();

                                                var сalibrationStationsGSIDGroupeStations = CalibrationStations(taskContext, station, currentDriveTest, data);
                                                // если результат выполнения метода CalibrationStations не пустой, тогда добавляем его в список tempResultCorrelationGSIDGroupeStations
                                                if (сalibrationStationsGSIDGroupeStations.Length > 0)
                                                {
                                                    tempResultCorrelationGSIDGroupeStations.AddRange(сalibrationStationsGSIDGroupeStations);
                                                }
                                                // По максимуму значения Correlation_pc из п.1 привязываем станции и драйв тесты.
                                                resultCorrelationGSIDGroupeStations = LinkedStationsAndDriveTests(tempResultCorrelationGSIDGroupeStations, out contextStations, out driveTestsResult);
                                            }


                                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                            ///    4.2.8. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов (схема бл 8)
                                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                            // Производим последовательную обработку массива (ResultCorrelationGSIDGroupeStations) запись за записью.

                                            if (resultCorrelationGSIDGroupeStations.Count > 0)
                                            {
                                                var calibrationStationsAndDriveTestsResult = FillCalibrationStationResultSecondBlock(resultCorrelationGSIDGroupeStations, data.CalibrationParameters, data.CorellationParameters, data.GeneralParameters);
                                                if (calibrationStationsAndDriveTestsResult != null)
                                                {
                                                    if ((calibrationStationsAndDriveTestsResult.ResultCalibrationStation.Length > 0) || (calibrationStationsAndDriveTestsResult.ResultCalibrationDriveTest.Length > 0))
                                                    {
                                                        // добавляем промежуточный результат в массив результатов
                                                        calibrationStationsAndDriveTestsResultByGroup.Add(calibrationStationsAndDriveTestsResult);

                                                        // убираем из общего списка станций и  драйв тестов  те которые попали в результаты
                                                        CompressedStationsAndDriveTest(ref outListContextStations, ref outListDriveTestsResults, resultCorrelationGSIDGroupeStations);
                                                    }
                                                }
                                            }
                                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                            ///   привязки нет, но блок 2 хоть раз проходили с положительным результатом
                                            /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                            else if ((resultCorrelationGSIDGroupeStations.Count == 0) && (statusCorellationLinkGroup == true))
                                            {
                                                var lstTempCalibrationStationResult = new List<CalibrationStationResult>();
                                                for (int d = 0; d < station.Length; d++)
                                                {
                                                    lstTempCalibrationStationResult.Add(new CalibrationStationResult()
                                                    {
                                                        ExternalSource = station[d].ExternalSource,
                                                        ExternalCode = station[d].ExternalCode,
                                                        LicenseGsid = station[d].LicenseGsid,
                                                        //RealGsid = station[d].RealGsid,
                                                        ResultStationStatus = StationStatusResult.UN,
                                                        IsContour = station[d].Type == ClientContextStationType.A ? true : false,
                                                        StationMonitoringId = station[d].Id,
                                                        MaxCorellation = (float)maxCorellation_pc,
                                                        ParametersStationOld = new ParametersStation()
                                                        {
                                                            Altitude_m = (int)station[d].Site.Altitude,
                                                            Azimuth_deg = station[d].Antenna.Azimuth_deg,
                                                            Tilt_Deg = station[d].Antenna.Tilt_deg,
                                                            Lat_deg = station[d].Site.Latitude,
                                                            Lon_deg = station[d].Site.Longitude,
                                                            Power_dB = station[d].Transmitter.MaxPower_dBm,
                                                            Freq_MHz = station[d].Transmitter.Freq_MHz
                                                        },
                                                        Standard = station[d].RealStandard,
                                                        Freq_MHz = (float)currentDriveTest.Freq_MHz
                                                    });
                                                }
                                                var lstCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
                                                for (int d = 0; d < station.Length; d++)
                                                {
                                                    lstCalibrationDriveTestResult.Add(
                                                         new CalibrationDriveTestResult()
                                                         {
                                                             CountPointsInDriveTest = currentDriveTest.Points.Length,
                                                             DriveTestId = currentDriveTest.DriveTestId,
                                                             Gsid = currentDriveTest.GSID,
                                                             LinkToStationMonitoringId = currentDriveTest.LinkToStationMonitoringId,
                                                             ResultDriveTestStatus = DriveTestStatusResult.UN,
                                                             Standard = currentDriveTest.RealStandard,
                                                             Freq_MHz = (float)currentDriveTest.Freq_MHz
                                                         }
                                                    );
                                                }

                                                // формируем результат с массивом драйв тестов CalibrationDriveTestResult[] = null
                                                calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                                                {
                                                    ResultCalibrationDriveTest = lstCalibrationDriveTestResult.ToArray(),
                                                    ResultCalibrationStation = lstTempCalibrationStationResult.ToArray()
                                                });

                                                // убираем из общего списка станций  те которые попали в результаты
                                                // ????????????? тут не понятно - возможно на этом этапе удалять их из списка outListContextStations нельзя, т.к. надо пройти проверку  - есть ли станции сто статусом "P" ??????????????
                                                //CompressedStations(ref outListContextStations, station); ///?????????????????????????????????????
                                                CompressedStationsAndDriveTest(ref outListContextStations, ref outListDriveTestsResults, resultCorrelationGSIDGroupeStations);
                                            }
                                        }
                                    }

                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    ///
                                    ///   4.2.10. Оценка Drive Test на предмет НДП (схема бл. 9)
                                    ///   
                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////

                                    /// обработка станций со статусом "P" 
                                    if ((outListContextStationsForStatusP != null) && (outListContextStationsForStatusP.Length > 0))
                                    {
                                        var lstStationsForStatusP = outListContextStationsForStatusP.ToList();
                                        for (int m = 0; m < lstStationsForStatusP.Count; m++)
                                        {
                                            // проводим анализ по каждой отдельно взятой группе станций отдельно
                                            var station = lstStationsForStatusP[m];

                                            // расчет корелляции
                                            bool statusCorellationLinkGroup = CalcCorellation(taskContext, station, currentDriveTest, data, out double? maxCorellation_pc);
                                            if (statusCorellationLinkGroup == true)
                                            {

                                                var lstCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
                                                var lsttempCalibrationStationResult = new List<CalibrationStationResult>();
                                                for (int d = 0; d < station.Length; d++)
                                                {
                                                    lsttempCalibrationStationResult.Add(new CalibrationStationResult()
                                                    {
                                                        ExternalSource = station[d].ExternalSource,
                                                        ExternalCode = station[d].ExternalCode,
                                                        LicenseGsid = station[d].LicenseGsid,
                                                        //RealGsid = station[d].RealGsid,
                                                        RealGsid = currentDriveTest.GSID,
                                                        ResultStationStatus = StationStatusResult.UN,
                                                        IsContour = station[d].Type == ClientContextStationType.A ? true : false,
                                                        StationMonitoringId = station[d].Id,
                                                        MaxCorellation = (float)maxCorellation_pc,
                                                        //ParametersStationNew=  ??????????????????????
                                                        ParametersStationOld = new ParametersStation()
                                                        {
                                                            Altitude_m = (int)station[d].Site.Altitude,
                                                            Azimuth_deg = station[d].Antenna.Azimuth_deg,
                                                            Tilt_Deg = station[d].Antenna.Tilt_deg,
                                                            Lat_deg = station[d].Site.Latitude,
                                                            Lon_deg = station[d].Site.Longitude,
                                                            Power_dB = station[d].Transmitter.MaxPower_dBm,
                                                            Freq_MHz = station[d].Transmitter.Freq_MHz
                                                        },
                                                        Standard = station[d].RealStandard,
                                                        Freq_MHz = (float)currentDriveTest.Freq_MHz
                                                    });
                                                }

                                                calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                                                {
                                                    ResultCalibrationDriveTest = new CalibrationDriveTestResult[1]
                                                    {
                                                    new CalibrationDriveTestResult()
                                                    {
                                                         CountPointsInDriveTest = currentDriveTest.Points.Length,
                                                         DriveTestId = currentDriveTest.DriveTestId,
                                                         Gsid = currentDriveTest.GSID,
                                                         LinkToStationMonitoringId = currentDriveTest.LinkToStationMonitoringId,
                                                         ResultDriveTestStatus = DriveTestStatusResult.IT,
                                                         Standard = currentDriveTest.RealStandard,
                                                         Freq_MHz = (float)currentDriveTest.Freq_MHz
                                                 }
                                                    },
                                                    ResultCalibrationStation = lsttempCalibrationStationResult.ToArray()
                                                });
                                                // убираем из общего списка станций  те которые попали в результаты
                                                CompressedStations(ref lstStationsForStatusP, station);
                                            }
                                        }
                                        outListContextStationsForStatusP = lstStationsForStatusP.ToArray();
                                    }

                                    ///////////////////////////  если до текущего мемента драйв тест никуда не попал, тогда формируем результат с ним ////////////////////////////////
                                    if (calibrationStationsAndDriveTestsResultByGroup.Count > 0)
                                    {
                                        var listTempCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
                                        for (int m = 0; m < calibrationStationsAndDriveTestsResultByGroup.Count; m++)
                                        {
                                            var calibrationStationsAndDriveTest = calibrationStationsAndDriveTestsResultByGroup[m].ResultCalibrationDriveTest;
                                            if (calibrationStationsAndDriveTest != null)
                                            {
                                                listTempCalibrationDriveTestResult.AddRange(calibrationStationsAndDriveTest);
                                            }
                                        }


                                        if (listTempCalibrationDriveTestResult.Find(x => x.DriveTestId == currentDriveTest.DriveTestId) == null)
                                        {
                                            calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                                            {
                                                ResultCalibrationDriveTest = new CalibrationDriveTestResult[1]
                                              {
                                                    new CalibrationDriveTestResult()
                                                    {
                                                         CountPointsInDriveTest = currentDriveTest.Points.Length,
                                                         DriveTestId = currentDriveTest.DriveTestId,
                                                         Gsid = currentDriveTest.GSID,
                                                         LinkToStationMonitoringId = currentDriveTest.LinkToStationMonitoringId,
                                                         ResultDriveTestStatus = DriveTestStatusResult.IT,
                                                         Standard = currentDriveTest.RealStandard,
                                                         Freq_MHz = (float)currentDriveTest.Freq_MHz
                                                 }
                                              },
                                                ResultCalibrationStation = null
                                            });

                                        }
                                    }
                                    else
                                    {
                                        calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                                        {
                                            ResultCalibrationDriveTest = new CalibrationDriveTestResult[1]
                                                  {
                                                    new CalibrationDriveTestResult()
                                                    {
                                                         CountPointsInDriveTest = currentDriveTest.Points.Length,
                                                         DriveTestId = currentDriveTest.DriveTestId,
                                                         Gsid = currentDriveTest.GSID,
                                                         LinkToStationMonitoringId = currentDriveTest.LinkToStationMonitoringId,
                                                         ResultDriveTestStatus = DriveTestStatusResult.IT,
                                                         Standard = currentDriveTest.RealStandard,
                                                         Freq_MHz = (float)currentDriveTest.Freq_MHz
                                                 }
                                                  },
                                            ResultCalibrationStation = null
                                        });
                                    }
                                }
                            }
                        }

                        if (outListDriveTestsResults.Count > 0)
                        {
                            percentComplete = 50 + ((i + 1) * (int)(50.0 / outListDriveTestsResults.Count));
                            UpdatePercentComplete(data.resultId, percentComplete);
                        }
                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///
                    ///   4.2.11. Все не изъятые Stations помечаются как необнаруженные (схема бл. 10)
                    ///   
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (outListContextStationsForStatusP.Length > 0)
                    {
                        outListContextStations.AddRange(outListContextStationsForStatusP);
                    }

                    var lstCalibrationStationResult = new List<CalibrationStationResult>();
                    for (int j = 0; j < outListContextStations.Count; j++)
                    {
                        var arrStations = outListContextStations[j];
                        for (int d = 0; d < arrStations.Length; d++)
                        {
                            lstCalibrationStationResult.Add(new CalibrationStationResult()
                            {
                                ExternalSource = arrStations[d].ExternalSource,
                                ExternalCode = arrStations[d].ExternalCode,
                                LicenseGsid = arrStations[d].LicenseGsid,
                                //RealGsid = arrStations[d].RealGsid,
                                ResultStationStatus = StationStatusResult.NF, // если была UN то NF быть не может !!!!
                                StationMonitoringId = arrStations[d].Id,
                                IsContour = arrStations[d].Type == ClientContextStationType.A ? true : false,
                                //ParametersStationNew=  ??????????????????????
                                ParametersStationOld = new ParametersStation()
                                {
                                    Altitude_m = (int)arrStations[d].Site.Altitude,
                                    Azimuth_deg = arrStations[d].Antenna.Azimuth_deg,
                                    Tilt_Deg = arrStations[d].Antenna.Tilt_deg,
                                    Lat_deg = arrStations[d].Site.Latitude,
                                    Lon_deg = arrStations[d].Site.Longitude,
                                    Power_dB = arrStations[d].Transmitter.MaxPower_dBm,
                                    Freq_MHz = arrStations[d].Transmitter.Freq_MHz
                                },
                                Standard = arrStations[d].RealStandard,
                                Freq_MHz = arrStations[d].Transmitter.Freq_MHz
                            });
                        }
                    }

                    // формируем результат с массивом драйв тестов CalibrationDriveTestResult[] = null
                    if (lstCalibrationStationResult.Count > 0)
                    {
                        calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                        {
                            ResultCalibrationDriveTest = null, /*lstCalibrationDriveTestResult.ToArray(),*/
                            ResultCalibrationStation = lstCalibrationStationResult.ToArray()
                        });
                    }


                    // до этого момента станции прошли весь алгоритм обработки и если остаются какие-то, то для них устанавиливаем статус NF в результате и очищаем соотвественно список outListContextStations
                    outListContextStations.Clear();
                    outListDriveTestsResults.Clear();

                    // подготовка общего результата

                    var listCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
                    var listCalibrationStationResult = new List<CalibrationStationResult>();
                    for (int m = 0; m < calibrationStationsAndDriveTestsResultByGroup.Count; m++)
                    {
                        var calibrationStationsAndDriveTest = calibrationStationsAndDriveTestsResultByGroup[m];
                        var driveTests = calibrationStationsAndDriveTest.ResultCalibrationDriveTest;
                        var stations = calibrationStationsAndDriveTest.ResultCalibrationStation;
                        if (driveTests != null)
                        {
                            for (int z = 0; z < driveTests.Length; z++)
                            {
                                if (driveTests[z] != null)
                                {
                                    listCalibrationDriveTestResult.Add(driveTests[z]);
                                }
                            }
                        }
                        if (stations != null)
                        {
                            for (int z = 0; z < stations.Length; z++)
                            {
                                if (stations[z] != null)
                                {
                                    listCalibrationStationResult.Add(stations[z]);
                                }
                            }
                        }
                    }


                    var areasSelect = data.GSIDGroupeStation.ToList().Select(x => x.RegionCode).Distinct();
                    string areas = string.Join(",", areasSelect.ToArray());

                    var calcCorellationResult = new CalibrationResult()
                    {
                        // заполнить все поля
                        Standard = standard, // стандарт
                        ResultCalibrationStation = listCalibrationStationResult.ToArray(), // станции
                        ResultCalibrationDriveTest = listCalibrationDriveTestResult.ToArray(), // драйв тесты
                        CountStation_CS = listCalibrationStationResult.FindAll(x => x.ResultStationStatus == StationStatusResult.CS).Count(), // число станций со статусом CS
                        CountStation_IT = listCalibrationStationResult.FindAll(x => x.ResultStationStatus == StationStatusResult.IT).Count(), // число станций со статусом IT
                        CountStation_NF = listCalibrationStationResult.FindAll(x => x.ResultStationStatus == StationStatusResult.NF).Count(), // число станций со статусом NF
                        CountStation_NS = listCalibrationStationResult.FindAll(x => x.ResultStationStatus == StationStatusResult.NS).Count(), // число станций со статусом NS
                        CountStation_UN = listCalibrationStationResult.FindAll(x => x.ResultStationStatus == StationStatusResult.UN).Count(), // число станций со статусом UN
                        AreaName = areas,
                        CountMeasGSID = listCalibrationDriveTestResult.Select(x => x.Gsid).Count(),  // число GCID в драйв тестах
                        CountMeasGSID_IT = listCalibrationDriveTestResult.FindAll(x => x.ResultDriveTestStatus == DriveTestStatusResult.IT).Count(), // число драйв тестов со статусом IT
                        CountMeasGSID_LS = listCalibrationDriveTestResult.FindAll(x => x.ResultDriveTestStatus == DriveTestStatusResult.LS).Count(), // число драйв тестов со статусом LS
                        GeneralParameters = data.GeneralParameters,
                        NumberStation = listCalibrationStationResult.Count(),  // общее число станций
                        NumberStationInContour = listCalibrationStationResult.FindAll(x => x.IsContour == true).Count(), // общее число станций, которіе имеют статус А (попадают в контур)
                        TimeStart = DateTime.Now
                    };
                    listCalcCorellationResult[v] = calcCorellationResult;
                }
                UpdatePercentComplete(data.resultId, 100);
                return listCalcCorellationResult;
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibration, e);
            }
            finally
            {
                if (_calcDbScope != null)
                {
                    _calcDbScope.Dispose();
                    _calcDbScope = null;
                }
            }
            return null;
        }

        /// <summary>
        /// Обновить процент выполнения задачи
        /// </summary>
        /// <param name="resultId"></param>
        /// <param name="percentComplete"></param>
        private void UpdatePercentComplete(long resultId, int percentComplete)
        {
            var updateQueryStationCalibrationResult = _calcServerDataLayer.GetBuilder<IStationCalibrationResult>()
                          .Update()
                          .SetValue(c => c.PercentComplete, percentComplete)
                          .Where(c => c.Id, ConditionOperator.Equal, resultId);
            _calcDbScope.Executor.Execute(updateQueryStationCalibrationResult);
        }

        /// <summary>
        /// Расчет корреляции weake (схема бл 2)
        /// </summary>
        /// <param name="taskContext"></param>
        /// <param name="stations"></param>
        /// <param name="driveTest"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CalcCorellation(ITaskContext taskContext, ContextStation[] stations, DriveTestsResult driveTest, AllStationCorellationCalcData data, out double? maxCorellation_pc)
        {
            maxCorellation_pc = 0;
            var listCorellation_pc = new List<double>();
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
                    GeneralParameters = data.GeneralParameters,
                    CodeProjection = data.Projection
                };

                var iterationCorellationCalc = _iterationsPool.GetIteration<StationCorellationCalcData, ResultCorrelationGSIDGroupeStationsWithoutParameters>();
                var resultCorellationCalcData = iterationCorellationCalc.Run(taskContext, stationCorellationCalcData);

                // Если максимальная корреляция превысит(или равна) Сorrelation threshold weak то возвращаем True
                if (resultCorellationCalcData.Corellation_pc >= stationCorellationCalcData.GeneralParameters.СorrelationThresholdWeak)
                {
                    listCorellation_pc.Add(resultCorellationCalcData.Corellation_pc);
                    StatusCorellationLinkGroup = true;
                    break;
                }
            }
            
            if (listCorellation_pc.Count > 0)
            {
                maxCorellation_pc = listCorellation_pc.Max();
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
        public ResultCorrelationGSIDGroupeStations[] CalibrationStations(ITaskContext taskContext, ContextStation[] stations, DriveTestsResult driveTest, AllStationCorellationCalcData data)
        {
            var tempListCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();
            for (int i = 0; i < stations.Length; i++)
            {
                // при условии совпадения частот
                if (stations[i].Transmitter.Freqs_MHz.Contains(driveTest.Freq_MHz))
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
                        GeneralParameters = data.GeneralParameters,
                        CodeProjection = data.Projection
                    };

                    var iterationCalibrationCalc = _iterationsPool.GetIteration<StationCalibrationCalcData, ResultCorrelationGSIDGroupeStations>();
                    var resultCalibrationCalcData = iterationCalibrationCalc.Run(taskContext, stationCalibrationCalcData);
                    if (!double.IsNaN(resultCalibrationCalcData.Corellation_factor))
                    {
                        var res = Atdi.Common.CopyHelper.CreateDeepCopy(resultCalibrationCalcData);
                        res.ClientContextStation = stations[i];
                        res.DriveTestsResult = driveTest;
                        res.DriveTestsResult.DriveTestId = driveTest.DriveTestId;
                        res.MaxCorrelation_PC = resultCalibrationCalcData.Corellation_pc;
                        tempListCorrelationGSIDGroupeStations.Add(res);
                    }
                }
            }
            return tempListCorrelationGSIDGroupeStations.ToArray();
        }

        /// <summary>
        /// Валидация параметров станции
        /// </summary>
        /// <param name="parametersStationOld"></param>
        /// <param name="parametersStationNew"></param>
        /// <param name="calibrationParameters"></param>
        /// <returns></returns>
        private bool CheckParameterStation(ParametersStation parametersStationOld, ParametersStation parametersStationNew, CalibrationParameters calibrationParameters)
        {
            bool isCorrect = true;
            if ((parametersStationOld !=null) && (parametersStationNew != null))
            {
                if (calibrationParameters.AltitudeStation)
                {
                    if ((Math.Abs(parametersStationOld.Altitude_m - parametersStationNew.Altitude_m) <= calibrationParameters.MaxDeviationAltitudeStation_m)==false)
                    {
                        isCorrect = false;
                    }
                }
                if (calibrationParameters.AzimuthStation)
                {
                    if ((Math.Abs(parametersStationOld.Azimuth_deg - parametersStationNew.Azimuth_deg) <= calibrationParameters.MaxDeviationAzimuthStation_deg)==false)
                    {
                        isCorrect = false;
                    }
                }
                if (calibrationParameters.CoordinatesStation)
                {
                    if ((Math.Abs(parametersStationOld.Lat_deg - parametersStationNew.Lat_deg) <= calibrationParameters.MaxDeviationCoordinatesStation_m)==false)
                    {
                        isCorrect = false;
                    }
                    if ((Math.Abs(parametersStationOld.Lon_deg - parametersStationNew.Lon_deg) <= calibrationParameters.MaxDeviationCoordinatesStation_m)==false)
                    {
                        isCorrect = false;
                    }
                }
                if (calibrationParameters.PowerStation)
                {
                    if ((Math.Abs(parametersStationOld.Power_dB - parametersStationNew.Power_dB) <= calibrationParameters.ShiftPowerStationStep_dB)==false) // ??????????????????????????
                    {
                        isCorrect = false;
                    }
                }
                if (calibrationParameters.TiltStation)
                {
                    if ((Math.Abs(parametersStationOld.Tilt_Deg - parametersStationNew.Tilt_Deg) <= calibrationParameters.MaxDeviationTiltStationDeg)==false)
                    {
                        isCorrect = false;
                    }
                }
            }
            return isCorrect;
        }

        /// <summary>
        /// Заполнение результата для случая описанного в пункте 4.2.4. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов
        /// </summary>
        /// <param name="resultsCorrelationGSIDGroupeStations"></param>
        /// <returns></returns>
        public CalibrationStationsAndDriveTestsResult FillCalibrationStationResultFirstBlock(List<ResultCorrelationGSIDGroupeStations> resultsCorrelationGSIDGroupeStations, CalibrationParameters calibrationParameters, CorellationParameters corellationParameters, GeneralParameters generalParameters)
        {
            var listCalibrationStationResults = new List<CalibrationStationResult>();
            var listCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
            for (int g = 0; g < resultsCorrelationGSIDGroupeStations.Count; g++)
            {
                // извлекаем очередной результат, полученный в методе CalibrationStations
                var calibrationStationsAndDriveTestsResult = resultsCorrelationGSIDGroupeStations[g];
                if ((calibrationStationsAndDriveTestsResult.DriveTestsResult != null) && (calibrationStationsAndDriveTestsResult.ClientContextStation != null))
                {
                    // статусы ResultStationStatus и ResultDriveTestStatus определяем из RulesStatusParameters
                    // здесь нужно корректно заполнить параметры ниже
                    var pointForCorrelation = calibrationStationsAndDriveTestsResult.DriveTestsResult.CountPoints >= generalParameters.MinNumberPointForCorrelation ? true : false;
                    var corellation = calibrationStationsAndDriveTestsResult.MaxCorrelation_PC > generalParameters.СorrelationThresholdHard ? true : false;
                    var trustOldResults = generalParameters.TrustOldResults;
                    var exceptionParameter = CheckParameterStation(calibrationStationsAndDriveTestsResult.ParametersStationOld, calibrationStationsAndDriveTestsResult.ParametersStationNew, calibrationParameters);
                    var status = calibrationStationsAndDriveTestsResult.ClientContextStation.Type;
                    var calibrationStatusParameters = RulesStatusParameters.CalibrationFirstStatusParameters.Find(c => c.Corellation == corellation && c.StatusStation == status && c.TrustOldResults == trustOldResults && c.PointForCorrelation == pointForCorrelation && c.ExceptionParameter == exceptionParameter);
                    if (calibrationStatusParameters != null)
                    {
                        var calibrationStationResult = new CalibrationStationResult()
                        {
                            ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                            ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                            LicenseGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                            //RealGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.RealGsid,
                            RealGsid = calibrationStationsAndDriveTestsResult.DriveTestsResult.GSID,
                            ParametersStationNew = calibrationStationsAndDriveTestsResult.ParametersStationNew,
                            ParametersStationOld = calibrationStationsAndDriveTestsResult.ParametersStationOld,
                            ResultStationStatus = calibrationStatusParameters.ResultStationStatus,
                            StationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id,
                            IsContour = calibrationStationsAndDriveTestsResult.ClientContextStation.Type== ClientContextStationType.A ? true : false,
                            MaxCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                            Standard = calibrationStationsAndDriveTestsResult.ClientContextStation.RealStandard,
                            Freq_MHz = (float)calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                        };
                        listCalibrationStationResults.Add(calibrationStationResult);

                        var calibrationDriveTestResult = new CalibrationDriveTestResult()
                        {
                            ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                            ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                            Gsid = calibrationStationsAndDriveTestsResult.DriveTestsResult.GSID,
                            GsidFromStation = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                            CountPointsInDriveTest = calibrationStationsAndDriveTestsResult.DriveTestsResult.Points.Length,
                            ResultDriveTestStatus = calibrationStatusParameters.ResultDriveTestStatus,
                            LinkToStationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id,
                            DriveTestId = calibrationStationsAndDriveTestsResult.DriveTestsResult.DriveTestId,
                            MaxPercentCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                            Standard = calibrationStationsAndDriveTestsResult.DriveTestsResult.RealStandard,
                            Freq_MHz = (float)calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                        };
                        listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                    }
                }
                // содержит только драйв тест
                else if ((calibrationStationsAndDriveTestsResult.DriveTestsResult != null) && (calibrationStationsAndDriveTestsResult.ClientContextStation == null))
                {
                    var calibrationDriveTestResult = new CalibrationDriveTestResult()
                    {
                        //ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                        //ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                        Gsid = calibrationStationsAndDriveTestsResult.DriveTestsResult.GSID,
                        //GsidFromStation = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                        CountPointsInDriveTest = calibrationStationsAndDriveTestsResult.DriveTestsResult.Points.Length,
                        ResultDriveTestStatus = DriveTestStatusResult.IT,
                        DriveTestId = calibrationStationsAndDriveTestsResult.DriveTestsResult.DriveTestId,
                        MaxPercentCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                        //LinkToStationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id
                        Standard = calibrationStationsAndDriveTestsResult.DriveTestsResult.RealStandard,
                        Freq_MHz = (float)calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                    };
                    listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                }
                // содержит только станцию
                else if ((calibrationStationsAndDriveTestsResult.DriveTestsResult == null) && (calibrationStationsAndDriveTestsResult.ClientContextStation != null))
                {
                    var calibrationStationResult = new CalibrationStationResult()
                    {
                        ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                        ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                        LicenseGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                        //RealGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.RealGsid,
                        ParametersStationNew = calibrationStationsAndDriveTestsResult.ParametersStationNew,
                        ParametersStationOld = calibrationStationsAndDriveTestsResult.ParametersStationOld,
                        //ResultStationStatus = calibrationStationsAndDriveTestsResult.MaxCorrelation_PC > 0 ? StationStatusResult.UN : StationStatusResult.NF,
                        ResultStationStatus = StationStatusResult.NF,
                        IsContour = calibrationStationsAndDriveTestsResult.ClientContextStation.Type == ClientContextStationType.A ? true : false,
                        MaxCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                        StationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id,
                        Standard = calibrationStationsAndDriveTestsResult.ClientContextStation.RealStandard,
                        Freq_MHz = calibrationStationsAndDriveTestsResult.ClientContextStation.Transmitter.Freq_MHz
                    };
                    listCalibrationStationResults.Add(calibrationStationResult);
                }
            }
            if ((listCalibrationStationResults.Count > 0) || (listCalibrationDriveTestResult.Count > 0))
            {
                return new CalibrationStationsAndDriveTestsResult()
                {
                    ResultCalibrationDriveTest = listCalibrationDriveTestResult.ToArray(),
                    ResultCalibrationStation = listCalibrationStationResults.ToArray()
                };
            }
            return null;
        }

        /// <summary>
        /// Заполнение результата для случая описанного в пункте 4.2.7 (Подбор параметров станции по результатам Drive Test)
        /// </summary>
        /// <param name="resultsCorrelationGSIDGroupeStations"></param>
        /// <returns></returns>
        public CalibrationStationsAndDriveTestsResult FillCalibrationStationResultSecondBlock(List<ResultCorrelationGSIDGroupeStations> resultsCorrelationGSIDGroupeStations, CalibrationParameters calibrationParameters, CorellationParameters corellationParameters, GeneralParameters generalParameters)
        {
            var listCalibrationStationResults = new List<CalibrationStationResult>();
            var listCalibrationDriveTestResult = new List<CalibrationDriveTestResult>();
            for (int g = 0; g < resultsCorrelationGSIDGroupeStations.Count; g++)
            {
                // извлекаем очередной результат, полученный в методе CalibrationStations
                var calibrationStationsAndDriveTestsResult = resultsCorrelationGSIDGroupeStations[g];
                if (calibrationStationsAndDriveTestsResult.ClientContextStation.Type == ClientContextStationType.P)
                {
                    continue;
                }

                if ((calibrationStationsAndDriveTestsResult.DriveTestsResult != null) && (calibrationStationsAndDriveTestsResult.ClientContextStation != null))
                {
                    // статусы ResultStationStatus и ResultDriveTestStatus определяем из RulesStatusParameters
                    // здесь нужно корректно заполнить параметры ниже
                    bool? pointForCorrelation = calibrationStationsAndDriveTestsResult.DriveTestsResult.CountPoints >= generalParameters.MinNumberPointForCorrelation ? true : false;
                    bool? exceptionParameter = CheckParameterStation(calibrationStationsAndDriveTestsResult.ParametersStationOld, calibrationStationsAndDriveTestsResult.ParametersStationNew, calibrationParameters);
                    var status = calibrationStationsAndDriveTestsResult.ClientContextStation.Type;
                    var calibrationStatusParameters = RulesStatusParameters.CalibrationSecondStatusParameter.Find(c => c.StatusStation == status && c.PointForCorrelation == pointForCorrelation && c.ExceptionParameter == exceptionParameter);
                    if (calibrationStatusParameters != null)
                    {
                        var calibrationStationResult = new CalibrationStationResult()
                        {
                            ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                            ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                            LicenseGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                            //RealGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.RealGsid,
                            RealGsid = calibrationStationsAndDriveTestsResult.DriveTestsResult.GSID,
                            ParametersStationNew = calibrationStationsAndDriveTestsResult.ParametersStationNew,
                            ParametersStationOld = calibrationStationsAndDriveTestsResult.ParametersStationOld,
                            ResultStationStatus = calibrationStatusParameters.ResultStationStatus,
                            StationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id,
                            IsContour = calibrationStationsAndDriveTestsResult.ClientContextStation.Type == ClientContextStationType.A ? true : false,
                            MaxCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                            Standard = calibrationStationsAndDriveTestsResult.ClientContextStation.RealStandard,
                            Freq_MHz = (float)calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                        };
                        listCalibrationStationResults.Add(calibrationStationResult);

                        var calibrationDriveTestResult = new CalibrationDriveTestResult()
                        {
                            ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                            ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                            Gsid = calibrationStationsAndDriveTestsResult.DriveTestsResult.GSID,
                            GsidFromStation = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                            CountPointsInDriveTest = calibrationStationsAndDriveTestsResult.DriveTestsResult.Points.Length,
                            ResultDriveTestStatus = calibrationStatusParameters.ResultDriveTestStatus,
                            LinkToStationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id,
                            DriveTestId = calibrationStationsAndDriveTestsResult.DriveTestsResult.DriveTestId,
                            MaxPercentCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                            Standard = calibrationStationsAndDriveTestsResult.DriveTestsResult.RealStandard,
                            Freq_MHz = (float)calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                        };
                        listCalibrationDriveTestResult.Add(calibrationDriveTestResult);
                    }
                }
                // содержит только драйв тест
                else if ((calibrationStationsAndDriveTestsResult.DriveTestsResult != null) && (calibrationStationsAndDriveTestsResult.ClientContextStation == null))
                {
                    var calibrationDriveTestResult = new CalibrationDriveTestResult()
                    {
                        //ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                        //ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                        Gsid = calibrationStationsAndDriveTestsResult.DriveTestsResult.GSID,
                        //GsidFromStation = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                        CountPointsInDriveTest = calibrationStationsAndDriveTestsResult.DriveTestsResult.Points.Length,
                        DriveTestId = calibrationStationsAndDriveTestsResult.DriveTestsResult.DriveTestId,
                        MaxPercentCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                        Standard = calibrationStationsAndDriveTestsResult.DriveTestsResult.RealStandard,
                        Freq_MHz = (float)calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                        //LinkToStationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id
                    };
                    if (calibrationStationsAndDriveTestsResult.DriveTestsResult.CountPoints >= generalParameters.MinNumberPointForCorrelation)
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
                else if ((calibrationStationsAndDriveTestsResult.DriveTestsResult == null) && (calibrationStationsAndDriveTestsResult.ClientContextStation != null))
                {
                    var calibrationStationResult = new CalibrationStationResult()
                    {
                        ExternalSource = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalSource,
                        ExternalCode = calibrationStationsAndDriveTestsResult.ClientContextStation.ExternalCode,
                        LicenseGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.LicenseGsid,
                        //RealGsid = calibrationStationsAndDriveTestsResult.ClientContextStation.RealGsid,
                        ParametersStationNew = calibrationStationsAndDriveTestsResult.ParametersStationNew,
                        ParametersStationOld = calibrationStationsAndDriveTestsResult.ParametersStationOld,
                        MaxCorellation = (float)calibrationStationsAndDriveTestsResult.MaxCorrelation_PC,
                        IsContour = calibrationStationsAndDriveTestsResult.ClientContextStation.Type == ClientContextStationType.A ? true : false,
                        StationMonitoringId = calibrationStationsAndDriveTestsResult.ClientContextStation.Id,
                        Standard = calibrationStationsAndDriveTestsResult.ClientContextStation.RealStandard,
                        Freq_MHz = calibrationStationsAndDriveTestsResult.ClientContextStation.Transmitter.Freq_MHz
                    };
                    calibrationStationResult.ResultStationStatus = StationStatusResult.UN;
                    listCalibrationStationResults.Add(calibrationStationResult);
                }
            }
            if ((listCalibrationStationResults.Count > 0) || (listCalibrationDriveTestResult.Count > 0))
            {
                return new CalibrationStationsAndDriveTestsResult()
                {
                    ResultCalibrationDriveTest = listCalibrationDriveTestResult.ToArray(),
                    ResultCalibrationStation = listCalibrationStationResults.ToArray()
                };
            }
            return null;
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
                    // если есть связывание между драйв тестом и станцией тогда включаем алгоритм ее удаления из общего перечня contextStations

                    if ((resultCorrelationGSIDGroupeStations[b].DriveTestsResult != null) && (resultCorrelationGSIDGroupeStations[b].ClientContextStation != null))
                    {
                        if (resultCorrelationGSIDGroupeStations[b].DriveTestsResult.LinkToStationMonitoringId == resultCorrelationGSIDGroupeStations[b].ClientContextStation.Id)
                        {
                            if (resultCorrelationGSIDGroupeStations[b].ClientContextStation != null)
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
                    }
                    else if ((resultCorrelationGSIDGroupeStations[b].DriveTestsResult == null) && (resultCorrelationGSIDGroupeStations[b].ClientContextStation != null))
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
            }

            // убираем из общего списка драйв тестов, те которые попали в результаты
            for (int f = 0; f < driveTests.Count; f++)
            {
                var arrDriveTests = driveTests[f].ToList();
                for (int b = 0; b < resultCorrelationGSIDGroupeStations.Count; b++)
                {
                    // если есть связывание между драйв тестом и станцией тогда включаем алгоритм ее удаления из общего перечня contextStations
                    if ((resultCorrelationGSIDGroupeStations[b].DriveTestsResult != null) && (resultCorrelationGSIDGroupeStations[b].ClientContextStation != null))
                    {
                        if (resultCorrelationGSIDGroupeStations[b].DriveTestsResult.LinkToStationMonitoringId == resultCorrelationGSIDGroupeStations[b].ClientContextStation.Id)
                        {
                            if (resultCorrelationGSIDGroupeStations[b].DriveTestsResult != null)
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
                    else if ((resultCorrelationGSIDGroupeStations[b].DriveTestsResult != null) && (resultCorrelationGSIDGroupeStations[b].ClientContextStation == null))
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
        }

        /// <summary>
        /// Удаление из общего перечня станций contextStations тех, для которых привязка найдена не была
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="gsidGroupeStations"></param>
        public void CompressedStations(ref List<ContextStation[]> contextStations,  ContextStation[] gsidGroupeStations)
        {
            // убираем из общего списка станций, те которые попали в результаты
            if (contextStations != null)
            {
                for (int f = 0; f < contextStations.Count; f++)
                {
                    var arrStations = contextStations[f].ToList();
                    if (gsidGroupeStations != null)
                    {
                        for (int b = 0; b < gsidGroupeStations.Length; b++)
                        {
                            if (gsidGroupeStations[b] != null)
                            {
                                arrStations.RemoveAll(x => x.Id == gsidGroupeStations[b].Id);
                                if (arrStations.Count > 0)
                                {
                                    contextStations[f] = arrStations.ToArray();
                                }
                                else
                                {
                                    if (contextStations.Count > 0)
                                    {
                                        contextStations.RemoveAt(f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 2. По максимуму значения Correlation_pc из п.1 привязываем станции и драйв тесты.
        /// </summary>
        /// <param name="tempResultCorrelationGSIDGroupeStations"></param>
        /// <param name="contextStations"></param>
        /// <param name="driveTestsResult"></param>
        /// <returns></returns>
        public List<ResultCorrelationGSIDGroupeStations> LinkedStationsAndDriveTests(List<ResultCorrelationGSIDGroupeStations> tempResultCorrelationGSIDGroupeStations, out List<ContextStation> contextStations, out List<DriveTestsResult> driveTestsResult)
        {
            var resultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();
            contextStations = new List<ContextStation>();
            driveTestsResult = new List<DriveTestsResult>();
            if ((tempResultCorrelationGSIDGroupeStations != null) && (tempResultCorrelationGSIDGroupeStations.Count > 0))
            {
                var cpyTempResultCorrelationGSIDGroupeStations = Atdi.Common.CopyHelper.CreateDeepCopy(tempResultCorrelationGSIDGroupeStations);
                while (tempResultCorrelationGSIDGroupeStations.Count > 0)
                {
                    if ((tempResultCorrelationGSIDGroupeStations != null) && (tempResultCorrelationGSIDGroupeStations.Count > 0))
                    {
                        var maxCorrelationPC = tempResultCorrelationGSIDGroupeStations.Select(x => x.MaxCorrelation_PC);
                        if (maxCorrelationPC != null)
                        {
                            var max = maxCorrelationPC.Max();
                            var recalcDriveTests = tempResultCorrelationGSIDGroupeStations.Find(x => x.MaxCorrelation_PC == max);
                            if (recalcDriveTests != null)
                            {
                                if (((contextStations.Find(x => x.Id == recalcDriveTests.ClientContextStation.Id)) == null) && ((driveTestsResult.Find(x => x.DriveTestId == recalcDriveTests.DriveTestsResult.DriveTestId)) == null))
                                {
                                    recalcDriveTests.DriveTestsResult.LinkToStationMonitoringId = recalcDriveTests.ClientContextStation.Id;
                                    resultCorrelationGSIDGroupeStations.Add(recalcDriveTests);
                                    contextStations.Add(recalcDriveTests.ClientContextStation);
                                    driveTestsResult.Add(recalcDriveTests.DriveTestsResult);
                                }
                                tempResultCorrelationGSIDGroupeStations.Remove(recalcDriveTests);
                            }
                        }
                    }
                }

                for (int u = 0; u < cpyTempResultCorrelationGSIDGroupeStations.Count; u++)
                {
                    var temp = cpyTempResultCorrelationGSIDGroupeStations[u];
                    if (((contextStations.Find(x => x.Id == temp.ClientContextStation.Id)) != null) && ((driveTestsResult.Find(x => x.DriveTestId == temp.DriveTestsResult.DriveTestId)) == null))
                    {
                        var copy = temp;
                        copy.ClientContextStation = null;
                        copy.DriveTestsResult.LinkToStationMonitoringId = 0;
                        driveTestsResult.Add(copy.DriveTestsResult);
                        resultCorrelationGSIDGroupeStations.Add(copy);
                    }
                    else if (((contextStations.Find(x => x.Id == temp.ClientContextStation.Id)) == null) && ((driveTestsResult.Find(x => x.DriveTestId == temp.DriveTestsResult.DriveTestId)) != null))
                    {
                        var copy = temp;
                        copy.DriveTestsResult = null;
                        contextStations.Add(copy.ClientContextStation);
                        resultCorrelationGSIDGroupeStations.Add(copy);
                    }
                }
            }
            return resultCorrelationGSIDGroupeStations;
        }
    }
}
