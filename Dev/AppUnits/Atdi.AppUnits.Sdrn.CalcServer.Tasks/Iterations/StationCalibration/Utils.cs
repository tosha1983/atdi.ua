using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Platform.Logging;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using Atdi.Platform.Data;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class StringExtension
    {
        static Dictionary<string, string> _dictStandardsForDriveTests = new Dictionary<string, string>
        {
            {"GSM-900", "GSM"},
            {"GSM-1800", "GSM"},
            {"E-GSM", "GSM"},
            {"GSM", "GSM"},
            {"UMTS", "UMTS"},
            {"WCDMA", "UMTS"},
            {"LTE-1800", "LTE"},
            {"LTE-2600", "LTE"},
            {"LTE-900", "LTE"},
            {"LTE", "LTE"},
            {"CDMA-450", "CDMA"},
            {"CDMA-800", "CDMA"},
            {"CDMA", "CDMA"},
            {"EVDO", "CDMA"}
        };

        /// <summary>
        /// Получить массив связанных стандартов для драйв тестов
        /// </summary>
        /// <param name="standard"></param>
        /// <returns></returns>
        public static string[] GetStandards(string standard)
        {
            var allStandards = new List<string>();
            allStandards.Add(standard);
            var listStandards = _dictStandardsForDriveTests.ToList();
            for (int i = 0; i < listStandards.Count; i++)
            {
                if ((listStandards[i].Key == standard) || (listStandards[i].Value == standard))
                {
                    allStandards.Add(listStandards[i].Key);
                    allStandards.Add(listStandards[i].Value);
                }
            }
            return allStandards.Distinct().ToArray();
        }

        public static string GetStandardForDriveTest(this string standard)
        {
            var findStandard = "";
            var listStandards = _dictStandardsForDriveTests.ToList();
            for (int i = 0; i < listStandards.Count; i++)
            {
                if (listStandards[i].Key == standard)
                {
                    findStandard = listStandards[i].Value;
                    break;
                }
            }
            return findStandard;
        }

    }

    public class Utils
    {
        private readonly ILogger _logger;

       


        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public Utils(ILogger logger)
        {
            _logger = logger;
        }

        public static bool IsInsideMap(double lon, double lat, double lonMin, double latMin, double lonMax, double latMax)
        {
            if (lon > lonMin && lon < lonMax &&
                lat > latMin && lat < latMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       
        // потом по возможности перенести в UTILS  ет к дублирует метод из Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements  -> class СorrelationСoefficient
        public static double Pearson(float[] arr1, float[] arr2)
        { // НЕ ТЕСТИРОВАННО
            if (arr1.Length != arr2.Length) { return -2; }//Выход с ошибкой
            int n = arr1.Length;
            double sumArr1 = 0; double sumArr2 = 0;
            for (int i = 0; n > i; i++)
            {
                sumArr1 = sumArr1 + arr1[i];
                sumArr2 = sumArr2 + arr2[i];
            }
            sumArr1 = sumArr1 / n;
            sumArr2 = sumArr2 / n;
            double a1 = 0; double a2 = 0; double a3 = 0;
            for (var i = 0; n > i; i++)
            {
                a1 = a1 + ((arr1[i] - sumArr1) * (arr2[i] - sumArr2));
                a2 = a2 + ((arr1[i] - sumArr1) * (arr1[i] - sumArr1));
                a3 = a3 + ((arr2[i] - sumArr2) * (arr2[i] - sumArr2));
            }
            return (a1 / (Math.Sqrt(a2 * a3)));
        }


        /// <summary>
        /// Сравнение драйв тестов со станциями
        /// </summary>
        /// <param name="GCID1"></param>
        /// <param name="GCID2"></param>
        /// <param name="standard"></param>
        /// <returns></returns>
        public static bool CompareGSIDBetweenDriveTestAndStation(string GCIDDriveTest, string GCIDStation, string standard, double freqDriveTest_MHz, double[] freqsStation_MHz)
        {
            if ((freqsStation_MHz.Contains(freqDriveTest_MHz)) && GCIDComparisonRDB.Compare(standard, GCIDDriveTest, GCIDStation))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Сравнение с базовыми станциями
        /// </summary>
        /// <param name="GCID1"></param>
        /// <param name="GCID2"></param>
        /// <param name="standard"></param>
        /// <returns></returns>
        public static bool CompareGSIDWithBaseStations(string GCID1, string GCID2, string standard)
        {
            return (GCID1 == GCID2);
        }

        public static bool CompareGSIDAndDistanceWithBaseStations(string GCID1, string GCID2, string standard, ITransformation transformation, IEarthGeometricService earthGeometricService, double X1, double Y1, double X2, double Y2, int maxDistance, string projection)
        {
            var coordinateStation1 = transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = X1, Latitude = Y1 }, transformation.ConvertProjectionToCode(projection));
            var coordinateStation2 = transformation.ConvertCoordinateToEpgs(new Wgs84Coordinate() { Longitude = X2, Latitude = Y2 }, transformation.ConvertProjectionToCode(projection));
            var pointSourceArgs = new PointEarthGeometric() { Longitude = coordinateStation1.X, Latitude = coordinateStation1.Y };
            var pointTargetArgs = new PointEarthGeometric() { Longitude = coordinateStation2.X, Latitude = coordinateStation2.Y };
            if ((earthGeometricService.GetDistance_km(in pointSourceArgs, in pointTargetArgs) <= maxDistance) && GCID1== GCID2) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод для предварительной подготовки данных
        /// </summary>
        /// <param name="driveTestsResults"></param>
        public static DriveTestsResult[] PrepareData(ref AllStationCorellationCalcData data, ref DriveTestsResult[][] arrDiveTestsResults, long countRecordsListDriveTestsResultBuffer,  IObjectPool<PointFS[]> calcPointArrayPool)
        {
            var alldriveTestsResults = new List<DriveTestsResult>();
            var lowerLeftCoord_m = data.FieldStrengthCalcData.MapArea.LowerLeft;
            var upperRightCoord_m = data.FieldStrengthCalcData.MapArea.UpperRight;
            // Step
            var lonStep_dec = data.FieldStrengthCalcData.MapArea.AxisX.Step;
            var latStep_dec = data.FieldStrengthCalcData.MapArea.AxisY.Step;
            for (int k = 0; k < countRecordsListDriveTestsResultBuffer; k++)
            {
                var groupDriveTestByGsid = arrDiveTestsResults[k];
                for (int z = 0; z < groupDriveTestByGsid.Length; z++)
                {
                    int counter = 0;
                    var calcPointArrayBuffer = default(PointFS[]);
                    try
                    {
                        calcPointArrayBuffer = calcPointArrayPool.Take();
                        var drivePoint = groupDriveTestByGsid[z];
                        for (int i = 0; i < drivePoint.Points.Length; i++)
                        {
                            if (drivePoint.Points[i].FieldStrength_dBmkVm >= data.CorellationParameters.MinRangeMeasurements_dBmkV &&
                               drivePoint.Points[i].FieldStrength_dBmkVm <= data.CorellationParameters.MaxRangeMeasurements_dBmkV &&
                               Utils.IsInsideMap(drivePoint.Points[i].Coordinate.X, drivePoint.Points[i].Coordinate.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y))
                            {
                                var isFoubdInBuffer = false;
                                if (counter > 0)
                                {
                                    // Сравнение следующих координат с приведенными к центру пикселя,
                                    for (int j = 0; j < counter; j++)
                                    {

                                        bool isInsidePixelLon = Math.Abs(drivePoint.Points[i].Coordinate.X - calcPointArrayBuffer[j].Coordinate.X) <= Math.Ceiling( data.FieldStrengthCalcData.MapArea.AxisX.Step / 2.0);
                                        bool isInsidePixelLat = Math.Abs(drivePoint.Points[i].Coordinate.Y - calcPointArrayBuffer[j].Coordinate.Y) <= Math.Ceiling(data.FieldStrengthCalcData.MapArea.AxisY.Step / 2.0);
                                        if (isInsidePixelLon && isInsidePixelLat)
                                        {
                                            //  в случае если по координатам уже есть изменерия, напряжённость усредняется
                                            var intermediateFS = (calcPointArrayBuffer[j].Count * calcPointArrayBuffer[j].FieldStrength_dBmkVm + drivePoint.Points[i].FieldStrength_dBmkVm) / (calcPointArrayBuffer[j].Count + 1);
                                            calcPointArrayBuffer[j].FieldStrength_dBmkVm = intermediateFS;//(float)(20 * Math.Log10((calcPointArrayBuffer[j].Count * Math.Pow(10, 0.05 * calcPointArrayBuffer[j].FieldStrength_dBmkVm) + Math.Pow(10, 0.05 * drivePoint.Points[i].FieldStrength_dBmkVm)) / (calcPointArrayBuffer[j].Count + 1)));
                                            calcPointArrayBuffer[j].Level_dBm = drivePoint.Points[i].Level_dBm;
                                            calcPointArrayBuffer[j].Height_m = drivePoint.Points[i].Height_m;
                                            calcPointArrayBuffer[j].Count += 1;
                                            isFoubdInBuffer = true;
                                            break; // как только нашли точку в буфере, у которой совпали координаты в пределах пикселя - поиск прекращается
                                        }
                                    }
                                }
                                if (isFoubdInBuffer == false || counter == 0)
                                {
                                    // выполняется для первой итерации и в случае если по координатам не было измерений
                                    calcPointArrayBuffer[counter].Count = 1;
                                    calcPointArrayBuffer[counter].Coordinate.X = lowerLeftCoord_m.X + Math.Floor((drivePoint.Points[i].Coordinate.X - lowerLeftCoord_m.X) / data.FieldStrengthCalcData.MapArea.AxisX.Step) * data.FieldStrengthCalcData.MapArea.AxisX.Step + data.FieldStrengthCalcData.MapArea.AxisX.Step / 2;
                                    calcPointArrayBuffer[counter].Coordinate.Y = lowerLeftCoord_m.Y + Math.Floor((drivePoint.Points[i].Coordinate.Y - lowerLeftCoord_m.Y) / data.FieldStrengthCalcData.MapArea.AxisY.Step) * data.FieldStrengthCalcData.MapArea.AxisY.Step + data.FieldStrengthCalcData.MapArea.AxisY.Step / 2;
                                    calcPointArrayBuffer[counter].FieldStrength_dBmkVm = drivePoint.Points[i].FieldStrength_dBmkVm;
                                    calcPointArrayBuffer[counter].Level_dBm = drivePoint.Points[i].Level_dBm;
                                    calcPointArrayBuffer[counter].Height_m = drivePoint.Points[i].Height_m;
                                    counter++;
                                }
                            }
                        }

                        var driveTestsResults = new PointFS[counter];
                        for (int v = 0; v < counter; v++)
                        {
                            driveTestsResults[v] = calcPointArrayBuffer[v];
                        }

                        drivePoint.Points = driveTestsResults;
                        drivePoint.CountPoints = driveTestsResults.Length;
                        groupDriveTestByGsid[z] = drivePoint;

                    }
                    finally
                    {
                        if (calcPointArrayBuffer != null)
                        {
                            calcPointArrayPool.Put(calcPointArrayBuffer);
                        }
                    }
                }

                var lstResDriveTest = groupDriveTestByGsid.ToList();
                lstResDriveTest.RemoveAll(x => x.Points.Length == 0);

                arrDiveTestsResults[k] = lstResDriveTest.ToArray();
                alldriveTestsResults.AddRange(arrDiveTestsResults[k]);
            }
            return alldriveTestsResults.ToArray();
        }

        /// <summary>
        /// Метод для перфорации точек драйв тестов
        /// </summary>
        /// <param name="pointFs"></param>
        public static void PerforationPoints(ref PointFS[] pointFs)
        {
            var lstPointFs = new List<PointFS>();
            for (int i = 0; i < pointFs.Length; i += 2)
            {
                lstPointFs.Add(pointFs[i]);
            }
            if (lstPointFs != null)
            {
                var orderByCountPoints = from z in lstPointFs orderby z.Coordinate.Y, z.Coordinate.X ascending select z;
                pointFs = orderByCountPoints.ToArray();
            }
        }


        /// <summary>
        /// Получить весь массив уникальных STANDARDS из станций
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayStandardsfromStations(ContextStation[] contextStations)
        {
            var arrayStandards = contextStations.Select(x => x.Standard);
            if (arrayStandards != null)
            {
                return arrayStandards.Distinct().ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получить весь массив уникальных STANDARDS из драйв тестов
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayStandardsFromDriveTests(DriveTestsResult[] driveTests)
        {
            var arrayStandards = driveTests.Select(x => x.Standard);
            if (arrayStandards != null)
            {
                return arrayStandards.Distinct().ToArray();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Получить массив уникальных GSID из станций, по заданному стандарту
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromStations(ContextStation[] contextStations, string Standard)
        {
            var stations = contextStations.ToList();
            var fndStations = stations.FindAll(x => x.Standard == Standard);
            if (fndStations != null)
            {
                var arrayLicenseGsid = fndStations.Select(x => x.LicenseGsid);
                if (arrayLicenseGsid != null)
                {
                    return arrayLicenseGsid.Distinct().ToArray();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получить весь массив уникальных GSID из драйв тестов, по заданному стандарту
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromDriveTests(DriveTestsResult[] driveTests, string Standard)
        {
            var drvTests = driveTests.ToList();
            var fndDriveTests = drvTests.FindAll(x => x.Standard == Standard);
            if (fndDriveTests != null)
            {
                var arrayGSID = fndDriveTests.Select(x => x.GSID);
                if (arrayGSID != null)
                {
                    return arrayGSID.Distinct().ToArray();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Получить весь массив уникальных GSID из станций
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromStations(ContextStation[] contextStations)
        {
            var arrayLicenseGsid = contextStations.Select(x => x.LicenseGsid);
            if (arrayLicenseGsid != null)
            {
                return arrayLicenseGsid.Distinct().ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получить весь массив уникальных GSID из драйв тестов
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromDriveTests(DriveTestsResult[] driveTests)
        {
            var arrayGSID = driveTests.Select(x => x.GSID);
            if (arrayGSID != null)
            {
                return arrayGSID.Distinct().ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Обработка всех Stations 
        /// Если у нас есть станции, которые имеют одинаковые GSID и разные статусы то удаляем все станции со статусом I.
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static ContextStation[] PerforationStations(ContextStation[] contextStations)
        {
            var listStations = contextStations.ToList();
            var licenseGsid = GetUniqueArrayGSIDfromStations(contextStations);
            if (licenseGsid != null)
            {
                for (int j = 0; j < licenseGsid.Length; j++)
                {
                    var fndStations = listStations.FindAll(x => x.LicenseGsid == licenseGsid[j]);
                    if ((fndStations != null) && (fndStations.Count > 0))
                    {
                        var allStatusByStandard = fndStations.Select(x => x.Type);
                        if (allStatusByStandard != null)
                        {
                            var cntUniqueStatus = allStatusByStandard.Distinct().ToArray();
                            if (cntUniqueStatus.Length > 1)
                            {
                                listStations.RemoveAll(x => x.Type == DataModels.Sdrn.CalcServer.Internal.Clients.ClientContextStationType.I && x.LicenseGsid == licenseGsid[j]);
                            }
                        }
                    }
                }
            }
            return listStations.ToArray();
        }


        /// <summary>
        /// Массив драйв тестов, сгруппированный на основании GSID
        /// </summary>
        /// <param name="driveTests"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static DriveTestsResult[] GroupingDriveTestsByGSID(DriveTestsResult[] driveTests, string GSID)
        {
            var listDriveTests = driveTests.ToList();
            var fndDriveTests = listDriveTests.FindAll(x => x.GSID == GSID );
            if ((fndDriveTests != null) && (fndDriveTests.Count > 0))
            {
                return fndDriveTests.ToArray();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Массив драйв тестов, сгруппированный на основании GSID и заданному стандарту
        /// </summary>
        /// <param name="driveTests"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static DriveTestsResult[] GroupingDriveTestsByStandardAndGSID(DriveTestsResult[] driveTests, string GSID, string Standard)
        {
            var listDriveTests = driveTests.ToList();
            var fndDriveTests = listDriveTests.FindAll(x => x.GSID == GSID && x.Standard == Standard);
            if ((fndDriveTests != null) && (fndDriveTests.Count > 0))
            {
                return fndDriveTests.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Массив станций, сгруппированный на основании GSID и заданному стандарту
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static ContextStation[] GroupingStationsByStandardAndGSID(ContextStation[] contextStations, string GSID, string Standard)
        {
            var listStations = contextStations.ToList();
            var fndStations = listStations.FindAll(x => x.LicenseGsid == GSID && x.Standard == Standard /*&& x.Type== ClientContextStationType.A*/);
            if ((fndStations != null) && (fndStations.Count > 0))
            {
                return fndStations.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Массив станций, сгруппированный на основании GSID, заданному стандарту и статусу станции
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static ContextStation[] GroupingStationsByStandardAndGSIDAndStatus(ContextStation[] contextStations, string GSID, string Standard, ClientContextStationType[]  clientContextStationTypes)
        {
            var listStations = contextStations.ToList();
            var fndStations = listStations.FindAll(x => x.LicenseGsid == GSID && x.Standard == Standard && clientContextStationTypes.Contains(x.Type));
            if ((fndStations != null) && (fndStations.Count > 0))
            {
                return fndStations.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Формирование групп станций на основе последовательного вызова метода CompareGSID для сравнения станций между собой (c учетом стандарта)
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static ContextStation[][] GroupStationsByStatus(ContextStation[] contextStations, string Standard, ITransformation transformation, int maxDistance, string projection)
        {
            var lstContextStations = new List<ContextStation[]>();
            var arrUniqueGSID = GetUniqueArrayGSIDfromStations(contextStations, Standard);
            if (arrUniqueGSID != null)
            {
                for (int i = 0; i < arrUniqueGSID.Length; i++)
                {
                    var groupContextStations = GroupingStationsByStandardAndGSIDAndStatus(contextStations, arrUniqueGSID[i], Standard, new ClientContextStationType[] { ClientContextStationType.P });
                    if (groupContextStations != null)
                    {
                        lstContextStations.Add(groupContextStations);
                    }
                }
            }
            return lstContextStations.ToArray();
        }



        /// <summary>
        /// Формирование групп станций на основе последовательного вызова метода CompareGSID для сравнения станций между собой ( c учетом стандарта)
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static ContextStation[][] CompareStations(ContextStation[] contextStations, string Standard, ITransformation transformation, IEarthGeometricService earthGeometricService, int maxDistance, string projection)
        {
            var lstContextStations = new List<ContextStation[]>();
            var lstAllStations = new List<ContextStation>();
            var lstRecalcContextStations = new List<ContextStation[]>();
            var arrUniqueGSID = GetUniqueArrayGSIDfromStations(contextStations, Standard);
            if (arrUniqueGSID != null)
            {
                for (int i = 0; i < arrUniqueGSID.Length; i++)
                {
                    var groupContextStations = GroupingStationsByStandardAndGSIDAndStatus(contextStations, arrUniqueGSID[i], Standard, new ClientContextStationType[] { ClientContextStationType.A, ClientContextStationType.I });
                    if (groupContextStations != null)
                    {
                        lstContextStations.Add(groupContextStations);
                        lstAllStations.AddRange(groupContextStations.ToArray());
                    }
                }
                if (lstContextStations.Count > 0)
                {
                    var lstGroupContextStations = new List<ContextStation>();
                    for (int k = 0; k < lstContextStations.Count; k++)
                    {
                        var stationOne = lstContextStations[k][0];
                        for (int j = 0; j < lstContextStations.Count; j++)
                        {
                            var stationTwo = lstContextStations[j][0];
                            if (CompareGSIDAndDistanceWithBaseStations(stationOne.LicenseGsid, stationTwo.LicenseGsid, Standard.GetStandardForDriveTest(), transformation, earthGeometricService, stationOne.Site.Longitude, stationOne.Site.Latitude, stationTwo.Site.Longitude, stationTwo.Site.Latitude, maxDistance, projection))
                            {
                                for (int v = 0; v < lstContextStations[k].Length; v++)
                                {
                                    lstContextStations[k][v].NameGroupGlobalSID = stationOne.LicenseGsid;
                                    if ((lstGroupContextStations.FindAll(x => x.Id == lstContextStations[k][v].Id).Count == 0))
                                    {
                                        lstGroupContextStations.Add(lstContextStations[k][v]);
                                    }
                                }

                                for (int v = 0; v < lstContextStations[j].Length; v++)
                                {
                                    lstContextStations[j][v].NameGroupGlobalSID = stationOne.LicenseGsid;
                                    if ((lstGroupContextStations.FindAll(x => x.Id == lstContextStations[j][v].Id).Count == 0))
                                    {
                                        lstGroupContextStations.Add(lstContextStations[j][v]);
                                    }
                                }
                            }
                        }
                    }


                    if ((lstGroupContextStations != null) && (lstGroupContextStations.Count > 0))
                    {
                        var arrUniqueGroups = GetUniqueArrayGroupsFromStations(lstGroupContextStations.ToArray());
                        if (arrUniqueGroups != null)
                        {
                            for (int k = 0; k < arrUniqueGroups.Length; k++)
                            {
                                var StationByOneGroup = lstGroupContextStations.FindAll(x => x.NameGroupGlobalSID == arrUniqueGroups[k]);
                                if (StationByOneGroup != null)
                                {
                                    StationByOneGroup.Distinct();
                                    var tempGroupStationByOneGroupResult = new List<ContextStation>();
                                    var distinctId = new List<long>();
                                    for (int j = 0; j < StationByOneGroup.Count; j++)
                                    {
                                        if (!distinctId.Contains(StationByOneGroup[j].Id))
                                        {
                                            tempGroupStationByOneGroupResult.Add(StationByOneGroup[j]);
                                            distinctId.Add(StationByOneGroup[j].Id);
                                        }
                                    }
                                    lstRecalcContextStations.Add(tempGroupStationByOneGroupResult.ToArray());
                                }
                            }
                        }
                    }
                    else
                    {
                        lstRecalcContextStations.Add(lstAllStations.ToArray());
                    }

                    //var cntRecalcContextStations = 0;

                    //var arrUniqueGroups = GetUniqueArrayGroupsFromStations(lstGroupContextStations.ToArray());
                    //if (arrUniqueGroups != null)
                    //{
                    //    for (int k = 0; k < arrUniqueGroups.Length; k++)
                    //    {
                    //        var tempGroupStations = new List<ContextStation>();
                    //        for (int j = 0; j < arrUniqueGSID.Length; j++)
                    //        {
                    //            if (!arrUniqueGroups.Contains(arrUniqueGSID[j]))
                    //            {
                    //                tempGroupStations.AddRange(lstAllStations.FindAll(x => x.LicenseGsid == arrUniqueGSID[j]));
                    //            }
                    //        }

                    //        var stationsByOneGroup = lstGroupContextStations.FindAll(x => x.NameGroupGlobalSID == arrUniqueGroups[k]);
                    //        for (int n = 0; n < tempGroupStations.Count; n++)
                    //        {
                    //            if (stationsByOneGroup.FindAll(x => x.Id == tempGroupStations[n].Id).Count == 0)
                    //            {
                    //                lstRecalcContextStations.Add(new ContextStation[] { tempGroupStations[n] });
                    //                cntRecalcContextStations += 1;
                    //            }
                    //        }
                    //        lstRecalcContextStations.Add(stationsByOneGroup.ToArray());
                    //        cntRecalcContextStations += stationsByOneGroup.Count;
                    //    }
                    //}
                }
            }
            return lstRecalcContextStations.ToArray();
        }

        /// <summary>
        /// Выделяем драйв тесты в отдельные группы c учетом стандарта
        /// </summary>
        /// <param name="contextDriveTestsResult"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static DriveTestsResult[][] CompareDriveTests(DriveTestsResult[] contextDriveTestsResult, string Standard)
        {
            var lstDriveTestsResult = new List<DriveTestsResult[]>();
            var lstAllDriveTestsResult = new List<DriveTestsResult>();
            var lstRecalcDriveTestsResult = new List<DriveTestsResult[]>();
            var arrUniqueGSID = GetUniqueArrayGSIDfromDriveTests(contextDriveTestsResult, Standard);
            if (arrUniqueGSID != null)
            {
                for (int i = 0; i < arrUniqueGSID.Length; i++)
                {
                    var groupDriveTest = GroupingDriveTestsByStandardAndGSID(contextDriveTestsResult, arrUniqueGSID[i], Standard);
                    if (groupDriveTest != null)
                    {
                        lstDriveTestsResult.Add(groupDriveTest);
                        lstAllDriveTestsResult.AddRange(groupDriveTest.ToArray());
                    }
                }

                var lstGroupDriveTestsResult = new List<DriveTestsResult>();
                for (int k = 0; k < lstDriveTestsResult.Count; k++)
                {
                    var driveTestsOne = lstDriveTestsResult[k][0];
                    for (int j = 0; j < lstDriveTestsResult.Count; j++)
                    {
                        var driveTestsTwo = lstDriveTestsResult[j][0];
                        var one = driveTestsOne.GSID.Remove(driveTestsOne.GSID.Length - 1, 1);
                        var two = driveTestsTwo.GSID.Remove(driveTestsTwo.GSID.Length - 1, 1);
                        if (CompareGSIDWithBaseStations(one, two, Standard.GetStandardForDriveTest()))
                        {
                            for (int v = 0; v < lstDriveTestsResult[k].Length; v++)
                            {
                                lstDriveTestsResult[k][v].NameGroupGlobalSID = driveTestsOne.GSID;
                                if ((lstGroupDriveTestsResult.FindAll(x => x.Num == lstDriveTestsResult[k][v].Num).Count == 0))
                                {
                                    lstGroupDriveTestsResult.Add(lstDriveTestsResult[k][v]);
                                }
                            }

                            for (int v = 0; v < lstDriveTestsResult[j].Length; v++)
                            {
                                lstDriveTestsResult[j][v].NameGroupGlobalSID = driveTestsOne.GSID;
                                if ((lstGroupDriveTestsResult.FindAll(x => x.Num == lstDriveTestsResult[j][v].Num).Count == 0))
                                {
                                    lstGroupDriveTestsResult.Add(lstDriveTestsResult[j][v]);
                                }
                            }
                        }
                    }
                }


                if ((lstGroupDriveTestsResult != null) && (lstGroupDriveTestsResult.Count > 0))
                {
                    //var lstDrvTest = contextDriveTestsResult.ToList();
                    //for (int v = 0; v < lstDrvTest.Count; v++)
                    //{
                    //    if (lstGroupDriveTestsResult.Find(x => x.DriveTestId == lstDrvTest[v].DriveTestId) == null)
                    //    {
                    //        lstGroupDriveTestsResult.Add(lstGroupDriveTestsResult[v]);
                    //    }
                    //}

                    var arrUniqueGroups = GetUniqueArrayGroupsFromDriveTests(lstGroupDriveTestsResult.ToArray());
                    if (arrUniqueGroups != null)
                    {
                        for (int k = 0; k < arrUniqueGroups.Length; k++)
                        {

                            var driveTestsByOneGroup = lstGroupDriveTestsResult.FindAll(x => x.NameGroupGlobalSID == arrUniqueGroups[k]);
                            if (driveTestsByOneGroup != null)
                            {
                                driveTestsByOneGroup.Distinct();
                                var tempGroupDriveTestsResult = new List<DriveTestsResult>();
                                var distinctId = new List<long>();
                                for (int j = 0; j < driveTestsByOneGroup.Count; j++)
                                {
                                    if (!distinctId.Contains(driveTestsByOneGroup[j].DriveTestId))
                                    {
                                        tempGroupDriveTestsResult.Add(driveTestsByOneGroup[j]);
                                        distinctId.Add(driveTestsByOneGroup[j].DriveTestId);
                                    }
                                }

                                var orderByCountPoints = from z in tempGroupDriveTestsResult orderby z.CountPoints descending select z;
                                var tempDriveTestsByOneGroup = orderByCountPoints.ToArray();
                                lstRecalcDriveTestsResult.Add(tempDriveTestsByOneGroup);
                            }
                        }
                    }
                }
                else
                {
                    var orderByCountPoints = from z in lstAllDriveTestsResult orderby z.CountPoints descending select z;
                    var tempDriveTestsByOneGroup = orderByCountPoints.ToArray();
                    lstRecalcDriveTestsResult.Add(tempDriveTestsByOneGroup);
                }
            }
            return lstRecalcDriveTestsResult.ToArray();
        }


        public static void CompareDriveTestsWithoutStandards(in DriveTestsResult[] GSIDGroupeDriveTests, DriveTestsResult[][] outListDriveTestsResult, out long countRecords)
        {
            var arrUniqueGSID = Utils.GetUniqueArrayGSIDfromDriveTests(GSIDGroupeDriveTests);
            if (arrUniqueGSID != null)
            {
                for (int i = 0; i < arrUniqueGSID.Length; i++)
                {
                    var groupDriveTest = Utils.GroupingDriveTestsByGSID(GSIDGroupeDriveTests, arrUniqueGSID[i]);
                    outListDriveTestsResult[i] = groupDriveTest;
                }
            }
            countRecords = arrUniqueGSID.Length;
        }


        /// <summary>
        /// Поиcк соответствий между драйв тестами и станицями
        /// </summary>
        /// <param name="contextDriveTestsResult"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static LinkGoupDriveTestsAndStations[] CompareDriveTestAndStation(DriveTestsResult[] contextDriveTestsResult,
                                                                                 ContextStation[] contextStations,
                                                                                 string Standard,
                                                                                 out DriveTestsResult[][] outDriveTestsResults,
                                                                                 out ContextStation[][] outContextStations,
                                                                                 ITransformation transformation,
                                                                                 IEarthGeometricService earthGeometricService,
                                                                                 int maxDistance,
                                                                                 string projection)
        {
            var lstLinkDriveTestsResultAndStation = new List<LinkGoupDriveTestsAndStations>();
            var forDeleteContextStation = new List<ContextStation>();
            var forDeleteDriveTestsResult = new List<DriveTestsResult>();
            var groupedDriveTests = CompareDriveTests(contextDriveTestsResult, Standard).ToList();
            var groupedStations = CompareStations(contextStations, Standard, transformation, earthGeometricService, maxDistance, projection).ToList();
            var lstStations = Atdi.Common.CopyHelper.CreateDeepCopy(contextStations).ToList();
            var lstDriveTests = Atdi.Common.CopyHelper.CreateDeepCopy(contextDriveTestsResult).ToList();

            for (int m = 0; m < groupedDriveTests.Count; m++)
            {
                if (groupedDriveTests.Count > 0)
                {
                    var driveTestGSID = groupedDriveTests[m].ToList();
                    for (int i = 0; i < groupedStations.Count; i++)
                    {
                        if (groupedStations.Count > 0)
                        {
                            var stationsGSID = groupedStations[i].ToList();

                            if ((stationsGSID != null) && (driveTestGSID != null))
                            {
                                if ((groupedDriveTests.Count > 0) && (groupedStations.Count > 0))
                                {
                                    if (CompareGSIDBetweenDriveTestAndStation(driveTestGSID[0].GSID, stationsGSID[0].LicenseGsid, Standard.GetStandardForDriveTest(), groupedDriveTests[m][0].Freq_MHz, groupedStations[i][0].Transmitter.Freqs_MHz))
                                    {
                                        var lnkDriveTest = new LinkGoupDriveTestsAndStations()
                                        {
                                            ContextStation = groupedStations[i],
                                            DriveTestsResults = groupedDriveTests[m]
                                        };

                                        lstLinkDriveTestsResultAndStation.Add(lnkDriveTest);
                                        forDeleteDriveTestsResult.AddRange(groupedDriveTests[m]);
                                        forDeleteContextStation.AddRange(groupedStations[i]);

                                        groupedStations.RemoveAt(i);
                                        groupedDriveTests.RemoveAt(m);
                                        i = 0;
                                        m = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            for (int i = 0; i < forDeleteContextStation.Count; i++)
            {
                for (int j = 0; j < lstStations.Count; j++)
                {
                    var station = lstStations[j];
                    if (station.Id == forDeleteContextStation[i].Id)
                    {
                        lstStations.RemoveAt(j);
                        break;
                    }
                }
            }
            if (lstStations.Count > 0)
            {
                outContextStations = new ContextStation[1][] { lstStations.ToArray() };
            }
            else
            {
                outContextStations = new ContextStation[0][];
            }
            


            for (int i = 0; i < forDeleteDriveTestsResult.Count; i++)
            {
                for (int j = 0; j < lstDriveTests.Count; j++)
                {
                    var driveTest = lstDriveTests[j];
                    if (driveTest.DriveTestId == forDeleteDriveTestsResult[i].DriveTestId)
                    {
                        lstDriveTests.RemoveAt(j);
                        break;
                    }
                }
            }
            if (lstDriveTests.Count > 0)
            {
                outDriveTestsResults = new DriveTestsResult[1][] { lstDriveTests.ToArray() };
            }
            else
            {
                outDriveTestsResults = new DriveTestsResult[0][];
            }

            return lstLinkDriveTestsResultAndStation.ToArray();
        }

        public static string[] GetUniqueArrayGroupsFromStations(ContextStation[] contextStations)
        {
            var stations = contextStations.ToList();
            var arrayNameGroupGlobalSID = stations.Select(x => x.NameGroupGlobalSID);
            if (arrayNameGroupGlobalSID != null)
            {
                return arrayNameGroupGlobalSID.Distinct().ToArray();
            }
            else
            {
                return null;
            }
        }

        public static string[] GetUniqueArrayGroupsFromDriveTests(DriveTestsResult[] driveTestsResult)
        {
            var driveTestsRes = driveTestsResult.ToList();
            var arrayNameGroupGlobalSID = driveTestsRes.Select(x => x.NameGroupGlobalSID);
            if (arrayNameGroupGlobalSID != null)
            {
                return arrayNameGroupGlobalSID.Distinct().ToArray();
            }
            else
            {
                return null;
            }
        }

        public static EpsgCoordinate CenterWeightAllCoordinates(PointFS[] pointFs)
        {
            double xCenterWeightCoord = 0;
            double yCenterWeightCoord = 0;
            for (int i = 0; i < pointFs.Length; i++)
            {
                var coordinate = pointFs[i].Coordinate;
                xCenterWeightCoord += coordinate.X;
                yCenterWeightCoord += coordinate.Y;
            }
            return new EpsgCoordinate() { X = xCenterWeightCoord / pointFs.Length,  Y = yCenterWeightCoord / pointFs.Length };
        }
    }
}
