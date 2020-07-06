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
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using IdwmDataModel = Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Idwm = Atdi.Contracts.Sdrn.DeepServices.IDWM;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// 
    /// </summary>
    public class Ge06CalcIteration : IIterationHandler<Ge06CalcData, Ge06CalcResult>
    {
        private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
        private readonly IObjectPool<PointEarthGeometric[]> _pointEarthGeometricPool;
        private readonly IObjectPoolSite _poolSite;
        private readonly ITransformation _transformation;
        private readonly Idwm.IIdwmService _idwmService;
        private readonly IEarthGeometricService _earthGeometricService;
        private readonly IGn06Service  _gn06Service;
        private readonly AppServerComponentConfig _appServerComponentConfig;
        private ITaskContext _taskContext;
        private Ge06CalcData _ge06CalcData;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public Ge06CalcIteration(
            IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> calcServerDataLayer,
            IEarthGeometricService earthGeometricService,
            IIterationsPool iterationsPool,
            IObjectPoolSite poolSite,
            AppServerComponentConfig appServerComponentConfig,
            ITransformation transformation,
            IGn06Service gn06Service,
            Idwm.IIdwmService idwmService,
            ILogger logger)
        {
            _calcServerDataLayer = calcServerDataLayer;
            _iterationsPool = iterationsPool;
            _earthGeometricService = earthGeometricService;
            _poolSite = poolSite;
            _appServerComponentConfig = appServerComponentConfig;
            _transformation = transformation;
            _gn06Service = gn06Service;
            _idwmService = idwmService;
            _logger = logger;
            _pointEarthGeometricPool = _poolSite.GetPool<PointEarthGeometric[]>(ObjectPools.GE06PointEarthGeometricObjectPool);
        }



        public Ge06CalcResult Run(ITaskContext taskContext, Ge06CalcData data)
        {
            LoadDataBrific.SetBRIFICDirectory(this._appServerComponentConfig.BrificDBSource);

            this._taskContext = taskContext;

            //здесь вызов функции "переопределения" модели распространения в зависимости от входных параметров
            //data.PropagationModel = GetPropagationModel(this._ge06CalcData.Ge06TaskParameters, this._ge06CalcData.PropagationModel, (CalculationType)this._ge06CalcData.Ge06TaskParameters.CalculationTypeCode);

            this._ge06CalcData = data;

            var ge06CalcResults = new Ge06CalcResult();
            var ge06CalcResultsForICSM = new Ge06CalcResult();
            var ge06CalcResultsForBRIFIC = new Ge06CalcResult();

            var affectedADMResult = new List<AffectedADMResult>();
            var contoursResult = new List<ContoursResult>();
            var allotmentOrAssignmentResult = new List<AllotmentOrAssignmentResult>();


            //временная проверка
            if (data.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM != null)
            {
                if ((data.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments != null))
                {
                    if ((data.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments.AdminData == null))
                    {
                        data.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments = null;
                    }
                }
            }
            if (data.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC != null)
            {
                if ((data.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Allotments != null))
                {
                    if ((data.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Allotments.AdminData == null))
                    {
                        data.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Allotments = null;
                    }
                }
            }


            if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.CreateContoursByDistance)
            {
                CalculationForCreateContoursByDistance(in data, BroadcastingTypeContext.Icsm, ref ge06CalcResultsForICSM);
                if (ge06CalcResultsForICSM.AffectedADMResult != null)
                {
                    affectedADMResult.AddRange(ge06CalcResultsForICSM.AffectedADMResult);
                }
                if (ge06CalcResultsForICSM.ContoursResult != null)
                {
                    contoursResult.AddRange(ge06CalcResultsForICSM.ContoursResult);
                }
                if (ge06CalcResultsForICSM.AllotmentOrAssignmentResult != null)
                {
                    allotmentOrAssignmentResult.AddRange(ge06CalcResultsForICSM.AllotmentOrAssignmentResult);
                }


                CalculationForCreateContoursByDistance(in data, BroadcastingTypeContext.Brific, ref ge06CalcResultsForBRIFIC);
                if (ge06CalcResultsForBRIFIC.AffectedADMResult != null)
                {
                    affectedADMResult.AddRange(ge06CalcResultsForBRIFIC.AffectedADMResult);
                }
                if (ge06CalcResultsForBRIFIC.ContoursResult != null)
                {
                    contoursResult.AddRange(ge06CalcResultsForBRIFIC.ContoursResult);
                }
                if (ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult != null)
                {
                    allotmentOrAssignmentResult.AddRange(ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult);
                }
            }
            else if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.CreateContoursByFS)
            {
                CalculationForCreateContoursByFS(in data, BroadcastingTypeContext.Icsm, ref ge06CalcResultsForICSM);
                if (ge06CalcResultsForICSM.AffectedADMResult != null)
                {
                    affectedADMResult.AddRange(ge06CalcResultsForICSM.AffectedADMResult);
                }
                if (ge06CalcResultsForICSM.ContoursResult != null)
                {
                    contoursResult.AddRange(ge06CalcResultsForICSM.ContoursResult);
                }
                if (ge06CalcResultsForICSM.AllotmentOrAssignmentResult != null)
                {
                    allotmentOrAssignmentResult.AddRange(ge06CalcResultsForICSM.AllotmentOrAssignmentResult);
                }

                CalculationForCreateContoursByFS(in data, BroadcastingTypeContext.Brific, ref ge06CalcResultsForBRIFIC);
                if (ge06CalcResultsForBRIFIC.AffectedADMResult != null)
                {
                    affectedADMResult.AddRange(ge06CalcResultsForBRIFIC.AffectedADMResult);
                }
                if (ge06CalcResultsForBRIFIC.ContoursResult != null)
                {
                    contoursResult.AddRange(ge06CalcResultsForBRIFIC.ContoursResult);
                }
                if (ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult != null)
                {
                    allotmentOrAssignmentResult.AddRange(ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult);
                }

            }
            else if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.ConformityCheck)
            {
                CalculationForConformityCheck(data, ref ge06CalcResults);
                if (ge06CalcResultsForICSM.AffectedADMResult != null)
                {
                    affectedADMResult.AddRange(ge06CalcResultsForICSM.AffectedADMResult);
                }
                if (ge06CalcResultsForICSM.ContoursResult != null)
                {
                    contoursResult.AddRange(ge06CalcResultsForICSM.ContoursResult);
                }
                if (ge06CalcResultsForICSM.AllotmentOrAssignmentResult != null)
                {
                    allotmentOrAssignmentResult.AddRange(ge06CalcResultsForICSM.AllotmentOrAssignmentResult);
                }
            }
            else if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.FindAffectedADM)
            {
                CalculationForFindAffectedADM(data, ref ge06CalcResultsForICSM);
                if (ge06CalcResultsForBRIFIC.AffectedADMResult != null)
                {
                    affectedADMResult.AddRange(ge06CalcResultsForBRIFIC.AffectedADMResult);
                }
                if (ge06CalcResultsForBRIFIC.ContoursResult != null)
                {
                    contoursResult.AddRange(ge06CalcResultsForBRIFIC.ContoursResult);
                }
                if (ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult != null)
                {
                    allotmentOrAssignmentResult.AddRange(ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult);
                }
            }

            ge06CalcResults.AffectedADMResult = affectedADMResult.ToArray();
            ge06CalcResults.AllotmentOrAssignmentResult = allotmentOrAssignmentResult.ToArray();
            ge06CalcResults.ContoursResult = contoursResult.ToArray();

            return ge06CalcResults;
        }


       

        /// <summary>
        /// Расчет  для CalculationType == CreateContoursByDistance
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        private void CalculationForCreateContoursByDistance(
                                            in Ge06CalcData ge06CalcData,
                                            BroadcastingTypeContext broadcastingTypeContext,
                                            ref Ge06CalcResult ge06CalcResult
                                            )
        {
            
            var pointEarthGeometricsResult = default(PointEarthGeometric[]);

            BroadcastingContextBase broadcastingContextBase = null;
            if (broadcastingTypeContext== BroadcastingTypeContext.Brific)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            }
            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;
            }


            var affectedServices = new List<string>();

            if (((GE06Validation.ValidationAssignment(broadcastingContextBase.Assignments)) && (GE06Validation.ValidationAllotment(broadcastingContextBase.Allotments))) == false)
            {
                throw new Exception("Input parameters failed validation");
            }


            if (broadcastingContextBase.Allotments != null)
            {
                affectedServices.Add(broadcastingContextBase.Allotments.AdminData.StnClass);
            }
            if (broadcastingContextBase.Assignments != null)
            {
                for (int i = 0; i < broadcastingContextBase.Assignments.Length; i++)
                {
                    if (!affectedServices.Contains(broadcastingContextBase.Assignments[i].AdmData.StnClass))
                    {
                        affectedServices.Add(broadcastingContextBase.Assignments[i].AdmData.StnClass);
                    }
                }
            }



            var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
            {
                BroadcastingAllotment = broadcastingContextBase.Allotments,
                BroadcastingAssignments = broadcastingContextBase.Assignments
            };
            //1.Определение центра гравитации(2.1)
            var pointEarthGeometricBarycenter = new PointEarthGeometric();
            this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);

            var dicCountoursPoints = new Dictionary<CountoursPoint, string>();

            for (int i = 0; i < ge06CalcData.Ge06TaskParameters.Distances.Length; i++)
            {
                // 2. Построение контуров фиксированной дистанции относительно центра гравитации.
                // Базируемся на функции CreateContourFromPointByDistance если у нас только BroadcastingAssignment []

                if ((broadcastingContextBase.Allotments == null) && ((broadcastingContextBase.Assignments!=null) && (broadcastingContextBase.Assignments.Length>0)))
                {
                    var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                    {
                        Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                        PointEarthGeometricCalc = pointEarthGeometricBarycenter
                    };


                    try
                    {
                        pointEarthGeometricsResult = _pointEarthGeometricPool.Take();
                        this._earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);
                        var countoursPoints =  new CountoursPoint[sizeResultBuffer];
                        for (int k = 0; k < sizeResultBuffer; k++)
                        {
                            var point = new Point()
                            {
                                Longitude = pointEarthGeometricsResult[k].Longitude,
                                Latitude = pointEarthGeometricsResult[k].Latitude
                            };

                            countoursPoints[k] = new CountoursPoint();
                            countoursPoints[k].Lon_DEC = pointEarthGeometricsResult[k].Longitude;
                            countoursPoints[k].Lat_DEC = pointEarthGeometricsResult[k].Latitude;
                            if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                            {
                                countoursPoints[k].PointType = PointType.Etalon; 
                            }
                            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                            {
                                countoursPoints[k].PointType = PointType.Unknown; 
                            }
                            countoursPoints[k].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                            countoursPoints[k].FS = (int)CalcFieldStrengthInPointGE06(ge06CalcData, in point, broadcastingTypeContext);
                            if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                            {
                                countoursPoints[k].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                            }
                           
                            var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                            {
                                Longitude_dec = point.Longitude,
                                Latitude_dec = point.Latitude
                            });

                            dicCountoursPoints.Add(countoursPoints[k], adm);
                        }
                    }
                    finally
                    {
                        if (pointEarthGeometricsResult != null)
                        {
                            _pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                        }
                    }

                }
                //или на функции CreateContourFromContureByDistance если у нас есть BroadcastingAllotment 
                else 
                {
                    if (broadcastingContextBase.Allotments != null)
                    {
                        if (broadcastingContextBase.Allotments.AllotmentParameters != null)
                        {
                            var areaPoints = broadcastingContextBase.Allotments.AllotmentParameters.Contur;
                            if ((areaPoints != null) && (areaPoints.Length > 0))
                            {
                                var pointEarthGeometrics = new PointEarthGeometric[areaPoints.Length];
                                for (int h = 0; h < areaPoints.Length; h++)
                                {
                                    pointEarthGeometrics[h] = new PointEarthGeometric(areaPoints[h].Lon_DEC, areaPoints[h].Lat_DEC, CoordinateUnits.deg);
                                }

                                var contourFromContureByDistanceArgs = new ContourFromContureByDistanceArgs()
                                {
                                    Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                    Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                                    PointBaryCenter = pointEarthGeometricBarycenter,
                                    ContourPoints = pointEarthGeometrics
                                };

                                try
                                {
                                    pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                                    this._earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                                    var countoursPoints = new CountoursPoint[sizeResultBuffer];
                                    for (int k = 0; k < sizeResultBuffer; k++)
                                    {
                                        var point = new Point()
                                        {
                                            Longitude = pointEarthGeometricsResult[k].Longitude,
                                            Latitude = pointEarthGeometricsResult[k].Latitude
                                        };

                                        countoursPoints[k] = new CountoursPoint();
                                        countoursPoints[k].Lon_DEC = pointEarthGeometricsResult[k].Longitude;
                                        countoursPoints[k].Lat_DEC = pointEarthGeometricsResult[k].Latitude;
                                        if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                        {
                                            countoursPoints[k].PointType = PointType.Etalon;
                                        }
                                        if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                        {
                                            countoursPoints[k].PointType = PointType.Unknown;
                                        }
                                        countoursPoints[k].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                                        countoursPoints[k].FS = (int)CalcFieldStrengthInPointGE06(ge06CalcData, in point, broadcastingTypeContext);
                                        if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                                        {
                                            countoursPoints[k].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                                        }

                                        var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                        {
                                            Longitude_dec = point.Longitude,
                                            Latitude_dec = point.Latitude
                                        });

                                        dicCountoursPoints.Add(countoursPoints[k], adm);
                                    }

                                }
                                finally
                                {
                                    if (pointEarthGeometricsResult != null)
                                    {
                                        _pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var lstContoursResults = new List<ContoursResult>();
            var distances = ge06CalcData.Ge06TaskParameters.Distances;
            if ((dicCountoursPoints != null) && (dicCountoursPoints.Count > 0))
            {
                var lstCountoursPoints = dicCountoursPoints.ToList();
                var arrPointType = new PointType[4] { PointType.Etalon, PointType.Unknown, PointType.Affected, PointType.Correct };
                if (lstCountoursPoints != null)
                {
                    for (int n = 0; n < arrPointType.Length; n++)
                    {
                        var distinctByPointType = lstCountoursPoints.FindAll(c => c.Key.PointType == arrPointType[n]);
                        if (distinctByPointType != null)
                        {
                            for (int i = 0; i < distances.Length; i++)
                            {
                                var distinctByDistance = distinctByPointType.FindAll(c => c.Key.Distance == distances[i]);
                                if (distinctByDistance != null)
                                {
                                    var distinctAdmByAdm = distinctByDistance.Select(c => c.Value).Distinct();
                                    if (distinctAdmByAdm != null)
                                    {
                                        var arrDistinctAdmByDistance = distinctAdmByAdm.ToArray();
                                        for (int k = 0; k < arrDistinctAdmByDistance.Length; k++)
                                        {
                                            var listContourPoints = lstCountoursPoints.FindAll(c => c.Key.Distance == distances[i] && c.Key.PointType == arrPointType[n] && c.Value == arrDistinctAdmByDistance[k]);
                                            if (listContourPoints != null)
                                            {

                                                var contourType = ContourType.Unknown;
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                                {
                                                    contourType = ContourType.Etalon;
                                                }
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                                {
                                                    contourType = ContourType.New;
                                                }

                                                var allPoints = listContourPoints.Select(c => c.Key).ToArray();
                                                lstContoursResults.Add(new ContoursResult()
                                                {
                                                    AffectedADM = arrDistinctAdmByDistance[k],
                                                    ContourType = contourType,
                                                    CountoursPoints = allPoints,
                                                    Distance = distances[i],
                                                    PointsCount = allPoints.Length
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            var distinctAdm = lstContoursResults.Select(c => c.AffectedADM).Distinct();
            if ((distinctAdm != null) && (distinctAdm.Count()>0))
            {
                var arrDistinctAdmByAdm = distinctAdm.ToArray();
                if (arrDistinctAdmByAdm.Length > 0)
                {
                    var affectedADMRes = new AffectedADMResult[arrDistinctAdmByAdm.Length];
                    for (int k = 0; k < arrDistinctAdmByAdm.Length; k++)
                    {
                        affectedADMRes[k] = new AffectedADMResult();
                        affectedADMRes[k].ADM = arrDistinctAdmByAdm[k];
                        affectedADMRes[k].AffectedServices =  string.Join(",", affectedServices);
                    }
                    ge06CalcResult.AffectedADMResult = affectedADMRes;
                }
            }

            GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBase, ref ge06CalcResult);
        }

        /// <summary>
        /// Расчет  для  CalculationType == CreateContoursByFS
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        private void CalculationForCreateContoursByFS(
                                           in Ge06CalcData ge06CalcData,
                                           BroadcastingTypeContext broadcastingTypeContext,
                                           ref Ge06CalcResult ge06CalcResult
                                           )
        {
            var pointEarthGeometricsResult = default(PointEarthGeometric[]);

            var affectedServices = new List<string>();

            BroadcastingContextBase broadcastingContextBase = null;
            if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            }
            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;
            }

            if (((GE06Validation.ValidationAssignment(broadcastingContextBase.Assignments)) && (GE06Validation.ValidationAllotment(broadcastingContextBase.Allotments))) == false)
            {
                throw new Exception("Input parameters failed validation");
            }


            if (broadcastingContextBase.Allotments != null)
            {
                affectedServices.Add(broadcastingContextBase.Allotments.AdminData.StnClass);
            }
            if (broadcastingContextBase.Assignments != null)
            {
                for (int i = 0; i < broadcastingContextBase.Assignments.Length; i++)
                {
                    if (!affectedServices.Contains(broadcastingContextBase.Assignments[i].AdmData.StnClass))
                    {
                        affectedServices.Add(broadcastingContextBase.Assignments[i].AdmData.StnClass);
                    }
                }
            }


            var lstContoursResults = new List<ContoursResult>();
            var dicCountoursPoints = new Dictionary<CountoursPoint, string>();


            if ((broadcastingContextBase.Allotments != null) || ((broadcastingContextBase.Assignments != null) && (broadcastingContextBase.Assignments.Length > 0)))
            {

                var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
                {
                    BroadcastingAllotment = broadcastingContextBase.Allotments,
                    BroadcastingAssignments = broadcastingContextBase.Assignments
                };

                //1.Определение центра гравитации(2.1)
                var pointEarthGeometricBarycenter = new PointEarthGeometric();
                this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);

                for (int i = 0; i < ge06CalcData.Ge06TaskParameters.FieldStrength.Length; i++)
                {

                    // 2. Построение контуров фиксированной дистанции относительно центра гравитации.
                    // Базируемся на функции CreateContourFromPointByDistance если у нас только BroadcastingAssignment []



                    var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                    {
                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                        TriggerFieldStrength = ge06CalcData.Ge06TaskParameters.FieldStrength[i],
                        BaryCenter = pointEarthGeometricBarycenter
                    };


                    try
                    {
                        pointEarthGeometricsResult = _pointEarthGeometricPool.Take();


                        int sizeResultBuffer = 0;

                        // модель из контекста, высота абонента из формы, процент времени из формы
                        var propModel = this._ge06CalcData.PropagationModel;
                        GE06PropagationModel.GetPropagationModelForContoursByFS(ref propModel, 50, (float)ge06CalcData.Ge06TaskParameters.PercentageTime.Value, ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value);
                        this._ge06CalcData.PropagationModel = propModel;


                        if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                        {
                            this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthBRIFIC(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                        }
                        if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                        {
                            this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthICSM(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                        }
                        if (sizeResultBuffer > 0)
                        {
                            var countoursPoints = new CountoursPoint[sizeResultBuffer];
                            for (int k = 0; k < sizeResultBuffer; k++)
                            {
                                var pointFS = new Point()
                                {
                                    Longitude = pointEarthGeometricsResult[k].Longitude,
                                    Latitude = pointEarthGeometricsResult[k].Latitude
                                };

                                countoursPoints[k] = new CountoursPoint();
                                countoursPoints[k].Lon_DEC = pointEarthGeometricsResult[k].Longitude;
                                countoursPoints[k].Lat_DEC = pointEarthGeometricsResult[k].Latitude;
                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                {
                                    countoursPoints[k].PointType = PointType.Etalon;
                                }
                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                {
                                    countoursPoints[k].PointType = PointType.Unknown;
                                }
                                countoursPoints[k].FS = ge06CalcData.Ge06TaskParameters.FieldStrength[i]; //(int)CalcFieldStrengthInPointGE06(ge06CalcData, in pointFS, broadcastingTypeContext);
                                if (ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue)
                                {
                                    countoursPoints[k].Height = ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value;
                                }

                                var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                {
                                    Longitude_dec = pointFS.Longitude,
                                    Latitude_dec = pointFS.Latitude
                                });

                                dicCountoursPoints.Add(countoursPoints[k], adm);
                            }
                        }
                    }

                    finally
                    {
                        if (pointEarthGeometricsResult != null)
                        {
                            _pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                        }
                    }
                }
            }
            if ((dicCountoursPoints != null) && (dicCountoursPoints.Count > 0))
            {

                var fieldStrength = ge06CalcData.Ge06TaskParameters.FieldStrength;
                var lstCountoursPoints = dicCountoursPoints.ToList();
                var arrPointType = new PointType[2] { PointType.Etalon, PointType.Unknown };
                if (lstCountoursPoints != null)
                {
                    for (int n = 0; n < arrPointType.Length; n++)
                    {
                        var distinctByPointType = lstCountoursPoints.FindAll(c => c.Key.PointType == arrPointType[n]);
                        if (distinctByPointType != null)
                        {
                            for (int i = 0; i < fieldStrength.Length; i++)
                            {
                                var distinctByFieldStrength = distinctByPointType.FindAll(c => c.Key.FS == fieldStrength[i]);
                                if (distinctByFieldStrength != null)
                                {
                                    var distinctAdmByAdm = distinctByFieldStrength.Select(c => c.Value).Distinct();
                                    if (distinctAdmByAdm != null)
                                    {
                                        var arrDistinctAdmByFieldStrength = distinctAdmByAdm.ToArray();
                                        for (int k = 0; k < arrDistinctAdmByFieldStrength.Length; k++)
                                        {
                                            var listContourPoints = lstCountoursPoints.FindAll(c => c.Key.FS == fieldStrength[i] && c.Key.PointType == arrPointType[n] && c.Value == arrDistinctAdmByFieldStrength[k]);
                                            if (listContourPoints != null)
                                            {

                                                var contourType = ContourType.Unknown;
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                                {
                                                    contourType = ContourType.Etalon;
                                                }
                                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                                {
                                                    contourType = ContourType.New;
                                                }

                                                var allPoints = listContourPoints.Select(c => c.Key).ToArray();
                                                lstContoursResults.Add(new ContoursResult()
                                                {
                                                    AffectedADM = arrDistinctAdmByFieldStrength[k],
                                                    ContourType = contourType,
                                                    CountoursPoints = allPoints,
                                                    FS = fieldStrength[i],
                                                    PointsCount = allPoints.Length
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ge06CalcResult.ContoursResult = lstContoursResults.ToArray();


                var distinctAdm = lstContoursResults.Select(c => c.AffectedADM).Distinct();
                if (distinctAdm != null)
                {
                    var arrDistinctAdmByAdm = distinctAdm.ToArray();
                    if (arrDistinctAdmByAdm.Length > 0)
                    {
                        var affectedADMRes = new AffectedADMResult[arrDistinctAdmByAdm.Length];
                        for (int k = 0; k < arrDistinctAdmByAdm.Length; k++)
                        {
                            affectedADMRes[k] = new AffectedADMResult();
                            affectedADMRes[k].ADM = arrDistinctAdmByAdm[k];
                            affectedADMRes[k].AffectedServices = string.Join(",", affectedServices);
                        }
                        ge06CalcResult.AffectedADMResult = affectedADMRes;
                    }
                }
            }

            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBase, ref ge06CalcResult);
        }

        /// <summary>
        /// Расчет  для  CalculationType == ConformityCheck
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        private void CalculationForConformityCheck(
                                          Ge06CalcData ge06CalcData,
                                          ref Ge06CalcResult ge06CalcResult
                                          )
        {
            var affectedServices = new List<string>();

            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultBRIFIC = default(PointEarthGeometric[]);

            var dicCountoursPointsByBRIFIC = new Dictionary<CountoursPoint, string>();
            var dicCountoursPointsByICSM = new Dictionary<CountoursPoint, string>();

            var lstContoursResultsByICSM = new List<ContoursResult>();
            var lstContoursResultsByBRIFIC = new List<ContoursResult>();
            var lstContoursResults = new List<ContoursResult>();

            if (((ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM!=null) && (ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC!= null))==false)
            {
                throw new Exception("Incomplete ICSM data or BRIFIC");
            }

            //0. Валидация входных данных. аналогично п.0 4.1. + обязательные наличие хотя по одному объекту для ICSM и BRIFIC
            if (((GE06Validation.ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments)) && (GE06Validation.ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments))) == false)
            {
                throw new Exception("Input parameters for ICSM failed validation");
            }

            if (((GE06Validation.ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Assignments)) && (GE06Validation.ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Allotments))) == false)
            {
                throw new Exception("Input parameters for BRIFIC failed validation");
            }


            var broadcastingContextBRIFIC = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;

         

            // поиск сведений о попроговых значениях напряженности поля 
            var thresholdFieldStrengths = new List<ThresholdFieldStrength>();

            var thresholdFieldStrengthsBRIFICAllotments =  ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextBRIFIC.Allotments, TypeThresholdFS.OnlyBroadcastingService);
            var thresholdFieldStrengthsBRIFICAssignments = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextBRIFIC.Assignments, TypeThresholdFS.OnlyBroadcastingService);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsBRIFICAllotments);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsBRIFICAssignments);
            




            var thresholdFieldStrengthsICSMAllotments =  ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextICSM.Allotments, TypeThresholdFS.OnlyBroadcastingService);
            var thresholdFieldStrengthsICSMAssignments =  ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextICSM.Assignments, TypeThresholdFS.OnlyBroadcastingService);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsICSMAllotments);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsICSMAssignments);




            //1.Определение центра гравитации(2.1)
            var pointEarthGeometricBarycenter = new PointEarthGeometric();

            if ((broadcastingContextBRIFIC != null) && (broadcastingContextICSM != null))
            {

                ///список затронутых служб для брифика
                if (broadcastingContextBRIFIC.Allotments != null)
                {
                    if (!affectedServices.Contains(broadcastingContextBRIFIC.Allotments.AdminData.StnClass))
                    {
                        affectedServices.Add(broadcastingContextBRIFIC.Allotments.AdminData.StnClass);
                    }

                }
                if (broadcastingContextBRIFIC.Assignments != null)
                {
                    for (int i = 0; i < broadcastingContextBRIFIC.Assignments.Length; i++)
                    {
                        if (!affectedServices.Contains(broadcastingContextBRIFIC.Assignments[i].AdmData.StnClass))
                        {
                            affectedServices.Add(broadcastingContextBRIFIC.Assignments[i].AdmData.StnClass);
                        }
                    }
                }

                ///список затронутых служб для ICSM
                if (broadcastingContextICSM.Allotments != null)
                {
                    if (!affectedServices.Contains(broadcastingContextICSM.Allotments.AdminData.StnClass))
                    {
                        affectedServices.Add(broadcastingContextICSM.Allotments.AdminData.StnClass);
                    }

                }
                if (broadcastingContextICSM.Assignments != null)
                {
                    for (int i = 0; i < broadcastingContextICSM.Assignments.Length; i++)
                    {
                        if (!affectedServices.Contains(broadcastingContextICSM.Assignments[i].AdmData.StnClass))
                        {
                            affectedServices.Add(broadcastingContextICSM.Assignments[i].AdmData.StnClass);
                        }
                    }
                }




                //1. Определение центра гравитации, но только для BR IFIC (2.1)
                var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
                {
                    BroadcastingAllotment = broadcastingContextBRIFIC.Allotments,
                    BroadcastingAssignments = broadcastingContextBRIFIC.Assignments
                };

                //1.Определение центра гравитации(2.1)
                this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);


                // 2.Определение контрольных точек для записей BR IFIC -> построение контуров для выделений для
                //     60, 100, 200, 300, 500, 750 и 1000 км.CreateContourFromContureByDistance(если присутствует выделение) 1.1.6 или CreateContourFromPointByDistance если его нет.
                var distances = new int[7] { 60, 100, 200, 300, 500, 750, 1000 };
                for (int i = 0; i < distances.Length; i++)
                {

                    if (broadcastingContextBRIFIC.Allotments == null)
                    {
                        try
                        {
                            pointEarthGeometricsResult = _pointEarthGeometricPool.Take();
                            pointEarthGeometricsResultBRIFIC = _pointEarthGeometricPool.Take();

                            var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                            {
                                Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                                Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                PointEarthGeometricCalc = pointEarthGeometricBarycenter
                            };

                            this._earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                            for (int k = 0; k < sizeResultBuffer; k++)
                            {
                                var pointForCalcFS = new Point()
                                {
                                    Longitude = pointEarthGeometricsResult[k].Longitude,
                                    Latitude = pointEarthGeometricsResult[k].Latitude
                                };

                                //3.Расчет напряженности поля в точках(2.2) для BR IFIC(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                var propModel = this._ge06CalcData.PropagationModel;
                                GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                ge06CalcData.PropagationModel = propModel;



                                var countoursPointBRIFIC = new CountoursPoint();
                                countoursPointBRIFIC.Distance = distances[i];
                                countoursPointBRIFIC.Lon_DEC = pointForCalcFS.Longitude;
                                countoursPointBRIFIC.Lat_DEC = pointForCalcFS.Latitude;
                                //countoursPoints[t].Height = ??????????
                                countoursPointBRIFIC.PointType = PointType.Etalon;
                                countoursPointBRIFIC.FS = (int)CalcFieldStrengthInPointGE06(ge06CalcData, in pointForCalcFS, BroadcastingTypeContext.Brific);
                                var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                {
                                    Longitude_dec = pointForCalcFS.Longitude,
                                    Latitude_dec = pointForCalcFS.Latitude
                                });

                                dicCountoursPointsByBRIFIC.Add(countoursPointBRIFIC, adm);



                                //4.Расчет напряженности поля в точках(2.2) для ICSM (Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                propModel = this._ge06CalcData.PropagationModel;
                                GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                ge06CalcData.PropagationModel = propModel;

                                var countoursPointICSM = new CountoursPoint();
                                countoursPointICSM.Distance = distances[i];
                                countoursPointICSM.Lon_DEC = pointForCalcFS.Longitude;
                                countoursPointICSM.Lat_DEC = pointForCalcFS.Latitude;
                                //countoursPoints[t].Height = ??????????
                              
                                countoursPointICSM.PointType = PointType.Correct;
                               

                                countoursPointICSM.FS = (int)CalcFieldStrengthInPointGE06(ge06CalcData, in pointForCalcFS, BroadcastingTypeContext.Icsm);
                                adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                {
                                    Longitude_dec = pointForCalcFS.Longitude,
                                    Latitude_dec = pointForCalcFS.Latitude
                                });

                                dicCountoursPointsByICSM.Add(countoursPointICSM, adm);

                            }

                            /// Определение напряженности поля для затронутой службы (таблица ТАБЛИЦА A.1.1 документа GE06 взять http://redmine3.lissoft.com.ua:3003/issues/697) в качестве определяющих данных это технология Broadcasting,  частота и служба stn_cls (п. Системы, испытующие влияние).
                            var triggersFS = thresholdFieldStrengths.Select(x => x.ThresholdFS);

                            if (triggersFS != null)
                            {
                                var arrTriggersFS = triggersFS.ToArray();
                                for (int d = 0; d < arrTriggersFS.Length; d++)
                                {
                                    var triggerFS = arrTriggersFS[d];

                                    // Построение контура (ов) для напряженности поля относительно центра гравитации для BRIFIC.
                                    var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                                    {
                                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                        TriggerFieldStrength = triggerFS,
                                        BaryCenter = pointEarthGeometricBarycenter
                                    };

                                    var propModel = this._ge06CalcData.PropagationModel;
                                    GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                    this._ge06CalcData.PropagationModel = propModel;

                                    this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthBRIFIC(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultBRIFIC, out int sizeResultBufferBRIFIC);
                                    if (sizeResultBufferBRIFIC > 0)
                                    {
                                        var countoursPoints = new CountoursPoint[sizeResultBufferBRIFIC];
                                        for (int t = 0; t < sizeResultBufferBRIFIC; t++)
                                        {
                                            countoursPoints[t] = new CountoursPoint();

                                            var pointForCalcFsBRIFIC = new Point()
                                            {
                                                Longitude = pointEarthGeometricsResultBRIFIC[t].Longitude,
                                                Latitude = pointEarthGeometricsResultBRIFIC[t].Latitude
                                            };

                                            // Расчет напряженности поля в каждой точке, полученной для контура в п 6: для BroadcastingAssignment[] +BroadcastingAllotment(ICSM) используя функцию 2.2.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).

                                            propModel = this._ge06CalcData.PropagationModel;
                                            GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                            this._ge06CalcData.PropagationModel = propModel;

                                            var fs = (int)CalcFieldStrengthInPointGE06(this._ge06CalcData, in pointForCalcFsBRIFIC, BroadcastingTypeContext.Icsm);

                                            if (triggerFS != -9999)
                                            {
                                                if (fs > triggerFS)
                                                {
                                                    countoursPoints[t].PointType = PointType.Affected;
                                                }
                                                else
                                                {
                                                    countoursPoints[t].PointType = PointType.Correct;
                                                }
                                            }
                                            else
                                            {
                                                countoursPoints[t].PointType = PointType.Correct;
                                            }
                                            countoursPoints[t].FS = (int)fs;


                                            countoursPoints[t].Distance = distances[i];
                                            countoursPoints[t].Lon_DEC = pointForCalcFsBRIFIC.Longitude;
                                            countoursPoints[t].Lat_DEC = pointForCalcFsBRIFIC.Latitude;
                                            //countoursPoints[t].Height = ??????????

                                            var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                            {
                                                Longitude_dec = pointForCalcFsBRIFIC.Longitude,
                                                Latitude_dec = pointForCalcFsBRIFIC.Latitude
                                            });

                                            dicCountoursPointsByICSM.Add(countoursPoints[t], adm);

                                        }
                                    }
                                }
                            }

                        }
                        finally
                        {
                            if (pointEarthGeometricsResult != null)
                            {
                                _pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                            }
                            if (pointEarthGeometricsResultBRIFIC != null)
                            {
                                _pointEarthGeometricPool.Put(pointEarthGeometricsResultBRIFIC);
                            }
                        }

                    }
                    //или на функции CreateContourFromContureByDistance если у нас есть BroadcastingAllotment 
                    else
                    {
                        if (broadcastingContextBRIFIC.Allotments != null)
                        {
                            if (broadcastingContextBRIFIC.Allotments.AllotmentParameters != null)
                            {
                                var areaPoints = broadcastingContextBRIFIC.Allotments.AllotmentParameters.Contur;
                                if ((areaPoints != null) && (areaPoints.Length > 0))
                                {

                                    try
                                    {
                                        pointEarthGeometricsResult = _pointEarthGeometricPool.Take();
                                        pointEarthGeometricsResultBRIFIC = _pointEarthGeometricPool.Take();


                                        var pointEarthGeometrics = new PointEarthGeometric[areaPoints.Length];
                                        for (int h = 0; h < areaPoints.Length; h++)
                                        {
                                            pointEarthGeometrics[h] = new PointEarthGeometric(areaPoints[h].Lon_DEC, areaPoints[h].Lat_DEC, CoordinateUnits.deg);
                                        }

                                        var contourFromContureByDistanceArgs = new ContourFromContureByDistanceArgs()
                                        {
                                            Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                            Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                                            PointBaryCenter = pointEarthGeometricBarycenter,
                                            ContourPoints = pointEarthGeometrics
                                        };

                                        this._earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                                        for (int k = 0; k < sizeResultBuffer; k++)
                                        {
                                            var pointForCalcFS = new Point()
                                            {
                                                Longitude = pointEarthGeometricsResult[k].Longitude,
                                                Latitude = pointEarthGeometricsResult[k].Latitude
                                            };
                                            //3.Расчет напряженности поля в точках(2.2) для BR IFIC(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                            var propModel = this._ge06CalcData.PropagationModel;
                                            GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                            ge06CalcData.PropagationModel = propModel;



                                            var countoursPointBRIFIC = new CountoursPoint();
                                            countoursPointBRIFIC.Distance = distances[i];
                                            countoursPointBRIFIC.Lon_DEC = pointForCalcFS.Longitude;
                                            countoursPointBRIFIC.Lat_DEC = pointForCalcFS.Latitude;
                                            //countoursPoints[t].Height = ??????????
                                            countoursPointBRIFIC.PointType = PointType.Etalon;
                                            countoursPointBRIFIC.FS = (int)CalcFieldStrengthInPointGE06(ge06CalcData, in pointForCalcFS, BroadcastingTypeContext.Brific);
                                            var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                            {
                                                Longitude_dec = pointForCalcFS.Longitude,
                                                Latitude_dec = pointForCalcFS.Latitude
                                            });

                                            dicCountoursPointsByBRIFIC.Add(countoursPointBRIFIC, adm);




                                            propModel = this._ge06CalcData.PropagationModel;
                                            GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                            ge06CalcData.PropagationModel = propModel;

                                            //4.Расчет напряженности поля в точках(2.2) для ICSM (Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).


                                            var countoursPointICSM = new CountoursPoint();
                                            countoursPointICSM.Distance = distances[i];
                                            countoursPointICSM.Lon_DEC = pointForCalcFS.Longitude;
                                            countoursPointICSM.Lat_DEC = pointForCalcFS.Latitude;
                                            //countoursPoints[t].Height = ??????????

                                            countoursPointICSM.PointType = PointType.Correct;


                                            countoursPointICSM.FS = (int)CalcFieldStrengthInPointGE06(ge06CalcData, in pointForCalcFS, BroadcastingTypeContext.Icsm);
                                            adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                            {
                                                Longitude_dec = pointForCalcFS.Longitude,
                                                Latitude_dec = pointForCalcFS.Latitude
                                            });

                                            dicCountoursPointsByICSM.Add(countoursPointICSM, adm);
                                        }




                                        /// Определение напряженности поля для затронутой службы (таблица ТАБЛИЦА A.1.1 документа GE06 взять http://redmine3.lissoft.com.ua:3003/issues/697) в качестве определяющих данных это технология Broadcasting,  частота и служба stn_cls (п. Системы, испытующие влияние).
                                        var triggersFS = thresholdFieldStrengths.Select(x => x.ThresholdFS);

                                        if (triggersFS != null)
                                        {
                                            var arrTriggersFS = triggersFS.ToArray();
                                            for (int d = 0; d < arrTriggersFS.Length; d++)
                                            {

                                                var triggerFS = arrTriggersFS[d];

                                                // Построение контура (ов) для напряженности поля относительно центра гравитации для BRIFIC.

                                                var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                                                {
                                                    Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                                    TriggerFieldStrength = triggerFS,
                                                    BaryCenter = pointEarthGeometricBarycenter
                                                };

                                                var propModel = this._ge06CalcData.PropagationModel;
                                                GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                                this._ge06CalcData.PropagationModel = propModel;

                                                this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthBRIFIC(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultBRIFIC, out int sizeResultBufferBRIFIC);
                                                if (sizeResultBufferBRIFIC > 0)
                                                {
                                                    var countoursPoints = new CountoursPoint[sizeResultBufferBRIFIC];
                                                    for (int t = 0; t < sizeResultBufferBRIFIC; t++)
                                                    {
                                                        countoursPoints[t] = new CountoursPoint();
                                                        var pointForCalcFsBRIFIC = new Point()
                                                        {
                                                            Longitude = pointEarthGeometricsResultBRIFIC[t].Longitude,
                                                            Latitude = pointEarthGeometricsResultBRIFIC[t].Latitude
                                                        };

                                                        propModel = this._ge06CalcData.PropagationModel;
                                                        GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                                        this._ge06CalcData.PropagationModel = propModel;

                                                        // Расчет напряженности поля в каждой точке, полученной для контура в п 6: для BroadcastingAssignment[] +BroadcastingAllotment(ICSM) используя функцию 2.2.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                                        countoursPoints[t].FS = (int)CalcFieldStrengthInPointGE06(ge06CalcData, in pointForCalcFsBRIFIC, BroadcastingTypeContext.Icsm);

                                                        if (triggerFS != -9999)
                                                        {
                                                            if (countoursPoints[t].FS > triggerFS)
                                                            {
                                                                countoursPoints[t].PointType = PointType.Affected;
                                                            }
                                                            else
                                                            {
                                                                countoursPoints[t].PointType = PointType.Correct;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            countoursPoints[t].PointType = PointType.Correct;
                                                        }


                                                        countoursPoints[t].Distance = distances[i];
                                                        countoursPoints[t].Lon_DEC = pointForCalcFsBRIFIC.Longitude;
                                                        countoursPoints[t].Lat_DEC = pointForCalcFsBRIFIC.Latitude;
                                                        //countoursPoints[t].Height = ??????????

                                                        var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                                        {
                                                            Longitude_dec = pointForCalcFsBRIFIC.Longitude,
                                                            Latitude_dec = pointForCalcFsBRIFIC.Latitude
                                                        });

                                                        dicCountoursPointsByICSM.Add(countoursPoints[t], adm);

                                                    }
                                                }
                                            }
                                        }

                                    }
                                    finally
                                    {
                                        if (pointEarthGeometricsResult != null)
                                        {
                                            _pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                                        }
                                        if (pointEarthGeometricsResultBRIFIC != null)
                                        {
                                            _pointEarthGeometricPool.Put(pointEarthGeometricsResultBRIFIC);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                // формирование результата ContoursResult для BRIFIC
                if ((dicCountoursPointsByBRIFIC != null) && (dicCountoursPointsByBRIFIC.Count > 0))
                {
                    var lstCountoursPointsBRIFIC = dicCountoursPointsByBRIFIC.ToList();
                    if (lstCountoursPointsBRIFIC != null)
                    {
                        for (int i = 0; i < distances.Length; i++)
                        {
                            var distinctByDistance = lstCountoursPointsBRIFIC.FindAll(c => c.Key.Distance == distances[i]);
                            if (distinctByDistance != null)
                            {
                                var distinctAdmByAdm = distinctByDistance.Select(c => c.Value).Distinct();
                                if (distinctAdmByAdm != null)
                                {
                                    var arrDistinctAdmByDistance = distinctAdmByAdm.ToArray();
                                    for (int k = 0; k < arrDistinctAdmByDistance.Length; k++)
                                    {
                                        var listContourPoints = lstCountoursPointsBRIFIC.FindAll(c => c.Key.Distance == distances[i] && c.Value == arrDistinctAdmByDistance[k]);
                                        if (listContourPoints != null)
                                        {
                                            var contourType = ContourType.Etalon;

                                            var allPoints = listContourPoints.Select(c => c.Key).ToArray();

                                            lstContoursResultsByBRIFIC.Add(new ContoursResult()
                                            {
                                                AffectedADM = arrDistinctAdmByDistance[k],
                                                ContourType = contourType,
                                                CountoursPoints = allPoints,
                                                Distance = distances[i],
                                                PointsCount = allPoints.Length
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // формирование результата ContoursResult для ICSM
                if ((dicCountoursPointsByICSM != null) && (dicCountoursPointsByICSM.Count > 0))
                {
                    var lstCountoursPointsICSM = dicCountoursPointsByICSM.ToList();
                    if (lstCountoursPointsICSM != null)
                    {
                        for (int i = 0; i < distances.Length; i++)
                        {
                            var distinctByDistance = lstCountoursPointsICSM.FindAll(c => c.Key.Distance == distances[i]);
                            if (distinctByDistance != null)
                            {
                                var distinctAdmByAdm = distinctByDistance.Select(c => c.Value).Distinct();
                                if (distinctAdmByAdm != null)
                                {
                                    var arrDistinctAdmByDistance = distinctAdmByAdm.ToArray();
                                    for (int k = 0; k < arrDistinctAdmByDistance.Length; k++)
                                    {
                                        var listContourPoints = lstCountoursPointsICSM.FindAll(c => c.Key.Distance == distances[i] && c.Value == arrDistinctAdmByDistance[k]);
                                        if (listContourPoints != null)
                                        {

                                            var contourType = ContourType.Unknown;

                                            var allPoints = listContourPoints.Select(c => c.Key).ToList();

                                            if (allPoints.Find(x => x.PointType == PointType.Affected) != null)
                                            {
                                                contourType = ContourType.Affected;
                                            }
                                            else
                                            {
                                                contourType = ContourType.Correct;
                                            }

                                            lstContoursResultsByICSM.Add(new ContoursResult()
                                            {
                                                AffectedADM = arrDistinctAdmByDistance[k],
                                                ContourType = contourType,
                                                CountoursPoints = allPoints.ToArray(),
                                                Distance = distances[i],
                                                PointsCount = allPoints.Count
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var isAffectedForICSM = lstContoursResultsByICSM.Find(c => c.ContourType== ContourType.Affected);
                var isAffectedForBRIFIC = lstContoursResultsByBRIFIC.Find(c => c.ContourType == ContourType.Affected);
                var distinctAdm = lstContoursResultsByICSM.Select(c => c.AffectedADM).Distinct();
                if (distinctAdm != null)
                {
                    var arrDistinctAdmByAdm = distinctAdm.ToArray();
                    if (arrDistinctAdmByAdm.Length > 0)
                    {
                        var affectedADMRes = new AffectedADMResult[arrDistinctAdmByAdm.Length];
                        for (int k = 0; k < arrDistinctAdmByAdm.Length; k++)
                        {
                            affectedADMRes[k] = new AffectedADMResult();
                            affectedADMRes[k].ADM = arrDistinctAdmByAdm[k];
                            affectedADMRes[k].AffectedServices = string.Join(",", affectedServices);
                            affectedADMRes[k].TypeAffected = (isAffectedForICSM!=null || isAffectedForBRIFIC!=null) ? "Affected" : "Not Affected";
                        }
                        ge06CalcResult.AffectedADMResult = affectedADMRes;
                    }
                }

                lstContoursResults.AddRange(lstContoursResultsByICSM);
                lstContoursResults.AddRange(lstContoursResultsByBRIFIC);
                ge06CalcResult.ContoursResult = lstContoursResults.ToArray();


                var allotmentOrAssignmentResult = new List<AllotmentOrAssignmentResult>();

                GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBRIFIC, ref ge06CalcResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResult.AllotmentOrAssignmentResult);

                GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextICSM, ref ge06CalcResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResult.AllotmentOrAssignmentResult);

                ge06CalcResult.AllotmentOrAssignmentResult = allotmentOrAssignmentResult.ToArray();
            }
        }



        /// <summary>
        /// Расчет  для  CalculationType == FindAffectedADM
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        private void CalculationForFindAffectedADM(
                                          Ge06CalcData ge06CalcData,
                                          ref Ge06CalcResult ge06CalcResult
                                          )
        {

            if (ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM==null)
            {
                throw new Exception("Input parameters BroadcastingContextICSM is null!");
            }
            if (((GE06Validation.ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments)) && (GE06Validation.ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments))) == false)
            {
                throw new Exception("Input parameters failed validation");
            }



            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultICSM = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultAffectedICSM = default(PointEarthGeometric[]);



            var lstContoursResults = new List<ContoursResult>();

            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;


            // Определение пороговых напряженностей для защиты систем радиовещательной службы
            // входными данными являются Allotments + Assignments[]
            var thresholdFieldStrengthsPrimaryServices = new List<ThresholdFieldStrength>();
            var thresholdFieldStrengthsICSMAllotmentsP = ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextICSM.Allotments, TypeThresholdFS.OnlyBroadcastingService);
            var thresholdFieldStrengthsICSMAssignmentsP = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextICSM.Assignments, TypeThresholdFS.OnlyBroadcastingService);
            thresholdFieldStrengthsPrimaryServices.AddRange(thresholdFieldStrengthsICSMAllotmentsP);
            thresholdFieldStrengthsPrimaryServices.AddRange(thresholdFieldStrengthsICSMAssignmentsP);

            // Определение пороговых напряженностей для защиты всех служб в том числе радиовещательной службы
            // входными данными являются Allotments + Assignments[]
            var thresholdFieldStrengthsAnotherServices = new List<ThresholdFieldStrength>();
            var thresholdFieldStrengthsICSMAllotmentsA = ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextICSM.Allotments, TypeThresholdFS.All);
            var thresholdFieldStrengthsICSMAssignmentsA = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextICSM.Assignments, TypeThresholdFS.All);
            thresholdFieldStrengthsAnotherServices.AddRange(thresholdFieldStrengthsICSMAllotmentsA);
            thresholdFieldStrengthsAnotherServices.AddRange(thresholdFieldStrengthsICSMAssignmentsA);




            var dicCountoursPointsByICSM = new Dictionary<CountoursPoint, string>();

            var listAffectedADMResults = new List<AffectedADMResult>();

            ///список потенциально затронутых администраций (TypeAffected = 1000).
            var admPotentiallyAffected_1000 = new List<string>();

            // список затронутых (радиовещательные службы затронуты) администраций. TypeAffected = Broadcast
            var admPotentiallyAffected_Broadcast = new List<string>();



            var pointEarthGeometricBarycenter = new PointEarthGeometric();

            //1.Определение центра гравитации(2.1)
            if ((broadcastingContextICSM != null) && (broadcastingContextICSM != null))
            {
                var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
                {
                    BroadcastingAllotment = broadcastingContextICSM.Allotments,
                    BroadcastingAssignments = broadcastingContextICSM.Assignments
                };
                this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);
            }


            if ((broadcastingContextICSM.Allotments == null) && ((broadcastingContextICSM.Assignments != null) && (broadcastingContextICSM.Assignments.Length>0)))
            {
                var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                {
                    Distance_km = 1000,
                    Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                    PointEarthGeometricCalc = pointEarthGeometricBarycenter
                };

                try
                {
                    pointEarthGeometricsResult = _pointEarthGeometricPool.Take();
                    pointEarthGeometricsResultICSM = _pointEarthGeometricPool.Take();
                    pointEarthGeometricsResultAffectedICSM = _pointEarthGeometricPool.Take();

                    this._earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                    var countoursPoint = new CountoursPoint[sizeResultBuffer];
                    for (int k = 0; k < sizeResultBuffer; k++)
                    {
                        countoursPoint[k] = new CountoursPoint();
                        //countoursPoint[k].Distance = 1000;

                        var pointForCalc = new IdwmDataModel.Point()
                        {
                            Longitude_dec = pointEarthGeometricsResult[k].Longitude,
                            Latitude_dec = pointEarthGeometricsResult[k].Latitude
                        };

                        countoursPoint[k].Lon_DEC = pointForCalc.Longitude_dec.Value;
                        countoursPoint[k].Lat_DEC = pointForCalc.Latitude_dec.Value;

                        countoursPoint[k].PointType = PointType.Correct;


                        var adm = this._idwmService.GetADMByPoint(pointForCalc);

                        if (!admPotentiallyAffected_1000.Contains(adm))
                        {
                            admPotentiallyAffected_1000.Add(adm);
                        }
                    }

                    // формируем контур
                    lstContoursResults.Add(new ContoursResult()
                    {
                        //AffectedADM = , что здесь нужно указать???? 
                        ContourType = ContourType.Correct,
                        CountoursPoints = countoursPoint,
                        Distance = 1000,
                        PointsCount = countoursPoint.Length
                    });


                    // формируем список потенциально затронутых администраций
                    if (admPotentiallyAffected_1000.Count > 0)
                    {
                        var distAdmAffected = admPotentiallyAffected_1000.ToList().Distinct().ToArray();
                        var affectedADMRes = new AffectedADMResult[distAdmAffected.Count()];
                        for (int k = 0; k < affectedADMRes.Length; k++)
                        {
                            affectedADMRes[k] = new AffectedADMResult();
                            affectedADMRes[k].ADM = distAdmAffected[k];
                            //affectedADMRes[k].AffectedServices = stnCls; ?????????? откуда эту информацию брать ??????????
                            affectedADMRes[k].TypeAffected = "1000";
                        }
                        listAffectedADMResults.AddRange(affectedADMRes);
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ////    2.Определение администраций с затронутой радиовещательной службой.
                    ////    Результат: список затронутых(радиовещательные службы затронуты) администраций.TypeAffected = Broadcast
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                    var triggersDataPrimaryServices = thresholdFieldStrengthsPrimaryServices;
                    if ((triggersDataPrimaryServices != null) && (triggersDataPrimaryServices.Count > 0))
                    {
                        var arrTriggersFS = triggersDataPrimaryServices;
                        if (arrTriggersFS != null)
                        {
                            var triggerFSData = arrTriggersFS.ToArray();
                            for (int d = 0; d < triggerFSData.Length; d++)
                            {
                                var triggerFS = triggerFSData[d];

                                var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                                {
                                    Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                    TriggerFieldStrength = triggerFS.ThresholdFS,
                                    BaryCenter = pointEarthGeometricBarycenter
                                };

                                // Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м
                                var propModel = this._ge06CalcData.PropagationModel;
                                GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, 1, 10);
                                this._ge06CalcData.PropagationModel = propModel;

                                this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthICSM(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultICSM, out int sizeResultBufferICSM);
                                if (sizeResultBufferICSM > 0)
                                {
                                    for (int t = 0; t < sizeResultBufferICSM; t++)
                                    {
                                        var pointForCalcFsICSM = new Point()
                                        {
                                            Longitude = pointEarthGeometricsResultICSM[t].Longitude,
                                            Latitude = pointEarthGeometricsResultICSM[t].Latitude
                                        };

                                        var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                        {
                                            Longitude_dec = pointForCalcFsICSM.Longitude,
                                            Latitude_dec = pointForCalcFsICSM.Latitude
                                        });
                                        if (!admPotentiallyAffected_Broadcast.Contains(adm))
                                        {
                                            // формирование очередного AffectedADMResult
                                            var affectedADMResTemp = new AffectedADMResult();
                                            affectedADMResTemp.ADM = adm;
                                            affectedADMResTemp.AffectedServices = triggerFS.StaClass;
                                            affectedADMResTemp.TypeAffected = "Broadcast";
                                            listAffectedADMResults.Add(affectedADMResTemp);

                                            admPotentiallyAffected_Broadcast.Add(adm);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////////////////////////////
                    ////
                    ///     3.Выбор присвоений других служб, которые расположены в контуре 1000 км
                    ////    А) Расматриваються только администрации полученные в п.1
                    ///
                    /////////////////////////////////////////////////////////////////////////////////////////////////////
                    //поиск в БД BRIFIC по службе, администрации и частоте сведений по присвоениям, с которыми  может пересекаться Assignments или Allotment
                    var fmtvTerra = new List<FmtvTerra>();
                    if (admPotentiallyAffected_1000.Count > 0)
                    {
                        for (int n = 0; n < admPotentiallyAffected_1000.Count; n++)
                        {
                            var admTemp = admPotentiallyAffected_1000[n];

                            for (int j = 0; j < thresholdFieldStrengthsAnotherServices.Count; j++)
                            {
                                var triggerInformationTemp = thresholdFieldStrengthsAnotherServices[j];

                                var freqTemp_MHz = triggerInformationTemp.Freq_MHz;

                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_TDAB(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_DVBT(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingServiceAnalog_TV(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NV(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NR(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NS(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NT(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NA(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NB(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_XN(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_YN(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_ZC(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_XG(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_AB(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_AA8(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_BD(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_BA(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadFixedServices_FF(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadFixedServices_FN(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadFixedServices_FK(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_MU(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_M1_RA(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_M2(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_XA(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_XM(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_MA(admTemp, freqTemp_MHz));
                                fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_MT(admTemp, freqTemp_MHz));



                                // Определение пороговых напряженностей для защиты служб в том числе радиовещательной службы
                                // входными данными является массив полученных на пред. шаге присвоений из БРИФИК, полученных в виде массва FmtvTerra[]
                                // выходными данными будет набор пороговых значений напряженностей поля для затронутых служб по заданной частоте
                                var thresholdFieldStrengthsByFmtvTerraAnotherServices = ThresholdFS.GetThresholdFieldStrengthByFmtvTerra(fmtvTerra.ToArray(), TypeThresholdFS.All);

                                for (int b = 0; b < thresholdFieldStrengthsByFmtvTerraAnotherServices.Length; b++)
                                {

                                    var triggerInformation = thresholdFieldStrengthsByFmtvTerraAnotherServices[b];

                                    var freqMHz = triggerInformation.Freq_MHz;


                                    var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                                    {
                                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                        TriggerFieldStrength = triggerInformation.ThresholdFS,
                                        BaryCenter = pointEarthGeometricBarycenter
                                    };




                                    // Модель распространения 1546, процент территории 50, процент времени c таблиц пороговых значений, высота абонента c таблиц пороговых значений
                                    var propModel = this._ge06CalcData.PropagationModel;
                                    GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, triggerInformation.Time_pc, triggerInformation.Height_m);
                                    this._ge06CalcData.PropagationModel = propModel;

                                    this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthICSM(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultAffectedICSM, out int sizeResultBufferRecalcICSM);
                                    if (sizeResultBufferRecalcICSM > 0)
                                    {

                                        /// проверка попадания найденных ранее точек "fmtvTerra" из БРИФИК для заданной администрации в контур pointEarthGeometricsResultAffectedICSM
                                        /// если есть хотябы одна точка некоторой адиминистрации, которая попадает в этот контур pointEarthGeometricsResultAffectedICSM, то флагу isFindAffectedPointsInContour выставляем true
                                        string typeAffected = "";
                                        for (int m = 0; m < fmtvTerra.Count; m++)
                                        {

                                            var countourPointsForCheckHittingArgs = new PointEarthGeometric[sizeResultBufferRecalcICSM];
                                            for (int t = 0; t < sizeResultBufferRecalcICSM; t++)
                                            {
                                                countourPointsForCheckHittingArgs[t] = new PointEarthGeometric()
                                                {
                                                    Longitude = pointEarthGeometricsResultAffectedICSM[t].Longitude,
                                                    Latitude = pointEarthGeometricsResultAffectedICSM[t].Latitude,
                                                    CoordinateUnits = CoordinateUnits.deg
                                                };
                                            }

                                            var checkHittingArgs = new CheckHittingArgs()
                                            {
                                                Poligon = countourPointsForCheckHittingArgs,
                                                Point = new PointEarthGeometric()
                                                {
                                                    Longitude = fmtvTerra[m].Longitude_dec,
                                                    Latitude = fmtvTerra[m].Latitude_dec,
                                                    CoordinateUnits = CoordinateUnits.deg
                                                }
                                            };

                                            // если точка  fmtvTerra[m].long_dec, fmtvTerra[m].lat_dec принадлежит контуру  pointEarthGeometricsResultAffectedICSM
                                            // то переменной typeAffected присваивается значение "Assignment"
                                            if (this._earthGeometricService.CheckHitting(in checkHittingArgs))
                                            {
                                                typeAffected = "Assignment";
                                                break;
                                            }
                                        }





                                        var countoursPoints = new CountoursPoint[sizeResultBufferRecalcICSM];
                                        for (int t = 0; t < sizeResultBufferRecalcICSM; t++)
                                        {
                                            // нахождение администрации к которой принадлежит точка контура pointEarthGeometricsResultAffectedICSM
                                            var adminRecalc = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                            {
                                                Longitude_dec = pointEarthGeometricsResultAffectedICSM[t].Longitude,
                                                Latitude_dec = pointEarthGeometricsResultAffectedICSM[t].Latitude
                                            });

                                            // если точка контура pointEarthGeometricsResultAffectedICSM принадлежит администрации admTemp
                                            // то переменной typeAffected присваивается значение "Territory"
                                            if (adminRecalc == admTemp)
                                            {
                                                if (typeAffected != "Assignment")
                                                {
                                                    typeAffected = "Territory";
                                                }
                                            }
                                            else
                                            {
                                                // если точка контура не пересекает администрацию admTemp,
                                                //тогда рассматривается следующая точка контура
                                                if (typeAffected != "Assignment")
                                                {
                                                    continue;
                                                }
                                            }


                                            countoursPoints[t] = new CountoursPoint();
                                            var pointForCalcFs = new Point()
                                            {
                                                Longitude = pointEarthGeometricsResultAffectedICSM[t].Longitude,
                                                Latitude = pointEarthGeometricsResultAffectedICSM[t].Latitude
                                            };

                                            //propModel = this._ge06CalcData.PropagationModel;
                                            //GetPropagationModelForConformityCheck(ref propModel, 50, triggerInformation.Time_pc, triggerInformation.Height_m);
                                            //ge06CalcData.PropagationModel = propModel;

                                            // Расчет напряженности поля в каждой точке, полученной для контура в п 6: для BroadcastingAssignment[] +BroadcastingAllotment(ICSM) используя функцию 2.2.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                            //countoursPoints[t].FS = CalcFieldStrengthInPointGE06(ge06CalcData, in pointForCalcFs, BroadcastingTypeContext.Icsm);

                                            countoursPoints[t].PointType = PointType.Affected;


                                            countoursPoints[t].Distance = 1000;
                                            countoursPoints[t].Lon_DEC = pointForCalcFs.Longitude;
                                            countoursPoints[t].Lat_DEC = pointForCalcFs.Latitude;
                                            //countoursPoints[t].Height = ??????????


                                            var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                            {
                                                Longitude_dec = pointForCalcFs.Longitude,
                                                Latitude_dec = pointForCalcFs.Latitude
                                            });


                                            dicCountoursPointsByICSM.Add(countoursPoints[t], adm);


                                        }

                                        if (!string.IsNullOrEmpty(typeAffected))
                                        {
                                            if (listAffectedADMResults.Find(x => x.TypeAffected == typeAffected && x.ADM == admTemp && x.AffectedServices.Contains(triggerInformation.StaClass)) == null)
                                            {
                                                // формирование очередного AffectedADMResult
                                                var affectedADMResTemp = new AffectedADMResult();
                                                affectedADMResTemp.ADM = admTemp;
                                                affectedADMResTemp.AffectedServices = triggerInformation.StaClass;
                                                affectedADMResTemp.TypeAffected = typeAffected;
                                                listAffectedADMResults.Add(affectedADMResTemp);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                finally
                {
                    if (pointEarthGeometricsResult != null)
                    {
                        _pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                    }
                    if (pointEarthGeometricsResultICSM != null)
                    {
                        _pointEarthGeometricPool.Put(pointEarthGeometricsResultICSM);
                    }
                    if (pointEarthGeometricsResultAffectedICSM != null)
                    {
                        _pointEarthGeometricPool.Put(pointEarthGeometricsResultAffectedICSM);
                    }
                }
            }
            //или на функции CreateContourFromContureByDistance если у нас есть BroadcastingAllotment 
            else
            {
                var areaPoints = broadcastingContextICSM.Allotments.AllotmentParameters.Contur;
                var freq_MHz = broadcastingContextICSM.Allotments.Target.Freq_MHz;
                var isDigital = broadcastingContextICSM.Allotments.AdminData.IsDigital;
                var stnCls = broadcastingContextICSM.Allotments.AdminData.StnClass;
                if ((areaPoints != null) && (areaPoints.Length > 0))
                {
                    var pointEarthGeometrics = new PointEarthGeometric[areaPoints.Length];
                    for (int h = 0; h < areaPoints.Length; h++)
                    {
                        pointEarthGeometrics[h] = new PointEarthGeometric(areaPoints[h].Lon_DEC, areaPoints[h].Lat_DEC, CoordinateUnits.deg);
                    }

                    var contourFromContureByDistanceArgs = new ContourFromContureByDistanceArgs()
                    {
                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                        Distance_km = 1000,
                        PointBaryCenter = pointEarthGeometricBarycenter,
                        ContourPoints = pointEarthGeometrics
                    };


                    try
                    {
                        /// Определение напряженности поля для затронутой службы (таблица ТАБЛИЦА A.1.1 документа GE06 взять http://redmine3.lissoft.com.ua:3003/issues/697) в качестве определяющих данных это технология Broadcasting,  частота и служба stn_cls (п. Системы, испытующие влияние).


                        pointEarthGeometricsResult = _pointEarthGeometricPool.Take();
                        pointEarthGeometricsResultICSM = _pointEarthGeometricPool.Take();
                        pointEarthGeometricsResultAffectedICSM = _pointEarthGeometricPool.Take();

                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////
                        ///   1 .Построение контура 1000км
                        ////  Результат: контур и список потенциально затронутых администраций (TypeAffected = 1000).
                        ///
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        this._earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);
                        var countoursPoint = new CountoursPoint[sizeResultBuffer];
                        for (int k = 0; k < sizeResultBuffer; k++)
                        {
                            countoursPoint[k] = new CountoursPoint();
                            //countoursPoint[k].Distance = 1000;

                            var pointForCalc = new IdwmDataModel.Point()
                            {
                                Longitude_dec = pointEarthGeometricsResult[k].Longitude,
                                Latitude_dec = pointEarthGeometricsResult[k].Latitude
                            };

                            countoursPoint[k].Lon_DEC = pointForCalc.Longitude_dec.Value;
                            countoursPoint[k].Lat_DEC = pointForCalc.Latitude_dec.Value;

                            countoursPoint[k].PointType = PointType.Correct;


                            var adm = this._idwmService.GetADMByPoint(pointForCalc);

                            if (!admPotentiallyAffected_1000.Contains(adm))
                            {
                                admPotentiallyAffected_1000.Add(adm);
                            }
                        }

                        // формируем контур
                        lstContoursResults.Add(new ContoursResult()
                        {
                            //AffectedADM = , что здесь нужно указать???? 
                            ContourType = ContourType.Correct,
                            CountoursPoints = countoursPoint,
                            Distance = 1000,
                            PointsCount = countoursPoint.Length
                        });


                        // формируем список потенциально затронутых администраций
                        if (admPotentiallyAffected_1000.Count > 0)
                        {
                            var distAdmAffected = admPotentiallyAffected_1000.ToList().Distinct().ToArray();
                            var affectedADMRes = new AffectedADMResult[distAdmAffected.Count()];
                            for (int k = 0; k < affectedADMRes.Length; k++)
                            {
                                affectedADMRes[k] = new AffectedADMResult();
                                affectedADMRes[k].ADM = distAdmAffected[k];
                                affectedADMRes[k].AffectedServices = stnCls;
                                affectedADMRes[k].TypeAffected = "1000";
                            }
                            listAffectedADMResults.AddRange(affectedADMRes);
                        }

                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////    2.Определение администраций с затронутой радиовещательной службой.
                        ////    Результат: список затронутых(радиовещательные службы затронуты) администраций.TypeAffected = Broadcast
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                        var triggersDataPrimaryServices = thresholdFieldStrengthsPrimaryServices;
                        if ((triggersDataPrimaryServices != null) && (triggersDataPrimaryServices.Count > 0))
                        {
                            var arrTriggersFS = triggersDataPrimaryServices;
                            if (arrTriggersFS != null)
                            {
                                var triggerFSData = arrTriggersFS.ToArray();
                                for (int d = 0; d < triggerFSData.Length; d++)
                                {
                                    var triggerFS = triggerFSData[d];

                                    var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                                    {
                                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                        TriggerFieldStrength = triggerFS.ThresholdFS,
                                        BaryCenter = pointEarthGeometricBarycenter
                                    };

                                    // Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м
                                    var propModel = this._ge06CalcData.PropagationModel;
                                    GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, 1, 10);
                                    this._ge06CalcData.PropagationModel = propModel;

                                    this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthICSM(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultICSM, out int sizeResultBufferICSM);
                                    if (sizeResultBufferICSM > 0)
                                    {
                                        for (int t = 0; t < sizeResultBufferICSM; t++)
                                        {
                                            var pointForCalcFsICSM = new Point()
                                            {
                                                Longitude = pointEarthGeometricsResultICSM[t].Longitude,
                                                Latitude = pointEarthGeometricsResultICSM[t].Latitude
                                            };

                                            var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                            {
                                                Longitude_dec = pointForCalcFsICSM.Longitude,
                                                Latitude_dec = pointForCalcFsICSM.Latitude
                                            });
                                            if (!admPotentiallyAffected_Broadcast.Contains(adm))
                                            {
                                                // формирование очередного AffectedADMResult
                                                var affectedADMResTemp = new AffectedADMResult();
                                                affectedADMResTemp.ADM = adm;
                                                affectedADMResTemp.AffectedServices = triggerFS.StaClass;
                                                affectedADMResTemp.TypeAffected = "Broadcast";
                                                listAffectedADMResults.Add(affectedADMResTemp);

                                                admPotentiallyAffected_Broadcast.Add(adm);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //////////////////////////////////////////////////////////////////////////////////////////////////
                        ////
                        ///     3.Выбор присвоений других служб, которые расположены в контуре 1000 км
                        ////    А) Расматриваються только администрации полученные в п.1
                        ///
                        /////////////////////////////////////////////////////////////////////////////////////////////////////
                        //поиск в БД BRIFIC по службе, администрации и частоте сведений по присвоениям, с которыми  может пересекаться Assignments или Allotment
                        var fmtvTerra = new List<FmtvTerra>();
                        if (admPotentiallyAffected_1000.Count > 0)
                        {
                            for (int n = 0; n < admPotentiallyAffected_1000.Count; n++)
                            {
                                var admTemp = admPotentiallyAffected_1000[n];

                                for (int j = 0; j < thresholdFieldStrengthsAnotherServices.Count; j++)
                                {
                                    var triggerInformationTemp = thresholdFieldStrengthsAnotherServices[j];

                                    var freqTemp_MHz = triggerInformationTemp.Freq_MHz;

                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_TDAB(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_DVBT(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingServiceAnalog_TV(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NV(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NR(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NS(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NT(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NA(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_NB(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_XN(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_YN(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadBroadcastingService_ZC(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_XG(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_AB(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_AA8(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_BD(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadNavigationServices_BA(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadFixedServices_FF(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadFixedServices_FN(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadFixedServices_FK(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_MU(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_M1_RA(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_M2(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_XA(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_XM(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_MA(admTemp, freqTemp_MHz));
                                    fmtvTerra.AddRange(LoadDataBrific.LoadMobileServices_MT(admTemp, freqTemp_MHz));



                                    // Определение пороговых напряженностей для защиты служб в том числе радиовещательной службы
                                    // входными данными является массив полученных на пред. шаге присвоений из БРИФИК, полученных в виде массва FmtvTerra[]
                                    // выходными данными будет набор пороговых значений напряженностей поля для затронутых служб по заданной частоте
                                    var thresholdFieldStrengthsByFmtvTerraAnotherServices = ThresholdFS.GetThresholdFieldStrengthByFmtvTerra(fmtvTerra.ToArray(), TypeThresholdFS.All);

                                    for (int b = 0; b < thresholdFieldStrengthsByFmtvTerraAnotherServices.Length; b++)
                                    {

                                        var triggerInformation = thresholdFieldStrengthsByFmtvTerraAnotherServices[b];

                                        var freqMHz = triggerInformation.Freq_MHz;


                                        var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                                        {
                                            Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                            TriggerFieldStrength = triggerInformation.ThresholdFS,
                                            BaryCenter = pointEarthGeometricBarycenter
                                        };




                                        // Модель распространения 1546, процент территории 50, процент времени c таблиц пороговых значений, высота абонента c таблиц пороговых значений
                                        var propModel = this._ge06CalcData.PropagationModel;
                                        GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, triggerInformation.Time_pc, triggerInformation.Height_m);
                                        this._ge06CalcData.PropagationModel = propModel;

                                        this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => CalcFieldStrengthICSM(destinationPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultAffectedICSM, out int sizeResultBufferRecalcICSM);
                                        if (sizeResultBufferRecalcICSM > 0)
                                        {

                                            /// проверка попадания найденных ранее точек "fmtvTerra" из БРИФИК для заданной администрации в контур pointEarthGeometricsResultAffectedICSM
                                            /// если есть хотябы одна точка некоторой адиминистрации, которая попадает в этот контур pointEarthGeometricsResultAffectedICSM, то флагу isFindAffectedPointsInContour выставляем true
                                            string typeAffected = "";
                                            for (int m = 0; m < fmtvTerra.Count; m++)
                                            {

                                                var countourPointsForCheckHittingArgs = new PointEarthGeometric[sizeResultBufferRecalcICSM];
                                                for (int t = 0; t < sizeResultBufferRecalcICSM; t++)
                                                {
                                                    countourPointsForCheckHittingArgs[t] = new PointEarthGeometric()
                                                    {
                                                        Longitude = pointEarthGeometricsResultAffectedICSM[t].Longitude,
                                                        Latitude = pointEarthGeometricsResultAffectedICSM[t].Latitude,
                                                        CoordinateUnits = CoordinateUnits.deg
                                                    };
                                                }

                                                var checkHittingArgs = new CheckHittingArgs()
                                                {
                                                    Poligon = countourPointsForCheckHittingArgs,
                                                    Point = new PointEarthGeometric()
                                                    {
                                                        Longitude = fmtvTerra[m].Longitude_dec,
                                                        Latitude = fmtvTerra[m].Latitude_dec,
                                                        CoordinateUnits = CoordinateUnits.deg
                                                    }
                                                };

                                                // если точка  fmtvTerra[m].long_dec, fmtvTerra[m].lat_dec принадлежит контуру  pointEarthGeometricsResultAffectedICSM
                                                // то переменной typeAffected присваивается значение "Assignment"
                                                if (this._earthGeometricService.CheckHitting(in checkHittingArgs))
                                                {
                                                    typeAffected = "Assignment";
                                                    break;
                                                }
                                            }





                                            var countoursPoints = new CountoursPoint[sizeResultBufferRecalcICSM];
                                            for (int t = 0; t < sizeResultBufferRecalcICSM; t++)
                                            {
                                                // нахождение администрации к которой принадлежит точка контура pointEarthGeometricsResultAffectedICSM
                                                var adminRecalc = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                                {
                                                    Longitude_dec = pointEarthGeometricsResultAffectedICSM[t].Longitude,
                                                    Latitude_dec = pointEarthGeometricsResultAffectedICSM[t].Latitude
                                                });

                                                // если точка контура pointEarthGeometricsResultAffectedICSM принадлежит администрации admTemp
                                                // то переменной typeAffected присваивается значение "Territory"
                                                if (adminRecalc == admTemp)
                                                {
                                                    if (typeAffected != "Assignment")
                                                    {
                                                        typeAffected = "Territory";
                                                    }
                                                }
                                                else
                                                {
                                                    // если точка контура не пересекает администрацию admTemp,
                                                    //тогда рассматривается следующая точка контура
                                                    if (typeAffected != "Assignment")
                                                    {
                                                        continue;
                                                    }
                                                }


                                                countoursPoints[t] = new CountoursPoint();
                                                var pointForCalcFs = new Point()
                                                {
                                                    Longitude = pointEarthGeometricsResultAffectedICSM[t].Longitude,
                                                    Latitude = pointEarthGeometricsResultAffectedICSM[t].Latitude
                                                };

                                                //propModel = this._ge06CalcData.PropagationModel;
                                                //GetPropagationModelForConformityCheck(ref propModel, 50, triggerInformation.Time_pc, triggerInformation.Height_m);
                                                //ge06CalcData.PropagationModel = propModel;

                                                // Расчет напряженности поля в каждой точке, полученной для контура в п 6: для BroadcastingAssignment[] +BroadcastingAllotment(ICSM) используя функцию 2.2.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                                //countoursPoints[t].FS = CalcFieldStrengthInPointGE06(ge06CalcData, in pointForCalcFs, BroadcastingTypeContext.Icsm);

                                                countoursPoints[t].FS = (int)triggerInformation.ThresholdFS;
                                                countoursPoints[t].PointType = PointType.Affected;


                                                countoursPoints[t].Distance = 1000;
                                                countoursPoints[t].Lon_DEC = pointForCalcFs.Longitude;
                                                countoursPoints[t].Lat_DEC = pointForCalcFs.Latitude;
                                                //countoursPoints[t].Height = ??????????


                                                var adm = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                                {
                                                    Longitude_dec = pointForCalcFs.Longitude,
                                                    Latitude_dec = pointForCalcFs.Latitude
                                                });


                                                dicCountoursPointsByICSM.Add(countoursPoints[t], adm);


                                            }

                                            if (!string.IsNullOrEmpty(typeAffected))
                                            {
                                                if (listAffectedADMResults.Find(x => x.TypeAffected == typeAffected && x.ADM == admTemp && x.AffectedServices.Contains(triggerInformation.StaClass)) == null)
                                                {
                                                    // формирование очередного AffectedADMResult
                                                    var affectedADMResTemp = new AffectedADMResult();
                                                    affectedADMResTemp.ADM = admTemp;
                                                    affectedADMResTemp.AffectedServices = triggerInformation.StaClass;
                                                    affectedADMResTemp.TypeAffected = typeAffected;
                                                    listAffectedADMResults.Add(affectedADMResTemp);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (pointEarthGeometricsResult != null)
                        {
                            _pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                        }
                        if (pointEarthGeometricsResultICSM != null)
                        {
                            _pointEarthGeometricPool.Put(pointEarthGeometricsResultICSM);
                        }
                        if (pointEarthGeometricsResultAffectedICSM != null)
                        {
                            _pointEarthGeometricPool.Put(pointEarthGeometricsResultAffectedICSM);
                        }
                    }
                }
            }

            // формирование результата ContoursResult для ICSM
            var lstCountoursPointsICSM = dicCountoursPointsByICSM.ToList();
            if (lstCountoursPointsICSM != null)
            {

                var distinctAdmByAdm = lstCountoursPointsICSM.Select(c => c.Value).Distinct();
                if (distinctAdmByAdm != null)
                {
                    var arrDistinctAdmByDistance = distinctAdmByAdm.ToArray();
                    for (int k = 0; k < arrDistinctAdmByDistance.Length; k++)
                    {
                        var listContourPoints = lstCountoursPointsICSM.FindAll(c => c.Value == arrDistinctAdmByDistance[k]);
                        if (listContourPoints != null)
                        {

                            var contourType = ContourType.Unknown;

                            var allPoints = listContourPoints.Select(c => c.Key).ToList();

                            if (allPoints.Find(x => x.PointType == PointType.Affected) != null)
                            {
                                contourType = ContourType.Affected;
                            }
                            else
                            {
                                contourType = ContourType.Correct;
                            }

                            lstContoursResults.Add(new ContoursResult()
                            {
                                AffectedADM = arrDistinctAdmByDistance[k],
                                ContourType = contourType,
                                CountoursPoints = allPoints.ToArray(),
                                //Distance = distances[i],
                                PointsCount = allPoints.Count
                            });
                        }
                    }
                }
            }


            ge06CalcResult.AffectedADMResult = listAffectedADMResults.ToArray();
            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            GE06FillData.FillAllotmentOrAssignmentResult(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM, ref ge06CalcResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationPoint"></param>
        /// <returns></returns>
        public  double CalcFieldStrengthBRIFIC(PointEarthGeometric destinationPoint)
        {
            var ge06CalcData = this._ge06CalcData;
            var point = new Point()
            {
                Longitude = destinationPoint.Longitude,
                Latitude = destinationPoint.Latitude,
                Height_m = this._ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue ? this._ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value : 0
            };
            return CalcFieldStrengthInPointGE06(ge06CalcData, in point, BroadcastingTypeContext.Brific);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationPoint"></param>
        /// <returns></returns>
        public double CalcFieldStrengthICSM(PointEarthGeometric destinationPoint)
        {
            var ge06CalcData = this._ge06CalcData;
            var point = new Point()
            {
                Longitude = destinationPoint.Longitude,
                Latitude = destinationPoint.Latitude,
                Height_m = this._ge06CalcData.Ge06TaskParameters.SubscribersHeight.HasValue ? this._ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value : 0
            };
            return CalcFieldStrengthInPointGE06(ge06CalcData, in point, BroadcastingTypeContext.Icsm);
        }

       

        


        /// <summary>
        /// Расчет напряженности поля в точке 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private float CalcFieldStrengthInPointGE06(Ge06CalcData ge06CalcData, in Point point, BroadcastingTypeContext  broadcastingTypeContext)
        {
            //Ge06TaskParameters ge06TaskParameters, PropagationModel propagationModel, BroadcastingContextBase broadcastingContextBase
            BroadcastingContextBase broadcastingContext = null;
            if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
            {
                broadcastingContext = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            }
            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
            {
                broadcastingContext = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;
            }

            float resultFieldStrengthCalcResultAllotment = -9999;
            var lstFieldStrengthAssignments = new List<float>();
            //1.Необходимо проверить установлена ли необходимая модель распространения и ее параметры являются ли корректными для данного рода расчета(2.2.3). 
            var propagModel = GE06PropagationModel.GetPropagationModel(ge06CalcData.Ge06TaskParameters, ge06CalcData.PropagationModel, (CalculationType)ge06CalcData.Ge06TaskParameters.CalculationTypeCode);
            //2.Далее если есть выделение, то необходимо рассчитать напряженность поля от выделения(2.2.1).
            if (broadcastingContext.Allotments != null)
            {
                 resultFieldStrengthCalcResultAllotment = CalcFieldStrengthInPointFromAllotmentGE06(broadcastingContext.Allotments, ge06CalcData.PropagationModel, point);
            }
            //3.Если есть BroadcastingAssignment, то рассчитать напряженность поля от каждой из них(2.2.4).При этом если станций несколько, то определяем суммарную напряженность поля для станций с одинаковым SFN_id методом суммирования мощностей(2.2.2).
            if ((broadcastingContext.Assignments != null) && (broadcastingContext.Assignments.Length>0))
            {
                var allAssignments = broadcastingContext.Assignments.ToList();
                var allSfnId = allAssignments.Select(x => x.DigitalPlanEntryParameters.SfnId);
                if ((allSfnId!=null) && (allSfnId.Count()>0))
                {
                    allSfnId = allSfnId.Distinct();
                    var arrSfnId = allSfnId.ToArray();
                    for (int k=0; k< arrSfnId.Length; k++)
                    {
                        var fndAssignments = allAssignments.FindAll(x => x.DigitalPlanEntryParameters.SfnId == arrSfnId[k]);
                        if ((fndAssignments!=null) && (fndAssignments.Count>0))
                        {
                            var sumFieldStrengthInPointFromAssignmentGE06 = new double[fndAssignments.Count];
                            for (int j = 0; j < fndAssignments.Count; j++)
                            {
                                sumFieldStrengthInPointFromAssignmentGE06[j] = CalcFieldStrengthInPointFromAssignmentGE06(fndAssignments[j], ge06CalcData.PropagationModel, point);
                            }
                            var recalcFieldStrengthInPointFromAssignmentGE06 = SumPowGE06(sumFieldStrengthInPointFromAssignmentGE06);
                            lstFieldStrengthAssignments.Add(recalcFieldStrengthInPointFromAssignmentGE06);
                        }
                    }
                }
            }
            //4.Определить максимальную напряженность поля от станций или выделения.
            return Math.Max(resultFieldStrengthCalcResultAllotment, lstFieldStrengthAssignments.Max());
        }


        /// <summary>
        /// Расчет напряженности поля в точке от выделения
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private float CalcFieldStrengthInPointFromAllotmentGE06(BroadcastingAllotment broadcastingAllotment, PropagationModel propagationModel, Point point)
        {
            var lstAllFieldStrengthAssignments = new List<double>();
            //1. Формирование эталонной BroadcastingAssignment на базе BroadcastingAllotment (1.3.1).
            var broadcastingAssignment = new BroadcastingAssignment(); 
            this._gn06Service.GetEtalonBroadcastingAssignmentFromAllotment(broadcastingAllotment, broadcastingAssignment);
            //2. Вычисляются все граничные точки выделения (1.3.3). 
            var broadcastingAllotmentWithStep = new BroadcastingAllotmentWithStep()
            {
                 BroadcastingAllotment = broadcastingAllotment
            };
            var points = new Points();
            this._gn06Service.GetBoundaryPointsFromAllotments(in broadcastingAllotmentWithStep, ref points);

            //3.Для каждой граничной точки формируется цикл по расчету напряженности поля от эталонной сети которая касается граничной точки в точке(исходные данные). При этом расчете совершаются 3 действия:
            var pointsWithAzimuthResult = new PointsWithAzimuthResult();
            pointsWithAzimuthResult.PointsWithAzimuth = new PointWithAzimuth[7];
            var lstBroadcastingAssignment = new BroadcastingAssignment[7];
            double maxFieldStrength = -9999;
            for (int j=0; j< points.SizeResultBuffer; j++)
            {

                var estimationAssignmentsPointsArgs = new EstimationAssignmentsPointsArgs()
                {
                    BroadcastingAllotment = broadcastingAllotment,
                    PointCalcFieldStrength = new AreaPoint()
                    {
                        Lon_DEC = point.Longitude,
                        Lat_DEC = point.Latitude
                    },
                    PointAllotment = new AreaPoint()
                    {
                        Lon_DEC = points.PointEarthGeometrics[j].Longitude,
                        Lat_DEC = points.PointEarthGeometrics[j].Latitude
                    }
                };

                //а) Определения положений эталонных присвоений BroadcastingAssignment 1.1.5;
                
                
                this._gn06Service.EstimationAssignmentsPointsForEtalonNetwork(in estimationAssignmentsPointsArgs, ref pointsWithAzimuthResult);


                var pointWithAzimuth = pointsWithAzimuthResult.PointsWithAzimuth;
                for (int k = 0; k < pointsWithAzimuthResult.sizeResultBuffer; k++)
                {
                    var broadcastingAssignmentTemp = Atdi.Common.CopyHelper.CreateDeepCopy(broadcastingAssignment);
                    broadcastingAssignmentTemp.SiteParameters.Lon_Dec = pointWithAzimuth[k].AreaPoint.Lon_DEC;
                    broadcastingAssignmentTemp.SiteParameters.Lat_Dec = pointWithAzimuth[k].AreaPoint.Lat_DEC;
                    if (broadcastingAssignmentTemp.EmissionCharacteristics.Polar == PolarType.H)
                    {
                        broadcastingAssignmentTemp.EmissionCharacteristics.ErpH_dBW = (float)(broadcastingAssignmentTemp.EmissionCharacteristics.ErpH_dBW - pointWithAzimuth[k].AntDiscrimination_dB);
                    }
                    else if (broadcastingAssignmentTemp.EmissionCharacteristics.Polar == PolarType.V)
                    {
                        broadcastingAssignmentTemp.EmissionCharacteristics.ErpV_dBW = (float)(broadcastingAssignmentTemp.EmissionCharacteristics.ErpV_dBW - pointWithAzimuth[k].AntDiscrimination_dB);
                    }
                    else
                    {
                        broadcastingAssignmentTemp.EmissionCharacteristics.ErpH_dBW = (float)(broadcastingAssignmentTemp.EmissionCharacteristics.ErpH_dBW - pointWithAzimuth[k].AntDiscrimination_dB);
                        broadcastingAssignmentTemp.EmissionCharacteristics.ErpV_dBW = (float)(broadcastingAssignmentTemp.EmissionCharacteristics.ErpV_dBW - pointWithAzimuth[k].AntDiscrimination_dB);
                    }
                    lstBroadcastingAssignment[k] = broadcastingAssignmentTemp;
                }
                var lstFieldStrengthAssignments = new double[pointsWithAzimuthResult.sizeResultBuffer];
                for (int k = 0; k < lstBroadcastingAssignment.Length; k++)
                {
                    var broadcastAssignment = lstBroadcastingAssignment[k];
                    //в) Расчет напряженности поля от каждого эталонного BroadcastingAssignment(2.2.4).Перед расчетом производиться корректировка паттерна BroadcastingAssignment в соответствии с его ориентацией(суть корректировки спросить Максима или Юру).
                    var resultFieldStrengthInPointFromAssignmentGE06 = CalcFieldStrengthInPointFromAssignmentGE06(broadcastAssignment, propagationModel, point);
                    lstFieldStrengthAssignments[k]=resultFieldStrengthInPointFromAssignmentGE06;
                }

                if (SumPowGE06(lstFieldStrengthAssignments) > maxFieldStrength)
                {
                    maxFieldStrength = SumPowGE06(lstFieldStrengthAssignments);
                }
            }

            //г) Определение суммарной напряженности поля(2.2.2) .
            return (float)maxFieldStrength;
        }

        /// <summary>
        /// Расчет напряженности поля от BroadcastingAssignment
        /// </summary>
        /// <param name="broadcastingAssignment"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private float CalcFieldStrengthInPointFromAssignmentGE06(BroadcastingAssignment broadcastingAssignment, PropagationModel propagationModel, Point point)
        {
            float resultCalcFieldStrength = 0;

            if ((propagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546)||(propagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546_4)
                ||(propagationModel.AbsorptionBlock.Available == false)
                || (propagationModel.AdditionalBlock.Available == false)
                || (propagationModel.AtmosphericBlock.Available == false)
                || (propagationModel.ClutterBlock.Available == false)
                || (propagationModel.DiffractionBlock.Available == false)
                || (propagationModel.DuctingBlock.Available == false)
                || (propagationModel.ReflectionBlock.Available == false)
                || (propagationModel.SubPathDiffractionBlock.Available == false)
                || (propagationModel.TropoBlock.Available == false))
            {
                //1) Если модель распространения ITU 1546 для этого необходимо делать отдельную итерацию(Будет подготовлен сам код Юрой или Максимом) на подобии FieldStrengthCalcIteration(1.4.1, 1.4.2)
                var broadcastingFieldStrengthCalcData = new BroadcastingFieldStrengthCalcData()
                {
                    BroadcastingAssignment = broadcastingAssignment,
                    PropagationModel = propagationModel,
                    //CluttersDesc = this._ge06CalcData.CluttersDesc,
                    MapArea = this._ge06CalcData.MapData.Area,
                    //BuildingContent = this._ge06CalcData.MapData.BuildingContent,
                    ClutterContent = this._ge06CalcData.MapData.ClutterContent,
                    ReliefContent = this._ge06CalcData.MapData.ReliefContent,
                    //PointCoordinate = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate() { Longitude = broadcastingAssignment.SiteParameters.Lon_Dec, Latitude = broadcastingAssignment.SiteParameters.Lat_Dec }, this._ge06CalcData.Projection),
                    TargetCoordinate = new PointEarthGeometric() { Longitude = point.Longitude, Latitude = point.Latitude, CoordinateUnits = CoordinateUnits.deg },
                };
                var iterationCorellationCalc = _iterationsPool.GetIteration<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>();
                var resFieldStrengthCalcResult = iterationCorellationCalc.Run(this._taskContext, broadcastingFieldStrengthCalcData);
                resultCalcFieldStrength = (float)resFieldStrengthCalcResult.FS_dBuVm.Value;
            }
            else
            {
                var contextStation = new Contracts.Sdrn.DeepServices.GN06.ContextStation();
                this._gn06Service.GetStationFromBroadcastingAssignment(broadcastingAssignment, ref contextStation);
                //2) Если модель распространения не ITU 1546.В данном случае BroadcastingAssignment преобразуется в Station(IContextStation)(1.3.2) и используется для расчета итерация FieldStrengthCalcIteration
                var fieldStrengthCalcData = new FieldStrengthCalcData()
                {
                    Antenna = contextStation.ClientContextStation.Antenna,
                    Transmitter = contextStation.ClientContextStation.Transmitter,
                    PropagationModel = propagationModel,
                    CluttersDesc = this._ge06CalcData.CluttersDesc,
                    MapArea = this._ge06CalcData.MapData.Area,
                    BuildingContent = this._ge06CalcData.MapData.BuildingContent,
                    ClutterContent = this._ge06CalcData.MapData.ClutterContent,
                    ReliefContent = this._ge06CalcData.MapData.ReliefContent,
                    PointCoordinate = contextStation.ClientContextStation.Coordinate,
                    TargetCoordinate = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate() { Longitude = point.Longitude, Latitude = point.Latitude }, this._ge06CalcData.Projection),
                };
                var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
                var resFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(this._taskContext, fieldStrengthCalcData);
                resultCalcFieldStrength = (float)resFieldStrengthCalcData.FS_dBuVm.Value;
            }
            return resultCalcFieldStrength;
        }

        /// <summary>
        /// Суммирование мощностей
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private float SumPowGE06(double[] fs)
        {
            double resFS = 0.0;
            for (int j = 0; j < fs.Length; j++)
            {
                resFS += Math.Pow(10, fs[j] / 10.0);
            }
            return (float)(10 * Math.Log10(resFS));
        }

    }
}
