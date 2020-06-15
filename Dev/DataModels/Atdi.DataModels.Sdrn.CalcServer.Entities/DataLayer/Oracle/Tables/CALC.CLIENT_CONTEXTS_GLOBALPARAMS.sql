﻿CREATE TABLE CALC.CLIENT_CONTEXTS_GLOBALPARAMS
(
  TIME_PC          NUMBER(22,8),
  LOCATION_PC      NUMBER(22,8),
  EARTH_RADIUS_KM  NUMBER(22,8),
  CONTEXT_ID       NUMBER(15)                   NOT NULL
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


CREATE UNIQUE INDEX CALC.PK_CLIENT_CONTEXTS_GLOBALPARAMS ON CALC.CLIENT_CONTEXTS_GLOBALPARAMS
(CONTEXT_ID)
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


ALTER TABLE CALC.CLIENT_CONTEXTS_GLOBALPARAMS ADD (
  CONSTRAINT PK_CLIENT_CONTEXTS_GLOBALPARAMS
 PRIMARY KEY
 (CONTEXT_ID));
