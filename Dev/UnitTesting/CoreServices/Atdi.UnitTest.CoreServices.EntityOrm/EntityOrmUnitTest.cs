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
            var config = new EntityOrmConfigFake()
            {
                RootPath ="",
                DataTypesPath = ""
            };

            var orm = new TST.EntityOrm(config);

            


        }
    }
}
