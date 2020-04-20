using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm.Api
{
    public interface IRequestResult
    {
    }

    public interface IFieldValueResult : IRequestResult
    {
        IFieldDescriptor Field { get; set; }

        object Value { get; set; }
    }

    public interface IFieldDescriptor
    {
        int Index { get; set; }

        string Path { get; set; }

        IDataTypeMetadata Type { get; set; }
    }

    public interface IRecordResult : IRequestResult
    {
        IFieldDescriptor[] Fields { get; set; }

        object[] Record { get; set; }
    }

    public interface IRecordCreateResult : IRequestResult
    {
	    int Count { get; set; }

	    object PrimaryKey { get; set; }
	}

    public interface IRecordApplyResult : IRequestResult
    {
	    int Count { get; set; }

	    object PrimaryKey { get; set; }
    }

	public interface IRecordUpdateResult : IRequestResult
    {
	    int Count { get; set; }
    }

    public interface IRecordDeleteResult : IRequestResult
    {
	    int Count { get; set; }
    }

	public interface IDataSetResult : IRequestResult
    {
        IFieldDescriptor[] Fields { get; set; }

        object[] Records { get; set; }

        long Count { get; set; }
    }

}
