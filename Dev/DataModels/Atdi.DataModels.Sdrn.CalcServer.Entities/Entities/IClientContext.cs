using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IClientContext_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IClientContext : IClientContext_PK
	{
		IProject PROJECT { get; set; }

		string OwnerInstance { get; set; }

		Guid OwnerContextId { get; set; }

		DateTimeOffset CreatedDate { get; set; }

		IClientContextGlobalParams GLOBAL_PARAMS { get; set; }

		IClientContextMainBlock MAIN_BLOCK { get; set; }

		IClientContextDiffraction DIFFRACTION_BLOCK { get; set; }

		IClientContextSubPathDiffraction SUB_PATH_DIFFRACTION_BLOCK { get; set; }

		IClientContextTropo TROPO_BLOCK { get; set; }

		IClientContextDucting  DUCTING_BLOCK { get; set; }

		IClientContextAbsorption  ABSORPTION_BLOCK { get; set; }

		IClientContextReflection  REFLECTION_BLOCK { get; set; }

		IClientContextAtmospheric ATMOSPHERIC_BLOCK { get; set; }

		IClientContextAdditional ADDITIONAL_BLOCK { get; set; }

		IClientContextClutter CLUTTER_BLOCK { get; set; }

	}

}
