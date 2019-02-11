using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Atdi.DataModels.Sdrns.Server;
using Atdi.DataModels.CommonOperation;

namespace Atdi.Contracts.WcfServices.Sdrn.Server
{
    [ServiceContract(Namespace = Specification.Namespace)]
    public interface ISdrnsController
    {
    }
}
