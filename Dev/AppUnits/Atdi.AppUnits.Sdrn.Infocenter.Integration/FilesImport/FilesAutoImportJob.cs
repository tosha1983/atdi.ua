using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.FilesImport
{
	internal class FilesAutoImportJob : IJobExecutor
	{
		public JobExecutionResult Execute(JobExecutionContext context)
		{
			return JobExecutionResult.Completed;
		}
	}
}
