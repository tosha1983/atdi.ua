using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Tools.Sdrn.Monitoring
{
    public class HostInfo
    {
        public ComponentConfig[] Components { get; set; }
        public string Instance { get; set; }
    }
    public class ComponentConfig
    {
        public string Instance { get; set; }
        public string Type { get; set; }
        public string Assembly { get; set; }
        public ComponentConfigParameter[] Parameters { get; set; }
    }

    public class ComponentConfigParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
