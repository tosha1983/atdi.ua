using Atdi.DataModels.Api.DataBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.DataBus
{
    internal sealed class HandlingResultData : IHandlingResult
    {
        public MessageHandlingStatus Status { get; set; }
        public string Reason { get; set; }
        public string Detail { get; set; }
    }
}
