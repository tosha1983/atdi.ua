using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using System.Windows;
using Atdi.Platform.Logging;
using Atdi.Icsm.Plugins.Core;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager
{

	[ViewXaml("ProjectManager.xaml")]
	[ViewCaption("Calc Server Client: Project Manager")]
	public class View : ViewBase
    {
	    private readonly ViewStarter _starter;
	    private readonly ILogger _logger;
	    private ProjectModel _currentProject;
        private ProjectMapModel _currentProjectMap;
        private ClientContextModel _currentProjectContext;

        public ProjectDataAdapter Projects { get; set; }
        public ProjectMapDataAdapter ProjectMaps { get; set; }

        public WpfCommand ProjectAddCommand { get; set; }
        public WpfCommand ProjectModifyCommand { get; set; }
        public WpfCommand ProjectDeleteCommand { get; set; }
        public WpfCommand ProjectActivateCommand { get; set; }
        public WpfCommand ProjectLockCommand { get; set; }
        public WpfCommand MapCreateNewCommand { get; set; }
        public WpfCommand MapDeleteCommand { get; set; }
        public WpfCommand ContextNewCommand { get; set; }
        public WpfCommand ContextModifyCommand { get; set; }
        public WpfCommand ContextDeleteCommand { get; set; }

        public View(ProjectDataAdapter projectDataAdapter, ProjectMapDataAdapter projectMapDataAdapter, ViewStarter starter, ILogger logger)
        {
	        this._starter = starter;
	        this._logger = logger;

	        this.ProjectAddCommand = new WpfCommand(this.OnProjectAddCommand);
            this.ProjectModifyCommand = new WpfCommand(this.OnProjectModifyCommand);
            this.ProjectDeleteCommand = new WpfCommand(this.OnProjectDeleteCommand);
            this.ProjectActivateCommand = new WpfCommand(this.OnProjectActivateCommand);
            this.ProjectLockCommand = new WpfCommand(this.OnProjectLockCommand);
            this.MapCreateNewCommand = new WpfCommand(this.OnMapCreateNewCommand);
            this.MapDeleteCommand = new WpfCommand(this.OnMapDeleteCommand);
            this.ContextNewCommand = new WpfCommand(this.OnContextNewCommand);
            this.ContextModifyCommand = new WpfCommand(this.OnContextModifyCommand);
            this.ContextDeleteCommand = new WpfCommand(this.OnContextDeleteCommand);

            this.Projects = projectDataAdapter;
            this.ProjectMaps = projectMapDataAdapter;

            ReloadData();
        }

        public ProjectModel CurrentProject
        {
            get => this._currentProject;
            set => this.Set(ref this._currentProject, value, () => { this.OnChangedCurrentProject(value); });
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

        private void ReloadData()
        {
            this.Projects.Refresh();
            this.ProjectMaps.Refresh();
        }
        private void ReloadProjectMaps(long projectId)
        {

        }
        private void ReloadProjectContexts(long projectId)
        {

        }
        private void OnProjectAddCommand(object parameter)
        {
			
            try
            {
                //var mainForm = new WpfStandardForm("ProjectCard.xaml", "ProjectCardViewModel");
                //mainForm.ShowDialog();
                //mainForm.Dispose();

                //_starter.Start<>();
			}
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
				//_logger.Exception();
            }
        }
        private void OnProjectModifyCommand(object parameter)
        {
            try
            {
                //var mainForm = new WpfStandardForm("ProjectCard.xaml", "ProjectCardViewModel");
                //mainForm.ShowDialog();
                //mainForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnProjectDeleteCommand(object parameter)
        {

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
                //var mainForm = new WpfStandardForm("ProjectMapCard.xaml", "ProjectMapCardViewModel");
                //mainForm.ShowDialog();
                //mainForm.Dispose();
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
			
		}
	}
}
