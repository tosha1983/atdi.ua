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
        //private readonly IObjectPool<Dictionary<CountoursPoint, string>> _countoursPointByAdministrationPool;
        //private readonly IObjectPool<ContoursResult[]> _countoursResultPool;
        //private readonly IObjectPool<CountoursPoint[]> _countoursPointPool;
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
            var iterationHandlerBroadcastingFieldStrengthCalcData = _iterationsPool.GetIteration<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>();
            var iterationHandlerFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();


            this._taskContext = taskContext;
            this._ge06CalcData = data;

            //здесь вызов функции "переопределения" модели распространения в зависимости от входных параметров
            data.PropagationModel = GE06PropagationModel.GetPropagationModel(this._ge06CalcData.Ge06TaskParameters, this._ge06CalcData.PropagationModel, (CalculationType)this._ge06CalcData.Ge06TaskParameters.CalculationTypeCode);
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
                GE06CalcContoursByDistance.Calculation(in data, BroadcastingTypeContext.Icsm,
                                                        ref ge06CalcResultsForICSM,
                                                        _pointEarthGeometricPool,
                                                        iterationHandlerBroadcastingFieldStrengthCalcData,
                                                        iterationHandlerFieldStrengthCalcData,
                                                        _poolSite,
                                                        _transformation,
                                                        _taskContext,
                                                        _gn06Service,
                                                        _earthGeometricService,
                                                        _idwmService);
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


                GE06CalcContoursByDistance.Calculation(in data, BroadcastingTypeContext.Brific,
                                                        ref ge06CalcResultsForBRIFIC,
                                                        _pointEarthGeometricPool,
                                                        iterationHandlerBroadcastingFieldStrengthCalcData,
                                                        iterationHandlerFieldStrengthCalcData,
                                                        _poolSite,
                                                        _transformation,
                                                        _taskContext,
                                                        _gn06Service,
                                                        _earthGeometricService,
                                                        _idwmService);
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
                GE06CalcContoursByFS.Calculation(data, BroadcastingTypeContext.Icsm,
                                                ref ge06CalcResultsForICSM,
                                                _pointEarthGeometricPool,
                                                iterationHandlerBroadcastingFieldStrengthCalcData,
                                                iterationHandlerFieldStrengthCalcData,
                                                _poolSite,
                                                _transformation,
                                                _taskContext,
                                                _gn06Service,
                                                _earthGeometricService,
                                                _idwmService);
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

                GE06CalcContoursByFS.Calculation(data, 
                                                 BroadcastingTypeContext.Brific,
                                                 ref ge06CalcResultsForBRIFIC,
                                                 _pointEarthGeometricPool,
                                                 iterationHandlerBroadcastingFieldStrengthCalcData,
                                                 iterationHandlerFieldStrengthCalcData,
                                                 _poolSite,
                                                 _transformation,
                                                 _taskContext,
                                                 _gn06Service,
                                                 _earthGeometricService,
                                                 _idwmService);
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
                GE06CalcConformityCheck.Calculation(data,
                                                    ref ge06CalcResults,
                                                    _pointEarthGeometricPool,
                                                    iterationHandlerBroadcastingFieldStrengthCalcData,
                                                    iterationHandlerFieldStrengthCalcData,
                                                    _poolSite,
                                                    _transformation,
                                                    _taskContext,
                                                    _gn06Service,
                                                    _earthGeometricService,
                                                    _idwmService);
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
                GE06CalcFindAffectedADM.Calculation(data,
                                                    ref ge06CalcResultsForICSM,
                                                    _pointEarthGeometricPool,
                                                    iterationHandlerBroadcastingFieldStrengthCalcData,
                                                    iterationHandlerFieldStrengthCalcData,
                                                    _poolSite,
                                                    _transformation,
                                                    _taskContext,
                                                    _gn06Service,
                                                    _earthGeometricService,
                                                    _idwmService);
                
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

    }
}
