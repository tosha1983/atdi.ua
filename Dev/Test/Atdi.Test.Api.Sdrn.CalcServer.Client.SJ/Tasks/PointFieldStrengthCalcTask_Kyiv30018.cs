using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.Tasks
{
    public static class PointFieldStrengthCalcTaskKyiv30018
    {
        private static readonly string OwnerInstance = "Atdi.Test.Api.Sdrn.CalcServer.Client";
        private static readonly Guid OwnerProjectId = Guid.Parse("d72ff52a-b655-4d28-8eff-b93f8e00ee15");
        private static readonly Guid OwnerMapId = Guid.Parse("d42ff53f-a652-4d28-8eff-b93f8e00ee3f");
        //private static readonly string MainProjectMap = "Lviv";
        private static readonly string MainProjectMap = "Kyiv5";

        private struct PointOnMap
        {
            public double Lat_dec;
            public double Lon_dec;
            public double Hei_m;
        }



        //// 5-6 land 101.7km

        private static double[] sitesLatitudes = new double[] { 50.5275 };
        private static double[] sitesLongitudes = new double[] { 30.620833 };
        private static double[] sitesHeights = new double[] { 74 };

        //private static double[] pointsLatitudes = new double[] { 50.470088, 50.474911, 50.471063 };
        //private static double[] pointsLongitudes = new double[] { 30.642755, 30.610237, 30.642072 };
        //private static double[] pointsHeights = new double[] { 2, 2, 2};

        //// j-746
        private static double[] pointsLatitudes = new double[] { 50.474911 };
        private static double[] pointsLongitudes = new double[] { 30.610237 };
        private static double[] pointsHeights = new double[] { 2 };

        //// j-0
        //private static double[] pointsLatitudes = new double[] { 50.474911 };
        //private static double[] pointsLongitudes = new double[] { 30.610237 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-2
        //private static double[] pointsLatitudes = new double[] { 50.470088 };
        //private static double[] pointsLongitudes = new double[] { 30.642755 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-4
        //private static double[] pointsLatitudes = new double[] { 50.471063 };
        //private static double[] pointsLongitudes = new double[] { 30.642072 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-18
        //private static double[] pointsLatitudes = new double[] { 50.474388 };
        //private static double[] pointsLongitudes = new double[] { 30.639722 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-21
        //private static double[] pointsLatitudes = new double[] { 50.474877 };
        //private static double[] pointsLongitudes = new double[] { 30.639415 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-31
        //private static double[] pointsLatitudes = new double[] { 50.479714 };
        //private static double[] pointsLongitudes = new double[] { 30.636214 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-38
        //private static double[] pointsLatitudes = new double[] { 50.483896 };
        //private static double[] pointsLongitudes = new double[] { 30.638402 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-59
        //private static double[] pointsLatitudes = new double[] { 50.486531 };
        //private static double[] pointsLongitudes = new double[] { 30.644121 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-96
        //private static double[] pointsLatitudes = new double[] { 50.489093 };
        //private static double[] pointsLongitudes = new double[] { 30.635182 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-118
        //private static double[] pointsLatitudes = new double[] { 50.487784 };
        //private static double[] pointsLongitudes = new double[] { 30.630524 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-124
        //private static double[] pointsLatitudes = new double[] { 50.486854 };
        //private static double[] pointsLongitudes = new double[] { 30.631276 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-161
        //private static double[] pointsLatitudes = new double[] { 50.475243 };
        //private static double[] pointsLongitudes = new double[] { 30.619875 };
        //private static double[] pointsHeights = new double[] { 2 };

        //// j-182
        //private static double[] pointsLatitudes = new double[] { 50.482784 };
        //private static double[] pointsLongitudes = new double[] { 30.614774 };
        //private static double[] pointsHeights = new double[] { 2 };

        public static void Run(WebApiDataLayer dataLayer, IQueryExecutor executor)
        {

            try
            {
                // определяем проект
                var projectId = DefineProject(dataLayer, executor);

                // определяем карту
                var mapId = DefineMap(dataLayer, executor, projectId);

                //если мы проект менять больше не собираемся, делаем проект доступным
                //MakeProjectAvailable(dataLayer, executor, projectId);

                Console.WriteLine("Press any keys to continue...");

                // определяем контекст
                var ownerClientContextId = Guid.NewGuid();
                var clientContextId = DefineClientContext(dataLayer, executor, projectId, ownerClientContextId);

                // станции из контекста
                var stations = GetClientContextStations(dataLayer, executor, clientContextId);

                // запускаем задачи для каждой станции
                for (int i = 0; i < pointsHeights.Length; i++)
                {
                    var stationId = stations[0];

                    Console.WriteLine();
                    Console.WriteLine($" Calculation for station with ID #{stationId}");

                    PointOnMap pointOnMapFS = new PointOnMap
                    {
                        Lat_dec = pointsLatitudes[i],
                        Lon_dec = pointsLongitudes[i],
                        Hei_m = pointsHeights[i]
                    };

                    // создаем новый таск в рамках контекста и первой станции из набора
                    var calcTaskId = CreateCalcTask(dataLayer, executor, clientContextId, stationId, pointOnMapFS);

                    // делаем задачу доступной для расчета
                    MakeCalcTaskAvailable(dataLayer, executor, calcTaskId, clientContextId);

                    // создаем запись для результатов
                    var calcResultId = CreateCalcTaskResult(dataLayer, executor, calcTaskId);

                    // запускаем расчет
                    RunCalcTask(dataLayer, executor, calcTaskId, calcResultId);

                    // ожидаем результат
                    var calcResultObject = WaitForCalcResult(dataLayer, executor, calcTaskId, calcResultId);

                    if (calcResultObject != null)
                    {
                        // выводим результат
                        Console.WriteLine($" Task Result ID #{calcResultObject.ResultId}");
                        Console.WriteLine($" StatusCode: {calcResultObject.RESULT.StatusCode}");
                        Console.WriteLine($" StatusName: {calcResultObject.RESULT.StatusName}");
                        Console.WriteLine($" StatusNote: {calcResultObject.RESULT.StatusNote}");
                        Console.WriteLine($"CreatedDate: {calcResultObject.RESULT.CreatedDate}");
                        Console.WriteLine($"  StartTime: {calcResultObject.RESULT.StartTime}");
                        Console.WriteLine($" FinishTime: {calcResultObject.RESULT.FinishTime}");
                        Console.WriteLine($" -- calculation result -- ");
                        Console.WriteLine($"   FS_dBuVm: {calcResultObject.FS_dBuVm}");
                        Console.WriteLine($"  Level_dBm: {calcResultObject.Level_dBm}");
                    }
                    else
                    {
                        Console.WriteLine($" Task Result ID #{calcResultId}");
                        Console.WriteLine($" -- calculation result -- ");
                        Console.WriteLine($"    <no result> ");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }



        private static long DefineProject(WebApiDataLayer dataLayer, IQueryExecutor executor)
        {
            var selQuery = dataLayer.GetBuilder<IProject>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.OwnerProjectId, OwnerProjectId);

            var projectId = executor.ExecuteAndFetch(selQuery, reader =>
            {
                if (reader.Count == 0 || !reader.Read())
                {
                    return (long)0;
                }

                var id = reader.GetValue(c => c.Id);
                Console.WriteLine($"Found the project with ID #{id}");

                return id;
            });

            if (projectId == 0)
            {
                var insQuery = dataLayer.GetBuilder<IProject>()
                    .Create()
                    .SetValue(c => c.Name, "Test project: PointFieldStrengthCalcTask")
                    .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                    .SetValue(c => c.OwnerInstance, OwnerInstance)
                    .SetValue(c => c.OwnerProjectId, OwnerProjectId)
                    .SetValue(c => c.StatusCode, (byte)ProjectStatusCode.Available)
                    .SetValue(c => c.StatusName, "Available")
                    .SetValue(c => c.StatusNote, "The new project was created")
                    .SetValue(c => c.Projection, "4UTN36")
                    .SetValue(c => c.Note,
                        "A test project was created to test the problem of calculating field strength");
                var projectPk = executor.Execute<IProject_PK>(insQuery);
                projectId = projectPk.Id;
                Console.WriteLine($"Created project with ID #{projectId}");
            }
            return projectId;
        }

        //private static void MakeProjectAvailable(WebApiDataLayer dataLayer, IQueryExecutor executor, long projectId)
        //{
        //    var updQuery = dataLayer.GetBuilder<IProject>()
        //        .Update()
        //        .SetValue(c => c.StatusCode, (byte)ProjectStatusCode.Available)
        //        .SetValue(c => c.StatusName, "Available")
        //        .SetValue(c => c.StatusNote, "The project was made available")
        //        // один вариант фильтра
        //        .Filter(c => c.OwnerInstance, OwnerInstance)
        //        .Filter(c => c.OwnerProjectId, OwnerProjectId)
        //        // второй вариант фильтра
        //        .Filter(c => c.Id, projectId);

        //    var count = executor.Execute(updQuery);
        //    if (count == 0)
        //    {
        //        throw new InvalidOperationException($"Can't make the project with ID #{projectId} Available");
        //    }
        //    Console.WriteLine($"The project with ID #{projectId} was made available");
        //}

        private static long DefineMap(WebApiDataLayer dataLayer, IQueryExecutor executor, long projectId)
        {
            var selQuery = dataLayer.GetBuilder<IProjectMap>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.OwnerMapId, OwnerMapId)
                .Filter(c => c.PROJECT.Id, projectId)
                .Filter(c => c.MapName, MainProjectMap);

            var mapId = executor.ExecuteAndFetch(selQuery, reader =>
            {
                if (reader.Count == 0 || !reader.Read())
                {
                    return (long)0;
                }

                var id = reader.GetValue(c => c.Id);
                Console.WriteLine($"Found the project map with ID #{id}");
                return id;
            });

            // 253 831.1563 5 681 087
            // 352599.9688 5597424


            // 98768.8125 - 83663
            // 50  1975.37625 - 1673.26
            // 20  4938.440625 - 4183.15


            if (mapId == 0)
            {
                var insQuery = dataLayer.GetBuilder<IProjectMap>()
                    .Create()
                    .SetValue(c => c.MapName, MainProjectMap)
                    .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                    .SetValue(c => c.OwnerInstance, OwnerInstance)
                    .SetValue(c => c.OwnerMapId, OwnerMapId)
                    .SetValue(c => c.StatusCode, (byte)ProjectMapStatusCode.Created)
                    .SetValue(c => c.StatusName, "Created")
                    .SetValue(c => c.StatusNote, "The new map was created")
                    .SetValue(c => c.PROJECT.Id, projectId)
                    .SetValue(c => c.MapNote,
                        "A test project map was created to test the problem of calculating field strength")
                    .SetValue(c => c.UpperLeftX, 302558)
                    .SetValue(c => c.UpperLeftY, 5607742)
                    .SetValue(c => c.StepUnit, "M")
                    .SetValue(c => c.AxisXNumber, 8807)
                    .SetValue(c => c.AxisXStep, 5)
                    .SetValue(c => c.AxisYNumber, 8699)
                    .SetValue(c => c.AxisYStep, 5)
                    .SetValue(c => c.OwnerUpperLeftX, 0)
                    .SetValue(c => c.OwnerUpperLeftY, 0)
                    .SetValue(c => c.OwnerAxisXNumber, 0)
                    .SetValue(c => c.OwnerAxisXStep, 0)
                    .SetValue(c => c.OwnerAxisYNumber, 0)
                    .SetValue(c => c.OwnerAxisYStep, 0)
                    .SetValue(c => c.SourceTypeCode, (byte)1)
                    .SetValue(c => c.SourceTypeName, "LocalStorage")
                    ;
                var projectMapPk = executor.Execute<IProjectMap_PK>(insQuery);
                mapId = projectMapPk.Id;

                // меняем статус
                var updQuery = dataLayer.GetBuilder<IProjectMap>()
                    .Update()
                    .SetValue(c => c.StatusCode, (byte)ProjectMapStatusCode.Pending)
                    .SetValue(c => c.StatusName, "Pending")
                    .SetValue(c => c.StatusNote, "")
                    .Filter(c => c.Id, mapId);

                executor.Execute(updQuery);

                Console.WriteLine($"Expectations for calculating map data with ID #{mapId}...");

                // ожидаем расчет карты
                var cancel = false;
                while (!cancel)
                {
                    System.Threading.Thread.Sleep(5 * 1000);

                    var checkQuery = dataLayer.GetBuilder<IProjectMap>()
                        .Read()
                        .Select(c => c.StatusCode)
                        .Select(c => c.StatusNote)
                        .Filter(c => c.Id, mapId);

                    cancel = executor.ExecuteAndFetch(checkQuery, reader =>
                    {
                        if (reader.Count == 0 || !reader.Read())
                        {
                            throw new InvalidOperationException($"A map not found by ID #{mapId}");
                        }

                        var status = (ProjectMapStatusCode)reader.GetValue(c => c.StatusCode);
                        var statusNote = reader.GetValue(c => c.StatusNote);

                        Console.WriteLine($"Checked the map status with ID #{mapId}: {status}, '{statusNote}'");

                        if (status == ProjectMapStatusCode.Failed)
                        {
                            throw new InvalidOperationException($"Error preparing map with ID #{mapId}: {statusNote}");
                        }
                        return status == ProjectMapStatusCode.Prepared;
                    });
                }
            }
            return mapId;
        }

        private static long DefineClientContext(WebApiDataLayer dataLayer, IQueryExecutor executor, long projectId, Guid ownerContextId)
        {
            var selQuery = dataLayer.GetBuilder<IClientContext>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.OwnerContextId, ownerContextId)
                .Filter(c => c.PROJECT.Id, projectId);

            var contextId = executor.ExecuteAndFetch(selQuery, reader =>
            {
                if (reader.Count == 0 || !reader.Read())
                {
                    return (long)0;
                }

                var id = reader.GetValue(c => c.Id);
                Console.WriteLine($"Found the client context with ID #{id}");
                return id;
            });

            if (contextId == 0)
            {
                var insQuery = dataLayer.GetBuilder<IClientContext>()
                    .Create()
                    .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                    .SetValue(c => c.OwnerInstance, OwnerInstance)
                    .SetValue(c => c.OwnerContextId, ownerContextId)
                    .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Created)
                    .SetValue(c => c.StatusName, ClientContextStatusCode.Created.ToString())
                    .SetValue(c => c.StatusNote, "The new context was created")
                    .SetValue(c => c.PROJECT.Id, projectId)
                    .SetValue(c => c.TypeCode, (byte)ClientContextTypeCode.Client)
                    .SetValue(c => c.TypeName, ClientContextTypeCode.Client.ToString())
                    .SetValue(c => c.Name, "Test client context")
                    .SetValue(c => c.Note, "The client context to test")
                    ;
                var contextPk = executor.Execute<IClientContext_PK>(insQuery);
                contextId = contextPk.Id;

                CreateClientContextPropagationModels(dataLayer, executor, contextId);



                var stations = new long[sitesLatitudes.Length];
                for (var i = 0; i < stations.Length; i++)
                {
                    PointOnMap pointOnMapSt = new PointOnMap
                    {
                        Lat_dec = sitesLatitudes[i],
                        Lon_dec = sitesLongitudes[i],
                        Hei_m = sitesHeights[i]
                    };
                    PointOnMap pointOnMapFS = new PointOnMap
                    {
                        Lat_dec = pointsLatitudes[i],
                        Lon_dec = pointsLongitudes[i],
                        Hei_m = pointsHeights[i]
                    };

                    stations[i] = CreateClientContextStation(dataLayer, executor, contextId, pointOnMapSt);
                    // планируемые задачи для каждой станции

                    //CreatePlannedCalcTask(dataLayer, executor, contextId, stations[i], pointOnMapFS);
                }

                // меняем статус
                var updQuery = dataLayer.GetBuilder<IClientContext>()
                    .Update()
                    .SetValue(c => c.StatusCode, (byte)ClientContextStatusCode.Pending)
                    .SetValue(c => c.StatusName, "Pending")
                    .SetValue(c => c.StatusNote, "")
                    .Filter(c => c.Id, contextId);

                executor.Execute(updQuery);

                Console.WriteLine($"Expectations for calculating client context data with ID #{contextId}...");

                // ожидаем расчет контекста
                var cancel = false;
                while (!cancel)
                {
                    System.Threading.Thread.Sleep(20 * 1000);

                    var checkQuery = dataLayer.GetBuilder<IClientContext>()
                        .Read()
                        .Select(c => c.StatusCode)
                        .Select(c => c.StatusNote)
                        .Filter(c => c.Id, contextId);

                    cancel = executor.ExecuteAndFetch(checkQuery, reader =>
                    {
                        if (reader.Count == 0 || !reader.Read())
                        {
                            throw new InvalidOperationException($"A client context not found by ID #{contextId}");
                        }

                        var status = (ClientContextStatusCode)reader.GetValue(c => c.StatusCode);
                        var statusNote = reader.GetValue(c => c.StatusNote);

                        Console.WriteLine($"  {DateTime.Now.ToLongTimeString()} - checked the client context status with ID #{contextId}: {status}, '{statusNote}'");

                        if (status == ClientContextStatusCode.Failed)
                        {
                            throw new InvalidOperationException($"Error preparing client context with ID #{contextId}: {statusNote}");
                        }
                        return status == ClientContextStatusCode.Prepared;
                    });
                }
            }
            return contextId;
        }

        private static void CreateClientContextPropagationModels(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId)
        {
            var mainQuery = dataLayer.GetBuilder<IClientContextMainBlock>()
                .Apply()
                .Filter(c => c.ContextId, contextId)
                .CreateIfNotExists()
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)MainBlockModelTypeCode.ITU1546)
                    .SetValue(c => c.ModelTypeName, MainBlockModelTypeCode.ITU1546.ToString())
                .UpdateIfExists()
                    .SetValue(c => c.ModelTypeCode, (byte)MainBlockModelTypeCode.ITU1546)
                    .SetValue(c => c.ModelTypeName, MainBlockModelTypeCode.ITU1546.ToString())
                ;
            var count = executor.Execute(mainQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of MainBlock in the client context with ID #{contextId}");
            }

            var absorptionQuery = dataLayer.GetBuilder<IClientContextAbsorption>()
                .Apply()
                .Filter(c => c.ContextId, contextId)
                .SetValue(c => c.ContextId, contextId)

                .SetValue(c => c.ModelTypeCode, (byte)AbsorptionModelTypeCode.FlatAndLinear)
                .SetValue(c => c.ModelTypeName, AbsorptionModelTypeCode.FlatAndLinear.ToString())
                //.SetValue(c => c.ModelTypeCode, (byte) AbsorptionModelTypeCode.ITU2109_2)
                //.SetValue(c => c.ModelTypeName, AbsorptionModelTypeCode.ITU2109_2.ToString())
                .SetValue(c => c.Available, true)
                ;
            count = executor.Execute(absorptionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Absorption in the client context with ID #{contextId}");
            }

            // в случаи не включения блока в расчет запись создавать не обязательно
            // но если нужно зарезервировать ее, то следует создавать так как показано ниже
            var additionalQuery = dataLayer.GetBuilder<IClientContextAdditional>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)AdditionalModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, AdditionalModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(additionalQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Additional in the client context with ID #{contextId}");
            }

            var atmosphericQuery = dataLayer.GetBuilder<IClientContextAtmospheric>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)AtmosphericModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, AtmosphericModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(atmosphericQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Atmospheric in the client context with ID #{contextId}");
            }

            var clutterQuery = dataLayer.GetBuilder<IClientContextClutter>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)ClutterModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, ClutterModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(clutterQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Clutter in the client context with ID #{contextId}");
            }

            var diffractionQuery = dataLayer.GetBuilder<IClientContextDiffraction>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)DiffractionModelTypeCode.Deygout91)
                    .SetValue(c => c.ModelTypeName, DiffractionModelTypeCode.Deygout91.ToString())
                    .SetValue(c => c.Available, true)
                ;
            count = executor.Execute(diffractionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Diffraction in the client context with ID #{contextId}");
            }

            var ductingQuery = dataLayer.GetBuilder<IClientContextDucting>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(ductingQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Ducting in the client context with ID #{contextId}");
            }

            var reflectionQuery = dataLayer.GetBuilder<IClientContextReflection>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(reflectionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Reflection in the client context with ID #{contextId}");
            }

            var subPathDiffractionQuery = dataLayer.GetBuilder<IClientContextSubPathDiffraction>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)SubPathDiffractionModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, SubPathDiffractionModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(subPathDiffractionQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of SubPathDiffraction in the client context with ID #{contextId}");
            }

            var tropoQuery = dataLayer.GetBuilder<IClientContextTropo>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.ModelTypeCode, (byte)TropoModelTypeCode.Unknown)
                    .SetValue(c => c.ModelTypeName, TropoModelTypeCode.Unknown.ToString())
                    .SetValue(c => c.Available, false)
                ;
            count = executor.Execute(tropoQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of Tropo in the client context with ID #{contextId}");
            }

            var paramsQuery = dataLayer.GetBuilder<IClientContextGlobalParams>()
                    .Apply()
                    .Filter(c => c.ContextId, contextId)
                    .SetValue(c => c.ContextId, contextId)
                    .SetValue(c => c.Time_pc, 7)
                    .SetValue(c => c.Location_pc, 50)
                    .SetValue(c => c.EarthRadius_km, 8500)
                ;
            count = executor.Execute(paramsQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't apply record of GlobalParams in the client context with ID #{contextId}");
            }
        }

        private static long CreateClientContextStation(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, PointOnMap pointOnMap)
        {
            //var rand = new Random();
            var insQuery = dataLayer.GetBuilder<IContextStation>()
                .Create()
                .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                .SetValue(c => c.CONTEXT.Id, contextId)
                .SetValue(c => c.Name, "Test station")
                .SetValue(c => c.StateCode, (byte)StationStateCode.A)
                .SetValue(c => c.StateName, "A")
                .SetValue(c => c.CallSign, "CS:298392750")

                //.SetValue(c => c.SITE.Longitude_DEC, 24.03174 + 0.1 * (0.5 - rand.NextDouble()))
                //.SetValue(c => c.SITE.Latitude_DEC, 49.86417 + 0.1 * (0.5 - rand.NextDouble()))
                //.SetValue(c => c.SITE.Altitude_m, 30)
                .SetValue(c => c.SITE.Longitude_DEC, pointOnMap.Lon_dec)
                .SetValue(c => c.SITE.Latitude_DEC, pointOnMap.Lat_dec)
                .SetValue(c => c.SITE.Altitude_m, pointOnMap.Hei_m)

                .SetValue(c => c.ANTENNA.ItuPatternCode, (byte)ItuPattern.None)
                .SetValue(c => c.ANTENNA.ItuPatternName, "None")
                .SetValue(c => c.ANTENNA.XPD_dB, 0)
                .SetValue(c => c.ANTENNA.Gain_dB, 0)
                .SetValue(c => c.ANTENNA.Tilt_deg, 0)
                .SetValue(c => c.ANTENNA.Azimuth_deg, 0);

            var stationPk = executor.Execute<IContextStation_PK>(insQuery);
            var stationId = stationPk.Id;

            //var patternHAngle_deg = new double[12] { 0, 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330 };
            //var patternHLoss_dB = new float[12] { 0, 3, 10, 20, 25, 30, 25, 30, 25, 20, 10, 3 };
            //var patternVAngle_deg = new double[17] { -90, -60, -30, -15, -10, -5, -3, -1, 0, 1, 3, 5, 10, 15, 30, 60, 90 };
            //var patternVLoss_dB = new float[17] { 25, 20, 15, 10, 9, 7, 5, 2, 0, 2, 5, 7, 9, 10, 15, 20, 25 };

            var patternHAngle_deg = new double[1] { 0 };
            var patternHLoss_dB = new float[1] { 0 };
            var patternVAngle_deg = new double[1] { 0 };
            var patternVLoss_dB = new float[1] { 0 };


            // создаем запись о трансмитере
            var transQuery = dataLayer.GetBuilder<IContextStationTransmitter>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.PolarizationCode, (byte)PolarizationCode.H)
                .SetValue(c => c.PolarizationName, "H")
                .SetValue(c => c.Loss_dB, 0)
                .SetValue(c => c.Freq_MHz, 933.4)
                .SetValue(c => c.Freqs_MHz, new double[1] { 933.4 } )
                .SetValue(c => c.BW_kHz, 0)
                .SetValue(c => c.MaxPower_dBm, 54);
            executor.Execute(transQuery);

            //  создаем патерн атенты
            var paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "H")
                .SetValue(c => c.WavePlane, "H")

                .SetValue(c => c.Angle_deg, patternHAngle_deg)
                .SetValue(c => c.Loss_dB, patternHLoss_dB);
            executor.Execute(paternQuery);

            paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "H")
                .SetValue(c => c.WavePlane, "V")

                .SetValue(c => c.Angle_deg, patternHAngle_deg)
                .SetValue(c => c.Loss_dB, patternHLoss_dB);
            executor.Execute(paternQuery);

            paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "V")
                .SetValue(c => c.WavePlane, "H")

                .SetValue(c => c.Angle_deg, patternVAngle_deg)
                .SetValue(c => c.Loss_dB, patternVLoss_dB);
            executor.Execute(paternQuery);

            paternQuery = dataLayer.GetBuilder<IContextStationPattern>()
                .Create()
                .SetValue(c => c.StationId, stationId)
                .SetValue(c => c.AntennaPlane, "V")
                .SetValue(c => c.WavePlane, "V")

                .SetValue(c => c.Angle_deg, patternVAngle_deg)
                .SetValue(c => c.Loss_dB, patternVLoss_dB);
            executor.Execute(paternQuery);
            return stationId;
        }

        private static long CreateCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, long stationId, PointOnMap pointOnMap)
        {
            //var rand = new Random();
            var insQuery = dataLayer.GetBuilder<ICalcTask>()
                .Create()
                .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                .SetValue(c => c.CONTEXT.Id, contextId)
                .SetValue(c => c.MapName, MainProjectMap)
                .SetValue(c => c.OwnerInstance, OwnerInstance)
                .SetValue(c => c.OwnerTaskId, Guid.NewGuid())
                .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Created)
                .SetValue(c => c.StatusName, CalcTaskStatusCode.Created.ToString())
                .SetValue(c => c.StatusNote, "The task was created")
                .SetValue(c => c.TypeCode, (byte)CalcTaskType.PointFieldStrengthCalc)
                .SetValue(c => c.TypeName, CalcTaskType.PointFieldStrengthCalc.ToString())
                ;
            var taskPk = executor.Execute<ICalcTask_PK>(insQuery);

            var argsQuery = dataLayer.GetBuilder<IPointFieldStrengthArgs>()
                    .Create()
                    .SetValue(c => c.TaskId, taskPk.Id)
                    .SetValue(c => c.STATION.Id, stationId)
                    //.SetValue(c => c.PointAltitude_m, 10)
                    //.SetValue(c => c.PointLongitude_DEC, 24.03175 + 0.1 * (0.5 - rand.NextDouble()))
                    //.SetValue(c => c.PointLatitude_DEC, 49.86411 + 0.1 * (0.5 - rand.NextDouble()))
                    .SetValue(c => c.PointAltitude_m, pointOnMap.Hei_m)
                    .SetValue(c => c.PointLongitude_DEC, pointOnMap.Lon_dec)
                    .SetValue(c => c.PointLatitude_DEC, pointOnMap.Lat_dec)
                ;
            executor.Execute(argsQuery);

            Console.WriteLine($"The new Point Field Strength Calc Task was created with ID #{taskPk.Id} ");

            return taskPk.Id;
        }

        //private static long CreatePlannedCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId, long stationId, PointOnMap pointOnMap)
        //{
        //    //var rand = new Random();
        //    var insQuery = dataLayer.GetBuilder<IContextPlannedCalcTask>()
        //            .Create()
        //            .SetValue(c => c.CONTEXT.Id, contextId)
        //            .SetValue(c => c.MapName, MainProjectMap)
        //            .SetValue(c => c.TypeCode, (byte)CalcTaskType.PointFieldStrengthCalc)
        //            .SetValue(c => c.TypeName, CalcTaskType.PointFieldStrengthCalc.ToString())
        //            .SetValue(c => c.StartNumber, (int)stationId)
        //        ;
        //    var plannedTaskPk = executor.Execute<IContextPlannedCalcTask_PK>(insQuery);

        //    var defaultArgsQuery = dataLayer.GetBuilder<IPointFieldStrengthArgsDefault>()
        //            .Create()
        //            .SetValue(c => c.TaskId, plannedTaskPk.Id)
        //            .SetValue(c => c.STATION.Id, stationId)
        //            //.SetValue(c => c.PointAltitude_m, 10)
        //            //.SetValue(c => c.PointLongitude_DEC, 24.03175 + 0.1 * (0.5 - rand.NextDouble()))
        //            //.SetValue(c => c.PointLatitude_DEC, 49.86411 + 0.1 * (0.5 - rand.NextDouble()))
        //            .SetValue(c => c.PointAltitude_m, pointOnMap.Hei_m)
        //            .SetValue(c => c.PointLongitude_DEC, pointOnMap.Lon_dec)
        //            .SetValue(c => c.PointLatitude_DEC, pointOnMap.Lat_dec)
        //        ;
        //    executor.Execute(defaultArgsQuery);

        //    Console.WriteLine($"The Point Field Strength Planned Calc Task was created with ID #{plannedTaskPk.Id} ");

        //    return plannedTaskPk.Id;
        //}

        private static long[] GetClientContextStations(WebApiDataLayer dataLayer, IQueryExecutor executor, long contextId)
        {
            var readQuery = dataLayer.GetBuilder<IContextStation>()
                .Read()
                .Select(c => c.Id)
                .Filter(c => c.CONTEXT.Id, contextId);

            var result = executor.ExecuteAndRead(readQuery, r => r.GetValue(c => c.Id));
            return result;
        }

        private static void MakeCalcTaskAvailable(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId, long contextId)
        {
            var updQuery = dataLayer.GetBuilder<ICalcTask>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)CalcTaskStatusCode.Available)
                .SetValue(c => c.StatusName, CalcTaskStatusCode.Available.ToString())
                .SetValue(c => c.StatusNote, "The task was was made available")
                // необязательно, но можем усиливать фильтрацию текущим контекстом
                .Filter(c => c.OwnerInstance, OwnerInstance)
                .Filter(c => c.CONTEXT.Id, contextId)
                // фильтр по ключу
                .Filter(c => c.Id, taskId);

            var count = executor.Execute(updQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't make the task with ID #{taskId} Available");
            }
            Console.WriteLine($"The task with ID #{taskId} was made available");
        }

        private static long CreateCalcTaskResult(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId)
        {
            var insQuery = dataLayer.GetBuilder<ICalcResult>()
                .Create()
                .SetValue(c => c.CreatedDate, DateTimeOffset.Now)
                .SetValue(c => c.TASK.Id, taskId)
                .SetValue(c => c.CallerInstance, OwnerInstance)
                .SetValue(c => c.CallerResultId, Guid.NewGuid())
                .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Created)
                .SetValue(c => c.StatusName, CalcResultStatusCode.Created.ToString())
                .SetValue(c => c.StatusNote, "The result was created by the client")
                ;
            var resultPk = executor.Execute<ICalcResult_PK>(insQuery);

            Console.WriteLine($"The Calc Result was created with ID #{resultPk.Id} ");

            return resultPk.Id;
        }

        private static void RunCalcTask(WebApiDataLayer dataLayer, IQueryExecutor executor, long taskId, long resultId)
        {
            var updQuery = dataLayer.GetBuilder<ICalcResult>()
                .Update()
                .SetValue(c => c.StatusCode, (byte)CalcResultStatusCode.Pending)
                .SetValue(c => c.StatusName, CalcResultStatusCode.Pending.ToString())
                .SetValue(c => c.StatusNote, "The result was ran by the client")
                // необязательно, но можем усиливать фильтрацию текущим контекстом
                .Filter(c => c.CallerInstance, OwnerInstance)
                .Filter(c => c.TASK.Id, taskId)
                // фильтр по ключу
                .Filter(c => c.Id, resultId);

            var count = executor.Execute(updQuery);
            if (count == 0)
            {
                throw new InvalidOperationException($"Can't run the task with ID #{taskId} (result ID #{resultId})");
            }
            Console.WriteLine($"The task with ID #{taskId} was run for the result with ID #{resultId}");
        }

        private static IPointFieldStrengthResult WaitForCalcResult(WebApiDataLayer dataLayer, IQueryExecutor executor, long calcTaskId, long calcResultId)
        {
            var cancel = false;
            var resultObject = dataLayer.ProxyInstanceFactory.Create<IPointFieldStrengthResult>();
            resultObject.RESULT = dataLayer.ProxyInstanceFactory.Create<ICalcResult>();

            while (!cancel)
            {
                System.Threading.Thread.Sleep(5 * 1000);

                var checkQuery = dataLayer.GetBuilder<ICalcResult>()
                    .Read()
                    .Select(c => c.Id)
                    .Select(c => c.CreatedDate)
                    .Select(c => c.StatusCode)
                    .Select(c => c.StatusName)
                    .Select(c => c.StatusNote)
                    .Select(c => c.StartTime)
                    .Select(c => c.FinishTime)
                    .Filter(c => c.Id, calcResultId);

                cancel = executor.ExecuteAndFetch(checkQuery, reader =>
                {
                    if (reader.Count == 0 || !reader.Read())
                    {
                        throw new InvalidOperationException($"A calc result not found by ID #{calcResultId}");
                    }

                    var status = (CalcResultStatusCode)reader.GetValue(c => c.StatusCode);
                    var statusNote = reader.GetValue(c => c.StatusNote);

                    Console.WriteLine($"  {DateTime.Now.ToLongTimeString()} - checked the calc result status with ID #{calcResultId} and task ID #{calcTaskId}: {status}, '{statusNote}'");

                    if (status != CalcResultStatusCode.Pending
                    && status != CalcResultStatusCode.Processing)
                    {
                        resultObject.RESULT.Id = reader.GetValue(c => c.Id);
                        resultObject.ResultId = reader.GetValue(c => c.Id);
                        resultObject.RESULT.StatusCode = reader.GetValue(c => c.StatusCode);
                        resultObject.RESULT.StatusName = reader.GetValue(c => c.StatusName);
                        resultObject.RESULT.StatusNote = reader.GetValue(c => c.StatusNote);
                        resultObject.RESULT.CreatedDate = reader.GetValue(c => c.CreatedDate);
                        resultObject.RESULT.StartTime = reader.GetValue(c => c.StartTime);
                        resultObject.RESULT.FinishTime = reader.GetValue(c => c.FinishTime);

                        var resultQuery = dataLayer.GetBuilder<IPointFieldStrengthResult>()
                            .Read()
                            .Select(c => c.FS_dBuVm)
                            .Select(c => c.Level_dBm)
                            .Filter(c => c.ResultId, calcResultId);

                        var rs = executor.ExecuteAndRead(resultQuery, r =>
                        {
                            resultObject.FS_dBuVm = r.GetValue(c => c.FS_dBuVm);
                            resultObject.Level_dBm = r.GetValue(c => c.Level_dBm);
                            return resultObject;
                        });

                    }

                    if (status == CalcResultStatusCode.Failed)
                    {
                        Console.WriteLine($"The calc task ID #{calcTaskId}(result ID #{calcResultId}) is Failed: {statusNote}");
                        return true;
                        //throw new InvalidOperationException($"Error calculation task with ID #{calcTaskId}(result ID #{calcResultId}): {statusNote}");
                    }

                    if (status == CalcResultStatusCode.Completed)
                    {


                        Console.WriteLine($"The calc task ID #{calcTaskId}(result ID #{calcResultId}) is Completed: {statusNote}");
                        return true;
                    }

                    if (status == CalcResultStatusCode.Aborted)
                    {
                        Console.WriteLine($"The calc task ID #{calcTaskId}(result ID #{calcResultId}) is Aborted: {statusNote}");
                        return true;
                    }

                    if (status == CalcResultStatusCode.Canceled)
                    {
                        Console.WriteLine($"The calc task ID #{calcTaskId}(result ID #{calcResultId}) is Canceled: {statusNote}");
                        return true;
                    }

                    return false;
                });
            }

            return resultObject;
        }
    }
}
