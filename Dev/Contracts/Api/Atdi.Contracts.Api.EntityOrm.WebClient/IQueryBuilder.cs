namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IQueryBuilder
	{
		string EntityName { get; }

		string EntityNamespace { get; }

		ICreateQuery Create();

		IReadQuery Read();

		IUpdateQuery Update();

		IDeleteQuery Delete();

		IApplyQuery Apply();
	}

	public interface IQueryBuilder<TEntity>
	{
		string EntityName { get; }

		string EntityNamespace { get; }

		ICreateQuery<TEntity> Create();

		IReadQuery<TEntity> Read();

		IUpdateQuery<TEntity> Update();

		IDeleteQuery<TEntity> Delete();

		IApplyQuery<TEntity> Apply();
	}
}