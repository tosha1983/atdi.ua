using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IReferenceFieldMetadata : IFieldMetadata
    {
        /// <summary>
        /// Сущность расширение
        /// </summary>
        IEntityMetadata RefEntity { get; }

        /// <summary>
        /// Мапинг полей первичного ключа сущности ссылки
        /// </summary>
        IPrimaryKeyMappingMetadata Mapping { get; }
    }
}
