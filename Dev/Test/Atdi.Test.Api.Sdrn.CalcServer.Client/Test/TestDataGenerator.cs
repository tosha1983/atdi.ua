using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Test.Api.Sdrn.CalcServer.Client.Test
{
	public static class TestDataGenerator
	{
		public static TInterface Create<TInterface>()
		{
			return default(TInterface);
		}

		public static IProject GenerateProject()
		{
			var project = Create<IProject>();

			project.Name = "Test project";

			return project;
		}

		public static IClientContext GenerateClientContext()
		{
			var clientContext = Create<IClientContext>();

			clientContext.OwnerContextId = Guid.NewGuid();

			clientContext.MAIN_BLOCK = Create<IClientContextMainBlock>();
			clientContext.MAIN_BLOCK.ModelTypeCode = (byte)MainBlockModelTypeCode.ITU1546;

			clientContext.ABSORPTION_BLOCK = Create<IClientContextAbsorption>();
			clientContext.ABSORPTION_BLOCK.Available = true;
			clientContext.ABSORPTION_BLOCK.ModelTypeCode = (byte) AbsorptionModelTypeCode.Flat;

			//  итак все блоки
			return clientContext;
		}
		public static IContextStation[] GenerateContextStations()
		{
			var contextStations = new IContextStation[10];

			for (int i = 0; i < contextStations.Length; i++)
			{
				var contextStation = Create<IContextStation>();
				contextStation.Name = "Test station";

				contextStation.SITE = Create<IContextStationSite>();
				contextStation.SITE.Longitude_DEC = 1;

				// и так все остальные вложенные  объекты

				contextStations[i] = contextStation;
			}
			
			return contextStations;
		}

		public static IPointFieldStrengthCalcTask GeneratePointFieldStrengthCalcTask()
		{
			var task = Create<IPointFieldStrengthCalcTask>();

			task.MapName = "MainMap";
			task.PointAltitude_m = 100;

			return task;
		}
	}
}
