using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;

namespace Atdi.AppUnits.Sdrn.CalcServer
{
	internal sealed class IterationsPool: IIterationsPool, IDisposable
	{
		public void Register(Type handlerType)
		{
			throw new NotImplementedException();
		}

		public IIterationHandler<TData, TResult> GetIteration<TData, TResult>()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
