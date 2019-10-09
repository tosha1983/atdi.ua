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
using Atdi.DataModels.CoverageCalculation;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;
using Atdi.DataModels.CommonOperation;
using Atdi.DataModels;
using System.ServiceModel;
using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;
using Atdi.WebQuery.CoverageCalculation;


namespace Atdi.WebQuery.CoverageCalculation
{
    public static class CopyStationsToEwxFile
    {

        private static DataSetColumn GetColumnId(TypedRowsDataSet typedRowsDataSet, string NameField)
        {
            DataSetColumn dataSetColumn = null;
            var rowsDataSet = typedRowsDataSet.Columns.ToList();
            dataSetColumn = rowsDataSet.Find(x => x.Name == NameField);
            return dataSetColumn;
        }
        private static ColumnValue GetValue(TypedRowsDataSet typedRowsDataSet, string NameField, int row)
        {
            var typedDataRow = typedRowsDataSet.Rows[row];
            return typedDataRow.GetColumnValue(GetColumnId(typedRowsDataSet, NameField));
        }
        public static bool Copy(Result<QueryResult> queryResult, DataConfig dataConfig, string icsTelecomProjectDir, ILogger logger)
        {
            bool isSuccessCopyStations = false;
            try
            {
                var dataset = queryResult.Data.Dataset as TypedRowsDataSet;
                if (dataset.RowCount > 0)
                {
                    var stationIds = new List<int>();
                    var dicStationNames = new Dictionary<string, int>();
                    var ewxFile = new EwxData();
                    ewxFile.Header = new Header();

                    var lstStations = new List<Station>();
                    for (int i = 0; i < dataset.RowCount; i++)
                    {
                        var id = GetValue(dataset, "ID", i);
                        if (id != null)
                        {
                            var idVal = (id as IntegerColumnValue).Value.Value;
                            if (stationIds.Contains(idVal))
                            {
                                continue;
                            }
                            stationIds.Add(idVal);
                        }

                        //ewxFile.Stations[i] = new Station();
                        var station = new Station();

                        var address = GetValue(dataset, "Position.ADDRESS", i);
                        if (address != null)
                        {
                            station.Address = (address as StringColumnValue).Value;
                        }

                        var Altitude = GetValue(dataset, "Position.ASL", i);
                        if (Altitude != null)
                        {
                            if ((Altitude as DoubleColumnValue).Value != null)
                            {
                                station.Altitude = (Altitude as DoubleColumnValue).Value.Value;
                            }
                        }

                        var Azimuth = GetValue(dataset, "AZIMUTH", i);
                        if (Azimuth != null)
                        {
                            if ((Azimuth as DoubleColumnValue).Value != null)
                            {
                                station.Azimuth = (Azimuth as DoubleColumnValue).Value.Value;
                            }
                        }

                        var Bandwidth = GetValue(dataset, "BW", i);
                        if (Bandwidth != null)
                        {
                            if ((Bandwidth as DoubleColumnValue).Value != null)
                            {
                                station.Bandwidth = (Bandwidth as DoubleColumnValue).Value.Value;
                            }
                        }

                        var BandwidthRx = GetValue(dataset, "BW", i);
                        if (BandwidthRx != null)
                        {
                            if ((BandwidthRx as DoubleColumnValue).Value != null)
                            {
                                station.BandwidthRx = (BandwidthRx as DoubleColumnValue).Value.Value;
                            }
                        }

                        var CallSign = GetValue(dataset, "NAME", i);
                        if (CallSign != null)
                        {
                            station.CallSign = (CallSign as StringColumnValue).Value;
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

                        var CoordX = GetValue(dataset, "Position.LONGITUDE", i);
                        if (CoordX != null)
                        {
                            if ((CoordX as DoubleColumnValue).Value != null)
                            {
                                station.CoordX = (CoordX as DoubleColumnValue).Value.Value;
                            }
                        }

                        var CoordY = GetValue(dataset, "Position.LATITUDE", i);
                        if (CoordY != null)
                        {
                            if ((CoordY as DoubleColumnValue).Value != null)
                            {
                                station.CoordY = (CoordY as DoubleColumnValue).Value.Value;
                            }
                        }

                        var DiagH = GetValue(dataset, "Antenna.DIAGH", i);
                        if (DiagH != null)
                        {
                            station.DiagH = (DiagH as StringColumnValue).Value;
                            station.DiagH = station.DiagH;
                        }

                        var DiagV = GetValue(dataset, "Antenna.DIAGV", i);
                        if (DiagV != null)
                        {
                            station.DiagV = (DiagV as StringColumnValue).Value;
                            station.DiagV = station.DiagV;
                        }

                        //var Frequency = GetValue(dataset, "TX_HIGH_FREQ", i);
                        var Frequency = GetValue(dataset, "AssignedFrequencies.TX_FREQ", i);
                        if (Frequency != null)
                        {
                            if ((Frequency as DoubleColumnValue).Value != null)
                            {
                                station.Frequency = (Frequency as DoubleColumnValue).Value.Value;
                            }
                        }

                        var Gain = GetValue(dataset, "GAIN", i);
                        if (Gain != null)
                        {
                            if ((Gain as DoubleColumnValue).Value != null)
                            {
                                station.Gain = (Gain as DoubleColumnValue).Value.Value;
                            }
                        }

                        var GainRx = GetValue(dataset, "GAIN", i);
                        if (GainRx != null)
                        {
                            if ((GainRx as DoubleColumnValue).Value != null)
                            {
                                station.GainRx = (GainRx as DoubleColumnValue).Value.Value;
                            }
                        }

                        var HAntenna = GetValue(dataset, "AGL", i);
                        if (HAntenna != null)
                        {
                            if ((HAntenna as DoubleColumnValue).Value != null)
                            {
                                station.HAntenna = (HAntenna as DoubleColumnValue).Value.Value;
                            }
                        }

                        var Losses = GetValue(dataset, "TX_LOSSES", i);
                        if (Losses != null)
                        {
                            if ((Losses as DoubleColumnValue).Value != null)
                            {
                                station.Losses = (Losses as DoubleColumnValue).Value.Value;
                            }
                        }

                        var LossesRx = GetValue(dataset, "TX_LOSSES", i);
                        if (LossesRx != null)
                        {
                            if ((LossesRx as DoubleColumnValue).Value != null)
                            {
                                station.LossesRx = (LossesRx as DoubleColumnValue).Value.Value;
                            }
                        }

                        var tilt = GetValue(dataset, "ELEVATION", i);
                        if (tilt != null)
                        {
                            if ((LossesRx as DoubleColumnValue).Value != null)
                            {
                                station.Tilt = (tilt as DoubleColumnValue).Value.Value;
                            }
                        }

                        var NetId = GetValue(dataset, "Licence.ID", i);
                        if (NetId != null)
                        {
                            if ((NetId as IntegerColumnValue).Value != null)
                            {
                                station.NetId = (NetId as IntegerColumnValue).Value.Value;
                            }
                        }

                        var NominalPower = GetValue(dataset, "PWR_ANT", i);
                        if (NominalPower != null)
                        {
                            if ((NominalPower as DoubleColumnValue).Value != null)
                            {
                                station.NominalPower = (NominalPower as DoubleColumnValue).Value.Value;
                                station.NominalPower = Math.Round(Math.Pow(10, station.NominalPower / 10), 7);
                            }
                        }

                        var Polar = GetValue(dataset, "Antenna.POLARIZATION", i);
                        if (Polar != null)
                        {
                            station.Polar = (Polar as StringColumnValue).Value;
                        }


                        var PolarRx = GetValue(dataset, "Antenna.POLARIZATION", i);
                        if (PolarRx != null)
                        {
                            station.PolarRx = (PolarRx as StringColumnValue).Value;
                        }

                        var D_cx1 = GetValue(dataset, "AssignedFrequencies.TX_FREQ", i);
                        if (D_cx1 != null)
                        {
                            station.D_cx1 = (D_cx1 as DoubleColumnValue).Value.Value;
                        }

                        var U_cx1 = GetValue(dataset, "AssignedFrequencies.RX_FREQ", i);
                        if (U_cx1 != null)
                        {
                            station.U_cx1 = (U_cx1 as DoubleColumnValue).Value.Value;
                        }
                        lstStations.Add(station);
                    }
                    ewxFile.Header.CountStation = lstStations.Count;
                    ewxFile.Stations = lstStations.ToArray();

                    var ewxFileCheck = icsTelecomProjectDir + @"\" + dataConfig.DirectoryConfig.NameEwxFile;
                    if (System.IO.File.Exists(ewxFileCheck))
                    {
                        System.IO.File.Delete(ewxFileCheck);
                    }
                    isSuccessCopyStations = CreateFileEwx.CreateFile(ewxFileCheck, ewxFile, logger);
                }
            }
            catch (Exception e)
            {
                logger.Exception(Contexts.CalcCoverages, e);
            }
            return isSuccessCopyStations;
        }
    }
}
