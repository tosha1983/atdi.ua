using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WebApi.RestOrm.ORM.DTO
{
	public class RecordUpdateRequest : EntityRequest
	{
		public string[] Fields;

		public object[] Values;

		public string[] Filter;
	}
}
