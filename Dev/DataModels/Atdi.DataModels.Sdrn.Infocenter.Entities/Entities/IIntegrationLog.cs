using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities.Entities
{
	public interface IIntegrationLog_PK
	{
		long Id { get; set; }
	}

	public interface IIntegrationLog : IIntegrationLog_PK
	{
		DateTimeOffset CreatedDate { get; set; }

		byte StatusCode { get; set; }

		string StatusName { get; set; }

		string StatusNote { get; set; }

		DateTimeOffset? StartTime{ get; set; }

		DateTimeOffset? FinishTime { get; set; }

		string DataSource { get; set; }

		string EntityName { get; set; }
	}
}
