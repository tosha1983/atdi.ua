using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class AutonumMetadata : IAutonumMetadata
    {
        public int Start { get; set; }

        public int Step { get; set; }
    }
}
