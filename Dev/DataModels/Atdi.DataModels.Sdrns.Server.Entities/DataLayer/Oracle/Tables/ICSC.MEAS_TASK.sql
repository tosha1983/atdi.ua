﻿CREATE TABLE ICSC.MEAS_TASK
(
  ID              NUMBER(15)                    NOT NULL,
  STATUS          NVARCHAR2(50),
  ORDER_ID        NUMBER(9),
  TYPE            NVARCHAR2(50),
  NAME            NVARCHAR2(100),
  EXECUTION_MODE  NVARCHAR2(50),
  TASK            NVARCHAR2(50),
  PRIO            NUMBER(9),
  RESULT_TYPE     NVARCHAR2(50),
  MAX_TIME_BS     NUMBER(9),
  DATE_CREATED    DATE,
  CREATED_BY      NVARCHAR2(50),
  ID_START        NVARCHAR2(50),
  PER_START       DATE,
  PER_STOP        DATE,
  TIME_START      DATE,
  TIME_STOP       DATE,
  DAYS            NVARCHAR2(250),
  PER_INTERVAL    NUMBER(22,8)
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


CREATE UNIQUE INDEX ICSC.XBS_MEASTASK_PK ON ICSC.MEAS_TASK
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


ALTER TABLE ICSC.MEAS_TASK ADD (
  CONSTRAINT XBS_MEASTASK_PK
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
