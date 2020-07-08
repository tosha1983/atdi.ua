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
        private const uint Epsg4326 = 4326;
        private const uint Epsg3035 = 3035;
        private const uint PrefixEpsgN = 326;
        private const uint PrefixEpsgS = 327;
        private const string PrefixAtdiProjectionN = "4UTN";
        private const string PrefixAtdiProjectionS = "4UTS";
        private const string PrefixAtdiProjectionEPSG = "EPSG";

        public TransformationService()
        {
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();
        }


        public EpsgCoordinate ConvertCoordinateToEpgs(Wgs84Coordinate coordinate, uint toProjection)
        {
            try
            {
                using (var source = new SpatialReference(null))
                using (var destination = new SpatialReference(null))
                {
                    source.ImportFromEPSG((int)Epsg4326);
                    destination.ImportFromEPSG((int)toProjection);
                    using (var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.Longitude, coordinate.Latitude)))
                    using (var transform = new CoordinateTransformation(source, destination))
                    {
                        ogrGeom.Transform(transform);
                        ogrGeom.ExportToWkt(out string outGeom);
                        return Utils.GetEpsgCoordinate(outGeom);
                    }
                } 
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertCoordinatesToEPSGFormat, e);
            }
        }

        public EpsgCoordinate ConvertCoordinateToEpgs(EpsgCoordinate coordinate, uint fromProjection, uint toProjection)
        {
            try
            {
                using (var source = new SpatialReference(null))
                using (var destination = new SpatialReference(null))
                {
                    source.ImportFromEPSG((int)fromProjection);
                    destination.ImportFromEPSG((int)toProjection);
                    using (var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y)))
                    using (var transform = new CoordinateTransformation(source, destination))
                    {
                        ogrGeom.Transform(transform);
                        ogrGeom.ExportToWkt(out string outGeom);
                        return Utils.GetEpsgCoordinate(outGeom);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertCoordinatesToEPSGFormat, e);
            }
        }

        public EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(Wgs84Coordinate coordinate, uint toProjection)
        {
            try
            {
                using (var source = new SpatialReference(null))
                using (var destination = new SpatialReference(null))
                {
                    source.ImportFromEPSG((int)Epsg4326);
                    destination.ImportFromEPSG((int)toProjection);
                    using (var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.Longitude, coordinate.Latitude)))
                    using (var transform = new CoordinateTransformation(source, destination))
                    {
                        ogrGeom.Transform(transform);
                        ogrGeom.ExportToWkt(out string outGeom);
                        var epsgProjCoord = Utils.GetEpsgCoordinate(outGeom);
                        return new EpsgProjectionCoordinate()
                        {
                            Projection = toProjection,
                            X = epsgProjCoord.X,
                            Y = epsgProjCoord.Y
                        };
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertCoordinatesToEPSGFormat, e);
            }
        }

        public EpsgProjectionCoordinate ConvertCoordinateToEpgsProjection(EpsgProjectionCoordinate coordinate, uint toProjection)
        {
            try
            {
                using (var source = new SpatialReference(null))
                using (var destination = new SpatialReference(null))
                {
                    source.ImportFromEPSG((int)coordinate.Projection);
                    destination.ImportFromEPSG((int)toProjection);
                    using (var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y)))
                    using (var transform = new CoordinateTransformation(source, destination))
                    {
                        ogrGeom.Transform(transform);
                        ogrGeom.ExportToWkt(out string outGeom);
                        var epsgProjCoord = Utils.GetEpsgCoordinate(outGeom);
                        return new EpsgProjectionCoordinate()
                        {
                            Projection = toProjection,
                            X = epsgProjCoord.X,
                            Y = epsgProjCoord.Y
                        };
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertCoordinatesToEPSGFormat, e);
            }
        }

        public Wgs84Coordinate ConvertCoordinateToWgs84(EpsgCoordinate coordinate, uint fromProjection)
        {
            try
            {
                using (var source = new SpatialReference(null))
                using (var destination = new SpatialReference(null))
                {
                    source.ImportFromEPSG((int)fromProjection);
                    destination.ImportFromEPSG((int)Epsg4326);
                    using (var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y)))
                    using (var transform = new CoordinateTransformation(source, destination))
                    {
                        ogrGeom.Transform(transform);
                        ogrGeom.ExportToWkt(out string outGeom);
                        return Utils.GetWgs84Coordinate(outGeom);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertCoordinatesToWgs84Format, e);
            }
        }

        public Wgs84Coordinate ConvertCoordinateToWgs84(EpsgProjectionCoordinate coordinate)
        {
            try
            {
                using (var source = new SpatialReference(null))
                using (var destination = new SpatialReference(null))
                {
                    source.ImportFromEPSG((int)coordinate.Projection);
                    destination.ImportFromEPSG((int)Epsg4326);
                    using (var ogrGeom = Geometry.CreateFromWkt(Utils.GenerateWktString(coordinate.X, coordinate.Y)))
                    using (var transform = new CoordinateTransformation(source, destination))
                    {
                        ogrGeom.Transform(transform);
                        ogrGeom.ExportToWkt(out string outGeom);
                        return Utils.GetWgs84Coordinate(outGeom);
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertCoordinatesToWgs84Format, e);
            }
        }

        public string ConvertProjectionToAtdiName(uint epsgCode)
        {
            try
            {
                var epsgCodeString = epsgCode.ToString();
                if (epsgCodeString.StartsWith(PrefixEpsgN.ToString()))
                {
                    var code = epsgCodeString.Replace(PrefixEpsgN.ToString(), "");
                    return PrefixAtdiProjectionN + code;
                }
                else if (epsgCodeString.StartsWith(PrefixEpsgS.ToString()))
                {
                    var code = epsgCodeString.Replace(PrefixEpsgS.ToString(), "");
                    return PrefixAtdiProjectionS + code;
                }
                else if ((epsgCodeString.StartsWith(Epsg4326.ToString())) || (epsgCodeString.StartsWith(Epsg3035.ToString()))) 
                {
                    return PrefixAtdiProjectionEPSG + epsgCode.ToString();
                }
                else
                {
                    throw new NotImplementedException(Events.ForEPSGNoAlgorithmConvertingToAtdiNameProjection.With(epsgCode).Text);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertEPSGCodeToAtdiProjectionName, e);
            }
        }

        public uint ConvertProjectionToCode(string atdiProjection)
        {
            try
            {
                if (atdiProjection.Contains(PrefixAtdiProjectionN))
                {
                    var code = atdiProjection.Replace(PrefixAtdiProjectionN, "");
                    return Convert.ToUInt32(PrefixEpsgN.ToString() + code);
                }
                else if (atdiProjection.Contains(PrefixAtdiProjectionS))
                {
                    var code = atdiProjection.Replace(PrefixAtdiProjectionS, "");
                    return Convert.ToUInt32(PrefixEpsgS.ToString() + code);
                }
                else if (atdiProjection.Contains(PrefixAtdiProjectionEPSG))
                {
                    var code = atdiProjection.Replace(PrefixAtdiProjectionEPSG, "");
                    return Convert.ToUInt32(code);
                }
                else
                {
                    throw new NotImplementedException(Events.ForAtdiNameProjectionNoAlgorithmConvertingToEPSG.With(atdiProjection).Text);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(Exceptions.ErrorConvertAtdiProjectionNameToEPSGCode, e);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
