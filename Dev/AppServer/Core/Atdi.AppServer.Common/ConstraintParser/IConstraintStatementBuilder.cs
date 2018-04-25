using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppServer.Common
{
    public interface IConstraintStatementBuilder
    {
        string LikeAnyChar { get; }

        string OpeningBracket { get; }

        string ClosingBracket { get; }

        string MakeAndStatement(string[] expressions);

        string MakeOrStatement(string[] expressions);

        string MakeNullStatement(string testExpression);

        string MakeNotNullStatement(string testExpression);

        string MakeInStatement(string testExpression, string[] expressions);

        string MakeNotInStatement(string testExpression, string[] expressions);

        string MakeBetweenStatement(string testExpression, string beginExpression, string endExpression);

        string MakeNotBetweenStatement(string testExpression, string beginExpression, string endExpression);

        string MakeLikeStatement(string matchExpression, string paternExpression);

        string MakeNotLikeStatement(string matchExpression, string paternExpression);

        string MakeEqualToStatement(string leftExpression, string rightExpression);

        string MakeGreaterThanStatement(string leftExpression, string rightExpression);

        string MakeLessThanStatement(string leftExpression, string rightExpression);

        string MakeGreaterThanOrEqualToStatement(string leftExpression, string rightExpression);

        string MakeLessThanOrEqualToStatement(string leftExpression, string rightExpression);

        string MakeNotEqualToStatement(string leftExpression, string rightExpression);

        string EncodeFieldName(string name);

        string EncodeFieldName(string alias, string name);

        string EncodeValue(string value);

        string EncodeValue(int value);

        string EncodeValue(bool value);

        string EncodeValue(DateTime value);

        string EncodeValue(double value);

        string EncodeValue(decimal value);
    }
}
