using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class PageStyle
    {
        [DataMember]
        public string ForeColor;
        [DataMember]
        public string BackColor;
    }
}
