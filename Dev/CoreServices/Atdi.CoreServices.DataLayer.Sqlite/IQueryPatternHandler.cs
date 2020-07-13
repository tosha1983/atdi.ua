using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.Sqlite
{
    internal interface IQueryPatternHandler
    {

        void Handle<TPattern>(TPattern queryPattern, SqliteExecutor executor)
            where TPattern : class, IEngineQueryPattern;

    }

   
}
