using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public class DefaultResourceResolver : IResourceResolver
    {
        public string Resolve(string name, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return name;
            }

            return string.Format(name, args);
        }
    }
}
