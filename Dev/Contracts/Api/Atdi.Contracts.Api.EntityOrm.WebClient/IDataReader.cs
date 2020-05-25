using System;
using System.Linq.Expressions;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IDataReader
	{
		long Position { get; }

		long Count { get; }

		bool Read();

		TValue GetValue<TValue>(string path);

		bool IsNull(string path);

		bool IsNotNull(string path);

		bool Has(string path);

		bool MoveTo(int index);
	}
	public interface IDataReader<TEntity>
	{
		long Position { get; }

		long Count { get; }

		bool Read();

		TData GetValueAs<TData>(Expression<Func<TEntity, byte[]>> columnExpression);

		TValue GetValue<TValue>(Expression<Func<TEntity, TValue>> pathExpression);

		bool IsNull<TValue>(Expression<Func<TEntity, TValue>> pathExpression);

		bool IsNotNull<TValue>(Expression<Func<TEntity, TValue>> pathExpression);

		bool Has<TValue>(Expression<Func<TEntity, TValue>> pathExpression);

		bool MoveTo(int index);
	}
}