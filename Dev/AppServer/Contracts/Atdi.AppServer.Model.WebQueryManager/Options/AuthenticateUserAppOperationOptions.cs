using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts;
using Atdi.AppServer.Contracts.WebQuery;

namespace Atdi.AppServer.Models.AppServices.WebQueryManager
{
    public class AuthenticateUserAppOperationOptions : WebQueryManagerAppOperationOptionsBase
    {
        public string UserName;
        public string Password;
    }
}
