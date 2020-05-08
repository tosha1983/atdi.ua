using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IMetadataSite
	{
		/// <summary>
		/// Метод получения методанных для сущности
		/// </summary>
		/// <param name="qualifiedName">Квалифицированное имя сущности</param>
		/// <returns>Объект методанных сущности</returns>
		EntityMetadata GetEntityMetadata(string qualifiedName);

		/// <summary>
		/// Метод получения методанных для сущности
		/// </summary>
		/// <typeparam name="TModel">Тип сущности</typeparam>
		/// <returns>Объект методанных сущности</returns>
		EntityMetadata GetEntityMetadata<TModel>();


	}

	public static class MetadataSiteExtension
	{
		public static EntityMetadata GetEntityMetadata(this IMetadataSite site, string ns, string name)
		{
			return site.GetEntityMetadata($"{ns}.{name}");
		}

	}
}
