using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;

using Atdi.Icsm.Plugins.SdrnCalcServerClient.Forms;
using System.Windows;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager
{
    public class View : WpfViewModelBase
    {
        private ProjectModel _currentProject;
        private ProjectMapModel _currentProjectMap;
        private ClientContextModel _currentProjectContext;

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

        public ProjectModel[] Projects { get; set; }

        public View()
        {
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

            this.Projects = new List<ProjectModel>().ToArray();

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
			///TODO:
        }

        private void ReloadData()
        {
			///TODO: нужно реализовать ReloadData
		}

		private void ReloadProjects()
        {
            var listProjects = new List<ProjectModel>();
            //var endpoint = PluginHelper.GetEndpoint();
            var endpoint = new WebApiEndpoint(new Uri("http://10.1.1.195:15020/"), "/appserver/v1");
            var dataContext = new WebApiDataContext("SDRN_Server_DB");
            var dataLayer = new WebApiDataLayer();
            var webQuery = dataLayer.GetBuilder<IProject>()
                .Read()
                .Select(c => c.Id, c => c.Name, c => c.Note, c => c.StatusName, c => c.StatusCode, c => c.StatusNote, c => c.CreatedDate, c => c.OwnerInstance, c => c.OwnerProjectId, c => c.Projection);
            var executor = dataLayer.GetExecutor(endpoint, dataContext);
            var records = executor.ExecuteAndFetch(webQuery, reader =>
            {
                while (reader.Read())
                {
                    var project = new ProjectModel()
                    {
                        Id = reader.GetValue(c => c.Id),
                        Name = reader.GetValue(c => c.Name),
                        Note = reader.GetValue(c => c.Note),
                        StatusName = reader.GetValue(c => c.StatusName),
                        StatusCode = reader.GetValue(c => c.StatusCode),
                        StatusNote = reader.GetValue(c => c.StatusNote),
                        CreatedDate = reader.GetValue(c => c.CreatedDate),
                        OwnerInstance = reader.GetValue(c => c.OwnerInstance),
                        OwnerProjectId = reader.GetValue(c => c.OwnerProjectId),
                        Projection = reader.GetValue(c => c.Projection),
                    };
                    listProjects.Add(project);
                }
                return true;
            });
            Projects = listProjects.ToArray();
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
                var mainForm = new WpfStandardForm("ProjectCard.xaml", "ProjectCardViewModel");
                mainForm.ShowDialog();
                mainForm.Dispose();
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
                var mainForm = new WpfStandardForm("ProjectCard.xaml", "ProjectCardViewModel");
                mainForm.ShowDialog();
                mainForm.Dispose();
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
                var mainForm = new WpfStandardForm("ProjectMapCard.xaml", "ProjectMapCardViewModel");
                mainForm.ShowDialog();
                mainForm.Dispose();
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
                var mainForm = new WpfStandardForm("ProjectContextCard.xaml", "ProjectContextCardViewModel");
                mainForm.ShowDialog();
                mainForm.Dispose();
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
                var mainForm = new WpfStandardForm("ProjectContextCard.xaml", "ProjectContextCardViewModel");
                mainForm.ShowDialog();
                mainForm.Dispose();
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
                var mainForm = new WpfStandardForm("ProjectContextCard.xaml", "ProjectContextCardViewModel");
                mainForm.ShowDialog();
                mainForm.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
