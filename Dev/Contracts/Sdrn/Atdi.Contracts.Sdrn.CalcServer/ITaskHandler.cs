using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TaskHandlerAttribute : Attribute
	{
		public CalcTaskType CalcType { get; }

		public TaskHandlerAttribute(CalcTaskType calcType)
		{
			this.CalcType = calcType;
		}
	}
	/// <summary>
	/// Интерфейс обработчика задачи расчета
	/// </summary>
	public interface ITaskHandler : IDisposable
	{
		void Load(ITaskContext taskContext);

		void Run();
	} 
}
