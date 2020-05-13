using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextDiffraction_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextDiffraction : IClientContextDiffraction_PK
	{
		byte ModelTypeCode { get; set; }

		string ModelTypeName { get; set; }

		bool Available { get; set; }
	}


	public enum DiffractionModelTypeCode
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Deygout 66 Model
		/// </summary>
		Deygout66 = 1,

		/// <summary>
		/// Deygout 91 Model
		/// </summary>
		Deygout91 = 2,

		/// <summary>
		/// ITU 526(15) Model
		/// </summary>
		ITU526_15 = 3,

		/// <summary>
		/// Bullington Model
		/// </summary>
		Bullington = 4
	}
}
