using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.Server
{
    public sealed class SdrnServerDataContext : DataContextBase
    {
        public SdrnServerDataContext() : base("SDRN_Server_DB")
        {
        }
    }
}
