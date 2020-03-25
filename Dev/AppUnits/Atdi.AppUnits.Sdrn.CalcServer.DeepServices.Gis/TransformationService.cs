using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.CalcServer.DeepServices.Gis;
using Atdi.DataModels.Sdrn.CalcServer.DeepServices.Gis;
using OSGeo.OGR;
using OSGeo.OSR;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.CalcServer.DeepServices.Gis
{
    public class TransformationService : ITransformation
    {
        private readonly ILogger _logger;


        public TransformationService(ILogger logger)
        {
            this._logger = logger;
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();
        }


        public EpsgCoordinate ConvertCoordinateToEpgs(Wgs84Coordinate coordinate, uint toProjection)
        {
            var epsgCoordinate = new EpsgCoordinate();
            try
            {
                Utils.PrepareConvertation(out SpatialReference source, out SpatialReference destination, 4326, toProjection);
                var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.Longitude, coordinate.Latitude));
                var transform = new CoordinateTransformation(source, destination);
                ogrGeom.Transform(transform);
                ogrGeom.ExportToWkt(out string outGeom);
                epsgCoordinate = Utils.GetEpsgCoordinate(outGeom);
                ogrGeom.Dispose();
                transform.Dispose();
                source.Dispose();
                destination.Dispose();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return epsgCoordinate;
        }

        public EpsgCoordinate ConvertCoordinateToEpgs(EpsgCoordinate coordinate, uint fromProjection, uint toProjection)
        {
            var epsgCoordinate = new EpsgCoordinate();
            try
            {
                Utils.PrepareConvertation(out SpatialReference source, out SpatialReference destination, fromProjection, toProjection);
                var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y));
                var transform = new CoordinateTransformation(source, destination);
                ogrGeom.Transform(transform);
                ogrGeom.ExportToWkt(out string outGeom);
                epsgCoordinate = Utils.GetEpsgCoordinate(outGeom);
                ogrGeom.Dispose();
                transform.Dispose();
                source.Dispose();
                destination.Dispose();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return epsgCoordinate;
        }

        public EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(Wgs84Coordinate coordinate, uint toProjection)
        {
            var epsgProjectionCoordinate = new EpsgProjectionCoordinate();
            try
            {
                Utils.PrepareConvertation(out SpatialReference source, out SpatialReference destination, 4326, toProjection);
                var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.Longitude, coordinate.Latitude));
                var transform = new CoordinateTransformation(source, destination);
                ogrGeom.Transform(transform);
                ogrGeom.ExportToWkt(out string outGeom);
                var epsgProjCoord = Utils.GetEpsgCoordinate(outGeom);
                epsgProjectionCoordinate = new EpsgProjectionCoordinate()
                {
                    Projection = toProjection,
                    X = epsgProjCoord.X,
                    Y = epsgProjCoord.Y
                };
                ogrGeom.Dispose();
                transform.Dispose();
                source.Dispose();
                destination.Dispose();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return epsgProjectionCoordinate;
        }

        public EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(EpsgProjectionCoordinate coordinate, uint toProjection)
        {
            var epsgProjectionCoordinate = new EpsgProjectionCoordinate();
            try
            {
                Utils.PrepareConvertation(out SpatialReference source, out SpatialReference destination, coordinate.Projection, toProjection);
                var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y));
                var transform = new CoordinateTransformation(source, destination);
                ogrGeom.Transform(transform);
                ogrGeom.ExportToWkt(out string outGeom);
                var epsgProjCoord = Utils.GetEpsgCoordinate(outGeom);
                epsgProjectionCoordinate = new EpsgProjectionCoordinate()
                {
                    Projection = toProjection,
                    X = epsgProjCoord.X,
                    Y = epsgProjCoord.Y
                };
                ogrGeom.Dispose();
                transform.Dispose();
                source.Dispose();
                destination.Dispose();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return epsgProjectionCoordinate;
        }

        public Wgs84Coordinate ConvertCoordinateToWgs84(EpsgCoordinate coordinate, uint fromProjection)
        {
            var wgs84Coordinate = new Wgs84Coordinate();
            try
            {
                Utils.PrepareConvertation(out SpatialReference source, out SpatialReference destination, fromProjection, 4326);
                var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y));
                var transform = new CoordinateTransformation(source, destination);
                ogrGeom.Transform(transform);
                ogrGeom.ExportToWkt(out string outGeom);
                wgs84Coordinate = Utils.GetWgs84Coordinate(outGeom);
                ogrGeom.Dispose();
                transform.Dispose();
                source.Dispose();
                destination.Dispose();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return wgs84Coordinate;
        }

        public Wgs84Coordinate ConvertCoordinateToWgs84(EpsgProjectionCoordinate coordinate)
        {
            var wgs84Coordinate = new Wgs84Coordinate();
            try
            {
                Utils.PrepareConvertation(out SpatialReference source, out SpatialReference destination, coordinate.Projection, 4326);
                var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y));
                var transform = new CoordinateTransformation(source, destination);
                ogrGeom.Transform(transform);
                ogrGeom.ExportToWkt(out string outGeom);
                wgs84Coordinate = Utils.GetWgs84Coordinate(outGeom);
                ogrGeom.Dispose();
                transform.Dispose();
                source.Dispose();
                destination.Dispose();
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return wgs84Coordinate;
        }

        public string ConvertProjectionToAtdiName(uint epsgCode)
        {
            var atdiNameProjection = "";
            try
            {
                var epsgCodeString = epsgCode.ToString();
                if (epsgCodeString.StartsWith("326"))
                {
                    var code = epsgCodeString.Replace("326", "");
                    atdiNameProjection = "4UTN" + code;
                }
                else if (epsgCodeString.StartsWith("327"))
                {
                    var code = epsgCodeString.Replace("327", "");
                    atdiNameProjection = "4UTS" + code;
                }
                else
                {
                    throw new NotImplementedException(Events.ForEPSGNoAlgorithmConvertingToAtdiNameProjection.With(epsgCode).Text);
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return atdiNameProjection;
        }

        public uint ConvertProjectionToCode(string atdiProjection)
        {
            uint? outProjectionCode = null;
            try
            {
                var number = "";
                if (atdiProjection.Contains("4UTN"))
                {
                    var code = atdiProjection.Replace("4UTN", "");
                    number = "326" + code;
                }
                else if (atdiProjection.Contains("4UTS"))
                {
                    var code = atdiProjection.Replace("4UTS", "");
                    number = "327" + code;
                }
                else
                {
                    throw new NotImplementedException(Events.ForAtdiNameProjectionNoAlgorithmConvertingToEPSG.With(atdiProjection).Text);
                }
                outProjectionCode = Convert.ToUInt32(number);
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return outProjectionCode.Value;
        }

        public void Dispose()
        {

        }
    }
}
