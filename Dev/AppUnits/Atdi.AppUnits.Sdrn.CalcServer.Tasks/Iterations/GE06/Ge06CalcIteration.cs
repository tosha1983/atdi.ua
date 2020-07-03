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
            this._taskContext = taskContext;

            this._ge06CalcData = data;

            //здесь вызов функции "переопределения" модели распространения в зависимости от входных параметров
            data.PropagationModel = GetPropagationModel(data.Ge06TaskParameters, data.PropagationModel, (CalculationType)data.Ge06TaskParameters.CalculationTypeCode);

            var ge06CalcResults = new Ge06CalcResult();
            var ge06CalcResultsForICSM = new Ge06CalcResult();
            var ge06CalcResultsForBRIFIC = new Ge06CalcResult();

            var affectedADMResult = new List<AffectedADMResult>();
            var contoursResult = new List<ContoursResult>();
            var allotmentOrAssignmentResult = new List<AllotmentOrAssignmentResult>();

           

            if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.CreateContoursByDistance)
            {
                //временная проверка
                if (data.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM!=null)
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

                CalculationForCreateContoursByDistance(in data, BroadcastingTypeContext.Icsm, ref ge06CalcResultsForICSM);
                affectedADMResult.AddRange(ge06CalcResultsForICSM.AffectedADMResult);
                contoursResult.AddRange(ge06CalcResultsForICSM.ContoursResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResultsForICSM.AllotmentOrAssignmentResult);


                CalculationForCreateContoursByDistance(in data, BroadcastingTypeContext.Icsm, ref ge06CalcResultsForBRIFIC);
                affectedADMResult.AddRange(ge06CalcResultsForBRIFIC.AffectedADMResult);
                contoursResult.AddRange(ge06CalcResultsForBRIFIC.ContoursResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult);
            }
            else if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.CreateContoursByFS)
            {

                CalculationForCreateContoursByFS(in data, BroadcastingTypeContext.Icsm, ref ge06CalcResultsForICSM);
                affectedADMResult.AddRange(ge06CalcResultsForICSM.AffectedADMResult);
                contoursResult.AddRange(ge06CalcResultsForICSM.ContoursResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResultsForICSM.AllotmentOrAssignmentResult);


                CalculationForCreateContoursByFS(in data, BroadcastingTypeContext.Icsm, ref ge06CalcResultsForBRIFIC);
                affectedADMResult.AddRange(ge06CalcResultsForBRIFIC.AffectedADMResult);
                contoursResult.AddRange(ge06CalcResultsForBRIFIC.ContoursResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResultsForBRIFIC.AllotmentOrAssignmentResult);
            }
            else if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.ConformityCheck)
            {
                CalculationForConformityCheck(in data, ref ge06CalcResults);
                affectedADMResult.AddRange(ge06CalcResults.AffectedADMResult);
                contoursResult.AddRange(ge06CalcResults.ContoursResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResults.AllotmentOrAssignmentResult);
            }
            else if ((CalculationType)data.Ge06TaskParameters.CalculationTypeCode == CalculationType.FindAffectedADM)
            {
                CalculationForFindAffectedADM(in data, ref ge06CalcResultsForICSM);
                affectedADMResult.AddRange(ge06CalcResultsForICSM.AffectedADMResult);
                contoursResult.AddRange(ge06CalcResultsForICSM.ContoursResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResultsForICSM.AllotmentOrAssignmentResult);
            }

            ge06CalcResults.AffectedADMResult = affectedADMResult.ToArray();
            ge06CalcResults.AllotmentOrAssignmentResult = allotmentOrAssignmentResult.ToArray();
            ge06CalcResults.ContoursResult = contoursResult.ToArray();

            return ge06CalcResults;
        }

        /// <summary>
        /// Валидация параметров для Allotment
        /// </summary>
        /// <param name="allotments"></param>
        /// <returns></returns>
        private bool ValidationAllotment(BroadcastingAllotment allotments)
        {
            bool isSuccess = true;
            if (allotments != null)
            {
                //AdminData
                if (allotments.AdminData == null)
                {
                    isSuccess = false;
                }
                else if (allotments.AdminData != null)
                {
                    if (string.IsNullOrEmpty(allotments.AdminData.Adm))
                    {
                        isSuccess = false;
                    }
                    if (string.IsNullOrEmpty(allotments.AdminData.NoticeType))
                    {
                        isSuccess = false;
                    }
                }
                //AllotmentParameters
                if (allotments.AllotmentParameters == null)
                {
                    isSuccess = false;
                }
                else if (allotments.AllotmentParameters != null)
                {
                    if (allotments.AllotmentParameters.ContourId == 0)
                    {
                        isSuccess = false;
                    }
                    if (string.IsNullOrEmpty(allotments.AllotmentParameters.Name))
                    {
                        isSuccess = false;
                    }
                    if (allotments.AllotmentParameters.Сontur == null)
                    {
                        isSuccess = false;
                    }
                }
                // DigitalPlanEntryParameters
                if (allotments.DigitalPlanEntryParameters == null)
                {
                    isSuccess = false;
                }
                // EmissionCharacteristics
                if (allotments.EmissionCharacteristics == null)
                {
                    isSuccess = false;
                }
                else if (allotments.EmissionCharacteristics != null)
                {
                    //if (allotments.EmissionCharacteristics.Freq_MHz) ??????????????
                }
                // Target
                if (allotments.Target == null)
                {
                    isSuccess = false;
                }
                else if (allotments.Target != null)
                {
                    if (string.IsNullOrEmpty(allotments.Target.AdmRefId))
                    {
                        isSuccess = false;
                    }
                    if (allotments.Target.Freq_MHz == 0)
                    {
                        isSuccess = false;
                    }
                    if (allotments.Target.Lon_Dec == 0)
                    {
                        isSuccess = false;
                    }
                    if (allotments.Target.Lat_Dec == 0)
                    {
                        isSuccess = false;
                    }
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Валидация параметров для Assignment
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        private bool ValidationAssignment(BroadcastingAssignment[] assignments)
        {
            bool isSuccess = true;
            if (assignments != null)
            {
                for (int i = 0; i < assignments.Length; i++)
                {
                    // AdmData
                    if (assignments[i].AdmData == null)
                    {
                        isSuccess = false;
                    }
                    else if (assignments[i].AdmData != null)
                    {
                        if (string.IsNullOrEmpty(assignments[i].AdmData.Adm))
                        {
                            isSuccess = false;
                        }
                        if (string.IsNullOrEmpty(assignments[i].AdmData.AdmRefId))
                        {
                            isSuccess = false;
                        }
                        if (string.IsNullOrEmpty(assignments[i].AdmData.Fragment))
                        {
                            isSuccess = false;
                        }
                    }
                    //AntennaCharacteristics
                    if (assignments[i].AntennaCharacteristics == null)
                    {
                        isSuccess = false;
                    }
                    else if (assignments[i].AntennaCharacteristics != null)
                    {

                        //MaxEffHeight_m
                        if (((assignments[i].AntennaCharacteristics.MaxEffHeight_m >= 0) && (assignments[i].AntennaCharacteristics.MaxEffHeight_m <= 800)) == false)
                        {
                            isSuccess = false;
                        }
                        //EffHeight_m
                        if (assignments[i].AntennaCharacteristics.EffHeight_m == null)
                        {
                            isSuccess = false;
                        }
                        else if (assignments[i].AntennaCharacteristics.EffHeight_m != null)
                        {
                            if (assignments[i].AntennaCharacteristics.EffHeight_m.Length != 36)
                            {
                                isSuccess = false;
                            }
                            for (int j = 0; j < assignments[i].AntennaCharacteristics.EffHeight_m.Length; j++)
                            {
                                var effHeight_m = assignments[i].AntennaCharacteristics.EffHeight_m[j];
                                if (((effHeight_m >= -3000) && (effHeight_m <= 3000)) == false)
                                {
                                    isSuccess = false;
                                }
                            }
                        }
                        //DiagrV
                        if (assignments[i].AntennaCharacteristics.DiagrV == null)
                        {
                            isSuccess = false;
                        }
                        else if (assignments[i].AntennaCharacteristics.DiagrV != null)
                        {
                            if (assignments[i].AntennaCharacteristics.DiagrV.Length != 36)
                            {
                                isSuccess = false;
                            }
                            for (int j = 0; j < assignments[i].AntennaCharacteristics.DiagrV.Length; j++)
                            {
                                var diagrV = assignments[i].AntennaCharacteristics.DiagrV[j];
                                if (((diagrV >= 0) && (diagrV <= 40)) == false)
                                {
                                    isSuccess = false;
                                }
                            }
                        }
                        //DiagrH
                        if (assignments[i].AntennaCharacteristics.DiagrH == null)
                        {
                            isSuccess = false;
                        }
                        else if (assignments[i].AntennaCharacteristics.DiagrH != null)
                        {
                            if (assignments[i].AntennaCharacteristics.DiagrH.Length != 36)
                            {
                                isSuccess = false;
                            }
                            for (int j = 0; j < assignments[i].AntennaCharacteristics.DiagrH.Length; j++)
                            {
                                var diagrH = assignments[i].AntennaCharacteristics.DiagrH[j];
                                if (((diagrH >= 0) && (diagrH <= 40)) == false)
                                {
                                    isSuccess = false;
                                }
                            }
                        }
                    }
                    //DigitalPlanEntryParameters
                    if (assignments[i].DigitalPlanEntryParameters == null)
                    {
                        isSuccess = false;
                    }
                    //EmissionCharacteristics
                    if (assignments[i].EmissionCharacteristics == null)
                    {
                        isSuccess = false;
                    }
                    else if (assignments[i].EmissionCharacteristics != null)
                    {
                        //if (assignments[i].EmissionCharacteristics.Freq_MHz) ?????????

                        if (assignments[i].EmissionCharacteristics.ErpH_dBW > 53)
                        {
                            isSuccess = false;
                        }
                        if (assignments[i].EmissionCharacteristics.ErpV_dBW > 53)
                        {
                            isSuccess = false;
                        }
                    }

                    //SiteParameters
                    if (assignments[i].SiteParameters == null)
                    {
                        isSuccess = false;
                    }
                    else if (assignments[i].SiteParameters != null)
                    {
                        if (((assignments[i].SiteParameters.Alt_m >= -1000) && (assignments[i].SiteParameters.Alt_m <= 8850)) == false)
                        {
                            isSuccess = false;
                        }
                        if (string.IsNullOrEmpty(assignments[i].SiteParameters.Name) == false)
                        {
                            isSuccess = false;
                        }
                    }

                    if (assignments[i].Target != null)
                    {
                        if (string.IsNullOrEmpty(assignments[i].Target.AdmRefId))
                        {
                            isSuccess = false;
                        }
                        if (assignments[i].Target.Freq_MHz == 0)
                        {
                            isSuccess = false;
                        }
                    }
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// Заполнение объектов AffectedADMResult[] +  AllotmentOrAssignmentResult
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcResult"></param>
        private void FillAllotmentOrAssignmentResult(BroadcastingContextBase broadcastingContextBase, ref Ge06CalcResult ge06CalcResult)
        {
            int countRecordsAllotmentOrAssignmentResult = 0;

            if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments != null))
            {
                countRecordsAllotmentOrAssignmentResult = broadcastingContextBase.Assignments.Length + 1;
            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments == null) && (broadcastingContextBase.Assignments != null))
            {
                countRecordsAllotmentOrAssignmentResult = broadcastingContextBase.Assignments.Length;
            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments == null))
            {
                countRecordsAllotmentOrAssignmentResult = 1;
            }

            var affectedADMResult = new AffectedADMResult[countRecordsAllotmentOrAssignmentResult];
            // как формировать AffectedADMResult ????????????????????????????????
            if (broadcastingContextBase.Assignments != null)
            {
               
                for (int k = 0; k < broadcastingContextBase.Assignments.Length; k++)
                {
                    affectedADMResult[k] = new AffectedADMResult();
                    affectedADMResult[k].ADM = broadcastingContextBase.Assignments[k].AdmData.Adm;
                    //affectedADMResult[k].AffectedServices = ???????????????????????????
                    //affectedADMResult[k].TypeAffected ??????????????????????????????
                }
            }
            if (broadcastingContextBase.Allotments != null)
            {
                affectedADMResult[affectedADMResult.Length-1].ADM = broadcastingContextBase.Allotments.AdminData.Adm;
                //affectedADMResult.AffectedServices = //???????????????????????????
                //affectedADMResult.TypeAffected = //??????????????????????????????
            }

            ge06CalcResult.AffectedADMResult = affectedADMResult;

            var  allotmentOrAssignmentResults = new AllotmentOrAssignmentResult[countRecordsAllotmentOrAssignmentResult];

            if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments != null))
            {
                for (int i = 0; i < broadcastingContextBase.Assignments.Length; i++)
                {
                    var assignment = broadcastingContextBase.Assignments[i];
                    allotmentOrAssignmentResults[i] = new AllotmentOrAssignmentResult()
                    {
                        Adm = assignment.AdmData.Adm,
                        AdmRefId = assignment.AdmData.AdmRefId,
                        Polar = assignment.EmissionCharacteristics.Polar.ToString(),
                        Name = assignment.SiteParameters.Name,
                        Longitude_DEC = assignment.SiteParameters.Lon_Dec,
                        Latitude_DEC = assignment.SiteParameters.Lat_Dec,
                        Freq_MHz = assignment.EmissionCharacteristics.Freq_MHz,
                        TypeTable = "Assignment",
                        AntennaDirectional = assignment.AntennaCharacteristics.Direction.ToString(),
                        ErpH_dbW = assignment.EmissionCharacteristics.ErpH_dBW,
                        ErpV_dbW = assignment.EmissionCharacteristics.ErpV_dBW,
                        MaxEffHeight_m = assignment.AntennaCharacteristics.MaxEffHeight_m
                    };
                }

                allotmentOrAssignmentResults[allotmentOrAssignmentResults.Length-1] = new AllotmentOrAssignmentResult()
                {
                    Adm = broadcastingContextBase.Allotments.AdminData.Adm,
                    AdmRefId = broadcastingContextBase.Allotments.AdminData.AdmRefId,
                    Polar = broadcastingContextBase.Allotments.EmissionCharacteristics.Polar.ToString(),
                    Name = broadcastingContextBase.Allotments.AllotmentParameters.Name,
                    Longitude_DEC = broadcastingContextBase.Allotments.Target.Lon_Dec,
                    Latitude_DEC = broadcastingContextBase.Allotments.Target.Lat_Dec,
                    Freq_MHz = broadcastingContextBase.Allotments.EmissionCharacteristics.Freq_MHz,
                    TypeTable = "Allotment",
                    //MaxEffHeight_m =  ?????????????????????????
                    //ErpV_dbW =  ?????????????????????????
                    //ErpH_dbW=  ?????????????????????????
                    //AntennaDirectional = calcForICSM.Allotments.Target.
                };
              
            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments == null) && (broadcastingContextBase.Assignments != null))
            {
                for (int i = 0; i < broadcastingContextBase.Assignments.Length; i++)
                {
                    var assignment = broadcastingContextBase.Assignments[i];
                    allotmentOrAssignmentResults[i] = new AllotmentOrAssignmentResult()
                    {
                        Adm = assignment.AdmData.Adm,
                        AdmRefId = assignment.AdmData.AdmRefId,
                        Polar = assignment.EmissionCharacteristics.Polar.ToString(),
                        Name = assignment.SiteParameters.Name,
                        Longitude_DEC = assignment.SiteParameters.Lon_Dec,
                        Latitude_DEC = assignment.SiteParameters.Lat_Dec,
                        Freq_MHz = assignment.EmissionCharacteristics.Freq_MHz,
                        TypeTable = "Assignment",
                        AntennaDirectional = assignment.AntennaCharacteristics.Direction.ToString(),
                        ErpH_dbW = assignment.EmissionCharacteristics.ErpH_dBW,
                        ErpV_dbW = assignment.EmissionCharacteristics.ErpV_dBW,
                        MaxEffHeight_m = assignment.AntennaCharacteristics.MaxEffHeight_m
                    };
                }
            }
            else if ((broadcastingContextBase != null) && (broadcastingContextBase.Allotments != null) && (broadcastingContextBase.Assignments == null))
            {
                allotmentOrAssignmentResults[0] = new AllotmentOrAssignmentResult()
                {
                    Adm = broadcastingContextBase.Allotments.AdminData.Adm,
                    AdmRefId = broadcastingContextBase.Allotments.AdminData.AdmRefId,
                    Polar = broadcastingContextBase.Allotments.EmissionCharacteristics.Polar.ToString(),
                    Name = broadcastingContextBase.Allotments.AllotmentParameters.Name,
                    Longitude_DEC = broadcastingContextBase.Allotments.Target.Lon_Dec,
                    Latitude_DEC = broadcastingContextBase.Allotments.Target.Lat_Dec,
                    Freq_MHz = broadcastingContextBase.Allotments.EmissionCharacteristics.Freq_MHz,
                    TypeTable = "Allotment",
                    //MaxEffHeight_m =  ?????????????????????????
                    //ErpV_dbW =  ?????????????????????????
                    //ErpH_dbW=  ?????????????????????????
                    //AntennaDirectional = calcForICSM.Allotments.Target.
                };
            }

            ge06CalcResult.AllotmentOrAssignmentResult = allotmentOrAssignmentResults;
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

            //if (((ValidationAssignment(broadcastingContextBase.Assignments)) && (ValidationAllotment(broadcastingContextBase.Allotments)))==false)
            //{
            //    throw new Exception("Input parameters failed validation");
            //}

            var lstContoursResults = new List<ContoursResult>();

            var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
            {
                BroadcastingAllotment = broadcastingContextBase.Allotments,
                BroadcastingAssignments = broadcastingContextBase.Assignments
            };
            //1.Определение центра гравитации(2.1)
            var pointEarthGeometric = new PointEarthGeometric();
            this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometric);

            for (int i = 0; i < ge06CalcData.Ge06TaskParameters.Distances.Length; i++)
            {
                // 2. Построение контуров фиксированной дистанции относительно центра гравитации.
                // Базируемся на функции CreateContourFromPointByDistance если у нас только BroadcastingAssignment []

                if (broadcastingContextBase.Allotments == null)
                {
                    var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                    {
                        Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                        PointEarthGeometricCalc = pointEarthGeometric
                    };


                    try
                    {
                        pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                        this._earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                        var contoursResult = new ContoursResult[sizeResultBuffer];

                        for (int k = 0; k < sizeResultBuffer; k++)
                        {
                            var point = new Point()
                            {
                                Longitude = pointEarthGeometricsResult[k].Longitude,
                                Latitude = pointEarthGeometricsResult[k].Latitude
                            };

                            contoursResult[k].FS = CalcFieldStrengthInPointGE06(in ge06CalcData, in point, broadcastingContextBase);
                            contoursResult[k].AffectedADM = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                            {
                                Longitude_dec = point.Longitude,
                                Latitude_dec = point.Latitude
                            });
                            if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                            {
                                contoursResult[k].ContourType = ContourType.Etalon;
                                //contoursResult[k].CountoursPoints = ???????????????????? что тут передавать????????
                            }
                            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                            {
                                contoursResult[k].ContourType = ContourType.New;
                                //contoursResult[k].CountoursPoints = ???????????????????? что тут передавать????????
                            }
                            contoursResult[k].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                            //contoursResult[k].PointsCount = ???????????????????? что здесь указать????????
                        }

                        lstContoursResults.AddRange(contoursResult);
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
                    var areaPoints = broadcastingContextBase.Allotments.AllotmentParameters.Сontur;
                    if ((areaPoints != null) && (areaPoints.Length > 0))
                    {
                        var pointEarthGeometrics = new PointEarthGeometric[areaPoints.Length];
                        for (int h=0; h< areaPoints.Length; h++)
                        {
                            pointEarthGeometrics[h] = new PointEarthGeometric(areaPoints[h].Lon_DEC, areaPoints[h].Lat_DEC, CoordinateUnits.deg);
                        }

                        var contourFromContureByDistanceArgs = new ContourFromContureByDistanceArgs()
                        {
                            Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                            Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                            PointBaryCenter = pointEarthGeometric,
                            ContourPoints = pointEarthGeometrics
                        };


                        
                        try
                        {
                            pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                            this._earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                            var contoursResult = new ContoursResult[sizeResultBuffer];
                            for (int k = 0; k < sizeResultBuffer; k++)
                            {
                                var point = new Point()
                                {
                                    Longitude = pointEarthGeometricsResult[k].Longitude,
                                    Latitude = pointEarthGeometricsResult[k].Latitude
                                };
                                contoursResult[k].FS = CalcFieldStrengthInPointGE06(in ge06CalcData, in point, broadcastingContextBase);
                                contoursResult[k].AffectedADM = this._idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                {
                                    Longitude_dec = point.Longitude,
                                    Latitude_dec = point.Latitude
                                });
                                if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
                                {
                                    contoursResult[k].ContourType = ContourType.Etalon;
                                    //contoursResult[k].CountoursPoints = ???????????????????? что тут передавать????????
                                }
                                if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
                                {
                                    contoursResult[k].ContourType = ContourType.New;
                                    //contoursResult[k].CountoursPoints = ???????????????????? что тут передавать????????
                                }
                                contoursResult[k].Distance = ge06CalcData.Ge06TaskParameters.Distances[i];
                                //contoursResult[k].PointsCount = ???????????????????? что здесь указать????????
                            }

                            lstContoursResults.AddRange(contoursResult);

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
            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            FillAllotmentOrAssignmentResult(broadcastingContextBase, ref ge06CalcResult);
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

            BroadcastingContextBase broadcastingContextBase = null;
            if (broadcastingTypeContext == BroadcastingTypeContext.Brific)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            }
            if (broadcastingTypeContext == BroadcastingTypeContext.Icsm)
            {
                broadcastingContextBase = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;
            }

            if (((ValidationAssignment(broadcastingContextBase.Assignments)) && (ValidationAllotment(broadcastingContextBase.Allotments))) == false)
            {
                throw new Exception("Input parameters failed validation");
            }

            var lstContoursResults = new List<ContoursResult>();

            var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
            {
                BroadcastingAllotment = broadcastingContextBase.Allotments,
                BroadcastingAssignments = broadcastingContextBase.Assignments
            };
            //1.Определение центра гравитации(2.1)
            var pointEarthGeometric = new PointEarthGeometric();
            this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometric);

            for (int i = 0; i < ge06CalcData.Ge06TaskParameters.FieldStrength.Length; i++)
            {
               
                // 2. Построение контуров фиксированной дистанции относительно центра гравитации.
                // Базируемся на функции CreateContourFromPointByDistance если у нас только BroadcastingAssignment []


                var areaPoints = broadcastingContextBase.Allotments.AllotmentParameters.Сontur;
                if ((areaPoints != null) && (areaPoints.Length > 0))
                {


                    var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                    {
                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                        TriggerFieldStrength = ge06CalcData.Ge06TaskParameters.FieldStrength[i],
                        PointEarthGeometricCalc = pointEarthGeometric
                    };

                    
                    try
                    {
                        pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                        this._earthGeometricService.CreateContourForStationByTriggerFieldStrengths((sourcePoint, destPoint) => CalcFieldStrength(sourcePoint, destPoint), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                        ///???????????????????????? как формировать ContoursResult ????????????????????????
                        var contoursResult = new ContoursResult[sizeResultBuffer];

                        lstContoursResults.AddRange(contoursResult);

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
            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            FillAllotmentOrAssignmentResult(broadcastingContextBase, ref ge06CalcResult);
        }

        /// <summary>
        /// Расчет  для  CalculationType == ConformityCheck
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        private void CalculationForConformityCheck(
                                          in Ge06CalcData ge06CalcData,
                                          ref Ge06CalcResult ge06CalcResult
                                          )
        {
            var pointEarthGeometricsResult = default(PointEarthGeometric[]);

            var lstContoursResults = new List<ContoursResult>();

            if (((ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM!=null) && (ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC!= null))==false)
            {
                throw new Exception("Incomplete ICSM data or BRIFIC");
            }

            //0. Валидация входных данных. аналогично п.0 4.1. + обязательные наличие хотя по одному объекту для ICSM и BRIFIC
            if (((ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments)) && (ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments))) == false)
            {
                throw new Exception("Input parameters for ICSM failed validation");
            }

            if (((ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Assignments)) && (ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Allotments))) == false)
            {
                throw new Exception("Input parameters for BRIFIC failed validation");
            }


            var broadcastingContextBRIFIC = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;

            //1.Определение центра гравитации(2.1)
            var pointEarthGeometric = new PointEarthGeometric();

            if ((broadcastingContextBRIFIC != null) && (broadcastingContextICSM != null))
            {
                //1. Определение центра гравитации, но только для BR IFIC (2.1)
                var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
                {
                    BroadcastingAllotment = broadcastingContextBRIFIC.Allotments,
                    BroadcastingAssignments = broadcastingContextBRIFIC.Assignments
                };

                //1.Определение центра гравитации(2.1)
                this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometric);


                // 2.Определение контрольных точек для записей BR IFIC -> построение контуров для выделений для
                //     60, 100, 200, 300, 500, 750 и 1000 км.CreateContourFromContureByDistance(если присутствует выделение) 1.1.6 или CreateContourFromPointByDistance если его нет.
                var distances = new int[7] { 60, 100, 200, 300, 500, 750, 1000 };
                for (int i = 0; i < distances.Length; i++)
                {

                    if (broadcastingContextBRIFIC.Allotments == null)
                    {
                        var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                        {
                            Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                            Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                            PointEarthGeometricCalc = pointEarthGeometric
                        };


                        try
                        {
                            pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                            this._earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                            for (int k = 0; k < sizeResultBuffer; k++)
                            {
                                var pointForCalcFS = new Point()
                                {
                                    Longitude = pointEarthGeometricsResult[k].Longitude,
                                    Latitude = pointEarthGeometricsResult[k].Latitude
                                };

                                //3.Расчет напряженности поля в точках(2.2) для BR IFIC(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                var fsBRIFIC = CalcFieldStrengthInPointGE06(in ge06CalcData, in pointForCalcFS, broadcastingContextBRIFIC);

                                //4.Расчет напряженности поля в точках(2.2) для ICSM (Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                var fsICSM = CalcFieldStrengthInPointGE06(in ge06CalcData, in pointForCalcFS, broadcastingContextICSM);

                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                ///
                                ///     5. Определение напряженности поля для затронутой службы (таблица ТАБЛИЦА A.1.1 документа GE06 взять http://redmine3.lissoft.com.ua:3003/issues/697) в качестве определяющих данных это технология Broadcasting,  частота и служба stn_cls (п. Системы, испытующие влияние).
                                ///     6.Построение контура(ов) для напряженности поля относительно центра гравитации для BRIFIC. Базируемся на функции CreateContourForStationByTriggerFieldStrengths. При этом в данный расчет мы должны просунуть функцию 2.2 Расчет напряженности поля в точке.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                ///     7 Расчет напряженности поля в каждой точке, полученной для контура в п 6: для BroadcastingAssignment[] +BroadcastingAllotment(ICSM) используя функцию 2.2.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                ///
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                        var areaPoints = broadcastingContextBRIFIC.Allotments.AllotmentParameters.Сontur;
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
                                PointBaryCenter = pointEarthGeometric,
                                ContourPoints = pointEarthGeometrics
                            };



                            try
                            {
                                pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                                this._earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                                for (int k = 0; k < sizeResultBuffer; k++)
                                {
                                    var pointForCalcFS = new Point()
                                    {
                                        Longitude = pointEarthGeometricsResult[k].Longitude,
                                        Latitude = pointEarthGeometricsResult[k].Latitude
                                    };

                                    //3.Расчет напряженности поля в точках(2.2) для BR IFIC(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                    var fsBRIFIC = CalcFieldStrengthInPointGE06(in ge06CalcData, in pointForCalcFS, broadcastingContextBRIFIC);

                                    //4.Расчет напряженности поля в точках(2.2) для ICSM (Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                    var fsICSM = CalcFieldStrengthInPointGE06(in ge06CalcData, in pointForCalcFS, broadcastingContextICSM);

                                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    ///
                                    ///     5. Определение напряженности поля для затронутой службы (таблица ТАБЛИЦА A.1.1 документа GE06 взять http://redmine3.lissoft.com.ua:3003/issues/697) в качестве определяющих данных это технология Broadcasting,  частота и служба stn_cls (п. Системы, испытующие влияние).
                                    ///     6.Построение контура(ов) для напряженности поля относительно центра гравитации для BRIFIC. Базируемся на функции CreateContourForStationByTriggerFieldStrengths. При этом в данный расчет мы должны просунуть функцию 2.2 Расчет напряженности поля в точке.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                    ///     7 Расчет напряженности поля в каждой точке, полученной для контура в п 6: для BroadcastingAssignment[] +BroadcastingAllotment(ICSM) используя функцию 2.2.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                    ///
                                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            FillAllotmentOrAssignmentResult(broadcastingContextBRIFIC, ref ge06CalcResult);
            FillAllotmentOrAssignmentResult(broadcastingContextICSM, ref ge06CalcResult);
        }


        /// <summary>
        /// Расчет  для  CalculationType == FindAffectedADM
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        private void CalculationForFindAffectedADM(
                                          in Ge06CalcData ge06CalcData,
                                           ref Ge06CalcResult ge06CalcResult
                                          )
        {

            if (ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM==null)
            {
                throw new Exception("Input parameters BroadcastingContextICSM is null!");
            }
            if (((ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments)) && (ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments))) == false)
            {
                throw new Exception("Input parameters failed validation");
            }

            var pointEarthGeometricsResult = default(PointEarthGeometric[]);

            var lstContoursResults = new List<ContoursResult>();

            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;

            var pointEarthGeometric = new PointEarthGeometric();

            //1.Определение центра гравитации(2.1)
            if ((broadcastingContextICSM != null) && (broadcastingContextICSM != null))
            {
                var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
                {
                    BroadcastingAllotment = broadcastingContextICSM.Allotments,
                    BroadcastingAssignments = broadcastingContextICSM.Assignments
                };
                this._gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometric);
            }


            if (broadcastingContextICSM.Allotments == null)
            {
                var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                {
                    Distance_km = 1000,
                    Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                    PointEarthGeometricCalc = pointEarthGeometric
                };

                try
                {
                    pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                    this._earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                    // Вычисление администраций в контуре 1.2.3
                    // Результат: контур и список потенциально затронутых администраций
                    var admAffected = new List<string>();

                    for (int k = 0; k < sizeResultBuffer; k++)
                    {
                        var pointForCalc= new IdwmDataModel.Point()
                        {
                            Longitude_dec = pointEarthGeometricsResult[k].Longitude,
                            Latitude_dec = pointEarthGeometricsResult[k].Latitude
                        };

                        admAffected.Add(this._idwmService.GetADMByPoint(in pointForCalc));
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
                var areaPoints = broadcastingContextICSM.Allotments.AllotmentParameters.Сontur;
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
                        PointBaryCenter = pointEarthGeometric,
                        ContourPoints = pointEarthGeometrics
                    };



                    try
                    {
                        pointEarthGeometricsResult = _pointEarthGeometricPool.Take();

                        this._earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                        // Вычисление администраций в контуре 1.2.3
                        // Результат: контур и список потенциально затронутых администраций
                        var admAffected = new List<string>();

                        for (int k = 0; k < sizeResultBuffer; k++)
                        {
                            var pointForCalc = new IdwmDataModel.Point()
                            {
                                Longitude_dec = pointEarthGeometricsResult[k].Longitude,
                                Latitude_dec = pointEarthGeometricsResult[k].Latitude
                            };

                            admAffected.Add(this._idwmService.GetADMByPoint(in pointForCalc));
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





            ge06CalcResult.ContoursResult = lstContoursResults.ToArray();
            FillAllotmentOrAssignmentResult(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM, ref ge06CalcResult);
        }

        /// <summary>
        /// Здесь тоже не понятно как обойтись всего двумя параметрами ??????????????
        /// </summary>
        /// <param name="pointEarthGeometric1"></param>
        /// <param name="pointEarthGeometric2"></param>
        /// <returns></returns>
        public static double CalcFieldStrength(PointEarthGeometric pointEarthGeometric1, PointEarthGeometric pointEarthGeometric2)
        {
            return -1;
        }

        /// <summary>
        /// Установка модели распространения и ее параметров 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private PropagationModel GetPropagationModel(Ge06TaskParameters ge06TaskParameters, PropagationModel data, CalculationType calculationType)
        {
            var propagationModel = new PropagationModel();
            if ((calculationType == CalculationType.CreateContoursByDistance)
                || (calculationType == CalculationType.CreateContoursByFS))
            {
                //?????????????
            }
            else if ((calculationType == CalculationType.ConformityCheck)
               || (calculationType == CalculationType.FindAffectedADM))
            {
                //?????????????
            }
            return propagationModel;
        }

        /// <summary>
        /// Расчет напряженности поля в точке 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private float CalcFieldStrengthInPointGE06(in Ge06CalcData ge06CalcData, in Point point, BroadcastingContextBase broadcastingContext)
        {
            //Ge06TaskParameters ge06TaskParameters, PropagationModel propagationModel, BroadcastingContextBase broadcastingContextBase
            float resultFieldStrengthCalcResultAllotment = 0;
            var lstFieldStrengthAssignments = new List<float>();
            //1.Необходимо проверить установлена ли необходимая модель распространения и ее параметры являются ли корректными для данного рода расчета(2.2.3). 
            var propagModel = GetPropagationModel(ge06CalcData.Ge06TaskParameters, ge06CalcData.PropagationModel, (CalculationType)ge06CalcData.Ge06TaskParameters.CalculationTypeCode);
            //2.Далее если есть выделение, то необходимо рассчитать напряженность поля от выделения(2.2.1).
            if (broadcastingContext.Allotments != null)
            {
                 resultFieldStrengthCalcResultAllotment = CalcFieldStrengthInPointFromAllotmentGE06(broadcastingContext.Allotments, ge06CalcData.PropagationModel, point);
            }
            //3.Если есть BroadcastingAssignment, то рассчитать напряженность поля от каждой из них(2.2.4).При этом если станций несколько, то определяем суммарную напряженность поля для станций с одинаковым SFN_id методом суммирования мощностей(2.2.2).
            if (broadcastingContext.Assignments != null)
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
                            var sumFieldStrengthInPointFromAssignmentGE06 = new double?[fndAssignments.Count];
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
            var lstAllFieldStrengthAssignments = new List<double?>();
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
                var pointWithAzimuthResult = new PointWithAzimuthResult();
                this._gn06Service.EstimationAssignmentsPointsForEtalonNetwork(in estimationAssignmentsPointsArgs, ref pointWithAzimuthResult);

                var lstBroadcastingAssignment = new List<BroadcastingAssignment>();
                var pointWithAzimuth = pointWithAzimuthResult.PointWithAzimuth;
                for (int k = 0; k < pointWithAzimuth.Length; k++)
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
                    lstBroadcastingAssignment.Add(broadcastingAssignmentTemp);
                }
                var lstFieldStrengthAssignments = new List<double?>();
                for (int k = 0; k < lstBroadcastingAssignment.Count; k++)
                {
                    var broadcastAssignment = lstBroadcastingAssignment[k];
                    //в) Расчет напряженности поля от каждого эталонного BroadcastingAssignment(2.2.4).Перед расчетом производиться корректировка паттерна BroadcastingAssignment в соответствии с его ориентацией(суть корректировки спросить Максима или Юру).
                    var resultFieldStrengthInPointFromAssignmentGE06 = CalcFieldStrengthInPointFromAssignmentGE06(broadcastAssignment, propagationModel, point);
                    lstFieldStrengthAssignments.Add(resultFieldStrengthInPointFromAssignmentGE06);
                }

                lstAllFieldStrengthAssignments.Add(SumPowGE06(lstFieldStrengthAssignments.ToArray()));
            }

            //г) Определение суммарной напряженности поля(2.2.2) .
            var maxFieldStrength = lstAllFieldStrengthAssignments.Max();

            return (float)maxFieldStrength.Value;
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

            if (propagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546)
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
                    //TargetCoordinate = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate() { Longitude = point.Longitude, Latitude = point.Latitude }, this._ge06CalcData.Projection)
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
        private float SumPowGE06(double?[] fs)
        {
            double resFS = 0.0;
            for (int j = 0; j < fs.Length; j++)
            {
                if (fs[j] != null)
                {
                    resFS += Math.Pow(10, fs[j].Value / 10.0);
                }
            }
            return (float)(10 * Math.Log10(resFS));
        }

    }
}
