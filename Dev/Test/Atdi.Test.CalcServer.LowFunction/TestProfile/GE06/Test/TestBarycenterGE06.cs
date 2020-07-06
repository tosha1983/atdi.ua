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
    public static class TestBarycenterGE06
    {
        public static void Test()
        {

            using (var host = PlatformConfigurator.BuildHost())
            {
                try
                {
                    host.Start();
                    host.Container.Register<IIdwmService, IdwmService>(ServiceLifetime.PerThread);
                    host.Container.Register<ITransformation, TransformationService>(ServiceLifetime.PerThread);
                    host.Container.Register<IEarthGeometricService, EarthGeometricService>(ServiceLifetime.PerThread);
                    //host.Container.Register<IGn06Service, EstimationAssignmentsService>(ServiceLifetime.PerThread);
                    var resolver = host.Container.GetResolver<IServicesResolver>();
                    var gn06Service = resolver.Resolve<IGn06Service>();



                    string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "AreaTestGe06BaryCenter.txt");

                    List<GE.AreaPoint> pointEarthGeometricslst = new List<GE.AreaPoint>();
                    var str = System.IO.File.ReadAllText(fileName);
                    string[] a = str.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < a.Length; i++)
                    {
                        string[] aa = a[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if ((aa != null) && (aa.Length > 0))
                        {
                            for (int j = 0; j < aa.Length; j++)
                            {
                                pointEarthGeometricslst.Add(new GE.AreaPoint()
                                {
                                    Lon_DEC = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[0].ConvertStringToDouble().Value),
                                    Lat_DEC = Atdi.AppUnits.Sdrn.DeepServices.RadioSystem.AntennaPattern.Position.DmsToDec(aa[1].ConvertStringToDouble().Value),
                                });
                                break;
                            }
                        }
                    }

                    var arrPnts = pointEarthGeometricslst.ToArray();

                    // нужно заполнить
                    BroadcastingCalcBarycenterGE06 AllotmAsssigments = new BroadcastingCalcBarycenterGE06();
                    AllotmAsssigments.BroadcastingAllotment = new  GE.BroadcastingAllotment()
                    {
                         AdminData = new GE.AdministrativeData()
                         {
                             Action = GE.ActionType.Add,
                             Adm = "F",
                             Fragment = "",
                             NoticeType = "",
                             AdmRefId = ""
                         },
                          AllotmentParameters = new GE.AllotmentParameters()
                          {
                               Name="Test",
                              Contur = arrPnts,
                          },
                           EmissionCharacteristics = new GE.BroadcastingAllotmentEmissionCharacteristics()
                           {
                                Polar =  GE.PolarType.H,
                           }
                    };
                    AllotmAsssigments.BroadcastingAssignments = new GE.BroadcastingAssignment[3]
                    {
                         new GE.BroadcastingAssignment()
                         {
                              AdmData = new GE.AdministrativeData()
                              {
                                   Action = GE.ActionType.Add,
                                   Adm = "F"
                              },
                               SiteParameters = new GE.SiteParameters()
                               {
                                    Name = "Name",
                                     Lon_Dec = -1.151966,
                                     Lat_Dec = 54.142403
                               }
                         },
                          new GE.BroadcastingAssignment()
                         {
                              AdmData = new GE.AdministrativeData()
                              {
                                   Action = GE.ActionType.Add,
                                   Adm = "F"
                              },
                               SiteParameters = new GE.SiteParameters()
                               {
                                    Name = "Name",
                                     Lon_Dec = 0.15359547,
                                     Lat_Dec = 52.644847
                               }
                         },
                           new GE.BroadcastingAssignment()
                         {
                              AdmData = new GE.AdministrativeData()
                              {
                                   Action = GE.ActionType.Add,
                                   Adm = "F"
                              },
                               SiteParameters = new GE.SiteParameters()
                               {
                                    Name = "Name",
                                     Lon_Dec = -0.59518244,
                                     Lat_Dec = 49.611336
                               }
                         }
                    };

                    // конец создания елотментов и асаймента

                    // сама функция расчета барицентра
                    PointEarthGeometric pointEarthGeometric = new PointEarthGeometric();
                    //var idwmServices = resolver.Resolve<IIdwmService>();

                    //var adm = idwmServices.GetADMByPoint(new Point() { Latitude_dec = 45, Longitude_dec = 0 });
                    //AllotmAsssigments.BroadcastingAllotment.AdminData.Adm = adm;
                    gn06Service.CalcBarycenterGE06(AllotmAsssigments, ref pointEarthGeometric);
                    


                    WPF.Location[] inputCoords = new WPF.Location[arrPnts.Length];
                    for (int u = 0; u < arrPnts.Length; u++)
                    {
                        inputCoords[u] = new WPF.Location(arrPnts[u].Lon_DEC, arrPnts[u].Lat_DEC);
                    }
                    //inputCoords[arrPnts.Length] = new WPF.Location(-1.151966, 54.142403);
                    //inputCoords[arrPnts.Length+1] = new WPF.Location(0.15359547, 52.644847);
                    //inputCoords[arrPnts.Length+2] = new WPF.Location(-0.59518244, 49.611336);
                    WPF.Location[] outputCoords = new WPF.Location[1]
                    {
                         new WPF.Location(pointEarthGeometric.Longitude, pointEarthGeometric.Latitude),
                    };
                    WPF.RunApp.Start(WPF.TypeObject.Polygon, inputCoords, WPF.TypeObject.Points, outputCoords);
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
