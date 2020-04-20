using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IContextPlannedCalcTask_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IContextPlannedCalcTask : IContextPlannedCalcTask_PK
	{
		IClientContext CONTEXT { get; set; }

		int TypeCode { get; set; }

		string TypeName { get; set; }

		int StartNumber { get; set; }

		string MapName { get; set; }

		string Note { get; set; }
	}

	
}
