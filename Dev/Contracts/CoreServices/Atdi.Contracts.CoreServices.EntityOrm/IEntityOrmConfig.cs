using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm
{
    public interface IEntityOrmConfig
    {
        string Name { get; }

        string Version { get; }

        string RootPath { get; }

        string Assembly { get; }

        string Namespace { get; }

        string EntitiesPath { get; }

        string DataTypesPath { get; }

        string UnitsPath { get; }
    }
}
