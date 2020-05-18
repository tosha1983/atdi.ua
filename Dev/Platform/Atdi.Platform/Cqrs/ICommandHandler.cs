using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Cqrs
{
	public interface ICommandHandler<in TCommand>
	{
		void Handle(TCommand command);
	}
}
