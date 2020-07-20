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
        /// <param name="data"></param>
        /// <param name="correlationData"></param>
        /// <param name="correlationResult"></param>
        /// <param name="corellationCalcIteration"></param>
        private void ExhaustiveSearchStationCallibration(in ITaskContext taskContext, ref StationCalibrationCalcData calibrationData, ref StationCorellationCalcData correlationData, ref ResultCorrelationGSIDGroupeStationsWithoutParameters correlationResult, ref StationCorellationCalcIteration corellationCalcIteration)
        {
            correlationResult = corellationCalcIteration.Run(taskContext, correlationData);

            var data = Atdi.Common.CopyHelper.CreateDeepCopy(calibrationData);

            var maxCorrelation_pc = correlationResult.Corellation_pc;
            //Define ranges and steps
            int powerCalcIterations = 1;
            int coordinatesStationMinX_m = data.GSIDGroupeStation.Coordinate.X;
            int coordinatesStationMinY_m = data.GSIDGroupeStation.Coordinate.Y;
            int coordinatesStationStep_m = 1;
            int coordinatesStationMaxX_m = data.GSIDGroupeStation.Coordinate.X + coordinatesStationStep_m;
            int coordinatesStationMaxY_m = data.GSIDGroupeStation.Coordinate.Y + coordinatesStationStep_m;
            

            var altitudeStationMin_m = data.GSIDGroupeStation.Site.Altitude;
            var altitudeStationStep_m = 1.0f;
            var altitudeStationMax_m = data.GSIDGroupeStation.Site.Altitude + altitudeStationStep_m;

            var azimuthStationMin = data.GSIDGroupeStation.Antenna.Azimuth_deg;
            var azimuthStationStep = 1.0f;
            var azimuthStationMax = data.GSIDGroupeStation.Antenna.Azimuth_deg + azimuthStationStep;

            var tiltStationMin = data.GSIDGroupeStation.Antenna.Tilt_deg;
            var tiltStationStep = 1.0f;
            var tiltStationMax = data.GSIDGroupeStation.Antenna.Tilt_deg + tiltStationStep;

            var powerStationMax = data.GSIDGroupeStation.Transmitter.MaxPower_dBm;

            //координаты, высота, азимут, угол места, мощность
            if (data.CalibrationParameters.CoordinatesStation)
            {
                coordinatesStationMinX_m = data.GSIDGroupeStation.Coordinate.X + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinX_m < data.FieldStrengthCalcData.MapArea.LowerLeft.X) { coordinatesStationMinX_m = data.FieldStrengthCalcData.MapArea.LowerLeft.X; }
                coordinatesStationMinY_m = data.GSIDGroupeStation.Coordinate.Y + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinY_m < data.FieldStrengthCalcData.MapArea.LowerLeft.Y) { coordinatesStationMinY_m = data.FieldStrengthCalcData.MapArea.LowerLeft.Y; }
                coordinatesStationMaxX_m = data.GSIDGroupeStation.Coordinate.X + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxX_m < data.FieldStrengthCalcData.MapArea.UpperRight.X) { coordinatesStationMaxX_m = data.FieldStrengthCalcData.MapArea.UpperRight.X; }
                coordinatesStationMaxY_m = data.GSIDGroupeStation.Coordinate.Y + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxY_m < data.FieldStrengthCalcData.MapArea.UpperRight.Y) { coordinatesStationMaxY_m = data.FieldStrengthCalcData.MapArea.UpperRight.Y; }
                coordinatesStationStep_m = data.CalibrationParameters.ShiftCoordinatesStationStep_m;
            }
            
            if (data.CalibrationParameters.AltitudeStation)
            {
                if (correlationData.GSIDGroupeStation.Site.Altitude < -data.CalibrationParameters.ShiftAltitudeStationMin_m)
                {
                    data.CalibrationParameters.ShiftAltitudeStationMin_m = (int)-correlationData.GSIDGroupeStation.Site.Altitude;
                }
                altitudeStationMin_m = data.GSIDGroupeStation.Site.Altitude + data.CalibrationParameters.ShiftAltitudeStationMin_m;
                altitudeStationMax_m = data.GSIDGroupeStation.Site.Altitude + data.CalibrationParameters.ShiftAltitudeStationMax_m;
                altitudeStationStep_m = data.CalibrationParameters.ShiftAltitudeStationStep_m;
            }
            
            if (data.CalibrationParameters.AzimuthStation)
            {
                azimuthStationMin = data.GSIDGroupeStation.Antenna.Azimuth_deg + data.CalibrationParameters.ShiftAzimuthStationMin_deg;
                azimuthStationMax = data.GSIDGroupeStation.Antenna.Azimuth_deg + data.CalibrationParameters.ShiftAzimuthStationMax_deg;
                azimuthStationStep = data.CalibrationParameters.ShiftAzimuthStationStep_deg;
                if (azimuthStationMin < -360) { azimuthStationMin = -360; }
                if (azimuthStationMax > 360) { azimuthStationMax = 360; }
            }

            if (data.CalibrationParameters.TiltStation)
            {
                tiltStationMin = data.GSIDGroupeStation.Antenna.Tilt_deg + data.CalibrationParameters.ShiftTiltStationMin_Deg;
                tiltStationMax = data.GSIDGroupeStation.Antenna.Tilt_deg + data.CalibrationParameters.ShiftTiltStationMax_Deg;
                tiltStationStep = data.CalibrationParameters.ShiftTiltStationStep_Deg;
                if (tiltStationMin < -90) { tiltStationMax = -90; }
                if (tiltStationMax > 90) { tiltStationMax = 90; }
            }

            if (data.CalibrationParameters.PowerStation)
            {
                powerStationMax += data.CalibrationParameters.ShiftPowerStationMax_dB;//??
                if (data.CalibrationParameters.CascadeTuning)
                {
                    powerCalcIterations = data.CalibrationParameters.NumberCascade;
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
                                        data.GSIDGroupeStation.Site.Longitude = correlationData.GSIDGroupeStation.Site.Longitude;
                                        data.GSIDGroupeStation.Site.Latitude = correlationData.GSIDGroupeStation.Site.Latitude;
                                        data.GSIDGroupeStation.Site.Altitude = correlationData.GSIDGroupeStation.Site.Altitude;
                                        data.GSIDGroupeStation.Antenna.Azimuth_deg = correlationData.GSIDGroupeStation.Antenna.Azimuth_deg;
                                        data.GSIDGroupeStation.Antenna.Tilt_deg = correlationData.GSIDGroupeStation.Antenna.Tilt_deg;
                                        data.GSIDGroupeStation.Transmitter.MaxPower_dBm = correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm;
                                        maxCorrelation_pc = correlationResult.Corellation_pc;
                                    }

                                }
                            }
                        }
                    }
                }
            }

            calibrationData = data;
        }

        /// <summary>
        /// Quick Descent Station Callibration Method
        /// </summary>
        /// <param name="taskContext"></param>
        /// <param name="data"></param>
        /// <param name="correlationData"></param>
        /// <param name="correlationResult"></param>
        /// <param name="corellationCalcIteration"></param>
        private void QuickDescentStationCallibration(in ITaskContext taskContext, ref StationCalibrationCalcData data, ref StationCorellationCalcData correlationData, ref ResultCorrelationGSIDGroupeStationsWithoutParameters correlationResult, ref StationCorellationCalcIteration corellationCalcIteration)
        {
            //var corellationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            //StationCorellationCalcIteration corellationCalcIteration = new StationCorellationCalcIteration(_iterationsPool, _transformation, _poolSite, _logger);
            //var data = Atdi.Common.CopyHelper.CreateDeepCopy(calibrationData);

            correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
            //var currentCorrelation_pc = correlationResult.Corellation_pc;
            var maxCorrelation_pc = correlationResult.Corellation_pc;
            
            var coordinatesStationMinX_m = data.GSIDGroupeStation.Coordinate.X;
            var coordinatesStationMaxX_m = data.GSIDGroupeStation.Coordinate.Y;
            var coordinatesStationMinY_m = data.GSIDGroupeStation.Coordinate.X;
            var coordinatesStationMaxY_m = data.GSIDGroupeStation.Coordinate.Y;
            //////////////////////////////////////

            var altitudeStationMin_m = data.GSIDGroupeStation.Site.Altitude;
            var altitudeStationMax_m = data.GSIDGroupeStation.Site.Altitude;

            var azimuthStationMin = data.GSIDGroupeStation.Antenna.Azimuth_deg;
            var azimuthStationMax = data.GSIDGroupeStation.Antenna.Azimuth_deg;

            var tiltStationMin = data.GSIDGroupeStation.Antenna.Tilt_deg;
            var tiltStationMax = data.GSIDGroupeStation.Antenna.Tilt_deg;

            var powerStationMax = data.GSIDGroupeStation.Transmitter.MaxPower_dBm;

            //координаты, высота, азимут, угол места, мощность
            if (data.CalibrationParameters.CoordinatesStation)
            {
                coordinatesStationMinX_m = data.GSIDGroupeStation.Coordinate.X + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinX_m < data.FieldStrengthCalcData.MapArea.LowerLeft.X) { coordinatesStationMinX_m = data.FieldStrengthCalcData.MapArea.LowerLeft.X; }
                coordinatesStationMinY_m = data.GSIDGroupeStation.Coordinate.Y + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinY_m < data.FieldStrengthCalcData.MapArea.LowerLeft.Y) { coordinatesStationMinY_m = data.FieldStrengthCalcData.MapArea.LowerLeft.Y; }
                coordinatesStationMaxX_m = data.GSIDGroupeStation.Coordinate.X + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxX_m < data.FieldStrengthCalcData.MapArea.UpperRight.X) { coordinatesStationMaxX_m = data.FieldStrengthCalcData.MapArea.UpperRight.X; }
                coordinatesStationMaxY_m = data.GSIDGroupeStation.Coordinate.Y + data.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxY_m < data.FieldStrengthCalcData.MapArea.UpperRight.Y) { coordinatesStationMaxY_m = data.FieldStrengthCalcData.MapArea.UpperRight.Y; }
                
            }

            if (data.CalibrationParameters.AltitudeStation)
            {
                if (correlationData.GSIDGroupeStation.Site.Altitude < -data.CalibrationParameters.ShiftAltitudeStationMin_m)
                {
                    data.CalibrationParameters.ShiftAltitudeStationMin_m = (int)-correlationData.GSIDGroupeStation.Site.Altitude;
                }
                altitudeStationMin_m = data.GSIDGroupeStation.Site.Altitude + data.CalibrationParameters.ShiftAltitudeStationMin_m;
                altitudeStationMax_m = data.GSIDGroupeStation.Site.Altitude + data.CalibrationParameters.ShiftAltitudeStationMax_m;
            }

            if (data.CalibrationParameters.AzimuthStation)
            {
                azimuthStationMin = data.GSIDGroupeStation.Antenna.Azimuth_deg + data.CalibrationParameters.ShiftAzimuthStationMin_deg;
                azimuthStationMax = data.GSIDGroupeStation.Antenna.Azimuth_deg + data.CalibrationParameters.ShiftAzimuthStationMax_deg;
                if (azimuthStationMin < -360) { azimuthStationMin = -360; }
                if (azimuthStationMax > 360) { azimuthStationMax = 360; }
            }

            if (data.CalibrationParameters.TiltStation)
            {
                tiltStationMin = data.GSIDGroupeStation.Antenna.Tilt_deg + data.CalibrationParameters.ShiftTiltStationMin_Deg;
                tiltStationMax = data.GSIDGroupeStation.Antenna.Tilt_deg + data.CalibrationParameters.ShiftTiltStationMax_Deg;
                
                if (tiltStationMin < -90) { tiltStationMax = -90; }
                if (tiltStationMax > 90) { tiltStationMax = 90; }
            }

            if (data.CalibrationParameters.PowerStation)
            {
                powerStationMax += data.CalibrationParameters.ShiftPowerStationMax_dB;//??
            }
            //////////////////////////////////////
            
            bool altitudeDimCorrelation_pc = false;
            bool altitudeIncCorrelation_pc = false;

            bool tiltDimCorrelation_pc = false;
            bool tiltIncCorrelation_pc = false;

            bool azimuthDimCorrelation_pc = false;
            bool azimuthIncCorrelation_pc = false;

            bool latDimCorrelation_pc = false;
            bool latIncCorrelation_pc = false;

            bool lonDimCorrelation_pc = false;
            bool lonIncCorrelation_pc = false;

            bool powerChgCorrelation_pc = false;

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

                latDimCorrelation_pc = false;
                latIncCorrelation_pc = false;
                lonDimCorrelation_pc = false;
                lonIncCorrelation_pc = false;

                powerChgCorrelation_pc = false;

                if (data.CalibrationParameters.AltitudeStation)
                {
                    if (correlationData.GSIDGroupeStation.Site.Altitude - data.CalibrationParameters.ShiftAltitudeStationStep_m >= altitudeStationMin_m)
                    {
                        correlationData.GSIDGroupeStation.Site.Altitude -= data.CalibrationParameters.ShiftAltitudeStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            //data.GSIDGroupeStation.Site.Altitude = correlation.GSIDGroupeStation.Site.Altitude;
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Site.Altitude += data.CalibrationParameters.ShiftAltitudeStationStep_m;
                    }

                    if (correlationData.GSIDGroupeStation.Site.Altitude + data.CalibrationParameters.ShiftAltitudeStationStep_m <= altitudeStationMax_m)
                    {
                        correlationData.GSIDGroupeStation.Site.Altitude += data.CalibrationParameters.ShiftAltitudeStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Site.Altitude -= data.CalibrationParameters.ShiftAltitudeStationStep_m;
                    }
                       
                }
                if (data.CalibrationParameters.TiltStation)
                {
                    if (correlationData.GSIDGroupeStation.Antenna.Tilt_deg - data.CalibrationParameters.ShiftTiltStationStep_Deg >= tiltStationMin)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg -= data.CalibrationParameters.ShiftTiltStationStep_Deg;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = true;
                        }
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg += data.CalibrationParameters.ShiftTiltStationStep_Deg;
                    }
                        
                    if (correlationData.GSIDGroupeStation.Antenna.Tilt_deg + data.CalibrationParameters.ShiftTiltStationStep_Deg <= tiltStationMax)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg += data.CalibrationParameters.ShiftTiltStationStep_Deg;
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
                        correlationData.GSIDGroupeStation.Antenna.Tilt_deg -= data.CalibrationParameters.ShiftTiltStationStep_Deg;
                    }
                }

                if (data.CalibrationParameters.AzimuthStation)
                {
                    if (correlationData.GSIDGroupeStation.Antenna.Azimuth_deg - data.CalibrationParameters.ShiftAzimuthStationStep_deg >= azimuthStationMin)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg -= data.CalibrationParameters.ShiftAzimuthStationStep_deg;
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
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg += data.CalibrationParameters.ShiftAzimuthStationStep_deg;
                    }
                        

                    if (correlationData.GSIDGroupeStation.Antenna.Azimuth_deg + data.CalibrationParameters.ShiftAzimuthStationStep_deg <= azimuthStationMax)
                    {
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg += data.CalibrationParameters.ShiftAzimuthStationStep_deg;
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
                        correlationData.GSIDGroupeStation.Antenna.Azimuth_deg -= data.CalibrationParameters.ShiftAzimuthStationStep_deg;
                    }
                }

                if (data.CalibrationParameters.CoordinatesStation)
                {
                    if (data.GSIDGroupeStation.Coordinate.X - data.CalibrationParameters.ShiftCoordinatesStationStep_m >= coordinatesStationMinX_m)
                    {
                        data.GSIDGroupeStation.Coordinate.X -= data.CalibrationParameters.ShiftCoordinatesStationStep_m;
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

                            latDimCorrelation_pc = true;
                        }
                        data.GSIDGroupeStation.Coordinate.X += data.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }

                    if (data.GSIDGroupeStation.Coordinate.X + data.CalibrationParameters.ShiftCoordinatesStationStep_m <= coordinatesStationMaxX_m)
                    {
                        data.GSIDGroupeStation.Coordinate.X += data.CalibrationParameters.ShiftCoordinatesStationStep_m;
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

                            latDimCorrelation_pc = false;
                            latIncCorrelation_pc = true;
                        }
                        data.GSIDGroupeStation.Coordinate.X -= data.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }
                    

                    if (data.GSIDGroupeStation.Coordinate.X - data.CalibrationParameters.ShiftCoordinatesStationStep_m >= coordinatesStationMinX_m)
                    {
                        data.GSIDGroupeStation.Coordinate.Y -= data.CalibrationParameters.ShiftCoordinatesStationStep_m;
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

                            latDimCorrelation_pc = false;
                            latIncCorrelation_pc = false;

                            lonDimCorrelation_pc = true;
                        }
                        data.GSIDGroupeStation.Coordinate.Y += data.CalibrationParameters.MaxDeviationCoordinatesStation_m;
                    }
                    
                    if (data.GSIDGroupeStation.Coordinate.X + data.CalibrationParameters.ShiftCoordinatesStationStep_m <= coordinatesStationMaxX_m)
                    {
                        data.GSIDGroupeStation.Coordinate.Y += data.CalibrationParameters.MaxDeviationCoordinatesStation_m;
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

                            latDimCorrelation_pc = false;
                            latIncCorrelation_pc = false;

                            lonDimCorrelation_pc = false;
                            lonIncCorrelation_pc = true;
                        }
                        data.GSIDGroupeStation.Coordinate.Y -= data.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }
                }
                if (data.CalibrationParameters.PowerStation)
                {
                    correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm += correlationResult.AvErr_dB;
                    if (correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm > powerStationMax)
                    {
                        correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm = powerStationMax;
                    }

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

                        latDimCorrelation_pc = false;
                        latIncCorrelation_pc = false;

                        lonDimCorrelation_pc = false;
                        lonIncCorrelation_pc = false;

                        powerChgCorrelation_pc = true;
                    }
                    correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm -= correlationResult.AvErr_dB;
                }

                //===
                if (altitudeDimCorrelation_pc) { correlationData.GSIDGroupeStation.Site.Altitude -= data.CalibrationParameters.ShiftAltitudeStationStep_m; }
                if (altitudeIncCorrelation_pc) { correlationData.GSIDGroupeStation.Site.Altitude += data.CalibrationParameters.ShiftAltitudeStationStep_m; }

                if (tiltDimCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Tilt_deg -= data.CalibrationParameters.ShiftTiltStationStep_Deg; }
                if (tiltIncCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Tilt_deg += data.CalibrationParameters.ShiftTiltStationStep_Deg; }

                if (azimuthDimCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Azimuth_deg -= data.CalibrationParameters.ShiftAzimuthStationStep_deg; }
                if (azimuthIncCorrelation_pc) { correlationData.GSIDGroupeStation.Antenna.Azimuth_deg += data.CalibrationParameters.ShiftAzimuthStationStep_deg; }

                if (latDimCorrelation_pc) { data.GSIDGroupeStation.Coordinate.X -= data.CalibrationParameters.ShiftCoordinatesStationStep_m; }
                if (latIncCorrelation_pc) { data.GSIDGroupeStation.Coordinate.X += data.CalibrationParameters.ShiftCoordinatesStationStep_m; }

                if (lonDimCorrelation_pc) { data.GSIDGroupeStation.Coordinate.Y -= data.CalibrationParameters.ShiftCoordinatesStationStep_m; }
                if (lonIncCorrelation_pc) { data.GSIDGroupeStation.Coordinate.Y += data.CalibrationParameters.ShiftCoordinatesStationStep_m; }

                if (powerChgCorrelation_pc) { correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm += correlationResult.AvErr_dB; }

                //if (altitudeDimCorrelation_pc || altitudeIncCorrelation_pc || tiltDimCorrelation_pc || tiltIncCorrelation_pc || 
                //    azimuthDimCorrelation_pc || azimuthIncCorrelation_pc || latDimCorrelation_pc || latIncCorrelation_pc || 
                //    lonDimCorrelation_pc || lonIncCorrelation_pc || powerChgCorrelation_pc )
                //{
                //    correlationResult = corellationCalcIteration.Run(taskContext, correlation);
                //}

            }
            while (correlationBecomesBetter);

            if (data.CalibrationParameters.AltitudeStation) { data.GSIDGroupeStation.Site.Altitude = correlationData.GSIDGroupeStation.Site.Altitude; }
            
            if (data.CalibrationParameters.AzimuthStation) { data.GSIDGroupeStation.Antenna.Azimuth_deg = correlationData.GSIDGroupeStation.Antenna.Azimuth_deg; }

            if (data.CalibrationParameters.TiltStation) { data.GSIDGroupeStation.Antenna.Tilt_deg = correlationData.GSIDGroupeStation.Antenna.Tilt_deg; }
            
            //if (latDimCorrelation_pc) { data.GSIDGroupeStation.Coordinate.X -= data.CalibrationParameters.ShiftCoordinatesStationStep_m; }
            //if (latIncCorrelation_pc) { data.GSIDGroupeStation.Coordinate.X += data.CalibrationParameters.ShiftCoordinatesStationStep_m; }

            //if (lonDimCorrelation_pc) { data.GSIDGroupeStation.Coordinate.Y -= data.CalibrationParameters.ShiftCoordinatesStationStep_m; }
            //if (lonIncCorrelation_pc) { data.GSIDGroupeStation.Coordinate.Y += data.CalibrationParameters.ShiftCoordinatesStationStep_m; }

            if (data.CalibrationParameters.PowerStation) { data.GSIDGroupeStation.Transmitter.MaxPower_dBm = correlationData.GSIDGroupeStation.Transmitter.MaxPower_dBm; }
        }

        public ResultCorrelationGSIDGroupeStations Run(ITaskContext taskContext, StationCalibrationCalcData data)
		{
            var calcCalibrationResult = new ResultCorrelationGSIDGroupeStations();

            var correlationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            var correlationData = new StationCorellationCalcData();

            //var corellationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            StationCorellationCalcIteration correlationCalcIteration = new StationCorellationCalcIteration(_iterationsPool, _transformation, _poolSite, _earthGeometricService, _logger);



            //correlationData.GSIDGroupeStation = Atdi.Common.CopyHelper.CreateDeepCopy(data.GSIDGroupeStation);
            correlationData.GSIDGroupeStation = data.GSIDGroupeStation;
            correlationData.GSIDGroupeDriveTests = data.GSIDGroupeDriveTests;
            correlationData.GeneralParameters = data.GeneralParameters;
            correlationData.FieldStrengthCalcData = data.FieldStrengthCalcData;
            correlationData.CorellationParameters = data.CorellationParameters;
            //// заполняем поля TargetCoordinate и TargetAltitude_m (координаты уже преобразованы в метры)
            //data.FieldStrengthCalcData.TargetCoordinate = data.GSIDGroupeDriveTests.Points[0].Coordinate;
            //data.FieldStrengthCalcData.TargetAltitude_m = data.GSIDGroupeDriveTests.Points[0].Height_m;

            //// вызываем механизм расчета FieldStrengthCalcData на основе переданных данных data.FieldStrengthCalcData
            //var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
            //var resultFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData);


            //- Parameters Station Old(Altitude Station, Tilt Station, Azimuth Station, Lat Station, Lon Station, Power Station)
            calcCalibrationResult.ParametersStationOld = new ParametersStation();
            calcCalibrationResult.ParametersStationOld.Altitude_m = (int)data.GSIDGroupeStation.Site.Altitude;//////////////!!!!!!!!!!!
            calcCalibrationResult.ParametersStationOld.Azimuth_deg = data.GSIDGroupeStation.Antenna.Azimuth_deg;
            calcCalibrationResult.ParametersStationOld.Tilt_Deg = data.GSIDGroupeStation.Antenna.Tilt_deg;
            calcCalibrationResult.ParametersStationOld.Lat_deg = data.GSIDGroupeStation.Site.Latitude;
            calcCalibrationResult.ParametersStationOld.Lon_deg = data.GSIDGroupeStation.Site.Longitude;
            calcCalibrationResult.ParametersStationOld.Power_dB = data.GSIDGroupeStation.Transmitter.MaxPower_dBm;
            calcCalibrationResult.ParametersStationOld.Freq_MHz = data.GSIDGroupeStation.Transmitter.Freq_MHz;

            //var siteCoords_m = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate() { Latitude = data.GSIDGroupeStation.Site.Latitude, Longitude = data.GSIDGroupeStation.Site.Longitude }, data.CodeProjection);
            
            if (data.CalibrationParameters.CascadeTuning)
            {
                float stationPowerMax = 0;
                if (data.CalibrationParameters.PowerStation)
                {
                    stationPowerMax = data.GSIDGroupeStation.Transmitter.MaxPower_dBm + data.CalibrationParameters.ShiftPowerStationMax_dB;
                }
                // первая итерация
                if (data.CalibrationParameters.Method is Method.ExhaustiveSearch)
                { ExhaustiveSearchStationCallibration(in taskContext, ref data,ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                else if (data.CalibrationParameters.Method is Method.QuickDescent)
                { QuickDescentStationCallibration(in taskContext, ref data,ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                // последующие итерации
                for (int i=1; i < data.CalibrationParameters.NumberCascade; i++)
                {
                    // преобразование шага и границ калибрации для поледующих итераций
                    if (data.CalibrationParameters.CoordinatesStation)
                    {
                        data.CalibrationParameters.ShiftCoordinatesStation_m = data.CalibrationParameters.ShiftCoordinatesStationStep_m;
                        data.CalibrationParameters.ShiftCoordinatesStationStep_m /= data.CalibrationParameters.DetailOfCascade;
                    }

                    if (data.CalibrationParameters.AltitudeStation)
                    {
                        if (correlationData.GSIDGroupeStation.Site.Altitude > data.CalibrationParameters.ShiftAltitudeStationStep_m)
                        {
                            data.CalibrationParameters.ShiftAltitudeStationMin_m = -data.CalibrationParameters.ShiftAltitudeStationStep_m;
                        }
                        else
                        {
                            data.CalibrationParameters.ShiftAltitudeStationMin_m = (int)-correlationData.GSIDGroupeStation.Site.Altitude;
                        }

                        data.CalibrationParameters.ShiftAltitudeStationMax_m = data.CalibrationParameters.ShiftAltitudeStationStep_m;
                        data.CalibrationParameters.ShiftAltitudeStationStep_m /= data.CalibrationParameters.DetailOfCascade;
                    }

                    if (data.CalibrationParameters.AzimuthStation)
                    {
                        data.CalibrationParameters.ShiftAzimuthStationMin_deg = -data.CalibrationParameters.ShiftAzimuthStationStep_deg;
                        data.CalibrationParameters.ShiftAzimuthStationMax_deg = data.CalibrationParameters.ShiftAzimuthStationStep_deg;
                        data.CalibrationParameters.ShiftAzimuthStationStep_deg /= data.CalibrationParameters.DetailOfCascade;
                    }

                    if (data.CalibrationParameters.TiltStation)
                    {
                        data.CalibrationParameters.ShiftTiltStationMin_Deg = -data.CalibrationParameters.ShiftTiltStationStep_Deg;
                        data.CalibrationParameters.ShiftTiltStationMax_Deg = data.CalibrationParameters.ShiftTiltStationStep_Deg;
                        data.CalibrationParameters.ShiftTiltStationStep_Deg /= data.CalibrationParameters.DetailOfCascade;
                    }
                    if (data.CalibrationParameters.PowerStation)
                    {
                        data.CalibrationParameters.ShiftPowerStationMax_dB = stationPowerMax - data.GSIDGroupeStation.Transmitter.MaxPower_dBm;
                    }

                    if (data.CalibrationParameters.Method is Method.ExhaustiveSearch)
                    { ExhaustiveSearchStationCallibration(in taskContext, ref data, ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                    else if (data.CalibrationParameters.Method is Method.QuickDescent)
                    { QuickDescentStationCallibration(in taskContext, ref data, ref correlationData, ref correlationResult, ref correlationCalcIteration); }
                }
            }
            else
            {
                if (data.CalibrationParameters.Method is Method.ExhaustiveSearch)
                { ExhaustiveSearchStationCallibration(in taskContext, ref data, ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                else if (data.CalibrationParameters.Method is Method.QuickDescent)
                { QuickDescentStationCallibration(in taskContext, ref data, ref correlationData, ref correlationResult, ref correlationCalcIteration); }
            }


            correlationData.GSIDGroupeStation = data.GSIDGroupeStation;
            correlationResult = correlationCalcIteration.Run(taskContext, correlationData);

            //- Parameters Station New(Altitude Station, Tilt Station, Azimuth Station, Lat Station, Lon Station, Power Station)
            
            var siteCoords_dec = _transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = data.GSIDGroupeStation.Coordinate.X, Y = data.GSIDGroupeStation.Coordinate.Y }, _transformation.ConvertProjectionToCode(data.CodeProjection));

            calcCalibrationResult.ParametersStationNew = new ParametersStation();
            calcCalibrationResult.ParametersStationNew.Altitude_m = (int)data.GSIDGroupeStation.Site.Altitude;//////////////!!!!!!!!!!!
            calcCalibrationResult.ParametersStationNew.Azimuth_deg = data.GSIDGroupeStation.Antenna.Azimuth_deg;
            calcCalibrationResult.ParametersStationNew.Tilt_Deg = data.GSIDGroupeStation.Antenna.Tilt_deg;
            calcCalibrationResult.ParametersStationNew.Lat_deg = siteCoords_dec.Latitude;//data.GSIDGroupeStation.Site.Latitude;
            calcCalibrationResult.ParametersStationNew.Lon_deg = siteCoords_dec.Longitude;//data.GSIDGroupeStation.Site.Longitude;
            calcCalibrationResult.ParametersStationNew.Power_dB = data.GSIDGroupeStation.Transmitter.MaxPower_dBm;

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


            return calcCalibrationResult;
		}
	}
}
