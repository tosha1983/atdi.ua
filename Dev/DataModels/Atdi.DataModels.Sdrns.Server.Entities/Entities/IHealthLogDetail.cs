using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IHealthLogDetail_PK
	{
        long Id { get; set; }
    }

    [Entity]
    public interface IHealthLogDetail : IHealthLogDetail_PK
	{
		IHealthLog HEALTH { get; set; }

        string Message { get; set; }

        string Note { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        int ThreadId { get; set; }

		string Source { get; set; }

		byte SiteTypeCode { get; set; }

		string SiteTypeName { get; set; }

		string SiteInstance { get; set; }

		string SiteHost { get; set; }
	}
}
