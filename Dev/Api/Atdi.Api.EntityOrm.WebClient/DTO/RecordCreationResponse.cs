using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient.DTO
{

	public class RecordCreationResponse
	{
		public int Count;

		public object PrimaryKey;
	}

	public class RecordCreationResponse<TPrimaryKey>
	{
		public int Count;

		public TPrimaryKey PrimaryKey;
	}
}
