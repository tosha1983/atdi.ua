using Atdi.Platform;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Cqrs;
using Atdi.Icsm.Plugins.Core;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc
{
	public class AppComponent : ComponentBase
	{
		public AppComponent() 
			: base("IcsmPluginsCore", ComponentType.IcsmPlugin, ComponentBehavior.SingleInstance)
		{

		}

		protected override void OnInstall()
		{
			
			this.Container.Register<ViewStarter>(ServiceLifetime.Singleton);
			
		}

		protected override void OnActivate()
		{
			
		}
	}
}
