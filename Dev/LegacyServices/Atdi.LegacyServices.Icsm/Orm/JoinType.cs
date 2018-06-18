using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public enum JoinType
    {
        fldj_NONE = 0,
        fldj_SGBD = 1,
        fldj_CASCDEL = 3,
        fldj_RCOMP = 4,
        fldj_NOIMPORT = 8
    }
}
