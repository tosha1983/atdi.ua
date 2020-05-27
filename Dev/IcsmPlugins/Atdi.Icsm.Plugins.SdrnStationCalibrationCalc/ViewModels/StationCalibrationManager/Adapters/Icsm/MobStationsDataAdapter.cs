using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using Atdi.DataModels.Sdrn.Infocenter.Entities.SdrnServer;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager;
using System.Globalization;
using OrmCs;
using ICSM;


namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters
{
    public class MobStationsDataAdapter : DataAdapter<YMobStationT, IcsmMobStation, MobStationsDataAdapter>
    {
        public string Standard;
        public string StatusForActiveStation;
        public string StatusForNotActiveStation;
        public int? IdentifierStation;
        public string TableName;
        public AreaModel AreaModel;
        public int DistanceAroundContour_km;

        public string[] GetStandards(string status)
        {
            return status.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool CheckHitting(DataLocationModel[] poligon, IcsmMobStationSite site)
        {
            if (poligon == null || poligon.Length == 0)
                return false;


            bool hit = false; // количество пересечений луча слева в право четное = false, нечетное = true;
            for (int i = 0; i < poligon.Length - 1; i++)
            {
                if (((poligon[i].Latitude <= site.Latitude_DEC) && ((poligon[i + 1].Latitude > site.Latitude_DEC))) || ((poligon[i].Latitude > site.Latitude_DEC) && ((poligon[i + 1].Latitude <= site.Latitude_DEC))))
                {
                    if ((poligon[i].Longitude > site.Longitude_DEC) && (poligon[i + 1].Longitude > site.Longitude_DEC))
                    {
                        hit = !hit;
                    }
                    else if (!((poligon[i].Longitude < site.Longitude_DEC) && (poligon[i + 1].Longitude < site.Longitude_DEC)))
                    {
                        if (site.Longitude_DEC < poligon[i + 1].Longitude - (site.Latitude_DEC - poligon[i + 1].Latitude) * (poligon[i + 1].Longitude - poligon[i].Longitude) / (poligon[i].Latitude - poligon[i + 1].Latitude))
                        {
                            hit = !hit;
                        }
                    }
                }
            }
            int i_ = poligon.Length - 1;
            if (((poligon[i_].Latitude <= site.Latitude_DEC) && ((poligon[0].Latitude > site.Latitude_DEC))) || ((poligon[i_].Latitude > site.Latitude_DEC) && ((poligon[0].Latitude <= site.Latitude_DEC))))
            {
                if ((poligon[i_].Longitude > site.Longitude_DEC) && (poligon[0].Longitude > site.Longitude_DEC))
                {
                    hit = !hit;
                }
                else if (!((poligon[i_].Longitude < site.Longitude_DEC) && (poligon[0].Longitude < site.Longitude_DEC)))
                {
                    if (site.Longitude_DEC < poligon[0].Longitude - (site.Latitude_DEC - poligon[0].Latitude) * (poligon[0].Longitude - poligon[i_].Longitude) / (poligon[i_].Latitude - poligon[0].Latitude))
                    {
                        hit = !hit;
                    }
                }
            }

            return hit;
        }

        public const double re = 6371;
        public static double GetDistance_km(double x1, double y1, double x2, double y2)
        {
            double d = 0;
            double dlon = x2 - x1;
            double r = Math.Sin(y1 * Math.PI / 180) * Math.Sin(y2 * Math.PI / 180) + Math.Cos(y1 * Math.PI / 180) * Math.Cos(y2 * Math.PI / 180) * Math.Cos(dlon * Math.PI / 180);
            double angle = 180 * Math.Acos(r) / Math.PI;
            d = angle * re;
            return d;
        }

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

        public void Refresh()
        {

            var icsmMobStation = new IcsmMobStation();
            //icsmMobStation.CallSign
            //icsmMobStation.CreatedDate
            //icsmMobStation.ExternalCode
            //icsmMobStation.ExternalSource

            if ((string.IsNullOrEmpty(StatusForActiveStation)) || (string.IsNullOrEmpty(StatusForNotActiveStation)))
            {
                throw new Exception();
            }
            if (IdentifierStation != null)
            {
                if (string.IsNullOrEmpty(TableName))
                {
                    throw new Exception();
                }
            }

            var activeStationStatuses = GetStandards(StatusForActiveStation);
            var notActiveStationStatuses = GetStandards(StatusForNotActiveStation);


            var listMobStationT = new List<YMobStationT>();

            if (IdentifierStation != null)
            {
                IMRecordset rs = new IMRecordset(TableName, IMRecordset.Mode.ReadOnly);
                rs.Select("ID,STANDARD,STATUS,CALL_SIGN,Position.LATITUDE,Position.LONGITUDE,Position.ASL,NAME,Owner.CODE,DATE_MODIFIED,DATE_CREATED");
                rs.SetWhere("ID", IMRecordset.Operation.Eq, IdentifierStation.Value);
                for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                {
                    // если статус очередной станции не найден в параметрах для поиска, которые задал клиент тогда пропускаем станцию
                    if (((activeStationStatuses.Contains(rs.GetS("STATUS"))) || (notActiveStationStatuses.Contains(rs.GetS("STATUS"))))==false)
                    {
                        continue;
                    }

                    var mobStationT = new YMobStationT();
                    mobStationT.m_call_sign = rs.GetS("CALL_SIGN");
                    mobStationT.m_standard = rs.GetS("STANDARD");
                    mobStationT.m_status = rs.GetS("STATUS");
                    mobStationT.m_id = rs.GetI("ID");
                    mobStationT.m_date_modified = rs.GetT("DATE_MODIFIED");
                    mobStationT.m_date_created = rs.GetT("DATE_CREATED");


                    IcsmMobStationSite icsmMobStationSite = new IcsmMobStationSite()
                    {
                        Longitude_DEC = rs.GetD("Position.LONGITUDE"),
                        Latitude_DEC = rs.GetD("Position.LATITUDE"),
                        Altitude_m = rs.GetD("Position.ASL")
                    };

                    // если станция попадает в контур, тогда выставляем для нее статус P
                    if (CheckHitting(AreaModel.Location, icsmMobStationSite))
                    {
                        mobStationT.m_status = MobStationStatus.P.ToString();
                    }
                    else
                    {
                        // здесь вычисляем расстояние станции до точек контура. Если найдено расстояние меньше чем параметр DistanceAroundContour, тогда выставляем статус P
                        bool isFindPositionWithDistanceAroundContour = false;
                        for (int i = 0; i < AreaModel.Location.Length - 1; i++)
                        {
                            var loc = AreaModel.Location[i];
                            if (GetDistance_km(loc.Longitude, loc.Latitude, icsmMobStationSite.Longitude_DEC, icsmMobStationSite.Latitude_DEC) < DistanceAroundContour_km)
                            {
                                isFindPositionWithDistanceAroundContour = true;
                                break;
                            }
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

                    // 1. Генерация GSID
                    mobStationT.m_cust_txt1 = GetGlobalSID(rs.GetS("Owner.CODE"), rs.GetS("NAME"));

                    // 2. Проверка - станция должна отправляться один раз (дуликатов быть не должно)
                    var fndStation = listMobStationT.Find(x => x.m_id == mobStationT.m_id);
                    if (fndStation == null)
                    {
                        listMobStationT.Add(mobStationT);
                    }
                }
                if (rs.IsOpen())
                    rs.Close();
                rs.Destroy();
            }
            else
            {
                //rs.SetWhere("ID", IMRecordset.Operation.Eq, IdentifierStation.Value);
                //rs.SetWhere("STANDARD", IMRecordset.Operation.Eq, Standard);
            }
            this.Source = listMobStationT.ToArray();
        }

        /// <summary>
        /// Сопоставление GSID в станции и из IGlobalIdentity
        /// если обнаружено соответствие - тогда производим обновление из поля RealGsid
        /// </summary>
        /// <param name="licenseGsid"></param>
        /// <returns></returns>
        private string GetRealGsid(string licenseGsid)
        {

            return licenseGsid;
        }

        protected override Func<YMobStationT, IcsmMobStation> GetMapper()
        {
            return source => new IcsmMobStation
            {
                CallSign = source.m_call_sign,
                ExternalCode = source.m_id.ToString(),
                ExternalSource = source.m_table_name,
                Standard = source.m_standard,
                StateName = source.m_status,
                Name = source.m_name,
                LicenseGsid = source.m_cust_txt1,
                RealGsid = GetRealGsid(source.m_cust_txt1),
                ModifiedDate = source.m_date_modified,
                CreatedDate = source.m_date_created,
                RegionCode = AreaModel.IdentifierFromICSM.ToString()
            };
        }
        
    }
}
