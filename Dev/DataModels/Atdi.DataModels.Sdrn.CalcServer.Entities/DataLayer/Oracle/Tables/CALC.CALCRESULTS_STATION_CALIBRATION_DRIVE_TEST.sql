﻿CREATE TABLE CALC.CALCRESULTS_STATION_CALIBRATION_DRIVE_TEST
(
  ID                                  NUMBER(15) NOT NULL,
  CALCRESULTS_STATION_CALIBRATION_ID  NUMBER(15) NOT NULL,
  EXTERNAL_SOURCE                     NVARCHAR2(50),
  EXTERNAL_CODE                       NVARCHAR2(50),
  STATION_GSID                        NVARCHAR2(50),
  MEAS_GSID                           NVARCHAR2(50),
  RESULT_DRIVE_TEST_STATUS            NVARCHAR2(50),
  MAX_PERCENT_CORELLATION             NUMBER(22,8),
  COUNT_POINTS_IN_DRIVE_TEST          NUMBER(9),
  DRIVE_TEST_ID                       NUMBER(15),
  LINK_STATION_MONITORING_ID          NUMBER(15),
  FREQ_MHZ                            NUMBER(22,8),
  STANDARD                            NVARCHAR2(50)
)
TABLESPACE USERS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX CALC.CALCRESULTS_STATION_DRVTEST_PK ON CALC.CALCRESULTS_STATION_CALIBRATION_DRIVE_TEST
(ID)
LOGGING
TABLESPACE USERS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;


ALTER TABLE CALC.CALCRESULTS_STATION_CALIBRATION_DRIVE_TEST ADD (
  CONSTRAINT CALCRESULTS_STATION_DRVTEST_PK
 PRIMARY KEY
 (ID));
