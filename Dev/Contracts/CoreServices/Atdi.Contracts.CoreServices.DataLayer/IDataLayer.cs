using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataLayer
    {
        IDataEngine GetDataEngine<TContext>()
            where TContext : IDataContext, new();

        IDataEngine GetDataEngine(IDataContext dataContext);

    }

    public interface IDataLayer<TDataOrm> //: IDataLayer
        where TDataOrm : IDataOrm
    {
        IQueryBuilder Builder { get; }

        IQueryExecutor Executor<TContext>() 
            where TContext : IDataContext, new();

        IQueryBuilder<TModel> GetBuilder<TModel>();

        IDataLayerScope CreateScope<TContext>()
             where TContext : IDataContext, new();

        IDataLayerScope CreateScope(IDataContext dataContext);
    }

    
    
}
