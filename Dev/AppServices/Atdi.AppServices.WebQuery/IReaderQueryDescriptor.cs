using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.LegacyServices.Icsm;


namespace Atdi.AppServices.WebQuery
{
    public interface IReaderQueryDescriptor
    {
        XWebQuery[] GetAllXWebQuery();
        XWebQuery GetXWebQuery(int QueryID, int CodeUser);
        XWebConstraint[] GetXWebConstraint(int ID);
    }
}
