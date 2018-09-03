using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    [Flags]
    public enum TableCat
    {
        cat_ICSMANAGER = 0,
        cat_FACSMAB = 256,
        cat_TXRX = 512,
        cat_CONO = 768,
        cat_DIRISI = 1024,
        cat_SYST = 1280,
        cat_BRIFIC = 1536,
        cat_SRS = 1792,
        cat_ICSTEL = 2048,
        cat_NEWMRFL = 2304,
        cat_NEWBIAF = 2560,
        cat_OLDBIAF = 2816,
        cat_OLDMRFL = 3072,
        cat_WRAP = 3328,
        cat_WIENFIX = 3584,
        cat_FML = 3840,
        cat_GE06D = 4096,
        cat_EXTERNAL1 = 4608,
        cat_EXTERNAL2 = 4864,
        cat_EXTERNAL3 = 5120,
        cat_EXTERNAL4 = 5376
    }
}
