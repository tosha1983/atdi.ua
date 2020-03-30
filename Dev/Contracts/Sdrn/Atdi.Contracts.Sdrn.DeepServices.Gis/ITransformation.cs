using Atdi.DataModels.Sdrn.DeepServices.Gis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.Sdrn.DeepServices.Gis
{
	
	public interface ITransformation : IDeepService
	{
		string ConvertProjectionToAtdiName(uint projectionCode);

		uint ConvertProjectionToCode(string atdiProjection);


		EpsgCoordinate ConvertCoordinateToEpgs(Wgs84Coordinate coordinate, uint toProjection);

		EpsgCoordinate ConvertCoordinateToEpgs(EpsgCoordinate coordinate, uint fromProjection, uint toProjection);

		EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(Wgs84Coordinate coordinate, uint toProjection);

		EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(EpsgProjectionCoordinate coordinate, uint toProjection);

		Wgs84Coordinate ConvertCoordinateToWgs84(EpsgCoordinate coordinate, uint fromProjection);

		Wgs84Coordinate ConvertCoordinateToWgs84(EpsgProjectionCoordinate coordinate);
	}
}
