using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.CalcServer.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
    public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnCalcServerTasksAppUnit")
		{

		}

		protected override void OnInstallUnit()
		{
			
		}
	}
}
