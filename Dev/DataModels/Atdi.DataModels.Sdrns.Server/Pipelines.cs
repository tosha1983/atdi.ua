using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server
{
    public static class Pipelines
    {
        public static readonly string ClientMeasTasks = "SDRN.Client.MeasTasks";
        public static readonly string ClientCommands = "SDRN.Client.Commands";
        public static readonly string ClientInitOnlineMeasurement = "SDRN.Client.InitOnlineMeasurement";
        public static readonly string ClientSendEventOnlineMeasurement = "SDRN.Client.SendEventOnlineMeasurement";
        public static readonly string ClientMeasTaskSendEvents = "SDRN.Client.MeasTaskSendEventsPipebox";
        public static readonly string ClientCommandsSendEvents = "SDRN.Client.CommandsSendEventsPipebox";
        public static readonly string ClientRegisterAggregationServer = "SDRN.Client.RegisterAggregationServerPipebox";
        public static readonly string ClientDeviceCommandAggregationServer = "SDRN.Client.DeviceCommandAggregationServerPipebox";
    }
}
