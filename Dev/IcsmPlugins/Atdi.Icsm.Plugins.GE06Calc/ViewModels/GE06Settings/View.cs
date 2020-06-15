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
        private CardEditMode _calcTaskEditedMode = CardEditMode.None;

        public ViewCommand ContextAddCommand { get; set; }
        public ViewCommand ContextModifyCommand { get; set; }
        public ViewCommand ContextDeleteCommand { get; set; }
        public ViewCommand ContextSaveCommand { get; set; }
        public ViewCommand TaskStartCalcCommand { get; set; }
        public ViewCommand TaskShowResultCommand { get; set; }

        public ProjectDataAdapter Projects { get; set; }
        public BaseClientContextDataAdapter BaseClientContexts { get; set; }
        public ClientContextDataAdapter ClientContexts { get; set; }
        public CalcTaskDataAdapter CalcTasks { get; set; }

        private IEventHandlerToken<Events.OnCreatedClientContext> _onCreatedClientContextToken;
        private IEventHandlerToken<Events.OnEditedClientContext> _onEditedClientContextToken;
        private IEventHandlerToken<Events.OnDeletedClientContext> _onDeletedClientContextToken;

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

            //this.TaskStartCalcCommand = new ViewCommand(this.OnTaskStartCalcCommand);
            //this.TaskShowResultCommand = new ViewCommand(this.OnTaskShowResultCommand);

            this.CurrentClientContextCard = new ClientContextModel();

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

        private bool _calcTaskStartCalcEnabled = false;
        public bool CalcTaskStartCalcEnabled
        {
            get => this._calcTaskStartCalcEnabled;
            set => this.Set(ref this._calcTaskStartCalcEnabled, value);
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
            CalcTaskStartCalcEnabled = false;
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
                CalcTaskStartCalcEnabled = true;
                CalcTaskShowResultEnabled = true;
            }
            else
            {
                CalcTaskStartCalcEnabled = false;
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
            CalcTaskStartCalcEnabled = false;
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
                MessageBox.Show(e.ToString());
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
        private void OnContextSaveCommand(object parameter)
        {
            if (_clientContextEditedMode == CardEditMode.Add)
            {
                _onCreatedClientContextToken = _eventBus.Subscribe<Events.OnCreatedClientContext>(this.OnCreatedClientContextHandle);

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
                _onEditedClientContextToken = _eventBus.Subscribe<Events.OnEditedClientContext>(this.OnEditedClientContextHandle);

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
        //private void OnTaskStartCalcCommand(object parameter)
        //{
        //    try
        //    {
        //        if (TaskStartCalcCommand == null)
        //            return;

        //        _onOnRunCalcTaskToken = _eventBus.Subscribe<Events.OnRunCalcTask>(this.OnRunCalcTaskHandle);

        //        var modifier = new Modifiers.RunCalcTask
        //        {
        //            Id = CurrentCalcTask.Id
        //        };

        //        _commandDispatcher.Send(modifier);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }
        //}

        //private void OnTaskShowResultCommand(object parameter)
        //{
        //    try
        //    {
        //        if (TaskShowResultCommand == null)
        //            return;

        //        var resultModel = _objectReader.Read<CalcResultModel>().By(new GetCalcResultById { Id = CurrentCalcTask.Id });
        //        if (resultModel != null)
        //        {
        //            _starter.Start<VM.StationCalibrationResult.View>(isModal: true, f => { f.ResultId = resultModel.Id; });
        //        }
        //        else
        //        {
        //            MessageBox.Show("For selected task not found information in ICalcResults!");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.ToString());
        //    }
        //}

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
        //private void OnRunCalcTaskHandle(Events.OnRunCalcTask data)
        //{
        //    ReloadCalcTask();
        //}
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
    enum CardEditMode
    {
        None,
        Add,
        Edit,
        Delete,
        Duplicate
    }
}
