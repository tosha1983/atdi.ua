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


            ///список затронутых служб для ICSM
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
            //if (broadcastingContextICSM.Assignments != null)
            //{
            //    for (int i = 0; i < broadcastingContextICSM.Assignments.Length; i++)
            //    {
            //        if (broadcastingContextICSM.Assignments[i].AdmData != null)
            //        {
            //            broadcastingContextICSM.Assignments[i].AdmData.IsDigital = true;
            //        }
            //    }
            //}

            ///список затронутых служб для BRIFIC
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
                    if (broadcastingContextICSM.Allotments.AdminData != null)
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
                // Константы используемые для расчет ConformytyCheck
                var distances = new int[7] { 60, 100, 200, 300, 500, 750, 1000 };
                float ProsentTime = 1;
                float ProsentTerr = 50;
                int Height = 10;

                try
                {
                    pointEarthGeometricsResult = pointEarthGeometricPool.Take();

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

                        if ((broadcastingContextBRIFIC.Allotments == null) || (broadcastingContextBRIFIC.Allotments.AllotmentParameters != null) || (broadcastingContextBRIFIC.Allotments.AllotmentParameters.Contur is null)
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
                            var countoursPointBRIFIC = new CountoursPoint();
                            countoursPointBRIFIC.Distance = distances[i];
                            countoursPointBRIFIC.Lon_DEC = pointForCalcFS.Longitude;
                            countoursPointBRIFIC.Lat_DEC = pointForCalcFS.Latitude;
                            countoursPointBRIFIC.Height = Height;
                            countoursPointBRIFIC.PointType = PointType.Etalon;
                            countoursPointBRIFIC.FS = (int)BRIFICFS;
                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point() { Longitude_dec = pointForCalcFS.Longitude, Latitude_dec = pointForCalcFS.Latitude });
                            dicCountoursPointsByBRIFIC.Add(countoursPointBRIFIC, adm);

                            // расчет напряженности поля ICSM
                            var ICSMFS = CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                in pointForCalcFS,
                                BroadcastingTypeContext.Icsm,
                                pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service);

                            // предварительное заполнение результатов ICSM
                            var countoursPointICSM = new CountoursPoint();
                            countoursPointICSM.Distance = distances[i];
                            countoursPointICSM.Lon_DEC = pointForCalcFS.Longitude;
                            countoursPointICSM.Lat_DEC = pointForCalcFS.Latitude;
                            countoursPointICSM.Height = Height;
                            countoursPointICSM.FS = (int)ICSMFS;
                            if (BRIFICFS >= ICSMFS) { countoursPointICSM.PointType = PointType.Correct; }
                            else { countoursPointICSM.PointType = PointType.Affected; }
                            dicCountoursPointsByICSM.Add(countoursPointICSM, adm);
                        }
                    }

                    //построение контуров напряженности поля (п 5, 6,7)
                    var triggersFS = thresholdFieldStrengths.Select(x => x.ThresholdFS);
                    if (triggersFS != null)
                    {
                        var arrTriggersFS = triggersFS.ToArray();
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
                                        Longitude = pointEarthGeometricsResultBRIFIC[t].Longitude,
                                        Latitude = pointEarthGeometricsResultBRIFIC[t].Latitude
                                    };
                                    var ICSMFS = CalcFieldStrengthInPointGE06.Calc(ge06CalcData,
                                        in pointForCalcFsBRIFIC,
                                        BroadcastingTypeContext.Icsm,
                                        pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service);

                                    // предварительное сохранение результатов
                                    // предварительное заполнение результатов BRIFIC
                                    var countoursPointBRIFIC = new CountoursPoint();
                                    countoursPointBRIFIC.Lon_DEC = pointForCalcFsBRIFIC.Longitude;
                                    countoursPointBRIFIC.Lat_DEC = pointForCalcFsBRIFIC.Latitude;
                                    countoursPointBRIFIC.Height = Height;
                                    countoursPointBRIFIC.PointType = PointType.Etalon;
                                    countoursPointBRIFIC.FS = (int)triggerFS;
                                    var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point() { Longitude_dec = pointForCalcFsBRIFIC.Longitude, Latitude_dec = pointForCalcFsBRIFIC.Latitude });
                                    dicCountoursPointsByBRIFIC.Add(countoursPointBRIFIC, adm);

                                    // предварительное заполнение результатов ICSM
                                    var countoursPointICSM = new CountoursPoint();
                                    countoursPointICSM.Lon_DEC = pointForCalcFsBRIFIC.Longitude;
                                    countoursPointICSM.Lat_DEC = pointForCalcFsBRIFIC.Latitude;
                                    countoursPointICSM.Height = Height;
                                    countoursPointICSM.FS = (int)ICSMFS;
                                    if (triggerFS >= ICSMFS) { countoursPointICSM.PointType = PointType.Correct; }
                                    else { countoursPointICSM.PointType = PointType.Affected; }
                                    dicCountoursPointsByICSM.Add(countoursPointICSM, adm);
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
