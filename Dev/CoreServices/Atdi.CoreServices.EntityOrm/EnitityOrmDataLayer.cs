using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public sealed class EnitityOrmDataLayer : LoggedObject, IDataLayer<EntityDataOrm>
    {
        public EnitityOrmDataLayer(IDataLayer dataLayer, IEntityOrm entityOrm, ILogger logger) : base(logger)
        {
        }

        public IQueryBuilder Builder => throw new NotImplementedException();

        public IQueryExecutor Executor<TContext>() where TContext : IDataContext, new()
        {
            throw new NotImplementedException();
        }

        public IDataEngine GetDataEngine<TContext>() where TContext : IDataContext, new()
        {
            throw new NotImplementedException();
        }
    }
}
