using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace XICSM.ICSControlClient.Models.Views
{
    public class PagesByOwnerAndStandard
    {
        public string PageName { get; set; }

        public string OwnerName { get; set; }

        public string Standard { get; set; }
    }

}
