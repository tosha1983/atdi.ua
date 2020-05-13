using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.WebApi.RestOrm.ORM.DTO
{
	public class DataTypeMetadata
	{
		public string Name;

		public int VarTypeCode;

		public string VarTypeName;

		public string ClrType;

		public long? Length;

		public int? Precision;

		public int? Scale;

		public bool Autonum;
	}

	public class FieldDescriptor
	{
		public int Index;

		public string Path;

		public DataTypeMetadata Type;
	}

	public class RecordReadResponse
	{
		public FieldDescriptor[] Fields { get; set; }

		public object[][] Records;

		public long Count;
	}
}
