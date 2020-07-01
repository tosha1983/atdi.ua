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


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// 
    /// </summary>
    public class Ge06CalcIteration : IIterationHandler<Ge06CalcData, Ge06CalcResult[]>
    {
        private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly IDataLayer<EntityDataOrm<CalcServerEntityOrmContext>> _calcServerDataLayer;
        private readonly IObjectPoolSite _poolSite;
        private readonly ITransformation _transformation;
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
            Ge06CalcData ge06CalcData,
            ILogger logger)
        {
            _calcServerDataLayer = calcServerDataLayer;
            _iterationsPool = iterationsPool;
            _earthGeometricService = earthGeometricService;
            _poolSite = poolSite;
            _appServerComponentConfig = appServerComponentConfig;
            _transformation = transformation;
            _gn06Service = gn06Service;
            _ge06CalcData = ge06CalcData;
            _logger = logger;
        }



        public Ge06CalcResult[] Run(ITaskContext taskContext, Ge06CalcData data)
        {
            this._taskContext = taskContext;
            var ge06CalcResults = new Ge06CalcResult[1];
            // где брать точку point????????????????
            // нужно последовательно в цикле перебирать все комбинации Allotments и Assignments ??????????????
            //CalcFieldStrengthInPointGE06((CalculationType)data.Ge06TaskParameters.CalculationTypeCode, data.PropagationModel, data.Ge06TaskParameters.BroadcastingContext.Allotments[i], data.Ge06TaskParameters.BroadcastingContext.Assignments, point)

            return ge06CalcResults;
        }


        /// <summary>
        /// Установка модели распространения и ее параметров 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private PropagationModel GetPropagationModel(PropagationModel data, CalculationType calculationType)
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
        private float CalcFieldStrengthInPointGE06(CalculationType calculationType, PropagationModel propagationModel, BroadcastingAllotment broadcastingAllotment, BroadcastingAssignment[] broadcastingAssignments, Point point)
        {
            float resultFieldStrengthCalcResultAllotment = 0;
            var lstFieldStrengthAssignments = new List<float>();
            //1.Необходимо проверить установлена ли необходимая модель распространения и ее параметры являются ли корректными для данного рода расчета(2.2.3). 
            var propagModel = GetPropagationModel(propagationModel, calculationType);
            //2.Далее если есть выделение, то необходимо рассчитать напряженность поля от выделения(2.2.1).
            if (broadcastingAllotment != null)
            {
                 resultFieldStrengthCalcResultAllotment = CalcFieldStrengthInPointFromAllotmentGE06(broadcastingAllotment, propagationModel, point);
            }
            //3.Если есть BroadcastingAssignment, то рассчитать напряженность поля от каждой из них(2.2.4).При этом если станций несколько, то определяем суммарную напряженность поля для станций с одинаковым SFN_id методом суммирования мощностей(2.2.2).
            if (broadcastingAssignments != null)
            {
                var allAssignments = broadcastingAssignments.ToList();
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
                                sumFieldStrengthInPointFromAssignmentGE06[j] = CalcFieldStrengthInPointFromAssignmentGE06(fndAssignments[j], propagationModel, point);
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
        private float  CalcFieldStrengthInPointFromAllotmentGE06(BroadcastingAllotment broadcastingAllotment, PropagationModel propagationModel, Point point)
        {
            var lstFieldStrengthAssignments = new List<double?>();
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
                        Lon_DEC = points.PointEarthGeometrics[j].Longitude,
                        Lat_DEC = points.PointEarthGeometrics[j].Latitude
                    },
                    PointAllotment = new AreaPoint()
                    {
                        Lon_DEC = point.Longitude,
                        Lat_DEC = point.Latitude
                    }
                };

                //а) Определения положений эталонных присвоений BroadcastingAssignment 1.1.5;
                var pointWithAzimuthResult = new PointWithAzimuthResult();
                this._gn06Service.EstimationAssignmentsPointsForEtalonNetwork(in estimationAssignmentsPointsArgs, ref pointWithAzimuthResult);

                //в) Расчет напряженности поля от каждого эталонного BroadcastingAssignment(2.2.4).Перед расчетом производиться корректировка паттерна BroadcastingAssignment в соответствии с его ориентацией(суть корректировки спросить Максима или Юру).
                var resultFieldStrengthInPointFromAssignmentGE06 = CalcFieldStrengthInPointFromAssignmentGE06(broadcastingAssignment, propagationModel, point);
                lstFieldStrengthAssignments.Add(resultFieldStrengthInPointFromAssignmentGE06);
            }

            //г) Определение суммарной напряженности поля(2.2.2) .

            return SumPowGE06(lstFieldStrengthAssignments.ToArray());
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

            //1) Если модель распространения ITU 1546 для этого необходимо делать отдельную итерацию(Будет подготовлен сам код Юрой или Максимом) на подобии FieldStrengthCalcIteration(1.4.1, 1.4.2)
            var broadcastingFieldStrengthCalcData = new BroadcastingFieldStrengthCalcData()
            {
                 BroadcastingAssignment = broadcastingAssignment,
                 PropagationModel = propagationModel,
                 CluttersDesc = this._ge06CalcData.CluttersDesc
                // заполнить надо
            };
            var iterationCorellationCalc = _iterationsPool.GetIteration<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>();
            var resFieldStrengthCalcResult = iterationCorellationCalc.Run(this._taskContext, broadcastingFieldStrengthCalcData);
            resultCalcFieldStrength = (float)resFieldStrengthCalcResult.FS_dBuVm.Value;

            //2) Если модель распространения не ITU 1546.В данном случае BroadcastingAssignment преобразуется в Station(IContextStation)(1.3.2) и используется для расчета итерация FieldStrengthCalcIteration
            var fieldStrengthCalcData = new FieldStrengthCalcData()
            {
                //следует заполнить 

            };
            var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
            var resFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(this._taskContext, fieldStrengthCalcData);
            resultCalcFieldStrength = (float)resFieldStrengthCalcData.FS_dBuVm.Value;

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
