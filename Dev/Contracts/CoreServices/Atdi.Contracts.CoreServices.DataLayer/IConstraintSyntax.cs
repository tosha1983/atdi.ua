using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IConstraintEngineSyntax
    {
        string LikeAnyChar { get; }

        //string OpeningBracket { get; }

        //string ClosingBracket { get; }

        string JoinExpressions(LogicalOperator operation, string[] expressions);

        string IsNull(string testExpression);

        string IsNotNull(string testExpression);

        string In(string testExpression, string[] expressions);

        string NotIn(string testExpression, string[] expressions);

        string Between(string testExpression, string beginExpression, string endExpression);

        string NotBetween(string testExpression, string beginExpression, string endExpression);

        string Like(string matchExpression, string paternExpression);

        string NotLike(string matchExpression, string paternExpression);

        string Equal(string leftExpression, string rightExpression);

        string GreaterThan(string leftExpression, string rightExpression);

        string LessThan(string leftExpression, string rightExpression);

        string GreaterEqual(string leftExpression, string rightExpression);

        string LessEqual(string leftExpression, string rightExpression);

        string NotEqual(string leftExpression, string rightExpression);

        
    }
}
