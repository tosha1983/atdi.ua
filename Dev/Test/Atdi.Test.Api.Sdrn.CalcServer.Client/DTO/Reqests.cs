using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm.Api;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.DTO
{
	class RecordCreateRequest : IDataRecordCreateRequest
	{
		public string Context { get; set; }
		public string Namespace { get; set; }
		public string Entity { get; set; }
		public string[] Fields { get; set; }
		public object[] Values { get; set; }
	}


	class RecordCreateResult : IRecordCreateResult
	{
		public int Count { get; set; }
		public object PrimaryKey { get; set; }
	}

	class SimplePrimaryKey
	{
		public long Id { get; set; }
	}
	class SimplePkRecordCreateResult 
	{
		public int Count { get; set; }

		public SimplePrimaryKey PrimaryKey { get; set; }
	}
}
