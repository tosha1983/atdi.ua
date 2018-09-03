using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.DependencyInjection
{
    public interface IHandlerResolver : IServicesResolver
    {
        void RegisterHandler<THandler>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where THandler : class;

        void RegisterHandler(Type handlerType, ServiceLifetime lifetime = ServiceLifetime.Transient);


        THandler ResolveHandler<THandler>();

        void ReleaseHandler<THandler>(THandler handler);
    }
}
