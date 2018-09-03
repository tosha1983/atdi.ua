using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.LegacyServices.Icsm
{
    public sealed class IcsmDataContext : DataContextBase
    {
        public IcsmDataContext() : base("ICSM_DB")
        {
        }
    }
}
