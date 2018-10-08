using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer
{
    public interface IQueryDeleteStatement : IQueryStatement
    {
        IQueryDeleteStatement Where(Condition condition);
    }

    public interface IQueryDeleteStatement<TModel> : IQueryStatement
    {
        IQueryDeleteStatement<TModel> Where(Expression<Func<TModel, bool>> expression);

        IQueryDeleteStatement<TModel> Where(Expression<Func<TModel, IQuerySelectStatementOperation, bool>> expression);

        IQueryDeleteStatement<TModel> Where<TValue>(Expression<Func<TModel, TValue>> columnExpression, ConditionOperator conditionOperator, params TValue[] values);
    }

    public static class QueryDeleteStatementExtensions
    {
        public static IQueryDeleteStatement Where(this IQueryDeleteStatement query, Condition[] conditions)
        {
            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }
            for (int i = 0; i < conditions.Length; i++)
            {
                query.Where(conditions[i]);
            }
            return query;
        }
    }
}
