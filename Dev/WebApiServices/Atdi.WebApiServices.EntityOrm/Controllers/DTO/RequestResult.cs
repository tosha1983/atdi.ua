﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;

namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public class RequestResult : API.IRequestResult
    {

    }

    public class FieldDescriptor : API.IFieldDescriptor
    {
        public string Path { get; set; }

        public API.IDataTypeMetadata Type { get; set; }

        public int Index { get; set; }

    }

    public class FieldValueResult : RequestResult, API.IFieldValueResult
    {
        public API.IFieldDescriptor Field { get; set; }

        public object Value { get; set; }
    }

    public class RecordResult : RequestResult, API.IRecordResult
    {
        public API.IFieldDescriptor[] Fields { get; set; }

        public object[] Record { get; set; }
    }

    public class RecordCreateResult : RequestResult, API.IRecordCreateResult
    {
	    public int Count { get; set; }

	    public object PrimaryKey { get; set; }
	}

    public class RecordApplyResult : RequestResult, API.IRecordApplyResult
	{
	    public int Count { get; set; }

	    public object PrimaryKey { get; set; }
    }

	public class RecordUpdateResult : RequestResult, API.IRecordUpdateResult
    {
	    public int Count { get; set; }
    }

    public class RecordDeleteResult : RequestResult, API.IRecordDeleteResult
    {
	    public int Count { get; set; }
    }

	public class DataSetResult : RequestResult, API.IDataSetResult
    {
        public API.IFieldDescriptor[] Fields { get; set; }

        public object[] Records { get; set; }

        public long Count { get; set; }
    }
}
