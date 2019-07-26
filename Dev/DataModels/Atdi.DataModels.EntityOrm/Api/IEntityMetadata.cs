using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IEntityMetadata
    {
        /// <summary>
        /// The qualified name
        /// </summary>
        string Name { get; }

        string QualifiedName { get; }

        string Namespace { get; }

        string Title { get; }

        string Desc { get; }

        int TypeCode { get; }

        string TypeName { get; }

        IEntityMetadata BaseEntity { get; }

        IEntityFieldMetadata[] Fields { get; }

        string[] PrimaryKey { get; }
    }
}
