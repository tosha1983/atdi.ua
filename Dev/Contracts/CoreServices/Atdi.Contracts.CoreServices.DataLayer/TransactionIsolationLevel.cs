using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public enum TransactionIsolationLevel
    {
        Default = 0,
        ReadUncommitted,
        ReadCommitted,
        RepeatableRead,
        Serializable,
        Snapshot
    }
}
