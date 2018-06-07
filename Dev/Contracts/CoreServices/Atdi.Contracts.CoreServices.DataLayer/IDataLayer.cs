using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataLayer
    {
        IDataEngine GetDataEngine<TContext>()
            where TContext : IDataContext, new();
    }

    public interface IDataLayer<TDataOrm> : IDataLayer
        where TDataOrm : IDataOrm
    {
        IQueryBuilder Builder { get; }

        IQueryExecutor Executor<TContext>() 
            where TContext : IDataContext, new();
        
    }
}
