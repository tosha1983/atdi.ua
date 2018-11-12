using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class LevelsByTaskParams
    {
        [DataMember]
        public List<int> MeasResultID;
        [DataMember]
        public int MeasTaskId;
        [DataMember]
        public int SectorId;
    }
}

