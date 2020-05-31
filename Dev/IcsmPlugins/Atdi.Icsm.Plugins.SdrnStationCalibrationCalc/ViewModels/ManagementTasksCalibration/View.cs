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
        private CalcTaskModel _currentCalcTaskCard;

        public ViewCommand ContextAddCommand { get; set; }
        public ViewCommand ContextModifyCommand { get; set; }
        public ViewCommand ContextDeleteCommand { get; set; }
        public ViewCommand TaskAddCommand { get; set; }
        public ViewCommand TaskModifyCommand { get; set; }
        public ViewCommand TaskDeleteCommand { get; set; }
        public ViewCommand TaskStartCalcCommand { get; set; }
        public ViewCommand TaskShowResultCommand { get; set; }


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

            this.TaskAddCommand = new ViewCommand(this.OnTaskAddCommand);
            this.TaskModifyCommand = new ViewCommand(this.OnTaskModifyCommand);
            this.TaskDeleteCommand = new ViewCommand(this.OnTaskDeleteCommand);

            this.TaskStartCalcCommand = new ViewCommand(this.OnTaskStartCalcCommand);
            this.TaskShowResultCommand = new ViewCommand(this.OnTaskShowResultCommand);

            this.CurrentClientContextCard = new ClientContextModel();
            this.CurrentCalcTaskCard = new CalcTaskModel();

            this.Projects = projectDataAdapter;
            this.BaseClientContexts = baseContextDataAdapter;
            this.ClientContexts = contextDataAdapter;
            this.CalcTasks = calcTaskDataAdapter;

            ReloadProjects();
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
            set => this.Set(ref this._currentBaseClientContext, value);
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
        public CalcTaskModel CurrentCalcTaskCard
        {
            get => this._currentCalcTaskCard;
            set => this.Set(ref this._currentCalcTaskCard, value);
        }
        private void OnChangedCurrentProject(ProjectModel project)
        {
            ReloadProjectContexts();
            CurrentClientContextCard = new ClientContextModel();
            CurrentCalcTaskCard = new CalcTaskModel();
        }
        private void OnChangedCurrentClientContext(ClientContextModel context)
        {
            if (CurrentClientContext != null)
            {
                ReloadCalcTask();
                this.CurrentClientContextCard = _objectReader.Read<ClientContextModel>().By(new GetClientContextById { Id = CurrentClientContext.Id });
                CurrentCalcTaskCard = new CalcTaskModel();
            }    
        }
        private void OnChangedCurrentCalcTask(CalcTaskModel task)
        {
            this.CurrentCalcTaskCard = _objectReader.Read<CalcTaskModel>().By(new GetCalcTaskById { Id = CurrentCalcTask.Id });
        }
        private void ReloadProjectContexts()
        {
            if (this.CurrentProject != null)
            {
                this.BaseClientContexts.ProjectId = this.CurrentProject.Id;
                this.BaseClientContexts.Refresh();
                ReloadClientContext();
            }
        }
        private void ReloadCalcTask()
        {
            this.CalcTasks.ContextId = this.CurrentClientContext.Id;
            this.CalcTasks.Refresh();
        }
        private void ReloadClientContext()
        {
            this.ClientContexts.ProjectId = this.CurrentProject.Id;
            this.ClientContexts.Refresh();
        }
        private void OnContextAddCommand(object parameter)
        {
            try
            {
                if (this.CurrentBaseClientContext == null)
                    return;

                _onCreatedClientContextToken = _eventBus.Subscribe<Events.OnCreatedClientContext>(this.OnCreatedClientContextHandle);

                var projectModifier = new Modifiers.CreateClientContext
                {
                    ProjectId = CurrentProject.Id,
                    BaseContextId = CurrentBaseClientContext.Id,
                    Name = CurrentClientContextCard.Name,
                    Note = CurrentClientContextCard.Note,
                    TypeCode = CurrentClientContextCard.TypeCode,
                    OwnerId = Guid.NewGuid()
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnContextModifyCommand(object parameter)
        {
            try
            {
                if (CurrentClientContext == null)
                    return;

                _onEditedClientContextToken = _eventBus.Subscribe<Events.OnEditedClientContext>(this.OnEditedClientContextHandle);

                var projectModifier = new Modifiers.EditClientContext
                {
                    Id = CurrentClientContext.Id,
                    Name = CurrentClientContextCard.Name,
                    Note = CurrentClientContextCard.Note,
                    TypeCode = CurrentClientContextCard.TypeCode
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnContextDeleteCommand(object parameter)
        {
            try
            {
                if (CurrentClientContext == null)
                    return;

                _onDeletedClientContextToken = _eventBus.Subscribe<Events.OnDeletedClientContext>(this.OnDeletedClientContextHandle);

                var projectModifier = new Modifiers.DeleteClientContext
                {
                    Id = CurrentClientContext.Id
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnTaskAddCommand(object parameter)
        {
            try
            {
                if (this.CurrentClientContext == null)
                    return;

                _onCreatedCalcTaskToken = _eventBus.Subscribe<Events.OnCreatedCalcTask>(this.OnCreatedCalcTaskHandle);

                var modifier = new Modifiers.CreateCalcTask
                {
                    ContextId = CurrentClientContext.Id,
                    MapName = CurrentCalcTaskCard.MapName,
                    TypeCode = CurrentCalcTaskCard.TypeCode,
                    OwnerId = Guid.NewGuid()
                };

                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnTaskModifyCommand(object parameter)
        {
            try
            {
                if (CurrentCalcTaskCard == null)
                    return;

                _onEditedCalcTaskToken = _eventBus.Subscribe<Events.OnEditedCalcTask>(this.OnEditedCalcTasktHandle);

                var modifier = new Modifiers.EditCalcTask
                {
                    Id = CurrentCalcTask.Id,
                    MapName = CurrentCalcTaskCard.MapName,
                    TypeCode = CurrentCalcTaskCard.TypeCode
                };

                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnTaskDeleteCommand(object parameter)
        {
            try
            {
                if (CurrentCalcTaskCard == null)
                    return;

                _onDeletedCalcTaskToken = _eventBus.Subscribe<Events.OnDeletedCalcTask>(this.OnDeletedCalcTaskHandle);

                var modifier = new Modifiers.DeleteCalcTask
                {
                    Id = CurrentCalcTask.Id
                };

                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnTaskStartCalcCommand(object parameter)
        {
            try
            {
                if (TaskStartCalcCommand == null)
                    return;

                _onOnRunCalcTaskToken = _eventBus.Subscribe<Events.OnRunCalcTask>(this.OnRunCalcTaskHandle);

                var modifier = new Modifiers.RunCalcTask
                {
                    Id = CurrentCalcTask.Id
                };

                _commandDispatcher.Send(modifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
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
                    MessageBox.Show("For selected task not found information in ICalcResults!");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
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
            _starter.Start<VM.StationCalibrationManager.View>(isModal: true, f => { f.TaskId = data.CalcTasktId; });
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
        }
    }
}
