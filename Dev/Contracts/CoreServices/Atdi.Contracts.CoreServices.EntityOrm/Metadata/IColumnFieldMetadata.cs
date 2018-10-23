using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IColumnFieldMetadata : IFieldMetadata
    {
        /// <summary>
        /// Имя источника определения значения поля
        /// </summary>
        new string SourceName { get;  }

    }
}
