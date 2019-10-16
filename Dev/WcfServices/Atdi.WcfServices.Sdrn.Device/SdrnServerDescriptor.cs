using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Device
{
    public class SdrnServerDescriptor
    {
        
        public SdrnServerDescriptor(IComponentConfig config, string sensorName)
        {
            this.SensorName = sensorName; // config.GetParameterAsString("Instance");
            this.SensorTechId = config.GetParameterAsString("SDRN.Device.SensorTechId");
            this.SdrnServer = config.GetParameterAsString("SDRN.Server.Instance");
            this.AllowedSensors = new Dictionary<string, string> {[sensorName] = sensorName};
        }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }

        public string SdrnServer{ get; set; }

        public IDictionary<string, string> AllowedSensors { get; set; }

    }
}
