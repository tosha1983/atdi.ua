using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrmCs;

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
        public OrmVarType TypeValue;
        public int Precision;
    }
}
