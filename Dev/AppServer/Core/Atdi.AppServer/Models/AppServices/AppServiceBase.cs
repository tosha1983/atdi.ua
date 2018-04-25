using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public abstract class AppServiceBase : IAppService
    {
        protected readonly string _name;
        protected readonly List<IAppOperation> _operations;

        public AppServiceBase(string name)
        {
            this._name = name;
            this._operations = new List<IAppOperation>();
        }

        public string Name => this._name;

        public IReadOnlyList<IAppOperation> Operations => this._operations;
    }
}
