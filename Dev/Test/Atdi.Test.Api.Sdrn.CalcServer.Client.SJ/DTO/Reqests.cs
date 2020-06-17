using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm.Api;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.DTO
{
	class RecordReadRequest : IDataRecordRequest
	{
		public string Context { get; set; }
		public string Namespace { get; set; }
		public string Entity { get; set; }
		public string PrimaryKey { get; set; }
		public string[] Select { get; set; }
		public string[] Filter { get; set; }
	}

	class RecordCreateRequest : IDataRecordCreateRequest
	{
		public string Context { get; set; }
		public string Namespace { get; set; }
		public string Entity { get; set; }
		public string[] Fields { get; set; }
		public object[] Values { get; set; }
	}

	class RecordUpdateRequest : IDataRecordUpdateRequest
	{
		public string Context { get; set; }
		public string Namespace { get; set; }
		public string Entity { get; set; }
		public string PrimaryKey { get; set; }
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

	class ObjectPkRecordCreateResult
	{
		public int Count { get; set; }

		public object PrimaryKey { get; set; }
	}

	class RecordResult 
	{
		public FieldDescriptor[] Fields { get; set; }

		public object[] Record { get; set; }
	}

	class FieldDescriptor 
	{
		public string Path { get; set; }

		public DataTypeMetadata Type { get; set; }

		public int Index { get; set; }

	}

	class DataTypeMetadata : IDataTypeMetadata
	{
		public string Name { get; set; }
		public int VarTypeCode { get; set; }
		public string VarTypeName { get; set; }
		public string ClrType { get; set; }
		public long? Length { get; set; }
		public int? Precision { get; set; }
		public int? Scale { get; set; }
		public bool Autonum { get; set; }
	}
}
