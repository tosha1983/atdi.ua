using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [Entity]
    public interface ITest
    {
        int? ID { get; set; }

        string STRING_TYPE { get; set; }

        bool? BOOLEAN_TYPE { get; set; }

        int? INTEGER_TYPE { get; set; }

        DateTime? DATETIME_TYPE { get; set; }

        double? DOUBLE_TYPE { get; set; }

        float? FLOAT_TYPE { get; set; }

        byte[] BYTES_TYPE { get; set; }

        Guid? GUID_TYPE { get; set; }

        decimal? DECIMAL_TYPE { get; set; }

        byte? BYTE_TYPE { get; set; }

        Int16? INT16 { get; set; }

        sbyte? INT08 { get; set; }

    }
}
