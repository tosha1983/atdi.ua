using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm.Metadata
{
    public class AutonumMetadata : IAutonumMetadata
    {
        public int Start { get; set; }

        public int Step { get; set; }
    }
}
