using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.Platform.DependencyInjection;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal sealed class TasksFactory : ITasksFactory, IDisposable
	{
		private readonly IServicesResolver _resolver;
		private readonly ConcurrentDictionary<CalcTaskType, Type> _taskTypes;

		public TasksFactory(IServicesResolver resolver)
		{
			_resolver = resolver;
			_taskTypes= new ConcurrentDictionary<CalcTaskType, Type>();
		}

		public void Register(CalcTaskType taskType, Type handlerType)
		{
			if (_taskTypes.TryGetValue(taskType, out var item))
			{
				if (handlerType != item)
				{
					throw new InvalidOperationException($"The calculation task type '{taskType}' has already been previously registered with another type of handler '{item}'. New handler type is '{handlerType}'");
				}
				return;
			}

			if (!_taskTypes.TryAdd(taskType, handlerType))
			{
				if (_taskTypes.TryGetValue(taskType, out item))
				{
					if (handlerType != item)
					{
						throw new InvalidOperationException($"The calculation task type '{taskType}' has already been previously registered with another type of handler '{item}'. New handler type is '{handlerType}'");
					}
					return;
				}
				// страная ситуация(не смогли не добавить не прочитать, что то одно должно было получиться), которой в принципе быть не должно, но обработаем
				throw new InvalidOperationException($"Something went wrong with registration the calculation task type '{taskType}' with handler type is '{handlerType}'");
			}
			
		}

		public ITaskHandler Create(CalcTaskType taskType)
		{
			if (!_taskTypes.TryGetValue(taskType, out var type))
			{
				throw new InvalidOperationException($"Handler type not found for calculation type '{taskType}'");
			}

			return _resolver.Resolve(type) as ITaskHandler;
		}

		public void Dispose()
		{
			if (_taskTypes.Count > 0)
			{
				_taskTypes.Clear();
			}
		}
	}
}
