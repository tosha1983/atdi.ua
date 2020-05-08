using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient.DTO
{
	public class DeleteQueryRequest : EntityQueryRequest
	{
		public string[] Filter;
	}
}
