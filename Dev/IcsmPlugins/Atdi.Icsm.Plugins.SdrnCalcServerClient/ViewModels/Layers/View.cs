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

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Layers
{
    [ViewXaml("Layers.xaml")]
    [ViewCaption("Calc Server Client: Creating layers")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        public View(ProjectDataAdapter projectDataAdapter,

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

            //this.Projects = projectDataAdapter;

            //ReloadData();

            //_onCreatedProjectToken = _eventBus.Subscribe<Events.OnCreatedProject>(this.OnCreatedProjectHandle);

            //var ownerId = Guid.NewGuid();

            //CreateProject(ownerId);
            //var p = ReadProject(ownerId);
        }
        //public ProjectModel CurrentProject
        //{
        //    get => this._currentProject;
        //    set => this.Set(ref this._currentProject, value, () => { this.OnChangedCurrentProject(value); });
        //}
        //private void OnChangedCurrentProject(ProjectModel project)
        //{
        //    //var currentProject = _objectReader.Read<ProjectModel>().By(new GetProjectById
        //    //    {
        //    //        Id = project.Id
        //    //    });

        //    ReloadProjectMaps(project.Id);
        //    ReloadProjectContexts(project.Id);
        //    this.CurrentProjectMap = null;
        //    this.CurrentProjectContext = null;
        //}
        //private void ReloadData()
        //{
        //    this.Projects.Refresh();
        //}
        public override void Dispose()
        {
            //_onCreatedProjectToken?.Dispose();
            //_onCreatedProjectToken = null;
        }
    }
}
