using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.DependencyInjection
{
    public enum ServiceLifetime
    {
        Transient,
        Singleton,
        PerThread,
        PerWebRequest,
        Pooled,
        PerWcfOperation,
        PerWcfSession
    }
}
