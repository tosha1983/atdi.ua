using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface ICalcCheckPointData_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface ICalcCheckPointData : ICalcCheckPointData_PK
	{
		ICalcResult CHECKPOINT { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		string DataContext { get; set; }

		string DataJson { get; set; }
	}

	

	
}
