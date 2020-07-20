using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows;
using Atdi.Modules.CalcServer.BrificDataLayer;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;


namespace Atdi.Modules.CalcServer.BrificDataLayer
{

    /// <summary>
    /// Класс для работы с БД
    /// </summary>
    public class DbSqLite
    {
        /// <summary>
        /// Возвращает IDbConnection
        /// </summary>
        /// <returns>IDbConnection</returns>
        public static IDbConnection GetConnection()
        {
            return new SQLiteConnection(GetDbConnectionString());
        }
        /// <summary>
        /// Возвращает строку подключения
        /// </summary>
        /// <returns></returns>
        private static string GetDbConnectionString()
        {
            return string.Format("data source={0};New=True;UseUTF16Encoding=True", DatabaseFileName);
        }

        public static void SetBrificDirectory(string fileName)
        {
            DatabaseFileName = fileName;
        }

        /// <summary>
        /// Имя файла БД
        /// </summary>
        private static string DatabaseFileName = "";

        /// <summary>
        /// Конструктор
        /// </summary>
        protected DbSqLite()
        {
        }
        /// <summary>
        /// инициализация БД
        /// </summary>
        /// <returns></returns>
        public static bool InitDatabase()
        {
            if (System.IO.File.Exists(DatabaseFileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// Системы радиовещательной службы
        /// DVB-T 
        /// 
        public static FmtvTerra[] LoadBroadcastingService_TDAB(string adm, double freq)
        {
            return ExecSqlFmtvTerra($@"SELECT long_dec,lat_dec,adm,is_digital,stn_cls,freq_assgn,stn_cls FROM fmtv_terra where (is_digital = 'TRUE' and stn_cls ='BC' and adm='{adm}' and freq_assgn={freq})");
        }

        /// <summary>
        /// Системы радиовещательной службы
        /// T-DAB 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static FmtvTerra[] LoadBroadcastingService_DVBT(string adm, double freq)
        {
            return ExecSqlFmtvTerra($@"SELECT long_dec,lat_dec,adm,is_digital,stn_cls,freq_assgn,stn_cls FROM fmtv_terra where (is_digital = 'TRUE' and stn_cls ='BT' and adm='{adm}' and freq_assgn={freq})");
        }

        //
        /// Системы радиовещательной службы
        /// Аналоговое ТВ 
        /// 
        public static FmtvTerra[] LoadBroadcastingServiceAnalog_TV(string adm, double freq)
        {
            return ExecSqlFmtvTerra($@"SELECT long_dec,lat_dec,adm,is_digital,stn_cls,freq_assgn,stn_cls FROM fmtv_terra where (is_digital = 'FALSE' and stn_cls ='BT' and adm='{adm}' and freq_assgn={freq})");
        }





        

        ///	Системы подвижной службы 
        ///	NV
        public static FmtvTerra[] LoadBroadcastingService_NV(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'NV' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	NR
        public static FmtvTerra[] LoadBroadcastingService_NR(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'NR' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	NS
        public static FmtvTerra[] LoadBroadcastingService_NS(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'NS' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	NT
        public static FmtvTerra[] LoadBroadcastingService_NT(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'NT' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	NA
        public static FmtvTerra[] LoadBroadcastingService_NA(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'NA' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	NB
        public static FmtvTerra[] LoadBroadcastingService_NB(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'NB' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	XN
        public static FmtvTerra[] LoadBroadcastingService_XN(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'XN' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	YN
        public static FmtvTerra[] LoadBroadcastingService_YN(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'YN' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы подвижной службы 
        ///	ZC
        public static FmtvTerra[] LoadBroadcastingService_ZC(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'ZC' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }







        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	XG
        public static FmtvTerra[] LoadNavigationServices_XG(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'XG' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	AB
        public static FmtvTerra[] LoadNavigationServices_AB(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'AB' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	AA8
        public static FmtvTerra[] LoadNavigationServices_AA8(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'AA8' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	BD
        public static FmtvTerra[] LoadNavigationServices_BD(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'BD' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	BA
        public static FmtvTerra[] LoadNavigationServices_BA(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'BA' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }







        ///	Системы фиксированной службы
        ///	FF
        public static FmtvTerra[] LoadFixedServices_FF(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'FF' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }


        ///	Системы фиксированной службы
        ///	FN
        public static FmtvTerra[] LoadFixedServices_FN(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'FN' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        ///	Системы фиксированной службы
        ///	FK
        public static FmtvTerra[] LoadFixedServices_FK(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'FK' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }








        //
        /// Системы подвижной службы
        /// MU
        /// 
        public static FmtvTerra[] LoadMobileServices_MU(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'MU' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        //
        /// Системы подвижной службы
        /// M1
        /// 
        public static FmtvTerra[] LoadMobileServices_M1(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type in ('M1') and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }


        //
        /// Системы подвижной службы
        /// RA
        /// 
        public static FmtvTerra[] LoadMobileServices_RA(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type in ('RA') and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }


        //
        /// Системы подвижной службы
        /// M2
        /// 
        public static FmtvTerra[] LoadMobileServices_M2(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'M2' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        //
        /// Системы подвижной службы
        /// XA
        /// 
        public static FmtvTerra[] LoadMobileServices_XA(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'XA' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        //
        /// Системы подвижной службы
        /// XM
        /// 
        public static FmtvTerra[] LoadMobileServices_XM(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'XM' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        //
        /// Системы подвижной службы
        /// MA
        /// 
        public static FmtvTerra[] LoadMobileServices_MA(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'MA' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }

        //
        /// Системы подвижной службы
        /// MT
        /// 
        public static FmtvTerra[] LoadMobileServices_MT(string adm, double minFreq_MHz, double maxFreq_MHz, string staClass)
        {
            return ExecSqlFmtvTerra($@"select distinct g.long_dec, g.lat_dec, a.adm, 'TRUE', a.stn_cls, a.freq_assgn, b.system_type from fxm_terra a  join fxm_system_type b on b.terrakey = a.terrakey LEFT OUTER JOIN fxm_geo_pt g on g.geo_key = a.geo_key where b.system_type = 'MT' and a.adm = '{adm}' and a.freq_assgn >= {minFreq_MHz} and a.freq_assgn <= {maxFreq_MHz} and a.stn_cls = '{staClass}'");
        }



        public static FmtvTerra[] ExecSqlFmtvTerra(string sql)
        {
            var obj = new List<FmtvTerra>();
            ConnectionState previousConnectionState = ConnectionState.Closed;
            using (IDbConnection connect = DbSqLite.GetConnection())
            {
                try
                {
                    //проверяем предыдущее состояние
                    previousConnectionState = connect.State;
                    if (connect.State == ConnectionState.Closed)
                        connect.Open();



                    using (IDbCommand cmd = connect.CreateCommand())
                    {
                        cmd.CommandText = string.Format(sql);
                        using (IDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if ((reader.IsDBNull(0)) && (reader.IsDBNull(1)))
                                {
                                    obj.Add(new FmtvTerra()
                                    {
                                        Administration = reader.GetString(2),
                                        IsDigital = reader.GetString(3),
                                        StnClass = reader.GetString(4),
                                        FreqAssgn_MHz = reader.GetDouble(5),
                                        System_type = reader.GetString(6)
                                    });
                                }
                                else
                                {
                                    obj.Add(new FmtvTerra()
                                    {
                                        Longitude_dec = reader.GetDouble(0),
                                        Latitude_dec = reader.GetDouble(1),
                                        Administration = reader.GetString(2),
                                        IsDigital = reader.GetString(3),
                                        StnClass = reader.GetString(4),
                                        FreqAssgn_MHz = reader.GetDouble(5),
                                        System_type = reader.GetString(6)
                                    });
                                }
                            }
                            reader.Close();
                        }

                    }

                }
                catch (Exception ex)
                {
                    obj = null;
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
                finally
                {
                    //закрываем соединение, если оно было закрыто перед открытием
                    if (previousConnectionState == ConnectionState.Closed)
                    {
                        connect.Close();
                    }
                }
            }
            return obj.ToArray();
        }

    }
}
