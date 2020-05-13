using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EntityOrm.WebClient
{
	public class EntityMetadata
	{
		public string Name;

		public string QualifiedName;

		public string Namespace;

		public string Title;

		public string Desc;

		public int TypeCode;

		public string TypeName;

		public EntityMetadata BaseEntity;

		public EntityFieldMetadata[] Fields;

		public string[] PrimaryKey;

		public override string ToString()
		{
			if (BaseEntity == null)
			{
				return $"{Name}, {TypeName}, <- root";
			}
			return $"{Name}, {TypeName}, <- {BaseEntity}";
		}
	}
}
