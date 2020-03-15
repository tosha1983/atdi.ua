using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IProject_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IProject : IProject_PK
	{
		string Name { get; set; }

		string Note { get; set; }

		string OwnerInstance { get; set; }

		Guid OwnerProjectId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		string Projection { get; set; }

	}

	public enum ProjectStatusCode
	{
		/// <summary>
		/// Проект создан но не доступна для использования
		/// В этой фазе обычно формируют остальные элементы структуры проекта 
		/// </summary>
		Created = 0,
		/// <summary>
		/// Проект изменяется
		/// </summary>
		Modifying = 1,
		/// <summary>
		/// Проект полностью сформирована, парамтеры определены и ее можно использовать для иницирования рассчетов
		/// </summary>
		Available = 2,
		/// <summary>
		/// Проект временно заблокирован для иницирования рассчетов
		/// </summary>
		Locked = 3,
		/// <summary>
		/// Проект неактуален
		/// </summary>
		Archived = 4
	}
}
