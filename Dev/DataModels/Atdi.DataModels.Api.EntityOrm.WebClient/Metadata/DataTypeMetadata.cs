using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EntityOrm.WebClient
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

		public bool Counter;

		public override string ToString()
		{
			if (string.IsNullOrEmpty(ClrType))
			{
				return $"{Name}";
			}
			return $"{Name}, {ClrType}";
		}
	}
}
