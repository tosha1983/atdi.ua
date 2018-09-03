using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class Module
    {
        public string Name;
        public bool IsInEdition;

        public Module(string name)
        {
            this.Name = name;
        }
    }
}
