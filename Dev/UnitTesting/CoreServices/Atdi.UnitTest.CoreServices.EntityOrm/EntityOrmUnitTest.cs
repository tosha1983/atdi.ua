using System;
using System.Collections.Generic;
using Atdi.UnitTest.CoreServices.EntityOrm.Fakes;
using Atdi.DataModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TST = Atdi.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.UnitTest.CoreServices.EntityOrm
{
    [TestClass]
    public class EntityOrmUnitTest
    {
        public TST.EntityOrm orm;

        [TestInitialize]
        public void TestSetUp()
        {
            string ExeDir = @"C:\ProjectTest\atdi.ua.commit\Dev\UnitTesting\CoreServices\Atdi.UnitTest.CoreServices.EntityOrm\bin\Debug\Metadata";
            var config = new EntityOrmConfigFake()
            {
                RootPath = ExeDir + "\\",
                DataTypesPath = ExeDir + "\\DataTypes",
                EntitiesPath = ExeDir + "\\Entities",
                UnitsPath = ExeDir + "\\Units"
            };
            orm = new TST.EntityOrm(config);
        }

        //[TestMethod]
        //public void Test_CheckCreation()
        //{

        // Конечный путь к файлам xml формируется следующим образом:
        // директория в которой расположен файл Environment.xml \ RootPath \ Поддиректории 
        //string ExeDir = @"C:\ProjectTest\atdi.ua\Dev\UnitTesting\CoreServices\Atdi.UnitTest.CoreServices.EntityOrm\bin\Debug\Metadata";
        //var config = new EntityOrmConfigFake()
        //{
        //    RootPath = ExeDir + "\\",
        //    DataTypesPath = ExeDir + "\\DataTypes",
        //    EntitiesPath = ExeDir + "\\Entities",
        //    UnitsPath = ExeDir + "\\Units"
        //};
        //var orm = new TST.EntityOrm(config);

        //Contracts.CoreServices.EntityOrm.Metadata.IDataTypeMetadata dataTypeMetadata = orm.GetDataTypeMetadata("Integer.64", Contracts.CoreServices.EntityOrm.Metadata.DataSourceType.Database);
        //Contracts.CoreServices.EntityOrm.Metadata.IEntityMetadata entityMetadata = orm.GetEntityMetadata("Station");
        //Contracts.CoreServices.EntityOrm.Metadata.IUnitMetadata unitMetadata = orm.GetUnitMetadata("Frequency.MHz.xml");

        //Assert.AreEqual(1234, orm.GetDataTypeMetadata("Integer.64", Contracts.CoreServices.EntityOrm.Metadata.DataSourceType.Database));
        //}

        [TestMethod]
        public void Test_GetDataTypeMetadata_Int64_Passed()
        {
            //Contracts.CoreServices.EntityOrm.Metadata.IDataTypeMetadata expectedDataTypeMetadata;
            DataTypeMetadata expectedDataTypeMetadata = new DataTypeMetadata();

            var autonumMetadata = new AutonumMetadata();
            autonumMetadata.Start = 0;
            autonumMetadata.Step = 0;
            expectedDataTypeMetadata.Autonum = autonumMetadata;
            expectedDataTypeMetadata.CodeVarClrType = null;
            expectedDataTypeMetadata.CodeVarType = DataType.Long;
            expectedDataTypeMetadata.DataSourceType = DataSourceType.Database;

            expectedDataTypeMetadata.Length = null;
            expectedDataTypeMetadata.Multiple = false;
            expectedDataTypeMetadata.Name = "Integer.64";
            expectedDataTypeMetadata.Precision = null;
            expectedDataTypeMetadata.Scale = null;

            expectedDataTypeMetadata.SourceVarType = DataSourceVarType.INT64;


            IDataTypeMetadata actualDataTypeMetadata = orm.GetDataTypeMetadata("Integer.64", DataSourceType.Database);
            //Assert.AreEqual(actualDataTypeMetadata, expectedDataTypeMetadata);

            //Assert.AreEqual(expectedDataTypeMetadata.Autonum, actualDataTypeMetadata.Autonum, "Autonum mismatch, actual value is " + actualDataTypeMetadata.Autonum.Start + " " + actualDataTypeMetadata.Autonum.Step);
            //Assert.AreEqual(expectedDataTypeMetadata.Name, actualDataTypeMetadata.Name, "Name mismatch, actual value is " + actualDataTypeMetadata.Name);

            bool testAssert = TestingUtils.JsonCompare(actualDataTypeMetadata, expectedDataTypeMetadata);

            //expectedDataTypeMetadata.CodeVarType.Equals(actualDataTypeMetadata.CodeVarType);
            Assert.IsTrue(testAssert);

        }
        [TestMethod]
        public void Test_GetDataTypeMetadata_Count32_Passed()
        {
            //Contracts.CoreServices.EntityOrm.Metadata.IDataTypeMetadata expectedDataTypeMetadata;
            DataTypeMetadata expectedDataTypeMetadata = new DataTypeMetadata();

            var autonumMetadata = new AutonumMetadata();
            autonumMetadata.Start = 1;
            autonumMetadata.Step = 1;

            expectedDataTypeMetadata.Autonum = autonumMetadata;
            expectedDataTypeMetadata.CodeVarClrType = null;
            expectedDataTypeMetadata.CodeVarType = DataType.Integer;
            expectedDataTypeMetadata.DataSourceType = DataSourceType.Database;

            expectedDataTypeMetadata.Length = null;
            expectedDataTypeMetadata.Multiple = false;
            expectedDataTypeMetadata.Name = "Counter.32";

            expectedDataTypeMetadata.Precision = null;
            expectedDataTypeMetadata.Scale = null;
            expectedDataTypeMetadata.SourceVarType = DataSourceVarType.INT32;

            IDataTypeMetadata actualDataTypeMetadata = orm.GetDataTypeMetadata("Counter.32", DataSourceType.Database);
            
            bool testAssert = TestingUtils.JsonCompare(actualDataTypeMetadata, expectedDataTypeMetadata);
            Assert.IsTrue(testAssert);

        }

        [TestMethod]
        public void Test_GetDataTypeMetadata_Decimal_Passed()
        {
            //Contracts.CoreServices.EntityOrm.Metadata.IDataTypeMetadata expectedDataTypeMetadata;
            DataTypeMetadata expectedDataTypeMetadata = new DataTypeMetadata();

            var autonumMetadata = new AutonumMetadata();

            expectedDataTypeMetadata.Autonum = autonumMetadata;
            expectedDataTypeMetadata.CodeVarClrType = null;
            expectedDataTypeMetadata.CodeVarType = DataType.Decimal;
            expectedDataTypeMetadata.DataSourceType = DataSourceType.Database;

            expectedDataTypeMetadata.Length = null;
            expectedDataTypeMetadata.Multiple = false;
            expectedDataTypeMetadata.Name = "Decimal.22.8";

            expectedDataTypeMetadata.Precision = 22;
            expectedDataTypeMetadata.Scale = 8;
            expectedDataTypeMetadata.SourceVarType = DataSourceVarType.DECIMAL;

            IDataTypeMetadata actualDataTypeMetadata = orm.GetDataTypeMetadata("Decimal.22.8", DataSourceType.Database);

            bool testAssert = TestingUtils.JsonCompare(actualDataTypeMetadata, expectedDataTypeMetadata);
            Assert.IsTrue(testAssert);
        }

        [TestMethod]
        public void Test_GetDataTypeMetadata_Text10_Passed()
        {
            //Contracts.CoreServices.EntityOrm.Metadata.IDataTypeMetadata expectedDataTypeMetadata;
            DataTypeMetadata expectedDataTypeMetadata = new DataTypeMetadata();

            var autonumMetadata = new AutonumMetadata();
            //autonumMetadata.Start = 1;
            //autonumMetadata.Step = 1;

            expectedDataTypeMetadata.Autonum = autonumMetadata;
            expectedDataTypeMetadata.CodeVarClrType = null;
            expectedDataTypeMetadata.CodeVarType = DataType.String;
            expectedDataTypeMetadata.DataSourceType = DataSourceType.Database;

            expectedDataTypeMetadata.Length = 10;
            expectedDataTypeMetadata.Multiple = false;
            expectedDataTypeMetadata.Name = "Text.10";

            expectedDataTypeMetadata.Precision = null;
            expectedDataTypeMetadata.Scale = null;
            expectedDataTypeMetadata.SourceVarType = DataSourceVarType.NVARCHAR;

            IDataTypeMetadata actualDataTypeMetadata = orm.GetDataTypeMetadata("Text.10", DataSourceType.Database);

            bool testAssert = TestingUtils.JsonCompare(actualDataTypeMetadata, expectedDataTypeMetadata);
            Assert.IsTrue(testAssert);
        }

        [TestMethod]
        public void Test_GetUnitMetadata_Positive()
        {
            //Contracts.CoreServices.EntityOrm.Metadata.IDataTypeMetadata expectedDataTypeMetadata;
            UnitMetadata expectedUnitMetadata = new UnitMetadata();

            expectedUnitMetadata.Name = "Angle.Degree";
            expectedUnitMetadata.Dimension = "Degree";
            expectedUnitMetadata.Category = "Angle";
            //не учитвается расширение файла
            IUnitMetadata actualUnitMetadata = orm.GetUnitMetadata("Angle.Degree");

            bool testAssert = TestingUtils.JsonCompare(actualUnitMetadata, expectedUnitMetadata);
            Assert.IsTrue(testAssert);
        }

        [TestMethod]
        public void Test_GetEntityMetadata_Positive()
        {
            //Used data types
            DataTypeMetadata typeDecimal_22_8 = new DataTypeMetadata();
            typeDecimal_22_8.Autonum = null;
            typeDecimal_22_8.CodeVarClrType = null;
            typeDecimal_22_8.CodeVarType = DataType.Decimal;
            typeDecimal_22_8.DataSourceType = DataSourceType.Database;

            typeDecimal_22_8.Length = null;
            typeDecimal_22_8.Multiple = false;
            typeDecimal_22_8.Name = "Decimal.22.8";

            typeDecimal_22_8.Precision = 22;
            typeDecimal_22_8.Scale = 8;
            typeDecimal_22_8.SourceVarType = DataSourceVarType.DECIMAL;
            //
            DataTypeMetadata typeCounter64 = new DataTypeMetadata();
            var autonumMetadata = new AutonumMetadata();
            autonumMetadata.Start = 1;
            autonumMetadata.Step = 1;

            typeCounter64.Autonum = autonumMetadata;
            typeCounter64.CodeVarClrType = null;
            typeCounter64.CodeVarType = DataType.Integer;
            typeCounter64.DataSourceType = DataSourceType.Database;

            typeCounter64.Length = null;
            typeCounter64.Multiple = false;
            typeCounter64.Name = "Counter.64";

            typeCounter64.Precision = null;
            typeCounter64.Scale = null;
            typeCounter64.SourceVarType = DataSourceVarType.INT64;
            
            //
            EntityMetadata expectedEntityMetadata = new EntityMetadata();

            expectedEntityMetadata.Name = "SectorMaskElement";
            expectedEntityMetadata.Title = "SectorMaskElement";
            expectedEntityMetadata.Desc = "The SectorMaskElement";
            expectedEntityMetadata.Type = EntityType.Normal;

            DataSourceMetadata entityMetadataDataSource = new DataSourceMetadata();
            entityMetadataDataSource.Type = DataSourceType.Database;
            entityMetadataDataSource.Object = DataSourceObject.Table;
            entityMetadataDataSource.Name = "XBS_SECTORMASKELEM";
            entityMetadataDataSource.Schema = "ICSM";

            //expectedEntityMetadata.Fields;
            Dictionary<String, IFieldMetadata> entityMetadataFields = new Dictionary<string, IFieldMetadata>();
            FieldMetadata fieldIdMetadata = new FieldMetadata();
            fieldIdMetadata.Desc = "Id";
            fieldIdMetadata.Title = "Id";
            fieldIdMetadata.Name = "Id";
            fieldIdMetadata.DataType = typeCounter64;
            fieldIdMetadata.SourceName = "ID";
            fieldIdMetadata.SourceType = FieldSourceType.Column;
            fieldIdMetadata.Required = true;

            FieldMetadata fieldBwMetadata = new FieldMetadata();
            fieldBwMetadata.Desc = "Bw";
            fieldBwMetadata.Title = "Bw";
            fieldBwMetadata.Name = "Bw";
            fieldBwMetadata.DataType = typeDecimal_22_8;
            fieldBwMetadata.SourceName = "BW";
            fieldBwMetadata.SourceType = FieldSourceType.Column;
            fieldBwMetadata.Required = false;

            FieldMetadata fieldLevelMetadata = new FieldMetadata();
            fieldLevelMetadata.Desc = "Level";
            fieldLevelMetadata.Title = "Level";
            fieldLevelMetadata.Name = "Level";
            fieldLevelMetadata.DataType = typeDecimal_22_8;
            
            fieldLevelMetadata.SourceName = "LEVEL";
            fieldLevelMetadata.SourceType = FieldSourceType.Column;
            fieldLevelMetadata.Required = false;

            entityMetadataFields.Add("Id", fieldIdMetadata);
            entityMetadataFields.Add("Bw", fieldBwMetadata);
            entityMetadataFields.Add("Level", fieldLevelMetadata);
            //initialize all objects
            expectedEntityMetadata.Fields = entityMetadataFields;


            IEntityMetadata actualEntityMetadata = orm.GetEntityMetadata("SectorMaskElement");
            bool testAssert = TestingUtils.JsonCompare(actualEntityMetadata, expectedEntityMetadata);
            //bool testAssert = TestingUtils.JsonCompare(actualEntityMetadata, actualEntityMetadata);
            Assert.IsTrue(testAssert);
        }
    }
}
