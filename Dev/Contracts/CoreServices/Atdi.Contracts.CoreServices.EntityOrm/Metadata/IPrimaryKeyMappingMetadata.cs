using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IPrimaryKeyMappingMetadata
    {
        /// <summary>
        /// Набор мапинга значений на поля первичного ключа сущности-ссылки
        /// </summary>
        IReadOnlyDictionary<string, IPrimaryKeyFieldMappedMetadata> Fields { get; }
    }
}
