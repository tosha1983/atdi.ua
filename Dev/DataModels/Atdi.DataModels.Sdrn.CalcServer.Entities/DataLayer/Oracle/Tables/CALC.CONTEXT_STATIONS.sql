﻿CREATE TABLE CALC.CONTEXT_STATIONS
(
  ID            NUMBER(15)                      NOT NULL,
  CONTEXT_ID    NUMBER(15)                      NOT NULL,
  CREATED_DATE  TIMESTAMP(7) WITH TIME ZONE     DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  NAME          NVARCHAR2(250)                  NOT NULL,
  CALL_SIGN     NVARCHAR2(50)                   NOT NULL,
  STATE_CODE    NUMBER(3)                       NOT NULL,
  STATE_NAME    NVARCHAR2(50)                   NOT NULL
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


CREATE UNIQUE INDEX CALC.CONTEXT_STATIONS_PK ON CALC.CONTEXT_STATIONS
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


ALTER TABLE CALC.CONTEXT_STATIONS ADD (
  CONSTRAINT CONTEXT_STATIONS_PK
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
