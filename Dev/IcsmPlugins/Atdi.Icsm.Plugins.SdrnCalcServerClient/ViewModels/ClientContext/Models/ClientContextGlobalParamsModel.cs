using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext
{
    public class ClientContextGlobalParamsModel
    {
        public long ContextId { get; set; }
        public float? Time_pc { get; set; }
        public float? Location_pc { get; set; }
        public float? EarthRadius_km { get; set; }
    }
}
