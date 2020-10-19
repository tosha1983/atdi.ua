using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface ICalcCheckPoint_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface ICalcCheckPoint : ICalcCheckPoint_PK
	{
		ICalcResult RESULT { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		string Name { get; set; }
	}

	public enum CalcCheckPointStatusCode
	{
		/// <summary>
		/// Точка восстановления только создана, но все данные еще не сериализованы в хранилище
		/// </summary>
		Created = 0,

		/// <summary>
		/// Точка восстановления сформирована, все данные удачно сериализованы а постоянно ехранилище и будет использоваться при восстановлении в случаи сбоя
		/// </summary>
		Available = 1,
		
	}

	
}
