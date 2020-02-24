using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.AppUnits.Sdrn.CalcServer.DataModel
{
	internal class CalcTaskResultData
	{
		public long ProjectId;

		public long TaskId;

		public long ResultId;

		public CalcTaskType TaskType;

		public CalcTaskStatusCode TaskStatus;

		public CalcResultStatusCode ResultStatus;

		public string TaskOwnerInstance;

		public Guid TaskOwnerTaskId;

		public string CallerInstance { get; set; }

		public Guid CallerResultId { get; set; }
	}
}
