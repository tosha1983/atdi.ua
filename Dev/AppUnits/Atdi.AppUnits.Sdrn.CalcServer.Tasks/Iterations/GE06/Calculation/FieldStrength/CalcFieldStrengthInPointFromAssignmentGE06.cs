using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.PropagationModels;
using Atdi.Platform.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Gis;
using Atdi.Contracts.Sdrn.DeepServices.GN06;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    public static class CalcFieldStrengthInPointFromAssignmentGE06
    {
        /// <summary>
        /// Расчет напряженности поля от BroadcastingAssignment
        /// </summary>
        /// <param name="broadcastingAssignment"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float Calc(BroadcastingAssignment broadcastingAssignment,
                                                                       PropagationModel propagationModel,
                                                                       Point point,
                                                                       IIterationsPool iterationsPool,
                                                                       IObjectPoolSite poolSite,
                                                                       ITransformation transformation,
                                                                       ITaskContext taskContext,
                                                                       IGn06Service gn06Service,
                                                                       ProjectMapData projectMapData,
                                                                       CluttersDesc cluttersDesc,
                                                                       string projection)
        {
            float resultCalcFieldStrength = 0;

            if ((propagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546) || (propagationModel.MainBlock.ModelType == MainCalcBlockModelType.ITU1546_4)
                || (propagationModel.AbsorptionBlock.Available == false)
                || (propagationModel.AdditionalBlock.Available == false)
                || (propagationModel.AtmosphericBlock.Available == false)
                || (propagationModel.ClutterBlock.Available == false)
                || (propagationModel.DiffractionBlock.Available == false)
                || (propagationModel.DuctingBlock.Available == false)
                || (propagationModel.ReflectionBlock.Available == false)
                || (propagationModel.SubPathDiffractionBlock.Available == false)
                || (propagationModel.TropoBlock.Available == false))
            {
                //1) Если модель распространения ITU 1546 для этого необходимо делать отдельную итерацию(Будет подготовлен сам код Юрой или Максимом) на подобии FieldStrengthCalcIteration(1.4.1, 1.4.2)
                var broadcastingFieldStrengthCalcData = new BroadcastingFieldStrengthCalcData()
                {
                    BroadcastingAssignment = broadcastingAssignment,
                    PropagationModel = propagationModel,
                    MapArea = projectMapData.Area,
                    ClutterContent = projectMapData.ClutterContent,
                    ReliefContent = projectMapData.ReliefContent,
                    TargetCoordinate = new PointEarthGeometric() { Longitude = point.Longitude, Latitude = point.Latitude, CoordinateUnits = CoordinateUnits.deg },
                };
                var iterationCorellationCalc = iterationsPool.GetIteration<BroadcastingFieldStrengthCalcData, BroadcastingFieldStrengthCalcResult>();
                var resFieldStrengthCalcResult = iterationCorellationCalc.Run(taskContext, broadcastingFieldStrengthCalcData);
                resultCalcFieldStrength = (float)resFieldStrengthCalcResult.FS_dBuVm.Value;
            }
            else
            {
                var contextStation = new Contracts.Sdrn.DeepServices.GN06.ContextStation();
                gn06Service.GetStationFromBroadcastingAssignment(broadcastingAssignment, ref contextStation);
                //2) Если модель распространения не ITU 1546.В данном случае BroadcastingAssignment преобразуется в Station(IContextStation)(1.3.2) и используется для расчета итерация FieldStrengthCalcIteration
                var fieldStrengthCalcData = new FieldStrengthCalcData()
                {
                    Antenna = contextStation.ClientContextStation.Antenna,
                    Transmitter = contextStation.ClientContextStation.Transmitter,
                    PropagationModel = propagationModel,
                    CluttersDesc = cluttersDesc,
                    MapArea = projectMapData.Area,
                    BuildingContent = projectMapData.BuildingContent,
                    ClutterContent = projectMapData.ClutterContent,
                    ReliefContent = projectMapData.ReliefContent,
                    PointCoordinate = contextStation.ClientContextStation.Coordinate,
                    TargetCoordinate = transformation.ConvertCoordinateToAtdi(new Wgs84Coordinate() { Longitude = point.Longitude, Latitude = point.Latitude }, projection),
                };
                var iterationFieldStrengthCalcData = iterationsPool.GetIteration<FieldStrengthCalcData, FieldStrengthCalcResult>();
                var resFieldStrengthCalcData = iterationFieldStrengthCalcData.Run(taskContext, fieldStrengthCalcData);
                resultCalcFieldStrength = (float)resFieldStrengthCalcData.FS_dBuVm.Value;
            }
            return resultCalcFieldStrength;
        }
    }
}
