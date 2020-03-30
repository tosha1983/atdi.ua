using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextSubPathDiffraction_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextSubPathDiffraction : IClientContextSubPathDiffraction_PK
	{
		byte ModelTypeCode { get; set; }

		string ModelTypeName { get; set; }

		bool Available { get; set; }
	}


	public enum SubPathDiffractionModelTypeCode
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Sub Deygout 91 Model
		/// </summary>
		SubDeygout91 = 1                                             
	}
}
