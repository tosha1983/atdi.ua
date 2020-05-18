using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Cqrs
{
	public interface IReadQueryExecutor<in TCriterion, out TResult>
	{
		TResult Read(TCriterion criterion);
	}
}
