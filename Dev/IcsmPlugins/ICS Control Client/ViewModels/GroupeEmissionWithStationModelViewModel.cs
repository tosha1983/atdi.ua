using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using XICSM.ICSControlClient.Models.Views;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Models.WcfDataApadters;
using SVC = XICSM.ICSControlClient.WcfServiceClients;
using MP = XICSM.ICSControlClient.WpfControls.Maps;
using SDR = Atdi.Contracts.WcfServices.Sdrn.Server;
using SDRI = Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using System.Windows;
using FRM = System.Windows.Forms;
using FM = XICSM.ICSControlClient.Forms;
using ICSM;
using System.Windows.Controls;
using System.Collections;
using XICSM.ICSControlClient.Models;
using System.Windows.Input;
using System.Configuration;
using System.Globalization;
using TR = System.Threading;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using Atdi.Common;

namespace XICSM.ICSControlClient.ViewModels
{
    public class GroupeEmissionWithStationModelViewModel : WpfViewModelBase
    {
        private IList _currentSensors;
        private long[] _currentSensorsIndexes;
        private IList _currentAreas;
        private IList _currentRefSpectrums;
        private MP.MapDrawingData _currentMapData;
        DateTime? _dateStart;
        DateTime? _dateStop;
        private bool _isEnabledStart = false;

        private ShortSensorDataAdatper _sensors;
        private AreasDataAdapter _areas;
        private RefSpectrumDataAdapter _refSpectrums;

        public WpfCommand StartCommand { get; set; }
        public WpfCommand ProtocolCommand { get; set; }
        public WpfCommand ImportRefSpectrumFromCSVCommand { get; set; }
        public WpfCommand DeleteRefSpectrumCommand { get; set; }

        public GroupeEmissionWithStationModelViewModel()
        {
            this.StartCommand = new WpfCommand(this.OnStartCommand);
            this.ProtocolCommand = new WpfCommand(this.OnProtocolCommand);
            this.ImportRefSpectrumFromCSVCommand = new WpfCommand(this.OnImportRefSpectrumFromCSVCommand);
            this.DeleteRefSpectrumCommand = new WpfCommand(this.OnDeleteRefSpectrumCommand);
            this._sensors = new ShortSensorDataAdatper();
            this._areas = new AreasDataAdapter();
            this._refSpectrums = new RefSpectrumDataAdapter(); 
            this.ReloadData();
            this.RedrawMap();
        }
        public ShortSensorDataAdatper Sensors => this._sensors;
        public AreasDataAdapter Areas => this._areas;
        public RefSpectrumDataAdapter RefSpectrums => this._refSpectrums;
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        public IList CurrentSensors
        {
            get => this._currentSensors;
            set
            {
                this._currentSensors = value;
                RedrawMap();
                CheckEnabledStart();
            }
        }
        public long[] CurrentSensorsIndexes
        {
            get => this._currentSensorsIndexes;
            set => this.Set(ref this._currentSensorsIndexes, value);
        }
        
        public IList CurrentAreas
        {
            get => this._currentAreas;
            set
            {
                this._currentAreas = value;
                SelectSensors();
                RedrawMap();
                CheckEnabledStart();
            }
        }
        public IList CurrentRefSpectrums
        {
            get => this._currentRefSpectrums;
            set
            {
                this._currentRefSpectrums = value;
                CheckEnabledStart();
            }
        }
        public DateTime? DateStart
        {
            get => this._dateStart;
            set => this.Set(ref this._dateStart, value);
        }
        public DateTime? DateStop
        {
            get => this._dateStop;
            set => this.Set(ref this._dateStop, value);
        }
        public bool IsEnabledStart
        {
            get => this._isEnabledStart;
            set => this.Set(ref this._isEnabledStart, value);
        }
        private void CheckEnabledStart()
        {
            if (this._currentSensors.Count > 0 && this._currentAreas.Count > 0 && this._currentRefSpectrums.Count > 0 && this._dateStart <= this._dateStop)
                IsEnabledStart = true;
            else
                IsEnabledStart = false;
        }
        private void ReloadData()
        {
            this.DateStart = DateAndTime.DateSerial(DateTime.Today.Year, DateTime.Today.Month, 1);
            this.DateStop = this.DateStart.Value.AddMonths(1).AddDays(-1);

            this.ReloadSensors();
            this.ReloadAreas();
            this.ReloadRefSpectrums();
        }
        private void ReloadSensors()
        {
            var sdrSensors = SVC.SdrnsControllerWcfClient.GetShortSensors();
            this._sensors.Source = sdrSensors;
            if (sdrSensors.Length == 1)
            {
                this._currentSensors = new List<ShortSensorViewModel>() { Mappers.Map(sdrSensors[0]) };
                RedrawMap();
            }
        }
        private void ReloadAreas()
        {
            var areas = new List<SDRI.Area>();

            IMRecordset rs = new IMRecordset("AREA", IMRecordset.Mode.ReadOnly);
            rs.Select("ID,NAME,DENSITY,CREATED_BY,DATE_CREATED,POINTS,CSYS");
            for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
            {
                var area = new SDRI.Area()
                {
                    IdentifierFromICSM = rs.GetI("ID"),
                    Name = rs.GetS("NAME"),
                    TypeArea = rs.GetS("DENSITY"),
                    CreatedBy = rs.GetS("CREATED_BY"),
                    DateCreated = rs.GetT("DATE_CREATED")
                };
                areas.Add(area);

                string csys = rs.GetS("CSYS");

                if (!"4DEC".Equals(csys, StringComparison.OrdinalIgnoreCase) && !"4DMS".Equals(csys, StringComparison.OrdinalIgnoreCase))
                    continue;

                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                var pointsString = rs.GetS("POINTS");
                var points = new List<SDRI.DataLocation>();

                if (!string.IsNullOrEmpty(pointsString))
                {
                    foreach (var a in pointsString.Split(new[] { "\r\n" }, StringSplitOptions.None))
                    {
                        if (!string.IsNullOrEmpty(a))
                        {
                            string[] b = a.Split(new[] { "\t" }, StringSplitOptions.None);
                            if (b.Length == 2)
                            {
                                double k1;
                                double k2;
                                if (double.TryParse(b[0].Replace(".", sep), out k1) && double.TryParse(b[1].Replace(".", sep), out k2))
                                {
                                    if ("4DMS".Equals(csys, StringComparison.OrdinalIgnoreCase))
                                    {
                                        var point = new SDRI.DataLocation()
                                        {
                                            Longitude = IMPosition.Dms2Dec(k1),
                                            Latitude = IMPosition.Dms2Dec(k2)
                                        };
                                        points.Add(point);
                                    }
                                    else
                                    {
                                        var point = new SDRI.DataLocation()
                                        {
                                            Longitude = k1,
                                            Latitude = k2
                                        };
                                        points.Add(point);
                                    }
                                }
                            }
                        }
                    }
                }
                if (points.Count > 0)
                    area.Location = points.ToArray();
            }
            if (rs.IsOpen())
                rs.Close();
            rs.Destroy();

            this._areas.Source = areas.ToArray();
        }
        private void ReloadRefSpectrums()
        {
            var spectrums = SVC.SdrnsControllerWcfClientIeStation.GetAllRefSpectrum();
            this._refSpectrums.Source = spectrums;
            if (spectrums.Length == 1)
            {
                this._currentRefSpectrums = new List<RefSpectrumViewModel>() { Mappers.Map(spectrums[0]) };
            }
        }
        private void SelectSensors()
        {
            var listSensors = new List<ShortSensorViewModel>();
            var listSensorsIndexes = new List<long>();
            long index = 0;

            var sensorsIds = new Dictionary<SDR.SensorIdentifier, SDR.ShortSensor>();

            if (this._currentAreas != null && this._sensors.Source != null && this._sensors.Source.Length > 0)
            {
                foreach (var sensor in this._sensors.Source)
                {
                    var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(sensor.Id.Value);
                    if (svcSensor != null)
                    {
                        if (svcSensor.Locations != null && svcSensor.Locations.Length > 0)
                        {
                            foreach (var loc in svcSensor.Locations
                                                        .Where(l => ("A".Equals(l.Status, StringComparison.OrdinalIgnoreCase)
                                                                || "Z".Equals(l.Status, StringComparison.OrdinalIgnoreCase))
                                                                && l.Lon.HasValue
                                                                && l.Lat.HasValue)
                                                        .ToArray())
                            {
                                foreach (AreasViewModel area in this._currentAreas)
                                {
                                    if (CheckHitting(area.Location, loc))
                                    {
                                        if (!sensorsIds.ContainsKey(sensor.Id))
                                        {
                                            sensorsIds.Add(sensor.Id, sensor);
                                            listSensorsIndexes.Add(index);
                                        }
                                            
                                    }
                                }
                            }
                        }
                    }
                    index++;
                }
            }

            foreach (var sensor in sensorsIds.Values)
            {
                listSensors.Add(Mappers.Map(sensor));
            }

            this._currentSensors = listSensors;
            this.CurrentSensorsIndexes = listSensorsIndexes.ToArray();
        }
        public bool CheckHitting(SDRI.DataLocation[] poligon, SDR.SensorLocation sensor)
        {
            if (poligon == null || poligon.Length == 0)
                return false;


            bool hit = false; // количество пересечений луча слева в право четное = false, нечетное = true;
            for (int i = 0; i < poligon.Length - 1; i++)
            {
                if (((poligon[i].Latitude <= sensor.Lat) && ((poligon[i + 1].Latitude > sensor.Lat))) || ((poligon[i].Latitude > sensor.Lat) && ((poligon[i + 1].Latitude <= sensor.Lat))))
                {
                    if ((poligon[i].Longitude > sensor.Lon) && (poligon[i + 1].Longitude > sensor.Lon))
                    {
                        hit = !hit;
                    }
                    else if (!((poligon[i].Longitude < sensor.Lon) && (poligon[i + 1].Longitude < sensor.Lon)))
                    {
                        if (sensor.Lon < poligon[i + 1].Longitude - (sensor.Lat - poligon[i + 1].Latitude) * (poligon[i + 1].Longitude - poligon[i].Longitude) / (poligon[i].Latitude - poligon[i + 1].Latitude))
                        {
                            hit = !hit;
                        }
                    }
                }
            }
            int i_ = poligon.Length - 1;
            if (((poligon[i_].Latitude <= sensor.Lat) && ((poligon[0].Latitude > sensor.Lat))) || ((poligon[i_].Latitude > sensor.Lat) && ((poligon[0].Latitude <= sensor.Lat))))
            {
                if ((poligon[i_].Longitude > sensor.Lon) && (poligon[0].Longitude > sensor.Lon))
                {
                    hit = !hit;
                }
                else if (!((poligon[i_].Longitude < sensor.Lon) && (poligon[0].Longitude < sensor.Lon)))
                {
                    if (sensor.Lon < poligon[0].Longitude - (sensor.Lat - poligon[0].Latitude) * (poligon[0].Longitude - poligon[i_].Longitude) / (poligon[i_].Latitude - poligon[0].Latitude))
                    {
                        hit = !hit;
                    }
                }
            }

            return hit;
        }
        private void OnProtocolCommand(object parameter)
        {
            var dlgForm = new FM.GroupeEmissionWithStationProtocolForm();
            dlgForm.ShowDialog();
            dlgForm.Dispose();
        }
        private void OnStartCommand(object parameter)
        {
            try
            {
                if (!DateStart.HasValue)
                {
                    MessageBox.Show("Undefined Date Start!");
                    return;
                }
                if (!DateStop.HasValue)
                {
                    MessageBox.Show("Undefined Date Stop!");
                    return;
                }
                if (DateStart > DateStop)
                {
                    MessageBox.Show("Date Stop should be great of the Date Start!");
                    return;
                }
                if (SVC.SdrnsControllerWcfClientIeStation.CurrentDataSynchronizationProcess() != null)
                {
                    MessageBox.Show("Synchronization process already running!");
                    return;
                }

                var dataSynchronization = new SDRI.DataSynchronizationBase()
                {
                    DateStart = DateStart.Value,
                    DateEnd = DateStop.Value,
                    DateCreated = DateTime.Now,
                    CreatedBy = IM.ConnectedUser()
                };

                //ReloadRefSpectrums();

                var RefSpectrumIdsBySDRN = new List<long>();
                var stationsExtended = new Dictionary<string, SDRI.StationExtended>();
                foreach (RefSpectrumViewModel spectrum in this._currentRefSpectrums)
                {
                    RefSpectrumIdsBySDRN.Add(spectrum.Id.Value);

                    foreach (var dataSpectrum in spectrum.DataRefSpectrum)
                    {
                        if (stationsExtended.ContainsKey(dataSpectrum.TableName + "/" + dataSpectrum.TableId))
                            continue;

                        var stationExtended = new SDRI.StationExtended();

                        IMRecordset rs = new IMRecordset(dataSpectrum.TableName, IMRecordset.Mode.ReadOnly);
                        rs.Select("Position.NAME,Position.REMARK,Position.LATITUDE,Position.LONGITUDE,BW,Owner.NAME,STANDARD,RadioSystem.DESCRIPTION,Position.PROVINCE,DESIG_EMISSION,NAME,Owner.CODE,STATUS");
                        rs.SetWhere("ID", IMRecordset.Operation.Eq, dataSpectrum.TableId);
                        for (rs.Open(); !rs.IsEOF(); rs.MoveNext())
                        {
                            stationExtended.Address = rs.GetS("Position.REMARK");
                            stationExtended.Location = new SDRI.DataLocation() { Latitude = rs.GetD("Position.LATITUDE"), Longitude = rs.GetD("Position.LONGITUDE") };
                            stationExtended.BandWidth = rs.GetD("BW");
                            stationExtended.OwnerName = rs.GetS("Owner.NAME");
                            stationExtended.DesigEmission = rs.GetS("DESIG_EMISSION");
                            stationExtended.Standard = rs.GetS("STANDARD");
                            stationExtended.StandardName = rs.GetS("RadioSystem.DESCRIPTION");
                            stationExtended.Province = rs.GetS("Position.PROVINCE");
                            stationExtended.CurentStatusStation = rs.GetS("STATUS");
                            stationExtended.OKPO = rs.GetS("Owner.CODE");
                            stationExtended.StationName = rs.GetS("NAME");

                            string mobstafreq_table = "MOBSTA_FREQS";
                            if (dataSpectrum.TableName.Substring(dataSpectrum.TableName.Length - 1) == "2")
                                mobstafreq_table = "MOBSTA_FREQS2";

                            var rssta = new IMRecordset(mobstafreq_table, IMRecordset.Mode.ReadOnly);
                            rssta.Select("ID,TX_FREQ,RX_FREQ,ChannelTx.CHANNEL");
                            rssta.SetWhere("STA_ID", IMRecordset.Operation.Eq, dataSpectrum.TableId);

                            var txFreq = new List<float>();
                            var rxFreq = new List<float>();
                            var chanels = new List<string>();

                            for (rssta.Open(); !rssta.IsEOF(); rssta.MoveNext())
                            {
                                var txfrq = rssta.GetD("TX_FREQ");
                                if (txfrq != IM.NullD)
                                    txFreq.Add((float)txfrq);
                                var rxfrq = rssta.GetD("RX_FREQ");
                                if (rxfrq != IM.NullD)
                                    rxFreq.Add((float)rxfrq);
                                var chanel = rssta.GetS("ChannelTx.CHANNEL");
                                chanels.Add(chanel);
                            }
                            if (rssta.IsOpen())
                                rssta.Close();
                            rssta.Destroy();

                            stationExtended.StationTxFreq = txFreq.ToArray();
                            stationExtended.StationRxFreq = rxFreq.ToArray();
                            stationExtended.StationChannel = chanels.ToArray();
                            stationExtended.TableId = dataSpectrum.TableId;
                            stationExtended.TableName = dataSpectrum.TableName;
                        }
                        if (rs.IsOpen())
                            rs.Close();
                        rs.Destroy();

                        int applId = IM.NullI;
                        IMRecordset rsAppl = new IMRecordset("XNRFA_APPL", IMRecordset.Mode.ReadOnly);
                        rsAppl.Select("ID,DOZV_DATE_CANCEL,DOZV_NUM,DOZV_DATE_FROM,DOZV_DATE_TO");
                        rsAppl.SetWhere("OBJ_TABLE", IMRecordset.Operation.Eq, dataSpectrum.TableName);
                        rsAppl.SetAdditional(string.Format("([OBJ_ID1]={0}) OR ([OBJ_ID2]={0}) OR ([OBJ_ID3]={0}) OR ([OBJ_ID4]={0}) OR ([OBJ_ID5]={0}) OR ([OBJ_ID6]={0})", dataSpectrum.TableId));
                        for (rsAppl.Open(); !rsAppl.IsEOF(); rsAppl.MoveNext())
                        {
                            stationExtended.PermissionNumber = rsAppl.GetS("DOZV_NUM");
                            if (rsAppl.GetT("DOZV_DATE_FROM") != IM.NullT)
                            {
                                stationExtended.PermissionStart = rsAppl.GetT("DOZV_DATE_FROM");
                            }
                            if (rsAppl.GetT("DOZV_DATE_TO") != IM.NullT)
                            {
                                stationExtended.PermissionStop = rsAppl.GetT("DOZV_DATE_TO");
                            }
                            if (rsAppl.GetT("DOZV_DATE_CANCEL") != IM.NullT)
                            {
                                stationExtended.PermissionCancelDate = rsAppl.GetT("DOZV_DATE_CANCEL");
                            }
                            applId = rsAppl.GetI("ID");
                        }
                        if (rsAppl.IsOpen())
                            rsAppl.Close();
                        rsAppl.Destroy();

                        if (applId != IM.NullI)
                        {
                            IMRecordset rs3 = new IMRecordset("XNRFA_PAC_TO_APPL", IMRecordset.Mode.ReadOnly);
                            rs3.Select("DOC_NUM_TV,DOC_DATE,DOC_END_DATE");
                            rs3.SetWhere("APPL_ID", IMRecordset.Operation.Eq, applId);

                            for (rs3.Open(); !rs3.IsEOF(); rs3.MoveNext())
                            {
                                stationExtended.DocNum = rs3.GetS("DOC_NUM_TV");
                                if (rs3.GetT("DOC_DATE") != IM.NullT)
                                {
                                    stationExtended.TestStartDate = rs3.GetT("DOC_DATE");
                                }
                                if (rs3.GetT("DOC_END_DATE") != IM.NullT)
                                {
                                    stationExtended.TestStopDate = rs3.GetT("DOC_END_DATE");
                                }
                            }
                            if (rs3.IsOpen())
                                rs3.Close();
                            rs3.Destroy();
                        }

                        //stationExtended.PermissionCancelDate = ;
                        //PermissionCancelDate <->PERM_DATE_STOP

                        if (stationExtended.TableId > 0)
                            stationsExtended.Add(dataSpectrum.TableName + "/" + dataSpectrum.TableId, stationExtended);
                    }
                }

                var sensorIdsBySDRN = new List<long>();
                foreach (ShortSensorViewModel sensor in this._currentSensors)
                    sensorIdsBySDRN.Add(sensor.Id);

                var areas = new List<SDRI.Area>();
                foreach (AreasViewModel areaModel in this._currentAreas)
                {
                    var area = new SDRI.Area()
                    {
                        Name = areaModel.Name,
                        TypeArea = areaModel.TypeArea,
                        CreatedBy = areaModel.CreatedBy,
                        DateCreated = areaModel.DateCreated,
                        Location = areaModel.Location,
                        IdentifierFromICSM = areaModel.IdentifierFromICSM
                    };
                    areas.Add(area);
                }

                if (SVC.SdrnsControllerWcfClientIeStation.RunDataSynchronizationProcess(dataSynchronization, RefSpectrumIdsBySDRN.ToArray(), sensorIdsBySDRN.ToArray(), areas.ToArray(), stationsExtended.Values.ToArray()))
                    MessageBox.Show(Properties.Resources.Message_ProcessStartedSuccessfully);
                else
                    MessageBox.Show(Properties.Resources.Message_GSIDSyncWithEmissionsCouldNotBeStarted);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnDeleteRefSpectrumCommand(object parameter)
        {
            try
            {
                var RefSpectrumIdsBySDRN = new List<long>();
                foreach (RefSpectrumViewModel spectrum in this._currentRefSpectrums)
                    RefSpectrumIdsBySDRN.Add(spectrum.Id.Value);

                SVC.SdrnsControllerWcfClientIeStation.DeleteRefSpectrum(RefSpectrumIdsBySDRN.ToArray());
                this.ReloadRefSpectrums();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnImportRefSpectrumFromCSVCommand(object parameter)
        {
            try
            {
                string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                FRM.OpenFileDialog openFile = new FRM.OpenFileDialog() { Filter = "Текстовые файлы(*.csv)|*.csv", Title = "Ref spectrum" };
                if (openFile.ShowDialog() == FRM.DialogResult.OK)
                {
                    var _waitForm = new FM.WaitForm();
                    _waitForm.SetMessage("Loading file. Please wait...");
                    _waitForm.TopMost = true;
                    _waitForm.Show();
                    _waitForm.Refresh();

                    var refSpec = new SDRI.RefSpectrum()
                    {
                        FileName = openFile.FileName,
                        DateCreated = DateTime.Now,
                        CreatedBy = IM.ConnectedUser()
                    };
                    var refSpecData = new List<SDRI.DataRefSpectrum>();

                    using (TextFieldParser parser = new TextFieldParser(openFile.FileName))
                    {
                        int i = 0;
                        try
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(";");
                            while (!parser.EndOfData)
                            {
                                var record = parser.ReadFields();

                                if (i >= 1)
                                {
                                    DateTime dateMeas = DateTime.ParseExact(record[10], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                                    var refSpecDataLine = new SDRI.DataRefSpectrum()
                                    {
                                        IdNum = record[0].TryToInt(),
                                        TableName = record[1].ToString(),
                                        TableId = record[2].TryToInt(),
                                        SensorId = record[3].TryToInt(),
                                        GlobalSID = record[4].ToString(),
                                        Freq_MHz = record[5].Replace(".", sep).TryToDouble().Value,
                                        Level_dBm = record[6].Replace(".", sep).TryToDouble().Value,
                                        DispersionLow = record[7].Replace(".", sep).TryToDouble(),
                                        DispersionUp = record[8].Replace(".", sep).TryToDouble(),
                                        Percent = record[9].Replace(".", sep).TryToDouble(),
                                        DateMeas = dateMeas
                                    };

                                    refSpecData.Add(refSpecDataLine);
                                }
                                i++;
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Incorrect format file: " + openFile.FileName + "!\r\n" + "Line: " + i.ToString() + "\r\n" + e.Message);
                            _waitForm.Close();
                            return;
                        }
                    }
                    refSpec.DataRefSpectrum = refSpecData.ToArray();
                    _waitForm.Close();

                    var id = SVC.SdrnsControllerWcfClientIeStation.ImportRefSpectrum(refSpec);
                    this.ReloadRefSpectrums();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void RedrawMap()
        {
            var data = new MP.MapDrawingData();
            var points = new List<MP.MapDrawingDataPoint>();
            var polygons = new List<MP.MapDrawingDataPolygon>();

            if (this._currentSensors != null)
                foreach (ShortSensorViewModel sensor in this._currentSensors)
                    DrawSensor(points, sensor.Id);
            else if (this._sensors.Source != null && this._sensors.Source.Length > 0)
                foreach (var sensor in this._sensors.Source)
                    DrawSensor(points, sensor.Id.Value);

            if (this._currentAreas != null)
                foreach (AreasViewModel area in this._currentAreas)
                {
                    if (area.Location != null)
                    {
                        var polygonPoints = new List<Location>();
                        foreach (var point in area.Location)
                        {
                            polygonPoints.Add(new Location(point.Longitude, point.Latitude));
                        }
                        polygons.Add(new MP.MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
                    }
                }

            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();
            this.CurrentMapData = data;
        }
        private void DrawSensor(List<MP.MapDrawingDataPoint> points, long sensorId)
        {
            var svcSensor = SVC.SdrnsControllerWcfClient.GetSensorById(sensorId);
            if (svcSensor != null)
            {
                var modelSensor = Mappers.Map(svcSensor);
                if (modelSensor.Locations != null && modelSensor.Locations.Length > 0)
                {
                    var sensorPoints = modelSensor.Locations
                        .Where(l => ("A".Equals(l.Status, StringComparison.OrdinalIgnoreCase)
                                || "Z".Equals(l.Status, StringComparison.OrdinalIgnoreCase))
                                && l.Lon.HasValue
                                && l.Lat.HasValue)
                        .Select(l => this.MakeDrawingPointForSensor(l.Status, l.Lon.Value, l.Lat.Value))
                        .ToArray();
                    points.AddRange(sensorPoints);
                }
            }
        }
        private MP.MapDrawingDataPoint MakeDrawingPointForSensor(string status, double lon, double lat)
        {
            return new MP.MapDrawingDataPoint
            {
                Color = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Fill = "A".Equals(status, StringComparison.OrdinalIgnoreCase) ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.Silver,
                Location = new Models.Location(lon, lat),
                Opacity = 0.85,
                Width = 10,
                Height = 10
            };
        }
    }
}
