using System;
using System.Linq.Expressions;

namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface ICreateQuery : IWebApiQuery
	{
		ICreateQuery SetValue<TValue>(string path, TValue value);
	}

	public interface ICreateQuery<TEntity> : IWebApiQuery<TEntity>
	{
		ICreateQuery<TEntity> SetValue<TValue>(Expression<Func<TEntity, TValue>> pathExpression, TValue value);
	}
}