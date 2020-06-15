using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Cqrs
{
	public class ObjectReader : IObjectReader
	{
		private readonly IServicesResolver _resolver;
		private readonly IServicesContainer _container;

		public ObjectReader(IServicesResolver resolver, IServicesContainer container)
		{
			_resolver = resolver;
			_container = container;
		}
		public IObjectReaderBy<TResult> Read<TResult>()
		{
			return _resolver.Resolve<IObjectReaderBy<TResult>>();
		}

		public void RegisterFrom(Assembly assembly)
		{
			_container.RegisterServicesBasedOn(
				assembly,
				typeof(IReadQueryExecutor<,>),
				BaseOnMode.AllInterface,
				ServiceLifetime.PerThread
			);
		}
	}
}
