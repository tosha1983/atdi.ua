using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration.FilesImport
{
	internal class ImportFileInfo
	{
		public string Path;
		public string Name;
		public string Extension;
	}

	internal enum ImportFileResultStatus
	{
		NotProcessed = 0,
		Processed = 1,
		Refused = 2
	}
	internal class ImportFileResult
	{
		public string FileName;

		public ImportFileResultStatus Status;
	}
}
