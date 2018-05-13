using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.DependencyInjection
{
    public interface IServicesResolver
    {
        object Resolve(Type serviceType);

        T Resolve<T>();

        void Release(object instance);

    }
}
