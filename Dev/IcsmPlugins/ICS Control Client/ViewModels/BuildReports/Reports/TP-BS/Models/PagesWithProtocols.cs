using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;


namespace XICSM.ICSControlClient.Models.Views
{
    public class PagesWithProtocols
    {
        public string OperatorName { get; set; }

        public string PageName { get; set; }

        public string Standard { get; set; }

        public HeadProtocols[] HeadProtocols { get; set; }
    }

}
