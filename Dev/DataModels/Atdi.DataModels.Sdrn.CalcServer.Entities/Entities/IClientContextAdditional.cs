using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextAdditional_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextAdditional : IClientContextAdditional_PK
	{
		byte ModelTypeCode { get; set; }

		string ModelTypeName { get; set; }

		bool Available { get; set; }
	}


	public enum AdditionalModelTypeCode
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		
		/// <summary>
		/// ITU 1820 Model
		/// </summary>
		ITU1820 = 1,
	}
}
