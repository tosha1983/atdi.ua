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
using VM = Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels;

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

        private IEventHandlerToken<Events.OnCreatedProject> _onCreatedProjectToken;
        private IEventHandlerToken<Events.OnEditedProject> _onEditedProjectToken;
        private IEventHandlerToken<Events.OnDeletedProject> _onDeletedProjectToken;

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
            this.ContextNewCommand = new ViewCommand(this.OnContextNewCommand);
            this.ContextModifyCommand = new ViewCommand(this.OnContextModifyCommand);
            this.ContextDeleteCommand = new ViewCommand(this.OnContextDeleteCommand);
            this.CurrentProjectCard = new ProjectModel();

            this.Projects = projectDataAdapter;
            this.ProjectMaps = projectMapDataAdapter;
            this.ProjectContexts = clientContextDataAdapter;

            ReloadProjects();

            //_onCreatedProjectToken = _eventBus.Subscribe<Events.OnCreatedProject>(this.OnCreatedProjectHandle);

            //CreateProject(Guid.NewGuid());
            //CreateProject(Guid.NewGuid());
        }
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
        private void OnProjectAddCommand(object parameter)
        {
            try
            {
                _onCreatedProjectToken = _eventBus.Subscribe<Events.OnCreatedProject>(this.OnCreatedProjectHandle);

                var projectModifier = new Modifiers.CreateProject
                {
                    Name = CurrentProjectCard.Name,
                    Note = CurrentProjectCard.Note,
                    OwnerId = Guid.NewGuid(),
                    Projection = CurrentProjectCard.Projection
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnProjectModifyCommand(object parameter)
        {
            try
            {
                _onEditedProjectToken = _eventBus.Subscribe<Events.OnEditedProject>(this.OnEditedProjectHandle);

                var projectModifier = new Modifiers.EditProject
                {
                    Id = CurrentProject.Id,
                    Name = CurrentProjectCard.Name,
                    Note = CurrentProjectCard.Note,
                    OwnerId = Guid.NewGuid(),
                    Projection = CurrentProjectCard.Projection
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnProjectDeleteCommand(object parameter)
        {
            try
            {
                _onDeletedProjectToken = _eventBus.Subscribe<Events.OnDeletedProject>(this.OnDeletedProjectHandle);

                var projectModifier = new Modifiers.DeleteProject
                {
                    Id = CurrentProject.Id
                };

                _commandDispatcher.Send(projectModifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public ProjectModel CurrentProject
        {
            get => this._currentProject;
            set => this.Set(ref this._currentProject, value, () => { this.OnChangedCurrentProject(value); });
        }
        public ProjectModel CurrentProjectCard
        {
            get => this._currentProjectCard;
            set => this.Set(ref this._currentProjectCard, value, () => { /*this.OnChangedCurrentProject(value);*/ });
        }
        public ProjectMapModel CurrentProjectMap
        {
            get => this._currentProjectMap;
            set => this.Set(ref this._currentProjectMap, value, () => { this.OnChangedCurrentProjectMap(value); });
        }
        public ClientContextModel CurrentProjectContext
        {
            get => this._currentProjectContext;
            set => this.Set(ref this._currentProjectContext, value, () => { this.OnChangedCurrentProjectContext(value); });
        }

        private void OnChangedCurrentProject(ProjectModel project)
        {
            CurrentProjectCard = _objectReader.Read<ProjectModel>().By(new GetProjectById
            {
                Id = project.Id
            });

            ReloadProjectMaps(project.Id);
            ReloadProjectContexts(project.Id);
            this.CurrentProjectMap = null;
            this.CurrentProjectContext = null;
        }
        private void OnChangedCurrentProjectMap(ProjectMapModel project)
        {
        }
        private void OnChangedCurrentProjectContext(ClientContextModel project)
        {

        }

        private void ReloadProjects()
        {
            this.Projects.Refresh();
            this.CurrentProject = null;
            this.CurrentProjectCard = null;
        }
        private void ReloadProjectMaps(long projectId)
        {
            this.ProjectMaps.ProjectId = projectId;
            this.ProjectMaps.Refresh();
        }
        private void ReloadProjectContexts(long projectId)
        {
            this.ProjectContexts.ProjectId = projectId;
            this.ProjectContexts.Refresh();
        }
        private void OnProjectActivateCommand(object parameter)
        {

        }
        private void OnProjectLockCommand(object parameter)
        {

        }
        private void OnMapCreateNewCommand(object parameter)
        {
            try
            {
                _starter.Start<VM.Layers.View>(isModal: true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnMapDeleteCommand(object parameter)
        {

        }
        private void OnContextNewCommand(object parameter)
        {
            try
            {
                //var mainForm = new WpfStandardForm("ProjectContextCard.xaml", "ProjectContextCardViewModel");
                //mainForm.ShowDialog();
                //mainForm.Dispose();
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
                //var mainForm = new WpfStandardForm("ProjectContextCard.xaml", "ProjectContextCardViewModel");
                //mainForm.ShowDialog();
                //mainForm.Dispose();
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
                //var mainForm = new WpfStandardForm("ProjectContextCard.xaml", "ProjectContextCardViewModel");
                //mainForm.ShowDialog();
                //mainForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

		public override void Dispose()
		{
            _onCreatedProjectToken?.Dispose();
            _onCreatedProjectToken = null;
        }
	}
}
