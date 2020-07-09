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
using Idwm = Atdi.Contracts.Sdrn.DeepServices.IDWM;
using IdwmDataModel = Atdi.DataModels.Sdrn.DeepServices.IDWM;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class GE06CalcFindAffectedADM
    {
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

            if (ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM == null)
            {
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
                throw new Exception(message);
            }



            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultICSM = default(PointEarthGeometric[]);
            var pointEarthGeometricsResultAffectedICSM = default(PointEarthGeometric[]);



            var lstContoursResults = new List<ContoursResult>();

            var broadcastingContextICSM = ge06CalcData.Ge06TaskParameters.BroadcastingContext.BroadcastingContextICSM;


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
                gn06Service.CalcBarycenterGE06(in broadcastingCalcBarycenterGE06, ref pointEarthGeometricBarycenter);
            }


            if ((broadcastingContextICSM.Allotments == null) && ((broadcastingContextICSM.Assignments != null) && (broadcastingContextICSM.Assignments.Length > 0)))
            {
                var contourFromPointByDistanceArgs = new ContourFromPointByDistanceArgs()
                {
                    Distance_km = 1000,
                    Step_deg = ge06CalcData.Ge06TaskParameters.AzimuthStep_deg.Value,
                    PointEarthGeometricCalc = pointEarthGeometricBarycenter
                };

                try
                {
                    pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                    pointEarthGeometricsResultICSM = pointEarthGeometricPool.Take();
                    pointEarthGeometricsResultAffectedICSM = pointEarthGeometricPool.Take();

                    earthGeometricService.CreateContourFromPointByDistance(in contourFromPointByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);

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


                        var adm = idwmService.GetADMByPoint(pointForCalc);

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
                                var propModel = ge06CalcData.PropagationModel;
                                GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, 1);
                                ge06CalcData.PropagationModel = propModel;

                                earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthICSM(destinationPoint, ge06CalcData, pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultICSM, out int sizeResultBufferICSM);
                                if (sizeResultBufferICSM > 0)
                                {
                                    for (int t = 0; t < sizeResultBufferICSM; t++)
                                    {
                                        var pointForCalcFsICSM = new Point()
                                        {
                                            Longitude = pointEarthGeometricsResultICSM[t].Longitude,
                                            Latitude = pointEarthGeometricsResultICSM[t].Latitude
                                        };

                                        var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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
                                    var propModel = ge06CalcData.PropagationModel;
                                    GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, triggerInformation.Time_pc);
                                    ge06CalcData.PropagationModel = propModel;

                                    earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthICSM(destinationPoint, ge06CalcData, pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultAffectedICSM, out int sizeResultBufferRecalcICSM);
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
                                            if (earthGeometricService.CheckHitting(in checkHittingArgs))
                                            {
                                                typeAffected = "Assignment";
                                                break;
                                            }
                                        }





                                        var countoursPoints = new CountoursPoint[sizeResultBufferRecalcICSM];
                                        for (int t = 0; t < sizeResultBufferRecalcICSM; t++)
                                        {
                                            // нахождение администрации к которой принадлежит точка контура pointEarthGeometricsResultAffectedICSM
                                            var adminRecalc = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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


                                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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
                }
            }
            //или на функции CreateContourFromContureByDistance если у нас есть BroadcastingAllotment 
            else
            {
                var areaPoints = broadcastingContextICSM.Allotments.AllotmentParameters.Contur;
                var freq_MHz = broadcastingContextICSM.Allotments.Target.Freq_MHz;
                var isDigital = broadcastingContextICSM.Allotments.AdminData.IsDigital;
                string stnCls = "";

                if ((broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC1)
                || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC2)
                || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC3))
                {
                    stnCls = "BT";
                }
                else if ((broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC4)
                || (broadcastingContextICSM.Allotments.EmissionCharacteristics.RefNetworkConfig == DataModels.Sdrn.DeepServices.GN06.RefNetworkConfigType.RPC5))
                {
                    stnCls = "BC";
                }


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


                        pointEarthGeometricsResult = pointEarthGeometricPool.Take();
                        pointEarthGeometricsResultICSM = pointEarthGeometricPool.Take();
                        pointEarthGeometricsResultAffectedICSM = pointEarthGeometricPool.Take();

                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////
                        ///   1 .Построение контура 1000км
                        ////  Результат: контур и список потенциально затронутых администраций (TypeAffected = 1000).
                        ///
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        earthGeometricService.CreateContourFromContureByDistance(in contourFromContureByDistanceArgs, ref pointEarthGeometricsResult, out int sizeResultBuffer);
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


                            var adm = idwmService.GetADMByPoint(pointForCalc);

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
                                    var propModel = ge06CalcData.PropagationModel;
                                    GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, 1);
                                    ge06CalcData.PropagationModel = propModel;

                                    earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthICSM(destinationPoint, ge06CalcData, pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultICSM, out int sizeResultBufferICSM);
                                    if (sizeResultBufferICSM > 0)
                                    {
                                        for (int t = 0; t < sizeResultBufferICSM; t++)
                                        {
                                            var pointForCalcFsICSM = new Point()
                                            {
                                                Longitude = pointEarthGeometricsResultICSM[t].Longitude,
                                                Latitude = pointEarthGeometricsResultICSM[t].Latitude
                                            };

                                            var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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
                                        var propModel = ge06CalcData.PropagationModel;
                                        GE06PropagationModel.GetPropagationModelForFindAffectedADM(ref propModel, 50, triggerInformation.Time_pc);
                                        ge06CalcData.PropagationModel = propModel;

                                        earthGeometricService.CreateContourForStationByTriggerFieldStrengths((destinationPoint) => GE06CalcContoursByFS.CalcFieldStrengthICSM(destinationPoint, ge06CalcData, pointEarthGeometricPool, iterationHandlerBroadcastingFieldStrengthCalcData, iterationHandlerFieldStrengthCalcData, poolSite, transformation, taskContext, gn06Service), in contourForStationByTriggerFieldStrengthsArgs, ref pointEarthGeometricsResultAffectedICSM, out int sizeResultBufferRecalcICSM);
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
                                                if (earthGeometricService.CheckHitting(in checkHittingArgs))
                                                {
                                                    typeAffected = "Assignment";
                                                    break;
                                                }
                                            }





                                            var countoursPoints = new CountoursPoint[sizeResultBufferRecalcICSM];
                                            for (int t = 0; t < sizeResultBufferRecalcICSM; t++)
                                            {
                                                // нахождение администрации к которой принадлежит точка контура pointEarthGeometricsResultAffectedICSM
                                                var adminRecalc = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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


                                                var adm = idwmService.GetADMByPoint(new IdwmDataModel.Point()
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
    }
}
