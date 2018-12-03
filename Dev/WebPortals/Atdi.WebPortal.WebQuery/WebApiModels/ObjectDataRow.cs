using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;



namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// Represents the metadata to the column
    /// </summary>
    [DataContract]
    public class ObjectDataRow
    {
        [DataMember]
        public object[] Cells{ get; set; }
    }

}
