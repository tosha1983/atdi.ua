using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.TechServices
{
    public interface ITechServicesHost : IDisposable
    {
        void Open();
        void Close();
    }
}
