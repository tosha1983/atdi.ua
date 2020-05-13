using Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Api.EntityOrm.WebClient;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest.Adapters;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.EntityOrmTest
{

	[ViewXaml("EntityOrmTest.xaml")]
	[ViewCaption("Calc Server Client: Entity ORM Test")]
	public class View : WpfViewModelBase
	{
		private readonly ViewStarter _starter;
		private readonly ILogger _logger;

		public View(
			ProjectDataAdapter projectDataAdapter, 
			ViewStarter starter, 
			ILogger logger)
		{
			_starter = starter;
			_logger = logger;

			this.Projects = projectDataAdapter;
			this.Projects.Refresh();
		}


		public ProjectDataAdapter Projects { get; set; }

	}
}
