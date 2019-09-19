using Atdi.Contracts.WcfServices.Sdrn.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.Server.PrimaryHandlers.Models
{
    class InitOnlineMeasurementModel
    {
        public long Id { get; set; }

        public Guid ServerToken { get; set; }

        public OnlineMeasurementStatus Status { get; set; }

        public string SensorName { get; set; }

        public string SensorTechId { get; set; }
    }
}
