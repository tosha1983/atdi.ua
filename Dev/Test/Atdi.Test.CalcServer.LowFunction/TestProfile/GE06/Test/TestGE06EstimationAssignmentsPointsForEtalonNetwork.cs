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
using System;

namespace Atdi.Test.CalcServer.LowFunction
{
    public class TestGE06EstimationAssignmentsPointsForEtalonNetwork
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

                    //START DATA
                    GE.RefNetworkType RefNetwork = GE.RefNetworkType.RN1;
                    GE.RefNetworkConfigType RefNetworkConfig = GE.RefNetworkConfigType.RPC2;
                    double PointLon = 3.775327;
                    double PointLat = 53.26631;
                    double AllotmentPointLon = 0.0808333;
                    double AllotmentPointLat = 49.35699444;
                    //

                    GE.BroadcastingAllotment broadcastingAllotment = new GE.BroadcastingAllotment();
                    broadcastingAllotment.EmissionCharacteristics = new GE.BroadcastingAllotmentEmissionCharacteristics()
                    { RefNetwork = RefNetwork, RefNetworkConfig = RefNetworkConfig };
                    GE.AreaPoint Point = new GE.AreaPoint() { Lat_DEC = PointLat, Lon_DEC = PointLon };
                    GE.AreaPoint AllotmentPoint = new GE.AreaPoint() { Lat_DEC = AllotmentPointLat, Lon_DEC = AllotmentPointLon };



                    EstimationAssignmentsPointsArgs estimationAssignmentsPointsArgs = new EstimationAssignmentsPointsArgs();
                    estimationAssignmentsPointsArgs.BroadcastingAllotment = broadcastingAllotment;
                    estimationAssignmentsPointsArgs.PointAllotment = AllotmentPoint;
                    estimationAssignmentsPointsArgs.PointCalcFieldStrength = Point;
                    PointsWithAzimuthResult pointWithAzimuthResult = new PointsWithAzimuthResult();
                    pointWithAzimuthResult.PointsWithAzimuth = new GE.PointWithAzimuth[7];

                    gn06Service.EstimationAssignmentsPointsForEtalonNetwork(in estimationAssignmentsPointsArgs, ref pointWithAzimuthResult);
                    // на карту 
                    WPF.Location[] InputData = new WPF.Location[2] { new WPF.Location(Point.Lon_DEC, Point.Lat_DEC), new WPF.Location(AllotmentPoint.Lon_DEC, AllotmentPoint.Lat_DEC) };
                    WPF.Location[] OutputData = new WPF.Location[pointWithAzimuthResult.sizeResultBuffer];
                    for (int j = 0; pointWithAzimuthResult.sizeResultBuffer > j; j++)
                    { OutputData[j] = new WPF.Location(pointWithAzimuthResult.PointsWithAzimuth[j].AreaPoint.Lon_DEC, pointWithAzimuthResult.PointsWithAzimuth[j].AreaPoint.Lat_DEC); }
                    WPF.RunApp.Start(WPF.TypeObject.Points, InputData, WPF.TypeObject.Points, OutputData);

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
