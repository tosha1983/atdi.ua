using System;
using System.Linq.Expressions;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface ICreateQuery : IWebApiQuery
	{
		ICreateQuery SetValue<TValue>(string path, TValue value);
	}

	public interface ICreateQuery<TEntity> : IWebApiQuery<TEntity>
	{
		ICreateQuery<TEntity> SetValue<TValue>(Expression<Func<TEntity, TValue>> pathExpression, TValue value);

		ICreateQuery<TEntity> SetValueAsJson<TValue>(Expression<Func<TEntity, string>> pathExpression, TValue value);
	}
}