using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IGn06ArgsDefault_PK
	{
		long TaskId { get; set; }
	}

	[Entity]
	public interface IGn06ArgsDefault : IGn06ArgsBase, IGn06ArgsDefault_PK
	{
		IContextPlannedCalcTask TASK { get; set; }
	}
	
}
