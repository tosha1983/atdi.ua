using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface ISignalingSysInfo_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface ISignalingSysInfo : ISignalingSysInfo_PK
    {
        decimal Freq_Hz { get; set; }
        string Standart { get; set; }
        double? BandWidth_Hz { get; set; }
        double? Level_dBm { get; set; }
        int? CID { get; set; }
        int? MCC { get; set; }
        int? MNC { get; set; }
        int? BSIC { get; set; }
        int? ChannelNumber { get; set; }
        int? LAC { get; set; }
        int? RNC { get; set; } 
        double? CtoI { get; set; }
        double? Power { get; set; }
        IEmitting EMITTING { get; set; }
    }
}
