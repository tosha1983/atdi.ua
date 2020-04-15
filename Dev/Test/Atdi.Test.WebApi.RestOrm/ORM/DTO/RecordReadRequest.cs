using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WebApi.RestOrm.ORM.DTO
{
	public class RecordReadRequest : EntityRequest
	{
		public string[] Select;

		public string[] OrderBy;

		public string[] Filter;

		public long Top;
	}
}
