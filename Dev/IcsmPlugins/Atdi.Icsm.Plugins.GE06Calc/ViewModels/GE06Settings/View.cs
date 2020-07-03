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
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Queries;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings.Adapters;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Data;
using System.Windows;
using CT = Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Settings
{

    [ViewXaml("GE06Settings.xaml")]
    [ViewCaption("GE06: Settings")]
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
        public ViewCommand TaskDeleteCommand { get; set; }
        public ViewCommand TaskShowResultCommand { get; set; }

        public ProjectDataAdapter Projects { get; set; }
        public BaseClientContextDataAdapter BaseClientContexts { get; set; }
        public ClientContextDataAdapter ClientContexts { get; set; }
        public CalcTaskDataAdapter CalcTasks { get; set; }

        private IEventHandlerToken<Events.OnCreatedClientContext> _onCreatedClientContextToken;
        private IEventHandlerToken<Events.OnEditedClientContext> _onEditedClientContextToken;
        private IEventHandlerToken<Events.OnDeletedClientContext> _onDeletedClientContextToken;
        private IEventHandlerToken<CT.Events.OnDeletedCalcTask> _onDeletedCalcTaskToken;

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

            this.TaskDeleteCommand = new ViewCommand(this.OnTaskDeleteCommand);
            this.TaskShowResultCommand = new ViewCommand(this.OnTaskShowResultCommand);

            this.CurrentClientContextCard = new ClientContextModel();

            this.Projects = projectDataAdapter;
            this.BaseClientContexts = baseContextDataAdapter;
            this.ClientContexts = contextDataAdapter;
            this.CalcTasks = calcTaskDataAdapter;

            _onCreatedClientContextToken = _eventBus.Subscribe<Events.OnCreatedClientContext>(this.OnCreatedClientContextHandle);
            _onEditedClientContextToken = _eventBus.Subscribe<Events.OnEditedClientContext>(this.OnEditedClientContextHandle);
            _onDeletedClientContextToken = _eventBus.Subscribe<Events.OnDeletedClientContext>(this.OnDeletedClientContextHandle);
            _onDeletedCalcTaskToken = _eventBus.Subscribe<CT.Events.OnDeletedCalcTask>(this.OnDeletedCalcTaskHandle);

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

        private bool _calcTaskDelEnabled = false;
        public bool CalcTaskDelEnabled
        {
            get => this._calcTaskDelEnabled;
            set => this.Set(ref this._calcTaskDelEnabled, value);
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
            ReloadProjectContexts();
            CurrentClientContextCard = new ClientContextModel();
            ClientContextSaveEnabled = false;
            CalcTaskDelEnabled = false;
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
            }
            else
            {
                ClientContextEditEnabled = false;
                ClientContextDelEnabled = false;

            }
            ClientContextSaveEnabled = false;
        }
        private void OnChangedCurrentCalcTask(CalcTaskModel task)
        {
            if (CurrentCalcTask != null)
            {
                CalcTaskDelEnabled = true;
                CalcTaskShowResultEnabled = true;
            }
            else
            {
                CalcTaskDelEnabled = false;
                CalcTaskShowResultEnabled = false;
            }
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
            CalcTaskDelEnabled = false;
            CalcTaskShowResultEnabled = false;
        }
        private void ReloadClientContext()
        {
            this.ClientContexts.ProjectId = this.CurrentProject.Id;
            this.ClientContexts.Refresh();
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
                this._logger.Exception(Exceptions.GE06Client, e);
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
                this._logger.Exception(Exceptions.GE06Client, e);
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
                this._logger.Exception(Exceptions.GE06Client, e); 
            }
        }
        private void OnContextSaveCommand(object parameter)
        {
            if (_clientContextEditedMode == CardEditMode.Add)
            {
                var projectModifier = new Modifiers.CreateClientContext
                {
                    ProjectId = CurrentClientContextCard.ProjectId,
                    BaseContextId = CurrentClientContextCard.BaseContextId,
                    Name = CurrentClientContextCard.Name,
                    Note = CurrentClientContextCard.Note,
                    ActiveContext = CurrentClientContextCard.ActiveContext,
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
                    Note = CurrentClientContextCard.Note,
                    ActiveContext = CurrentClientContextCard.ActiveContext
                };

                _commandDispatcher.Send(projectModifier);
            }

            _clientContextEditedMode = CardEditMode.None;
            ClientContextSaveEnabled = false;
            if (CurrentBaseClientContext != null)
                ClientContextAddEnabled = true;
        }
        private void OnTaskDeleteCommand(object parameter)
        {
            try
            {
                if (CurrentCalcTask == null)
                    return;

                var projectModifier = new CT.Modifiers.DeleteCalcTask
                {
                    Id = CurrentCalcTask.Id
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
            }
        }

        private void OnTaskShowResultCommand(object parameter)
        {
            try
            {
                var resultId = _objectReader.Read<long?>().By(new GetResultIdByTaskId { TaskId = CurrentCalcTask.Id });
                if (resultId.HasValue)
                    _starter.Start<VM.GE06TaskResult.View>(isModal: true);
                //else
                //{
                //    this._logger.Exception(Exceptions.GE06Client, "For selected task not found information in ICalcResults!");
                //}
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
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
        private void OnDeletedCalcTaskHandle(CT.Events.OnDeletedCalcTask data)
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
            _onDeletedCalcTaskToken?.Dispose();
            _onDeletedCalcTaskToken = null;
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
