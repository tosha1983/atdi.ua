﻿using Atdi.Platform;
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
                                    //Lat_DEC = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[0].ConvertStringToDouble().Value),
                                    //Lon_DEC = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[1].ConvertStringToDouble().Value),
                                    Lat_DEC = aa[0].ConvertStringToDouble().Value,
                                    Lon_DEC = aa[1].ConvertStringToDouble().Value,
                                    Level_dBm = aa[2].ConvertStringToDouble().Value
                                });
                                break;
                            }
                        }
                    }

                    var arrPnts = pointEarthGeometricslst.ToArray();


                    WPF.Location[] inputCoords = new WPF.Location[1];
                    for (int u = 0; u < arrPnts.Length; u++)
                    {
                        inputCoords[u] = new WPF.Location(arrPnts[u].Lon_DEC, arrPnts[u].Lat_DEC);
                        break;
                    }

                    WPF.RunApp.Start(WPF.TypeObject.Points, inputCoords, WPF.TypeObject.Points, null);
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                Console.WriteLine($"Press any key to stop test DeepServices GE06 ...");
                Console.ReadLine();
            }
        }
    }
}
