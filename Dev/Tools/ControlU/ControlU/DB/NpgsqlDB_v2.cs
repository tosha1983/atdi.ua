using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NpgsqlTypes;
using System.Windows.Threading;
using System.Windows.Data;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class NpgsqlDB_v2 : PropertyChangedBase// INotifyPropertyChanged
    {
        private static string DefaultToDb = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=123456789;Database=postgres;CommandTimeout=300;";//
        private static string GlobconnToDb = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=10488009;Database=postgres;CommandTimeout=300;";//prog 10488009   out 9846318921  и в CheckServerIsRunnig
        private static string UserconnToDb = "Server=127.0.0.1;Port=5432;User Id=controluuser;Password=xf45hndf65hshn3h;Database=controludb;CommandTimeout=300;";
        //private static string GlobconnToDb = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=10488009;Database=postgres;";// "Server=127.0.0.1;Port=5432;User Id=fmeas;Password=fmeas;Database=fmeas2;";
        //private static string UserconnToDb = "Server=127.0.0.1;Port=5432;User Id=fmeasuser;Password=e445rvatj1jkd;Database=fmeasdb;";
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        Settings.XMLSettings Sett = App.Sett;
        Helpers.Helper h = new Helpers.Helper();
        public delegate void DBTreadDelegate();
        public DBTreadDelegate dbt;
        Thread tr;
        public bool DataCycle
        {
            get { return _DataCycle; }
            set { _DataCycle = value; }
        }
        bool _DataCycle = true;
        public DBTreadDelegate GPSt;
        Thread trGPS;
        bool DataCycleGPS = true;

        public bool _ServerIsRunnig = false;
        public bool ServerIsRunning
        {
            get { return _ServerIsRunnig; }
            set { _ServerIsRunnig = value; OnPropertyChanged("ServerIsRunnig"); }
        }
        public bool _ServerIsLoaded = false;
        public bool ServerIsLoaded
        {
            get { return _ServerIsLoaded; }
            set { _ServerIsLoaded = value; OnPropertyChanged("ServerIsLoaded"); }
        }

        public System.Timers.Timer tmrUpdate = new System.Timers.Timer(2000);
        public System.Timers.Timer tmrLic;
        private bool LoadingData = false;


        //public Settings.ATDIConnection ATDIConnectionData_Selsected
        //{
        //    get { return _ATDIConnectionData_Selsected; }
        //    set { _ATDIConnectionData_Selsected = value; OnPropertyChanged("ATDIConnectionData_Selsected"); }
        //}
        //private Settings.ATDIConnection _ATDIConnectionData_Selsected = new Settings.ATDIConnection();

        public ObservableCollection<DFData> BearingsData
        {
            get { return _BearingsData; }
            set { _BearingsData = value; OnPropertyChanged("BearingsData"); }
        }
        ObservableCollection<DFData> _BearingsData = new ObservableCollection<DFData>() { };
        //[Magic]
        //public ObservableCollection<LocalAtdiTask> LocalAtdiTasks
        //{
        //    get { return _LocalAtdiTasks; }
        //    set { _LocalAtdiTasks = value; OnPropertyChanged("LocalAtdiTasks"); }
        //}
        //ObservableCollection<LocalAtdiTask> _LocalAtdiTasks = new ObservableCollection<LocalAtdiTask>() { };

        /// <summary>
        /// API v2 //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public ObservableCollection<localatdi_meas_task> AtdiTasks
        {
            get { return _AtdiTasks; }
            set { _AtdiTasks = value; OnPropertyChanged("AtdiTasks"); }
        }
        ObservableCollection<localatdi_meas_task> _AtdiTasks = new ObservableCollection<localatdi_meas_task>() { };
        /// <summary>
        /// текущий task 
        /// </summary>
        public localatdi_meas_task AtdiTask
        {
            get { return _AtdiTask; }
            set { _AtdiTask = value; OnPropertyChanged("AtdiTask"); }
        }
        private localatdi_meas_task _AtdiTask = new localatdi_meas_task() { };

        public ObservableCollection<localatdi_unknown_result> AtdiUnknownResults
        {
            get { return _AtdiUnknownResults; }
            set { _AtdiUnknownResults = value; OnPropertyChanged("AtdiUnknownResults"); }
        }
        ObservableCollection<localatdi_unknown_result> _AtdiUnknownResults = new ObservableCollection<localatdi_unknown_result>() { };
        /// <summary>
        /// текущий месяц
        /// </summary>
        public localatdi_unknown_result AtdiUnknownResult
        {
            get { return _AtdiUnknownResult; }
            set { _AtdiUnknownResult = value; OnPropertyChanged("AtdiUnknownResults"); }
        }
        private localatdi_unknown_result _AtdiUnknownResult = new localatdi_unknown_result() { };

        public ObservableCollection<Track> TracksData
        {
            get { return _TracksData; }
            set { _TracksData = value; OnPropertyChanged("TracksData"); }
        }
        ObservableCollection<Track> _TracksData = new ObservableCollection<Track>() { };

        public Track TracksDataSelected
        {
            get { return _TracksDataSelected; }
            set { _TracksDataSelected = value; OnPropertyChanged("TracksDataSelected"); }
        }
        Track _TracksDataSelected = new Track() { };

        public bool RouteSaveToTask
        {
            get { return _RouteSaveToTask; }
            set { _RouteSaveToTask = value; OnPropertyChanged("RouteSaveToTask"); }
        }
        public bool _RouteSaveToTask = false;

        public bool RouteSaveToUnkResult
        {
            get { return _RouteSaveToUnkResult; }
            set { _RouteSaveToUnkResult = value; OnPropertyChanged("RouteSaveToUnkResult"); }
        }
        public bool _RouteSaveToUnkResult = false;

        private readonly object _itemsLock1 = new object();
        private readonly object _itemsLock2 = new object();



        public ObservableCollection<WRLSMacBinding> WRLSMacBindings
        {
            get { return _WRLSMacBindings; }
            set { _WRLSMacBindings = value; OnPropertyChanged("WRLSMacBindings"); }
        }
        private ObservableCollection<WRLSMacBinding> _WRLSMacBindings = new ObservableCollection<WRLSMacBinding>() { };
        public MeasMon MeasMon
        {
            get { return _MeasMon; }
            set { _MeasMon = value; OnPropertyChanged("MeasMon"); }
        }
        private MeasMon _MeasMon = new MeasMon() { };

        public bool MeasMonState
        {
            get { return _MeasMonState; }
            set
            {
                if (_MeasMonState == true && value == false)
                { dbt += SaveUpdateATDIMeasData_v2; }
                _MeasMonState = value;
                if (_MeasMonState == true)
                {
                    Atdi_LevelResults_DistanceStep = (decimal)App.Sett.MeasMons_Settings.TrackDistanceStep;
                    Atdi_LevelsMeasurementsCar_TimeStep = new TimeSpan(0, 0, App.Sett.MeasMons_Settings.TrackTimeStep);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    MeasMon.Mode = App.Sett.MeasMons_Settings.Mode;
                    tmrUpdate.AutoReset = true;
                    tmrUpdate.Enabled = true;
                    tmrUpdate.Elapsed += SaveUpdateMeasData;
                    tmrUpdate.Start();

                    if (MeasMon.FromTask)
                    {
                        AtdiTask.routes_id++;
                    }
                    else
                    {
                        AtdiUnknownResult.routes_id++;
                    }

                    GPSt = new DBTreadDelegate(sameWork);
                    trGPS = new Thread(GPSDataWorks);
                    trGPS.Name = "DB_GPS_Thread";
                    trGPS.Priority = ThreadPriority.BelowNormal;
                    trGPS.IsBackground = true;
                    DataCycleGPS = true;
                    trGPS.Start();
                    GPSt += AddPointToRoute;//запись при старте
                    MainWindow.gps.PropertyChanged += GPSCoor_PropertyChanged;
                }
                else if (_MeasMonState == false)
                {
                    tmrUpdate.Stop();
                    tmrUpdate.Elapsed -= SaveUpdateMeasData;

                    MainWindow.gps.PropertyChanged -= GPSCoor_PropertyChanged;
                    DataCycleGPS = false;

                }
                OnPropertyChanged("MeasMonState");
            }
        }
        private bool _MeasMonState;

        public bool MeasTrackState
        {
            get { return _MeasTrackState; }
            set
            {
                if (_MeasTrackState == true && value == false)
                {
                    dbt += SaveUpdateTrackData;
                }
                _MeasTrackState = value;
                if (_MeasTrackState == true)
                {
                    Atdi_LevelResults_DistanceStep = (decimal)App.Sett.MeasMons_Settings.TrackDistanceStep;
                    Atdi_LevelsMeasurementsCar_TimeStep = new TimeSpan(0, 0, App.Sett.MeasMons_Settings.TrackTimeStep);
                    dbt += AddTrackData;
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    MeasMon.Mode = App.Sett.MeasMons_Settings.Mode;
                    tmrUpdate.AutoReset = true;
                    tmrUpdate.Enabled = true;
                    tmrUpdate.Elapsed += SaveUpdateTrackData;
                    tmrUpdate.Start();
                }
                else if (_MeasTrackState == false)
                {
                    tmrUpdate.Stop();
                    tmrUpdate.Elapsed -= SaveUpdateTrackData;

                }
                OnPropertyChanged("MeasTrackState");
            }
        }
        private bool _MeasTrackState;

        public ObservableCollection<Equipment.GSMBandMeas> GSMBandMeass
        {
            get { return _GSMBandMeass; }
            set { _GSMBandMeass = value; OnPropertyChanged("GSMBandMeass"); }
        }
        private ObservableCollection<Equipment.GSMBandMeas> _GSMBandMeass = new ObservableCollection<Equipment.GSMBandMeas>() { };

        private ImageConverter converter = null;

        private System.Windows.Point _CoorFind = new System.Windows.Point(0, 0);
        public System.Windows.Point CoorFind
        {
            get { return _CoorFind; }
            set { if (_CoorFind != value) { _CoorFind = value; OnPropertyChanged("CoorFind"); } }
        }
        #region прогресс загрузки
        private int _NumbLoadItem;
        public int NumbLoadItem
        {
            get { return _NumbLoadItem; }
            set { _NumbLoadItem = value; OnPropertyChanged("NumbLoadItem"); }
        }
        private int _MaxLoadProgressBar;
        public int MaxLoadProgressBar
        {
            get { return _MaxLoadProgressBar; }
            set { _MaxLoadProgressBar = value; OnPropertyChanged("MaxLoadProgressBar"); }
        }
        private bool _VisibilityProgressBar;
        public bool VisibilityProgressBar
        {
            get { return _VisibilityProgressBar; }
            set { _VisibilityProgressBar = value; OnPropertyChanged("VisibilityProgressBar"); }
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////
        public NpgsqlDB_v2()
        {
            ServerIsRunning = CheckServerIsRunnig();
        }

        #region Загрузка данных из бд
        public void Load()
        {
            if (ServerIsRunning)
            {
                LoadData();
            }
            else { ServerIsLoaded = false; }
        }
        public void LoadData()
        {
            dbt = sameWork;
            dbt += ATDI_CheckDbData_v2;
            //dbt += LoadATDIConnectionTable;
            //dbt += FindATDIConnectionFile;
            dbt += ATDI_LoadAtdiUnknownResultsTable_v2;//Загрузка основной инфы результатов без тасков
            dbt += ATDI_LoadAtdiUnknownResults_v2;//Загрузка данных результатов без тасков
            dbt += ATDI_CheckUnknownResults;//Проверка месяца и добавление нового
            dbt += ATDI_LoadAtdiTasksTable_v2;//Загрузка основной инфы тасков
            dbt += ATDI_LoadAtdiTasks_v2;//загрузка данных таска
            dbt += GetSelectedTask;
            dbt += LoadTracksTableResults;
            tr = new Thread(LoadDataworks);
            tr.Name = "PGSQL_Thread";
            tr.Priority = ThreadPriority.Normal;
            tr.IsBackground = true;
            tr.Start();
        }
        long ddddd = 0;
        private void SaveUpdateMeasData(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool methods = false;
            foreach (Delegate d in dbt.GetInvocationList())
            {
                if (methods == false)
                { methods = ((DBTreadDelegate)d).Method.Name == "SaveUpdateATDIMeasData_v2"; }
            }
            if (methods == false)
            {
                dbt += SaveUpdateATDIMeasData_v2;
                //#region Выпилить
                //long ttt = DateTime.Now.Ticks - ddddd;
                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "add " + ((double)(ttt) / 10000).ToString();
                //}));

                //ddddd = DateTime.Now.Ticks;
                //#endregion
            }
        }
        private void SaveUpdateTrackData(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool methods = false;
            foreach (Delegate d in dbt.GetInvocationList())
            {
                if (methods == false)
                { methods = ((DBTreadDelegate)d).Method.Name == "SaveUpdateTrackData"; }
            }
            if (methods == false)
            {
                dbt += SaveUpdateTrackData;
                //#region Выпилить
                //long ttt = DateTime.Now.Ticks - ddddd;
                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "add " + ((double)(ttt) / 10000).ToString();
                //}));

                //ddddd = DateTime.Now.Ticks;
                //#endregion
            }
        }
        private static bool CheckServerIsRunnig()
        {
            bool t = true;
            try
            {
                try
                {
                    using (NpgsqlConnection cdb = new NpgsqlConnection(DefaultToDb))
                    {
                        #region 
                        cdb.Open();
                        string SQL = "ALTER USER postgres WITH PASSWORD '10488009';";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                        }
                        cdb.Close();
                        cdb.Dispose();
                        #endregion
                    }
                }
                catch { }
                using (NpgsqlConnection cdb = new NpgsqlConnection(GlobconnToDb))
                {
                    #region проверка существования базы и изера
                    cdb.Open();
                    string SQL = "select * from pg_shadow;";
                    bool ControlUUserIsExist = false;
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            if (dr[0] as string == "controluuser")//pas = ;ervatj,jkd
                            { ControlUUserIsExist = true; }
                        }
                    }
                    cdb.Close();
                    if (!ControlUUserIsExist)
                    {
                        cdb.Open();
                        SQL = "CREATE USER controluuser WITH LOGIN PASSWORD 'xf45hndf65hshn3h' SUPERUSER INHERIT CREATEDB CREATEROLE REPLICATION;";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                    }
                    cdb.Open();
                    SQL = "select datname from pg_database where datistemplate = false;";
                    bool controludbIsExist = false;
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        //dr2.NextResult
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if (temp == "controludb")
                            { controludbIsExist = true; }
                        }
                    }
                    cdb.Close();
                    if (!controludbIsExist)
                    {
                        cdb.Open(); //ENCODING = 'UTF8' LC_COLLATE = 'Ukrainian_Ukraine.1251' LC_CTYPE = 'Ukrainian_Ukraine.1251'
                        SQL = "CREATE DATABASE controludb WITH OWNER = controluuser ENCODING = 'UTF8' TABLESPACE = pg_default CONNECTION LIMIT = -1;";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                        cdb.Open();
                        SQL = "COMMENT ON DATABASE controludb IS 'controluuser administrative connection database';";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                    }
                    cdb.Dispose();
                    #endregion
                }
            }
            catch (Exception e)
            { t = false; /*System.Windows.MessageBox.Show(e.Message);*/ MainWindow.exp.ExceptionData = new ExData() { ex = e, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
            return t;// false;//t
        }


        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public void ATDI_CheckDbData_v2()
        {
            try
            {
                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    string SQL = string.Empty;
                    #region проверка существования type localatdi_freq_occupancy ++
                    bool mytypeexist_localatdi_freq_occupancy = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_freq_occupancy';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_freq_occupancy = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_freq_occupancy == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_freq_occupancy AS (freq numeric, count integer, occupancy numeric);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_freq_occupancy = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type tracepoint ++
                    bool mytypeexist_tracepoint = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'tracepoint';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_tracepoint = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_tracepoint == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE tracepoint AS (freq numeric, level double precision);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_tracepoint = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type bandwidth_data ++
                    bool mytypeexist_bandwidth_data = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'bandwidth_data';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_bandwidth_data = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_bandwidth_data == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE bandwidth_data AS (" +
                            "bw_meas_min numeric, bw_meas_max numeric, bw_mar_peak numeric, bw_limit numeric, " +
                            "bw_measured numeric, bw_identification numeric, ndb_level double precision, ndb_result integer[], " +
                            "obw_percent numeric, obw_result integer[]" +
                            ");";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_bandwidth_data = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type channelpower_data ++
                    bool mytypeexist_channelpower_data = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'channelpower_data';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_channelpower_data = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_channelpower_data == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE channelpower_data AS (" +
                            "freq_centr numeric, channel_power_bw numeric, channel_power_result double precision" +
                            ");";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_channelpower_data = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type spectrum_data ++
                    bool mytypeexist_spectrum_data = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'spectrum_data';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_spectrum_data = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_spectrum_data == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE spectrum_data AS (" +
                            "rbw numeric, vbw numeric, freq_start numeric, freq_centr numeric, " +
                            "freq_stop numeric, freq_span numeric, meas_duration double precision, " +
                            "pre_amp integer, att integer, ref_level double precision, " +
                            "last_meas_latitude double precision, last_meas_longitude double precision, " +
                            "last_meas_altitude double precision, meas_start timestamp without time zone, " +
                            "meas_stop timestamp without time zone, trace_count integer, " +
                            "trace_points integer, trace tracepoint[]" +
                            ");";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_spectrum_data = true;
                        }
                    }
                    #endregion
                    #endregion


                    #region проверка существования type localatdi_meas_device ++
                    bool mytypeexist_localatdi_meas_device = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_meas_device';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_meas_device = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_meas_device == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_meas_device AS " +
                            "(manufacture character varying(200), model character varying(200), " +
                            "sn character varying(200), antenna character varying(200)[]);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_meas_device = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type localatdi_license_info ++
                    bool mytypeexist_localatdi_license_info = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_license_info';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_license_info = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_license_info == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_license_info AS (icsm_id integer, name character varying(500), " +
                            "start_date timestamp without time zone, close_date timestamp without time zone, end_date timestamp without time zone);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_license_info = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type localatdi_station_owner ++
                    bool mytypeexist_localatdi_station_owner = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_station_owner';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_station_owner = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_station_owner == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_station_owner AS (id integer, address character varying(500), " +
                            "code character varying(200), okpo character varying(200), name character varying(500), zip character varying(200));";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_station_owner = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type localatdi_geo_location ++
                    bool mytypeexist_localatdi_geo_location = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_geo_location';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_geo_location = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_geo_location == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_geo_location AS (agl double precision, asl double precision, " +
                            "latitude double precision, longitude double precision);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_geo_location = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type localatdi_station_site ++
                    bool mytypeexist_localatdi_station_site = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_station_site';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_station_site = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_station_site == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_station_site AS (address character varying(500), " +
                            "region character varying(200), location localatdi_geo_location);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_station_site = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type localatdi_sector_frequency ++
                    bool mytypeexist_localatdi_sector_frequency = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_sector_frequency';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_sector_frequency = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_sector_frequency == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_sector_frequency AS (id integer, id_plan integer, channel_number integer, frequency numeric);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_sector_frequency = true;
                        }
                    }
                    #endregion

                    #endregion

                    #region проверка существования type localatdi_elements_mask ++
                    bool mytypeexist_localatdi_elements_mask = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_elements_mask';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_elements_mask = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_elements_mask == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_elements_mask AS (bw numeric, level numeric);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_elements_mask = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type localatdi_station_sector ++
                    bool mytypeexist_localatdi_station_sector = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_station_sector';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_station_sector = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_station_sector == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_station_sector AS (" +
                            "sector_id character varying(200), agl numeric, azimuth numeric, bw numeric, " +
                            "eirp numeric, class_emission character varying(50), " +
                            "frequencies localatdi_sector_frequency[], bw_mask localatdi_elements_mask[]" +
                            ");";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_station_sector = true;
                        }
                    }
                    #endregion
                    #endregion

                    #region проверка существования type localatdi_standard_scan_parameter ++
                    bool mytypeexist_localatdi_standard_scan_parameter = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_standard_scan_parameter';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_standard_scan_parameter = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_standard_scan_parameter == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_standard_scan_parameter AS (" +
                            "detection_level_dbm numeric, max_frequency_relative_offset_mk numeric, " +
                            "max_permission_bw numeric, standard character varying(50), " +
                            "xdb_level_db numeric, detector_type integer, meas_time_sec numeric, " +
                            "preamplification_db integer, rbw numeric, vbw numeric, " +
                            "ref_level_dbm numeric, rf_attenuation_db numeric, meas_span numeric" +
                            ");";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_standard_scan_parameter = true;
                        }
                    }
                    #endregion
                    #endregion                    

                    #region проверка существования type localatdi_result_state_data ++
                    bool mytypeexist_localatdi_result_state_data = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_result_state_data';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_result_state_data = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_result_state_data == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_result_state_data AS (" +
                            "delivery_confirmation integer, saved_in_db boolean, " +
                            "result_id character varying(100), result_sended timestamp without time zone, " +
                            "response_received timestamp without time zone, error_text character varying(1000)" +
                            ");";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_result_state_data = true;
                        }
                    }
                    #endregion
                    #endregion


                    #region проверка существования type atdi_table_name_with_tech ++
                    bool mytypeexist_atdi_table_name_with_tech = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_task_with_tech';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_atdi_table_name_with_tech = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_atdi_table_name_with_tech == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_task_with_tech AS (" +
                            "tech character varying(20), task_table_name character varying(200), " +
                            "result_table_name character varying(200), scan_parameters localatdi_standard_scan_parameter[]);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_atdi_table_name_with_tech = true;
                        }
                    }
                    #endregion

                    #endregion

                    #region проверка существования type localatdi_level_meas_result ++
                    bool mytypeexist_localatdi_level_meas_result = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_level_meas_result';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_level_meas_result = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_level_meas_result == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_level_meas_result AS (" +
                            "saved_in_db boolean, difference_time_stamp_ns numeric, level_dbm  double precision, " +
                            "level_dbmkvm  double precision, measurement_time timestamp without time zone, " +
                            "location localatdi_geo_location);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_level_meas_result = true;
                        }
                    }
                    #endregion

                    #endregion

                    #region проверка существования type local_3gpp_system_information_block ++
                    bool mytypeexist_3gppsib = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'local_3gpp_system_information_block';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_3gppsib = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_3gppsib == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE local_3gpp_system_information_block AS (type character varying(120), " +
                            "datastring character varying, " +
                            "saved timestamp without time zone);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_3gppsib = true;
                        }
                    }
                    #endregion

                    #endregion

                    #region проверка существования type localatdi_station_sys_info ++
                    bool mytypeexist_localatdi_station_sys_info = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_station_sys_info';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_station_sys_info = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_station_sys_info == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_station_sys_info AS (" +
                            "base_id integer, bsic integer, channel_number integer, cid integer, " +
                            "eci integer, e_node_b_id integer, lac integer, mcc integer, mnc integer, " +
                            "nid integer, pci integer, pn integer, rnc integer, sc integer, sid integer, " +
                            "tac integer, ucid integer, " +
                            "bandwidth numeric, code_power double precision, ctoi double precision, " +
                            "freq numeric, icio double precision, inband_power double precision, " +
                            "iscp double precision, power double precision, ptotal double precision, " +
                            "rscp double precision, rsrp double precision, rsrq double precision, " +
                            "type_cdmaevdo character varying(30), " +
                            "location localatdi_geo_location," +
                            "information_blocks local_3gpp_system_information_block[]);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_station_sys_info = true;
                        }
                    }
                    #endregion

                    #endregion

                    #region проверка существования type localatdi_route_point ++
                    bool mytypeexist_localatdi_route_point = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_route_point';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_route_point = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_route_point == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_route_point AS (route_id integer, saved_in_db boolean, " +
                            "point_stay_type integer, start_time timestamp without time zone, " +
                            "finish_time timestamp without time zone, location localatdi_geo_location);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_route_point = true;
                        }
                    }
                    #endregion

                    #endregion



                    #region проверка существования таблицы localatdi_meas_task ++
                    cdb.Open();
                    SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace " +
                         "= pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = 'localatdi_meas_task';";
                    bool TasksTableExist = false;
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if (temp == "localatdi_meas_task")
                            { TasksTableExist = true; }
                        }
                        cdb.Close();
                    }
                    if (!TasksTableExist)
                    {
                        cdb.Open();
                        SQL = "CREATE TABLE public.localatdi_meas_task(task_id character varying(200), equipment_tech_id character varying(200), " +
                              "xms_eqipment integer[], priority integer, scan_per_task_number integer, sdrn_server character varying(300), " +
                              "sensor_name character varying(200), status character varying(50), time_start timestamp without time zone, " +
                              "time_stop timestamp without time zone, time_save timestamp without time zone, time_last_send_result timestamp without time zone, " +
                              "with_results_measured integer, without_results_measured integer, with_ppe_results_measured integer, with_ndp_results_measured integer, " +
                              "task_station_count integer, task_state integer, new_result_to_send integer, " +
                              "task_data_from_tech localatdi_task_with_tech[], routes_tb_name character varying(200), " +
                              "results_info localatdi_result_state_data[]" +
                              ") WITH (OIDS = FALSE);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                    }
                    #endregion


                    #region проверка существования type localatdi_unknown_result_with_tech ++
                    bool mytypeexist_localatdi_unknown_result_with_tech = false;
                    cdb.Open();
                    SQL = "SELECT 1 FROM pg_type WHERE typname = 'localatdi_unknown_result_with_tech';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {

                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if ((int)dr2[0] == 1 || temp.Contains("1"))
                            {
                                mytypeexist_localatdi_unknown_result_with_tech = true;
                            }
                        }
                        cdb.Close();
                    }
                    #region создаем
                    if (mytypeexist_localatdi_unknown_result_with_tech == false)
                    {
                        cdb.Open();
                        SQL = "CREATE TYPE localatdi_unknown_result_with_tech AS (" +
                            "tech character varying(20), result_table_name character varying(200)" + //, " +
                                                                                                     //"results_info localatdi_result_state_data[]" +
                            ");";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            cdb.Close();
                            mytypeexist_localatdi_unknown_result_with_tech = true;
                        }
                    }
                    #endregion

                    #endregion

                    #region проверка существования таблицы localatdi_unknown_result ++
                    cdb.Open();
                    SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace " +
                         "= pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = 'localatdi_unknown_result';";
                    bool TableExist_localatdi_unknown_result = false;
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if (temp == "localatdi_unknown_result")
                            { TableExist_localatdi_unknown_result = true; }
                        }
                        cdb.Close();
                    }
                    if (!TableExist_localatdi_unknown_result)
                    {
                        cdb.Open();
                        SQL = "CREATE TABLE localatdi_unknown_result " +
                            "(id character varying(100), time_start timestamp without time zone, " +
                            "time_stop timestamp without time zone, routes_tb_name character varying(200), " +
                            "new_result_to_send integer, data_from_tech localatdi_unknown_result_with_tech[], " +
                            "time_last_send_result timestamp without time zone, results_info localatdi_result_state_data[]" +
                            ") WITH (OIDS = FALSE);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                    }
                    #endregion

                    #region проверка существования таблицы atdi_ids ++
                    cdb.Open();
                    SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace " +
                        "= pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = 'atdi_ids';";
                    bool TRsultIDsTableExist = false;
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if (temp == "atdi_ids")
                            { TRsultIDsTableExist = true; }
                        }
                        cdb.Close();
                    }
                    if (!TRsultIDsTableExist)
                    {
                        cdb.Open();
                        SQL = @"CREATE TABLE atdi_ids(id integer, result_send_id integer);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                        cdb.Open();
                        SQL = @"INSERT INTO atdi_ids (id, result_send_id) VALUES (0, 0);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                    }
                    #endregion                    

                    #region проверка существования таблицы sensor_registration_data
                    //cdb.Open();
                    //SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = 'sensor_registration_data';";
                    //bool SensorRegistrationDataTableExist = false;
                    //using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    //{
                    //    NpgsqlDataReader dr2 = command.ExecuteReader();
                    //    while (dr2.Read())
                    //    {
                    //        string temp = dr2[0] as string;
                    //        if (temp == "sensor_registration_data")
                    //        { SensorRegistrationDataTableExist = true; }
                    //    }
                    //    cdb.Close();
                    //}
                    //if (!SensorRegistrationDataTableExist)
                    //{
                    //    cdb.Open();
                    //    SQL = "CREATE TABLE sensor_registration_data(id integer, owner_id character varying(500), product_key character varying(500), " +
                    //        "sensor_equipment_tech_id character varying(500), rabbit_host_name character varying(500), rabbit_virtual_host_name character varying(500), " +
                    //        "rabbit_host_port character varying(10), rabbit_user_name character varying(300), " +
                    //        "rabbit_password character varying(500), sensor_queue character varying(500), " +
                    //        "sensor_confirm_queue character varying(500), task_queue character varying(500), result_queue character varying(500), " +
                    //        "server_instance character varying(500));";

                    //    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    //    {
                    //        command.ExecuteReader();
                    //    }
                    //    cdb.Close();
                    //    cdb.Open();
                    //    SQL = @"INSERT INTO sensor_registration_data " +
                    //        "(id, owner_id, product_key, sensor_equipment_tech_id, rabbit_host_name, rabbit_virtual_host_name, " +
                    //        "rabbit_user_name, rabbit_host_port, " +
                    //        "rabbit_password, sensor_queue, sensor_confirm_queue, task_queue, result_queue, " +
                    //        "server_instance) VALUES " +
                    //        "(0, 's', 's', 's', 's', 's', " +
                    //        "'s', 's', " +
                    //        "'s', 's', 's', 's', 's', " +
                    //        "'s');";
                    //    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    //    {
                    //        command.ExecuteReader();
                    //    }
                    //    cdb.Close();
                    //}
                    #endregion

                    #region проверка существования таблицы tracks_data
                    cdb.Open();
                    SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = 'tracks_data';";
                    bool TracksDataTableExist = false;
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if (temp == "tracks_data")
                            { TracksDataTableExist = true; }
                        }
                        cdb.Close();
                    }
                    if (!TracksDataTableExist)
                    {
                        cdb.Open();
                        SQL = "CREATE TABLE public.tracks_data(name character varying(200), time_start timestamp without time zone, " +
                              "time_stop timestamp without time zone, table_name character varying(200)" +
                              ") WITH (OIDS = FALSE);";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteReader();
                        }
                        cdb.Close();
                    }
                    #endregion
                }
            }
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            dbt -= ATDI_CheckDbData_v2;
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public void ATDI_CheckUnknownResults()
        {
            //if (AtdiUnknownResults.Count() == 0)
            //{

            //    localatdi_unknown_result ur = new localatdi_unknown_result() { };
            //    DateTime date = MainWindow.gps.LocalTime;
            //    ur.time_start = new DateTime(date.Year, date.Month, 1);
            //    ur.time_stop = ur.time_start.AddMonths(1).AddDays(-1);
            //    ur.routes_tb_name = "unknown_result_" + date.Year + "_" + date.Month + "_routes"; //"unknown_result_" + date.Year + "_" + date.Month;
            //    ur.id = date.Year + "_" + date.Month;
            //    ur.routes = new ObservableCollection<localatdi_route_point>() { };
            //    ur.data_from_tech = new ObservableCollection<localatdi_unknown_result_with_tech>() { };
            //    bool res = ATDI_AddNewMothYoUnknownResults_v2(ur);
            //    //System.Windows.MessageBox.Show(res.ToString());
            //}
            //else
            //{
            bool findthismonth = false;
            for (int i = 0; i < AtdiUnknownResults.Count(); i++)
            {
                if (!findthismonth && AtdiUnknownResults[i].time_stop.Month == DateTime.Now.Month && AtdiUnknownResults[i].time_stop.Year == DateTime.Now.Year)
                {
                    findthismonth = true;
                    AtdiUnknownResult = AtdiUnknownResults[i];
                    //System.Windows.MessageBox.Show("есть такой месяц");
                }

            }
            if (!findthismonth)
            {
                //создадим новый месяц для результатов
                localatdi_unknown_result ur = new localatdi_unknown_result() { };
                DateTime date = MainWindow.gps.LocalTime;
                ur.time_start = new DateTime(date.Year, date.Month, 1);
                ur.time_stop = ur.time_start.AddMonths(1).AddDays(-1);
                ur.routes_tb_name = "unknown_result_" + date.Year + "_" + date.Month + "_routes"; //"unknown_result_" + date.Year + "_" + date.Month;
                ur.id = date.Year + "_" + date.Month;
                ur.routes = new ObservableCollection<localatdi_route_point>() { };
                ur.data_from_tech = new ObservableCollection<localatdi_unknown_result_with_tech>() { };
                ur.ResultsInfo = new ObservableCollection<localatdi_result_state_data>() { };
                bool res = ATDI_AddNewMothYoUnknownResults_v2(ur);
                //MainWindow.gps.LocalTime = DateTime.Now;
                //System.Windows.MessageBox.Show("нету такой месяц");

            }
            //}

            dbt -= ATDI_CheckUnknownResults;
        }

        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public void ATDI_LoadAtdiTasksTable_v2()
        {
            AtdiTasks = new ObservableCollection<localatdi_meas_task>();
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_task_with_tech>("localatdi_task_with_tech");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_route_point>("localatdi_route_point");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_standard_scan_parameter>("localatdi_standard_scan_parameter");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_result_state_data>("localatdi_result_state_data");
                NpgsqlCommand command;
                NpgsqlDataReader dr;
                cdb.Open();
                #region Data
                #region узнаем количевство строк
                using (command = new NpgsqlCommand("SELECT COUNT(*) FROM localatdi_meas_task;", cdb))
                {
                    try
                    {
                        dr = command.ExecuteReader();
                        dr.Read();
                        MaxLoadProgressBar += Int32.Parse(dr["count"].ToString());
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                }
                #endregion
                cdb.Close();
                cdb.Open();
                try
                {
                    using (command = new NpgsqlCommand("SELECT * FROM localatdi_meas_task;", cdb))
                    {
                        #region
                        dr = command.ExecuteReader();
                        ObservableCollection<localatdi_meas_task> tsks = new ObservableCollection<localatdi_meas_task>();
                        while (dr.Read())
                        {
                            localatdi_meas_task tsk = new localatdi_meas_task() { };

                            tsk.task_id = (string)dr["task_id"];
                            tsk.equipment_tech_id = (string)dr["equipment_tech_id"];
                            tsk.xms_eqipment = (int[])dr["xms_eqipment"];
                            tsk.priority = (int)dr["priority"];
                            tsk.scan_per_task_number = (int)dr["scan_per_task_number"];
                            tsk.sdrn_server = (string)dr["sdrn_server"];
                            tsk.sensor_name = (string)dr["sensor_name"];
                            tsk.status = (string)dr["status"];
                            tsk.time_start = (DateTime)dr["time_start"];
                            tsk.time_stop = (DateTime)dr["time_stop"];
                            tsk.time_save = (DateTime)dr["time_save"];
                            tsk.time_last_send_result = (DateTime)dr["time_last_send_result"];
                            tsk.with_results_measured = (int)dr["with_results_measured"];
                            tsk.without_results_measured = (int)dr["without_results_measured"];
                            tsk.with_ppe_results_measured = (int)dr["with_ppe_results_measured"];
                            tsk.with_ndp_results_measured = (int)dr["with_ndp_results_measured"];
                            tsk.task_station_count = (int)dr["task_station_count"];
                            tsk.task_state = (int)dr["task_state"];
                            tsk.new_result_to_send = (int)dr["new_result_to_send"];
                            tsk.data_from_tech = new ObservableCollection<localatdi_task_with_tech>((localatdi_task_with_tech[])dr["task_data_from_tech"]);
                            tsk.routes_tb_name = (string)dr["routes_tb_name"];
                            tsk.routes = new ObservableCollection<localatdi_route_point>() { };
                            tsk.ResultsInfo = new ObservableCollection<localatdi_result_state_data>((localatdi_result_state_data[])dr["results_info"]);

                            tsks.Add(tsk);
                        }
                        //сортируем по убыванию meas_task_id
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            AtdiTasks = new ObservableCollection<localatdi_meas_task>(tsks.OrderByDescending(i => i.task_id));
                        });
                        #endregion
                    }
                }
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                finally
                {
                    cdb.Close();
                    //NpgsqlConnection.GlobalTypeMapper.UnmapComposite<localatdi_standard_scan_parameter>("localatdi_standard_scan_parameter");
                    //NpgsqlConnection.GlobalTypeMapper.UnmapComposite<localatdi_task_with_tech>("localatdi_task_with_tech");
                    //NpgsqlConnection.GlobalTypeMapper.UnmapComposite<localatdi_route_point>("localatdi_route_point");
                    //NpgsqlConnection.GlobalTypeMapper.Reset();
                }
                #endregion
            }
            dbt -= ATDI_LoadAtdiTasksTable_v2;
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        private void ATDI_LoadAtdiTasks_v2()
        {
            LoadingData = true;
            for (int i = 0; i < AtdiTasks.Count; i++)
            {
                localatdi_meas_task latdi = AtdiTasks[i];

                #region routes
                NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_route_point>("localatdi_route_point");//license

                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    NpgsqlCommand command;
                    NpgsqlDataReader dr;
                    //cdb.Open();
                    #region routes
                    cdb.Open();
                    try
                    {
                        using (command = new NpgsqlCommand("SELECT * FROM " + latdi.routes_tb_name.ToLower() + ";", cdb))
                        #region
                        {
                            dr = command.ExecuteReader();
                            latdi.routes = new ObservableCollection<localatdi_route_point>() { };
                            while (dr.Read())
                            {
                                localatdi_route_point point = (localatdi_route_point)dr["data"];
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    AtdiTasks[i].routes.Add(point);
                                });
                            }
                            if (AtdiTasks[i].routes.Count() > 0) AtdiTasks[i].routes_id = AtdiTasks[i].routes.Max(x => x.route_id);
                        }
                        #endregion
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                    finally { cdb.Close(); }
                    #endregion routes
                }
                #endregion routes
                #region data
                for (int j = 0; j < latdi.data_from_tech.Count(); j++)
                {
                    NpgsqlConnection.GlobalTypeMapper.Reset();
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.tracepoint>("tracepoint");//tracepoint
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.spectrum_data>("spectrum_data");//spectrum_data
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.bandwidth_data>("bandwidth_data");//bandwidth_data
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.channelpower_data>("channelpower_data");//bandwidth_data
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_license_info>("localatdi_license_info");//license
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_owner>("localatdi_station_owner");//owner
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_site>("localatdi_station_site");//site
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_sector_frequency>("localatdi_sector_frequency");//frequencies
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_elements_mask>("localatdi_elements_mask");//mask
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sector>("localatdi_station_sector");//sectors

                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_level_meas_result>("localatdi_level_meas_result");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sys_info>("localatdi_station_sys_info");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_meas_device>("localatdi_meas_device");

                    using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                    {
                        NpgsqlCommand command;
                        NpgsqlDataReader dr;
                        //cdb.Open();
                        #region task Data
                        #region узнаем количевство строк
                        //using (command = new NpgsqlCommand("SELECT COUNT(*) FROM " + latdi.TaskDataFromTech[j].tasktablename.ToLower() + ";", cdb))
                        //{
                        //    try
                        //    {
                        //        dr = command.ExecuteReader();
                        //        dr.Read();
                        //        MaxLoadProgressBar += Int32.Parse(dr["count"].ToString());
                        //    }
                        //    catch (Exception exp)
                        //    {
                        //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        //    }
                        //}
                        #endregion
                        //cdb.Close();
                        cdb.Open();
                        try
                        {
                            using (command = new NpgsqlCommand("SELECT * FROM " + latdi.data_from_tech[j].task_table_name.ToLower() + ";", cdb))
                            #region
                            {
                                dr = command.ExecuteReader();
                                //try
                                //{
                                //    latft.TasksItems.Clear();
                                //}
                                //catch { }
                                while (dr.Read())
                                {
                                    #region
                                    int db_s0 = 0, ra_s0 = 0;
                                    int db_s1 = 0, ra_s1 = 0;
                                    int db_s2 = 0, ra_s2 = 0;
                                    int db_s3 = 0, ra_s3 = 0;
                                    if (latdi.data_from_tech[j].tech.ToLower().Contains("gsm") || latdi.data_from_tech[j].tech.ToLower().Contains("umts") || latdi.data_from_tech[j].tech.ToLower().Contains("lte") || latdi.data_from_tech[j].tech.ToLower().Contains("cdma"))
                                    {
                                        string[] db_s = ((string)dr["callsign_db"]).Split(' ');
                                        if (db_s.Length == 4)
                                        {
                                            db_s0 = Convert.ToInt32(db_s[0]);
                                            db_s1 = Convert.ToInt32(db_s[1]);
                                            db_s2 = Convert.ToInt32(db_s[2]);
                                            db_s3 = Convert.ToInt32(db_s[3]);
                                        }
                                        string[] ra_s = ((string)dr["callsign_radio"]).Split(' ');
                                        if (ra_s.Length == 4)
                                        {
                                            ra_s0 = Convert.ToInt32(ra_s[0]);
                                            ra_s1 = Convert.ToInt32(ra_s[1]);
                                            ra_s2 = Convert.ToInt32(ra_s[2]);
                                            ra_s3 = Convert.ToInt32(ra_s[3]);
                                        }
                                    }
                                    #endregion
                                    localatdi_station ddd = new localatdi_station()
                                    {
                                        id = (string)dr["id"],
                                        callsign_db = (string)dr["callsign_db"],
                                        Callsign_db_S0 = db_s0,
                                        Callsign_db_S1 = db_s1,
                                        Callsign_db_S2 = db_s2,
                                        Callsign_db_S3 = db_s3,
                                        callsign_radio = (string)dr["callsign_radio"],
                                        Callsign_radio_S0 = ra_s0,
                                        Callsign_radio_S1 = ra_s1,
                                        Callsign_radio_S2 = ra_s2,
                                        Callsign_radio_S3 = ra_s3,
                                        standard = (string)dr["standard"],
                                        status = (string)dr["status"],
                                        license = (localatdi_license_info)dr["license"],
                                        owner = (localatdi_station_owner)dr["owner"],
                                        site = (localatdi_station_site)dr["site"],
                                        sectors = new ObservableCollection<localatdi_station_sector>((localatdi_station_sector[])dr["sectors"]),
                                        meas_data_exist = (bool)dr["meas_data_exist"],
                                        IsIdentified = false,
                                    };
                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                    {
                                        latdi.data_from_tech[j].SectorsCount += ddd.sectors.Count;
                                        latdi.data_from_tech[j].TaskItems.Add(ddd);
                                    });
                                }
                            }
                            #endregion
                        }
                        catch (Exception exp)
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }
                        finally { cdb.Close(); }
                        #endregion

                        #region result Data
                        #region узнаем количевство строк
                        //cdb.Open();
                        //using (command = new NpgsqlCommand("SELECT COUNT(*) FROM " + latdi.TaskDataFromTech[j].resulttablename.ToLower() + ";", cdb))
                        //{
                        //    try
                        //    {
                        //        dr = command.ExecuteReader();
                        //        dr.Read();
                        //        MaxLoadProgressBar += Int32.Parse(dr["count"].ToString());
                        //    }
                        //    catch (Exception exp)
                        //    {
                        //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        //    }
                        //}
                        #endregion

                        cdb.Open();

                        try
                        {
                            using (command = new NpgsqlCommand("SELECT * FROM " + latdi.data_from_tech[j].result_table_name.ToLower() + ";", cdb))
                            #region
                            {
                                dr = command.ExecuteReader();
                                while (dr.Read())
                                {
                                    #region
                                    string station_identifier_from_radio = "";
                                    string station_identifier_atdi = "";
                                    int r_s0 = 0, atdi_s0 = 0;
                                    int r_s1 = 0, atdi_s1 = 0;
                                    int r_s2 = 0, atdi_s2 = 0;
                                    int r_s3 = 0, atdi_s3 = 0;
                                    if (AtdiTasks[i].data_from_tech[j].tech.ToLower().Contains("gsm") ||
                                        AtdiTasks[i].data_from_tech[j].tech.ToLower().Contains("umts") ||
                                        AtdiTasks[i].data_from_tech[j].tech.ToLower().Contains("lte") ||
                                        AtdiTasks[i].data_from_tech[j].tech.ToLower().Contains("cdma") ||
                                        AtdiTasks[i].data_from_tech[j].tech.ToLower().Contains("unknown")
                                        )
                                    {
                                        station_identifier_from_radio = (string)dr["station_identifier_from_radio"];
                                        if (station_identifier_from_radio.Length > 0)
                                        {
                                            string[] ss = station_identifier_from_radio.Split(' ');
                                            if (ss.Length == 4)
                                            {
                                                r_s0 = Convert.ToInt32(ss[0]);
                                                r_s1 = Convert.ToInt32(ss[1]);
                                                r_s2 = Convert.ToInt32(ss[2]);
                                                r_s3 = Convert.ToInt32(ss[3]);
                                            }
                                        }
                                        station_identifier_atdi = (string)dr["station_identifier_atdi"];
                                        if (station_identifier_atdi.Length > 0)
                                        {
                                            string[] ss = station_identifier_atdi.Split(' ');
                                            if (ss.Length == 4)
                                            {
                                                atdi_s0 = Convert.ToInt32(ss[0]);
                                                atdi_s1 = Convert.ToInt32(ss[1]);
                                                atdi_s2 = Convert.ToInt32(ss[2]);
                                                atdi_s3 = Convert.ToInt32(ss[3]);
                                            }
                                        }
                                    }
                                    #endregion
                                    #region
                                    localatdi_result_item data = new localatdi_result_item()
                                    {
                                        id_permission = (int)dr["id_permission"],
                                        id_station = (string)dr["id_station"],
                                        id_sector = (string)dr["id_sector"],
                                        id_frequency = (int)dr["id_frequency"],
                                        id_task = (string)dr["id_task"],
                                        freq_centr_perm = (decimal)dr["freq_centr_perm"],
                                        meas_strength = (decimal)dr["meas_strength"],
                                        meas_mask = (localatdi_elements_mask[])dr["meas_mask"],
                                        mask_result = (int)dr["mask_result"],
                                        meas_correctness = (bool)dr["meas_correctness"],
                                        spec_data = (Equipment.spectrum_data)dr["spec_data"],
                                        bw_data = (Equipment.bandwidth_data)dr["bw_data"],
                                        cp_data = (Equipment.channelpower_data[])dr["cp_data"],

                                        station_identifier_from_radio = station_identifier_from_radio,
                                        station_identifier_from_radio_s0 = r_s0,
                                        station_identifier_from_radio_s1 = r_s1,
                                        station_identifier_from_radio_s2 = r_s2,
                                        station_identifier_from_radio_s3 = r_s3,
                                        station_identifier_from_radio_tech_sub_ind = (int)dr["station_identifier_from_radio_tech_sub_ind"],
                                        station_identifier_atdi = station_identifier_atdi,
                                        station_identifier_atdi_s0 = atdi_s0,
                                        station_identifier_atdi_s1 = atdi_s1,
                                        station_identifier_atdi_s2 = atdi_s2,
                                        station_identifier_atdi_s3 = atdi_s3,
                                        status = (string)dr["status"],
                                        user_id = (string)dr["user_id"],
                                        user_name = (string)dr["user_name"],
                                        new_meas_data_to_send = (bool)dr["new_meas_data_to_send"],
                                        level_results_sended = (bool)dr["level_results_sended"],
                                        level_results = new ObservableCollection<localatdi_level_meas_result>((localatdi_level_meas_result[])dr["level_results"]),
                                        station_sys_info = (localatdi_station_sys_info)dr["station_sys_info"],
                                        device_ident = (localatdi_meas_device)dr["device_ident"],
                                        device_meas = (localatdi_meas_device)dr["device_meas"],
                                    };
                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                    {
                                        AtdiTasks[i].data_from_tech[j].ResultItems.Add(data);
                                    });
                                    #endregion
                                }
                            }
                            #endregion

                            latdi.data_from_tech[j].ResultTask_WithMeasurement = 0;
                            latdi.data_from_tech[j].ResultTask_NDPWithMeasurement = 0;
                            latdi.data_from_tech[j].ResultTask_PPEWithMeasurement = 0;
                            for (int k = 0; k < latdi.data_from_tech[j].ResultItems.Count; k++)
                            {
                                if (latdi.data_from_tech[j].ResultItems[k].id_sector != "" && latdi.data_from_tech[j].ResultItems[k].id_frequency != 0)
                                { latdi.data_from_tech[j].ResultTask_WithMeasurement++; }
                                else if (latdi.data_from_tech[j].ResultItems[k].id_sector != "" && latdi.data_from_tech[j].ResultItems[k].id_frequency == 0)
                                { latdi.data_from_tech[j].ResultTask_WithMeasurement++; }
                            }
                        }
                        catch (Exception exp)
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }
                        finally { cdb.Close(); }
                        #endregion

                    }
                    //for (int s = 0; s < latdi.task_data_from_tech[j].TaskItems.Count; s++)
                    //{
                    //    latdi.task_data_from_tech[j].SectorsCount += latdi.task_data_from_tech[j].TaskItems[s].sectors.Count;
                    //}
                }
                #endregion
            }
            OnPropertyChanged("LoadedTasks");
            ServerIsLoaded = true;
            dbt -= ATDI_LoadAtdiTasks_v2;
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        private void ATDI_LoadAtdiUnknownResultsTable_v2()
        {
            AtdiUnknownResults = new ObservableCollection<localatdi_unknown_result>();
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_unknown_result_with_tech>("localatdi_unknown_result_with_tech");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_result_state_data>("localatdi_result_state_data");
                //NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_route_point>("localatdi_route_point");
                NpgsqlCommand command;
                NpgsqlDataReader dr;
                cdb.Open();
                #region Data
                #region узнаем количевство строк
                using (command = new NpgsqlCommand("SELECT COUNT(*) FROM localatdi_unknown_result;", cdb))
                {
                    try
                    {
                        dr = command.ExecuteReader();
                        dr.Read();
                        MaxLoadProgressBar += Int32.Parse(dr["count"].ToString());
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                }
                #endregion
                cdb.Close();
                cdb.Open();
                try
                {
                    using (command = new NpgsqlCommand("SELECT * FROM localatdi_unknown_result;", cdb))
                    {
                        #region
                        dr = command.ExecuteReader();
                        ObservableCollection<localatdi_unknown_result> ress = new ObservableCollection<localatdi_unknown_result>();
                        while (dr.Read())
                        {
                            localatdi_unknown_result res = new localatdi_unknown_result() { };
                            res.id = (string)dr["id"];
                            res.time_start = (DateTime)dr["time_start"];
                            res.time_stop = (DateTime)dr["time_stop"];
                            res.routes_tb_name = (string)dr["routes_tb_name"];
                            res.routes = new ObservableCollection<localatdi_route_point>() { };
                            res.new_result_to_send = (int)dr["new_result_to_send"];
                            res.data_from_tech = new ObservableCollection<localatdi_unknown_result_with_tech>((localatdi_unknown_result_with_tech[])dr["data_from_tech"]);
                            res.time_last_send_result = (DateTime)dr["time_last_send_result"];
                            res.ResultsInfo = new ObservableCollection<localatdi_result_state_data>((localatdi_result_state_data[])dr["results_info"]);

                            ress.Add(res);

                        }
                        //сортируем по убыванию meas_task_id
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            AtdiUnknownResults = new ObservableCollection<localatdi_unknown_result>(ress.OrderByDescending(i => i.time_start));
                        });
                        #endregion
                    }
                }
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                finally
                {
                    //    NpgsqlConnection.GlobalTypeMapper.UnmapComposite<localatdi_unknown_result_with_tech>("localatdi_unknown_result_with_tech");
                    //    NpgsqlConnection.GlobalTypeMapper.UnmapComposite<localatdi_route_point>();
                    //    NpgsqlConnection.GlobalTypeMapper.Reset();
                    cdb.Close();
                    cdb.Dispose();
                }
                #endregion
            }
            dbt -= ATDI_LoadAtdiUnknownResultsTable_v2;
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        private void ATDI_LoadAtdiUnknownResults_v2()
        {
            LoadingData = true;
            for (int i = 0; i < AtdiUnknownResults.Count; i++)
            {
                #region routes
                NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_route_point>("localatdi_route_point");//license
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    NpgsqlCommand command;
                    NpgsqlDataReader dr;
                    //cdb.Open();
                    #region routes
                    cdb.Open();
                    try
                    {
                        using (command = new NpgsqlCommand("SELECT * FROM " + AtdiUnknownResults[i].routes_tb_name.ToLower() + ";", cdb))
                        #region
                        {
                            dr = command.ExecuteReader();
                            AtdiUnknownResults[i].routes = new ObservableCollection<localatdi_route_point>() { };
                            while (dr.Read())
                            {
                                localatdi_route_point ddd = (localatdi_route_point)dr["data"];
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    AtdiUnknownResults[i].routes.Add(ddd);
                                });
                            }
                            if (AtdiUnknownResults[i].routes.Count() > 0) AtdiUnknownResults[i].routes_id = AtdiUnknownResults[i].routes.Max(x => x.route_id);
                        }
                        if (AtdiUnknownResults[i].time_stop.Month == DateTime.Now.Month && AtdiUnknownResults[i].time_stop.Year == DateTime.Now.Year)
                            App.Current.Dispatcher.Invoke((Action)delegate
                            {
                                AtdiUnknownResult = AtdiUnknownResults[i];

                            });
                        #endregion
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                    finally { cdb.Close(); }
                    #endregion routes
                }
                #endregion routes
                #region data
                AtdiUnknownResults[i].ResultsCount = 0;
                NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.tracepoint>("tracepoint");//tracepoint
                NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.spectrum_data>("spectrum_data");//spectrum_data
                NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.bandwidth_data>("bandwidth_data");//bandwidth_data
                NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.channelpower_data>("channelpower_data");//channelpower_data
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_license_info>("localatdi_license_info");//license
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_owner>("localatdi_station_owner");//owner
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_site>("localatdi_station_site");//site
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sector>("localatdi_station_sector");//sectors

                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_elements_mask>("localatdi_elements_mask");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_level_meas_result>("localatdi_level_meas_result");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sys_info>("localatdi_station_sys_info");

                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_meas_device>("localatdi_meas_device");
                for (int j = 0; j < AtdiUnknownResults[i].data_from_tech.Count(); j++)
                {
                    using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                    {
                        NpgsqlCommand command;
                        NpgsqlDataReader dr;

                        #region result Data
                        #region узнаем количевство строк
                        //cdb.Open();
                        //using (command = new NpgsqlCommand("SELECT COUNT(*) FROM " + latdi.TaskDataFromTech[j].resulttablename.ToLower() + ";", cdb))
                        //{
                        //    try
                        //    {
                        //        dr = command.ExecuteReader();
                        //        dr.Read();
                        //        MaxLoadProgressBar += Int32.Parse(dr["count"].ToString());
                        //    }
                        //    catch (Exception exp)
                        //    {
                        //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        //    }
                        //}
                        #endregion

                        cdb.Open();
                        try
                        {
                            using (command = new NpgsqlCommand("SELECT * FROM " + AtdiUnknownResults[i].data_from_tech[j].result_table_name.ToLower() + ";", cdb))
                            #region
                            {
                                dr = command.ExecuteReader();
                                while (dr.Read())
                                {
                                    #region
                                    string station_identifier_from_radio = "";
                                    string station_identifier_atdi = "";
                                    int r_s0 = 0, atdi_s0 = 0;
                                    int r_s1 = 0, atdi_s1 = 0;
                                    int r_s2 = 0, atdi_s2 = 0;
                                    int r_s3 = 0, atdi_s3 = 0;
                                    if (AtdiUnknownResults[i].data_from_tech[j].tech.ToLower().Contains("gsm") || AtdiUnknownResults[i].data_from_tech[j].tech.ToLower().Contains("umts") || AtdiUnknownResults[i].data_from_tech[j].tech.ToLower().Contains("lte") || AtdiUnknownResults[i].data_from_tech[j].tech.ToLower().Contains("cdma"))
                                    {
                                        station_identifier_from_radio = (string)dr["station_identifier_from_radio"];
                                        if (station_identifier_from_radio.Length > 0)
                                        {
                                            string[] ss = station_identifier_from_radio.Split(' ');
                                            if (ss.Length == 4)
                                            {
                                                r_s0 = Convert.ToInt32(ss[0]);
                                                r_s1 = Convert.ToInt32(ss[1]);
                                                r_s2 = Convert.ToInt32(ss[2]);
                                                r_s3 = Convert.ToInt32(ss[3]);
                                            }
                                        }
                                        station_identifier_atdi = (string)dr["station_identifier_atdi"];
                                        if (station_identifier_atdi.Length > 0)
                                        {
                                            string[] ss = station_identifier_atdi.Split(' ');
                                            if (ss.Length == 4)
                                            {
                                                atdi_s0 = Convert.ToInt32(ss[0]);
                                                atdi_s1 = Convert.ToInt32(ss[1]);
                                                atdi_s2 = Convert.ToInt32(ss[2]);
                                                atdi_s3 = Convert.ToInt32(ss[3]);
                                            }
                                        }
                                    }
                                    localatdi_station_sys_info station_sys_info = (localatdi_station_sys_info)dr["station_sys_info"];
                                    #endregion
                                    #region
                                    localatdi_result_item data = new localatdi_result_item() { };

                                    data.id_permission = (int)dr["id_permission"];
                                    data.id_station = (string)dr["id_station"];
                                    data.id_sector = (string)dr["id_sector"];
                                    data.id_frequency = (int)dr["id_frequency"];
                                    data.id_task = (string)dr["id_task"];
                                    data.freq_centr_perm = (decimal)dr["freq_centr_perm"];
                                    data.meas_strength = (decimal)dr["meas_strength"];
                                    data.meas_mask = (localatdi_elements_mask[])dr["meas_mask"];
                                    data.mask_result = (int)dr["mask_result"];
                                    data.meas_correctness = (bool)dr["meas_correctness"];
                                    data.spec_data = (Equipment.spectrum_data)dr["spec_data"];
                                    data.bw_data = (Equipment.bandwidth_data)dr["bw_data"];
                                    data.cp_data = (Equipment.channelpower_data[])dr["cp_data"];
                                    data.station_identifier_from_radio = station_identifier_from_radio;
                                    data.station_identifier_from_radio_s0 = r_s0;
                                    data.station_identifier_from_radio_s1 = r_s1;
                                    data.station_identifier_from_radio_s2 = r_s2;
                                    data.station_identifier_from_radio_s3 = r_s3;
                                    data.station_identifier_from_radio_tech_sub_ind = (int)dr["station_identifier_from_radio_tech_sub_ind"];
                                    data.station_identifier_atdi = station_identifier_atdi;
                                    data.station_identifier_atdi_s0 = atdi_s0;
                                    data.station_identifier_atdi_s1 = atdi_s1;
                                    data.station_identifier_atdi_s2 = atdi_s2;
                                    data.station_identifier_atdi_s3 = atdi_s3;
                                    data.status = (string)dr["status"];
                                    data.user_id = (string)dr["user_id"];
                                    data.user_name = (string)dr["user_name"];
                                    data.new_meas_data_to_send = (bool)dr["new_meas_data_to_send"];
                                    data.level_results_sended = (bool)dr["level_results_sended"];
                                    data.level_results = new ObservableCollection<localatdi_level_meas_result>((localatdi_level_meas_result[])dr["level_results"]);
                                    data.station_sys_info = station_sys_info;
                                    data.device_ident = (localatdi_meas_device)dr["device_ident"];
                                    data.device_meas = (localatdi_meas_device)dr["device_meas"];

                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                    {
                                        AtdiUnknownResults[i].data_from_tech[j].ResultItems.Add(data);
                                        AtdiUnknownResults[i].ResultsCount++;
                                    });
                                    #endregion
                                }
                            }
                            #endregion

                            //AtdiUnknownResults[i].data_from_tech[j].ResultTask_WithMeasurement = 0;
                            //latdi.task_data_from_tech[j].ResultTask_NDPWithMeasurement = 0;
                            //latdi.task_data_from_tech[j].ResultTask_PPEWithMeasurement = 0;
                            //for (int k = 0; k < latdi.task_data_from_tech[j].ResultTaskItems.Count; k++)
                            //{
                            //    if (latdi.task_data_from_tech[j].ResultTaskItems[k].id_sector != "" && latdi.task_data_from_tech[j].ResultTaskItems[k].id_frequency != 0)
                            //    { latdi.task_data_from_tech[j].ResultTask_WithMeasurement++; }
                            //    else if (latdi.task_data_from_tech[j].ResultTaskItems[k].id_sector != "" && latdi.task_data_from_tech[j].ResultTaskItems[k].id_frequency == 0)
                            //    { latdi.task_data_from_tech[j].ResultTask_WithMeasurement++; }
                            //}
                        }
                        catch (Exception exp)
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }
                        finally { cdb.Close(); }
                        #endregion

                    }
                    //for (int s = 0; s < latdi.task_data_from_tech[j].TaskItems.Count; s++)
                    //{
                    //    latdi.task_data_from_tech[j].SectorsCount += latdi.task_data_from_tech[j].TaskItems[s].sectors.Count;
                    //}
                }
                #endregion
            }
            //OnPropertyChanged("LoadedTasks");
            //ServerIsLoaded = true;
            dbt -= ATDI_LoadAtdiUnknownResults_v2;
        }
        private bool ATDI_AddNewMothYoUnknownResults_v2(localatdi_unknown_result month)
        {
            bool res = false;
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                try
                {
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_unknown_result_with_tech>("localatdi_unknown_result_with_tech");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_route_point>("localatdi_route_point");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_result_state_data>("localatdi_result_state_data");
                    bool add = false;
                    #region add
                    string SQL = "INSERT INTO localatdi_unknown_result" +
                    "(id, time_start, time_stop, routes_tb_name, new_result_to_send, " +
                    "data_from_tech, time_last_send_result, results_info)" +
                    "VALUES " +
                    "(@id, @time_start, @time_stop, @routes_tb_name, @new_result_to_send, " +
                    "@data_from_tech, @time_last_send_result, @results_info);";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        cdb.Open();
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region
                                new NpgsqlParameter("@id", month.id),
                                new NpgsqlParameter("@time_start", month.time_start),
                                new NpgsqlParameter("@time_stop", month.time_stop),
                                new NpgsqlParameter("@routes_tb_name", month.routes_tb_name),
                                new NpgsqlParameter("@new_result_to_send", month.new_result_to_send),
                                new NpgsqlParameter("@data_from_tech", month.data_from_tech.ToArray()),
                                new NpgsqlParameter("@time_last_send_result", month.time_last_send_result),
                                new NpgsqlParameter("@results_info", month.ResultsInfo.ToArray()),

                                #endregion
                            });
                        int recordAffected = cmd.ExecuteNonQuery();
                        cdb.Close();
                        add = Convert.ToBoolean(recordAffected);
                        cmd.Dispose();
                    }
                    #endregion
                    bool route = false;
                    #region CREATE route
                    cdb.Open();
                    SQL = "CREATE TABLE " + month.routes_tb_name.ToLower() + " (data localatdi_route_point)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        int recordAffected = cmd.ExecuteNonQuery();
                        route = Convert.ToBoolean(recordAffected);
                    }
                    cdb.Close();
                    #endregion
                    if (add && route)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            AtdiUnknownResults.Add(month);
                            AtdiUnknownResult = month;
                        }));
                        res = true;
                    }
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    //NpgsqlConnection.GlobalTypeMapper.UnmapComposite<localatdi_unknown_result_with_tech>("localatdi_unknown_result_with_tech");
                    //NpgsqlConnection.GlobalTypeMapper.UnmapComposite<localatdi_route_point>("localatdi_route_point");
                    //NpgsqlConnection.ClearAllPools();
                    if (cdb != null) cdb.Close();
                }
            }
            return res;
        }

        public string Check(string datain1, string datain2)
        {
            DB.ConvertData cd = new DB.ConvertData(datain1, datain2);
            string s = cd.GetConvertedData();
            App.Lic = s != "";
            if (!App.Lic)
            {
                tmrLic = new System.Timers.Timer(11000);
                tmrLic.AutoReset = true;
                tmrLic.Enabled = true;
                tmrLic.Elapsed += LicenseFailShowMessage;
                tmrLic.Start();
            }
            return s;
        }
        private void LicenseFailShowMessage(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (App.Lic)
                {
                    tmrLic.Stop();
                    tmrLic.Elapsed -= LicenseFailShowMessage;
                }
                else
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("LicenseFail").ToString();
                    }));
                }
            }
            catch { }
        }
        #endregion Загрузка данных из бд

        #region Connection Control Server data
        //public void LoadATDIConnectionTable()
        //{
        //    using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
        //    {
        //        NpgsqlCommand command;
        //        NpgsqlDataReader dr;
        //        cdb.Open();
        //        #region Data
        //        #region узнаем количевство строк
        //        using (command = new NpgsqlCommand("SELECT COUNT(*) FROM sensor_registration_data;", cdb))
        //        {
        //            try
        //            {
        //                dr = command.ExecuteReader();
        //                dr.Read();
        //                MaxLoadProgressBar += Int32.Parse(dr["count"].ToString());
        //            }
        //            catch (Exception exp)
        //            {
        //                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //            }
        //        }
        //        #endregion
        //        cdb.Close();
        //        cdb.Open();
        //        try
        //        {
        //            using (command = new NpgsqlCommand("SELECT * FROM sensor_registration_data WHERE id = 0;", cdb))
        //            {
        //                #region
        //                dr = command.ExecuteReader();
        //                while (dr.Read())
        //                {
        //                    Settings.ATDIConnection dt = new Settings.ATDIConnection();

        //                    dt.owner_id = (string)dr["owner_id"];
        //                    dt.product_key = (string)dr["product_key"];
        //                    dt.sensor_equipment_tech_id = (string)dr["sensor_equipment_tech_id"];
        //                    dt.rabbit_host_name = (string)dr["rabbit_host_name"];
        //                    dt.rabbit_virtual_host_name = (string)dr["rabbit_virtual_host_name"];
        //                    dt.rabbit_host_port = (string)dr["rabbit_host_port"];
        //                    dt.rabbit_user_name = (string)dr["rabbit_user_name"];
        //                    dt.rabbit_password = (string)dr["rabbit_password"];
        //                    dt.sensor_queue = (string)dr["sensor_queue"];
        //                    dt.sensor_confirm_queue = (string)dr["sensor_confirm_queue"];
        //                    dt.task_queue = (string)dr["task_queue"];
        //                    dt.result_queue = (string)dr["result_queue"];
        //                    dt.server_instance = (string)dr["server_instance"];
        //                    ATDIConnectionData_Selsected = dt;

        //                }

        //                #endregion
        //            }
        //        }
        //        catch (Exception exp)
        //        {
        //            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //        }
        //        finally
        //        {
        //            if (cdb != null) { cdb.Close(); cdb.Dispose(); }
        //        }
        //        #endregion
        //    }
        //    dbt -= LoadATDIConnectionTable;
        //}
        //public void FindATDIConnectionFile()
        //{
        //    try
        //    {
        //        string file = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Connection.xml";
        //        if (File.Exists(file))
        //        {
        //            XMLLibrary.XmlReaderStruct obj = XMLLibrary.XMLReader.GetXmlSettings(file);
        //            Settings.ATDIConnection dt = new Settings.ATDIConnection();
        //            dt.owner_id = obj._OwnerId;
        //            dt.product_key = obj._ProductKey;
        //            dt.sensor_equipment_tech_id = obj._SensorEquipmentTechId;
        //            dt.rabbit_host_name = obj._RabbitHostName;
        //            dt.rabbit_virtual_host_name = obj._RabbitVirtualHostName;
        //            dt.rabbit_host_port = obj._RabbitHostPort;
        //            dt.rabbit_user_name = obj._RabbitUserName;
        //            dt.rabbit_password = obj._RabbitPassword;
        //            dt.sensor_queue = obj._SensorQueue;
        //            dt.sensor_confirm_queue = obj._SensorConfirmQueue;
        //            dt.task_queue = obj._TaskQueue;
        //            dt.result_queue = obj._ResultQueue;
        //            dt.server_instance = obj._ServerInstance;
        //            bool res = UpdateATDIConnection(dt);
        //            if (res)
        //            {
        //                File.Delete(file);
        //                //System.Windows.MessageBox.Show("alahabar");
        //            }
        //        }
        //    }
        //    catch { }
        //    finally { dbt -= FindATDIConnectionFile; }
        //}

        ///// <summary>
        ///// добавляет новые настройки подключения и новый выбирает текущим
        ///// </summary>
        ///// <param name="newdata"></param>

        //public bool UpdateATDIConnection(Settings.ATDIConnection data)
        //{
        //    bool result = false;

        //    using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
        //    {
        //        string SQL = "UPDATE sensor_registration_data SET " +
        //            "owner_id = :owner_id, product_key = :product_key, sensor_equipment_tech_id = :sensor_equipment_tech_id, " +
        //            "rabbit_host_name = :rabbit_host_name, rabbit_virtual_host_name = :rabbit_virtual_host_name, " +
        //            "rabbit_host_port = :rabbit_host_port, rabbit_user_name = :rabbit_user_name, " +
        //            "rabbit_password = :rabbit_password, sensor_queue = :sensor_queue, sensor_confirm_queue = :sensor_confirm_queue, " +
        //            "task_queue = :task_queue, result_queue = :result_queue, server_instance = :server_instance WHERE id = 0";

        //        // (id, owner_id, product_key, sensor_equipment_tech_id, rabbit_host_name, rabbit_user_name, " +
        //        //"rabbit_password, sensor_queue, sensor_confirm_queue, task_queue, result_queue)
        //        try
        //        {
        //            cdb.Open();
        //            using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
        //            {
        //                command.Parameters.Add(new NpgsqlParameter(":owner_id", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":product_key", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":sensor_equipment_tech_id", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":rabbit_host_name", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":rabbit_virtual_host_name", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":rabbit_host_port", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":rabbit_user_name", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":rabbit_password", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":sensor_queue", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":sensor_confirm_queue", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":task_queue", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":result_queue", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                command.Parameters.Add(new NpgsqlParameter(":server_instance", NpgsqlTypes.NpgsqlDbType.Varchar));
        //                // Prepare the command.
        //                command.Prepare();
        //                // Add value to the paramater.
        //                command.Parameters[0].Value = data.owner_id;
        //                command.Parameters[1].Value = data.product_key;
        //                command.Parameters[2].Value = data.sensor_equipment_tech_id;
        //                command.Parameters[3].Value = data.rabbit_host_name;
        //                command.Parameters[4].Value = data.rabbit_virtual_host_name;
        //                command.Parameters[5].Value = data.rabbit_host_port;
        //                command.Parameters[6].Value = data.rabbit_user_name;
        //                command.Parameters[7].Value = data.rabbit_password;
        //                command.Parameters[8].Value = data.sensor_queue;
        //                command.Parameters[9].Value = data.sensor_confirm_queue;
        //                command.Parameters[10].Value = data.task_queue;
        //                command.Parameters[11].Value = data.result_queue;
        //                command.Parameters[12].Value = data.server_instance;
        //                // Execute SQL command.
        //                int recordAffected = command.ExecuteNonQuery();
        //                if (Convert.ToBoolean(recordAffected))
        //                {
        //                    ATDIConnectionData_Selsected = data;
        //                    App.Sett.ATDIConnection_Settings.Selected = data;
        //                    App.Sett.SaveAll();
        //                    result = true;
        //                }
        //            }
        //            cdb.Close();
        //        }
        //        catch (NpgsqlException exp)
        //        {
        //            App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //            {
        //                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //            }));
        //        }
        //        catch (Exception exp)
        //        {
        //            App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //            {
        //                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //            }));
        //        }
        //        finally
        //        {
        //            if (cdb != null) { cdb.Close(); cdb.Dispose(); }
        //        }
        //    }
        //    return result;
        //}
        #endregion



        #region работа с тасками
        /// <summary>
        /// изменяем состояние таска
        /// </summary>
        /// <param name="meas_task_id">ади таска</param>
        /// <param name="task_state">на что изменилось</param>
        public void updateATDITaskStateInMeasTasks(int meas_task_id, int task_state)
        {
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                cdb.Open();
                #region Data
                if (cdb.State == ConnectionState.Open)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand("UPDATE atdi_meas_tasks SET task_state = :task_state WHERE meas_task_id = :meas_task_id", cdb))
                    {
                        try
                        {
                            command.Parameters.Add(new NpgsqlParameter("task_state", NpgsqlTypes.NpgsqlDbType.Integer));
                            command.Parameters.Add(new NpgsqlParameter("meas_task_id", NpgsqlTypes.NpgsqlDbType.Integer));
                            // Prepare the command.
                            command.Prepare();
                            // Add value to the paramater.
                            command.Parameters[0].Value = task_state;
                            command.Parameters[1].Value = meas_task_id;
                            // Execute SQL command.
                            int recordAffected = command.ExecuteNonQuery();
                            if (Convert.ToBoolean(recordAffected))
                            {
                                //System.Windows.MessageBox.Show("Data successfully saved!");
                            }
                        }
                        catch (Exception exp)
                        { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
                    }
                }
                #endregion
                cdb.Close();
            }
            OnPropertyChanged("UpdateTaskState");
        }
        /// <summary>
        /// изменяем состояние таска
        /// </summary>
        /// <param name="task_id">ади таска</param>
        /// <param name="task_state">на что изменилось</param>
        public void updateATDITaskStateInMeasTasks_v2(string task_id, int task_state)
        {
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                cdb.Open();
                #region Data
                if (cdb.State == ConnectionState.Open)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand("UPDATE localatdi_meas_task SET task_state = :task_state WHERE task_id = :task_id", cdb))
                    {
                        try
                        {
                            command.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                new NpgsqlParameter("task_state", task_state),
                                new NpgsqlParameter("task_id", task_id),
                            });
                            //command.Parameters.Add(new NpgsqlParameter("task_state", NpgsqlTypes.NpgsqlDbType.Varchar));
                            //command.Parameters.Add(new NpgsqlParameter("task_id", NpgsqlTypes.NpgsqlDbType.Integer));
                            //// Prepare the command.
                            //command.Prepare();
                            //// Add value to the paramater.
                            //command.Parameters[0].Value = task_state;
                            //command.Parameters[1].Value = task_id;
                            // Execute SQL command.
                            int recordAffected = command.ExecuteNonQuery();
                            if (Convert.ToBoolean(recordAffected))
                            {
                                GetSelectedTask();
                                //System.Windows.MessageBox.Show("Data successfully saved!");
                            }
                        }
                        catch (Exception exp)
                        { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
                    }
                }
                #endregion
                cdb.Close();
            }
            OnPropertyChanged("UpdateTaskState");
        }


        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public void SaveReceivedTask_v2(List<LocalMeasSdrTask_v2> tasks)
        {
            try
            {

                for (int t = 0; t < tasks.Count; t++)
                {
                    string tasknumber = "";
                    if (tasks[t].Saved == false)
                    {
                        localatdi_meas_task task = tasks[t].MeasTask;
                        tasknumber += "№ " + task.task_id + "; ";
                        //bool ReplaceOldTask = false; //обновить таск

                        //создаем таблицы
                        using (NpgsqlConnection ncdb = new NpgsqlConnection(UserconnToDb))
                        {
                            bool TableExist_routes = false;
                            #region проверка существования таблицы загружаемого routes и создание таблицы routes
                            ncdb.Open();
                            string SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = " +
                                "pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = '" + task.routes_tb_name.ToLower() + "';";
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, ncdb))
                            {
                                NpgsqlDataReader dr2 = command.ExecuteReader();
                                while (dr2.Read())
                                {
                                    string temp = dr2[0] as string;
                                    if (temp.ToLower().Contains(task.routes_tb_name.ToLower()))
                                    {
                                        TableExist_routes = true;
                                        //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Такой план уже есть!";
                                    }
                                }
                                ncdb.Close();
                            }
                            #endregion
                            SQL = "";
                            #region создаем таблицы по технологиям
                            //есть такая таблица в базе и мы обновляем текущий таск
                            if (TableExist_routes == true/* && ReplaceOldTask == true*/)
                            {
                                //убиваем старую таблицу с данными таска и создаем новую ибо неф.. еще и вакуум пилить
                                SQL = "DROP TABLE " + task.routes_tb_name.ToLower() + ";\r\n";
                            }
                            //пишем посекторно (если несколько секторов то дублируем инфу)
                            ncdb.Open();
                            SQL += "CREATE TABLE " + task.routes_tb_name.ToLower() + " (data localatdi_route_point);";
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, ncdb))
                            {
                                command.ExecuteNonQuery();
                            }
                            ncdb.Close();
                            #endregion
                        }
                        for (int i = 0; i < task.data_from_tech.Count; i++)
                        {
                            bool TaskTableExist = false;
                            bool ResultTaskTableExist = false;
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_license_info>("localatdi_license_info");//license
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_owner>("localatdi_station_owner");//owner
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");//location
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_site>("localatdi_station_site");//site

                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_sector_frequency>("localatdi_sector_frequency");//frequencies
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_elements_mask>("localatdi_elements_mask");//mask
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sector>("localatdi_station_sector");//sectors
                            using (NpgsqlConnection ncdb = new NpgsqlConnection(UserconnToDb))
                            {
                                #region Task
                                #region проверка существования таблицы загружаемого Task и создание таблицы Task
                                ncdb.Open();
                                string SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = " +
                                    "pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = '" + task.data_from_tech[i].task_table_name.ToLower() + "';";
                                using (NpgsqlCommand command = new NpgsqlCommand(SQL, ncdb))
                                {
                                    NpgsqlDataReader dr2 = command.ExecuteReader();
                                    while (dr2.Read())
                                    {
                                        string temp = dr2[0] as string;
                                        if (temp.ToLower().Contains(task.data_from_tech[i].task_table_name.ToLower()))
                                        {
                                            TaskTableExist = true;
                                            //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Такой план уже есть!";
                                        }
                                    }
                                    ncdb.Close();
                                }
                                #endregion
                                SQL = "";
                                #region создаем таблицы по технологиям
                                //есть такая таблица в базе и мы обновляем текущий таск
                                if (TaskTableExist == true/* && ReplaceOldTask == true*/)
                                {
                                    //убиваем старую таблицу с данными таска и создаем новую ибо неф.. еще и вакуум пилить
                                    SQL = "DROP TABLE " + task.data_from_tech[i].task_table_name.ToLower() + ";\r\n";
                                }
                                //пишем посекторно (если несколько секторов то дублируем инфу)
                                ncdb.Open();
                                SQL += "CREATE TABLE public." + task.data_from_tech[i].task_table_name.ToLower() +
                                    "(id character varying(200), callsign_db character varying(200), callsign_radio character varying(200), " +
                                    "standard character varying(50), status character varying(50), " +
                                    "license localatdi_license_info, " +
                                    "owner localatdi_station_owner, " +
                                    "site localatdi_station_site, " +
                                    "sectors localatdi_station_sector[], " +
                                    "meas_data_exist boolean) " +
                                    "WITH(OIDS = FALSE)";
                                using (NpgsqlCommand command = new NpgsqlCommand(SQL, ncdb))
                                {
                                    command.ExecuteNonQuery();
                                }
                                ncdb.Close();
                                #endregion
                                #endregion

                                #region Result
                                #region проверка существования таблицы загружаемого Task и создание таблицы Task
                                ncdb.Open();
                                SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = " +
                                    "pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = '" + task.data_from_tech[i].result_table_name.ToLower() + "';";
                                using (NpgsqlCommand command = new NpgsqlCommand(SQL, ncdb))
                                {
                                    NpgsqlDataReader dr2 = command.ExecuteReader();
                                    while (dr2.Read())
                                    {
                                        string temp = dr2[0] as string;
                                        if (temp.ToLower().Contains(task.data_from_tech[i].result_table_name.ToLower()))
                                        {
                                            ResultTaskTableExist = true;
                                            //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Такой план уже есть!";
                                        }
                                    }
                                    ncdb.Close();
                                }
                                #endregion
                                #region создаем таблицы по технологиям
                                if (ResultTaskTableExist == false)
                                {
                                    ///пишем посекторно (если несколько секторов то дублируем инфу)
                                    ncdb.Open();
                                    SQL = "CREATE TABLE " + task.data_from_tech[i].result_table_name.ToLower() + " " +
                                        "(id_permission integer, id_station character varying(200), id_sector character varying(200), " +
                                        "id_frequency integer, id_task character varying(200), freq_centr_perm numeric, " +
                                        "meas_strength numeric, meas_mask localatdi_elements_mask[], mask_result integer, " +
                                        "meas_correctness boolean, spec_data spectrum_data, bw_data bandwidth_data, cp_data channelpower_data[], " +
                                        "station_identifier_from_radio character varying(200), station_identifier_from_radio_tech_sub_ind integer, " +
                                        "station_identifier_atdi character varying(200), " +
                                        "status character varying(300), user_id character varying(200), user_name character varying(500), " +
                                        "new_meas_data_to_send boolean, level_results_sended boolean, level_results localatdi_level_meas_result[], " +
                                        "station_sys_info localatdi_station_sys_info, device_ident localatdi_meas_device, device_meas localatdi_meas_device" +
                                        ")WITH(OIDS = FALSE)";
                                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, ncdb))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    ncdb.Close();
                                }
                                #endregion
                                ncdb.Dispose();
                            }
                            #endregion
                        }
                        for (int i = 0; i < task.data_from_tech.Count(); i++)
                        {
                            #region пишем строки в таблицы в их таблицу
                            //bool ThisTechWriteComplite = false;
                            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                            {
                                //NpgsqlConnection.GlobalTypeMapper.Reset();
                                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_license_info>("localatdi_license_info");//license
                                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_owner>("localatdi_station_owner");//owner
                                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");//location
                                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_site>("localatdi_station_site");//site

                                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_sector_frequency>("localatdi_sector_frequency");//frequencies
                                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_elements_mask>("localatdi_elements_mask");//mask
                                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sector>("localatdi_station_sector");//sectors
                                cdb.Open();
                                for (int j = 0; j < task.data_from_tech[i].TaskItems.Count; j++)
                                {
                                    string sql = "INSERT INTO " + task.data_from_tech[i].task_table_name.ToLower() +
                                        "(id, callsign_db, callsign_radio, standard, status, " +
                                        "license, owner, site, sectors, meas_data_exist) " +
                                        "VALUES(@id, @callsign_db, @callsign_radio, @standard, @status, " +
                                        "@license, @owner, @site, @sectors, @meas_data_exist);";
                                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, cdb))
                                    {

                                        cmd.Parameters.AddRange(new NpgsqlParameter[]
                                        {
                                        #region
                                        new NpgsqlParameter("@id", task.data_from_tech[i].TaskItems[j].id),
                                        new NpgsqlParameter("@callsign_db", task.data_from_tech[i].TaskItems[j].callsign_db),
                                        new NpgsqlParameter("@callsign_radio", task.data_from_tech[i].TaskItems[j].callsign_radio),
                                        new NpgsqlParameter("@standard", task.data_from_tech[i].TaskItems[j].standard),
                                        new NpgsqlParameter("@status", task.data_from_tech[i].TaskItems[j].status),

                                        new NpgsqlParameter("@license", task.data_from_tech[i].TaskItems[j].license),
                                        new NpgsqlParameter("@owner", task.data_from_tech[i].TaskItems[j].owner),
                                        new NpgsqlParameter("@site", task.data_from_tech[i].TaskItems[j].site),
                                        new NpgsqlParameter("@sectors", task.data_from_tech[i].TaskItems[j].sectors.ToArray()),//new localatdi_station_sector[] { }),// 

                                        new NpgsqlParameter("@meas_data_exist", task.data_from_tech[i].TaskItems[j].meas_data_exist)
                                            #endregion
                                        });
                                        cmd.ExecuteNonQuery();
                                    }
                                    task.data_from_tech[i].SectorsCount += task.data_from_tech[i].TaskItems[j].sectors.Count();
                                }
                                cdb.Close();
                            }
                            #endregion
                        }
                        #region пишем таск в таблицу тасков

                        bool addtolocalatdi_meas_task = false;
                        #region
                        using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                        {
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_task_with_tech>("localatdi_task_with_tech");
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_result_state_data>("localatdi_result_state_data");
                            //NpgsqlConnection.MapCompositeGlobally<LocalAtdiTaskWithTech>("atdi_table_name_with_tech");
                            cdb.Open();
                            using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO localatdi_meas_task " +
                                "(task_id, equipment_tech_id, xms_eqipment, priority, scan_per_task_number, sdrn_server, " +
                                "sensor_name, status, time_start, time_stop, time_save, time_last_send_result, " +
                                "with_results_measured, without_results_measured, with_ppe_results_measured, with_ndp_results_measured, " +
                                "task_station_count, task_state, new_result_to_send, task_data_from_tech, routes_tb_name, results_info) " +
                                "VALUES(@task_id, @equipment_tech_id, @xms_eqipment, @priority, @scan_per_task_number, @sdrn_server, " +
                                "@sensor_name, @status, @time_start, @time_stop, @time_save, @time_last_send_result, " +
                                "@with_results_measured, @without_results_measured, @with_ppe_results_measured, @with_ndp_results_measured, " +
                                "@task_station_count, @task_state, @new_result_to_send, @task_data_from_tech, @routes_tb_name, @results_info);", cdb))
                            {
                                #region Command                           
                                cmd.Parameters.AddRange(new NpgsqlParameter[]
                                {
                                 new NpgsqlParameter("@task_id", task.task_id),
                                 new NpgsqlParameter("@equipment_tech_id", task.equipment_tech_id),
                                 new NpgsqlParameter("@xms_eqipment", task.xms_eqipment),
                                 new NpgsqlParameter("@priority", task.priority),
                                 new NpgsqlParameter("@scan_per_task_number", task.scan_per_task_number),
                                 new NpgsqlParameter("@sdrn_server", task.sdrn_server),
                                 new NpgsqlParameter("@sensor_name", task.sensor_name),
                                 new NpgsqlParameter("@status", task.status),
                                 new NpgsqlParameter("@time_start", task.time_start),
                                 new NpgsqlParameter("@time_stop", task.time_stop),
                                 new NpgsqlParameter("@time_save", task.time_save),
                                 new NpgsqlParameter("@time_last_send_result", task.time_last_send_result),
                                 new NpgsqlParameter("@with_results_measured", task.with_results_measured),
                                 new NpgsqlParameter("@without_results_measured", task.without_results_measured),
                                 new NpgsqlParameter("@with_ppe_results_measured", task.with_ppe_results_measured),
                                 new NpgsqlParameter("@with_ndp_results_measured", task.with_ndp_results_measured),
                                 new NpgsqlParameter("@task_station_count", task.task_station_count),
                                 new NpgsqlParameter("@task_state", task.task_state),
                                 new NpgsqlParameter("@new_result_to_send", task.new_result_to_send),
                                 new NpgsqlParameter("@task_data_from_tech", task.data_from_tech.ToArray()),
                                 new NpgsqlParameter("@routes_tb_name", task.routes_tb_name),
                                 new NpgsqlParameter("@results_info", task.ResultsInfo.ToArray()),
                                 //new NpgsqlParameter("@routes", task.routes),
                                });
                                int recordAffected = cmd.ExecuteNonQuery();
                                if (Convert.ToBoolean(recordAffected))
                                {
                                    addtolocalatdi_meas_task = true;
                                    App.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        AtdiTasks.Add(task);

                                    }));
                                }
                                cmd.Dispose();
                                #endregion
                            }
                            cdb.Close();
                            cdb.Dispose();
                        }
                        #endregion
                        #endregion
                        tasks[t].Saved = addtolocalatdi_meas_task;
                    }
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        OnPropertyChanged("LoadedTasks");
                        ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message =
                        ((SplashWindow)App.Current.MainWindow).m_mainWindow.FindResource("MyATDI_ReceivedTaskSaved").ToString().Replace("*tasknumber*", tasknumber); //"Принятый(е) " + tasknumber + " таск сохранен";
                    }));
                }

                if (true) { }
            }
            catch (Exception exp)
            { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
        }


        public bool DeleteTask(localatdi_meas_task task)
        {
            bool res = false;
            try
            {
                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    bool TableExist = false;
                    string SQL = "";
                    #region Table
                    for (int i = 0; i < task.data_from_tech.Count(); i++)
                    {
                        #region task data
                        TableExist = false;
                        SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = " +
                            "pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = '" + task.data_from_tech[i].task_table_name.ToLower() + "';";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            cdb.Open();
                            NpgsqlDataReader dr2 = command.ExecuteReader();
                            while (dr2.Read())
                            {
                                string temp = dr2[0] as string;
                                if (temp.ToLower().Contains(task.data_from_tech[i].task_table_name.ToLower()))
                                {
                                    TableExist = true;
                                }
                            }
                            cdb.Close();
                        }
                        if (TableExist == true)
                        {
                            SQL = "DROP TABLE " + task.data_from_tech[i].task_table_name.ToLower() + ";";
                            cdb.Open();
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                            {
                                command.ExecuteNonQuery();
                            }
                            cdb.Close();
                        }
                        #endregion
                        #region task result
                        TableExist = false;
                        SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = " +
                            "pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = '" + task.data_from_tech[i].result_table_name.ToLower() + "';";
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            cdb.Open();
                            NpgsqlDataReader dr2 = command.ExecuteReader();
                            while (dr2.Read())
                            {
                                string temp = dr2[0] as string;
                                if (temp.ToLower().Contains(task.data_from_tech[i].result_table_name.ToLower()))
                                {
                                    TableExist = true;
                                }
                            }
                            cdb.Close();
                        }
                        if (TableExist == true)
                        {
                            SQL = "DROP TABLE " + task.data_from_tech[i].result_table_name.ToLower() + ";";
                            cdb.Open();
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                            {
                                command.ExecuteNonQuery();
                            }
                            cdb.Close();
                        }
                        #endregion
                    }
                    #endregion
                    #region task route
                    TableExist = false;
                    SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = " +
                        "pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = '" + task.routes_tb_name.ToLower() + "';";
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        cdb.Open();
                        NpgsqlDataReader dr2 = command.ExecuteReader();
                        while (dr2.Read())
                        {
                            string temp = dr2[0] as string;
                            if (temp.ToLower().Contains(task.routes_tb_name.ToLower()))
                            {
                                TableExist = true;
                            }
                        }
                        cdb.Close();
                    }
                    if (TableExist == true)
                    {
                        SQL = "DROP TABLE " + task.routes_tb_name.ToLower() + ";";
                        cdb.Open();
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                        }
                        cdb.Close();
                    }
                    #endregion
                }
                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    #region Task
                    for (int i = 0; i < task.data_from_tech.Count(); i++)
                    {
                        #region task data

                        string SQL = "DELETE FROM localatdi_meas_task WHERE task_id = '" + task.task_id + "';";
                        cdb.Open();
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            int recordAffected = command.ExecuteNonQuery();
                            if (Convert.ToBoolean(recordAffected))
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    AtdiTasks.Remove(task);
                                });
                            }
                        }
                        cdb.Close();
                        #endregion

                    }
                    #endregion
                }
                res = true;
            }
            catch (Exception exp)
            { res = false; MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
            finally { GetSelectedTask(); }
            return res;
        }
        public bool DeleteResults(object datatodel)
        {
            bool res = false;
            localatdi_meas_task task = null;
            localatdi_unknown_result unk = null;
            Track track = null;
            if (datatodel is localatdi_meas_task)
            {
                task = (localatdi_meas_task)datatodel;
            }
            else if (datatodel is localatdi_unknown_result)
            {
                unk = (localatdi_unknown_result)datatodel;
            }
            else if (datatodel is Track)
            {
                track = (Track)datatodel;
            }
            try
            {
                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    string SQL = "";
                    #region Results
                    if (task != null)
                    {
                        for (int i = 0; i < task.data_from_tech.Count(); i++)
                        {
                            #region task result
                            for (int j = 0; j < task.data_from_tech[i].TaskItems.Count(); j++)
                            {
                                task.data_from_tech[i].TaskItems[j].meas_data_exist = false;
                                ATDI_UpdateTaskItemMeasDataExistToDB_v2(
                                    task.data_from_tech[i].TaskItems[j].id,
                                    //task.data_from_tech[i].TaskItems[j].sectors[sec].sector_id,
                                    task.data_from_tech[i].TaskItems[j].callsign_db,
                                    task.data_from_tech[i].TaskItems[j].meas_data_exist,
                                    task.data_from_tech[i].task_table_name);
                            }
                            SQL = "DELETE FROM " + task.data_from_tech[i].result_table_name.ToLower() + ";";
                            cdb.Open();
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                            {
                                command.ExecuteNonQuery();
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    task.data_from_tech[i].ResultItems.Clear();
                                });
                            }
                            cdb.Close();
                            #endregion
                        }
                    }
                    else if (unk != null)
                    {
                        for (int i = 0; i < unk.data_from_tech.Count(); i++)
                        {
                            #region task result
                            SQL = "DELETE FROM " + unk.data_from_tech[i].result_table_name.ToLower() + ";";
                            cdb.Open();
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                            {
                                command.ExecuteNonQuery();
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    unk.data_from_tech[i].ResultItems.Clear();
                                });
                            }
                            cdb.Close();
                            #endregion
                        }
                    }
                    else if (track != null)
                    {

                        #region task result
                        SQL = "DELETE FROM tracks_data WHERE table_name = '" + track.table_name + "';\r\nDROP TABLE " + track.table_name + ";\r\n";// DELETE FROM tracks_data;";
                        cdb.Open();
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                            {
                                TracksData.Remove(track);
                            });
                        }
                        cdb.Close();
                        //SQL = "DROP TABLE " + result_table_name + ";\r\n"; = '" + track.table_name + "';";// DELETE FROM tracks_data;";
                        //cdb.Open();
                        //using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        //{
                        //    command.ExecuteNonQuery();
                        //    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        //    {
                        //        TracksData.Remove(track);
                        //    });
                        //}
                        //cdb.Close();
                        #endregion

                    }
                    #endregion
                    #region task route
                    if (task != null || unk != null)
                    {
                        if (task != null)
                            SQL = "DELETE FROM " + task.routes_tb_name.ToLower() + ";";
                        else if (unk != null)
                            SQL = "DELETE FROM " + unk.routes_tb_name.ToLower() + ";";
                        cdb.Open();
                        using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                        {
                            command.ExecuteNonQuery();
                            if (task != null)
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    task.routes.Clear();
                                });
                            else if (unk != null)
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    unk.routes.Clear();
                                });

                        }
                        cdb.Close();
                    }
                    #endregion
                }
                res = true;
            }
            catch (Exception exp)
            {
                res = false; MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            finally { }
            return res;
        }
        public bool DeleteResults(bool Task, string IdOrYYYYMM)
        {
            bool res = false;
            localatdi_meas_task task = new localatdi_meas_task() { };
            localatdi_unknown_result unk = new localatdi_unknown_result() { };
            if (Task)
                for (int i = 0; i < AtdiTasks.Count(); i++)
                {
                    if (AtdiTasks[i].task_id == IdOrYYYYMM) task = AtdiTasks[i];
                }
            else
                for (int i = 0; i < AtdiUnknownResults.Count(); i++)
                {
                    if (AtdiUnknownResults[i].id == IdOrYYYYMM) unk = AtdiUnknownResults[i];
                }
            try
            {
                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    bool TableExist = false;
                    string SQL = "";

                    #region Results
                    if (Task)
                        for (int i = 0; i < task.data_from_tech.Count(); i++)
                        {
                            #region task result
                            for (int j = 0; j < task.data_from_tech[i].TaskItems.Count(); j++)
                            {
                                task.data_from_tech[i].TaskItems[j].meas_data_exist = false;
                                ATDI_UpdateTaskItemMeasDataExistToDB_v2(
                                    task.data_from_tech[i].TaskItems[j].id,
                                    //task.data_from_tech[i].TaskItems[j].sectors[sec].sector_id,
                                    task.data_from_tech[i].TaskItems[j].callsign_db,
                                    task.data_from_tech[i].TaskItems[j].meas_data_exist,
                                    task.data_from_tech[i].task_table_name);
                            }
                            SQL = "DELETE FROM " + task.data_from_tech[i].result_table_name.ToLower() + ";";
                            cdb.Open();
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                            {
                                command.ExecuteNonQuery();
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    task.data_from_tech[i].ResultItems.Clear();
                                });
                            }
                            cdb.Close();
                            #endregion
                        }
                    else
                        for (int i = 0; i < unk.data_from_tech.Count(); i++)
                        {
                            #region task result
                            SQL = "DELETE FROM " + unk.data_from_tech[i].result_table_name.ToLower() + ";";
                            cdb.Open();
                            using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                            {
                                command.ExecuteNonQuery();
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    unk.data_from_tech[i].ResultItems.Clear();
                                });
                            }
                            cdb.Close();
                            #endregion
                        }
                    #endregion
                    #region task route
                    if (Task)
                        SQL = "DELETE FROM " + task.routes_tb_name.ToLower() + ";";
                    else
                        SQL = "DELETE FROM " + unk.routes_tb_name.ToLower() + ";";
                    cdb.Open();
                    using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                    {
                        command.ExecuteNonQuery();
                        if (Task)
                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                            {
                                task.routes.Clear();
                            });
                        else
                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                            {
                                unk.routes.Clear();
                            });

                    }
                    cdb.Close();
                    #endregion
                }
                res = true;
            }
            catch (Exception exp)
            {
                res = false; MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            finally { }
            return res;
        }

        public void GetSelectedTask()
        {
            MeasMon.FromTask = false;
            for (int i = 0; i < MainWindow.db_v2.AtdiTasks.Count(); i++)
            {
                if (MeasMon.FromTask == false && MainWindow.db_v2.AtdiTasks[i].task_state == 2)
                {
                    MeasMon.FromTask = true;
                    AtdiTask = MainWindow.db_v2.AtdiTasks[i];
                }
            }
            dbt -= GetSelectedTask;
        }
        public int MeasMonCheckSelectedTask()
        {
            int res = 0;
            MeasMon.FromTask = false;
            for (int i = 0; i < MainWindow.db_v2.AtdiTasks.Count(); i++)
            {
                if (MainWindow.db_v2.AtdiTasks[i].task_state == 2)
                {
                    MeasMon.FromTask = true;
                    AtdiTask = MainWindow.db_v2.AtdiTasks[i];
                    res++;
                }
            }
            return res;
        }
        #endregion работа с тасками


        #region сохранение результатов

        public bool MonMeasSaveAll = false;
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        #region Tracks
        private void LoadTracksTableResults()
        {
            TracksData = new ObservableCollection<Track>();
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                NpgsqlCommand command;
                NpgsqlDataReader dr;
                cdb.Open();
                #region Data
                #region узнаем количевство строк
                using (command = new NpgsqlCommand("SELECT COUNT(*) FROM tracks_data;", cdb))
                {
                    try
                    {
                        dr = command.ExecuteReader();
                        dr.Read();
                        MaxLoadProgressBar += Int32.Parse(dr["count"].ToString());
                    }
                    catch (Exception exp)
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }
                }
                #endregion
                cdb.Close();
                try
                {
                    #region читать tracks_data
                    cdb.Open();
                    using (command = new NpgsqlCommand("SELECT * FROM tracks_data;", cdb))
                    {
                        dr = command.ExecuteReader();
                        ObservableCollection<Track> tsks = new ObservableCollection<Track>();
                        while (dr.Read())
                        {
                            Track tr = new Track() { };

                            tr.name = (string)dr["name"];
                            tr.time_start = (DateTime)dr["time_start"];
                            tr.time_stop = (DateTime)dr["time_stop"];
                            tr.table_name = (string)dr["table_name"];
                            tsks.Add(tr);
                        }
                        //сортируем по убыванию meas_task_id
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            TracksData = new ObservableCollection<Track>(tsks.OrderByDescending(i => i.time_start));
                        });
                    }
                    cdb.Close();
                    #endregion

                    #region читаем таблицы
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_level_meas_result>("localatdi_level_meas_result");
                    NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
                    for (int j = 0; j < TracksData.Count(); j++)
                    {
                        try
                        {
                            NpgsqlCommand com;
                            cdb.Open();
                            using (com = new NpgsqlCommand("SELECT * FROM " + TracksData[j].table_name.ToLower() + ";", cdb))
                            #region
                            {
                                dr = com.ExecuteReader();
                                while (dr.Read())
                                {
                                    #region
                                    TrackData data = new TrackData() { };

                                    data.tech = (string)dr["tech"];
                                    data.freq = (decimal)dr["freq"];
                                    data.gcid = (string)dr["gcid"];
                                    data.identifier_sub_ind = (int)dr["identifier_sub_ind"];
                                    data.level_results = new ObservableCollection<localatdi_level_meas_result>((localatdi_level_meas_result[])dr["level_results"]);
                                    data.information_blocks = (local3GPPSystemInformationBlock[])dr["information_blocks"];

                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                    {
                                        TracksData[j].Data.Add(data);
                                    });
                                    #endregion
                                }
                            }
                            #endregion
                            cdb.Close();
                        }
                        catch (Exception exp)
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }
                        finally { cdb.Close(); }
                    }
                    #endregion
                }
                catch (Exception exp)
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }
                finally
                {
                    cdb.Close();
                }
                #endregion
            }
            dbt -= LoadTracksTableResults;
        }
        private void AddTrackData()
        {
            try
            {
                Track td = new Track()
                {
                    time_start = DateTime.Now,
                    time_stop = DateTime.Now,
                };
                string tb_name = "trackdata";
                string name = "";
                if (Equipment.IdentificationData.GSM.TechIsEnabled)
                {
                    tb_name += "_gsm";
                    name += "GSM;";
                }
                if (Equipment.IdentificationData.UMTS.TechIsEnabled)
                {
                    tb_name += "_umts";
                    name += "UMTS;";
                }
                if (Equipment.IdentificationData.CDMA.TechIsEnabled)
                {
                    tb_name += "_cdma";
                    name += "CDMA;";
                }
                if (Equipment.IdentificationData.LTE.TechIsEnabled)
                {
                    tb_name += "_lte";
                    name += "LTE;";
                }
                tb_name += td.time_start.ToString("_yyyyMMdd_HHmmss");
                td.table_name = tb_name;
                td.name = name;
                using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                {
                    string SQL = "";
                    SQL = "CREATE TABLE public." + td.table_name + "(tech character varying(50), freq numeric, " +
                        "gcid character varying(50), identifier_sub_ind integer, level_results localatdi_level_meas_result[], " +
                        "information_blocks local_3gpp_system_information_block[]" +
                        ") WITH (OIDS = FALSE);";
                    cdb.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    cdb.Close();



                    try
                    {
                        cdb.Open();
                        SQL = "INSERT INTO tracks_data" +
                           "(name, time_start, time_stop, table_name)" +
                           "VALUES (" +
                           "@name, @time_start, @time_stop, @table_name);";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                        {
                            cmd.Parameters.AddRange(
                                new NpgsqlParameter[]
                                {
                                    #region
                                    new NpgsqlParameter("@name", td.name),
                                    new NpgsqlParameter("@time_start", td.time_start),
                                    new NpgsqlParameter("@time_stop", td.time_stop),
                                    new NpgsqlParameter("@table_name", td.table_name),
                                    #endregion
                                });
                            cmd.ExecuteNonQuery();
                        }
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            TracksDataSelected = td;
                            TracksData.Add(TracksDataSelected);
                        }));

                    }
                    catch (NpgsqlException exp)
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    catch (Exception exp)
                    {
                        App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                        }));
                    }
                    finally
                    {
                        if (cdb != null) cdb.Close();
                    }
                }
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            finally
            {
                //long ttt = DateTime.Now.Ticks;
                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Data Track ADD" + "\r\nLevelCar ";
                //}));
                //ddddd = DateTime.Now.Ticks;
                dbt -= AddTrackData;
            }
        }
        private void SaveUpdateTrackData()
        {
            try
            {
                //NpgsqlConnection.GlobalTypeMapper.Reset();
                //NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
                //NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_level_meas_result>("localatdi_level_meas_result");
                //NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
                using (NpgsqlConnection ncdb = new NpgsqlConnection(UserconnToDb))
                {
                    ncdb.Open();
                    using (NpgsqlTransaction oTransaction = ncdb.BeginTransaction())
                    {
                        using (NpgsqlCommand cmd = ncdb.CreateCommand())
                        {
                            try
                            {
                                cmd.Transaction = oTransaction;
                                #region GSM
                                for (int i = 0; i < Equipment.IdentificationData.GSM.BTS.Count(); i++)
                                {
                                    if (Equipment.IdentificationData.GSM.BTS[i].FullData &&
                                        Equipment.IdentificationData.GSM.BTS[i].level_results.Count() > 0 &&
                                        Equipment.IdentificationData.GSM.BTS[i].level_results[Equipment.IdentificationData.GSM.BTS[i].level_results.Count() - 1].saved_in_db == false)
                                    {
                                        bool find = false;
                                        //List<localatdi_level_meas_result> lrsTemp = Equipment.IdentificationData.GSM.BTS[i].level_results.ToList();
                                        for (int t = 0; t < TracksDataSelected.Data.Count(); t++)
                                        {
                                            #region Updete
                                            if (TracksDataSelected.Data[t].freq == Equipment.IdentificationData.GSM.BTS[i].FreqDn &&
                                                TracksDataSelected.Data[t].tech == "GSM" &&
                                                TracksDataSelected.Data[t].identifier_sub_ind == Equipment.IdentificationData.GSM.BTS[i].BSIC &&
                                                TracksDataSelected.Data[t].gcid == Equipment.IdentificationData.GSM.BTS[i].GCID)
                                            {
                                                find = true;
                                                List<localatdi_level_meas_result> lrs = new List<localatdi_level_meas_result>() { };
                                                for (int l = 0; l < Equipment.IdentificationData.GSM.BTS[i].level_results.Count(); l++)
                                                {
                                                    if (!Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_db)
                                                    {
                                                        Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_db = true;
                                                        Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_result = true;
                                                        localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = Equipment.IdentificationData.GSM.BTS[i].level_results[l].difference_time_stamp_ns,
                                                            level_dbm = Equipment.IdentificationData.GSM.BTS[i].level_results[l].level_dbm,
                                                            level_dbmkvm = Equipment.IdentificationData.GSM.BTS[i].level_results[l].level_dbmkvm,
                                                            measurement_time = Equipment.IdentificationData.GSM.BTS[i].level_results[l].measurement_time,
                                                            saved_in_db = Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_db,
                                                            saved_in_result = Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_result,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.agl,
                                                                asl = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.asl,
                                                                longitude = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.longitude,
                                                                latitude = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.latitude,
                                                            }
                                                        };
                                                        lrs.Add(lr);
                                                    }
                                                }
                                                cmd.CommandText = "UPDATE " + TracksDataSelected.table_name + " SET information_blocks = @information_blocks, level_results = array_cat(level_results, @level_results) " +
                                                    "WHERE tech = @tech and freq = @freq and identifier_sub_ind = @identifier_sub_ind and gcid = @gcid;";
                                                cmd.Parameters.Clear();
                                                cmd.Parameters.AddRange(
                                                    new NpgsqlParameter[]
                                                    {
                                                        #region
                                                        new NpgsqlParameter{ParameterName = "@tech", Value = TracksDataSelected.Data[t].tech },
                                                        new NpgsqlParameter{ParameterName = "@freq", Value = TracksDataSelected.Data[t].freq },
                                                        new NpgsqlParameter{ParameterName = "@gcid", Value = TracksDataSelected.Data[t].gcid },
                                                        new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = TracksDataSelected.Data[t].identifier_sub_ind },
                                                        new NpgsqlParameter{ParameterName = "@level_results", Value = lrs.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                        new NpgsqlParameter{ParameterName = "@information_blocks", Value = Equipment.IdentificationData.GSM.BTS[i].station_sys_info.information_blocks.ToArray(), DataTypeName ="local_3gpp_system_information_block[]"},

                                                        //new NpgsqlParameter("@tech", TracksDataSelected.Data[t].tech),
                                                        //new NpgsqlParameter("@freq", TracksDataSelected.Data[t].freq),
                                                        //new NpgsqlParameter("@gcid", TracksDataSelected.Data[t].gcid),
                                                        //new NpgsqlParameter("@identifier_sub_ind", TracksDataSelected.Data[t].identifier_sub_ind),
                                                        //new NpgsqlParameter("@information_blocks", Equipment.IdentificationData.GSM.BTS[i].station_sys_info.information_blocks.ToArray()),
                                                        //new NpgsqlParameter("@level_results", lrs.ToArray()),
                                                        #endregion
                                                    });

                                                int k = cmd.ExecuteNonQuery();
                                                if (k > 0)
                                                {
                                                    for (int o = 0; o < lrs.Count(); o++)
                                                    {
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            TracksDataSelected.Data[t].level_results.Add(lrs[o]);
                                                        });
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        #region new
                                        if (!find)
                                        {
                                            TrackData td = new TrackData()
                                            {
                                                freq = Equipment.IdentificationData.GSM.BTS[i].FreqDn,
                                                gcid = Equipment.IdentificationData.GSM.BTS[i].GCID,
                                                identifier_sub_ind = Equipment.IdentificationData.GSM.BTS[i].BSIC,
                                                tech = "GSM"
                                            };
                                            //НЕЗАБЫВАЙ ПРО ССЫЛОЧНЫЕ ТИПЫ
                                            for (int l = 0; l < Equipment.IdentificationData.GSM.BTS[i].level_results.Count(); l++)
                                            {
                                                Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_db = true;
                                                Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_result = true;
                                                localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = Equipment.IdentificationData.GSM.BTS[i].level_results[l].difference_time_stamp_ns,
                                                    level_dbm = Equipment.IdentificationData.GSM.BTS[i].level_results[l].level_dbm,
                                                    level_dbmkvm = Equipment.IdentificationData.GSM.BTS[i].level_results[l].level_dbmkvm,
                                                    measurement_time = Equipment.IdentificationData.GSM.BTS[i].level_results[l].measurement_time,
                                                    saved_in_db = Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_db,
                                                    saved_in_result = Equipment.IdentificationData.GSM.BTS[i].level_results[l].saved_in_result,
                                                    location = new localatdi_geo_location()
                                                    {
                                                        agl = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.agl,
                                                        asl = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.asl,
                                                        longitude = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.longitude,
                                                        latitude = Equipment.IdentificationData.GSM.BTS[i].level_results[l].location.latitude,
                                                    }
                                                };
                                                td.level_results.Add(lr);
                                            }
                                            td.information_blocks = new local3GPPSystemInformationBlock[Equipment.IdentificationData.GSM.BTS[i].station_sys_info.information_blocks.Count()];
                                            for (int b = 0; b < Equipment.IdentificationData.GSM.BTS[i].station_sys_info.information_blocks.Count(); b++)
                                            {
                                                local3GPPSystemInformationBlock ib = new local3GPPSystemInformationBlock()
                                                {
                                                    datastring = Equipment.IdentificationData.GSM.BTS[i].station_sys_info.information_blocks[b].datastring,
                                                    saved = Equipment.IdentificationData.GSM.BTS[i].station_sys_info.information_blocks[b].saved,
                                                    type = Equipment.IdentificationData.GSM.BTS[i].station_sys_info.information_blocks[b].type,
                                                };
                                                td.information_blocks[b] = ib;
                                            }
                                            cmd.Parameters.Clear();
                                            cmd.CommandText = "INSERT INTO " + TracksDataSelected.table_name + " (tech, freq, gcid, identifier_sub_ind, level_results, information_blocks) " +
                                                "VALUES (@tech, @freq, @gcid, @identifier_sub_ind, @level_results, @information_blocks);";
                                            cmd.Parameters.AddRange(
                                                new NpgsqlParameter[]
                                                {
                                                    #region
                                                    new NpgsqlParameter{ParameterName = "@tech", Value = td.tech },
                                                    new NpgsqlParameter{ParameterName = "@freq", Value = td.freq },
                                                    new NpgsqlParameter{ParameterName = "@gcid", Value = td.gcid },
                                                    new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = td.identifier_sub_ind },
                                                    new NpgsqlParameter{ParameterName = "@level_results", Value = td.level_results.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                    new NpgsqlParameter{ParameterName = "@information_blocks", Value = td.information_blocks, DataTypeName ="local_3gpp_system_information_block[]"},

                                                    //new NpgsqlParameter("@tech", td.tech),
                                                    //new NpgsqlParameter("@freq", td.freq),
                                                    //new NpgsqlParameter("@gcid", td.gcid),
                                                    //new NpgsqlParameter("@identifier_sub_ind", td.identifier_sub_ind),
                                                    //new NpgsqlParameter("@level_results", td.level_results.ToArray()),
                                                    //new NpgsqlParameter("@information_blocks", td.information_blocks.ToArray()),
                                                    
                                                    #endregion
                                                });
                                            if (cmd.ExecuteNonQuery() != 1)
                                            {
                                                //'handled as needed, 
                                                //' but this snippet will throw an exception to force a rollback
                                                throw new InvalidProgramException();
                                            }
                                            else
                                            {
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    TracksDataSelected.Data.Add(td);
                                                });

                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion GSM
                                #region UMTS
                                for (int i = 0; i < Equipment.IdentificationData.UMTS.BTS.Count(); i++)
                                {
                                    if (Equipment.IdentificationData.UMTS.BTS[i].FullData &&
                                        Equipment.IdentificationData.UMTS.BTS[i].level_results.Count() > 0 &&
                                        Equipment.IdentificationData.UMTS.BTS[i].level_results[Equipment.IdentificationData.UMTS.BTS[i].level_results.Count() - 1].saved_in_db == false)
                                    {
                                        bool find = false;
                                        //List<localatdi_level_meas_result> lrsTemp = Equipment.IdentificationData.GSM.BTS[i].level_results.ToList();
                                        for (int t = 0; t < TracksDataSelected.Data.Count(); t++)
                                        {
                                            #region Updete
                                            if (TracksDataSelected.Data[t].freq == Equipment.IdentificationData.UMTS.BTS[i].FreqDn &&
                                                TracksDataSelected.Data[t].tech == "UMTS" &&
                                                TracksDataSelected.Data[t].identifier_sub_ind == Equipment.IdentificationData.UMTS.BTS[i].SC &&
                                                TracksDataSelected.Data[t].gcid == Equipment.IdentificationData.UMTS.BTS[i].GCID)
                                            {
                                                find = true;
                                                List<localatdi_level_meas_result> lrs = new List<localatdi_level_meas_result>() { };
                                                for (int l = 0; l < Equipment.IdentificationData.UMTS.BTS[i].level_results.Count(); l++)
                                                {
                                                    if (!Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_db)
                                                    {
                                                        Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_db = true;
                                                        Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_result = true;
                                                        localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].difference_time_stamp_ns,
                                                            level_dbm = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].level_dbm,
                                                            level_dbmkvm = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].level_dbmkvm,
                                                            measurement_time = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].measurement_time,
                                                            saved_in_db = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_db,
                                                            saved_in_result = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_result,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.agl,
                                                                asl = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.asl,
                                                                longitude = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.longitude,
                                                                latitude = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.latitude,
                                                            }
                                                        };
                                                        lrs.Add(lr);
                                                    }
                                                }
                                                cmd.CommandText = "UPDATE " + TracksDataSelected.table_name + " SET information_blocks = @information_blocks, level_results = array_cat(level_results, @level_results) " +
                                                    "WHERE tech = @tech and freq = @freq and identifier_sub_ind = @identifier_sub_ind and gcid = @gcid;";
                                                cmd.Parameters.Clear();
                                                cmd.Parameters.AddRange(
                                                    new NpgsqlParameter[]
                                                    {
                                                        #region
                                                        new NpgsqlParameter{ParameterName = "@tech", Value = TracksDataSelected.Data[t].tech },
                                                        new NpgsqlParameter{ParameterName = "@freq", Value = TracksDataSelected.Data[t].freq },
                                                        new NpgsqlParameter{ParameterName = "@gcid", Value = TracksDataSelected.Data[t].gcid },
                                                        new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = TracksDataSelected.Data[t].identifier_sub_ind },
                                                        new NpgsqlParameter{ParameterName = "@level_results", Value = lrs.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                        new NpgsqlParameter{ParameterName = "@information_blocks", Value = Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks.ToArray(), DataTypeName ="local_3gpp_system_information_block[]"},

                                                        //new NpgsqlParameter("@tech", TracksDataSelected.Data[t].tech),
                                                        //new NpgsqlParameter("@freq", TracksDataSelected.Data[t].freq),
                                                        //new NpgsqlParameter("@gcid", TracksDataSelected.Data[t].gcid),
                                                        //new NpgsqlParameter("@identifier_sub_ind", TracksDataSelected.Data[t].identifier_sub_ind),
                                                        //new NpgsqlParameter("@information_blocks", Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks.ToArray()),
                                                        //new NpgsqlParameter("@level_results", lrs.ToArray()),
                                                        #endregion
                                                    });

                                                int k = cmd.ExecuteNonQuery();
                                                if (k > 0)
                                                {
                                                    for (int o = 0; o < lrs.Count(); o++)
                                                    {
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            TracksDataSelected.Data[t].level_results.Add(lrs[o]);
                                                        });
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        #region new
                                        if (!find)
                                        {
                                            TrackData td = new TrackData()
                                            {
                                                freq = Equipment.IdentificationData.UMTS.BTS[i].FreqDn,
                                                gcid = Equipment.IdentificationData.UMTS.BTS[i].GCID,
                                                identifier_sub_ind = Equipment.IdentificationData.UMTS.BTS[i].SC,
                                                tech = "UMTS"
                                            };
                                            //НЕЗАБЫВАЙ ПРО ССЫЛОЧНЫЕ ТИПЫ
                                            for (int l = 0; l < Equipment.IdentificationData.UMTS.BTS[i].level_results.Count(); l++)
                                            {
                                                Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_db = true;
                                                Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_result = true;
                                                localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].difference_time_stamp_ns,
                                                    level_dbm = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].level_dbm,
                                                    level_dbmkvm = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].level_dbmkvm,
                                                    measurement_time = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].measurement_time,
                                                    saved_in_db = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_db,
                                                    saved_in_result = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].saved_in_result,
                                                    location = new localatdi_geo_location()
                                                    {
                                                        agl = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.agl,
                                                        asl = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.asl,
                                                        longitude = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.longitude,
                                                        latitude = Equipment.IdentificationData.UMTS.BTS[i].level_results[l].location.latitude,
                                                    }
                                                };
                                                td.level_results.Add(lr);
                                            }
                                            td.information_blocks = new local3GPPSystemInformationBlock[Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks.Count()];
                                            for (int b = 0; b < Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks.Count(); b++)
                                            {
                                                local3GPPSystemInformationBlock ib = new local3GPPSystemInformationBlock()
                                                {
                                                    datastring = Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks[b].datastring,
                                                    saved = Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks[b].saved,
                                                    type = Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks[b].type,
                                                };
                                                td.information_blocks[b] = ib;
                                            }
                                            cmd.Parameters.Clear();
                                            cmd.CommandText = "INSERT INTO " + TracksDataSelected.table_name + " (tech, freq, gcid, identifier_sub_ind, level_results, information_blocks) " +
                                                "VALUES (@tech, @freq, @gcid, @identifier_sub_ind, @level_results, @information_blocks);";
                                            cmd.Parameters.AddRange(
                                                new NpgsqlParameter[]
                                                {
                                                    #region
                                                    new NpgsqlParameter{ParameterName = "@tech", Value = td.tech },
                                                    new NpgsqlParameter{ParameterName = "@freq", Value = td.freq },
                                                    new NpgsqlParameter{ParameterName = "@gcid", Value = td.gcid },
                                                    new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = td.identifier_sub_ind },
                                                    new NpgsqlParameter{ParameterName = "@level_results", Value = td.level_results.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                    new NpgsqlParameter{ParameterName = "@information_blocks", Value = td.information_blocks, DataTypeName ="local_3gpp_system_information_block[]"},

                                                    //new NpgsqlParameter("@tech", td.tech),
                                                    //new NpgsqlParameter("@freq", td.freq),
                                                    //new NpgsqlParameter("@gcid", td.gcid),
                                                    //new NpgsqlParameter("@identifier_sub_ind", td.identifier_sub_ind),
                                                    //new NpgsqlParameter("@level_results", td.level_results.ToArray()),
                                                    //new NpgsqlParameter("@information_blocks", td.information_blocks.ToArray()),
                                                    
                                                    #endregion
                                                });
                                            if (cmd.ExecuteNonQuery() != 1)
                                            {
                                                //'handled as needed, 
                                                //' but this snippet will throw an exception to force a rollback
                                                throw new InvalidProgramException();
                                            }
                                            else
                                            {
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    TracksDataSelected.Data.Add(td);
                                                });

                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion UMTS
                                #region LTE
                                for (int i = 0; i < Equipment.IdentificationData.LTE.BTS.Count(); i++)
                                {
                                    if (Equipment.IdentificationData.LTE.BTS[i].FullData &&
                                        Equipment.IdentificationData.LTE.BTS[i].level_results.Count() > 0 &&
                                        Equipment.IdentificationData.LTE.BTS[i].level_results[Equipment.IdentificationData.LTE.BTS[i].level_results.Count() - 1].saved_in_db == false)
                                    {
                                        bool find = false;
                                        //List<localatdi_level_meas_result> lrsTemp = Equipment.IdentificationData.GSM.BTS[i].level_results.ToList();
                                        for (int t = 0; t < TracksDataSelected.Data.Count(); t++)
                                        {
                                            #region Updete
                                            if (TracksDataSelected.Data[t].freq == Equipment.IdentificationData.LTE.BTS[i].FreqDn &&
                                                TracksDataSelected.Data[t].tech == "LTE" &&
                                                TracksDataSelected.Data[t].identifier_sub_ind == Equipment.IdentificationData.LTE.BTS[i].PCI &&
                                                TracksDataSelected.Data[t].gcid == Equipment.IdentificationData.LTE.BTS[i].GCID)
                                            {
                                                find = true;
                                                List<localatdi_level_meas_result> lrs = new List<localatdi_level_meas_result>() { };
                                                for (int l = 0; l < Equipment.IdentificationData.LTE.BTS[i].level_results.Count(); l++)
                                                {
                                                    if (!Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_db)
                                                    {
                                                        Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_db = true;
                                                        Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_result = true;
                                                        localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = Equipment.IdentificationData.LTE.BTS[i].level_results[l].difference_time_stamp_ns,
                                                            level_dbm = Equipment.IdentificationData.LTE.BTS[i].level_results[l].level_dbm,
                                                            level_dbmkvm = Equipment.IdentificationData.LTE.BTS[i].level_results[l].level_dbmkvm,
                                                            measurement_time = Equipment.IdentificationData.LTE.BTS[i].level_results[l].measurement_time,
                                                            saved_in_db = Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_db,
                                                            saved_in_result = Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_result,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.agl,
                                                                asl = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.asl,
                                                                longitude = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.longitude,
                                                                latitude = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.latitude,
                                                            }
                                                        };
                                                        lrs.Add(lr);
                                                    }
                                                }
                                                cmd.CommandText = "UPDATE " + TracksDataSelected.table_name + " SET information_blocks = @information_blocks, level_results = array_cat(level_results, @level_results) " +
                                                    "WHERE tech = @tech and freq = @freq and identifier_sub_ind = @identifier_sub_ind and gcid = @gcid;";
                                                cmd.Parameters.Clear();
                                                cmd.Parameters.AddRange(
                                                    new NpgsqlParameter[]
                                                    {
                                                        #region
                                                        new NpgsqlParameter{ParameterName = "@tech", Value = TracksDataSelected.Data[t].tech },
                                                        new NpgsqlParameter{ParameterName = "@freq", Value = TracksDataSelected.Data[t].freq },
                                                        new NpgsqlParameter{ParameterName = "@gcid", Value = TracksDataSelected.Data[t].gcid },
                                                        new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = TracksDataSelected.Data[t].identifier_sub_ind },
                                                        new NpgsqlParameter{ParameterName = "@level_results", Value = lrs.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                        new NpgsqlParameter{ParameterName = "@information_blocks", Value = Equipment.IdentificationData.LTE.BTS[i].station_sys_info.information_blocks.ToArray(), DataTypeName ="local_3gpp_system_information_block[]"},

                                                        //new NpgsqlParameter("@tech", TracksDataSelected.Data[t].tech),
                                                        //new NpgsqlParameter("@freq", TracksDataSelected.Data[t].freq),
                                                        //new NpgsqlParameter("@gcid", TracksDataSelected.Data[t].gcid),
                                                        //new NpgsqlParameter("@identifier_sub_ind", TracksDataSelected.Data[t].identifier_sub_ind),
                                                        //new NpgsqlParameter("@information_blocks", Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks.ToArray()),
                                                        //new NpgsqlParameter("@level_results", lrs.ToArray()),
                                                        #endregion
                                                    });

                                                int k = cmd.ExecuteNonQuery();
                                                if (k > 0)
                                                {
                                                    for (int o = 0; o < lrs.Count(); o++)
                                                    {
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            TracksDataSelected.Data[t].level_results.Add(lrs[o]);
                                                        });
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        #region new
                                        if (!find)
                                        {
                                            TrackData td = new TrackData()
                                            {
                                                freq = Equipment.IdentificationData.LTE.BTS[i].FreqDn,
                                                gcid = Equipment.IdentificationData.LTE.BTS[i].GCID,
                                                identifier_sub_ind = Equipment.IdentificationData.LTE.BTS[i].PCI,
                                                tech = "LTE"
                                            };
                                            //НЕЗАБЫВАЙ ПРО ССЫЛОЧНЫЕ ТИПЫ
                                            for (int l = 0; l < Equipment.IdentificationData.LTE.BTS[i].level_results.Count(); l++)
                                            {
                                                Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_db = true;
                                                Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_result = true;
                                                localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = Equipment.IdentificationData.LTE.BTS[i].level_results[l].difference_time_stamp_ns,
                                                    level_dbm = Equipment.IdentificationData.LTE.BTS[i].level_results[l].level_dbm,
                                                    level_dbmkvm = Equipment.IdentificationData.LTE.BTS[i].level_results[l].level_dbmkvm,
                                                    measurement_time = Equipment.IdentificationData.LTE.BTS[i].level_results[l].measurement_time,
                                                    saved_in_db = Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_db,
                                                    saved_in_result = Equipment.IdentificationData.LTE.BTS[i].level_results[l].saved_in_result,
                                                    location = new localatdi_geo_location()
                                                    {
                                                        agl = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.agl,
                                                        asl = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.asl,
                                                        longitude = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.longitude,
                                                        latitude = Equipment.IdentificationData.LTE.BTS[i].level_results[l].location.latitude,
                                                    }
                                                };
                                                td.level_results.Add(lr);
                                            }
                                            td.information_blocks = new local3GPPSystemInformationBlock[Equipment.IdentificationData.LTE.BTS[i].station_sys_info.information_blocks.Count()];
                                            for (int b = 0; b < Equipment.IdentificationData.LTE.BTS[i].station_sys_info.information_blocks.Count(); b++)
                                            {
                                                local3GPPSystemInformationBlock ib = new local3GPPSystemInformationBlock()
                                                {
                                                    datastring = Equipment.IdentificationData.LTE.BTS[i].station_sys_info.information_blocks[b].datastring,
                                                    saved = Equipment.IdentificationData.LTE.BTS[i].station_sys_info.information_blocks[b].saved,
                                                    type = Equipment.IdentificationData.LTE.BTS[i].station_sys_info.information_blocks[b].type,
                                                };
                                                td.information_blocks[b] = ib;
                                            }
                                            cmd.Parameters.Clear();
                                            cmd.CommandText = "INSERT INTO " + TracksDataSelected.table_name + " (tech, freq, gcid, identifier_sub_ind, level_results, information_blocks) " +
                                                "VALUES (@tech, @freq, @gcid, @identifier_sub_ind, @level_results, @information_blocks);";
                                            cmd.Parameters.AddRange(
                                                new NpgsqlParameter[]
                                                {
                                                    #region
                                                    new NpgsqlParameter{ParameterName = "@tech", Value = td.tech },
                                                    new NpgsqlParameter{ParameterName = "@freq", Value = td.freq },
                                                    new NpgsqlParameter{ParameterName = "@gcid", Value = td.gcid },
                                                    new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = td.identifier_sub_ind },
                                                    new NpgsqlParameter{ParameterName = "@level_results", Value = td.level_results.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                    new NpgsqlParameter{ParameterName = "@information_blocks", Value = td.information_blocks, DataTypeName ="local_3gpp_system_information_block[]"},

                                                    //new NpgsqlParameter("@tech", td.tech),
                                                    //new NpgsqlParameter("@freq", td.freq),
                                                    //new NpgsqlParameter("@gcid", td.gcid),
                                                    //new NpgsqlParameter("@identifier_sub_ind", td.identifier_sub_ind),
                                                    //new NpgsqlParameter("@level_results", td.level_results.ToArray()),
                                                    //new NpgsqlParameter("@information_blocks", td.information_blocks.ToArray()),
                                                    
                                                    #endregion
                                                });
                                            if (cmd.ExecuteNonQuery() != 1)
                                            {
                                                //'handled as needed, 
                                                //' but this snippet will throw an exception to force a rollback
                                                throw new InvalidProgramException();
                                            }
                                            else
                                            {
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    TracksDataSelected.Data.Add(td);
                                                });

                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion LTE
                                #region CDMA
                                for (int i = 0; i < Equipment.IdentificationData.CDMA.BTS.Count(); i++)
                                {
                                    if (Equipment.IdentificationData.CDMA.BTS[i].FullData &&
                                        Equipment.IdentificationData.CDMA.BTS[i].level_results.Count() > 0 &&
                                        Equipment.IdentificationData.CDMA.BTS[i].level_results[Equipment.IdentificationData.CDMA.BTS[i].level_results.Count() - 1].saved_in_db == false)
                                    {
                                        bool find = false;
                                        //List<localatdi_level_meas_result> lrsTemp = Equipment.IdentificationData.GSM.BTS[i].level_results.ToList();
                                        for (int t = 0; t < TracksDataSelected.Data.Count(); t++)
                                        {
                                            #region Updete
                                            if (TracksDataSelected.Data[t].freq == Equipment.IdentificationData.CDMA.BTS[i].FreqDn &&
                                                TracksDataSelected.Data[t].tech == "CDMA" &&
                                                TracksDataSelected.Data[t].identifier_sub_ind == Equipment.IdentificationData.CDMA.BTS[i].PN &&
                                                TracksDataSelected.Data[t].gcid == Equipment.IdentificationData.CDMA.BTS[i].GCID)
                                            {
                                                find = true;
                                                List<localatdi_level_meas_result> lrs = new List<localatdi_level_meas_result>() { };
                                                for (int l = 0; l < Equipment.IdentificationData.CDMA.BTS[i].level_results.Count(); l++)
                                                {
                                                    if (!Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_db)
                                                    {
                                                        Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_db = true;
                                                        Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_result = true;
                                                        localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].difference_time_stamp_ns,
                                                            level_dbm = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].level_dbm,
                                                            level_dbmkvm = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].level_dbmkvm,
                                                            measurement_time = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].measurement_time,
                                                            saved_in_db = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_db,
                                                            saved_in_result = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_result,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.agl,
                                                                asl = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.asl,
                                                                longitude = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.longitude,
                                                                latitude = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.latitude,
                                                            }
                                                        };
                                                        lrs.Add(lr);
                                                    }
                                                }
                                                cmd.CommandText = "UPDATE " + TracksDataSelected.table_name + " SET information_blocks = @information_blocks, level_results = array_cat(level_results, @level_results) " +
                                                    "WHERE tech = @tech and freq = @freq and identifier_sub_ind = @identifier_sub_ind and gcid = @gcid;";
                                                cmd.Parameters.Clear();
                                                cmd.Parameters.AddRange(
                                                    new NpgsqlParameter[]
                                                    {
                                                        #region
                                                        new NpgsqlParameter{ParameterName = "@tech", Value = TracksDataSelected.Data[t].tech },
                                                        new NpgsqlParameter{ParameterName = "@freq", Value = TracksDataSelected.Data[t].freq },
                                                        new NpgsqlParameter{ParameterName = "@gcid", Value = TracksDataSelected.Data[t].gcid },
                                                        new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = TracksDataSelected.Data[t].identifier_sub_ind },
                                                        new NpgsqlParameter{ParameterName = "@level_results", Value = lrs.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                        new NpgsqlParameter{ParameterName = "@information_blocks", Value = Equipment.IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks.ToArray(), DataTypeName ="local_3gpp_system_information_block[]"},

                                                        //new NpgsqlParameter("@tech", TracksDataSelected.Data[t].tech),
                                                        //new NpgsqlParameter("@freq", TracksDataSelected.Data[t].freq),
                                                        //new NpgsqlParameter("@gcid", TracksDataSelected.Data[t].gcid),
                                                        //new NpgsqlParameter("@identifier_sub_ind", TracksDataSelected.Data[t].identifier_sub_ind),
                                                        //new NpgsqlParameter("@information_blocks", Equipment.IdentificationData.UMTS.BTS[i].station_sys_info.information_blocks.ToArray()),
                                                        //new NpgsqlParameter("@level_results", lrs.ToArray()),
                                                        #endregion
                                                    });

                                                int k = cmd.ExecuteNonQuery();
                                                if (k > 0)
                                                {
                                                    for (int o = 0; o < lrs.Count(); o++)
                                                    {
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            TracksDataSelected.Data[t].level_results.Add(lrs[o]);
                                                        });
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        #region new
                                        if (!find)
                                        {
                                            TrackData td = new TrackData()
                                            {
                                                freq = Equipment.IdentificationData.CDMA.BTS[i].FreqDn,
                                                gcid = Equipment.IdentificationData.CDMA.BTS[i].GCID,
                                                identifier_sub_ind = Equipment.IdentificationData.CDMA.BTS[i].PN,
                                                tech = "CDMA"
                                            };
                                            //НЕЗАБЫВАЙ ПРО ССЫЛОЧНЫЕ ТИПЫ
                                            for (int l = 0; l < Equipment.IdentificationData.CDMA.BTS[i].level_results.Count(); l++)
                                            {
                                                Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_db = true;
                                                Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_result = true;
                                                localatdi_level_meas_result lr = new localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].difference_time_stamp_ns,
                                                    level_dbm = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].level_dbm,
                                                    level_dbmkvm = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].level_dbmkvm,
                                                    measurement_time = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].measurement_time,
                                                    saved_in_db = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_db,
                                                    saved_in_result = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].saved_in_result,
                                                    location = new localatdi_geo_location()
                                                    {
                                                        agl = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.agl,
                                                        asl = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.asl,
                                                        longitude = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.longitude,
                                                        latitude = Equipment.IdentificationData.CDMA.BTS[i].level_results[l].location.latitude,
                                                    }
                                                };
                                                td.level_results.Add(lr);
                                            }
                                            td.information_blocks = new local3GPPSystemInformationBlock[Equipment.IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks.Count()];
                                            for (int b = 0; b < Equipment.IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks.Count(); b++)
                                            {
                                                local3GPPSystemInformationBlock ib = new local3GPPSystemInformationBlock()
                                                {
                                                    datastring = Equipment.IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks[b].datastring,
                                                    saved = Equipment.IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks[b].saved,
                                                    type = Equipment.IdentificationData.CDMA.BTS[i].station_sys_info.information_blocks[b].type,
                                                };
                                                td.information_blocks[b] = ib;
                                            }
                                            cmd.Parameters.Clear();
                                            cmd.CommandText = "INSERT INTO " + TracksDataSelected.table_name + " (tech, freq, gcid, identifier_sub_ind, level_results, information_blocks) " +
                                                "VALUES (@tech, @freq, @gcid, @identifier_sub_ind, @level_results, @information_blocks);";
                                            cmd.Parameters.AddRange(
                                                new NpgsqlParameter[]
                                                {
                                                    #region
                                                    new NpgsqlParameter{ParameterName = "@tech", Value = td.tech },
                                                    new NpgsqlParameter{ParameterName = "@freq", Value = td.freq },
                                                    new NpgsqlParameter{ParameterName = "@gcid", Value = td.gcid },
                                                    new NpgsqlParameter{ParameterName = "@identifier_sub_ind", Value = td.identifier_sub_ind },
                                                    new NpgsqlParameter{ParameterName = "@level_results", Value = td.level_results.ToArray(), DataTypeName ="localatdi_level_meas_result[]"},
                                                    new NpgsqlParameter{ParameterName = "@information_blocks", Value = td.information_blocks, DataTypeName ="local_3gpp_system_information_block[]"},

                                                    //new NpgsqlParameter("@tech", td.tech),
                                                    //new NpgsqlParameter("@freq", td.freq),
                                                    //new NpgsqlParameter("@gcid", td.gcid),
                                                    //new NpgsqlParameter("@identifier_sub_ind", td.identifier_sub_ind),
                                                    //new NpgsqlParameter("@level_results", td.level_results.ToArray()),
                                                    //new NpgsqlParameter("@information_blocks", td.information_blocks.ToArray()),
                                                    
                                                    #endregion
                                                });
                                            if (cmd.ExecuteNonQuery() != 1)
                                            {
                                                //'handled as needed, 
                                                //' but this snippet will throw an exception to force a rollback
                                                throw new InvalidProgramException();
                                            }
                                            else
                                            {
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    TracksDataSelected.Data.Add(td);
                                                });

                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion CDMA
                                oTransaction.Commit();
                            }
                            catch (Exception exp)
                            {
                                oTransaction.Rollback();
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                                }));
                            }
                        }
                    }
                    ncdb.Close();
                }
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            finally
            {
                //long ttt = DateTime.Now.Ticks;
                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Data Track Udate";
                //}));
                //ddddd = DateTime.Now.Ticks;
                dbt -= SaveUpdateTrackData;
            }
        }



        #endregion Tracks

        private void SaveUpdateATDIMeasData_v2()
        {
            long data = 0, data1 = 0;
            long lrcount = 0, lrcount1 = 0;
            long bandres = 0, bandres1 = 0;
            try
            {
                //ищем в известных            
                for (int mdi = 0; mdi < MeasMon.Data.Count; mdi++)
                {
                    if (MeasMon.Data[mdi].ThisToMeas == false)
                    {
                        MeasData md = MeasMon.Data[mdi];
                        #region
                        if ((md.NewDataToSave == true || MonMeasSaveAll) && new TimeSpan(DateTime.Now.Ticks - md.LastSave.Ticks) > new TimeSpan(0, 0, 0, 5, 0))//смотрим когда последний раз сохранялось если больше 5 сек сохраняем
                        {
                            #region
                            #region trace to db                                                       
                            //double[] tr = new double[md.Trace.Length];
                            //for (int i = 0; i < tr.Length; i++)
                            //{
                            //    tr[i] = md.Trace[i].level;
                            //}
                            #endregion
                            #region status
                            string status = "";
                            //НДП
                            if (md.ATDI_Id_Station == "" && md.ATDI_Id_Sector == "" && md.ATDI_Id_Permission == 0 && md.ATDI_Id_Frequency == 0)
                            { status = "E"; }
                            //ППЕ
                            else if (md.ATDI_Id_Station != "" && md.ATDI_Id_Permission == 0 && md.ATDI_Id_Sector == "")
                            { status = "I"; }
                            //Кошерно
                            else if (md.ATDI_Id_Station != "" && md.ATDI_Id_Sector != "" && md.ATDI_Id_Permission != 0 && md.ATDI_Id_Frequency != 0)
                            { status = "A"; }
                            #endregion
                            data1 = DateTime.Now.Ticks;
                            if (MeasMon.FromTask)
                            {
                                #region ищем в результатах тасков
                                //нашли таск от этой записи
                                //ищем по технологиям
                                bool findInResult = false;
                                for (int j = 0; j < AtdiTask.data_from_tech.Count; j++)
                                {
                                    if (AtdiTask.data_from_tech[j].tech.ToUpper().Contains(md.Techonology))//нашли по технологии т.к. она есть в таске
                                    {
                                        //ищем в результатах эту запись
                                        for (int z = 0; z < AtdiTask.data_from_tech[j].ResultItems.Count; z++)
                                        {
                                            if (md.ATDI_Id_Station == AtdiTask.data_from_tech[j].ResultItems[z].id_station &&
                                                md.ATDI_Id_Sector == AtdiTask.data_from_tech[j].ResultItems[z].id_sector &&
                                                md.ATDI_Id_Permission == AtdiTask.data_from_tech[j].ResultItems[z].id_permission &&
                                                md.ATDI_Id_Frequency == AtdiTask.data_from_tech[j].ResultItems[z].id_frequency &&
                                                md.ATDI_Id_Task == AtdiTask.data_from_tech[j].ResultItems[z].id_task &&
                                                md.SpecData.FreqCentr == AtdiTask.data_from_tech[j].ResultItems[z].spec_data.FreqCentr &&
                                                md.GCID == AtdiTask.data_from_tech[j].ResultItems[z].station_identifier_from_radio &&
                                                md.TechSubInd == AtdiTask.data_from_tech[j].ResultItems[z].station_identifier_from_radio_tech_sub_ind
                                                )//нашли такое измерение то обновим в базе и софте при условии что оно не ндп
                                            {
                                                findInResult = true;
                                                #region
                                                //__AtdiTask.task_data_from_tech[j].ResultTaskItems[z].id_permission = md.ATDI_Id_Permission;
                                                //__AtdiTask.task_data_from_tech[j].ResultTaskItems[z].id_station = md.ATDI_Id_Station;
                                                //__AtdiTask.task_data_from_tech[j].ResultTaskItems[z].id_sector = md.ATDI_Id_Sector;
                                                //__AtdiTask.task_data_from_tech[j].ResultTaskItems[z].id_frequency = md.ATDI_Id_Frequency;
                                                //AtdiTask.data_from_tech[j].ResultItems[z].id_task = md.ATDI_Id_Task;
                                                AtdiTask.data_from_tech[j].ResultItems[z].freq_centr_perm = md.ATDI_FrequencyPermission;
                                                AtdiTask.data_from_tech[j].ResultItems[z].meas_strength = md.ChannelStrenght;
                                                AtdiTask.data_from_tech[j].ResultItems[z].meas_mask = md.MeasMask;
                                                AtdiTask.data_from_tech[j].ResultItems[z].meas_correctness = md.MeasCorrectness;
                                                AtdiTask.data_from_tech[j].ResultItems[z].spec_data = md.SpecData;
                                                AtdiTask.data_from_tech[j].ResultItems[z].bw_data = md.BWData;
                                                AtdiTask.data_from_tech[j].ResultItems[z].cp_data = md.CPData;

                                                //__AtdiTask.task_data_from_tech[j].ResultTaskItems[z].station_identifier_from_radio = md.GCID;
                                                //__AtdiTask.task_data_from_tech[j].ResultTaskItems[z].station_identifier_from_radio_tech_sub_ind = md.TechSubInd;
                                                AtdiTask.data_from_tech[j].ResultItems[z].station_identifier_atdi = md.ATDI_GCID;
                                                AtdiTask.data_from_tech[j].ResultItems[z].status = status;
                                                AtdiTask.data_from_tech[j].ResultItems[z].user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString();
                                                AtdiTask.data_from_tech[j].ResultItems[z].user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials;
                                                AtdiTask.data_from_tech[j].ResultItems[z].new_meas_data_to_send = true;
                                                AtdiTask.data_from_tech[j].ResultItems[z].station_sys_info = md.station_sys_info;
                                                AtdiTask.data_from_tech[j].ResultItems[z].device_ident = md.device_ident;
                                                AtdiTask.data_from_tech[j].ResultItems[z].device_meas = md.device_meas;

                                                #region
                                                if (md.LevelResults.Count() > 0)
                                                {
                                                    for (int m = 0; m < md.LevelResults.Count(); m++)
                                                    {
                                                        if (md.LevelResults[m].saved_in_result == false)
                                                        {
                                                            md.LevelResults[m].saved_in_result = true;
                                                            localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                            {
                                                                difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                                level_dbm = md.LevelResults[m].level_dbm,
                                                                level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                                measurement_time = md.LevelResults[m].measurement_time,
                                                                saved_in_db = md.LevelResults[m].saved_in_db,
                                                                saved_in_result = md.LevelResults[m].saved_in_result,
                                                                location = new localatdi_geo_location()
                                                                {
                                                                    agl = md.LevelResults[m].location.agl,
                                                                    asl = md.LevelResults[m].location.asl,
                                                                    latitude = md.LevelResults[m].location.latitude,
                                                                    longitude = md.LevelResults[m].location.longitude,
                                                                }
                                                            };
                                                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                            {
                                                                AtdiTask.data_from_tech[j].ResultItems[z].level_results.Add(l);
                                                            });
                                                            if (AtdiTask.data_from_tech[j].ResultItems[z].level_results_sended == true)
                                                            { AtdiTask.data_from_tech[j].ResultItems[z].level_results_sended = false; }
                                                        }
                                                    }
                                                    md.LR_NewDataToSave = false;
                                                }
                                                #endregion
                                                //обновляем в БД
                                                ATDI_UpdateResultToDB_v2(md.NewSpecDataToSave, AtdiTask.data_from_tech[j].ResultItems[z], AtdiTask.data_from_tech[j].result_table_name);
                                                md.LastSave = DateTime.Now;
                                                md.NewDataToSave = false;
                                                #endregion
                                            }
                                        }
                                        #region ищем запись в тасках и бновляем статус измерения что есть измерение
                                        for (int z = 0; z < AtdiTask.data_from_tech[j].TaskItems.Count; z++)
                                        {
                                            if (md.ATDI_Id_Station == AtdiTask.data_from_tech[j].TaskItems[z].id &&
                                            md.ATDI_Id_Permission == AtdiTask.data_from_tech[j].TaskItems[z].license.icsm_id &&
                                            md.ATDI_GCID == AtdiTask.data_from_tech[j].TaskItems[z].callsign_db)
                                            {
                                                for (int sec = 0; sec < AtdiTask.data_from_tech[j].TaskItems[z].sectors.Count(); sec++)
                                                {
                                                    //нашли такое измерение то обновим в базе и софте при условии что оно не ндп
                                                    if (md.ATDI_Id_Sector == AtdiTask.data_from_tech[j].TaskItems[z].sectors[sec].sector_id)
                                                    {
                                                        #region
                                                        AtdiTask.data_from_tech[j].TaskItems[z].meas_data_exist = true;
                                                        ATDI_UpdateTaskItemMeasDataExistToDB_v2(
                                                            AtdiTask.data_from_tech[j].TaskItems[z].id,
                                                            //AtdiTask.data_from_tech[j].TaskItems[z].sectors[sec].sector_id,
                                                            AtdiTask.data_from_tech[j].TaskItems[z].callsign_db,
                                                            AtdiTask.data_from_tech[j].TaskItems[z].meas_data_exist,
                                                            AtdiTask.data_from_tech[j].task_table_name);

                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        //ненашли результат то добавим
                                        if (findInResult == false)
                                        {
                                            #region
                                            ObservableCollection<DB.localatdi_level_meas_result> lmc = new ObservableCollection<DB.localatdi_level_meas_result>() { };
                                            if (md.LevelResults.Count() > 0)
                                            {
                                                for (int m = 0; m < md.LevelResults.Count(); m++)
                                                {
                                                    if (md.LevelResults[m].saved_in_result == false)
                                                    {
                                                        md.LevelResults[m].saved_in_result = true;
                                                        localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                            level_dbm = md.LevelResults[m].level_dbm,
                                                            level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                            measurement_time = md.LevelResults[m].measurement_time,
                                                            saved_in_db = md.LevelResults[m].saved_in_db,
                                                            saved_in_result = md.LevelResults[m].saved_in_result,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = md.LevelResults[m].location.agl,
                                                                asl = md.LevelResults[m].location.asl,
                                                                latitude = md.LevelResults[m].location.latitude,
                                                                longitude = md.LevelResults[m].location.longitude,
                                                            }
                                                        };
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            lmc.Add(l);
                                                        });
                                                    }
                                                }
                                            }
                                            md.LR_NewDataToSave = false;
                                            localatdi_result_item ri = new localatdi_result_item()
                                            {
                                                id_permission = md.ATDI_Id_Permission,
                                                id_station = md.ATDI_Id_Station,
                                                id_sector = md.ATDI_Id_Sector,
                                                id_frequency = md.ATDI_Id_Frequency,
                                                id_task = md.ATDI_Id_Task,
                                                freq_centr_perm = md.ATDI_FrequencyPermission,
                                                meas_strength = md.ChannelStrenght,
                                                meas_mask = md.MeasMask,
                                                mask_result = md.MaskResult,
                                                meas_correctness = md.MeasCorrectness,
                                                spec_data = md.SpecData,
                                                bw_data = md.BWData,
                                                cp_data = md.CPData,
                                                station_identifier_from_radio = md.GCID,
                                                station_identifier_from_radio_tech_sub_ind = md.TechSubInd,
                                                station_identifier_atdi = md.ATDI_GCID,
                                                status = status,
                                                user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString(),
                                                user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials,
                                                new_meas_data_to_send = true,
                                                level_results_sended = false,
                                                level_results = lmc,
                                                station_sys_info = md.station_sys_info,
                                                device_ident = md.device_ident,
                                                device_meas = md.device_meas,
                                            };
                                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                            {
                                                AtdiTask.data_from_tech[j].ResultItems.Add(ri);
                                            });
                                            ATDI_AddNewResultToDB_v2(ri, AtdiTask.data_from_tech[j].result_table_name);
                                            //for (int lr = 0; lr < ri.level_results.Count; lr++)
                                            //{
                                            //    ri.level_results[lr].saved_in_db = true;
                                            //    ri.level_results[lr].saved_in_result = true;
                                            //}
                                            md.NewDataToSave = false;

                                            #region ищем запись в тасках и бновляем статус измерения что есть измерение
                                            for (int z = 0; z < AtdiTask.data_from_tech[j].TaskItems.Count; z++)
                                            {
                                                if (md.ATDI_Id_Station == AtdiTask.data_from_tech[j].TaskItems[z].id &&
                                                md.ATDI_Id_Permission == AtdiTask.data_from_tech[j].TaskItems[z].license.icsm_id &&
                                                md.ATDI_GCID == AtdiTask.data_from_tech[j].TaskItems[z].callsign_db)
                                                {
                                                    for (int sec = 0; sec < AtdiTask.data_from_tech[j].TaskItems[z].sectors.Count; sec++)
                                                    {
                                                        //нашли такое измерение то обновим в базе и софте при условии что оно не ндп
                                                        if (md.ATDI_Id_Sector == AtdiTask.data_from_tech[j].TaskItems[z].sectors[sec].sector_id)
                                                        {
                                                            #region
                                                            AtdiTask.data_from_tech[j].TaskItems[z].meas_data_exist = true;
                                                            ATDI_UpdateTaskItemMeasDataExistToDB_v2(
                                                                AtdiTask.data_from_tech[j].TaskItems[z].id,
                                                                //AtdiTask.data_from_tech[j].TaskItems[z].sectors[sec].sector_id,
                                                                AtdiTask.data_from_tech[j].TaskItems[z].callsign_db,
                                                                AtdiTask.data_from_tech[j].TaskItems[z].meas_data_exist,
                                                                AtdiTask.data_from_tech[j].task_table_name);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion
                                            #endregion
                                        }
                                    }
                                }
                                //а вдруг ненашли по технологии пишем в таблицу неизвестного
                                if (md.NewDataToSave == true && findInResult == false)
                                {
                                    localatdi_task_with_tech unktech = AtdiTask.data_from_tech.Where(x => x.tech == "unknown").First();
                                    bool findInUnknownResult = false;
                                    #region ищем если уже было то пишем
                                    for (int z = 0; z < unktech.ResultItems.Count; z++)
                                    {
                                        if (md.ATDI_Id_Station == unktech.ResultItems[z].id_station &&
                                            md.ATDI_Id_Sector == unktech.ResultItems[z].id_sector &&
                                            md.ATDI_Id_Permission == unktech.ResultItems[z].id_permission &&
                                            md.ATDI_Id_Frequency == unktech.ResultItems[z].id_frequency &&
                                            md.SpecData.FreqCentr == unktech.ResultItems[z].spec_data.FreqCentr &&
                                            md.GCID == unktech.ResultItems[z].station_identifier_from_radio &&
                                            md.TechSubInd == unktech.ResultItems[z].station_identifier_from_radio_tech_sub_ind
                                            )//нашли такое измерение то обновим в базе и софте при условии что оно не ндп
                                        {
                                            findInUnknownResult = true;
                                            #region
                                            unktech.ResultItems[z].id_permission = md.ATDI_Id_Permission;
                                            unktech.ResultItems[z].id_station = md.ATDI_Id_Station;
                                            unktech.ResultItems[z].id_sector = md.ATDI_Id_Sector;
                                            unktech.ResultItems[z].id_frequency = md.ATDI_Id_Frequency;
                                            unktech.ResultItems[z].id_task = md.ATDI_Id_Task;
                                            unktech.ResultItems[z].freq_centr_perm = md.ATDI_FrequencyPermission;
                                            unktech.ResultItems[z].meas_strength = md.ChannelStrenght;
                                            unktech.ResultItems[z].meas_mask = md.MeasMask;
                                            unktech.ResultItems[z].mask_result = md.MaskResult;
                                            unktech.ResultItems[z].meas_correctness = md.MeasCorrectness;
                                            unktech.ResultItems[z].spec_data = md.SpecData;
                                            unktech.ResultItems[z].bw_data = md.BWData;
                                            unktech.ResultItems[z].cp_data = md.CPData;
                                            unktech.ResultItems[z].station_identifier_from_radio = md.GCID;
                                            unktech.ResultItems[z].station_identifier_from_radio_tech_sub_ind = md.TechSubInd;
                                            unktech.ResultItems[z].station_identifier_atdi = md.ATDI_GCID;
                                            unktech.ResultItems[z].status = status;
                                            unktech.ResultItems[z].user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString();
                                            unktech.ResultItems[z].user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials;
                                            unktech.ResultItems[z].new_meas_data_to_send = true;
                                            unktech.ResultItems[z].level_results_sended = false;
                                            unktech.ResultItems[z].station_sys_info = md.station_sys_info;
                                            unktech.ResultItems[z].device_ident = md.device_ident;
                                            unktech.ResultItems[z].device_meas = md.device_meas;

                                            if (md.LevelResults.Count() > 0)
                                            {
                                                for (int m = 0; m < md.LevelResults.Count(); m++)
                                                {
                                                    if (md.LevelResults[m].saved_in_result == false)
                                                    {
                                                        md.LevelResults[m].saved_in_result = true;
                                                        DB.localatdi_level_meas_result l = new DB.localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                            level_dbm = md.LevelResults[m].level_dbm,
                                                            level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                            measurement_time = md.LevelResults[m].measurement_time,
                                                            saved_in_db = md.LevelResults[m].saved_in_db,
                                                            saved_in_result = md.LevelResults[m].saved_in_result,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = md.LevelResults[m].location.agl,
                                                                asl = md.LevelResults[m].location.asl,
                                                                latitude = md.LevelResults[m].location.latitude,
                                                                longitude = md.LevelResults[m].location.longitude,
                                                            }
                                                        };
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            unktech.ResultItems[z].level_results.Add(l);
                                                        });
                                                        if (unktech.ResultItems[z].level_results_sended == true)
                                                        { unktech.ResultItems[z].level_results_sended = false; }
                                                    }
                                                }
                                                md.LR_NewDataToSave = false;
                                            }
                                            ATDI_UpdateResultToDB_v2(md.NewSpecDataToSave, unktech.ResultItems[z], unktech.result_table_name);
                                            //for (int lr = 0; lr < unktech.ResultItems[z].level_results.Count; lr++)
                                            //{
                                            //    unktech.ResultItems[z].level_results[lr].saved_in_db = true;
                                            //    unktech.ResultItems[z].level_results[lr].saved_in_result = true;
                                            //}
                                            md.LastSave = DateTime.Now;
                                            md.NewDataToSave = false;
                                            #endregion
                                        }
                                    }
                                    #endregion

                                    #region ненашли
                                    if (findInUnknownResult == false)
                                    {
                                        #region
                                        ObservableCollection<DB.localatdi_level_meas_result> lmc = new ObservableCollection<DB.localatdi_level_meas_result>() { };
                                        if (md.LevelResults.Count() > 0)
                                        {
                                            for (int m = 0; m < md.LevelResults.Count(); m++)
                                            {
                                                if (md.LevelResults[m].saved_in_result == false)
                                                {
                                                    md.LevelResults[m].saved_in_result = true;
                                                    localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                    {
                                                        difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                        level_dbm = md.LevelResults[m].level_dbm,
                                                        level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                        measurement_time = md.LevelResults[m].measurement_time,
                                                        saved_in_db = md.LevelResults[m].saved_in_db,
                                                        saved_in_result = md.LevelResults[m].saved_in_result,
                                                        location = new localatdi_geo_location()
                                                        {
                                                            agl = md.LevelResults[m].location.agl,
                                                            asl = md.LevelResults[m].location.asl,
                                                            latitude = md.LevelResults[m].location.latitude,
                                                            longitude = md.LevelResults[m].location.longitude,
                                                        }
                                                    };
                                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                    {
                                                        lmc.Add(l);
                                                    });
                                                }
                                            }
                                        }
                                        md.LR_NewDataToSave = false;
                                        localatdi_result_item ri = new localatdi_result_item()
                                        {
                                            id_permission = md.ATDI_Id_Permission,
                                            id_station = md.ATDI_Id_Station,
                                            id_sector = md.ATDI_Id_Sector,
                                            id_frequency = md.ATDI_Id_Frequency,
                                            id_task = md.ATDI_Id_Task,
                                            freq_centr_perm = md.ATDI_FrequencyPermission,
                                            meas_strength = md.ChannelStrenght,
                                            meas_mask = md.MeasMask,
                                            mask_result = md.MaskResult,
                                            meas_correctness = md.MeasCorrectness,
                                            spec_data = md.SpecData,
                                            bw_data = md.BWData,
                                            cp_data = md.CPData,
                                            station_identifier_from_radio = md.GCID,
                                            station_identifier_from_radio_tech_sub_ind = md.TechSubInd,
                                            station_identifier_atdi = md.ATDI_GCID,
                                            status = status,
                                            user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString(),
                                            user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials,
                                            new_meas_data_to_send = true,
                                            level_results_sended = false,
                                            level_results = lmc,
                                            station_sys_info = md.station_sys_info,
                                            device_ident = md.device_ident,
                                            device_meas = md.device_meas,
                                        };
                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                        {
                                            unktech.ResultItems.Add(ri);
                                        });
                                        ATDI_AddNewResultToDB_v2(ri, unktech.result_table_name);
                                        //for (int lr = 0; lr < ri.level_results.Count; lr++)
                                        //{
                                        //    ri.level_results[lr].saved_in_db = true;
                                        //    ri.level_results[lr].saved_in_result = true;
                                        //}
                                        md.NewDataToSave = false;
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region
                                bool findTech = false;
                                //string newtech = md.Techonology;
                                for (int j = 0; j < AtdiUnknownResult.data_from_tech.Count(); j++)
                                {
                                    if (AtdiUnknownResult.data_from_tech[j].tech.ToUpper().Contains(md.Techonology))//нашли по технологии т.к. она есть в таске
                                    {
                                        findTech = true;
                                        //newtech = ;
                                    }
                                }

                                if (!findTech)//создаем таблицу в бд и добавим в результат
                                {
                                    findTech = AddTechToUnknownResult(AtdiUnknownResult, md.Techonology);
                                    //if (!findTech) System.Windows.MessageBox.Show("все плохо");
                                }
                                if (findTech)//
                                {

                                    for (int j = 0; j < AtdiUnknownResult.data_from_tech.Count(); j++)
                                    {
                                        if (AtdiUnknownResult.data_from_tech[j].tech.ToUpper().Contains(md.Techonology))//нашли по технологии
                                        {
                                            bool findresult = false;
                                            for (int r = 0; r < AtdiUnknownResult.data_from_tech[j].ResultItems.Count(); r++)
                                            {
                                                if (md.ATDI_Id_Station == AtdiUnknownResult.data_from_tech[j].ResultItems[r].id_station &&
                                                    md.SpecData.FreqCentr == AtdiUnknownResult.data_from_tech[j].ResultItems[r].spec_data.FreqCentr &&
                                                    md.GCID == AtdiUnknownResult.data_from_tech[j].ResultItems[r].station_identifier_from_radio &&
                                                    md.TechSubInd == AtdiUnknownResult.data_from_tech[j].ResultItems[r].station_identifier_from_radio_tech_sub_ind
                                                    )//нашли такое измерение то обновим в базе и софте
                                                {
                                                    findresult = true;
                                                    #region
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].id_permission = md.ATDI_Id_Permission;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].id_station = md.ATDI_Id_Station;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].id_sector = md.ATDI_Id_Sector;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].id_frequency = md.ATDI_Id_Frequency;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].id_task = md.ATDI_Id_Task;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].freq_centr_perm = md.ATDI_FrequencyPermission;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].meas_strength = md.ChannelStrenght;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].meas_mask = md.MeasMask;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].mask_result = md.MaskResult;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].meas_correctness = md.MeasCorrectness;

                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].spec_data = md.SpecData;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].bw_data = md.BWData;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].cp_data = md.CPData;

                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].station_identifier_from_radio = md.GCID;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].station_identifier_from_radio_tech_sub_ind = md.TechSubInd;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].station_identifier_atdi = md.ATDI_GCID;

                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].status = "E"; //т.к. все это НДП

                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString();
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].new_meas_data_to_send = true;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].station_sys_info = md.station_sys_info;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].device_ident = md.device_ident;
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].device_meas = md.device_meas;

                                                    if (md.LevelResults.Count > 0)
                                                    {
                                                        #region
                                                        for (int m = 0; m < md.LevelResults.Count(); m++)
                                                        {
                                                            if (md.LevelResults[m].saved_in_result == false)
                                                            {
                                                                md.LevelResults[m].saved_in_result = true;
                                                                localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                                {
                                                                    difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                                    level_dbm = md.LevelResults[m].level_dbm,
                                                                    level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                                    measurement_time = md.LevelResults[m].measurement_time,
                                                                    saved_in_db = md.LevelResults[m].saved_in_db,
                                                                    saved_in_result = md.LevelResults[m].saved_in_result,
                                                                    location = new localatdi_geo_location()
                                                                    {
                                                                        agl = md.LevelResults[m].location.agl,
                                                                        asl = md.LevelResults[m].location.asl,
                                                                        latitude = md.LevelResults[m].location.latitude,
                                                                        longitude = md.LevelResults[m].location.longitude,
                                                                    }
                                                                };
                                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                                {
                                                                    AtdiUnknownResult.data_from_tech[j].ResultItems[r].level_results.Add(l);
                                                                });
                                                            }
                                                        }
                                                        md.LR_NewDataToSave = false;
                                                        #endregion
                                                    }
                                                    #endregion

                                                    ATDI_UpdateResultToDB_v2(md.NewSpecDataToSave, AtdiUnknownResult.data_from_tech[j].ResultItems[r], AtdiUnknownResult.data_from_tech[j].result_table_name);
                                                    //for (int lr = 0; lr < AtdiUnknownResult.data_from_tech[j].ResultItems[r].level_results.Count; lr++)
                                                    //{
                                                    //    AtdiUnknownResult.data_from_tech[j].ResultItems[r].level_results[lr].saved_in_db = true;
                                                    //    AtdiUnknownResult.data_from_tech[j].ResultItems[r].level_results[lr].saved_in_result = true;
                                                    //}
                                                }
                                            }
                                            if (!findresult)//ненашли результат
                                            {
                                                #region
                                                ObservableCollection<DB.localatdi_level_meas_result> lmc = new ObservableCollection<DB.localatdi_level_meas_result>() { };
                                                if (md.LevelResults.Count > 0)
                                                {
                                                    #region
                                                    for (int m = 0; m < md.LevelResults.Count(); m++)
                                                    {
                                                        if (md.LevelResults[m].saved_in_result == false)
                                                        {
                                                            md.LevelResults[m].saved_in_result = true;
                                                            localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                            {
                                                                difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                                level_dbm = md.LevelResults[m].level_dbm,
                                                                level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                                measurement_time = md.LevelResults[m].measurement_time,
                                                                saved_in_db = md.LevelResults[m].saved_in_db,
                                                                saved_in_result = md.LevelResults[m].saved_in_result,
                                                                location = new localatdi_geo_location()
                                                                {
                                                                    agl = md.LevelResults[m].location.agl,
                                                                    asl = md.LevelResults[m].location.asl,
                                                                    latitude = md.LevelResults[m].location.latitude,
                                                                    longitude = md.LevelResults[m].location.longitude,
                                                                }
                                                            };
                                                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                            {
                                                                lmc.Add(l);
                                                            });
                                                        }
                                                    }
                                                    md.LR_NewDataToSave = false;
                                                    #endregion
                                                }
                                                localatdi_result_item res = new localatdi_result_item() { };
                                                res.id_permission = md.ATDI_Id_Permission;
                                                res.id_station = md.ATDI_Id_Station;
                                                res.id_sector = md.ATDI_Id_Sector;
                                                res.id_frequency = md.ATDI_Id_Frequency;
                                                res.id_task = md.ATDI_Id_Task;
                                                res.freq_centr_perm = md.ATDI_FrequencyPermission;
                                                res.meas_strength = md.ChannelStrenght;
                                                res.meas_mask = md.MeasMask;
                                                res.mask_result = md.MaskResult;
                                                res.meas_correctness = md.MeasCorrectness;

                                                res.spec_data = md.SpecData;
                                                res.bw_data = md.BWData;
                                                res.cp_data = md.CPData;

                                                res.station_identifier_from_radio = md.GCID;
                                                res.station_identifier_from_radio_tech_sub_ind = md.TechSubInd;
                                                res.station_identifier_atdi = md.ATDI_GCID;

                                                res.status = "E"; //т.к. все это НДП

                                                res.user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString();
                                                res.user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials;
                                                res.new_meas_data_to_send = true;
                                                res.level_results = lmc;
                                                res.station_sys_info = md.station_sys_info;
                                                res.device_ident = md.device_ident;
                                                res.device_meas = md.device_meas;
                                                #endregion
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems.Add(res);
                                                });
                                                ATDI_AddNewResultToDB_v2(res, AtdiUnknownResult.data_from_tech[j].result_table_name);
                                                //for (int lr = 0; lr < res.level_results.Count; lr++)
                                                //{
                                                //    res.level_results[lr].saved_in_db = true;
                                                //    res.level_results[lr].saved_in_result = true;
                                                //}

                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            data += DateTime.Now.Ticks - data1;
                            #endregion
                        }
                        lrcount1 = DateTime.Now.Ticks;
                        if (md.LR_NewDataToSave == true)
                        {
                            #region
                            if (MeasMon.FromTask)
                            {
                                #region
                                //ищем по технологиям
                                bool findInResult = false;
                                for (int j = 0; j < AtdiTask.data_from_tech.Count; j++)
                                {
                                    if (AtdiTask.data_from_tech[j].tech.ToUpper().Contains(md.Techonology))//нашли по технологии т.к. она есть в таске
                                    {
                                        //ищем в результатах эту запись
                                        for (int z = 0; z < AtdiTask.data_from_tech[j].ResultItems.Count; z++)
                                        {
                                            #region
                                            if (md.ATDI_Id_Station == AtdiTask.data_from_tech[j].ResultItems[z].id_station &&
                                                md.ATDI_Id_Sector == AtdiTask.data_from_tech[j].ResultItems[z].id_sector &&
                                                md.ATDI_Id_Permission == AtdiTask.data_from_tech[j].ResultItems[z].id_permission &&
                                                md.ATDI_Id_Frequency == AtdiTask.data_from_tech[j].ResultItems[z].id_frequency &&
                                                md.ATDI_Id_Task == AtdiTask.data_from_tech[j].ResultItems[z].id_task &&
                                                md.SpecData.FreqCentr == AtdiTask.data_from_tech[j].ResultItems[z].spec_data.FreqCentr &&
                                                md.GCID == AtdiTask.data_from_tech[j].ResultItems[z].station_identifier_from_radio &&
                                                md.TechSubInd == AtdiTask.data_from_tech[j].ResultItems[z].station_identifier_from_radio_tech_sub_ind)
                                            {
                                                findInResult = true;
                                                for (int m = 0; m < md.LevelResults.Count; m++)
                                                {
                                                    if (md.LevelResults[m].saved_in_result == false)
                                                    {
                                                        md.LevelResults[m].saved_in_result = true;
                                                        localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                            level_dbm = md.LevelResults[m].level_dbm,
                                                            level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                            measurement_time = md.LevelResults[m].measurement_time,
                                                            saved_in_db = false,
                                                            saved_in_result = md.LevelResults[m].saved_in_result,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = md.LevelResults[m].location.agl,
                                                                asl = md.LevelResults[m].location.asl,
                                                                longitude = md.LevelResults[m].location.longitude,
                                                                latitude = md.LevelResults[m].location.latitude,
                                                            }
                                                        };
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            AtdiTask.data_from_tech[j].ResultItems[z].level_results.Add(l);
                                                        });
                                                        if (AtdiTask.data_from_tech[j].ResultItems[z].level_results_sended == true)
                                                        { AtdiTask.data_from_tech[j].ResultItems[z].level_results_sended = false; }
                                                    }
                                                }
                                                md.LR_NewDataToSave = false;
                                                ATDI_UpdateLCMInResultToDB_v2(AtdiTask.data_from_tech[j].ResultItems[z], AtdiTask.data_from_tech[j].result_table_name);
                                                //for (int lr = 0; lr < AtdiTask.data_from_tech[j].ResultItems[z].level_results.Count; lr++)
                                                //{
                                                //    AtdiTask.data_from_tech[j].ResultItems[z].level_results[lr].saved_in_db = true;
                                                //    AtdiTask.data_from_tech[j].ResultItems[z].level_results[lr].saved_in_result = true;
                                                //}
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                if (findInResult == false)
                                {
                                    localatdi_task_with_tech unktech = AtdiTask.data_from_tech.Where(x => x.tech == "unknown").First();
                                    bool findInUnknownResult = false;
                                    #region ищем если уже было то пишем
                                    for (int z = 0; z < unktech.ResultItems.Count; z++)
                                    {
                                        if (md.ATDI_Id_Station == unktech.ResultItems[z].id_station &&
                                            md.ATDI_Id_Sector == unktech.ResultItems[z].id_sector &&
                                            md.ATDI_Id_Permission == unktech.ResultItems[z].id_permission &&
                                            md.ATDI_Id_Frequency == unktech.ResultItems[z].id_frequency &&
                                            md.SpecData.FreqCentr == unktech.ResultItems[z].spec_data.FreqCentr &&
                                            md.GCID == unktech.ResultItems[z].station_identifier_from_radio &&
                                            md.TechSubInd == unktech.ResultItems[z].station_identifier_from_radio_tech_sub_ind
                                            )//нашли такое измерение то обновим в базе и софте при условии что оно не ндп
                                        {
                                            findInUnknownResult = true;
                                            #region
                                            for (int m = 0; m < md.LevelResults.Count; m++)
                                            {
                                                if (md.LevelResults[m].saved_in_result == false)
                                                {
                                                    md.LevelResults[m].saved_in_result = true;
                                                    localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                    {
                                                        difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                        level_dbm = md.LevelResults[m].level_dbm,
                                                        level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                        measurement_time = md.LevelResults[m].measurement_time,
                                                        saved_in_db = md.LevelResults[m].saved_in_db,
                                                        saved_in_result = md.LevelResults[m].saved_in_result,
                                                        location = new localatdi_geo_location()
                                                        {
                                                            agl = md.LevelResults[m].location.agl,
                                                            asl = md.LevelResults[m].location.asl,
                                                            latitude = md.LevelResults[m].location.latitude,
                                                            longitude = md.LevelResults[m].location.longitude,
                                                        }
                                                    };

                                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                    {
                                                        unktech.ResultItems[z].level_results.Add(l);
                                                    });
                                                    if (unktech.ResultItems[z].level_results_sended == true)
                                                    { unktech.ResultItems[z].level_results_sended = false; }
                                                }
                                            }
                                            md.LR_NewDataToSave = false;
                                            //тут надо записать в базу
                                            ATDI_UpdateLCMInResultToDB_v2(unktech.ResultItems[z], unktech.result_table_name);
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region
                                bool findInResult = false;
                                //ищем по технологиям
                                for (int j = 0; j < AtdiUnknownResult.data_from_tech.Count; j++)
                                {
                                    if (AtdiUnknownResult.data_from_tech[j].tech.ToUpper().Contains(md.Techonology))//нашли по технологии т.к. она есть в таске
                                    {
                                        //ищем в результатах эту запись
                                        for (int z = 0; z < AtdiUnknownResult.data_from_tech[j].ResultItems.Count; z++)
                                        {
                                            #region
                                            if (md.ATDI_Id_Station == AtdiUnknownResult.data_from_tech[j].ResultItems[z].id_station &&
                                                md.ATDI_Id_Sector == AtdiUnknownResult.data_from_tech[j].ResultItems[z].id_sector &&
                                                md.ATDI_Id_Permission == AtdiUnknownResult.data_from_tech[j].ResultItems[z].id_permission &&
                                                md.ATDI_Id_Frequency == AtdiUnknownResult.data_from_tech[j].ResultItems[z].id_frequency &&
                                                md.ATDI_Id_Task == AtdiUnknownResult.data_from_tech[j].ResultItems[z].id_task &&
                                                md.SpecData.FreqCentr == AtdiUnknownResult.data_from_tech[j].ResultItems[z].spec_data.FreqCentr &&
                                                md.GCID == AtdiUnknownResult.data_from_tech[j].ResultItems[z].station_identifier_from_radio &&
                                                md.TechSubInd == AtdiUnknownResult.data_from_tech[j].ResultItems[z].station_identifier_from_radio_tech_sub_ind)
                                            {
                                                findInResult = true;
                                                for (int m = 0; m < md.LevelResults.Count; m++)
                                                {
                                                    if (md.LevelResults[m].saved_in_result == false)
                                                    {
                                                        localatdi_level_meas_result l = new localatdi_level_meas_result()
                                                        {
                                                            difference_time_stamp_ns = md.LevelResults[m].difference_time_stamp_ns,
                                                            level_dbm = md.LevelResults[m].level_dbm,
                                                            level_dbmkvm = md.LevelResults[m].level_dbmkvm,
                                                            measurement_time = md.LevelResults[m].measurement_time,
                                                            saved_in_db = false,
                                                            saved_in_result = true,
                                                            location = new localatdi_geo_location()
                                                            {
                                                                agl = md.LevelResults[m].location.agl,
                                                                asl = md.LevelResults[m].location.asl,
                                                                longitude = md.LevelResults[m].location.longitude,
                                                                latitude = md.LevelResults[m].location.latitude,
                                                            }
                                                        };
                                                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                        {
                                                            AtdiUnknownResult.data_from_tech[j].ResultItems[z].level_results.Add(l);
                                                        });
                                                        md.LevelResults[m].saved_in_result = true;
                                                        if (AtdiUnknownResult.data_from_tech[j].ResultItems[z].level_results_sended == true)
                                                        { AtdiUnknownResult.data_from_tech[j].ResultItems[z].level_results_sended = false; }
                                                    }
                                                }
                                                md.LR_NewDataToSave = false;
                                                ATDI_UpdateLCMInResultToDB_v2(AtdiUnknownResult.data_from_tech[j].ResultItems[z], AtdiUnknownResult.data_from_tech[j].result_table_name);
                                                //for (int lr = 0; lr < AtdiUnknownResult.data_from_tech[j].ResultItems[z].level_results.Count; lr++)
                                                //{
                                                //    AtdiUnknownResult.data_from_tech[j].ResultItems[z].level_results[lr].saved_in_db = true;
                                                //    AtdiUnknownResult.data_from_tech[j].ResultItems[z].level_results[lr].saved_in_result = true;
                                                //}
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        lrcount += DateTime.Now.Ticks - lrcount1;
                        #endregion
                    }
                }
                bandres1 = DateTime.Now.Ticks;
                double DetectionLevelGSM = App.Sett.MeasMons_Settings.GSM.DetectionLevel;
                for (int i = 0; i < GSMBandMeass.Count; i++)
                {
                    if (GSMBandMeass[i].Count > 0 && GSMBandMeass[i].saved == false)
                    {
                        #region 
                        for (int l = 0; l < GSMBandMeass[i].Trace.Length; l++)
                        {
                            if (GSMBandMeass[i].Trace[l].level >= DetectionLevelGSM)
                            {
                                if (MeasMon.FromTask)
                                {
                                    #region ищем в результатах
                                    bool findtech = false;
                                    #region в известных
                                    for (int j = 0; j < AtdiTask.data_from_tech.Count; j++)
                                    {
                                        if (AtdiTask.data_from_tech[j].tech.ToUpper().Contains("GSM"))//нашли по технологии т.к. она есть в таске
                                        {
                                            bool findInResult = false;
                                            findtech = true;
                                            #region
                                            for (int r = 0; r < AtdiTask.data_from_tech[j].ResultItems.Count; r++)
                                            {
                                                localatdi_result_item res = AtdiTask.data_from_tech[j].ResultItems[r];

                                                if (res.spec_data.FreqCentr == GSMBandMeass[i].Trace[l].freq && res.station_identifier_from_radio == "")
                                                {
                                                    //if (new TimeSpan(MainWindow.gps.LocalTime.Ticks - res.level_measurements_car[res.level_measurements_car.Count - 1].time_of_measurements.Ticks) > Atdi_LevelsMeasurementsCar_TimeStep)
                                                    //{
                                                    localatdi_level_meas_result lmc = new localatdi_level_meas_result()
                                                    {
                                                        difference_time_stamp_ns = -1,
                                                        level_dbm = GSMBandMeass[i].Trace[l].level,
                                                        level_dbmkvm = -1000,
                                                        measurement_time = GSMBandMeass[i].MeasTime,
                                                        saved_in_db = false,
                                                        saved_in_result = true,
                                                        location = new localatdi_geo_location()
                                                        {
                                                            asl = GSMBandMeass[i].altitude,
                                                            agl = -100000,
                                                            longitude = GSMBandMeass[i].longitude,
                                                            latitude = GSMBandMeass[i].latitude,
                                                        }
                                                    };
                                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                    {
                                                        res.level_results.Add(lmc);
                                                    });
                                                    ATDI_UpdateLCMInResultToDB_v2(res, AtdiTask.data_from_tech[j].result_table_name);

                                                    //тут надо записать в базу

                                                    //}
                                                    findInResult = true;
                                                }
                                            }
                                            #endregion
                                            if (findInResult == false)
                                            {
                                                #region
                                                ObservableCollection<localatdi_level_meas_result> lmcs = new ObservableCollection<localatdi_level_meas_result>() { };
                                                localatdi_level_meas_result lmc = new localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = -1,
                                                    level_dbm = GSMBandMeass[i].Trace[l].level,
                                                    level_dbmkvm = -1000,
                                                    measurement_time = GSMBandMeass[i].MeasTime,
                                                    saved_in_db = false,
                                                    saved_in_result = true,
                                                    location = new localatdi_geo_location()
                                                    {
                                                        agl = GSMBandMeass[i].altitude,
                                                        asl = -100000,
                                                        longitude = GSMBandMeass[i].longitude,
                                                        latitude = GSMBandMeass[i].latitude,
                                                    }
                                                };
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    lmcs.Add(lmc); ;
                                                });
                                                #region
                                                #region
                                                Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.GSM;
                                                double _NdbLevel = set.Data[0].NdBLevel;
                                                decimal _NdBBWMin = set.Data[0].NdBBWMin;
                                                decimal _NdBBWMax = set.Data[0].NdBBWMax;
                                                decimal _BWLimit = set.Data[0].BWLimit;
                                                decimal _MarPeakBW = set.Data[0].MarPeakBW;

                                                decimal _MeasBW = set.Data[0].MeasBW;
                                                int _TracePoints = set.Data[0].TracePoints;
                                                decimal _RBW = set.Data[0].RBW;
                                                decimal _VBW = set.Data[0].VBW;
                                                decimal _OBWP = set.Data[0].OBWPercent;
                                                decimal _DeltaFreqLimit = set.Data[0].DeltaFreqLimit;
                                                Equipment.spectrum_data sd = new Equipment.spectrum_data()
                                                {
                                                    FreqCentr = GSMBandMeass[i].Trace[l].freq,
                                                    FreqSpan = _MeasBW,
                                                    FreqStart = GSMBandMeass[i].Trace[l].freq - _MeasBW / 2,
                                                    FreqStop = GSMBandMeass[i].Trace[l].freq + _MeasBW / 2,
                                                    LastMeasAltitude = 0,
                                                    LastMeasLatitude = 0,
                                                    LastMeasLongitude = 0,
                                                    MeasDuration = 0,
                                                    MeasStart = DateTime.MinValue,
                                                    MeasStop = DateTime.MinValue,
                                                    PreAmp = 0,
                                                    RBW = _RBW,
                                                    VBW = _VBW,
                                                    TraceCount = 0,
                                                };
                                                sd.Trace = new Equipment.tracepoint[_TracePoints];
                                                decimal tracestep = _MeasBW / (_TracePoints - 1);
                                                decimal fstart = GSMBandMeass[i].Trace[l].freq - _MeasBW / 2;
                                                for (int y = 0; y < _TracePoints; y++)
                                                {
                                                    sd.Trace[y] = new Equipment.tracepoint() { freq = fstart + tracestep * y, level = -1000 };
                                                }
                                                Equipment.bandwidth_data bd = new Equipment.bandwidth_data()
                                                {
                                                    BWLimit = _BWLimit,
                                                    BWMarPeak = _MarPeakBW,
                                                    BWMeasMax = _NdBBWMax,
                                                    BWMeasMin = _NdBBWMin,
                                                    BWMeasured = -1,
                                                    NdBLevel = _NdbLevel,
                                                    NdBResult = new int[3] { -1, -1, -1 },
                                                    OBWPercent = _OBWP,
                                                    OBWResult = new int[3] { -1, -1, -1 },
                                                    BWIdentification = _NdBBWMin,
                                                };
                                                Equipment.channelpower_data[] cd = new Equipment.channelpower_data[]
                                                {
                                                    new Equipment.channelpower_data()
                                                    {
                                                        FreqCentr = GSMBandMeass[i].Trace[l].freq,
                                                        ChannelPowerBW = bd.BWMeasured,
                                                        ChannelPowerResult = -1000
                                                    }
                                                };
                                                #endregion
                                                localatdi_result_item ri = new localatdi_result_item()
                                                {
                                                    id_permission = -1,
                                                    id_station = "-1",
                                                    id_sector = "-1",
                                                    id_frequency = -1,
                                                    id_task = AtdiTask.task_id,
                                                    freq_centr_perm = -1,
                                                    meas_strength = -1,
                                                    meas_mask = new localatdi_elements_mask[] { },
                                                    mask_result = -1,
                                                    meas_correctness = false,
                                                    spec_data = sd,
                                                    bw_data = bd,
                                                    cp_data = cd,
                                                    station_identifier_from_radio = "",
                                                    station_identifier_from_radio_tech_sub_ind = -1,
                                                    station_identifier_atdi = "",
                                                    status = "",
                                                    user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString(),
                                                    user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials,
                                                    new_meas_data_to_send = true,
                                                    level_results_sended = false,
                                                    level_results = lmcs,
                                                    station_sys_info = new localatdi_station_sys_info(),
                                                    device_ident = GSMBandMeass[i].device_ident,
                                                    device_meas = GSMBandMeass[i].device_meas,
                                                };
                                                #endregion
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    AtdiTask.data_from_tech[j].ResultItems.Add(ri);
                                                });
                                                ATDI_AddNewResultToDB_v2(ri, AtdiTask.data_from_tech[j].result_table_name);
                                                #endregion
                                            }
                                        }
                                    }
                                    #region ненашел технологию то в неизвестную технологию
                                    if (findtech == false)
                                    {
                                        bool findInUnknownResult = false;
                                        localatdi_task_with_tech unktech = AtdiTask.data_from_tech.Where(x => x.tech == "unknown").First();
                                        #region ищем если уже было то пишем
                                        for (int z = 0; z < unktech.ResultItems.Count; z++)
                                        {
                                            if (unktech.ResultItems[z].spec_data.FreqCentr == GSMBandMeass[i].Trace[l].freq &&
                                                unktech.ResultItems[z].station_identifier_from_radio == "")
                                            //нашли такое измерение то обновим в базе и софте при условии что оно не ндп
                                            {
                                                findInUnknownResult = true;
                                                #region
                                                localatdi_level_meas_result lmc = new localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = -1,
                                                    level_dbm = GSMBandMeass[i].Trace[l].level,
                                                    level_dbmkvm = -1000,
                                                    measurement_time = GSMBandMeass[i].MeasTime,
                                                    saved_in_db = false,
                                                    saved_in_result = true,
                                                    location = new localatdi_geo_location()
                                                    {
                                                        asl = GSMBandMeass[i].altitude,
                                                        agl = -100000,
                                                        longitude = GSMBandMeass[i].longitude,
                                                        latitude = GSMBandMeass[i].latitude,
                                                    }
                                                };
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    unktech.ResultItems[z].level_results.Add(lmc);
                                                });
                                                ATDI_UpdateLCMInResultToDB_v2(unktech.ResultItems[z], unktech.result_table_name);

                                                findInUnknownResult = true;
                                                #endregion
                                            }
                                        }
                                        #endregion

                                        #region ненашли
                                        if (findInUnknownResult == false)
                                        {
                                            #region
                                            ObservableCollection<localatdi_level_meas_result> lmcs = new ObservableCollection<localatdi_level_meas_result>() { };
                                            localatdi_level_meas_result lmc = new localatdi_level_meas_result()
                                            {
                                                difference_time_stamp_ns = -1,
                                                level_dbm = GSMBandMeass[i].Trace[l].level,
                                                level_dbmkvm = -1000,
                                                measurement_time = GSMBandMeass[i].MeasTime,
                                                saved_in_db = false,
                                                saved_in_result = true,
                                                location = new localatdi_geo_location()
                                                {
                                                    agl = GSMBandMeass[i].altitude,
                                                    asl = -100000,
                                                    longitude = GSMBandMeass[i].longitude,
                                                    latitude = GSMBandMeass[i].latitude,
                                                }
                                            };
                                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                            {
                                                lmcs.Add(lmc); ;
                                            });
                                            #region
                                            #region
                                            Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.GSM;
                                            double _NdbLevel = set.Data[0].NdBLevel;
                                            decimal _NdBBWMin = set.Data[0].NdBBWMin;
                                            decimal _NdBBWMax = set.Data[0].NdBBWMax;
                                            decimal _BWLimit = set.Data[0].BWLimit;
                                            decimal _MarPeakBW = set.Data[0].MarPeakBW;

                                            decimal _MeasBW = set.Data[0].MeasBW;
                                            int _TracePoints = set.Data[0].TracePoints;
                                            decimal _RBW = set.Data[0].RBW;
                                            decimal _VBW = set.Data[0].VBW;
                                            decimal _OBWP = set.Data[0].OBWPercent;
                                            decimal _DeltaFreqLimit = set.Data[0].DeltaFreqLimit;
                                            Equipment.spectrum_data sd = new Equipment.spectrum_data()
                                            {
                                                FreqCentr = GSMBandMeass[i].Trace[l].freq,
                                                FreqSpan = _MeasBW,
                                                FreqStart = GSMBandMeass[i].Trace[l].freq - _MeasBW / 2,
                                                FreqStop = GSMBandMeass[i].Trace[l].freq + _MeasBW / 2,
                                                LastMeasAltitude = 0,
                                                LastMeasLatitude = 0,
                                                LastMeasLongitude = 0,
                                                MeasDuration = 0,
                                                MeasStart = DateTime.MinValue,
                                                MeasStop = DateTime.MinValue,
                                                PreAmp = 0,
                                                RBW = _RBW,
                                                VBW = _VBW,
                                                TraceCount = 0,
                                            };
                                            sd.Trace = new Equipment.tracepoint[_TracePoints];
                                            decimal tracestep = _MeasBW / (_TracePoints - 1);
                                            decimal fstart = GSMBandMeass[i].Trace[l].freq - _MeasBW / 2;
                                            for (int y = 0; y < _TracePoints; y++)
                                            {
                                                sd.Trace[y] = new Equipment.tracepoint() { freq = fstart + tracestep * y, level = -1000 };
                                            }
                                            Equipment.bandwidth_data bd = new Equipment.bandwidth_data()
                                            {
                                                BWLimit = _BWLimit,
                                                BWMarPeak = _MarPeakBW,
                                                BWMeasMax = _NdBBWMax,
                                                BWMeasMin = _NdBBWMin,
                                                BWMeasured = -1,
                                                NdBLevel = _NdbLevel,
                                                NdBResult = new int[3] { -1, -1, -1 },
                                                OBWPercent = _OBWP,
                                                OBWResult = new int[3] { -1, -1, -1 },
                                                BWIdentification = _NdBBWMin,
                                            };
                                            Equipment.channelpower_data[] cd = new Equipment.channelpower_data[]
                                            {
                                                    new Equipment.channelpower_data()
                                                    {
                                                        FreqCentr = GSMBandMeass[i].Trace[l].freq,
                                                        ChannelPowerBW = bd.BWMeasured,
                                                        ChannelPowerResult = -1000
                                                    }
                                            };
                                            #endregion
                                            localatdi_result_item ri = new localatdi_result_item()
                                            {
                                                id_permission = -1,
                                                id_station = "-1",
                                                id_sector = "-1",
                                                id_frequency = -1,
                                                id_task = AtdiTask.task_id,
                                                freq_centr_perm = -1,
                                                meas_strength = -1,
                                                meas_mask = new localatdi_elements_mask[] { },
                                                mask_result = -1,
                                                meas_correctness = false,
                                                spec_data = sd,
                                                bw_data = bd,
                                                cp_data = cd,
                                                station_identifier_from_radio = "",
                                                station_identifier_from_radio_tech_sub_ind = -1,
                                                station_identifier_atdi = "",
                                                status = "",
                                                user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString(),
                                                user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials,
                                                new_meas_data_to_send = true,
                                                level_results_sended = false,
                                                level_results = lmcs,
                                                station_sys_info = new localatdi_station_sys_info(),
                                                device_ident = GSMBandMeass[i].device_ident,
                                                device_meas = GSMBandMeass[i].device_meas,
                                            };
                                            #endregion
                                            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                            {
                                                unktech.ResultItems.Add(ri);
                                            });
                                            ATDI_AddNewResultToDB_v2(ri, unktech.result_table_name);
                                            #endregion

                                        }
                                        #endregion
                                    }
                                    #endregion
                                    #endregion

                                    #endregion
                                }
                                else
                                {
                                    #region ищем в результатах
                                    for (int j = 0; j < AtdiUnknownResult.data_from_tech.Count; j++)
                                    {
                                        if (AtdiUnknownResult.data_from_tech[j].tech.ToUpper().Contains("GSM"))//нашли по технологии т.к. она есть в таске
                                        {
                                            bool findInResult = false;
                                            #region
                                            for (int r = 0; r < AtdiUnknownResult.data_from_tech[j].ResultItems.Count; r++)
                                            {
                                                localatdi_result_item res = AtdiUnknownResult.data_from_tech[j].ResultItems[r];

                                                if (res.spec_data.FreqCentr == GSMBandMeass[i].Trace[l].freq && res.station_identifier_from_radio == "")
                                                {
                                                    localatdi_level_meas_result lmc = new localatdi_level_meas_result()
                                                    {
                                                        difference_time_stamp_ns = -1,
                                                        level_dbm = GSMBandMeass[i].Trace[l].level,
                                                        level_dbmkvm = -1000,
                                                        measurement_time = GSMBandMeass[i].MeasTime,
                                                        saved_in_db = false,
                                                        saved_in_result = true,
                                                        location = new localatdi_geo_location()
                                                        {
                                                            asl = GSMBandMeass[i].altitude,
                                                            agl = -100000,
                                                            longitude = GSMBandMeass[i].longitude,
                                                            latitude = GSMBandMeass[i].latitude,
                                                        }
                                                    };
                                                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                    {
                                                        res.level_results.Add(lmc);
                                                    });
                                                    ATDI_UpdateLCMInResultToDB_v2(res, AtdiUnknownResult.data_from_tech[j].result_table_name);
                                                    for (int lr = 0; lr < res.level_results.Count; lr++)
                                                    {
                                                        res.level_results[lr].saved_in_db = true;
                                                        res.level_results[lr].saved_in_result = true;
                                                    }
                                                    findInResult = true;
                                                }
                                            }
                                            #endregion
                                            if (findInResult == false)
                                            {
                                                #region
                                                ObservableCollection<localatdi_level_meas_result> lmcs = new ObservableCollection<localatdi_level_meas_result>() { };
                                                localatdi_level_meas_result lmc = new localatdi_level_meas_result()
                                                {
                                                    difference_time_stamp_ns = -1,
                                                    level_dbm = GSMBandMeass[i].Trace[l].level,
                                                    level_dbmkvm = -1000,
                                                    measurement_time = GSMBandMeass[i].MeasTime,
                                                    saved_in_db = false,
                                                    saved_in_result = true,
                                                    location = new localatdi_geo_location()
                                                    {
                                                        agl = GSMBandMeass[i].altitude,
                                                        asl = -100000,
                                                        longitude = GSMBandMeass[i].longitude,
                                                        latitude = GSMBandMeass[i].latitude,
                                                    }
                                                };
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    lmcs.Add(lmc); ;
                                                });
                                                #region
                                                #region
                                                Settings.MeasMonTech_Set set = App.Sett.MeasMons_Settings.GSM;
                                                double _NdbLevel = set.Data[0].NdBLevel;
                                                decimal _NdBBWMin = set.Data[0].NdBBWMin;
                                                decimal _NdBBWMax = set.Data[0].NdBBWMax;
                                                decimal _BWLimit = set.Data[0].BWLimit;
                                                decimal _MarPeakBW = set.Data[0].MarPeakBW;

                                                decimal _MeasBW = set.Data[0].MeasBW;
                                                int _TracePoints = set.Data[0].TracePoints;
                                                decimal _RBW = set.Data[0].RBW;
                                                decimal _VBW = set.Data[0].VBW;
                                                decimal _OBWP = set.Data[0].OBWPercent;
                                                decimal _DeltaFreqLimit = set.Data[0].DeltaFreqLimit;
                                                Equipment.spectrum_data sd = new Equipment.spectrum_data()
                                                {
                                                    FreqCentr = GSMBandMeass[i].Trace[l].freq,
                                                    FreqSpan = _MeasBW,
                                                    FreqStart = GSMBandMeass[i].Trace[l].freq - _MeasBW / 2,
                                                    FreqStop = GSMBandMeass[i].Trace[l].freq + _MeasBW / 2,
                                                    LastMeasAltitude = 0,
                                                    LastMeasLatitude = 0,
                                                    LastMeasLongitude = 0,
                                                    MeasDuration = 0,
                                                    MeasStart = DateTime.MinValue,
                                                    MeasStop = DateTime.MinValue,
                                                    PreAmp = 0,
                                                    RBW = _RBW,
                                                    VBW = _VBW,
                                                    TraceCount = 0,
                                                };
                                                sd.Trace = new Equipment.tracepoint[_TracePoints];
                                                decimal tracestep = _MeasBW / (_TracePoints - 1);
                                                decimal fstart = GSMBandMeass[i].Trace[l].freq - _MeasBW / 2;
                                                for (int y = 0; y < _TracePoints; y++)
                                                {
                                                    sd.Trace[y] = new Equipment.tracepoint() { freq = fstart + tracestep * y, level = -1000 };
                                                }
                                                Equipment.bandwidth_data bd = new Equipment.bandwidth_data()
                                                {
                                                    BWLimit = _BWLimit,
                                                    BWMarPeak = _MarPeakBW,
                                                    BWMeasMax = _NdBBWMax,
                                                    BWMeasMin = _NdBBWMin,
                                                    BWMeasured = -1,
                                                    NdBLevel = _NdbLevel,
                                                    NdBResult = new int[3] { -1, -1, -1 },
                                                    OBWPercent = _OBWP,
                                                    OBWResult = new int[3] { -1, -1, -1 },
                                                    BWIdentification = _NdBBWMin,
                                                };
                                                Equipment.channelpower_data[] cd = new Equipment.channelpower_data[]
                                                {
                                                    new Equipment.channelpower_data()
                                                    {
                                                        FreqCentr = GSMBandMeass[i].Trace[l].freq,
                                                        ChannelPowerBW = bd.BWMeasured,
                                                        ChannelPowerResult = -1000
                                                    }
                                                };
                                                #endregion
                                                localatdi_result_item ri = new localatdi_result_item()
                                                {
                                                    id_permission = -1,
                                                    id_station = "-1",
                                                    id_sector = "-1",
                                                    id_frequency = -1,
                                                    id_task = "",
                                                    freq_centr_perm = -1,
                                                    meas_strength = -1,
                                                    meas_mask = new localatdi_elements_mask[] { },
                                                    mask_result = -1,
                                                    meas_correctness = false,
                                                    spec_data = sd,
                                                    bw_data = bd,
                                                    cp_data = cd,
                                                    station_identifier_from_radio = "",
                                                    station_identifier_from_radio_tech_sub_ind = -1,
                                                    station_identifier_atdi = "",
                                                    status = "",
                                                    user_id = App.Sett.UsersApps_Settings.SelectedUser.ID.ToString(),
                                                    user_name = App.Sett.UsersApps_Settings.SelectedUser.LAST_NAME + " " + App.Sett.UsersApps_Settings.SelectedUser.Initials,
                                                    new_meas_data_to_send = true,
                                                    level_results_sended = false,
                                                    level_results = lmcs,
                                                    station_sys_info = new localatdi_station_sys_info(),
                                                    device_ident = GSMBandMeass[i].device_ident,
                                                    device_meas = GSMBandMeass[i].device_meas,
                                                };
                                                #endregion
                                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                                {
                                                    AtdiUnknownResult.data_from_tech[j].ResultItems.Add(ri);
                                                });
                                                ATDI_AddNewResultToDB_v2(ri, AtdiUnknownResult.data_from_tech[j].result_table_name);
                                                #endregion
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        GSMBandMeass[i].saved = true;
                        #endregion
                    }
                }
                bandres += DateTime.Now.Ticks - bandres1;
                //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = sss;
                //}));
            }
            catch (Exception exp)
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            finally
            {
                //long ttt = DateTime.Now.Ticks;
                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    ((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Data " + (data / 10000000).ToString() + "\r\nLevelCar " + (lr / 10000000).ToString() + "\r\nBandGSM " + (bandres / 10000000).ToString();
                //}));
                //ddddd = DateTime.Now.Ticks;
                dbt -= SaveUpdateATDIMeasData_v2;
            }
        }
        private bool AddTechToUnknownResult(localatdi_unknown_result atdiUnknownResult, string newtech)
        {
            bool res = false;
            NpgsqlConnection.GlobalTypeMapper.Reset();
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_result_state_data>("localatdi_result_state_data");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_unknown_result_with_tech>("localatdi_unknown_result_with_tech");
            bool resadd = false;
            string result_table_name = ("unknown_result_" + atdiUnknownResult.time_start.Year + "_" + atdiUnknownResult.time_start.Month + "_" + newtech).ToLower();
            using (NpgsqlConnection ncdb = new NpgsqlConnection(UserconnToDb))
            {
                #region Result
                #region проверка существования таблицы загружаемого Task и создание таблицы Task
                bool ResultTaskTableExist = false;
                string SQL = "SELECT relname, pg_class.relkind as relkind FROM pg_class, pg_namespace WHERE pg_class.relnamespace = " +
                    "pg_namespace.oid AND pg_class.relkind IN ('r') AND relname = '" + result_table_name + "';";
                //using (NpgsqlCommand command = new NpgsqlCommand(SQL, ncdb))
                //{
                //    ncdb.Open();
                //    NpgsqlDataReader dr2 = command.ExecuteReader();
                //    while (dr2.Read())
                //    {
                //        string temp = dr2[0] as string;
                //        if (temp.ToLower().Contains(result_table_name))
                //        {
                //            ResultTaskTableExist = true;
                //            //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = "Такой план уже есть!";
                //        }
                //    }
                //    ncdb.Close();
                //}
                #endregion
                #region создаем таблицы по технологиям
                SQL = "";
                //if (ResultTaskTableExist == false)
                //{
                //    SQL = "DROP TABLE " + result_table_name + ";\r\n";
                //}
                ///пишем посекторно (если несколько секторов то дублируем инфу)
                SQL += "CREATE TABLE " + result_table_name + " " +
                    "(id_permission integer, id_station character varying(200), id_sector character varying(200), " +
                    "id_frequency integer, id_task character varying(200), freq_centr_perm numeric, " +
                    "meas_strength numeric, meas_mask localatdi_elements_mask[], mask_result integer, " +
                    "meas_correctness boolean, spec_data spectrum_data, bw_data bandwidth_data, cp_data channelpower_data[], " +
                    "station_identifier_from_radio character varying(200), station_identifier_from_radio_tech_sub_ind integer, " +
                    "station_identifier_atdi character varying(200), " +
                    "status character varying(300), user_id character varying(200), user_name character varying(500), " +
                    "new_meas_data_to_send boolean, level_results_sended boolean, level_results localatdi_level_meas_result[], " +
                    "station_sys_info localatdi_station_sys_info, device_ident localatdi_meas_device, device_meas localatdi_meas_device" +
                    ")WITH(OIDS = FALSE)";
                using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, ncdb))
                {
                    ncdb.Open();
                    int recordAffected = cmd.ExecuteNonQuery();
                    resadd = Convert.ToBoolean(recordAffected);
                    ncdb.Close();
                    cmd.Dispose();

                }

                if (resadd)//записать в строку localatdi_unknown_result
                {
                    localatdi_unknown_result_with_tech rt = new localatdi_unknown_result_with_tech() { };
                    rt.result_table_name = result_table_name;
                    rt.tech = newtech;
                    ObservableCollection<localatdi_unknown_result_with_tech> dft = atdiUnknownResult.data_from_tech;
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        dft.Add(rt);
                    }));
                    SQL = "UPDATE localatdi_unknown_result SET data_from_tech = @data_from_tech " +
                         "WHERE time_start = @time_start AND time_stop = @time_stop;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, ncdb))
                    {
                        #region Command   
                        cmd.CommandText = SQL;
                        ncdb.Open();
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(new NpgsqlParameter[]
                        {
                            new NpgsqlParameter { ParameterName= "@data_from_tech", Value = dft.ToArray(), DataTypeName ="localatdi_unknown_result_with_tech[]"},
                            new NpgsqlParameter("@time_start", atdiUnknownResult.time_start),
                            new NpgsqlParameter("@time_stop", atdiUnknownResult.time_stop),
                        });
                        int recordAffected = cmd.ExecuteNonQuery();
                        if (Convert.ToBoolean(recordAffected))
                        {
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                atdiUnknownResult.data_from_tech = dft;
                            }));
                            res = true;
                        }
                        cmd.Dispose();
                        ncdb.Close();
                        #endregion
                    }
                }
                ncdb.Dispose();
                #endregion
                #endregion
            }
            return res;
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////

        public void ATDI_UpdateResultToDB_v2(bool NewSpecDataToSave, localatdi_result_item res, string TableName)
        {
            #region Data
            NpgsqlConnection.GlobalTypeMapper.Reset();
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.tracepoint>("tracepoint");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.spectrum_data>("spectrum_data");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.bandwidth_data>("bandwidth_data");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.channelpower_data>("channelpower_data");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_elements_mask>("localatdi_elements_mask");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_level_meas_result>("localatdi_level_meas_result");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sys_info>("localatdi_station_sys_info");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_meas_device>("localatdi_meas_device");
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                string SQL = "";
                if (NewSpecDataToSave == true)
                    SQL = "UPDATE " + TableName.ToLower() + " SET " +
                        "id_permission = :id_permission, id_task = :id_task, freq_centr_perm = :freq_centr_perm, " +
                        "meas_strength = :meas_strength, meas_mask = :meas_mask, mask_result = :mask_result, " +
                        "meas_correctness = :meas_correctness, spec_data = :spec_data, bw_data = :bw_data, " +
                        "cp_data = :cp_data, station_identifier_atdi = :station_identifier_atdi, " +
                        "status = :status, user_id = :user_id, user_name = :user_name, new_meas_data_to_send = :new_meas_data_to_send, " +
                        "level_results_sended = :level_results_sended, level_results = array_cat(level_results, :level_results), " +
                        "station_sys_info = :station_sys_info, device_ident = :device_ident, device_meas = :device_meas " +

                        "WHERE id_station = :id_station AND id_sector = :id_sector AND id_frequency = id_frequency AND " +
                        "(spec_data).freq_centr = :freq_centr AND station_identifier_from_radio = :station_identifier_from_radio AND " +
                        "station_identifier_from_radio_tech_sub_ind = :station_identifier_from_radio_tech_sub_ind;";
                else
                    SQL = "UPDATE " + TableName.ToLower() + " SET " +
                        "id_permission = :id_permission, id_task = :id_task, freq_centr_perm = :freq_centr_perm, " +
                        "meas_strength = :meas_strength, meas_mask = :meas_mask, mask_result = :mask_result, " +
                        "meas_correctness = :meas_correctness, station_identifier_atdi = :station_identifier_atdi, " +
                        "status = :status, user_id = :user_id, user_name = :user_name, new_meas_data_to_send = :new_meas_data_to_send, " +
                        "level_results_sended = :level_results_sended, level_results = array_cat(level_results, :level_results), " +
                        "station_sys_info = :station_sys_info, device_ident = :device_ident, device_meas = :device_meas " +

                        "WHERE id_station = :id_station AND id_sector = :id_sector AND id_frequency = id_frequency AND " +
                        "(spec_data).freq_centr = :freq_centr AND station_identifier_from_radio = :station_identifier_from_radio AND " +
                        "station_identifier_from_radio_tech_sub_ind = :station_identifier_from_radio_tech_sub_ind;";
                try
                {
                    ObservableCollection<localatdi_level_meas_result> new_level_measurements_car = new ObservableCollection<localatdi_level_meas_result>() { };
                    for (int i = 0; i < res.level_results.Count; i++)
                    {
                        if (res.level_results[i].saved_in_db == false)
                        {
                            res.level_results[i].saved_in_db = true;
                            new_level_measurements_car.Add(res.level_results[i]);
                        }
                    }
                    cdb.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        #region Command
                        if (NewSpecDataToSave == true)
                        {
                            cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region
                                new NpgsqlParameter(":id_permission", res.id_permission),
                                new NpgsqlParameter(":id_task", res.id_task),
                                new NpgsqlParameter(":freq_centr_perm", res.freq_centr_perm),
                                new NpgsqlParameter(":meas_strength", res.meas_strength),
                                new NpgsqlParameter(":meas_mask", res.meas_mask),
                                new NpgsqlParameter(":mask_result", res.mask_result),
                                new NpgsqlParameter(":meas_correctness", res.meas_correctness),
                                new NpgsqlParameter(":spec_data", res.spec_data),
                                new NpgsqlParameter(":bw_data", res.bw_data),
                                new NpgsqlParameter(":cp_data", res.cp_data),

                                new NpgsqlParameter(":station_identifier_atdi", res.station_identifier_atdi),
                                new NpgsqlParameter(":status", res.status),
                                new NpgsqlParameter(":user_id", res.user_id),
                                new NpgsqlParameter(":user_name", res.user_name),
                                new NpgsqlParameter(":new_meas_data_to_send", res.new_meas_data_to_send),
                                new NpgsqlParameter(":level_results_sended", res.level_results_sended),
                                ////
                                new NpgsqlParameter(":level_results", new_level_measurements_car.ToArray()),
                                new NpgsqlParameter(":station_sys_info", res.station_sys_info),
                                new NpgsqlParameter(":device_ident", res.device_ident),
                                new NpgsqlParameter(":device_meas", res.device_meas),


                                new NpgsqlParameter(":id_station", res.id_station),
                                new NpgsqlParameter(":id_sector", res.id_sector),
                                new NpgsqlParameter(":id_frequency", res.id_frequency),
                                new NpgsqlParameter(":freq_centr", res.spec_data.FreqCentr),
                                new NpgsqlParameter(":station_identifier_from_radio", res.station_identifier_from_radio),
                                new NpgsqlParameter(":station_identifier_from_radio_tech_sub_ind", res.station_identifier_from_radio_tech_sub_ind),
                                #endregion
                            });
                        }
                        else
                        {
                            cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region
                                new NpgsqlParameter(":id_permission", res.id_permission),
                                new NpgsqlParameter(":id_task", res.id_task),
                                new NpgsqlParameter(":freq_centr_perm", res.freq_centr_perm),
                                new NpgsqlParameter(":meas_strength", res.meas_strength),
                                new NpgsqlParameter(":meas_mask", res.meas_mask),
                                new NpgsqlParameter(":mask_result", res.mask_result),
                                new NpgsqlParameter(":meas_correctness", res.meas_correctness),

                                new NpgsqlParameter(":station_identifier_atdi", res.station_identifier_atdi),
                                new NpgsqlParameter(":status", res.status),
                                new NpgsqlParameter(":user_id", res.user_id),
                                new NpgsqlParameter(":user_name", res.user_name),
                                new NpgsqlParameter(":new_meas_data_to_send", res.new_meas_data_to_send),
                                new NpgsqlParameter(":level_results_sended", res.level_results_sended),
                                ////
                                new NpgsqlParameter(":level_results", new_level_measurements_car.ToArray()),
                                new NpgsqlParameter(":station_sys_info", res.station_sys_info),
                                new NpgsqlParameter(":device_ident", res.device_ident),
                                new NpgsqlParameter(":device_meas", res.device_meas),


                                new NpgsqlParameter(":id_station", res.id_station),
                                new NpgsqlParameter(":id_sector", res.id_sector),
                                new NpgsqlParameter(":id_frequency", res.id_frequency),
                                new NpgsqlParameter(":freq_centr", res.spec_data.FreqCentr),
                                new NpgsqlParameter(":station_identifier_from_radio", res.station_identifier_from_radio),
                                new NpgsqlParameter(":station_identifier_from_radio_tech_sub_ind", res.station_identifier_from_radio_tech_sub_ind),
                                #endregion
                            });
                        }
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                    //for (int i = 0; i < res.level_results.Count; i++)
                    //{
                    //    res.level_results[i].saved_in_db = true;
                    //}

                    //cdb.Close();
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData()
                        {
                            ex = exp,
                            ClassName = "NpgsqlDB_NpgsqlException",
                            AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" +
                            "TB:" + TableName + " FREQ:" + res.spec_data.FreqCentr + "\r\n" +
                            "NewSpecDataToSave = " + NewSpecDataToSave.ToString()
                        };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData()
                        {
                            ex = exp,
                            ClassName = "NpgsqlDB_Exception",
                            AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + "\r\n" +
                            "TB:" + TableName + " FREQ:" + res.spec_data.FreqCentr + "\r\n" +
                            "NewSpecDataToSave = " + NewSpecDataToSave.ToString()
                        };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
            }
            #endregion
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public void ATDI_AddNewResultToDB_v2(localatdi_result_item res, string TableName)
        {
            NpgsqlConnection.GlobalTypeMapper.Reset();
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.tracepoint>("tracepoint");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.spectrum_data>("spectrum_data");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.bandwidth_data>("bandwidth_data");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<Equipment.channelpower_data>("channelpower_data");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_elements_mask>("localatdi_elements_mask");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_level_meas_result>("localatdi_level_meas_result");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station_sys_info>("localatdi_station_sys_info");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_meas_device>("localatdi_meas_device");
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                string SQL = "INSERT INTO " + TableName.ToLower() +
                       "(id_permission, id_station, id_sector, id_frequency, id_task, freq_centr_perm, meas_strength, " +
                       "meas_mask, mask_result, meas_correctness, spec_data, bw_data, cp_data, " +
                       "station_identifier_from_radio, station_identifier_from_radio_tech_sub_ind, " +
                       "station_identifier_atdi, status, user_id, user_name, new_meas_data_to_send, " +
                       "level_results_sended, level_results, station_sys_info, device_ident, device_meas)" +
                       "VALUES (" +
                       "@id_permission, @id_station, @id_sector, @id_frequency, @id_task, @freq_centr_perm, @meas_strength, " +
                       "@meas_mask, @mask_result, @meas_correctness, @spec_data, @bw_data, @cp_data, " +
                       "@station_identifier_from_radio, @station_identifier_from_radio_tech_sub_ind, " +
                       "@station_identifier_atdi, @status, @user_id, @user_name, @new_meas_data_to_send, " +
                       "@level_results_sended, @level_results, @station_sys_info, @device_ident, @device_meas);";
                try
                {
                    cdb.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        for (int i = 0; i < res.level_results.Count; i++)
                        {
                            res.level_results[i].saved_in_db = true;
                        }
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region
                                new NpgsqlParameter("@id_permission", res.id_permission),
                                new NpgsqlParameter("@id_station", res.id_station),
                                new NpgsqlParameter("@id_sector", res.id_sector),
                                new NpgsqlParameter("@id_frequency", res.id_frequency),
                                new NpgsqlParameter("@id_task", res.id_task),
                                new NpgsqlParameter("@freq_centr_perm", res.freq_centr_perm),
                                new NpgsqlParameter("@meas_strength", res.meas_strength),
                                new NpgsqlParameter("@meas_mask", res.meas_mask),
                                new NpgsqlParameter("@mask_result", res.mask_result),
                                new NpgsqlParameter("@meas_correctness", res.meas_correctness),
                                new NpgsqlParameter("@spec_data", res.spec_data),
                                new NpgsqlParameter("@bw_data", res.bw_data),
                                new NpgsqlParameter("@cp_data", res.cp_data),
                                new NpgsqlParameter("@station_identifier_from_radio", res.station_identifier_from_radio),
                                new NpgsqlParameter("@station_identifier_from_radio_tech_sub_ind", res.station_identifier_from_radio_tech_sub_ind),
                                new NpgsqlParameter("@station_identifier_atdi", res.station_identifier_atdi),
                                new NpgsqlParameter("@status", res.status),
                                new NpgsqlParameter("@user_id", res.user_id),
                                new NpgsqlParameter("@user_name", res.user_name),
                                new NpgsqlParameter("@new_meas_data_to_send", res.new_meas_data_to_send),
                                new NpgsqlParameter("@level_results_sended", res.level_results_sended),
                                new NpgsqlParameter("@level_results", res.level_results.ToArray()),
                                new NpgsqlParameter("@station_sys_info", res.station_sys_info),
                                new NpgsqlParameter("@device_ident", res.device_ident),
                                new NpgsqlParameter("@device_meas", res.device_meas),
                                #endregion
                            });
                        cmd.ExecuteNonQuery();
                    }

                    cdb.Close();
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
            }

        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public void ATDI_UpdateTaskItemMeasDataExistToDB_v2(string id_station, string callsign_db, bool newstate, string TableName)
        {
            #region Data
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_station>("localatdi_station");
                string SQL = "UPDATE " + TableName.ToLower() + " SET " +
                    "meas_data_exist = :meas_data_exist " +
                    "WHERE id = :id AND " +
                    "callsign_db = :callsign_db;";
                try
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        #region Command
                        cdb.Open();
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region
                                new NpgsqlParameter(":meas_data_exist", newstate),
                                new NpgsqlParameter(":id", id_station),
                                new NpgsqlParameter(":callsign_db", callsign_db),
                                #endregion
                            });
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
                //NpgsqlConnection.GlobalTypeMapper.UnmapComposite<atdi_mask_element>();
                //NpgsqlConnection.GlobalTypeMapper.Reset();
                //NpgsqlConnection.ClearAllPools();
            }
            #endregion
        }

        public void ATDI_UpdateLCMInResultToDB_v2(localatdi_result_item res, string TableName)
        {
            #region Data
            NpgsqlConnection.GlobalTypeMapper.Reset();
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_level_meas_result>("localatdi_level_meas_result");
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                string SQL = "UPDATE " + TableName.ToLower() + " SET " +
                    "user_id = :user_id, user_name = :user_name, new_meas_data_to_send = :new_meas_data_to_send, " +
                    "level_results_sended = :level_results_sended, level_results = array_cat(level_results, :level_results) " +


                    "WHERE id_station = :id_station AND id_sector = :id_sector AND id_frequency = :id_frequency AND " +
                    "(spec_data).freq_centr = :freq_centr AND station_identifier_from_radio = :station_identifier_from_radio AND " +
                    "station_identifier_from_radio_tech_sub_ind = :station_identifier_from_radio_tech_sub_ind;";
                try
                {
                    ObservableCollection<localatdi_level_meas_result> new_level_results = new ObservableCollection<localatdi_level_meas_result>() { };
                    for (int i = 0; i < res.level_results.Count; i++)
                    {
                        if (res.level_results[i].saved_in_db == false)
                        {
                            res.level_results[i].saved_in_db = true;
                            new_level_results.Add(res.level_results[i]);
                        }
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        #region Command
                        cdb.Open();
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                            #region                            
                            new NpgsqlParameter(":user_id", res.user_id),
                            new NpgsqlParameter(":user_name", res.user_name),
                            new NpgsqlParameter(":new_meas_data_to_send", res.new_meas_data_to_send),
                            new NpgsqlParameter(":level_results_sended", res.level_results_sended),
                            new NpgsqlParameter(":level_results", new_level_results.ToArray()),

                            new NpgsqlParameter(":id_station", res.id_station),
                            new NpgsqlParameter(":id_sector", res.id_sector),
                            new NpgsqlParameter(":id_frequency", res.id_frequency),
                            new NpgsqlParameter(":freq_centr", res.spec_data.FreqCentr),
                            new NpgsqlParameter(":station_identifier_from_radio", res.station_identifier_from_radio),
                            new NpgsqlParameter(":station_identifier_from_radio_tech_sub_ind", res.station_identifier_from_radio_tech_sub_ind),
                                #endregion
                            });
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                    //for (int i = 0; i < res.level_results.Count; i++)
                    //{
                    //    res.level_results[i].saved_in_db = true;
                    //}
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
                //cdb.Close();
            }
            #endregion
        }

        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ATDI_AddNewResultInfoToDB_v2(localatdi_result_state_data resinfo, bool Task, string IdOrYYYYMM)
        {
            bool res = false;
            NpgsqlConnection.GlobalTypeMapper.Reset();
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_result_state_data>("localatdi_result_state_data");
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                resinfo.SaveInDB = true;
                localatdi_result_state_data[] arr = new localatdi_result_state_data[] { resinfo };
                string SQL = "";
                if (Task)
                    SQL = "UPDATE localatdi_meas_task SET " +
                        "results_info = array_cat(results_info, :results_info) " +
                        "WHERE task_id = :task_id;";
                else
                {
                    SQL = "UPDATE localatdi_unknown_result SET " +
                           "results_info = array_cat(results_info, :results_info) " +
                           "WHERE id = :id;";
                }
                try
                {
                    cdb.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        if (Task)
                            cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region                                
                                new NpgsqlParameter(":results_info", arr),
                                new NpgsqlParameter(":task_id", IdOrYYYYMM),
                                #endregion
                            });
                        else
                            cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region                                
                                new NpgsqlParameter(":results_info", arr),
                                new NpgsqlParameter(":id", IdOrYYYYMM),
                                #endregion
                            });
                        int recordAffected = cmd.ExecuteNonQuery();
                        res = Convert.ToBoolean(recordAffected);
                    }
                    cdb.Close();
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
            }
            return res;
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ATDI_UpdateResultInfoToDB_v2(localatdi_result_state_data[] resinfo, bool Task, string IdOrYYYYMM)
        {
            bool res = false;
            //NpgsqlConnection.GlobalTypeMapper.Reset();
            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_result_state_data>("localatdi_result_state_data");
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                //localatdi_result_state_data[] arr = new localatdi_result_state_data[] { resinfo };
                string SQL = "";
                if (Task)
                    SQL = "UPDATE localatdi_meas_task SET " +
                        "results_info = :results_info " +
                        "WHERE task_id = :task_id;";
                else
                {
                    SQL = "UPDATE localatdi_unknown_result SET " +
                           "results_info = :results_info " +
                           "WHERE id = :id;";
                }
                try
                {
                    cdb.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        if (Task)
                            cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region                                
                                new NpgsqlParameter(":results_info", resinfo),
                                new NpgsqlParameter(":task_id", IdOrYYYYMM),
                                #endregion
                            });
                        else
                            cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region                                
                                new NpgsqlParameter(":results_info", resinfo),
                                new NpgsqlParameter(":id", IdOrYYYYMM),
                                #endregion
                            });
                        int recordAffected = cmd.ExecuteNonQuery();
                        res = Convert.ToBoolean(recordAffected);
                    }
                    cdb.Close();
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Status">Success / Fail</param>
        /// <param name="ResultId">номер результата</param>
        /// <param name="Message">текст ошибки</param>
        /// <returns></returns>
        public bool ATDI_FindUpdateResultInfoToDB_v2(string Status, string ResultId, string Message, DateTime ResponseReceived)
        {
            bool res = false;
            bool findintask = false;
            bool findinunk = false;

            for (int i = 0; i < AtdiTasks.Count(); i++)
            {
                for (int j = 0; j < AtdiTasks[i].ResultsInfo.Count(); j++)
                {
                    if (AtdiTasks[i].ResultsInfo[j].ResultId.ToLower() == ResultId.ToLower())
                    {
                        findintask = true;
                        AtdiTasks[i].ResultsInfo[j].ResponseReceived = ResponseReceived;
                        if (Status.ToLower().Contains("success"))
                        {
                            AtdiTasks[i].ResultsInfo[j].DeliveryConfirmation = 2;
                            AtdiTasks[i].ResultsInfo[j].ErrorText = "";
                            AtdiTasks[i].ResultsInfo[j].SaveInDB = true;
                        }
                        else if (Status.ToLower().Contains("fail"))
                        {
                            AtdiTasks[i].ResultsInfo[j].DeliveryConfirmation = 2;
                            AtdiTasks[i].ResultsInfo[j].ErrorText = Message;
                            AtdiTasks[i].ResultsInfo[j].SaveInDB = true;
                        }
                        res = ATDI_UpdateResultInfoToDB_v2(AtdiTasks[i].ResultsInfo.ToArray(), findintask, AtdiTasks[i].task_id);
                    }
                }
            }
            if (findintask == false)
            {
                for (int i = 0; i < AtdiUnknownResults.Count(); i++)
                {
                    for (int j = 0; j < AtdiUnknownResults[i].ResultsInfo.Count(); j++)
                    {
                        if (AtdiUnknownResults[i].ResultsInfo[j].ResultId.ToLower() == ResultId.ToLower())
                        {
                            findinunk = true;
                            AtdiUnknownResults[i].ResultsInfo[j].ResponseReceived = ResponseReceived;
                            if (Status.ToLower().Contains("success"))
                            {
                                AtdiUnknownResults[i].ResultsInfo[j].DeliveryConfirmation = 2;
                                AtdiUnknownResults[i].ResultsInfo[j].ErrorText = "";
                                AtdiUnknownResults[i].ResultsInfo[j].SaveInDB = true;
                            }
                            else if (Status.ToLower().Contains("fail"))
                            {
                                AtdiUnknownResults[i].ResultsInfo[j].DeliveryConfirmation = 2;
                                AtdiUnknownResults[i].ResultsInfo[j].ErrorText = Message;
                                AtdiUnknownResults[i].ResultsInfo[j].SaveInDB = true;
                            }
                            res = ATDI_UpdateResultInfoToDB_v2(AtdiUnknownResults[i].ResultsInfo.ToArray(), findintask, AtdiUnknownResults[i].id);
                        }
                    }
                }
            }
            if (findinunk == false)
            {
                //for (int i = 0; i < AtdiUnknownResults.Count(); i++)
                //{
                //    for (int j = 0; j < AtdiUnknownResults[i].ResultsInfo.Count(); j++)
                //    {
                //        if (AtdiUnknownResults[i].ResultsInfo[j].ResultId.ToLower() == ResultId.ToLower())
                //        {
                //            AtdiUnknownResults[i].ResultsInfo[j].ResponseReceived = ResponseReceived;
                //            if (Status.ToLower().Contains("success"))
                //            {
                //                AtdiUnknownResults[i].ResultsInfo[j].DeliveryConfirmation = 2;
                //                AtdiUnknownResults[i].ResultsInfo[j].ErrorText = "";
                //                AtdiUnknownResults[i].ResultsInfo[j].SaveInDB = true;
                //            }
                //            else if (Status.ToLower().Contains("fail"))
                //            {
                //                AtdiUnknownResults[i].ResultsInfo[j].DeliveryConfirmation = 2;
                //                AtdiUnknownResults[i].ResultsInfo[j].ErrorText = Message;
                //                AtdiUnknownResults[i].ResultsInfo[j].SaveInDB = true;
                //            }
                //            res = ATDI_UpdateResultInfoToDB_v2(AtdiUnknownResults[i].ResultsInfo.ToArray(), findintask, AtdiUnknownResults[i].id);
                //        }
                //    }
                //}
            }
            return res;
        }
        #endregion сохранение результатов



        /// <summary>
        /// Всякая байда потом разобраться
        /// </summary>
        #region 
        public void add()
        {
            if (true)
            {
                CoorFind = CoorFind;
                System.Windows.MessageBox.Show("");
            }
        }
        public void sameWork()
        {
            Thread.Sleep(1);
        }
        private void LoadDataworks()
        {
            while (_DataCycle)
            {
                dbt();
                //foreach (Delegate d in dbt.GetInvocationList())
                //{
                //    Text += ((DBTreadDelegate)d).Method.Name + "\r\n";
                //}
                //VisibilityProgressBar = false;
            }
            tr.Abort();
        }

        private void ADDTestTask()
        {
            Atdi.DataModels.Sdrns.Device.MeasTask t = new ClassTest().res();
            //App.Current.Dispatcher.Invoke((Action)(() =>
            //{

            //    AtdiTasks.Add(new ATDI.AtdiDataConverter().ConvertToLocal(t));
            //}));

            dbt -= ADDTestTask;
        }
        #region ATDI //////////////////////////////////////////////////////////////////////////////////////////////////////////////


        #region Load Data From DB






        #endregion


        #region Send Data Measurment to ICSM
        public int ATDI_DeleteTaskWithoutResult()
        {
            int outint = 0;
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                cdb.Open();
                #region проверка существования таблицы rs135ids
                string SQL = "SELECT result_send_id FROM atdi_ids;";

                using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                {
                    NpgsqlDataReader dr;
                    dr = command.ExecuteReader();
                    dr.Read();
                    outint = (int)dr[0];
                    cdb.Close();
                }
                #endregion
                outint++;
                cdb.Open();
                #region Data
                if (cdb.State == ConnectionState.Open)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand("UPDATE atdi_ids SET result_send_id = :result_send_id WHERE id = 0", cdb))
                    {
                        try
                        {
                            command.Parameters.Add(new NpgsqlParameter(":result_send_id", NpgsqlTypes.NpgsqlDbType.Integer));
                            // Prepare the command.
                            command.Prepare();
                            // Add value to the paramater.
                            command.Parameters[0].Value = outint;
                            // Execute SQL command.
                            int recordAffected = command.ExecuteNonQuery();
                            if (Convert.ToBoolean(recordAffected))
                            {
                                //System.Windows.MessageBox.Show("Data successfully saved!");
                            }
                        }
                        catch (Exception exp)
                        { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
                    }
                }
                #endregion
                cdb.Close();
            }
            return outint;
        }

        #endregion

        #region Save Received Data
        public void ATDI_UpdateTaskItemMeasDataExistToDB(int id_station, int sector_id, int permission_id, string station_callsign, bool newstate, string TableName)
        {
            #region Data
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<atdi_mask_element>("atdi_mask_element");
                string SQL = "update " + TableName.ToLower() + " set " +
                    "meas_data_exist = :meas_data_exist " +
                    "WHERE station_id = :station_id " +/*AND sector_id = :sector_id*/ "AND permission_id = :permission_id AND " +
                    "station_callsign = :station_callsign;";
                try
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        #region Command
                        cdb.Open();
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region
                                new NpgsqlParameter(":meas_data_exist", newstate),
                                new NpgsqlParameter(":station_id", id_station),
                                //new NpgsqlParameter(":sector_id", sector_id),
                                new NpgsqlParameter(":permission_id", permission_id),
                                new NpgsqlParameter(":station_callsign", station_callsign),
                                #endregion
                            });
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
                //NpgsqlConnection.GlobalTypeMapper.UnmapComposite<atdi_mask_element>();
                //NpgsqlConnection.GlobalTypeMapper.Reset();
                //NpgsqlConnection.ClearAllPools();
            }
            #endregion
        }



        public void ATDI_UpdateTaskResultToDB(LocalAtdiResultTaskItem res, string TableName)
        {
            #region Data
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                //NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<atdi_mask_element>("atdi_mask_element");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<LacalAtdi_LevelMeasurementsCar>("atdi_level_measurements_car");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
                string SQL = "update " + TableName.ToLower() + " set " +
                    "permission_id = :permission_id, meas_task_id = :meas_task_id, meas_sub_task_id = :meas_sub_task_id, " +
                    "meas_sub_task_station_id = :meas_sub_task_station_id, " +
                    "sensor_id = :sensor_id, nn = :nn, sw_number = :sw_number, " +
                    "measurment_last_latitude = :measurment_last_latitude, measurment_last_longitude = :measurment_last_longitude, measurment_altitude = :measurment_altitude, " +

                    "measurment_strength = :measurment_strength, measurment_duration = :measurment_duration, measurment_datetime = :measurment_datetime, " +
                    "measurment_datetime_start = :measurment_datetime_start, measurment_freq_centr_permision = :measurment_freq_centr_permision, measurment_sector_mask = :measurment_sector_mask, " +
                    "sources_freq_start = :sources_freq_start, sources_freq_stop = :sources_freq_stop, " +
                    "sources_freq_step = :sources_freq_step, sources_trace = :sources_trace, sources_trace_level_type = :sources_trace_level_type, " +
                    "sources_m1_marker_index = :sources_m1_marker_index, sources_t1_marker_index = :sources_t1_marker_index, sources_t2_marker_index = :sources_t2_marker_index, " +
                    "station_identifier_atdi = :station_identifier_atdi, station_status = :station_status, " +
                    "user_id = :user_id, user_name = :user_name, new_meas_data_to_send = :new_meas_data_to_send, " +
                    "level_measurements_car_sended = :level_measurements_car_sended, level_measurements_car = array_cat(level_measurements_car, :level_measurements_car), " +
                    "information_blocks = :information_blocks " +

                    "WHERE id_station = :id_station AND sector_id = :sector_id AND frequency_id = :frequency_id AND " +
                    "sources_freq_centr = :sources_freq_centr AND station_identifier_from_radio = :station_identifier_from_radio AND " +
                    "station_identifier_from_radio_tech_sub_ind = :station_identifier_from_radio_tech_sub_ind;";
                try
                {
                    ObservableCollection<LacalAtdi_LevelMeasurementsCar> new_level_measurements_car = new ObservableCollection<LacalAtdi_LevelMeasurementsCar>() { };
                    for (int i = 0; i < res.level_measurements_car.Count; i++)
                    {
                        if (res.level_measurements_car[i].saved_in_db == false)
                        {
                            res.level_measurements_car[i].saved_in_db = true;
                            new_level_measurements_car.Add(res.level_measurements_car[i]);
                        }
                    }

                    cdb.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        #region Command
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                            #region
                            new NpgsqlParameter(":permission_id", res.permission_id),
                            new NpgsqlParameter(":meas_task_id", res.meas_task_id),
                            new NpgsqlParameter(":meas_sub_task_id", res.meas_sub_task_id),
                            new NpgsqlParameter(":meas_sub_task_station_id", res.meas_sub_task_station_id),
                            new NpgsqlParameter(":sensor_id", res.sensor_id),

                            new NpgsqlParameter(":nn", res.nn),
                            new NpgsqlParameter(":sw_number", res.sw_number),
                            new NpgsqlParameter(":measurment_last_latitude", res.measurment_last_latitude),
                            new NpgsqlParameter(":measurment_last_longitude", res.measurment_last_longitude),
                            new NpgsqlParameter(":measurment_altitude", res.measurment_altitude),
                            new NpgsqlParameter(":measurment_strength", res.measurment_strength),
                            new NpgsqlParameter(":measurment_duration", res.measurment_duration),
                            new NpgsqlParameter(":measurment_datetime", res.measurment_datetime),
                            new NpgsqlParameter(":measurment_datetime_start", res.measurment_datetime_start),
                            new NpgsqlParameter(":measurment_freq_centr_permision", res.measurment_freq_centr_permision),
                            new NpgsqlParameter(":measurment_sector_mask", res.measurment_sector_mask.ToArray()),
                            new NpgsqlParameter(":sources_freq_start", res.sources_freq_start),
                            new NpgsqlParameter(":sources_freq_stop", res.sources_freq_stop),
                            new NpgsqlParameter(":sources_freq_step", res.sources_freq_step),
                            new NpgsqlParameter(":sources_trace", res.sources_trace),
                            new NpgsqlParameter(":sources_trace_level_type", res.sources_trace_level_type),
                            new NpgsqlParameter(":sources_m1_marker_index", res.sources_m1_marker_index),
                            new NpgsqlParameter(":sources_t1_marker_index", res.sources_t1_marker_index),
                            new NpgsqlParameter(":sources_t2_marker_index", res.sources_t2_marker_index),
                            new NpgsqlParameter(":station_identifier_atdi", res.station_identifier_atdi),
                            new NpgsqlParameter(":station_status", res.station_status),
                            new NpgsqlParameter(":user_id", res.user_id),
                            new NpgsqlParameter(":user_name", res.user_name),
                            new NpgsqlParameter(":new_meas_data_to_send", res.new_meas_data_to_send),
                            new NpgsqlParameter(":level_measurements_car_sended", res.level_measurements_car_sended),
                            new NpgsqlParameter(":level_measurements_car", new_level_measurements_car.ToArray()),
                            new NpgsqlParameter(":information_blocks", res.information_blocks),


                            new NpgsqlParameter(":id_station", res.id_station),
                            new NpgsqlParameter(":sector_id", res.sector_id),
                            new NpgsqlParameter(":frequency_id", res.frequency_id),
                            new NpgsqlParameter(":sources_freq_centr", res.sources_freq_centr),
                            new NpgsqlParameter(":station_identifier_from_radio", res.station_identifier_from_radio),
                            new NpgsqlParameter(":station_identifier_from_radio_tech_sub_ind", res.station_identifier_from_radio_tech_sub_ind),
                                #endregion
                            });
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                    for (int i = 0; i < res.level_measurements_car.Count; i++)
                    {
                        res.level_measurements_car[i].saved_in_db = true;
                    }
                    //cdb.Close();
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
            }
            #endregion
        }

        public void ATDI_AddNewTaskResultToDB(LocalAtdiResultTaskItem res, string TableName)
        {
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                //NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<atdi_mask_element>("atdi_mask_element");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<LacalAtdi_LevelMeasurementsCar>("atdi_level_measurements_car");
                NpgsqlConnection.GlobalTypeMapper.MapComposite<local3GPPSystemInformationBlock>("local_3gpp_system_information_block");
                string SQL = "INSERT INTO " + TableName.ToLower() +
                       "(permission_id, id_station, sector_id, frequency_id, meas_task_id, meas_sub_task_id, meas_sub_task_station_id, " +
                       "sensor_id, nn, sw_number, measurment_last_latitude, measurment_last_longitude, measurment_altitude, " +
                       "measurment_strength, measurment_duration, measurment_datetime, measurment_datetime_start, " +
                       "measurment_freq_centr_permision, measurment_sector_mask, " +
                       "sources_freq_centr, sources_freq_start, sources_freq_stop, sources_freq_step, sources_trace, sources_trace_level_type, " +
                       "sources_m1_marker_index, sources_t1_marker_index, sources_t2_marker_index, " +
                       "station_identifier_from_radio, station_identifier_from_radio_tech_sub_ind, " +
                       "station_identifier_atdi, station_status, " +
                       "user_id, user_name, new_meas_data_to_send, level_measurements_car_sended, level_measurements_car, information_blocks)" +
                       "VALUES (" +
                       "@permission_id, @id_station, @sector_id, @frequency_id, @meas_task_id, @meas_sub_task_id, @meas_sub_task_station_id, " +
                       "@sensor_id, @nn, @sw_number, @measurment_last_latitude, @measurment_last_longitude, @measurment_altitude, " +
                       "@measurment_strength, @measurment_duration, @measurment_datetime, @measurment_datetime_start, " +
                       "@measurment_freq_centr_permision, @measurment_sector_mask, " +
                       "@sources_freq_centr, @sources_freq_start, @sources_freq_stop, @sources_freq_step, @sources_trace, @sources_trace_level_type, " +
                       "@sources_m1_marker_index, @sources_t1_marker_index, @sources_t2_marker_index, " +
                       "@station_identifier_from_radio, @station_identifier_from_radio_tech_sub_ind, " +
                       "@station_identifier_atdi, @station_status, " +
                       "@user_id, @user_name, @new_meas_data_to_send, @level_measurements_car_sended, @level_measurements_car, @information_blocks);";
                try
                {
                    cdb.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                                #region
                                new NpgsqlParameter("@permission_id", res.permission_id),
                                new NpgsqlParameter("@id_station", res.id_station),
                                new NpgsqlParameter("@sector_id", res.sector_id),
                                new NpgsqlParameter("@frequency_id", res.frequency_id),
                                new NpgsqlParameter("@meas_task_id", res.meas_task_id),
                                new NpgsqlParameter("@meas_sub_task_id", res.meas_sub_task_id),
                                new NpgsqlParameter("@meas_sub_task_station_id", res.meas_sub_task_station_id),
                                new NpgsqlParameter("@sensor_id", res.sensor_id),
                                new NpgsqlParameter("@nn", res.nn),
                                new NpgsqlParameter("@sw_number", res.sw_number),
                                new NpgsqlParameter("@measurment_last_latitude", res.measurment_last_latitude),
                                new NpgsqlParameter("@measurment_last_longitude", res.measurment_last_longitude),
                                new NpgsqlParameter("@measurment_altitude", res.measurment_altitude),
                                new NpgsqlParameter("@measurment_strength", res.measurment_strength),
                                new NpgsqlParameter("@measurment_duration", res.measurment_duration),
                                new NpgsqlParameter("@measurment_datetime", res.measurment_datetime),
                                new NpgsqlParameter("@measurment_datetime_start", res.measurment_datetime_start),
                                new NpgsqlParameter("@measurment_freq_centr_permision", res.measurment_freq_centr_permision),
                                new NpgsqlParameter("@measurment_sector_mask", res.measurment_sector_mask.ToArray()),
                                new NpgsqlParameter("@sources_freq_centr", res.sources_freq_centr),
                                new NpgsqlParameter("@sources_freq_start", res.sources_freq_start),
                                new NpgsqlParameter("@sources_freq_stop", res.sources_freq_stop),
                                new NpgsqlParameter("@sources_freq_step", res.sources_freq_step),
                                new NpgsqlParameter("@sources_trace", res.sources_trace),
                                new NpgsqlParameter("@sources_trace_level_type", res.sources_trace_level_type),
                                new NpgsqlParameter("@sources_m1_marker_index", res.sources_m1_marker_index),
                                new NpgsqlParameter("@sources_t1_marker_index", res.sources_t1_marker_index),
                                new NpgsqlParameter("@sources_t2_marker_index", res.sources_t2_marker_index),
                                new NpgsqlParameter("@station_identifier_from_radio", res.station_identifier_from_radio),
                                new NpgsqlParameter("@station_identifier_from_radio_tech_sub_ind", res.station_identifier_from_radio_tech_sub_ind),
                                new NpgsqlParameter("@station_identifier_atdi", res.station_identifier_atdi),
                                new NpgsqlParameter("@station_status", res.station_status),
                                new NpgsqlParameter("@user_id", res.user_id),
                                new NpgsqlParameter("@user_name", res.user_name),
                                new NpgsqlParameter("@new_meas_data_to_send", res.new_meas_data_to_send),
                                new NpgsqlParameter("@level_measurements_car_sended", res.level_measurements_car_sended),
                                new NpgsqlParameter("@level_measurements_car", res.level_measurements_car.ToArray()),
                                new NpgsqlParameter("@information_blocks", res.information_blocks),

                                #endregion
                            });
                        cmd.ExecuteNonQuery();
                    }
                    for (int i = 0; i < res.level_measurements_car.Count; i++)
                    {
                        res.level_measurements_car[i].saved_in_db = true;
                    }
                    cdb.Close();
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
            }

        }
        /// <summary>
        /// добавляем level_measurements_car к резкльтатам
        /// </summary>
        /// <param name="res"></param>
        /// <param name="TableName"></param>
        public void ATDI_UpdateLCMInTaskResultToDB(LocalAtdiResultTaskItem res, string TableName)
        {
            #region Data
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                //NpgsqlConnection.GlobalTypeMapper.Reset();
                NpgsqlConnection.GlobalTypeMapper.MapComposite<LacalAtdi_LevelMeasurementsCar>("atdi_level_measurements_car");
                string SQL = "update " + TableName.ToLower() + " set " +
                    "user_id = :user_id, user_name = :user_name, new_meas_data_to_send = :new_meas_data_to_send, " +
                    "level_measurements_car_sended = :level_measurements_car_sended, level_measurements_car = array_cat(level_measurements_car, :level_measurements_car) " +


                    "WHERE id_station = :id_station AND sector_id = :sector_id AND frequency_id = :frequency_id AND " +
                    "sources_freq_centr = :sources_freq_centr AND station_identifier_from_radio = :station_identifier_from_radio AND " +
                    "station_identifier_from_radio_tech_sub_ind = :station_identifier_from_radio_tech_sub_ind;";
                try
                {
                    ObservableCollection<LacalAtdi_LevelMeasurementsCar> new_level_measurements_car = new ObservableCollection<LacalAtdi_LevelMeasurementsCar>() { };
                    for (int i = 0; i < res.level_measurements_car.Count; i++)
                    {
                        if (res.level_measurements_car[i].saved_in_db == false)
                        {
                            res.level_measurements_car[i].saved_in_db = true;
                            new_level_measurements_car.Add(res.level_measurements_car[i]);
                        }
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                    {
                        #region Command
                        cdb.Open();
                        cmd.Parameters.AddRange(
                            new NpgsqlParameter[]
                            {
                            #region                            
                            new NpgsqlParameter(":user_id", res.user_id),
                            new NpgsqlParameter(":user_name", res.user_name),
                            new NpgsqlParameter(":new_meas_data_to_send", res.new_meas_data_to_send),
                            new NpgsqlParameter(":level_measurements_car_sended", res.level_measurements_car_sended),
                            new NpgsqlParameter(":level_measurements_car", new_level_measurements_car.ToArray()),

                            new NpgsqlParameter(":id_station", res.id_station),
                            new NpgsqlParameter(":sector_id", res.sector_id),
                            new NpgsqlParameter(":frequency_id", res.frequency_id),
                            new NpgsqlParameter(":sources_freq_centr", res.sources_freq_centr),
                            new NpgsqlParameter(":station_identifier_from_radio", res.station_identifier_from_radio),
                            new NpgsqlParameter(":station_identifier_from_radio_tech_sub_ind", res.station_identifier_from_radio_tech_sub_ind),
                                #endregion
                            });
                        cmd.ExecuteNonQuery();
                        #endregion
                    }
                    for (int i = 0; i < res.level_measurements_car.Count; i++)
                    {
                        res.level_measurements_car[i].saved_in_db = true;
                    }
                }
                catch (NpgsqlException exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                catch (Exception exp)
                {
                    App.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                    }));
                }
                finally
                {
                    if (cdb != null) cdb.Close();
                }
                //cdb.Close();
            }
            #endregion
        }

        #endregion

        #region Send Data Measurment to ICSM
        public int ATDI_AddResultID()
        {
            int outint = 0;
            using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
            {
                cdb.Open();
                #region получаем текущий id результата
                string SQL = "SELECT result_send_id FROM atdi_ids;";
                using (NpgsqlCommand command = new NpgsqlCommand(SQL, cdb))
                {
                    NpgsqlDataReader dr;
                    dr = command.ExecuteReader();
                    dr.Read();
                    outint = (int)dr[0];
                    cdb.Close();
                }
                #endregion
                outint++;
                cdb.Open();
                #region Data
                if (cdb.State == ConnectionState.Open)
                {
                    using (NpgsqlCommand command = new NpgsqlCommand("UPDATE atdi_ids SET result_send_id = :result_send_id WHERE id = 0", cdb))
                    {
                        try
                        {
                            command.Parameters.Add(new NpgsqlParameter(":result_send_id", NpgsqlTypes.NpgsqlDbType.Integer));
                            // Prepare the command.
                            command.Prepare();
                            // Add value to the paramater.
                            command.Parameters[0].Value = outint;
                            // Execute SQL command.
                            int recordAffected = command.ExecuteNonQuery();
                            if (Convert.ToBoolean(recordAffected))
                            {
                                //System.Windows.MessageBox.Show("Data successfully saved!");
                            }
                        }
                        catch (Exception exp)
                        { MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name }; }
                    }
                }
                #endregion
                cdb.Close();
            }
            return outint;
        }
        #endregion


        #region следилка за координатами и запись Route
        private void GPSCoor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Contains("LatitudeD") || e.PropertyName.Contains("LongitudeD"))
            {
                bool methods = false;
                foreach (Delegate d in GPSt.GetInvocationList())
                {
                    if (methods == false &&
                        (((DBTreadDelegate)d).Method.Name == "SaveLevelMeasurementsCarToMonMeas" || ((DBTreadDelegate)d).Method.Name == "AddPointToRoute"))
                    { methods = true; }
                }
                if (methods == false)
                {
                    GPSt += SaveLevelMeasurementsCarToMonMeas;
                }
            }
        }
        private void GPSDataWorks()
        {
            while (DataCycleGPS)
            {
                GPSt();
                //foreach (Delegate d in dbt.GetInvocationList())
                //{
                //    Text += ((DBTreadDelegate)d).Method.Name + "\r\n";
                //}
                //VisibilityProgressBar = false;
            }
            trGPS.Abort();
        }
        public double LastLatitude
        {
            get { return _LastLatitude; }
            set { _LastLatitude = value; }
        }
        private double _LastLatitude = 0;
        public double LastLongitude
        {
            get { return _LastLongitude; }
            set { _LastLongitude = value; }
        }
        private double _LastLongitude = 0;

        /// <summary>
        /// шаг записи уровней в м
        /// </summary>
        public decimal Atdi_Route_DistanceStep
        {
            get { return _Atdi_Route_DistanceStep; }
            set { _Atdi_Route_DistanceStep = value; }
        }
        private decimal _Atdi_Route_DistanceStep = 100;

        public decimal Atdi_LevelResults_DistanceStep
        {
            get { return _Atdi_LevelResults_DistanceStep; }
            set { _Atdi_LevelResults_DistanceStep = value; }
        }
        private decimal _Atdi_LevelResults_DistanceStep = (decimal)App.Sett.MeasMons_Settings.TrackDistanceStep;
        /// <summary>
        /// шаг записи уровней в мин
        /// </summary>
        public TimeSpan Atdi_LevelsMeasurementsCar_TimeStep
        {
            get { return _Atdi_LevelsMeasurementsCar_TimeStep; }
            set { _Atdi_LevelsMeasurementsCar_TimeStep = value; }
        }
        private TimeSpan _Atdi_LevelsMeasurementsCar_TimeStep = new TimeSpan(0, 0, App.Sett.MeasMons_Settings.TrackTimeStep); //new TimeSpan(0, 1, 0);
                                                                                                                              /// <summary>
                                                                                                                              /// приклепать напряженность поля, антенн сейчас нет
                                                                                                                              /// </summary>
        private void SaveLevelMeasurementsCarToMonMeas()
        {
            double dist = 0, ang = 0;
            MainWindow.help.calcDistance(
                  LastLatitude,
                  LastLongitude,
                  (double)MainWindow.gps.LatitudeDecimal,
                  (double)MainWindow.gps.LongitudeDecimal,
                  out dist, out ang);
            if ((decimal)dist > Atdi_Route_DistanceStep)
            {
                LastLatitude = (double)MainWindow.gps.LatitudeDecimal;
                LastLongitude = (double)MainWindow.gps.LongitudeDecimal;
                GPSt += AddPointToRoute;

            }
            //((SplashWindow)App.Current.MainWindow).m_mainWindow.Message = new TimeSpan(DateTime.Now.Ticks - LastUpdateUDP).ToString();
            GPSt -= SaveLevelMeasurementsCarToMonMeas;
        }
        // v2 ////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Записывает новую точку в роут работает до бесконечности пока не получит нормальных координат и если все норм то самовыпиливается
        /// </summary>
        private void AddPointToRoute()
        {
            try
            {
                if (MainWindow.gps.LatitudeDecimal != 0 && MainWindow.gps.LongitudeDecimal != 0 && MainWindow.gps.Altitude != 0)
                {
                    string TableName = "";
                    int RouteId = 0;
                    if (MeasMon.FromTask)
                    {
                        TableName = AtdiTask.routes_tb_name;
                        RouteId = AtdiTask.routes_id;
                    }
                    else
                    {
                        TableName = AtdiUnknownResult.routes_tb_name;
                        RouteId = AtdiUnknownResult.routes_id;
                    }
                    localatdi_route_point p = new localatdi_route_point() { };
                    p.start_time = MainWindow.gps.LocalTime;
                    p.finish_time = MainWindow.gps.LocalTime;
                    p.point_stay_type = (int)Atdi.DataModels.Sdrns.PointStayType.InMove;
                    p.route_id = RouteId;// AtdiUnknownResult.routes.Max(x => x.route_id);// 0;
                    p.location = new localatdi_geo_location()
                    {
                        asl = MainWindow.gps.Altitude,
                        latitude = (double)MainWindow.gps.LatitudeDecimal,
                        longitude = (double)MainWindow.gps.LongitudeDecimal,
                    };
                    if (TableName != "")
                    {
                        using (NpgsqlConnection cdb = new NpgsqlConnection(UserconnToDb))
                        {
                            //NpgsqlConnection.GlobalTypeMapper.Reset();
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_geo_location>("localatdi_geo_location");
                            NpgsqlConnection.GlobalTypeMapper.MapComposite<localatdi_route_point>("localatdi_route_point");
                            #region add
                            cdb.Open();
                            bool add = false;
                            string SQL = "INSERT INTO " + TableName +
                                " (data) VALUES (@data);";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(SQL, cdb))
                            {
                                cmd.Parameters.Add(new NpgsqlParameter("@data", p));
                                int recordAffected = cmd.ExecuteNonQuery();
                                add = Convert.ToBoolean(recordAffected);
                                if (add)
                                    App.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        if (MeasMon.FromTask)
                                        {
                                            AtdiTask.routes.Add(p);
                                        }
                                        else
                                        {
                                            AtdiUnknownResult.routes.Add(p);
                                        }
                                    }));
                            }
                            cdb.Close();
                            #endregion

                        }

                    }
                }
            }
            catch (NpgsqlException exp)
            {
                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                //}));
            }
            catch (Exception exp)
            {
                //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "NpgsqlDB", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                //}));
            }
            finally
            {
                Thread.Sleep(10);//костыль чтоб елсли залипнет то не грузило проц
                GPSt -= AddPointToRoute;
            }
        }

        #endregion

        #endregion
        #endregion
    }
    /// <summary>
    /// определяет один сохраненный пеленг
    /// </summary>
    public class DFData : INotifyPropertyChanged
    {
        /// <summary>
        /// время получения пеленга из GNSS по UTC скорее всего 
        /// </summary>
        public ulong TimeStamp
        {
            get { return _TimeStamp; }
            set { _TimeStamp = value; OnPropertyChanged("TimeStamp"); }
        }
        private ulong _TimeStamp = 0;

        /// <summary>
        /// центральная частота
        /// </summary>
        public decimal DFCentrFrequency
        {
            get { return _DFCentrFrequency; }
            set { _DFCentrFrequency = value; OnPropertyChanged("DFCentrFrequency"); }
        }
        private decimal _DFCentrFrequency = 0;
        /// <summary>
        /// центральная частота
        /// </summary>
        public decimal DFSpanFrequency
        {
            get { return _DFSpanFrequency; }
            set { _DFSpanFrequency = value; OnPropertyChanged("DFSpanFrequency"); }
        }
        private decimal _DFSpanFrequency = 0;

        /// <summary>
        /// азимут пеленга
        /// </summary>
        public decimal DFAzimuth
        {
            get { return _DFAzimuth; }
            set { _DFAzimuth = value; OnPropertyChanged("DFAzimuth"); }
        }
        private decimal _DFAzimuth = 0;

        /// <summary>
        /// азимут компаса
        /// </summary>
        public decimal CompassAzimuth
        {
            get { return _CompassAzimuth; }
            set { _CompassAzimuth = value; OnPropertyChanged("CompassAzimuth"); }
        }
        private decimal _CompassAzimuth = 0;
        /// <summary>
        /// уровень сигнала по пеленгу
        /// </summary>
        public decimal DFLevel
        {
            get { return _DFLevel; }
            set { _DFLevel = value; OnPropertyChanged("DFLevel"); }
        }
        private decimal _DFLevel = 0;
        /// <summary>
        /// качевство пеленга
        /// </summary>
        public decimal DFQuality
        {
            get { return _DFQuality; }
            set { _DFQuality = value; OnPropertyChanged("DFQuality"); }
        }
        private decimal _DFQuality = 0;
        /// <summary>
        /// напряженность сигнала
        /// </summary>
        public decimal DFLevelStrength
        {
            get { return _DFLevelStrength; }
            set { _DFLevelStrength = value; OnPropertyChanged("DFLevelStrength"); }
        }
        private decimal _DFLevelStrength = 0;
        /// <summary>
        /// полоса пеленга
        /// </summary>
        public decimal DFBW
        {
            get { return _DFBW; }
            set { _DFBW = value; OnPropertyChanged("DFBW"); }
        }
        private decimal _DFBW = 0;
        /// <summary>
        /// step on trace
        /// </summary>
        public decimal TraceStep
        {
            get { return _TraceStep; }
            set { _TraceStep = value; OnPropertyChanged("TraceStep"); }
        }
        private decimal _TraceStep = 0;
        /// <summary>
        /// координаты текущего пеленга
        /// </summary>
        public decimal Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; OnPropertyChanged("Latitude"); }
        }
        private decimal _Latitude = 0;
        /// <summary>
        /// координаты текущего пеленга
        /// </summary>
        public decimal Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; OnPropertyChanged("Longitude"); }
        }
        private decimal _Longitude = 0;

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MeasMon : PropertyChangedBase
    {
        public int Mode
        {
            get { return _Mode; }
            set { _Mode = value; OnPropertyChanged("Mode"); }
        }
        private int _Mode = 0;
        /// <summary>
        /// Всего БС по базе с GCID
        /// </summary>
        public int ATDI_BTSCountWithGCID
        {
            get { return _ATDI_BTSCountWithGCID; }
            set { _ATDI_BTSCountWithGCID = value; OnPropertyChanged("ATDI_BTSCountWithGCID"); }
        }
        private int _ATDI_BTSCountWithGCID = 0;

        /// <summary>
        /// Количевство НДП (которые не идентифицировались по базе)
        /// </summary>
        public int ATDI_BTSCountNDP
        {
            get { return _ATDI_BTSCountNDP; }
            set { _ATDI_BTSCountNDP = value; OnPropertyChanged("ATDI_BTSCountNDP"); }
        }
        private int _ATDI_BTSCountNDP = 0;

        /// <summary>
        /// Количевство ППЕ (которые идентифицировались по базе но не совпали по частотам)
        /// </summary>
        public int ATDI_BTSCountNPE
        {
            get { return _ATDI_BTSCountNPE; }
            set { _ATDI_BTSCountNPE = value; OnPropertyChanged("ATDI_BTSCountNPE"); }
        }
        private int _ATDI_BTSCountNPE = 0;

        /// <summary>
        /// всего БС увиденых
        /// </summary>
        public int BTSCount
        {
            get { return _BTSCount; }
            set { _BTSCount = value; OnPropertyChanged("BTSCount"); }
        }
        private int _BTSCount = 0;

        /// <summary>
        /// Всего БС с GCID
        /// </summary>
        public int BTSCountWithGCID
        {
            get { return _BTSCountWithGCID; }
            set { _BTSCountWithGCID = value; OnPropertyChanged("BTSCountWithGCID"); }
        }
        private int _BTSCountWithGCID = 0;

        /// <summary>
        /// обновляет инфу о станциях
        /// </summary>
        public void UpdateBTSInfo()
        {
            #region пилим количевство всякого по базам и т.д.
            int BTSCountWithGCID = 0;

            int RS135_BTSCountWithGCID = 0;
            int RS135_BTSCountNDP = 0;
            int RS135_BTSCountNPE = 0;

            int ATDI_BTSCountWithGCID = 0;
            int ATDI_BTSCountNDP = 0;
            int ATDI_BTSCountNPE = 0;
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].ATDI_Identifier_Find == 1) ATDI_BTSCountNDP++;
                if (Data[i].ATDI_Identifier_Find == 2 && Data[i].ATDI_FreqCheck_Find == 1) ATDI_BTSCountNPE++;
                if (Data[i].ATDI_Identifier_Find == 2) ATDI_BTSCountWithGCID++;

                //всего с идентификатором
                if (Data[i].FullData == true) BTSCountWithGCID++;
            }
            GUIThreadDispatcher.Instance.Invoke(() =>
            {
                ATDI_BTSCountWithGCID = ATDI_BTSCountWithGCID;
                ATDI_BTSCountNDP = ATDI_BTSCountNDP;
                ATDI_BTSCountNPE = ATDI_BTSCountNPE;

                BTSCount = Data.Count;
                BTSCountWithGCID = BTSCountWithGCID;
            });

            #endregion
        }

        public ObservableCollection<MeasData> Data
        {
            get { return _Data; }
            set { _Data = value; OnPropertyChanged("Data"); }
        }
        public ObservableCollection<MeasData> _Data = new ObservableCollection<MeasData>() { };

        public bool FromTask
        {
            get { return _FromTask; }
            set { _FromTask = value; OnPropertyChanged("FromTask"); }
        }
        private bool _FromTask = false;

        public ObservableCollection<MeasMonBand> Bands
        {
            get { return _Bands; }
            set { _Bands = value; OnPropertyChanged("Bands"); }
        }
        public ObservableCollection<MeasMonBand> _Bands = new ObservableCollection<MeasMonBand>() { };

        public ObservableCollection<localatdi_freq_occupancy> FreqOccupancy
        {
            get { return _FreqOccupancy; }
            set { _FreqOccupancy = value; OnPropertyChanged("FreqOccupancy"); }
        }
        private ObservableCollection<localatdi_freq_occupancy> _FreqOccupancy = new ObservableCollection<localatdi_freq_occupancy>() { };

        public void GetUnifreqsOccupancy()
        {
            #region 
            List<decimal> freq = new List<decimal>() { };
            foreach (Settings.GSMBand_Set t in App.Sett.TSMxReceiver_Settings.GSM.Bands)
            {
                if (t.Use == true)
                    for (decimal i = t.FreqStart; i <= t.FreqStop; i += 200000)
                    { freq.Add(i); }
            }
            foreach (Settings.UMTSFreqs_Set t in App.Sett.TSMxReceiver_Settings.UMTS.Freqs)
            {
                if (t.Use == true)
                { freq.Add(t.FreqDn); }
            }
            foreach (Settings.LTEFreqs_Set t in App.Sett.TSMxReceiver_Settings.LTE.Freqs)
            {
                if (t.Use == true)
                { freq.Add(t.FreqDn); }
            }
            foreach (Settings.CDMAFreqs_Set t in App.Sett.TSMxReceiver_Settings.CDMA.Freqs)
            {
                if (t.Use == true)
                { freq.Add(t.FreqDn); }
            }
            System.Collections.Generic.HashSet<decimal> hs = new System.Collections.Generic.HashSet<decimal>();
            foreach (decimal al in freq)
            {
                hs.Add(al);
            }
            ObservableCollection<decimal> freqsoccupancy = new ObservableCollection<decimal>(hs.OrderBy(i => i));
            FreqOccupancy.Clear();
            for (int i = 0; i < freqsoccupancy.Count; i++)
            {
                localatdi_freq_occupancy lfo = new localatdi_freq_occupancy()
                {
                    freq = freqsoccupancy[i],
                    count = 0,
                    occupancy = 0
                };
                FreqOccupancy.Add(lfo);
            }
            #endregion
        }

        public int[] TracePointsOnSpectrum
        {
            get { return new int[] { 501, 551, 1001, 1501, 1601, 2001 }; }
        }
        //public double DetectionLevelGSM = -100;
        //public double DetectionLevelUMTS = -100;
        //public double DetectionLevelLTE = -100;
        //public double DetectionLevelCDMA = -100;


        //public void GetTechSettings()
        //{
        //    DetectionLevelGSM = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.GSM).First().DetectionLevel;
        //    DetectionLevelUMTS = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.UMTS).First().DetectionLevel;
        //    DetectionLevelLTE = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.LTE).First().DetectionLevel;
        //    DetectionLevelCDMA = App.Sett.MeasMons_Settings.MeasMons.Where(x => x.Techonology == DB.Technologys.CDMA).First().DetectionLevel;

        //}
    }

    /// <summary>
    /// Класс для измерений
    /// </summary>    
    public class MeasData : PropertyChangedBase
    {
        #region ATDI
        #region ID
        public string ATDI_Id_Task
        {
            get { return _ATDI_MeasTask_id; }
            set { _ATDI_MeasTask_id = value; OnPropertyChanged("ATDI_MeasTask_id"); }
        }
        private string _ATDI_MeasTask_id = "";

        public string ATDI_Id_Station
        {
            get { return _ATDI_Id_Station; }
            set { _ATDI_Id_Station = value; OnPropertyChanged("ATDI_Id_Station"); }
        }
        private string _ATDI_Id_Station = "";

        public int ATDI_Id_Permission
        {
            get { return _ATDI_Id_Permission; }
            set { _ATDI_Id_Permission = value; OnPropertyChanged("ATDI_Id_Permission"); }
        }
        private int _ATDI_Id_Permission = 0;

        public string ATDI_Id_Sector
        {
            get { return _ATDI_Id_Sector; }
            set { _ATDI_Id_Sector = value; OnPropertyChanged("ATDI_Id_Sector"); }
        }
        private string _ATDI_Id_Sector = "";
        #endregion ID

        public string ATDI_GCID
        {
            get { return _ATDI_GCID; }
            set { _ATDI_GCID = value; OnPropertyChanged("ATDI_GCID"); }
        }
        private string _ATDI_GCID = string.Empty;

        /// <summary>
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_Identifier_Find
        {
            get { return _ATDI_Identifier_Find; }
            set { _ATDI_Identifier_Find = value; OnPropertyChanged("ATDI_Identifier_Find"); }
        }
        private int _ATDI_Identifier_Find = 0;

        #endregion

        /// <summary>
        /// Это сейчас мереется
        /// </summary>
        public bool ThisToMeas
        {
            get { return _ThisToMeas; }
            set { _ThisToMeas = value; OnPropertyChanged("ThisToMeas"); }
        }
        private bool _ThisToMeas = false;

        public localatdi_elements_mask[] MeasMask
        {
            get { return _MeasMask; }
            set { _MeasMask = value; OnPropertyChanged("MeasMask"); }
        }
        private localatdi_elements_mask[] _MeasMask = new localatdi_elements_mask[] { };

        /// <summary>
        /// 0 = не меряли(нет маски) 1 = плохо 2 =хорошо 
        /// </summary>
        public int MaskResult
        {
            get { return _MaskResult; }
            set { _MaskResult = value; OnPropertyChanged("MaskResult"); }
        }
        private int _MaskResult = 0;

        public localatdi_station_sys_info station_sys_info
        {
            get { return _station_sys_info; }
            set { _station_sys_info = value; OnPropertyChanged("station_sys_info"); }
        }
        private localatdi_station_sys_info _station_sys_info = new localatdi_station_sys_info() { };

        public decimal difference_time_stamp_ns
        {
            get { return _difference_time_stamp_ns; }
            set { _difference_time_stamp_ns = value; OnPropertyChanged("difference_time_stamp_ns"); }
        }
        private decimal _difference_time_stamp_ns = 0;

        #region Frequency
        public int ATDI_Id_Frequency
        {
            get { return _ATDI_Id_Frequency; }
            set { _ATDI_Id_Frequency = value; OnPropertyChanged("ATDI_Id_Frequency"); }
        }
        private int _ATDI_Id_Frequency = 0;

        public decimal ATDI_FrequencyPermission
        {
            get { return _ATDI_FrequencyPermission; }
            set { _ATDI_FrequencyPermission = value; OnPropertyChanged("ATDI_FrequencyPermission"); }
        }
        private decimal _ATDI_FrequencyPermission = 0;

        /// <summary>
        /// если нашлась строка в плане то здесь отметится соответствие частоте
        /// 0 не искали
        /// 1 искали не нашли
        /// 2 нашли
        /// </summary>
        public int ATDI_FreqCheck_Find
        {
            get { return _ATDI_FreqCheck_Find; }
            set { _ATDI_FreqCheck_Find = value; OnPropertyChanged("ATDI_FreqCheck_Find"); }
        }
        private int _ATDI_FreqCheck_Find = 0;

        /// <summary>
        /// Относительное отклонение центральной частоты
        /// в 1*10Е-6
        /// </summary>
        public decimal DeltaFreqMeasured
        {
            get { return _DeltaFreqMeasured; }
            set
            {
                _DeltaFreqMeasured = value;
                if (_DeltaFreqMeasured > -1 && _DeltaFreqMeasured <= _DeltaFreqLimit)
                { DeltaFreqConclusion = 2; }
                else if (_DeltaFreqMeasured > -1 && _DeltaFreqMeasured > _DeltaFreqLimit)
                { DeltaFreqConclusion = 1; }
                OnPropertyChanged("DeltaFreqMeasured");
            }
        }
        private decimal _DeltaFreqMeasured = -1;

        /// <summary>
        /// не привышать эту полосу сигнала 
        /// </summary>
        public decimal DeltaFreqLimit
        {
            get { return _DeltaFreqLimit; }
            set
            {
                _DeltaFreqLimit = value;
                if (_DeltaFreqMeasured > -1 && _DeltaFreqMeasured <= _DeltaFreqLimit)
                { DeltaFreqConclusion = 2; }
                else if (_DeltaFreqMeasured > -1 && _DeltaFreqMeasured > _DeltaFreqLimit)
                { DeltaFreqConclusion = 1; }
                OnPropertyChanged("DeltaFreqLimit");
            }
        }
        private decimal _DeltaFreqLimit = -1;

        /// <summary>
        /// 0 еще не мерялось
        /// 1 мерялось нарушение
        /// 2 мерялось Агонь
        /// </summary>
        public int DeltaFreqConclusion
        {
            get { return _DeltaFreqConclusion; }
            set { _DeltaFreqConclusion = value; OnPropertyChanged("DeltaFreqConclusion"); }
        }
        private int _DeltaFreqConclusion = 0;
        #endregion

        #region BW
        ///// <summary>
        ///// полоса сигнала из идентификации (с эфира)
        ///// </summary>
        //public decimal BWIdentification
        //{
        //    get { return _BWIdentification; }
        //    set { _BWIdentification = value; BWIdentificationComparison(); OnPropertyChanged("BWIdentification"); }
        //}
        //private decimal _BWIdentification = -1;

        ///// <summary>
        ///// результат сравнения полосы сигнала из идентификации(с эфира) и из разрешения
        ///// \t 0 = не сравнивалось 1 = не совпадает (всеплохо) 2 = совпадает (все хорошо)
        ///// </summary>
        //public decimal BWIdentificationComparisonResult
        //{
        //    get { return _BWIdentificationComparisonResult; }
        //    set { _BWIdentificationComparisonResult = value; OnPropertyChanged("BWIdentificationComparisonResult"); }
        //}
        //private decimal _BWIdentificationComparisonResult = -1;

        //private void BWIdentificationComparison()
        //{
        //    if (BWData.BWLimit != -1 && BWIdentification != -1)
        //    {
        //        if (BWIdentification <= BWData.BWLimit) BWIdentificationComparisonResult = 2;
        //        if (BWIdentification > BWData.BWLimit) BWIdentificationComparisonResult = 1;
        //    }
        //    else BWIdentificationComparisonResult = 0;
        //}
        ///// <summary>
        ///// результат сравнения полосы сигнала из идентификации и из разрешения
        ///// \t 0 = не сравнивалось 1 = не совпадает 2 = совпадает
        ///// </summary>
        //public decimal BWMeasuredComparisonResult
        //{
        //    get { return _BWMeasuredComparisonResult; }
        //    set { _BWMeasuredComparisonResult = value; OnPropertyChanged("BWMeasuredComparisonResult"); }
        //}
        //private decimal _BWMeasuredComparisonResult = -1;

        //private void BWMeasuredComparison()
        //{
        //    if (BWData.BWLimit != -1 && BWData.BWMeasured != -1)
        //    {
        //        if (BWData.BWMeasured <= BWData.BWLimit) BWMeasuredComparisonResult = 2;
        //        if (BWData.BWMeasured > BWData.BWLimit) BWMeasuredComparisonResult = 1;
        //    }
        //    else BWMeasuredComparisonResult = 0;
        //}
        #endregion

        public bool NewSpecDataToSave { get; set; }
        /// <summary>
        /// спектр этого сигнала
        /// </summary>
        public Equipment.spectrum_data SpecData { get; set; }
        public Equipment.bandwidth_data BWData { get; set; }
        public Equipment.channelpower_data[] CPData { get; set; }

        /// <summary>
        /// Новые данные для сохранения Тру если поменялось:
        /// трейс, полоса сигнала, уровень, отклонение, все что угодно
        /// Сбрасывает сохранялка
        /// подумать про CDMA уже забыл про что
        /// </summary>
        public bool NewDataToSave
        {
            get { return _NewDataToSave; }
            set { _NewDataToSave = value; OnPropertyChanged("NewDataToSave"); }
        }
        private bool _NewDataToSave;

        public bool LR_NewDataToSave
        {
            get { return _LR_NewDataToSave; }
            set { _LR_NewDataToSave = value; OnPropertyChanged("LR_NewDataToSave"); }
        }
        private bool _LR_NewDataToSave = false;

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<localatdi_level_meas_result> LevelResults
        {
            get { return _LevelResults; }
            set { _LevelResults = value; OnPropertyChanged("LevelResults"); }
        }
        private ObservableCollection<localatdi_level_meas_result> _LevelResults = new ObservableCollection<localatdi_level_meas_result>() { };

        /// <summary>
        /// ссылка на станцию в идентификации что меряем
        /// </summary>
        public object IdentificationData
        {
            get { return _IdentificationData; }
            set { _IdentificationData = value; OnPropertyChanged("IdentificationData"); }
        }
        private object _IdentificationData = false;

        public string Techonology { get; set; }

        /// <summary>
        /// Частота приема БС в Гц
        /// </summary>
        public decimal FreqUP
        {
            get { return _FreqUP; }
            set { _FreqUP = value; OnPropertyChanged("FreqUP"); }
        }
        private decimal _FreqUP = 0;

        /// <summary>
        /// Частота передачи БС в Гц
        /// </summary>
        public decimal FreqDN
        {
            get { return _FreqDN; }
            set { _FreqDN = value; OnPropertyChanged("FreqDN"); }
        }
        private decimal _FreqDN = 0;


        /// <summary>
        /// Номер канала у данной технологии
        /// </summary>
        public int ChannelN { get; set; }

        public string StandartSubband { get; set; }

        /// <summary>
        /// ИЗ ЭФИРА
        /// MCC MNC LAC CID
        /// </summary>
        public string GCID
        {
            get { return _GCID; }
            set { _GCID = value; }
        }
        private string _GCID = "";

        /// <summary>
        /// BSIC SC...
        /// </summary>
        public int TechSubInd
        {
            get { return _TechSubInd; }
            set { _TechSubInd = value; OnPropertyChanged("TechSubInd"); }
        }
        private int _TechSubInd = 0;

        /// <summary>
        /// Есть ли весь идентификатор GCID BSIC SC PN
        /// </summary>
        public bool FullData { get; set; }

        /// <summary>
        /// уровень этого сигнала RSCP... с идентификатора
        /// </summary>
        public double Power
        {
            get { return _Power; }
            set { _Power = value; OnPropertyChanged("Power"); }
        }
        private double _Power = 0;

        /// <summary>
        ///это максимальный сигнал на этой частоте, т.е. его можно мерять если TRUE   пропускать если FALSE
        /// </summary>
        public bool ThisIsMaximumSignalAtThisFrequency
        {
            get { return _ThisIsMaximumSignalAtThisFrequency; }
            set { _ThisIsMaximumSignalAtThisFrequency = value; /*OnPropertyChanged("ThisIsMaximumSignalAtThisFrequency");*/ }
        }
        private bool _ThisIsMaximumSignalAtThisFrequency = false;

        public int Resets
        {
            get { return _Resets; }
            set { _Resets = value; OnPropertyChanged("Resets"); }
        }
        private int _Resets = 0;

        public int LevelUnit { get; set; }

        /// <summary>
        /// Всех трейсов что питались померить
        /// </summary>
        public int AllTraceCount
        {
            get { return _AllTraceCount; }
            set { _AllTraceCount = value; OnPropertyChanged("AllTraceCount"); }
        }
        private int _AllTraceCount = 0;

        /// <summary>
        /// Назначено трейсов для измерений
        /// </summary>
        public int AllTraceCountToMeas
        {
            get { return _AllTraceCountToMeas; }
            set { _AllTraceCountToMeas = value; OnPropertyChanged("AllTraceCountToMeas"); }
        }
        private int _AllTraceCountToMeas = 0;

        /// <summary>
        /// Напряженность поля в полосе сигнала
        /// </summary>
        public decimal ChannelStrenght
        {
            get { return _ChannelStrenght; }
            set { _ChannelStrenght = value; OnPropertyChanged("ChannelStrenght"); }
        }
        private decimal _ChannelStrenght = -1000;

        /// <summary>
        /// Последний раз видели идентификатор
        /// </summary>
        public DateTime LastSeenSignal
        {
            get { return _LastSeenSignal; }
            set { _LastSeenSignal = value; OnPropertyChanged("LastSeenSignal"); }
        }
        private DateTime _LastSeenSignal;

        public localatdi_meas_device device_ident
        {
            get { return _device_ident; }
            set { _device_ident = value; /*OnPropertyChanged("device_ident"); */}
        }
        private localatdi_meas_device _device_ident = new localatdi_meas_device() { };

        public localatdi_meas_device device_meas
        {
            get { return _device_meas; }
            set { _device_meas = value; /*OnPropertyChanged("device_meas");*/ }
        }
        private localatdi_meas_device _device_meas = new localatdi_meas_device() { };

        /// <summary>
        /// Корректктно ли проведены измерения согласно задаче
        /// </summary>
        //[PgName("meas_correctness")]
        public bool MeasCorrectness
        {
            get { return _MeasCorrectness; }
            set { _MeasCorrectness = value; OnPropertyChanged("MeasCorrectness"); }
        }
        private bool _MeasCorrectness = false;

        public DateTime LastSave
        {
            get { return _LastSave; }
            set { _LastSave = value; OnPropertyChanged("LastSave"); }
        }
        private DateTime _LastSave;

        public long LastDetectionLevelUpdete
        {
            get { return _LastDetectionLevelUpdete; }
            set { _LastDetectionLevelUpdete = value; /*OnPropertyChanged("LastDetectionLevelUpdete"); */}
        }
        private long _LastDetectionLevelUpdete = 0;

        public bool DeleteFromMeasMon
        {
            get { return _DeleteFromMeasMon; }
            set { _DeleteFromMeasMon = value; OnPropertyChanged("DeleteFromMeasMon"); }
        }
        private bool _DeleteFromMeasMon = false;












        ///// <summary>
        ///// Полоса просмотра спектра
        ///// </summary>
        //public decimal MeasSpan
        //{
        //    get { return _MeasSpan; }
        //    set { _MeasSpan = value; }
        //}
        //private decimal _MeasSpan = 0;




        //public Guid MeasGuid { get; set; }
        //public Equipment.spectrum_data Spec { get; set; }








        ///// <summary>
        ///// ПО ХАРЬКУ
        ///// MCC MNC LAC CID
        ///// </summary>
        //public string UCRFGCID
        //{
        //    get { return _UCRFGCID; }
        //    set { _UCRFGCID = value; }
        //}
        //private string _UCRFGCID = "";




        /// <summary>
        /// в каком уровне меряется все
        /// True = в dBmV False = dBm
        /// Забить и все в dBm
        /// </summary>





        ///// <summary>
        ///// типа RBW в Гц
        ///// </summary>
        //public decimal TraceStep
        //{
        //    get { return _TraceStep; }
        //    set { _TraceStep = value; OnPropertyChanged("TraceStep"); }
        //}
        //private decimal _TraceStep = 0;

        //public int TracePoints
        //{
        //    get { return _TracePoints; }
        //    set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        //}
        //private int _TracePoints = 0;

        ///// <summary>
        ///// Трейсов попавших в измерение
        ///// Хай живет вместе с базой (общее кол-во измерений)
        ///// </summary>
        //public int TraceCount
        //{
        //    get { return _TraceCount; }
        //    set { _TraceCount = value; OnPropertyChanged("TraceCount"); }
        //}
        //private int _TraceCount = 0;








        ///// <summary>
        ///// Индекс с трейса уровеня на маркере
        ///// </summary>
        //public int MarkerInd
        //{
        //    get { return _MarkerInd; }
        //    set { _MarkerInd = value; OnPropertyChanged("MarkerInd"); }
        //}
        //private int _MarkerInd = -1;

        ///// <summary>
        ///// Индекс с трейса уровеня на маркере Т1
        ///// </summary>        
        //public int MarkerT1Ind
        //{
        //    get { return _MarkerT1Ind; }
        //    set { _MarkerT1Ind = value; OnPropertyChanged("MarkerT1Ind"); }
        //}
        //private int _MarkerT1Ind = -1;

        ///// <summary>
        ///// Индекс с трейса уровеня на маркере Т2
        ///// </summary>    

        //public int MarkerT2Ind
        //{
        //    get { return _MarkerT2Ind; }
        //    set { _MarkerT2Ind = value; OnPropertyChanged("MarkerT2Ind"); }
        //}
        //private int _MarkerT2Ind = -1;







        ///// <summary>
        ///// Начальная дата измерения
        ///// живет вместе с базой
        ///// </summary>
        //public DateTime MeasStart
        //{
        //    get { return _MeasStart; }
        //    set { _MeasStart = value; OnPropertyChanged("MeasStart"); }
        //}
        //private DateTime _MeasStart;

        ///// <summary>
        ///// Последнее время измерения
        ///// </summary>
        //public DateTime MeasStop
        //{
        //    get { return _MeasStop; }
        //    set { _MeasStop = value; OnPropertyChanged("MeasStop"); }
        //}
        //private DateTime _MeasStop;



        ///// <summary>
        ///// Общее время измерения
        ///// </summary>
        //public decimal MeasDuration
        //{
        //    get { return _MeasDuration; }
        //    set { _MeasDuration = value; OnPropertyChanged("MeasDuration"); }
        //}
        //private decimal _MeasDuration;

        ///// <summary>
        ///// 0 не искали
        ///// 1 искали не нашли
        ///// 2 нашли
        ///// </summary>
        //public int Identifier_Find
        //{
        //    get { return _Identifier_Find; }
        //    set { _Identifier_Find = value; OnPropertyChanged("Identifier_Find"); }
        //}
        //private int _Identifier_Find = 0;

        ///// <summary>
        ///// если нашлась строка в плане то здесь отметится соответствие частоте
        ///// 0 не искали
        ///// 1 искали не нашли
        ///// 2 нашли
        ///// </summary>
        //public int FreqCheck_Find
        //{
        //    get { return _FreqCheck_Find; }
        //    set { _FreqCheck_Find = value; OnPropertyChanged("FreqCheck_Find"); }
        //}
        //private int _FreqCheck_Find = 0;




        ///// <summary>
        ///// измереннно полосу и отклонение центр частоты
        ///// </summary>
        //public bool Measured
        //{
        //    get { return _Measured; }
        //    set { _Measured = value; OnPropertyChanged("Measured"); }
        //}
        //private bool _Measured = false;

        ///// <summary>
        ///// Измеренная полоса сигнала 
        ///// </summary>
        //public decimal ChanelBWMeasured
        //{
        //    get { return _ChanelBWMeasured; }
        //    set
        //    {
        //        _ChanelBWMeasured = value;
        //        if (_ChanelBWMeasured > -1 && _ChanelBWMeasured <= _ChanelBWLimit)
        //        { ChanelBWConclusion = 2; }
        //        else if (_ChanelBWMeasured > -1 && _ChanelBWMeasured > _ChanelBWLimit)
        //        { ChanelBWConclusion = 1; }
        //        OnPropertyChanged("ChanelBWMeasured");
        //    }
        //}
        //private decimal _ChanelBWMeasured = 0;

        ///// <summary>
        ///// не привышать эту полосу сигнала 
        ///// </summary>
        //public decimal ChanelBWLimit
        //{
        //    get { return _ChanelBWLimit; }
        //    set { _ChanelBWLimit = value; OnPropertyChanged("ChanelBWLimit"); }
        //}
        //private decimal _ChanelBWLimit = 0;

        ///// <summary>
        ///// 0 еще не мерялось
        ///// 1 мерялось нарушение
        ///// 2 мерялось Агонь
        ///// </summary>
        //public int ChanelBWConclusion
        //{
        //    get { return _ChanelBWConclusion; }
        //    set { _ChanelBWConclusion = value; OnPropertyChanged("ChanelBWConclusion"); }
        //}
        //private int _ChanelBWConclusion = 0;



        ///// <summary>
        ///// Относительное отклонение центральной частоты
        ///// в 1*10Е-6
        ///// </summary>
        //public decimal DeltaFreqMeasured
        //{
        //    get { return _DeltaFreqMeasured; }
        //    set
        //    {
        //        _DeltaFreqMeasured = value;
        //        if (_DeltaFreqMeasured > -1 && _DeltaFreqMeasured <= _DeltaFreqLimit)
        //        { DeltaFreqConclusion = 2; }
        //        else if (_DeltaFreqMeasured > -1 && _DeltaFreqMeasured > _DeltaFreqLimit)
        //        { DeltaFreqConclusion = 1; }
        //        OnPropertyChanged("DeltaFreqMeasured");
        //    }
        //}
        //private decimal _DeltaFreqMeasured = -1;
        ///// <summary>
        ///// 0 еще не мерялось
        ///// 1 мерялось нарушение
        ///// 2 мерялось Агонь
        ///// </summary>
        //public int DeltaFreqConclusion
        //{
        //    get { return _DeltaFreqConclusion; }
        //    set { _DeltaFreqConclusion = value; OnPropertyChanged("DeltaFreqConclusion"); }
        //}
        //private int _DeltaFreqConclusion = 0;

        //public decimal RBW
        //{
        //    get { return _RBW; }
        //    set { _RBW = value; OnPropertyChanged("RBW"); }
        //}
        //private decimal _RBW = -1;

        //public decimal VBW
        //{
        //    get { return _VBW; }
        //    set { _VBW = value; OnPropertyChanged("VBW"); }
        //}
        //private decimal _VBW = -1;





        ///// <summary>
        ///// количество учтенных трейсов
        ///// </summary>
        //public int MeasTraceCount
        //{
        //    get { return _trace_count; }
        //    set { _trace_count = value; OnPropertyChanged("trace_count"); }
        //}
        //private int _trace_count = 0;
        //#endregion



        //public localatdi_meas_device device_ident
        //{
        //    get { return _device_ident; }
        //    set { _device_ident = value; /*OnPropertyChanged("device_ident"); */}
        //}
        //private localatdi_meas_device _device_ident = new localatdi_meas_device() { };

        //public localatdi_meas_device device_meas
        //{
        //    get { return _device_meas; }
        //    set { _device_meas = value; /*OnPropertyChanged("device_meas");*/ }
        //}
        //private localatdi_meas_device _device_meas = new localatdi_meas_device() { };
        //public ObservableCollection<local3GPPSystemInformationBlock> InformationBlocks
        //{
        //    get { return _InformationBlocks; }
        //    set { _InformationBlocks = value; OnPropertyChanged("InformationBlocks"); }
        //}
        //public ObservableCollection<local3GPPSystemInformationBlock> _InformationBlocks = new ObservableCollection<local3GPPSystemInformationBlock>() { };
    }
    public partial class MeasMonBand : PropertyChangedBase
    {
        public decimal Start
        {
            get { return _Start; }
            set { _Start = value; OnPropertyChanged("Start"); }
        }
        private decimal _Start = 0;
        public decimal Stop
        {
            get { return _Stop; }
            set { _Stop = value; OnPropertyChanged("Stop"); }
        }
        private decimal _Stop = 0;
        public decimal Step
        {
            get { return _Step; }
            set { _Step = value; OnPropertyChanged("Step"); }
        }
        private decimal _Step = 0;
        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        private int _TracePoints = 0;

        /// <summary>
        /// Всех трейсов что пытались померить
        /// </summary>
        public int AllTraceCount
        {
            get { return _AllTraceCount; }
            set { _AllTraceCount = value; OnPropertyChanged("AllTraceCount"); }
        }
        private int _AllTraceCount = 0;

        /// <summary>
        /// Назначено трейсов для измерений
        /// </summary>
        public int AllTraceCountToMeas
        {
            get { return _AllTraceCountToMeas; }
            set { _AllTraceCountToMeas = value; OnPropertyChanged("AllTraceCountToMeas"); }
        }
        private int _AllTraceCountToMeas = 0;

    }

    public class localatdi_freq_occupancy : PropertyChangedBase
    {
        public decimal freq
        {
            get { return _freq; }
            set { _freq = value; /*OnPropertyChanged("freq"); */}
        }
        private decimal _freq = 0;

        public int count
        {
            get { return _count; }
            set { _count = value; /*OnPropertyChanged("count");*/ }
        }
        private int _count = 0;

        public decimal occupancy
        {
            get { return _occupancy; }
            set { _occupancy = value; /*OnPropertyChanged("occupancy");*/ }
        }
        private decimal _occupancy = 0;
    }

    public class Track : PropertyChangedBase
    {
        public string name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("name"); }
        }
        private string _name = "";

        public DateTime time_start
        {
            get { return _time_start; }
            set { _time_start = value; OnPropertyChanged("time_start"); }
        }
        private DateTime _time_start;

        public DateTime time_stop
        {
            get { return _time_stop; }
            set { _time_stop = value; OnPropertyChanged("time_stop"); }
        }
        private DateTime _time_stop;

        public string table_name
        {
            get { return _table_name; }
            set { _table_name = value; OnPropertyChanged("table_name"); }
        }
        private string _table_name = "";

        public ObservableCollection<TrackData> Data
        {
            get { return _Data; }
            set { _Data = value; OnPropertyChanged("Data"); }
        }
        private ObservableCollection<TrackData> _Data = new ObservableCollection<TrackData>() { };
    }

    public class TrackData : PropertyChangedBase
    {
        public string tech
        {
            get { return _tech; }
            set { _tech = value; OnPropertyChanged("tech"); }
        }
        private string _tech = "";

        public decimal freq
        {
            get { return _freq; }
            set { _freq = value; OnPropertyChanged("freq"); }
        }
        private decimal _freq = 0;

        public string gcid
        {
            get { return _gcid; }
            set { _gcid = value; OnPropertyChanged("gcid"); }
        }
        private string _gcid = "";

        public int identifier_sub_ind
        {
            get { return _identifier_sub_ind; }
            set { _identifier_sub_ind = value; OnPropertyChanged("identifier_sub_ind"); }
        }
        private int _identifier_sub_ind = -1;

        /// <summary>
        /// LevelResults
        /// </summary>
        public ObservableCollection<DB.localatdi_level_meas_result> level_results
        {
            get { return _level_results; }
            set { _level_results = value; OnPropertyChanged("level_results"); }
        }
        private ObservableCollection<DB.localatdi_level_meas_result> _level_results = new ObservableCollection<DB.localatdi_level_meas_result>() { };

        public local3GPPSystemInformationBlock[] information_blocks
        {
            get { return _information_blocks; }
            set { _information_blocks = value; OnPropertyChanged("information_blocks"); }
        }
        private local3GPPSystemInformationBlock[] _information_blocks = new local3GPPSystemInformationBlock[] { };
    }

    #region class ATDI API_v1
    /// <summary>
    /// таблица и инфой о таске
    /// </summary>
    public class LocalAtdiTask : PropertyChangedBase
    {
        public int id
        {
            get { return _id; }
            set { _id = value; /*OnPropertyChanged("id");*/ }
        }
        private int _id = 0;

        public int meas_task_id
        {
            get { return _meas_task_id; }
            set { _meas_task_id = value; OnPropertyChanged("meas_task_id"); }
        }
        private int _meas_task_id = 0;

        public int meas_sub_task_id
        {
            get { return _meas_sub_task_id; }
            set { _meas_sub_task_id = value; OnPropertyChanged("meas_sub_task_id"); }
        }
        private int _meas_sub_task_id = 0;

        public int meas_sub_task_station_id
        {
            get { return _meas_sub_task_station_id; }
            set { _meas_sub_task_station_id = value; OnPropertyChanged("meas_sub_task_station_id"); }
        }
        private int _meas_sub_task_station_id = 0;

        public int sensor_id
        {
            get { return _sensor_id; }
            set { _sensor_id = value; /*OnPropertyChanged("sensor_id"); */}
        }
        private int _sensor_id = 0;

        public string meas_data_type
        {
            get { return _meas_data_type; }
            set { _meas_data_type = value; /*OnPropertyChanged("meas_data_type");*/ }
        }
        private string _meas_data_type = string.Empty;

        public string type_m
        {
            get { return _type_m; }
            set { _type_m = value; /*OnPropertyChanged("type_m");*/ }
        }
        private string _type_m = string.Empty;

        public int sw_number
        {
            get { return _sw_number; }
            set { _sw_number = value; /*OnPropertyChanged("sw_number");*/ }
        }
        private int _sw_number = 0;



        public DateTime time_first
        {
            get { return _time_first; }
            set { _time_first = value; OnPropertyChanged("time_first"); }
        }
        private DateTime _time_first = DateTime.MinValue;

        public DateTime time_save
        {
            get { return _time_save; }
            set { _time_save = value; OnPropertyChanged("time_save"); }
        }
        private DateTime _time_save = DateTime.MinValue;

        public DateTime time_start
        {
            get { return _time_start; }
            set { _time_start = value; OnPropertyChanged("time_start"); }
        }
        private DateTime _time_start = DateTime.MinValue;

        public DateTime time_stop
        {
            get { return _time_stop; }
            set { _time_stop = value; OnPropertyChanged("time_stop"); }
        }
        private DateTime _time_stop = DateTime.MinValue;

        public decimal per_interval
        {
            get { return _per_interval; }
            set { _per_interval = value; /*OnPropertyChanged("per_interval");*/ }
        }
        private decimal _per_interval = 0;

        public int prio
        {
            get { return _prio; }
            set { _prio = value; /*OnPropertyChanged("prio");*/ }
        }
        private int _prio = 0;

        public string status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("status"); }
        }
        private string _status = string.Empty;

        public int with_results_measured
        {
            get { return _with_results_measured; }
            set { _with_results_measured = value; OnPropertyChanged("with_results_measured"); }
        }
        private int _with_results_measured = 0;

        public int without_results_measured
        {
            get { return _without_results_measured; }
            set { _without_results_measured = value; OnPropertyChanged("without_results_measured"); }
        }
        private int _without_results_measured = 0;

        public int with_ppe_results_measured
        {
            get { return _with_ppe_results_measured; }
            set { _with_ppe_results_measured = value; OnPropertyChanged("with_ppe_results_measured"); }
        }
        private int _with_ppe_results_measured = 0;

        public int with_ndp_results_measured
        {
            get { return _with_ndp_results_measured; }
            set { _with_ndp_results_measured = value; OnPropertyChanged("with_ndp_results_measured"); }
        }
        private int _with_ndp_results_measured = 0;


        public int task_station_count
        {
            get { return _task_station_count; }
            set { _task_station_count = value; OnPropertyChanged("task_station_count"); }
        }
        private int _task_station_count = 0;

        public DateTime time_last_send_result
        {
            get { return _time_last_send_result; }
            set { _time_last_send_result = value; OnPropertyChanged("time_last_send_result"); }
        }
        private DateTime _time_last_send_result = DateTime.MinValue;

        /// <summary>
        /// 0 = ожидание, 1 = выкл, 2 = вкл
        /// </summary>
        public int TaskState
        {
            get { return _TaskState; }
            set { _TaskState = value; OnPropertyChanged("TaskState"); }
        }
        private int _TaskState = 0;

        public int new_result_to_send
        {
            get { return _new_result_to_send; }
            set { _new_result_to_send = value; OnPropertyChanged("new_result_to_send"); }
        }
        private int _new_result_to_send = 0;
        //[Magic]
        public ObservableCollection<LocalAtdiTaskWithTech> TaskDataFromTech
        {
            get { return _TaskDataFromTech; }
            set { _TaskDataFromTech = value; OnPropertyChanged("TaskDataFronTech"); }
        }
        private ObservableCollection<LocalAtdiTaskWithTech> _TaskDataFromTech = new ObservableCollection<LocalAtdiTaskWithTech>() { };
    }
    #region Tasks
    public class LocalAtdiTaskWithTech : PropertyChangedBase
    {
        public string tech
        {
            get { return _tech; }
            set { _tech = value; OnPropertyChanged("tech"); }
        }
        private string _tech = string.Empty;

        public string tasktablename
        {
            get { return _tasktablename; }
            set { _tasktablename = value; OnPropertyChanged("tasktablename"); }
        }
        private string _tasktablename = string.Empty;

        public string resulttablename
        {
            get { return _resulttablename; }
            set { _resulttablename = value; OnPropertyChanged("resulttablename"); }
        }
        private string _resulttablename = string.Empty;

        public ObservableCollection<atdi_station_data_for_measurements> TaskItems
        {
            get { return _TaskItems; }
            set { _TaskItems = value; OnPropertyChanged("TaskItems"); }
        }
        private ObservableCollection<atdi_station_data_for_measurements> _TaskItems = new ObservableCollection<atdi_station_data_for_measurements>() { };

        public ObservableCollection<LocalAtdiResultTaskItem> ResultTaskItems
        {
            get { return _ResultTaskItems; }
            set { _ResultTaskItems = value; OnPropertyChanged("ResultTaskItems"); }
        }
        private ObservableCollection<LocalAtdiResultTaskItem> _ResultTaskItems = new ObservableCollection<LocalAtdiResultTaskItem>() { };

        public int ResultTask_WithMeasurement
        {
            get { return _ResultTask_WithMeasurement; }
            set { _ResultTask_WithMeasurement = value; OnPropertyChanged("ResultTask_WithMeasurement"); }
        }
        private int _ResultTask_WithMeasurement = 0;

        public int ResultTask_PPEWithMeasurement
        {
            get { return _ResultTask_PPEWithMeasurement; }
            set { _ResultTask_PPEWithMeasurement = value; OnPropertyChanged("ResultTask_PPEWithMeasurement"); }
        }
        private int _ResultTask_PPEWithMeasurement = 0;

        public int ResultTask_NDPWithMeasurement
        {
            get { return _ResultTask_NDPWithMeasurement; }
            set { _ResultTask_NDPWithMeasurement = value; OnPropertyChanged("ResultTask_NDPWithMeasurement"); }
        }
        private int _ResultTask_NDPWithMeasurement = 0;

        public int ResultTask_ToSend
        {
            get { return _ResultTask_ToSend; }
            set { _ResultTask_ToSend = value; OnPropertyChanged("ResultTask_ToSend"); }
        }
        private int _ResultTask_ToSend = 0;

        public int SectorsCount
        {
            get { return _SectorsCount; }
            set { _SectorsCount = value; OnPropertyChanged("SectorsCount"); }
        }
        private int _SectorsCount = 0;
    }

    public class LocalAtdiResultTaskItem : PropertyChangedBase
    {
        public int permission_id
        {
            get { return _permission_id; }
            set { _permission_id = value; OnPropertyChanged("permission_id"); }
        }
        private int _permission_id = 0;
        public int id_station
        {
            get { return _id_station; }
            set { _id_station = value; OnPropertyChanged("id_station"); }
        }
        private int _id_station = 0;
        public int sector_id
        {
            get { return _sector_id; }
            set { _sector_id = value; OnPropertyChanged("sector_id"); }
        }
        private int _sector_id = 0;
        public int frequency_id
        {
            get { return _frequency_id; }
            set { _frequency_id = value; OnPropertyChanged("frequency_id"); }
        }
        private int _frequency_id = 0;
        public int meas_task_id
        {
            get { return _meas_task_id; }
            set { _meas_task_id = value; OnPropertyChanged("meas_task_id"); }
        }
        private int _meas_task_id = 0;
        public int meas_sub_task_id
        {
            get { return _meas_sub_task_id; }
            set { _meas_sub_task_id = value; OnPropertyChanged("meas_sub_task_id"); }
        }
        private int _meas_sub_task_id = 0;
        public int meas_sub_task_station_id
        {
            get { return _meas_sub_task_station_id; }
            set { _meas_sub_task_station_id = value; OnPropertyChanged("meas_sub_task_station_id"); }
        }
        private int _meas_sub_task_station_id = 0;
        public int sensor_id
        {
            get { return _sensor_id; }
            set { _sensor_id = value; /*OnPropertyChanged("sensor_id");*/ }
        }
        private int _sensor_id = 0;
        public int nn
        {
            get { return _nn; }
            set { _nn = value; /*OnPropertyChanged("nn");*/ }
        }
        private int _nn = 0;
        public int sw_number
        {
            get { return _sw_number; }
            set { _sw_number = value; /*OnPropertyChanged("sw_number");*/ }
        }
        private int _sw_number = 0;
        public decimal measurment_last_latitude
        {
            get { return _measurment_last_latitude; }
            set { _measurment_last_latitude = value; OnPropertyChanged("measurment_last_latitude"); }
        }
        private decimal _measurment_last_latitude = 0;
        public decimal measurment_last_longitude
        {
            get { return _measurment_last_longitude; }
            set { _measurment_last_longitude = value; OnPropertyChanged("measurment_last_longitude"); }
        }
        private decimal _measurment_last_longitude = 0;

        public decimal measurment_altitude
        {
            get { return _measurment_altitude; }
            set { _measurment_altitude = value; OnPropertyChanged("measurment_altitude"); }
        }
        private decimal _measurment_altitude = 0;

        public decimal measurment_strength
        {
            get { return _measurment_strength; }
            set { _measurment_strength = value; OnPropertyChanged("measurment_strength"); }
        }
        private decimal _measurment_strength = 0;

        public decimal measurment_duration
        {
            get { return _measurment_duration; }
            set { _measurment_duration = value; OnPropertyChanged("measurment_duration"); }
        }
        private decimal _measurment_duration = 0;

        public DateTime measurment_datetime
        {
            get { return _measurment_datetime; }
            set { _measurment_datetime = value; OnPropertyChanged("measurment_datetime"); }
        }
        private DateTime _measurment_datetime = DateTime.MinValue;
        public DateTime measurment_datetime_start
        {
            get { return _measurment_datetime_start; }
            set { _measurment_datetime_start = value; OnPropertyChanged("measurment_datetime_start"); }
        }
        private DateTime _measurment_datetime_start = DateTime.MinValue;
        public decimal measurment_freq_centr_permision
        {
            get { return _measurment_freq_centr_permision; }
            set { _measurment_freq_centr_permision = value; OnPropertyChanged("measurment_freq_centr_permision"); }
        }
        private decimal _measurment_freq_centr_permision = 0;
        public ObservableCollection<atdi_mask_element> measurment_sector_mask
        {
            get { return _measurment_sector_mask; }
            set { _measurment_sector_mask = value; OnPropertyChanged("measurment_sector_mask"); }
        }
        private ObservableCollection<atdi_mask_element> _measurment_sector_mask = new ObservableCollection<atdi_mask_element>() { };
        public decimal sources_freq_centr
        {
            get { return _sources_freq_centr; }
            set { _sources_freq_centr = value; OnPropertyChanged("sources_freq_centr"); }
        }
        private decimal _sources_freq_centr = 0;
        public decimal sources_freq_start
        {
            get { return _sources_freq_start; }
            set { _sources_freq_start = value; OnPropertyChanged("sources_freq_start"); }
        }
        private decimal _sources_freq_start = 0;
        public decimal sources_freq_stop
        {
            get { return _sources_freq_stop; }
            set { _sources_freq_stop = value; OnPropertyChanged("sources_freq_stop"); }
        }
        private decimal _sources_freq_stop = 0;
        public decimal sources_freq_step
        {
            get { return _sources_freq_step; }
            set { _sources_freq_step = value; OnPropertyChanged("sources_freq_step"); }
        }
        private decimal _sources_freq_step = 0;
        public decimal[] sources_trace
        {
            get { return _sources_trace; }
            set { _sources_trace = value; /*OnPropertyChanged("sources_trace");*/ }
        }
        private decimal[] _sources_trace = new decimal[] { };

        /// <summary>
        /// True = dBµV           False = dBm
        /// </summary>
        public bool sources_trace_level_type
        {
            get { return _sources_trace_level_type; }
            set { _sources_trace_level_type = value; /*OnPropertyChanged("sources_trace_level_type"); */}
        }
        private bool _sources_trace_level_type = false;
        public int sources_m1_marker_index
        {
            get { return _sources_m1_marker_index; }
            set { _sources_m1_marker_index = value; OnPropertyChanged("sources_m1_marker_index"); }
        }
        private int _sources_m1_marker_index = 0;
        public int sources_t1_marker_index
        {
            get { return _sources_t1_marker_index; }
            set { _sources_t1_marker_index = value; OnPropertyChanged("sources_t1_marker_index"); }
        }
        private int _sources_t1_marker_index = 0;
        public int sources_t2_marker_index
        {
            get { return _sources_t2_marker_index; }
            set { _sources_t2_marker_index = value; OnPropertyChanged("sources_t2_marker_index"); }
        }
        private int _sources_t2_marker_index = 0;
        public string station_identifier_from_radio
        {
            get { return _station_identifier_from_radio; }
            set { _station_identifier_from_radio = value; OnPropertyChanged("station_identifier_from_radio"); }
        }
        private string _station_identifier_from_radio = string.Empty;

        #region station_identifier_from_radio
        public int station_identifier_from_radio_MCC
        {
            get { return _station_identifier_from_radio_MCC; }
            set { _station_identifier_from_radio_MCC = value; }
        }
        private int _station_identifier_from_radio_MCC = 0;
        public int station_identifier_from_radio_MNC
        {
            get { return _station_identifier_from_radio_MNC; }
            set { _station_identifier_from_radio_MNC = value; }
        }
        private int _station_identifier_from_radio_MNC = 0;
        public int station_identifier_from_radio_LAC
        {
            get { return _station_identifier_from_radio_LAC; }
            set { _station_identifier_from_radio_LAC = value; }
        }
        private int _station_identifier_from_radio_LAC = 0;
        public int station_identifier_from_radio_CID
        {
            get { return _station_identifier_from_radio_CID; }
            set { _station_identifier_from_radio_CID = value; }
        }
        private int _station_identifier_from_radio_CID = 0;
        #endregion
        public int station_identifier_from_radio_tech_sub_ind
        {
            get { return _station_identifier_from_radio_tech_sub_ind; }
            set { _station_identifier_from_radio_tech_sub_ind = value; OnPropertyChanged("station_identifier_from_radio_tech_sub_ind"); }
        }
        private int _station_identifier_from_radio_tech_sub_ind = 0;
        public string station_identifier_atdi
        {
            get { return _station_identifier_atdi; }
            set { _station_identifier_atdi = value; OnPropertyChanged("station_identifier_atdi"); }
        }
        private string _station_identifier_atdi = string.Empty;

        #region station_identifier_atdi
        public int station_identifier_atdi_MCC
        {
            get { return _station_identifier_atdi_MCC; }
            set { _station_identifier_atdi_MCC = value; }
        }
        private int _station_identifier_atdi_MCC = 0;
        public int station_identifier_atdi_MNC
        {
            get { return _station_identifier_atdi_MNC; }
            set { _station_identifier_atdi_MNC = value; }
        }
        private int _station_identifier_atdi_MNC = 0;
        public int station_identifier_atdi_LAC
        {
            get { return _station_identifier_atdi_LAC; }
            set { _station_identifier_atdi_LAC = value; }
        }
        private int _station_identifier_atdi_LAC = 0;
        public int station_identifier_atdi_CID
        {
            get { return _station_identifier_atdi_CID; }
            set { _station_identifier_atdi_CID = value; }
        }
        private int _station_identifier_atdi_CID = 0;
        #endregion

        /// <summary>
        /// Status = “E” в случае НДП, “A” – если это не НДП, “I” – порушенная правил эксплуатации
        /// </summary>
        public string station_status
        {
            get { return _station_status; }
            set { _station_status = value; OnPropertyChanged("station_status"); }
        }
        private string _station_status = string.Empty;
        public int user_id
        {
            get { return _user_id; }
            set { _user_id = value; /*OnPropertyChanged("user_id");*/ }
        }
        private int _user_id = 0;
        public string user_name
        {
            get { return _user_name; }
            set { _user_name = value; OnPropertyChanged("user_name"); }
        }
        private string _user_name = string.Empty;

        /// <summary>
        /// есть новые результаты нужно отправить
        /// </summary>
        public bool new_meas_data_to_send
        {
            get { return _new_meas_data_to_send; }
            set { _new_meas_data_to_send = value; OnPropertyChanged("new_meas_data_to_send"); }
        }
        private bool _new_meas_data_to_send = false;

        /// <summary>
        /// отправлено ли в ICSC
        /// </summary>
        public bool level_measurements_car_sended
        {
            get { return _level_measurements_car_sended; }
            set { _level_measurements_car_sended = value; OnPropertyChanged("level_measurements_car_sended"); }
        }
        private bool _level_measurements_car_sended = false;
        public ObservableCollection<LacalAtdi_LevelMeasurementsCar> level_measurements_car
        {
            get { return _level_measurements_car; }
            set { _level_measurements_car = value; OnPropertyChanged("level_measurements_car"); }
        }
        private ObservableCollection<LacalAtdi_LevelMeasurementsCar> _level_measurements_car = new ObservableCollection<LacalAtdi_LevelMeasurementsCar>() { };
        public ObservableCollection<local3GPPSystemInformationBlock> information_blocks
        {
            get { return _InformationBlocks; }
            set { _InformationBlocks = value; OnPropertyChanged("InformationBlocks"); }
        }
        public ObservableCollection<local3GPPSystemInformationBlock> _InformationBlocks = new ObservableCollection<local3GPPSystemInformationBlock>() { };


    }

    public class LacalAtdi_LevelMeasurementsCar : PropertyChangedBase
    {
        /// <summary>
        /// добавлена ли эта строка в результаты
        /// </summary>        
        public bool saved_in_result
        {
            get { return _saved_in_measurement; }
            set { _saved_in_measurement = value;/* OnPropertyChanged("saved_in_measurement"); */}
        }
        private bool _saved_in_measurement = false;

        [PgName("saved_in_db")]
        public bool saved_in_db
        {
            get { return _saved_in_db; }
            set { _saved_in_db = value; /*OnPropertyChanged("saved_in_db"); */}
        }
        private bool _saved_in_db = false;

        [PgName("altitude")]
        public decimal altitude
        {
            get { return _altitude; }
            set { _altitude = value; /*OnPropertyChanged("altitude");*/ }
        }
        private decimal _altitude = decimal.MaxValue;

        /// <summary>
        /// пока нулл
        /// </summary>
        [PgName("bw")]
        public decimal bw
        {
            get { return _bw; }
            set { _bw = value; /*OnPropertyChanged("bw");*/ }
        }
        private decimal _bw = decimal.MaxValue;

        /// <summary>
        /// В МГц
        /// </summary>
        [PgName("central_frequency")]
        public decimal central_frequency
        {
            get { return _central_frequency; }
            set { _central_frequency = value; /*OnPropertyChanged("central_frequency");*/ }
        }
        private decimal _central_frequency = decimal.MaxValue;

        /// <summary>
        /// время прохождения сигнала в эфире от БС к Приемнику в нс
        /// </summary>
        [PgName("difference_timestamp")]
        public decimal difference_timestamp
        {
            get { return _difference_timestamp; }
            set { _difference_timestamp = value; /*OnPropertyChanged("difference_timestamp");*/ }
        }
        private decimal _difference_timestamp = 0;

        [PgName("latitude")]
        public decimal latitude
        {
            get { return _latitude; }
            set { _latitude = value; /*OnPropertyChanged("latitude");*/ }
        }
        private decimal _latitude = decimal.MaxValue;

        [PgName("longitude")]
        public decimal longitude
        {
            get { return _longitude; }
            set { _longitude = value; /*OnPropertyChanged("longitude"); */}
        }
        private decimal _longitude = decimal.MaxValue;

        [PgName("level_dbm")]
        public decimal level_dbm
        {
            get { return _level_dbm; }
            set { _level_dbm = value; /*OnPropertyChanged("level_dbm"); */}
        }
        private decimal _level_dbm = decimal.MaxValue;

        [PgName("level_dbmkvm")]
        public decimal level_dbmkvm
        {
            get { return _level_dbmkvm; }
            set { _level_dbmkvm = value; /*OnPropertyChanged("level_dbmkvm");*/ }
        }
        private decimal _level_dbmkvm = decimal.MaxValue;

        [PgName("rbw")]
        public decimal rbw
        {
            get { return _rbw; }
            set { _rbw = value; /*OnPropertyChanged("rbw"); */}
        }
        private decimal _rbw = decimal.MaxValue;

        [PgName("vbw")]
        public decimal vbw
        {
            get { return _vbw; }
            set { _vbw = value; /*OnPropertyChanged("vbw");*/ }
        }
        private decimal _vbw = decimal.MaxValue;

        /// <summary>
        /// время измерения
        /// </summary>
        [PgName("time_of_measurements")]
        public DateTime time_of_measurements
        {
            get { return _time_of_measurements; }
            set { _time_of_measurements = value; /*OnPropertyChanged("time_of_measurements"); */}
        }
        private DateTime _time_of_measurements = DateTime.MinValue;
    }
    #endregion
    public class atdi_station_data_for_measurements : PropertyChangedBase
    {
        public int station_id
        {
            get { return _station_id; }
            set { _station_id = value; OnPropertyChanged("station_id"); }
        }
        private int _station_id = 0;

        public string station_callsign
        {
            get { return _station_callsign; }
            set { _station_callsign = value; OnPropertyChanged("station_callsign"); }
        }
        private string _station_callsign = "";

        #region
        /// <summary>
        /// MCC
        /// </summary>
        public int Callsign_S0
        {
            get { return _Callsign_S0; }
            set { _Callsign_S0 = value; }
        }
        private int _Callsign_S0 = 0;
        /// <summary>
        /// MNC
        /// </summary>
        public int Callsign_S1
        {
            get { return _Callsign_S1; }
            set { _Callsign_S1 = value; }
        }
        private int _Callsign_S1 = 0;
        /// <summary>
        /// LAC / eNodeBid
        /// </summary>
        public int Callsign_S2
        {
            get { return _Callsign_S2; }
            set { _Callsign_S2 = value; }
        }
        private int _Callsign_S2 = 0;
        /// <summary>
        /// CID / PN
        /// </summary>
        public int Callsign_S3
        {
            get { return _Callsign_S3; }
            set { _Callsign_S3 = value; }
        }
        private int _Callsign_S3 = 0;
        #endregion

        public string standart
        {
            get { return _standart; }
            set { _standart = value; OnPropertyChanged("standart"); }
        }
        private string _standart = "";

        public string status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("status"); }
        }
        private string _status = "";

        #region PermissionForAssignment
        public int permission_id
        {
            get { return _permission_id; }
            set { _permission_id = value; OnPropertyChanged("permission_id"); }
        }
        private int _permission_id = 0;

        public string permission_name
        {
            get { return _permission_name; }
            set { _permission_name = value; OnPropertyChanged("permission_name"); }
        }
        private string _permission_name = "";

        public DateTime permission_start_date
        {
            get { return _permission_start_date; }
            set { _permission_start_date = value; OnPropertyChanged("permission_start_date"); }
        }
        private DateTime _permission_start_date = DateTime.MinValue;

        public DateTime permission_end_date
        {
            get { return _permission_end_date; }
            set { _permission_end_date = value; OnPropertyChanged("permission_end_date"); }
        }
        private DateTime _permission_end_date = DateTime.MinValue;

        public DateTime permission_close_date
        {
            get { return _permission_close_date; }
            set { _permission_close_date = value; OnPropertyChanged("permission_close_date"); }
        }
        private DateTime _permission_close_date = DateTime.MinValue;
        #endregion

        #region OwnerData
        public int owner_id
        {
            get { return _owner_id; }
            set { _owner_id = value; OnPropertyChanged("owner_id"); }
        }
        private int _owner_id = 0;

        public string owner_name
        {
            get { return _owner_name; }
            set { _owner_name = value; OnPropertyChanged("owner_name"); }
        }
        private string _owner_name = "";

        public string owner_code
        {
            get { return _owner_code; }
            set { _owner_code = value; OnPropertyChanged("owner_code"); }
        }
        private string _owner_code = "";

        public string owner_okpo
        {
            get { return _owner_okpo; }
            set { _owner_okpo = value; OnPropertyChanged("owner_okpo"); }
        }
        private string _owner_okpo = "";

        public string owner_zip
        {
            get { return _owner_zip; }
            set { _owner_zip = value; OnPropertyChanged("owner_zip"); }
        }
        private string _owner_zip = "";

        public string owner_address
        {
            get { return _owner_address; }
            set { _owner_address = value; OnPropertyChanged("owner_address"); }
        }
        private string _owner_address = "";
        #endregion

        #region SiteStationForMeas
        public string site_address
        {
            get { return _site_address; }
            set { _site_address = value; OnPropertyChanged("site_address"); }
        }
        private string _site_address = "";

        public string site_region
        {
            get { return _site_region; }
            set { _site_region = value; OnPropertyChanged("site_region"); }
        }
        private string _site_region = "";

        public decimal site_lat
        {
            get { return _site_lat; }
            set { _site_lat = value; OnPropertyChanged("site_lat"); }
        }
        private decimal _site_lat = 0;

        public decimal site_lon
        {
            get { return _site_lon; }
            set { _site_lon = value; OnPropertyChanged("site_lon"); }
        }
        private decimal _site_lon = 0;
        #endregion

        public ObservableCollection<atdi_sector_station> sectors
        {
            get { return _sectors; }
            set { _sectors = value; OnPropertyChanged("sectors"); }
        }
        private ObservableCollection<atdi_sector_station> _sectors = new ObservableCollection<atdi_sector_station>() { };

        public bool meas_data_exist
        {
            get { return _meas_data_exist; }
            set { _meas_data_exist = value; OnPropertyChanged("meas_data_exist"); }
        }
        private bool _meas_data_exist = false;

        public bool IsIdentified
        {
            get { return _IsIdentified; }
            set { _IsIdentified = value; OnPropertyChanged("IsIdentified"); }
        }
        private bool _IsIdentified = false;

    }
    public class atdi_sector_station : PropertyChangedBase
    {
        //[Magic]//
        //public int sector_id { get; set; }
        //[Magic]//
        //public decimal agl { get; set; }
        //[Magic]//
        //public decimal azimuth { get; set; }
        //[Magic]//
        //public decimal bw { get; set; }
        //[Magic]//
        //public string class_emission { get; set; }
        //[Magic]//
        //public decimal eirp { get; set; }
        //[Magic]//
        //public atdi_frequency_for_sector[] sector_frequencies { get; set; }
        //[Magic]//
        //public atdi_mask_element[] sector_mask { get; set; }
        public int sector_id
        {
            get { return _sector_id; }
            set { _sector_id = value; OnPropertyChanged("sector_id"); }
        }
        private int _sector_id = 0;

        public decimal agl
        {
            get { return _agl; }
            set { _agl = value; OnPropertyChanged("agl"); }
        }
        private decimal _agl = 0;

        public decimal azimuth
        {
            get { return _azimuth; }
            set { _azimuth = value; OnPropertyChanged("azimuth"); }
        }
        private decimal _azimuth = 0;

        public decimal bw
        {
            get { return _bw; }
            set { _bw = value; OnPropertyChanged("bw"); }
        }
        private decimal _bw = 0;

        public string class_emission
        {
            get { return _class_emission; }
            set { _class_emission = value; OnPropertyChanged("class_emission"); }
        }
        private string _class_emission = "";

        public decimal eirp
        {
            get { return _eirp; }
            set { _eirp = value; OnPropertyChanged("eirp"); }
        }
        private decimal _eirp = 0;

        public atdi_frequency_for_sector[] sector_frequencies
        {
            get { return _sector_frequencies; }
            set { _sector_frequencies = value; OnPropertyChanged("sector_frequencies"); }
        }
        private atdi_frequency_for_sector[] _sector_frequencies = new atdi_frequency_for_sector[] { };

        public atdi_mask_element[] sector_mask
        {
            get { return _sector_mask; }
            set { _sector_mask = value; OnPropertyChanged("sector_mask"); }
        }
        private atdi_mask_element[] _sector_mask = new atdi_mask_element[] { };
    }

    public class atdi_frequency_for_sector : PropertyChangedBase
    {
        public int id { get; set; }

        public int id_plan { get; set; }

        public int channal_number { get; set; }

        public decimal frequency { get; set; }
        //public int id
        //{
        //    get { return _id; }
        //    set { _id = value; OnPropertyChanged("Id"); }
        //}
        //private int _id = 0;

        //public int id_plan
        //{
        //    get { return _id_plan; }
        //    set { _id_plan = value; OnPropertyChanged("id_plan"); }
        //}
        //private int _id_plan = 0;

        //public int channal_number
        //{
        //    get { return _channal_number; }
        //    set { _channal_number = value; OnPropertyChanged("channal_number"); }
        //}
        //private int _channal_number = 0;
        ///// <summary>
        ///// МГц
        ///// </summary>
        //public decimal frequency
        //{
        //    get { return _frequency; }
        //    set { _frequency = value; OnPropertyChanged("frequency"); }
        //}
        //private decimal _frequency = 0;        
    }
    public class atdi_mask_element : PropertyChangedBase
    {
        //public bool saved
        //{
        //    get { return _saved; }
        //    set { _saved = value; /*OnPropertyChanged("saved");*/ }
        //}
        //private bool _saved = false;
        /// <summary>
        /// кГц оно же BW
        /// </summary>
        //[PgName("frequency")]
        public decimal frequency
        {
            get { return _frequency; }
            set { _frequency = value; /*OnPropertyChanged("frequency");*/ }
        }
        private decimal _frequency = 0;
        /// <summary>
        /// кГц оно же BW
        /// </summary>
        //[PgName("level")]
        public decimal level
        {
            get { return _level; }
            set { _level = value; /*OnPropertyChanged("level");*/ }
        }
        private decimal _level = -1000;
    }
    public class local3GPPSystemInformationBlock : PropertyChangedBase
    {
        public string type { get; set; }
        //{
        //    get { return _type; }
        //    set { _type = value; /*OnPropertyChanged("type"); */}
        //}
        //private string _type = "";

        public string datastring { get; set; }
        //{
        //    get { return _datastring; }
        //    set { _datastring = value; /*OnPropertyChanged("datastring");*/ }
        //}
        //private string _datastring = "";

        public DateTime saved { get; set; }
        //{
        //    get { return _saved; }
        //    set { _saved = value; /*OnPropertyChanged("saved");*/ }
        //}
        //private DateTime _saved = DateTime.MinValue;
    }

    //public class local_ATDIConnection
    //{
    //    [PgName("selected")]
    //    public bool selected
    //    {
    //        get { return _selected; }
    //        set { _selected = value; }
    //    }
    //    private bool _selected = false;

    //    [PgName("host")]
    //    public string host
    //    {
    //        get { return _host; }
    //        set { _host = value; }
    //    }
    //    private string _host = "000.000.000.000";

    //    [PgName("user_name")]
    //    public string username
    //    {
    //        get { return _username; }
    //        set { _username = value; }
    //    }
    //    private string _username = "";

    //    [PgName("password")]
    //    public string Password
    //    {
    //        get { return _Password; }
    //        set { _Password = value; }
    //    }
    //    private string _Password = "";

    //    [PgName("sensor_queue")]
    //    public string SensorQueue
    //    {
    //        get { return _SensorQueue; }
    //        set { _SensorQueue = value; }
    //    }
    //    private string _SensorQueue = "SENSORS_List";

    //    [PgName("sensor_confirm_queue")]
    //    public string SensorConfirmQueue
    //    {
    //        get { return _SensorConfirmQueue; }
    //        set { _SensorConfirmQueue = value; }
    //    }
    //    private string _SensorConfirmQueue = "Event_Confirm_SENSORS_Send_";

    //    [PgName("task_queue")]
    //    public string TaskQueue
    //    {
    //        get { return _TaskQueue; }
    //        set { _TaskQueue = value; }
    //    }
    //    private string _TaskQueue = "MEAS_TASK_Main_List_APPServer_";

    //    [PgName("result_queue")]
    //    public string ResultQueue
    //    {
    //        get { return _ResultQueue; }
    //        set { _ResultQueue = value; }
    //    }
    //    private string _ResultQueue = "MEAS_SDR_RESULTS_Main_List_APPServer_";

    //    [PgName("sensor_name")]
    //    public string SensorName
    //    {
    //        get { return _SensorName; }
    //        set { _SensorName = value; }
    //    }
    //    private string _SensorName = "";

    //    [PgName("sensor_equipment_tech_id")]
    //    public string SensorEquipmentTechId
    //    {
    //        get { return _SensorEquipmentTechId; }
    //        set { _SensorEquipmentTechId = value; }
    //    }
    //    private string _SensorEquipmentTechId = "";
    //}
    #endregion

    public class WRLSMacBinding : INotifyPropertyChanged
    {
        #region Global
        public string ap_mac
        {
            get { return _ap_mac; }
            set { _ap_mac = value; OnPropertyChanged("ap_mac"); }
        }
        private string _ap_mac = "";

        public string sta_mac
        {
            get { return _sta_mac; }
            set { _sta_mac = value; OnPropertyChanged("sta_mac"); }
        }
        private string _sta_mac = "";

        public string main_secondary
        {
            get { return _main_secondary; }
            set { _main_secondary = value; OnPropertyChanged("main_secondary"); }
        }
        private string _main_secondary = "";

        public decimal binding_latitude
        {
            get { return _binding_latitude; }
            set { _binding_latitude = value; OnPropertyChanged("binding_latitude"); }
        }
        private decimal _binding_latitude = 0;

        public decimal binding_longitude
        {
            get { return _binding_longitude; }
            set { _binding_longitude = value; OnPropertyChanged("binding_longitude"); }
        }
        private decimal _binding_longitude = 0;
        #endregion Global

        #region RS135
        public int rs135_stn_frq_id
        {
            get { return _rs135_stn_frq_id; }
            set { _rs135_stn_frq_id = value; OnPropertyChanged("rs135_stn_frq_id"); }
        }
        private int _rs135_stn_frq_id = 0;

        public decimal rs135_freq
        {
            get { return _rs135_freq; }
            set { _rs135_freq = value; OnPropertyChanged("rs135_freq"); }
        }
        private decimal _rs135_freq = 0;

        public string rs135_permissionnumber
        {
            get { return _rs135_permissionnumber; }
            set { _rs135_permissionnumber = value; OnPropertyChanged("rs135_permissionnumber"); }
        }
        private string _rs135_permissionnumber = "";

        #endregion RS135

        #region ATDI
        public int atdi_station_id
        {
            get { return _atdi_station_id; }
            set { _atdi_station_id = value; OnPropertyChanged("atdi_station_id"); }
        }
        private int _atdi_station_id = 0;

        public string atdi_station_callsign
        {
            get { return _atdi_station_callsign; }
            set { _atdi_station_callsign = value; OnPropertyChanged("atdi_station_callsign"); }
        }
        private string _atdi_station_callsign = "";

        public int atdi_permission_id
        {
            get { return _atdi_permission_id; }
            set { _atdi_permission_id = value; OnPropertyChanged("atdi_permission_id"); }
        }
        private int _atdi_permission_id = 0;

        public string atdi_permissionnumber
        {
            get { return _atdi_permissionnumber; }
            set { _atdi_permissionnumber = value; OnPropertyChanged("atdi_permissionnumber"); }
        }
        private string _atdi_permissionnumber = "";

        public string atdi_owner_name
        {
            get { return _atdi_owner_name; }
            set { _atdi_owner_name = value; OnPropertyChanged("atdi_owner_name"); }
        }
        private string _atdi_owner_name = "";

        public int atdi_sector_id
        {
            get { return _atdi_sector_id; }
            set { _atdi_sector_id = value; OnPropertyChanged("atdi_sector_id"); }
        }
        private int _atdi_sector_id = 0;

        public decimal atdi_sector_azimuth
        {
            get { return _atdi_sector_azimuth; }
            set { _atdi_sector_azimuth = value; OnPropertyChanged("atdi_sector_azimuth"); }
        }
        private decimal _atdi_sector_azimuth = 0;

        public int atdi_freq_id
        {
            get { return _atdi_freq_id; }
            set { _atdi_freq_id = value; OnPropertyChanged("atdi_freq_id"); }
        }
        private int _atdi_freq_id = 0;

        public decimal atdi_freq
        {
            get { return _atdi_freq; }
            set { _atdi_freq = value; OnPropertyChanged("atdi_freq"); }
        }
        private decimal _atdi_freq = 0;


        #endregion ATDI
        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
