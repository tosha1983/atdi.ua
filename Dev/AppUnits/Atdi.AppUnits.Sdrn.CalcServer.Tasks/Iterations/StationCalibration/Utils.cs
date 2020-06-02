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



namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{

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


        public static bool CompareGSIDSimple(string GCID1, string GCID2)
        {
            if (GCID2 == GCID2)
            {
                return true;
            }
            else
            {
                return false;
            }
            //return resCompare;
        }

        public static bool CompareGSID(string GCID1, string GCID2, string standard)
        {
            //var resCompare = GCIDComparison.Compare(standard, GCID1, GCID2);
            if (GCID2 == GCID2)
            {
                return true;
            }
            else
            {
                return false;
            }
            //return resCompare;
        }

        /// <summary>
        /// Метод для предварительной подготовки данных
        /// </summary>
        /// <param name="driveTestsResults"></param>
        public static DriveTestsResult[] PrepareData(ref AllStationCorellationCalcData data, ref List<DriveTestsResult[]> arrDiveTestsResults,  IObjectPool<PointFS[]> calcPointArrayPool)
        {
            var alldriveTestsResults = new List<DriveTestsResult>();
            var lowerLeftCoord_m = data.FieldStrengthCalcData.MapArea.LowerLeft;
            var upperRightCoord_m = data.FieldStrengthCalcData.MapArea.UpperRight;
            // Step
            double lonStep_dec = data.FieldStrengthCalcData.MapArea.AxisX.Step;//_transformation.ConvertCoordinateToWgs84(data.FieldStrengthCalcData.MapArea.LowerLeft.X, data.CodeProjection);
            double latStep_dec = data.FieldStrengthCalcData.MapArea.AxisY.Step;


            for (int k = 0; k < arrDiveTestsResults.Count; k++)
            {
                var groupDriveTestByGsid = arrDiveTestsResults[k];

                for (int z = 0; z < groupDriveTestByGsid.Length; z++)
                {
                    int counter = 0;
                    var calcPointArrayBuffer = default(PointFS[]);
                    calcPointArrayBuffer = calcPointArrayPool.Take();

                    var drivePoint = groupDriveTestByGsid[z];
                    for (int i = 0; i < drivePoint.Points.Length; i++)
                    {
                        if (drivePoint.Points[i].FieldStrength_dBmkVm >= data.CorellationParameters.MinRangeMeasurements_dBmkV &&
                           drivePoint.Points[i].FieldStrength_dBmkVm <= data.CorellationParameters.MaxRangeMeasurements_dBmkV &&
                           Utils.IsInsideMap(drivePoint.Points[i].Coordinate.X, drivePoint.Points[i].Coordinate.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y))
                        {
                            bool isFoubdInBuffer = false;

                            if (counter > 0)
                            {
                                // Сравнение следующих координат с приведенными к центру пикселя,
                                for (int j = 0; j < counter; j++)
                                {

                                    bool isInsidePixelLon = Math.Abs(drivePoint.Points[i].Coordinate.X - calcPointArrayBuffer[j].Coordinate.X) < data.FieldStrengthCalcData.MapArea.AxisX.Step/2;
                                    bool isInsidePixelLat = Math.Abs(drivePoint.Points[i].Coordinate.Y - calcPointArrayBuffer[j].Coordinate.Y) < data.FieldStrengthCalcData.MapArea.AxisY.Step/2;
                                    if (isInsidePixelLon && isInsidePixelLat)
                                    {
                                        //  в случае если по координатам уже есть изменерия, напряжённость усредняется
                                        var intermediateFS = (calcPointArrayBuffer[j].Count * calcPointArrayBuffer[j].FieldStrength_dBmkVm + drivePoint.Points[i].FieldStrength_dBmkVm ) / ( calcPointArrayBuffer[j].Count + 1 );
                                        calcPointArrayBuffer[j].FieldStrength_dBmkVm = intermediateFS;//(float)(20 * Math.Log10((calcPointArrayBuffer[j].Count * Math.Pow(10, 0.05 * calcPointArrayBuffer[j].FieldStrength_dBmkVm) + Math.Pow(10, 0.05 * drivePoint.Points[i].FieldStrength_dBmkVm)) / (calcPointArrayBuffer[j].Count + 1)));
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
                    groupDriveTestByGsid[z] = drivePoint;

                    if (calcPointArrayBuffer != null)
                    {
                        calcPointArrayPool.Put(calcPointArrayBuffer);
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

            var orderByCountPoints = from z in lstPointFs orderby z.Coordinate.Y, z.Coordinate.X ascending select z;
            pointFs = orderByCountPoints.ToArray();
        }


        /// <summary>
        /// Получить весь массив уникальных STANDARDS из станций
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayStandardsfromStations(ContextStation[] contextStations)
        {
            var arrayStandards = contextStations.Select(x => x.Standard);
            return arrayStandards.Distinct().ToArray();
        }

        /// <summary>
        /// Получить весь массив уникальных STANDARDS из драйв тестов
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayStandardsFromDriveTests(DriveTestsResult[] driveTests)
        {
            var arrayStandards = driveTests.Select(x => x.Standard);
            return arrayStandards.Distinct().ToArray();
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
            var arrayLicenseGsid = fndStations.Select(x => x.LicenseGsid);
            return arrayLicenseGsid.Distinct().ToArray();
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
            var arrayGSID = fndDriveTests.Select(x => x.GSID);
            return arrayGSID.Distinct().ToArray();
        }


        /// <summary>
        /// Получить весь массив уникальных GSID из станций
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromStations(ContextStation[] contextStations)
        {
            var arrayLicenseGsid = contextStations.Select(x => x.LicenseGsid);
            return arrayLicenseGsid.Distinct().ToArray();
        }

        /// <summary>
        /// Получить весь массив уникальных GSID из драйв тестов
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromDriveTests(DriveTestsResult[] driveTests)
        {
            var arrayGSID = driveTests.Select(x => x.GSID);
            return arrayGSID.Distinct().ToArray();
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
            for (int j = 0; j < licenseGsid.Length; j++)
            {
                var fndStations = listStations.FindAll(x => x.LicenseGsid == licenseGsid[j]);
                if ((fndStations != null) && (fndStations.Count > 0))
                {
                    var allStatusByStandard = fndStations.Select(x => x.Type);
                    var cntUniqueStatus = allStatusByStandard.Distinct().ToArray();
                    if (cntUniqueStatus.Length > 1)
                    {
                        listStations.RemoveAll(x => x.Type == DataModels.Sdrn.CalcServer.Internal.Clients.ClientContextStationType.I && x.LicenseGsid == licenseGsid[j]);
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
            var lstDriveTests = new List<DriveTestsResult>();
            var listDriveTests = driveTests.ToList();
            var fndDriveTests = listDriveTests.FindAll(x => x.GSID == GSID );
            if ((fndDriveTests != null) && (fndDriveTests.Count > 0))
            {
                lstDriveTests.AddRange(fndDriveTests);
            }
            return lstDriveTests.ToArray();
        }


        /// <summary>
        /// Массив драйв тестов, сгруппированный на основании GSID и заданному стандарту
        /// </summary>
        /// <param name="driveTests"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static DriveTestsResult[] GroupingDriveTestsByStandardAndGSID(DriveTestsResult[] driveTests, string GSID, string Standard)
        {
            var lstDriveTests = new List<DriveTestsResult>();
            var listDriveTests = driveTests.ToList();
            var fndDriveTests = listDriveTests.FindAll(x => x.GSID == GSID && x.Standard == Standard);
            if ((fndDriveTests != null) && (fndDriveTests.Count > 0))
            {
                lstDriveTests.AddRange(fndDriveTests);
            }
            return lstDriveTests.ToArray();
        }

        /// <summary>
        /// Массив станций, сгруппированный на основании GSID и заданному стандарту
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static ContextStation[] GroupingStationsByStandardAndGSID(ContextStation[] contextStations, string GSID, string Standard)
        {
            var lstContextStations = new List<ContextStation>();
            var listStations = contextStations.ToList();
            var fndStations = listStations.FindAll(x => x.LicenseGsid == GSID && x.Standard == Standard);
            if ((fndStations != null) && (fndStations.Count > 0))
            {
                lstContextStations.AddRange(fndStations);
            }
            return lstContextStations.ToArray();
        }


        /// <summary>
        /// Формирование групп станций на основе последовательного вызова метода CompareGSID для сравнения станций между собой ( c учетом стандарта)
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static ContextStation[][] CompareStations(ContextStation[] contextStations, string Standard)
        {
            var lstContextStations = new List<ContextStation[]>();
            var lstAllStations = new List<ContextStation>();
            var arrUniqueGSID = GetUniqueArrayGSIDfromStations(contextStations, Standard);
            for (int i = 0; i < arrUniqueGSID.Length; i++)
            {
                var groupContextStations = GroupingStationsByStandardAndGSID(contextStations, arrUniqueGSID[i], Standard);
                lstContextStations.Add(groupContextStations);
                lstAllStations.AddRange(groupContextStations.ToArray());
            }

            var lstGroupContextStations = new List<ContextStation>();
            for (int k = 0; k < arrUniqueGSID.Length; k++)
            {
                var stationOne = lstContextStations[k][0];
                for (int j = 0; j < arrUniqueGSID.Length; j++)
                {
                    var stationTwo = lstContextStations[j][0];
                    if (CompareGSID(stationOne.LicenseGsid, stationTwo.LicenseGsid, Standard))
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

            var cntRecalcContextStations = 0;
            var lstRecalcContextStations = new List<ContextStation[]>();
            var arrUniqueGroups = GetUniqueArrayGroupsFromStations(lstGroupContextStations.ToArray());

            for (int k = 0; k < arrUniqueGroups.Length; k++)
            {
                var tempGroupStations = new List<ContextStation>();
                for (int j = 0; j < arrUniqueGSID.Length; j++)
                {
                    if (!arrUniqueGroups.Contains(arrUniqueGSID[j]))
                    {
                        tempGroupStations.AddRange(lstAllStations.FindAll(x => x.LicenseGsid == arrUniqueGSID[j]));
                    }
                }

                var stationsByOneGroup = lstGroupContextStations.FindAll(x => x.NameGroupGlobalSID == arrUniqueGroups[k]);
                for (int n = 0; n < tempGroupStations.Count; n++)
                {
                    if (stationsByOneGroup.FindAll(x => x.Id == tempGroupStations[n].Id).Count == 0)
                    {
                        lstRecalcContextStations.Add(new ContextStation[] { tempGroupStations[n] });
                        cntRecalcContextStations += 1;
                    }
                }
                lstRecalcContextStations.Add(stationsByOneGroup.ToArray());
                cntRecalcContextStations += stationsByOneGroup.Count;
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
            var arrUniqueGSID = GetUniqueArrayGSIDfromDriveTests(contextDriveTestsResult, Standard);
            for (int i = 0; i < arrUniqueGSID.Length; i++)
            {
                var groupDriveTest = GroupingDriveTestsByStandardAndGSID(contextDriveTestsResult, arrUniqueGSID[i], Standard);
                lstDriveTestsResult.Add(groupDriveTest);
                lstAllDriveTestsResult.AddRange(groupDriveTest.ToArray());
            }

            var lstGroupDriveTestsResult = new List<DriveTestsResult>();
            for (int k = 0; k < arrUniqueGSID.Length; k++)
            {
                var driveTestsOne = lstDriveTestsResult[k][0];
                for (int j = 0; j < arrUniqueGSID.Length; j++)
                {
                    var driveTestsTwo = lstDriveTestsResult[j][0];
                    if (CompareGSID(driveTestsOne.GSID, driveTestsTwo.GSID, Standard))
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

            var cntRecalcDriveTest = 0;
            var lstRecalcDriveTestsResult = new List<DriveTestsResult[]>();
            var arrUniqueGroups = GetUniqueArrayGroupsFromDriveTests(lstGroupDriveTestsResult.ToArray());

            for (int k = 0; k < arrUniqueGroups.Length; k++)
            {
                var tempGroupDriveTestsResult = new List<DriveTestsResult>();
                for (int j = 0; j < arrUniqueGSID.Length; j++)
                {
                    if (!arrUniqueGroups.Contains(arrUniqueGSID[j]))
                    {
                        tempGroupDriveTestsResult.AddRange(lstAllDriveTestsResult.FindAll(x => x.GSID == arrUniqueGSID[j]));
                    }
                }

                var driveTestsByOneGroup = lstGroupDriveTestsResult.FindAll(x => x.NameGroupGlobalSID == arrUniqueGroups[k]);
                for (int n = 0; n < tempGroupDriveTestsResult.Count; n++)
                {
                    if (driveTestsByOneGroup.FindAll(x => x.Num == tempGroupDriveTestsResult[n].Num).Count == 0)
                    {
                        lstRecalcDriveTestsResult.Add(new DriveTestsResult[] { tempGroupDriveTestsResult[n] });
                        cntRecalcDriveTest += 1;
                    }
                }


                var orderByCountPoints = from z in driveTestsByOneGroup orderby z.CountPoints descending select z;
                var tempDriveTestsByOneGroup = orderByCountPoints.ToArray();
                lstRecalcDriveTestsResult.Add(tempDriveTestsByOneGroup);
                cntRecalcDriveTest += tempDriveTestsByOneGroup.Length;
            }

            return lstRecalcDriveTestsResult.ToArray();
        }

        /// <summary>
        /// Выделяем драйв тесты в отдельные группы без учета стандарта
        /// </summary>
        /// <param name="contextDriveTestsResult"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static DriveTestsResult[][] CompareDriveTestsWithoutStandards(DriveTestsResult[] contextDriveTestsResult)
        {
            var lstDriveTestsResult = new List<DriveTestsResult[]>();
            var lstAllDriveTestsResult = new List<DriveTestsResult>();
            var arrUniqueGSID = GetUniqueArrayGSIDfromDriveTests(contextDriveTestsResult);
            for (int i = 0; i < arrUniqueGSID.Length; i++)
            {
                var groupDriveTest = GroupingDriveTestsByGSID(contextDriveTestsResult, arrUniqueGSID[i]);
                lstDriveTestsResult.Add(groupDriveTest);
                lstAllDriveTestsResult.AddRange(groupDriveTest.ToArray());
            }

            var lstGroupDriveTestsResult = new List<DriveTestsResult>();
            for (int k = 0; k < arrUniqueGSID.Length; k++)
            {
                var driveTestsOne = lstDriveTestsResult[k][0];
                for (int j = 0; j < arrUniqueGSID.Length; j++)
                {
                    var driveTestsTwo = lstDriveTestsResult[j][0];
                    if (CompareGSIDSimple(driveTestsOne.GSID, driveTestsTwo.GSID))
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

            var cntRecalcDriveTest = 0;
            var lstRecalcDriveTestsResult = new List<DriveTestsResult[]>();
            var arrUniqueGroups = GetUniqueArrayGroupsFromDriveTests(lstGroupDriveTestsResult.ToArray());

            for (int k = 0; k < arrUniqueGroups.Length; k++)
            {
                var tempGroupDriveTestsResult = new List<DriveTestsResult>();
                for (int j = 0; j < arrUniqueGSID.Length; j++)
                {
                    if (!arrUniqueGroups.Contains(arrUniqueGSID[j]))
                    {
                        tempGroupDriveTestsResult.AddRange(lstAllDriveTestsResult.FindAll(x => x.GSID == arrUniqueGSID[j]));
                    }
                }

                var driveTestsByOneGroup = lstGroupDriveTestsResult.FindAll(x => x.NameGroupGlobalSID == arrUniqueGroups[k]);
                for (int n = 0; n < tempGroupDriveTestsResult.Count; n++)
                {
                    if (driveTestsByOneGroup.FindAll(x => x.Num == tempGroupDriveTestsResult[n].Num).Count == 0)
                    {
                        lstRecalcDriveTestsResult.Add(new DriveTestsResult[] { tempGroupDriveTestsResult[n] });
                        cntRecalcDriveTest += 1;
                    }
                }


                var orderByCountPoints = from z in driveTestsByOneGroup orderby z.CountPoints descending select z;
                var tempDriveTestsByOneGroup = orderByCountPoints.ToArray();
                lstRecalcDriveTestsResult.Add(tempDriveTestsByOneGroup);
                cntRecalcDriveTest += tempDriveTestsByOneGroup.Length;
            }

            return lstRecalcDriveTestsResult.ToArray();
        }

        /// <summary>
        /// Поиск соответствий между драйв тестами и станицями
        /// </summary>
        /// <param name="contextDriveTestsResult"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static LinkGoupDriveTestsAndStations[] CompareDriveTestAndStation(DriveTestsResult[] contextDriveTestsResult, ContextStation[] contextStations, string Standard, out DriveTestsResult[][] outDriveTestsResults, out ContextStation[][] outContextStations)
        {
            var lstLinkDriveTestsResultAndStation = new List<LinkGoupDriveTestsAndStations>();
            var forDeleteContextStation = new List<ContextStation>();
            var forDeleteDriveTestsResult = new List<DriveTestsResult>();

            var groupedDriveTests = CompareDriveTests(contextDriveTestsResult, Standard);
            var groupedStations = CompareStations(contextStations, Standard);
            for (int i = 0; i < groupedStations.Length; i++)
            {
                var stationsGSID = groupedStations[i][0].LicenseGsid;
                for (int m = 0; m < groupedDriveTests.Length; m++)
                {
                    var driveTestGSID = groupedDriveTests[m][0].GSID;
                    if (CompareGSID(driveTestGSID, stationsGSID, Standard))
                    {
                        var lnkDriveTest = new LinkGoupDriveTestsAndStations()
                        {
                            ContextStation = groupedStations[i],
                            DriveTestsResults = groupedDriveTests[m]
                        };

                        lstLinkDriveTestsResultAndStation.Add(lnkDriveTest);
                        forDeleteDriveTestsResult.AddRange(groupedDriveTests[m]);
                        forDeleteContextStation.AddRange(groupedStations[i]);
                    }
                }
            }
            var lstStations = groupedStations.ToList();
            for (int i = 0; i < forDeleteContextStation.Count; i++)
            {
                for (int j = 0; j < lstStations.Count; j++)
                {
                    var station = lstStations[j].ToList();
                    if (station.FindAll(x => x.Id == forDeleteContextStation[i].Id).Count > 0)
                    {
                        station.RemoveAll(x => x.Id == forDeleteContextStation[i].Id);
                        if (station.Count > 0)
                        {
                            lstStations[j] = station.ToArray();
                        }
                        else
                        {
                            lstStations.RemoveAt(j);
                        }
                        break;
                    }
                }
            }
            outContextStations = lstStations.ToArray();


            var lstDriveTests = groupedDriveTests.ToList();
            for (int i = 0; i < forDeleteDriveTestsResult.Count; i++)
            {
                for (int j = 0; j < lstDriveTests.Count; j++)
                {
                    var driveTest = lstDriveTests[j].ToList();
                    if (driveTest.FindAll(x => x.Num == forDeleteDriveTestsResult[i].Num).Count > 0)
                    {
                        driveTest.RemoveAll(x => x.Num == forDeleteDriveTestsResult[i].Num);
                        if (driveTest.Count > 0)
                        {
                            lstDriveTests[j] = driveTest.ToArray();
                        }
                        else
                        {
                            lstDriveTests.RemoveAt(j);
                        }
                        break;
                    }
                }
            }
            outDriveTestsResults = lstDriveTests.ToArray();
            return lstLinkDriveTestsResultAndStation.ToArray();
        }

        public static string[] GetUniqueArrayGroupsFromStations(ContextStation[] contextStations)
        {
            var stations = contextStations.ToList();
            var arrayNameGroupGlobalSID = stations.Select(x => x.NameGroupGlobalSID);
            return arrayNameGroupGlobalSID.Distinct().ToArray();
        }

        public static string[] GetUniqueArrayGroupsFromDriveTests(DriveTestsResult[] driveTestsResult)
        {
            var driveTestsRes = driveTestsResult.ToList();
            var arrayNameGroupGlobalSID = driveTestsRes.Select(x => x.NameGroupGlobalSID);
            return arrayNameGroupGlobalSID.Distinct().ToArray();
        }

        public static EpsgCoordinate CenterWeightAllCoordinates(PointFS[] pointFs)
        {
            return new EpsgCoordinate();
        }

    }
}
