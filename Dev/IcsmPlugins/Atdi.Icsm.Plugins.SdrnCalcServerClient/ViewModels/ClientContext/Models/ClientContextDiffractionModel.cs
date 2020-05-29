using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext
{
    public class ClientContextDiffractionModel
    {
        public long ContextId { get; set; }
        public byte ModelTypeCode { get; set; }
        public string ModelTypeName { get; set; }
        public bool Available { get; set; }
    }
}
