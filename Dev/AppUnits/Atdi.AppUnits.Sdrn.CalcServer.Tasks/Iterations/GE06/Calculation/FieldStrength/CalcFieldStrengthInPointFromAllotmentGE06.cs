using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.Contracts.Sdrn.DeepServices.GN06;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class CalcFieldStrengthInPointFromAllotmentGE06
    {
        /// <summary>
        /// Расчет напряженности поля в точке от выделения
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static float Calc(BroadcastingAllotment broadcastingAllotment,
                                PropagationModel propagationModel,
                                Point point,
                                IObjectPool<PointEarthGeometric[]> pointEarthGeometricPool,
                                IIterationsPool iterationsPool,
                                IObjectPoolSite poolSite,
                                ITransformation transformation,
                                ITaskContext taskContext,
                                IGn06Service gn06Service,
                                ProjectMapData projectMapData,
                                CluttersDesc cluttersDesc,
                                string projection,
                                float Hrx_m 
                                )
        {
            var pointEarthGeometricsResult = default(PointEarthGeometric[]);
            //1. Формирование эталонной BroadcastingAssignment на базе BroadcastingAllotment (1.3.1).
            var broadcastingAssignment = new BroadcastingAssignment();
            gn06Service.GetEtalonBroadcastingAssignmentFromAllotment(broadcastingAllotment, broadcastingAssignment);
            //2. Вычисляются все граничные точки выделения (1.3.3). 
            var broadcastingAllotmentWithStep = new BroadcastingAllotmentWithStep()
            {
                BroadcastingAllotment = broadcastingAllotment
            };

            var points = new Points();
            try
            {
                points.PointEarthGeometrics = default(PointEarthGeometric[]);
                points.PointEarthGeometrics = pointEarthGeometricPool.Take();
                gn06Service.GetBoundaryPointsFromAllotments(in broadcastingAllotmentWithStep, ref points);
            }

            finally
            {
                if (pointEarthGeometricsResult != null)
                {
                    pointEarthGeometricPool.Put(pointEarthGeometricsResult);
                }
            }

            //3.Для каждой граничной точки формируется цикл по расчету напряженности поля от эталонной сети которая касается граничной точки в точке(исходные данные). При этом расчете совершаются 3 действия:
            var pointsWithAzimuthResult = new PointsWithAzimuthResult();
            pointsWithAzimuthResult.PointsWithAzimuth = new PointWithAzimuth[7];
            var lstBroadcastingAssignment = new BroadcastingAssignment[7];
            double maxFieldStrength = -9999;
            for (int j = 0; j < points.SizeResultBuffer; j++)
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
                gn06Service.EstimationAssignmentsPointsForEtalonNetwork(in estimationAssignmentsPointsArgs, ref pointsWithAzimuthResult);

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
                    var resultFieldStrengthInPointFromAssignmentGE06 = CalcFieldStrengthInPointFromAssignmentGE06.Calc(broadcastAssignment,
                                                                                                                       propagationModel,
                                                                                                                       point,
                                                                                                                       iterationsPool,
                                                                                                                       poolSite,
                                                                                                                       transformation,
                                                                                                                       taskContext,
                                                                                                                       gn06Service,
                                                                                                                       projectMapData,
                                                                                                                       cluttersDesc,
                                                                                                                       projection,
                                                                                                                       Hrx_m);
                    lstFieldStrengthAssignments[k] = resultFieldStrengthInPointFromAssignmentGE06;
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
        /// Максимальное значение напряженности поля
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static float SumPowGE06(double[] fs)
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
