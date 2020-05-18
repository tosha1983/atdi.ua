using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Cqrs
{
	public interface IObjectReaderBy<out TResult>
	{
		TResult By<TCriterion>(TCriterion criterion);

	}
}
