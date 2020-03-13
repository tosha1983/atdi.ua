using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.IeStation
{
    [EntityPrimaryKeyAttribute]
    public interface IRefSpectrum_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IRefSpectrum : IRefSpectrum_PK
    {
        int IdNum { get; set; }
        string TableName { get; set; }
        int TableId { get; set; }
        long SensorId { get; set; }
        string GlobalSID { get; set; }
        double Freq_MHz { get; set; }
        double Level_dBm { get; set; }
        double? DispersionLow { get; set; }
        double? DispersionUp { get; set; }
        double? Percent { get; set; }
        string StatusMeas { get; set; }
        DateTime DateMeas { get; set; }
        IHeadRefSpectrum HEAD_REF_SPECTRUM { get; set; }
    }
}
