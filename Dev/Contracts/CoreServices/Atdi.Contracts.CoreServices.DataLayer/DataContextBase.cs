using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public abstract class DataContextBase : IDataContext
    {
        public DataContextBase(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
}
