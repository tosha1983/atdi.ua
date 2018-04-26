using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDR = Atdi.AppServer.Contracts.Sdrns;
using VM = XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;

namespace XICSM.ICSControlClient.Models.WcfDataApadters
{
    public class ShortMeasTaskDataAdatper : WpfDataAdapter<SDR.ShortMeasTask, VM.ShortMeasTaskViewModel, ShortMeasTaskDataAdatper>
    {
        protected override Func<SDR.ShortMeasTask, VM.ShortMeasTaskViewModel> GetMapper()
        {
            return source => new VM.ShortMeasTaskViewModel
            {
                CreatedBy = source.CreatedBy,
                DateCreated = source.DateCreated.ToNull(),
                ExecutionMode = source.ExecutionMode,
                Id = source.Id.Value,
                MaxTimeBs = source.MaxTimeBs.ToNull(),
                Name = source.Name,
                OrderId = source.OrderId,
                Prio = source.Prio.ToNull(),
                ResultType = source.ResultType,
                Status = source.Status,
                Task = source.Task,
                Type = source.Type,
                TypeMeasurements = source.TypeMeasurements
            };
        }


    }
}
