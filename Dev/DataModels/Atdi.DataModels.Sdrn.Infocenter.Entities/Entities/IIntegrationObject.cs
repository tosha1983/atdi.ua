using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities
{
	public interface IIntegrationObject_PK
	{
		long Id { get; set; }
	}
	public interface IIntegrationObject : IIntegrationObject_PK
	{
		string DataSource { get; set; }

		string ObjectName { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		byte[] SyncKeyContent { get; set; }

		string SyncKeyType { get; set; }

		string SyncKeyNote { get; set; }

		DateTimeOffset? LastSyncTime { get; set; }
	}
}
