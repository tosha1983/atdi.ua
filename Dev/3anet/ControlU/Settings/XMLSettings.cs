using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.IO.Compression;

namespace ControlU.Settings
{
    [Serializable]
    public class XMLSettings : INotifyPropertyChanged
    {
        string AppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private Global_Set GSet = new Global_Set();

        private GNSS_Set _GNSS_Settings;
        public GNSS_Set GNSS_Settings
        {
            get { return _GNSS_Settings; }
            set { _GNSS_Settings = value; OnPropertyChanged("GNSS_Settings"); }
        }
        private Analyzer_Set _Analyzer_Settings;
        public Analyzer_Set Analyzer_Settings
        {
            get { return _Analyzer_Settings; }
            set { _Analyzer_Settings = value; OnPropertyChanged("Analyzer_Settings"); }
        }
        private RsReceiver_Set _RsReceiver_Settings;
        public RsReceiver_Set RsReceiver_Settings
        {
            get { return _RsReceiver_Settings; }
            set { _RsReceiver_Settings = value; OnPropertyChanged("RsReceiver_Settings"); }
        }
        private TSMxReceiver_Set _TSMxReceiver_Settings;
        public TSMxReceiver_Set TSMxReceiver_Settings
        {
            get { return _TSMxReceiver_Settings; }
            set { _TSMxReceiver_Settings = value; OnPropertyChanged("TSMxReceiver_Settings"); }
        }
        private Screen_Set _Screen_Settings;
        public Screen_Set Screen_Settings
        {
            get { return _Screen_Settings; }
            set { _Screen_Settings = value; OnPropertyChanged("Screen_Settings"); }
        }
        private GlogalApps_Set _GlogalApps_Settings;
        public GlogalApps_Set GlogalApps_Settings
        {
            get { return _GlogalApps_Settings; }
            set { _GlogalApps_Settings = value; OnPropertyChanged("GlogalApps_Settings"); }
        }
        private DB_Set _DB_Settings;
        public DB_Set DB_Settings
        {
            get { return _DB_Settings; }
            set { _DB_Settings = value; OnPropertyChanged("DB_Settings"); }
        }
        private UsersApps_Set _UsersApps_Settings;
        public UsersApps_Set UsersApps_Settings
        {
            get { return _UsersApps_Settings; }
            set { _UsersApps_Settings = value; OnPropertyChanged("UsersApps_Settings"); }
        }
        private Map_Set _Map_Settings;
        public Map_Set Map_Settings
        {
            get { return _Map_Settings; }
            set { _Map_Settings = value; OnPropertyChanged("Map_Settings"); }
        }
        private AnUserPresets_Set _AnUserPresets_Settings;
        public AnUserPresets_Set AnUserPresets_Settings
        {
            get { return _AnUserPresets_Settings; }
            set { _AnUserPresets_Settings = value; OnPropertyChanged("AnUserPresets_Settings"); }
        }
        private Template_Set _Template_Settings;
        public Template_Set Template_Settings
        {
            get { return _Template_Settings; }
            set { _Template_Settings = value; OnPropertyChanged("Template_Settings"); }
        }


        private MeasMons_Set _MeasMons_Settings;
        public MeasMons_Set MeasMons_Settings
        {
            get { return _MeasMons_Settings; }
            set { _MeasMons_Settings = value; OnPropertyChanged("MeasMons_Settings"); }
        }
        private Antennas_Set _Antennas_Settings;
        public Antennas_Set Antennas_Settings
        {
            get { return _Antennas_Settings; }
            set { _Antennas_Settings = value; OnPropertyChanged("Antennas_Settings"); }
        }
        private Antena_Set _SelectedAntena = new Antena_Set();
        public Antena_Set SelectedAntena
        {
            get { return _SelectedAntena; }
            set { _SelectedAntena = value; OnPropertyChanged("SelectedAntena"); }
        }
        private Equipments_Set _Equipments_Settings;
        public Equipments_Set Equipments_Settings
        {
            get { return _Equipments_Settings; }
            set { _Equipments_Settings = value; OnPropertyChanged("Equipments_Settings"); }
        }


        public ATDIConnection_Set ATDIConnection_Settings
        {
            get { return _ATDIConnection_Settings; }
            set { _ATDIConnection_Settings = value; OnPropertyChanged("ATDIConnection_Settings"); }
        }
        private ATDIConnection_Set _ATDIConnection_Settings;

        public XMLSettings()
        {
            GNSS_Settings = new GNSS_Set();
            Analyzer_Settings = new Analyzer_Set();
            RsReceiver_Settings = new RsReceiver_Set();
            TSMxReceiver_Settings = new TSMxReceiver_Set();
            Screen_Settings = new Screen_Set();
            GlogalApps_Settings = new GlogalApps_Set();
            DB_Settings = new DB_Set();
            UsersApps_Settings = new UsersApps_Set();

            Map_Settings = new Map_Set();
            AnUserPresets_Settings = new AnUserPresets_Set();
            Template_Settings = new Template_Set();

            MeasMons_Settings = new MeasMons_Set();
            Antennas_Settings = new Antennas_Set();
            Equipments_Settings = new Equipments_Set();
            ATDIConnection_Settings = new ATDIConnection_Set();
            ReadAll();
        }
        public void SetFromAnotherSett(XMLSettings sett)
        {
            this.GNSS_Settings = sett.GNSS_Settings;
            this.Analyzer_Settings = sett.Analyzer_Settings;
            this.RsReceiver_Settings = sett.RsReceiver_Settings;
            this.TSMxReceiver_Settings = sett.TSMxReceiver_Settings;
            this.Screen_Settings = sett.Screen_Settings;
            this.GlogalApps_Settings = sett.GlogalApps_Settings;
            this.DB_Settings = sett.DB_Settings;
            this.UsersApps_Settings = sett.UsersApps_Settings;
            this.Map_Settings = sett.Map_Settings;
            this.AnUserPresets_Settings = sett.AnUserPresets_Settings;
            this.Template_Settings = sett.Template_Settings;
            this.MeasMons_Settings = sett.MeasMons_Settings;
            this.Antennas_Settings = sett.Antennas_Settings;
            this.Equipments_Settings = sett.Equipments_Settings;
            this.ATDIConnection_Settings = sett.ATDIConnection_Settings;
        }





        public void ReadAll()
        {
            _ReadAll();
        }
        public void SaveAll()
        {
            _SaveAll();
        }
        public void SaveGNSS()
        {
            _SaveGNSS();
        }
        public void SaveAnalyzer()
        {
            _SaveAnalyzer();
        }
        public void SaveRsReceiver()
        {
            _SaveRsReceiver();
        }
        public void SaveTSMxReceiver()
        {
            _SaveTSMxReceiver();
        }
        public void SaveScreen()
        {
            _SaveScreen();
        }
        public void SaveGlogalApps()
        {
            _SaveGlogalApps();
        }
        public void SaveDB()
        {
            _SaveDB();
        }
        public void SaveUsersApps()
        {
            _SaveUsersApps();
        }
        public void SaveMap()
        {
            _SaveMap();
        }
        public void SaveAnUserPresets()
        {
            _SaveAnUserPresets();
        }
        public void SaveTemplate()
        {
            _SaveTemplate();
        }
        public void SaveMeasMons()
        {
            _SaveMeasMons();
        }
        public void SaveAntennas()
        {
            _SaveAntennas();
        }
        public void SaveEquipments()
        {
            _SaveEquipments();
        }
        public void SaveATDIConnection()
        {
            _SaveATDIConnection();
        }
        public void ReloadAll()
        {
            try
            {
                StringCipher sc = new StringCipher();
                #region GNSS
                if (GetHash(Encoding.UTF8.GetBytes(GSet.GNSS.d1)) == GSet.GNSS.d2)//проверили
                { GPS_Read(sc.Decrypt(GSet.GNSS.d1)); }
                else
                {
                    GNSS_Settings = new GNSS_Set();
                    GSet.GNSS.d1 = sc.Encrypt(GNSS_Settings.GetString());
                    GSet.GNSS.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.GNSS.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion GNSS
                #region Analyzer
                if (GetHash(Encoding.UTF8.GetBytes(GSet.Analyzer.d1)) == GSet.Analyzer.d2)//проверили
                { Analyzer_Read(sc.Decrypt(GSet.Analyzer.d1)); }
                else
                {
                    Analyzer_Settings = new Analyzer_Set();
                    GSet.Analyzer.d1 = sc.Encrypt(Analyzer_Settings.GetString());
                    GSet.Analyzer.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Analyzer.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion Analyzer
                #region RsReceiver
                if (GetHash(Encoding.UTF8.GetBytes(GSet.RsReceiver.d1)) == GSet.RsReceiver.d2)//проверили
                { RsReceiver_Read(sc.Decrypt(GSet.RsReceiver.d1)); }
                else
                {
                    RsReceiver_Settings = new RsReceiver_Set();
                    GSet.RsReceiver.d1 = sc.Encrypt(RsReceiver_Settings.GetString());
                    GSet.RsReceiver.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.RsReceiver.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion RsReceiver
                #region TSMxReceiver
                if (GetHash(Encoding.UTF8.GetBytes(GSet.TSMxReceiver.d1)) == GSet.TSMxReceiver.d2)//проверили
                { TSMxReceiver_Read(sc.Decrypt(GSet.TSMxReceiver.d1)); AddDataTSMxReceiver(); }
                else
                {
                    TSMxReceiver_Settings = new TSMxReceiver_Set();
                    AddDataTSMxReceiver();
                    GSet.TSMxReceiver.d1 = sc.Encrypt(TSMxReceiver_Settings.GetString());
                    GSet.TSMxReceiver.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.TSMxReceiver.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion TSMxReceiver
                #region Screen
                if (GetHash(Encoding.UTF8.GetBytes(GSet.Screen.d1)) == GSet.Screen.d2)//проверили
                { Screen_Read(sc.Decrypt(GSet.Screen.d1)); }
                else
                {
                    Screen_Settings = new Screen_Set();
                    GSet.Screen.d1 = sc.Encrypt(Screen_Settings.GetString());
                    GSet.Screen.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Screen.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion Screen
                #region GlogalApps
                if (GetHash(Encoding.UTF8.GetBytes(GSet.GlogalApps.d1)) == GSet.GlogalApps.d2)//проверили
                { GlogalApps_Read(sc.Decrypt(GSet.GlogalApps.d1)); }
                else
                {
                    GlogalApps_Settings = new GlogalApps_Set();
                    GSet.GlogalApps.d1 = sc.Encrypt(GlogalApps_Settings.GetString());
                    GSet.GlogalApps.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.GlogalApps.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion GlogalApps
                #region DB
                if (GetHash(Encoding.UTF8.GetBytes(GSet.DB.d1)) == GSet.DB.d2)//проверили
                { DB_Read(sc.Decrypt(GSet.DB.d1)); }
                else
                {
                    DB_Settings = new DB_Set();
                    GSet.DB.d1 = sc.Encrypt(DB_Settings.GetString());
                    GSet.DB.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.DB.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion DB
                #region UsersApps
                if (GetHash(Encoding.UTF8.GetBytes(GSet.UsersApps.d1)) == GSet.UsersApps.d2)//проверили
                { UsersApps_Read(sc.Decrypt(GSet.UsersApps.d1)); }
                else
                {
                    UsersApps_Settings = new UsersApps_Set();
                    GSet.UsersApps.d1 = sc.Encrypt(UsersApps_Settings.GetString());
                    GSet.UsersApps.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.UsersApps.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion UsersApps
                #region Map
                if (GetHash(Encoding.UTF8.GetBytes(GSet.Map.d1)) == GSet.Map.d2)//проверили
                { Map_Read(sc.Decrypt(GSet.Map.d1)); }
                else
                {
                    Map_Settings = new Map_Set();
                    GSet.Map.d1 = sc.Encrypt(Map_Settings.GetString());
                    GSet.Map.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Map.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion Map
                #region AnUserPresets
                if (GetHash(Encoding.UTF8.GetBytes(GSet.AnUserPresets.d1)) == GSet.AnUserPresets.d2)//проверили
                { AnUserPresets_Read(sc.Decrypt(GSet.AnUserPresets.d1)); AddAnUserPresets(); }
                else
                {
                    AnUserPresets_Settings = new AnUserPresets_Set();
                    AddAnUserPresets();
                    GSet.AnUserPresets.d1 = sc.Encrypt(AnUserPresets_Settings.GetString());
                    GSet.AnUserPresets.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.AnUserPresets.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion AnUserPresets
                #region Template
                if (GetHash(Encoding.UTF8.GetBytes(GSet.Template.d1)) == GSet.Template.d2)//проверили
                { Template_Read(sc.Decrypt(GSet.Template.d1)); }
                else
                {
                    Template_Settings = new Template_Set();
                    GSet.Template.d1 = sc.Encrypt(Template_Settings.GetString());
                    GSet.Template.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Template.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion Template
                #region MeasMons
                if (GetHash(Encoding.UTF8.GetBytes(GSet.MeasMons.d1)) == GSet.MeasMons.d2)//проверили
                { MeasMons_Read(sc.Decrypt(GSet.MeasMons.d1)); /*AddMeasMons();*/ }
                else
                {
                    MeasMons_Settings = new MeasMons_Set();
                    //AddMeasMons();
                    GSet.MeasMons.d1 = sc.Encrypt(MeasMons_Settings.GetString());
                    GSet.MeasMons.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.MeasMons.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion MeasMons
                #region Antennas
                if (GetHash(Encoding.UTF8.GetBytes(GSet.Antennas.d1)) == GSet.Antennas.d2)//проверили
                { Antennas_Read(sc.Decrypt(GSet.Antennas.d1)); }
                else
                {
                    Antennas_Settings = new Antennas_Set();
                    GSet.Antennas.d1 = sc.Encrypt(Antennas_Settings.GetString());
                    GSet.Antennas.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Antennas.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion Antennas
                #region Equipments
                if (GetHash(Encoding.UTF8.GetBytes(GSet.Equipments.d1)) == GSet.Equipments.d2)//проверили
                { Equipments_Read(sc.Decrypt(GSet.Equipments.d1)); }
                else
                {
                    Equipments_Settings = new Equipments_Set();
                    GSet.Equipments.d1 = sc.Encrypt(Equipments_Settings.GetString());
                    GSet.Equipments.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Equipments.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion Equipments
                #region ATDIConnection
                if (GetHash(Encoding.UTF8.GetBytes(GSet.ATDIConnection.d1)) == GSet.ATDIConnection.d2)//проверили
                { ATDIConnection_Read(sc.Decrypt(GSet.ATDIConnection.d1)); }
                else
                {
                    ATDIConnection_Settings = new ATDIConnection_Set();
                    GSet.ATDIConnection.d1 = sc.Encrypt(ATDIConnection_Settings.GetString());
                    GSet.ATDIConnection.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.ATDIConnection.d1));
                    //чето написать что не стоит лазить 
                }
                #endregion ATDIConnection
            }
            catch (Exception exp) { }
        }
        #region Private
        private void _ReadAll()
        {
            try
            {
                StringCipher sc = new StringCipher();
                if (File.Exists(GSet.FileName))//файл есть
                {
                    Global_ReadXml();//прочитали 
                    bool resetdata = false;
                    #region GNSS
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.GNSS.d1)) == GSet.GNSS.d2)//проверили
                    { GPS_Read(sc.Decrypt(GSet.GNSS.d1)); }
                    else
                    {
                        GNSS_Settings = new GNSS_Set();
                        GSet.GNSS.d1 = sc.Encrypt(GNSS_Settings.GetString());
                        GSet.GNSS.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.GNSS.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion GNSS
                    #region Analyzer
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.Analyzer.d1)) == GSet.Analyzer.d2)//проверили
                    { Analyzer_Read(sc.Decrypt(GSet.Analyzer.d1)); }
                    else
                    {
                        Analyzer_Settings = new Analyzer_Set();
                        GSet.Analyzer.d1 = sc.Encrypt(Analyzer_Settings.GetString());
                        GSet.Analyzer.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Analyzer.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion Analyzer
                    #region RsReceiver
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.RsReceiver.d1)) == GSet.RsReceiver.d2)//проверили
                    { RsReceiver_Read(sc.Decrypt(GSet.RsReceiver.d1)); }
                    else
                    {
                        RsReceiver_Settings = new RsReceiver_Set();
                        GSet.RsReceiver.d1 = sc.Encrypt(RsReceiver_Settings.GetString());
                        GSet.RsReceiver.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.RsReceiver.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion RsReceiver
                    #region TSMxReceiver
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.TSMxReceiver.d1)) == GSet.TSMxReceiver.d2)//проверили
                    { TSMxReceiver_Read(sc.Decrypt(GSet.TSMxReceiver.d1)); AddDataTSMxReceiver(); }
                    else
                    {
                        TSMxReceiver_Settings = new TSMxReceiver_Set();
                        AddDataTSMxReceiver();
                        GSet.TSMxReceiver.d1 = sc.Encrypt(TSMxReceiver_Settings.GetString());
                        GSet.TSMxReceiver.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.TSMxReceiver.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion TSMxReceiver
                    #region Screen
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.Screen.d1)) == GSet.Screen.d2)//проверили
                    { Screen_Read(sc.Decrypt(GSet.Screen.d1)); }
                    else
                    {
                        Screen_Settings = new Screen_Set();
                        GSet.Screen.d1 = sc.Encrypt(Screen_Settings.GetString());
                        GSet.Screen.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Screen.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion Screen
                    #region GlogalApps
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.GlogalApps.d1)) == GSet.GlogalApps.d2)//проверили
                    { GlogalApps_Read(sc.Decrypt(GSet.GlogalApps.d1)); }
                    else
                    {
                        GlogalApps_Settings = new GlogalApps_Set();
                        GSet.GlogalApps.d1 = sc.Encrypt(GlogalApps_Settings.GetString());
                        GSet.GlogalApps.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.GlogalApps.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion GlogalApps
                    #region DB
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.DB.d1)) == GSet.DB.d2)//проверили
                    { DB_Read(sc.Decrypt(GSet.DB.d1)); }
                    else
                    {
                        DB_Settings = new DB_Set();
                        GSet.DB.d1 = sc.Encrypt(DB_Settings.GetString());
                        GSet.DB.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.DB.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion DB
                    #region UsersApps
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.UsersApps.d1)) == GSet.UsersApps.d2)//проверили
                    { UsersApps_Read(sc.Decrypt(GSet.UsersApps.d1)); }
                    else
                    {
                        UsersApps_Settings = new UsersApps_Set();
                        GSet.UsersApps.d1 = sc.Encrypt(UsersApps_Settings.GetString());
                        GSet.UsersApps.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.UsersApps.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion UsersApps
                    #region Map
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.Map.d1)) == GSet.Map.d2)//проверили
                    { Map_Read(sc.Decrypt(GSet.Map.d1)); }
                    else
                    {
                        Map_Settings = new Map_Set();
                        GSet.Map.d1 = sc.Encrypt(Map_Settings.GetString());
                        GSet.Map.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Map.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion Map
                    #region AnUserPresets
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.AnUserPresets.d1)) == GSet.AnUserPresets.d2)//проверили
                    { AnUserPresets_Read(sc.Decrypt(GSet.AnUserPresets.d1)); AddAnUserPresets(); }
                    else
                    {
                        AnUserPresets_Settings = new AnUserPresets_Set();
                        AddAnUserPresets();
                        GSet.AnUserPresets.d1 = sc.Encrypt(AnUserPresets_Settings.GetString());
                        GSet.AnUserPresets.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.AnUserPresets.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion AnUserPresets
                    #region Template
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.Template.d1)) == GSet.Template.d2)//проверили
                    { Template_Read(sc.Decrypt(GSet.Template.d1)); }
                    else
                    {
                        Template_Settings = new Template_Set();
                        GSet.Template.d1 = sc.Encrypt(Template_Settings.GetString());
                        GSet.Template.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Template.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion Template
                    #region MeasMons
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.MeasMons.d1)) == GSet.MeasMons.d2)//проверили
                    { MeasMons_Read(sc.Decrypt(GSet.MeasMons.d1)); /*AddMeasMons();*/ }
                    else
                    {
                        MeasMons_Settings = new MeasMons_Set();
                        //AddMeasMons();
                        GSet.MeasMons.d1 = sc.Encrypt(MeasMons_Settings.GetString());
                        GSet.MeasMons.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.MeasMons.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion MeasMons
                    #region Antennas
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.Antennas.d1)) == GSet.Antennas.d2)//проверили
                    { Antennas_Read(sc.Decrypt(GSet.Antennas.d1)); }
                    else
                    {
                        Antennas_Settings = new Antennas_Set();
                        GSet.Antennas.d1 = sc.Encrypt(Antennas_Settings.GetString());
                        GSet.Antennas.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Antennas.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion Antennas
                    #region Equipments
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.Equipments.d1)) == GSet.Equipments.d2)//проверили
                    { Equipments_Read(sc.Decrypt(GSet.Equipments.d1)); }
                    else
                    {
                        Equipments_Settings = new Equipments_Set();
                        GSet.Equipments.d1 = sc.Encrypt(Equipments_Settings.GetString());
                        GSet.Equipments.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Equipments.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion Equipments
                    #region ATDIConnection
                    if (GetHash(Encoding.UTF8.GetBytes(GSet.ATDIConnection.d1)) == GSet.ATDIConnection.d2)//проверили
                    { ATDIConnection_Read(sc.Decrypt(GSet.ATDIConnection.d1)); }
                    else
                    {
                        ATDIConnection_Settings = new ATDIConnection_Set();
                        GSet.ATDIConnection.d1 = sc.Encrypt(ATDIConnection_Settings.GetString());
                        GSet.ATDIConnection.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.ATDIConnection.d1));
                        resetdata = true;
                        //чето написать что не стоит лазить 
                    }
                    #endregion ATDIConnection
                    if (resetdata) GSet.Save();
                }
                else//неувидили файл с настройками все дропаем на деaаулт и сохраняем
                {
                    _SaveGNSS();
                    _SaveAnalyzer();
                    _SaveRsReceiver();
                    AddDataTSMxReceiver();
                    _SaveTSMxReceiver();
                    _SaveScreen();
                    _SaveGlogalApps();
                    _SaveDB();
                    _SaveUsersApps();
                    _SaveMap();
                    AddAnUserPresets();
                    _SaveAnUserPresets();
                    _SaveTemplate();
                    //AddMeasMons();
                    _SaveMeasMons();
                    _SaveAntennas();
                    _SaveEquipments();
                    _SaveATDIConnection();
                    GSet.Save();
                }
            }
            catch (Exception exp) { }
        }

        private void _SaveAll()
        {
            try
            {
                _SaveGNSS();
                _SaveAnalyzer();
                _SaveRsReceiver();
                _SaveTSMxReceiver();
                _SaveScreen();
                _SaveGlogalApps();
                _SaveDB();
                _SaveUsersApps();
                _SaveMap();
                AddAnUserPresets();
                _SaveAnUserPresets();
                _SaveTemplate();
                _SaveMeasMons();
                _SaveAntennas();
                _SaveEquipments();
                _SaveATDIConnection();
                GSet.Save();
            }
            catch (Exception exp) { }
        }

        private void _SaveGNSS()
        {
            StringCipher sc = new StringCipher();
            GSet.GNSS.d1 = sc.Encrypt(GNSS_Settings.GetString());
            GSet.GNSS.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.GNSS.d1));
        }
        private void _SaveAnalyzer()
        {
            StringCipher sc = new StringCipher();
            GSet.Analyzer.d1 = sc.Encrypt(Analyzer_Settings.GetString());
            GSet.Analyzer.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Analyzer.d1));
        }
        private void _SaveRsReceiver()
        {
            StringCipher sc = new StringCipher();
            GSet.RsReceiver.d1 = sc.Encrypt(RsReceiver_Settings.GetString());
            GSet.RsReceiver.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.RsReceiver.d1));
        }
        private void _SaveTSMxReceiver()
        {
            StringCipher sc = new StringCipher();
            GSet.TSMxReceiver.d1 = sc.Encrypt(TSMxReceiver_Settings.GetString());
            GSet.TSMxReceiver.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.TSMxReceiver.d1));
        }
        private void AddDataTSMxReceiver()
        {
            foreach (string t in Enum.GetNames(typeof(RohdeSchwarz.ViCom.Net.GSM.Pdu.Type)))
            {
                bool find = false;
                foreach (GSMSIType sit in TSMxReceiver_Settings.GSM.SITypes)
                {
                    if (t == sit.SiType) find = true;
                }
                if (!find) TSMxReceiver_Settings.GSM.SITypes.Add(new GSMSIType() { SiType = t, Use = false });
            }
            foreach (string t in Enum.GetNames(typeof(RohdeSchwarz.ViCom.Net.WCDMA.Pdu.Type)))
            {
                bool find = false;
                foreach (UMTSSIBType sit in TSMxReceiver_Settings.UMTS.SIBTypes)
                {
                    if (t == sit.SibType) find = true;
                }
                if (!find) TSMxReceiver_Settings.UMTS.SIBTypes.Add(new UMTSSIBType() { SibType = t, Use = false });
            }
            foreach (string t in Enum.GetNames(typeof(RohdeSchwarz.ViCom.Net.LTE.Pdu.Type)))
            {
                bool find = false;
                foreach (LTESIBType sit in TSMxReceiver_Settings.LTE.SIBTypes)
                {
                    if (t == sit.SibType) find = true;
                }
                if (!find) TSMxReceiver_Settings.LTE.SIBTypes.Add(new LTESIBType() { SibType = t, Use = false });
            }
            foreach (string t in Enum.GetNames(typeof(RohdeSchwarz.ViCom.Net.CDMA.Pdu.Type)))
            {
                bool find = false;
                foreach (CDMASIType sit in TSMxReceiver_Settings.CDMA.SITypes)
                {
                    if (t == sit.SiType) find = true;
                }
                if (!find) TSMxReceiver_Settings.CDMA.SITypes.Add(new CDMASIType() { SiType = t, Use = false });
            }
            if (TSMxReceiver_Settings.ACD.Data.Count() <= 0)
            {
                ObservableCollection<ACD_DataSet> Data = new ObservableCollection<ACD_DataSet>()
                {
                    new ACD_DataSet() {Band = 1, BandStr = "Band-1 2100", Tech = 2, TechStr = "UMTS", Use = false },
                    new ACD_DataSet() {Band = 2, BandStr = "Band-2 1900", Tech = 2, TechStr = "UMTS", Use = false },
                    new ACD_DataSet() {Band = 3, BandStr = "Band-3 1800", Tech = 2, TechStr = "UMTS", Use = false },
                    new ACD_DataSet() {Band = 4, BandStr = "Band-4 1700", Tech = 2, TechStr = "UMTS", Use = false },
                    new ACD_DataSet() {Band = 0, BandStr = "Band-0 800", Tech = 3, TechStr = "CDMA", Use = false },
                    new ACD_DataSet() {Band = 1, BandStr = "Band-1 1900", Tech = 3, TechStr = "CDMA", Use = false },
                    new ACD_DataSet() {Band = 5, BandStr = "Band-5 450", Tech = 3, TechStr = "CDMA", Use = false },
                    new ACD_DataSet() {Band = 14, BandStr = "Band-14 1900", Tech = 3, TechStr = "CDMA", Use = false },
                    new ACD_DataSet() {Band = 0, BandStr = "Band-0 800", Tech = 4, TechStr = "EVDO", Use = false },
                    new ACD_DataSet() {Band = 1, BandStr = "Band-1 1900", Tech = 4, TechStr = "EVDO", Use = false },
                    new ACD_DataSet() {Band = 5, BandStr = "Band-5 450", Tech = 4, TechStr = "EVDO", Use = false },
                    new ACD_DataSet() {Band = 14, BandStr = "Band-14 1900", Tech = 4, TechStr = "EVDO", Use = false },
                    new ACD_DataSet() {Band = 3, BandStr = "Band-3 1800", Tech = 5, TechStr = "LTE", Use = false },
                    new ACD_DataSet() {Band = 7, BandStr = "Band-7 2600", Tech = 5, TechStr = "LTE", Use = false },
                };
                foreach (ACD_DataSet t in Data)
                {
                    bool find = false;
                    foreach (ACD_DataSet s in TSMxReceiver_Settings.ACD.Data)
                    {
                        if (t.BandStr == s.BandStr && t.Tech == s.Tech) find = true;
                    }
                    if (!find) TSMxReceiver_Settings.ACD.Data.Add(t);
                }

            }
        }




        private void _SaveScreen()
        {
            StringCipher sc = new StringCipher();
            GSet.Screen.d1 = sc.Encrypt(Screen_Settings.GetString());
            GSet.Screen.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Screen.d1));
        }
        private void _SaveGlogalApps()
        {
            StringCipher sc = new StringCipher();
            GSet.GlogalApps.d1 = sc.Encrypt(GlogalApps_Settings.GetString());
            GSet.GlogalApps.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.GlogalApps.d1));
        }
        private void _SaveDB()
        {
            StringCipher sc = new StringCipher();
            GSet.DB.d1 = sc.Encrypt(DB_Settings.GetString());
            GSet.DB.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.DB.d1));
        }
        private void _SaveUsersApps()
        {
            StringCipher sc = new StringCipher();
            GSet.UsersApps.d1 = sc.Encrypt(UsersApps_Settings.GetString());
            GSet.UsersApps.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.UsersApps.d1));
        }
        private void _SaveMap()
        {
            StringCipher sc = new StringCipher();
            GSet.Map.d1 = sc.Encrypt(Map_Settings.GetString());
            GSet.Map.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Map.d1));
        }
        private void AddAnUserPresets()
        {
            ObservableCollection<UserPresetType_Set> Data = new ObservableCollection<UserPresetType_Set>()
            {
                new UserPresetType_Set() { },
                new UserPresetType_Set() { },
                new UserPresetType_Set() { },
                new UserPresetType_Set() { },
                new UserPresetType_Set() { },
                new UserPresetType_Set() { }
            };
            //AnUserPresets_Settings.UserPresets 
            if (AnUserPresets_Settings.UserPresets.Count == 0)
            {
                AnUserPresets_Settings.UserPresets = Data;
            }
        }
        private void _SaveAnUserPresets()
        {
            StringCipher sc = new StringCipher();
            GSet.AnUserPresets.d1 = sc.Encrypt(AnUserPresets_Settings.GetString());
            GSet.AnUserPresets.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.AnUserPresets.d1));
        }
        private void _SaveTemplate()
        {
            StringCipher sc = new StringCipher();
            GSet.Template.d1 = sc.Encrypt(Template_Settings.GetString());
            GSet.Template.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Template.d1));
        }
        //private void AddMeasMons()
        //{
        //    ObservableCollection<MeasMon_Set> Data = new ObservableCollection<MeasMon_Set>()
        //    {
        //        new MeasMon_Set() {
        //            Techonology = DB.Technologys.GSM,
        //            DetectionLevel = -100,
        //            Data = new MeasMonData_Set[]
        //            {
        //                new MeasMonData_Set()
        //                {
        //                    BW = 200000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 20,
        //                    MeasBW = 500000,
        //                    BWLimit = 350000,
        //                    RBW = 312.5m,
        //                    VBW = 312.5m,
        //                    NdBBWMin = 150000,
        //                    NdBBWMax = 400000,
        //                    MarPeakBW = 200000,
        //                    TracePoints = 1601
        //                }
        //            }
        //        },
        //        new MeasMon_Set()
        //        {
        //            Techonology = DB.Technologys.UMTS,
        //            DetectionLevel = -100,
        //            Data = new MeasMonData_Set[]
        //            {
        //                new MeasMonData_Set()
        //                {
        //                    BW = 5000000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 5,
        //                    MeasBW = 5000000,
        //                    BWLimit = 4850000,
        //                    RBW = 3125,
        //                    VBW = 3125,
        //                    NdBBWMin = 4000000,
        //                    NdBBWMax = 4850000,
        //                    MarPeakBW = 4800000,
        //                    TracePoints = 1601
        //                }
        //            }
        //        },
        //        new MeasMon_Set()
        //        {
        //            Techonology = DB.Technologys.CDMA,
        //            DetectionLevel = -100,
        //            Data = new MeasMonData_Set[]
        //            {
        //                new MeasMonData_Set()
        //                {
        //                    BW = 1250000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 0.05m,
        //                    MeasBW = 2000000,
        //                    BWLimit = 1300000,
        //                    RBW = 1250,
        //                    VBW = 1250,
        //                    NdBBWMin = 1100000,
        //                    NdBBWMax = 1300000,
        //                    MarPeakBW = 1250000,
        //                    TracePoints = 1601
        //                }
        //            }
        //        },
        //        new MeasMon_Set()
        //        {
        //            Techonology = DB.Technologys.LTE,
        //            DetectionLevel = -100,
        //            Data = new MeasMonData_Set[]
        //            {
        //                new MeasMonData_Set()
        //                {
        //                    BW = 1400000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 20,
        //                    MeasBW = 2000000,
        //                    BWLimit = 1358000,
        //                    RBW = 1250,
        //                    VBW = 1250,
        //                    NdBBWMin = 1190000,
        //                    NdBBWMax = 1372000,
        //                    MarPeakBW = 1330000,
        //                    TracePoints = 1601
        //                },
        //                new MeasMonData_Set()
        //                {
        //                    BW = 3000000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 20,
        //                    MeasBW = 3000000,
        //                    BWLimit = 2910000,
        //                    RBW = 3125,
        //                    VBW = 3125,
        //                    NdBBWMin = 2550000,
        //                    NdBBWMax = 2940000,
        //                    MarPeakBW = 2850000,
        //                    TracePoints = 1601
        //                },
        //                new MeasMonData_Set()
        //                {
        //                    BW = 5000000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 20,
        //                    MeasBW = 5000000,
        //                    BWLimit = 4850000,
        //                    RBW = 3125,
        //                    VBW = 3125,
        //                    NdBBWMin = 4250000,
        //                    NdBBWMax = 4900000,
        //                    MarPeakBW = 4750000,
        //                    TracePoints = 1601
        //                },
        //                new MeasMonData_Set()
        //                {
        //                    BW = 10000000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 20,
        //                    MeasBW = 10000000,
        //                    BWLimit = 9700000,
        //                    RBW = 6250,
        //                    VBW = 6250,
        //                    NdBBWMin = 8500000,
        //                    NdBBWMax = 9800000,
        //                    MarPeakBW = 9500000,
        //                    TracePoints = 1601
        //                },
        //                new MeasMonData_Set()
        //                {
        //                    BW = 15000000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 20,
        //                    MeasBW = 15000000,
        //                    BWLimit = 14550000,
        //                    RBW = 6250,
        //                    VBW = 6250,
        //                    NdBBWMin = 12750000,
        //                    NdBBWMax = 14700000,
        //                    MarPeakBW = 14250000,
        //                    TracePoints = 1601
        //                },
        //                new MeasMonData_Set()
        //                {
        //                    BW = 20000000,
        //                    OBWPercent = 99,
        //                    NdBLevel = 30,
        //                    DeltaFreqLimit = 20,
        //                    MeasBW = 20000000,
        //                    BWLimit = 19400000,
        //                    RBW = 12500,
        //                    VBW = 12500,
        //                    NdBBWMin = 17000000,
        //                    NdBBWMax = 19600000,
        //                    MarPeakBW = 19000000,
        //                    TracePoints = 1601
        //                }
        //            },
        //        },


        //    };
        //    foreach (MeasMon_Set t in Data)
        //    {
        //        bool find = false;
        //        foreach (MeasMon_Set us in MeasMons_Settings.MeasMons)
        //        {
        //            if (t.Techonology == us.Techonology) find = true;
        //        }
        //        if (!find) MeasMons_Settings.MeasMons.Add(t);
        //    }
        //}
        private void _SaveMeasMons()
        {
            StringCipher sc = new StringCipher();
            GSet.MeasMons.d1 = sc.Encrypt(MeasMons_Settings.GetString());
            GSet.MeasMons.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.MeasMons.d1));
        }
        private void _SaveAntennas()
        {
            StringCipher sc = new StringCipher();
            GSet.Antennas.d1 = sc.Encrypt(Antennas_Settings.GetString());
            GSet.Antennas.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Antennas.d1));
        }
        private void _SaveEquipments()
        {
            StringCipher sc = new StringCipher();
            GSet.Equipments.d1 = sc.Encrypt(Equipments_Settings.GetString());
            GSet.Equipments.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.Equipments.d1));
        }
        private void _SaveATDIConnection()
        {
            StringCipher sc = new StringCipher();
            GSet.ATDIConnection.d1 = sc.Encrypt(ATDIConnection_Settings.GetString());
            GSet.ATDIConnection.d2 = GetHash(Encoding.UTF8.GetBytes(GSet.ATDIConnection.d1));
        }
        private string GetHash(byte[] stream)
        {
            byte[] bytes;
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                bytes = md5.ComputeHash(stream);
            return Convert.ToBase64String(bytes);
            //var buffer = new StringBuilder(bytes.Length * 2);
            //foreach (var b in bytes)
            //    buffer.AppendFormat("{0:x2}", b);
            //return buffer.ToString();
        }
        private void Global_ReadXml()
        {
            if (File.Exists(GSet.FileName))
            {
                XmlSerializer ser = new XmlSerializer(typeof(Global_Set));
                TextReader reader = new StreamReader(GSet.FileName);
                GSet = ser.Deserialize(reader) as Global_Set;
                reader.Close();
            }
            else
            {//можно написать вывод какова то сообщения если файла не существует}

            }

        }
        #region         
        private void GPS_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(GNSS_Set));
            TextReader reader = new StringReader(inpstr);
            GNSS_Settings = ser.Deserialize(reader) as GNSS_Set;
            reader.Close();
        }
        private void Analyzer_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Analyzer_Set));
            TextReader reader = new StringReader(inpstr);
            Analyzer_Settings = ser.Deserialize(reader) as Analyzer_Set;
            reader.Close();
        }
        private void RsReceiver_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(RsReceiver_Set));
            TextReader reader = new StringReader(inpstr);
            RsReceiver_Settings = ser.Deserialize(reader) as RsReceiver_Set;
            reader.Close();
        }
        private void TSMxReceiver_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TSMxReceiver_Set));
            TextReader reader = new StringReader(inpstr);
            TSMxReceiver_Settings = ser.Deserialize(reader) as TSMxReceiver_Set;
            reader.Close();
        }
        private void Screen_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Screen_Set));
            TextReader reader = new StringReader(inpstr);
            Screen_Settings = ser.Deserialize(reader) as Screen_Set;
            reader.Close();
        }
        private void GlogalApps_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(GlogalApps_Set));
            TextReader reader = new StringReader(inpstr);
            GlogalApps_Settings = ser.Deserialize(reader) as GlogalApps_Set;
            reader.Close();
        }
        private void DB_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DB_Set));
            TextReader reader = new StringReader(inpstr);
            DB_Settings = ser.Deserialize(reader) as DB_Set;
            reader.Close();
        }
        private void UsersApps_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(UsersApps_Set));
            TextReader reader = new StringReader(inpstr);
            UsersApps_Settings = ser.Deserialize(reader) as UsersApps_Set;
            reader.Close();
        }
        private void Map_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Map_Set));
            TextReader reader = new StringReader(inpstr);
            Map_Settings = ser.Deserialize(reader) as Map_Set;
            reader.Close();
        }
        private void AnUserPresets_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(AnUserPresets_Set));
            TextReader reader = new StringReader(inpstr);
            AnUserPresets_Settings = ser.Deserialize(reader) as AnUserPresets_Set;
            reader.Close();
        }
        private void Template_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Template_Set));
            TextReader reader = new StringReader(inpstr);
            Template_Settings = ser.Deserialize(reader) as Template_Set;
            reader.Close();
        }
        private void MeasMons_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MeasMons_Set));
            TextReader reader = new StringReader(inpstr);
            MeasMons_Settings = ser.Deserialize(reader) as MeasMons_Set;
            reader.Close();
        }
        private void Antennas_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Antennas_Set));
            TextReader reader = new StringReader(inpstr);
            Antennas_Settings = ser.Deserialize(reader) as Antennas_Set;
            reader.Close();
        }
        private void Equipments_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Equipments_Set));
            TextReader reader = new StringReader(inpstr);
            Equipments_Settings = ser.Deserialize(reader) as Equipments_Set;
            reader.Close();
        }
        private void ATDIConnection_Read(string inpstr)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ATDIConnection_Set));
            TextReader reader = new StringReader(inpstr);
            ATDIConnection_Settings = ser.Deserialize(reader) as ATDIConnection_Set;
            reader.Close();
        }
        #endregion

        #region **crypt
        private class StringCipher
        {
            private string pass = "28031985KEAZs0X+sjzqujq+CdnsaPmdxDJv5iocLFw=";
            // This constant is used to determine the keysize of the encryption algorithm in bits.
            // We divide this by 8 within the code below to get the equivalent number of bytes.
            private const int Keysize = 256;

            // This constant determines the number of iterations for the password bytes generation function.
            private const int DerivationIterations = 1000;

            public string Encrypt(string plainText)
            {
                // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                var saltStringBytes = Generate256BitsOfRandomEntropy();
                var ivStringBytes = Generate256BitsOfRandomEntropy();
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                using (var password = new Rfc2898DeriveBytes(pass, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                    var cipherTextBytes = saltStringBytes;
                                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }

            public string Decrypt(string cipherText)
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(pass, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }

            public byte[] Generate256BitsOfRandomEntropy()
            {
                byte[] randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
                {
                    // Fill the array with cryptographically secure random bytes.
                    rngCsp.GetBytes(randomBytes);
                }
                return randomBytes;
            }
        }

        #endregion
        #endregion



























        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }

        #region BackupAllSettings
        public void SaveBackupAllSettings()
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "BackupSettings"; // Default file name
                dlg.DefaultExt = ".cusb"; // Default file extension
                dlg.Filter = "ControlU Settings Backup (.cusb)|*.cusb"; // Filter files by extension
                string fileNamePath = string.Empty;
                if (dlg.ShowDialog() == true)
                {
                    fileNamePath = dlg.FileName;
                    if (File.Exists(fileNamePath))
                    {
                        File.Delete(fileNamePath);
                    }
                    using (System.IO.Compression.ZipArchive zip = System.IO.Compression.ZipFile.Open(fileNamePath, ZipArchiveMode.Create))
                    {
                        if (File.Exists(AppPath + "\\Settings.xml")) zip.CreateEntryFromFile(AppPath + "\\Settings.xml", "Settings.xml");
                        if (File.Exists(AppPath + "\\license.lic")) zip.CreateEntryFromFile(AppPath + "\\license.lic", "license.lic");
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "XMLSettings", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        public void RestoreBackupAllSettings()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "BackupSettings"; // Default file name
                dlg.DefaultExt = ".cusb"; // Default file extension
                dlg.Filter = "ControlU Settings Backup (.cusb)|*.cusb"; // Filter files by extension
                string fileNamePath = string.Empty;
                if (dlg.ShowDialog() == true)
                {
                    fileNamePath = dlg.FileName;
                    using (System.IO.Compression.ZipArchive zip = System.IO.Compression.ZipFile.Open(fileNamePath, ZipArchiveMode.Read))
                    {
                        string file1 = AppPath + "\\license.lic";
                        if (File.Exists(file1))
                        {
                            File.Delete(file1);
                        }
                        string file2 = AppPath + "\\Settings.xml";
                        if (File.Exists(file2))
                        {
                            File.Delete(file2);
                        }
                        zip.ExtractToDirectory(AppPath);
                        ReadAll();
                        MainWindow.db_v2.UpdateATDIConnection(App.Sett.ATDIConnection_Settings.Selected);
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "XMLSettings", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
        }
        #endregion BackupAllSettings
        static string GetHash(Stream stream)
        {
            byte[] bytes;
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                bytes = md5.ComputeHash(stream);

            var buffer = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                buffer.AppendFormat("{0:x2}", b);
            return buffer.ToString();
        }



        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class Global_Set
    {
        [XmlIgnore]
        public string FileName
        {
            get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Settings.xml"; }
            private set { }
        }
        [XmlElement(ElementName = "Data0")]
        public Set GNSS
        {
            get { return _GNSS; }
            set { _GNSS = value; }
        }
        [XmlIgnore]
        private Set _GNSS = new Set();

        [XmlElement(ElementName = "Data1")]
        public Set Analyzer
        {
            get { return _Analyzer; }
            set { _Analyzer = value; }
        }
        [XmlIgnore]
        private Set _Analyzer = new Set();

        [XmlElement(ElementName = "Data2")]
        public Set RsReceiver
        {
            get { return _RsReceiver; }
            set { _RsReceiver = value; }
        }
        [XmlIgnore]
        private Set _RsReceiver = new Set();

        [XmlElement(ElementName = "Data3")]
        public Set TSMxReceiver
        {
            get { return _TSMxReceiver; }
            set { _TSMxReceiver = value; }
        }
        [XmlIgnore]
        private Set _TSMxReceiver = new Set();

        [XmlElement(ElementName = "Data4")]
        public Set Screen
        {
            get { return _Screen; }
            set { _Screen = value; }
        }
        [XmlIgnore]
        private Set _Screen = new Set();

        [XmlElement(ElementName = "Data5")]
        public Set GlogalApps
        {
            get { return _GlogalApps; }
            set { _GlogalApps = value; }
        }
        [XmlIgnore]
        private Set _GlogalApps = new Set();

        [XmlElement(ElementName = "Data6")]
        public Set DB
        {
            get { return _DB; }
            set { _DB = value; }
        }
        [XmlIgnore]
        private Set _DB = new Set();

        [XmlElement(ElementName = "Data7")]
        public Set UsersApps
        {
            get { return _UsersApps; }
            set { _UsersApps = value; }
        }
        [XmlIgnore]
        private Set _UsersApps = new Set();

        [XmlElement(ElementName = "Data8")]
        public Set Map
        {
            get { return _Map; }
            set { _Map = value; }
        }
        [XmlIgnore]
        private Set _Map = new Set();

        [XmlElement(ElementName = "Data9")]
        public Set AnUserPresets
        {
            get { return _AnUserPresets; }
            set { _AnUserPresets = value; }
        }
        [XmlIgnore]
        private Set _AnUserPresets = new Set();

        [XmlElement(ElementName = "Data10")]
        public Set Template
        {
            get { return _Template; }
            set { _Template = value; }
        }
        [XmlIgnore]
        private Set _Template = new Set();

        [XmlElement(ElementName = "Data11")]
        public Set MeasMons
        {
            get { return _MeasMons; }
            set { _MeasMons = value; }
        }
        [XmlIgnore]
        private Set _MeasMons = new Set();

        [XmlElement(ElementName = "Data12")]
        public Set Antennas
        {
            get { return _Antennas; }
            set { _Antennas = value; }
        }
        [XmlIgnore]
        private Set _Antennas = new Set();

        [XmlElement(ElementName = "Data13")]
        public Set Equipments
        {
            get { return _Equipments; }
            set { _Equipments = value; }
        }
        [XmlIgnore]
        private Set _Equipments = new Set();

        [XmlElement(ElementName = "Data14")]
        public Set ATDIConnection
        {
            get { return _ATDIConnection; }
            set { _ATDIConnection = value; }
        }
        [XmlIgnore]
        private Set _ATDIConnection = new Set();
        public void Save()
        {
            Serialize();
        }
        private void Serialize()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Global_Set));
            TextWriter writer = new StreamWriter(FileName, false);
            ser.Serialize(writer, this);
            writer.Close();
        }
    }
    [Serializable]
    public class Set
    {
        [XmlElement]
        public string d1
        {
            get { return _d1; }
            set { _d1 = value; }
        }
        [XmlIgnore]
        private string _d1 = string.Empty;

        [XmlElement]
        public string d2
        {
            get { return _d2; }
            set { _d2 = value; }
        }
        [XmlIgnore]
        private string _d2 = string.Empty;
    }


    [Serializable]
    public class GNSS_Set : INotifyPropertyChanged
    {

        [XmlElement]
        public string PortName
        {
            get { return _PortName; }
            set { _PortName = value; OnPropertyChanged("PortName"); }
        }
        private string _PortName = string.Empty;

        [XmlElement]
        public int PortBaudRate
        {
            get { return _PortBaudRate; }
            set { _PortBaudRate = value; OnPropertyChanged("PortBaudRate"); }
        }
        private int _PortBaudRate = 4800;

        [XmlElement]
        public System.IO.Ports.Parity PortParity
        {
            get { return _PortParity; }
            set { _PortParity = value; OnPropertyChanged("PortParity"); }
        }
        private System.IO.Ports.Parity _PortParity = System.IO.Ports.Parity.None;

        [XmlElement]
        public int PortDataBits
        {
            get { return _PortDataBits; }
            set { _PortDataBits = value; OnPropertyChanged("PortDataBits"); }
        }
        private int _PortDataBits = 8;

        [XmlElement]
        public System.IO.Ports.StopBits PortStopBits
        {
            get { return _PortStopBits; }
            set { _PortStopBits = value; OnPropertyChanged("PortStopBits"); }
        }
        private System.IO.Ports.StopBits _PortStopBits = System.IO.Ports.StopBits.One;

        [XmlElement]
        public System.IO.Ports.Handshake PortHandshake
        {
            get { return _PortHandshake; }
            set { _PortHandshake = value; OnPropertyChanged("PortHandshake"); }
        }
        private System.IO.Ports.Handshake _PortHandshake = System.IO.Ports.Handshake.None;

        [XmlElement]
        public bool ConnectAtStartup
        {
            get { return _ConnectAtStartup; }
            set { _ConnectAtStartup = value; OnPropertyChanged("ConnectAtStartup"); }
        }
        private bool _ConnectAtStartup = false;

        [XmlElement]
        public double PreviousLatitudeDouble
        {
            get { return _PreviousLatitudeDouble; }
            set { _PreviousLatitudeDouble = value; OnPropertyChanged("PreviousLatitudeDouble"); }
        }
        private double _PreviousLatitudeDouble = 0;

        [XmlElement]
        public double PreviousLongitudeDouble
        {
            get { return _PreviousLongitudeDouble; }
            set { _PreviousLongitudeDouble = value; OnPropertyChanged("PreviousLongitudeDouble"); }
        }
        private double _PreviousLongitudeDouble = 0;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(GNSS_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class Analyzer_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public string IPAdress
        {
            get { return _IPAdress; }
            set { _IPAdress = value; OnPropertyChanged("IPAdress"); }
        }
        private string _IPAdress = "";

        [XmlElement]
        public bool ConnectAtStartup
        {
            get { return _ConnectAtStartup; }
            set { _ConnectAtStartup = value; OnPropertyChanged("ConnectAtStartup"); }
        }
        private bool _ConnectAtStartup = false;

        [XmlElement]
        public bool ResetDevice
        {
            get { return _ResetDevice; }
            set { _ResetDevice = value; OnPropertyChanged("ResetDevice"); }
        }
        private bool _ResetDevice = false;

        [XmlElement]
        public bool IdQuery
        {
            get { return _IdQuery; }
            set { _IdQuery = value; OnPropertyChanged("IdQuery"); }
        }
        private bool _IdQuery = false;

        [XmlElement]
        public bool DisplayUpdate
        {
            get { return _DisplayUpdate; }
            set { _DisplayUpdate = value; OnPropertyChanged("DisplayUpdate"); }
        }
        private bool _DisplayUpdate = false;

        [XmlElement]
        public string UserPresetName1
        {
            get { return _UserPresetName1; }
            set { _UserPresetName1 = value; OnPropertyChanged("UserPresetName1"); }
        }
        private string _UserPresetName1 = "";

        [XmlElement]
        public string UserPresetName2
        {
            get { return _UserPresetName2; }
            set { _UserPresetName2 = value; OnPropertyChanged("UserPresetName2"); }
        }
        private string _UserPresetName2 = "";

        [XmlElement]
        public string UserPresetName3
        {
            get { return _UserPresetName3; }
            set { _UserPresetName3 = value; OnPropertyChanged("UserPresetName3"); }
        }
        private string _UserPresetName3 = "";

        [XmlElement]
        public string UserPresetName4
        {
            get { return _UserPresetName4; }
            set { _UserPresetName4 = value; OnPropertyChanged("UserPresetName4"); }
        }
        private string _UserPresetName4 = "";

        [XmlElement]
        public string UserPresetName5
        {
            get { return _UserPresetName5; }
            set { _UserPresetName5 = value; OnPropertyChanged("UserPresetName5"); }
        }
        private string _UserPresetName5 = "";

        [XmlElement]
        public string UserPresetName6
        {
            get { return _UserPresetName6; }
            set { _UserPresetName6 = value; OnPropertyChanged("UserPresetName6"); }
        }
        private string _UserPresetName6 = "";

        [XmlElement]
        public string UserPresetFilePath
        {
            get { return _UserPresetFilePath; }
            set { _UserPresetFilePath = value; OnPropertyChanged("UserPresetFilePath"); }
        }
        private string _UserPresetFilePath = "";

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(Analyzer_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class RsReceiver_Set : INotifyPropertyChanged
    {
        private string _IPAdress = "";
        [XmlElement]
        public string IPAdress
        {
            get { return _IPAdress; }
            set { _IPAdress = value; OnPropertyChanged("IPAdress"); }
        }
        private int _TCPPort = 5555;
        [XmlElement]
        public int TCPPort
        {
            get { return _TCPPort; }
            set { _TCPPort = value; OnPropertyChanged("TCPPort"); }
        }
        private int _UdpPort = 5555;
        [XmlElement]
        public int UdpPort
        {
            get { return _UdpPort; }
            set { _UdpPort = value; OnPropertyChanged("UdpPort"); }
        }
        private bool _ConnectAtStartup = false;
        [XmlElement]
        public bool ConnectAtStartup
        {
            get { return _ConnectAtStartup; }
            set { _ConnectAtStartup = value; OnPropertyChanged("ConnectAtStartup"); }
        }
        private bool _IntegratedGPS = false;
        [XmlElement]
        public bool IntegratedGPS
        {
            get { return _IntegratedGPS; }
            set { _IntegratedGPS = value; OnPropertyChanged("IntegratedGPS"); }
        }
        private string _UserPresetName1 = "";
        [XmlElement]
        public string UserPresetName1
        {
            get { return _UserPresetName1; }
            set { _UserPresetName1 = value; OnPropertyChanged("UserPresetName1"); }
        }
        private string _UserPresetName2 = "";
        [XmlElement]
        public string UserPresetName2
        {
            get { return _UserPresetName2; }
            set { _UserPresetName2 = value; OnPropertyChanged("UserPresetName2"); }
        }
        private string _UserPresetName3 = "";
        [XmlElement]
        public string UserPresetName3
        {
            get { return _UserPresetName3; }
            set { _UserPresetName3 = value; OnPropertyChanged("UserPresetName3"); }
        }
        private string _UserPresetName4 = "";
        [XmlElement]
        public string UserPresetName4
        {
            get { return _UserPresetName4; }
            set { _UserPresetName4 = value; OnPropertyChanged("UserPresetName4"); }
        }
        private string _UserPresetName5 = "";
        [XmlElement]
        public string UserPresetName5
        {
            get { return _UserPresetName5; }
            set { _UserPresetName5 = value; OnPropertyChanged("UserPresetName5"); }
        }
        private string _UserPresetName6 = "";
        [XmlElement]
        public string UserPresetName6
        {
            get { return _UserPresetName6; }
            set { _UserPresetName6 = value; OnPropertyChanged("UserPresetName6"); }
        }
        private string _UserPresetFilePath = "";
        [XmlElement]
        public string UserPresetFilePath
        {
            get { return _UserPresetFilePath; }
            set { _UserPresetFilePath = value; OnPropertyChanged("UserPresetFilePath"); }
        }
        private string _Auxiliary1 = "";
        [XmlElement]
        public string Auxiliary1
        {
            get { return _Auxiliary1; }
            set { _Auxiliary1 = value; OnPropertyChanged("Auxiliary1"); }
        }
        private string _Auxiliary2 = "";
        [XmlElement]
        public string Auxiliary2
        {
            get { return _Auxiliary2; }
            set { _Auxiliary2 = value; OnPropertyChanged("Auxiliary2"); }
        }
        private string _GPS = "";
        [XmlElement]
        public string GPS
        {
            get { return _GPS; }
            set { _GPS = value; OnPropertyChanged("GPS"); }
        }
        private string _COM = "";
        [XmlElement]
        public string COM
        {
            get { return _COM; }
            set { _COM = value; OnPropertyChanged("COM"); }
        }
        [XmlElement]
        public string DFAntennaReference
        {
            get { return _DFAntennaReference; }
            set { _DFAntennaReference = value; OnPropertyChanged("DFAntennaReference"); }
        }
        private string _DFAntennaReference = "";
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(RsReceiver_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class TSMxReceiver_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public string IPAdress
        {
            get { return _IPAdress; }
            set { _IPAdress = value; OnPropertyChanged("IPAdress"); }
        }
        private string _IPAdress = "";

        /// <summary>
        /// 0: Not selected
        /// 1: TSMW
        /// 2: TSME
        /// </summary>
        [XmlElement]
        public int DeviceType
        {
            get { return _DeviceType; }
            set
            {
                if (value < 0) _DeviceType = 0;
                else if (value > 2) _DeviceType = 2;
                else _DeviceType = value;
                OnPropertyChanged("DeviceType");
            }
        }
        private int _DeviceType = 1;

        [XmlElement]
        public string SerialNumber
        {
            get { return _SerialNumber; }
            set { _SerialNumber = value; OnPropertyChanged("SerialNumber"); }
        }
        private string _SerialNumber = "";

        [XmlElement]
        public TechOption_Set Option
        {
            get { return _Option; }
            set { _Option = value; OnPropertyChanged("Option"); }
        }
        private TechOption_Set _Option = new TechOption_Set();


        /// <summary>
        /// 0: Not selected
        /// 1: RF1 = Identification RF2 = Spectrum Analyzer
        /// 2: RF1 = Spectrum Analyzer RF2 = Identification
        /// 3: RF1 = Spectrum Analyzer RF2 = Spectrum Analyzer
        /// 4: RF1 = Identification RF2 = Identification
        /// </summary>
        [XmlElement]
        public int TSMWRFInput
        {
            get { return _TSMWRFInput; }
            set { _TSMWRFInput = value; OnPropertyChanged("TSMWRFInput"); }
        }
        private int _TSMWRFInput = 1;

        [XmlElement]
        public Antena_Set TSMWRX1Antena
        {
            get { return _TSMWRX1Antena; }
            set { _TSMWRX1Antena = value; OnPropertyChanged("TSMWRX1Antena"); }
        }
        private Antena_Set _TSMWRX1Antena;

        [XmlElement]
        public Antena_Set TSMWRX2Antena
        {
            get { return _TSMWRX2Antena; }
            set { _TSMWRX2Antena = value; OnPropertyChanged("TSMWRX2Antena"); }
        }
        private Antena_Set _TSMWRX2Antena;

        [XmlElement]
        public Antena_Set TSMExRXAntena
        {
            get { return _TSMExRXAntena; }
            set { _TSMExRXAntena = value; OnPropertyChanged("TSMExRXAntena"); }
        }
        private Antena_Set _TSMExRXAntena;

        [XmlElement]
        public bool ConnectAtStartup
        {
            get { return _ConnectAtStartup; }
            set { _ConnectAtStartup = value; OnPropertyChanged("ConnectAtStartup"); }
        }
        private bool _ConnectAtStartup = false;

        [XmlElement]
        public bool IntegratedGPS
        {
            get { return _IntegratedGPS; }
            set { _IntegratedGPS = value; OnPropertyChanged("IntegratedGPS"); }
        }
        private bool _IntegratedGPS = false;

        [XmlElement]
        public GSM_Set GSM
        {
            get { return _GSM; }
            set { _GSM = value; OnPropertyChanged("GSM"); }
        }
        private GSM_Set _GSM = new GSM_Set();

        [XmlElement]
        public UMTS_Set UMTS
        {
            get { return _UMTS; }
            set { _UMTS = value; OnPropertyChanged("UMTS"); }
        }
        private UMTS_Set _UMTS = new UMTS_Set();

        [XmlElement]
        public LTE_Set LTE
        {
            get { return _LTE; }
            set { _LTE = value; OnPropertyChanged("LTE"); }
        }
        private LTE_Set _LTE = new LTE_Set();

        [XmlElement]
        public CDMA_Set CDMA
        {
            get { return _CDMA; }
            set { _CDMA = value; OnPropertyChanged("CDMA"); }
        }
        private CDMA_Set _CDMA = new CDMA_Set();

        [XmlElement]
        public bool UseRFPowerScan
        {
            get { return _UseRFPowerScan; }
            set { _UseRFPowerScan = value; OnPropertyChanged("UseRFPowerScan"); }
        }
        private bool _UseRFPowerScan = false;

        [XmlElement]
        public ACD_Set ACD
        {
            get { return _ACD; }
            set { _ACD = value; OnPropertyChanged("ACD"); }
        }
        private ACD_Set _ACD = new ACD_Set();
        private Antena_Set _TSMxRX2Antena;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(TSMxReceiver_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    #region GSM
    [Serializable]
    public class GSM_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public bool TechIsEnabled
        {
            get { return _TechIsEnabled; }
            set { _TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        private bool _TechIsEnabled = false;

        [XmlElement]
        public int TSMWRfInput
        {
            get { return _TSMWRfInput; }
            set
            {
                if (value > 2) _TSMWRfInput = 2;
                else if (value < 1) _TSMWRfInput = 1;
                else _TSMWRfInput = value;
                OnPropertyChanged("TSMWRfInput");
            }
        }
        private int _TSMWRfInput = 1;

        [XmlArray]
        public ObservableCollection<GSMBand_Set> Bands
        {
            get { return _Bands; }
            set { _Bands = value; OnPropertyChanged("Bands"); }
        }
        private ObservableCollection<GSMBand_Set> _Bands = new ObservableCollection<GSMBand_Set>() { };

        [XmlArray]
        public ObservableCollection<GSMSIType> SITypes
        {
            get { return _SITypes; }
            set { _SITypes = value; OnPropertyChanged("SITypes"); }
        }
        private ObservableCollection<GSMSIType> _SITypes = new ObservableCollection<GSMSIType>() { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class GSMBand_Set : INotifyPropertyChanged
    {
        private Equipment.GSMBands _Band = 0;
        [XmlElement]
        public Equipment.GSMBands Band
        {
            get { return _Band; }
            set { _Band = value; OnPropertyChanged("Band"); }
        }
        private decimal _FreqStart = 0;
        [XmlElement]
        public decimal FreqStart
        {
            get { return _FreqStart; }
            set { _FreqStart = value; OnPropertyChanged("FreqStart"); }
        }
        private decimal _FreqStop = 0;
        [XmlElement]
        public decimal FreqStop
        {
            get { return _FreqStop; }
            set { _FreqStop = value; OnPropertyChanged("FreqStop"); }
        }
        private decimal _ARFCNStart = 0;
        [XmlElement]
        public decimal ARFCNStart
        {
            get { return _ARFCNStart; }
            set { _ARFCNStart = value; OnPropertyChanged("ARFCNStart"); }
        }
        private decimal _ARFCNStop = 0;
        [XmlElement]
        public decimal ARFCNStop
        {
            get { return _ARFCNStop; }
            set { _ARFCNStop = value; OnPropertyChanged("ARFCNStop"); }
        }
        private bool _Use = false;
        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class GSMSIType : INotifyPropertyChanged
    {
        #region 
        private string _SiType;
        [XmlElement]
        public string SiType
        {
            get { return _SiType; }
            set { _SiType = value; OnPropertyChanged("SiType"); }
        }
        private bool _Use;
        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    #endregion
    #region UMTS
    [Serializable]
    public class UMTS_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public bool TechIsEnabled
        {
            get { return _TechIsEnabled; }
            set { _TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        private bool _TechIsEnabled = false;

        [XmlElement]
        public int TSMWRfInput
        {
            get { return _TSMWRfInput; }
            set
            {
                if (value > 2) _TSMWRfInput = 2;
                else if (value < 1) _TSMWRfInput = 1;
                else _TSMWRfInput = value;
                OnPropertyChanged("TSMWRfInput");
            }
        }
        private int _TSMWRfInput = 1;


        [XmlArray]
        public ObservableCollection<UMTSFreqs_Set> Freqs
        {
            get { return _Freqs; }
            set { _Freqs = value; OnPropertyChanged("Freqs"); }
        }
        private ObservableCollection<UMTSFreqs_Set> _Freqs = new ObservableCollection<UMTSFreqs_Set>() { };

        [XmlArray]
        public ObservableCollection<UMTSSIBType> SIBTypes
        {
            get { return _SIBTypes; }
            set { _SIBTypes = value; OnPropertyChanged("SIBTypes"); }
        }
        private ObservableCollection<UMTSSIBType> _SIBTypes = new ObservableCollection<UMTSSIBType>() { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class UMTSFreqs_Set : INotifyPropertyChanged
    {

        private decimal _FreqUp = 0;
        [XmlElement]
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        [XmlElement]
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private int _UARFCN_UP = 0;
        [XmlElement]
        public int UARFCN_UP
        {
            get { return _UARFCN_UP; }
            set { _UARFCN_UP = value; OnPropertyChanged("UARFCN_UP"); }
        }
        private int _UARFCN_DN = 0;
        [XmlElement]
        public int UARFCN_DN
        {
            get { return _UARFCN_DN; }
            set { _UARFCN_DN = value; OnPropertyChanged("UARFCN_DN"); }
        }
        private string _StandartSubband = "";
        [XmlElement]
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }

        private bool _Use = false;
        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class UMTSSIBType : INotifyPropertyChanged
    {
        #region 
        private string _SibType;
        [XmlElement]
        public string SibType
        {
            get { return _SibType; }
            set { _SibType = value; OnPropertyChanged("SibType"); }
        }
        private bool _Use;
        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    #endregion
    #region LTE
    [Serializable]
    public class LTE_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public bool TechIsEnabled
        {
            get { return _TechIsEnabled; }
            set { _TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        private bool _TechIsEnabled = false;

        [XmlElement]
        public int TSMWRfInput
        {
            get { return _TSMWRfInput; }
            set
            {
                if (value > 2) _TSMWRfInput = 2;
                else if (value < 1) _TSMWRfInput = 1;
                else _TSMWRfInput = value;
                OnPropertyChanged("TSMWRfInput");
            }
        }
        private int _TSMWRfInput = 1;

        [XmlArray]
        public ObservableCollection<LTEFreqs_Set> Freqs
        {
            get { return _Freqs; }
            set { _Freqs = value; OnPropertyChanged("Freqs"); }
        }
        private ObservableCollection<LTEFreqs_Set> _Freqs = new ObservableCollection<LTEFreqs_Set>() { };

        [XmlArray]
        public ObservableCollection<LTESIBType> SIBTypes
        {
            get { return _SIBTypes; }
            set { _SIBTypes = value; OnPropertyChanged("SIBTypes"); }
        }
        private ObservableCollection<LTESIBType> _SIBTypes = new ObservableCollection<LTESIBType>() { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class LTEFreqs_Set : INotifyPropertyChanged
    {

        private decimal _FreqUp = 0;
        [XmlElement]
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        [XmlElement]
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private int _EARFCN_UP = 0;
        [XmlElement]
        public int EARFCN_UP
        {
            get { return _EARFCN_UP; }
            set { _EARFCN_UP = value; OnPropertyChanged("EARFCN_UP"); }
        }
        private int _EARFCN_DN = 0;
        [XmlElement]
        public int EARFCN_DN
        {
            get { return _EARFCN_DN; }
            set { _EARFCN_DN = value; OnPropertyChanged("EARFCN_DN"); }
        }
        private string _StandartSubband = "";
        [XmlElement]
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }

        private bool _Use = false;
        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class LTESIBType : INotifyPropertyChanged
    {
        #region 
        private string _SibType;
        [XmlElement]
        public string SibType
        {
            get { return _SibType; }
            set { _SibType = value; OnPropertyChanged("SibType"); }
        }
        private bool _Use;
        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    #endregion
    #region CDMA
    [Serializable]
    public class CDMA_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public bool TechIsEnabled
        {
            get { return _TechIsEnabled; }
            set { _TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        private bool _TechIsEnabled = false;

        [XmlElement]
        public int TSMWRfInput
        {
            get { return _TSMWRfInput; }
            set
            {
                if (value > 2) _TSMWRfInput = 2;
                else if (value < 1) _TSMWRfInput = 1;
                else _TSMWRfInput = value;
                OnPropertyChanged("TSMWRfInput");
            }
        }
        private int _TSMWRfInput = 1;

        [XmlArray]
        public ObservableCollection<CDMAFreqs_Set> Freqs
        {
            get { return _Freqs; }
            set { _Freqs = value; OnPropertyChanged("Freqs"); }
        }
        private ObservableCollection<CDMAFreqs_Set> _Freqs = new ObservableCollection<CDMAFreqs_Set>() { };

        [XmlArray]
        public ObservableCollection<CDMASIType> SITypes
        {
            get { return _SITypes; }
            set { _SITypes = value; OnPropertyChanged("SITypes"); }
        }
        private ObservableCollection<CDMASIType> _SITypes = new ObservableCollection<CDMASIType>() { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class CDMAFreqs_Set : INotifyPropertyChanged
    {

        private decimal _FreqUp = 0;
        [XmlElement]
        public decimal FreqUp
        {
            get { return _FreqUp; }
            set { _FreqUp = value; OnPropertyChanged("FreqUp"); }
        }
        private decimal _FreqDn = 0;
        [XmlElement]
        public decimal FreqDn
        {
            get { return _FreqDn; }
            set { _FreqDn = value; OnPropertyChanged("FreqDn"); }
        }
        private int _Channel = 0;
        [XmlElement]
        public int Channel
        {
            get { return _Channel; }
            set { _Channel = value; OnPropertyChanged("Channel"); }
        }
        private string _StandartSubband = "";
        [XmlElement]
        public string StandartSubband
        {
            get { return _StandartSubband; }
            set { _StandartSubband = value; OnPropertyChanged("StandartSubband"); }
        }
        /// <summary>
        /// True = EVDO, False = CDMA
        /// </summary>
        [XmlElement]
        public bool EVDOvsCDMA
        {
            get { return _EVDOvsCDMA; }
            set { _EVDOvsCDMA = value; OnPropertyChanged("EVDOvsCDMA"); }
        }
        private bool _EVDOvsCDMA = false;

        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        private bool _Use = false;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class CDMASIType : INotifyPropertyChanged
    {
        #region 
        private string _SiType;
        [XmlElement]
        public string SiType
        {
            get { return _SiType; }
            set { _SiType = value; OnPropertyChanged("SiType"); }
        }
        private bool _Use;
        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
                                                                  // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    #endregion
    #region ACD
    [Serializable]
    public class ACD_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public bool TechIsEnabled
        {
            get { return _TechIsEnabled; }
            set { _TechIsEnabled = value; OnPropertyChanged("TechIsEnabled"); }
        }
        private bool _TechIsEnabled = false;

        /// <summary>
        /// SIMPLE = 1  SMART = 2
        /// </summary>
        [XmlElement]
        public int MeasurementMode
        {
            get { return _MeasurementMode; }
            set
            {
                if (value > 2) _MeasurementMode = 2;
                else if (value < 1) _MeasurementMode = 1;
                else _MeasurementMode = value;
                OnPropertyChanged("MeasurementMode");
            }
        }
        private int _MeasurementMode = 2;

        /// <summary>
        /// FAIR = 1  GOOD = 2  EXCELLENT = 3
        /// </summary>
        [XmlElement]
        public int Sensitivity
        {
            get { return _Sensitivity; }
            set
            {
                if (value > 3) _Sensitivity = 3;
                else if (value < 1) _Sensitivity = 1;
                else _Sensitivity = value;
                OnPropertyChanged("Sensitivity");
            }
        }
        private int _Sensitivity = 2;

        [XmlArray]
        public ObservableCollection<ACD_DataSet> Data
        {
            get { return _Data; }
            set { _Data = value; OnPropertyChanged("Data"); }
        }
        private ObservableCollection<ACD_DataSet> _Data = new ObservableCollection<ACD_DataSet>() { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class ACD_DataSet : INotifyPropertyChanged
    {
        [XmlElement]
        public int Band
        {
            get { return _Band; }
            set { _Band = value; OnPropertyChanged("Band"); }
        }
        private int _Band = 0;
        [XmlElement]
        public string BandStr
        {
            get { return _BandStr; }
            set { _BandStr = value; OnPropertyChanged("BandStr"); }
        }
        private string _BandStr = "";

        /// <summary>
        /// WCDMA = 2 CDMA = 3 EVDO = 4 LTE = 5
        /// </summary>
        [XmlElement]
        public int Tech
        {
            get { return _Tech; }
            set { _Tech = value; OnPropertyChanged("Tech"); }
        }
        private int _Tech = 0;
        [XmlElement]
        public string TechStr
        {
            get { return _TechStr; }
            set { _TechStr = value; OnPropertyChanged("TechStr"); }
        }
        private string _TechStr = "";

        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        private bool _Use = false;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion ACD
    [Serializable]
    public class TechOption_Set : INotifyPropertyChanged
    {
        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        [XmlElement]
        public int GNSS
        {
            get { return _GNSS; }
            set
            {
                if (value < 0) _GNSS = 0;
                else if (value > 2) _GNSS = 2;
                else _GNSS = value;
                OnPropertyChanged("GNSS");
            }
        }
        private int _GNSS = 0;

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        [XmlElement]
        public int GSM
        {
            get { return _GSM; }
            set
            {
                if (value < 0) _GSM = 0;
                else if (value > 2) _GSM = 2;
                else _GSM = value;
                OnPropertyChanged("GSM");
            }
        }
        private int _GSM = 0;

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        [XmlElement]
        public int UMTS
        {
            get { return _UMTS; }
            set
            {
                if (value < 0) _UMTS = 0;
                else if (value > 2) _UMTS = 2;
                else _UMTS = value;
                OnPropertyChanged("UMTS");
            }
        }
        private int _UMTS = 0;

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        [XmlElement]
        public int LTE
        {
            get { return _LTE; }
            set
            {
                if (value < 0) _LTE = 0;
                else if (value > 2) _LTE = 2;
                else _LTE = value;
                OnPropertyChanged("LTE");
            }
        }
        private int _LTE = 0;

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        [XmlElement]
        public int CDMA
        {
            get { return _CDMA; }
            set
            {
                if (value < 0) _CDMA = 0;
                else if (value > 2) _CDMA = 2;
                else _CDMA = value;
                OnPropertyChanged("CDMA");
            }
        }
        private int _CDMA = 0;

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        [XmlElement]
        public int RFPS
        {
            get { return _RFPS; }
            set
            {
                if (value < 0) _RFPS = 0;
                else if (value > 2) _RFPS = 2;
                else _RFPS = value;
                OnPropertyChanged("RFPS");
            }
        }
        private int _RFPS = 0;

        /// <summary>
        /// 0: не подключались и незнаем 1: подключались и есть 2: подключались и нет
        /// </summary>
        [XmlElement]
        public int ACD
        {
            get { return _ACD; }
            set
            {
                if (value < 0) _ACD = 0;
                else if (value > 2) _ACD = 2;
                else _ACD = value;
                OnPropertyChanged("ACD");
            }
        }
        private int _ACD = 0;
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class Screen_Set : INotifyPropertyChanged
    {
        private string _ScreenFolder = "";
        [XmlElement]
        public string ScreenFolder
        {
            //get { return _ScreenFolder; }
            //set { _ScreenFolder = value; OnPropertyChanged("ScreenFolder"); }
            get
            {
                if (_ScreenFolder != "")
                {
                    if (!Directory.Exists(_ScreenFolder))
                    {
                        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Screens";
                        Directory.CreateDirectory(path);
                        _ScreenFolder = path;
                    }
                    return _ScreenFolder;
                }
                else
                {
                    string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Screens";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    _ScreenFolder = path;
                    return _ScreenFolder;
                }
            }
            set
            {
                if (value == "")
                {
                    string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Screens";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    _ScreenFolder = path;
                }
                else _ScreenFolder = value;
                OnPropertyChanged("ScreenFolder");
            }
        }
        private string _SaveScreenImageFormat = "Png";
        [XmlElement]
        public string SaveScreenImageFormat
        {
            get { return _SaveScreenImageFormat; }
            set { _SaveScreenImageFormat = value; OnPropertyChanged("SaveScreenImageFormat"); }
        }
        private string _ScreenResolution = "";
        [XmlElement]
        public string ScreenResolution
        {
            get { return _ScreenResolution; }
            set { _ScreenResolution = value; OnPropertyChanged("ScreenResolution"); }
        }
        private bool _SaveScreenFromInstr = false;
        [XmlElement]
        public bool SaveScreenFromInstr
        {
            get { return _SaveScreenFromInstr; }
            set { _SaveScreenFromInstr = value; OnPropertyChanged("SaveScreenFromInstr"); }
        }
        private string _SMBHost = "";
        [XmlElement]
        public string SMBHost
        {
            get { return _SMBHost; }
            set { _SMBHost = value; OnPropertyChanged("SMBHost"); }
        }
        private string _SMBHostUser = "";
        [XmlElement]
        public string SMBHostUser
        {
            get { return _SMBHostUser; }
            set { _SMBHostUser = value; OnPropertyChanged("SMBHostUser"); }
        }
        private string _SMBHostPass = "";
        [XmlElement]
        public string SMBHostPass
        {
            get { return _SMBHostPass; }
            set { _SMBHostPass = value; OnPropertyChanged("SMBHostPass"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(Screen_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class GlogalApps_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public string UILanguage
        {
            get { return _UILanguage; }
            set { _UILanguage = value; OnPropertyChanged("UILanguage"); }
        }
        private string _UILanguage = "en-US";

        [XmlElement]
        public int SelectedStyle
        {
            get { return _SelectedStyle; }
            set { /*if (value == 0 || value == 1) {*/ _SelectedStyle = value; OnPropertyChanged("SelectedStyle");/* }*/ }
        }
        private int _SelectedStyle = 0;


        private double _MainWindowLeft = 0;
        [XmlElement]
        public double MainWindowLeft
        {
            get
            {
                //System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                double d = _MainWindowLeft;
                if ((_MainWindowLeft + _MainWindowWidth < 100) || (System.Windows.SystemParameters.WorkArea.Width - _MainWindowLeft > 100)) d = 100;
                return d;
            }
            set { _MainWindowLeft = value; OnPropertyChanged("MainWindowLeft"); }
        }
        private double _MainWindowTop = 0;
        [XmlElement]
        public double MainWindowTop
        {
            get
            {
                double d = _MainWindowTop;
                if ((_MainWindowTop + _MainWindowHeight < 100) || (System.Windows.SystemParameters.WorkArea.Height - _MainWindowTop > 200)) d = 100;
                return d;
            }
            set { _MainWindowTop = value; OnPropertyChanged("MainWindowTop"); }
        }
        private double _MainWindowWidth = 0;
        [XmlElement]
        public double MainWindowWidth
        {
            get { return _MainWindowWidth; }
            set { _MainWindowWidth = value; OnPropertyChanged("MainWindowWidth"); }
        }
        private double _MainWindowHeight = 0;
        [XmlElement]
        public double MainWindowHeight
        {
            get { return _MainWindowHeight; }
            set { _MainWindowHeight = value; OnPropertyChanged("MainWindowHeight"); }
        }

        [XmlElement]
        public int DefaultSelectedTabItem
        {
            get { return _DefaultSelectedTabItem; }
            set { _DefaultSelectedTabItem = value; OnPropertyChanged("DefaultSelectedTabItem"); }
        }
        private int _DefaultSelectedTabItem = 0;


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(GlogalApps_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }

    [Serializable]
    public class UsersApps_Set : INotifyPropertyChanged
    {
        [XmlIgnore]
        public string FileName
        {
            get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Settings/" + "Users_Settings.xml"; }
            private set { }
        }
        [XmlIgnore]
        private UserApps_Set _SelectedUser;//= new ObservableCollection<UserApps_Set>();
        [XmlElement]
        public UserApps_Set SelectedUser //{ get ; set; }
        {
            get { return _SelectedUser; }
            set { _SelectedUser = value; OnPropertyChanged("SelectedUser"); }
        }
        [XmlIgnore]
        private ObservableCollection<UserApps_Set> _Users = new ObservableCollection<UserApps_Set>();
        [XmlArray]
        public ObservableCollection<UserApps_Set> Users //{ get ; set; }
        {
            get { return _Users; }
            set { _Users = value; OnPropertyChanged("Users"); }
        }


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(UsersApps_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class UserApps_Set : INotifyPropertyChanged
    {
        private int _ID = -1;
        [XmlElement(IsNullable = false)]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged("ID"); }
        }
        private int _SYS_ID = -1;
        [XmlElement(IsNullable = false)]
        public int SYS_ID
        {
            get { return _SYS_ID; }
            set { _SYS_ID = value; OnPropertyChanged("SYS_ID"); }
        }
        private string _FIRST_NAME = "Имя";
        /// <summary>
        /// Имя
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string FIRST_NAME
        {
            get { return _FIRST_NAME; }
            set
            {
                _FIRST_NAME = value;
                if (FIRST_NAME.Length > 0 && SECOND_NAME.Length > 0) { Initials = FIRST_NAME.Substring(0, 1) + "." + SECOND_NAME.Substring(0, 1) + "."; }
                OnPropertyChanged("FIRST_NAME");
            }
        }
        private string _LAST_NAME = "Фамилия";
        /// <summary>
        /// Фамилия
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string LAST_NAME
        {
            get { return _LAST_NAME; }
            set { _LAST_NAME = value; OnPropertyChanged("LAST_NAME"); }
        }
        private string _SECOND_NAME = "Отчество";
        /// <summary>
        /// Отчество
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string SECOND_NAME
        {
            get { return _SECOND_NAME; }
            set
            {
                _SECOND_NAME = value;
                if (FIRST_NAME.Length > 0 && SECOND_NAME.Length > 0) { Initials = FIRST_NAME.Substring(0, 1) + "." + SECOND_NAME.Substring(0, 1) + "."; }
                OnPropertyChanged("SECOND_NAME");
            }
        }
        private string _Initials = "";
        /// <summary>
        /// Инициалы
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string Initials
        {
            get { return _Initials; }
            set { _Initials = value; OnPropertyChanged("Initials"); }
        }

        private string _POSITION = "Инженер № категории";
        /// <summary>
        /// Отчество
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string POSITION
        {
            get { return _POSITION; }
            set { _POSITION = value; OnPropertyChanged("POSITION"); }
        }
        private string _Pass = "Pass";
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(IsNullable = false)]
        public string Pass
        {
            get { return _Pass; }
            set { _Pass = value; OnPropertyChanged("Pass"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class DB_Set : INotifyPropertyChanged
    {
        private double _RadiusFind = 0;
        [XmlElement]
        public double RadiusFind
        {
            get { return _RadiusFind; }
            set { _RadiusFind = value; OnPropertyChanged("RadiusFind"); }
        }
        private bool _CheckRadiusFindState = false;
        [XmlElement]
        public bool CheckRadiusFindState
        {
            get { return _CheckRadiusFindState; }
            set { _CheckRadiusFindState = value; OnPropertyChanged("CheckRadiusFindState"); }
        }
        private bool _CoorChangedFromGPS = false;
        [XmlElement]
        public bool CoorChangedFromGPS
        {
            get { return _CoorChangedFromGPS; }
            set { _CoorChangedFromGPS = value; OnPropertyChanged("CoorChangedFromGPS"); }
        }
        private bool _CheckFilterFromGPS = false;
        [XmlElement]
        public bool CheckFilterFromGPS
        {
            get { return _CheckFilterFromGPS; }
            set { _CheckFilterFromGPS = value; OnPropertyChanged("CheckFilterFromGPS"); }
        }
        private bool _DataFromPlan = false;
        [XmlElement]
        public bool DataFromPlan
        {
            get { return _DataFromPlan; }
            set { _DataFromPlan = value; OnPropertyChanged("DataFromPlan"); }
        }
        private bool _DataFromPermission = false;
        [XmlElement]
        public bool DataFromPermission
        {
            get { return _DataFromPermission; }
            set { _DataFromPermission = value; OnPropertyChanged("DataFromPermission"); }
        }
        private bool _FreqFilter = false;
        [XmlElement]
        public bool FreqFilter
        {
            get { return _FreqFilter; }
            set { _FreqFilter = value; OnPropertyChanged("FreqFilter"); }
        }
        private double _MinFreqFilter = 0;
        [XmlElement]
        public double MinFreqFilter
        {
            get { return _MinFreqFilter; }
            set { _MinFreqFilter = value; MinFreqFilterStr = (value / 1000000).ToString(); OnPropertyChanged("MinFreqFilter"); }
        }
        private string _MinFreqFilterStr = string.Empty;
        [XmlElement]
        public string MinFreqFilterStr
        {
            get { return _MinFreqFilterStr; }
            set { _MinFreqFilterStr = value; OnPropertyChanged("MinFreqFilterStr"); }
        }
        private double _MaxFreqFilter = 0;
        [XmlElement]
        public double MaxFreqFilter
        {
            get { return _MaxFreqFilter; }
            set { _MaxFreqFilter = value; MaxFreqFilterStr = (value / 1000000).ToString(); OnPropertyChanged("MaxFreqFilter"); }
        }
        private string _MaxFreqFilterStr = string.Empty;
        [XmlElement]
        public string MaxFreqFilterStr
        {
            get { return _MaxFreqFilterStr; }
            set { _MaxFreqFilterStr = value; OnPropertyChanged("MaxFreqFilterStr"); }
        }


        private bool _CheckFreqFilter = false;
        [XmlElement]
        public bool CheckFreqFilter
        {
            get { return _CheckFreqFilter; }
            set { _CheckFreqFilter = value; OnPropertyChanged("CheckFreqFilter"); }
        }

        //private ObservableCollection<CheckTechnologies_Set> _CheckTechnologiesFilter = new ObservableCollection<CheckTechnologies_Set>() { };
        //[XmlElement]
        //public ObservableCollection<CheckTechnologies_Set> CheckTechnologiesFilter
        //{
        //    get { return _CheckTechnologiesFilter; }
        //    set { _CheckTechnologiesFilter = value; OnPropertyChanged("CheckTechnologiesFilter"); }
        //}
        private bool _CheckStatusFilterState = false;
        [XmlElement]
        public bool CheckStatusFilterState
        {
            get { return _CheckStatusFilterState; }
            set { _CheckStatusFilterState = value; OnPropertyChanged("CheckStatusFilterState"); }
        }
        private ObservableCollection<CheckStatus_Set> _CheckStatusFilter = new ObservableCollection<CheckStatus_Set>() { };
        [XmlElement]
        public ObservableCollection<CheckStatus_Set> CheckStatusFilter
        {
            get { return _CheckStatusFilter; }
            set { _CheckStatusFilter = value; OnPropertyChanged("CheckStatusFilter"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(DB_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

    }
    [Serializable]
    public class CheckTechnologies_Set : INotifyPropertyChanged
    {
        private string _Technologies = "";
        [XmlElement]
        public string Technologies
        {
            get { return _Technologies; }
            set { _Technologies = value; OnPropertyChanged("Technologies"); }
        }
        private bool _IsSelected = false;
        [XmlElement]
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class CheckStatus_Set : INotifyPropertyChanged
    {
        private string _Status = "";
        [XmlElement]
        public string Status
        {
            get { return _Status; }
            set { _Status = value; OnPropertyChanged("Status"); }
        }
        private string _StatusInDB = "";
        [XmlElement]
        public string StatusInDB
        {
            get { return _StatusInDB; }
            set { _StatusInDB = value; OnPropertyChanged("StatusInDB"); }
        }
        private bool _IsSelected = false;
        [XmlElement]
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class Map_Set : INotifyPropertyChanged
    {
        private string _MapPath = "";
        [XmlElement]
        public string MapPath
        {
            get
            {
                if (_MapPath != "")
                {
                    if (!Directory.Exists(_MapPath))
                    {
                        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\OSMMAP";
                        Directory.CreateDirectory(path);
                        _MapPath = path;
                    }
                    return _MapPath;
                }
                else
                {
                    string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\OSMMAP";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    _MapPath = path;
                    return _MapPath;
                }
            }
            set
            {
                if (value == "")
                {
                    string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\OSMMAP";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    _MapPath = path;
                }
                else _MapPath = value;
                OnPropertyChanged("MapPath");
            }
        }
        private bool _SaveMapPosition = false;
        [XmlElement]
        public bool SaveMapPosition
        {
            get { return _SaveMapPosition; }
            set { _SaveMapPosition = value; OnPropertyChanged("SaveMapPosition"); }
        }

        [XmlElement]
        public int PrintScreenType
        {
            get { return _PrintScreenType; }
            set { _PrintScreenType = value; OnPropertyChanged("PrintScreenType"); }
        }
        private int _PrintScreenType = 0;

        private ObservableCollection<AnyMap_Set> _ControlsSettings = new ObservableCollection<AnyMap_Set>() { };
        [XmlArray]
        public ObservableCollection<AnyMap_Set> ControlsSettings
        {
            get { return _ControlsSettings; }
            set { _ControlsSettings = value; OnPropertyChanged("ControlsSettings"); }
        }
        private ObservableCollection<DataGridHeaderVisibility> _ATDI_TooltipColumnChooserMenu = new ObservableCollection<DataGridHeaderVisibility>() { };
        [XmlArray]
        public ObservableCollection<DataGridHeaderVisibility> ATDI_TooltipColumnChooserMenu
        {
            get { return _ATDI_TooltipColumnChooserMenu; }
            set { _ATDI_TooltipColumnChooserMenu = value; OnPropertyChanged("ATDI_TooltipColumnChooserMenu"); }
        }
        //private ObservableCollection<AnyMap_Set> _AllMaps = new ObservableCollection<AnyMap_Set>() { };
        //[XmlArray]
        //public ObservableCollection<AnyMap_Set> AllMaps
        //{
        //    get { return _AllMaps; }
        //    set { _AllMaps = value; OnPropertyChanged("AllMaps"); }
        //}
        //public AnyMap_Set GetSetiings(string ControlName)
        //{
        //    AnyMap_Set t = new AnyMap_Set() { };
        //    bool find = false;
        //    //ищем 
        //    for (int i = 0; i < AllMaps.Count; i++)
        //    {
        //        if (AllMaps[i].MapsControlName == ControlName)
        //        {
        //            t = AllMaps[i];
        //            find = true;
        //        }
        //    }
        //    if (find == false)
        //    {
        //        t.MapsControlName = ControlName;
        //        AllMaps.Add(t);
        //    }
        //    return t;
        //}

        //private AnyMap_Set _DVBMap = new AnyMap_Set() { };
        //[XmlElement]
        //public AnyMap_Set DVBMap
        //{
        //    get { return _DVBMap; }
        //    set { _DVBMap = value; OnPropertyChanged("DVBMap"); }
        //}

        //private double _RRSMapsInitialX = 0;
        //[XmlElement]
        //public double RRSMapsInitialX
        //{
        //    get { return _RRSMapsInitialX; }
        //    set { _RRSMapsInitialX = value; OnPropertyChanged("RRSMapsInitialX"); }
        //}
        //private double _RRSMapsInitialY = 0;
        //[XmlElement]
        //public double RRSMapsInitialY
        //{
        //    get { return _RRSMapsInitialY; }
        //    set { _RRSMapsInitialY = value; OnPropertyChanged("RRSMapsInitialY"); }
        //}
        //private double _RRSMapsInitialScale = 0;
        //[XmlElement]
        //public double RRSMapsInitialScale
        //{
        //    get { return _RRSMapsInitialScale; }
        //    set { _RRSMapsInitialScale = value; OnPropertyChanged("RRSMapsInitialScale"); }
        //}
        //private double _GlobalMapsInitialX = 0;
        //[XmlElement]
        //public double GlobalMapsInitialX
        //{
        //    get { return _GlobalMapsInitialX; }
        //    set { _GlobalMapsInitialX = value; OnPropertyChanged("GlobalMapsInitialX"); }
        //}
        //private double _GlobalMapsInitialY = 0;
        //[XmlElement]
        //public double GlobalMapsInitialY
        //{
        //    get { return _GlobalMapsInitialY; }
        //    set { _GlobalMapsInitialY = value; OnPropertyChanged("GlobalMapsInitialY"); }
        //}
        //private double _GlobalMapsInitialScale = 0;
        //[XmlElement]
        //public double GlobalMapsInitialScale
        //{
        //    get { return _GlobalMapsInitialScale; }
        //    set { _GlobalMapsInitialScale = value; OnPropertyChanged("GlobalMapsInitialScale"); }
        //}

        //private double _DVBMapsInitialX = 0;
        //[XmlElement]
        //public double DVBMapsInitialX
        //{
        //    get { return _DVBMapsInitialX; }
        //    set { _DVBMapsInitialX = value; OnPropertyChanged("DVBMapsInitialX"); }
        //}
        //private double _DVBMapsInitialY = 0;
        //[XmlElement]
        //public double DVBMapsInitialY
        //{
        //    get { return _DVBMapsInitialY; }
        //    set { _DVBMapsInitialY = value; OnPropertyChanged("DVBMapsInitialY"); }
        //}
        //private double _DVBMapsInitialScale = 0;
        //[XmlElement]
        //public double DVBMapsInitialScale
        //{
        //    get { return _DVBMapsInitialScale; }
        //    set { _DVBMapsInitialScale = value; OnPropertyChanged("DVBMapsInitialScale"); }
        //}


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(Map_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class AnyMap_Set : INotifyPropertyChanged
    {
        private string _MapsControlName = "";
        [XmlElement]
        public string MapsControlName
        {
            get { return _MapsControlName; }
            set { _MapsControlName = value; OnPropertyChanged("MapsControlName"); }
        }
        private double _MapsInitialX = 0;
        [XmlElement]
        public double MapsInitialX
        {
            get { return _MapsInitialX; }
            set { _MapsInitialX = value; OnPropertyChanged("MapsInitialX"); }
        }
        private double _MapsInitialY = 0;
        [XmlElement]
        public double MapsInitialY
        {
            get { return _MapsInitialY; }
            set { _MapsInitialY = value; OnPropertyChanged("MapsInitialY"); }
        }
        private int _MapsInitialScale = 0;
        [XmlElement]
        public int MapsInitialScale
        {
            get { return _MapsInitialScale; }
            set { _MapsInitialScale = value; OnPropertyChanged("MapsInitialScale"); }
        }
        private bool _ShowRadiusOnSearchState = false;
        [XmlElement]
        public bool ShowRadiusOnSearchState
        {
            get { return _ShowRadiusOnSearchState; }
            set { _ShowRadiusOnSearchState = value; OnPropertyChanged("ShowRadiusState"); }
        }
        private bool _ShowMyLocation = false;
        [XmlElement]
        public bool ShowMyLocation
        {
            get { return _ShowMyLocation; }
            set { _ShowMyLocation = value; OnPropertyChanged("ShowMyLocation"); }
        }
        private bool _AutoPanModeMap = false;
        [XmlElement]
        public bool AutoPanModeMap
        {
            get { return _AutoPanModeMap; }
            set { _AutoPanModeMap = value; OnPropertyChanged("AutoPanModeMap"); }
        }

        private bool _ShowPlanPanelExpander = false;
        [XmlElement]
        public bool ShowPlanPanelExpander
        {
            get { return _ShowPlanPanelExpander; }
            set { _ShowPlanPanelExpander = value; OnPropertyChanged("ShowPlanPanelExpander"); }
        }
        private bool _ShowInfoWithRadiusPanelExpander = false;
        [XmlElement]
        public bool ShowInfoWithRadiusPanelExpander
        {
            get { return _ShowInfoWithRadiusPanelExpander; }
            set { _ShowInfoWithRadiusPanelExpander = value; OnPropertyChanged("ShowInfoWithRadiusPanelExpander"); }
        }

        [XmlElement]
        public bool ShowSelectedInfoPanelExpander
        {
            get { return _ShowSelectedInfoPanelExpander; }
            set { _ShowSelectedInfoPanelExpander = value; OnPropertyChanged("ShowSelectedInfoPanelExpander"); }
        }
        private bool _ShowSelectedInfoPanelExpander = false;

        [XmlElement]
        public double SelectedInfoPanelWidth
        {
            get { return _SelectedInfoPanelWidth; }
            set { if (value > 100 && value < 1000) { _SelectedInfoPanelWidth = value; OnPropertyChanged("SelectedInfoPanelWidth"); } }
        }
        private double _SelectedInfoPanelWidth = 100;


        [XmlElement]
        public double MapOpacity
        {
            get { return _MapOpacity; }
            set { _MapOpacity = value; OnPropertyChanged("MapOpacity"); }
        }
        private double _MapOpacity = 1;

        [XmlElement]
        public double LayerOpacity
        {
            get { return _LayerOpacity; }
            set { _LayerOpacity = value; OnPropertyChanged("LayerOpacity"); }
        }
        private double _LayerOpacity = 1;

        [XmlElement]
        public double RadiusLayerOpacity
        {
            get { return _RadiusLayerOpacity; }
            set { _RadiusLayerOpacity = value; OnPropertyChanged("RadiusLayerOpacity"); }
        }
        private double _RadiusLayerOpacity = 1;

        private ObservableCollection<DataGridHeaderVisibility> _ATDI_StationInfoColumnChooserMenu = new ObservableCollection<DataGridHeaderVisibility>() { };
        [XmlArray]
        public ObservableCollection<DataGridHeaderVisibility> ATDI_StationInfoColumnChooserMenu
        {
            get { return _ATDI_StationInfoColumnChooserMenu; }
            set { _ATDI_StationInfoColumnChooserMenu = value; OnPropertyChanged("ATDI_StationInfoColumnChooserMenu"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class DataGridHeaderVisibility : INotifyPropertyChanged
    {
        private string _Name = "Name";
        [XmlElement]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged("Name"); }
        }
        private string _VariableName = "VariableName";
        [XmlElement]
        public string VariableName
        {
            get { return _VariableName; }
            set { _VariableName = value; OnPropertyChanged("VariableName"); }
        }
        private bool _Visible = true;
        [XmlElement]
        public bool Visible
        {
            get { return _Visible; }// (col.Visibility == Visibility.Visible) ? true : false; }
            set
            {
                _Visible = value;
                if (col != null)
                {
                    if (_Visible == true) col.Visibility = Visibility.Visible;
                    else col.Visibility = Visibility.Collapsed;
                }
                OnPropertyChanged("Visible");
            }
        }
        [XmlIgnore]
        public DataGridColumn col { get; set; } = null;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class AnUserPresets_Set : INotifyPropertyChanged
    {
        private ObservableCollection<UserPresetType_Set> _UserPresets = new ObservableCollection<UserPresetType_Set>() { };
        [XmlElement]
        public ObservableCollection<UserPresetType_Set> UserPresets
        {
            get { return _UserPresets; }
            set { _UserPresets = value; OnPropertyChanged("UserPresets"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(AnUserPresets_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class UserPresetType_Set : INotifyPropertyChanged
    {
        private string _UserPresetName = "UserPresetName";
        [XmlElement]
        public string UserPresetName
        {
            get { return _UserPresetName; }
            set { _UserPresetName = value; OnPropertyChanged("UserPresetName"); }
        }
        private int _InstrManufacrure = 0;
        [XmlElement]
        public int InstrManufacrure
        {
            get { return _InstrManufacrure; }
            set { _InstrManufacrure = value; OnPropertyChanged("InstrManufacrure"); }
        }
        private string _InstrModel = "";
        [XmlElement]
        public string InstrModel
        {
            get { return _InstrModel; }
            set { _InstrModel = value; OnPropertyChanged("InstrModel"); }
        }
        //private double _FreqCentr = 0;
        //[XmlElement]
        //public double FreqCentr
        //{
        //    get { return _FreqCentr; }
        //    set { _FreqCentr = value; OnPropertyChanged("FreqCentr"); }
        //}
        //private double _FreqSpan = 0;
        //[XmlElement]
        //public double FreqSpan
        //{
        //    get { return _FreqSpan; }
        //    set { _FreqSpan = value; OnPropertyChanged("FreqSpan"); }
        //}
        private double _FreqStart = 0;
        [XmlElement]
        public double FreqStart
        {
            get { return _FreqStart; }
            set { _FreqStart = value; OnPropertyChanged("FreqStart"); }
        }
        private double _FreqStop = 0;
        [XmlElement]
        public double FreqStop
        {
            get { return _FreqStop; }
            set { _FreqStop = value; OnPropertyChanged("FreqStop"); }
        }
        private double _RBW = 0;
        [XmlElement]
        public double RBW
        {
            get { return _RBW; }
            set { _RBW = value; OnPropertyChanged("RBW"); }
        }
        private bool _AutoRBW = false;
        [XmlElement]
        public bool AutoRBW
        {
            get { return _AutoRBW; }
            set { _AutoRBW = value; OnPropertyChanged("AutoRBW"); }
        }
        private double _VBW = 0;
        [XmlElement]
        public double VBW
        {
            get { return _VBW; }
            set { _VBW = value; OnPropertyChanged("VBW"); }
        }
        private bool _AutoVBW = false;
        [XmlElement]
        public bool AutoVBW
        {
            get { return _AutoVBW; }
            set { _AutoVBW = value; OnPropertyChanged("AutoVBW"); }
        }
        private double _SWT = 0;
        [XmlElement]
        public double SWT
        {
            get { return _SWT; }
            set { _SWT = value; OnPropertyChanged("SWT"); }
        }
        private bool _AutoSWT = false;
        [XmlElement]
        public bool AutoSWT
        {
            get { return _AutoSWT; }
            set { _AutoSWT = value; OnPropertyChanged("AutoSWT"); }
        }
        private int _SweepType = 0;
        [XmlElement]
        public int SweepType
        {
            get { return _SweepType; }
            set { _SweepType = value; OnPropertyChanged("SweepType"); }
        }
        private int _SweepPoints = 0;
        [XmlElement]
        public int SweepPoints
        {
            get { return _SweepPoints; }
            set { _SweepPoints = value; OnPropertyChanged("SweepPoints"); }
        }
        private double _RefLevel = 0;
        [XmlElement]
        public double RefLevel
        {
            get { return _RefLevel; }
            set { _RefLevel = value; OnPropertyChanged("RefLevel"); }
        }
        private double _Range = 0;
        [XmlElement]
        public double Range
        {
            get { return _Range; }
            set { _Range = value; OnPropertyChanged("Range"); }
        }
        private int _LevelUnit = 0;
        [XmlElement]
        public int LevelUnit
        {
            get { return _LevelUnit; }
            set { _LevelUnit = value; OnPropertyChanged("LevelUnit"); }
        }
        private double _AttLevel = 0;
        [XmlElement]
        public double AttLevel
        {
            get { return _AttLevel; }
            set { _AttLevel = value; OnPropertyChanged("AttLevel"); }
        }
        private bool _AttAuto = false;
        [XmlElement]
        public bool AttAuto
        {
            get { return _AttAuto; }
            set { _AttAuto = value; OnPropertyChanged("AttAuto"); }
        }
        private bool _PreAmp = false;
        [XmlElement]
        public bool PreAmp
        {
            get { return _PreAmp; }
            set { _PreAmp = value; OnPropertyChanged("PreAmp"); }
        }
        private Equipment.Analyzer.TrType _Trace1Type = new Equipment.Analyzer.TrType();
        [XmlElement]
        public Equipment.Analyzer.TrType Trace1Type
        {
            get { return _Trace1Type; }
            set { _Trace1Type = value; OnPropertyChanged("Trace1Type"); }
        }
        private Equipment.Analyzer.TrType _Trace2Type = new Equipment.Analyzer.TrType();
        [XmlElement]
        public Equipment.Analyzer.TrType Trace2Type
        {
            get { return _Trace2Type; }
            set { _Trace2Type = value; OnPropertyChanged("Trace2Type"); }
        }
        private Equipment.Analyzer.TrType _Trace3Type = new Equipment.Analyzer.TrType();
        [XmlElement]
        public Equipment.Analyzer.TrType Trace3Type
        {
            get { return _Trace3Type; }
            set { _Trace3Type = value; OnPropertyChanged("Trace3Type"); }
        }

        private Equipment.Analyzer.TrDetector _Trace1Detector = new Equipment.Analyzer.TrDetector();
        [XmlElement]
        public Equipment.Analyzer.TrDetector Trace1Detector
        {
            get { return _Trace1Detector; }
            set { _Trace1Detector = value; OnPropertyChanged("Trace1Detector"); }
        }
        private Equipment.Analyzer.TrDetector _Trace2Detector = new Equipment.Analyzer.TrDetector();
        [XmlElement]
        public Equipment.Analyzer.TrDetector Trace2Detector
        {
            get { return _Trace2Detector; }
            set { _Trace2Detector = value; OnPropertyChanged("Trace2Detector"); }
        }
        private Equipment.Analyzer.TrDetector _Trace3Detector = new Equipment.Analyzer.TrDetector();
        [XmlElement]
        public Equipment.Analyzer.TrDetector Trace3Detector
        {
            get { return _Trace3Detector; }
            set { _Trace3Detector = value; OnPropertyChanged("Trace3Detector"); }
        }
        private int _AveragingCount = 0;
        [XmlElement]
        public int AveragingCount
        {
            get { return _AveragingCount; }
            set { _AveragingCount = value; OnPropertyChanged("AveragingCount"); }
        }



        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class Template_Set : INotifyPropertyChanged
    {
        private DVBTemplate_Set _DVB_T = new DVBTemplate_Set();
        [XmlElement]
        public DVBTemplate_Set DVB_T
        {
            get { return _DVB_T; }
            set { _DVB_T = value; OnPropertyChanged("DVB_T"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(Template_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class DVBTemplate_Set : INotifyPropertyChanged
    {
        private int _MeasDevice = 0;
        [XmlElement]
        public int MeasDevice
        {
            get { return _MeasDevice; }
            set { _MeasDevice = value; OnPropertyChanged("MeasDevice"); }
        }
        private decimal _SygmaBW = 0;
        [XmlElement]
        public decimal SygmaBW
        {
            get { return _SygmaBW; }
            set { _SygmaBW = value; OnPropertyChanged("SygmaBW"); OnPropertyChanged("SygmaBWStr"); }
        }
        private decimal _PowerBW = 0;
        [XmlElement]
        public decimal PowerBW
        {
            get { return _PowerBW; }
            set { _PowerBW = value; OnPropertyChanged("PowerBW"); OnPropertyChanged("PowerBWStr"); }
        }
        private string _PermitionFileICSM = string.Empty;
        [XmlElement]
        public string PermitionFileICSM
        {
            get { return _PermitionFileICSM; }
            set { _PermitionFileICSM = value; OnPropertyChanged("PermitionFileICSM"); }
        }
        private string _DotsFile = string.Empty;
        [XmlElement]
        public string DotsFile
        {
            get { return _DotsFile; }
            set { _DotsFile = value; OnPropertyChanged("DotsFile"); }
        }
        private string _AnalogTV = "";
        [XmlElement]
        public string AnalogTV
        {
            get { return _AnalogTV; }
            set { _AnalogTV = value; OnPropertyChanged("AnalogTV"); }
        }
        private string _DigitalTV = "";
        [XmlElement]
        public string DigitalTV
        {
            get { return _DigitalTV; }
            set { _DigitalTV = value; OnPropertyChanged("DigitalTV"); }
        }
        private string _NoImage = "";
        [XmlElement]
        public string NoImage
        {
            get { return _NoImage; }
            set { _NoImage = value; OnPropertyChanged("NoImage"); }
        }
        private bool _CreateFolderFoto = false;
        [XmlElement]
        public bool CreateFolderFoto
        {
            get { return _CreateFolderFoto; }
            set { _CreateFolderFoto = value; OnPropertyChanged("CreateFolderFoto"); }
        }
        private bool _PlaySoundAfterStop = false;
        [XmlElement]
        public bool PlaySoundAfterStop
        {
            get { return _PlaySoundAfterStop; }
            set { _PlaySoundAfterStop = value; OnPropertyChanged("PlaySoundAfterStop"); }
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    [Serializable]
    public class MeasMons_Set : INotifyPropertyChanged
    {
        /// <summary>
        /// 0 Channel; 1 Band
        /// </summary>
        [XmlElement]
        public int Mode
        {
            get { return _Mode; }
            set { _Mode = value; OnPropertyChanged("Mode"); }
        }
        private int _Mode = 0;

        /// <summary>
        /// 0 Not selected, 1 Spectrum Analyzer, 2 R&S Receiver, 3 R&S Network Analyzer TSMx
        /// </summary>
        [XmlElement]
        public int SpectrumMeasDeviece
        {
            get { return _SpectrumMeasDeviece; }
            set { _SpectrumMeasDeviece = value; OnPropertyChanged("SpectrumMeasDeviece"); }
        }
        private int _SpectrumMeasDeviece = 0;

        [XmlElement]
        public int IdentificationDeviece
        {
            get { return _IdentificationDeviece; }
            set { _IdentificationDeviece = value; OnPropertyChanged("IdentificationDeviece"); }
        }
        private int _IdentificationDeviece = 0;

        [XmlElement]
        public int IdentificationDevieceWRLS
        {
            get { return _IdentificationDevieceWRLS; }
            set { _IdentificationDevieceWRLS = value; OnPropertyChanged("IdentificationDevieceWRLS"); }
        }
        private int _IdentificationDevieceWRLS = 0;

        [XmlElement]
        public MeasMonTech_Set GSM
        {
            get { return _GSM; }
            set { _GSM = value; OnPropertyChanged("GSM"); }
        }
        private MeasMonTech_Set _GSM = new MeasMonTech_Set()
        {
            Techonology = DB.Technologys.GSM,
            DetectionLevel = -100,
            #region
            Data = new MeasMonData_Set[]
            {
                new MeasMonData_Set()
                {
                    BW = 200000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 20,
                    MeasBW = 500000,
                    BWLimit = 350000,
                    RBW = 312.5m,
                    VBW = 312.5m,
                    NdBBWMin = 150000,
                    NdBBWMax = 400000,
                    MarPeakBW = 200000,
                    TracePoints = 1601
                }
            }
            #endregion
        };
        [XmlElement]
        public MeasMonTech_Set UMTS
        {
            get { return _UMTS; }
            set { _UMTS = value; OnPropertyChanged("UMTS"); }
        }
        private MeasMonTech_Set _UMTS = new MeasMonTech_Set()
        {
            Techonology = DB.Technologys.UMTS,
            DetectionLevel = -100,
            #region
            Data = new MeasMonData_Set[]
            {
                new MeasMonData_Set()
                {
                    BW = 5000000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 5,
                    MeasBW = 5000000,
                    BWLimit = 4850000,
                    RBW = 3125,
                    VBW = 3125,
                    NdBBWMin = 4000000,
                    NdBBWMax = 4850000,
                    MarPeakBW = 4800000,
                    TracePoints = 1601
                }
            }
            #endregion
        };
        [XmlElement]
        public MeasMonTech_Set CDMA
        {
            get { return _CDMA; }
            set { _CDMA = value; OnPropertyChanged("CDMA"); }
        }
        private MeasMonTech_Set _CDMA = new MeasMonTech_Set()
        {
            Techonology = DB.Technologys.CDMA,
            DetectionLevel = -100,
            #region
            Data = new MeasMonData_Set[]
            {
                new MeasMonData_Set()
                {
                    BW = 1250000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 0.05m,
                    MeasBW = 2000000,
                    BWLimit = 1300000,
                    RBW = 1250,
                    VBW = 1250,
                    NdBBWMin = 1100000,
                    NdBBWMax = 1300000,
                    MarPeakBW = 1250000,
                    TracePoints = 1601
                }
            }
            #endregion
        };
        [XmlElement]
        public MeasMonTech_Set LTE
        {
            get { return _LTE; }
            set { _LTE = value; OnPropertyChanged("LTE"); }
        }
        private MeasMonTech_Set _LTE = new MeasMonTech_Set()
        {
            Techonology = DB.Technologys.LTE,
            DetectionLevel = -100,
            #region
            Data = new MeasMonData_Set[]
            {
                new MeasMonData_Set()
                {
                    BW = 1400000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 20,
                    MeasBW = 2000000,
                    BWLimit = 1358000,
                    RBW = 1250,
                    VBW = 1250,
                    NdBBWMin = 1190000,
                    NdBBWMax = 1372000,
                    MarPeakBW = 1330000,
                    TracePoints = 1601
                },
                new MeasMonData_Set()
                {
                    BW = 3000000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 20,
                    MeasBW = 3000000,
                    BWLimit = 2910000,
                    RBW = 3125,
                    VBW = 3125,
                    NdBBWMin = 2550000,
                    NdBBWMax = 2940000,
                    MarPeakBW = 2850000,
                    TracePoints = 1601
                },
                new MeasMonData_Set()
                {
                    BW = 5000000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 20,
                    MeasBW = 5000000,
                    BWLimit = 4850000,
                    RBW = 3125,
                    VBW = 3125,
                    NdBBWMin = 4250000,
                    NdBBWMax = 4900000,
                    MarPeakBW = 4750000,
                    TracePoints = 1601
                },
                new MeasMonData_Set()
                {
                    BW = 10000000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 20,
                    MeasBW = 10000000,
                    BWLimit = 9700000,
                    RBW = 6250,
                    VBW = 6250,
                    NdBBWMin = 8500000,
                    NdBBWMax = 9800000,
                    MarPeakBW = 9500000,
                    TracePoints = 1601
                },
                new MeasMonData_Set()
                {
                    BW = 15000000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 20,
                    MeasBW = 15000000,
                    BWLimit = 14550000,
                    RBW = 6250,
                    VBW = 6250,
                    NdBBWMin = 12750000,
                    NdBBWMax = 14700000,
                    MarPeakBW = 14250000,
                    TracePoints = 1601
                },
                new MeasMonData_Set()
                {
                    BW = 20000000,
                    OBWPercent = 99,
                    NdBLevel = 30,
                    DeltaFreqLimit = 20,
                    MeasBW = 20000000,
                    BWLimit = 19400000,
                    RBW = 12500,
                    VBW = 12500,
                    NdBBWMin = 17000000,
                    NdBBWMax = 19600000,
                    MarPeakBW = 19000000,
                    TracePoints = 1601
                }
            }
            #endregion
        };

        [XmlElement]
        public ObservableCollection<OPSOSIdentification_Set> OPSOSIdentifications
        {
            get { return _OPSOSIdentifications; }
            set { _OPSOSIdentifications = value; OnPropertyChanged("OPSOSIdentifications"); }
        }
        private ObservableCollection<OPSOSIdentification_Set> _OPSOSIdentifications = new ObservableCollection<OPSOSIdentification_Set>() { };

        [XmlElement]
        public ObservableCollection<SignalMask_Set> SignalMasks
        {
            get { return _SignalMasks; }
            set { _SignalMasks = value; OnPropertyChanged("SignalMasks"); }
        }
        private ObservableCollection<SignalMask_Set> _SignalMasks = new ObservableCollection<SignalMask_Set>() { };

        [XmlElement]
        public ObservableCollection<MeasMonConclusion_Set> Conclusions
        {
            get { return _Conclusions; }
            set { _Conclusions = value; OnPropertyChanged("Conclusions"); }
        }
        private ObservableCollection<MeasMonConclusion_Set> _Conclusions = new ObservableCollection<MeasMonConclusion_Set>() { };

        [XmlElement]
        public ObservableCollection<MeasMonBand> MeasMonBands
        {
            get { return _MeasMonBands; }
            set { _MeasMonBands = value; OnPropertyChanged("MeasMonBands"); }
        }
        private ObservableCollection<MeasMonBand> _MeasMonBands = new ObservableCollection<MeasMonBand>() { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(MeasMons_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class MeasMonTech_Set : INotifyPropertyChanged
    {

        [XmlElement]
        public DB.Technologys Techonology
        {
            get { return _Techonology; }
            set { _Techonology = value; OnPropertyChanged("Techonology"); }
        }
        private DB.Technologys _Techonology = DB.Technologys.Unknown;

        [XmlElement]
        public double DetectionLevel
        {
            get { return _DetectionLevel; }
            set { if (value < -120) { _DetectionLevel = -120; } else if (value > 130) { _DetectionLevel = 130; } else { _DetectionLevel = value; } OnPropertyChanged("DetectionLevel"); }
        }
        private double _DetectionLevel = 10;
        [XmlElement]
        public MeasMonData_Set[] Data
        {
            get { return _Data; }
            set { _Data = value; OnPropertyChanged("Data"); }
        }
        private MeasMonData_Set[] _Data = new MeasMonData_Set[] { };

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class MeasMonData_Set : INotifyPropertyChanged
    {
        /// <summary>
        /// Полоса сигнала по стандарту
        /// </summary>
        [XmlElement]
        public decimal BW
        {
            get { return _BW; }
            set { _BW = value; OnPropertyChanged("BW"); }
        }
        private decimal _BW = 5;

        [XmlElement]
        public double NdBLevel
        {
            get { return _NdBLevel; }
            set { _NdBLevel = value; OnPropertyChanged("NdBLevel"); }
        }
        private double _NdBLevel = -30;

        [XmlElement]
        public decimal OBWPercent
        {
            get { return _OBWPercent; }
            set { if (value < 0) { _OBWPercent = 0; } else if (value > 100) { _OBWPercent = 100; } else { _OBWPercent = value; } OnPropertyChanged("OBWPercent"); }
        }
        private decimal _OBWPercent = -30;

        /// <summary>
        /// граница отклонения относительно частоты в PPM
        /// </summary>
        [XmlElement]
        public decimal DeltaFreqLimit
        {
            get { return _DeltaFreqLimit; }
            set { _DeltaFreqLimit = value; OnPropertyChanged("DeltaFreqLimit"); }
        }
        private decimal _DeltaFreqLimit = 5;

        [XmlElement]
        public decimal MeasBW
        {
            get { return _MeasBW; }
            set { _MeasBW = value; OnPropertyChanged("MeasBW"); }
        }
        private decimal _MeasBW = 5;

        [XmlElement]
        public decimal BWLimit
        {
            get { return _BWLimit; }
            set { _BWLimit = value; OnPropertyChanged("BWLimit"); }
        }
        private decimal _BWLimit = 5;

        [XmlElement]
        public decimal NdBBWMin
        {
            get { return _NdBBWMin; }
            set { _NdBBWMin = value; OnPropertyChanged("NdBBWMin"); }
        }
        private decimal _NdBBWMin = 5;

        [XmlElement]
        public decimal NdBBWMax
        {
            get { return _NdBBWMax; }
            set { _NdBBWMax = value; OnPropertyChanged("NdBBWMax"); }
        }
        private decimal _NdBBWMax = 5;

        [XmlElement]
        public decimal MarPeakBW
        {
            get { return _MarPeakBW; }
            set { _MarPeakBW = value; OnPropertyChanged("MarPeakBW"); }
        }
        private decimal _MarPeakBW = 5;

        [XmlElement]
        public decimal RBW
        {
            get { return _RBW; }
            set { _RBW = value; OnPropertyChanged("RBW"); }
        }
        private decimal _RBW = 5;

        [XmlElement]
        public decimal VBW
        {
            get { return _VBW; }
            set { _VBW = value; OnPropertyChanged("VBW"); }
        }
        private decimal _VBW = 5;

        [XmlElement]
        public int TracePoints
        {
            get { return _TracePoints; }
            set { _TracePoints = value; OnPropertyChanged("TracePoints"); }
        }
        private int _TracePoints = 1601;
        /// <summary>
        /// 0 = "Одно измерение на БС"
        /// 1 = "Одно измерение на Сектор" 
        /// 2 = "Одно измерение на Номинал"
        /// </summary>
        [XmlElement]
        public int PlanImplementation
        {
            get { return _PlanImplementation; }
            set { _PlanImplementation = value; OnPropertyChanged("PlanImplementation"); }
        }
        private int _PlanImplementation = 5;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public partial class MeasMonBand : PropertyChangedBase
    {
        [XmlElement]
        public decimal Start
        {
            get { return _Start; }
            set { _Start = value; OnPropertyChanged("Start"); }
        }
        private decimal _Start = 0;

        [XmlElement]
        public decimal Stop
        {
            get { return _Stop; }
            set { _Stop = value; OnPropertyChanged("Stop"); }
        }
        private decimal _Stop = 0;

        [XmlElement]
        public decimal Step
        {
            get { return _Step; }
            set { _Step = value; OnPropertyChanged("Step"); }
        }
        private decimal _Step = 0;

        [XmlElement]
        public bool Use
        {
            get { return _Use; }
            set { _Use = value; OnPropertyChanged("Use"); }
        }
        private bool _Use = false;
    }
    [Serializable]
    public class SignalMask_Set : INotifyPropertyChanged
    {

        [XmlElement]
        public string Techonology
        {
            get { return _Techonology; }
            set { _Techonology = value; OnPropertyChanged("Techonology"); }
        }
        private string _Techonology = "";


        [XmlElement]
        public string Equipment
        {
            get { return _Equipment; }
            set { _Equipment = value; OnPropertyChanged("Equipment"); }
        }
        private string _Equipment = "";

        [XmlElement]
        public string EmissionClass
        {
            get { return _EmissionClass; }
            set { _EmissionClass = value; OnPropertyChanged("EmissionClass"); }
        }
        private string _EmissionClass = "";

        /// <summary>
        /// граница откланения относительной центральной частоты в РРМ или *Е-6
        /// </summary>
        [XmlElement]
        public decimal FrequencyDeviation
        {
            get { return _FrequencyDeviation; }
            set { _FrequencyDeviation = value; OnPropertyChanged("FrequencyDeviation"); }
        }
        private decimal _FrequencyDeviation = 5;

        private ObservableCollection<Equipment.tracepoint> _MaskPoints = new ObservableCollection<Equipment.tracepoint>() { };
        [XmlArray]
        public ObservableCollection<Equipment.tracepoint> MaskPoints
        {
            get { return _MaskPoints; }
            set { _MaskPoints = value; OnPropertyChanged("MaskPoints"); }
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class OPSOSIdentification_Set : PropertyChangedBase
    {
        [XmlElement]
        public string OpsosName
        {
            get { return _OpsosName; }
            set { _OpsosName = value; OnPropertyChanged("OpsosName"); }
        }
        [XmlIgnore]
        private string _OpsosName = "";

        [XmlElement]
        public string Techonology
        {
            get { return _Techonology; }
            set { _Techonology = value; OnPropertyChanged("Techonology"); }
        }
        [XmlIgnore]
        private string _Techonology = "";

        #region MCC
        [XmlElement]
        public uint MCC
        {
            get { return _MCC; }
            set { _MCC = value; OnPropertyChanged("MCC"); }
        }
        [XmlIgnore]
        private uint _MCC = 255;
        [XmlElement]
        public bool MCCUse
        {
            get { return _MCCUse; }
            set { _MCCUse = value; OnPropertyChanged("MCCUse"); }
        }
        [XmlIgnore]
        private bool _MCCUse = false;

        [XmlElement]
        public string MCCFrom
        {
            get { return _MCCFrom; }
            set { _MCCFrom = value; OnPropertyChanged("MCCFrom"); }
        }
        [XmlIgnore]
        private string _MCCFrom = "";

        [XmlElement]
        public int MCCFromIndex
        {
            get { return _MCCFromIndex; }
            set { _MCCFromIndex = value; OnPropertyChanged("MCCFromIndex"); }
        }
        [XmlIgnore]
        private int _MCCFromIndex = 0;
        #endregion

        #region MNC
        [XmlElement]
        public uint MNC
        {
            get { return _MNC; }
            set { _MNC = value; OnPropertyChanged("MNC"); }
        }
        [XmlIgnore]
        private uint _MNC = 0;

        [XmlElement]
        public bool MNCUse
        {
            get { return _MNCUse; }
            set { _MNCUse = value; OnPropertyChanged("MNCUse"); }
        }
        [XmlIgnore]
        private bool _MNCUse = false;

        [XmlElement]
        public string MNCFrom
        {
            get { return _MNCFrom; }
            set { _MNCFrom = value; OnPropertyChanged("MNCFrom"); }
        }
        [XmlIgnore]
        private string _MNCFrom = "";
        [XmlElement]
        public int MNCFromIndex
        {
            get { return _MNCFromIndex; }
            set { _MNCFromIndex = value; OnPropertyChanged("MNCFromIndex"); }
        }
        [XmlIgnore]
        private int _MNCFromIndex = 0;
        #endregion

        #region Area
        [XmlElement]
        public uint Area
        {
            get { return _Area; }
            set { _Area = value; OnPropertyChanged("Area"); }
        }
        [XmlIgnore]
        private uint _Area = 0;

        [XmlElement]
        public bool AreaUse
        {
            get { return _AreaUse; }
            set { _AreaUse = value; OnPropertyChanged("AreaUse"); }
        }
        [XmlIgnore]
        private bool _AreaUse = false;

        [XmlElement]
        public string AreaFrom
        {
            get { return _AreaFrom; }
            set { _AreaFrom = value; OnPropertyChanged("AreaFrom"); }
        }
        [XmlIgnore]
        private string _AreaFrom = "";

        [XmlElement]
        public int AreaFromIndex
        {
            get { return _AreaFromIndex; }
            set { _AreaFromIndex = value; OnPropertyChanged("AreaFromIndex"); }
        }
        [XmlIgnore]
        private int _AreaFromIndex = 0;
        #endregion

        #region ID
        [XmlElement]
        public int[] ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged("ID"); }
        }
        [XmlIgnore]
        private int[] _ID = new int[] { };

        [XmlElement]
        public bool IDUse
        {
            get { return _IDUse; }
            set { _IDUse = value; OnPropertyChanged("IDUse"); }
        }
        [XmlIgnore]
        private bool _IDUse = false;

        [XmlElement]
        public string IDFrom
        {
            get { return _IDFrom; }
            set { _IDFrom = value; OnPropertyChanged("IDFrom"); }
        }
        [XmlIgnore]
        private string _IDFrom = "";

        [XmlElement]
        public int IDFromIndex
        {
            get { return _IDFromIndex; }
            set { _IDFromIndex = value; OnPropertyChanged("IDFromIndex"); }
        }
        [XmlIgnore]
        private int _IDFromIndex = 0;


        #endregion

        #region Sector
        [XmlElement]
        public int[] Sector
        {
            get { return _Sector; }
            set { _Sector = value; OnPropertyChanged("Sector"); }
        }
        [XmlIgnore]
        private int[] _Sector = new int[] { };

        [XmlElement]
        public bool SectorUse
        {
            get { return _SectorUse; }
            set { _SectorUse = value; OnPropertyChanged("SectorUse"); }
        }
        [XmlIgnore]
        private bool _SectorUse = false;

        [XmlElement]
        public string SectorFrom
        {
            get { return _SectorFrom; }
            set { _SectorFrom = value; OnPropertyChanged("SectorFrom"); }
        }
        [XmlIgnore]
        private string _SectorFrom = "";

        [XmlElement]
        public int SectorFromIndex
        {
            get { return _SectorFromIndex; }
            set { _SectorFromIndex = value; OnPropertyChanged("SectorFromIndex"); }
        }
        [XmlIgnore]
        private int _SectorFromIndex = 0;

        [XmlElement]
        public ObservableCollection<SectorComparison> SectorComparisons
        {
            get { return _SectorComparisons; }
            set { _SectorComparisons = value; OnPropertyChanged("SectorComparisons"); }
        }
        [XmlIgnore]
        private ObservableCollection<SectorComparison> _SectorComparisons = new ObservableCollection<SectorComparison>() { };

        public class SectorComparison : PropertyChangedBase
        {
            [XmlElement]
            public int Radio 
            {
                get { return _Radio; }
                set { _Radio = value; OnPropertyChanged("Radio"); }
            }
            [XmlIgnore]
            private int _Radio = 0;

            [XmlElement]
            public int Real
            {
                get { return _Real; }
                set { _Real = value; OnPropertyChanged("Real"); }
            }
            [XmlIgnore]
            private int _Real = 0;
        }
        #endregion

        [XmlIgnore]
        //[XmlElement]
        public uint MNC_Radio
        {
            get { return _MNC_Radio; }
            set { _MNC_Radio = value; OnPropertyChanged("MNC_Radio"); }
        }
        private uint _MNC_Radio = 0;

        [XmlIgnore]
        //[XmlElement]
        public string MNC_Radio_From
        {
            get { return _MNC_Radio_From; }
            set { _MNC_Radio_From = value; OnPropertyChanged("MNC_Radio_From"); }
        }
        private string _MNC_Radio_From = "";

        [XmlElement]
        public string Formula
        {
            get { return _Formula; }
            set { _Formula = value; parseFormula(_Formula); OnPropertyChanged("Formula"); }
        }
        [XmlIgnore]
        private string _Formula = "";

        

        //[XmlIgnore]
        
        

        

        [XmlIgnore]
        //[XmlArray(IsNullable = true)]
        public int[] Carrier
        {
            get { return _Carrier; }
            set { _Carrier = value; OnPropertyChanged("Carrier"); }
        }
        [XmlIgnore]
        private int[] _Carrier = new int[] { };

        [XmlIgnore]
        //[XmlElement]
        public string CarrierFrom
        {
            get { return _CarrierFrom; }
            set { _CarrierFrom = value; OnPropertyChanged("CarrierFrom"); }
        }
        [XmlIgnore]
        private string _CarrierFrom = "";


        [XmlArray(IsNullable = true)]
        public int[] S0
        {
            get { return _S0; }
            set { _S0 = value; OnPropertyChanged("S0"); }
        }
        [XmlIgnore]
        private int[] _S0 = new int[] { };

        [XmlArray(IsNullable = true)]
        public int[] S1
        {
            get { return _S1; }
            set { _S1 = value; OnPropertyChanged("S1"); }
        }
        [XmlIgnore]
        private int[] _S1 = new int[] { };

        [XmlArray(IsNullable = true)]
        public int[] S2
        {
            get { return _S2; }
            set { _S2 = value; OnPropertyChanged("S2"); }
        }
        [XmlIgnore]
        private int[] _S2 = new int[] { };

        [XmlArray(IsNullable = true)]
        public int[] S3
        {
            get { return _S3; }
            set { _S3 = value; OnPropertyChanged("S3"); }
        }
        [XmlIgnore]
        private int[] _S3 = new int[] { };
        private void parseFormula(string str)
        {
            try
            {
                string[] astr = str.Split(';');
                #region
                if (astr.Length > 0)
                {
                    int[] cid = new int[] { };
                    int[] sector = new int[] { };
                    int[] carrier = new int[] { };
                    for (int i = 0; i < astr.Length; i++)
                    {
                        if (astr[i].ToUpper().StartsWith("ID") == true)//ID(eNodeB:0,1,2,3);CAR(CID:4);SEC(CID:-1)  ID(CID:0,1,2,3);CAR(CID:4);SEC(CID:-1)
                        {
                            int start = astr[i].IndexOf(':') + 1;
                            int stop = astr[i].IndexOf(')');
                            string[] st = astr[i].Substring(start, stop - start).Split(',');
                            cid = st.Select(int.Parse).ToArray();
                            int IDFromstart = astr[i].IndexOf('(') + 1;
                            int IDFromstop = astr[i].IndexOf(':');
                            IDFrom = astr[i].Substring(IDFromstart, IDFromstop - IDFromstart);
                            ID = cid;
                        }
                        if (astr[i].ToUpper().StartsWith("CAR") == true)
                        {
                            int start = astr[i].IndexOf(':') + 1;
                            int stop = astr[i].IndexOf(')');
                            carrier = astr[i].Substring(start, stop - start).Split(',').Select(int.Parse).ToArray();
                            int IDFromstart = astr[i].IndexOf('(') + 1;
                            int IDFromstop = astr[i].IndexOf(':');
                            CarrierFrom = astr[i].Substring(IDFromstart, IDFromstop - IDFromstart);
                            Carrier = carrier;
                        }
                        if (astr[i].ToUpper().StartsWith("SEC") == true)
                        {
                            int start = astr[i].IndexOf(':') + 1;
                            int stop = astr[i].IndexOf(')');
                            sector = astr[i].Substring(start, stop - start).Split(',').Select(int.Parse).ToArray();
                            int IDFromstart = astr[i].IndexOf('(') + 1;
                            int IDFromstop = astr[i].IndexOf(':');
                            SectorFrom = astr[i].Substring(IDFromstart, IDFromstop - IDFromstart);
                            Sector = sector;
                        }
                        if (astr[i].ToUpper().StartsWith("NC") == true)
                        {
                            int start = astr[i].IndexOf(':') + 1;
                            int stop = astr[i].IndexOf(')');
                            MNC_Radio = uint.Parse(astr[i].Substring(start, stop - start));
                            int IDFromstart = astr[i].IndexOf('(') + 1;
                            int IDFromstop = astr[i].IndexOf(':');
                            MNC_Radio_From = astr[i].Substring(IDFromstart, IDFromstop - IDFromstart);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
            }
        }


        //public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        //// Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        //public void OnPropertyChanged(string propertyName)
        //{
        //    // Если кто-то на него подписан, то вызывем его
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
    [Serializable]
    public class MeasMonConclusion_Set : INotifyPropertyChanged
    {
        private int _ID = 0;
        [XmlElement]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged("ID"); }
        }
        private string _Type = "";
        [XmlElement]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertyChanged("Type"); }
        }
        private string _Conclusion = "";
        [XmlElement]
        public string Conclusion
        {
            get { return _Conclusion; }
            set { _Conclusion = value; OnPropertyChanged("Conclusion"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    [Serializable]
    public class Antennas_Set : INotifyPropertyChanged
    {
        private ObservableCollection<Antena_Set> _Antennas = new ObservableCollection<Antena_Set>() { };
        [XmlArray]
        public ObservableCollection<Antena_Set> Antennas
        {
            get { return _Antennas; }
            set { _Antennas = value; OnPropertyChanged("Antennas"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(Antennas_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class Antena_Set : INotifyPropertyChanged
    {
        private string _AntenaName = "AntenaName";
        [XmlElement]
        public string AntenaName
        {
            get { return _AntenaName; }
            set { _AntenaName = value; OnPropertyChanged("AntenaName"); }
        }
        private ObservableCollection<AntennaLevel_Set> _AntennaData = new ObservableCollection<AntennaLevel_Set>() { };
        [XmlArray]
        public ObservableCollection<AntennaLevel_Set> AntennaData
        {
            get { return _AntennaData; }
            set { _AntennaData = value; OnPropertyChanged("AntennaData"); }
        }

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    [Serializable]
    public class AntennaLevel_Set : INotifyPropertyChanged
    {
        private decimal _NumPoint = 0;
        [XmlElement]
        public decimal NumPoint
        {
            get { return _NumPoint; }
            set { _NumPoint = value; OnPropertyChanged("NumPoint"); }
        }
        private decimal _Freq = 0;
        [XmlElement]
        public decimal Freq
        {
            get { return _Freq; }
            set { _Freq = value; OnPropertyChanged("Freq"); }
        }
        private decimal _AntennaFactor = 0;
        [XmlElement]
        public decimal AntennaFactor
        {
            get { return _AntennaFactor; }
            set { _AntennaFactor = value; OnPropertyChanged("AntennaFactor"); }
        }
        private decimal _Fieldstrength = 0;
        [XmlElement]
        public decimal Fieldstrength
        {
            get { return _Fieldstrength; }
            set { _Fieldstrength = value; OnPropertyChanged("Fieldstrength"); }
        }
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class Equipments_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public Equipment_Set SpectrumAnalyzer
        {
            get { return _SpectrumAnalyzer; }
            set { _SpectrumAnalyzer = value; _SpectrumAnalyzer.ID = 1; _SpectrumAnalyzer.EquipmentName = "Spectrum Analyzer"; OnPropertyChanged("SpectrumAnalyzer"); }
        }
        private Equipment_Set _SpectrumAnalyzer = new Equipment_Set() { };

        [XmlElement]
        public Equipment_Set RuSReceiver
        {
            get { return _RuSReceiver; }
            set { _RuSReceiver = value; _RuSReceiver.ID = 2; _RuSReceiver.EquipmentName = "R&S Receiver"; OnPropertyChanged("RuSReceiver"); }
        }
        private Equipment_Set _RuSReceiver = new Equipment_Set() { };

        [XmlElement]
        public Equipment_Set RuSTSMx
        {
            get { return _RuSTSMx; }
            set { _RuSTSMx = value; _RuSTSMx.ID = 3; _RuSTSMx.EquipmentName = "R&S Network Analyzer TSMx"; OnPropertyChanged("RuSTSMx"); }
        }
        private Equipment_Set _RuSTSMx = new Equipment_Set() { };

        [XmlElement]
        public Equipment_Set RuSRomesRC
        {
            get { return _RuSRomesRC; }
            set { _RuSRomesRC = value; _RuSRomesRC.ID = 4; _RuSRomesRC.EquipmentName = "R&S Romes Remote Control"; OnPropertyChanged("RuSRomesRC"); }
        }
        private Equipment_Set _RuSRomesRC = new Equipment_Set() { };

        [XmlElement]
        public Equipment_Set SignalHound
        {
            get { return _SignalHound; }
            set { _SignalHound = value; _SignalHound.ID = 5; _SignalHound.EquipmentName = "Signal Hound"; OnPropertyChanged("SignalHound"); }
        }
        private Equipment_Set _SignalHound = new Equipment_Set() { };

        [XmlElement]
        public Equipment_Set WR61
        {
            get { return _WR61; }
            set { _WR61 = value; _WR61.ID = 5; _WR61.EquipmentName = "WR61 Receiver"; OnPropertyChanged("WR61"); }
        }
        private Equipment_Set _WR61 = new Equipment_Set() { };

        [XmlElement]
        public Equipment_Set GNSS
        {
            get { return _GNSS; }
            set { _GNSS = value; _GNSS.ID = 5; _GNSS.EquipmentName = "GNSS Receiver"; OnPropertyChanged("GNSS"); }
        }
        private Equipment_Set _GNSS = new Equipment_Set() { };


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(Equipments_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class Equipment_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public int ID
        {
            get { return _ID; }
            set { _ID = value; OnPropertyChanged("ID"); }
        }
        private int _ID = 0;

        [XmlElement]
        public string EquipmentName
        {
            get { return _EquipmentName; }
            set { _EquipmentName = value; OnPropertyChanged("EquipmentName"); }
        }
        private string _EquipmentName = "EquipmentName";

        [XmlElement]
        public bool UseEquipment
        {
            get { return _UseEquipment; }
            set { _UseEquipment = value; OnPropertyChanged("UseEquipment"); }
        }
        private bool _UseEquipment = false;

        [XmlElement]
        public bool UseEquipmentFromOption
        {
            get { return _UseEquipmentFromOption; }
            set { _UseEquipmentFromOption = value; OnPropertyChanged("UseEquipmentFromOption"); }
        }
        private bool _UseEquipmentFromOption = false;

        [XmlElement]
        public int PrintScreenType
        {
            get { return _PrintScreenType; }
            set { _PrintScreenType = value; OnPropertyChanged("PrintScreenType"); }
        }
        private int _PrintScreenType = 0;

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class ATDIConnection_Set : INotifyPropertyChanged
    {
        [XmlElement]
        public ATDIConnection Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged("Selected"); }
        }
        private ATDIConnection _Selected = new ATDIConnection() { };


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetString()
        {
            return Serialize(this);
        }
        private string Serialize(ATDIConnection_Set toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
    [Serializable]
    public class ATDIConnection : INotifyPropertyChanged
    {
        [XmlElement(ElementName = "Data0")]
        public string owner_id
        {
            get { return _owner_id; }
            set { _owner_id = value; OnPropertyChanged("owner_id"); }
        }
        private string _owner_id = "";

        [XmlElement(ElementName = "Data1")]
        public string product_key
        {
            get { return _product_key; }
            set { _product_key = value; OnPropertyChanged("product_key"); }
        }
        private string _product_key = "";

        [XmlElement(ElementName = "Data2")]
        public string sensor_equipment_tech_id
        {
            get { return _sensor_equipment_tech_id; }
            set { _sensor_equipment_tech_id = value; OnPropertyChanged("sensor_equipment_tech_id"); }
        }
        private string _sensor_equipment_tech_id = "";

        [XmlElement(ElementName = "Data3")]
        public string rabbit_host_name
        {
            get { return _rabbit_host_name; }
            set { _rabbit_host_name = value; OnPropertyChanged("rabbit_host_name"); }
        }
        private string _rabbit_host_name = "";

        [XmlElement(ElementName = "Data4")]
        public string rabbit_virtual_host_name
        {
            get { return _rabbit_virtual_host_name; }
            set { _rabbit_virtual_host_name = value; OnPropertyChanged("rabbit_virtual_host_name"); }
        }
        private string _rabbit_virtual_host_name = "";

        [XmlElement(ElementName = "Data5")]
        public string rabbit_host_port
        {
            get { return _rabbit_host_port; }
            set { _rabbit_host_port = value; OnPropertyChanged("rabbit_host_port"); }
        }
        private string _rabbit_host_port = "";

        [XmlElement(ElementName = "Data6")]
        public string rabbit_user_name
        {
            get { return _rabbit_user_name; }
            set { _rabbit_user_name = value; OnPropertyChanged("rabbit_user_name"); }
        }
        private string _rabbit_user_name = "";

        [XmlElement(ElementName = "Data7")]
        public string rabbit_password
        {
            get { return _rabbit_password; }
            set { _rabbit_password = value; OnPropertyChanged("rabbit_password"); }
        }
        private string _rabbit_password = "";

        [XmlElement(ElementName = "Data8")]
        public string sensor_queue
        {
            get { return _sensor_queue; }
            set { _sensor_queue = value; OnPropertyChanged("sensor_queue"); }
        }
        private string _sensor_queue = "";

        [XmlElement(ElementName = "Data9")]
        public string sensor_confirm_queue
        {
            get { return _sensor_confirm_queue; }
            set { _sensor_confirm_queue = value; OnPropertyChanged("sensor_confirm_queue"); }
        }
        private string _sensor_confirm_queue = "";

        [XmlElement(ElementName = "Data10")]
        public string task_queue
        {
            get { return _task_queue; }
            set { _task_queue = value; OnPropertyChanged("task_queue"); }
        }
        private string _task_queue = "";

        [XmlElement(ElementName = "Data11")]
        public string result_queue
        {
            get { return _result_queue; }
            set { _result_queue = value; OnPropertyChanged("result_queue"); }
        }
        private string _result_queue = "";


        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #region Global


    #endregion Global

}