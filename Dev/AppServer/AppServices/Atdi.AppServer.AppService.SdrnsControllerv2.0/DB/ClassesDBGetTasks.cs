﻿using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Atdi.AppServer.AppService.SdrnsControllerv2_0
{
    /// <summary>
    /// Класс, извлекающий данные
    /// из поля ORDER_DATA таблицы SYS_ARGUS_ORM
    /// с целью заполнения структуры MEAS_TASK
    /// </summary>
    public class ClassesDBGetTasks 
    {
        public static ILogger logger;
        public ClassesDBGetTasks(ILogger log)
        {
            if (logger==null) logger = log;
        }
       

        private List<ClassTasks> L_IN { get; set; }


        public static void GetMeasTaskSDRNum(int NumValue, out string TaskId, out int SubTaskId, out int SubTaskStationId, out int SensorId)
        {
           string _TaskId = "";
           int _SubTaskId = 0;
           int _SubTaskStationId = 0;
           int _SensorId = 0;
            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //{
                try
                { 
                    YXbsMeasTaskSDR meastask = new YXbsMeasTaskSDR();
                    meastask.Format("*");
                    if (meastask.Fetch(string.Format("NUM={0}", NumValue)))
                    {
                        _TaskId = meastask.m_meastaskid;
                        _SubTaskId = meastask.m_meassubtaskid.Value;
                        _SubTaskStationId = meastask.m_meassubtaskstationid.Value;
                        _SensorId = meastask.m_sensorid.Value;
                    }
                    meastask.Close();
                    meastask.Dispose();
                }
                catch (Exception ex)
                {
                    logger.Trace("Error in procedure GetMeasTaskSDRNum... " + ex.Message);
                }

            //});
           // thread.Start();
           // thread.Join();

            TaskId = _TaskId;
            SubTaskId = _SubTaskId;
            SubTaskStationId = _SubTaskStationId;
            SensorId = _SensorId;
        }

        public static void GetMeasTaskSDRIdentifier(string TaskId, out int SubTaskId, out int SubTaskStationId, out int SensorId)
        {
            string _TaskId = "";
            int _SubTaskId = 0;
            int _SubTaskStationId = 0;
            int _SensorId = 0;
           // System.Threading.Thread thread = new System.Threading.Thread(() =>
           // {
                try
                {
                    YXbsMeasTaskSDR meastask = new YXbsMeasTaskSDR();
                    meastask.Format("*");
                    if (meastask.Fetch(string.Format("MEASTASKID='{0}'", TaskId)))
                    {
                        _TaskId = meastask.m_meastaskid;
                        _SubTaskId = meastask.m_meassubtaskid.Value;
                        _SubTaskStationId = meastask.m_meassubtaskstationid.Value;
                        _SensorId = meastask.m_sensorid.Value;
                    }
                    meastask.Close();
                    meastask.Dispose();
                }
                catch (Exception ex)
                {
                    logger.Trace("Error in procedure GetMeasTaskSDRIdentifier... " + ex.Message);
                }

           // });
           // thread.Start();
           // thread.Join();

            TaskId = _TaskId;
            SubTaskId = _SubTaskId;
            SubTaskStationId = _SubTaskStationId;
            SensorId = _SensorId;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? SaveTaskSDRToDB(int SubTaskId, int SubTaskStationId, string TaskId, int SensorId)
        {
            int? NUM_Val = null;
            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //{
                Yyy yyy = new Yyy();
                DbConnection dbConnect = null;
                try
                {
                    dbConnect = yyy.NewConnection(yyy.GetConnectionString());
                    if (dbConnect.State == System.Data.ConnectionState.Open)
                    {
                        DbTransaction transaction = dbConnect.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                        try
                        {
                            logger.Trace("Start procedure SaveTaskToDB...");
                            int? Num = yyy.GetMaxId("XBS_MEASTASK_SDR", "NUM");
                            ++Num;

                            YXbsMeasTaskSDR meastask = new YXbsMeasTaskSDR();
                            meastask.Format("*");
                            meastask.New();
                            meastask.m_meastaskid = TaskId;
                            meastask.m_meassubtaskid = SubTaskId;
                            meastask.m_meassubtaskstationid = SubTaskStationId;
                            meastask.m_sensorid = SensorId;
                            meastask.m_num = Num;
                            meastask.Save(dbConnect, transaction);
                            meastask.Close();
                            meastask.Dispose();
                            transaction.Commit();
                            NUM_Val = Num;
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch (Exception e) { transaction.Dispose(); logger.Error(e.Message); }
                            logger.Error("Error in SaveTaskToDB: " + ex.Message);
                        }
                        finally
                        {
                            transaction.Dispose();
                        }
                    }
                    else
                    {
                        logger.Error("[CreateNewObjectSensor] Error connection  to Database");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
                finally
                {
                    dbConnect.Close();
                    dbConnect.Dispose();
                }
            //});
            //thread.Start();
            //thread.Join();
            return NUM_Val;
        }
        
    }
}
