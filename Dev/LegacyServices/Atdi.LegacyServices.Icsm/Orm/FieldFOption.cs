using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    [Flags]
    public enum FieldFOption
    {
        fld_NONE = 0,
        fld_NOTNULL = 1,
        fld_PRIMARY = 2,
        fld_FKEY = 8,
        fld_CONSTANT = 17,
        fld_OPT = 64,
        fld_VLEN = 128,
        fld_ZEROLENGTHNOTALLOWED = 256,
        fld_PAST = 512,
        fld_OBSOLETE = 1024,
        fld_AUTONUMBER = 2048,
        fld_XMLSIGNED = 4096,
        fld_XMLEMPTYISNULL = 8192
    }
}
