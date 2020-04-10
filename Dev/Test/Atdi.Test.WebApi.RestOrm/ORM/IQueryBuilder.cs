namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IQueryBuilder
	{
		string EntityName { get; }

		string EntityNamespace { get; }

		ICreateQuery Create();

		IReadQuery Read();

		IUpdateQuery Update();

		IDeleteQuery Delete();
	}

	public interface IQueryBuilder<TEntity>
	{
		string EntityName { get; }

		string EntityNamespace { get; }

		ICreateQuery<TEntity> Create();

		IReadQuery<TEntity> Read();

		IUpdateQuery<TEntity> Update();

		IDeleteQuery<TEntity> Delete();
	}
}