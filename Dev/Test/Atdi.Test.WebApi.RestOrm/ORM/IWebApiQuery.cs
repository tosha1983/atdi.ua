namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public interface IWebApiQuery
	{
		WebApiQueryType QueryType { get; }
	}
	public interface IWebApiQuery<TEntity> : IWebApiQuery
	{

	}

	public enum WebApiQueryType
	{
		Create = 1,
		Read = 2,
		Update = 3,
		Delete = 4
	}
}