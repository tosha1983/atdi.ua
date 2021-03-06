﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [ICSC].[XV_RES_LEVELS]
(
   ID,
   FREQ_MEAS,
   OCCUPANCY_SPECT,
   VMIN_LVL,
   VALUE_LVL,
   VMMAX_LVL,
   TIME_MEAS,
   TYPE_MEASUREMENTS,
   SCANS_NUMBER,
   SENSOR_ID,
   TYPE_SPECTRUM_OCCUPATION,
   LEVEL_MIN_OCCUP,
   TASK_ID,
   SENSOR_NAME,
   SENSOR_TITLE,
   LON,
   LAT,
   CHANNEL_BANDWIDTH
)
AS
   SELECT   DISTINCT SRC_1."ID" COL_1,
                     SRC_1."FREQ_MEAS" COL_2,
                     SRC_1."OCCUPANCY_SPECT" COL_3,
                     SRC_1."VMIN_LVL" COL_4,
                     SRC_1."VALUE_LVL" COL_5,
                     SRC_1."VMMAX_LVL" COL_6,
                     SRC_2."TIME_MEAS" COL_7,
                     SRC_2."TYPE_MEASUREMENTS" COL_8,
                     SRC_2."SCANS_NUMBER" COL_9,
                     SRC_4."ID" COL_10,
                     SRC_7."TYPE_SPECTRUM_OCCUPATION" COL_11,
                     SRC_7."LEVEL_MIN_OCCUP" COL_12,
                     SRC_3."ID" COL_13,
                     SRC_4."NAME" COL_14,
                     SRC_4."TITLE" COL_15,
                     SRC_8."LON" COL_16,
                     SRC_8."LAT" COL_17,
                     SRC_9."STEP" COL_18
     FROM                           ICSC."RES_LEVELS" SRC_1
                                 INNER JOIN
                                    ICSC.RES_MEAS SRC_2 -- Key: <[singlton].ROOT.RES_MEAS.[Atdi.DataModels.Sdrns.Server.Entities.ResMeas]>
                                 ON (SRC_2.ID = SRC_1.RES_MEAS_ID)
                              INNER JOIN
                                 ICSC.SUBTASK_SENSOR SRC_5 -- Key: <[singlton].ROOT.RES_MEAS.SUBTASK_SENSOR.[Atdi.DataModels.Sdrns.Server.Entities.SubTaskSensor]>
                              ON (SRC_5.ID = SRC_2.SUBTASK_SENSOR_ID)
                           INNER JOIN
                              ICSC.SUBTASK SRC_6 -- Key: <[singlton].ROOT.RES_MEAS.SUBTASK_SENSOR.SUBTASK.[Atdi.DataModels.Sdrns.Server.Entities.SubTask]>
                           ON (SRC_6.ID = SRC_5.SUBTASK_ID)
                        INNER JOIN
                           ICSC.MEAS_TASK SRC_3 -- Key: <[singlton].ROOT.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.[Atdi.DataModels.Sdrns.Server.Entities.MeasTask]>
                        ON (SRC_3.ID = SRC_6.MEAS_TASK_ID)
                     INNER JOIN
                        ICSC.SENSOR SRC_4 -- Key: <[singlton].ROOT.RES_MEAS.SUBTASK_SENSOR.SENSOR.[Atdi.DataModels.Sdrns.Server.Entities.Sensor]>
                     ON (SRC_4.ID = SRC_5.SENSOR_ID)
                  INNER JOIN
                     ICSC.MEAS_OTHER SRC_7 -- Key: <[singlton].ROOT.RES_MEAS.SUBTASK_SENSOR.SUBTASK.MEAS_TASK.MEAS_OTHER.[Atdi.DataModels.Sdrns.Server.Entities.MeasOther]>
                  ON (SRC_7.MEAS_TASK_ID = SRC_3.ID)
               INNER JOIN
                  ICSC.MEAS_FREQ_PARAM SRC_9
               ON (SRC_9.MEAS_TASK_ID = SRC_7.ID)
            FULL OUTER JOIN
               ICSC.SENSOR_LOCATION SRC_8
            ON (SRC_8.SENSOR_ID = SRC_4.ID AND SRC_8.STATUS = 'A');

GO


