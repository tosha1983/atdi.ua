using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextClutter_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextClutter : IClientContextClutter_PK
	{
		byte ModelTypeCode { get; set; }

		string ModelTypeName { get; set; }

		bool Available { get; set; }
	}


	public enum ClutterModelTypeCode
	{
		/// <summary>
		/// Unknown Model
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// ITU 2109 Model
		/// </summary>
		ITU2109 = 1,

		/// <summary>
		/// Flat model
		/// </summary>
		Flat = 2
	}
}
