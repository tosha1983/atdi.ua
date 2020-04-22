using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContext_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IClientContext : IClientContext_PK
	{
		IClientContext BASE_CONTEXT { get; set; }

		IProject PROJECT { get; set; }

		string Name { get; set; }

		string Note { get; set; }

		string OwnerInstance { get; set; }

		Guid OwnerContextId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte TypeCode { get; set; }

		string TypeName { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		IClientContextGlobalParams GLOBAL_PARAMS { get; set; }

		IClientContextMainBlock MAIN_BLOCK { get; set; }

		IClientContextDiffraction DIFFRACTION_BLOCK { get; set; }

		IClientContextSubPathDiffraction SUB_PATH_DIFFRACTION_BLOCK { get; set; }

		IClientContextTropo TROPO_BLOCK { get; set; }

		IClientContextDucting  DUCTING_BLOCK { get; set; }

		IClientContextAbsorption  ABSORPTION_BLOCK { get; set; }

		IClientContextReflection  REFLECTION_BLOCK { get; set; }

		IClientContextAtmospheric ATMOSPHERIC_BLOCK { get; set; }

		IClientContextAdditional ADDITIONAL_BLOCK { get; set; }

		IClientContextClutter CLUTTER_BLOCK { get; set; }

	}

	public enum ClientContextTypeCode
	{
		/// <summary>
		/// Контекст создан 
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Базовый контекст
		/// </summary>
		Base = 1,

		/// <summary>
		/// Обычный клиентский контекст 
		/// </summary>
		Client = 2
	}

	public enum ClientContextStatusCode
	{
		/// <summary>
		/// Контекст создан 
		/// </summary>
		Created = 0,

		/// <summary>
		/// Контекст изменяется
		/// </summary>
		Modifying = 1,

		/// <summary>
		/// Контекст в процессе ожидания подготовки
		/// </summary>
		Pending = 2,

		/// <summary>
		/// Контекст в процессе подготовки
		/// </summary>
		Processing = 3,
		
		/// <summary>
		/// Контекст полностью подготовлен и доступен для использования в рассчетах
		/// </summary>
		Prepared = 4,
		
		/// <summary>
		/// Контекст не удалось подготовить
		/// </summary>
		Failed = 5,
		
		/// <summary>
		/// Контекст более неактуален для использования в рассчетах
		/// </summary>
		Archived = 6
	}
}
