using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Idwm = Atdi.Contracts.Sdrn.DeepServices.IDWM;
using IdwmDataModel = Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Common;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class GE06CalcFindAffectedADM
    {

        /// <summary>
        /// Поиск для администраций, которые попали в контер 1000 км всех затронутых служб и соотвествующих им пороговых значений напряженности поля
        /// </summary>
        /// <param name="thresholdFieldStrengthsAnotherServices">Входной набор пороговых напряженностей поля</param>
        /// <param name="outArrFmtvTerra">Выходной массив сведений о затронутых службах из БД BRIFIC</param>
        /// <param name="pointEarthGeometricBarycenter">Точка барицентра</param>
        /// <param name="idwmService">сервис idwm</param>
        /// <returns></returns>
        public static ThresholdFieldStrength[] ClarifyAffectedServicesFromBrific(List<ThresholdFieldStrength> thresholdFieldStrengthsAnotherServices, out FmtvTerra[] outArrFmtvTerra, PointEarthGeometric pointEarthGeometricBarycenter, Idwm.IIdwmService idwmService)
        {
            var pointCalc = new IdwmDataModel.PointAndDistance()
            {
                Distance = 1000,
                Point = new IdwmDataModel.Point()
                {
                    Longitude_dec = pointEarthGeometricBarycenter.Longitude,
                    Latitude_dec = pointEarthGeometricBarycenter.Latitude,
                }
            };

            // создаем массив для хранения сведений о всех администрациях, которые попадают в радиус 1000 км от точки pointEarthGeometricBarycenter
            var allFindAdministrations = new IdwmDataModel.AdministrationsResult[1000];
            idwmService.GetADMByPointAndDistance(in pointCalc, ref allFindAdministrations, out int SizeBufferFindAdministrations);
            var selectedDataFromBrific = new List<FmtvTerra>();
            for (int j = 0; j < thresholdFieldStrengthsAnotherServices.Count; j++)
            {
                var triggerInformationTemp = thresholdFieldStrengthsAnotherServices[j];
                var minFreq_MHz = triggerInformationTemp.MinFreq_MHz;
                var maxFreq_MHz = triggerInformationTemp.MaxFreq_MHz;
                var staClass = triggerInformationTemp.StaClass;
                var systemType = triggerInformationTemp.System_type;

                for (int n = 0; n < SizeBufferFindAdministrations; n++)
                {
                    var admTemp = allFindAdministrations[n].Administration;
                    switch (systemType)
                    {
                        case "NV":
                            FmtvTerra[] brificNV = LoadDataBrific.LoadBroadcastingService_NV(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificNV != null) && (brificNV.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificNV);
                            }
                            break;
                        case "NR":
                            var brificNR = LoadDataBrific.LoadBroadcastingService_NR(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificNR != null) && (brificNR.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificNR);
                            }
                            break;
                        case "NS":
                            var brificNS = LoadDataBrific.LoadBroadcastingService_NS(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificNS != null) && (brificNS.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificNS);
                            }
                            break;
                        case "NT":
                            var brificNT = LoadDataBrific.LoadBroadcastingService_NT(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificNT != null) && (brificNT.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificNT);
                            }
                            break;
                        case "NA":
                            var brificNA = LoadDataBrific.LoadBroadcastingService_NA(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificNA != null) && (brificNA.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificNA);
                            }
                            break;
                        case "NB":
                            var brificNB = LoadDataBrific.LoadBroadcastingService_NB(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificNB != null) && (brificNB.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificNB);
                            }
                            break;
                        case "XN":
                            var brificXN = LoadDataBrific.LoadBroadcastingService_XN(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificXN != null) && (brificXN.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificXN);
                            }
                            break;
                        case "YN":
                            var brificYN = LoadDataBrific.LoadBroadcastingService_XN(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificYN != null) && (brificYN.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificYN);
                            }
                            break;
                        case "ZC":
                            var brificZC = LoadDataBrific.LoadBroadcastingService_ZC(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificZC != null) && (brificZC.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificZC);
                            }
                            break;
                        case "XG":
                            var brificXG = LoadDataBrific.LoadBroadcastingService_ZC(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificXG != null) && (brificXG.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificXG);
                            }
                            break;
                        case "AB":
                            var brificAB = LoadDataBrific.LoadNavigationServices_AB(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificAB != null) && (brificAB.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificAB);
                            }
                            break;
                        case "AA8":
                            var brificAA8 = LoadDataBrific.LoadNavigationServices_AA8(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificAA8 != null) && (brificAA8.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificAA8);
                            }
                            break;
                        case "BD":
                            var brificBD = LoadDataBrific.LoadNavigationServices_BD(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificBD != null) && (brificBD.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificBD);
                            }
                            break;
                        case "BA":
                            var brificBA = LoadDataBrific.LoadNavigationServices_BA(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificBA != null) && (brificBA.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificBA);
                            }
                            break;
                        case "FF":
                            var brificFF = LoadDataBrific.LoadFixedServices_FF(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificFF != null) && (brificFF.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificFF);
                            }
                            break;
                        case "FN":
                            var brificFN = LoadDataBrific.LoadFixedServices_FN(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificFN != null) && (brificFN.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificFN);
                            }
                            break;
                        case "FK":
                            var brificFK = LoadDataBrific.LoadFixedServices_FK(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificFK != null) && (brificFK.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificFK);
                            }
                            break;
                        case "MU":
                            var brificMU = LoadDataBrific.LoadMobileServices_MU(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificMU != null) && (brificMU.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificMU);
                            }
                            break;
                        case "M1":
                            var brificM1 = LoadDataBrific.LoadMobileServices_M1(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificM1 != null) && (brificM1.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificM1);
                            }
                            break;
                        case "RA":
                            var brific_RA = LoadDataBrific.LoadMobileServices_RA(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brific_RA != null) && (brific_RA.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brific_RA);
                            }
                            break;
                        case "M2":
                            var brificM2 = LoadDataBrific.LoadMobileServices_M2(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificM2 != null) && (brificM2.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificM2);
                            }
                            break;
                        case "XA":
                            var brificXA = LoadDataBrific.LoadMobileServices_XA(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificXA != null) && (brificXA.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificXA);
                            }
                            break;
                        case "XM":
                            var brificXM = LoadDataBrific.LoadMobileServices_XM(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificXM != null) && (brificXM.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificXM);
                            }
                            break;
                        case "MA":
                            var brificMA = LoadDataBrific.LoadMobileServices_MA(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificMA != null) && (brificMA.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificMA);
                            }
                            break;
                        case "MT":
                            var brificMT = LoadDataBrific.LoadMobileServices_MT(admTemp, minFreq_MHz, maxFreq_MHz, staClass);
                            if ((brificMT != null) && (brificMT.Length > 0))
                            {
                                selectedDataFromBrific.AddRange(brificMT);
                            }
                            break;
                    }
                }
            }
            outArrFmtvTerra = selectedDataFromBrific.ToArray();
            return ThresholdFS.GetThresholdFieldStrengthByFmtvTerra(outArrFmtvTerra, TypeThresholdFS.All);
        }
        /// <summary>
        /// Результат по контурам
        /// </summary>
        /// <param name="countoursPointExtendeds"></param>
        /// <param name="sizeCountoursPointExtendedBuffer"></param>
        /// <returns></returns>
        public static ContoursResult[] GenerateAdministrationContour(CountoursPointExtended[] countoursPointExtendeds, int sizeCountoursPointExtendedBuffer)
        {
            return null;
        }
        /// <summary>
        /// Результат по администрациям
        /// </summary>
        /// <param name="countoursPointExtendeds"></param>
        /// <param name="sizeCountoursPointExtendedBuffer"></param>
        /// <param name="arrFmtvTerra"></param>
        /// <returns></returns>
        public static AffectedADMResult[] GenerateAdministration(CountoursPointExtended[] countoursPointExtendeds, int sizeCountoursPointExtendedBuffer, FmtvTerra[] arrFmtvTerra)
        {
            return null;
        }


        /// <summary>
        /// Расчет  для  CalculationType == FindAffectedADM
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        public static void Calculation(
                                          Ge06CalcData ge06CalcData,
                                          ref Ge06CalcResult ge06CalcResult,
                                          IObjectPool<PointEarthGeometric[]> pointEarthGeometricPool,
                                          IObjectPool<CountoursPointExtended[]> countoursPointExtendedPool,
                                          IObjectPool<ContoursResult[]> contoursResultPool,
                                          IIterationHandler<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult> iterationHandlerBroadcastingFieldStrengthCalcData,
                                          IIterationHandler<FieldStrengthCalcData, FieldStrengthCalcResult> iterationHandlerFieldStrengthCalcData,
                                          IObjectPoolSite poolSite,
                                          ITransformation transformation,
                                          ITaskContext taskContext,
                                          IGn06Service gn06Service,
                                          IEarthGeometricService earthGeometricService,
                                          Idwm.IIdwmService idwmService
                                          )
        {




            if ((ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM == null)||
                (((ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments==null)||(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments.AllotmentParameters.Contur == null)|| (ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments.AllotmentParameters.Contur.Length <3)) &&
                (ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments == null)||((ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments.Length == 0))))
            {
                taskContext.SendEvent(new CalcResultEvent
                {
                    Level = CalcResultEventLevel.Error,
                    Context = "Ge06CalcIteration",
                    Message = "Input parameters BroadcastingContextICSM is null!"
                });
                throw new Exception("Input parameters BroadcastingContextICSM is null!");
            }

            string notValidBroadcastingAssignment = string.Empty;
            string notValidBroadcastingAllotment = string.Empty;
            if (((GE06Validation.ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments, out notValidBroadcastingAssignment)) && (GE06Validation.ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments, out notValidBroadcastingAllotment))) == false)
            {
                string message = "";
                if (!string.IsNullOrEmpty(notValidBroadcastingAssignment))
                {
                    message += $"The following Assignments are not validated: {notValidBroadcastingAssignment}";
                }
                if (!string.IsNullOrEmpty(notValidBroadcastingAllotment))
                {
                    message += $"The following Alotment are not validated: {notValidBroadcastingAllotment}";
                }
                taskContext.SendEvent(new CalcResultEvent
                {
                    Level = CalcResultEventLevel.Error,
                    Context = "Ge06CalcIteration",
                    Message = message
                });
                throw new Exception(message);
            }

            
            float ProsentTerr = 50;
            int Height = 10;
            var countoursPointExtendedBuffer = default(CountoursPointExtended[]);
            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultICSM = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultAffectedICSM = default(PointEarthGeometric[]);
            var lstContoursResults = new List<ContoursResult>();
            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;
            // Определение класов входящих данных 
            if (broadcastingContextICSM.Allotments != null)
            {
                if ((broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC1)
                       || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC2)
                           || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC3))
                {
                    broadcastingContextICSM.Allotments.AdminData.StnClass = "BT";
                    broadcastingContextICSM.Allotments.AdminData.IsDigital = true;
                }
                else if ((broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC4)
                    || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC5)
                        )
                {
                    broadcastingContextICSM.Allotments.AdminData.StnClass = "BC";
                    broadcastingContextICSM.Allotments.AdminData.IsDigital = false;
                }
            }
            // Конец определения класов входящих данных.


            // Определение пороговых напряженностей для защиты всех служб в том числе радиовещательной службы
            // входными данными являются Allotments + Assignments[]
            var thresholdFieldStrengthsAnotherServices = new List<ThresholdFieldStrength>();
            var thresholdFieldStrengthsICSMAllotmentsAll = ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextICSM.Allotments, TypeThresholdFS.All);
            var thresholdFieldStrengthsICSMAssignmentsAll = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextICSM.Assignments, TypeThresholdFS.All);

            for (int v = 0; v < thresholdFieldStrengthsICSMAllotmentsAll.Length; v++)
            {
                if (thresholdFieldStrengthsAnotherServices.Find(x => x.Freq_MHz == thresholdFieldStrengthsICSMAllotmentsAll[v].Freq_MHz && x.StaClass == thresholdFieldStrengthsICSMAllotmentsAll[v].StaClass && x.IsDigital == thresholdFieldStrengthsICSMAllotmentsAll[v].IsDigital) == null)
                {
                    thresholdFieldStrengthsAnotherServices.Add(thresholdFieldStrengthsICSMAllotmentsAll[v]);
                }
            }
            for (int v = 0; v < thresholdFieldStrengthsICSMAssignmentsAll.Length; v++)
            {
                if (thresholdFieldStrengthsAnotherServices.Find(x => x.Freq_MHz == thresholdFieldStrengthsICSMAssignmentsAll[v].Freq_MHz && x.StaClass == thresholdFieldStrengthsICSMAssignmentsAll[v].StaClass && x.IsDigital == thresholdFieldStrengthsICSMAssignmentsAll[v].IsDigital) == null)
                {
                    thresholdFieldStrengthsAnotherServices.Add(thresholdFieldStrengthsICSMAssignmentsAll[v]);
                }
            }


            var dicCountoursPointsByICSM = new Dictionary<CountoursPoint, string>();
            var listAffectedADMResults = new List<AffectedADMResult>();
            ///список потенциально затронутых администраций (TypeAffected = 1000).
            var admPotentiallyAffected_1000 = new List<string>();
            // список затронутых (радиовещательные службы затронуты) администраций. TypeAffected = Broadcast
            var admPotentiallyAffected_Broadcast = new List<string>();

            var pointEarthGeometricBarycenter = new PointEarthGeometric();
            //1.А Определение центра гравитации(2.1)
            var broadcastingCalcBarycenterGE06 = new BroadcastingCalcBarycenterGE06()
            { BroadcastingAllotment = broadcastingContextICSM.Allotments,
                BroadcastingAssignments = broadcastingContextICSM.Assignments};
            gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);


            var thresholdFieldStrengths = ClarifyAffectedServicesFromBrific(thresholdFieldStrengthsAnotherServices, out FmtvTerra[] outArrFmtvTerra, pointEarthGeometricBarycenter, idwmService);

            try
            {
                // Получение элементов из пула
                pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                pointEarthGeometricsResultICSM = pointEarthGeometricPool.Take();
                pointEarthGeometricsResultAffectedICSM = pointEarthGeometricPool.Take();
                countoursPointExtendedBuffer = countoursPointExtendedPool.Take();

                //1.Б Построение контура 
                int sizeResultBuffer = 0;
                if ((broadcastingContextICSM.Allotments != null) && (broadcastingContextICSM.Allotments.AllotmentParameters.Contur != null) && (broadcastingContextICSM.Allotments.AllotmentParameters.Contur.Length > 3))
                {
                    var areaPoints = broadcastingContextICSM.Allotments.AllotmentParameters.Contur;
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
                    earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                }
                else
                {
                    var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                    {
                        Distance_km = 1000,
                        Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                        PointEarthGeometricCalc = pointEarthGeometricBarycenter
                    };
                    earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                }
                // 1.B Вычисление администраций в контуре 
                var countoursPoint1000 = new CountoursPoint[sizeResultBuffer];
                for (int k = 0; k < sizeResultBuffer; k++)
                {
                    var pointForCalc = new IdwmDataModel.Point()
                    {
                        Longitude_dec = pointEarthGeometricsResult[k].Longitude,
                        Latitude_dec = pointEarthGeometricsResult[k].Latitude
                    };
                    var adm = idwmService.GetADMByPoint(pointForCalc);
                    countoursPoint1000[k] = new CountoursPoint();
                    countoursPoint1000[k].Lon_DEC = pointForCalc.Longitude_dec.Value;
                    countoursPoint1000[k].Lat_DEC = pointForCalc.Latitude_dec.Value;
                    countoursPoint1000[k].PointType = PointType.Affected;
                    if (!admPotentiallyAffected_1000.Contains(adm)){admPotentiallyAffected_1000.Add(adm);}
                }

                // 3. Определение затронутых других первичных служб 

                //var thresholdFieldStrengths = ClarifyAffectedServicesFromBrific(thresholdFieldStrengthsAnotherServices, out FmtvTerra[] outArrFmtvTerra, pointEarthGeometricBarycenter, idwmService);
                //var thresholdFieldStrengths = thresholdFieldStrengthsAnotherServices; //на самом деле тут найдем все затронутые сервисы



                // 2. Определение администраций с затронутой радиовещательной службой 4. и прочими службами
                if (thresholdFieldStrengths != null)
                {
                    int[] arrFieldStrength = null;
                    var arrTriggersFS = thresholdFieldStrengths.ToArray();
                    arrFieldStrength = new int[arrTriggersFS.Length];
                    int indexForCountoursPointExtendedBuffer = 0;
                    int indexResult = 0;
                    // идем по значениям напряженности поля

                    for (int d = 0; d < arrTriggersFS.Length; d++)
                    {
                        // Установка соответсвующей модели распространения 
                        var propModel = ge06CalcData.PropagationModel;
                        GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, ProsentTerr, arrTriggersFS[d].Time_pc);
                        ge06CalcData.PropagationModel = propModel;
                        ge06CalcData.Ge06TaskParameters.SubscribersHeight = (int)arrTriggersFS[d].Height_m;

                    
                        // Построение контура для напряженности поля относительно центра гравитации для ICSM
                        var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                        {
                            Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                            TriggerFieldStrength = arrTriggersFS[d].ThresholdFS,
                            BaryCenter = pointEarthGeometricBarycenter
                        };
                        earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthICSM(destinationPoint,
                            ge06CalcData, pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service),
                            in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResult, out int sizeResultBufferICSM);
                        // Конец построения контура для напряженности поля относительно центра гравитации для ICSM. 
                        // запись результатов по контуру
                        if (sizeResultBufferICSM > 0)
                        {
                            for (int t = 0; t < sizeResultBufferICSM; t++)
                            {
                                // предварительное сохранение результатов
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer] = new CountoursPointExtended();
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lon_DEC = pointEarthGeometricsResult[t].Longitude;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lat_DEC = pointEarthGeometricsResult[t].Latitude;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = (int)arrTriggersFS[d].Height_m;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = (int)arrTriggersFS[d].ThresholdFS;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Affected;
                                var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point() { Longitude_dec = pointEarthGeometricsResult[t].Longitude, Latitude_dec = pointEarthGeometricsResult[t].Latitude });
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.FieldStrength;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Id = indexResult;
                                countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Service = arrTriggersFS[d].StaClass;
                                indexForCountoursPointExtendedBuffer++;
                            }
                            indexResult++;
                        }
                    }


                    // пример вызова метода проаерки попадания точки в контур (входными данными являются переменная типа CheckHittingArgs)
                    //earthGeometricService.CheckHitting(in CheckHittingArgs);

                    // функция по формированию контуров
                    ge06CalcResult.ContoursResult = GenerateAdministrationContour(countoursPointExtendedBuffer, indexForCountoursPointExtendedBuffer);

                    // функция по формированию администраций
                    ge06CalcResult.AffectedADMResult = GenerateAdministration(countoursPointExtendedBuffer, indexForCountoursPointExtendedBuffer, outArrFmtvTerra);

                }
            }
            finally
            {
                if (pointEarthGeometricsResult != null)
                {
                    pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                }
                if (pointEarthGeometricsResultICSM != null)
                {
                    pointEarthGeometricPool.Put(pointEarthGeometricsResultICSM);
                }
                if (pointEarthGeometricsResultAffectedICSM != null)
                {
                    pointEarthGeometricPool.Put(pointEarthGeometricsResultAffectedICSM);
                }
                if (countoursPointExtendedBuffer != null)
                {
                    countoursPointExtendedPool.Put(countoursPointExtendedBuffer);
                }
                
            }
            // тут конец расчетам нужна обработка результатов 
            GE06FillData.FillAllotmentOrAssignmentResult(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM, ref ge06CalcResult);
        }
    }
}
