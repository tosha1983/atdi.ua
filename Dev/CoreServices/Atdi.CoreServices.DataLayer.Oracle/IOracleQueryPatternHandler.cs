using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.Oracle
{
    internal interface IOracleQueryPatternHandler
    {

        void Handle<TPattern>(TPattern queryPattern, OracleExecuter executer)
            where TPattern : class, IEngineQueryPattern;

    }

   
}
