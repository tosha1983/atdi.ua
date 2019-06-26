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

        ITestEntityAbsSmpExt1 SMP_EXT1 { get; set; }

        ITestEntityAbsSmpExt2 SMP_EXT2 { get; set; }
    }

    [Entity]
    public interface ITestEntityAbsSmpExt1 : ITestEntityAbsSmp_PK
    {
        string Ext1SmpField1 { get; set; }

        string Ext1SmpField2 { get; set; }

        string Ext1SmpField3 { get; set; }

    }

    [Entity]
    public interface ITestEntityAbsSmpExt2 : ITestEntityAbsSmp_PK
    {
        string Ext2SmpField1 { get; set; }

        string Ext2SmpField2 { get; set; }

        string Ext2SmpField3 { get; set; }

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

        ITestEntityAbsSmpProto0Ext1 PRT0_EXT1 { get; set; }

        ITestEntityAbsSmpProto0Ext2 PRT0_EXT2 { get; set; }
    }

    [Entity]
    public interface ITestEntityAbsSmpProto0Ext1 : ITestEntityAbsSmpProto0_PK
    {
        string Ext1Proto0Field1 { get; set; }

        string Ext1Proto0Field2 { get; set; }

        string Ext1Proto0Field3 { get; set; }

    }

    [Entity]
    public interface ITestEntityAbsSmpProto0Ext2 : ITestEntityAbsSmpProto0_PK
    {
        string Ext2Proto0Field1 { get; set; }

        string Ext2Proto0Field2 { get; set; }

        string Ext2Proto0Field3 { get; set; }

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

        ITestEntityAbsSmpProto1Ext1 PRT1_EXT1 { get; set; }

        ITestEntityAbsSmpProto1Ext2 PRT1_EXT2 { get; set; }
    }

    [Entity]
    public interface ITestEntityAbsSmpProto1Ext1 : ITestEntityAbsSmpProto1_PK
    {
        string Ext1Proto1Field1 { get; set; }

        string Ext1Proto1Field2 { get; set; }

        string Ext1Proto1Field3 { get; set; }

    }

    [Entity]
    public interface ITestEntityAbsSmpProto1Ext2 : ITestEntityAbsSmpProto1_PK
    {
        string Ext2Proto1Field1 { get; set; }

        string Ext2Proto1Field2 { get; set; }

        string Ext2Proto1Field3 { get; set; }

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

        ITestEntityAbsSmpProtoEndExt1 PRTEND_EXT1 { get; set; }

        ITestEntityAbsSmpProtoEndExt2 PRTEND_EXT2 { get; set; }
    }


    [Entity]
    public interface ITestEntityAbsSmpProtoEndExt1 : ITestEntityAbsSmpProtoEnd_PK
    {
        string Ext1ProtoEndField1 { get; set; }

        string Ext1ProtoEndField2 { get; set; }

        string Ext1ProtoEndField3 { get; set; }

    }

    [Entity]
    public interface ITestEntityAbsSmpProtoEndExt2 : ITestEntityAbsSmpProtoEnd_PK
    {
        string Ext2ProtoEndField1 { get; set; }

        string Ext2ProtoEndField2 { get; set; }

        string Ext2ProtoEndField3 { get; set; }

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
