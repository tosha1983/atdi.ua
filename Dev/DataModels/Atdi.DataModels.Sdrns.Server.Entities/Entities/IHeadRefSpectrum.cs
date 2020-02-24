using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IHeadRefSpectrum_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IHeadRefSpectrum : IHeadRefSpectrum_PK
    {
        string FileName { get; set; }
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        int? CountImportRecords { get; set; }
        double? MinFreqMHz { get; set; }
        double? MaxFreqMHz { get; set; }
        int? CountSensors { get; set; }
    }
}
