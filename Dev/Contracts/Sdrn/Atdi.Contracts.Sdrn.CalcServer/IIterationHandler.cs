using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface IIterationHandler
	{

	}

	public interface IIterationHandler<in TData, out TResult> : IIterationHandler
	{
		TResult Run(ITaskContext taskContext, TData data);
	}
}
