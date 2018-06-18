using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public enum VarType
    {
        var_Null,
        var_String = 2,
        var_Dou,
        var_Flo,
        var_Int,
        var_Tim,
        var_Bytes,
        var_Ref = 9,
        var_Guid
    }
}
