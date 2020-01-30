using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer
{
	public class TaskLaunchHandle
	{
		/// <summary>
		/// Идентификатора ранее подготовленой задачи расчета
		/// </summary>
		public int TaskId { get; set; }

		public string CallerInstance { get; set; }

		public Guid ResultCode { get; set; }
	}
}
