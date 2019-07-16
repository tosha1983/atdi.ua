using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.DataLayer.Assemblies
{
    internal interface IQueryPatternHandler
    {

        void Handle<TPattern>(TPattern queryPattern, ServiceObjectResolver resolver)
            where TPattern : class, IEngineQueryPattern;

    }

   
}
