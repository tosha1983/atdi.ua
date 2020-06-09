﻿CREATE TABLE CALC.CONTEXT_STATIONS_RECEIVER
(
  STATION_ID         NUMBER(15)                 NOT NULL,
  FREQ_MHZ           NUMBER(22,8)               NOT NULL,
  BW_KHZ             NUMBER(22,8)               NOT NULL,
  LOSS_DB            NUMBER(30,10)              NOT NULL,
  KTBF_DBM           NUMBER(30,10)              NOT NULL,
  THRESHOLD_DBM      NUMBER(30,10)              NOT NULL,
  POLARIZATION_CODE  NUMBER(3)                  NOT NULL,
  POLARIZATION_NAME  NVARCHAR2(50)              NOT NULL,
  FREQS_MHZ          BLOB                       NOT NULL
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


CREATE UNIQUE INDEX CALC.CONTEXT_STATIONS_RECEIVER_PK ON CALC.CONTEXT_STATIONS_RECEIVER
(STATION_ID)
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


ALTER TABLE CALC.CONTEXT_STATIONS_RECEIVER ADD (
  CONSTRAINT CONTEXT_STATIONS_RECEIVER_PK
 PRIMARY KEY
 (STATION_ID)
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
