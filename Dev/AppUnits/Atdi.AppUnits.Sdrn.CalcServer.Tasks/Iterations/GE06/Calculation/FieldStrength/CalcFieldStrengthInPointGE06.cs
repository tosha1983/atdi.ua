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
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class CalcFieldStrengthInPointGE06
    {
        /// <summary>
        /// Расчет напряженности поля в точке 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static float Calc(Ge06CalcData ge06CalcData,
                                in Point point,
                                BroadcastingTypeContext broadcastingTypeContext,
                                IObjectPool<PointEarthGeometric[]> pointEarthGeometricPool,
                                IIterationsPool iterationsPool,
                                IObjectPoolSite poolSite,
                                ITransformation transformation,
                                ITaskContext taskContext,
                                IGn06Service gn06Service
                                )
        {
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
            float maxFieldStrengthAssignments = -9999;
            //1.Необходимо проверить установлена ли необходимая модель распространения и ее параметры являются ли корректными для данного рода расчета(2.2.3). 
            var propagModel = GE06PropagationModel.GetPropagationModel(ge06CalcData.Ge06TaskParameters, ge06CalcData.PropagationModel, (CalculationType)ge06CalcData.Ge06TaskParameters.CalculationTypeCode);
            //2.Далее если есть выделение, то необходимо рассчитать напряженность поля от выделения(2.2.1).
            if (broadcastingContext.Allotments != null)
            {
                resultFieldStrengthCalcResultAllotment = CalcFieldStrengthInPointFromAllotmentGE06.Calc(broadcastingContext.Allotments,
                                                                                                        ge06CalcData.PropagationModel,
                                                                                                        point,
                                                                                                        pointEarthGeometricPool,
                                                                                                        iterationsPool,
                                                                                                        poolSite,
                                                                                                        transformation,
                                                                                                        taskContext,
                                                                                                        gn06Service,
                                                                                                        ge06CalcData.MapData,
                                                                                                        ge06CalcData.CluttersDesc,
                                                                                                        ge06CalcData.Projection,
                                                                                                        ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value
                                                                                                        );
            }
            //3.Если есть BroadcastingAssignment, то рассчитать напряженность поля от каждой из них(2.2.4).При этом если станций несколько, то определяем суммарную напряженность поля для станций с одинаковым SFN_id методом суммирования мощностей(2.2.2).
            if ((broadcastingContext.Assignments != null) && (broadcastingContext.Assignments.Length > 0))
            {
                var allAssignments = broadcastingContext.Assignments.ToList();
                var allSfnId = allAssignments.Select(x => x.DigitalPlanEntryParameters.SfnId);
                if ((allSfnId != null) && (allSfnId.Count() > 0))
                {
                    allSfnId = allSfnId.Distinct();
                    var arrSfnId = allSfnId.ToArray();
                    for (int k = 0; k < arrSfnId.Length; k++)
                    {
                        var fndAssignments = allAssignments.FindAll(x => x.DigitalPlanEntryParameters.SfnId == arrSfnId[k]);
                        if ((fndAssignments != null) && (fndAssignments.Count > 0))
                        {
                            var sumFieldStrengthInPointFromAssignmentGE06 = new double[fndAssignments.Count];
                            for (int j = 0; j < fndAssignments.Count; j++)
                            {
                                sumFieldStrengthInPointFromAssignmentGE06[j] = CalcFieldStrengthInPointFromAssignmentGE06.Calc(fndAssignments[j],
                                                                                                                                ge06CalcData.PropagationModel,
                                                                                                                                point,
                                                                                                                                iterationsPool,
                                                                                                                                poolSite,
                                                                                                                                transformation,
                                                                                                                                taskContext,
                                                                                                                                gn06Service,
                                                                                                                                ge06CalcData.MapData,
                                                                                                                                ge06CalcData.CluttersDesc,
                                                                                                                                ge06CalcData.Projection,
                                                                                                                                ge06CalcData.Ge06TaskParameters.SubscribersHeight.Value
                                                                                                                                );

                            }
                            var recalcFieldStrengthInPointFromAssignmentGE06 = CalcFieldStrengthInPointFromAllotmentGE06.SumPowGE06(sumFieldStrengthInPointFromAssignmentGE06);
                            if (recalcFieldStrengthInPointFromAssignmentGE06 > maxFieldStrengthAssignments)
                            {
                                maxFieldStrengthAssignments = recalcFieldStrengthInPointFromAssignmentGE06;
                            }
                        }
                    }
                }
            }
            //4.Определить максимальную напряженность поля от станций или выделения.
            return Math.Max(resultFieldStrengthCalcResultAllotment, maxFieldStrengthAssignments);
        }
    }
}
