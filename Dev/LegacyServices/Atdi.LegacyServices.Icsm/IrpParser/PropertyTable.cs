using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.LegacyServices.Icsm
{
    internal sealed class PropertyTable
    {
        public string FieldJoinFrom;
        public string FieldJoinTo;
        public string NameTableFrom;
        public string NameTableTo;
        public string NameFieldForSetValue;
        public string Name;
        public Orm.VarType TypeValue;
        public int Precision;
        public string DefaultValueFrom;
        public string DefaultValueTo;
        public int PrecisionFieldJoinFrom;
        public int PrecisionFieldJoinTo;
    }
}
