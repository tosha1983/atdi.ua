using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Api.EntityOrm.WebClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Api.EntityOrm.WebClient
{

	internal static class ConditionBuilder
	{
		public static string Build<TEntity, TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, TValue value)
		{
			if (IsNull(value))
			{
				return $"{leftOperandPathExpression.Body.GetMemberName()} is null";
			}
			return $"{leftOperandPathExpression.Body.GetMemberName()} eq {BuildValue(value)}";
		}

		public static string Build<TEntity, TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params TValue[] values)
		{
			switch (filterOperator)
			{
				case FilterOperator.Equal:
					return $"{leftOperandPathExpression.Body.GetMemberName()} eq {BuildValue(values[0])}";
				case FilterOperator.GreaterEqual:
					return $"{leftOperandPathExpression.Body.GetMemberName()} ge {BuildValue(values[0])}";
				case FilterOperator.GreaterThan:
					return $"{leftOperandPathExpression.Body.GetMemberName()} gt {BuildValue(values[0])}";
				case FilterOperator.LessEqual:
					return $"{leftOperandPathExpression.Body.GetMemberName()} le {BuildValue(values[0])}";
				case FilterOperator.LessThan:
					return $"{leftOperandPathExpression.Body.GetMemberName()} lt {BuildValue(values[0])}";
				case FilterOperator.NotEqual:
					return $"{leftOperandPathExpression.Body.GetMemberName()} nq {BuildValue(values[0])}";
				case FilterOperator.IsNull:
					return $"{leftOperandPathExpression.Body.GetMemberName()} is null";
				case FilterOperator.IsNotNull:
					return $"{leftOperandPathExpression.Body.GetMemberName()} not is null";
				case FilterOperator.In:
					return $"{leftOperandPathExpression.Body.GetMemberName()} in ({string.Join(",", BuildValues(values))})";
				case FilterOperator.NotIn:
					return $"{leftOperandPathExpression.Body.GetMemberName()} not in ({string.Join(",", BuildValues(values))})";
				case FilterOperator.Between:
					return $"{leftOperandPathExpression.Body.GetMemberName()} between {BuildValue(values[0])} and {BuildValue(values[1])}";
				case FilterOperator.NotBetween:
					return $"{leftOperandPathExpression.Body.GetMemberName()} not between {BuildValue(values[0])} and {BuildValue(values[1])}";

				case FilterOperator.Like:
				case FilterOperator.NotLike:
				case FilterOperator.BeginWith:
				case FilterOperator.EndWith:
				case FilterOperator.Contains:
				case FilterOperator.NotBeginWith:
				case FilterOperator.NotEndWith:
				case FilterOperator.NotContains:
				default:
					throw new InvalidOperationException($"Unsupported the filter operator '{filterOperator}'");
			}
		}

		public static string Build<TEntity, TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, Expression<Func<TEntity, TValue>> rightOperandPathExpression)
		{
			return $"{leftOperandPathExpression.Body.GetMemberName()} eq {rightOperandPathExpression.Body.GetMemberName()}";
		}

		public static string Build<TEntity, TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions)
		{
			switch (filterOperator)
			{
				case FilterOperator.Equal:
					return $"{leftOperandPathExpression.Body.GetMemberName()} eq {rightOperandPathExpressions[0].Body.GetMemberName()}";
				case FilterOperator.GreaterEqual:
					return $"{leftOperandPathExpression.Body.GetMemberName()} ge {rightOperandPathExpressions[0].Body.GetMemberName()}";
				case FilterOperator.GreaterThan:
					return $"{leftOperandPathExpression.Body.GetMemberName()} gt {rightOperandPathExpressions[0].Body.GetMemberName()}";
				case FilterOperator.LessEqual:
					return $"{leftOperandPathExpression.Body.GetMemberName()} le {rightOperandPathExpressions[0].Body.GetMemberName()}";
				case FilterOperator.LessThan:
					return $"{leftOperandPathExpression.Body.GetMemberName()} lt {rightOperandPathExpressions[0].Body.GetMemberName()}";
				case FilterOperator.NotEqual:
					return $"{leftOperandPathExpression.Body.GetMemberName()} nq {rightOperandPathExpressions[0].Body.GetMemberName()}";
				case FilterOperator.IsNull:
					return $"{leftOperandPathExpression.Body.GetMemberName()} is null";
				case FilterOperator.IsNotNull:
					return $"{leftOperandPathExpression.Body.GetMemberName()} not is null";
				case FilterOperator.In:
					return $"{leftOperandPathExpression.Body.GetMemberName()} in ({string.Join(",", rightOperandPathExpressions.Select(r => r.Body.GetMemberName()).ToArray())})";
				case FilterOperator.NotIn:
					return $"{leftOperandPathExpression.Body.GetMemberName()} not in ({string.Join(",", rightOperandPathExpressions.Select(r => r.Body.GetMemberName()).ToArray())})";
				case FilterOperator.Between:
					return $"{leftOperandPathExpression.Body.GetMemberName()} between {rightOperandPathExpressions[0].Body.GetMemberName()} and {rightOperandPathExpressions[1].Body.GetMemberName()}";
				case FilterOperator.NotBetween:
					return $"{leftOperandPathExpression.Body.GetMemberName()} not between {rightOperandPathExpressions[0].Body.GetMemberName()} and {rightOperandPathExpressions[2].Body.GetMemberName()}";

				case FilterOperator.Like:
				case FilterOperator.NotLike:
				case FilterOperator.BeginWith:
				case FilterOperator.EndWith:
				case FilterOperator.Contains:
				case FilterOperator.NotBeginWith:
				case FilterOperator.NotEndWith:
				case FilterOperator.NotContains:
				default:
					throw new InvalidOperationException($"Unsupported the filter operator '{filterOperator}'");
			}
		}

		public static bool IsNull<TValue>(TValue value)
		{
			if (value == null)
			{
				return true;
			}

			return false;
		}

		public static string BuildValue<TValue>(TValue value)
		{
			var type = typeof(TValue);
			if (type == typeof(string))
			{
				var realValue = value as string;
				if (realValue == null)
				{
					return "null";
				}

				return $"'{realValue}'";
			}
			if (type == typeof(DateTime))
			{
				var realValue = (DateTime)(object)value;
				return realValue.ToString("O");
			}
			if (type == typeof(DateTime?))
			{
				var realValue = (DateTime?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString("O");
			}

			if (type == typeof(DateTimeOffset))
			{
				var realValue = (DateTimeOffset)(object)value;
				return realValue.ToString("O");
			}
			if (type == typeof(DateTimeOffset?))
			{
				var realValue = (DateTimeOffset?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString("O");
			}

			if (type == typeof(bool))
			{
				return value.ToString();
			}
			if (type == typeof(bool?))
			{
				var realValue = (bool?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString();
			}
			if (type == typeof(int))
			{
				return value.ToString() + "I";
			}
			if (type == typeof(int?))
			{
				var realValue = (int?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString() + "I";
			}

			if (type == typeof(byte))
			{
				return value.ToString() + "B";
			}
			if (type == typeof(byte?))
			{
				var realValue = (byte?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString() + "B";
			}

			if (type == typeof(short))
			{
				return value.ToString() + "S";
			}
			if (type == typeof(short?))
			{
				var realValue = (short?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString() + "S";
			}

			if (type == typeof(long))
			{
				return value.ToString() + "L";
			}
			if (type == typeof(long?))
			{
				var realValue = (long?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString() + "L";
			}

			if (type == typeof(float))
			{
				return value.ToString() + "F";
			}
			if (type == typeof(float?))
			{
				var realValue = (float?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString(CultureInfo.InvariantCulture) + "F";
			}

			if (type == typeof(double))
			{
				return value.ToString() + "D";
			}
			if (type == typeof(double?))
			{
				var realValue = (double?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString(CultureInfo.InvariantCulture) + "D";
			}

			if (type == typeof(Guid))
			{
				return "'" + value.ToString() + "'";
			}
			if (type == typeof(Guid?))
			{
				var realValue = (Guid?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return "'" + realValue.Value.ToString() + "'";
			}

			if (type == typeof(decimal))
			{
				return value.ToString() + "N";
			}
			if (type == typeof(decimal?))
			{
				var realValue = (decimal?)(object)value;
				if (!realValue.HasValue)
				{
					return "null";
				}
				return realValue.Value.ToString(CultureInfo.InvariantCulture) +"N";
			}

			return value.ToString();
		}

		public static string[] BuildValues<TValue>(params TValue[] values)
		{
			return values.Select(BuildValue).ToArray();
		}
	}

	internal class QueryFilter<TEntity, TQuery> : IQueryFilter<TEntity, TQuery>
	{
		
		private readonly IFilterSite _filterSite;
		private readonly TQuery _query;
		private readonly StringBuilder _builder;
		private bool _processing;

		public QueryFilter(IFilterSite filterSite, TQuery query)
		{
			_filterSite = filterSite;
			_query = query;
			_processing = true;
			_builder = new StringBuilder();
		}

		public IQueryFilter<TEntity, TQuery> And()
		{
			ValidateSate();
			_builder.Append(" and ");
			return this;
		}

		public IQueryFilter<TEntity, TQuery> Begin()
		{
			ValidateSate();
			_builder.Append("(");
			return this;
		}

		public IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, TValue value)
		{
			ValidateSate();
			_builder.Append(ConditionBuilder.Build(leftOperandPathExpression, value));
			return this;
		}

		public IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params TValue[] values)
		{
			ValidateSate();
			_builder.Append(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, values));
			return this;
		}

		public IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, Expression<Func<TEntity, TValue>> rightOperandPathExpression)
		{
			ValidateSate();
			_builder.Append(ConditionBuilder.Build(leftOperandPathExpression, rightOperandPathExpression));
			return this;
		}

		public IQueryFilter<TEntity, TQuery> Condition<TValue>(Expression<Func<TEntity, TValue>> leftOperandPathExpression, FilterOperator filterOperator, params Expression<Func<TEntity, TValue>>[] rightOperandPathExpressions)
		{
			ValidateSate();
			_builder.Append(ConditionBuilder.Build(leftOperandPathExpression, filterOperator, rightOperandPathExpressions));
			return this;
		}

		public IQueryFilter<TEntity, TQuery> End()
		{
			ValidateSate();
			_builder.Append(")");
			return this;
		}

		public TQuery EndFilter()
		{
			ValidateSate();
			_processing = false;
			if (_builder.Length == 0)
			{
				return _query;
			}
			_filterSite.SetFilter(_builder.ToString());
			return _query;
		}

		public IQueryFilter<TEntity, TQuery> Or()
		{
			ValidateSate();
			_builder.Append(" or ");
			return this;
		}

		private void ValidateSate()
		{
			if (!_processing)
			{
				throw new InvalidOperationException("Incorrect processing state. Filter building was finished");
			}
		}
	}
}
