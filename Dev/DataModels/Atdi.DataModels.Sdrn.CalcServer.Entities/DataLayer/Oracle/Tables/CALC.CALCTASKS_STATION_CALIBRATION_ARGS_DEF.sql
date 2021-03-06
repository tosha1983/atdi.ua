﻿CREATE TABLE CALC.CALCTASKS_STATION_CALIBRATION_ARGS_DEF
(
  STANDARD                             NVARCHAR2(50),
  TASK_ID                              NUMBER(15) NOT NULL,
  CORRELATION_THRESHOLD_HARD           NUMBER(22,8),
  CORRELATION_THRESHOLD_WEAK           NUMBER(22,8),
  TRUST_OLD_RESULTS                    NUMBER(1),
  USE_MEASUREMENT_SAME_GSID            NUMBER(1),
  DISTANCE_AROUND_CONTOUR_KM           NUMBER(9),
  MIN_NUMBER_POINT_FOR_CORRELATION     NUMBER(9),
  MIN_RANGE_MEASUREMENTS_DBMKV         NUMBER(22,8),
  MAX_RANGE_MEASUREMENTS_DBMKV         NUMBER(22,8),
  CORRELATION_DISTANCE_M               NUMBER(9),
  DELTA_DB                             NUMBER(22,8),
  MAX_ANTENNAS_PATTERN_LOSS_DB         NUMBER(22,8),
  DETAIL                               NUMBER(1),
  ALTITUDE_STATION                     NUMBER(1),
  SHIFT_ALTITUDE_STATION_MIN_M         NUMBER(9),
  SHIFT_ALTITUDE_STATION_MAX_M         NUMBER(9),
  SHIFT_ALTITUDE_STATION_STEP_M        NUMBER(9),
  MAX_DEVIATION_ALTITUDE_STATION_M     NUMBER(9),
  TILT_STATION                         NUMBER(1),
  SHIFT_TILT_STATION_MIN_DEG           NUMBER(22,8),
  SHIFT_TILT_STATION_MAX_DEG           NUMBER(22,8),
  SHIFT_TILT_STATION_STEP_DEG          NUMBER(22,8),
  MAX_DEVIATION_TILT_STATION_DEG       NUMBER(22,8),
  AZIMUTH_STATION                      NUMBER(1),
  SHIFT_AZIMUTH_STATION_MIN_DEG        NUMBER(22,8),
  SHIFT_AZIMUTH_STATION_MAX_DEG        NUMBER(22,8),
  SHIFT_AZIMUTH_STATION_STEP_DEG       NUMBER(22,8),
  MAX_DEVIATION_AZIMUTH_STATION_DEG    NUMBER(22,8),
  COORDINATES_STATION                  NUMBER(1),
  SHIFT_COORDINATES_STATION_M          NUMBER(9),
  SHIFT_COORDINATES_STATION_STEP_M     NUMBER(9),
  MAX_DEVIATION_COORDINATES_STATION_M  NUMBER(9),
  POWER_STATION                        NUMBER(1),
  SHIFT_POWER_STATION_MIN_DB           NUMBER(22,8),
  SHIFT_POWER_STATION_MAX_DB           NUMBER(22,8),
  SHIFT_POWER_STATION_STEP_DB          NUMBER(22,8),
  CASCADE_TUNING                       NUMBER(1),
  NUMBER_CASCADE                       NUMBER(9),
  DETAIL_OF_CASCADE                    NUMBER(9),
  METHOD                               NUMBER(3),
  INFOC_MEAS_RESULTS                   BLOB,
  STATION_IDS                          BLOB,
  MAX_DEVIATION_POWER_STATION_DB       NUMBER(22,8),
  AREA_CONTOURS                        BLOB,
  AREAS                                NVARCHAR2(1000)
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
LOB (STATION_IDS) STORE AS SECUREFILE 
      ( TABLESPACE  USERS 
        ENABLE      STORAGE IN ROW
        CHUNK       8192
        NOCACHE
        INDEX       (
          TABLESPACE USERS
          STORAGE    (
                      INITIAL          64K
                      NEXT             1
                      MINEXTENTS       1
                      MAXEXTENTS       UNLIMITED
                      PCTINCREASE      0
                      BUFFER_POOL      DEFAULT
                     ))
        STORAGE    (
                    INITIAL          104K
                    NEXT             1M
                    MINEXTENTS       1
                    MAXEXTENTS       UNLIMITED
                    PCTINCREASE      0
                    BUFFER_POOL      DEFAULT
                   )
      )
  LOB (INFOC_MEAS_RESULTS) STORE AS SECUREFILE 
      ( TABLESPACE  USERS 
        ENABLE      STORAGE IN ROW
        CHUNK       8192
        NOCACHE
        INDEX       (
          TABLESPACE USERS
          STORAGE    (
                      INITIAL          64K
                      NEXT             1
                      MINEXTENTS       1
                      MAXEXTENTS       UNLIMITED
                      PCTINCREASE      0
                      BUFFER_POOL      DEFAULT
                     ))
        STORAGE    (
                    INITIAL          104K
                    NEXT             1M
                    MINEXTENTS       1
                    MAXEXTENTS       UNLIMITED
                    PCTINCREASE      0
                    BUFFER_POOL      DEFAULT
                   )
      )
  LOB (AREA_CONTOURS) STORE AS SECUREFILE 
      ( TABLESPACE  USERS 
        ENABLE      STORAGE IN ROW
        CHUNK       8192
        NOCACHE
        INDEX       (
          TABLESPACE USERS
          STORAGE    (
                      INITIAL          64K
                      NEXT             1
                      MINEXTENTS       1
                      MAXEXTENTS       UNLIMITED
                      PCTINCREASE      0
                      BUFFER_POOL      DEFAULT
                     ))
        STORAGE    (
                    INITIAL          104K
                    NEXT             1M
                    MINEXTENTS       1
                    MAXEXTENTS       UNLIMITED
                    PCTINCREASE      0
                    BUFFER_POOL      DEFAULT
                   )
      )
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX CALC.PK_CALCTASKS_STATION_CALIBRATION_ARGS_DEF_NEW_ ON CALC.CALCTASKS_STATION_CALIBRATION_ARGS_DEF
(TASK_ID)
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


ALTER TABLE CALC.CALCTASKS_STATION_CALIBRATION_ARGS_DEF ADD (
  CONSTRAINT PK_CALCTASKS_STATION_CALIBRATION_ARGS_DEF_NEW_
 PRIMARY KEY
 (TASK_ID)
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
