using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.Workflows;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.SdrnServer
{
	internal class SdrnServerSyncJob : IJobExecutor
	{
		public JobExecutionResult Execute(JobExecutionContext context)
		{
			return JobExecutionResult.Completed;
		}
	}
}
