using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IHealthLog_PK
	{
        long Id { get; set; }
    }

    [Entity]
    public interface IHealthLog : IHealthLog_PK
	{
		long? SenderLogId { get; set; }

		byte? SenderTypeCode { get; set; }

        string SenderTypeName { get; set; }

        string SenderInstance { get; set; }

        string SenderHost { get; set; }


		byte SourceTypeCode { get; set; }

        string SourceTypeName { get; set; }

        string SourceInstance { get; set; }

        string SourceTechId { get; set; }

        string SourceHost { get; set; }

		byte EventCode { get; set; }

        string EventName { get; set; }

        string EventNote { get; set; }

		DateTimeOffset DispatchTime { get; set; }

		DateTimeOffset ReceivedTime { get; set; }

		DateTimeOffset? ForwardedTime { get; set; }

    }

    public enum EnvironmentElementTypeCode
	{
		Unknown = 0,

		SdrnServer = 1,

		MasterServer = 2,

		AggregationServer = 3,

		DeviceServer = 4,

		DeviceClientApi = 5,
	}

    public enum EnvironmentEventCode
    {
	    Unknown = 0,

		Started = 1,

	    Stopped = 2,

		HealthyData = 3,

		Heartbeat = 4
	}
}
