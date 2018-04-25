using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the view style of the table of result data
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class QueryTableStyle
    {
        [DataMember]
        public string ForeColor;
        [DataMember]
        public string BackColor;
        [DataMember]
        public string FontName;
        [DataMember]
        public string FontStyle;
        [DataMember]
        public uint FontSize;
    }
}
