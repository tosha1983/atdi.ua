using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContextGlobalParams_PK
	{
		long ContextId { get; set; }
	}
	[Entity]
	public interface IClientContextGlobalParams : IClientContextGlobalParams_PK
	{
		float? Time_pc { get; set; }

		float? Location_pc { get; set; }

		float? EarthRadius_km { get; set; }
	}


	
}
