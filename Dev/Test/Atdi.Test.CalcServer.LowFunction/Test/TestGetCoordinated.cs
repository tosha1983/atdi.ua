using Atdi.Platform;
using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.AppUnits.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using Atdi.AppUnits.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.AppUnits.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Atdi.AppUnits.Sdrn.DeepServices.GN06;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using GE = Atdi.DataModels.Sdrn.DeepServices.GN06;
using WPF = Atdi.Test.DeepServices.Client.WPF;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;
using System;
using System.Collections.Generic;
using Atdi.Common;


namespace Atdi.Test.CalcServer.LowFunction
{
    public static class TestGetCoordinated
    {
        public static void Test()
        {

            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    int[] index_start_stop = new int[12];

                    index_start_stop[0] = 33500;
                    index_start_stop[1] = 45000;
                    index_start_stop[2] = 48500;
                    index_start_stop[3] = 62000;
                    index_start_stop[4] = 104000;
                    index_start_stop[5] = 117000;
                    index_start_stop[6] = 119500;
                    index_start_stop[7] = 135500;
                    index_start_stop[8] = 189000;
                    index_start_stop[9] = 201000;
                    index_start_stop[10] = 203500;
                    index_start_stop[11] = 219500;
                    ValueProsessing valueProsessing = ValueProsessing.Average;

                    host.Start();
                    string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "Save545.6MHz.dat");
                    List<Coordinated> pointEarthGeometricslst = new List<Coordinated>();
                    var str = System.IO.File.ReadAllText(fileName);
                    string[] a = str.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < a.Length; i++)
                    {
                        string[] aa = a[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if ((aa != null) && (aa.Length > 0))
                        {
                            for (int j = 0; j < aa.Length; j++)
                            {
                                pointEarthGeometricslst.Add(new Coordinated()
                                {
                                    Lat_DEC = aa[0].ConvertStringToDouble().Value,
                                    Lon_DEC = aa[1].ConvertStringToDouble().Value,
                                    Level_dBm = aa[2].ConvertStringToDouble().Value
                                });
                                break;
                            }
                        }
                    }
                    List<Coordinated> tracks = new List<Coordinated>();
                    List<int> extrs = new List<int>();
                    TestForm testForm = new TestForm();
                    for (int k = 0; index_start_stop.Length > k; k = k + 2)
                    {
                        var index_start = index_start_stop[k];
                        var index_stop = index_start_stop[k + 1];
                        // начало оботки данных
                        var pointEarthGeometricslst1 = convertmkVtodBm(ref pointEarthGeometricslst, index_start, index_stop);
                        var track = Filter(pointEarthGeometricslst1, valueProsessing, -100);
                        tracks.AddRange(track);
                        var extr = FoundMaxExtrim(track);
                        extrs.AddRange(extr);
                        // данные подчищены
                        double[] a1 = new double[track.Count * 2];
                        for (int i = 0; track.Count > i; i++)
                        {
                            //testForm.Arr1[2 * i] = arrPnts[i].Lon_DEC;
                            a1[2 * i] = track[i].Lat_DEC;
                            a1[2 * i + 1] = track[i].Level_dBm;
                        }
                        testForm.Arr1.Add(a1);
                    }
                    var arrPnts = tracks.ToArray();
                    WPF.Location[] inputCoords = new WPF.Location[arrPnts.Length];
                    for (int u = 0; u < arrPnts.Length; u++)
                    {
                            inputCoords[u] = new WPF.Location(arrPnts[u].Lon_DEC, arrPnts[u].Lat_DEC);
                    }
                    int point_start = 0;
                    int point_end = Math.Min(extrs.Count, extrs.Count);
                    WPF.Location[] inputCoords1 = new WPF.Location[point_end - point_start];
                    for (int i = 0; inputCoords1.Length > i; i++)
                    {
                        string value = arrPnts[extrs[point_start + i]].Level_dBm.ToString();
                        inputCoords1[i] = new WPF.Location(arrPnts[extrs[point_start + i]].Lon_DEC, arrPnts[extrs[point_start + i]].Lat_DEC, value);
                    }
                    WPF.RunApp.Start(WPF.TypeObject.Points, inputCoords, WPF.TypeObject.Points, inputCoords1);


                    testForm.ShowDialog();

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                Console.WriteLine($"Press any key to stop test DeepServices GE06 ...");
                Console.ReadLine();
            }
        }
        public static List<Coordinated> Filter(List<Coordinated> Points, ValueProsessing valueProsessing, double Noice)
        {
            // constant
            double Aaccuracy_DEC = 0.0001;
            // enad constant
            var curLon = Points[0].Lon_DEC;
            var curLat = Points[0].Lat_DEC;
            List<double> FS_value = new List<double>();
            FS_value.Add(Points[0].Level_dBm);
            List<Coordinated> outPoints = new List<Coordinated>();
            for (int i = 1; i < Points.Count; i++)
            {
                if ((Math.Abs(curLon - Points[i].Lon_DEC) > Aaccuracy_DEC) || (Math.Abs(curLat - Points[i].Lat_DEC) > Aaccuracy_DEC))
                {// переход + сохраняем старое
                    Coordinated coordinated = new Coordinated();
                    coordinated.Lon_DEC = curLon;
                    coordinated.Lat_DEC = curLat;
                    coordinated.Level_dBm = Get_Value(FS_value, valueProsessing, Noice);
                    outPoints.Add(coordinated);
                    curLon = Points[i].Lon_DEC;
                    curLat = Points[i].Lat_DEC;
                    FS_value = new List<double>();
                    FS_value.Add(Points[i].Level_dBm);
                }
                else
                {
                    FS_value.Add(Points[i].Level_dBm);
                }
            }
            Coordinated coordinated1 = new Coordinated();
            coordinated1.Lon_DEC = curLon;
            coordinated1.Lat_DEC = curLat;
            coordinated1.Level_dBm = Get_Value(FS_value, valueProsessing, Noice);
            outPoints.Add(coordinated1);
            return outPoints;
        }
        public static double Get_Value(List<double> arr, ValueProsessing valueProsessing, double Noice)
        {
            var Min = arr[0];
            var Max = arr[0];
            double Av = 0;
            int count = 0;
            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i] > Max) { Max = arr[i]; }
                if (arr[i] < Min) { Min = arr[i]; }
                if (arr[i] >= Noice) { Av = Av + arr[i]; count++; }
            }
            Av = Av / count;
            if (Max - Min > 20)
            {

            }
            if (valueProsessing == ValueProsessing.Max) { return Max; }
            return Av;
        }
        public enum ValueProsessing
        {
            Average,
            Max,
        }
        public static List<Coordinated> convertmkVtodBm(ref List<Coordinated> arr, int start_index, int stop_index)
        {
            List<Coordinated> outArr = new List<Coordinated>();
            if (stop_index > arr.Count) { stop_index = arr.Count; }
            for (int i = start_index; stop_index > i; i++)
            {
                var coord = arr[i];
                coord.Level_dBm = coord.Level_dBm - 106.9;
                outArr.Add(coord);
            }
            return outArr;
        }
        public static List<int> FoundMaxExtrim(List<Coordinated> Points, int number_points = 1)
        {
            bool goToMax = true;
            List<int> extr = new List<int>();
            double CurMin= Points[0].Level_dBm; double CurMax=Points[0].Level_dBm;
            int indexCurMax =0; int indexCurMin = 0;
            int countAfterMax=0; int countAfterMin=0;
            for (int i = 0; Points.Count > i; i++)
            {
                if (goToMax)
                {// движение вверх 
                    if (CurMax <= Points[i].Level_dBm)
                    { CurMax = Points[i].Level_dBm;
                        indexCurMax = i;
                        countAfterMax = 0; }
                    else
                    {
                        countAfterMax++;
                        if (countAfterMax > number_points)
                        {
                            goToMax = false;
                            extr.Add(indexCurMax);
                            CurMin = CurMax;
                            i = indexCurMax;
                        }
                    }
                }
                else
                { // движение вниз
                    if (CurMin >= Points[i].Level_dBm)
                    {
                        CurMin = Points[i].Level_dBm;
                        indexCurMin = i;
                        countAfterMin = 0;
                    }
                    else
                    {
                        countAfterMin++;
                        if (countAfterMin > number_points)
                        {
                            goToMax = true;
                            extr.Add(indexCurMin);
                            CurMax = CurMin;
                            i = indexCurMin;
                        }
                    }
                }
            }
            return extr;
        }
    }
}
