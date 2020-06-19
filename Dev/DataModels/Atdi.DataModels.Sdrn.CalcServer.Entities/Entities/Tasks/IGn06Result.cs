using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
	[EntityPrimaryKey]
	public interface IGn06Result_PK
	{
		long ResultId { get; set; }
	}

	[Entity]
	public interface IGn06Result : IGn06Result_PK
	{
		ICalcResult RESULT { get; set; }

	}

}