using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeepServices.IDWM;
using Atdi.DataModels.Sdrn.DeepServices.IDWM;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.GN06;


namespace Atdi.AppUnits.Sdrn.DeepServices.GN06
{
    public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnDeepServicesGN06AppUnit")
		{


        }

		protected override void OnInstallUnit()
		{
            this.Container.Register<IGn06Service, EstimationAssignmentsService>(Platform.DependencyInjection.ServiceLifetime.PerThread);
        }
	}
}
