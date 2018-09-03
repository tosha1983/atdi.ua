using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class DbTable
    {
        public string tbNameInDb;
        public Table zetb;
        public string logTab;
        public string alias;
        public TotoDBR tmp;
        public string Tcaz;
        public bool aliasUsed;

        public string Key
        {
            get
            {
                if (this.alias == null)
                {
                    return this.logTab;
                }
                return this.alias;
            }
        }
    }

    public sealed class TotoDBR
    {
        public string nam;
        public string cond;
        public DbTable outerEd;
        public int NbLeft;
    }
}
