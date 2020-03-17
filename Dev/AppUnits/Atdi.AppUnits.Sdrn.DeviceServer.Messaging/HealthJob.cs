using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.Sdrn.MessageBus;
using Atdi.Contracts.CoreServices.Monitoring;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using Newtonsoft.Json;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Messaging
{
	internal class HealthJob : IJobExecutor
	{
		private readonly IStatisticEntries _statisticEntries;
		private readonly IBusGate _busGate;
		private readonly ILogger _logger;

		public HealthJob(IStatisticEntries statisticEntries, IBusGate busGate, ILogger logger)
		{
			_statisticEntries = statisticEntries;
			_busGate = busGate;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			var data = new DM.SendHealthData()
			{
				CreatedTime = DateTimeOffset.Now,
				Event = DM.HealthLogEvent.HealthyData,
				ElementType = DM.EnvironmentElementType.DeviceServer,
				HostName = Environment.MachineName
			};
			if (context.IsRecovery == false && context.IsRepeat == false)
			{
				data.Event = DM.HealthLogEvent.Started;
			}
			
			var statistics = new List<object>();
			foreach (var entry in _statisticEntries)
			{
				statistics.Add(entry);
			}

			data.JsonData = JsonConvert.SerializeObject(statistics);

			using (var publisher = this._busGate.CreatePublisher("main"))
			{
				publisher.Send<DM.SendHealthData>("SendHealthData", data);
			}

			return JobExecutionResult.Completed;
		}
	}
}
