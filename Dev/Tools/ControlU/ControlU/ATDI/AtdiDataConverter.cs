using System;
using System.Collections.Generic;
using System.Linq;
using Atdi.DataModels.Sdrns.Device;
using System.Collections.ObjectModel;
using ControlU.DB;

namespace ControlU.ATDI
{
    public class AtdiDataConverter
    {
        public LocalMeasSdrTask_v2 ConvertToLocal(MeasTask task)
        {
            localatdi_meas_task to = new localatdi_meas_task() { };
            string errors = "";
            bool doubleerror = false;

            to.task_id = task.TaskId.Replace("|","I");
            to.equipment_tech_id = task.EquipmentTechId;
            to.xms_eqipment = Array.ConvertAll(task.MobEqipmentMeasurements, value => (int)value); // проверить
            to.priority = task.Priority;
            to.scan_per_task_number = task.ScanPerTaskNumber;
            to.sdrn_server = task.SdrnServer;
            to.sensor_name = task.SensorName;
            to.time_start = task.StartTime;
            to.time_stop = task.StopTime;
            to.time_save = DateTime.Now;
            to.status = task.Status;
            to.ResultsInfo = new ObservableCollection<localatdi_result_state_data>() { };
            if (task.Stations != null && task.Stations.Length > 0)
            {
                #region уникальные технологии в таске

                ObservableCollection<localatdi_task_with_tech> tablenames = new ObservableCollection<localatdi_task_with_tech>() { };
                System.Collections.Generic.HashSet<string> hs = new System.Collections.Generic.HashSet<string>();
                if (task.Stations != null && task.Stations.Length > 0)
                {
                    foreach (Atdi.DataModels.Sdrns.Device.MeasuredStation al in task.Stations)
                    {
                        hs.Add(MainWindow.help.LeaveOnlyLetters(al.Standard).Replace("-", "").ToUpper());
                    }
                }
                List<string> tech = hs.ToList();
                foreach (string ss in tech)
                {
                    localatdi_task_with_tech twt = new localatdi_task_with_tech()
                    {
                        task_table_name = "atdi_task_v2_" + task.TaskId.Replace("|", "I") + "_" + ss,
                        result_table_name = "atdi_task_result_v2_" + task.TaskId.Replace("|", "I") + "_" + ss,
                        tech = ss,
                        TaskItems = new ObservableCollection<localatdi_station>(),
                        ResultItems = new ObservableCollection<localatdi_result_item>()
                    };
                    List<localatdi_standard_scan_parameter> ssps = new List<localatdi_standard_scan_parameter>();
                    for (int i = 0; i < task.ScanParameters.Length; i++)
                    {
                        if (ss.Contains(task.ScanParameters[i].Standard.ToUpper()))
                        {
                            localatdi_standard_scan_parameter ssp = new localatdi_standard_scan_parameter() { };
                            ssp.detection_level_dbm = (double)task.ScanParameters[i].DetectionLevel_dBm;
                            ssp.max_frequency_relative_offset_mk = (decimal)task.ScanParameters[i].MaxFrequencyRelativeOffset_mk;
                            ssp.max_permission_bw = ((decimal)task.ScanParameters[i].MaxPermissionBW_kHz) * 1000;
                            ssp.standard = task.ScanParameters[i].Standard.ToUpper();
                            ssp.xdb_level_db = (decimal)task.ScanParameters[i].XdBLevel_dB;
                            ssp.detector_type = (int)task.ScanParameters[i].DeviceParam.DetectType;
                            ssp.meas_time_sec = (decimal)task.ScanParameters[i].DeviceParam.MeasTime_sec;
                            ssp.preamplification_db = (int)task.ScanParameters[i].DeviceParam.Preamplification_dB;
                            ssp.rbw = ((decimal)task.ScanParameters[i].DeviceParam.RBW_kHz) * 1000;
                            ssp.vbw = ((decimal)task.ScanParameters[i].DeviceParam.VBW_kHz) * 1000;
                            ssp.ref_level_dbm = (decimal)task.ScanParameters[i].DeviceParam.RefLevel_dBm;
                            ssp.rf_attenuation_db = (decimal)task.ScanParameters[i].DeviceParam.RfAttenuation_dB;
                            ssp.meas_span = ((decimal)task.ScanParameters[i].DeviceParam.ScanBW_kHz) * 1000;
                            ssps.Add(ssp);
                        }
                    }
                    twt.scan_parameters = ssps.ToArray();
                    tablenames.Add(twt);
                }

                localatdi_task_with_tech twtunk = new localatdi_task_with_tech()
                {
                    task_table_name = "atdi_task_v2_" + task.TaskId.Replace("|", "I") + "_unknown",
                    result_table_name = "atdi_task_result_v2_" + task.TaskId.Replace("|", "I") + "_unknown",
                    tech = "unknown",
                    scan_parameters = new localatdi_standard_scan_parameter[] { },
                    TaskItems = new ObservableCollection<localatdi_station>(),
                    ResultItems = new ObservableCollection<localatdi_result_item>()
                };
                tablenames.Add(twtunk);
                to.data_from_tech = tablenames;
                to.routes_tb_name = "atdi_task_v2_" + task.TaskId.Replace("|", "I") + "_routes";
                to.routes = new ObservableCollection<localatdi_route_point>() { };
                #endregion
                #region распихиваем станции по их местам
                to.task_station_count = 0;
                for (int i = 0; i < to.data_from_tech.Count(); i++)
                {
                    for (int j = 0; j < task.Stations.Length; j++)
                    {
                        if (task.Stations[j].Standard.ToUpper().Contains(to.data_from_tech[i].tech))
                        {
                            localatdi_station st = new localatdi_station() { };
                            st.id = task.Stations[j].StationId;
                            st.callsign_db = "";
                            st.callsign_radio = "";
                            st.standard = to.data_from_tech[i].tech;// task.Stations[j].Standard.ToUpper();
                            if (task.Stations[j].GlobalSid != null && task.Stations[j].GlobalSid != "") st.callsign_db = task.Stations[j].GlobalSid;
                            if (task.Stations[j].OwnerGlobalSid != null && task.Stations[j].OwnerGlobalSid != "") st.callsign_radio = task.Stations[j].OwnerGlobalSid;

                            #region parse GlobalCid
                            int db_s0 = 0, ra_s0 = 0;
                            int db_s1 = 0, ra_s1 = 0;
                            int db_s2 = 0, ra_s2 = 0;
                            int db_s3 = 0, ra_s3 = 0;
                            if (st.standard.Contains("GSM") || st.standard.Contains("UMTS") || st.standard.Contains("LTE") || st.standard.Contains("CDMA"))
                            {
                                try
                                {
                                    if (st.callsign_db != null)
                                    {
                                        string[] db_s = st.callsign_db.Split(' ');
                                        if (db_s.Length == 4)
                                        {
                                            int.TryParse(db_s[0], out db_s0);
                                            int.TryParse(db_s[1], out db_s1);
                                            int.TryParse(db_s[2], out db_s2);
                                            int.TryParse(db_s[3], out db_s3);
                                            //db_s0 = Convert.ToInt32(db_s[0]);
                                            //db_s1 = Convert.ToInt32(db_s[1]);
                                            //db_s2 = Convert.ToInt32(db_s[2]);
                                            //db_s3 = Convert.ToInt32(db_s[3]);
                                        }
                                    }
                                }
                                catch (Exception exp)
                                {
                                    ControlU.App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                    {
                                        ControlU.MainWindow.exp.ExceptionData = new ControlU.ExData() { ex = exp, ClassName = "AtdiDataConverter", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + " station_id: " + st.id };
                                    }));
                                }
                                try
                                {
                                    if (st.callsign_radio != null)
                                    {
                                        string[] ra_s = st.callsign_radio.Split(' ');
                                        if (ra_s.Length == 4)
                                        {

                                            int.TryParse(ra_s[0], out ra_s0);
                                            int.TryParse(ra_s[1], out ra_s1);
                                            int.TryParse(ra_s[2], out ra_s2);
                                            int.TryParse(ra_s[3], out ra_s3);
                                            //ra_s0 = Convert.ToInt32(ra_s[0]);
                                            //ra_s1 = Convert.ToInt32(ra_s[1]);
                                            //ra_s2 = Convert.ToInt32(ra_s[2]);
                                            //ra_s3 = Convert.ToInt32(ra_s[3]);

                                        }
                                    }
                                }
                                catch (Exception exp)
                                {
                                    ControlU.App.Current.Dispatcher.BeginInvoke((Action)(() =>
                                    {
                                        ControlU.MainWindow.exp.ExceptionData = new ControlU.ExData() { ex = exp, ClassName = "AtdiDataConverter", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name + " station_id: " + st.id };
                                    }));

                                }
                            }
                            st.Callsign_db_S0 = db_s0;
                            st.Callsign_db_S1 = db_s1;
                            st.Callsign_db_S2 = db_s2;
                            st.Callsign_db_S3 = db_s3;

                            st.Callsign_radio_S0 = ra_s0;
                            st.Callsign_radio_S1 = ra_s1;
                            st.Callsign_radio_S2 = ra_s2;
                            st.Callsign_radio_S3 = ra_s3;
                            #endregion
                            st.status = "";
                            if (task.Stations[j].Status != null && task.Stations[j].Status != "") st.status = task.Stations[j].Status;
                            #region license_info
                            localatdi_license_info lic = new localatdi_license_info() { };
                            lic.name = task.Stations[j].License.Name;//проверку
                            lic.start_date = (DateTime)task.Stations[j].License.StartDate;//проверку
                            lic.close_date = (DateTime)task.Stations[j].License.CloseDate;//проверку
                            lic.end_date = (DateTime)task.Stations[j].License.EndDate;//проверку
                            if (task.Stations[j].License.IcsmId == null)
                            { lic.icsm_id = j; }
                            else { lic.icsm_id = (int)task.Stations[j].License.IcsmId; }//проверку
                            st.license = lic;
                            #endregion license_info
                            #region Owner
                            localatdi_station_owner ow = new localatdi_station_owner() { };
                            ow.address = task.Stations[j].Owner.Address;//проверку
                            ow.code = task.Stations[j].Owner.Code;//проверку
                            ow.id = task.Stations[j].Owner.Id;//проверку
                            ow.okpo = task.Stations[j].Owner.OKPO;//проверку
                            ow.name = task.Stations[j].Owner.OwnerName;//проверку
                            ow.zip = task.Stations[j].Owner.Zip;//проверку
                            st.owner = ow;
                            #endregion Owner
                            #region Site

                            localatdi_station_site si = new localatdi_station_site() { };
                            si.address = task.Stations[j].Site.Adress;//проверку
                            if (task.Stations[j].Site.Region == null) si.region = "";
                            else si.region = task.Stations[j].Site.Region;//проверку
                            si.location = new localatdi_geo_location()
                            {
                                latitude = (double)task.Stations[j].Site.Lat,
                                longitude = (double)task.Stations[j].Site.Lon,
                            };
                            //проверка
                            if (doubleerror == false &&
                                task.Stations[j].Site.Lat != null &&
                                task.Stations[j].Site.Lon != null &&
                                ((double)task.Stations[j].Site.Lat > 180 || (double)task.Stations[j].Site.Lat < -180) &&
                                ((double)task.Stations[j].Site.Lon > 180 || (double)task.Stations[j].Site.Lat < -180))
                            {
                                doubleerror = true;
                                errors += "double variable lost fractional part\r\n";
                            }
                            st.site = si;
                            #endregion Site
                            #region Sectors
                            ObservableCollection<localatdi_station_sector> secs = new ObservableCollection<localatdi_station_sector>();
                            for (int s = 0; s < task.Stations[j].Sectors.Length; s++)
                            {
                                localatdi_station_sector sec = new localatdi_station_sector() { };
                                sec.agl = (decimal)task.Stations[j].Sectors[s].AGL;//проверку
                                sec.azimuth = (decimal)task.Stations[j].Sectors[s].Azimuth;//проверку
                                sec.bw = ((decimal)task.Stations[j].Sectors[s].BW_kHz) * 1000;//проверку
                                sec.class_emission = task.Stations[j].Sectors[s].ClassEmission;//проверку
                                sec.eirp = (decimal)task.Stations[j].Sectors[s].EIRP_dBm;//проверку
                                sec.sector_id = task.Stations[j].Sectors[s].SectorId;//проверку
                                List<localatdi_sector_frequency> freqs = new List<localatdi_sector_frequency>();
                                for (int f = 0; f < task.Stations[j].Sectors[s].Frequencies.Length; f++)
                                {
                                    localatdi_sector_frequency freq = new localatdi_sector_frequency() { };
                                    freq.channel_number = (int)task.Stations[j].Sectors[s].Frequencies[f].ChannelNumber;//проверку
                                    freq.frequency = ((decimal)task.Stations[j].Sectors[s].Frequencies[f].Frequency_MHz) * 1000000;//проверку
                                    if (task.Stations[j].Sectors[s].Frequencies[f].Id == null) freq.id = f;
                                    else freq.id = (int)task.Stations[j].Sectors[s].Frequencies[f].Id;//проверку
                                    freq.id_plan = (int)task.Stations[j].Sectors[s].Frequencies[f].PlanId;//проверку
                                    freqs.Add(freq);
                                }
                                sec.frequencies = freqs.ToArray();
                                List<localatdi_elements_mask> masks = new List<localatdi_elements_mask>();
                                for (int m = 0; m < task.Stations[j].Sectors[s].BWMask.Length; m++)
                                {
                                    localatdi_elements_mask mask = new localatdi_elements_mask() { };
                                    mask.bw = ((decimal)task.Stations[j].Sectors[s].BWMask[m].BW_kHz) * 1000;
                                    mask.level = (decimal)task.Stations[j].Sectors[s].BWMask[m].Level_dB;
                                    masks.Add(mask);
                                }
                                sec.bw_mask = masks.ToArray();
                                secs.Add(sec);
                            }
                            st.sectors = secs;
                            #endregion Sectors

                            to.data_from_tech[i].TaskItems.Add(st);
                            to.task_station_count++;
                        }
                    }
                }

                #endregion
            }
            else
            {
                if (task.Stations == null) errors += "Task.Stations is null \r\n";
                if (task.Stations != null && task.Stations.Length == 0) errors += "Task.Stations.Length is 0 \r\n";
            }
            LocalMeasSdrTask_v2 outdata = new LocalMeasSdrTask_v2();
            outdata.MeasTask = to;
            outdata.Saved = false;
            outdata.Errors = errors;
            return outdata;
        }
        public Atdi.DataModels.Sdrns.Device.MeasResults ConvertToATDI(DB.localatdi_meas_task task)
        {
            Atdi.DataModels.Sdrns.Device.MeasResults to = new Atdi.DataModels.Sdrns.Device.MeasResults() { };
            to.ResultId = MainWindow.db_v2.ATDI_AddResultID().ToString();//"";//попровить на id
            to.Status = "";
            to.SwNumber = 0;
            to.TaskId = task.task_id.Replace("I", "|");
            to.Measured = DateTime.Now;
            DateTime StartTime = DateTime.MaxValue;
            DateTime StopTime = DateTime.MinValue;
            #region Routes            
            int routid = -1;
            List<Route> routes = new List<Atdi.DataModels.Sdrns.Device.Route>();
            Route route = new Route();
            List<RoutePoint> points = new List<RoutePoint>();
            for (int rs = 0; rs < task.routes.Count(); rs++)
            {
                if (routid != task.routes[rs].route_id)
                {
                    routid = task.routes[rs].route_id;
                    route.RoutePoints = points.ToArray();
                    route.RouteId = routid.ToString();

                    route = new Route();
                    points = new List<RoutePoint>();
                    routes.Add(route);
                }

                //route.RoutePoints = new RoutePoint[task.routes.Count()];
                RoutePoint point = new RoutePoint();
                if (task.routes[rs].location.agl > -100000)
                    point.AGL = (double)task.routes[rs].location.agl;
                if (task.routes[rs].location.asl > -100000)
                    point.ASL = (double)task.routes[rs].location.asl;
                point.Lat = (double)task.routes[rs].location.latitude;
                point.Lon = (double)task.routes[rs].location.longitude;
                point.StartTime = task.routes[rs].start_time;
                point.FinishTime = task.routes[rs].finish_time;
                point.PointStayType = (Atdi.DataModels.Sdrns.PointStayType)task.routes[rs].point_stay_type;
                points.Add(point);

                if (rs == task.routes.Count() - 1)
                {
                    route.RoutePoints = points.ToArray();
                    route.RouteId = routid.ToString();
                }
            }
            to.Routes = routes.ToArray();

            #endregion
            #region StationResults
            List<StationMeasResult> smrs = new List<StationMeasResult>() { };
            #region StationMeasResult
            for (int t = 0; t < task.data_from_tech.Count(); t++)
            {
                for (int rt = 0; rt < task.data_from_tech[t].ResultItems.Count(); rt++)
                {
                    StationMeasResult smr = new StationMeasResult() { };
                    smr.StationId = task.data_from_tech[t].ResultItems[rt].id_station;
                    smr.SectorId = task.data_from_tech[t].ResultItems[rt].id_sector;
                    smr.TaskGlobalSid = task.data_from_tech[t].ResultItems[rt].station_identifier_atdi;
                    smr.RealGlobalSid = task.data_from_tech[t].ResultItems[rt].station_identifier_from_radio;
                    smr.Standard = task.data_from_tech[t].tech;
                    smr.Status = task.data_from_tech[t].ResultItems[rt].status;
                    smr.GeneralResult = new Atdi.DataModels.Sdrns.Device.GeneralMeasResult() { };


                    Equipment.tracepoint[] trace = task.data_from_tech[t].ResultItems[rt].spec_data.Trace;
                    if (trace.Length > 0 && trace[0].level > -1000 && trace[trace.Length - 1].level > -1000)
                    {
                        float[] tr = new float[task.data_from_tech[t].ResultItems[rt].spec_data.Trace.Length];
                        for (int tt = 0; tt < task.data_from_tech[t].ResultItems[rt].spec_data.Trace.Length; tt++)
                        {
                            tr[tt] = (float)task.data_from_tech[t].ResultItems[rt].spec_data.Trace[tt].level;
                        }
                        if (task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1] > -1 &&
                            task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2] > -1)
                            smr.GeneralResult.CentralFrequencyMeas_MHz = (double)((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq + trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq) / 2000000);

                        smr.GeneralResult.LevelsSpectrum_dBm = tr;
                        smr.GeneralResult.SpectrumSteps_kHz = (task.data_from_tech[t].ResultItems[rt].spec_data.Trace[10].freq - task.data_from_tech[t].ResultItems[rt].spec_data.Trace[9].freq) / 1000;
                        smr.GeneralResult.SpectrumStartFreq_MHz = task.data_from_tech[t].ResultItems[rt].spec_data.FreqStart / 1000000;
                        smr.GeneralResult.RBW_kHz = (double)(task.data_from_tech[t].ResultItems[rt].spec_data.RBW / 1000);
                        smr.GeneralResult.VBW_kHz = (double)(task.data_from_tech[t].ResultItems[rt].spec_data.VBW / 1000);

                        smr.GeneralResult.BandwidthResult = new BandwidthMeasResult() { };
                        if (task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1] > -1 &&
                            task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2] > -1)
                        {
                            smr.GeneralResult.BandwidthResult.Bandwidth_kHz = (double)((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq - trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq) / 1000);
                            smr.GeneralResult.BandwidthResult.MarkerIndex = task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[0];
                            smr.GeneralResult.BandwidthResult.T1 = task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1];
                            smr.GeneralResult.BandwidthResult.T2 = task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2];
                        }
                        smr.GeneralResult.BandwidthResult.TraceCount = task.data_from_tech[t].ResultItems[rt].spec_data.TraceCount;
                        smr.GeneralResult.BandwidthResult.СorrectnessEstimations = task.data_from_tech[t].ResultItems[rt].meas_correctness;



                    }
                    if (trace.Length > 0 && trace[0].level > -1000 && trace[trace.Length - 1].level > -1000 &&
                        task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1] > -1 &&
                        task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2] > -1)
                    {
                        double OffsetFrequency = 0;
                        if (task.data_from_tech[t].ResultItems[rt].freq_centr_perm > 0)
                        {
                            OffsetFrequency = (double)Math.Round(((Math.Abs(((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq +
                                trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq) / 2) -
                                (task.data_from_tech[t].ResultItems[rt].freq_centr_perm))) / (task.data_from_tech[t].ResultItems[rt].freq_centr_perm / 1000000)), 3);
                        }
                        else if (task.data_from_tech[t].ResultItems[rt].freq_centr_perm < 0)
                        {
                            OffsetFrequency = (double)Math.Round(((Math.Abs(((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq +
                                trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq) / 2) -
                                (task.data_from_tech[t].ResultItems[rt].spec_data.FreqCentr))) / (task.data_from_tech[t].ResultItems[rt].spec_data.FreqCentr / 1000000)), 3);
                        }
                        smr.GeneralResult.OffsetFrequency_mk = OffsetFrequency;
                    }

                    if (task.data_from_tech[t].ResultItems[rt].freq_centr_perm > 0)
                        smr.GeneralResult.CentralFrequency_MHz = (double)(task.data_from_tech[t].ResultItems[rt].freq_centr_perm / 1000000);
                    else smr.GeneralResult.CentralFrequency_MHz = (double)(task.data_from_tech[t].ResultItems[rt].spec_data.FreqCentr / 1000000);
                    smr.GeneralResult.MeasDuration_sec = (double)task.data_from_tech[t].ResultItems[rt].spec_data.MeasDuration;
                    smr.GeneralResult.MeasStartTime = task.data_from_tech[t].ResultItems[rt].spec_data.MeasStart;
                    smr.GeneralResult.MeasFinishTime = task.data_from_tech[t].ResultItems[rt].spec_data.MeasStop;
                    if (smr.GeneralResult.MeasStartTime < StartTime && smr.GeneralResult.MeasStartTime != DateTime.MinValue)
                    {
                        StartTime = (DateTime)smr.GeneralResult.MeasStartTime;
                    }
                    if (smr.GeneralResult.MeasFinishTime > StopTime && smr.GeneralResult.MeasStartTime != DateTime.MinValue)
                    {
                        StopTime = (DateTime)smr.GeneralResult.MeasFinishTime;
                    }

                    List<ElementsMask> bwms = new List<ElementsMask>() { };
                    for (int b = 0; b < task.data_from_tech[t].ResultItems[rt].meas_mask.Length; b++)
                    {
                        ElementsMask bwm = new ElementsMask() { };
                        bwm.BW_kHz = (double)(task.data_from_tech[t].ResultItems[rt].meas_mask[b].bw / 1000);
                        bwm.Level_dB = (double)task.data_from_tech[t].ResultItems[rt].meas_mask[b].level;
                        bwms.Add(bwm);
                    }
                    smr.GeneralResult.BWMask = bwms.ToArray();
                    StationSysInfo ssi = new StationSysInfo();
                    #region ssi
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.bandwidth > -1)
                        ssi.BandWidth = (double)(task.data_from_tech[t].ResultItems[rt].station_sys_info.bandwidth / 1000);
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.base_id > -1)
                        ssi.BaseID = task.data_from_tech[t].ResultItems[rt].station_sys_info.base_id;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.bsic > -1)
                        ssi.BSIC = task.data_from_tech[t].ResultItems[rt].station_sys_info.bsic;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.channel_number > -1)
                        ssi.ChannelNumber = task.data_from_tech[t].ResultItems[rt].station_sys_info.channel_number;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.cid > -1)
                        ssi.CID = task.data_from_tech[t].ResultItems[rt].station_sys_info.cid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.code_power > -1000)
                        ssi.Code = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.code_power;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.ctoi > -1000)
                        ssi.CtoI = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.ctoi;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.eci > -1)
                        ssi.ECI = task.data_from_tech[t].ResultItems[rt].station_sys_info.eci;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.e_node_b_id > -1)
                        ssi.eNodeBId = task.data_from_tech[t].ResultItems[rt].station_sys_info.e_node_b_id;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.freq > -1)
                        ssi.Freq = (double)(task.data_from_tech[t].ResultItems[rt].station_sys_info.freq / 1000000);///////////////
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.icio > -1000)
                        ssi.IcIo = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.icio;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.inband_power > -1000)
                        ssi.INBAND_POWER = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.inband_power;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.iscp > -1000)
                        ssi.INBAND_POWER = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.iscp;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.lac > -1)
                        ssi.LAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.lac;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.lac > -1)
                        ssi.LAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.lac;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.mcc > -1)
                        ssi.MCC = task.data_from_tech[t].ResultItems[rt].station_sys_info.mcc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.mnc > -1)
                        ssi.MNC = task.data_from_tech[t].ResultItems[rt].station_sys_info.mnc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.nid > -1)
                        ssi.NID = task.data_from_tech[t].ResultItems[rt].station_sys_info.nid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.pci > -1)
                        ssi.PCI = task.data_from_tech[t].ResultItems[rt].station_sys_info.pci;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.pn > -1)
                        ssi.PN = task.data_from_tech[t].ResultItems[rt].station_sys_info.pn;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.power > -1000)
                        ssi.Power = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.power;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.ptotal > -1000)
                        ssi.Ptotal = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.ptotal;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.rnc > -1)
                        ssi.RNC = task.data_from_tech[t].ResultItems[rt].station_sys_info.rnc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.rscp > -1000)
                        ssi.RSCP = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.rscp;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.rsrq > -1000)
                        ssi.RSRQ = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.rsrq;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.sc > -1)
                        ssi.SC = task.data_from_tech[t].ResultItems[rt].station_sys_info.sc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.sid > -1)
                        ssi.SID = task.data_from_tech[t].ResultItems[rt].station_sys_info.sid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.tac > -1)
                        ssi.TAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.tac;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.ucid > -1)
                        ssi.TAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.ucid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.type_cdmaevdo != "")
                        ssi.TypeCDMAEVDO = task.data_from_tech[t].ResultItems[rt].station_sys_info.type_cdmaevdo;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks.Length > 0)
                    {
                        List<StationSysInfoBlock> ssibs = new List<StationSysInfoBlock>() { };
                        for (int bl = 0; bl < task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks.Length; bl++)
                        {
                            StationSysInfoBlock ssib = new StationSysInfoBlock() { };
                            ssib.Type = task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks[bl].type;
                            ssib.Data = task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks[bl].datastring;
                            ssibs.Add(ssib);
                        }
                        ssi.InfoBlocks = ssibs.ToArray();
                    }
                    ssi.Location = new Atdi.DataModels.Sdrns.GeoLocation() { };
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.location.asl > -100000)
                        ssi.Location.ASL = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.asl;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.location.agl > -100000)
                        ssi.Location.AGL = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.agl;
                    ssi.Location.Lat = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.latitude;
                    ssi.Location.Lon = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.longitude;
                    #endregion ssi
                    smr.GeneralResult.StationSysInfo = ssi;

                    List<LevelMeasResult> lmrs = new List<LevelMeasResult>() { };
                    for (int lm = 0; lm < task.data_from_tech[t].ResultItems[rt].level_results.Count(); lm++)
                    {
                        LevelMeasResult lmr = new LevelMeasResult() { };
                        lmr.MeasurementTime = task.data_from_tech[t].ResultItems[rt].level_results[lm].measurement_time;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].difference_time_stamp_ns > -1)
                            lmr.DifferenceTimeStamp_ns = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].difference_time_stamp_ns;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbm > -1000)
                            lmr.Level_dBm = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbm;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbmkvm > -1000)
                            lmr.Level_dBmkVm = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbmkvm;

                        lmr.Location = new Atdi.DataModels.Sdrns.GeoLocation() { };
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].location.agl > -100000)
                            lmr.Location.AGL = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.agl;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].location.asl > -100000)
                            lmr.Location.ASL = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.asl;
                        lmr.Location.Lat = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.latitude;
                        lmr.Location.Lon = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.longitude;

                        lmrs.Add(lmr);
                    }
                    smr.LevelResults = lmrs.ToArray();

                    smrs.Add(smr);
                }
            }
            #endregion StationMeasResult
            to.StationResults = smrs.ToArray();
            to.StartTime = StartTime;
            to.StopTime = StopTime;

            #endregion StationResults

            return to;
        }
        public Atdi.DataModels.Sdrns.Device.MeasResults ConvertToATDI(DB.localatdi_unknown_result task)
        {
            Atdi.DataModels.Sdrns.Device.MeasResults to = new Atdi.DataModels.Sdrns.Device.MeasResults() { };
            to.ResultId = MainWindow.db_v2.ATDI_AddResultID().ToString();//"";//попровить на id
            to.Status = "";
            to.SwNumber = 0;
            to.TaskId = "";// task.task_id;
            to.Measured = DateTime.Now;
            DateTime StartTime = DateTime.MaxValue;
            DateTime StopTime = DateTime.MinValue;
            #region Routes            
            int routid = -1;
            List<Route> routes = new List<Atdi.DataModels.Sdrns.Device.Route>();
            Route route = new Route();
            List<RoutePoint> points = new List<RoutePoint>();
            for (int rs = 0; rs < task.routes.Count(); rs++)
            {
                if (routid != task.routes[rs].route_id)
                {
                    routid = task.routes[rs].route_id;
                    route.RoutePoints = points.ToArray();
                    route.RouteId = routid.ToString();

                    route = new Route();
                    points = new List<RoutePoint>();
                    routes.Add(route);
                }

                //route.RoutePoints = new RoutePoint[task.routes.Count()];
                RoutePoint point = new RoutePoint();
                if (task.routes[rs].location.agl > -100000)
                    point.AGL = (double)task.routes[rs].location.agl;
                if (task.routes[rs].location.asl > -100000)
                    point.ASL = (double)task.routes[rs].location.asl;
                point.Lat = (double)task.routes[rs].location.latitude;
                point.Lon = (double)task.routes[rs].location.longitude;
                point.StartTime = task.routes[rs].start_time;
                point.FinishTime = task.routes[rs].finish_time;
                point.PointStayType = (Atdi.DataModels.Sdrns.PointStayType)task.routes[rs].point_stay_type;
                points.Add(point);

                if (rs == task.routes.Count() - 1)
                {
                    route.RoutePoints = points.ToArray();
                    route.RouteId = routid.ToString();
                }
            }
            to.Routes = routes.ToArray();

            #endregion
            #region StationResults
            List<StationMeasResult> smrs = new List<StationMeasResult>() { };
            #region StationMeasResult
            for (int t = 0; t < task.data_from_tech.Count(); t++)
            {
                for (int rt = 0; rt < task.data_from_tech[t].ResultItems.Count(); rt++)
                {
                    StationMeasResult smr = new StationMeasResult() { };
                    smr.StationId = task.data_from_tech[t].ResultItems[rt].id_station;
                    smr.SectorId = task.data_from_tech[t].ResultItems[rt].id_sector;
                    smr.TaskGlobalSid = task.data_from_tech[t].ResultItems[rt].station_identifier_atdi;
                    smr.RealGlobalSid = task.data_from_tech[t].ResultItems[rt].station_identifier_from_radio;
                    smr.Standard = task.data_from_tech[t].tech;
                    smr.Status = task.data_from_tech[t].ResultItems[rt].status;
                    smr.GeneralResult = new Atdi.DataModels.Sdrns.Device.GeneralMeasResult() { };

                    Equipment.tracepoint[] trace = task.data_from_tech[t].ResultItems[rt].spec_data.Trace;
                    if (trace.Length > 0 && trace[0].level > -1000 && trace[trace.Length - 1].level > -1000)
                    {
                        float[] tr = new float[task.data_from_tech[t].ResultItems[rt].spec_data.Trace.Length];
                        for (int tt = 0; tt < task.data_from_tech[t].ResultItems[rt].spec_data.Trace.Length; tt++)
                        {
                            tr[tt] = (float)task.data_from_tech[t].ResultItems[rt].spec_data.Trace[tt].level;
                        }
                        if (task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1] > -1 &&
                            task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2] > -1)
                            smr.GeneralResult.CentralFrequencyMeas_MHz = (double)((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq + trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq) / 2000000);

                        smr.GeneralResult.LevelsSpectrum_dBm = tr;
                        smr.GeneralResult.SpectrumSteps_kHz = (task.data_from_tech[t].ResultItems[rt].spec_data.Trace[10].freq - task.data_from_tech[t].ResultItems[rt].spec_data.Trace[9].freq) / 1000;
                        smr.GeneralResult.SpectrumStartFreq_MHz = task.data_from_tech[t].ResultItems[rt].spec_data.FreqStart / 1000000;
                        smr.GeneralResult.RBW_kHz = (double)(task.data_from_tech[t].ResultItems[rt].spec_data.RBW / 1000);
                        smr.GeneralResult.VBW_kHz = (double)(task.data_from_tech[t].ResultItems[rt].spec_data.VBW / 1000);

                        if (task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1] > -1 &&
                            task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2] > -1)
                        {
                            smr.GeneralResult.BandwidthResult = new BandwidthMeasResult() { };
                            smr.GeneralResult.BandwidthResult.Bandwidth_kHz = (double)((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq - trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq) / 1000);
                            smr.GeneralResult.BandwidthResult.MarkerIndex = task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[0];
                            smr.GeneralResult.BandwidthResult.T1 = task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1];
                            smr.GeneralResult.BandwidthResult.T2 = task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2];
                        }
                        smr.GeneralResult.BandwidthResult.TraceCount = task.data_from_tech[t].ResultItems[rt].spec_data.TraceCount;
                        smr.GeneralResult.BandwidthResult.СorrectnessEstimations = task.data_from_tech[t].ResultItems[rt].meas_correctness;
                    }
                    if (trace.Length > 0 && trace[0].level > -1000 && trace[trace.Length - 1].level > -1000 &&
                        task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1] > -1 &&
                        task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2] > -1)
                    {
                        double OffsetFrequency = 0;
                        if (task.data_from_tech[t].ResultItems[rt].freq_centr_perm > 0)
                        {
                            OffsetFrequency = (double)Math.Round(((Math.Abs(((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq +
                                trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq) / 2) -
                                (task.data_from_tech[t].ResultItems[rt].freq_centr_perm))) / (task.data_from_tech[t].ResultItems[rt].freq_centr_perm / 1000000)), 3);
                        }
                        else if (task.data_from_tech[t].ResultItems[rt].freq_centr_perm < 0)
                        {
                            OffsetFrequency = (double)Math.Round(((Math.Abs(((trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[1]].freq +
                                trace[task.data_from_tech[t].ResultItems[rt].bw_data.NdBResult[2]].freq) / 2) -
                                (task.data_from_tech[t].ResultItems[rt].spec_data.FreqCentr))) / (task.data_from_tech[t].ResultItems[rt].spec_data.FreqCentr / 1000000)), 3);
                        }
                        smr.GeneralResult.OffsetFrequency_mk = OffsetFrequency;
                    }
                    if (task.data_from_tech[t].ResultItems[rt].freq_centr_perm > 0)
                        smr.GeneralResult.CentralFrequency_MHz = (double)(task.data_from_tech[t].ResultItems[rt].freq_centr_perm / 1000000);
                    else smr.GeneralResult.CentralFrequency_MHz = (double)(task.data_from_tech[t].ResultItems[rt].spec_data.FreqCentr / 1000000);

                    smr.GeneralResult.MeasDuration_sec = (double)task.data_from_tech[t].ResultItems[rt].spec_data.MeasDuration;
                    smr.GeneralResult.MeasStartTime = task.data_from_tech[t].ResultItems[rt].spec_data.MeasStart;
                    smr.GeneralResult.MeasFinishTime = task.data_from_tech[t].ResultItems[rt].spec_data.MeasStop;

                    if (smr.GeneralResult.MeasStartTime < StartTime && smr.GeneralResult.MeasStartTime != DateTime.MinValue)
                    {
                        StartTime = (DateTime)smr.GeneralResult.MeasStartTime;
                    }
                    if (smr.GeneralResult.MeasFinishTime > StopTime && smr.GeneralResult.MeasStartTime != DateTime.MinValue)
                    {
                        StopTime = (DateTime)smr.GeneralResult.MeasFinishTime;
                    }
                    List<ElementsMask> bwms = new List<ElementsMask>() { };
                    for (int b = 0; b < task.data_from_tech[t].ResultItems[rt].meas_mask.Length; b++)
                    {
                        ElementsMask bwm = new ElementsMask() { };
                        bwm.BW_kHz = (double)(task.data_from_tech[t].ResultItems[rt].meas_mask[b].bw / 1000);
                        bwm.Level_dB = (double)task.data_from_tech[t].ResultItems[rt].meas_mask[b].level;
                        bwms.Add(bwm);
                    }
                    smr.GeneralResult.BWMask = bwms.ToArray();
                    StationSysInfo ssi = new StationSysInfo();
                    #region ssi
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.bandwidth > -1)
                        ssi.BandWidth = (double)(task.data_from_tech[t].ResultItems[rt].station_sys_info.bandwidth / 1000);
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.base_id > -1)
                        ssi.BaseID = task.data_from_tech[t].ResultItems[rt].station_sys_info.base_id;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.bsic > -1)
                        ssi.BSIC = task.data_from_tech[t].ResultItems[rt].station_sys_info.bsic;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.channel_number > -1)
                        ssi.ChannelNumber = task.data_from_tech[t].ResultItems[rt].station_sys_info.channel_number;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.cid > -1)
                        ssi.CID = task.data_from_tech[t].ResultItems[rt].station_sys_info.cid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.code_power > -1000)
                        ssi.Code = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.code_power;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.ctoi > -1000)
                        ssi.CtoI = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.ctoi;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.eci > -1)
                        ssi.ECI = task.data_from_tech[t].ResultItems[rt].station_sys_info.eci;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.e_node_b_id > -1)
                        ssi.eNodeBId = task.data_from_tech[t].ResultItems[rt].station_sys_info.e_node_b_id;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.freq > -1)
                        ssi.Freq = (double)(task.data_from_tech[t].ResultItems[rt].station_sys_info.freq / 1000000);///////////////
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.icio > -1000)
                        ssi.IcIo = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.icio;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.inband_power > -1000)
                        ssi.INBAND_POWER = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.inband_power;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.iscp > -1000)
                        ssi.INBAND_POWER = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.iscp;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.lac > -1)
                        ssi.LAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.lac;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.lac > -1)
                        ssi.LAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.lac;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.mcc > -1)
                        ssi.MCC = task.data_from_tech[t].ResultItems[rt].station_sys_info.mcc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.mnc > -1)
                        ssi.MNC = task.data_from_tech[t].ResultItems[rt].station_sys_info.mnc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.nid > -1)
                        ssi.NID = task.data_from_tech[t].ResultItems[rt].station_sys_info.nid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.pci > -1)
                        ssi.PCI = task.data_from_tech[t].ResultItems[rt].station_sys_info.pci;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.pn > -1)
                        ssi.PN = task.data_from_tech[t].ResultItems[rt].station_sys_info.pn;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.power > -1000)
                        ssi.Power = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.power;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.ptotal > -1000)
                        ssi.Ptotal = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.ptotal;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.rnc > -1)
                        ssi.RNC = task.data_from_tech[t].ResultItems[rt].station_sys_info.rnc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.rscp > -1000)
                        ssi.RSCP = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.rscp;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.rsrq > -1000)
                        ssi.RSRQ = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.rsrq;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.sc > -1)
                        ssi.SC = task.data_from_tech[t].ResultItems[rt].station_sys_info.sc;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.sid > -1)
                        ssi.SID = task.data_from_tech[t].ResultItems[rt].station_sys_info.sid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.tac > -1)
                        ssi.TAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.tac;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.ucid > -1)
                        ssi.TAC = task.data_from_tech[t].ResultItems[rt].station_sys_info.ucid;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.type_cdmaevdo != "")
                        ssi.TypeCDMAEVDO = task.data_from_tech[t].ResultItems[rt].station_sys_info.type_cdmaevdo;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks.Length > 0)
                    {
                        List<StationSysInfoBlock> ssibs = new List<StationSysInfoBlock>() { };
                        for (int bl = 0; bl < task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks.Length; bl++)
                        {
                            StationSysInfoBlock ssib = new StationSysInfoBlock() { };
                            ssib.Type = task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks[bl].type;
                            ssib.Data = task.data_from_tech[t].ResultItems[rt].station_sys_info.information_blocks[bl].datastring;
                            ssibs.Add(ssib);
                        }
                        ssi.InfoBlocks = ssibs.ToArray();
                    }
                    ssi.Location = new Atdi.DataModels.Sdrns.GeoLocation() { };
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.location.asl > -100000)
                        ssi.Location.ASL = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.asl;
                    if (task.data_from_tech[t].ResultItems[rt].station_sys_info.location.agl > -100000)
                        ssi.Location.AGL = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.agl;
                    ssi.Location.Lat = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.latitude;
                    ssi.Location.Lon = (double)task.data_from_tech[t].ResultItems[rt].station_sys_info.location.longitude;
                    #endregion ssi
                    smr.GeneralResult.StationSysInfo = ssi;

                    List<LevelMeasResult> lmrs = new List<LevelMeasResult>() { };
                    for (int lm = 0; lm < task.data_from_tech[t].ResultItems[rt].level_results.Count(); lm++)
                    {
                        LevelMeasResult lmr = new LevelMeasResult() { };
                        lmr.MeasurementTime = task.data_from_tech[t].ResultItems[rt].level_results[lm].measurement_time;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].difference_time_stamp_ns > -1)
                            lmr.DifferenceTimeStamp_ns = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].difference_time_stamp_ns;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbm > -1000)
                            lmr.Level_dBm = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbm;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbmkvm > -1000)
                            lmr.Level_dBmkVm = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].level_dbmkvm;

                        lmr.Location = new Atdi.DataModels.Sdrns.GeoLocation() { };
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].location.agl > -100000)
                            lmr.Location.AGL = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.agl;
                        if (task.data_from_tech[t].ResultItems[rt].level_results[lm].location.asl > -100000)
                            lmr.Location.ASL = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.asl;
                        lmr.Location.Lat = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.latitude;
                        lmr.Location.Lon = (double)task.data_from_tech[t].ResultItems[rt].level_results[lm].location.longitude;

                        lmrs.Add(lmr);
                    }
                    smr.LevelResults = lmrs.ToArray();

                    smrs.Add(smr);
                }
            }
            #endregion StationMeasResult
            to.StartTime = StartTime;
            to.StopTime = StopTime;
            to.StationResults = smrs.ToArray();
            #endregion StationResults

            return to;
        }
    }
}
