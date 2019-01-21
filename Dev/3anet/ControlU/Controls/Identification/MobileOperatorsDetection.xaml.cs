using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlU.Controls.Identification
{
    /// <summary>
    /// Логика взаимодействия для MobileOperatorsDetection.xaml
    /// </summary>
    public partial class MobileOperatorsDetection : UserControl, INotifyPropertyChanged
    {
        public Settings.OPSOSIdentification_Set Data
        {
            get { return (Settings.OPSOSIdentification_Set)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); OnPropertyChanged("Data"); }
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                "Data",
                typeof(Settings.OPSOSIdentification_Set),
                typeof(MobileOperatorsDetection),
                new PropertyMetadata(
                    new Settings.OPSOSIdentification_Set() { },
                    new PropertyChangedCallback(DataOnPropertyChanged)
                    )
                );

        public ObservableCollection<data> IDArrayData
        {
            get { return _IDArrayData; }
            set { _IDArrayData = value; OnPropertyChanged("IDArrayData"); }
        }
        ObservableCollection<data> _IDArrayData = new ObservableCollection<data>() { };

        public ObservableCollection<data> SectorArrayData
        {
            get { return _SectorArrayData; }
            set { _SectorArrayData = value; OnPropertyChanged("SectorArrayData"); }
        }
        ObservableCollection<data> _SectorArrayData = new ObservableCollection<data>() { };

        public ObservableCollection<string> MCCParameters
        {
            get { return _MCCParameters; }
            set { _MCCParameters = value; OnPropertyChanged("MCCParameters"); }
        }
        ObservableCollection<string> _MCCParameters = new ObservableCollection<string>() { };

        public ObservableCollection<string> MNCParameters
        {
            get { return _MNCParameters; }
            set { _MNCParameters = value; OnPropertyChanged("MNCParameters"); }
        }
        ObservableCollection<string> _MNCParameters = new ObservableCollection<string>() { };

        public ObservableCollection<string> AreaParameters
        {
            get { return _AreaParameters; }
            set { _AreaParameters = value; OnPropertyChanged("AreaParameters"); }
        }
        ObservableCollection<string> _AreaParameters = new ObservableCollection<string>() { };

        public ObservableCollection<string> IDParameters
        {
            get { return _IDParameters; }
            set { _IDParameters = value; OnPropertyChanged("IDParameters"); }
        }
        ObservableCollection<string> _IDParameters = new ObservableCollection<string>() { };

        public ObservableCollection<string> SectorParameters
        {
            get { return _SectorParameters; }
            set { _SectorParameters = value; OnPropertyChanged("SectorParameters"); }
        }
        ObservableCollection<string> _SectorParameters = new ObservableCollection<string>() { };
        bool load = false;

        public MobileOperatorsDetection()
        {
            InitializeComponent();
            this.DataContext = this;

        }
        private static void DataOnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MobileOperatorsDetection thisControl = (MobileOperatorsDetection)sender;
            thisControl.load = true;
            if (thisControl.Data != null)
            {
                thisControl.Load(thisControl.Data);
            }
            thisControl.load = false;

        }
        private void IDType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if(load == false)
            Load(Data);
            //load = false;
        }
        private void SectorType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //load = true;
            Load(Data);
            //load = false;
        }
        private void Load(Settings.OPSOSIdentification_Set data)
        {
            int idlength = 0;
            int seclength = 0;
            if (data.Techonology == "GSM")
            {
                #region
                MCCParameters = new ObservableCollection<string>() { "MCC" };
                MNCParameters = new ObservableCollection<string>() { "MNC" };
                AreaParameters = new ObservableCollection<string>() { "LAC" };
                IDParameters = new ObservableCollection<string>() { "CID" };
                SectorParameters = new ObservableCollection<string>() { "CID" };
                IDArrayData = new ObservableCollection<data>() { };
                SectorArrayData = new ObservableCollection<data>() { };
                idlength = 5;
                seclength = 5;
                for (int i = 0; i < idlength; i++)
                {
                    IDArrayData.Add(new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                                new dataWithId() { Selected = "", Index = 0, Name = "ID0" },
                                new dataWithId() { Selected = "", Index = 1, Name = "ID1" },
                                new dataWithId() { Selected = "", Index = 2, Name = "ID2" },
                                new dataWithId() { Selected = "", Index = 3, Name = "ID3" },
                                new dataWithId() { Selected = "", Index = 4, Name = "ID4" },
                            }
                    });
                    SectorArrayData.Add(new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                                new dataWithId() { Selected = "", Index = 0, Name = "SEC0" },
                                new dataWithId() { Selected = "", Index = 1, Name = "SEC1" },
                                new dataWithId() { Selected = "", Index = 2, Name = "SEC2" },
                                new dataWithId() { Selected = "", Index = 3, Name = "SEC3" },
                                new dataWithId() { Selected = "", Index = 4, Name = "SEC4" },
                            }
                    });
                }
                foreach (data d in IDArrayData)
                {
                    d.IDDataSelected = d.IDData[0];
                }
                foreach (data s in SectorArrayData)
                {
                    s.IDDataSelected = s.IDData[0];
                }
                #endregion
            }
            else if (data.Techonology == "UMTS")
            {
                #region
                MCCParameters = new ObservableCollection<string>() { "MCC" };
                MNCParameters = new ObservableCollection<string>() { "MNC" };
                AreaParameters = new ObservableCollection<string>() { "LAC" };
                IDParameters = new ObservableCollection<string>() { "CID", "RNC", "UCID" };
                SectorParameters = new ObservableCollection<string>() { "CID", "RNC", "UCID" };
                IDArrayData = new ObservableCollection<data>() { };
                SectorArrayData = new ObservableCollection<data>() { };

                //https://github.com/mroczis/NetMonster-docs/wiki/NetMonster-cell-format
                if (data.IDFromIndex == 0) idlength = 5;//CID  0 - 65535 
                else if (data.IDFromIndex == 1) idlength = 5;//RNC  0 to 65535
                else if (data.IDFromIndex == 2) idlength = 9;//UCID  0 to 268435455
                if (data.SectorFromIndex == 0) seclength = 5;//CID  0 - 65535 
                else if (data.SectorFromIndex == 1) seclength = 5;//RNC  0 to 65535
                else if (data.SectorFromIndex == 2) seclength = 9;//UCID  0 to 268435455
                for (int i = 0; i < idlength; i++)
                {
                    data d = new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                            }
                    };
                    for (int j = 0; j < idlength; j++)
                    {
                        d.IDData.Add(
                            new dataWithId() { Selected = "", Index = j, Name = "ID" + j.ToString() });
                    }
                    IDArrayData.Add(d);
                }
                for (int i = 0; i < seclength; i++)
                {
                    data d = new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                            }
                    };
                    for (int j = 0; j < seclength; j++)
                    {
                        d.IDData.Add(
                            new dataWithId() { Selected = "", Index = j, Name = "SEC" + j.ToString() });
                    }
                    SectorArrayData.Add(d);
                }
                foreach (data d in IDArrayData)
                {
                    d.IDDataSelected = d.IDData[0];
                }
                foreach (data d in SectorArrayData)
                {
                    d.IDDataSelected = d.IDData[0];
                }
                #endregion
            }
            else if (data.Techonology == "LTE")
            {
                #region
                MCCParameters = new ObservableCollection<string>() { "MCC" };
                MNCParameters = new ObservableCollection<string>() { "MNC" };
                AreaParameters = new ObservableCollection<string>() { "TAC", "PCI" };
                IDParameters = new ObservableCollection<string>() { "eNodeBId", "ECI", "CID", "TAC", "PCI" };
                SectorParameters = new ObservableCollection<string>() { "eNodeBId", "ECI", "CID", "TAC", "PCI" };
                IDArrayData = new ObservableCollection<data>() { };
                SectorArrayData = new ObservableCollection<data>() { };

                //https://github.com/mroczis/NetMonster-docs/wiki/NetMonster-cell-format
                if (data.IDFromIndex == 0) idlength = 6;//eNodeBId  0 - 999999
                else if (data.IDFromIndex == 1) idlength = 9;//ECI  0 to 268435455
                else if (data.IDFromIndex == 2) idlength = 3;//CID  0 to 255
                else if (data.IDFromIndex == 3) idlength = 5;//TAC  1 to 65534
                else if (data.IDFromIndex == 4) idlength = 3;//PCI  0 to 503
                if (data.SectorFromIndex == 0) seclength = 6;//eNodeBId  0 - 999999
                else if (data.SectorFromIndex == 1) seclength = 9;//ECI  0 to 268435455
                else if (data.SectorFromIndex == 2) seclength = 3;//CID  0 to 255
                else if (data.SectorFromIndex == 3) seclength = 5;//TAC  1 to 65534
                else if (data.SectorFromIndex == 4) seclength = 3;//PCI  0 to 503
                for (int i = 0; i < idlength; i++)
                {
                    data d = new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                            }
                    };
                    for (int j = 0; j < idlength; j++)
                    {
                        d.IDData.Add(
                            new dataWithId() { Selected = "", Index = j, Name = "ID" + j.ToString() });
                    }
                    IDArrayData.Add(d);
                }
                for (int i = 0; i < seclength; i++)
                {
                    data d = new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                            }
                    };
                    for (int j = 0; j < seclength; j++)
                    {
                        d.IDData.Add(
                            new dataWithId() { Selected = "", Index = j, Name = "SEC" + j.ToString() });
                    }
                    SectorArrayData.Add(d);
                }
                foreach (data d in IDArrayData)
                {
                    d.IDDataSelected = d.IDData[0];
                }
                foreach (data d in SectorArrayData)
                {
                    d.IDDataSelected = d.IDData[0];
                }
                #endregion
            }
            else if (data.Techonology == "CDMA")
            {
                #region
                MCCParameters = new ObservableCollection<string>() { "MCC", "NID" };
                MNCParameters = new ObservableCollection<string>() { "MNC", "NID", "SID" };
                AreaParameters = new ObservableCollection<string>() { "LAC" };
                IDParameters = new ObservableCollection<string>() { "BaseID", "PN" };
                SectorParameters = new ObservableCollection<string>() { "BaseID", "PN" };
                IDArrayData = new ObservableCollection<data>() { };
                SectorArrayData = new ObservableCollection<data>() { };

                //https://github.com/mroczis/NetMonster-docs/wiki/NetMonster-cell-format
                if (data.IDFromIndex == 0) idlength = 5;//BaseID  1 - 2 147 483 646 
                else if (data.IDFromIndex == 1) idlength = 5;//PN  0 to 65535
                else if (data.IDFromIndex == 2) idlength = 9;//UCID  0 to 268435455
                if (data.SectorFromIndex == 0) seclength = 5;//BaseID  1 - 2 147 483 646 
                else if (data.SectorFromIndex == 1) seclength = 5;//PN  0 to 65535
                else if (data.SectorFromIndex == 2) seclength = 9;//UCID  0 to 268435455
                for (int i = 0; i < idlength; i++)
                {
                    data d = new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                            }
                    };
                    for (int j = 0; j < idlength; j++)
                    {
                        d.IDData.Add(
                            new dataWithId() { Selected = "", Index = j, Name = "ID" + j.ToString() });
                    }
                    IDArrayData.Add(d);
                }
                for (int i = 0; i < seclength; i++)
                {
                    data d = new data()
                    {
                        Index = i,
                        IDData = new ObservableCollection<dataWithId>
                            {
                                new dataWithId() { Selected = "", Index = -1, Name = "NotSelected" },
                            }
                    };
                    for (int j = 0; j < seclength; j++)
                    {
                        d.IDData.Add(
                            new dataWithId() { Selected = "", Index = j, Name = "SEC" + j.ToString() });
                    }
                    SectorArrayData.Add(d);
                }
                foreach (data d in IDArrayData)
                {
                    d.IDDataSelected = d.IDData[0];
                }
                foreach (data d in SectorArrayData)
                {
                    d.IDDataSelected = d.IDData[0];
                }
                #endregion
            }
            #region
            if (data.ID == null || data.ID.Length != idlength)
            {
                int[] ar = new int[idlength];
                for (int j = 0; j < ar.Length; j++)
                {
                    ar[j] = -1;
                }
                data.ID = ar;
            }
            if (data.ID.Length > 0)
            {
                for (int j = 0; j < IDArrayData.Count(); j++)
                {
                    for (int g = 0; g < IDArrayData[j].IDData.Count(); g++)
                    {
                        if (data.ID[j] == IDArrayData[j].IDData[g].Index)
                            IDArrayData[j].IDDataSelected = IDArrayData[j].IDData[g];
                    }
                }
            }
            if (data.Sector == null || data.Sector.Length != seclength)
            {
                int[] ar = new int[seclength];
                for (int j = 0; j < ar.Length; j++)
                {
                    ar[j] = -1;
                }
                data.Sector = ar;
            }
            if (data.Sector.Length > 0)
            {
                for (int j = 0; j < SectorArrayData.Count(); j++)
                {
                    for (int g = 0; g < SectorArrayData[j].IDData.Count(); g++)
                    {
                        if (data.Sector[j] == SectorArrayData[j].IDData[g].Index)
                            SectorArrayData[j].IDDataSelected = SectorArrayData[j].IDData[g];
                    }
                }
            }
            #endregion
            UpdateExample(Data);
        }
        /// <summary>
        /// Возвращает идентификатор как в БД и номер сектора
        /// </summary>
        /// <param name="tech">GSM/UMTS/LTE/CDMA</param>
        /// <param name="s0">MCC</param>
        /// <param name="s1">MNC</param>
        /// <param name="s2">AREA(LAC,TAC...)</param>
        /// <param name="s3">ID(CID,BASEID,eNodeBid..)</param>
        private int[] parse(string tech, uint s0, uint s1, uint s2, uint s3)
        {
            int[] res = new int[] { -1, -1 };
            int id = -1;
            //for (int i = 0; i < App.Sett.MeasMons_Settings.OPSOSIdentifications.Count(); i++)
            //{
            Settings.OPSOSIdentification_Set op = Data;// App.Sett.MeasMons_Settings.OPSOSIdentifications[i];
            if (op.MCC == s0 && op.MNC == s1 && op.Techonology == tech)
            {
                string idlength = "";
                for (int j = 0; j < op.ID.Length; j++)
                    idlength += "0";
                string str = string.Format("{0:" + idlength + "}", s3);
                string cid = str;
                int[] CIDarr = op.ID;
                //Array.Sort(CIDarr);
                if (CIDarr.Length > 0)
                {
                    cid = "";
                    for (int s = 0; s < CIDarr.Length; s++)
                    {
                        if (CIDarr[s] > -1) cid += str.Substring(CIDarr[s], 1);
                    }
                }
                if (cid.Length > 0) res[0] = Convert.ToInt32(cid);
                string sec = str;
                int[] SECarr = op.Sector;
                //Array.Sort(CIDarr);
                if (SECarr.Length > 0)
                {
                    sec = "";
                    for (int s = 0; s < SECarr.Length; s++)
                    {
                        if (SECarr[s] > -1) sec += str.Substring(SECarr[s], 1);
                    }
                }
                if (sec.Length > 0) res[1] = Convert.ToInt32(sec);
            }
            //}
            //res = "inp: " + tech + " " + s0 + " " + s1 + " " + s2 + " " + s3 + " ret:" + id.ToString();
            Debug.WriteLine(res);
            return res;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            parse(Data.Techonology, 255, 1, 0, 12345);
        }
        private void MCCType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MNCType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void IDValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (load == false && sender is ComboBox && ((ComboBox)sender).SelectedItem is dataWithId)
            {
                ComboBox cb = (ComboBox)sender;
                Grid ob = (Grid)cb.Parent;
                string str = ((TextBlock)ob.Children[0]).Text;
                dataWithId selected = (dataWithId)((ComboBox)sender).SelectedItem;

                for (int i = 0; i < IDArrayData.Count(); i++)
                {
                    if (IDArrayData[i].Index.ToString() != str &&
                        IDArrayData[i].IDDataSelected.Index > -1 &&
                        IDArrayData[i].IDDataSelected.Index == selected.Index)
                        IDArrayData[i].IDDataSelected = IDArrayData[i].IDData[0];
                }
                List<int> IDarr = new List<int>() { };
                for (int i = 0; i < IDArrayData.Count(); i++)
                {
                    if (IDArrayData[i].IDDataSelected.Name.Contains("ID") &&
                        IDArrayData[i].IDDataSelected.Index > -1)
                        IDarr.Add(IDArrayData[i].IDDataSelected.Index);
                    else IDarr.Add(-1);
                }
                Data.ID = IDarr.ToArray();

                UpdateExample(Data);
            }

        }

        private void SectorValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (load == false && sender is ComboBox && ((ComboBox)sender).SelectedItem is dataWithId)
            {
                ComboBox cb = (ComboBox)sender;
                Grid ob = (Grid)cb.Parent;
                string str = ((TextBlock)ob.Children[0]).Text;
                dataWithId selected = (dataWithId)((ComboBox)sender).SelectedItem;

                for (int i = 0; i < SectorArrayData.Count(); i++)
                {
                    if (SectorArrayData[i].Index.ToString() != str &&
                        SectorArrayData[i].IDDataSelected.Index > -1 &&
                        SectorArrayData[i].IDDataSelected.Index == selected.Index)
                        SectorArrayData[i].IDDataSelected = SectorArrayData[i].IDData[0];
                }
                List<int> IDarr = new List<int>() { };
                int NumberOfUsedSymbols = 0;
                for (int i = 0; i < SectorArrayData.Count(); i++)
                {
                    if (SectorArrayData[i].IDDataSelected.Name.Contains("SEC") && SectorArrayData[i].IDDataSelected.Index > -1)
                    { IDarr.Add(SectorArrayData[i].IDDataSelected.Index); NumberOfUsedSymbols++; }
                    else IDarr.Add(-1);
                }
                Data.Sector = IDarr.ToArray();
                UpdateExample(Data);
                if (NumberOfUsedSymbols == 1)
                {
                    if (Data.SectorComparisons.Count() != 10)
                    {
                        Data.SectorComparisons.Clear();
                        for (int i = 0; i < 10; i++)
                        {
                            Data.SectorComparisons.Add(new Settings.OPSOSIdentification_Set.SectorComparison() { Radio = i, Real = i });
                        }
                    }
                }
            }

        }

        private void UpdateExample(Settings.OPSOSIdentification_Set data)
        {
            uint s0 = 0, s1 = 0, s2 = 0, s3 = 0;
            if (data.Techonology == "GSM")
            {
                s0 = data.MCC;
                s1 = data.MNC;
                s2 = 0;
                s3 = 67890;
            }
            if (data.Techonology == "UMTS")
            {
                s0 = data.MCC;
                s1 = data.MNC;
                s2 = 0;
                if (data.IDFromIndex == 0) s3 = 67890;
                else if (data.IDFromIndex == 1) s3 = 67890;
                else if (data.IDFromIndex == 2) s3 = 123456789;
            }
            if (data.Techonology == "LTE")
            {
                s0 = data.MCC;
                s1 = data.MNC;
                s2 = 0;
                if (data.IDFromIndex == 0) s3 = 567890;
                else if (data.IDFromIndex == 1) s3 = 123456789;
                else if (data.IDFromIndex == 2) s3 = 123;
                else if (data.IDFromIndex == 3) s3 = 12345;
                else if (data.IDFromIndex == 4) s3 = 123;
            }
            ExampleIN.Text = "GCID " + s0 + " " + s1 + " " + s2 + " " + s3;
            int[] res = parse(Data.Techonology, s0, s1, s2, s3);
            string resstr = "";
            if (res[0] > -1) resstr += "ID " + res[0].ToString() + " ";
            if (res[1] > -1) resstr += "SEC " + res[1].ToString() + " ";
            ExampleOUT.Text = resstr;
        }










        public virtual void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public class data : INotifyPropertyChanged
        {
            public int Index
            {
                get { return _Index; }
                set { _Index = value; OnPropertyChanged("Index"); }
            }
            private int _Index = 0;

            public ObservableCollection<dataWithId> IDData
            {
                get { return _IDData; }
                set { _IDData = value; OnPropertyChanged("IDData"); }
            }
            ObservableCollection<dataWithId> _IDData = new ObservableCollection<dataWithId>() { };
            public dataWithId IDDataSelected
            {
                get { return _IDDataSelected; }
                set { _IDDataSelected = value; OnPropertyChanged("IDDataSelected"); }
            }
            dataWithId _IDDataSelected = new dataWithId() { };

            public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

            // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
            public void OnPropertyChanged(string propertyName)
            {
                // Если кто-то на него подписан, то вызывем его
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class dataWithId : INotifyPropertyChanged
        {
            public string Name
            {
                get { return _Name; }
                set { _Name = value; OnPropertyChanged("Name"); }
            }
            private string _Name = "";

            public string Selected { get; set; }
            public int Index { get; set; }

            public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

            // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
            public void OnPropertyChanged(string propertyName)
            {
                // Если кто-то на него подписан, то вызывем его
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

}
