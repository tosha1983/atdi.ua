
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.DataModels.EntityOrm;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Atdi.WebApiServices.EntityOrm.Controllers
{
    [RoutePrefix("api/orm")]
    public class MetadataController : WebApiController
    {
        private readonly IEntityOrmConfig _ormConfig;
        private readonly IEntityOrm _entityOrm;
        
        public MetadataController(IEntityOrmConfig ormConfig, IEntityOrm entityOrm, ILogger logger) : base(logger)
        {
            this._ormConfig = ormConfig;
            this._entityOrm = entityOrm;
        }

        [HttpGet]
        [Route("config")]
        public DTO.EntityOrmConfig Config()
        {
            var config = new DTO.EntityOrmConfig(this._ormConfig);
            return config;
        }

        [HttpGet]
        [Route("metadata/entities")]
        public DTO.EntitiesNode[] Entities()
        {
            return this.Entities(null);
        }

        [HttpGet]
        [Route("metadata/entities/{folderPath}")]
        public DTO.EntitiesNode[] Entities(string folderPath)
        {
            string folder = string.Empty;
            if (!string.IsNullOrEmpty(folderPath))
            {
                folderPath = folderPath.Replace(".", "\\");
                folder = Path.Combine(_ormConfig.EntitiesPath, folderPath);
            }
            else
            {
                folder = _ormConfig.EntitiesPath;
            }
            
            var files = Directory.GetFiles(folder, "*.xml", System.IO.SearchOption.TopDirectoryOnly);
            var folders = Directory.GetDirectories(folder);
            var result = new DTO.EntitiesNode[files.Length + folders.Length];
            for (int i = 0; i < folders.Length; i++)
            {
                var name = Path.GetFileName(folders[i]);
                if (string.IsNullOrEmpty(folderPath))
                {
                    result[i] = new DTO.FolderNode(name, Entities(name));
                }
                else
                {
                    result[i] = new DTO.FolderNode(name, Entities(Path.Combine(folderPath, name)));
                }
                
            }
            for (int i = 0; i < files.Length; i++)
            {
                var name = Path.GetFileNameWithoutExtension(files[i]);
                var path = _ormConfig.Namespace + "." + (string.IsNullOrEmpty(folderPath)? "" : folderPath.Replace("\\", ".") + ".") + name;
                result[folders.Length + i] = new DTO.EntityNode(name, path);
            }

            return result;
        }


        [HttpGet]
        [Route("metadata/entity/{ns}/{entity}")]
        public DTO.EntityMetadata Entity(string ns, string entity)
        {
            var ormMetadata = _entityOrm.GetEntityMetadata($"{ns}.{entity}");
            return new DTO.EntityMetadata(ormMetadata);
        }
    }
}
