using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IEntitiesNode
    {
        string Name { get; }

        int KindCode { get; }

        string KindName { get; }
    }

    public interface IFolderNode : IEntitiesNode
    {
        IEntitiesNode[] Content { get; }
    }
    public interface IEntityNode : IEntitiesNode
    {
        string Path { get; }
    }
}
