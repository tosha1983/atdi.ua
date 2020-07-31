using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.Platform.Logging;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	/// <summary>
    /// 
	/// </summary>
	public class StationCalibrationCalcIteration : IIterationHandler<StationCalibrationCalcData, ResultCorrelationGSIDGroupeStations>
	{
		private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;
        private readonly IEarthGeometricService _earthGeometricService;

        //
        private readonly Platform.Data.IObjectPoolSite _poolSite;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public StationCalibrationCalcIteration(
            IIterationsPool iterationsPool,
            ITransformation transformation,
            IEarthGeometricService earthGeometricService,
            Platform.Data.IObjectPoolSite poolSite,
            ILogger logger)
		{
            _transformation = transformation;
            _iterationsPool = iterationsPool;
            _poolSite = poolSite;
            _earthGeometricService = earthGeometricService;
            _logger = logger;
		}


        /// <summary>
        /// Exhaustive Search Station Callibration Method
        /// </summary>
        /// <param name="taskContext"></param>
        /// <param name="calibrationData"></param>
        /// <param name="correlationData"></param>
        /// <param name="correlationResult"></param>
        /// <param name="corellationCalcIteration"></param>
        private void ExhaustiveSearchStationCallibration(in ITaskContext taskContext, ref StationCalibrationCalcData calibrationData, ref StationCorellationCalcData correlationData, ref ResultCorrelationGSIDGroupeStationsWithoutParameters correlationResult, ref StationCorellationCalcIteration corellationCalcIteration)
        {
            correlationResult = corellationCalcIteration.Run(taskContext, correlationData);

            //var data = Atdi.Common.CopyHelper.CreateDeepCopy(calibrationData);

            var maxCorrelation_pc = correlationResult.Corellation_pc;
            //Define ranges and steps
            int powerCalcIterations = 1;
            int coordinatesStationMinX_m = calibrationData.GSIDGroupeStation.Coordinate.X;
            int coordinatesStationMinY_m = calibrationData.GSIDGroupeStation.Coordinate.Y;
            int coordinatesStationStep_m = 1;
            int coordinatesStationMaxX_m = calibrationData.GSIDGroupeStation.Coordinate.X + coordinatesStationStep_m;
            int coordinatesStationMaxY_m = calibrationData.GSIDGroupeStation.Coordinate.Y + coordinatesStationStep_m;
            

            var altitudeStationMin_m = calibrationData.GSIDGroupeStation.Site.Altitude;
            var altitudeStationStep_m = 1.0f;
            var altitudeStationMax_m = calibrationData.GSIDGroupeStation.Site.Altitude + altitudeStationStep_m;

            var azimuthStationMin = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg;
            var azimuthStationStep = 1.0f;
            var azimuthStationMax = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg + azimuthStationStep;

            var tiltStationMin = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg;
            var tiltStationStep = 1.0f;
            var tiltStationMax = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg + tiltStationStep;

            var powerStationMax = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;

            //координаты, высота, азимут, угол места, мощность
            if (calibrationData.CalibrationParameters.CoordinatesStation)
            {
                coordinatesStationMinX_m = calibrationData.GSIDGroupeStation.Coordinate.X - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinX_m < calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.X) { coordinatesStationMinX_m = calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.X; }
                coordinatesStationMinY_m = calibrationData.GSIDGroupeStation.Coordinate.Y - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinY_m < calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.Y) { coordinatesStationMinY_m = calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.Y; }
                coordinatesStationMaxX_m = calibrationData.GSIDGroupeStation.Coordinate.X + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxX_m > calibrationData.FieldStrengthCalcData.MapArea.UpperRight.X) { coordinatesStationMaxX_m = calibrationData.FieldStrengthCalcData.MapArea.UpperRight.X; }
                coordinatesStationMaxY_m = calibrationData.GSIDGroupeStation.Coordinate.Y + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxY_m > calibrationData.FieldStrengthCalcData.MapArea.UpperRight.Y) { coordinatesStationMaxY_m = calibrationData.FieldStrengthCalcData.MapArea.UpperRight.Y; }
                coordinatesStationStep_m = calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
            }
            
            if (calibrationData.CalibrationParameters.AltitudeStation)
            {
                if (correlationData.GSIDGroupeStation.Site.Altitude < -calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m)
                {
                    calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m = (int)-correlationData.GSIDGroupeStation.Site.Altitude;
                }
                altitudeStationMin_m = calibrationData.GSIDGroupeStation.Site.Altitude + calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m;
                altitudeStationMax_m = calibrationData.GSIDGroupeStation.Site.Altitude + calibrationData.CalibrationParameters.ShiftAltitudeStationMax_m;
                altitudeStationStep_m = calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
            }
            
            if (calibrationData.CalibrationParameters.AzimuthStation)
            {
                azimuthStationMin = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg + calibrationData.CalibrationParameters.ShiftAzimuthStationMin_deg;
                azimuthStationMax = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg + calibrationData.CalibrationParameters.ShiftAzimuthStationMax_deg;
                azimuthStationStep = calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                if (azimuthStationMin < -360) { azimuthStationMin = -360; }
                if (azimuthStationMax > 360) { azimuthStationMax = 360; }
            }

            if (calibrationData.CalibrationParameters.TiltStation)
            {
                tiltStationMin = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg + calibrationData.CalibrationParameters.ShiftTiltStationMin_Deg;
                tiltStationMax = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg + calibrationData.CalibrationParameters.ShiftTiltStationMax_Deg;
                tiltStationStep = calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                if (tiltStationMin < -90) { tiltStationMax = -90; }
                if (tiltStationMax > 90) { tiltStationMax = 90; }
            }

            if (calibrationData.CalibrationParameters.PowerStation)
            {
                powerStationMax += calibrationData.CalibrationParameters.ShiftPowerStationMax_dB;//??
                if (calibrationData.CalibrationParameters.CascadeTuning)
                {
                    powerCalcIterations = calibrationData.CalibrationParameters.NumberCascade;
                }
            }

            //координаты, высота, азимут, угол места, мощность
            for (var coordinatesStationX = coordinatesStationMinX_m; coordinatesStationX < coordinatesStationMaxX_m; coordinatesStationX += coordinatesStationStep_m)
            {
                correlationData.GSIDGroupeStation.Site.Longitude = coordinatesStationX;
                
                for (var coordinatesStationY = coordinatesStationMinY_m; coordinatesStationY < coordinatesStationMaxY_m; coordinatesStationY += coordinatesStationStep_m)
                {
                    correlationData.GSIDGroupeStation.Site.Latitude = coordinatesStationY;
                    
                    // высота, азимут, угол места, мощность
                    for (var altitudeStation = altitudeStationMin_m; altitudeStation < altitudeStationMax_m; altitudeStation += altitudeStationStep_m)
                    {
                        correlationData.GSIDGroupeStation.Site.Altitude = altitudeStation;
                        
                        // азимут, угол места, мощность
                        for (var azimuthStation = azimuthStationMin; azimuthStation < azimuthStationMax; azimuthStation += azimuthStationStep)
                        {
                            correlationData.GSIDGroupeStation.Antenna.Azimuth_deg = azimuthStation;
                            
                            // угол места, мощность
                            for (var tiltStation = tiltStationMin; tiltStation < tiltStationMax; tiltStation += tiltStationStep)
                            {
                                correlationData.GSIDGroupeStation.Antenna.Tilt_deg = tiltStation;
                                
                                // мощность
                                for (int i = 0; i < powerCalcIterations; i++)
                                {
                                    correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm += correlationResult.AvErr_dB;
                                    
                                    if (correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm > powerStationMax)
                                    {
                                        powerCalcIterations -= i;
                                        correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm = powerStationMax;
                                    }
                                    correlationResult = corellationCalcIteration.Run(taskContext, correlationData);

                                    //data.CalibrationParameters.ShiftPowerStationStep_dB < correlationResult.AvErr_dB
                                    if (maxCorrelation_pc < correlationResult.Corellation_pc)
                                    {
                                        calibrationData.GSIDGroupeStation.Site.Longitude = correlationData.GSIDGroupeStation.Site.Longitude;
                                        calibrationData.GSIDGroupeStation.Site.Latitude = correlationData.GSIDGroupeStation.Site.Latitude;
                                        calibrationData.GSIDGroupeStation.Site.Altitude = correlationData.GSIDGroupeStation.Site.Altitude;
                                        calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg = correlationData.GSIDGroupeStation.Antenna.Azimuth_deg;
                                        calibrationData.GSIDGroupeStation.Antenna.Tilt_deg = correlationData.GSIDGroupeStation.Antenna.Tilt_deg;
                                        calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm = correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;
                                        maxCorrelation_pc = correlationResult.Corellation_pc;
                                    }

                                }
                            }
                        }
                    }
                }
            }

            //calibrationData = data;
        }

        /// <summary>
        /// Quick Descent Station Callibration Method
        /// </summary>
        /// <param name="taskContext"></param>
        /// <param name="calibrationData"></param>
        /// <param name="correlationData"></param>
        /// <param name="correlationResult"></param>
        /// <param name="corellationCalcIteration"></param>
        private void QuickDescentStationCallibration(in ITaskContext taskContext, ref StationCalibrationCalcData calibrationData, ref StationCorellationCalcData correlationData, ref ResultCorrelationGSIDGroupeStationsWithoutParameters correlationResult, ref StationCorellationCalcIteration corellationCalcIteration)
        {
            //var corellationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            //StationCorellationCalcIteration corellationCalcIteration = new StationCorellationCalcIteration(_iterationsPool, _transformation, _poolSite, _logger);
            //var data = Atdi.Common.CopyHelper.CreateDeepCopy(calibrationData);

            correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
            //var currentCorrelation_pc = correlationResult.Corellation_pc;
            var maxCorrelation_pc = correlationResult.Corellation_pc;
            
            var coordinatesStationMinX_m = calibrationData.GSIDGroupeStation.Coordinate.X;
            var coordinatesStationMaxX_m = calibrationData.GSIDGroupeStation.Coordinate.X;
            var coordinatesStationMinY_m = calibrationData.GSIDGroupeStation.Coordinate.Y;
            var coordinatesStationMaxY_m = calibrationData.GSIDGroupeStation.Coordinate.Y;
            //////////////////////////////////////

            var altitudeStationMin_m = calibrationData.GSIDGroupeStation.Site.Altitude;
            var altitudeStationMax_m = calibrationData.GSIDGroupeStation.Site.Altitude;

            var azimuthStationMin = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg;
            var azimuthStationMax = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg;

            var tiltStationMin = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg;
            var tiltStationMax = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg;

            var powerStationMax = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;
            var powerStationMin = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;

            //координаты, высота, азимут, угол места, мощность
            if (calibrationData.CalibrationParameters.CoordinatesStation)
            {
                coordinatesStationMinX_m = calibrationData.GSIDGroupeStation.Coordinate.X - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinX_m < calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.X) { coordinatesStationMinX_m = calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.X; }
                coordinatesStationMinY_m = calibrationData.GSIDGroupeStation.Coordinate.Y - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinY_m < calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.Y) { coordinatesStationMinY_m = calibrationData.FieldStrengthCalcData.MapArea.LowerLeft.Y; }
                coordinatesStationMaxX_m = calibrationData.GSIDGroupeStation.Coordinate.X + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxX_m > calibrationData.FieldStrengthCalcData.MapArea.UpperRight.X) { coordinatesStationMaxX_m = calibrationData.FieldStrengthCalcData.MapArea.UpperRight.X; }
                coordinatesStationMaxY_m = calibrationData.GSIDGroupeStation.Coordinate.Y + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxY_m > calibrationData.FieldStrengthCalcData.MapArea.UpperRight.Y) { coordinatesStationMaxY_m = calibrationData.FieldStrengthCalcData.MapArea.UpperRight.Y; }
                
            }

            if (calibrationData.CalibrationParameters.AltitudeStation)
            {
                if (correlationData.GSIDGroupeStation.Site.Altitude < -calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m)
                {
                    calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m = (int)-correlationData.GSIDGroupeStation.Site.Altitude;
                }
                altitudeStationMin_m = calibrationData.GSIDGroupeStation.Site.Altitude + calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m;
                altitudeStationMax_m = calibrationData.GSIDGroupeStation.Site.Altitude + calibrationData.CalibrationParameters.ShiftAltitudeStationMax_m;
            }

            if (calibrationData.CalibrationParameters.AzimuthStation)
            {
                azimuthStationMin = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg + calibrationData.CalibrationParameters.ShiftAzimuthStationMin_deg;
                azimuthStationMax = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg + calibrationData.CalibrationParameters.ShiftAzimuthStationMax_deg;
            }

            if (calibrationData.CalibrationParameters.TiltStation)
            {
                tiltStationMin = calibrationData.CalibrationParameters.ShiftTiltStationMin_Deg;
                tiltStationMax = calibrationData.CalibrationParameters.ShiftTiltStationMax_Deg;
                
                if (tiltStationMin < -90) { tiltStationMax = -90; }
                if (tiltStationMax > 90) { tiltStationMax = 90; }
            }

            if (calibrationData.CalibrationParameters.PowerStation)
            {
                powerStationMax = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm + calibrationData.CalibrationParameters.ShiftPowerStationMax_dB;
                powerStationMin = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm + calibrationData.CalibrationParameters.ShiftPowerStationMin_dB;
            }
            //////////////////////////////////////
            
            bool altitudeDimCorrelation_pc = false;
            bool altitudeIncCorrelation_pc = false;

            bool tiltDimCorrelation_pc = false;
            bool tiltIncCorrelation_pc = false;

            bool azimuthDimCorrelation_pc = false;
            bool azimuthIncCorrelation_pc = false;

            bool xCoordDimCorrelation_pc = false;
            bool xCoordIncCorrelation_pc = false;

            bool yCoordDimCorrelation_pc = false;
            bool yCoordIncCorrelation_pc = false;

            bool powerDimCorrelation_pc = false;
            bool powerIncCorrelation_pc = false;

            bool correlationBecomesBetter = false;

            do
            {
                correlationBecomesBetter = false;
                altitudeDimCorrelation_pc = false;
                altitudeIncCorrelation_pc = false;

                tiltDimCorrelation_pc = false;
                tiltIncCorrelation_pc = false;
                azimuthDimCorrelation_pc = false;
                azimuthIncCorrelation_pc = false;

                xCoordDimCorrelation_pc = false;
                xCoordIncCorrelation_pc = false;
                yCoordDimCorrelation_pc = false;
                yCoordIncCorrelation_pc = false;

                powerDimCorrelation_pc = false;
                powerIncCorrelation_pc = false;

                if (calibrationData.CalibrationParameters.AltitudeStation)
                {
                    if (correlationData.GSIDGroupeStation.Site.Altitude - calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m >= altitudeStationMin_m)
                    {
                        correlationData.GSIDGroupeStation.Site.Altitude -= calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            //data.GSIDGroupeStation.Site.Altitude = correlation.GSIDGroupeStation.Site.Altitude;
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Site.Altitude += calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                    }

                    if (correlationData.GSIDGroupeStation.Site.Altitude + calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m <= altitudeStationMax_m)
                    {
                        correlationData.GSIDGroupeStation.Site.Altitude += calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Site.Altitude -= calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                    }
                       
                }
                if (calibrationData.CalibrationParameters.TiltStation)
                {
                    if (correlationData.GSIDGroupeStation.Antenna.Tilt_deg - calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg >= tiltStationMin)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg -= calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg += calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                    }
                        
                    if (correlationData.GSIDGroupeStation.Antenna.Tilt_deg + calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg <= tiltStationMax)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg += calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg -= calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                    }
                }

                if (calibrationData.CalibrationParameters.AzimuthStation)
                {
                    if (correlationData.GSIDGroupeStation.Antenna.Azimuth_deg - calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg >= azimuthStationMin)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg -= calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg += calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                    }
                        

                    if (correlationData.GSIDGroupeStation.Antenna.Azimuth_deg + calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg <= azimuthStationMax)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg += calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = false;
                            azimuthIncCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg -= calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                    }
                }

                if (calibrationData.CalibrationParameters.CoordinatesStation)
                {
                    if (calibrationData.GSIDGroupeStation.Coordinate.X - calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m >= coordinatesStationMinX_m)
                    {
                        calibrationData.GSIDGroupeStation.Coordinate.X -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = false;
                            azimuthIncCorrelation_pc = false;

                            xCoordDimCorrelation_pc = true;
                        }
                        calibrationData.GSIDGroupeStation.Coordinate.X += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }

                    if (calibrationData.GSIDGroupeStation.Coordinate.X + calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m <= coordinatesStationMaxX_m)
                    {
                        calibrationData.GSIDGroupeStation.Coordinate.X += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = false;
                            azimuthIncCorrelation_pc = false;

                            xCoordDimCorrelation_pc = false;
                            xCoordIncCorrelation_pc = true;
                        }
                        calibrationData.GSIDGroupeStation.Coordinate.X -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }
                    

                    if (calibrationData.GSIDGroupeStation.Coordinate.Y - calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m >= coordinatesStationMinY_m)
                    {
                        calibrationData.GSIDGroupeStation.Coordinate.Y -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = false;
                            azimuthIncCorrelation_pc = false;

                            xCoordDimCorrelation_pc = false;
                            xCoordIncCorrelation_pc = false;

                            yCoordDimCorrelation_pc = true;
                        }
                        calibrationData.GSIDGroupeStation.Coordinate.Y += calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                    }
                    
                    if (calibrationData.GSIDGroupeStation.Coordinate.Y + calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m <= coordinatesStationMaxY_m)
                    {
                        calibrationData.GSIDGroupeStation.Coordinate.Y += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = false;
                            azimuthIncCorrelation_pc = false;

                            xCoordDimCorrelation_pc = false;
                            xCoordIncCorrelation_pc = false;

                            yCoordDimCorrelation_pc = false;
                            yCoordIncCorrelation_pc = true;
                        }
                        calibrationData.GSIDGroupeStation.Coordinate.Y -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }
                }
                if (calibrationData.CalibrationParameters.PowerStation)
                {
                    if (correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm + calibrationData.CalibrationParameters.ShiftPowerStationStep_dB <= powerStationMax)
                    {
                        correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm += calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
                        //data.CalibrationParameters.ShiftPowerStationStep_dB < correlationResult.AvErr_dB
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = false;
                            azimuthIncCorrelation_pc = false;

                            xCoordDimCorrelation_pc = false;
                            xCoordIncCorrelation_pc = false;

                            yCoordDimCorrelation_pc = false;
                            yCoordIncCorrelation_pc = false;

                            powerIncCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm -= calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
                    }

                    if (correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm - calibrationData.CalibrationParameters.ShiftPowerStationStep_dB >= powerStationMin)
                    {
                        correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm -= calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
                        //data.CalibrationParameters.ShiftPowerStationStep_dB < correlationResult.AvErr_dB
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = false;
                            tiltIncCorrelation_pc = false;

                            azimuthDimCorrelation_pc = false;
                            azimuthIncCorrelation_pc = false;

                            xCoordDimCorrelation_pc = false;
                            xCoordIncCorrelation_pc = false;

                            yCoordDimCorrelation_pc = false;
                            yCoordIncCorrelation_pc = false;

                            powerIncCorrelation_pc = false;
                            powerDimCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm += calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
                    }
                }



                //===
                if (altitudeDimCorrelation_pc) { correlationData.GSIDGroupeStation.Site.Altitude -= calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m; }
                if (altitudeIncCorrelation_pc) { correlationData.GSIDGroupeStation.Site.Altitude += calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m; }

                if (tiltDimCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Tilt_deg -= calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg; }
                if (tiltIncCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Tilt_deg += calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg; }

                if (azimuthDimCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Azimuth_deg -= calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg; }
                if (azimuthIncCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Azimuth_deg += calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg; }

                if (xCoordDimCorrelation_pc) { correlationData.GSIDGroupeStation.Coordinate.X -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }
                if (xCoordIncCorrelation_pc) { correlationData.GSIDGroupeStation.Coordinate.X += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }

                if (yCoordDimCorrelation_pc) { correlationData.GSIDGroupeStation.Coordinate.Y -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }
                if (yCoordIncCorrelation_pc) { correlationData.GSIDGroupeStation.Coordinate.Y += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }

                if (powerIncCorrelation_pc) { correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm += calibrationData.CalibrationParameters.ShiftPowerStationStep_dB; }
                if (powerDimCorrelation_pc) { correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm -= calibrationData.CalibrationParameters.ShiftPowerStationStep_dB; }

                //if (altitudeDimCorrelation_pc || altitudeIncCorrelation_pc || tiltDimCorrelation_pc || tiltIncCorrelation_pc || 
                //    azimuthDimCorrelation_pc || azimuthIncCorrelation_pc || latDimCorrelation_pc || latIncCorrelation_pc || 
                //    lonDimCorrelation_pc || lonIncCorrelation_pc || powerChgCorrelation_pc )
                //{
                //    correlationResult = corellationCalcIteration.Run(taskContext, correlation);
                //}

            }
            while (correlationBecomesBetter);

            if (calibrationData.CalibrationParameters.AltitudeStation) { calibrationData.GSIDGroupeStation.Site.Altitude = correlationData.GSIDGroupeStation.Site.Altitude; }
            
            if (calibrationData.CalibrationParameters.AzimuthStation) { calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg = correlationData.GSIDGroupeStation.Antenna.Azimuth_deg; }

            if (calibrationData.CalibrationParameters.TiltStation) { calibrationData.GSIDGroupeStation.Antenna.Tilt_deg = correlationData.GSIDGroupeStation.Antenna.Tilt_deg; }
            
            if (calibrationData.CalibrationParameters.CoordinatesStation)
            {
                calibrationData.GSIDGroupeStation.Coordinate.X = correlationData.GSIDGroupeStation.Coordinate.X;
                calibrationData.GSIDGroupeStation.Coordinate.Y = correlationData.GSIDGroupeStation.Coordinate.Y;
            }

            if (calibrationData.CalibrationParameters.PowerStation) { calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm = correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm; }
        }

        public ResultCorrelationGSIDGroupeStations Run(ITaskContext taskContext, StationCalibrationCalcData data)
		{
            var calcCalibrationResult = new ResultCorrelationGSIDGroupeStations();

            var correlationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            var correlationData = new StationCorellationCalcData();
            var calibrationData = Atdi.Common.CopyHelper.CreateDeepCopy(data);

            //var corellationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            StationCorellationCalcIteration correlationCalcIteration = new StationCorellationCalcIteration(_iterationsPool, _transformation, _poolSite, _earthGeometricService, _logger);



            //correlationData.GSIDGroupeStation = Atdi.Common.CopyHelper.CreateDeepCopy(data.GSIDGroupeStation);
            correlationData.GSIDGroupeStation = calibrationData.GSIDGroupeStation;
            correlationData.GSIDGroupeDriveTests = calibrationData.GSIDGroupeDriveTests;
            correlationData.GeneralParameters = calibrationData.GeneralParameters;
            correlationData.FieldStrengthCalcData = calibrationData.FieldStrengthCalcData;
            correlationData.CorellationParameters = calibrationData.CorellationParameters;
            //// заполняем поля TargetCoordinate и TargetAltitude_m (координаты уже преобразованы в метры)
            //data.FieldStrengthCalcData.TargetCoordinate = data.GSIDGroupeDriveTests.Points[0].Coordinate;
            //data.FieldStrengthCalcData.TargetAltitude_m = data.GSIDGroupeDriveTests.Points[0].Height_m;

            //// вызываем механизм расчета FieldStrengthCalcData на основе переданных данных data.FieldStrengthCalcData
            //var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
            //var resultFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData);


            //- Parameters Station Old(Altitude Station, Tilt Station, Azimuth Station, Lat Station, Lon Station, Power Station)
            calcCalibrationResult.ParametersStationOld = new ParametersStation();
            calcCalibrationResult.ParametersStationOld.Altitude_m = (int)calibrationData.GSIDGroupeStation.Site.Altitude;//////////////!!!!!!!!!!!
            calcCalibrationResult.ParametersStationOld.Azimuth_deg = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg;
            calcCalibrationResult.ParametersStationOld.Tilt_Deg = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg;
            calcCalibrationResult.ParametersStationOld.Lat_deg = calibrationData.GSIDGroupeStation.Site.Latitude;
            calcCalibrationResult.ParametersStationOld.Lon_deg = calibrationData.GSIDGroupeStation.Site.Longitude;
            calcCalibrationResult.ParametersStationOld.Power_dB = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;
            calcCalibrationResult.ParametersStationOld.Freq_MHz = calibrationData.GSIDGroupeStation.Transmitter.Freq_MHz;

            //var siteCoords_m = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate() { Latitude = data.GSIDGroupeStation.Site.Latitude, Longitude = data.GSIDGroupeStation.Site.Longitude }, data.CodeProjection);
            
            if (calibrationData.CalibrationParameters.CascadeTuning)
            {
                float stationPowerMax = 0;
                if (calibrationData.CalibrationParameters.PowerStation)
                {
                    stationPowerMax = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm + calibrationData.CalibrationParameters.ShiftPowerStationMax_dB;
                }
                // первая итерация
                if (calibrationData.CalibrationParameters.Method is Method.ExhaustiveSearch)
                { ExhaustiveSearchStationCallibration(in taskContext, ref calibrationData,ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                else if (calibrationData.CalibrationParameters.Method is Method.QuickDescent)
                { QuickDescentStationCallibration(in taskContext, ref calibrationData,ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                // последующие итерации
                for (int i=1; i < calibrationData.CalibrationParameters.NumberCascade; i++)
                {
                    // преобразование шага и границ калибрации для поледующих итераций
                    if (calibrationData.CalibrationParameters.CoordinatesStation)
                    {
                        calibrationData.CalibrationParameters.ShiftCoordinatesStation_m = calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                        calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m /= calibrationData.CalibrationParameters.DetailOfCascade;
                    }

                    if (calibrationData.CalibrationParameters.AltitudeStation)
                    {
                        if (correlationData.GSIDGroupeStation.Site.Altitude > calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m)
                        {
                            calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m = -calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                        }
                        else
                        {
                            calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m = (int)-correlationData.GSIDGroupeStation.Site.Altitude;
                        }

                        calibrationData.CalibrationParameters.ShiftAltitudeStationMax_m = calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                        calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m /= calibrationData.CalibrationParameters.DetailOfCascade;
                    }

                    if (calibrationData.CalibrationParameters.AzimuthStation)
                    {
                        calibrationData.CalibrationParameters.ShiftAzimuthStationMin_deg = -calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                        calibrationData.CalibrationParameters.ShiftAzimuthStationMax_deg = calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                        calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg /= calibrationData.CalibrationParameters.DetailOfCascade;
                    }

                    if (calibrationData.CalibrationParameters.TiltStation)
                    {
                        calibrationData.CalibrationParameters.ShiftTiltStationMin_Deg = -calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                        calibrationData.CalibrationParameters.ShiftTiltStationMax_Deg = calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                        calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg /= calibrationData.CalibrationParameters.DetailOfCascade;
                    }
                    if (calibrationData.CalibrationParameters.PowerStation)
                    {
                        calibrationData.CalibrationParameters.ShiftPowerStationMax_dB = stationPowerMax - calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;
                    }

                    if (calibrationData.CalibrationParameters.Method is Method.ExhaustiveSearch)
                    { ExhaustiveSearchStationCallibration(in taskContext, ref calibrationData, ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                    else if (calibrationData.CalibrationParameters.Method is Method.QuickDescent)
                    { QuickDescentStationCallibration(in taskContext, ref calibrationData, ref correlationData, ref correlationResult, ref correlationCalcIteration); }
                }
            }
            else
            {
                if (calibrationData.CalibrationParameters.Method is Method.ExhaustiveSearch)
                { ExhaustiveSearchStationCallibration(in taskContext, ref calibrationData, ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                else if (calibrationData.CalibrationParameters.Method is Method.QuickDescent)
                { QuickDescentStationCallibration(in taskContext, ref calibrationData, ref correlationData, ref correlationResult, ref correlationCalcIteration); }
            }


            correlationData.GSIDGroupeStation = calibrationData.GSIDGroupeStation;
            correlationResult = correlationCalcIteration.Run(taskContext, correlationData);

            //- Parameters Station New(Altitude Station, Tilt Station, Azimuth Station, Lat Station, Lon Station, Power Station)
            
            var siteCoords_dec = _transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = calibrationData.GSIDGroupeStation.Coordinate.X, Y = calibrationData.GSIDGroupeStation.Coordinate.Y }, _transformation.ConvertProjectionToCode(calibrationData.CodeProjection));

            calcCalibrationResult.ParametersStationNew = new ParametersStation();
            calcCalibrationResult.ParametersStationNew.Altitude_m = (int)calibrationData.GSIDGroupeStation.Site.Altitude;//////////////!!!!!!!!!!!
            calcCalibrationResult.ParametersStationNew.Azimuth_deg = calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg;
            calcCalibrationResult.ParametersStationNew.Tilt_Deg = calibrationData.GSIDGroupeStation.Antenna.Tilt_deg;
            calcCalibrationResult.ParametersStationNew.Lat_deg = siteCoords_dec.Latitude;//data.GSIDGroupeStation.Site.Latitude;
            calcCalibrationResult.ParametersStationNew.Lon_deg = siteCoords_dec.Longitude;//data.GSIDGroupeStation.Site.Longitude;
            calcCalibrationResult.ParametersStationNew.Power_dB = calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;

            //- Freq_MHz(частота передатчика станции)
            calcCalibrationResult.Freq_MHz = correlationResult.Freq_MHz;
            //- Delta_dB(входной параметр)
            calcCalibrationResult.Delta_dB = correlationResult.Delta_dB;
            //- Correlation_pc(процент точек где результаты измерений отличаться от расчетного менее чем на Delta_dB)
            calcCalibrationResult.Corellation_pc = correlationResult.Corellation_pc;
            //- StdDev_dB
            calcCalibrationResult.StdDev_dB = correlationResult.StdDev_dB;
            //- AvErr_dB
            calcCalibrationResult.AvErr_dB = correlationResult.AvErr_dB;
            //- Correl factor(логарифмическая корреляция пирсона у нас реализована)
            calcCalibrationResult.Corellation_factor = correlationResult.Corellation_factor;
            //- CorreIPoints[](Lon_DEC, Lat_DEC, FSMeas_dBmkVm, FSCalc_dBmkVm, Dist_km) выдаётся только если Detail = true(нам для отладки не плохо было бы это хоть как визуализировать на карте)
            calcCalibrationResult.CorrellationPoints = correlationResult.CorrellationPoints;
            calcCalibrationResult.CountPoints = correlationResult.CountPoints;
            return calcCalibrationResult;
		}
	}
}
