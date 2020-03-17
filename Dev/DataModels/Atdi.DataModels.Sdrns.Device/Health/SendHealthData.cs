using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Device
{
    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
    public class SendHealthData
	{
		[DataMember]
		public HealthLogEvent Event { get; set; }

		[DataMember]
		public string Note { get; set; }

		[DataMember]
		public DateTimeOffset CreatedTime { get; set; }

		[DataMember]
		public string JsonData{ get; set; }

		[DataMember]
		public EnvironmentElementType ElementType { get; set; }

		[DataMember]
		public string HostName { get; set; }
	}

    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
	public enum HealthLogEvent
    {
		[EnumMember]
	    Started = 1,
	    [EnumMember]
		Stopped = 2,
		[EnumMember]
		HealthyData = 3,
		[EnumMember]
		Heartbeat = 4
	}

    [DataContract(Namespace = Specification.Namespace)]
    [Serializable]
	public enum EnvironmentElementType
    {
	    [EnumMember]
		Unknown = 0,
		[EnumMember]
		SdrnServer = 1,
		[EnumMember]
		MasterServer = 2,
		[EnumMember]
		AggregationServer = 3,
		[EnumMember]
		DeviceServer = 4,
		[EnumMember]
		DeviceClientApi = 5,
    }

}
