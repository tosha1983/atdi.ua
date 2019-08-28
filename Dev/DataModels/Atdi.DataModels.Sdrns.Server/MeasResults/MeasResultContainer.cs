using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Device;

namespace Atdi.DataModels.Sdrns.Server
{
    [Serializable]
    public class MeasResultContainer
    {
        public MeasResults MeasResult { get; set; }
    }
}
