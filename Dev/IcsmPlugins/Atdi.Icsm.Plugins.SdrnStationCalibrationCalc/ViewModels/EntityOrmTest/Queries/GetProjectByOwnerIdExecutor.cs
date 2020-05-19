using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.EntityOrmTest.Queries
{
	public class GetProjectByOwnerIdExecutor : IReadQueryExecutor<GetProjectByOwnerId, ProjectModel>
	{
		private readonly AppComponentConfig _config;
		private readonly CalcServerDataLayer _dataLayer;


		public GetProjectByOwnerIdExecutor(AppComponentConfig config, CalcServerDataLayer dataLayer)
		{
			_config = config;
			_dataLayer = dataLayer;
		}

		public ProjectModel Read(GetProjectByOwnerId criterion)
		{
			var query = _dataLayer.GetBuilder<IProject>()
				.Read()
				.Select(
					c => c.Id,
					c => c.Name)
				.Filter(c => c.OwnerProjectId, criterion.OwnerId);

			var reader = _dataLayer.Executor.ExecuteReader(query);

			if (!reader.Read())
			{
				return null;
			}

			return new ProjectModel()
			{
				Id = reader.GetValue(c => c.Id),
				Name = reader.GetValue(c => c.Name),
			};
		}
	}
}
