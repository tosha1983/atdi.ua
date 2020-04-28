using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.DeepServices;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example
{
    public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnCalcServerExampleTasksAppUnit")
		{

		}

		protected override void OnInstallUnit()
		{
			
		}
	}
}
