using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServer.Contracts;

namespace Atdi.AppServer.Common
{
    public interface IConstraintParser<TResult>
    {
        TResult Parse(DataConstraint constraint);
    }
}
