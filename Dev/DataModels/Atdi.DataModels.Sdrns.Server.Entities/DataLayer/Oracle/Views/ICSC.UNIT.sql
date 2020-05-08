DROP VIEW ICSC.XV_UNIT;

/* Formatted on 24.02.2020 10:39:03 (QP5 v5.115.810.9015) */
CREATE OR REPLACE FORCE VIEW ICSC.UNIT
(
   LON,
   LAT,
   LEVELDBM,
   CENTRALFREQUENCY,
   TIMEOFMEASUREMENTS,
   BW,
   IDSTATION,
   SPECRUMSTEPS,
   T1,
   T2,
   ID,
   MEASGLOBALSID
)
AS
   SELECT   DISTINCT Tcaz_1.LON AS LON,
                     Tcaz_1.LAT AS LAT,
                     Tcaz_1.LEVEL_DBM AS LEVELDBM,
                     Tcaz_4.CENTRAL_FREQUENCY AS CENTRALFREQUENCY,
                     Tcaz_1.TIME_OF_MEASUREMENTS AS TIMEOFMEASUREMENTS,
                     Tcaz_4.BW AS BW,
                     Tcaz_2.CLIENT_SECTOR_CODE AS IDSTATION,
                     Tcaz_4.SPECRUM_STEPS AS SPECRUMSTEPS,
                     Tcaz_4.T1 AS T1,
                     Tcaz_4.T2 AS T2,
                     Tcaz_3.ID AS ID,
                     Tcaz_2.MEAS_GLOBAL_SID AS MEASGLOBALSID
     FROM            ICSC.RES_STLEVEL_CAR Tcaz_1
                  INNER JOIN
                     ICSC.RES_MEAS_STATION Tcaz_2
                  ON Tcaz_1.RES_MEAS_STATION_ID = Tcaz_2.ID
               INNER JOIN
                  ICSC.RES_MEAS Tcaz_3
               ON Tcaz_2.RES_MEAS_ID = Tcaz_3.ID
            INNER JOIN
               ICSC.RES_STGENERAL Tcaz_4
            ON Tcaz_4.RES_MEAS_STATION_ID = Tcaz_2.ID;