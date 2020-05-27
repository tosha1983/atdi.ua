using Atdi.AppUnits.Sdrn.Infocenter.Integration.FilesImport;
using Atdi.Platform.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.Stations
{
	internal class GlobalIdentityPipelineHandler : IPipelineHandler<ImportFileInfo, ImportFileResult>
	{
		public ImportFileResult Handle(ImportFileInfo data, IPipelineContext<ImportFileInfo, ImportFileResult> context)
		{
			return context.GoAhead(data);
		}
	}
}
