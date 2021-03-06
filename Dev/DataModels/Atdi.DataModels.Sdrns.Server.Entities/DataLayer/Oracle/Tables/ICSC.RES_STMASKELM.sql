﻿CREATE TABLE ICSC.RES_STMASKELM
(
  ID                NUMBER(15)                  NOT NULL,
  RES_STGENERAL_ID  NUMBER(15),
  BW                NUMBER(30,10),
  "LEVEL"           NUMBER(30,10)
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


CREATE UNIQUE INDEX ICSC.RES_STMASKELM_ID_PK ON ICSC.RES_STMASKELM
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


CREATE INDEX ICSC.RES_STMASKELM_RESSTGEN_ID_PK ON ICSC.RES_STMASKELM
(RES_STGENERAL_ID)
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


ALTER TABLE ICSC.RES_STMASKELM ADD (
  CONSTRAINT RESSTMASKELM_PK
 PRIMARY KEY
 (ID));
