using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;



namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public class CopyMobStationToEwxFile
    {
        private ILogger _logger { get; set; }
        private Condition _condition { get; set; }
        private readonly IQueryExecutor _queryExecutor;
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private string _tableName { get; set; }
        private const int CountInParams = 100;



        public CopyMobStationToEwxFile(Condition condition, string tableName, IDataLayer<IcsmDataOrm> dataLayer,  ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._queryExecutor = this._dataLayer.Executor<IcsmDataContext>();
            this._condition = condition;
            this._logger = logger;
            this._tableName = tableName;
        }


        public static List<Station[]> BreakDown(Station[] elements, int? countStationsInEwxFile)
        {
            var arrIntEmitting = new List<Station[]>();
            var listIntEmitting = new List<Station>();
            int cnt = 1;
            for (int i = 0; i < elements.Length; i++)
            {
                listIntEmitting.Add(elements[i]);
                if (cnt >= countStationsInEwxFile.Value)
                {
                    arrIntEmitting.Add(listIntEmitting.ToArray());
                    listIntEmitting.Clear();
                    cnt = 0;
                }
                ++cnt;
            }
            if ((listIntEmitting != null) && (listIntEmitting.Count > 0))
            {
                arrIntEmitting.Add(listIntEmitting.ToArray());
            }
            return arrIntEmitting;
        }



        public EwxData[] Copy(DataConfig dataConfig, string icsTelecomEwxFile, ILogger logger)
        {
            var stationIds = new List<int>();
            var dicStationNames = new Dictionary<string, int>();
            var lstewxFiles = new List<EwxData>();
            var lstStations = new List<Station>();
            try
            {
                var selectedColumns = new string[] {
                    "ID",
                    "NAME",
                    "ELEVATION",
                    "Position.ADDRESS",
                    "Position.ASL",
                    "PWR_ANT",
                    "TX_HIGH_FREQ",
                    "BW",
                    "AZIMUTH",
                    "AGL",
                    "GAIN",
                    "TX_LOSSES",
                    "RX_LOSSES",
                    "Position.LONGITUDE",
                    "Position.LATITUDE",
                    "Licence.ID",
                    "Antenna.POLARIZATION",
                    "Antenna.DIAGA",
                    "Antenna.DIAGH",
                    "Antenna.DIAGV",
                    "STATUS",
                    "STANDARD",
                    "Owner.CODE",
                    "Position.PROVINCE",
                    "AssignedFrequencies.RX_FREQ",
                    "AssignedFrequencies.TX_FREQ"
                };


                var QueryFromTable = _dataLayer.Builder
                .From(this._tableName)
                .Where(this._condition)
                .OrderByAsc("ID")
                .Select(selectedColumns);

                var isNotEmptyInTable = this._queryExecutor
             .Fetch(QueryFromTable, reader =>
             {
                 var res = false;
                 while (reader.Read())
                 {
                     var id = reader.GetValueAsInt32(typeof(int), reader.GetOrdinal("ID"));

                     if (stationIds.Contains(id))
                     {
                         continue;
                     }
                     stationIds.Add(id);

                     var station = new Station();

                     station.Id = id;

                     station.Address = reader.GetNullableValueAsString(typeof(string), reader.GetOrdinal("Position.ADDRESS"));
                     var Altitude = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("Position.ASL"));
                     if (Altitude != null)
                     {
                         station.Altitude = Altitude.Value;
                     }

                     var Azimuth = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("AZIMUTH"));
                     if (Azimuth != null)
                     {
                         station.Azimuth = Azimuth.Value;
                     }

                     var Bandwidth = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("BW"));
                     if (Bandwidth != null)
                     {
                         station.Bandwidth = Bandwidth.Value;
                     }

                     var BandwidthRx = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("BW"));
                     if (BandwidthRx != null)
                     {
                         station.BandwidthRx = BandwidthRx.Value;
                     }

                     var CallSign = reader.GetNullableValueAsString(typeof(string), reader.GetOrdinal("NAME"));
                     if (CallSign != null)
                     {
                         station.CallSign = CallSign;
                         if (dicStationNames.ContainsKey(station.CallSign))
                         {
                             dicStationNames[station.CallSign] = dicStationNames[station.CallSign] + 1;
                             station.CallSign = station.CallSign + "_" + dicStationNames[station.CallSign];
                         }
                         else
                         {
                             dicStationNames.Add(station.CallSign, 0);
                         }
                     }

                     var CoordX = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("Position.LONGITUDE"));
                     if (CoordX != null)
                     {
                         station.CoordX = CoordX.Value;
                     }


                     var CoordY = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("Position.LATITUDE"));
                     if (CoordY != null)
                     {
                         station.CoordY = CoordY.Value;
                     }

                     var DiagH = reader.GetNullableValueAsString(typeof(string), reader.GetOrdinal("Antenna.DIAGH"));
                     if (DiagH != null)
                     {
                         station.DiagH = DiagH;
                         station.DiagH = station.DiagH;
                     }

                     var DiagV = reader.GetNullableValueAsString(typeof(string), reader.GetOrdinal("Antenna.DIAGV"));
                     if (DiagV != null)
                     {
                         station.DiagV = DiagV;
                         station.DiagV = station.DiagV;
                     }

                     var Frequency = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("AssignedFrequencies.TX_FREQ"));
                     if (Frequency != null)
                     {
                         station.Frequency = Frequency.Value;
                     }

                     var Gain = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("GAIN"));
                     if (Gain != null)
                     {
                         station.Gain = Gain.Value;
                         station.GainRx = Gain.Value;
                     }

                     var HAntenna = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("AGL"));
                     if (HAntenna != null)
                     {
                         station.HAntenna = HAntenna.Value;
                     }

                     var Losses = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("TX_LOSSES"));
                     if (Losses != null)
                     {
                         station.Losses = Losses.Value;
                         station.LossesRx = Losses.Value;
                     }

                     var tilt = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("ELEVATION"));
                     if (tilt != null)
                     {
                         if (dataConfig.AnotherParameters == null)
                         {
                             station.Tilt = tilt.Value;
                         }
                         else
                         {
                             if (dataConfig.AnotherParameters.Elevation != null)
                             {
                                 station.Tilt = dataConfig.AnotherParameters.Elevation.Value;
                             }
                             else
                             {
                                 station.Tilt = tilt.Value;
                             }
                         }
                     }

                     var NetId = reader.GetNullableValueAsInt32(typeof(int), reader.GetOrdinal("Licence.ID"));
                     if (NetId != null)
                     {
                         station.NetId = NetId.Value;
                     }

                     var NominalPower = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("PWR_ANT"));
                     if (NominalPower != null)
                     {
                         station.NominalPower = NominalPower.Value;
                         station.NominalPower = Math.Round(Math.Pow(10, station.NominalPower / 10), 7);
                     }

                     station.Polar = reader.GetNullableValueAsString(typeof(string), reader.GetOrdinal("Antenna.POLARIZATION"));
                     station.PolarRx = reader.GetNullableValueAsString(typeof(string), reader.GetOrdinal("Antenna.POLARIZATION"));

                     var D_cx1 = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("AssignedFrequencies.TX_FREQ"));
                     if (D_cx1 != null)
                     {
                         station.D_cx1 = D_cx1.Value;
                     }

                     var U_cx1 = reader.GetNullableValueAsDouble(typeof(double), reader.GetOrdinal("AssignedFrequencies.RX_FREQ"));
                     if (U_cx1 != null)
                     {
                         station.U_cx1 = U_cx1.Value;
                     }


                     lstStations.Add(new Station()
                     {
                         Address = station.Address,
                         Altitude = station.Altitude,
                         Azimuth = station.Azimuth,
                         Bandwidth = station.Bandwidth,
                         BandwidthRx = station.BandwidthRx,
                         CallSign = station.CallSign,
                         Category = station.Category,
                         CoordX = station.CoordX,
                         CoordY = station.CoordY,
                         DiagH = station.DiagH,
                         DiagV = station.DiagV,
                         D_cx1 = station.D_cx1,
                         FKTB = station.FKTB,
                         Frequency = station.Frequency,
                         Gain = station.Gain,
                         GainRx = station.GainRx,
                         HAntenna = station.HAntenna,
                         Info1 = station.Info1,
                         Losses = station.Losses,
                         LossesRx = station.LossesRx,
                         NetId = station.NetId,
                         NominalPower = station.NominalPower,
                         Polar = station.Polar,
                         PolarRx = station.PolarRx,
                         Tilt = station.Tilt,
                         TypeCoord = station.TypeCoord,
                         U_cx1 = station.U_cx1
                     }
                     );


                     res = true;
                 }
                 return res;
             });

                int? countStationsInBlock = null;
                if (dataConfig.AnotherParameters == null)
                {
                    countStationsInBlock = CountInParams;
                }
                else
                {
                    if (dataConfig.AnotherParameters.CountStationsInEwxFile == null)
                    {
                        countStationsInBlock = CountInParams;
                    }
                    else
                    {
                        countStationsInBlock = dataConfig.AnotherParameters.CountStationsInEwxFile;
                    }
                }
                var elments = BreakDown(lstStations.ToArray(), countStationsInBlock);
                for (int l = 0; l < elments.Count; l++)
                {
                    var ewxFile = new EwxData();
                    ewxFile.Header = new Header();
                    ewxFile.Header.CountStation = elments[l].Length;
                    ewxFile.Stations = elments[l].ToArray();
                    lstewxFiles.Add(ewxFile);
                }
            }
            catch (Exception e)
            {
                logger.Exception(Contexts.CalcCoverages, e);
            }
            return lstewxFiles.ToArray();
        }
    }
}
