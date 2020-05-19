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
		}
		

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
	}
}
