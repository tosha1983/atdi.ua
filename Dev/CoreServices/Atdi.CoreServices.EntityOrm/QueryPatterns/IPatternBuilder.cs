using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm.QueryPatterns
{
    internal interface IPatternBuilder
    {
        TResult BuildAndExecute<TResult, TModel>(PatternExecutionContex<TResult, TModel> executionContex);
    }
}
