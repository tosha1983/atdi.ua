﻿CREATE TABLE CALC.CALCRESULTS_STATION_CALIBRATION
(
  ID                         NUMBER(15)         NOT NULL,
  RESULT_ID                  NUMBER(15)         NOT NULL,
  TIME_START                 TIMESTAMP(7) WITH TIME ZONE,
  STANDARD                   NVARCHAR2(50),
  AREA_NAME                  NVARCHAR2(1000),
  NUMBER_STATION             NUMBER(9),
  NUMBER_STATION_IN_CONTOUR  NUMBER(9),
  COUNT_STATION_CS           NUMBER(9),
  COUNT_STATION_NS           NUMBER(9),
  COUNT_STATION_IT           NUMBER(9),
  COUNT_STATION_NF           NUMBER(9),
  COUNT_STATION_UN           NUMBER(9),
  COUNT_MEAS_GSID            NUMBER(9),
  COUNT_MEAS_GSID_LS         NUMBER(9),
  COUNT_MEAS_GSID_IT         NUMBER(9),
  PARAMETERS_TASK_ID         NUMBER(15),
  PERCENT_COMPLETE           NUMBER(9)
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


CREATE UNIQUE INDEX CALC.CALCRESULTS_STATION_CALIBRA_PK ON CALC.CALCRESULTS_STATION_CALIBRATION
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


ALTER TABLE CALC.CALCRESULTS_STATION_CALIBRATION ADD (
  CONSTRAINT CALCRESULTS_STATION_CALIBRA_PK
 PRIMARY KEY
 (ID));
