using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public class Field
    {
        public string Name;
        public FieldNature Nature;
        public Module Module;
        public Table Table;
        public Semant Special;
        public string Info;
        public DataDesc DDesc;
        public int Index;
        public int ClassIndex;
        public bool IsCompiledInEdition
        {
            get
            {
                return this.Module.IsInEdition;
            }
        }
    }
}
