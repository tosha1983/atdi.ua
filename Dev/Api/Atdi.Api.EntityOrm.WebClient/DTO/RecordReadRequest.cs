using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient.DTO
{
	public class RecordReadRequest : EntityRequest
	{
		public string[] Select;

		public string[] OrderBy;

		public string[] Filter;

		public long Top;

		public long? Offset;

		public long? Fetch;

		public bool Distinct;
	}
}
