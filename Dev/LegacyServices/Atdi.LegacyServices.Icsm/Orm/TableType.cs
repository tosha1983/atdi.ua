using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    [Flags]
    public enum TableType
    {
        tbl_EXTERNAL = 1,
        tbl_DATA = 2,
        tbl_NODBCOPY = 4,
        tbl_SOURCER = 8,
        tbl_TEMPLATE_TBL = 16,
        tbl_TEMPLATE_VW = 32,
        tbl_TEMPLATE_UVW = 48,
        tbl_TEMPLATE_XX = 48,
        tbl_MEMORY = 128,
        tbl_WORKFLOW = 8192,
        tbl_UNMERGEABLE = 16384,
        tbl_OPT = 32768,
        tbl_AUDIT = 65536
    }
}
