using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface IResSysInfoRaw
    {
        int Id { get; set; }
        double? Bandwidth { get; set; }
        int? BaseId { get; set; }
        int? Bsic { get; set; }
        int? ChannelNumber { get; set; }
        int? Cid { get; set; }
        double? Code { get; set; }
        double? Ctoi { get; set; }
        int? Eci { get; set; }
        int? Enodebid { get; set; }
        double? Freq { get; set; }
        double? Icio { get; set; }
        double? InbandPower { get; set; }
        double? Iscp { get; set; }
        int? Lac { get; set; }
        double? Agl { get; set; }
        double? Asl { get; set; }
        double? Lon { get; set; }
        double? Lat { get; set; }
        int? Mcc { get; set; }
        int? Mnc { get; set; }
        int? Nid { get; set; }
        int? Pci { get; set; }
        int? Pn { get; set; }
        double? Power { get; set; }
        double? Ptotal { get; set; }
        int? Rnc { get; set; }
        double? Rscp { get; set; }
        double? Rsrp { get; set; }
        double? Rsrq { get; set; }
        int? Sc { get; set; }
        int? Sid { get; set; }
        int? Tac { get; set; }
        string TypeCdmaevdo { get; set; }
        int? Ucid { get; set; }
        int? ResStGeneralId { get; set; }
        IResStGeneralRaw RESSTGENERAL { get; set; }
    }
}
