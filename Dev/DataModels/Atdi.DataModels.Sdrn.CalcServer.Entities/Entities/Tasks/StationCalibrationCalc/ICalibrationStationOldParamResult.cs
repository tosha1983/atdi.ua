using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.EntityOrm;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks
{
    [EntityPrimaryKey]
    public interface ICalibrationStationParamByMeasResult_PK
    {
        long ParamId { get; set; }
    }

    [Entity]
    public interface ICalibrationStationOldParamResult : ICalibrationStationParamBase, ICalibrationStationParamByMeasResult_PK
    {

    }

}