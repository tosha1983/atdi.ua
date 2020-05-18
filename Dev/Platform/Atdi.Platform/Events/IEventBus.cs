using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Events
{
	public interface IEventBus : IDisposable
	{
		IEventHandlerToken<TEvent> Subscribe<TEvent>(Action<TEvent> eventHandler);

		void Send<TEvent>(TEvent data);
	}
}
