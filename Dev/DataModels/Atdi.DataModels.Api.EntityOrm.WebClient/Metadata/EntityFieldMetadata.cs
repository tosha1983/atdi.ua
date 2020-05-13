using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Api.EntityOrm.WebClient
{
	public class EntityFieldMetadata
	{
		/// <summary>
		/// Имя поля или отношения
		/// </summary>
		public string Name;

		/// <summary>
		/// Отображаемое название поля или отношения
		/// </summary>
		public string Title;

		/// <summary>
		/// Описание сущности
		/// </summary>
		public string Desc;

		/// <summary>
		/// признак обязательности поля
		/// </summary>
		public bool Required;

		/// <summary>
		/// Тип источника определения значения поля
		/// </summary>
		public int SourceTypeCode;

		/// <summary>
		/// наименование типа источника определения значения поля
		/// </summary>
		public string SourceTypeName;

		/// <summary>
		/// Квалифицированное имя сущности владелеца поля
		/// </summary>
		public string Entity;

		/// <summary>
		/// Квалифицированное имя базовой сущности, с которого поле было наследовано
		/// </summary>
		public string BaseEntity;


		/// <summary>
		/// Описание типа поля в случаи обычной колонки
		/// </summary>
		public DataTypeMetadata DataType;

		/// <summary>
		/// Единица измерения значения поля в случаи обычного поля
		/// </summary>
		public UnitMetadata Unit;

		/// <summary>
		/// Квалифицированное имя сущности на которую определено поле-ссылка
		/// </summary>
		public string RefEntity;

		/// <summary>
		/// Квалифицированное имя сущности  расширение, определяется в полях-расширениях 
		/// </summary>
		public string ExtensionEntity;

		/// <summary>
		/// Квалифицированное имя сущности с которым есть отношение/связь, определяется в полях-отношений
		/// </summary>
		public string RelatedEntity;

		public override string ToString()
		{
			if (DataType == null)
			{
				return $"{Name}, {SourceTypeName} -> {RefEntity}{ExtensionEntity}{RelatedEntity}";
			}

			if (Unit == null)
			{
				return $"{Name}, {SourceTypeName}, Type='{DataType}'";
			}
			return $"{Name}, {SourceTypeName}, Type='{DataType}', Unit='{Unit}'";
		}
	}
}
