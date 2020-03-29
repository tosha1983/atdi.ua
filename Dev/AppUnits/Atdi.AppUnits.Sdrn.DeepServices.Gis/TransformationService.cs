using System;
using Atdi.Platform;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using OSGeo.OGR;
using OSGeo.OSR;
using Atdi.Platform.Logging;

namespace Atdi.AppUnits.Sdrn.DeepServices.Gis
{
    public class TransformationService : ITransformation
    {
        private readonly ILogger _logger;
        private const uint Epsg4326 = 4326;
        private const uint PrefixEpsgN = 326;
        private const uint PrefixEpsgS = 327;
        private const string PrefixAtdiProjectionN = "4UTN";
        private const string PrefixAtdiProjectionS = "4UTS";


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
                Utils.PrepareConverting(out SpatialReference source, out SpatialReference destination, Epsg4326, toProjection);
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
                Utils.PrepareConverting(out SpatialReference source, out SpatialReference destination, fromProjection, toProjection);
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
                Utils.PrepareConverting(out SpatialReference source, out SpatialReference destination, Epsg4326, toProjection);
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
                Utils.PrepareConverting(out SpatialReference source, out SpatialReference destination, coordinate.Projection, toProjection);
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
                Utils.PrepareConverting(out SpatialReference source, out SpatialReference destination, fromProjection, Epsg4326);
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
                Utils.PrepareConverting(out SpatialReference source, out SpatialReference destination, coordinate.Projection, Epsg4326);
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
                if (epsgCodeString.StartsWith(PrefixEpsgN.ToString()))
                {
                    var code = epsgCodeString.Replace(PrefixEpsgN.ToString(), "");
                    atdiNameProjection = PrefixAtdiProjectionN + code;
                }
                else if (epsgCodeString.StartsWith(PrefixEpsgS.ToString()))
                {
                    var code = epsgCodeString.Replace(PrefixEpsgS.ToString(), "");
                    atdiNameProjection = PrefixAtdiProjectionS + code;
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
            var number = "";
            try
            {
                if (atdiProjection.Contains(PrefixAtdiProjectionN))
                {
                    var code = atdiProjection.Replace(PrefixAtdiProjectionN, "");
                    number = PrefixEpsgN.ToString() + code;
                }
                else if (atdiProjection.Contains(PrefixAtdiProjectionS))
                {
                    var code = atdiProjection.Replace(PrefixAtdiProjectionS, "");
                    number = PrefixEpsgS.ToString() + code;
                }
                else
                {
                    throw new NotImplementedException(Events.ForAtdiNameProjectionNoAlgorithmConvertingToEPSG.With(atdiProjection).Text);
                }
            }
            catch (Exception e)
            {
                _logger.Exception(Contexts.ThisComponent, Categories.Convert, e, this);
            }
            return Convert.ToUInt32(number); 
        }

        public void Dispose()
        {
            
        }
    }
}
