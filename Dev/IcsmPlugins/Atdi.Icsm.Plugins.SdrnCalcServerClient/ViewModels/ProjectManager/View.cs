using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Adapters;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Queries;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Windows;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using VM = Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels;
using MP = Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map;
using CT = Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext;
using Atdi.WpfControls.EntityOrm.Controls;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager
{
	[ViewXaml("ProjectManager.xaml")]
	[ViewCaption("Calc Server Client: Project Manager")]
	public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private ProjectModel _currentProject;
        private ProjectModel _currentProjectCard;
        private ProjectMapModel _currentProjectMap;
        private ClientContextModel _currentProjectContext;
        private OrmEnumBoxData[] _projectionSource;
        private Dictionary<string, OrmEnumBoxData> _projections;

        private IEventHandlerToken<Events.OnCreatedProject> _onCreatedProjectToken;
        private IEventHandlerToken<Events.OnEditedProject> _onEditedProjectToken;
        private IEventHandlerToken<Events.OnDeletedProject> _onDeletedProjectToken;
        private IEventHandlerToken<MP.Events.OnCreatedMap> _onCreatedMapToken;
        private IEventHandlerToken<MP.Events.OnEditedMap> _onEditedMapToken;
        private IEventHandlerToken<MP.Events.OnDeletedMap> _onDeletedMapToken;
        private IEventHandlerToken<CT.Events.OnCreatedClientContext> _onCreatedClientContextToken;
        private IEventHandlerToken<CT.Events.OnEditedClientContext> _onEditedClientContextToken;
        private IEventHandlerToken<CT.Events.OnDeletedClientContext> _onDeletedClientContextToken;

        public ProjectDataAdapter Projects { get; set; }
        public ProjectMapDataAdapter ProjectMaps { get; set; }
        public ClientContextDataAdapter ProjectContexts { get; set; }

        public ViewCommand ProjectAddCommand { get; set; }
        public ViewCommand ProjectModifyCommand { get; set; }
        public ViewCommand ProjectDeleteCommand { get; set; }
        public ViewCommand ProjectActivateCommand { get; set; }
        public ViewCommand ProjectLockCommand { get; set; }
        public ViewCommand MapCreateNewCommand { get; set; }
        public ViewCommand MapDeleteCommand { get; set; }
        public ViewCommand MapRefreshCommand { get; set; }
        public ViewCommand ContextNewCommand { get; set; }
        public ViewCommand ContextModifyCommand { get; set; }
        public ViewCommand ContextDeleteCommand { get; set; }

        public View(
            ProjectDataAdapter projectDataAdapter,
            ProjectMapDataAdapter projectMapDataAdapter,
            ClientContextDataAdapter clientContextDataAdapter,
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

            this.ProjectAddCommand = new ViewCommand(this.OnProjectAddCommand);
            this.ProjectModifyCommand = new ViewCommand(this.OnProjectModifyCommand);
            this.ProjectDeleteCommand = new ViewCommand(this.OnProjectDeleteCommand);
            this.ProjectActivateCommand = new ViewCommand(this.OnProjectActivateCommand);
            this.ProjectLockCommand = new ViewCommand(this.OnProjectLockCommand);
            this.MapCreateNewCommand = new ViewCommand(this.OnMapCreateNewCommand);
            this.MapDeleteCommand = new ViewCommand(this.OnMapDeleteCommand);
            this.MapRefreshCommand = new ViewCommand(this.OnMapRefreshCommand);
            this.ContextNewCommand = new ViewCommand(this.OnContextNewCommand);
            this.ContextModifyCommand = new ViewCommand(this.OnContextModifyCommand);
            this.ContextDeleteCommand = new ViewCommand(this.OnContextDeleteCommand);
            this.CurrentProjectCard = new ProjectModel();

            this.Projects = projectDataAdapter;
            this.ProjectMaps = projectMapDataAdapter;
            this.ProjectContexts = clientContextDataAdapter;

            _onCreatedProjectToken = _eventBus.Subscribe<Events.OnCreatedProject>(this.OnCreatedProjectHandle);
            _onEditedProjectToken = _eventBus.Subscribe<Events.OnEditedProject>(this.OnEditedProjectHandle);
            _onDeletedProjectToken = _eventBus.Subscribe<Events.OnDeletedProject>(this.OnDeletedProjectHandle);
            _onCreatedMapToken = _eventBus.Subscribe<MP.Events.OnCreatedMap>(this.OnCreatedMapHandle);
            _onEditedMapToken = _eventBus.Subscribe<MP.Events.OnEditedMap>(this.OnEditedMapHandle);
            _onDeletedMapToken = _eventBus.Subscribe<MP.Events.OnDeletedMap>(this.OnDeletedMapHandle);
            _onCreatedClientContextToken = _eventBus.Subscribe<CT.Events.OnCreatedClientContext>(this.OnCreatedClientContextHandle);
            _onEditedClientContextToken = _eventBus.Subscribe<CT.Events.OnEditedClientContext>(this.OnEditedClientContextHandle);
            _onDeletedClientContextToken = _eventBus.Subscribe<CT.Events.OnDeletedClientContext>(this.OnDeletedClientContextHandle);


            PrepareProjectionCombo();
            ReloadProjects();
            ReloadProjectContexts();
            ReloadProjectMaps();
        }
        public ProjectModel CurrentProject
        {
            get => this._currentProject;
            set => this.Set(ref this._currentProject, value, () => { this.OnChangedCurrentProject(value); });
        }
        public ProjectModel CurrentProjectCard
        {
            get => this._currentProjectCard;
            set => this.Set(ref this._currentProjectCard, value);
        }
        public ProjectMapModel CurrentProjectMap
        {
            get => this._currentProjectMap;
            set => this.Set(ref this._currentProjectMap, value);
        }
        public ClientContextModel CurrentProjectContext
        {
            get => this._currentProjectContext;
            set => this.Set(ref this._currentProjectContext, value);
        }
        public OrmEnumBoxData[] ProjectionSource
        {
            get => this._projectionSource;
            set => this.Set(ref this._projectionSource, value);
        }
        private void PrepareProjectionCombo()
        {
            var projections = _objectReader.Read<string[]>().By(new GetProjections());
            this._projections = new Dictionary<string, OrmEnumBoxData>();
            var enumData = new List<OrmEnumBoxData>();
            int i = 0;

            foreach (var item in projections)
            {
                var data = new OrmEnumBoxData() { Id = ++i, Name = item, ViewName = item };
                enumData.Add(data);
                this._projections.Add(item, data);
            }
            ProjectionSource = enumData.ToArray();
        }

        private void OnChangedCurrentProject(ProjectModel project)
        {
            var card = _objectReader.Read<ProjectModel>().By(new GetProjectById { Id = project.Id });

            if (this._projections.ContainsKey(card.Projection))
                card.ProjectionCombo = this._projections[card.Projection];

            this.CurrentProjectCard = card;
            ReloadProjectMaps();
            ReloadProjectContexts();
            this.CurrentProjectMap = null;
            this.CurrentProjectContext = null;
        }
        private void ReloadProjects()
        {
            this.Projects.Refresh();
            this.CurrentProject = null;
            this.CurrentProjectCard = new ProjectModel();
        }
        private void ReloadProjectMaps()
        {
            if (this.CurrentProject != null)
            {
                this.ProjectMaps.ProjectId = this.CurrentProject.Id;
                this.ProjectMaps.Refresh();
            }
            else
            {
                this.ProjectMaps.ProjectId = 0;
                this.ProjectMaps.Refresh();
            }
        }
        private void ReloadProjectContexts()
        {
            if (this.CurrentProject != null)
            {
                this.ProjectContexts.ProjectId = this.CurrentProject.Id;
                this.ProjectContexts.Refresh();
            }
            else
            {
                this.ProjectContexts.ProjectId = 0;
                this.ProjectContexts.Refresh();
            }
        }

        #region WPF commands
        private void OnProjectAddCommand(object parameter)
        {
            try
            {
                var projectModifier = new Modifiers.CreateProject
                {
                    Name = CurrentProjectCard.Name,
                    Note = CurrentProjectCard.Note,
                    OwnerId = Guid.NewGuid(),
                    Projection = CurrentProjectCard.ProjectionCombo.Name
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnProjectModifyCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                    return;

                var projectModifier = new Modifiers.EditProject
                {
                    Id = CurrentProject.Id,
                    Name = CurrentProjectCard.Name,
                    Note = CurrentProjectCard.Note,
                    Projection = CurrentProjectCard.ProjectionCombo.Name
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnProjectDeleteCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                    return;

                var projectModifier = new Modifiers.DeleteProject
                {
                    Id = CurrentProject.Id
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnProjectActivateCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                    return;

                var projectModifier = new Modifiers.ChangeStateProject
                {
                    Id = CurrentProject.Id,
                    StatusCode = (byte)ProjectStatusCode.Available
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnProjectLockCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                    return;

                var projectModifier = new Modifiers.ChangeStateProject
                {
                    Id = CurrentProject.Id,
                    StatusCode = (byte)ProjectStatusCode.Locked
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnMapCreateNewCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                {
                    _starter.ShowException(Exceptions.CalcServerClient, new Exception(Properties.Resources.Message_YouMustSelectTheRequiredProject));
                    return;
                }
                _starter.Start<VM.Map.View>(isModal: true, f => f.ProjectId = CurrentProject.Id );
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnMapDeleteCommand(object parameter)
        {
            try
            {
                if (CurrentProjectMap == null)
                {
                    _starter.ShowException(Exceptions.CalcServerClient, new Exception(Properties.Resources.Message_YouMustSelectTheRequiredProject));
                    return;
                }

                var projectModifier = new MP.Modifiers.DeleteMap
                {
                    Id = CurrentProjectMap.Id
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnMapRefreshCommand(object parameter)
        {
            ReloadProjectMaps();
        }
        private void OnContextNewCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                {
                    _starter.ShowException(Exceptions.CalcServerClient, new Exception(Properties.Resources.Message_YouMustSelectTheRequiredProject));
                    return;
                }

                _starter.Start<VM.ClientContext.View>(isModal: true, f => { f.ProjectId = CurrentProject.Id; });
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnContextModifyCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                {
                    _starter.ShowException(Exceptions.CalcServerClient, new Exception(Properties.Resources.Message_YouMustSelectTheRequiredProject));
                    return;
                }

                if (CurrentProjectContext == null)
                    return;

                _starter.Start<VM.ClientContext.View>(isModal: true, f => { f.ContextId = CurrentProjectContext.Id; });
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnContextDeleteCommand(object parameter)
        {
            try
            {
                if (CurrentProject == null)
                {
                    _starter.ShowException(Exceptions.CalcServerClient, new Exception(Properties.Resources.Message_YouMustSelectTheRequiredProject));
                    return;
                }

                if (CurrentProjectContext == null)
                    return;

                var projectModifier = new CT.Modifiers.DeleteClientContext
                {
                    Id = CurrentProjectContext.Id
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        #endregion

        #region Handlers
        private void OnCreatedProjectHandle(Events.OnCreatedProject data)
        {
            ReloadProjects();
        }
        private void OnEditedProjectHandle(Events.OnEditedProject data)
        {
            ReloadProjects();
        }
        private void OnDeletedProjectHandle(Events.OnDeletedProject data)
        {
            ReloadProjects();
        }
        private void OnCreatedMapHandle(MP.Events.OnCreatedMap data)
        {
            ReloadProjectMaps();
        }
        private void OnEditedMapHandle(MP.Events.OnEditedMap data)
        {
            ReloadProjectMaps();
        }
        private void OnDeletedMapHandle(MP.Events.OnDeletedMap data)
        {
            ReloadProjectMaps();
        }
        private void OnCreatedClientContextHandle(CT.Events.OnCreatedClientContext data)
        {
            ReloadProjectContexts();
        }
        private void OnEditedClientContextHandle(CT.Events.OnEditedClientContext data)
        {
            ReloadProjectContexts();
        }
        private void OnDeletedClientContextHandle(CT.Events.OnDeletedClientContext data)
        {
            ReloadProjectContexts();
        }
        #endregion
        public override void Dispose()
		{
            _onCreatedProjectToken?.Dispose();
            _onCreatedProjectToken = null;
            _onEditedProjectToken?.Dispose();
            _onEditedProjectToken = null;
            _onDeletedProjectToken?.Dispose();
            _onDeletedProjectToken = null;
            _onCreatedMapToken?.Dispose();
            _onCreatedMapToken = null;
            _onEditedMapToken?.Dispose();
            _onEditedMapToken = null;
            _onDeletedMapToken?.Dispose();
            _onDeletedMapToken = null;
            _onCreatedClientContextToken?.Dispose();
            _onCreatedClientContextToken = null;
            _onEditedClientContextToken?.Dispose();
            _onEditedClientContextToken = null;
            _onDeletedClientContextToken?.Dispose();
            _onDeletedClientContextToken = null;
        }
    }
}
