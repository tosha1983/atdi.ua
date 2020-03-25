using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.DeepServices.Gis;

namespace Atdi.AppUnits.Sdrn.CalcServer.DeepServices.Gis
{
	public class TransformationService : ITransformation
	{
		public EpsgCoordinate ConvertCoordinateToEpgs(Wgs84Coordinate coordinate, uint toProjection)
		{
			throw new NotImplementedException();
		}

		public EpsgCoordinate ConvertCoordinateToEpgs(EpsgCoordinate coordinate, uint fromProjection, uint toProjection)
		{
			throw new NotImplementedException();
		}

		public EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(Wgs84Coordinate coordinate, uint toProjection)
		{
			throw new NotImplementedException();
		}

		public EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(EpsgProjectionCoordinate coordinate, uint toProjection)
		{
			throw new NotImplementedException();
		}

		public Wgs84Coordinate ConvertCoordinateToWgs84(EpsgCoordinate coordinate, uint fromProjection)
		{
			throw new NotImplementedException();
		}

		public Wgs84Coordinate ConvertCoordinateToWgs84(EpsgProjectionCoordinate coordinate)
		{
			throw new NotImplementedException();
		}

		public string ConvertProjectionToAtdiName(uint projectionCode)
		{
			throw new NotImplementedException();
		}

		public uint ConvertProjectionToCode(string atdiProjection)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
