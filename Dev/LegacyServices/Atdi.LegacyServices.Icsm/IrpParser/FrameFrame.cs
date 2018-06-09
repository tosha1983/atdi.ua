using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    public class frameFrame : frameobject
    {
        public frameFrame(frameobject v) { value = v; }
        public frameobject value;
    }
}
