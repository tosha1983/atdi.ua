using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;

namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public enum EntityNodeKind
    {
        Entity = 1,
        Folder = 2
    }

    public class EntitiesNode : API.IEntitiesNode
    {
        public EntitiesNode(string name, EntityNodeKind kind)
        {
            Name = name;
            KindCode = (int)kind;
            KindName = kind.ToString();
        }

        public string Name { get; }

        public int KindCode { get; }

        public string KindName { get; }
    }

    public class FolderNode : EntitiesNode, API.IFolderNode
    {
        public FolderNode(string name, EntitiesNode[] content) : base(name, EntityNodeKind.Folder)
        {
            this.Content = content;
        }

        public API.IEntitiesNode[] Content { get; }
    }
    public class EntityNode : EntitiesNode, API.IEntitiesNode
    {
        public EntityNode(string name, string path) : base(name, EntityNodeKind.Entity)
        {
            this.Path = path;
        }

        public string Path { get;  }
    }

}
