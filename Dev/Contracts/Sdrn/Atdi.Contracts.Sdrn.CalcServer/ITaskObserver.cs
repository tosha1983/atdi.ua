using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.CalcServer
{
	public interface ITaskObserver
	{
		void OnCompleted(ICalcContextHandle context);

		void OnEvent(ICalcContextHandle context, ICalcEvent @event);

	}
}
