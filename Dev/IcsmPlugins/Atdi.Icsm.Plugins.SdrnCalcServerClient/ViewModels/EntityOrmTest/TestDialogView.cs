using Atdi.Icsm.Plugins.Core;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Queries;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest
{
	[ViewXaml("TestDialog.xaml", WindowState = FormWindowState.Normal, Width = 150, Height = 150)]
	[ViewCaption("Test Dialog")]
	public class TestDialogView : ViewBase
	{
		private readonly IEventBus _eventBus;
		private readonly IObjectReader _objectReader;
		private readonly ViewStarter _viewStarter;

		public TestDialogView(IEventBus eventBus, IObjectReader objectReader, ViewStarter viewStarter)
		{
			_eventBus = eventBus;
			_objectReader = objectReader;
			_viewStarter = viewStarter;
			_viewStarter.StartLongProcess(
				new LongProcessOptions()
				{
					CanStop = false,
					CanAbort = true,
					UseProgressBar = true,
					UseLog = true,
					IsModal = false,
					MinValue = 0,
					MaxValue = 1000,
					ValueKind = LongProcessValueKind.Absolute,
					Title = "Test processing ...",
					Note = "Some test processing." + Environment.NewLine + "Please control the log processes below."
				},
				token =>
				{

					for (int i = 0; i < token.Options.MaxValue; i++)
					{
						var project = _objectReader
							.Read<Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.ProjectModel>()
							.By(new GetProjectById
							{
								Id = 10
							});

						_eventBus.Send(new LongProcessLogEvent
						{
							ProcessToken = token,
							Message = $"{DateTimeOffset.Now} - #{i:D5} - Project: {project.Name}"
						});

						_eventBus.Send(new LongProcessUpdateEvent
						{
							ProcessToken = token,
							Value = i
						});

						System.Threading.Thread.Sleep(50);

						token.AbortToken.ThrowIfCancellationRequested();
					}

					

				});
		}
		public override void Dispose()
		{
			
		}
	}
}
