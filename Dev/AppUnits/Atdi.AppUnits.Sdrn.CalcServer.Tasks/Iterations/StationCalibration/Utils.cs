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

        public static bool CompareGSID(string GSID1, string GSID2, string Standard)
        {
            if (GSID1 == GSID2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод для перфорации драйв тестов
        /// </summary>
        /// <param name="driveTestsResults"></param>
        public static void PerforationDriveTestResults(ref DriveTestsResult[] driveTestsResults)
        {

        }

        /// <summary>
        /// Метод для перфорации точек драйв тестов
        /// </summary>
        /// <param name="pointFs"></param>
        public static void PerforationPoints(ref PointFS[] pointFs)
        {

        }


        /// <summary>
        /// Получить весь массив уникальных STANDARDS из станций
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayStandardsfromStations(in ContextStation[] contextStations)
        {
            var arrayStandards = contextStations.Select(x => x.Standard);
            return arrayStandards.Distinct().ToArray();
        }

        /// <summary>
        /// Получить весь массив уникальных STANDARDS из драйв тестов
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayStandardsFromDriveTests(in DriveTestsResult[] driveTests)
        {
            var arrayStandards = driveTests.Select(x => x.Standard);
            return arrayStandards.Distinct().ToArray();
        }


        /// <summary>
        /// Получить массив уникальных GSID из станций, по заданному стандарту
        /// </summary>
        /// <param name="contextStations"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromStations(in ContextStation[] contextStations, string Standard)
        {
            var stations = contextStations.ToList();
            var fndStations = stations.FindAll(x => x.Standard == Standard);
            var arrayGSIDByICSM = fndStations.Select(x => x.GlobalSIDByICSM);
            return arrayGSIDByICSM.Distinct().ToArray();
        }

        /// <summary>
        /// Получить весь массив уникальных GSID из драйв тестов, по заданному стандарту
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromDriveTests(in DriveTestsResult[] driveTests, string Standard)
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
        public static string[] GetUniqueArrayGSIDfromStations(in ContextStation[] contextStations)
        {
            var arrayGSIDByICSM = contextStations.Select(x => x.GlobalSIDByICSM);
            return arrayGSIDByICSM.Distinct().ToArray();
        }

        /// <summary>
        /// Получить весь массив уникальных GSID из драйв тестов
        /// </summary>
        /// <param name="driveTests"></param>
        /// <returns></returns>
        public static string[] GetUniqueArrayGSIDfromDriveTests(in DriveTestsResult[] driveTests)
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
        public static ContextStation[] PerforationStations(in ContextStation[] contextStations)
        {
            var listStations = contextStations.ToList();
            var globalSIDByICSM = GetUniqueArrayGSIDfromStations(contextStations);
            for (int j = 0; j < globalSIDByICSM.Length; j++)
            {
                var fndStations = listStations.FindAll(x => x.GlobalSIDByICSM == globalSIDByICSM[j]);
                if ((fndStations != null) && (fndStations.Count > 0))
                {
                    var allStatusByStandard = fndStations.Select(x => x.Status);
                    var cntUniqueStatus = allStatusByStandard.Distinct().ToArray();
                    if (cntUniqueStatus.Length > 1)
                    {
                        listStations.RemoveAll(x => x.Status == StationStatus.I && x.GlobalSIDByICSM == globalSIDByICSM[j]);
                    }
                }
            }
            return listStations.ToArray();
        }


        /// <summary>
        /// Массив драйв тестов, сгруппированный на основании GSID и заданному стандарту
        /// </summary>
        /// <param name="driveTests"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static DriveTestsResult[] GroupingDriveTestsByStandardAndGSID(in DriveTestsResult[] driveTests, string GSID, string Standard)
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
        public static ContextStation[] GroupingStationsByStandardAndGSID(in ContextStation[] contextStations, string GSID, string Standard)
        {
            var lstContextStations = new List<ContextStation>();
            var listStations = contextStations.ToList();
            var fndStations = listStations.FindAll(x => x.GlobalSIDByICSM == GSID && x.Standard == Standard);
            if ((fndStations != null) && (fndStations.Count > 0))
            {
                lstContextStations.AddRange(fndStations);
            }
            return lstContextStations.ToArray();
        }


        /// <summary>
        /// Формирование групп станций на основе последовательного вызова метода CompareGSID для сравнения станций между собой
        /// </summary>
        /// <param name="contextStations"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static ContextStation[][] CompareStations(ContextStation[] contextStations, string Standard)
        {
            var lstContextStations = new List<ContextStation[]>();
            var lstAllStations = new List<ContextStation>();
            var arrUniqueGSID =  GetUniqueArrayGSIDfromStations(contextStations, Standard);
            for (int i=0; i< arrUniqueGSID.Length; i++)
            {
                var groupContextStations = GroupingStationsByStandardAndGSID(contextStations, arrUniqueGSID[i], Standard);
                lstContextStations.Add(groupContextStations);
                lstAllStations.AddRange(groupContextStations.ToArray());
            }

            var lstGroupContextStations = new List<ContextStation>();

            var Groups = new List<string>();
            for (int k = 0; k < arrUniqueGSID.Length; k++)
            {
                for (int l = 0; l < lstContextStations[k].Length; l++)
                {
                    var stationOne = lstContextStations[k][l];
                    for (int j = 0; j < arrUniqueGSID.Length; j++)
                    {
                        for (int r = 0; r < lstContextStations[j].Length; r++)
                        {
                            var stationTwo = lstContextStations[j][r];
                            if (CompareGSID(stationOne.GlobalSIDByICSM, stationTwo.GlobalSIDByICSM, Standard))
                            {
                                stationOne.NameGroupGlobalSID = stationOne.GlobalSIDByICSM;
                                stationTwo.NameGroupGlobalSID = stationOne.GlobalSIDByICSM;

                                if ((lstGroupContextStations.FindAll(x => x.Id == stationOne.Id).Count == 0))
                                {
                                    lstGroupContextStations.Add(stationOne);
                                }
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
                    if (!arrUniqueGroups.Contains(arrUniqueGSID[k]))
                    {
                        tempGroupStations.AddRange(lstAllStations.FindAll(x => x.GlobalSIDByICSM == arrUniqueGSID[k]));
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

            //if (lstAllStations.Count != cntRecalcContextStations)
            //{
            //    throw new Exception();
            //}
            return lstRecalcContextStations.ToArray();
        }

        /// <summary>
        /// Выделяем драйв тесты в отдельные группы
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

            var Groups = new List<string>();
            for (int k = 0; k < arrUniqueGSID.Length; k++)
            {
                for (int l = 0; l < lstDriveTestsResult[k].Length; l++)
                {
                    var driveTestsResultOne = lstDriveTestsResult[k][l];
                    for (int j = 0; j < arrUniqueGSID.Length; j++)
                    {
                        for (int r = 0; r < lstDriveTestsResult[j].Length; r++)
                        {
                            var driveTestsTwo = lstDriveTestsResult[j][r];
                            if (CompareGSID(driveTestsResultOne.GSID, driveTestsTwo.GSID, Standard))
                            {
                                driveTestsResultOne.NameGroupGlobalSID = driveTestsResultOne.GSID;
                                driveTestsTwo.NameGroupGlobalSID = driveTestsResultOne.GSID;

                                if (lstGroupDriveTestsResult.FindAll(x => x.Num == driveTestsResultOne.Num).Count == 0)
                                {
                                    lstGroupDriveTestsResult.Add(driveTestsResultOne);
                                }
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
                    if (!arrUniqueGroups.Contains(arrUniqueGSID[k]))
                    {
                        tempGroupDriveTestsResult.AddRange(lstAllDriveTestsResult.FindAll(x => x.GSID == arrUniqueGSID[k]));
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

            //if (lstAllDriveTestsResult.Count!= cntRecalcDriveTest)
            //{
                //throw new Exception();
            //}

            return lstRecalcDriveTestsResult.ToArray();
        }


        /// <summary>
        /// Поиск соответствий между драйв тестами и станицями
        /// </summary>
        /// <param name="contextDriveTestsResult"></param>
        /// <param name="Standard"></param>
        /// <returns></returns>
        public static LinkDriveTestsResultAndStation[] CompareDriveTestAndStation(DriveTestsResult[] contextDriveTestsResult,  ContextStation[] contextStations, string Standard)
        {
            var lstLinkDriveTestsResultAndStation = new List<LinkDriveTestsResultAndStation>();
            var forDeleteContextStation = new List<ContextStation>();
            var forDeleteDriveTestsResult = new List<DriveTestsResult>();
            
            var groupedDriveTests = CompareDriveTests(contextDriveTestsResult, Standard);
            var groupedStations = CompareStations(contextStations, Standard);
            for (int i = 0; i < groupedStations.Length; i++)
            {
                var stationsGSID = groupedStations[i][0].GlobalSIDByICSM;
                
                for (int m = 0; m < groupedDriveTests.Length; m++)
                {
                    var driveTestGSID = groupedDriveTests[m][0].GSID;
                    if (CompareGSID(driveTestGSID, stationsGSID, Standard))
                    {
                        var lnkDriveTest = new LinkDriveTestsResultAndStation()
                        {
                            ContextStation = groupedStations[i][0],
                            DriveTestsResults = groupedDriveTests[m]
                        };
                        if (lstLinkDriveTestsResultAndStation.FindAll(x => x.ContextStation.Id == groupedStations[i][0].Id).Count == 0)
                        {
                            lstLinkDriveTestsResultAndStation.Add(lnkDriveTest);

                            for (int f = 0; f < groupedStations[i].Count(); f++)
                            {
                                if (forDeleteContextStation.FindAll(x=>x.Id== groupedStations[i][f].Id)==null)
                                {

                                }
                            }
                        }
                    }
                }
            }
            return lstLinkDriveTestsResultAndStation.ToArray();
        }

        public static string[] GetUniqueArrayGroupsFromStations(in ContextStation[] contextStations)
        {
            var stations = contextStations.ToList();
            var arrayNameGroupGlobalSID = stations.Select(x => x.NameGroupGlobalSID);
            return arrayNameGroupGlobalSID.Distinct().ToArray();
        }

        public static string[] GetUniqueArrayGroupsFromDriveTests(in DriveTestsResult[] driveTestsResult)
        {
            var driveTestsRes = driveTestsResult.ToList();
            var arrayNameGroupGlobalSID = driveTestsRes.Select(x => x.NameGroupGlobalSID);
            return arrayNameGroupGlobalSID.Distinct().ToArray();
        }
    }
}
