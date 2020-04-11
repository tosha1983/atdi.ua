using System;
using System.Linq.Expressions;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IDataReader
	{
		long Count { get; }

		bool Read();

		TValue GetValue<TValue>(string path);

		bool IsNull(string path);

		bool IsNotNull(string path);

		bool Has(string path);
	}
	public interface IDataReader<TEntity>
	{
		long Count { get; }

		bool Read();

		TValue GetValue<TValue>(Expression<Func<TEntity, TValue>> pathExpression);

		bool IsNull<TValue>(Expression<Func<TEntity, TValue>> pathExpression);

		bool IsNotNull<TValue>(Expression<Func<TEntity, TValue>> pathExpression);

		bool Has<TValue>(Expression<Func<TEntity, TValue>> pathExpression);
	}
}