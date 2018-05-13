using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Logging
{
    class TraceScopeData : ITraceScopeData
    {
        public TraceScopeData(TraceScopeName? name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
        }

        public TraceScopeName? Name { get; set; }

        public Guid Id { get; set; }
    }
}
