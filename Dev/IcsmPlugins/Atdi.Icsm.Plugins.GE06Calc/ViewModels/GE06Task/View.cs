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
        private IMQueryMenuNode.Context _context;

        AssignmentsAllotmentsModel[] _assignmentsAllotmentsList;

        public ViewCommand ConformityCheckCommand { get; set; }
        public ViewCommand FindAffectedCommand { get; set; }
        public ViewCommand CreateContoursByDistanceCommand { get; set; }
        public ViewCommand CreateContoursByFSCommand { get; set; }

        private IEventHandlerToken<Events.OnCreatedCalcTask> _onCreatedCalcTaskToken;
        public View(
            //ProjectDataAdapter projectDataAdapter,
            //BaseClientContextDataAdapter baseContextDataAdapter,
            //ClientContextDataAdapter contextDataAdapter,
            //CalcTaskDataAdapter calcTaskDataAdapter,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;

            this.ConformityCheckCommand = new ViewCommand(this.OnConformityCheckCommand);
            this.FindAffectedCommand = new ViewCommand(this.OnFindAffectedCommand);
            this.CreateContoursByDistanceCommand = new ViewCommand(this.OnCreateContoursByDistanceCommand);
            this.CreateContoursByFSCommand = new ViewCommand(this.OnCreateContoursByFSCommand);

            //this.TaskDeleteCommand = new ViewCommand(this.OnTaskDeleteCommand);
            //this.TaskShowResultCommand = new ViewCommand(this.OnTaskShowResultCommand);

            this.CurrentCalcTaskCard = new CalcTaskModel()
            {
                AzimuthStep_deg = 5,
                AdditionalContoursByDistances = false,
                DistancesString = "50,80,150",
                ContureByFieldStrength = false,
                FieldStrengthString = "12,25",
                SubscribersHeight = 10,
                PercentageTime = 50,
                UseEffectiveHeight = true
            };

            _onCreatedCalcTaskToken = _eventBus.Subscribe<Events.OnCreatedCalcTask>(this.OnCreatedCalcTaskHandle);

            //this.Projects = projectDataAdapter;
            //this.BaseClientContexts = baseContextDataAdapter;
            //this.ClientContexts = contextDataAdapter;
            //this.CalcTasks = calcTaskDataAdapter;

            //_onCreatedClientContextToken = _eventBus.Subscribe<Events.OnCreatedClientContext>(this.OnCreatedClientContextHandle);
            //_onEditedClientContextToken = _eventBus.Subscribe<Events.OnEditedClientContext>(this.OnEditedClientContextHandle);
            //_onDeletedClientContextToken = _eventBus.Subscribe<Events.OnDeletedClientContext>(this.OnDeletedClientContextHandle);
            //_onDeletedCalcTaskToken = _eventBus.Subscribe<CT.Events.OnDeletedCalcTask>(this.OnDeletedCalcTaskHandle);

            //ReloadProjects();
        }

        public CalcTaskModel CurrentCalcTaskCard
        {
            get => this._currentCalcTaskCard;
            set => this.Set(ref this._currentCalcTaskCard, value);
        }
        public IMQueryMenuNode.Context Context
        {
            get => this._context;
            set => this.Set(ref this._context, value, () => { this.OnChangedContext(value); });
        }
        public AssignmentsAllotmentsModel[] AssignmentsAllotmentsList
        {
            get => this._assignmentsAllotmentsList;
            set => this.Set(ref this._assignmentsAllotmentsList, value);
        }
        private void OnChangedContext(IMQueryMenuNode.Context context)
        {
            var assignmentsAllotmentsList = new List<AssignmentsAllotmentsModel>();
            if (context != null)
            {
                if (context.TableName == "ge06_allot_terra")
                {
                    var allot = _objectReader.Read<AssignmentsAllotmentsModel>().By(new GetAllotmentByBrificId { Id = context.TableId });
                    if (allot != null)
                    {
                        assignmentsAllotmentsList.Add(allot);

                        if (!string.IsNullOrEmpty(allot.AdmRefId))
                        {
                            var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAssigmentsByAdmAllotId { Adm_Allot_Id = allot.AdmRefId });
                            if (allotsIcsm != null)
                            {
                                assignmentsAllotmentsList.AddRange(allotsIcsm);
                            }

                            var allotsBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByAdmAllotId { Adm_Allot_Id = allot.AdmRefId });
                            if (allotsBrific != null)
                            {
                                assignmentsAllotmentsList.AddRange(allotsBrific);
                            }
                        }
                    }
                }
                if (context.TableName == "fmtv_terra")
                {
                    var assign = _objectReader.Read<AssignmentsAllotmentsModel>().By(new GetAssignmentByBrificId { Id = context.TableId });
                    if (assign != null)
                    {
                        assignmentsAllotmentsList.Add(assign);

                        if (!string.IsNullOrEmpty(assign.AdmAllotAssociatedId))
                        {
                            var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByAdmRefId { Adm_Ref_Id = assign.AdmAllotAssociatedId });
                            if (allotsIcsm != null)
                            {
                                assignmentsAllotmentsList.AddRange(allotsIcsm);
                            }
                        }

                        if (!string.IsNullOrEmpty(assign.SfnId))
                        {
                            var assignIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAssigmentsBySfnId { SfnId = assign.SfnId });
                            if (assignIcsm != null)
                            {
                                assignmentsAllotmentsList.AddRange(assignIcsm);
                            }
                        }
                    }
                }
                if (context.TableName == "FMTV_ASSIGN")
                {
                    var assign = _objectReader.Read<AssignmentsAllotmentsModel>().By(new GetAssignmentByIcsmId { Id = context.TableId });
                    if (assign != null)
                    {
                        assignmentsAllotmentsList.Add(assign);

                        if (!string.IsNullOrEmpty(assign.AdmAllotAssociatedId))
                        {
                            var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAllotmentsByAdmRefId { Adm_Ref_Id = assign.AdmAllotAssociatedId });
                            if (allotsIcsm != null)
                            {
                                assignmentsAllotmentsList.AddRange(allotsIcsm);
                            }
                        }

                        if (!string.IsNullOrEmpty(assign.SfnId))
                        {
                            var assignBrific = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsBySfnId { SfnId = assign.SfnId });
                            if (assignBrific != null)
                            {
                                assignmentsAllotmentsList.AddRange(assignBrific);
                            }
                        }
                    }
                    // need add recursion
                }
            }
            if (assignmentsAllotmentsList.Count > 0)
                this.AssignmentsAllotmentsList = assignmentsAllotmentsList.ToArray();
        }

        private void OnConformityCheckCommand(object parameter)
        {
            try
            {
                CreateCalcTask(CalculationType.ConformityCheck);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
            }
        }
        private void OnFindAffectedCommand(object parameter)
        {
            try
            {
                CreateCalcTask(CalculationType.FindAffectedADM);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
            }
        }
        private void OnCreateContoursByDistanceCommand(object parameter)
        {
            try
            {
                CreateCalcTask(CalculationType.CreateContoursByDistance);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
            }
        }
        private void OnCreateContoursByFSCommand(object parameter)
        {
            try
            {
                CreateCalcTask(CalculationType.CreateContoursByFS);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
            }
        }

        private void CreateCalcTask(CalculationType calcType)
        {
            if (Properties.Settings.Default.ActiveContext == 0)
            {
                _starter.ShowException("Warning!", new Exception($"Undefined Active context"));
                return;
            }

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

            var modifier = new Modifiers.CreateCalcTask
            {
                AzimuthStep_deg = CurrentCalcTaskCard.AzimuthStep_deg,
                AdditionalContoursByDistances = CurrentCalcTaskCard.AdditionalContoursByDistances,
                Distances = CurrentCalcTaskCard.Distances,
                ContureByFieldStrength = CurrentCalcTaskCard.ContureByFieldStrength,
                FieldStrength = CurrentCalcTaskCard.FieldStrength,
                SubscribersHeight = CurrentCalcTaskCard.SubscribersHeight,
                PercentageTime = CurrentCalcTaskCard.PercentageTime,
                UseEffectiveHeight = CurrentCalcTaskCard.UseEffectiveHeight,
                CalculationTypeCode = (byte)calcType,
                CalculationTypeName = calcType.ToString(),
                BroadcastingExtend = GetBroadcastingContext(),
                ContextId = Properties.Settings.Default.ActiveContext,
                OwnerId = Guid.NewGuid()
            };
            _commandDispatcher.Send(modifier);
        }
        private void OnCreatedCalcTaskHandle(Events.OnCreatedCalcTask data)
        {
            var modifier = new Modifiers.RunCalcTask
            {
                Id = data.CalcTaskId
            };
            _commandDispatcher.Send(modifier);
        }
        private BroadcastingContext GetBroadcastingContext()
        {
            var allotments = new List<BroadcastingAllotment>();
            var assignments = new List<BroadcastingAssignment>();

            foreach (var item in AssignmentsAllotmentsList)
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
                            AdmRefId = item.AdmRefId
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
                            Сontur = item.Сontur
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
                    allotments.Add(allotment);
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
                            AdmRefId = item.AdmRefId
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
                    assignments.Add(assignment);
                }
            }
            return new BroadcastingContext() { Allotments = allotments.ToArray(), Assignments = assignments.ToArray() };
        }
        public override void Dispose()
        {
            _onCreatedCalcTaskToken?.Dispose();
            _onCreatedCalcTaskToken = null;
        }
    }
}
