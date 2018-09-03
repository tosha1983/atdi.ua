using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebApiServices.SelfHost.Controllers.Models
{
    public class HostInfo
    {
        public IComponentConfig[] Components { get; set; }

        public string Instance { get; set; }
    }
}
