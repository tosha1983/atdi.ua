using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Idwm = Atdi.Contracts.Sdrn.DeepServices.IDWM;
using IdwmDataModel = Atdi.DataModels.Sdrn.DeepServices.IDWM;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class GE06CalcConformityCheck
    {
        /// <summary>
        /// Расчет  для  CalculationType == ConformityCheck
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="ge06CalcData"></param>
        /// <param name="broadcastingTypeContext"></param>
        /// <param name="ge06CalcResult"></param>
        public static void Calculation(
                                          Ge06CalcData ge06CalcData,
                                          ref Ge06CalcResult ge06CalcResult,
                                          IObjectPool<PointEarthGeometric[]> pointEarthGeometricPool,
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
            var affectedServices = new List<string>();

            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultBRIFIC = default(PointEarthGeometric[]);

            var dicCountoursPointsByBRIFIC = new Dictionary<CountoursPoint, string>();
            var dicCountoursPointsByICSM = new Dictionary<CountoursPoint, string>();

            var lstContoursResultsByICSM = new List<ContoursResult>();
            var lstContoursResultsByBRIFIC = new List<ContoursResult>();
            var lstContoursResults = new List<ContoursResult>();

            if (((ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM != null) && (ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC != null)) == false)
            {
                throw new Exception("Incomplete ICSM data or BRIFIC");
            }

            //0. Валидация входных данных. аналогично п.0 4.1. + обязательные наличие хотя по одному объекту для ICSM и BRIFIC
            string notValidBroadcastingAssignmentICSM = string.Empty;
            string notValidBroadcastingAllotmentsICSM = string.Empty;
            string notValidBroadcastingAssignmentBRIFIC = string.Empty;
            string notValidBroadcastingAllotmentsBRIFIC = string.Empty;
            string message = string.Empty;
            if (((GE06Validation.ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments, out notValidBroadcastingAssignmentICSM)) && (GE06Validation.ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Allotments, out  notValidBroadcastingAllotmentsICSM))) == false)
            {
                if (!string.IsNullOrEmpty(notValidBroadcastingAssignmentICSM))
                {
                    message += $"The following Assignments for ICSM are not validated: {notValidBroadcastingAssignmentICSM}";
                }
                if (!string.IsNullOrEmpty(notValidBroadcastingAllotmentsICSM))
                {
                    message += $"The following Allotment for ICSM are not validated: {notValidBroadcastingAllotmentsICSM}";
                }
            }
            if (((GE06Validation.ValidationAssignment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Assignments, out notValidBroadcastingAssignmentBRIFIC)) && (GE06Validation.ValidationAllotment(ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Allotments, out  notValidBroadcastingAllotmentsBRIFIC))) == false)
            {
                if (!string.IsNullOrEmpty(notValidBroadcastingAssignmentICSM))
                {
                    message += $"The following Assignments for BRIFIC are not validated: {notValidBroadcastingAssignmentBRIFIC}";
                }
                if (!string.IsNullOrEmpty(notValidBroadcastingAllotmentsICSM))
                {
                    message += $"The following Allotment for BRIFIC are not validated: {notValidBroadcastingAllotmentsBRIFIC}";
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                throw new Exception(message);
            }


            var broadcastingContextBRIFIC = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;



            // поиск сведений о попроговых значениях напряженности поля 
            var thresholdFieldStrengths = new List<ThresholdFieldStrength>();

            var thresholdFieldStrengthsBRIFICAllotments = ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextBRIFIC.Allotments, TypeThresholdFS.OnlyBroadcastingService);
            var thresholdFieldStrengthsBRIFICAssignments = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextBRIFIC.Assignments, TypeThresholdFS.OnlyBroadcastingService);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsBRIFICAllotments);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsBRIFICAssignments);





            var thresholdFieldStrengthsICSMAllotments = ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextICSM.Allotments, TypeThresholdFS.OnlyBroadcastingService);
            var thresholdFieldStrengthsICSMAssignments = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextICSM.Assignments, TypeThresholdFS.OnlyBroadcastingService);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsICSMAllotments);
            thresholdFieldStrengths.AddRange(thresholdFieldStrengthsICSMAssignments);




            //1.Определение центра гравитации(2.1)
            var pointEarthGeometricBarycenter = new PointEarthGeometric();

            if ((broadcastingContextBRIFIC != null) && (broadcastingContextICSM != null))
            {

                ///список затронутых служб для брифика
                if (broadcastingContextBRIFIC.Allotments != null)
                {
                    if (broadcastingContextBRIFIC.Allotments.EmissionCharacteristics != null)
                    {
                        if ((broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC1)
                            || (broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC2)
                                || (broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC3))
                        {
                            affectedServices.Add("BT");
                        }
                        else if ((broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC4)
                            || (broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC5)
                                )
                        {
                            affectedServices.Add("BC");
                        }
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
                gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);


                // 2.Определение контрольных точек для записей BR IFIC -> построение контуров для выделений для
                //     60, 100, 200, 300, 500, 750 и 1000 км.CreateContourFromContureByDistance(если присутствует выделение) 1.1.6 или CreateContourFromPointByDistance если его нет.
                var distances = new int[7] { 60, 100, 200, 300, 500, 750, 1000 };
                for (int i = 0; i < distances.Length; i++)
                {

                    if (broadcastingContextBRIFIC.Allotments == null)
                    {
                        try
                        {
                            pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                            pointEarthGeometricsResultBRIFIC = pointEarthGeometricPool.Take();

                            var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                            {
                                Distance_km = ge06CalcData.Ge06TaskParameters.Distances[i],
                                Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                PointEarthGeometricCalc = pointEarthGeometricBarycenter
                            };

                            earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                            for (int k = 0; k < sizeResultBuffer; k++)
                            {
                                var pointForCalcFS = new Point()
                                {
                                    Longitude = pointEarthGeometricsResult[k].Longitude,
                                    Latitude = pointEarthGeometricsResult[k].Latitude
                                };

                                //3.Расчет напряженности поля в точках(2.2) для BR IFIC(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                var propModel = ge06CalcData.PropagationModel;
                                GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                ge06CalcData.PropagationModel = propModel;



                                var countoursPointBRIFIC = new CountoursPoint();
                                countoursPointBRIFIC.Distance = distances[i];
                                countoursPointBRIFIC.Lon_DEC = pointForCalcFS.Longitude;
                                countoursPointBRIFIC.Lat_DEC = pointForCalcFS.Latitude;
                                //countoursPoints[t].Height = ??????????
                                countoursPointBRIFIC.PointType = PointType.Etalon;
                                countoursPointBRIFIC.FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in pointForCalcFS,
                                                                                            BroadcastingTypeContext.Brific,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );



                                var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                {
                                    Longitude_dec = pointForCalcFS.Longitude,
                                    Latitude_dec = pointForCalcFS.Latitude
                                });

                                dicCountoursPointsByBRIFIC.Add(countoursPointBRIFIC, adm);



                                //4.Расчет напряженности поля в точках(2.2) для ICSM (Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                propModel = ge06CalcData.PropagationModel;
                                GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                ge06CalcData.PropagationModel = propModel;

                                var countoursPointICSM = new CountoursPoint();
                                countoursPointICSM.Distance = distances[i];
                                countoursPointICSM.Lon_DEC = pointForCalcFS.Longitude;
                                countoursPointICSM.Lat_DEC = pointForCalcFS.Latitude;
                                //countoursPoints[t].Height = ??????????

                                countoursPointICSM.PointType = PointType.Correct;


                                countoursPointICSM.FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in pointForCalcFS,
                                                                                            BroadcastingTypeContext.Icsm,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );


                                adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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

                                    var propModel = ge06CalcData.PropagationModel;
                                    GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                    ge06CalcData.PropagationModel = propModel;

                                    earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthBRIFIC(destinationPoint,
                                                                                                                                                                            ge06CalcData,
                                                                                                                                                                            pointEarthGeometricPool,
                                                                                                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                                                                                                            poolSite,
                                                                                                                                                                            transformation,
                                                                                                                                                                            taskContext,
                                                                                                                                                                            gn06Service),
                                                                                                                                                                            in contourForStationByTriggerFieldStrengthsArgs,
                                                                                                                                                                            ref pointEarthGeometricsResultBRIFIC,
                                                                                                                                                                            out int sizeResultBufferBRIFIC);
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

                                            propModel = ge06CalcData.PropagationModel;
                                            GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                            ge06CalcData.PropagationModel = propModel;

                                            var fs = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in pointForCalcFsBRIFIC,
                                                                                            BroadcastingTypeContext.Icsm,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );

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

                                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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
                                pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                            }
                            if (pointEarthGeometricsResultBRIFIC != null)
                            {
                                pointEarthGeometricPool.Put(pointEarthGeometricsResultBRIFIC);
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
                                        pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                                        pointEarthGeometricsResultBRIFIC = pointEarthGeometricPool.Take();


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

                                        earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

                                        for (int k = 0; k < sizeResultBuffer; k++)
                                        {
                                            var pointForCalcFS = new Point()
                                            {
                                                Longitude = pointEarthGeometricsResult[k].Longitude,
                                                Latitude = pointEarthGeometricsResult[k].Latitude
                                            };
                                            //3.Расчет напряженности поля в точках(2.2) для BR IFIC(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                            var propModel = ge06CalcData.PropagationModel;
                                            GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                            ge06CalcData.PropagationModel = propModel;



                                            var countoursPointBRIFIC = new CountoursPoint();
                                            countoursPointBRIFIC.Distance = distances[i];
                                            countoursPointBRIFIC.Lon_DEC = pointForCalcFS.Longitude;
                                            countoursPointBRIFIC.Lat_DEC = pointForCalcFS.Latitude;
                                            //countoursPoints[t].Height = ??????????
                                            countoursPointBRIFIC.PointType = PointType.Etalon;
                                            countoursPointBRIFIC.FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in pointForCalcFS,
                                                                                            BroadcastingTypeContext.Brific,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );


                                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
                                            {
                                                Longitude_dec = pointForCalcFS.Longitude,
                                                Latitude_dec = pointForCalcFS.Latitude
                                            });

                                            dicCountoursPointsByBRIFIC.Add(countoursPointBRIFIC, adm);




                                            propModel = ge06CalcData.PropagationModel;
                                            GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                            ge06CalcData.PropagationModel = propModel;

                                            //4.Расчет напряженности поля в точках(2.2) для ICSM (Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).


                                            var countoursPointICSM = new CountoursPoint();
                                            countoursPointICSM.Distance = distances[i];
                                            countoursPointICSM.Lon_DEC = pointForCalcFS.Longitude;
                                            countoursPointICSM.Lat_DEC = pointForCalcFS.Latitude;
                                            //countoursPoints[t].Height = ??????????

                                            countoursPointICSM.PointType = PointType.Correct;


                                            countoursPointICSM.FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in pointForCalcFS,
                                                                                            BroadcastingTypeContext.Icsm,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );


                                            adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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

                                                var propModel = ge06CalcData.PropagationModel;
                                                GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                                ge06CalcData.PropagationModel = propModel;

                                                earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthBRIFIC(destinationPoint,
                                                                                                                                                                                        ge06CalcData,
                                                                                                                                                                                        pointEarthGeometricPool,
                                                                                                                                                                                        iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                                                                                                                        iterationHandlerFieldStrengthCalcData,
                                                                                                                                                                                        poolSite,
                                                                                                                                                                                        transformation,
                                                                                                                                                                                        taskContext,
                                                                                                                                                                                        gn06Service),
                                                                                                                                                                                        in contourForStationByTriggerFieldStrengthsArgs,
                                                                                                                                                                                        ref pointEarthGeometricsResultBRIFIC,
                                                                                                                                                                                        out int sizeResultBufferBRIFIC);
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

                                                        propModel = ge06CalcData.PropagationModel;
                                                        GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, 50, 1, 10);
                                                        ge06CalcData.PropagationModel = propModel;

                                                        // Расчет напряженности поля в каждой точке, полученной для контура в п 6: для BroadcastingAssignment[] +BroadcastingAllotment(ICSM) используя функцию 2.2.(Модель распространения 1546, процент территории 50, процент времени 1, высота абонента 10м).
                                                        countoursPoints[t].FS = (int)CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                                                                            in pointForCalcFsBRIFIC,
                                                                                            BroadcastingTypeContext.Icsm,
                                                                                            pointEarthGeometricPool,
                                                                                            iterationHandlerBroadcastingFieldStrengthCalcData,
                                                                                            iterationHandlerFieldStrengthCalcData,
                                                                                            poolSite,
                                                                                            transformation,
                                                                                            taskContext,
                                                                                            gn06Service
                                                                                            );


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

                                                        var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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
                                            pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                                        }
                                        if (pointEarthGeometricsResultBRIFIC != null)
                                        {
                                            pointEarthGeometricPool.Put(pointEarthGeometricsResultBRIFIC);
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

                var isAffectedForICSM = lstContoursResultsByICSM.Find(c => c.ContourType == ContourType.Affected);
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
                            affectedADMRes[k].TypeAffected = (isAffectedForICSM != null || isAffectedForBRIFIC != null) ? "Affected" : "Not Affected";
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
    }
}
