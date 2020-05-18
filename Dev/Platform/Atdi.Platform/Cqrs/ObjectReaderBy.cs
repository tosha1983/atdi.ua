using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Cqrs
{
	public class ObjectReaderBy<TResult> : IObjectReaderBy<TResult>
	{
		private readonly IServicesResolver _resolver;

		public ObjectReaderBy(IServicesResolver resolver)
		{
			_resolver = resolver;
		}

		public TResult By<TCriterion>(TCriterion criterion)
		{
			var executor = _resolver.Resolve<IReadQueryExecutor<TCriterion, TResult>>();
			return executor.Read(criterion);
		}
	}
}
