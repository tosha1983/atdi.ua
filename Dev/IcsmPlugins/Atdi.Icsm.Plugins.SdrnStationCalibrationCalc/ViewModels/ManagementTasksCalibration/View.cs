using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using VM = Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Queries;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration.Adapters;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Data;
using System.Windows;
using System.Windows.Forms;
using Atdi.WpfControls.EntityOrm.Controls;
using Microsoft.VisualBasic;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ManagementTasksCalibration
{
    [ViewXaml("ManagementTasksCalibration.xaml")]
    [ViewCaption("Management of tasks for calibration of parameters stations")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private ProjectModel _currentProject;
        private ClientContextModel _currentBaseClientContext;
        private ClientContextModel _currentClientContext;
        private ClientContextModel _currentClientContextCard;
        private CalcTaskModel _currentCalcTask;

        private CardEditMode _clientContextEditedMode = CardEditMode.None;

        public ViewCommand ContextAddCommand { get; set; }
        public ViewCommand ContextModifyCommand { get; set; }
        public ViewCommand ContextDeleteCommand { get; set; }
        public ViewCommand ContextSaveCommand { get; set; }
        public ViewCommand TaskAddCommand { get; set; }
        public ViewCommand TaskModifyCommand { get; set; }
        public ViewCommand TaskDeleteCommand { get; set; }
        public ViewCommand TaskStartCalcCommand { get; set; }
        public ViewCommand TaskShowResultCommand { get; set; }
        public ViewCommand TaskStopCommand { get; set; }
        public ViewCommand TaskAbortCommand { get; set; }
        public ViewCommand EditNoteCommand { get; set; }
        public ViewCommand TaskRefreshCommand { get; set; }

        public ProjectDataAdapter Projects { get; set; }
        public BaseClientContextDataAdapter BaseClientContexts { get; set; }
        public ClientContextDataAdapter ClientContexts { get; set; }
        public CalcTaskDataAdapter CalcTasks { get; set; }

        private IEventHandlerToken<Events.OnCreatedClientContext> _onCreatedClientContextToken;
        private IEventHandlerToken<Events.OnEditedClientContext> _onEditedClientContextToken;
        private IEventHandlerToken<Events.OnDeletedClientContext> _onDeletedClientContextToken;
        private IEventHandlerToken<Events.OnCreatedCalcTask> _onCreatedCalcTaskToken;
        private IEventHandlerToken<Events.OnEditedCalcTask> _onEditedCalcTaskToken;
        private IEventHandlerToken<Events.OnDeletedCalcTask> _onDeletedCalcTaskToken;
        private IEventHandlerToken<Events.OnRunCalcTask> _onOnRunCalcTaskToken;

        public View(
            ProjectDataAdapter projectDataAdapter,
            BaseClientContextDataAdapter baseContextDataAdapter,
            ClientContextDataAdapter contextDataAdapter,
            CalcTaskDataAdapter calcTaskDataAdapter,
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

            this.ContextAddCommand = new ViewCommand(this.OnContextAddCommand);
            this.ContextModifyCommand = new ViewCommand(this.OnContextModifyCommand);
            this.ContextDeleteCommand = new ViewCommand(this.OnContextDeleteCommand);
            this.ContextSaveCommand = new ViewCommand(this.OnContextSaveCommand);

            this.TaskAddCommand = new ViewCommand(this.OnTaskAddCommand);
            this.TaskModifyCommand = new ViewCommand(this.OnTaskModifyCommand);
            this.TaskDeleteCommand = new ViewCommand(this.OnTaskDeleteCommand);
            this.TaskStartCalcCommand = new ViewCommand(this.OnTaskStartCalcCommand);
            this.TaskShowResultCommand = new ViewCommand(this.OnTaskShowResultCommand);
            this.TaskStopCommand = new ViewCommand(this.OnTaskStopCommand);
            this.TaskAbortCommand = new ViewCommand(this.OnTaskAbortCommand);
            this.TaskRefreshCommand = new ViewCommand(this.OnTaskRefreshCommand);

            this.EditNoteCommand = new ViewCommand(this.OnEditNoteCommand);

            this.CurrentClientContextCard = new ClientContextModel();

            this.Projects = projectDataAdapter;
            this.BaseClientContexts = baseContextDataAdapter;
            this.ClientContexts = contextDataAdapter;
            this.CalcTasks = calcTaskDataAdapter;

            _onCreatedClientContextToken = _eventBus.Subscribe<Events.OnCreatedClientContext>(this.OnCreatedClientContextHandle);
            _onEditedClientContextToken = _eventBus.Subscribe<Events.OnEditedClientContext>(this.OnEditedClientContextHandle);
            _onDeletedClientContextToken = _eventBus.Subscribe<Events.OnDeletedClientContext>(this.OnDeletedClientContextHandle);
            _onCreatedCalcTaskToken = _eventBus.Subscribe<Events.OnCreatedCalcTask>(this.OnCreatedCalcTaskHandle);
            _onEditedCalcTaskToken = _eventBus.Subscribe<Events.OnEditedCalcTask>(this.OnEditedCalcTasktHandle);
            _onDeletedCalcTaskToken = _eventBus.Subscribe<Events.OnDeletedCalcTask>(this.OnDeletedCalcTaskHandle);
            _onOnRunCalcTaskToken = _eventBus.Subscribe<Events.OnRunCalcTask>(this.OnRunCalcTaskHandle);

            ReloadProjects();
            ReloadProjectContext();
            ReloadClientContext();
            ReloadCalcTask();
        }
        private void ReloadProjects()
        {
            this.Projects.Refresh();
            this.CurrentProject = null;
        }
        public ProjectModel CurrentProject
        {
            get => this._currentProject;
            set => this.Set(ref this._currentProject, value, () => { this.OnChangedCurrentProject(value); });
        }
        public ClientContextModel CurrentBaseClientContext
        {
            get => this._currentBaseClientContext;
            set => this.Set(ref this._currentBaseClientContext, value, () => { this.OnChangedCurrentBaseClientContext(value); });
        }
        public ClientContextModel CurrentClientContext
        {
            get => this._currentClientContext;
            set => this.Set(ref this._currentClientContext, value, () => { this.OnChangedCurrentClientContext(value); });
        }
        public ClientContextModel CurrentClientContextCard
        {
            get => this._currentClientContextCard;
            set => this.Set(ref this._currentClientContextCard, value);
        }
        public CalcTaskModel CurrentCalcTask
        {
            get => this._currentCalcTask;
            set => this.Set(ref this._currentCalcTask, value, () => { this.OnChangedCurrentCalcTask(value); });
        }

        #region visible properties
        private bool _clientContextAddEnabled = false;
        public bool ClientContextAddEnabled
        {
            get => this._clientContextAddEnabled;
            set => this.Set(ref this._clientContextAddEnabled, value);
        }

        private bool _clientContextEditEnabled = false;
        public bool ClientContextEditEnabled
        {
            get => this._clientContextEditEnabled;
            set => this.Set(ref this._clientContextEditEnabled, value);
        }

        private bool _clientContextDelEnabled = false;
        public bool ClientContextDelEnabled
        {
            get => this._clientContextDelEnabled;
            set => this.Set(ref this._clientContextDelEnabled, value);
        }

        private bool _clientContextSaveEnabled = false;
        public bool ClientContextSaveEnabled
        {
            get => this._clientContextSaveEnabled;
            set => this.Set(ref this._clientContextSaveEnabled, value);
        }

        private bool _calcTaskAddEnabled = false;
        public bool CalcTaskAddEnabled
        {
            get => this._calcTaskAddEnabled;
            set => this.Set(ref this._calcTaskAddEnabled, value);
        }

        private bool _calcTaskEditEnabled = false;
        public bool CalcTaskEditEnabled
        {
            get => this._calcTaskEditEnabled;
            set => this.Set(ref this._calcTaskEditEnabled, value);
        }

        private bool _calcTaskDelEnabled = false;
        public bool CalcTaskDelEnabled
        {
            get => this._calcTaskDelEnabled;
            set => this.Set(ref this._calcTaskDelEnabled, value);
        }

        private bool _calcTaskStartCalcEnabled = false;
        public bool CalcTaskStartCalcEnabled
        {
            get => this._calcTaskStartCalcEnabled;
            set => this.Set(ref this._calcTaskStartCalcEnabled, value);
        }

        private bool _calcTaskStopCalcEnabled = false;
        public bool CalcTaskStopCalcEnabled
        {
            get => this._calcTaskStopCalcEnabled;
            set => this.Set(ref this._calcTaskStopCalcEnabled, value);
        }

        private bool _calcTaskAbortCalcEnabled = false;
        public bool CalcTaskAbortCalcEnabled
        {
            get => this._calcTaskAbortCalcEnabled;
            set => this.Set(ref this._calcTaskAbortCalcEnabled, value);
        }

        private bool _calcTaskShowResultEnabled = false;
        public bool CalcTaskShowResultEnabled
        {
            get => this._calcTaskShowResultEnabled;
            set => this.Set(ref this._calcTaskShowResultEnabled, value);
        }
        #endregion

        private void OnChangedCurrentProject(ProjectModel project)
        {
            ReloadProjectContext();
            ReloadClientContext();
            ReloadCalcTask();
            CurrentClientContextCard = new ClientContextModel();
            ClientContextSaveEnabled = false;
            CalcTaskEditEnabled = false;
            CalcTaskDelEnabled = false;
            CalcTaskStartCalcEnabled = false;
            CalcTaskStopCalcEnabled = false;
            CalcTaskAbortCalcEnabled = false;
            CalcTaskShowResultEnabled = false;
        }
        private void OnChangedCurrentBaseClientContext(ClientContextModel context)
        {
            if (CurrentBaseClientContext != null)
            {
                ClientContextAddEnabled = true;
            }
            else
            {
                ClientContextAddEnabled = false;
            }
            ClientContextSaveEnabled = false;
        }
        private void OnChangedCurrentClientContext(ClientContextModel context)
        {
            if (CurrentClientContext != null)
            {
                ReloadCalcTask();
                this.CurrentClientContextCard = _objectReader.Read<ClientContextModel>().By(new GetClientContextById { Id = CurrentClientContext.Id });
                ClientContextEditEnabled = true;
                ClientContextDelEnabled = true;
                CalcTaskAddEnabled = true;
            }
            else
            {
                ClientContextEditEnabled = false;
                ClientContextDelEnabled = false;
                CalcTaskAddEnabled = false;

            }
            ClientContextSaveEnabled = false;
        }
        private void OnChangedCurrentCalcTask(CalcTaskModel task)
        {
            if (CurrentCalcTask != null)
            {
                CalcTaskEditEnabled = true;
                CalcTaskDelEnabled = true;
                CalcTaskShowResultEnabled = true;

                if (CurrentCalcTask.ResultStatusCode == 3)
                {
                    CalcTaskStartCalcEnabled = false;
                    CalcTaskStopCalcEnabled = true;
                    CalcTaskAbortCalcEnabled = true;
                }
                else
                {
                    CalcTaskStartCalcEnabled = true;
                    CalcTaskStopCalcEnabled = false;
                    CalcTaskAbortCalcEnabled = false;
                }
            }
            else
            {
                CalcTaskEditEnabled = false;
                CalcTaskDelEnabled = false;
                CalcTaskStartCalcEnabled = false;
                CalcTaskStopCalcEnabled = false;
                CalcTaskAbortCalcEnabled = false;
                CalcTaskShowResultEnabled = false;
            }
        }
        private void ReloadProjectContext()
        {
            if (this.CurrentProject != null)
            {
                this.BaseClientContexts.ProjectId = this.CurrentProject.Id;
                this.BaseClientContexts.Refresh();
            }
            else
            {
                this.BaseClientContexts.ProjectId = 0;
                this.BaseClientContexts.Refresh();
            }
        }
        private void ReloadCalcTask()
        {
            if (this.CurrentClientContext != null)
            {
                this.CalcTasks.ContextId = this.CurrentClientContext.Id;
                this.CalcTasks.Refresh();
            }
            else
            {
                this.CalcTasks.ContextId = 0;
                this.CalcTasks.Refresh();
            }
            CalcTaskEditEnabled = false;
            CalcTaskDelEnabled = false;
            CalcTaskStartCalcEnabled = false;
            CalcTaskStopCalcEnabled = false;
            CalcTaskAbortCalcEnabled = false;
            CalcTaskShowResultEnabled = false;
        }
        private void ReloadClientContext()
        {
            if (this.CurrentProject != null)
            {
                this.ClientContexts.ProjectId = this.CurrentProject.Id;
                this.ClientContexts.Refresh();
            }
            else
            {
                this.ClientContexts.ProjectId = 0;
                this.ClientContexts.Refresh();
            }
            this.CurrentClientContext = null;
            ClientContextEditEnabled = false;
            ClientContextDelEnabled = false;
        }
        private void OnContextAddCommand(object parameter)
        {
            try
            {
                if (this.CurrentBaseClientContext == null)
                    return;

                CurrentClientContextCard = new ClientContextModel()
                {
                    Name = "",
                    Note = "",
                    ProjectId = CurrentProject.Id,
                    BaseContextId = CurrentBaseClientContext.Id,
                    BaseContextName = CurrentBaseClientContext.Name,
                };

                ClientContextSaveEnabled = true;
                ClientContextAddEnabled = false;
                ClientContextEditEnabled = false;
                ClientContextDelEnabled = false;
                _clientContextEditedMode = CardEditMode.Add;
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnContextModifyCommand(object parameter)
        {
            try
            {
                if (CurrentClientContext == null)
                    return;

                ClientContextSaveEnabled = true;
                ClientContextAddEnabled = false;
                ClientContextEditEnabled = false;
                ClientContextDelEnabled = false;
                _clientContextEditedMode = CardEditMode.Edit;
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnContextDeleteCommand(object parameter)
        {
            try
            {
                if (CurrentClientContext == null)
                    return;

                var projectModifier = new Modifiers.DeleteClientContext
                {
                    Id = CurrentClientContext.Id
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnContextSaveCommand(object parameter)
        {
            if (string.IsNullOrEmpty(CurrentClientContextCard.Name))
            {
                _starter.ShowException("Warning!", new Exception($"You must specify the name of the context!"));
                return;
            }

            if(_clientContextEditedMode == CardEditMode.Add)
            {
                var projectModifier = new Modifiers.CreateClientContext
                {
                    ProjectId = CurrentClientContextCard.ProjectId,
                    BaseContextId = CurrentClientContextCard.BaseContextId,
                    Name = CurrentClientContextCard.Name,
                    Note = CurrentClientContextCard.Note,
                    OwnerId = Guid.NewGuid()
                };

                _commandDispatcher.Send(projectModifier);
            }

            if (_clientContextEditedMode == CardEditMode.Edit)
            {
                var projectModifier = new Modifiers.EditClientContext
                {
                    Id = CurrentClientContext.Id,
                    Name = CurrentClientContextCard.Name,
                    Note = CurrentClientContextCard.Note
                };

                _commandDispatcher.Send(projectModifier);
            }

            _clientContextEditedMode = CardEditMode.None;
            ClientContextSaveEnabled = false;
            if (CurrentBaseClientContext != null)
                ClientContextAddEnabled = true;
        }
        private void OnTaskAddCommand(object parameter)
        {
            try
            {
                if (this.CurrentClientContext == null)
                    return;

                var modifier = new Modifiers.CreateCalcTask
                {
                    ContextId = this.CurrentClientContext.Id,
                    MapName = _objectReader.Read<string>().By(new GetMapByProjectId { Id = CurrentProject.Id }),
                    OwnerId = Guid.NewGuid()
                };
                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnTaskModifyCommand(object parameter)
        {
            try
            {
                if (CurrentCalcTask == null)
                    return;

                _starter.Start<VM.StationCalibrationManager.View>(isModal: true, f => { f.Mode = StationCalibrationManager.FormMode.Edit; f.TaskId = CurrentCalcTask.Id; });
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnTaskDeleteCommand(object parameter)
        {
            _starter.StartInUserContext("Warning!", "Are you sure?", TaskDelete);
        }
        private void TaskDelete()
        {
            try
            {
                if (CurrentCalcTask == null)
                    return;

                var modifier = new Modifiers.DeleteCalcTask { Id = CurrentCalcTask.Id };
                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnTaskStartCalcCommand(object parameter)
        {
            try
            {
                if (TaskStartCalcCommand == null)
                    return;

                if (CurrentCalcTask == null)
                    return;

                if (string.IsNullOrEmpty(CurrentCalcTask.MapName))
                {
                    _starter.ShowException("Warning!", new Exception($"Undefined value '{Properties.Resources.MapName}'!"));
                    return;
                }

                var modifier = new Modifiers.RunCalcTask
                {
                    Id = CurrentCalcTask.Id
                };

                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }

        private void OnTaskShowResultCommand(object parameter)
        {
            try
            {
                if (TaskShowResultCommand == null)
                    return;

                var resultModel = _objectReader.Read<CalcResultModel>().By(new GetCalcResultById { Id = CurrentCalcTask.Id });
                if (resultModel != null)
                {
                    _starter.Start<VM.StationCalibrationResult.View>(isModal: true, f => { f.ResultId = resultModel.Id; });
                }
                else
                {
                    _starter.ShowException("Warning!", new Exception($"For selected task not found information in ICalcResults!"));
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnTaskStopCommand(object parameter)
        {
            _starter.StartInUserContext("Warning!", "Are you sure?", TaskStop);
        }
        private void TaskStop()
        {
            try
            {
                var modifier = new Modifiers.StopCalcTask
                {
                    ResultId = CurrentCalcTask.ResultId
                };
                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnTaskAbortCommand(object parameter)
        {
            _starter.StartInUserContext("Warning!", "Are you sure?", TaskAbort);
        }
        private void TaskAbort()
        {
            try
            {
                var modifier = new Modifiers.AbortCalcTask
                {
                    ResultId = CurrentCalcTask.ResultId
                };
                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnTaskRefreshCommand(object parameter)
        {
            ReloadCalcTask();
        }

        private void OnEditNoteCommand(object parameter)
        {
            try
            {
                if (CurrentCalcTask != null)
                {
                    var newNote = Interaction.InputBox("Enter note", Exceptions.StationCalibrationCalculation);
                    if (!string.IsNullOrEmpty(newNote))
                    {
                        var modifier = new Modifiers.EditNoteCalcTask
                        {
                            Id = CurrentCalcTask.Id,
                            Note = newNote
                        };
                        _commandDispatcher.Send(modifier);
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.StationCalibrationCalculation, e);
            }
        }
        private void OnCreatedClientContextHandle(Events.OnCreatedClientContext data)
        {
            ReloadClientContext();
        }
        private void OnEditedClientContextHandle(Events.OnEditedClientContext data)
        {
            ReloadClientContext();
        }
        private void OnDeletedClientContextHandle(Events.OnDeletedClientContext data)
        {
            ReloadClientContext();
        }
        private void OnCreatedCalcTaskHandle(Events.OnCreatedCalcTask data)
        {
            ReloadCalcTask();
            _starter.Start<VM.StationCalibrationManager.View>(isModal: true, f => { f.Mode = StationCalibrationManager.FormMode.Add; f.TaskId = data.CalcTasktId; });
        }
        private void OnEditedCalcTasktHandle(Events.OnEditedCalcTask data)
        {
            ReloadCalcTask();
        }
        private void OnDeletedCalcTaskHandle(Events.OnDeletedCalcTask data)
        {
            ReloadCalcTask();
        }
        private void OnRunCalcTaskHandle(Events.OnRunCalcTask data)
        {
            System.Windows.Forms.MessageBox.Show("The calculation process has been started successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            ReloadCalcTask();
        }
        public override void Dispose()
        {
            _onCreatedClientContextToken?.Dispose();
            _onCreatedClientContextToken = null;
            _onEditedClientContextToken?.Dispose();
            _onEditedClientContextToken = null;
            _onDeletedClientContextToken?.Dispose();
            _onDeletedClientContextToken = null;
            _onCreatedCalcTaskToken?.Dispose();
            _onCreatedCalcTaskToken = null;
            _onEditedCalcTaskToken?.Dispose();
            _onEditedCalcTaskToken = null;
            _onDeletedCalcTaskToken?.Dispose();
            _onDeletedCalcTaskToken = null;
            _onOnRunCalcTaskToken?.Dispose();
            _onOnRunCalcTaskToken = null;
        }
    }
    enum CardEditMode
    {
        None,
        Add,
        Edit,
        Delete,
        Duplicate
    }
}
