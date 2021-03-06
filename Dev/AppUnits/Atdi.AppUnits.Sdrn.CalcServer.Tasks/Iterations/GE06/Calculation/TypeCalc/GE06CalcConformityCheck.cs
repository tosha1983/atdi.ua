﻿using Atdi.Contracts.Sdrn.CalcServer;
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
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

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
            var affectedServices = new List<string>();

            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            var countoursPointExtendedBuffer = default(CountoursPointExtended[]);
            //var contoursResultBufferBRIFIC = default(ContoursResult[]);
            //var contoursResultBufferICSM = default(ContoursResult[]);

            //var contoursResultBufferFSBRIFIC = default(ContoursResult[]);
            //var contoursResultBufferFSICSM = default(ContoursResult[]);



            if (((ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM != null) && (ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC != null)) == false)
            {
                taskContext.SendEvent(new CalcResultEvent
                {
                    Level = CalcResultEventLevel.Error,
                    Context = "Ge06CalcIteration",
                    Message = "Incomplete ICSM data or BRIFIC"
                });
                throw new Exception("Incomplete ICSM data or BRIFIC");
            }

            //0. Валидация входных данных. аналогично п.0 4.1. + обязательные наличие хотя по одному объекту для ICSM и BRIFIC
            string notValidBroadcastingAssignmentICSM = string.Empty;
            string notValidBroadcastingAllotmentsICSM = string.Empty;
            string notValidBroadcastingAssignmentBRIFIC = string.Empty;
            string notValidBroadcastingAllotmentsBRIFIC = string.Empty;
            string message = string.Empty;

            GE06CheckEffectiveHeight.CheckEffectiveHeightForAssignment(ref ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM.Assignments, ge06CalcData.Ge06TaskParameters.UseEffectiveHeight);
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

            GE06CheckEffectiveHeight.CheckEffectiveHeightForAssignment(ref ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC.Assignments, ge06CalcData.Ge06TaskParameters.UseEffectiveHeight);
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
                taskContext.SendEvent(new CalcResultEvent
                {
                    Level = CalcResultEventLevel.Error,
                    Context = "Ge06CalcIteration",
                    Message = message
                });
                throw new Exception(message);
            }


            var broadcastingContextBRIFIC = ge06CalcData.Ge06TaskParameters.BroadcastingContext.broadcastingContextBRIFIC;
            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;



            if (broadcastingContextICSM.Allotments != null)
            {
                if ((broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC1)
                       || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC2)
                           || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC3))
                {
                    broadcastingContextICSM.Allotments.AdminData.StnClass = "BT";
                    broadcastingContextICSM.Allotments.AdminData.IsDigital = true;
                }
                else if ((broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC4)
                    || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC5)
                        )
                {
                    broadcastingContextICSM.Allotments.AdminData.StnClass = "BC";
                    broadcastingContextICSM.Allotments.AdminData.IsDigital = false;
                }
            }


            if (broadcastingContextBRIFIC.Allotments != null)
            {
                if ((broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC1)
                       || (broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC2)
                           || (broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC3))
                {
                    broadcastingContextBRIFIC.Allotments.AdminData.StnClass = "BT";
                    broadcastingContextBRIFIC.Allotments.AdminData.IsDigital = true;
                }
                else if ((broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC4)
                    || (broadcastingContextBRIFIC.Allotments.EmissionCharacteristics.RefNetworkConfig == RefNetworkConfigType.RPC5)
                        )
                {
                    broadcastingContextBRIFIC.Allotments.AdminData.StnClass = "BC";
                    broadcastingContextBRIFIC.Allotments.AdminData.IsDigital = false;
                }
            }
            if (broadcastingContextBRIFIC.Assignments != null)
            {
                for (int i = 0; i < broadcastingContextBRIFIC.Assignments.Length; i++)
                {
                    if (broadcastingContextBRIFIC.Assignments[i].AdmData!=null)
                    {
                        broadcastingContextBRIFIC.Assignments[i].AdmData.IsDigital = true;
                    }
                }
            }




            // поиск сведений о пороговых значениях напряженности поля 
            var thresholdFieldStrengths = new List<ThresholdFieldStrength>();

            var thresholdFieldStrengthsBRIFICAllotments = ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextBRIFIC.Allotments, TypeThresholdFS.OnlyBroadcastingService);
            var thresholdFieldStrengthsBRIFICAssignments = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextBRIFIC.Assignments, TypeThresholdFS.OnlyBroadcastingService);
            for (int v = 0; v < thresholdFieldStrengthsBRIFICAllotments.Length; v++)
            {
                if (thresholdFieldStrengths.Find(x => x.Freq_MHz == thresholdFieldStrengthsBRIFICAllotments[v].Freq_MHz && x.StaClass == thresholdFieldStrengthsBRIFICAllotments[v].StaClass && x.IsDigital == thresholdFieldStrengthsBRIFICAllotments[v].IsDigital) == null)
                {
                    thresholdFieldStrengths.Add(thresholdFieldStrengthsBRIFICAllotments[v]);
                }
            }
            for (int v = 0; v < thresholdFieldStrengthsBRIFICAssignments.Length; v++)
            {
                if (thresholdFieldStrengths.Find(x => x.Freq_MHz == thresholdFieldStrengthsBRIFICAssignments[v].Freq_MHz && x.StaClass == thresholdFieldStrengthsBRIFICAssignments[v].StaClass && x.IsDigital == thresholdFieldStrengthsBRIFICAssignments[v].IsDigital) == null)
                {
                    thresholdFieldStrengths.Add(thresholdFieldStrengthsBRIFICAssignments[v]);
                }
            }



            var thresholdFieldStrengthsICSMAllotments = ThresholdFS.GetThresholdFieldStrengthByAllotments(broadcastingContextICSM.Allotments, TypeThresholdFS.OnlyBroadcastingService);
            var thresholdFieldStrengthsICSMAssignments = ThresholdFS.GetThresholdFieldStrengthByAssignments(broadcastingContextICSM.Assignments, TypeThresholdFS.OnlyBroadcastingService);

            for (int v = 0; v < thresholdFieldStrengthsICSMAllotments.Length; v++)
            {
                if (thresholdFieldStrengths.Find(x => x.Freq_MHz == thresholdFieldStrengthsICSMAllotments[v].Freq_MHz && x.StaClass == thresholdFieldStrengthsICSMAllotments[v].StaClass && x.IsDigital == thresholdFieldStrengthsICSMAllotments[v].IsDigital) == null)
                {
                    thresholdFieldStrengths.Add(thresholdFieldStrengthsICSMAllotments[v]);
                }
            }
            for (int v = 0; v < thresholdFieldStrengthsICSMAssignments.Length; v++)
            {
                if (thresholdFieldStrengths.Find(x => x.Freq_MHz == thresholdFieldStrengthsICSMAssignments[v].Freq_MHz && x.StaClass == thresholdFieldStrengthsICSMAssignments[v].StaClass && x.IsDigital == thresholdFieldStrengthsICSMAssignments[v].IsDigital) == null)
                {
                    thresholdFieldStrengths.Add(thresholdFieldStrengthsICSMAssignments[v]);
                }
            }




            //1.Определение центра гравитации(2.1)
            var pointEarthGeometricBarycenter = new PointEarthGeometric();

            if ((broadcastingContextBRIFIC != null) && (broadcastingContextICSM != null))
            {

                ///список затронутых служб для брифика
                if (broadcastingContextBRIFIC.Allotments != null)
                {
                    if (broadcastingContextBRIFIC.Allotments.AdminData != null)
                    {
                        if (!affectedServices.Contains(broadcastingContextBRIFIC.Allotments.AdminData.StnClass))
                        {
                            affectedServices.Add(broadcastingContextBRIFIC.Allotments.AdminData.StnClass);
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
                    if (broadcastingContextICSM.Allotments.AdminData != null)
                    {
                        if (!affectedServices.Contains(broadcastingContextICSM.Allotments.AdminData.StnClass))
                        {
                            affectedServices.Add(broadcastingContextICSM.Allotments.AdminData.StnClass);
                        }
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
                // Константы используемые для расчет ConformytyCheck
                var distances = new int[7] { 60, 100, 200, 300, 500, 750, 1000 };
                float ProsentTime = 1;
                float ProsentTerr = 50;
                int Height = 10;
                int indexResult = 0;

                try
                {
                    int currPercentComplete = 0;

                    int indexForCountoursPointExtendedBuffer = 0;

                    pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                    countoursPointExtendedBuffer = countoursPointExtendedPool.Take();
                    //contoursResultBufferBRIFIC = contoursResultPool.Take();
                    //contoursResultBufferICSM = contoursResultPool.Take();
                    //contoursResultBufferFSBRIFIC = contoursResultPool.Take();
                    //contoursResultBufferFSICSM = contoursResultPool.Take();

                    //установка модели распространения
                    var propModel = ge06CalcData.PropagationModel;
                    GE06PropagationModel.GetPropagationModelForConformityCheck(ref propModel, ProsentTerr, ProsentTime);
                    ge06CalcData.PropagationModel = propModel;
                    ge06CalcData.Ge06TaskParameters.SubscribersHeight = Height;

                    // цикл по определению контуров по фиксированнім дистанциям и напряженностей поля в точка в них 
                    for (int i = 0; i < distances.Length; i++)
                    {
                        int sizeResultBuffer = 0;
                        // строим контура и считаем напряженность поля для assigments

                        if ((broadcastingContextBRIFIC.Allotments == null) || (broadcastingContextBRIFIC.Allotments.AllotmentParameters == null) || (broadcastingContextBRIFIC.Allotments.AllotmentParameters.Contur == null)
                            || (broadcastingContextBRIFIC.Allotments.AllotmentParameters.Contur.Length < 3))
                        {
                            var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                            {
                                Distance_km = distances[i],
                                Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                PointEarthGeometricCalc = pointEarthGeometricBarycenter
                            };
                            earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                        }
                        else
                        {
                            var areaPoints = broadcastingContextBRIFIC.Allotments.AllotmentParameters.Contur;
                            var pointEarthGeometrics = new PointEarthGeometric[areaPoints.Length];
                            for (int h = 0; h < areaPoints.Length; h++)
                            { pointEarthGeometrics[h] = new PointEarthGeometric(areaPoints[h].Lon_DEC, areaPoints[h].Lat_DEC, CoordinateUnits.deg); }
                            var contourFromContureByDistanceArgs = new ContourFromContureByDistanceArgs()
                            {
                                Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                Distance_km = distances[i],
                                PointBaryCenter = pointEarthGeometricBarycenter,
                                ContourPoints = pointEarthGeometrics
                            };
                            earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out sizeResultBuffer);
                        }

                        // контур получен 

                        // проводим расчет напряженности поля для контура BRIFIC и ICSM (п 2, 3, 4)
                        for (int k = 0; k < sizeResultBuffer; k++)
                        {
                            var pointForCalcFS = new Point()
                            {
                                Longitude = pointEarthGeometricsResult[k].Longitude,
                                Latitude = pointEarthGeometricsResult[k].Latitude
                            };


                            // расчет напряженности поля BRIFIC
                            var BRIFICFS = CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                in pointForCalcFS,
                                BroadcastingTypeContext.Brific,
                                pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service);

                            // предварительное заполнение результатов BRIFIC

                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer] = new CountoursPointExtended();
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Distance = distances[i];
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lon_DEC = pointForCalcFS.Longitude;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lat_DEC = pointForCalcFS.Latitude;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = Height;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Etalon;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = BRIFICFS;
                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point() { Longitude_dec = pointForCalcFS.Longitude, Latitude_dec = pointForCalcFS.Latitude });
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeContext = BroadcastingTypeContext.Brific;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.Distance;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Id = indexResult;
                            indexForCountoursPointExtendedBuffer++;


                            // расчет напряженности поля ICSM
                            var ICSMFS = CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                in pointForCalcFS,
                                BroadcastingTypeContext.Icsm,
                                pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service);

                            // предварительное заполнение результатов ICSM
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer] = new CountoursPointExtended();
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Distance = distances[i];
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lon_DEC = pointForCalcFS.Longitude;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lat_DEC = pointForCalcFS.Latitude;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = Height;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = ICSMFS;
                            if (BRIFICFS >= ICSMFS-0.1) { countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Correct; }
                            else { countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Affected; }
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeContext = BroadcastingTypeContext.Icsm;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.Distance;
                            countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Id = indexResult;
                            indexForCountoursPointExtendedBuffer++;

                            UpdateProgress.UpdatePercentComplete50(distances.Length, sizeResultBuffer, i, k, 0, ref currPercentComplete, "ConformityCheck", taskContext);
                        }
                        indexResult++;
                    }

                    //построение контуров напряженности поля (п 5, 6,7)
                    int[] arrFieldStrength = null;
                    var triggersFS = thresholdFieldStrengths.Select(x => x.ThresholdFS);
                    if (triggersFS != null)
                    {
                        var arrTriggersFS = triggersFS.ToArray();
                        arrFieldStrength = new int[arrTriggersFS.Length];
                        // идем по значениям напряженности поля
                        for (int d = 0; d < arrTriggersFS.Length; d++)
                        {
                            var triggerFS = arrTriggersFS[d];
                            // Построение контура для напряженности поля относительно центра гравитации для BRIFIC.
                            var contourForStationByTriggerFieldStrengthsArgs = new ContourForStationByTriggerFieldStrengthsArgs()
                            {
                                Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                                TriggerFieldStrength = triggerFS,
                                BaryCenter = pointEarthGeometricBarycenter
                            };
                            earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthBRIFIC(destinationPoint,
                                ge06CalcData, pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service),
                                in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResult, out int sizeResultBufferBRIFIC);
                            // Конец построения контура для напряженности поля относительно центра гравитации для BRIFIC. 

                            // расчент напряженности поля на контур BRIFIC от ICSM
                            if (sizeResultBufferBRIFIC > 0)
                            {
                                for (int t = 0; t < sizeResultBufferBRIFIC; t++)
                                {
                                    // расчент напряженности поля на контур BRIFIC от ICSM в точке
                                    var pointForCalcFsBRIFIC = new Point()
                                    {
                                        Longitude = pointEarthGeometricsResult[t].Longitude,
                                        Latitude = pointEarthGeometricsResult[t].Latitude
                                    };
                                    var ICSMFS = CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                        in pointForCalcFsBRIFIC,
                                        BroadcastingTypeContext.Icsm,
                                        pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service);

                                    // предварительное сохранение результатов
                                    // предварительное заполнение результатов BRIFIC
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer] = new CountoursPointExtended();
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lon_DEC = pointForCalcFsBRIFIC.Longitude;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lat_DEC = pointForCalcFsBRIFIC.Latitude;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = Height;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Etalon;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = triggerFS;
                                    var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point() { Longitude_dec = pointForCalcFsBRIFIC.Longitude, Latitude_dec = pointForCalcFsBRIFIC.Latitude });
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeContext = BroadcastingTypeContext.Brific;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.FieldStrength;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Id = indexResult;
                                    indexForCountoursPointExtendedBuffer++;

                                    // предварительное заполнение результатов ICSM
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer] = new CountoursPointExtended();
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lon_DEC = pointForCalcFsBRIFIC.Longitude;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Lat_DEC = pointForCalcFsBRIFIC.Latitude;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Height = Height;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].FS = ICSMFS;
                                    if (triggerFS >= ICSMFS-0.1) { countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Correct; }
                                    else { countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].PointType = PointType.Affected; }
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].administration = adm;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeContext = BroadcastingTypeContext.Icsm;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].broadcastingTypeCalculation = BroadcastingTypeCalculation.FieldStrength;
                                    countoursPointExtendedBuffer[indexForCountoursPointExtendedBuffer].Id = indexResult;
                                    indexForCountoursPointExtendedBuffer++;

                                    UpdateProgress.UpdatePercentComplete50(arrTriggersFS.Length, sizeResultBufferBRIFIC, d, t, 50, ref currPercentComplete, "ConformityCheck", taskContext);
                                }
                                indexResult++;
                            }
                        }
                    }
                    GetResult(in countoursPointExtendedBuffer, indexForCountoursPointExtendedBuffer, out ge06CalcResult.ContoursResult);
                    ge06CalcResult.AffectedADMResult = FillAffectedADMResult.Fill(ge06CalcResult.ContoursResult, string.Join(",", affectedServices));
                    UpdateProgress.UpdatePercentComplete100(ref currPercentComplete, "ConformityCheck", taskContext);
                }
                finally
                {
                    if (pointEarthGeometricsResult != null)
                    {
                        pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                    }
                    if (countoursPointExtendedBuffer!=null)
                    {
                        countoursPointExtendedPool.Put(countoursPointExtendedBuffer);
                    }

                    //if (contoursResultBufferBRIFIC != null)
                    //{
                    //    contoursResultPool.Put(contoursResultBufferBRIFIC);
                    //}
                    //if (contoursResultBufferICSM != null)
                    //{
                    //    contoursResultPool.Put(contoursResultBufferICSM);
                    //}
                    //if (contoursResultBufferFSBRIFIC != null)
                    //{
                    //    contoursResultPool.Put(contoursResultBufferFSBRIFIC);
                    //}
                    //if (contoursResultBufferFSICSM != null)
                    //{
                    //    contoursResultPool.Put(contoursResultBufferFSICSM);
                    //}
                    
                }

                var allotmentOrAssignmentResult = new List<AllotmentOrAssignmentResult>();

                GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextBRIFIC, BroadcastingTypeContext.Brific, ref ge06CalcResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResult.AllotmentOrAssignmentResult);

                GE06FillData.FillAllotmentOrAssignmentResult(broadcastingContextICSM, BroadcastingTypeContext.Icsm, ref ge06CalcResult);
                allotmentOrAssignmentResult.AddRange(ge06CalcResult.AllotmentOrAssignmentResult);

                ge06CalcResult.AllotmentOrAssignmentResult = allotmentOrAssignmentResult.ToArray();
            }
        }
        private static void GetResult(in CountoursPointExtended[] countoursPointExtendedBuffer, int indexForCountoursPointExtendedBuffer, out ContoursResult[] contoursResult)
        {

            List<ContoursResult> contoursResultList = new List<ContoursResult>();
            List<string> admList = new List<string>(); // обычно до до 6 елементов
            List<int> IdList = new List<int>();// обычно до до 10 елементов
            List<CountoursPoint> countoursPointsICSMLocal = new List<CountoursPoint>(); // можно через пул после отладки
            List<CountoursPoint> countoursPointsBRIFICLocal = new List<CountoursPoint>(); // можно через пул после отладки

            // расчет количества елементов в массиве результатов 
            for (int i = 0; indexForCountoursPointExtendedBuffer > i; i++)
            {
                var Id  = countoursPointExtendedBuffer[i].Id;
                if (!(IdList.Exists(x => x == Id))) { IdList.Add(Id);}
                var adm = countoursPointExtendedBuffer[i].administration;
                if (!(admList.Exists(x => x == adm))){admList.Add(adm);}
            }
            foreach (int Id in IdList)
            {
                foreach (string adm in admList)
                {
                    countoursPointsICSMLocal = new List<CountoursPoint>();
                    countoursPointsBRIFICLocal = new List<CountoursPoint>();
                    BroadcastingTypeCalculation broadcastingTypeCalculation = BroadcastingTypeCalculation.Distance;
                    for (int i = 0; indexForCountoursPointExtendedBuffer > i; i++)
                    {
                        if ((countoursPointExtendedBuffer[i].Id == Id) && (countoursPointExtendedBuffer[i].administration == adm))
                        {
                            if (countoursPointExtendedBuffer[i].broadcastingTypeContext == BroadcastingTypeContext.Brific)
                            { countoursPointsBRIFICLocal.Add(countoursPointExtendedBuffer[i]);
                                broadcastingTypeCalculation = countoursPointExtendedBuffer[i].broadcastingTypeCalculation;}
                            else
                            { countoursPointsICSMLocal.Add(countoursPointExtendedBuffer[i]);
                                broadcastingTypeCalculation = countoursPointExtendedBuffer[i].broadcastingTypeCalculation;}
                        }
                    }
                    // заполнение последующих параметров
                    if (countoursPointsBRIFICLocal.Count != 0)
                    {
                        ContoursResult contourBRIFIC = new ContoursResult
                        {
                            AffectedADM = adm,
                            CountoursPoints = countoursPointsBRIFICLocal.ToArray(),
                            ContourType = ContourType.Etalon,
                            PointsCount = countoursPointsBRIFICLocal.Count,
                        };
                        if (broadcastingTypeCalculation == BroadcastingTypeCalculation.Distance)
                        { contourBRIFIC.Distance = countoursPointsBRIFICLocal[0].Distance; }
                        else { contourBRIFIC.FS = countoursPointsBRIFICLocal[0].FS; }
                        contoursResultList.Add(contourBRIFIC);
                    }
                    if (countoursPointsICSMLocal.Count != 0)
                    {

                        var arrCountoursPoint = new CountoursPoint[countoursPointsICSMLocal.Count];
                        for (int x = 0; x < countoursPointsICSMLocal.Count; x++)
                        {
                            arrCountoursPoint[x] = new CountoursPoint()
                            {
                                Distance = countoursPointsICSMLocal[x].Distance,
                                FS = countoursPointsICSMLocal[x].FS,
                                Height = countoursPointsICSMLocal[x].Height,
                                Lat_DEC = countoursPointsICSMLocal[x].Lat_DEC,
                                Lon_DEC = countoursPointsICSMLocal[x].Lon_DEC,
                                PointType = countoursPointsICSMLocal[x].PointType
                            };
                        }

                        ContoursResult contourICSM = new ContoursResult
                        {
                            AffectedADM = adm,
                            CountoursPoints = arrCountoursPoint,
                            PointsCount = arrCountoursPoint.Length
                        };
                        if (broadcastingTypeCalculation == BroadcastingTypeCalculation.Distance)
                        { contourICSM.Distance = countoursPointsICSMLocal[0].Distance;}
                        if (countoursPointsICSMLocal.Exists(x => x.PointType == PointType.Affected))
                        {contourICSM.ContourType = ContourType.Affected;}
                        else {contourICSM.ContourType = ContourType.Correct;}
                        contoursResultList.Add(contourICSM);
                    }
                }
            }
            contoursResult = contoursResultList.ToArray(); 
        }
    }
}
