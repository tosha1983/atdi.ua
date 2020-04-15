using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Test.WebApi.RestOrm.ORM
{
	internal static class ExpressionExtension
	{
		public static string GetMemberName(this Expression expression)
		{
			var memberName = string.Empty;

			var currentExpression = expression;
			if (currentExpression.NodeType == ExpressionType.Convert)
			{
				var unaryExpression = currentExpression as UnaryExpression;
				currentExpression = unaryExpression.Operand;
			}

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

			if (string.IsNullOrEmpty(memberName))
			{
				throw new InvalidOperationException($"A member name in the expression '{expression}' is not defined.");
			}
			return memberName;
		}
	}

}
