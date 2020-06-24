using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using System.Globalization;
using OrmCs;
using ICSM;
using Atdi.Platform.Cqrs;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters;
using Atdi.Contracts.Sdrn.DeepServices.RadioSystem;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.AntennaPattern;
using Atdi.DataModels.Sdrn.DeepServices;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries
{
    public class MobStationsDataAdapter  
    {

        private readonly ILogger _logger;
        public readonly ISignalService _signalService; 
        public readonly IObjectReader _objectReader;
        public readonly MobStationsLoadModelByParams  _mobStationsLoadModelByParams;

        public MobStationsDataAdapter(MobStationsLoadModelByParams  mobStationsLoadModelByParams,
            ILogger logger,
            ISignalService signalService,
            IObjectReader objectReader)
        {
            this._logger = logger;
            this._signalService = signalService;
            this._objectReader = objectReader;
            this._mobStationsLoadModelByParams =  mobStationsLoadModelByParams;
        }


        public enum TypeAntennaPattern
        {
            HH = 0,
            HV = 1,
            VH = 2,
            VV = 3
        }

        public enum PolarizationCode
        {
            /// <summary>
            /// Unknown
            /// </summary>
            U = 0,

            /// <summary>
            /// Vertical
            /// </summary>
            V = 1,

            /// <summary>
            /// Horizontal
            /// </summary>
            H = 2,

            /// <summary>
            /// CL
            /// </summary>
            CL = 3,

            /// <summary>
            /// RL
            /// </summary>
            RL = 4,

            /// <summary>
            /// M
            /// </summary>
            M = 5
        }

        public enum AntennaItuPattern
        {
            None = 0,
            ITU465 = 1,
            ITU580 = 2,
            ITU699 = 3,
            ITU1213 = 4,
            ITU1245 = 5,
            ITU1428 = 6
        }

        public class PointObject
        {
            public double Angle { get; set; }
            public float Loss { get; set; }
        }


        public string[] GetStandards(string status)
        {
            return status.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool CheckHitting(DataLocationModel[] poligon, YPosition site)
        {
            if (poligon == null || poligon.Length == 0)
                return false;


            bool hit = false; // количество пересечений луча слева в право четное = false, нечетное = true;
            for (int i = 0; i < poligon.Length - 1; i++)
            {
                if (((poligon[i].Latitude <= site.m_latitude) && ((poligon[i + 1].Latitude > site.m_latitude))) || ((poligon[i].Latitude > site.m_latitude) && ((poligon[i + 1].Latitude <= site.m_latitude))))
                {
                    if ((poligon[i].Longitude > site.m_longitude) && (poligon[i + 1].Longitude > site.m_longitude))
                    {
                        hit = !hit;
                    }
                    else if (!((poligon[i].Longitude < site.m_longitude) && (poligon[i + 1].Longitude < site.m_longitude)))
                    {
                        if (site.m_longitude < poligon[i + 1].Longitude - (site.m_latitude - poligon[i + 1].Latitude) * (poligon[i + 1].Longitude - poligon[i].Longitude) / (poligon[i].Latitude - poligon[i + 1].Latitude))
                        {
                            hit = !hit;
                        }
                    }
                }
            }
            int i_ = poligon.Length - 1;
            if (((poligon[i_].Latitude <= site.m_latitude) && ((poligon[0].Latitude > site.m_latitude))) || ((poligon[i_].Latitude > site.m_latitude) && ((poligon[0].Latitude <= site.m_latitude))))
            {
                if ((poligon[i_].Longitude > site.m_longitude) && (poligon[0].Longitude > site.m_longitude))
                {
                    hit = !hit;
                }
                else if (!((poligon[i_].Longitude < site.m_longitude) && (poligon[0].Longitude < site.m_longitude)))
                {
                    if (site.m_longitude < poligon[0].Longitude - (site.m_latitude - poligon[0].Latitude) * (poligon[0].Longitude - poligon[i_].Longitude) / (poligon[i_].Latitude - poligon[0].Latitude))
                    {
                        hit = !hit;
                    }
                }
            }

            return hit;
        }

        //public const double re = 6371;
        //public static double GetDistance_km(double x1, double y1, double x2, double y2)
        //{
        //    double d = 0;
        //    double dlon = x2 - x1;
        //    double r = Math.Sin(y1 * Math.PI / 180) * Math.Sin(y2 * Math.PI / 180) + Math.Cos(y1 * Math.PI / 180) * Math.Cos(y2 * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
        //    double angle = 180 * Math.Acos(r) / Math.PI;
        //    d = angle * re;
        //    return d;
        //}

        public static string GetGlobalSID(string okpo, string stationName)
        {
            if (!string.IsNullOrEmpty(stationName))
            {
                string CodeOwener = "0";
                if ((okpo == "14333937") || (okpo == "35862001")) { CodeOwener = "1"; };
                if (okpo == "22859846") { CodeOwener = "6"; };
                if (okpo == "21673832") { CodeOwener = "3"; };
                if (okpo == "37815221") { CodeOwener = "7"; };
                return "255 " + CodeOwener + " 00000 " + string.Format("{0:00000}", stationName);
            }
            else return "";
        }

        public IcsmMobStation[] LoadStations()
        {
            var arrayTables = new string[] { "MOB_STATION", "MOB_STATION2" };
            string mobstafreq_table = "MOBSTA_FREQS";
            string mobstafreq_table2 = "MOBSTA_FREQS2";
            var listIcsmMobStation = new List<IcsmMobStation>();



            if ((this._mobStationsLoadModelByParams.AreaModel == null) || ((this._mobStationsLoadModelByParams.AreaModel != null) && (this._mobStationsLoadModelByParams.AreaModel.Length == 0)))
            {
                throw new Exception("'Areas' is null or contains 0 elements");
            }
            if (((this._mobStationsLoadModelByParams.IdentifierStation != null) && (this._mobStationsLoadModelByParams.IdentifierStation != 0)) && (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.OneStation))
            {
                if (string.IsNullOrEmpty(this._mobStationsLoadModelByParams.TableName))
                {
                    throw new Exception("'TableName' is null or empty");
                }
            }

            string[] activeStationStatuses = null;
            string[] notActiveStationStatuses = null;
            if (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.MultyStations)
            {
                activeStationStatuses = GetStandards(this._mobStationsLoadModelByParams.StatusForActiveStation);
                notActiveStationStatuses = GetStandards(this._mobStationsLoadModelByParams.StatusForNotActiveStation);
            }

            var listMobStationT = new List<int>();

            for (int v = 0; v < arrayTables.Length; v++)
            {
                if ((((this._mobStationsLoadModelByParams.IdentifierStation != null) && (this._mobStationsLoadModelByParams.IdentifierStation != 0)) && (!string.IsNullOrEmpty(this._mobStationsLoadModelByParams.TableName)) && (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.OneStation)))
                {
                    if (arrayTables[v] != this._mobStationsLoadModelByParams.TableName)
                    {
                        continue;
                    }
                }

                for (int z = 0; z < this._mobStationsLoadModelByParams.AreaModel.Length; z++)
                {
                    var rs = new IMRecordset(arrayTables[v], IMRecordset.Mode.ReadOnly);
                    rs.Select("ID,AGL,AZIMUTH,STANDARD,STATUS,CALL_SIGN,Position.LATITUDE,Position.LONGITUDE,NAME,Owner.REGIST_NUM,DATE_MODIFIED,DATE_CREATED,BW,RX_LOSSES,TX_LOSSES,PWR_ANT,Equipment.KTBF,Equipment.RXTH_6,Antenna.POLARIZATION,GAIN,Antenna.DIAGV,Antenna.DIAGH,Antenna.DIAGA,ELEVATION,Antenna.XPD,Position.City.PROVINCE,Equipment.SENSITIVITY");
                    if (((this._mobStationsLoadModelByParams.IdentifierStation != null) && (this._mobStationsLoadModelByParams.IdentifierStation != 0) && (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.OneStation)))
                    {
                        rs.SetWhere("ID", IMRecordset.Operation.Eq, this._mobStationsLoadModelByParams.IdentifierStation.Value);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(this._mobStationsLoadModelByParams.Standard))
                        {
                            throw new Exception("'Standard' is null or empty");
                        }
                        rs.SetWhere("STANDARD", IMRecordset.Operation.Eq, this._mobStationsLoadModelByParams.Standard);
                    }

                    rs.SetWhere("Position.LATITUDE", IMRecordset.Operation.Le, this._mobStationsLoadModelByParams.AreaModel[z].ExternalContour[1].Latitude);
                    rs.SetWhere("Position.LATITUDE", IMRecordset.Operation.Ge, this._mobStationsLoadModelByParams.AreaModel[z].ExternalContour[2].Latitude);

                    rs.SetWhere("Position.LONGITUDE", IMRecordset.Operation.Ge, this._mobStationsLoadModelByParams.AreaModel[z].ExternalContour[0].Longitude);
                    rs.SetWhere("Position.LONGITUDE", IMRecordset.Operation.Le, this._mobStationsLoadModelByParams.AreaModel[z].ExternalContour[1].Longitude);


                    if (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.MultyStations)
                    {
                        var allStatuses = new List<string>();
                        var newactiveStationStatuses = new string[activeStationStatuses.Length];
                        var newnotActiveStationStatuses = new string[notActiveStationStatuses.Length];
                        for (int s = 0; s < activeStationStatuses.Length; s++)
                        {
                            newactiveStationStatuses[s] = "'" + activeStationStatuses[s] + "'";
                        }
                        for (int s = 0; s < notActiveStationStatuses.Length; s++)
                        {
                            newnotActiveStationStatuses[s] = "'" + notActiveStationStatuses[s] + "'";
                        }
                        allStatuses.AddRange(newactiveStationStatuses);
                        allStatuses.AddRange(newnotActiveStationStatuses);
                        var filter = string.Format("[STATUS] in ({0})", string.Join(",", allStatuses.ToArray()));
                        rs.SetAdditional(filter);
                    }
                    for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                    {
                        // если статус очередной станции не найден в параметрах для поиска, которые задал клиент тогда пропускаем станцию
                        if (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.MultyStations)
                        {
                            if (((activeStationStatuses.Contains(rs.GetS("STATUS"))) || (notActiveStationStatuses.Contains(rs.GetS("STATUS")))) == false)
                            {
                                continue;
                            }
                        }

                        var mobStationT = new YMobStationT();
                        mobStationT.Format("*,Position(*),AssignedFrequencies(*),Equipment(*),Antenna(*)");
                        mobStationT.m_call_sign = rs.GetS("CALL_SIGN");
                        mobStationT.m_standard = rs.GetS("STANDARD");
                        mobStationT.m_status = rs.GetS("STATUS");
                        mobStationT.m_id = rs.GetI("ID");
                        mobStationT.m_date_modified = rs.GetT("DATE_MODIFIED");
                        mobStationT.m_date_created = rs.GetT("DATE_CREATED");

                        mobStationT.m_Position = new YPosition();
                        mobStationT.m_Position.Format("*");
                        mobStationT.m_Position.m_longitude = rs.GetD("Position.LONGITUDE");
                        mobStationT.m_Position.m_latitude = rs.GetD("Position.LATITUDE");
                        mobStationT.m_agl = rs.GetD("AGL");

                        // Генерация GSID
                        mobStationT.m_cust_txt1 = GetGlobalSID(rs.GetS("Owner.REGIST_NUM"), rs.GetS("NAME"));
                        mobStationT.m_call_sign = rs.GetS("CALL_SIGN");
                        mobStationT.m_name = rs.GetS("NAME");



                        string mobstafreq = "";
                        if (arrayTables[v] == "MOB_STATION")
                        {
                            mobstafreq = mobstafreq_table;
                        }
                        else if (arrayTables[v] == "MOB_STATION2")
                        {
                            mobstafreq = mobstafreq_table2;
                        }


                        List<YMobstaFreqsT> yMobStationTxRx = new List<YMobstaFreqsT>();
                        var rssta = new IMRecordset(mobstafreq, IMRecordset.Mode.ReadOnly);
                        rssta.Select("ID,TX_FREQ,RX_FREQ");
                        rssta.SetWhere("STA_ID", IMRecordset.Operation.Eq, mobStationT.m_id);
                        for (rssta.Open(); !rssta.IsEOF(); rssta.MoveNext())
                        {
                            var txfrq = rssta.GetD("TX_FREQ");
                            if (txfrq != IM.NullD)
                            {
                                var rsTx = new YMobstaFreqsT();
                                rsTx.Format("*");
                                rsTx.Table = mobstafreq;
                                rsTx.m_tx_freq = txfrq;
                                rsTx.m_status = "Tx";

                                yMobStationTxRx.Add(rsTx);
                                rsTx.Close();
                                rsTx.Dispose();

                            }
                            var rxfrq = rssta.GetD("RX_FREQ");
                            if (rxfrq != IM.NullD)
                            {
                                var rsRx = new YMobstaFreqsT();
                                rsRx.Format("*");
                                rsRx.Table = mobstafreq;
                                rsRx.m_rx_freq = rxfrq;
                                rsRx.m_status = "Rx";
                                yMobStationTxRx.Add(rsRx);

                                rsRx.Close();
                                rsRx.Dispose();
                            }
                        }
                        if (rssta.IsOpen())
                            rssta.Close();
                        rssta.Destroy();



                        mobStationT.Tag = yMobStationTxRx;


                        var txFreqList = yMobStationTxRx.Where(x => x.m_status == "Tx");
                        var rxFreqList = yMobStationTxRx.Where(x => x.m_status == "Rx");

                        if (txFreqList != null)
                        {
                            var selectFreqTx = txFreqList.Select(x => x.m_tx_freq);
                            if ((selectFreqTx != null) && (selectFreqTx.Count() > 0))
                            {
                                mobStationT.m_tx_low_freq = selectFreqTx.Min();
                            }
                        }
                        if (rxFreqList != null)
                        {
                            var selectFreqRx = rxFreqList.Select(x => x.m_rx_freq);
                            if ((selectFreqRx != null) && (selectFreqRx.Count() > 0))
                            {
                                mobStationT.m_rx_low_freq = selectFreqRx.Min();
                            }
                        }


                        mobStationT.m_bw = rs.GetD("BW");
                        mobStationT.m_rx_losses = rs.GetD("RX_LOSSES");
                        mobStationT.m_tx_losses = rs.GetD("TX_LOSSES");
                        mobStationT.m_power = rs.GetD("PWR_ANT");
                        mobStationT.m_elevation = rs.GetD("ELEVATION");


                        mobStationT.m_gain = rs.GetD("GAIN");
                        mobStationT.m_azimuth = rs.GetD("AZIMUTH");
                        mobStationT.m_polar = rs.GetS("Antenna.POLARIZATION");

                        mobStationT.m_Equipment = new YEquipt();
                        mobStationT.m_Equipment.Format("*");
                        mobStationT.m_Equipment.m_ktbf = rs.GetD("Equipment.KTBF");
                        //mobStationT.m_Equipment.m_rxth_6 = rs.GetD("Equipment.RXTH_6");
                        mobStationT.m_Equipment.m_sensitivity = rs.GetD("Equipment.SENSITIVITY");



                        mobStationT.m_Antenna = new YAntennat();
                        mobStationT.m_Antenna.Format("*");
                        mobStationT.m_Antenna.m_diagh = rs.GetS("Antenna.DIAGH");
                        mobStationT.m_Antenna.m_diagv = rs.GetS("Antenna.DIAGV");
                        mobStationT.m_Antenna.m_diaga = rs.GetS("Antenna.DIAGA");
                        mobStationT.m_Antenna.m_xpd = rs.GetD("Antenna.XPD");
                        mobStationT.m_rec_area = rs.GetS("Position.City.PROVINCE");
                        mobStationT.m_cust_txt2 = ReadGCIDDataModel(mobStationT.m_cust_txt1, mobStationT.m_rec_area, mobStationT.m_standard);
                        if ((mobStationT.m_cust_txt2 != mobStationT.m_cust_txt1) && !string.IsNullOrEmpty(mobStationT.m_cust_txt2))
                        {
                            mobStationT.m_cust_txt1 = mobStationT.m_cust_txt2;
                        }

                        if (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.MultyStations)
                        {
                            if (notActiveStationStatuses.Contains(rs.GetS("STATUS")))
                            {
                                mobStationT.m_status = MobStationStatus.I.ToString();
                            }
                            else if (activeStationStatuses.Contains(rs.GetS("STATUS")))
                            {
                                for (int w = 0; w < this._mobStationsLoadModelByParams.AreaModel.Length; w++)
                                {
                                    // конвертация в 4DEC
                                    var area = this._mobStationsLoadModelByParams.AreaModel[w];
                                    var listDataLocationModel = new DataLocationModel[area.Location.Length];
                                    for (int j = 0; j < area.Location.Length; j++)
                                    {
                                        listDataLocationModel[j] = new DataLocationModel()
                                        {
                                            Longitude = ICSM.IMPosition.Dms2Dec(area.Location[j].Longitude),
                                            Latitude = ICSM.IMPosition.Dms2Dec(area.Location[j].Latitude)
                                        };
                                    }

                                    // если станция попадает в контур, тогда выставляем для нее статус P
                                    if (CheckHitting(listDataLocationModel, mobStationT.m_Position))
                                    {
                                        mobStationT.m_status = MobStationStatus.A.ToString();
                                    }
                                    else
                                    {
                                        // если станция попадает во "внешний" контур, тогда выставляем для нее статус P (т.е. когда рассточние до границы заданного контура "Location" менее чем DistanceAroundContour_km)
                                        bool isFindPositionWithDistanceAroundContour = false;

                                        if (CheckHitting(this._mobStationsLoadModelByParams.AreaModel[w].ExternalContour, mobStationT.m_Position))
                                        {
                                            isFindPositionWithDistanceAroundContour = true;
                                        }

                                        if (isFindPositionWithDistanceAroundContour)
                                        {
                                            mobStationT.m_status = MobStationStatus.P.ToString();
                                        }
                                        else
                                        {
                                            // для всех остальных случаев выставляем статус I
                                            mobStationT.m_status = MobStationStatus.I.ToString();
                                        }
                                    }
                                }
                            }
                        }
                        else if (this._mobStationsLoadModelByParams.SelectedStationType == SelectedStationType.OneStation)
                        {
                            for (int w = 0; w < this._mobStationsLoadModelByParams.AreaModel.Length; w++)
                            {
                                // конвертация в 4DEC
                                var area = this._mobStationsLoadModelByParams.AreaModel[w];
                                var listDataLocationModel = new DataLocationModel[area.Location.Length];
                                for (int j = 0; j < area.Location.Length; j++)
                                {
                                    listDataLocationModel[j] = new DataLocationModel()
                                    {
                                        Longitude = ICSM.IMPosition.Dms2Dec(area.Location[j].Longitude),
                                        Latitude = ICSM.IMPosition.Dms2Dec(area.Location[j].Latitude)
                                    };
                                }

                                // если станция попадает в контур, тогда выставляем для нее статус P
                                if (CheckHitting(listDataLocationModel, mobStationT.m_Position))
                                {
                                    mobStationT.m_status = MobStationStatus.A.ToString();
                                }
                                else
                                {
                                    // если станция попадает во "внешний" контур, тогда выставляем для нее статус P (т.е. когда рассточние до границы заданного контура "Location" менее чем DistanceAroundContour_km)
                                    bool isFindPositionWithDistanceAroundContour = false;

                                    if (CheckHitting(this._mobStationsLoadModelByParams.AreaModel[w].ExternalContour, mobStationT.m_Position))
                                    {
                                        isFindPositionWithDistanceAroundContour = true;
                                    }

                                    if (isFindPositionWithDistanceAroundContour)
                                    {
                                        mobStationT.m_status = MobStationStatus.P.ToString();
                                    }
                                    else
                                    {
                                        // для всех остальных случаев выставляем статус I
                                        mobStationT.m_status = MobStationStatus.I.ToString();
                                    }
                                }
                            }
                        }

                        //  Проверка - станция должна отправляться один раз (дуликатов быть не должно)
                        if (!listMobStationT.Contains(mobStationT.m_id))
                        {
                            listMobStationT.Add(mobStationT.m_id);
                            var source = mobStationT;


                            double[] TxFreq = null;
                            double[] RxFreq = null;

                            if (source.Tag != null)
                            {
                                var temptxFreqList = ((List<YMobstaFreqsT>)source.Tag).Where(x => x.m_status == "Tx");
                                var temprxFreqList = ((List<YMobstaFreqsT>)source.Tag).Where(x => x.m_status == "Rx");

                                if (temptxFreqList != null)
                                {
                                    var selectFreqTx = temptxFreqList.Select(x => x.m_tx_freq);
                                    if (selectFreqTx != null)
                                    {
                                        TxFreq = selectFreqTx.ToArray();
                                    }

                                }
                                if (temprxFreqList != null)
                                {
                                    var selectFreqRx = temprxFreqList.Select(x => x.m_rx_freq);
                                    if (selectFreqRx != null)
                                    {
                                        RxFreq = selectFreqRx.ToArray();
                                    }
                                }
                            }


                            IcsmMobStationPattern hh_pattern = null;
                            IcsmMobStationPattern hv_pattern = null;
                            IcsmMobStationPattern vh_pattern = null;
                            IcsmMobStationPattern vv_pattern = null;

                            // HH
                            var argsHH = new DiagrammArgs()
                            {
                                 AntennaPatternType = AntennaPatternType.HH,
                                  Gain = source.m_gain,
                                 Points = source.m_Antenna.m_diagh
                            };
                            var diagrammPointsResult = new DiagrammPoint[1000]; 

                            this._signalService.CalcAntennaPattern(in argsHH, ref diagrammPointsResult);
                            if (diagrammPointsResult[0].Angle!=null)
                            {
                                hh_pattern = new IcsmMobStationPattern();
                                hh_pattern.Loss_dB = diagrammPointsResult.Where(x=>x.Loss!=null).Select(c => c.Loss.Value).ToArray();
                                hh_pattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                            };


                            // HV
                            var argsHV = new DiagrammArgs()
                            {
                                AntennaPatternType = AntennaPatternType.HV,
                                Gain = source.m_gain,
                                Points = source.m_Antenna.m_diagh
                            };
                            diagrammPointsResult = new DiagrammPoint[1000];

                            this._signalService.CalcAntennaPattern(in argsHV, ref diagrammPointsResult);
                            if (diagrammPointsResult[0].Angle != null)
                            {
                                hv_pattern = new IcsmMobStationPattern();
                                hv_pattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                hv_pattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                            };

                            // VH
                            var argsVH = new DiagrammArgs()
                            {
                                AntennaPatternType = AntennaPatternType.VH,
                                Gain = source.m_gain,
                                Points = source.m_Antenna.m_diagv
                            };
                            diagrammPointsResult = new DiagrammPoint[1000];

                            this._signalService.CalcAntennaPattern(in argsVH, ref diagrammPointsResult);
                            if (diagrammPointsResult[0].Angle != null)
                            {
                                vh_pattern = new IcsmMobStationPattern();
                                vh_pattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                vh_pattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                            };


                            // VV
                            var argsVV = new DiagrammArgs()
                            {
                                AntennaPatternType = AntennaPatternType.VV,
                                Gain = source.m_gain,
                                Points = source.m_Antenna.m_diagv
                            };
                            diagrammPointsResult = new DiagrammPoint[1000];

                            this._signalService.CalcAntennaPattern(in argsVV, ref diagrammPointsResult);
                            if (diagrammPointsResult[0].Angle != null)
                            {
                                vv_pattern = new IcsmMobStationPattern();
                                vv_pattern.Loss_dB = diagrammPointsResult.Where(x => x.Loss != null).Select(c => c.Loss.Value).ToArray();
                                vv_pattern.Angle_deg = diagrammPointsResult.Where(x => x.Angle != null).Select(c => c.Angle.Value).ToArray();
                            };

                            DateTime? modifiedDate = null;
                            if (source.m_date_modified != IM.NullT)
                            {
                                modifiedDate = source.m_date_modified;
                            }

                            DateTime? createdDate = null;
                            if (source.m_date_created != IM.NullT)
                            {
                                createdDate = source.m_date_created;
                            }

                            listIcsmMobStation.Add(new IcsmMobStation
                            {
                                CallSign = source.m_call_sign,
                                ExternalCode = source.m_id.ToString(),
                                ExternalSource = arrayTables[v],//source.m_table_name,
                                Standard = source.m_standard,
                                StateName = source.m_status,
                                Name = source.m_name,
                                LicenseGsid = source.m_cust_txt1,
                                RealGsid = source.m_cust_txt2,
                                ModifiedDate = modifiedDate,
                                CreatedDate = createdDate == null ? DateTime.Now : createdDate.Value,
                                RegionCode = source.m_rec_area,
                                SITE = new IcsmMobStationSite()
                                {
                                    Altitude_m = source.m_agl,
                                    Longitude_DEC = source.m_Position.m_longitude,
                                    Latitude_DEC = source.m_Position.m_latitude,
                                },
                                ANTENNA = new IcsmMobStationAntenna()
                                {
                                    Gain_dB = (float)source.m_gain,
                                    Azimuth_deg = source.m_azimuth,
                                    Tilt_deg = source.m_elevation,
                                    XPD_dB = (float)source.m_Antenna.m_xpd,
                                    ItuPatternCode = (byte)AntennaItuPattern.None,
                                    ItuPatternName = AntennaItuPattern.None.ToString(),
                                    HH_PATTERN = hh_pattern,
                                    HV_PATTERN = hv_pattern,
                                    VH_PATTERN = vh_pattern,
                                    VV_PATTERN = vv_pattern,
                                },
                                TRANSMITTER = new IcsmMobStationTransmitter()
                                {
                                    BW_kHz = source.m_bw,
                                    Freq_MHz = source.m_tx_low_freq,
                                    Loss_dB = (float)source.m_tx_losses,
                                    MaxPower_dBm = (float)source.m_power + 30,
                                    PolarizationCode = (byte)GetPolarizationCode(source.m_polar),
                                    PolarizationName = GetPolarizationCode(source.m_polar).ToString(),
                                    Freqs_MHz = TxFreq,
                                },
                                RECEIVER = new IcsmMobStationReceiver()
                                {
                                    BW_kHz = source.m_bw,
                                    Freq_MHz = source.m_rx_low_freq,
                                    Loss_dB = (float)source.m_rx_losses,
                                    KTBF_dBm = (float)source.m_Equipment.m_ktbf,
                                    Threshold_dBm = (float)source.m_Equipment.m_sensitivity,
                                    PolarizationCode = (byte)GetPolarizationCode(source.m_polar),
                                    PolarizationName = GetPolarizationCode(source.m_polar).ToString(),
                                    Freqs_MHz = RxFreq
                                }
                            });
                        }

                    }
                    if (rs.IsOpen())
                        rs.Close();
                    rs.Destroy();
                }
            }

            return listIcsmMobStation.ToArray();
        }


        /// <summary>
        /// Сопоставление GSID в станции и из IGlobalIdentity
        /// если обнаружено соответствие - тогда производим обновление из поля RealGsid
        /// </summary>
        /// <param name="licenseGsid"></param>
        /// <param name="regionCode"></param>
        /// <param name="standard"></param>
        /// <returns></returns>
        public string ReadGCIDDataModel(string licenseGsid, string regionCode, string standard)
        {
            var resGCID = this._objectReader
                .Read<GCIDDataModel>()
                .By(new GCIDDataModelByParams()
                {
                    LicenseGsid = licenseGsid,
                    RegionCode = regionCode,
                    Standard = standard
                });
            if (resGCID == null)
            {
                return licenseGsid;
            }
            else
            {
                return resGCID.RealGsid;
            }
        }

        private PolarizationCode GetPolarizationCode(string polar)
        {
            if ((polar == "D") || (polar == "M"))
            {
                return PolarizationCode.M;
            }
            else
            {
                var lstEnum = Enum.GetValues(typeof(PolarizationCode)).Cast<PolarizationCode>().ToList();
                return  lstEnum.Find(x => x.ToString() == polar);
            }
        }
    }
}
