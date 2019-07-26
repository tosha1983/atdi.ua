using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IUnitMetadata
    {
        string Name { get; }

        string Dimension { get; }

        string Category { get; }
    }
}
