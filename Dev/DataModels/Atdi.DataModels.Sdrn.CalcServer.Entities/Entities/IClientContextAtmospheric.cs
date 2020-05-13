using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextAtmospheric_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextAtmospheric : IClientContextAtmospheric_PK
	{
		byte ModelTypeCode { get; set; }

		string ModelTypeName { get; set; }

		bool Available { get; set; }
	}


	public enum AtmosphericModelTypeCode
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Deygout 66 Model
		/// </summary>
		ITU838_530 = 1,

		/// <summary>
		/// Deygout 91 Model
		/// </summary>
		ITU678 = 2,

		/// <summary>
		/// ITU 526(15) Model
		/// </summary>
		ITU1820 = 3
	}
}
