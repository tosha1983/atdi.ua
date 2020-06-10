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
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.Platform.Data;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
	/// <summary>
    /// 
	/// </summary>
	public class StationCorellationCalcIteration : IIterationHandler<StationCorellationCalcData, ResultCorrelationGSIDGroupeStationsWithoutParameters>
	{
		private readonly ILogger _logger;
        private readonly IIterationsPool _iterationsPool;
        private readonly ITransformation _transformation;
        private readonly IObjectPool<CalcPoint[]> _calcPointArrayPool;
        private readonly IObjectPoolSite _poolSite;

        /// <summary>
        /// Заказываем у контейнера нужные сервисы
        /// </summary>
        public StationCorellationCalcIteration(
            IIterationsPool iterationsPool,
            ITransformation transformation,
            IObjectPoolSite poolSite,
            ILogger logger)
        {
            _iterationsPool = iterationsPool;
            _transformation = transformation;
            _poolSite = poolSite;
            _calcPointArrayPool = _poolSite.GetPool<CalcPoint[]>(ObjectPools.StationCalibrationCalcPointArrayObjectPool);
            _logger = logger;
        }

        public static double ConvertdBuVmTouV (double dBuVm)
        {
            return Math.Pow(10, 0.05 * dBuVm);
        }

        public ResultCorrelationGSIDGroupeStationsWithoutParameters Run(ITaskContext taskContext, StationCorellationCalcData data)
		{
            var calcCorellationResult = new ResultCorrelationGSIDGroupeStationsWithoutParameters();
            var calcPointArrayBuffer = default(CalcPoint[]);

            // вызываем механизм расчета FieldStrengthCalcData на основе переданных данных data.FieldStrengthCalcData
            var iterationFieldStrengthCalcData = _iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();


            try
            {
                calcPointArrayBuffer = _calcPointArrayPool.Take();


                // Corner coordinates
                //data.FieldStrengthCalcData.MapArea.LowerLeft.X;
                //data.FieldStrengthCalcData.TargetAltitude_m = 
                var lowerLeftCoord_m = data.FieldStrengthCalcData.MapArea.LowerLeft;
                var upperRightCoord_m = data.FieldStrengthCalcData.MapArea.UpperRight;
                // Step
                double lonStep_dec = data.FieldStrengthCalcData.MapArea.AxisX.Step;//_transformation.ConvertCoordinateToWgs84(data.FieldStrengthCalcData.MapArea.LowerLeft.X, data.CodeProjection);
                double latStep_dec = data.FieldStrengthCalcData.MapArea.AxisY.Step;


                // 

                // 0 - приведение координат к центру условного "пикселя" карты, 
                // 1 - усреднение FS по одним координатам
                int counter = 0;
                int patternLossExceedCount = 0;
                int outOfMapCount = 0;
                for (int i = 0; i < data.GSIDGroupeDriveTests.Points.Length; i++)
                {
                    if (data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm >= data.CorellationParameters.MinRangeMeasurements_dBmkV &&
                        data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm <= data.CorellationParameters.MaxRangeMeasurements_dBmkV &&
                       Utils.IsInsideMap(data.GSIDGroupeDriveTests.Points[i].Coordinate.X, data.GSIDGroupeDriveTests.Points[i].Coordinate.Y, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y))
                        {
                        bool isFoubdInBuffer = false;

                        if (counter > 0)
                        {
                            // Сравнение следующих координат с приведенными к центру пикселя,
                            for (int j = 0; j < counter; j++)
                            {
                                bool isInsidePixelLon = Math.Abs(data.GSIDGroupeDriveTests.Points[i].Coordinate.X - calcPointArrayBuffer[j].X) < data.FieldStrengthCalcData.MapArea.AxisX.Step / 2;
                                bool isInsidePixelLat = Math.Abs(data.GSIDGroupeDriveTests.Points[i].Coordinate.Y - calcPointArrayBuffer[j].Y) < data.FieldStrengthCalcData.MapArea.AxisY.Step / 2;
                                if (isInsidePixelLon && isInsidePixelLat)
                                {
                                    //  в случае если по координатам уже есть изменерия, напряжённость усредняется
                                    calcPointArrayBuffer[j].FSMeas = (calcPointArrayBuffer[j].Count * calcPointArrayBuffer[j].FSMeas + data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm) / (calcPointArrayBuffer[j].Count + 1);
                                    //20 * Math.Log10((calcPointArrayBuffer[j].Count * Math.Pow(10, 0.05 * calcPointArrayBuffer[j].FSMeas) + Math.Pow(10, 0.05 * data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm)) / (calcPointArrayBuffer[j].Count + 1));//data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm;
                                    calcPointArrayBuffer[j].Count += 1;
                                    isFoubdInBuffer = true;
                                    break; // как только нашли точку в буфере, у которой совпали координаты в пределах пикселя - поиск прекращается
                                }
                            }
                        }
                        if (isFoubdInBuffer == false || counter == 0)
                        {
                            int lowerLeftX = (int)(lowerLeftCoord_m.X + Math.Floor((data.GSIDGroupeDriveTests.Points[i].Coordinate.X - lowerLeftCoord_m.X) / data.FieldStrengthCalcData.MapArea.AxisX.Step) * data.FieldStrengthCalcData.MapArea.AxisX.Step + data.FieldStrengthCalcData.MapArea.AxisX.Step / 2);
                            int lowerLeftY = (int)(lowerLeftCoord_m.Y + Math.Floor((data.GSIDGroupeDriveTests.Points[i].Coordinate.Y - lowerLeftCoord_m.Y) / data.FieldStrengthCalcData.MapArea.AxisY.Step) * data.FieldStrengthCalcData.MapArea.AxisY.Step + data.FieldStrengthCalcData.MapArea.AxisY.Step / 2);

                            data.FieldStrengthCalcData.TargetCoordinate.X = lowerLeftX;
                            data.FieldStrengthCalcData.TargetCoordinate.Y = lowerLeftY;
                            data.FieldStrengthCalcData.TargetAltitude_m = data.GSIDGroupeDriveTests.Points[0].Height_m; // add to FS buffer model ?????????????????
                            var faCalculationResult = iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData);

                            if (faCalculationResult.AntennaPatternLoss_dB <= 30.0)
                            {
                                // выполняется для первой итерации и в случае если по координатам не было измерений
                                calcPointArrayBuffer[counter].Count = 1;
                                calcPointArrayBuffer[counter].X = lowerLeftX;
                                calcPointArrayBuffer[counter].Y = lowerLeftY;

                                calcPointArrayBuffer[counter].FSCalc = faCalculationResult.FS_dBuVm.Value;// iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData).FS_dBuVm.Value;
                                calcPointArrayBuffer[counter].FSMeas = data.GSIDGroupeDriveTests.Points[i].FieldStrength_dBmkVm;
                                
                                counter++;
                            }
                            else
                            {
                                patternLossExceedCount++;
                            }
                        }
                    }
                }


                
                if (counter > 0)
                {
                    // 2 - расчёт напряжённости в окрестности точки
                    for (int i = 0; i < counter; i++)
                    {
                        if (data.CorellationParameters.CorrelationDistance_m > data.FieldStrengthCalcData.MapArea.AxisX.Step)
                        {
                            int lonAroundStart = (int)calcPointArrayBuffer[i].X - data.CorellationParameters.CorrelationDistance_m / 2;
                            int latAroundStart = (int)calcPointArrayBuffer[i].Y - data.CorellationParameters.CorrelationDistance_m / 2;
                            int lonAroundStop = (int)calcPointArrayBuffer[i].X + data.CorellationParameters.CorrelationDistance_m / 2;
                            int latAroundStop = (int)calcPointArrayBuffer[i].Y + data.CorellationParameters.CorrelationDistance_m / 2;

                            double measCalcFSdifference = (calcPointArrayBuffer[i].FSMeas - calcPointArrayBuffer[i].FSCalc);
                            double minMeasCalcFSdifference = measCalcFSdifference;

                            for (int lonPointAround = lonAroundStart; lonPointAround < lonAroundStop; lonPointAround += data.FieldStrengthCalcData.MapArea.AxisX.Step)
                            {
                                for (int latPointAround = latAroundStart; latPointAround < latAroundStop; latPointAround += data.FieldStrengthCalcData.MapArea.AxisY.Step)
                                {
                                    if (Utils.IsInsideMap(lonPointAround, latPointAround, lowerLeftCoord_m.X, lowerLeftCoord_m.Y, upperRightCoord_m.X, upperRightCoord_m.Y))
                                    {
                                        data.FieldStrengthCalcData.TargetCoordinate.X = lonPointAround;
                                        data.FieldStrengthCalcData.TargetCoordinate.Y = latPointAround;
                                        double FSaroundDif = (calcPointArrayBuffer[i].FSMeas - iterationFieldStrengthCalcData.Run(taskContext, data.FieldStrengthCalcData).FS_dBuVm.Value);

                                        if (Math.Abs(FSaroundDif) < Math.Abs(minMeasCalcFSdifference))
                                        {
                                            minMeasCalcFSdifference = FSaroundDif;
                                        }
                                    }
                                    else
                                    {
                                        outOfMapCount++;
                                    }
                                    
                                }
                            }
                            // если находится точка с расчётной напряжённостью ближе к измеренной - запоминается это значение
                            calcPointArrayBuffer[i].FSCalc = calcPointArrayBuffer[i].FSMeas - minMeasCalcFSdifference;


                        }
                    }


                    //// 3 - расчёт корреляционных параметров, подготовка результата
                    if (data.CorellationParameters.Detail)
                    {
                        calcCorellationResult.CorrellationPoints = new CorrellationPoint[counter];
                    }

                    double diffLessThanDeltaCount = 0;
                    double diffCalcMeas = 0;
                    double sumDiffCalcMeas = 0;

                    double meanCalcFS = 0;
                    double meanMeasFS = 0;


                    for (int i = 0; i < counter; i++)
                    {
                        diffCalcMeas = Math.Abs(calcPointArrayBuffer[i].FSMeas - calcPointArrayBuffer[i].FSCalc);
                        if (diffCalcMeas <= data.CorellationParameters.Delta_dB)
                        {
                            diffLessThanDeltaCount += 1;
                        }
                        sumDiffCalcMeas += diffCalcMeas;

                        //pierson
                        meanMeasFS += calcPointArrayBuffer[i].FSMeas;
                        meanCalcFS += calcPointArrayBuffer[i].FSCalc;

                        //- CorreIPoints[](Lon_DEC, Lat_DEC, FSMeas_dBmkVm, FSCalc_dBmkVm, Dist_km) выдаётся только если Detail = true(нам для отладки не плохо было бы это хоть как визуализировать на карте)
                        if (data.CorellationParameters.Detail)
                        {
                            calcCorellationResult.CorrellationPoints[i].Dist_km = GeometricСalculations.GetDistance_km(calcPointArrayBuffer[i].X, calcPointArrayBuffer[i].Y, data.GSIDGroupeStation.Site.Longitude, data.GSIDGroupeStation.Site.Latitude);
                            calcCorellationResult.CorrellationPoints[i].FSCalc_dBmkVm = calcPointArrayBuffer[i].FSCalc;
                            calcCorellationResult.CorrellationPoints[i].FSMeas_dBmkVm = calcPointArrayBuffer[i].FSMeas;

                            var coordinateTransform = _transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = calcPointArrayBuffer[i].X, Y = calcPointArrayBuffer[i].Y }, _transformation.ConvertProjectionToCode(data.CodeProjection));
                            calcCorellationResult.CorrellationPoints[i].Lon_DEC = coordinateTransform.Longitude;
                            calcCorellationResult.CorrellationPoints[i].Lat_DEC = coordinateTransform.Latitude;
                            //var coord = _transformation.ConvertCoordinateToWgs84(data.FieldStrengthCalcData.MapArea.LowerLeft.X, data.CodeProjection)
                        }
                    }



                    meanMeasFS /= counter;
                    meanCalcFS /= counter;
                    double a1 = 0; double a2 = 0; double a3 = 0;
                    for (int i = 0; i < counter; i++)
                    {
                        //a1 = a1 + ((ConvertdBuVmTouV(calcPointArrayBuffer[i].FSMeas) - meanMeasFS) * (ConvertdBuVmTouV(calcPointArrayBuffer[i].FSCalc) - meanCalcFS));
                        //a2 = a2 + ((ConvertdBuVmTouV(calcPointArrayBuffer[i].FSMeas) - meanMeasFS) * (ConvertdBuVmTouV(calcPointArrayBuffer[i].FSMeas) - meanMeasFS));
                        //a3 = a3 + ((ConvertdBuVmTouV(calcPointArrayBuffer[i].FSCalc) - meanCalcFS) * (ConvertdBuVmTouV(calcPointArrayBuffer[i].FSCalc) - meanCalcFS));
                        a1 = a1 + ((calcPointArrayBuffer[i].FSMeas - meanMeasFS) * (calcPointArrayBuffer[i].FSCalc - meanCalcFS));
                        a2 = a2 + ((calcPointArrayBuffer[i].FSMeas - meanMeasFS) * (calcPointArrayBuffer[i].FSMeas - meanMeasFS));
                        a3 = a3 + ((calcPointArrayBuffer[i].FSCalc - meanCalcFS) * (calcPointArrayBuffer[i].FSCalc - meanCalcFS));
                    }

                    //- Freq_MHz(частота передатчика станции)
                    calcCorellationResult.Freq_MHz = data.GSIDGroupeStation.Transmitter.Freq_MHz;
                    //- Delta_dB(входной параметр)
                    calcCorellationResult.Delta_dB = data.CorellationParameters.Delta_dB;
                    //- Correlation_pc(процент точек где результаты измерений отличаться от расчетного менее чем на Delta_dB)
                    calcCorellationResult.Corellation_pc = diffLessThanDeltaCount / counter * 100;
                    //- StdDev_dB = sqrt(sum(y - x)) / n
                    calcCorellationResult.StdDev_dB = (float)(Math.Sqrt(sumDiffCalcMeas) / counter);
                    //- AvErr_dB = sum(y - x) / n
                    calcCorellationResult.AvErr_dB = (float)(sumDiffCalcMeas / counter);
                    //- Correl factor(логарифмическая корреляция пирсона у нас реализована)
                    calcCorellationResult.Corellation_factor = a1 / Math.Sqrt(a2 * a3);
                }
                else
                {
                    calcCorellationResult.Freq_MHz = 0;
                    //- Delta_dB(входной параметр)
                    calcCorellationResult.Delta_dB = 0;
                    //- Correlation_pc(процент точек где результаты измерений отличаться от расчетного менее чем на Delta_dB)
                    calcCorellationResult.Corellation_pc = 0;
                    //- StdDev_dB = sqrt(sum(y - x)) / n
                    calcCorellationResult.StdDev_dB = 0;
                    //- AvErr_dB = sum(y - x) / n
                    calcCorellationResult.AvErr_dB = 0;
                    //- Correl factor(логарифмическая корреляция пирсона у нас реализована)
                    calcCorellationResult.Corellation_factor = 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (calcPointArrayBuffer != null)
                {
                    _calcPointArrayPool.Put(calcPointArrayBuffer);
                }
            }
            return calcCorellationResult;
        }
	}
}
