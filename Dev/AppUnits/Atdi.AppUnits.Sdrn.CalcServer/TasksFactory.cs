using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal sealed class TasksFactory : ITasksFactory, IDisposable
	{
		public void Register(CalcTaskType taskType, Type handlerType)
		{
			throw new NotImplementedException();
		}

		public ITaskHandler Create(CalcTaskType taskType)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
