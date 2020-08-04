using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using Atdi.Platform.AppComponent;
using Atdi.Platform.DependencyInjection;

namespace Atdi.WebApiServices.SelfHost
{
    public sealed class SelfHostComponent : WebApiServicesComponent
    {
        private HttpSelfHostConfiguration _config;
        private HttpSelfHostServer _server;

        public SelfHostComponent() : base("SelfHostWebApiServices", ComponentBehavior.Simple)
        {
        }

        protected override void OnInstall()
        {
            base.OnInstall();

            this._config = new HttpSelfHostConfiguration(this.Config["Url"].ToString());
            this._config.MaxReceivedMessageSize = 50 * 1024 * 1024;
			this.Container.RegisterBoth<IRoutesConfig, RoutesConfig>(ServiceLifetime.Singleton);
            this.Container.Register<IHttpControllerActivator, WebApiControllerActivator>(ServiceLifetime.Transient);
        }

        protected override void OnUninstall()
        {
            if (this._server != null)
            {
                CloseServer();
            }
            base.OnUninstall();
        }


        protected override void OnActivate()
        {
            base.OnActivate();
            var resolver = this.Container.GetResolver<IServicesResolver>();

            var routersConfig = resolver.Resolve<RoutesConfig>();
            routersConfig.Apply(this._config.Routes);

            this._config.MapHttpAttributeRoutes();
            this._config.Routes.MapHttpRoute(
                name: "DefaultApiRoute",
                routeTemplate: "api/{controller}/{action}",
                defaults: null
            );

            var appXmlType = this._config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            this._config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            var controllerActivator = resolver.Resolve<IHttpControllerActivator>();
            this._config.Services.Replace(typeof(IHttpControllerActivator), controllerActivator);
            this._server = new HttpSelfHostServer(this._config);
            _server.OpenAsync().Wait();



        }

        protected override void OnDeactivate()
        {
            if (this._server != null)
            {
                CloseServer();
            }
            base.OnDeactivate();
        }

        private void CloseServer()
        {
            this._server.CloseAsync().Wait();
            this._server.Dispose();
            this._server = null;
        }
    }
}
