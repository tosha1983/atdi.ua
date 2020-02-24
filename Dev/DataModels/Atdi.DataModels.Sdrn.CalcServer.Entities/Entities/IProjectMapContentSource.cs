using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IProjectMapContentSource_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IProjectMapContentSource : IProjectMapContentSource_PK
	{
		IProjectMapContent CONTENT { get; set; }

		long InfocMapId { get; set; }

		string InfocMapName { get; set; }

		decimal? Coverage { get; set; }

		int UpperLeftX { get; set; }

		int UpperLeftY { get; set; }

		int LowerRightX { get; set; }

		int LowerRightY { get; set; }
	}

	
}
