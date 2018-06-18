using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    [Flags]
    public enum IndexType
    {
        index_INDEX = 0,
        index_NODUPL = 1,
        index_PRIMARY = 3
    }
}
