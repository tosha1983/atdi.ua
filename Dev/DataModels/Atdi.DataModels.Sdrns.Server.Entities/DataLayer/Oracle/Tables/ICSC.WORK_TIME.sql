﻿CREATE TABLE ICSC.WORK_TIME
(
  ID                 NUMBER(15)                 NOT NULL,
  START_EMIT         DATE                       NOT NULL,
  STOP_EMIT          DATE                       NOT NULL,
  HIT_COUNT          NUMBER(9)                  NOT NULL,
  PERCENT_AVAILABLE  NUMBER(22,8)               NOT NULL,
  EMITTING_ID        NUMBER(15)                 NOT NULL
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


CREATE INDEX ICSC.WORKTIME_EMITTING_ID_FK ON ICSC.WORK_TIME
(EMITTING_ID)
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


CREATE UNIQUE INDEX ICSC.WORKTIME_ID_PK ON ICSC.WORK_TIME
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


ALTER TABLE ICSC.WORK_TIME ADD (
  CONSTRAINT WORKTIME_PK
 PRIMARY KEY
 (ID));
