using System;
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
            string ExeDir = @"C:\ProjectTest\atdi.ua\Dev\UnitTesting\CoreServices\Atdi.UnitTest.CoreServices.EntityOrm\bin\Debug\Metadata";
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
        public void Test_GetDataTypeMetadata_dataTypeDefIsNull_Failed()
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
    }
}
