using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{
    public static class ExpressionExtensions
    {
        public static string GetMemberName(this Expression expression)
        {
            var memberName = string.Empty;

            var currentExpression = expression;
            while (currentExpression.NodeType == ExpressionType.MemberAccess)
            {
                var currentMember = currentExpression as MemberExpression;
                if (!string.IsNullOrEmpty(memberName))
                {
                    memberName = currentMember.Member.Name + "." + memberName;
                }
                else
                {
                    memberName = currentMember.Member.Name;
                }
                currentExpression = currentMember.Expression;
            }

            return memberName;
        }
    }
}
