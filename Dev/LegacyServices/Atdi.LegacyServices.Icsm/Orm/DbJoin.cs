using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class DbJoin
    {
        public string sTab;
        public string dTab;
        public string slTab;
        public string dlTab;
        public bool outer;
        public int nequ;
        public string name;
        public string[] sFld;
        public DbField[] sItem;
        public string[] dFld;
        public DbField[] dItem;
        public bool Equal(DbJoin jc)
        {
            if (this.nequ != jc.nequ)
            {
                return false;
            }
            for (int i = 0; i < this.nequ; i++)
            {
                if (this.sItem[i] != jc.sItem[i] || this.dItem[i] != jc.dItem[i])
                {
                    return false;
                }
            }
            return true;
        }
        public bool SameFields(DbJoin jc)
        {
            if (this.nequ != jc.nequ)
            {
                return false;
            }
            for (int i = 0; i < this.nequ; i++)
            {
                int num = 0;
                while (num < this.nequ && (!(jc.sFld[i] == this.sFld[num]) || !(jc.dFld[i] == this.dFld[num])))
                {
                    num++;
                }
                if (num == this.nequ)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
