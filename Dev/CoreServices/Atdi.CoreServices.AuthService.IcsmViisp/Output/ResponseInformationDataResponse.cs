using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atdi.CoreServices.AuthService.IcsmViisp
{
    public class ResponseInformationDataResponse
    {
        public string AuthenticationProvider { get; set; }
        public AuthenticationAttribute[] AuthenticationAttribute { get; set; }
        public UserInformation UserInformation { get; set; }
        public string CustomData { get; set; }
    }
}
