using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQueryExecutor
    {
        TResult Fetch<TResult>(IQuerySelectStatement statement, Func<System.Data.IDataReader, TResult> handler);

    }
}
