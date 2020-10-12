using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface ITaskDispatcher
	{
		void RunTask(TaskLaunchHandle launchHandle, ITaskObserver observer);

		void RunTask(long resultId, ITaskObserver observer);

		void ResumeTask(long resultId, ITaskObserver observer);

		void StopTask(long resultId);

		void AbortTask(long resultId);
	} 
}
