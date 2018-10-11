using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public class UnitMetadata : IUnitMetadata
    {
        public string Name { get; set; }

        public string Dimension { get; set; }

        public string Category { get; set; }
    }
}
