using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebPortal.WebQuery.Licensing
{
    public class VerificationData
    {

        public string OwnerId { get; set; }

        public string ProductName { get; set; }

        public string ProductKey { get; set; }

        public DateTime Date { get; set; }

        public string LicenseType { get; set; }
    }
}
