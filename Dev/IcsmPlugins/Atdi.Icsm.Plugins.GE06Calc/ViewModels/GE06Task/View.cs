using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using VM = Atdi.Icsm.Plugins.GE06Calc.ViewModels;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries;
//using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Adapters;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Data;
using System.Windows;
using ICSM;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;
using Atdi.WpfControls.EntityOrm.Controls;
using ST = Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Models;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task
{
    [ViewXaml("GE06Task.xaml")]
    [ViewCaption("GE06: Task parameters")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private CalcTaskModel _currentCalcTaskCard;
        private IList _currentAssignmentsAllotments;
        private IMQueryMenuNode.Context _context;
        private bool _startEnabled = false;
        private CalculationType _calcType;
        private OrmEnumBoxData[] _typeSource;
        private long _activeContextId;
        private bool _stepBetweenBoundaryPointsDefault;
        private bool _stepBetweenBoundaryPointsDefaultEnabled;

        private Visibility _azimuthStepVisibility = Visibility.Collapsed;
        private Visibility _additionalContoursByDistancesVisibility = Visibility.Collapsed;
        private Visibility _contureByFieldStrengthVisibility = Visibility.Collapsed;
        private Visibility _subscribersHeightVisibility = Visibility.Collapsed;
        private Visibility _percentageTimeVisibility = Visibility.Collapsed;
        private Visibility _useEffectiveHeightVisibility = Visibility.Collapsed;
        private Visibility _stepBetweenBoundaryPointsVisibility = Visibility.Collapsed;

        List<AssignmentsAllotmentsModel> _assignmentsAllotmentsList;
        AssignmentsAllotmentsModel[] _assignmentsAllotmentsArray;

        public ViewCommand AllotDeleteCommand { get; set; }
        public ViewCommand StartCommand { get; set; }

        private MapDrawingData _currentMapData;

        private IEventHandlerToken<Events.OnCreatedCalcTask> _onCreatedCalcTaskToken;
        private IEventHandlerToken<Events.OnRunedCalcTask> _onRunedCalcTaskToken;
        public View(
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger,
            AppComponentConfig config)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;

            this.AllotDeleteCommand = new ViewCommand(this.OnAllotDeleteCommand);
            this.StartCommand = new ViewCommand(this.OnStartCommand);
            this._assignmentsAllotmentsList = new List<AssignmentsAllotmentsModel>();

            this.CurrentCalcTaskCard = new CalcTaskModel()
            {
                AzimuthStep_deg = 5,
                AdditionalContoursByDistances = false,
                DistancesString = "50,80,150",
                ContureByFieldStrength = false,
                FieldStrengthString = "12,25",
                SubscribersHeight = 10,
                PercentageTime = 50,
                UseEffectiveHeight = true,
                StepBetweenBoundaryPoints = 5,
                StepBetweenBoundaryPointsDefault = true
            };
            StepBetweenBoundaryPointsDefault = true;
            StepBetweenBoundaryPointsDefaultEnabled = false;

            _onCreatedCalcTaskToken = _eventBus.Subscribe<Events.OnCreatedCalcTask>(this.OnCreatedCalcTaskHandle);
            _onRunedCalcTaskToken = _eventBus.Subscribe<Events.OnRunedCalcTask>(this.OnRunedCalcTaskHandle);

            if (long.TryParse(config.CalcServerActiveContextId, out long contextId))
                this._activeContextId = contextId;

            if (Properties.Settings.Default.ActiveContext != 0)
                this._activeContextId = Properties.Settings.Default.ActiveContext;
        }
        public Visibility AzimuthStepVisibility
        {
            get => this._azimuthStepVisibility;
            set => this.Set(ref this._azimuthStepVisibility, value);
        }
        public Visibility AdditionalContoursByDistancesVisibility
        {
            get => this._additionalContoursByDistancesVisibility;
            set => this.Set(ref this._additionalContoursByDistancesVisibility, value);
        }
        public Visibility ContureByFieldStrengthVisibility
        {
            get => this._contureByFieldStrengthVisibility;
            set => this.Set(ref this._contureByFieldStrengthVisibility, value);
        }
        public Visibility SubscribersHeightVisibility
        {
            get => this._subscribersHeightVisibility;
            set => this.Set(ref this._subscribersHeightVisibility, value);
        }
        public Visibility PercentageTimeVisibility
        {
            get => this._percentageTimeVisibility;
            set => this.Set(ref this._percentageTimeVisibility, value);
        }
        public Visibility UseEffectiveHeightVisibility
        {
            get => this._useEffectiveHeightVisibility;
            set => this.Set(ref this._useEffectiveHeightVisibility, value);
        }
        public Visibility StepBetweenBoundaryPointsVisibility
        {
            get => this._stepBetweenBoundaryPointsVisibility;
            set => this.Set(ref this._stepBetweenBoundaryPointsVisibility, value);
        }
        public OrmEnumBoxData[] CalcTypeSource
        {
            get => this._typeSource;
            set => this.Set(ref this._typeSource, value);
        }
        public CalculationType CalcType
        {
            get => this._calcType;
            set => this.Set(ref this._calcType, value, () => { this.OnChangedCalcType(value); });
        }
        public CalcTaskModel CurrentCalcTaskCard
        {
            get => this._currentCalcTaskCard;
            set => this.Set(ref this._currentCalcTaskCard, value);
        }
        public IList CurrentAssignmentsAllotments
        {
            get => this._currentAssignmentsAllotments;
            set
            {
                this._currentAssignmentsAllotments = value;
                RedrawMap();
            }
        }
        public bool StartEnabled
        {
            get => this._startEnabled;
            set => this.Set(ref this._startEnabled, value);
        }
        public IMQueryMenuNode.Context Context
        {
            get => this._context;
            set => this.Set(ref this._context, value, () => { this.OnChangedContext(value); });
        }
        public AssignmentsAllotmentsModel[] AssignmentsAllotmentsArray
        {
            get => this._assignmentsAllotmentsArray;
            set => this.Set(ref this._assignmentsAllotmentsArray, value);
        }
        public MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        public bool StepBetweenBoundaryPointsDefault
        {
            get => this._stepBetweenBoundaryPointsDefault;
            set => this.Set(ref this._stepBetweenBoundaryPointsDefault, value, () => { CurrentCalcTaskCard.StepBetweenBoundaryPointsDefault = value;  this.StepBetweenBoundaryPointsDefaultEnabled = !value; });
        }
        public bool StepBetweenBoundaryPointsDefaultEnabled
        {
            get => this._stepBetweenBoundaryPointsDefaultEnabled;
            set => this.Set(ref this._stepBetweenBoundaryPointsDefaultEnabled, value);
        }
        private void OnChangedCalcType(CalculationType type)
        {
            StartEnabled = true;
            if (type == CalculationType.ConformityCheck)
            {
                this.AzimuthStepVisibility = Visibility.Visible;
                this.AdditionalContoursByDistancesVisibility = Visibility.Collapsed;
                this.ContureByFieldStrengthVisibility = Visibility.Collapsed;
                this.SubscribersHeightVisibility = Visibility.Collapsed;
                this.PercentageTimeVisibility = Visibility.Collapsed;
                this.UseEffectiveHeightVisibility = Visibility.Visible;
                this.StepBetweenBoundaryPointsVisibility = Visibility.Visible;
            }
            else if (type == CalculationType.CreateContoursByDistance)
            {
                this.AzimuthStepVisibility = Visibility.Visible;
                this.AdditionalContoursByDistancesVisibility = Visibility.Visible;
                this.ContureByFieldStrengthVisibility = Visibility.Collapsed;
                this.SubscribersHeightVisibility = Visibility.Visible;
                this.PercentageTimeVisibility = Visibility.Visible;
                this.UseEffectiveHeightVisibility = Visibility.Visible;
                this.StepBetweenBoundaryPointsVisibility = Visibility.Visible;
            }
            else if (type == CalculationType.CreateContoursByFS)
            {
                this.AzimuthStepVisibility = Visibility.Visible;
                this.AdditionalContoursByDistancesVisibility = Visibility.Collapsed;
                this.ContureByFieldStrengthVisibility = Visibility.Visible;
                this.SubscribersHeightVisibility = Visibility.Collapsed;
                this.PercentageTimeVisibility = Visibility.Visible;
                this.UseEffectiveHeightVisibility = Visibility.Visible;
                this.StepBetweenBoundaryPointsVisibility = Visibility.Visible;
            }
            else if (type == CalculationType.FindAffectedADM)
            {
                this.AzimuthStepVisibility = Visibility.Visible;
                this.AdditionalContoursByDistancesVisibility = Visibility.Collapsed;
                this.ContureByFieldStrengthVisibility = Visibility.Collapsed;
                this.SubscribersHeightVisibility = Visibility.Collapsed;
                this.PercentageTimeVisibility = Visibility.Collapsed;
                this.UseEffectiveHeightVisibility = Visibility.Visible;
                this.StepBetweenBoundaryPointsVisibility = Visibility.Visible;
            }
            else
            {
                this.AzimuthStepVisibility = Visibility.Collapsed;
                this.AdditionalContoursByDistancesVisibility = Visibility.Collapsed;
                this.ContureByFieldStrengthVisibility = Visibility.Collapsed;
                this.SubscribersHeightVisibility = Visibility.Collapsed;
                this.PercentageTimeVisibility = Visibility.Collapsed;
                this.UseEffectiveHeightVisibility = Visibility.Collapsed;
                this.StepBetweenBoundaryPointsVisibility = Visibility.Collapsed;
                StartEnabled = false;
            }
        }
        private void OnChangedContext(IMQueryMenuNode.Context context)
        {
            this._assignmentsAllotmentsList.Clear();
            if (context != null)
            {
                if (context.TableName == "ge06_allot_terra")
                {
                    var allot = _objectReader.Read<AssignmentsAllotmentsModel>().By(new GetAllotmentByBrificId { Id = context.TableId });
                    if (allot != null)
                    {
                        this._assignmentsAllotmentsList.Add(allot);

                        if (!string.IsNullOrEmpty(allot.AdmRefId))
                        {
                            var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAssigmentsByAdmAllotId { Adm_Allot_Id = allot.AdmRefId });
                            if (allotsIcsm != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(allotsIcsm);
                            }

                            var assignBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByAdmAllotId { Adm_Allot_Id = allot.AdmRefId });
                            if (assignBrific != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(assignBrific);
                            }
                        }

                        if (!string.IsNullOrEmpty(allot.SfnId))
                        {
                            var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAllotmentsBySfnId { SfnId = allot.SfnId });
                            if (allotsIcsm != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(allotsIcsm);
                            }
                        }
                    }
                }

                if (context.TableName == "fmtv_terra")
                {
                    var assign = _objectReader.Read<AssignmentsAllotmentsModel>().By(new GetAssignmentByBrificId { Id = context.TableId });
                    if (assign != null)
                    {
                        this._assignmentsAllotmentsList.Add(assign);

                        if (!string.IsNullOrEmpty(assign.AdmAllotAssociatedId))
                        {
                            var allotsBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByAdmRefId { Adm_Ref_Id = assign.AdmAllotAssociatedId });
                            if (allotsBrific != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(allotsBrific);
                            }
                            var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAllotmentsByAdmRefId { Adm_Ref_Id = assign.AdmRefId });
                            if (allotsIcsm != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(allotsIcsm);
                            }
                        }

                        if (!string.IsNullOrEmpty(assign.SfnId))
                        {
                            var assignIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAssigmentsBySfnId { SfnId = assign.SfnId });
                            if (assignIcsm != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(assignIcsm);
                            }
                        }

                        if (!string.IsNullOrEmpty(assign.SfnId))
                        {
                            var assignBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsBySfnId { SfnId = assign.SfnId });
                            if (assignBrific != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(assignBrific);
                            }
                        }
                    }
                }

                if (context.TableName == "FMTV_ASSIGN")
                {
                    var allotAssign = _objectReader.Read<AssignmentsAllotmentsModel>().By(new GetAssignmentAllotmentByIcsmId { Id = context.TableId });
                    if (allotAssign != null)
                    {
                        this._assignmentsAllotmentsList.Add(allotAssign);

                        if (allotAssign.Type == AssignmentsAllotmentsModelType.Assignment)
                        {
                            if (!string.IsNullOrEmpty(allotAssign.AdmAllotAssociatedId))
                            {
                                var allotsBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByAdmRefId { Adm_Ref_Id = allotAssign.AdmAllotAssociatedId });
                                if (allotsBrific != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(allotsBrific);
                                }
                                foreach (var item in allotsBrific)
                                {
                                    if (!string.IsNullOrEmpty(item.AdmRefId))
                                    {
                                        var assignBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByAdmAllotId { Adm_Allot_Id = item.AdmRefId });
                                        if (assignBrific != null)
                                        {
                                            this._assignmentsAllotmentsList.AddRange(assignBrific);
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(allotAssign.AdmRefId))
                            {
                                var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAllotmentsByAdmRefId { Adm_Ref_Id = allotAssign.AdmRefId });
                                if (allotsIcsm != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(allotsIcsm);
                                }
                            }

                            if (!string.IsNullOrEmpty(allotAssign.SfnId))
                            {
                                var assignIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAssigmentsBySfnId { SfnId = allotAssign.SfnId });
                                if (assignIcsm != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(assignIcsm);
                                }

                                var assignBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsBySfnId { SfnId = allotAssign.SfnId });
                                if (assignBrific != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(assignBrific);
                                }

                                foreach (var item in assignBrific)
                                {
                                    if (!string.IsNullOrEmpty(item.AdmAllotAssociatedId))
                                    {
                                        var allotsBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByAdmRefId { Adm_Ref_Id = item.AdmAllotAssociatedId });
                                        if (allotsBrific != null)
                                        {
                                            this._assignmentsAllotmentsList.AddRange(allotsBrific);
                                        }
                                    }
                                }
                            }

                            var assignBrificTarget = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByTarget { target = new BroadcastingAssignmentTarget() { AdmRefId = allotAssign.TargetAdmRefId, Freq_MHz = allotAssign.TargetFreq_MHz, Lat_Dec = allotAssign.TargetLat_Dec, Lon_Dec = allotAssign.TargetLon_Dec } });
                            if (assignBrificTarget != null)
                            {
                                this._assignmentsAllotmentsList.AddRange(assignBrificTarget);
                            }
                            foreach (var item in assignBrificTarget)
                            {
                                if (!string.IsNullOrEmpty(item.AdmAllotAssociatedId))
                                {
                                    var allotsBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByAdmRefId { Adm_Ref_Id = item.AdmAllotAssociatedId });
                                    if (allotsBrific != null)
                                    {
                                        this._assignmentsAllotmentsList.AddRange(allotsBrific);
                                    }
                                }
                            }
                        }
                        if (allotAssign.Type == AssignmentsAllotmentsModelType.Allotment)
                        {
                            if (!string.IsNullOrEmpty(allotAssign.AdmRefId))
                            {
                                var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAssigmentsByAdmAllotId { Adm_Allot_Id = allotAssign.AdmRefId });
                                if (allotsIcsm != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(allotsIcsm);
                                }

                                var assignBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByAdmAllotId { Adm_Allot_Id = allotAssign.AdmRefId });
                                if (assignBrific != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(assignBrific);
                                }
                                foreach (var item in assignBrific)
                                {
                                    if (!string.IsNullOrEmpty(item.AdmAllotAssociatedId))
                                    {
                                        var allotsBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByAdmRefId { Adm_Ref_Id = item.AdmAllotAssociatedId });
                                        if (allotsBrific != null)
                                        {
                                            this._assignmentsAllotmentsList.AddRange(allotsBrific);
                                        }
                                    }
                                }

                                var allotsIcsmTarget = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAllotmentsByTarget { PlanAssignNo = allotAssign.PlanAssgnNo });
                                if (allotsIcsmTarget != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(allotsIcsmTarget);
                                }
                                foreach (var item in allotsIcsmTarget)
                                {
                                    if (!string.IsNullOrEmpty(item.AdmRefId))
                                    {
                                        var assignBrificTarget = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByAdmAllotId { Adm_Allot_Id = item.AdmRefId });
                                        if (assignBrificTarget != null)
                                        {
                                            this._assignmentsAllotmentsList.AddRange(assignBrificTarget);
                                        }
                                    }
                                }

                                var allotsBrificTarget = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByTarget { PlanAssignNo = allotAssign.PlanAssgnNo });
                                if (allotsBrificTarget != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(allotsBrificTarget);
                                }
                                foreach (var item in allotsBrificTarget)
                                {
                                    if (!string.IsNullOrEmpty(item.AdmRefId))
                                    {
                                        var assignBrificTarget = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByAdmAllotId { Adm_Allot_Id = item.AdmRefId });
                                        if (assignBrificTarget != null)
                                        {
                                            this._assignmentsAllotmentsList.AddRange(assignBrificTarget);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                this._assignmentsAllotmentsList = this._assignmentsAllotmentsList.GroupBy(x => x.Id).Select(group => group.First()).ToList();
                this.AssignmentsAllotmentsArray = _assignmentsAllotmentsList.ToArray();
                UpdateEnableButoonState();
            }
        }
        private void OnAllotDeleteCommand(object parameter)
        {
            try
            {
                if (this._currentAssignmentsAllotments != null)
                {
                    foreach (AssignmentsAllotmentsModel item in this._currentAssignmentsAllotments)
                    {
                        this._assignmentsAllotmentsList.Remove(item);
                    }
                    this.AssignmentsAllotmentsArray = this._assignmentsAllotmentsList.ToArray();
                    UpdateEnableButoonState();
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
            }
        }
        private void OnStartCommand(object parameter)
        {
            if (this._activeContextId == 0)
            {
                _starter.ShowException("Warning!", new Exception($"Undefined Active context"));
                return;
            }

            if (this.AssignmentsAllotmentsArray.Where(c => c.Source == AssignmentsAllotmentsSourceType.Brific && c.Type == AssignmentsAllotmentsModelType.Allotment).Count() > 1
                || this.AssignmentsAllotmentsArray.Where(c => c.Source == AssignmentsAllotmentsSourceType.ICSM && c.Type == AssignmentsAllotmentsModelType.Allotment).Count() > 1)
            {
                _starter.ShowException("Warning!", new Exception($"The table cannot have more than one Allotment"));
                return;
            }
            var projectMap = _objectReader.Read<ProjectMapsModel>().By(new GetProjectMapByClientContextId { ContextId = this._activeContextId });

            var splitVariants = new char[] { ',', ';', ' ' };
            var distances = new List<int>();
            foreach (var item in CurrentCalcTaskCard.DistancesString.Split(splitVariants))
            {
                if (Int32.TryParse(item, out int value))
                    distances.Add(value);
            }
            CurrentCalcTaskCard.Distances = distances.ToArray();

            var fieldStrength = new List<int>();
            foreach (var item in CurrentCalcTaskCard.FieldStrengthString.Split(splitVariants))
            {
                if (Int32.TryParse(item, out int value))
                    fieldStrength.Add(value);

            }
            CurrentCalcTaskCard.Distances = distances.ToArray();
            CurrentCalcTaskCard.FieldStrength = fieldStrength.ToArray();

            if (CurrentCalcTaskCard.StepBetweenBoundaryPointsDefault)
                CurrentCalcTaskCard.StepBetweenBoundaryPoints = 100;

            if (this._calcType == CalculationType.CreateContoursByDistance)
            {
                CurrentCalcTaskCard.AdditionalContoursByDistances = true;
                CurrentCalcTaskCard.ContureByFieldStrength = false;
            }
            else if (this._calcType == CalculationType.CreateContoursByFS)
            {
                CurrentCalcTaskCard.AdditionalContoursByDistances = false;
                CurrentCalcTaskCard.ContureByFieldStrength = true;
            }
            else
            {
                CurrentCalcTaskCard.AdditionalContoursByDistances = false;
                CurrentCalcTaskCard.ContureByFieldStrength = false;
            }

            if (!ValidateData())
                return;

            var modifier = new Modifiers.CreateCalcTask
            {
                MapName = projectMap != null ? projectMap.MapName : "",
                AzimuthStep_deg = CurrentCalcTaskCard.AzimuthStep_deg,
                AdditionalContoursByDistances = CurrentCalcTaskCard.AdditionalContoursByDistances,
                Distances = CurrentCalcTaskCard.Distances,
                ContureByFieldStrength = CurrentCalcTaskCard.ContureByFieldStrength,
                FieldStrength = CurrentCalcTaskCard.FieldStrength,
                SubscribersHeight = CurrentCalcTaskCard.SubscribersHeight,
                PercentageTime = CurrentCalcTaskCard.PercentageTime,
                UseEffectiveHeight = CurrentCalcTaskCard.UseEffectiveHeight,
                StepBetweenBoundaryPoints = CurrentCalcTaskCard.StepBetweenBoundaryPoints,
                CalculationTypeCode = (byte)this._calcType,
                CalculationTypeName = this._calcType.ToString(),
                BroadcastingExtend = GetBroadcastingContext(),
                ContextId = this._activeContextId,
                OwnerId = Guid.NewGuid()
            };
            _commandDispatcher.Send(modifier);
        }
        private bool ValidateData()
        {
            if (CurrentCalcTaskCard.AzimuthStep_deg < 1 || CurrentCalcTaskCard.AzimuthStep_deg > 60)
            {
                _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.AzimuthStep}'"));
                return false;
            }
            if (CurrentCalcTaskCard.SubscribersHeight < 1 || CurrentCalcTaskCard.SubscribersHeight > 30)
            {
                _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.SubscribersHeight}'"));
                return false;
            }
            if (CurrentCalcTaskCard.PercentageTime < 0.1 || CurrentCalcTaskCard.PercentageTime > 50)
            {
                _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.PercentageTime}'"));
                return false;
            }
            if (CurrentCalcTaskCard.StepBetweenBoundaryPoints < 1 || CurrentCalcTaskCard.StepBetweenBoundaryPoints > 100)
            {
                _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.StepBetweenBoundaryPoints}'"));
                return false;
            }

            if (this._calcType == CalculationType.CreateContoursByDistance)
            {
                if (CurrentCalcTaskCard.Distances == null || CurrentCalcTaskCard.Distances.Length == 0)
                {
                    _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.AdditionalContoursByDistances}'"));
                    return false;
                }
                foreach (var item in CurrentCalcTaskCard.Distances)
                {
                    if (item < 30 || item > 1000)
                    {
                        _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.AdditionalContoursByDistances}'"));
                        return false;
                    }
                }
            }

            if (this._calcType == CalculationType.CreateContoursByFS)
            {
                if (CurrentCalcTaskCard.FieldStrength == null || CurrentCalcTaskCard.FieldStrength.Length == 0)
                {
                    _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.ContureByFieldStrength}'"));
                    return false;
                }
                foreach (var item in CurrentCalcTaskCard.FieldStrength)
                {
                    if (item < -2 || item > 100)
                    {
                        _starter.ShowException("Warning!", new Exception($"Incorrect value '{Properties.Resources.ContureByFieldStrength}'"));
                        return false;
                    }
                }
            }

            return true;
        }

        private void OnCreatedCalcTaskHandle(Events.OnCreatedCalcTask data)
        {
            var modifier = new Modifiers.RunCalcTask
            {
                Id = data.CalcTaskId
            };
            _commandDispatcher.Send(modifier);
        }
        private void OnRunedCalcTaskHandle(Events.OnRunedCalcTask data)
        {
            _objectReader.Read<byte?>().By(new GetResultStatusById { ResultId = data.Id });

            var resultId = _objectReader.Read<long?>().By(new ST.Queries.GetResultIdByTaskId { TaskId = data.Id });
            if (resultId.HasValue)
            {
                if (WaitForCalcResult(data.Id, resultId.Value))
                {
                    var ge06resultId = _objectReader.Read<long?>().By(new ST.Queries.GetGe06ResultIdByResultId { ResultId = resultId.Value });
                    if (ge06resultId.HasValue)
                    {
                        _starter.Start<VM.GE06TaskResult.View>(isModal: true, c => c.ResultId = ge06resultId.Value);
                    }
                    else
                    {
                        this._logger.Exception(Exceptions.GE06Client, new Exception($"For selected task not found information in IGn06Result!"));
                        _starter.ShowException("Warning!", new Exception($"For selected task not found information in IGn06Result!"));
                    }
                }
            }
            else
            {
                this._logger.Exception(Exceptions.GE06Client, new Exception($"For selected task not found information in ICalcResults!"));
                _starter.ShowException("Warning!", new Exception($"For selected task not found information in ICalcResults!"));
            }
        }
        private bool WaitForCalcResult(long calcTaskId, long calcResultId)
        {
            bool result = false;
            _starter.StartLongProcess(
                new LongProcessOptions()
                {
                    CanStop = false,
                    CanAbort = true,
                    UseProgressBar = true,
                    UseLog = true,
                    IsModal = true,
                    MinValue = 0,
                    MaxValue = 1000,
                    ValueKind = LongProcessValueKind.Infinity,
                    Title = "Calculating task ...",
                    Note = "Please control the log processes below."
                },
                token =>
                {
                    var cancel = false;
                    long eventId = 0;

                    while (!cancel)
                    {
                        var status = _objectReader.Read<byte?>().By(new GetResultStatusById { ResultId = calcResultId });

                        if (status.HasValue)
                        {
                            if (status == (byte)CalcResultStatusCode.Completed)
                            {
                                result = true;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }

                            if (status == (byte)CalcResultStatusCode.Failed)
                            {
                                _starter.ShowException("Warning!", new Exception($"Task calculation completed with status '{CalcResultStatusCode.Failed.ToString()}'!"));
                                this._logger.Exception(Exceptions.GE06Client, new Exception($"Task calculation completed with status '{CalcResultStatusCode.Failed.ToString()}'!"));
                                result = false;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }
                            if (status == (byte)CalcResultStatusCode.Aborted)
                            {
                                _starter.ShowException("Warning!", new Exception($"Task calculation completed with status '{CalcResultStatusCode.Aborted.ToString()}'!"));
                                this._logger.Exception(Exceptions.GE06Client, new Exception($"Task calculation completed with status '{CalcResultStatusCode.Aborted.ToString()}'!"));
                                result = false;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }
                            if (status == (byte)CalcResultStatusCode.Canceled)
                            {
                                _starter.ShowException("Warning!", new Exception($"Task calculation completed with status '{CalcResultStatusCode.Canceled.ToString()}'!"));
                                this._logger.Exception(Exceptions.GE06Client, new Exception($"Task calculation completed with status '{CalcResultStatusCode.Canceled.ToString()}'!"));
                                result = false;
                                cancel = true;
                                _eventBus.Send(new LongProcessFinishEvent { ProcessToken = token });
                            }
                        }

                        var events = _objectReader.Read<CalcResultEventsModel[]>().By(new GetResultEventsByEventIdAndResultId { ResultId = calcResultId, EventId = eventId });
                        foreach (var item in events)
                        {
                            eventId = item.Id;
                            var message = item.Message;

                            if (item.State != null)
                                message = $"{item.Message}: {item.State.State.ToString()}%";

                            _eventBus.Send(new LongProcessLogEvent
                            {
                                ProcessToken = token,
                                Message = message
                            });

                            if (item.LevelCode == 2)
                            {
                                _starter.ShowException("Error!", new Exception(item.Message));
                                result = false;
                                cancel = true;
                            }

                        }
                        System.Threading.Thread.Sleep(5 * 1000);

                        token.AbortToken.ThrowIfCancellationRequested();
                    }
                    //Created = 0, // Фаза создания и подготовки окружения к запуску процесса расчета
                    //Pending = 1, // Фаза ожидания запуска процесса расчета
                    //Accepted = 2, // Фаза ожидания запуска процесса расчета
                    //Processing = 3, // Расчет выполняется
                    //Completed = 4, // Расчет завершен
                    //Canceled = 5, // Расчет был отменен по внешней причине
                    //Aborted = 6, // Расчет был прерван по внутреней причине
                    //Failed = 7  // Попытка запуска завершилась не удачей
                });
            return result;
        }

        private BroadcastingContext GetBroadcastingContext()
        {
            var allotmentBrific = new BroadcastingAllotment();
            var allotmentIcsm = new BroadcastingAllotment();
            var assignmentsBrific = new List<BroadcastingAssignment>();
            var assignmentsIcsm = new List<BroadcastingAssignment>();

            foreach (var item in this.AssignmentsAllotmentsArray)
            {
                if (item.Type == AssignmentsAllotmentsModelType.Allotment)
                {
                    var allotment = new BroadcastingAllotment()
                    {
                        AdminData = new AdministrativeData()
                        {
                            Adm = item.Adm,
                            NoticeType = item.NoticeType,
                            Fragment = item.Fragment,
                            Action = item.Action,
                            AdmRefId = item.AdmRefId,
                            IsDigital = item.IsDigital,
                            StnClass = item.StnClass
                        },
                        Target = new BroadcastingAssignmentTarget()
                        {
                            AdmRefId = item.AdmRefId,
                            Freq_MHz = item.Freq_MHz,
                            Lon_Dec = item.Lon_Dec,
                            Lat_Dec = item.Lat_Dec
                        },
                        EmissionCharacteristics = new BroadcastingAllotmentEmissionCharacteristics()
                        {
                            Freq_MHz = item.Freq_MHz,
                            Polar = item.Polar,
                            RefNetwork = item.RefNetwork,
                            RefNetworkConfig = item.RefNetworkConfig,
                            SpectrumMask = item.SpectrumMask
                        },
                        AllotmentParameters = new AllotmentParameters()
                        {
                            Name = item.Name,
                            ContourId = item.ContourId,
                            Contur = item.Contur
                        },
                        DigitalPlanEntryParameters = new DigitalPlanEntryParameters()
                        {
                            PlanEntry = item.PlanEntry,
                            AssignmentCode = item.AssignmentCode,
                            AdmAllotAssociatedId = item.AdmAllotAssociatedId,
                            SfnAllotAssociatedId = item.SfnAllotAssociatedId,
                            SfnId = item.SfnId
                        }
                    };
                    if (item.Source == AssignmentsAllotmentsSourceType.Brific)
                        allotmentBrific = allotment;
                    else if (item.Source == AssignmentsAllotmentsSourceType.ICSM)
                        allotmentIcsm = allotment;
                }
                if (item.Type == AssignmentsAllotmentsModelType.Assignment)
                {
                    var assignment = new BroadcastingAssignment()
                    {
                        AdmData = new AdministrativeData()
                        {
                            Adm = item.Adm,
                            NoticeType = item.NoticeType,
                            Fragment = item.Fragment,
                            Action = item.Action,
                            AdmRefId = item.AdmRefId,
                            IsDigital = item.IsDigital,
                            StnClass = item.StnClass
                        },
                        Target = new BroadcastingAssignmentTarget()
                        {
                            AdmRefId = item.AdmRefId,
                            Freq_MHz = item.Freq_MHz,
                            Lon_Dec = item.Lon_Dec,
                            Lat_Dec = item.Lat_Dec
                        },
                        EmissionCharacteristics = new BroadcastingAssignmentEmissionCharacteristics()
                        {
                            Freq_MHz = item.Freq_MHz,
                            Polar = item.Polar,
                            ErpH_dBW =  item.ErpH_dBW,
                            ErpV_dBW = item.ErpV_dBW,
                            RxMode = item.RxMode,
                            SystemVariation = item.SystemVariation,
                            RefNetworkConfig = item.RefNetworkConfig,
                            SpectrumMask = item.SpectrumMask
                        },
                        SiteParameters = new SiteParameters()
                        {
                            Alt_m = item.Alt_m,
                            Lat_Dec = item.Lat_Dec,
                            Lon_Dec = item.Lon_Dec,
                            Name = item.Name
                        },
                        AntennaCharacteristics = new AntennaCharacteristics()
                        {
                            Direction = item.Direction,
                            AglHeight_m = item.AglHeight_m,
                            DiagrH = item.DiagrH,
                            DiagrV = item.DiagrV,
                            EffHeight_m = item.EffHeight_m,
                            MaxEffHeight_m = item.MaxEffHeight_m
                        },
                        DigitalPlanEntryParameters = new DigitalPlanEntryParameters()
                        {
                            PlanEntry = item.PlanEntry,
                            AssignmentCode = item.AssignmentCode,
                            AdmAllotAssociatedId = item.AdmAllotAssociatedId,
                            SfnAllotAssociatedId = item.SfnAllotAssociatedId,
                            SfnId = item.SfnId
                        }
                    };

                    if (item.Source == AssignmentsAllotmentsSourceType.Brific)
                        assignmentsBrific.Add(assignment);
                    else if (item.Source == AssignmentsAllotmentsSourceType.ICSM)
                        assignmentsIcsm.Add(assignment);
                }
            }
            return new BroadcastingContext()
            {
                broadcastingContextBRIFIC = new BroadcastingContextBase() { Allotments = allotmentBrific, Assignments = assignmentsBrific.ToArray() },
                BroadcastingContextICSM = new BroadcastingContextBase() { Allotments = allotmentIcsm, Assignments = assignmentsIcsm.ToArray() }
            };
        }
        private void UpdateEnableButoonState()
        {
            bool isHaveIcsmObject = false;
            bool isHaveBrificObject = false;
            foreach (var item in this.AssignmentsAllotmentsArray)
            {
                if (item.Source == AssignmentsAllotmentsSourceType.ICSM)
                    isHaveIcsmObject = true;

                if (item.Source == AssignmentsAllotmentsSourceType.Brific)
                    isHaveBrificObject = true;

                if (isHaveIcsmObject && isHaveBrificObject)
                    break;
            }

            var enumData = new List<OrmEnumBoxData>();
            if (isHaveIcsmObject && isHaveBrificObject)
                enumData.Add(new OrmEnumBoxData() { Id = 1, Name = "ConformityCheck", ViewName = Properties.Resources.ConformityCheck });
            else if (this.CalcType == CalculationType.ConformityCheck)
                OnChangedCalcType(0);

            if (isHaveIcsmObject)
                enumData.Add(new OrmEnumBoxData() { Id = 2, Name = "FindAffectedADM", ViewName = Properties.Resources.FindAffected });
            else if (this.CalcType == CalculationType.FindAffectedADM)
                OnChangedCalcType(0);

            enumData.Add(new OrmEnumBoxData() { Id = 3, Name = "CreateContoursByDistance", ViewName = Properties.Resources.CreateContoursByDistance });
            enumData.Add(new OrmEnumBoxData() { Id = 4, Name = "CreateContoursByFS", ViewName = Properties.Resources.CreateContoursByFS });
            CalcTypeSource = enumData.ToArray();
        }
        private void RedrawMap()
        {
            var data = new MapDrawingData();
            var polygons = new List<MapDrawingDataPolygon>();
            var points = new List<MapDrawingDataPoint>();

            if (this._currentAssignmentsAllotments != null)
            {
                foreach (AssignmentsAllotmentsModel item in this._currentAssignmentsAllotments)
                {
                    if (item.Type == AssignmentsAllotmentsModelType.Assignment)
                    {
                        points.Add(Environment.MapsDrawingHelper.MakeDrawingPointForSensor(item.Lon_Dec, item.Lat_Dec));
                    }
                    if (item.Type == AssignmentsAllotmentsModelType.Allotment)
                    {
                        var polygonPoints = new List<Location>();

                        item.Contur.ToList().ForEach(areaPoint =>
                        {
                            polygonPoints.Add(new Location() { Lat = areaPoint.Lat_DEC, Lon = areaPoint.Lon_DEC });
                        });

                        polygons.Add(new MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
                    }
                }
            }

            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();

            this.CurrentMapData = data;
        }

        public override void Dispose()
        {
            _onCreatedCalcTaskToken?.Dispose();
            _onCreatedCalcTaskToken = null;
            _onRunedCalcTaskToken?.Dispose();
            _onRunedCalcTaskToken = null;
        }
    }
}
