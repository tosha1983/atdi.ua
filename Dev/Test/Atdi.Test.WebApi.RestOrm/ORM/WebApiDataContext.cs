namespace Atdi.Test.WebApi.RestOrm.ORM
{
	public class WebApiDataContext
	{
		public readonly string Name;

		public WebApiDataContext(string contextName)
		{
			this.Name = contextName;
		}
	}
}