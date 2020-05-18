using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Cqrs
{
	public interface ICommandDispatcher
	{
		void Send<TCommand>(TCommand command);

		void RegisterFrom(Assembly assembly);
	}
}
