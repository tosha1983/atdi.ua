using System;
using Atdi.UnitTest.CoreServices.EntityOrm.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TST = Atdi.CoreServices.EntityOrm;
namespace Atdi.UnitTest.CoreServices.EntityOrm
{
    [TestClass]
    public class EntityOrmUnitTest
    {
        [TestMethod]
        public void Test_CheckCreation()
        {
            // Конечный путь к файлам xml формируется следующим образом:
            // директория в которой расположен файл Environment.xml \ RootPath \ Поддиректории 
            string ExeDir = @"c:\projects\reposApi2\Dev\UnitTesting\CoreServices\Atdi.UnitTest.CoreServices.EntityOrm\bin\Debug\Metadata";
            var config = new EntityOrmConfigFake()
            {
                RootPath = ExeDir + "\\",
                DataTypesPath = ExeDir + "\\DataTypes",
                EntitiesPath = ExeDir + "\\Entities",
                UnitsPath = ExeDir + "\\Units"
            };
            var orm = new TST.EntityOrm(config);
            Contracts.CoreServices.EntityOrm.Metadata.IDataTypeMetadata dataTypeMetadata = orm.GetDataTypeMetadata("Int64", Contracts.CoreServices.EntityOrm.Metadata.DataSourceType.Database);
            Contracts.CoreServices.EntityOrm.Metadata.IEntityMetadata entityMetadata = orm.GetEntityMetadata("Sensor");
            Contracts.CoreServices.EntityOrm.Metadata.IUnitMetadata unitMetadata = orm.GetUnitMetadata("Frequency.MHz.xml");
        }
    }
}
