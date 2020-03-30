using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextMainBlock_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextMainBlock : IClientContextMainBlock_PK
	{
		byte ModelTypeCode { get; set; }

		string ModelTypeName { get; set; }
	}


	public enum MainBlockModelTypeCode
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// ITU 525 Model
		/// </summary>
		ITU525 = 1,

		/// <summary>
		/// ITU 1546 Model
		/// </summary>
		ITU1546 = 2,

		/// <summary>
		/// ITU 2001 Model
		/// </summary>
		ITU2001 = 3
	}
}
