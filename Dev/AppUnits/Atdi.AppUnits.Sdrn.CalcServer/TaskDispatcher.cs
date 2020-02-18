using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal sealed class TaskDispatcher : ITaskDispatcher, IDisposable
	{
		public void RunTask(TaskLaunchHandle launchHandle, ITaskObserver observer)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
