using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.DependencyInjection
{
    public interface IWcfServicesResolver : IServicesResolver
    {
        TServiceHost CreateWcfServiceHost<TServiceHost>(string constructorString, Uri[] baseAddresses)
            where TServiceHost : class;
    }

    public static class ServicesContainerExtensions
    {
        public static TServiceHost CreateWcfServiceHost<TServiceHost, TContract>(this IWcfServicesResolver container)
            where TServiceHost : class
        {
            return container.CreateWcfServiceHost<TServiceHost>(typeof(TContract).AssemblyQualifiedName, new Uri[0]);
        }
    }
}
