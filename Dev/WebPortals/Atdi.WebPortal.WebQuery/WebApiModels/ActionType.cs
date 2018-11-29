using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.WebPortal.WebQuery.WebApiModels
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public enum  ActionType
    {
        [EnumMember]
        Create,

        [EnumMember]
        Update,

        [EnumMember]
        Delete
    }
}
