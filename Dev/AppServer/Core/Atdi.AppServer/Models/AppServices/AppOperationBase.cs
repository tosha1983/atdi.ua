using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Models.AppServices
{
    public abstract class AppOperationBase<TService> : IAppOperation
        where TService : class, IAppService
    {
        protected readonly string _name;

        public AppOperationBase(string name)
        {
            this._name = name;
        }

        public string Name => this._name;
    }
}
