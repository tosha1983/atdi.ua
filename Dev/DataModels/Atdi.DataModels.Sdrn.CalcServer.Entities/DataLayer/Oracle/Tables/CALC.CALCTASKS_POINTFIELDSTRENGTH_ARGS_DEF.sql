﻿CREATE TABLE CALC.CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF
(
  TASK_ID        NUMBER(15)                     NOT NULL,
  STATION_ID     NUMBER(15),
  POINT_LON_DEC  NUMBER(22,8),
  POINT_LAT_DEC  NUMBER(22,8),
  POINT_ALT_M    NUMBER(22,8)
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


CREATE UNIQUE INDEX CALC.CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF_PK ON CALC.CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF
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


ALTER TABLE CALC.CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF ADD (
  CONSTRAINT CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF_PK
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
