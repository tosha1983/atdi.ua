using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
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


        private readonly IObjectPoolSite _poolSite;
        private readonly ITransformation _transformation;
        private readonly IEarthGeometricService _earthGeometricService;
        private readonly AppServerComponentConfig _appServerComponentConfig;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public DetermineStationParametersCalcIteration(
            IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
            IEarthGeometricService earthGeometricService,
            IIterationsPool iterationsPool,
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            ITransformation transformation,
            ILogger logger)
        {
            _calcServerDataLayer = calcServerDataLayer;
            _iterationsPool = iterationsPool;
            _earthGeometricService = earthGeometricService;
            _poolSite = poolSite;
            _appServerComponentConfig = appServerComponentConfig;
            _transformation = transformation;
            _calcPointArrayPool = _poolSite.GetPool<PointFS[]>(ObjectPools.StationCalibrationPointFSArrayObjectPool);
            _calcListDriveTestsResultPool = _poolSite.GetPool<DriveTestsResult[][]>(ObjectPools.StationCalibrationListDriveTestsResultObjectPool);
            _calibrationResultPool = _poolSite.GetPool<CalibrationResult[]>(ObjectPools.StationCalibrationResultObjectPool);
            _logger = logger;
        }

        /// <summary>
        ///  4.2.2. Расчет корреляции weake (схема бл 2)
        ///  4.2.3  Подбор параметров станции по результатам Drive Test (hard) (схема бл 3)
        /// </summary>
        /// <param name="arrDriveTests">Входной набор драйв тестов</param>
        /// <param name="taskContext">контекст</param>
        /// <param name="station">Входной набор станций</param>
        /// <param name="data">Вспомогательные параметры</param>
        /// <param name="outContextStations">Итоговый набор станций</param>
        /// <param name="outDriveTestsResults">Итоговый набор драйв тестов</param>
        /// <param name="statusCorellationLinkGroup">Флаг указаывающий на то, что расчет корреляции weake прошел успешно</param>
        /// <param name="maxCorellation_pc">Рассчитанное значение корреляции</param>
        /// <returns>Список результатов List<ResultCorrelationGSIDGroupeStations></returns>
        public List<ResultCorrelationGSIDGroupeStations> CalcStationCalibration(DriveTestsResult[] arrDriveTests, ITaskContext taskContext, ContextStation[] station, AllStationCorellationCalcData data, ref List<ContextStation[]> outContextStations, ref List<DriveTestsResult[]> outDriveTestsResults, out bool statusCorellationLinkGroup, out double? maxCorellation_pc)
        {
            var tempResultCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();
            var lstMaxCorellation_pc = new List<double>();
            //обнуляем значение maxCorellation_pc
            maxCorellation_pc = null;
            // переводим статус statusCorellationLinkGroup в false
            statusCorellationLinkGroup = false;
            // цикл по входному массиву драйв тестов
            for (int j = 0; j < arrDriveTests.Length; j++)
            {
                // расчет корреляции
                statusCorellationLinkGroup = CalcCorellation(taskContext, station, arrDriveTests[j], data, out maxCorellation_pc);
                // если расчет корреляции прошел успешно, тогда флаг statusCorellationLinkGroup изменяем в состояние true и выходим из цикла обработки драйв тестов
                if (statusCorellationLinkGroup)
                {
                    lstMaxCorellation_pc.Add(maxCorellation_pc.Value);
                    break;
                }
            }
            // если расчет корреляции не прошел, тогда 
            if (statusCorellationLinkGroup == false)
            {
                // в итоговый массив станций добавляем входной массив станций для возможной дальнейшей обработки
                AppendContextStations(outContextStations, station);
                // в итоговый массив драйв тестов добавляем входной массив драйв тестов для возможной дальнейшей обработки
                AppendDriveTest(outDriveTestsResults, arrDriveTests);
            }
            // если расчет корреляции прошел успешно, тогда 
            else
            {
                if (arrDriveTests != null)
                {
                    // цикл по входному массиву драйв тестов
                    for (int w = 0; w < arrDriveTests.Length; w++)
                    {
                        // получаем очередной драйв тест из массива
                        var currentDriveTest = arrDriveTests[w];
                        // Подбор параметров станции по результатам Drive Test
                        var сalibrationStationsGSIDGroupeStations = CalibrationStations(taskContext, station, currentDriveTest, data);
                        // если результат выполнения метода CalibrationStations не пустой, тогда добавляем его в список tempResultCorrelationGSIDGroupeStations
                        if (сalibrationStationsGSIDGroupeStations.Length > 0)
                        {
                            tempResultCorrelationGSIDGroupeStations.AddRange(сalibrationStationsGSIDGroupeStations);
                        }
                    }
                }
            }
            if (lstMaxCorellation_pc.Count>0)
            {
                lstMaxCorellation_pc.Sort();
                maxCorellation_pc = lstMaxCorellation_pc[0];
            }
            return tempResultCorrelationGSIDGroupeStations;
        }


        /// <summary>
        /// Минимальное расстояние к drive point для заданного в файле конфигурации стандарта
        /// </summary>
        /// <param name="standard">Стандарт</param>
        /// <returns>Минимальное расстояние к drive point</returns>
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

        /// <summary>
        /// Основной вычислительный метод итерации
        /// </summary>
        /// <param name="taskContext">Контекст задачи</param>
        /// <param name="data">Набор вспомогательных параметров</param>
        /// <returns>массив CalibrationResult[]</returns>
        public CalibrationResult[] Run(ITaskContext taskContext, AllStationCorellationCalcData data)
        {
            // если нет ни одной группы драйв тестов с набором значений, тогда генерируем ошибку
            if (data.GSIDGroupeDriveTests.Length==0)
            {
                throw new Exception("The count of drive tests is 0!");
            }
            // если нет ни одной станции с набором значений, тогда генерируем ошибку
            if (data.GSIDGroupeStation.Length == 0)
            {
                throw new Exception("The count of stations is 0!");
            }

            try
            {
                // переменная для хранения процента выполнения задачи
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
                    // извлекаем из пула доступный элемент типа DriveTestsResult[][]
                    listDriveTestsResultBuffer = _calcListDriveTestsResultPool.Take();
                    // разбиение входного массива GSIDGroupeDriveTest на группы драйв тестов outListDriveTestsResult
                    Utils.CompareDriveTestsWithoutStandards(data.GSIDGroupeDriveTests, listDriveTestsResultBuffer, out countRecordsListDriveTestsResultBuffer);
                    // предварительная подготовка данных
                    data.GSIDGroupeDriveTests = Utils.PrepareData(data, ref listDriveTestsResultBuffer, countRecordsListDriveTestsResultBuffer, this._calcPointArrayPool);
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
               
                // если набор стандартов пустой - генерируем ошибку
                if (allStandards.Count == 0)
                {
                    throw new Exception("The count of standards is 0!");
                }

                // создаем список неповторяющихся значений стандартов
                var arrStandards = allStandards.Distinct().ToArray();

                // выполняем фильтрацию перечня драйв тестов по перечню уникальных стандартов, полученных с массива станций
                var selectDriveTestsByStandards = new List<DriveTestsResult>();
                if (data.GSIDGroupeDriveTests != null)
                {
                    for (int c = 0; c < arrStandards.Length; c++)
                    {
                        var listDriveTests = data.GSIDGroupeDriveTests.ToList();
                        var fndDrivetTests = listDriveTests.FindAll(x => x.Standard == arrStandards[c].GetStandardForDriveTest());
                        if (fndDrivetTests != null)
                        {
                            selectDriveTestsByStandards.AddRange(fndDrivetTests);
                        }
                    }
                }
                data.GSIDGroupeDriveTests = selectDriveTestsByStandards.ToArray();

                // массив с результатами обработки
                var listCalcCorellationResult = new CalibrationResult[arrStandards.Length];

                // если нет ни одной группы драйв тестов с набором значений, тогда генерируем ошибку
                if (data.GSIDGroupeDriveTests.Length == 0)
                {
                    throw new Exception("The count of drive tests is 0!");
                }
                // если нет ни одной станции с набором значений, тогда генерируем ошибку
                if (data.GSIDGroupeStation.Length == 0)
                {
                    throw new Exception("The count of stations is 0!");
                }

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
                    var linkDriveTestsAndStations = Utils.CompareDriveTestAndStation(data.GSIDGroupeDriveTests, data.GSIDGroupeStation, standard, out DriveTestsResult[][] outDriveTestsResults, out ContextStation[][] outContextStations, _transformation, _earthGeometricService, 100, data.Projection);

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
                            for (int g = 0; g < listStations.Count; )
                            {
                                if (listStations[g].Type == ClientContextStationType.P)
                                {
                                    listStations.RemoveAt(g);
                                    g = 0;
                                    continue;
                                }
                                g++;
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
                        ///  4.2.3  Подбор параметров станции по результатам Drive Test (hard) (схема бл 3)
                        var tempResultCorrelationGSIDGroupeStations = CalcStationCalibration(driveTest, taskContext, station, data, ref outListContextStations, ref outListDriveTestsResults, out bool statusCorellationLinkGroup, out double? maxCorellation_pc);
                        var contextStations = new List<ContextStation>();
                        var driveTestsResult = new List<DriveTestsResult>();
                        // По максимуму значения Correlation_pc из п.1 привязываем станции и драйв тесты.
                        var resultCorrelationGSIDGroupeStations = LinkedStationsAndDriveTests(tempResultCorrelationGSIDGroupeStations, out contextStations, out driveTestsResult);

                        if (resultCorrelationGSIDGroupeStations.Count > 0)
                        {

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
                            // если результат пустой, тогда станции и драйв тесты возвращаем в итоговые массивы для дальнейшей обработки
                            else
                            {
                                if ((contextStations != null) && (contextStations.Count > 0))
                                {
                                    outListContextStations.Add(contextStations.ToArray());
                                    outListDriveTestsResults.Add(driveTestsResult.ToArray());
                                }
                            }
                        }
                        
                        // обновление процента выполнения задачи
                        if (linkDriveTestsAndStations.Length > 0)
                        {
                            percentComplete = (z + 1) * (int)(50.0 / linkDriveTestsAndStations.Length);
                            UpdatePercentComplete(data.resultId, percentComplete);
                        }
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///                     БЛОК
                    ///         "НДП, изменение GSID, новые РЕЗ"
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///     4.2.5. Ранжирование драйв тестов с GSID (по количеству данных и их уровню)
                    ///     Внутри GSIDGroupeDriveTests данные уже ранжированы в данном блоки необходимо ранжировать сам массив (или список) массивов драйв тестов. 
                    ///     Ранжируется на основании количества точек первого DriveTests из GSIDGroupeDriveTests с большим количеством точек идут первые. 
                    ///     При равном количестве точек первым будет идти тот у кого средний уровень по всем точкам выше. 
                    ///     На входи и выходе одинаковое количество элементов у массивов.Ничего не удаляется и не переноситься.
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
                    ////////////////////////// Асоциирование станций когда не совпадают GSID
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    
                    // список для хранения сведений о драйв тестах со статусом UN
                    var groupsDriveTestsResult = new List<GroupsDriveTestsResult>();
                    // список для хранения сведений о станциях со статусом UN
                    var groupsContextStations = new List<GroupsContextStations>();

                    for (int i = 0; i < outListDriveTestsResults.Count; i++)
                    {
                        var GSIDGroupeStations = new List<ContextStation[]>();
                        var arrDriveTests = outListDriveTestsResults[i];
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///     4.2.6. Ранжирование Stations под данный Drive Test
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////
                        if (arrDriveTests != null)
                        {GSIDGroupeStations = GetOrderStationForDriveTests(ref arrDriveTests, ref outListContextStations, standard); }
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///    4.2.7 Подбор параметров станции по результатам Drive Test (hard) (схема бл 7)
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
                                ///  Расчет корреляции weake (схема бл 2)
                                ///  Подбор параметров станции по результатам Drive Test (hard) (схема бл 3)
                                var tempResultCorrelationGSIDGroupeStations = CalcStationCalibration(arrDriveTests, taskContext, station, data, ref outListContextStations, ref outListDriveTestsResults, out bool statusCorellationLinkGroup, out double? maxCorellation_pc);
                                bool isСorrelationThresholdHard = false;
                                if (tempResultCorrelationGSIDGroupeStations.Count > 0)
                                {
                                    for (int a = 0; a < tempResultCorrelationGSIDGroupeStations.Count; a++)
                                    {
                                        if (tempResultCorrelationGSIDGroupeStations[a].MaxCorrelation_PC > data.GeneralParameters.СorrelationThresholdHard)
                                        {
                                            isСorrelationThresholdHard = true;
                                            break;
                                        }
                                    }
                                }
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                ///    4.2.8. Сохранение соответствия, изымание станций и Drive Test из дальнейших расчетов (схема бл 8)
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////
                                // Производим последовательную обработку массива (ResultCorrelationGSIDGroupeStations) запись за записью.
                                // в случае, если флаги Drive Test (hard) и Drive Test (weak) имеют значения true
                                if ((statusCorellationLinkGroup) && (isСorrelationThresholdHard))
                                {
                                    // По максимуму значения Correlation_pc из п.1 привязываем станции и драйв тесты.
                                    resultCorrelationGSIDGroupeStations = LinkedStationsAndDriveTests(tempResultCorrelationGSIDGroupeStations, out contextStations, out driveTestsResult);
                                    //Определение статусов 
                                    var calibrationStationsAndDriveTestsResult = FillCalibrationStationResultSecondBlock(resultCorrelationGSIDGroupeStations, data.CalibrationParameters, data.CorellationParameters, data.GeneralParameters);
                                    if (calibrationStationsAndDriveTestsResult != null)
                                    {
                                        if ((calibrationStationsAndDriveTestsResult.ResultCalibrationStation.Length > 0) || (calibrationStationsAndDriveTestsResult.ResultCalibrationDriveTest.Length > 0))
                                        {
                                            calibrationStationsAndDriveTestsResultByGroup.Add(calibrationStationsAndDriveTestsResult); // добавляем промежуточный результат в массив результатов
                                            CompressedStationsAndDriveTest(ref outListContextStations, ref outListDriveTestsResults, resultCorrelationGSIDGroupeStations); // убираем из общего списка станций и  драйв тестов  те которые попали в результаты
                                            RemoveFromGroupsContextStations(ref groupsContextStations, station); // удаление из списка со сведениями о станциях со статусом UN массива station
                                            RemoveFromGroupsDriveTestsResult(ref groupsDriveTestsResult, arrDriveTests); // удаление из списка со сведениями о драйв тестах со статусом UN массива arrDriveTests
                                        }
                                    }
                                    break;
                                }
                                ///   в случае, если флаг Drive Test (hard) равен false,  Drive Test (weak) имеют значение true
                                else if ((isСorrelationThresholdHard==false) && (statusCorellationLinkGroup == true))
                                {
                                    AppendGroupsDriveTestsResult(ref groupsDriveTestsResult, arrDriveTests, maxCorellation_pc);
                                    AppendGroupsContextStations(ref groupsContextStations, station, maxCorellation_pc);
                                }
                            }
                        }

                        //обновление процента времени
                        if (outListDriveTestsResults.Count > 0)
                        {
                            percentComplete = 50 + ((i + 1) * (int)(50.0 / outListDriveTestsResults.Count));
                            UpdatePercentComplete(data.resultId, percentComplete);
                        }
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///   4.2.11. Все не изъятые Stations помечаются как необнаруженные (схема бл. 10)
                    ///   Все драйв тесты для которых не найдена пара станций и которые не помечены как UN выставляется статус IT
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    for (int dw = 0; dw < outListDriveTestsResults.Count; dw++)
                    {
                        // извлекаем очередную группу драйв тестов
                        var arrDriveTests = outListDriveTestsResults[dw];

                        if ((arrDriveTests != null) && (arrDriveTests.Length > 0))
                        {

                            // извлекаем драйв тест
                            var currentDriveTest = arrDriveTests[0];
                            // обнуляем значение коэффициента корреляции
                            double? maxPercentCorellation = 0;
                            // по умолчанию устанавливаем значение статуса для драйв теста в IT и уточняем:
                            var driveTestStatusResult = DriveTestStatusResult.IT;
                            // если в списке драйв тестов groupsDriveTestsResult есть драйв тест с таким же идентификатором, тогда извлекаем его статус
                            var fndDriveTest = groupsDriveTestsResult.Find(x => x.DriveTestsResult.DriveTestId == currentDriveTest.DriveTestId);
                            if (fndDriveTest != null)
                            {
                                driveTestStatusResult = fndDriveTest.DriveTestStatusResult;
                                maxPercentCorellation = fndDriveTest.MaxCorellation_pc;
                            }
                            // если в списке драйв тестов groupsDriveTestsResult нет драйв тест с таким же идентификатором, тогда поиск выполняем в перечне станций со статусом P
                            else
                            {
                                // передаем в метод RecaclNDP драйв тест currentDriveTest для поиска среди станций со статусом P (outListContextStationsForStatusP) таких для которых процент корреляции maxPercentCorellation будет выше 0
                                // в данном случае для такого драйв теста currentDriveTest извлекаем новый статус (обновляем статус IT на  UN в случае если maxPercentCorellation > 0)
                                RecaclNDP(outListContextStationsForStatusP, taskContext, arrDriveTests, data, out maxPercentCorellation, out driveTestStatusResult);
                            }

                            var tempCalibrationDriveTestResult = new CalibrationDriveTestResult[arrDriveTests.Length];
                            for (int p=0; p< arrDriveTests.Length; p++)
                            {
                                tempCalibrationDriveTestResult[p] = new CalibrationDriveTestResult()
                                {
                                    CountPointsInDriveTest = arrDriveTests[p].Points.Length,
                                    DriveTestId = arrDriveTests[p].DriveTestId,
                                    Gsid = arrDriveTests[p].GSID,
                                    LinkToStationMonitoringId = arrDriveTests[p].LinkToStationMonitoringId,
                                    ResultDriveTestStatus = driveTestStatusResult,
                                    Standard = arrDriveTests[p].RealStandard,
                                    Freq_MHz = (float)arrDriveTests[p].Freq_MHz,
                                    MaxPercentCorellation = (float)maxPercentCorellation.Value
                                };
                            }

                            // формируем результат по драйв тесту currentDriveTest со статусом, который дважды уточнялся - по списку groupsDriveTestsResult (который содержит сведения по драйв тестах со статусами UN) и в методе RecaclNDP поиска по станциям со статусом P
                            calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                            {
                                ResultCalibrationDriveTest = tempCalibrationDriveTestResult,
                                ResultCalibrationStation = null
                            });
                        }
                    }

                    var lstCalibrationStationResult = new List<CalibrationStationResult>();
                    for (int j = 0; j < outListContextStations.Count; j++)
                    {
                        // извлекаем очередную группу драйв станций
                        var arrStations = outListContextStations[j];
                        for (int d = 0; d < arrStations.Length; d++)
                        {
                            if (arrStations[d] != null)
                            {
                                // обнуляем значение коэффициента корреляции
                                double? maxPercentCorellation = 0;
                                // по умолчанию устанавливаем значение статуса для станции в NF и уточняем:
                                var stationStatusResult = StationStatusResult.NF;
                                // если в списке станций groupsContextStations есть станция с таким же идентификатором, тогда извлекаем ее статус и процент корреляции
                                var fndContextStation = groupsContextStations.Find(x => x.ContextStations.Id == arrStations[d].Id);
                                if (fndContextStation != null)
                                {
                                    stationStatusResult = fndContextStation.StationStatusResult;
                                    maxPercentCorellation = fndContextStation.MaxCorellation_pc;
                                }

                                // формируем результат по станциям arrStations со статусом, который  уточнялся - по списку groupsContextStations (который содержит сведения по станциям со статусами UN) 

                                var parametersStationOld = new ParametersStation();
                                if (arrStations[d].Antenna != null)
                                {
                                    parametersStationOld.Azimuth_deg = arrStations[d].Antenna.Azimuth_deg;
                                    parametersStationOld.Tilt_Deg = arrStations[d].Antenna.Tilt_deg;
                                }
                                parametersStationOld.Lat_deg = arrStations[d].Site.Latitude;
                                parametersStationOld.Altitude_m = (int)arrStations[d].Site.Altitude;
                                parametersStationOld.Lon_deg = arrStations[d].Site.Longitude;

                                if (arrStations[d].Transmitter != null)
                                {
                                    parametersStationOld.Power_dB = arrStations[d].Transmitter.MaxPower_dBm;
                                    parametersStationOld.Freq_MHz = arrStations[d].Transmitter.Freq_MHz;
                                }


                                lstCalibrationStationResult.Add(new CalibrationStationResult()
                                {
                                    ExternalSource = arrStations[d].ExternalSource,
                                    ExternalCode = arrStations[d].ExternalCode,
                                    LicenseGsid = arrStations[d].LicenseGsid,
                                    ResultStationStatus = stationStatusResult,
                                    StationMonitoringId = arrStations[d].Id,
                                    IsContour = arrStations[d].Type == ClientContextStationType.A ? true : false,
                                    ParametersStationOld = parametersStationOld,
                                    Standard = arrStations[d].RealStandard,
                                    MaxCorellation = (float)maxPercentCorellation.Value
                                });
                            }
                        }
                    }

                    // формируем результат с массивом драйв тестов CalibrationDriveTestResult[] = null
                    if (lstCalibrationStationResult.Count > 0)
                    {
                        calibrationStationsAndDriveTestsResultByGroup.Add(new CalibrationStationsAndDriveTestsResult()
                        {
                            ResultCalibrationDriveTest = null, 
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
                                    if ((listCalibrationDriveTestResult.Find(n => n.DriveTestId == driveTests[z].DriveTestId)) == null)
                                    {
                                        listCalibrationDriveTestResult.Add(driveTests[z]);
                                    }
                                }
                            }
                        }
                        if (stations != null)
                        {
                            for (int z = 0; z < stations.Length; z++)
                            {
                                if (stations[z] != null)
                                {
                                    if ((listCalibrationStationResult.Find(n => n.ExternalSource == stations[z].ExternalSource && n.ExternalCode == stations[z].ExternalCode)) == null)
                                    {
                                        listCalibrationStationResult.Add(stations[z]);
                                    }
                                }
                            }
                        }
                    }


                    // извлекаем неповторяющиеся значения регионов
                    //var areasSelect = data.GSIDGroupeStation.ToList().Select(x => x.RegionCode).Distinct();

                    // формируем их через запятую
                    //string areas = string.Join(",", areasSelect.ToArray());
                    string areas = data.Areas!=null ? data.Areas : "";
                    // генерация итогового результата
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
                        NumberStationInContour = listCalibrationStationResult.FindAll(x => x.IsContour == true).Count(), // общее число станций, которые имеют статус А (попадают в контур)
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
            return null;
        }


        /// <summary>
        /// 4.2.10. Оценка Drive Test на предмет НДП (схема бл. 9)
        /// </summary>
        /// <param name="outListContextStationsForStatusP">Массив групп станций со статусами P</param>
        /// <param name="taskContext">Контекст таска</param>
        /// <param name="currentDriveTestP">драйв тест, который будет сравниваться с групппой станций outListContextStationsForStatusP</param>
        /// <param name="data">Набор вспомогательных параметров</param>
        /// <param name="maxCorellationP_pc">Максимальное значение процента корреляции</param>
        /// <param name="driveTestStatusResult">итоговый статус драйв теста currentDriveTestP </param>
        private void RecaclNDP(ContextStation[][] outListContextStationsForStatusP, ITaskContext taskContext, DriveTestsResult[] driveTestsP, AllStationCorellationCalcData data, out double? maxCorellationP_pc, out DriveTestStatusResult driveTestStatusResult)
        {
            maxCorellationP_pc = 0;
            driveTestStatusResult = DriveTestStatusResult.IT;
            //обработка станций со статусом "P"
            if ((outListContextStationsForStatusP != null) && (outListContextStationsForStatusP.Length > 0))
            {
                var lstStationsForStatusP = outListContextStationsForStatusP.ToList();
                for (int mx = 0; mx < lstStationsForStatusP.Count; mx++)
                {
                    var stationP = lstStationsForStatusP[mx];

                    if ((stationP != null && stationP.Length > 0))
                    {
                        for (int w = 0; w < driveTestsP.Length; w++)
                        {
                            // расчет корелляции
                            bool statusCorellationLinkGroupP = CalcCorellation(taskContext, stationP, driveTestsP[w], data, out maxCorellationP_pc);
                            if (statusCorellationLinkGroupP == true)
                            {
                                driveTestStatusResult = DriveTestStatusResult.UN;
                                break;
                            }
                        }
                        if (driveTestStatusResult== DriveTestStatusResult.UN)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обновить процент выполнения задачи
        /// </summary>
        /// <param name="resultId">Идентификатор результата</param>
        /// <param name="percentComplete">Значение процента выполнения задачи</param>
        private void UpdatePercentComplete(long resultId, int percentComplete)
        {
            var calcDbScope = this._calcServerDataLayer.CreateScope<CalcServerDataContext>();
            var updateQueryStationCalibrationResult = _calcServerDataLayer.GetBuilder<IStationCalibrationResult>()
                          .Update()
                          .SetValue(c => c.PercentComplete, percentComplete)
                          .Where(c => c.Id, ConditionOperator.Equal, resultId);
            calcDbScope.Executor.Execute(updateQueryStationCalibrationResult);
            calcDbScope.Dispose();
            calcDbScope = null;
        }

        /// <summary>
        /// Расчет корреляции weake (схема бл 2)
        /// </summary>
        /// <param name="taskContext">Контекст задачи</param>
        /// <param name="stations">Набор станций</param>
        /// <param name="driveTest">Драйв тест</param>
        /// <param name="data">Набор вспомогательных параметров</param>
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
        /// <param name="taskContext">Контест таска</param>
        /// <param name="stations">Набор станций</param>
        /// <param name="driveTest">Драйв тест</param>
        /// <param name="data">Набор вспомогательных параметров</param>
        /// <returns></returns>
        public ResultCorrelationGSIDGroupeStations[] CalibrationStations(ITaskContext taskContext, ContextStation[] stations, DriveTestsResult driveTest, AllStationCorellationCalcData data)
        {
            var tempListCorrelationGSIDGroupeStations = new List<ResultCorrelationGSIDGroupeStations>();
            for (int i = 0; i < stations.Length; i++)
            {
                // при условии совпадения частот
                var freq_MHz = GetFreqStation(driveTest, stations[i]);
                if (freq_MHz != null)
                {
                //if (stations[i].Transmitter.Freqs_MHz.Contains(driveTest.Freq_MHz))
                //{
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
                    if ((!double.IsNaN(resultCalibrationCalcData.Corellation_factor)) && (!float.IsNaN(resultCalibrationCalcData.ParametersStationNew.Power_dB)) && (!float.IsNaN(resultCalibrationCalcData.ParametersStationOld.Power_dB)))
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
        /// <param name="parametersStationOld">Пред. набор параметров станции</param>
        /// <param name="parametersStationNew">Новый набор параметров станции</param>
        /// <param name="calibrationParameters">Набор ограничений</param>
        /// <returns></returns>
        private bool CheckParameterStation(ParametersStation parametersStationOld, ParametersStation parametersStationNew, CalibrationParameters calibrationParameters)
        {
            bool isNotCorrect = false;
            if ((parametersStationOld !=null) && (parametersStationNew != null))
            {
                if (calibrationParameters.AltitudeStation)
                {
                    if ((Math.Abs(parametersStationOld.Altitude_m - parametersStationNew.Altitude_m) <= calibrationParameters.MaxDeviationAltitudeStation_m)==false)
                    {
                        isNotCorrect = true;
                    }
                }
                if (calibrationParameters.AzimuthStation)
                {
                    if ((Math.Abs(parametersStationOld.Azimuth_deg - parametersStationNew.Azimuth_deg) <= calibrationParameters.MaxDeviationAzimuthStation_deg)==false)
                    {
                        isNotCorrect = true;
                    }
                }
                if (calibrationParameters.CoordinatesStation)
                {
                    if ((Math.Abs(parametersStationOld.Lat_deg - parametersStationNew.Lat_deg) <= calibrationParameters.MaxDeviationCoordinatesStation_m)==false)
                    {
                        isNotCorrect = true;
                    }
                    if ((Math.Abs(parametersStationOld.Lon_deg - parametersStationNew.Lon_deg) <= calibrationParameters.MaxDeviationCoordinatesStation_m)==false)
                    {
                        isNotCorrect = true;
                    }
                }
                if (calibrationParameters.PowerStation)
                {
                    if ((Math.Abs(parametersStationOld.Power_dB - parametersStationNew.Power_dB) <= calibrationParameters.ShiftPowerStationStep_dB)==false) // ??????????????????????????
                    {
                        isNotCorrect = true;
                    }
                }
                if (calibrationParameters.TiltStation)
                {
                    if ((Math.Abs(parametersStationOld.Tilt_Deg - parametersStationNew.Tilt_Deg) <= calibrationParameters.MaxDeviationTiltStationDeg)==false)
                    {
                        isNotCorrect = true;
                    }
                }
            }
            return isNotCorrect;
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
                            Freq_MHz = calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                        };

                        var freq_MHz = GetFreqStation(calibrationStationsAndDriveTestsResult.DriveTestsResult, calibrationStationsAndDriveTestsResult.ClientContextStation);
                        if (freq_MHz!=null)
                        {
                            calibrationStationResult.ParametersStationOld.Freq_MHz = freq_MHz.Value;
                        }
                        else
                        {
                            calibrationStationResult.ParametersStationOld.Freq_MHz = calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz;
                        }

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
                if (calibrationStationsAndDriveTestsResult.ClientContextStation != null)
                {
                    if (calibrationStationsAndDriveTestsResult.ClientContextStation.Type == ClientContextStationType.P)
                    {
                        continue;
                    }
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
                            Freq_MHz = calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz
                        };

                        var freq_MHz = GetFreqStation(calibrationStationsAndDriveTestsResult.DriveTestsResult, calibrationStationsAndDriveTestsResult.ClientContextStation);
                        if (freq_MHz != null)
                        {
                            calibrationStationResult.ParametersStationOld.Freq_MHz = freq_MHz.Value;
                        }
                        else
                        {
                            calibrationStationResult.ParametersStationOld.Freq_MHz = calibrationStationsAndDriveTestsResult.DriveTestsResult.Freq_MHz;
                        }

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
        /// Обновление итогового списка станций outListContextStation массивом станций contextStation с проверкой:
        /// если станции contextStation есть в outListContextStation, тогда не добавляем их 
        /// </summary>
        /// <param name="outListContextStation">итоговый список станций outListContextStation</param>
        /// <param name="contextStation">массив станций contextStation</param>
        public void AppendContextStations(List<ContextStation[]> outListContextStation, ContextStation[] contextStation)
        {
            if (outListContextStation == null)
            {
                outListContextStation.Add(contextStation);
            }
            else if ((outListContextStation != null) && (outListContextStation.Count == 0))
            {
                outListContextStation.Add(contextStation);
            }
            else
            {
                bool isFind = false;
                for (int d = 0; d < outListContextStation.Count; d++)
                {
                    var lstStations = outListContextStation[d].ToList();
                    for (int s = 0; s < contextStation.Length; s++)
                    {
                        if ((lstStations.Find(x => x.Id == contextStation[s].Id)) != null)
                        {
                            isFind = true;
                            break;
                            //lstStations.Add(contextStation[s]);
                        }
                    }
                    //outListContextStation[d] = lstStations.ToArray();
                }
                if (!isFind)
                {
                    outListContextStation.Add(contextStation);
                }
            }
        }

        /// <summary>
        ///  Обновление итогового списка драйв тестов outListDriveTestsResults массивом драйв тестов driveTestsResults с проверкой:
        /// если драйв тесты driveTestsResults есть в outListDriveTestsResults, тогда не добавляем их
        /// </summary>
        /// <param name="outListDriveTestsResults">итоговый список драйв тестов outListDriveTestsResults</param>
        /// <param name="driveTestsResults">массив драйв тестов</param>
        public void AppendDriveTest(List<DriveTestsResult[]> outListDriveTestsResults, DriveTestsResult[] driveTestsResults)
        {
            if (outListDriveTestsResults == null)
            {
                outListDriveTestsResults.Add(driveTestsResults);
            }
            else if ((outListDriveTestsResults != null) && (outListDriveTestsResults.Count == 0))
            {
                outListDriveTestsResults.Add(driveTestsResults);
            }
            else
            {
                bool isFind = false;
                for (int d = 0; d < outListDriveTestsResults.Count; d++)
                {
                    var lstdriveTestsResults = outListDriveTestsResults[d].ToList();
                    for (int s = 0; s < driveTestsResults.Length; s++)
                    {
                        if ((lstdriveTestsResults.Find(x => x.DriveTestId == driveTestsResults[s].DriveTestId)) != null)
                        {
                            isFind = true;
                            break;
                            //lstdriveTestsResults.Add(driveTestsResults[s]);
                        }
                    }
                }
                if (!isFind)
                {
                    outListDriveTestsResults.Add(driveTestsResults);
                }
            }
        }

        /// <summary>
        /// Удаление массива драйв тестов groupsDriveTestsResults из списка outGroupsDriveTestsResult (метод вызывается только в случае, когда для группы драйв тестов groupsDriveTestsResults были найдены подходящие станции)
        /// </summary>
        /// <param name="outGroupsDriveTestsResult">Обновленный список драйв тестов groupsDriveTestsResults со статусами UN</param>
        /// <param name="groupsDriveTestsResults">входящий массив драйв тестов</param>
        public void RemoveFromGroupsDriveTestsResult(ref List<GroupsDriveTestsResult> outGroupsDriveTestsResult, DriveTestsResult[] groupsDriveTestsResults)
        {
            if ((outGroupsDriveTestsResult != null) && (groupsDriveTestsResults != null))
            {
                var lstDriveTestsResults = outGroupsDriveTestsResult;
                for (int s = 0; s < groupsDriveTestsResults.Length; s++)
                {
                    if (lstDriveTestsResults.Find(x => x.DriveTestsResult.DriveTestId == groupsDriveTestsResults[s].DriveTestId) != null)
                    {
                        lstDriveTestsResults.RemoveAll(x => x.DriveTestsResult.DriveTestId == groupsDriveTestsResults[s].DriveTestId);
                    }
                }
                outGroupsDriveTestsResult = lstDriveTestsResults;
            }
        }


        /// <summary>
        /// Обновление списка драйв тестов outGroupsDriveTestsResult новыми набором groupsDriveTestsResults (метод вызывается в случае когда для входящего набора драйв тестов groupsDriveTestsResults сработал только флаг Drive Test weak)
        /// </summary>
        /// <param name="outGroupsDriveTestsResult">Обновленный список драйв тестов outGroupsDriveTestsResult со статусами UN</param>
        /// <param name="groupsDriveTestsResults">входящий массив драйв тестов</param>
        /// <param name="maxCorellation_pc">Значение процента корреляции</param>
        public void AppendGroupsDriveTestsResult(ref List<GroupsDriveTestsResult> outGroupsDriveTestsResult, DriveTestsResult[]  groupsDriveTestsResults, double? maxCorellation_pc)
        {
            if (outGroupsDriveTestsResult == null)
            {
                for (int w = 0; w < groupsDriveTestsResults.Length; w++)
                {
                    outGroupsDriveTestsResult.Add(new GroupsDriveTestsResult()
                    {
                         DriveTestsResult = groupsDriveTestsResults[w],
                         MaxCorellation_pc = maxCorellation_pc,
                         DriveTestStatusResult = DriveTestStatusResult.UN 
                    });
                }
            }
            else if ((outGroupsDriveTestsResult != null) && (outGroupsDriveTestsResult.Count == 0))
            {
                for (int w = 0; w < groupsDriveTestsResults.Length; w++)
                {
                    outGroupsDriveTestsResult.Add(new GroupsDriveTestsResult()
                    {
                         DriveTestsResult = groupsDriveTestsResults[w],
                         MaxCorellation_pc = maxCorellation_pc,
                         DriveTestStatusResult = DriveTestStatusResult.UN
                    });
                }
            }
            else
            {
                for (int s = 0; s < groupsDriveTestsResults.Length; s++)
                {
                    var fndGroupsDriveTestsResult = outGroupsDriveTestsResult.Find(x => x.DriveTestsResult.DriveTestId == groupsDriveTestsResults[s].DriveTestId);
                    if (fndGroupsDriveTestsResult==null)
                    {
                        outGroupsDriveTestsResult.Add(new GroupsDriveTestsResult()
                        {
                            DriveTestsResult = groupsDriveTestsResults[s],
                            MaxCorellation_pc = maxCorellation_pc,
                            DriveTestStatusResult = DriveTestStatusResult.UN
                        });
                    }
                    else
                    {
                        if (maxCorellation_pc > fndGroupsDriveTestsResult.MaxCorellation_pc)
                        {
                            fndGroupsDriveTestsResult.MaxCorellation_pc = maxCorellation_pc;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обновление списка станций outGroupsContextStations новыми набором contextStations (метод вызывается в случае когда для входящего набора станций contextStations сработал только флаг Drive Test weak)
        /// </summary>
        /// <param name="outGroupsContextStations">Обновленный список станций outGroupsContextStations со статусами UN</param>
        /// <param name="contextStations">входящий массив станций</param>
        /// <param name="maxCorellation_pc">Значение процента корреляции</param>
        public void AppendGroupsContextStations(ref List<GroupsContextStations> outGroupsContextStations, ContextStation[] contextStations, double? maxCorellation_pc)
        {
            if (outGroupsContextStations == null)
            {
                for (int w = 0; w < contextStations.Length; w++)
                {
                    outGroupsContextStations.Add(new GroupsContextStations() {
                        ContextStations = contextStations[w],
                         MaxCorellation_pc = maxCorellation_pc,
                          StationStatusResult = StationStatusResult.UN
                    });
                }
            }
            else if ((outGroupsContextStations != null) && (outGroupsContextStations.Count == 0))
            {
                for (int w = 0; w < contextStations.Length; w++)
                {
                    outGroupsContextStations.Add(new GroupsContextStations()
                    {
                        ContextStations = contextStations[w],
                        MaxCorellation_pc = maxCorellation_pc,
                        StationStatusResult = StationStatusResult.UN
                    });
                }
            }
            else
            {
                for (int s = 0; s < contextStations.Length; s++)
                {
                    var fndContextStations = outGroupsContextStations.Find(x => x.ContextStations.Id == contextStations[s].Id);
                    if (fndContextStations==null)
                    {
                        outGroupsContextStations.Add(new GroupsContextStations()
                        {
                            ContextStations = contextStations[s],
                            MaxCorellation_pc = maxCorellation_pc,
                            StationStatusResult = StationStatusResult.UN
                        });
                    }
                    else
                    {
                        if (maxCorellation_pc> fndContextStations.MaxCorellation_pc)
                        {
                            fndContextStations.MaxCorellation_pc = maxCorellation_pc;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Удаление массива станций contextStations из списка outGroupsContextStations (метод вызывается только в случае, когда для группы станций contextStations были найдены подходящие драйв тесты)
        /// </summary>
        /// <param name="outGroupsContextStations">Обновленный список групп станций со статусами UN</param>
        /// <param name="contextStations">входящий массив станций</param>
        public void RemoveFromGroupsContextStations(ref List<GroupsContextStations> outGroupsContextStations, ContextStation[] contextStations)
        {
            if ((outGroupsContextStations != null) && (contextStations != null))
            {
                var lstContextStation = outGroupsContextStations;
                for (int s = 0; s < contextStations.Length; s++)
                {
                    if (lstContextStation.Find(x => x.ContextStations.Id == contextStations[s].Id) != null)
                    {
                        lstContextStation.RemoveAll(x => x.ContextStations.Id == contextStations[s].Id);
                    }
                }
                outGroupsContextStations = lstContextStation; 
            }
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
            for (int f = 0; f < contextStations.Count; )
            {
                var arrStations = contextStations[f].ToList();
                bool isRemove = false;
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
                                    f = 0;
                                    isRemove = true;
                                    break;
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
                            f = 0;
                            isRemove = true;
                            break;
                        }
                    }
                }
                if (isRemove)
                {
                    continue;
                }
                f++;
            }

            // убираем из общего списка драйв тестов, те которые попали в результаты
            for (int f = 0; f < driveTests.Count; )
            {
                var arrDriveTests = driveTests[f].ToList();
                bool isRemove = false;
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
                                    f = 0;
                                    isRemove = true;
                                    break;
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
                            isRemove = true;
                            f = 0;
                            break;
                        }
                    }
                }
                if (isRemove)
                {
                    continue;
                }
                f++;
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
                for (int f = 0; f < contextStations.Count; )
                {
                    bool isRemove = false;
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
                                        f = 0;
                                        isRemove = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (isRemove)
                    {
                        continue;
                    }
                    f++;
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
        /// <summary>
        /// Отбирает и ранжирует станции для данного блока драйвтестов
        /// </summary>
        /// <param name="arrDriveTests"></param>
        /// <param name="outListContextStations"></param>
        /// <param name="standard"></param>
        /// <returns></returns>
        private List<ContextStation[]> GetOrderStationForDriveTests(ref DriveTestsResult[] arrDriveTests, ref List<ContextStation[]> outListContextStations, string standard)
        {
            var GSIDGroupeStations = new List<ContextStation[]>();
            for (int w = 0; w < arrDriveTests.Length; w++)
            {
                //получаем очередной драйв тест
                var currentDriveTest = arrDriveTests[w];
                if (currentDriveTest != null)
                {
                    var coordinatesDrivePoint = currentDriveTest.Points.Select(x => x.Coordinate).ToArray();
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///     1. Для GSIDGroupeDriveTests мы считаем центр масс всех координат, всех точек по заданному драйв тесту. 
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    var centerWeightCoordinateOfDriveTest = Utils.CenterWeightAllCoordinates(currentDriveTest.Points);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///    2. Формируем новый массив (или список) массивов станций (GSIDGroupeStations) на основании того перемещаем туда все станции
                    ///     из массива (или списка) массивов станций (outListContextStations) если хотябы одна из станций  outListContextStations 
                    ///     имеют координаты ближе чем 1км (параметр вынести в файл конфигурации в зависимости от STANDART) к координатам GSIDGroupeDriveTests.
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    for (int j = 0; j < outListContextStations.Count; j++)
                    {
                        var arrStations = outListContextStations[j];
                        if ((arrStations != null) && (arrStations.Length > 0))
                        {
                            if (CompareFreqStAndDriveTest(currentDriveTest, arrStations[0]))
                            {
                                for (int p = 0; p < coordinatesDrivePoint.Length; p++)
                                {
                                    var sourcePointArgs = new PointEarthGeometric() { Longitude = coordinatesDrivePoint[p].X, Latitude = coordinatesDrivePoint[p].Y, CoordinateUnits = CoordinateUnits.m };
                                    var targetPointArgs = new PointEarthGeometric() { Longitude = arrStations[0].Coordinate.X, Latitude = arrStations[0].Coordinate.Y, CoordinateUnits = CoordinateUnits.m };
                                    if (this._earthGeometricService.GetDistance_km(in sourcePointArgs, in targetPointArgs) <= GetMinDistanceFromConfigByStandard(standard))
                                    {
                                        // добавляем весь массив станций arrStations в случае если одна из станций, которая входит в arrStations имеет расстояние до одной из точек текущего DrivePoint меньше 1 км (берем с конфигурации)
                                        var lstGSID = GSIDGroupeStations;
                                        List<ContextStation> listContextStation = new List<ContextStation>();
                                        for (int d = 0; d < lstGSID.Count; d++)
                                        {
                                            var lstStations = lstGSID[d].ToList();
                                            for (int s = 0; s < arrStations.Length; s++)
                                            {
                                                if ((lstStations.Find(x => x.Id == arrStations[s].Id)) == null)
                                                {
                                                    listContextStation.Add(arrStations[s]);
                                                }
                                            }

                                        }
                                        if (listContextStation.Count > 0)
                                        {
                                            GSIDGroupeStations.Add(listContextStation.ToArray());
                                        }
                                        else
                                        {
                                            if (GSIDGroupeStations.Count == 0)
                                            {
                                                GSIDGroupeStations.Add(arrStations);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///    3. В новый массив (или список) массивов станций (GSIDGroupeStations) проводим ранжирование 
                    ///    GSIDGroupeStations по критерию минимальное расстояние от центра масс GSIDGroupeDriveTests 
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    for (int q = 0; q < GSIDGroupeStations.Count; q++)
                    {
                        var arrStations = GSIDGroupeStations[q];
                        if ((arrStations != null) && (arrStations.Length > 0))
                        {
                            var stationsResults = new ContextStation[arrStations.Length];
                            var keyValueStations = new List<KeyValuePair<long, double>>();
                            for (int z = 0; z < arrStations.Length; z++)
                            {
                                var sourcePointArgs = new PointEarthGeometric() { Longitude = centerWeightCoordinateOfDriveTest.X, Latitude = centerWeightCoordinateOfDriveTest.Y, CoordinateUnits = CoordinateUnits.m };
                                var targetPointArgs = new PointEarthGeometric() { Longitude = arrStations[z].Coordinate.X, Latitude = arrStations[z].Coordinate.Y, CoordinateUnits = CoordinateUnits.m };
                                var distance = this._earthGeometricService.GetDistance_km(in sourcePointArgs, in targetPointArgs);
                                keyValueStations.Add(new KeyValuePair<long, double>(arrStations[z].Id, distance));
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
                    }
                }
            }
            return GSIDGroupeStations;
        }
        private bool CompareFreqStAndDriveTest(DriveTestsResult driveTestsResult, ContextStation contextStation)
        {
            var FreqDT = driveTestsResult.Freq_MHz;
            var FreqST = contextStation.Transmitter.Freq_MHz;
            var FreqArr = contextStation.Transmitter.Freqs_MHz;
            var BW = contextStation.Transmitter.BW_kHz / 1000.0;
            if ((FreqST - BW <= FreqDT) && (FreqST + BW >= FreqDT)) { return true; }
            if ((FreqArr != null) && (FreqArr.Length > 0))
            {
                for (int i = 0; FreqArr.Length > i; i++)
                {
                    if ((FreqArr[i] - BW * 0.49 <= FreqDT) && (FreqArr[i] + BW * 0.49 >= FreqDT)) { return true; }
                }
            }
            return false; 
        }

        private double? GetFreqStation(DriveTestsResult driveTestsResult, ContextStation contextStation)
        {
            var FreqDT = driveTestsResult.Freq_MHz;
            var FreqST = contextStation.Transmitter.Freq_MHz;
            var FreqArr = contextStation.Transmitter.Freqs_MHz;
            var BW = contextStation.Transmitter.BW_kHz / 1000.0;
            if ((FreqST - BW <= FreqDT) && (FreqST + BW >= FreqDT)) { return FreqST; }
            if ((FreqArr != null) && (FreqArr.Length > 0))
            {
                for (int i = 0; FreqArr.Length > i; i++)
                {
                    if ((FreqArr[i] - BW*0.49 <= FreqDT) && (FreqArr[i] + BW * 0.49 >= FreqDT)) { return FreqArr[i]; }
                }
            }
            return null;
        }

    }
}
