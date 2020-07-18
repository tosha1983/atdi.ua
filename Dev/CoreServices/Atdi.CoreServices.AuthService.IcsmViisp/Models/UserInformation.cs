using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atdi.CoreServices.AuthService.IcsmViisp
{
    public class UserInformation
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string birthday { get; set; }
        public string companyName { get; set; }
    }
}
