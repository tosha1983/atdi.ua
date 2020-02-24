using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IDataOrm
    {
        string Name { get; }

        string Context { get; }
	}

}
