using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;

namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public class EntityRequest : API.IEntityRequest
    {
        public string Context { get; set; }

        public string Namespace { get; set; }

        public string Entity { get; set; }

        public string QName { get => Namespace + "." + Entity; }
    }

    public class DataSetRequest : EntityRequest, API.IDataSetRequest
    {
        public string[] Select { get; set; }

        public string[] OrderBy { get; set; }

        public string[] Filter { get; set; }

        public long Top { get; set; } 
    }

    public class DataRecordRequest : EntityRequest, API.IDataRecordRequest
    {

        public string PrimaryKey { get; set; }

        public string[] Select { get; set; }

        public string[] Filter { get; set; }

    }

    public class FieldValueRequest : EntityRequest, API.IFieldValueRequest
    {

        public string PrimaryKey { get; set; }

        public string FieldPath { get; set; }

        public string[] Filter { get; set; }

    }
}
