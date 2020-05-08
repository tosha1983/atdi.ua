using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient.DTO
{
	public class ApplyQueryRequest : EntityQueryRequest
	{
		public string[] FieldsToCreate;

		public object[] ValuesToCreate;

		public string[] FieldsToUpdate;

		public object[] ValuesToUpdate;

		public string[] Filter;
	}
}
