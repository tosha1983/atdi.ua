using Atdi.Contracts.CoreServices.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.UnitTest.CoreServices.EntityOrm.Fakes
{
    class EntityOrmConfigFake : IEntityOrmConfig
    {
        public string Name {get; set;}

        public string Version { get; set; }

        public string RootPath { get; set; }

        public string Assembly { get; set; }

        public string Namespace { get; set; }

        public string EntitiesPath { get; set; }

        public string DataTypesPath { get; set; }

        public string UnitsPath { get; set; }
    }
}
