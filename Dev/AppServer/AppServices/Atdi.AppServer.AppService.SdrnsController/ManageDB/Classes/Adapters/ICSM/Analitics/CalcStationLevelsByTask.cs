using System;
using System.Collections.Generic;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;
using System.Collections;
using System.Linq;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{

  


    public class CalcStationLevelsByTask : IDisposable
    {
        public static ILogger logger;
        public CalcStationLevelsByTask(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~CalcStationLevelsByTask()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public StationLevelsByTask[] GetStationLevelsByTask(LevelsByTaskParams paramsStationLevelsByTask)
        {
            const int Cn = 900;
            List<StationLevelsByTask> L_OUT = new List<StationLevelsByTask>();
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    
                    double ANT_VAL = 0; // єто костыль
                    logger.Trace("Start procedure GetStationLevelsByTask...");
                    try
                    {
                        List<StationLevelsByTask> listLevelMeas2 = new List<StationLevelsByTask>();
                        YXbsResMeas resMeas = new YXbsResMeas();  
                        resMeas.Format("*");
                        resMeas.Filter = string.Format("(ID IN ({0})) AND (MEASTASKID = '{1}')", string.Join(",", paramsStationLevelsByTask.MeasResultID), paramsStationLevelsByTask.MeasTaskId);
                        for (resMeas.OpenRs(); !resMeas.IsEOF(); resMeas.MoveNext())
                        {
                            int idx_resMeasStation = 0;
                            List<int> sql_resMeasStation = new List<int>();
                            string sql_resMeasStation_ = "";
                            string SQL = string.Format("(XBSRESMEASID = {0}) AND (IDSECTOR={1})", resMeas.m_id.Value, paramsStationLevelsByTask.SectorId);
                            var resMeasStation = new YXbsResmeasstation();
                            resMeasStation.Format("*");
                            resMeasStation.Filter = SQL;
                            for (resMeasStation.OpenRs(); !resMeasStation.IsEOF(); resMeasStation.MoveNext())
                            {
                                if (sql_resMeasStation.Count <= Cn)
                                {
                                    sql_resMeasStation.Add(resMeasStation.m_id.Value);
                                }
                                if ((sql_resMeasStation.Count > Cn) || ((idx_resMeasStation + 1) == resMeasStation.GetCount()))
                                {
                                    sql_resMeasStation_ = string.Format("(XBS_RESMEASSTATIONID IN ({0}))", string.Join(",", sql_resMeasStation));
                                    sql_resMeasStation.Clear();
                                    var bsResStLevelCa = new YXbsResStLevelCar();
                                    bsResStLevelCa.Format("*");
                                    bsResStLevelCa.Filter = sql_resMeasStation_;;
                                    for (bsResStLevelCa.OpenRs(); !bsResStLevelCa.IsEOF(); bsResStLevelCa.MoveNext())
                                    {
                                        StationLevelsByTask tx = new StationLevelsByTask();
                                        tx.Lon = bsResStLevelCa.m_lon;
                                        tx.Lat = bsResStLevelCa.m_lat;
                                        if (((bsResStLevelCa.m_leveldbmkvm != 0) && (bsResStLevelCa.m_leveldbmkvm != -1)) && (bsResStLevelCa.m_leveldbmkvm > -30) && (bsResStLevelCa.m_leveldbmkvm < 200))
                                        {
                                            tx.Level_dBmkVm = Math.Round(bsResStLevelCa.m_leveldbmkvm.Value,2);
                                            L_OUT.Add(tx);
                                        }
                                        else
                                        {
                                            if ((bsResStLevelCa.m_centralfrequency.Value > 0.01) && (bsResStLevelCa.m_leveldbm > -300) && (bsResStLevelCa.m_leveldbm < -10))
                                            {
                                                tx.Level_dBmkVm = Math.Round((float)(77.2 + 20 * Math.Log10(bsResStLevelCa.m_centralfrequency.Value) + bsResStLevelCa.m_leveldbm - ANT_VAL),2);
                                                L_OUT.Add(tx);
                                            }
                                        }
                                    }
                                    bsResStLevelCa.Close();
                                    bsResStLevelCa.Dispose();
                                }
                                idx_resMeasStation++;
                            }
                            resMeasStation.Close();
                            resMeasStation.Dispose();
                        }
                        resMeas.Close();
                        resMeas.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure GetStationLevelsByTask... " + ex.Message);
                    }
                    logger.Trace("End procedure GetStationLevelsByTask...");
                });
                thread.Start();
                thread.Join();
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure GetStationLevelsByTask..." + ex.Message);
            }
            return L_OUT.ToArray();
        }

    }
}

