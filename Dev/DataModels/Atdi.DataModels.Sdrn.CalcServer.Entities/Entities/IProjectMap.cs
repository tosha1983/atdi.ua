using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IProjectMap_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IProjectMap : IProjectMap_PK
	{
		IProject PROJECT { get; set; }

		string MapName { get; set; }

		string MapNote { get; set; }

		string OwnerInstance { get; set; }

		Guid OwnerMapId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		byte? SourceTypeCode { get; set; }

		string SourceTypeName { get; set; }

		string StepUnit { get; set; }

		int OwnerAxisXNumber { get; set; }

		int OwnerAxisXStep { get; set; }

		int OwnerAxisYNumber { get; set; }

		int OwnerAxisYStep { get; set; }

		int OwnerUpperLeftX { get; set; }

		int OwnerUpperLeftY { get; set; }

		int? AxisXNumber { get; set; }

		int? AxisXStep { get; set; }

		int? AxisYNumber { get; set; }

		int? AxisYStep { get; set; }

		int? UpperLeftX { get; set; }

		int? UpperLeftY { get; set; }

		int? LowerRightX { get; set; }

		int? LowerRightY { get; set; }

	}

	public enum ProjectMapStatusCode
	{
		/// <summary>
		/// заданы базовые реквизиты 
		/// </summary>
		Created = 0,
		/// <summary>
		/// Карта в процессе ожидания подготовки
		/// </summary>
		Pending = 1,
		/// <summary>
		/// Карта в процессе подготовки
		/// </summary>
		Processing = 2,
		/// <summary>
		/// Карта полностью подготовлена
		/// </summary>
		Prepared = 3,
		/// <summary>
		/// Карта не удалось подготовить
		/// </summary>
		Failed = 4,
		/// <summary>
		/// Карта более неактуальна для использования
		/// </summary>
		Archived = 5
	}

	public enum ProjectMapSourceTypeCode
	{
		Unknown = 0,

		/// <summary>
		/// Используемая карта размещена в локальном хранилище
		/// </summary>
		LocalStorage = 1,

		/// <summary>
		/// Используемая карта поставляется Ифоцентром
		/// </summary>
		Infocenter = 2
	}
}
