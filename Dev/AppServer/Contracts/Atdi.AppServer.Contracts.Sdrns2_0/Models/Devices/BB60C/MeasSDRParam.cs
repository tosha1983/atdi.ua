using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.AppServer.Contracts.Sdrns2_0
{

    /// <summary>
    ///
    /// </summary>
    [DataContract(Namespace = ServicesSpecification.Namespace)]
    public class MeasSdrParam
    {
        [DataMember]
        public double RBW; //kHz
        [DataMember]
        public double VBW; //kHz
        [DataMember]
        public double MeasTime;//sec
        [DataMember]
        public double ref_level_dbm; // dBm 
        [DataMember]
        public DetectingType DetectTypeSDR;// Average -> bb_api.BB_AVERAGE;  othre -> bb_api.BB_MIN_AND_MAX
        [DataMember]
        public int PreamplificationSDR;//0, 10, 20, ..., dB
        [DataMember]
        public int RfAttenuationSDR; //0, 10, 20, 30, ...,  dB 
    }
}
