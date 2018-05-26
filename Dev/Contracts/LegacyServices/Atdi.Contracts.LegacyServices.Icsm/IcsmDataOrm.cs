using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.LegacyServices.Icsm
{
    public sealed class IcsmDataOrm : DataOrmBase
    {
        public IcsmDataOrm() : base("ICSM Data ORM")
        {
        }
    }
}
