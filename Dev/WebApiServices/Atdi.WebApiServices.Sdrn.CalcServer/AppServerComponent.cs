using Atdi.Platform.AppComponent;

namespace Atdi.WebApiServices.Sdrn.CalcServer
{
	public sealed class AppServerComponent : WebApiServicesComponent
	{
		public AppServerComponent() 
			: base("SdrnCalcServerWebApiServices", ComponentBehavior.Simple)
		{
		}
	}
}
