using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IAmqpMessageLog_PK
	{
        long Id { get; set; }
    }

    [Entity]
    public interface IAmqpMessageLog : IAmqpMessageLog_PK
	{
		IAmqpMessage MESSAGE { get; set; }

		byte StatusCode { get; set; }

        string StatusName { get; set; }

        string StatusNote { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        int ThreadId { get; set; }

		string Source { get; set; }
	}
}
