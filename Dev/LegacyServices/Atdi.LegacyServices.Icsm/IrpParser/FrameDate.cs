using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    public class frameDate : frameobject
    {
        public frameDate(DateTime v) { value = v; }
        public DateTime value;
    }
}
