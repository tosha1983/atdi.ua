using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.DeviceServer.Entities
{
    [Entity]
    public interface IMeasSDRResults
    {
        int Id { get; set; }
        int? MeasSubTaskId { get; set; }
        int? MeasSubTaskStationId { get; set; }
        int? MeasTaskId { get; set; }
        int? SensorId { get; set; }
        DateTime? DataMeas { get; set; }
        string Status { get; set; }
        string MeasDataType { get; set; }
        int? NN { get; set; }
        int? SwNumber { get; set; }
        int? IsSend { get; set; }
    }
}


