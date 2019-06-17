using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities.Entities
{
    [Entity]
    public interface ITestDataType
    {
        long Id { get; set; }

        bool? Boolean_BOOL_Bit { get; set; }

        bool? Boolean_BOOL_Tinyint { get; set; }

        bool? Boolean_BOOL_Integer { get; set; }

        bool? Boolean_BOOL_Byte { get; set; }

        bool? Boolean_BOOL_Char { get; set; }

        bool? Boolean_BOOL_Nchar { get; set; }

        bool? Boolean_BOOL_Varchar { get; set; }

        bool? Boolean_BOOL_Nvarchar { get; set; }
    }

    [Entity]
    public interface ITestRefRoot
    {
        long Id { get; set; }

        ITestRefBook BOOK1 { get; set; }

        ITestRefBook BOOK2 { get; set; }

        long SecondBookId { get; set; }

        ITestRefSubBook REL1 { get; set; }
    }

    [Entity]
    public interface ITestRefBook
    {
        long Id { get; set; }

        string Name { get; set; }

        ITestRefSubBook SUBBOOK1 { get; set; }
    }

    [Entity]
    public interface ITestRefSubBook
    {
        string Code { get; set; }

        string SubType { get; set; }

        string Name { get; set; }

        ITestRefSubBookExt1 EXT1 { get; set; }
        ITestRefSubBookExt2 EXT2 { get; set; }
    }

    [Entity]
    public interface ITestRefSubBookExt1
    {
        string Prop1 { get; set; }

        string Prop2 { get; set; }

        string Prop3 { get; set; }
    }

    [Entity]
    public interface ITestRefSubBookExt2
    {
        string Prop1 { get; set; }

        string Prop2 { get; set; }

        string Prop3 { get; set; }
    }
}
