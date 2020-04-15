using Atdi.DataModels.Api.EntityOrm.WebClient;

namespace Atdi.Contracts.Api.EntityOrm.WebClient
{
	public interface IWebApiQuery
	{
		WebApiQueryType QueryType { get; }
	}
	public interface IWebApiQuery<TEntity> : IWebApiQuery
	{

	}

	
}