using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IUnitMetadata
    {
        /// <summary>
        /// Имя измерения
        /// </summary>
        string Name { get; }

        string Dimension { get; }

        string Category { get; }
    }
}
