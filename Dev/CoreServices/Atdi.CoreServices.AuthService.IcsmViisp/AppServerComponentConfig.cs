using Atdi.Platform.AppComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.AuthService.IcsmViisp
{
	public class AppServerComponentConfig
	{
		[ComponentConfigProperty("VIISP.ServiceUrl")]
		public string ViispServiceUrl { get; set; }

        [ComponentConfigProperty("VIISP.PublicKey")]
        public string ViispPublicKey { get; set; }

        [ComponentConfigProperty("VIISP.SecretKey")]
        public string ViispSecretKey { get; set; }

        [ComponentConfigProperty("VIISP.PID")]
        public string ViispPID { get; set; }
        
    }
}
