CREATE TABLE CALC.CALCRESULTS_STATION_CALIBRATION_STA
(
  ID                                  NUMBER(15) NOT NULL,
  CALCRESULTS_STATION_CALIBRATION_ID  NUMBER(15) NOT NULL,
  EXTERNAL_SOURCE                     NVARCHAR2(50),
  EXTERNAL_CODE                       NVARCHAR2(50),
  LICENSE_GSID                        NVARCHAR2(50),
  REAL_GSID                           NVARCHAR2(50),
  RESULT_STATION_STATUS               NVARCHAR2(50),
  MAX_CORELLATION                     NUMBER(22,8),
  OLD_ALTITUDE_M                      NUMBER(9),
  OLD_TILT_DEG                        NUMBER(22,8),
  OLD_AZIMUTRH_DEG                    NUMBER(22,8),
  OLD_LAT_DEG                         NUMBER(30,10),
  OLD_LON_DEG                         NUMBER(30,10),
  OLD_POWER_DB                        NUMBER(30,10),
  OLD_FREQ_MHZ                        NUMBER(30,10),
  NEW_ALTITUDE_M                      NUMBER(9),
  NEW_TILT_DEG                        NUMBER(22,8),
  NEW_AZIMUTRH_DEG                    NUMBER(22,8),
  NEW_LAT_DEG                         NUMBER(30,10),
  NEW_LON_DEG                         NUMBER(30,10),
  NEW_POWER_DB                        NUMBER(30,10),
  STATION_MONITORING_ID               NUMBER(15),
  FREQ_MHZ                            NUMBER(30,10),
  STANDARD                            NVARCHAR2(50),
  DELTA_CORRELATION_PC                NUMBER(30,10),
  USED_POINTS_PC                      NUMBER(9),
  COUNT_POINTS_IN_DRIVE_TEST          NUMBER(9)
)
TABLESPACE USERS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX CALC.CALCRESULTS_STATION_STA_PK ON CALC.CALCRESULTS_STATION_CALIBRATION_STA
(ID)
LOGGING
TABLESPACE USERS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            INITIAL          64K
            NEXT             1M
            MINEXTENTS       1
            MAXEXTENTS       UNLIMITED
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;


ALTER TABLE CALC.CALCRESULTS_STATION_CALIBRATION_STA ADD (
  CONSTRAINT CALCRESULTS_STATION_STA_PK
 PRIMARY KEY
 (ID)
    USING INDEX 
    TABLESPACE USERS
    PCTFREE    10
    INITRANS   2
    MAXTRANS   255
    STORAGE    (
                INITIAL          64K
                NEXT             1M
                MINEXTENTS       1
                MAXEXTENTS       UNLIMITED
                PCTINCREASE      0
               ));
