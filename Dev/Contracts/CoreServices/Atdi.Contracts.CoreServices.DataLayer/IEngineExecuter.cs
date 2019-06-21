using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IEngineExecuter : IDisposable
    {
        void BeginTran(TransactionIsolationLevel isoLevel = TransactionIsolationLevel.Default);

        void Execute<TPattern>(TPattern queryPattern)
            where TPattern : class, IEngineQueryPattern;

        void Commit();

        void Rollback();

        TransactionIsolationLevel IsolationLevel { get; }

        bool HasTransaction { get; }
    }
}
