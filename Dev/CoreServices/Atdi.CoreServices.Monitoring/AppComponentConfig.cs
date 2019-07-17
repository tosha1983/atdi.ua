using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.Monitoring
{
    

    public class AppComponentConfig
    {
        [ComponentConfigProperty("LogEvent.BufferSize")]
        public int? LogEventBufferSize { get; set; }
    }
}
