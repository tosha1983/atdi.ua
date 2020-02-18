using Atdi.Platform.AppComponent;

namespace Atdi.WebApiServices.Sdrn.Infocenter
{
	public sealed class AppServerComponent : WebApiServicesComponent
	{
		public AppServerComponent() 
			: base("SdrnInfocenterWebApiServices", ComponentBehavior.Simple)
		{
		}
	}
}
