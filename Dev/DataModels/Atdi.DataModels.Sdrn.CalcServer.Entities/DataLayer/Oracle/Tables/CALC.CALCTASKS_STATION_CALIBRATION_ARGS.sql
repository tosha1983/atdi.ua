﻿CREATE TABLE CALC.CALCTASKS_STATION_CALIBRATION_ARGS
(
  ID                              NUMBER(15)    NOT NULL,
  TASK_ID                         NUMBER(15)    NOT NULL,
  CORRELATION_THRESHOLD_HARD      NUMBER(22,8),
  CORRELATION_THRESHOLD_WEAK      NUMBER(22,8),
  TRUST_OLD_RESULTS               NUMBER(1),
  USE_MEASUREMENT_SAME_GSID       NUMBER(1),
  DISTANCE_AROUND_CONTOUR_KM      NUMBER(9),
  MIN_NUMBER_POINT_FOR_CORRELATI  NUMBER(9),
  MIN_RANGE_MEASUREMENTS_DBMKV    NUMBER(22,8),
  MAX_RANGE_MEASUREMENTS_DBMKV    NUMBER(22,8),
  CORRELATION_DISTANCE_M          NUMBER(9),
  DELTA_DB                        NUMBER(22,8),
  MAX_ANTENNAS_PATTERN_LOSS_DB    NUMBER(22,8),
  DETAIL                          NUMBER(1),
  ALTITUDE_STATION                NUMBER(1),
  SHIFT_ALTITUDE_STATION_MIN_M    NUMBER(9),
  SHIFT_ALTITUDE_STATION_MAX_M    NUMBER(9),
  SHIFT_ALTITUDE_STATION_STEP_M   NUMBER(9),
  MAX_DEVIATION_ALTITUDE_STATION  NUMBER(9),
  TILT_STATION                    NUMBER(1),
  SHIFT_TILT_STATION_MIN_DEG      NUMBER(22,8),
  SHIFT_TILT_STATION_MAX_DEG      NUMBER(22,8),
  SHIFT_TILT_STATION_STEP_DEG     NUMBER(22,8),
  MAX_DEVIATION_TILT_STATION_DEG  NUMBER(22,8),
  AZIMUTH_STATION                 NUMBER(1),
  SHIFT_AZIMUTH_STATION_MIN_DEG   NUMBER(22,8),
  SHIFT_AZIMUTH_STATION_MAX_DEG   NUMBER(22,8),
  SHIFT_AZIMUTH_STATION_STEP_DEG  NUMBER(22,8),
  MAX_DEVIATION_AZIMUTH_STATION_  NUMBER(22,8),
  COORDINATES_STATION             NUMBER(1),
  SHIFT_COORDINATES_STATION_M     NUMBER(9),
  SHIFT_COORDINATES_STATION_STEP  NUMBER(9),
  MAX_DEVIATION_COORDINATES_STAT  NUMBER(9),
  POWER_STATION                   NUMBER(1),
  SHIFT_POWER_STATION_MIN_DB      NUMBER(22,8),
  SHIFT_POWER_STATION_MAX_DB      NUMBER(22,8),
  SHIFT_POWER_STATION_STEP_DB     NUMBER(22,8),
  CASCADE_TUNING                  NUMBER(1),
  NUMBER_CASCADE                  NUMBER(9),
  DETAIL_OF_CASCADE               NUMBER(9),
  METHOD                          NUMBER(3),
  INFOC_MEAS_RESULTS              BLOB,
  STATION_IDS                     BLOB
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


CREATE UNIQUE INDEX CALC.CALCTASKS_STATION_ARG_PK ON CALC.CALCTASKS_STATION_CALIBRATION_ARGS
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


ALTER TABLE CALC.CALCTASKS_STATION_CALIBRATION_ARGS ADD (
  CONSTRAINT CALCTASKS_STATION_ARG_PK
 PRIMARY KEY
 (ID));
