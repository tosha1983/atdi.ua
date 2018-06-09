using Atdi.Platform.DependencyInjection;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Atdi.WebApiServices.SelfHost
{
    public sealed class WebApiControllerActivator : LoggedObject, IHttpControllerActivator
    {
        private readonly IServicesResolver _resolver;

        public WebApiControllerActivator(IServicesResolver resolver, ILogger logger) : base(logger)
        {
            this._resolver = resolver;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            return (IHttpController)this._resolver.Resolve(controllerType);
        }
    }
}
