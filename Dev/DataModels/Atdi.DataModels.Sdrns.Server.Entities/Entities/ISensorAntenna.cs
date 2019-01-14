using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ISensorAntenna
    {
        int Id { get; set; }
        int? SensorId { get; set; }
        string Code { get; set; }
        double? Slewang { get; set; }
        string Manufacturer { get; set; }
        string Name { get; set; }
        string TechId { get; set; }
        string AntDir { get; set; }
        double? HbeamWidth { get; set; }
        double? VbeamWidth { get; set; }
        string Polarization { get; set; }
        string UseType { get; set; }
        string Category { get; set; }
        string GainType { get; set; }
        double? GainMax { get; set; }
        double? LowerFreq { get; set; }
        double? UpperFreq { get; set; }
        double? AddLoss { get; set; }
        double? Xpd { get; set; }
        string AntClass { get; set; }
        string Remark { get; set; }
        string CustTxt1 { get; set; }
        string CustData1 { get; set; }
        double? CustNbr1 { get; set; }
        ISensor SENSOR { get; set; }
    }
}
