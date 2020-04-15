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
			clientContext.MAIN_BLOCK.ModelTypeCode = (byte)MainBlockModelTypeCode.ITU525;

			clientContext.ABSORPTION_BLOCK = Create<IClientContextAbsorption>();
			clientContext.ABSORPTION_BLOCK.Available = true;
			clientContext.ABSORPTION_BLOCK.ModelTypeCode = (byte) AbsorptionModelTypeCode.FlatAndLinear;

            clientContext.ADDITIONAL_BLOCK = Create<IClientContextAdditional>();
            clientContext.ADDITIONAL_BLOCK.Available = false;

            clientContext.ATMOSPHERIC_BLOCK = Create<IClientContextAtmospheric>();
            clientContext.ATMOSPHERIC_BLOCK.Available = false;

            clientContext.CLUTTER_BLOCK = Create<IClientContextClutter>();
            clientContext.CLUTTER_BLOCK.Available = false;

            clientContext.DIFFRACTION_BLOCK = Create<IClientContextDiffraction>();
            clientContext.DIFFRACTION_BLOCK.Available = true;
            clientContext.DIFFRACTION_BLOCK.ModelTypeCode = (byte)DiffractionModelTypeCode.Deygout91;
            
            clientContext.DUCTING_BLOCK = Create<IClientContextDucting>();
            clientContext.DUCTING_BLOCK.Available = false;

            clientContext.GLOBAL_PARAMS = Create<IClientContextGlobalParams>();
            clientContext.GLOBAL_PARAMS.EarthRadius_km = 8500;
            clientContext.GLOBAL_PARAMS.Location_pc = 50;
            clientContext.GLOBAL_PARAMS.Time_pc = 50;

            clientContext.REFLECTION_BLOCK = Create<IClientContextReflection>();
            clientContext.REFLECTION_BLOCK.Available = false;

            clientContext.SUB_PATH_DIFFRACTION_BLOCK = Create<IClientContextSubPathDiffraction>();
            clientContext.SUB_PATH_DIFFRACTION_BLOCK.Available = false;

            clientContext.TROPO_BLOCK = Create<IClientContextTropo>();
            clientContext.TROPO_BLOCK.Available = false; 

            return clientContext;
		}
		public static IContextStation[] GenerateContextStations()
		{
			var contextStations = new IContextStation[10];
            Random rand = new Random();
			for (int i = 0; i < contextStations.Length; i++)
			{
				var contextStation = Create<IContextStation>();
				contextStation.Name = "Test station";
				contextStation.SITE = Create<IContextStationSite>();
				contextStation.SITE.Longitude_DEC = 24.03174 + 0.1* (0.5 - rand.NextDouble()); // на самом деле лучше брать из списка станций которые у наc есть
                contextStation.SITE.Latitude_DEC = 49.86417 + 0.1 * (0.5 - rand.NextDouble()); // на самом деле лучше брать из списка станций которые у наc есть
                contextStation.SITE.Altitude_m = 30;
                contextStation.TRANSMITTER = Create<IContextStationTransmitter>();
                contextStation.TRANSMITTER.PolarizingCode = (byte)PolarizationCode.H;
                contextStation.TRANSMITTER.Loss_dB = 3;
                contextStation.TRANSMITTER.MaxPower_dBm = 30;
                contextStation.TRANSMITTER.Freq_MHz = 10000;
                contextStation.ANTENNA = Create<IContextStationAntenna>();
                contextStation.ANTENNA.Gain_dB = 15;
                contextStation.ANTENNA.XPD_dB = 15;
                contextStation.ANTENNA.Azimuth_deg = 135;
                contextStation.ANTENNA.Tilt_deg = -5;
                var patternH = Create<IContextStationPattern>();
                patternH.Angle_deg = new double[12] {0,30,60,90,120,150,180,210,240,270,300,330};
                patternH.Loss_dB = new float[12] { 0, 3, 10, 20, 25, 30, 25, 30, 25, 20, 10, 3};
                var patternV = Create<IContextStationPattern>();
                patternV.Angle_deg = new double[17] { -90, -60, -30, -15, -10, -5, -3, -1, 0, 1, 3, 5, 10, 15, 30, 60, 90 };
                patternV.Loss_dB = new float[17] { 25, 20, 15, 10, 9,7,5,2,0,2,5,7,9,10,15,20,25};
                contextStation.ANTENNA.HV_PATTERN = patternH;
                contextStation.ANTENNA.HH_PATTERN = patternH;
                contextStation.ANTENNA.VV_PATTERN = patternV;
                contextStation.ANTENNA.VH_PATTERN = patternV;
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
