namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IDeleteQuery : IFilteringQuery<IDeleteQuery>, IWebApiQuery
	{
	}

	public interface IDeleteQuery<TEntity> : IFilteringQuery<TEntity, IDeleteQuery<TEntity>>, IWebApiQuery<TEntity>
	{
	}
}