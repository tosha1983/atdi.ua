using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextAbsorption_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextAbsorption : IClientContextAbsorption_PK
	{
		byte ModelTypeCode { get; set; }

		string ModelTypeName { get; set; }

		bool Available { get; set; }
	}


	public enum AbsorptionModelTypeCode
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Flat Model
		/// </summary>
		Flat = 1,

		/// <summary>
		/// Linear Model
		/// </summary>
		Linear = 2,

		/// <summary>
		/// ITU 2109(2) Model
		/// </summary>
		ITU2109_2 = 3,

		/// <summary>
		/// Flat + Linear Model
		/// </summary>
		FlatAndLinear = 4,

		/// <summary>
		/// ITU 2109 + Linear Model
		/// </summary>
		ITU2109AndLinear = 5
	}
}
