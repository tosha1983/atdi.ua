using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient.DTO
{

	public class ApplyQueryResponse
	{
		public int Count;

		public object PrimaryKey;
	}

	public class ApplyQueryResponse<TPrimaryKey>
	{
		public int Count;

		public TPrimaryKey PrimaryKey;
	}
}
