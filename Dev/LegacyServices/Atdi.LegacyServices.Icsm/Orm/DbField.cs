using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public class DbField
    {
        public string Path;
        public string m_name;
        public string m_alias;
        public Field logFd;
        public string m_logTab;
        public string m_logFld;
        public string m_realTab;
        public string m_realFld;
        public int Index;
        public bool m_fetched;
        public object Val;
        public bool ValNull;
        public VarType LkType;
        public object[] LkArray;
        public bool[] LkNull;
        public int LkIndex;
        internal DbTable m_idxTable;
        public FieldFOption m_fldOptions;
        public bool m_OnlyValLinked;
        public DataDesc m_dataDesc;
        public Semant m_sp;

        public virtual string GetDataName()
        {
            return this.m_name;
        }

        public void Init(DataDesc desc, Semant sp, FieldFOption opt)
        {
            
            this.m_dataDesc = desc;
            this.m_fldOptions = opt;
            this.Val = null;
            this.ValNull = ((opt & FieldFOption.fld_NOTNULL) != FieldFOption.fld_NOTNULL);
            this.m_OnlyValLinked = false;
            this.m_sp = sp;
            this.LkType = VarType.var_Null;
        }
    }
}
