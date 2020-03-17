using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server
{
	[Serializable]
	public class HealthDataDetail
	{
		public string Message { get; set; }

		public string Note { get; set; }

		public DateTimeOffset CreatedDate { get; set; }

		public int ThreadId { get; set; }

		public string Source { get; set; }

		public byte SiteTypeCode { get; set; }

		public string SiteTypeName { get; set; }

		public string SiteInstance { get; set; }

		public string SiteHost { get; set; }
	}
}
