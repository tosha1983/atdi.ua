using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Atdi.WebApiServices.SelfHost
{
    public sealed class RoutesConfig : IRoutesConfig
    {
        private class Entry
        {
            public string Name;
            public string Template;
            public object Defaults;
            public object Constraints;
            public HttpMessageHandler Handler;
        }

        private List<Entry> _routes;

        public RoutesConfig()
        {
            this._routes = new List<Entry>();
        }

        public void Registry(string name, string routeTemplate)
        {
            this._routes.Add(new Entry
            {
                Name = name,
                Template = routeTemplate
            });
        }

        public void Registry(string name, string routeTemplate, object defaults)
        {
            this._routes.Add(new Entry
            {
                Name = name,
                Template = routeTemplate,
                Defaults = defaults
            });
        }

        public void Registry(string name, string routeTemplate, object defaults, object constraints)
        {
            this._routes.Add(new Entry
            {
                Name = name,
                Template = routeTemplate,
                Defaults = defaults,
                Constraints = constraints
            });
        }

        public void Registry(string name, string routeTemplate, object defaults, object constraints, HttpMessageHandler handler)
        {
            this._routes.Add(new Entry
            {
                Name = name,
                Template = routeTemplate,
                Defaults = defaults,
                Constraints = constraints,
                Handler = handler
            });
        }

        public void Apply(HttpRouteCollection routes)
        {
            this._routes.ForEach(r =>
            {
                if (r.Handler != null)
                {
                    routes.MapHttpRoute(r.Name, r.Template, r.Defaults, r.Constraints, r.Handler);
                }
                else if (r.Constraints != null)
                {
                    routes.MapHttpRoute(r.Name, r.Template, r.Defaults, r.Constraints);
                }
                else if (r.Defaults != null)
                {
                    routes.MapHttpRoute(r.Name, r.Template, r.Defaults);
                }
                else
                {
                    routes.MapHttpRoute(r.Name, r.Template);
                }
            });
        }
    }
}
