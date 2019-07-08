using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Atdi.DataModels.Sdrns.Server;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{ 
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class LevelsByTaskParams
    {
        [DataMember]
        public List<long> MeasResultID;
        [DataMember]
        public int MeasTaskId;
        [DataMember]
        public int SectorId;
    }
}

