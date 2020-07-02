using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
    [EntityPrimaryKey]
    public interface IGn06AllotmentOrAssignmentResult_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IGn06AllotmentOrAssignmentResult : IGn06AllotmentOrAssignmentResult_PK
    {
        long Gn06ResultId { get; set; }
        string Adm { get; set; }
        string TypeTable { get; set; }
        string Name { get; set; }
        double? Freq_MHz { get; set; }
        double? Longitude_DEC { get; set; }
        double? Latitude_DEC { get; set; }
        int? MaxEffHeight_m { get; set; }
        string Polar { get; set; }
        float? ErpH_dbW { get; set; }
        float? ErpV_dbW { get; set; }
        string AntennaDirectional { get; set; }
        string AdmRefId { get; set; }
    }
}