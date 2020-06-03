using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Platform.AppComponent;
using Atdi.Platform.Data;
using Atdi.Platform.DependencyInjection;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Clients;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks
{
    public class AppServerComponent : AppUnitComponent
	{
		public AppServerComponent()
			: base("SdrnCalcServerTasksAppUnit")
		{
			
		}

		protected override void OnInstallUnit()
		{
			// конфигурация
			var componentConfig = this.Config.Extract<AppServerComponentConfig>();
			this.Container.RegisterInstance(componentConfig, ServiceLifetime.Singleton);
		}

		protected override void OnActivateUnit()
		{
			var poolSite = this.Resolver.Resolve<IObjectPoolSite>();
			var appConfig = this.Resolver.Resolve<AppServerComponentConfig>();
			var indexerArrayPool = poolSite.Register(new ObjectPoolDescriptor<ProfileIndexer[]>()
			{
				Key = ObjectPools.GisProfileIndexerArrayObjectPool,
				MinSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(0),
				MaxSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(10),
				Factory = () => new ProfileIndexer[appConfig.ThresholdsGisProfileDataArrayLength.GetValueOrDefault(10_000)]
			});

			var clutterArrayPool = poolSite.Register(new ObjectPoolDescriptor<byte[]>()
			{
				Key = ObjectPools.GisProfileClutterArrayObjectPool,
				MinSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(0),
				MaxSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(10),
				Factory = () => new byte[appConfig.ThresholdsGisProfileDataArrayLength.GetValueOrDefault(10_000)]
			});

			var buildingArrayPool = poolSite.Register(new ObjectPoolDescriptor<byte[]>()
			{
				Key = ObjectPools.GisProfileBuildingArrayObjectPool,
				MinSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(0),
				MaxSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(10),
				Factory = () => new byte[appConfig.ThresholdsGisProfileDataArrayLength.GetValueOrDefault(10_000)]
			});

			var reliefArrayPool = poolSite.Register(new ObjectPoolDescriptor<short[]>()
			{
				Key = ObjectPools.GisProfileReliefArrayObjectPool,
				MinSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(0),
				MaxSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(10),
				Factory = () => new short[appConfig.ThresholdsGisProfileDataArrayLength.GetValueOrDefault(10_000)]
			});

			var heightArrayPool = poolSite.Register(new ObjectPoolDescriptor<short[]>()
			{
				Key = ObjectPools.GisProfileHeightArrayObjectPool,
				MinSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(0),
				MaxSize = appConfig.ThresholdsGisProfileDataObjectPoolMaxSize.GetValueOrDefault(10),
				Factory = () => new short[appConfig.ThresholdsGisProfileDataArrayLength.GetValueOrDefault(10_000)]
			});


           var driveCalcPointArrayPool = poolSite.Register(new ObjectPoolDescriptor<CalcPoint[]>()
           {
                Key = ObjectPools.StationCalibrationCalcPointArrayObjectPool,
                MinSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(0),
                MaxSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(10),
                Factory = () => new CalcPoint[appConfig.ThresholdsStationCalibrationArrayLength.GetValueOrDefault(1_0000)]
            });

            var driveCalcPointFSArrayPool = poolSite.Register(new ObjectPoolDescriptor<PointFS[]>()
            {
                Key = ObjectPools.StationCalibrationPointFSArrayObjectPool,
                MinSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(0),
                MaxSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(10),
                Factory = () => new PointFS[appConfig.ThresholdsStationCalibrationArrayLength.GetValueOrDefault(1_0000)]
            });

            var lstDriveTestsResultPool = poolSite.Register(new ObjectPoolDescriptor<DriveTestsResult[][]>()
            {
                Key = ObjectPools.StationCalibrationListDriveTestsResultObjectPool,
                MinSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(0),
                MaxSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(10),
                Factory = () => new DriveTestsResult[appConfig.ThresholdsStationCalibrationArrayLength.GetValueOrDefault(1_0000)][]
            });

            var calibrationResultlPool = poolSite.Register(new ObjectPoolDescriptor<CalibrationResult[]>()
            {
                Key = ObjectPools.StationCalibrationResultObjectPool,
                MinSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(0),
                MaxSize = appConfig.ThresholdsStationCalibrationObjectPoolMaxSize.GetValueOrDefault(10),
                Factory = () => new CalibrationResult[appConfig.ThresholdsStationCalibrationArrayLength.GetValueOrDefault(1_0000)]
            });


            

            base.OnActivateUnit();
		}
	}
}
