﻿CREATE TABLE ICSC.SIGN_SYSINFO_WTIMES
(
  ID                 NUMBER(15)                 NOT NULL,
  START_EMIT         DATE                       NOT NULL,
  STOP_EMIT          DATE                       NOT NULL,
  HIT_COUNT          NUMBER(9)                  NOT NULL,
  PERCENT_AVAILABLE  NUMBER(22,8)               NOT NULL,
  SYSINFO_ID         NUMBER(15)                 NOT NULL
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


CREATE INDEX ICSC.SIGN_SIGNSYSINFOSID_FK ON ICSC.SIGN_SYSINFO_WTIMES
(SYSINFO_ID)
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


CREATE UNIQUE INDEX ICSC.SIGN_SYSINFO_WTIMES_PK ON ICSC.SIGN_SYSINFO_WTIMES
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


ALTER TABLE ICSC.SIGN_SYSINFO_WTIMES ADD (
  CONSTRAINT SIGN_SYSINFO_WTIMES_PK
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
