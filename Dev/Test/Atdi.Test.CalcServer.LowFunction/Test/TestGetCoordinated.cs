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
                    host.Start();
                    string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "Save545.2MHz.dat");
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
                    // начало оботки данных
                    pointEarthGeometricslst = convertmkVtodBm(ref pointEarthGeometricslst);
                    var track = Filter(pointEarthGeometricslst, ValueProsessing.Max, -100);
                    // данные подчищены

                    var arrPnts = track.ToArray();
                    WPF.Location[] inputCoords = new WPF.Location[arrPnts.Length];
                    WPF.Location[] inputCoords1 = new WPF.Location[1];
                    inputCoords1[0] = new WPF.Location(arrPnts[0].Lon_DEC, arrPnts[0].Lat_DEC, "Test");
                    for (int u = 0; u < arrPnts.Length; u++)
                    {
                        inputCoords[u] = new WPF.Location(arrPnts[u].Lon_DEC, arrPnts[u].Lat_DEC);
                    }
                    WPF.RunApp.Start(WPF.TypeObject.Points, inputCoords, WPF.TypeObject.Points, inputCoords1);

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
            if (Max - Min > 10)
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
        public static List<Coordinated> convertmkVtodBm(ref List<Coordinated> arr)
        {
            List<Coordinated> outArr = new List<Coordinated>();
            for (int i = 0; arr.Count > i; i++)
            {
                var coord = arr[i];
                coord.Level_dBm = coord.Level_dBm - 106.9;
                outArr.Add(coord);
            }
            return outArr;
        }
        public static List<int> FoundExtrim(List<Coordinated> Points)
        {  
        return null;
        }


    }
}
