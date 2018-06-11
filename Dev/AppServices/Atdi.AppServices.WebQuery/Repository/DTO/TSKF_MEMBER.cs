using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServices.WebQuery.DTO
{
    internal sealed class TSKF_MEMBER
    {
        public int TSKF_ID { get; set; }

        public string APP_USER { get; set; }

        public TASKFORCE Taskforce { get; set; }
    }
}
