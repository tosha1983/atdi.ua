using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.WebApiServices
{
    public interface IRoutesConfig
    {
        void Registry(string name, string routeTemplate);

        void Registry(string name, string routeTemplate, object defaults);

        void Registry(string name, string routeTemplate, object defaults, object constraints);

        void Registry(string name, string routeTemplate, object defaults, object constraints, HttpMessageHandler handler);
    }
}
