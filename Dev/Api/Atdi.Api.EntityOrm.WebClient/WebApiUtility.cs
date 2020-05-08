using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient
{
	internal struct EntityQualifiedName
	{
		public string Namespace;

		public string Name;


	}

	internal static class WebApiUtility
	{

		public static EntityQualifiedName DecodeEntityQualifiedName(string qualifiedName)
		{
			if (string.IsNullOrEmpty(qualifiedName))
			{
				throw new ArgumentNullException(nameof(qualifiedName));
			}

			var nameParts = qualifiedName.Split('.');
			if (nameParts.Length == 1)
			{
				throw new InvalidOperationException($"Incorrect entity qualified name '{qualifiedName}'. Undefined namespace or entity name");
			}

			var entityName = nameParts[nameParts.Length - 1];
			var entityNamespace = qualifiedName.Substring(0, qualifiedName.Length - 1 - entityName.Length);

			if (string.IsNullOrEmpty(entityName))
			{
				throw new InvalidOperationException($"Incorrect entity full name '{qualifiedName}'. Undefined entity name");
			}
			if (string.IsNullOrEmpty(entityNamespace))
			{
				throw new InvalidOperationException($"Incorrect entity full name '{qualifiedName}'. Undefined entity namespace");
			}

			return new EntityQualifiedName()
			{
				Name = entityName,
				Namespace = entityNamespace
			};
		}

		public static EntityQualifiedName DecodeEntityQualifiedName<TEntity>()
		{
			var entityType = typeof(TEntity);
			var entityTypeName = entityType.Name;

			var entityName = (entityTypeName[0] == 'I' ? entityTypeName.Substring(1, entityTypeName.Length - 1) : entityTypeName);

			return new EntityQualifiedName()
			{
				Name = entityName,
				Namespace = entityType.Namespace
			};
		}
		

		public static string CombineUri(string uri1, string uri2)
		{
			uri1 = uri1.TrimEnd('/');
			uri2 = uri2.TrimStart('/');
			return $"{uri1}/{uri2}";
		}
	}
}
