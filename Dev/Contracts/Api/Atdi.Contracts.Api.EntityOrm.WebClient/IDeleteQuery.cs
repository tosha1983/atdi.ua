namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IDeleteQuery : IFilteringQuery<IDeleteQuery>, IWebApiQuery
	{
	}

	public interface IDeleteQuery<TEntity> : IFilteringQuery<TEntity, IDeleteQuery<TEntity>>, IWebApiQuery<TEntity>
	{
	}
}