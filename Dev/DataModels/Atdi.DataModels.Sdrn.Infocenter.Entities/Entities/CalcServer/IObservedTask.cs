using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.CalcServer
{
	[EntityPrimaryKey]
	public interface IObservedTask_PK
	{
		long Id { get; set; }
	}

	[Entity]
	public interface IObservedTask : IObservedTask_PK
	{
		DateTimeOffset CreatedDate { get; set; }

		DateTimeOffset? UpdatedDate { get; set; }

		long TaskId { get; set; }

		int TaskTypeCode { get; set; }

		string TaskTypeName { get; set; }

		byte TaskStatusCode { get; set; }

		string TaskStatusName { get; set; }

		long ResultId { get; set; }

		byte ResultStatusCode { get; set; }

		string ResultStatusName { get; set; }
	}
}
