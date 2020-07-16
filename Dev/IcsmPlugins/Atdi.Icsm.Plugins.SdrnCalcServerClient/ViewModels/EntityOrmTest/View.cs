using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest.Adapters;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest.Queries;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using Atdi.WpfControls.EntityOrm.Controls;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest
{

    [ViewXaml("EntityOrmTest.xaml")]
    [ViewCaption("Calc Server Client: Entity ORM Test")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private IEventHandlerToken<Events.OnCreatedProject> _onCreatedProjectToken;

        private OrmEnumBoxData[] _statusTestEnum;

        public View(
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ProjectDataAdapter projectDataAdapter,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;

            this.Projects = projectDataAdapter;
            this.Projects.Refresh();

            _onCreatedProjectToken = _eventBus.Subscribe<Events.OnCreatedProject>(this.OnCreatedProjectHandle);

            var ownerId = Guid.NewGuid();

            CreateProject(ownerId);
            var p = ReadProject(ownerId);

            var enumData = new List<OrmEnumBoxData>();
            enumData.Add(new OrmEnumBoxData() { Id = 0, Name = "Created", ViewName = "Created" });
            enumData.Add(new OrmEnumBoxData() { Id = 1, Name = "Modifying", ViewName = "Modifying" });
            enumData.Add(new OrmEnumBoxData() { Id = 2, Name = "Available", ViewName = "Available" });
            enumData.Add(new OrmEnumBoxData() { Id = 3, Name = "Locked", ViewName = "Locked" });
            enumData.Add(new OrmEnumBoxData() { Id = 4, Name = "Archived", ViewName = "Archived" });
            this.StatusTestEnum = enumData.ToArray();

            EnumValue = enumData[2];
            EnumValueId = 3;
            CheckBoxValue = true;
        }

        public OrmEnumBoxData EnumValue { get; set; }
        public byte EnumValueId { get; set; }
        public bool? CheckBoxValue { get; set; }

        private void OnCreatedProjectHandle(Events.OnCreatedProject data)
        {
            this.Projects.Refresh();

        }

        public ProjectDataAdapter Projects { get; set; }

        public override void Dispose()
        {
            _onCreatedProjectToken?.Dispose();
            _onCreatedProjectToken = null;

        }

        public void CreateProject(Guid ownerId)
        {
            var projectModifier = new Modifiers.CreateProject
            {
                Name = "Some name",
                OwnerId = ownerId,
                Projection = "USM"
            };

            _commandDispatcher.Send(projectModifier);

        }


        public ProjectModel ReadProject(Guid id)
        {
            var project = _objectReader
                .Read<ProjectModel>()
                .By(new GetProjectByOwnerId
                {
                    OwnerId = id
                });

            return project;
        }
        public OrmEnumBoxData[] StatusTestEnum
        {
            get => this._statusTestEnum;
            set => this.Set(ref this._statusTestEnum, value);
        }
    }
}
