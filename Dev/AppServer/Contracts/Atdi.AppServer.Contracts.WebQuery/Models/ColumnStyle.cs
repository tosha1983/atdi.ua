using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace Atdi.AppServer.Contracts.WebQuery
{
    /// <summary>
    /// Represents the color of the column
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class ColumnStyle
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
