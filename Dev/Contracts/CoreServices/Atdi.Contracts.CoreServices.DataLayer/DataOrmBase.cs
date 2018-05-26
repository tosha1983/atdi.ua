using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public abstract class DataOrmBase : IDataOrm
    {
        public DataOrmBase(string name)
        {
            this.Name = name;
        }
        public string Name { get; private set; }
    }
}
