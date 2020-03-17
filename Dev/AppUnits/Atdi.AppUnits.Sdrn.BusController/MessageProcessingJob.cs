using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.Contracts.Sdrn.Server.DevicesBus;
using Atdi.Platform.Logging;
using Atdi.Platform.Workflows;
using MD = Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.AppUnits.Sdrn.BusController
{
	internal class MessageProcessingJob : IJobExecutor
	{
		private readonly IMessagesSite _messagesSite;
		private readonly ILogger _logger;

		public MessageProcessingJob(
			IMessagesSite messagesSite,
			IDataLayer<EntityDataOrm> dataLayer,
			
			ILogger logger)
		{
			_messagesSite = messagesSite;
			_logger = logger;
		}

		public JobExecutionResult Execute(JobExecutionContext context)
		{
			_messagesSite.RedirectMessages();
			return JobExecutionResult.Completed;
		}
	}
}
