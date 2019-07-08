    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataLayerScope: IDisposable
    {
        void BeginTran(TransactionIsolationLevel isoLevel = TransactionIsolationLevel.Default);

        IQueryExecutor Executor { get; }

        void Commit();

        void Rollback();

        TransactionIsolationLevel IsolationLevel { get; }

        bool HasTransaction { get; }
    }

    public interface IDataLayerScope<TContext> : IDataLayerScope
        where TContext : IDataContext, new()
    {
    }
}
