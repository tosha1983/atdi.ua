using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Processing
{
    [Serializable]
    public class LastUpdate
    {
        public string TableName { get; set; }
        public DateTime? LastDateTimeUpdate { get; set; }
        public string Status { get; set; }
    }
}
