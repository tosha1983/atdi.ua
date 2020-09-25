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
            var maxCorrelation_pc = correlationResult.Corellation_pc;


            ///////////////////////////// Установка границ за которые мы не выходим. 
            // Стартовая точка correlationData.FieldStrengthCalcData 
            // Диапазон calibrationData.CalibrationParameters
            // Координаты 
            int coordinatesStationMinX_m = correlationData.FieldStrengthCalcData.PointCoordinate.X;
            int coordinatesStationMaxX_m = correlationData.FieldStrengthCalcData.PointCoordinate.X;
            int coordinatesStationMinY_m = correlationData.FieldStrengthCalcData.PointCoordinate.Y;
            int coordinatesStationMaxY_m = correlationData.FieldStrengthCalcData.PointCoordinate.Y;
            int coordinatesStationStep_m = 1;
            if (calibrationData.CalibrationParameters.CoordinatesStation)
            {
                coordinatesStationMinX_m = coordinatesStationMinX_m - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinX_m < correlationData.FieldStrengthCalcData.MapArea.LowerLeft.X) { coordinatesStationMinX_m = correlationData.FieldStrengthCalcData.MapArea.LowerLeft.X; }
                coordinatesStationMinY_m = coordinatesStationMinY_m - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinY_m < correlationData.FieldStrengthCalcData.MapArea.LowerLeft.Y) { coordinatesStationMinY_m = correlationData.FieldStrengthCalcData.MapArea.LowerLeft.Y; }
                coordinatesStationMaxX_m = coordinatesStationMaxX_m + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxX_m > correlationData.FieldStrengthCalcData.MapArea.UpperRight.X) { coordinatesStationMaxX_m = correlationData.FieldStrengthCalcData.MapArea.UpperRight.X; }
                coordinatesStationMaxY_m = coordinatesStationMaxY_m + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxY_m > correlationData.FieldStrengthCalcData.MapArea.UpperRight.Y) { coordinatesStationMaxY_m = correlationData.FieldStrengthCalcData.MapArea.UpperRight.Y; }
                coordinatesStationStep_m = calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
            }
            //Высота
            double altitudeStationMin_m = correlationData.FieldStrengthCalcData.PointAltitude_m;
            double altitudeStationMax_m = correlationData.FieldStrengthCalcData.PointAltitude_m;
            var altitudeStationStep_m = 1.0f;
            if (calibrationData.CalibrationParameters.AltitudeStation)
            {
                altitudeStationMin_m = altitudeStationMin_m + calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m;
                if (altitudeStationMin_m < 3) { altitudeStationMin_m = 3; }
                altitudeStationMax_m = altitudeStationMax_m + calibrationData.CalibrationParameters.ShiftAltitudeStationMax_m;
                altitudeStationStep_m = calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
            }
            // Азимут
            float azimuthStationMin = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg;
            float azimuthStationMax = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg;
            var azimuthStationStep = 1.0f;
            if (calibrationData.CalibrationParameters.AzimuthStation)
            {
                azimuthStationMin = azimuthStationMin + calibrationData.CalibrationParameters.ShiftAzimuthStationMin_deg;
                azimuthStationMax = azimuthStationMax + calibrationData.CalibrationParameters.ShiftAzimuthStationMax_deg;
                azimuthStationStep = calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
            }
            //Угол места
            float tiltStationMin = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg;
            float tiltStationMax = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg;
            var tiltStationStep = 1.0f;
            if (calibrationData.CalibrationParameters.TiltStation)
            {
                tiltStationMin = tiltStationMin + calibrationData.CalibrationParameters.ShiftTiltStationMin_Deg;
                tiltStationMax = tiltStationMax + calibrationData.CalibrationParameters.ShiftTiltStationMax_Deg;
                tiltStationStep = calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                if (tiltStationMin < -90) { tiltStationMax = -90; }
                if (tiltStationMax > 90) { tiltStationMax = 90; }
            }
            // Мощность
            float powerStationMax = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm;
            float powerStationMin = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm;
            float powerStationStep = 1.0f;
            if (calibrationData.CalibrationParameters.PowerStation)
            {
                powerStationMax = powerStationMax + calibrationData.CalibrationParameters.ShiftPowerStationMax_dB;
                powerStationMin = powerStationMin + calibrationData.CalibrationParameters.ShiftPowerStationMin_dB;
                powerStationStep = calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
            }
            ////////////////////////////////////// Границы установленны

            //координаты, высота, азимут, угол места, мощность
            var optAz = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg;
            var optTilt = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg;
            var optX = correlationData.FieldStrengthCalcData.PointCoordinate.X;
            var optY = correlationData.FieldStrengthCalcData.PointCoordinate.Y;
            var optAlt = correlationData.FieldStrengthCalcData.PointAltitude_m;
            var optPow = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm;
            for (var coordinatesStationX = coordinatesStationMinX_m; coordinatesStationX <= coordinatesStationMaxX_m; coordinatesStationX += coordinatesStationStep_m)
            {
                correlationData.FieldStrengthCalcData.PointCoordinate.X = coordinatesStationX;

                for (var coordinatesStationY = coordinatesStationMinY_m; coordinatesStationY <= coordinatesStationMaxY_m; coordinatesStationY += coordinatesStationStep_m)
                {
                    correlationData.FieldStrengthCalcData.PointCoordinate.Y = coordinatesStationY;

                    // высота, азимут, угол места, мощность
                    for (var altitudeStation = altitudeStationMin_m; altitudeStation <= altitudeStationMax_m; altitudeStation += altitudeStationStep_m)
                    {
                        correlationData.FieldStrengthCalcData.PointAltitude_m = altitudeStation;

                        // азимут, угол места, мощность
                        for (var azimuthStation = azimuthStationMin; azimuthStation <= azimuthStationMax; azimuthStation += azimuthStationStep)
                        {
                            correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg = azimuthStation;

                            // угол места, мощность
                            for (var tiltStation = tiltStationMin; tiltStation <= tiltStationMax; tiltStation += tiltStationStep)
                            {
                                correlationData.FieldStrengthCalcData.Antenna.Tilt_deg = tiltStation;
                                // мощность
                                for (var powerStation = powerStationMin; powerStation <= powerStationMax; powerStation += powerStationStep)
                                {
                                    correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm = powerStation;
                                    correlationResult = corellationCalcIteration.Run(taskContext, correlationData);

                                    if (maxCorrelation_pc < correlationResult.Corellation_pc)
                                    {
                                        optAz = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg;
                                        optTilt = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg;
                                        optX = correlationData.FieldStrengthCalcData.PointCoordinate.X;
                                        optY = correlationData.FieldStrengthCalcData.PointCoordinate.Y;
                                        optAlt = correlationData.FieldStrengthCalcData.PointAltitude_m;
                                        optPow = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm;
                                        maxCorrelation_pc = correlationResult.Corellation_pc;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            // устанавливаем оптимальные координаты
            correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg = optAz;
            correlationData.FieldStrengthCalcData.Antenna.Tilt_deg = optTilt;
            correlationData.FieldStrengthCalcData.PointCoordinate.X = optX;
            correlationData.FieldStrengthCalcData.PointCoordinate.Y = optY;
            correlationData.FieldStrengthCalcData.PointAltitude_m = optAlt;
            correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm = optPow;

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

            // расчет уровня корреляции начальной точки correlationData.FieldStrengthCalcData
            correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
            var maxCorrelation_pc = correlationResult.Corellation_pc;

            ///////////////////////////// Установка границ за которые мы не выходим. 
            // Стартовая точка correlationData.FieldStrengthCalcData 
            // Диапазон calibrationData.CalibrationParameters
            // Координаты 
            int coordinatesStationMinX_m = correlationData.FieldStrengthCalcData.PointCoordinate.X;
            int coordinatesStationMaxX_m = correlationData.FieldStrengthCalcData.PointCoordinate.X;
            int coordinatesStationMinY_m = correlationData.FieldStrengthCalcData.PointCoordinate.Y;
            int coordinatesStationMaxY_m = correlationData.FieldStrengthCalcData.PointCoordinate.Y;
            if (calibrationData.CalibrationParameters.CoordinatesStation)
            {
                coordinatesStationMinX_m = coordinatesStationMinX_m - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinX_m < correlationData.FieldStrengthCalcData.MapArea.LowerLeft.X) { coordinatesStationMinX_m = correlationData.FieldStrengthCalcData.MapArea.LowerLeft.X; }
                coordinatesStationMinY_m = coordinatesStationMinY_m - calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMinY_m < correlationData.FieldStrengthCalcData.MapArea.LowerLeft.Y) { coordinatesStationMinY_m = correlationData.FieldStrengthCalcData.MapArea.LowerLeft.Y; }
                coordinatesStationMaxX_m = coordinatesStationMaxX_m + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxX_m > correlationData.FieldStrengthCalcData.MapArea.UpperRight.X) { coordinatesStationMaxX_m = correlationData.FieldStrengthCalcData.MapArea.UpperRight.X; }
                coordinatesStationMaxY_m = coordinatesStationMaxY_m + calibrationData.CalibrationParameters.ShiftCoordinatesStation_m;
                if (coordinatesStationMaxY_m > correlationData.FieldStrengthCalcData.MapArea.UpperRight.Y) { coordinatesStationMaxY_m = correlationData.FieldStrengthCalcData.MapArea.UpperRight.Y; }
            }
            //Высота
            double altitudeStationMin_m = correlationData.FieldStrengthCalcData.PointAltitude_m;
            double altitudeStationMax_m = correlationData.FieldStrengthCalcData.PointAltitude_m;
            if (calibrationData.CalibrationParameters.AltitudeStation)
            {
                altitudeStationMin_m = altitudeStationMin_m + calibrationData.CalibrationParameters.ShiftAltitudeStationMin_m;
                if (altitudeStationMin_m < 3) { altitudeStationMin_m = 3; }
                altitudeStationMax_m = altitudeStationMax_m + calibrationData.CalibrationParameters.ShiftAltitudeStationMax_m;
            }
            // Азимут
            float azimuthStationMin = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg;
            float azimuthStationMax = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg;
            if (calibrationData.CalibrationParameters.AzimuthStation)
            {
                azimuthStationMin = azimuthStationMin + calibrationData.CalibrationParameters.ShiftAzimuthStationMin_deg;
                azimuthStationMax = azimuthStationMax + calibrationData.CalibrationParameters.ShiftAzimuthStationMax_deg;
            }
            //Угол места
            float tiltStationMin = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg;
            float tiltStationMax = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg;
            if (calibrationData.CalibrationParameters.TiltStation)
            {
                tiltStationMin = tiltStationMin + calibrationData.CalibrationParameters.ShiftTiltStationMin_Deg;
                tiltStationMax = tiltStationMax + calibrationData.CalibrationParameters.ShiftTiltStationMax_Deg;
                if (tiltStationMin < -90) { tiltStationMax = -90; }
                if (tiltStationMax > 90) { tiltStationMax = 90; }
            }
            // Мощность
            float powerStationMax = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm;
            float powerStationMin = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm;
            if (calibrationData.CalibrationParameters.PowerStation)
            {
                powerStationMax = powerStationMax + calibrationData.CalibrationParameters.ShiftPowerStationMax_dB;
                powerStationMin = powerStationMin + calibrationData.CalibrationParameters.ShiftPowerStationMin_dB;
            }
            ////////////////////////////////////// Границы установленны


            //// Подбор параметров
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
                    if (correlationData.FieldStrengthCalcData.PointAltitude_m - calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m >= altitudeStationMin_m)
                    {
                        correlationData.FieldStrengthCalcData.PointAltitude_m -= calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            //data.GSIDGroupeStation.Site.Altitude = correlation.GSIDGroupeStation.Site.Altitude;
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = true;
                        }
                        correlationData.FieldStrengthCalcData.PointAltitude_m += calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                    }

                    if (correlationData.FieldStrengthCalcData.PointAltitude_m + calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m <= altitudeStationMax_m)
                    {
                        correlationData.FieldStrengthCalcData.PointAltitude_m += calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = true;
                        }
                        correlationData.FieldStrengthCalcData.PointAltitude_m -= calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m;
                    }

                }
                if (calibrationData.CalibrationParameters.TiltStation)
                {
                    if (correlationData.FieldStrengthCalcData.Antenna.Tilt_deg - calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg >= tiltStationMin)
                    {
                        correlationData.FieldStrengthCalcData.Antenna.Tilt_deg -= calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                        correlationResult = corellationCalcIteration.Run(taskContext, correlationData);
                        if (maxCorrelation_pc < correlationResult.Corellation_pc)
                        {
                            maxCorrelation_pc = correlationResult.Corellation_pc;
                            correlationBecomesBetter = true;

                            altitudeDimCorrelation_pc = false;
                            altitudeIncCorrelation_pc = false;

                            tiltDimCorrelation_pc = true;
                        }
                        correlationData.FieldStrengthCalcData.Antenna.Tilt_deg += calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                    }

                    if (correlationData.FieldStrengthCalcData.Antenna.Tilt_deg + calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg <= tiltStationMax)
                    {
                        correlationData.FieldStrengthCalcData.Antenna.Tilt_deg += calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
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
                        correlationData.FieldStrengthCalcData.Antenna.Tilt_deg -= calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg;
                    }
                }

                if (calibrationData.CalibrationParameters.AzimuthStation)
                {
                    if (correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg - calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg >= azimuthStationMin)
                    {
                        correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg -= calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
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
                        correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg += calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                    }


                    if (correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg + calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg <= azimuthStationMax)
                    {
                        correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg += calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
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
                        correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg -= calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg;
                    }
                }

                if (calibrationData.CalibrationParameters.CoordinatesStation)
                {
                    if (correlationData.FieldStrengthCalcData.PointCoordinate.X - calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m >= coordinatesStationMinX_m)
                    {
                        correlationData.FieldStrengthCalcData.PointCoordinate.X -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
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
                        correlationData.FieldStrengthCalcData.PointCoordinate.X += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }

                    if (calibrationData.FieldStrengthCalcData.PointCoordinate.X + calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m <= coordinatesStationMaxX_m)
                    {
                        correlationData.FieldStrengthCalcData.PointCoordinate.X += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
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
                        correlationData.FieldStrengthCalcData.PointCoordinate.X -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }


                    if (correlationData.FieldStrengthCalcData.PointCoordinate.Y - calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m >= coordinatesStationMinY_m)
                    {
                        correlationData.FieldStrengthCalcData.PointCoordinate.Y -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
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
                        correlationData.FieldStrengthCalcData.PointCoordinate.Y += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }

                    if (correlationData.FieldStrengthCalcData.PointCoordinate.Y + calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m <= coordinatesStationMaxY_m)
                    {
                        correlationData.FieldStrengthCalcData.PointCoordinate.Y += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
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
                        correlationData.FieldStrengthCalcData.PointCoordinate.Y -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m;
                    }
                }
                if (calibrationData.CalibrationParameters.PowerStation)
                {
                    if (correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm + calibrationData.CalibrationParameters.ShiftPowerStationStep_dB <= powerStationMax)
                    {
                        correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm += calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
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
                        correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm -= calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
                    }

                    if (correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm - calibrationData.CalibrationParameters.ShiftPowerStationStep_dB >= powerStationMin)
                    {
                        correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm -= calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
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
                        correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm += calibrationData.CalibrationParameters.ShiftPowerStationStep_dB;
                    }
                }
                //===
                if (altitudeDimCorrelation_pc) { correlationData.FieldStrengthCalcData.PointAltitude_m -= calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m; }
                if (altitudeIncCorrelation_pc) { correlationData.FieldStrengthCalcData.PointAltitude_m += calibrationData.CalibrationParameters.ShiftAltitudeStationStep_m; }

                if (tiltDimCorrelation_pc) { correlationData.FieldStrengthCalcData.Antenna.Tilt_deg -= calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg; }
                if (tiltIncCorrelation_pc) { correlationData.FieldStrengthCalcData.Antenna.Tilt_deg += calibrationData.CalibrationParameters.ShiftTiltStationStep_Deg; }

                if (azimuthDimCorrelation_pc) { correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg -= calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg; }
                if (azimuthIncCorrelation_pc) { correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg += calibrationData.CalibrationParameters.ShiftAzimuthStationStep_deg; }

                if (xCoordDimCorrelation_pc) { correlationData.FieldStrengthCalcData.PointCoordinate.X -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }
                if (xCoordIncCorrelation_pc) { correlationData.FieldStrengthCalcData.PointCoordinate.X += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }

                if (yCoordDimCorrelation_pc) { correlationData.FieldStrengthCalcData.PointCoordinate.Y -= calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }
                if (yCoordIncCorrelation_pc) { correlationData.FieldStrengthCalcData.PointCoordinate.Y += calibrationData.CalibrationParameters.ShiftCoordinatesStationStep_m; }

                if (powerIncCorrelation_pc) { correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm += calibrationData.CalibrationParameters.ShiftPowerStationStep_dB; }
                if (powerDimCorrelation_pc) { correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm -= calibrationData.CalibrationParameters.ShiftPowerStationStep_dB; }

                //if (altitudeDimCorrelation_pc || altitudeIncCorrelation_pc || tiltDimCorrelation_pc || tiltIncCorrelation_pc || 
                //    azimuthDimCorrelation_pc || azimuthIncCorrelation_pc || latDimCorrelation_pc || latIncCorrelation_pc || 
                //    lonDimCorrelation_pc || lonIncCorrelation_pc || powerChgCorrelation_pc )
                //{
                //    correlationResult = corellationCalcIteration.Run(taskContext, correlation);
                //}

            }
            while (correlationBecomesBetter);

            //if (calibrationData.CalibrationParameters.AltitudeStation) { calibrationData.GSIDGroupeStation.Site.Altitude = correlationData.FieldStrengthCalcData.PointAltitude_m; }

            //if (calibrationData.CalibrationParameters.AzimuthStation) { calibrationData.GSIDGroupeStation.Antenna.Azimuth_deg = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg; }

            //if (calibrationData.CalibrationParameters.TiltStation) { calibrationData.GSIDGroupeStation.Antenna.Tilt_deg = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg; }

            //if (calibrationData.CalibrationParameters.CoordinatesStation)
            //{
            //    calibrationData.GSIDGroupeStation.Coordinate.X = correlationData.FieldStrengthCalcData.PointCoordinate.X;
            //    calibrationData.GSIDGroupeStation.Coordinate.Y = correlationData.FieldStrengthCalcData.PointCoordinate.Y;
            //}

            //if (calibrationData.CalibrationParameters.PowerStation) { calibrationData.GSIDGroupeStation.Transmitter.MaxPower_dBm = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm; }
        }

        public ResultCorrelationGSIDGroupeStations Run(ITaskContext taskContext, StationCalibrationCalcData data)
        {
            var calcCalibrationResult = new ResultCorrelationGSIDGroupeStations();

            var correlationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            var correlationData = new StationCorellationCalcData();
            //var calibrationData = Atdi.Common.CopyHelper.CreateDeepCopy(data);
            var calibrationData = new StationCalibrationCalcData()
            {
                FieldStrengthCalcData = new FieldStrengthCalcData()
                {
                    BuildingContent = data.FieldStrengthCalcData.BuildingContent,
                    ClutterContent = data.FieldStrengthCalcData.ClutterContent,
                    CluttersDesc = data.FieldStrengthCalcData.CluttersDesc,
                    MapArea = data.FieldStrengthCalcData.MapArea,
                    ReliefContent = data.FieldStrengthCalcData.ReliefContent,
                    Antenna = Atdi.Common.CopyHelper.CreateDeepCopy(data.FieldStrengthCalcData.Antenna),
                    PointAltitude_m = Atdi.Common.CopyHelper.CreateDeepCopy(data.FieldStrengthCalcData.PointAltitude_m),
                    PointCoordinate = Atdi.Common.CopyHelper.CreateDeepCopy(data.FieldStrengthCalcData.PointCoordinate),
                    PropagationModel = Atdi.Common.CopyHelper.CreateDeepCopy(data.FieldStrengthCalcData.PropagationModel),
                    TargetAltitude_m = Atdi.Common.CopyHelper.CreateDeepCopy(data.FieldStrengthCalcData.TargetAltitude_m),
                    TargetCoordinate = Atdi.Common.CopyHelper.CreateDeepCopy(data.FieldStrengthCalcData.TargetCoordinate),
                    Transmitter = Atdi.Common.CopyHelper.CreateDeepCopy(data.FieldStrengthCalcData.Transmitter)
                },

                CalibrationParameters = Atdi.Common.CopyHelper.CreateDeepCopy(data.CalibrationParameters),
                CodeProjection = data.CodeProjection,
                CorellationParameters = Atdi.Common.CopyHelper.CreateDeepCopy(data.CorellationParameters),
                GeneralParameters = Atdi.Common.CopyHelper.CreateDeepCopy(data.GeneralParameters),
                GSIDGroupeDriveTests = Atdi.Common.CopyHelper.CreateDeepCopy(data.GSIDGroupeDriveTests),
                GSIDGroupeStation = Atdi.Common.CopyHelper.CreateDeepCopy(data.GSIDGroupeStation)
            };

            //var corellationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            StationCorellationCalcIteration correlationCalcIteration = new StationCorellationCalcIteration(_iterationsPool, _transformation, _poolSite, _earthGeometricService, _logger);

            //correlationData.GSIDGroupeStation = Atdi.Common.CopyHelper.CreateDeepCopy(data.GSIDGroupeStation);
            //correlationData.GSIDGroupeStation = calibrationData.GSIDGroupeStation;
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

            correlationResult = correlationCalcIteration.Run(taskContext, correlationData);
            var initialCorrelation = correlationResult.Corellation_pc;
            //var siteCoords_m = _transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate() { Latitude = data.GSIDGroupeStation.Site.Latitude, Longitude = data.GSIDGroupeStation.Site.Longitude }, data.CodeProjection);

            if (calibrationData.CalibrationParameters.CascadeTuning)
            {
                float stationPowerMax = 0;
                // первая итерация
                if (calibrationData.CalibrationParameters.Method is Method.ExhaustiveSearch)
                { ExhaustiveSearchStationCallibration(in taskContext, ref calibrationData, ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                else if (calibrationData.CalibrationParameters.Method is Method.QuickDescent)
                { QuickDescentStationCallibration(in taskContext, ref calibrationData, ref correlationData, ref correlationResult, ref correlationCalcIteration); }

                // последующие итерации
                for (int i = 1; i < calibrationData.CalibrationParameters.NumberCascade; i++)
                {
                    // преобразование шага и границ калибрации для поледующих итераций
                    UpdateLimith( correlationData.FieldStrengthCalcData, ref calibrationData.CalibrationParameters, calibrationData.GSIDGroupeStation);
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


            //correlationData.GSIDGroupeStation = calibrationData.GSIDGroupeStation;
            correlationResult = correlationCalcIteration.Run(taskContext, correlationData);

            //var deltaCorrelation_pc = correlationResult.Corellation_pc - initialCorrelation;
            //- Parameters Station New(Altitude Station, Tilt Station, Azimuth Station, Lat Station, Lon Station, Power Station)

            var siteCoords_dec = _transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = correlationData.FieldStrengthCalcData.PointCoordinate.X, Y = correlationData.FieldStrengthCalcData.PointCoordinate.Y }, _transformation.ConvertProjectionToCode(calibrationData.CodeProjection));
            calcCalibrationResult.ParametersStationNew = new ParametersStation();
            calcCalibrationResult.ParametersStationNew.Altitude_m = (int)correlationData.FieldStrengthCalcData.PointAltitude_m;//////////////!!!!!!!!!!!
            calcCalibrationResult.ParametersStationNew.Azimuth_deg = correlationData.FieldStrengthCalcData.Antenna.Azimuth_deg;
            calcCalibrationResult.ParametersStationNew.Tilt_Deg = correlationData.FieldStrengthCalcData.Antenna.Tilt_deg;
            calcCalibrationResult.ParametersStationNew.Lat_deg = siteCoords_dec.Latitude;//data.GSIDGroupeStation.Site.Latitude;
            calcCalibrationResult.ParametersStationNew.Lon_deg = siteCoords_dec.Longitude;//data.GSIDGroupeStation.Site.Longitude;
            calcCalibrationResult.ParametersStationNew.Power_dB = correlationData.FieldStrengthCalcData.Transmitter.MaxPower_dBm;

            //- Freq_MHz(частота передатчика станции)
            //calcCalibrationResult.Freq_MHz = correlationResult.Freq_MHz;
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
        private void UpdateLimith(FieldStrengthCalcData curentDate, ref CalibrationParameters CalibrationParameters, ContextStation EtalonStation)
        {
            // изменение координат будет без проверки границ.
            if (CalibrationParameters.CoordinatesStation)
            {
               CalibrationParameters.ShiftCoordinatesStation_m = CalibrationParameters.ShiftCoordinatesStationStep_m;
               CalibrationParameters.ShiftCoordinatesStationStep_m = CalibrationParameters.ShiftCoordinatesStationStep_m/CalibrationParameters.DetailOfCascade;
            }
            // Изменение высоты станции
            if (CalibrationParameters.AltitudeStation)
            {
                var NewShift = CalibrationParameters.ShiftAltitudeStationStep_m;
                var LimithMin = EtalonStation.Site.Altitude + CalibrationParameters.ShiftAltitudeStationMin_m;
                var LimithMax = EtalonStation.Site.Altitude + CalibrationParameters.ShiftAltitudeStationMax_m;
                var CurrentMin = curentDate.PointAltitude_m - NewShift;
                var CurrentMax = curentDate.PointAltitude_m + NewShift;
                if (LimithMin <= CurrentMin){ CalibrationParameters.ShiftAltitudeStationMin_m = -NewShift;}
                else {CalibrationParameters.ShiftAltitudeStationMin_m = (int)(LimithMin- curentDate.Antenna.Azimuth_deg);}
                if (LimithMax >= CurrentMax) { CalibrationParameters.ShiftAltitudeStationMax_m = NewShift; }
                else {CalibrationParameters.ShiftAltitudeStationMax_m = (int)(LimithMax - curentDate.PointAltitude_m);}
                CalibrationParameters.ShiftAltitudeStationStep_m = CalibrationParameters.ShiftAltitudeStationStep_m/CalibrationParameters.DetailOfCascade;
            }
            // Изменение азимута станции
            if (CalibrationParameters.AzimuthStation)
            {
                var NewShift = CalibrationParameters.ShiftAzimuthStationStep_deg;
                var LimithMin = EtalonStation.Antenna.Azimuth_deg + CalibrationParameters.ShiftAzimuthStationMin_deg;
                var LimithMax = EtalonStation.Antenna.Azimuth_deg + CalibrationParameters.ShiftAzimuthStationMax_deg;
                var CurrentMin = curentDate.Antenna.Azimuth_deg - NewShift;
                var CurrentMax = curentDate.Antenna.Azimuth_deg + NewShift;
                if (LimithMin <= CurrentMin) { CalibrationParameters.ShiftAzimuthStationMin_deg = - NewShift; }
                else { CalibrationParameters.ShiftAzimuthStationMin_deg = (int)(LimithMin - curentDate.Antenna.Azimuth_deg); }
                if (LimithMax >= CurrentMax) { CalibrationParameters.ShiftAzimuthStationMax_deg = NewShift; }
                else { CalibrationParameters.ShiftAzimuthStationMax_deg = (int)(LimithMax - curentDate.Antenna.Azimuth_deg); }
                CalibrationParameters.ShiftAzimuthStationStep_deg = CalibrationParameters.ShiftAzimuthStationStep_deg / CalibrationParameters.DetailOfCascade;
            }
            // Изменение тилта станции

            if (CalibrationParameters.TiltStation)
            {
                var NewShift = CalibrationParameters.ShiftTiltStationStep_Deg;
                var LimithMin = EtalonStation.Antenna.Tilt_deg + CalibrationParameters.ShiftTiltStationMin_Deg;
                var LimithMax = EtalonStation.Antenna.Tilt_deg + CalibrationParameters.ShiftTiltStationMax_Deg;
                var CurrentMin = curentDate.Antenna.Tilt_deg - NewShift;
                var CurrentMax = curentDate.Antenna.Tilt_deg + NewShift;
                if (LimithMin <= CurrentMin) { CalibrationParameters.ShiftTiltStationMin_Deg = -NewShift; }
                else { CalibrationParameters.ShiftTiltStationMin_Deg = (int)(LimithMin - curentDate.Antenna.Tilt_deg); }
                if (LimithMax >= CurrentMax) { CalibrationParameters.ShiftTiltStationMax_Deg = NewShift;}
                else { CalibrationParameters.ShiftTiltStationMax_Deg = (int)(LimithMax - curentDate.Antenna.Tilt_deg); }
                CalibrationParameters.ShiftTiltStationStep_Deg = CalibrationParameters.ShiftTiltStationStep_Deg/CalibrationParameters.DetailOfCascade;
            }
            // Изменение мощности станции
            if (CalibrationParameters.PowerStation)
            {
                var NewShift = CalibrationParameters.ShiftPowerStationStep_dB;
                var LimithMin = EtalonStation.Transmitter.MaxPower_dBm + CalibrationParameters.ShiftPowerStationMin_dB;
                var LimithMax = EtalonStation.Transmitter.MaxPower_dBm + CalibrationParameters.ShiftPowerStationMax_dB;
                var CurrentMin = curentDate.Transmitter.MaxPower_dBm - NewShift;
                var CurrentMax = curentDate.Transmitter.MaxPower_dBm + NewShift;
                if (LimithMin <= CurrentMin) { CalibrationParameters.ShiftPowerStationMin_dB = -NewShift; }
                else { CalibrationParameters.ShiftPowerStationMin_dB = (int)(LimithMin - curentDate.Transmitter.MaxPower_dBm); }
                if (LimithMax >= CurrentMax) { CalibrationParameters.ShiftPowerStationMax_dB = NewShift; }
                else { CalibrationParameters.ShiftPowerStationMax_dB = (int)(LimithMax - curentDate.Transmitter.MaxPower_dBm); }
                CalibrationParameters.ShiftPowerStationStep_dB = CalibrationParameters.ShiftPowerStationStep_dB / CalibrationParameters.DetailOfCascade;
            }

        }
	}
}
