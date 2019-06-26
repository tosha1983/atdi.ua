using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrns.Server.Entities
{
    [EntityPrimaryKey]
    public interface ITestEntityAbs_PK
    {
        long AbsPkId1 { get; set; }

        Guid AbsPkId2 { get; set; }

        DateTimeOffset AbsPkId3 { get; set; }
    }

    [Entity]
    public interface ITestEntityAbs : ITestEntityAbs_PK
    {
        string AbsField1 { get; set; }

        string AbsField2 { get; set; }

        string AbsField3 { get; set; }

    }

    [EntityPrimaryKey]
    public interface ITestEntityAbsSmp_PK : ITestEntityAbs_PK
    {
    }

    [Entity]
    public interface ITestEntityAbsSmp : ITestEntityAbs, ITestEntityAbsSmp_PK
    {
        string SmpField1 { get; set; }

        string SmpField2 { get; set; }

        string SmpField3 { get; set; }

    }


    [EntityPrimaryKey]
    public interface ITestEntityAbsSmpProto0_PK : ITestEntityAbsSmp_PK
    {
    }

    [Entity]
    public interface ITestEntityAbsSmpProto0 : ITestEntityAbsSmp, ITestEntityAbsSmpProto0_PK
    {
        string Proto0Field1 { get; set; }

        string Proto0Field2 { get; set; }

        string Proto0Field3 { get; set; }

    }

    [EntityPrimaryKey]
    public interface ITestEntityAbsSmpProto1_PK : ITestEntityAbsSmpProto0_PK
    {
    }

    [Entity]
    public interface ITestEntityAbsSmpProto1 : ITestEntityAbsSmpProto0, ITestEntityAbsSmpProto1_PK
    {
        string Proto1Field1 { get; set; }

        string Proto1Field2 { get; set; }

        string Proto1Field3 { get; set; }

    }

    [EntityPrimaryKey]
    public interface ITestEntityAbsSmpProtoEnd_PK : ITestEntityAbsSmpProto1_PK
    {
    }

    [Entity]
    public interface ITestEntityAbsSmpProtoEnd : ITestEntityAbsSmpProto1, ITestEntityAbsSmpProtoEnd_PK
    {
        string ProtoEndField1 { get; set; }

        string ProtoEndField2 { get; set; }

        string ProtoEndField3 { get; set; }

    }
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

    [EntityPrimaryKey]
    public interface ITestRefRoot_PK
    {
        long Id { get; set; }

        Guid GuidId { get; set; }
    }

    [Entity]
    public interface ITestRefRoot : ITestRefRoot_PK
    {
        

        ITestRefBook BOOK1 { get; set; }

        ITestRefBook BOOK2 { get; set; }

        long SecondBookId { get; set; }

        ITestRefSubBook REL1 { get; set; }
    }

    [EntityPrimaryKey]
    public interface ITestRefBook_PK
    {
        long Id { get; set; }

        Guid GuidId { get; set; }
    }
    [Entity]
    public interface ITestRefBook : ITestRefBook_PK
    {
        

        string Name { get; set; }

        ITestRefSubBook SUBBOOK1 { get; set; }
    }

    [EntityPrimaryKey]
    public interface ITestRefSubBook_PK
    {
        string Code { get; set; }

        string SubType { get; set; }
    }

    [Entity]
    public interface ITestRefSubBook : ITestRefSubBook_PK
    {
        string Name { get; set; }

        ITestRefSubBookExt1 EXT1 { get; set; }

        ITestRefSubBookExt2 EXT2 { get; set; }
    }

    [Entity]
    public interface ITestRefSubBookExt1 : ITestRefSubBook_PK
    {
        string Prop1 { get; set; }

        string Prop2 { get; set; }

        string Prop3 { get; set; }
    }

    [Entity]
    public interface ITestRefSubBookExt2 : ITestRefSubBook_PK
    {
        string Prop1 { get; set; }

        string Prop2 { get; set; }

        string Prop3 { get; set; }
    }
}
