﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Platform.AppComponent;

namespace Atdi.AppUnits.Sdrn.Infocenter.Integration
{
	class AppServerComponentConfig
	{

		[ComponentConfigProperty("AutoImport.SdrnServer.StartDelay")]
		public int? AutoImportSdrnServerStartDelay { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.RepeatDelay")]
		public int? AutoImportSdrnServerRepeatDelay { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.StationMonitoring.FetchRows")]
		public int? AutoImportSdrnServerStationMonitoringFetchRows { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.StationMonitoring.Period")]
		public int? AutoImportSdrnServerStationMonitoringPeriod { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.StationMonitoring.FirstResultId")]
		public long? AutoImportSdrnServerStationMonitoringFirstResultId { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.StationMonitoring.Points.FetchRows")]
		public int? AutoImportSdrnServerStationMonitoringPointsFetchRows { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.StationMonitoring.Points.BufferSize")]
		public int? AutoImportSdrnServerStationMonitoringPointsBufferSize { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.StationMonitoring.Route.BufferSize")]
		public int? AutoImportSdrnServerStationMonitoringRouteBufferSize { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.Sensors.FetchRows")]
		public int? AutoImportSdrnServerSensorsFetchRows { get; set; }
		[ComponentConfigProperty("AutoImport.SdrnServer.Sensors.Period")]
		public int? AutoImportSdrnServerSensorsPeriod { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.SensorAntennas.FetchRows")]
		public int? AutoImportSdrnServerSensorAntennasFetchRows { get; set; }
		[ComponentConfigProperty("AutoImport.SdrnServer.SensorAntennas.Period")]
		public int? AutoImportSdrnServerSensorAntennasPeriod { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.SensorAntennaPatterns.FetchRows")]
		public int? AutoImportSdrnServerSensorAntennaPatternsFetchRows { get; set; }
		[ComponentConfigProperty("AutoImport.SdrnServer.SensorAntennaPatterns.Period")]
		public int? AutoImportSdrnServerSensorAntennaPatternsPeriod { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.SensorEquipment.FetchRows")]
		public int? AutoImportSdrnServerSensorEquipmentFetchRows { get; set; }
		[ComponentConfigProperty("AutoImport.SdrnServer.SensorEquipment.Period")]
		public int? AutoImportSdrnServerSensorEquipmentPeriod { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.SensorEquipmentSensitivities.FetchRows")]
		public int? AutoImportSdrnServerSensorEquipmentSensitivitiesFetchRows { get; set; }
		[ComponentConfigProperty("AutoImport.SdrnServer.SensorEquipmentSensitivities.Period")]
		public int? AutoImportSdrnServerSensorEquipmentSensitivitiesPeriod { get; set; }

		[ComponentConfigProperty("AutoImport.SdrnServer.SensorLocations.FetchRows")]
		public int? AutoImportSdrnServerSensorLocationsFetchRows { get; set; }
		[ComponentConfigProperty("AutoImport.SdrnServer.SensorLocations.Period")]
		public int? AutoImportSdrnServerSensorLocationsPeriod { get; set; }

		[ComponentConfigProperty("AutoImport.Files.Folder")]
		public string AutoImportFilesFolder { get; set; }

		[ComponentConfigProperty("AutoImport.Files.StartDelay")]
		public int? AutoImportFilesStartDelay { get; set; }

		[ComponentConfigProperty("AutoImport.Files.RepeatDelay")]
		public int? AutoImportFilesRepeatDelay { get; set; }


		[ComponentConfigProperty("AutoImport.CalcServer.StartDelay")]
		public int? AutoImportCalcServerStartDelay { get; set; }

		[ComponentConfigProperty("AutoImport.CalcServer.RepeatDelay")]
		public int? AutoImportCalcServerRepeatDelay { get; set; }

		[ComponentConfigProperty("AutoImport.CalcServer.CalcResults.FetchRows")]
		public int? AutoImportCalcServerCalcResultsFetchRows { get; set; }
	}
}
