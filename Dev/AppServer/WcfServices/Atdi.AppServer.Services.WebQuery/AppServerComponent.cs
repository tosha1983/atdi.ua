using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Services.WebQuery
{
    public class AppServerComponent : AppServerComponentBase
    {
        public AppServerComponent() 
            : base(AppServerComponentType.TechService, "WebQuery")
        {
        }
    }
}
