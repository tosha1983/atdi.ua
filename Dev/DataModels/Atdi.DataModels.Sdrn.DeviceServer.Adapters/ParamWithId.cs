using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Adapters
{
    public class ParamWithId
    {
        public ParamWithId()
        {
        }
        public int Id { get; set; }
        public string Parameter { get; set; }
    }
}
