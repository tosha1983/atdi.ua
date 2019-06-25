﻿CREATE TABLE ICSC.RES_STLEVEL_CAR
(
  ID                    NUMBER(15)              NOT NULL,
  RES_MEAS_STATION_ID   NUMBER(15),
  ALTITUDE              NUMBER(22,8),
  DIFFERENCE_TIMESTAMP  NUMBER(22,8),
  LAT                   NUMBER(22,8),
  LON                   NUMBER(22,8),
  LEVEL_DBM             NUMBER(22,8),
  LEVEL_DBMKVM          NUMBER(22,8),
  TIME_OF_MEASUREMENTS  DATE,
  AGL                   NUMBER(22,8)
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


CREATE INDEX ICSC.XBS_RESSTCARSTATIONID_FK ON ICSC.RES_STLEVEL_CAR
(RES_MEAS_STATION_ID)
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


CREATE UNIQUE INDEX ICSC.XBS_RESSTLEVELCAR_PK ON ICSC.RES_STLEVEL_CAR
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


ALTER TABLE ICSC.RES_STLEVEL_CAR ADD (
  CONSTRAINT XBS_RESSTLEVELCAR_PK
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

ALTER TABLE ICSC.RES_STLEVEL_CAR ADD (
  CONSTRAINT FK_XBS_RESSTLEVELCAR_XBS_RESME 
 FOREIGN KEY (RES_MEAS_STATION_ID) 
 REFERENCES ICSC.RES_MEAS_STATION (ID));