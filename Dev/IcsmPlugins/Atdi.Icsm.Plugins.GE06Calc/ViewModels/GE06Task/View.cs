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
using Atdi.Icsm.Plugins.GE06Calc.Environment;

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
        private bool _conformityCheckEnabled = false;
        private bool _findAffectedEnabled = false;

        List<AssignmentsAllotmentsModel> _assignmentsAllotmentsList;
        AssignmentsAllotmentsModel[] _assignmentsAllotmentsArray;

        public ViewCommand AllotDeleteCommand { get; set; }
        public ViewCommand ConformityCheckCommand { get; set; }
        public ViewCommand FindAffectedCommand { get; set; }
        public ViewCommand CreateContoursByDistanceCommand { get; set; }
        public ViewCommand CreateContoursByFSCommand { get; set; }

        private MapDrawingData _currentMapData;

        private IEventHandlerToken<Events.OnCreatedCalcTask> _onCreatedCalcTaskToken;
        public View(
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

            this.AllotDeleteCommand = new ViewCommand(this.OnAllotDeleteCommand);
            this.ConformityCheckCommand = new ViewCommand(this.OnConformityCheckCommand);
            this.FindAffectedCommand = new ViewCommand(this.OnFindAffectedCommand);
            this.CreateContoursByDistanceCommand = new ViewCommand(this.OnCreateContoursByDistanceCommand);
            this.CreateContoursByFSCommand = new ViewCommand(this.OnCreateContoursByFSCommand);
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
                UseEffectiveHeight = true
            };

            _onCreatedCalcTaskToken = _eventBus.Subscribe<Events.OnCreatedCalcTask>(this.OnCreatedCalcTaskHandle);
        }
        public CalcTaskModel CurrentCalcTaskCard
        {
            get => this._currentCalcTaskCard;
            set => this.Set(ref this._currentCalcTaskCard, value);
        }
        public IList CurrentAssignmentsAllotments
        {
            get => this._currentAssignmentsAllotments;
            //set => this.Set(ref this._currentAssignmentsAllotments, value, this.RedrawMap);
            set
            {
                this._currentAssignmentsAllotments = value;
                RedrawMap();
            }
        }

        public bool ConformityCheckEnabled
        {
            get => this._conformityCheckEnabled;
            set => this.Set(ref this._conformityCheckEnabled, value);
        }
        public bool FindAffectedEnabled
        {
            get => this._findAffectedEnabled;
            set => this.Set(ref this._findAffectedEnabled, value);
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
                            var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAllotmentsByAdmRefId { Adm_Ref_Id = assign.AdmAllotAssociatedId });
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

                                var allotsIcsm = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetIcsmAllotmentsByAdmRefId { Adm_Ref_Id = allotAssign.AdmAllotAssociatedId });
                                if (allotsIcsm != null)
                                {
                                    this._assignmentsAllotmentsList.AddRange(allotsIcsm);
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
                            }
                        }
                    }
                    var assignBrificTarget = _objectReader.Read<List<AssignmentsAllotmentsModel>>().By(new GetBrificAssigmentsByTarget { target = new BroadcastingAssignmentTarget() { AdmRefId = allotAssign.TargetAdmRefId, Freq_MHz = allotAssign.TargetFreq_MHz, Lat_Dec = allotAssign.TargetLat_Dec, Lon_Dec = allotAssign.TargetLon_Dec } });
                    if (assignBrificTarget != null)
                    {
                        this._assignmentsAllotmentsList.AddRange(assignBrificTarget);
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
        private void OnConformityCheckCommand(object parameter)
        {
            try
            {
                CreateCalcTask(CalculationType.ConformityCheck);
                _starter.Stop(this);
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
                _starter.Stop(this);
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
                _starter.Stop(this);
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
                _starter.Stop(this);
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

            if (this.AssignmentsAllotmentsArray.Where(c => c.Source == AssignmentsAllotmentsSourceType.Brific && c.Type == AssignmentsAllotmentsModelType.Allotment).Count() > 1
                || this.AssignmentsAllotmentsArray.Where(c => c.Source == AssignmentsAllotmentsSourceType.ICSM && c.Type == AssignmentsAllotmentsModelType.Allotment).Count() > 1)
            {
                _starter.ShowException("Warning!", new Exception($"Еhe table cannot have more than one Allotment"));
                return;
            }

            var projectMap  = _objectReader.Read<ProjectMapsModel>().By(new GetProjectMapByClientContextId { ContextId = Properties.Settings.Default.ActiveContext });

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
                MapName = projectMap.MapName,
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
            ConformityCheckEnabled = (isHaveIcsmObject && isHaveBrificObject);
            FindAffectedEnabled = isHaveIcsmObject;
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
                        points.Add(MapsDrawingHelper.MakeDrawingPointForSensor(item.Lon_Dec, item.Lat_Dec));
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
        }
    }
}
