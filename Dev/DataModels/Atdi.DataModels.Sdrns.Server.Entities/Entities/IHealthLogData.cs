using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IHealthLogData_PK : IHealthLog_PK
	{
        long Id { get; set; }
    }

    [Entity]
    public interface IHealthLogData : IHealthLog, IHealthLogData_PK
{
		string JsonData { get; set; }
	}
}
