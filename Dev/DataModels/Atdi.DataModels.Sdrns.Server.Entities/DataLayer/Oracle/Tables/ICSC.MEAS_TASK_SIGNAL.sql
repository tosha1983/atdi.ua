﻿CREATE TABLE ICSC.MEAS_TASK_SIGNAL
(
 ID                             NUMBER(15)     NOT NULL,
  AUTO_DIV_EMIT                  NUMBER(1),
  COMPARE_TRACE_JUST_REF_LEVELS  NUMBER(1),
  DIFF_MAX_MAX                   NUMBER(30,10),
  FILTRATION_TRACE               NUMBER(1),
  SIGN_NCHENAL                   NUMBER(9),
  SIGN_NCOUNT                    NUMBER(9),
  ALLOW_EXCESS_DB                NUMBER(30,10),
  MEAS_TASK_ID                   NUMBER(15),
  CORELLATION_ANALIZE            NUMBER(1),
  CHECK_FREQ_CH                  NUMBER(1),
  ANALIZE_BY_CH                  NUMBER(1),
  ANALIZE_SYSINFO_CH             NUMBER(1),
  MEAS_BW_EMISSION               NUMBER(1),
  CORRELATION_FACTOR             NUMBER(30,10),
  STANDARD                       VARCHAR2(50 BYTE),
  TRIGGER_LEVEL_DBM_HZ           NUMBER(30,10),
  NUMBER_POINT_FOR               NUMBER(30,10),
  WINDOW_BW                      NUMBER(30,10),
  DIFF_LEVEL_FOR_BW              NUMBER(30,10),
  NDBLEVEL_DB                    NUMBER(30,10),
  NUM_IGNORED_POINTS             NUMBER(9),
  MIN_EXCESS_NOSE_LVL            NUMBER(30,10),
  TIME_BETWEEN_WORKTIMES         NUMBER(9),
  TYPE_JOIN_SPECTRUM             NUMBER(9),
  CROSSING_BW_PERCENT_GOOD       NUMBER(30,10),
  CROSSING_BW_PERCENT_BAD        NUMBER(30,10),
  MAX_FREQ_DEVIATION             NUMBER(30,10),
  MIN_POINT_DETAIL_BW            NUMBER(9),
  CHECK_LEVEL_CHANNEL            NUMBER(1)
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


CREATE UNIQUE INDEX ICSC.MEAS_TASK_SIGNAL_ID_PK ON ICSC.MEAS_TASK_SIGNAL
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


CREATE INDEX ICSC.MEAS_TASK_SIGNAL_TASK_ID_FK ON ICSC.MEAS_TASK_SIGNAL
(MEAS_TASK_ID)
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


ALTER TABLE ICSC.MEAS_TASK_SIGNAL ADD (
  CONSTRAINT MEASTASKSIGNAL_PK
 PRIMARY KEY
 (ID));
