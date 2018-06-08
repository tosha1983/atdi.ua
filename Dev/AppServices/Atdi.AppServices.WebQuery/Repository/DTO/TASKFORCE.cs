using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery.DTO
{
    internal sealed class TASKFORCE
    {
        public int ID { get; set; }

        public string CODE { get; set; }

        public string SHORT_NAME { get; set; }

        public string FULL_NAME { get; set; }

        public string DESCRIPTION { get; set; }
        public int CUST_CHB1 { get; set; }
        public int CUST_CHB2 { get; set; }
    }
}
