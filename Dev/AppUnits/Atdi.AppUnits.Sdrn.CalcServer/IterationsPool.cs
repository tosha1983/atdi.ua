using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Platform.DependencyInjection;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal sealed class IterationsPool: IIterationsPool, IDisposable
	{
		private readonly IServicesResolver _resolver;
		private readonly ConcurrentDictionary<string, IterationDescriptor> _iterationDescriptors;

		public IterationsPool(IServicesResolver resolver)
		{
			_resolver = resolver;
			_iterationDescriptors = new ConcurrentDictionary<string, IterationDescriptor>();
		}

		public void Register(Type handlerType)
		{
			var descriptor = new IterationDescriptor(handlerType);

			if (_iterationDescriptors.TryGetValue(descriptor.Key, out var item))
			{
				if (descriptor.HandlerType != item.HandlerType)
				{
					throw new InvalidOperationException($"The iteration handler type '{handlerType}' has already been previously registered with another type of handler '{item.HandlerType}'. New handler type is '{descriptor.HandlerType}'");
				}
				return;
			}

			if (!_iterationDescriptors.TryAdd(descriptor.Key, descriptor))
			{
				if (_iterationDescriptors.TryGetValue(descriptor.Key, out item))
				{
					if (descriptor.HandlerType != item.HandlerType)
					{
						throw new InvalidOperationException($"The iteration handler type '{handlerType}' has already been previously registered with another type of handler '{item.HandlerType}'. New handler type is '{descriptor.HandlerType}'");
					}
					return;
				}
				// страная ситуация(не смогли не добавить не прочитать, что то одно должно было получиться), которой в принципе быть не должно, но обработаем
				throw new InvalidOperationException($"Something went wrong with registration the iteration handler type '{handlerType}'");
			}
		}

		public IIterationHandler<TData, TResult> GetIteration<TData, TResult>()
		{
			var key = IterationDescriptor.MakeKey<TData, TResult>();
			if (!_iterationDescriptors.TryGetValue(key, out var descriptor))
			{
				throw new InvalidOperationException($"Handler type not found by '{key}'");
			}

			return _resolver.Resolve(descriptor.HandlerType) as IIterationHandler<TData, TResult>;
		}

		public void Dispose()
		{
			if (_iterationDescriptors.Count > 0)
			{
				_iterationDescriptors.Clear();
			}
		}
	}
}
