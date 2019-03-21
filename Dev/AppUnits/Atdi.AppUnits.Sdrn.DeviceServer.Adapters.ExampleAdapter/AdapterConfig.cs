using Atdi.Platform.AppComponent;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.ExampleAdapter
{
    public class AdapterConfig
    {
        [ComponentConfigProperty("Property.string")]
        public string Prop1 { get; set;}

        //[ComponentConfigProperty("Property.SharedSecret", SharedSecret = "SomeSecret")]
        //public string Prop2 { get; set; }


        public int? PropertyAsInt { get; set; }

        [ComponentConfigProperty("Property.float")]
        public float? Prop4 { get; set; }

        public double? PropertyAsDouble { get; set; }

        [ComponentConfigProperty("Property.decimal")]
        public decimal? PropAsDecimal { get; set; }

    }
}
