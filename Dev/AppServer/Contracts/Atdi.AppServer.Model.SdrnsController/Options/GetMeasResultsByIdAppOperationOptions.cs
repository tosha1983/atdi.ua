using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.Sdrns;
    
namespace Atdi.AppServer.Models.AppServices.SdrnsController
{
    public class GetMeasResultsByIdAppOperationOptions : SdrnsControllerAppOperationOptionsBase
    {
        public MeasurementResultsIdentifier MeasResultsId;
    }
}
