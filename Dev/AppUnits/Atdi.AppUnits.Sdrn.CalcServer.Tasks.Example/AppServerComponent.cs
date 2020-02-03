using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.DeepServices;
using Atdi.AppUnits.Sdrn.CalcServer.Tasks.Example.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.CalcServer.DeepServices;
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


			// из контейнера получаем фабрику обработчиков задач
			var tasksFactory = this.Resolver.Resolve<ITasksFactory>();
			// регестрируем обработчик ассоцированый с конкретным расчетов
			tasksFactory.Register(CalcTaskType.FirstExampleTask, typeof(FirstTask));

		}
	}
}
