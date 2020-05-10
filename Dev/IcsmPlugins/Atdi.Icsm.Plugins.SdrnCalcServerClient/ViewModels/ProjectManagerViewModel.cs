using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Models.Views;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels
{
    public class ProjectManagerViewModel : WpfViewModelBase
    {
        private ProjectViewModel _currentProject;
        private ProjectMapViewModel _currentProjectMap;
        private ClientContextViewModel _currentProjectContext;

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

        public ProjectManagerViewModel()
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
            ReloadData();
        }

        public ProjectViewModel CurrentProject
        {
            get => this._currentProject;
            set => this.Set(ref this._currentProject, value, () => { this.OnChangedCurrentProject(value); });
        }
        public ProjectMapViewModel CurrentProjectMap
        {
            get => this._currentProjectMap;
            set => this.Set(ref this._currentProjectMap, value, () => { this.OnChangedCurrentProjectMap(value); });
        }
        public ClientContextViewModel CurrentProjectContext
        {
            get => this._currentProjectContext;
            set => this.Set(ref this._currentProjectContext, value, () => { this.OnChangedCurrentProjectContext(value); });
        }

        private void OnChangedCurrentProject(ProjectViewModel project)
        {
            this.CurrentProjectMap = null;
            this.CurrentProjectContext = null;
        }
        private void OnChangedCurrentProjectMap(ProjectMapViewModel project)
        {
        }
        private void OnChangedCurrentProjectContext(ClientContextViewModel project)
        {
        }

        private void ReloadData()
        {
            //throw new NotImplementedException();
        }

        private void OnProjectAddCommand(object parameter)
        {

        }
        private void OnProjectModifyCommand(object parameter)
        {

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

        }
        private void OnMapDeleteCommand(object parameter)
        {

        }
        private void OnContextNewCommand(object parameter)
        {

        }
        private void OnGetCSVCommand(object parameter)
        {

        }
        private void OnContextModifyCommand(object parameter)
        {

        }
        private void OnContextDeleteCommand(object parameter)
        {

        }
    }
}
