using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WcfServices.Sdrn.Server.Handlers.OnlineMeasurement
{
    class InitiationOnlineMesurementModel
    {
        public long Id { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid ServerToken { get; set; }

        public byte Status { get; set; }

        public int PeriodMinutes { get; set; }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? FinishTime { get; set; }
    }
}
