﻿CREATE TABLE ICSC.SIGN_SYSINFO
(
  ID              NUMBER(15)                    NOT NULL,
  BANDWIDTH_HZ    NUMBER(30,10),
  FREQ_HZ         NUMBER(22,8)                  NOT NULL,
  LEVEL_DBM       NUMBER(30,10),
  CID             NUMBER(9),
  MCC             NUMBER(9),
  MNC             NUMBER(9),
  BSIC            NUMBER(9),
  CHANNEL_NUMBER  NUMBER(9),
  LAC             NUMBER(9),
  RNC             NUMBER(9),
  CTOI            NUMBER(30,10),
  POWER           NUMBER(30,10),
  EMITTING_ID     NUMBER(15),
  STANDARD        VARCHAR2(20 CHAR)             NOT NULL
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


CREATE INDEX ICSC.SIGN_SYSINFO_EMITTINGID_FK ON ICSC.SIGN_SYSINFO
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


CREATE UNIQUE INDEX ICSC.SIGN_SYSINFOS_PK ON ICSC.SIGN_SYSINFO
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


ALTER TABLE ICSC.SIGN_SYSINFO ADD (
  CONSTRAINT SIGN_SYSINFOS_PK
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
