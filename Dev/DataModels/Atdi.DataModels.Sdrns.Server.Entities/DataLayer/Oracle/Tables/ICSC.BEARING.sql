CREATE TABLE ICSC.BEARING
(
  ID                    NUMBER(15)              NOT NULL,
  RES_MEAS_STATION_ID   NUMBER(15),
  LEVEL_DBM             NUMBER(30,10),
  LEVEL_DBMKVM          NUMBER(30,10),
  TIME_OF_MEASUREMENTS  DATE,
  BW                    NUMBER(30,10),
  QUALITY               NUMBER(30,10),
  CENTRAL_FREQUENCY     NUMBER(30,10),
  BEARING               NUMBER(30,10),
  AZIMUTH               NUMBER(30,10),
  ASL                   NUMBER(30,10),
  LON                   NUMBER(30,10),
  LAT                   NUMBER(30,10),
  AGL                   NUMBER(30,10)
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


CREATE UNIQUE INDEX ICSC.BEARING_PK ON ICSC.BEARING
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


CREATE INDEX ICSC.BEARING_RES_MEAS_STATION_ID_PK ON ICSC.BEARING
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


ALTER TABLE ICSC.BEARING ADD (
  CONSTRAINT BEARING_PK
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
