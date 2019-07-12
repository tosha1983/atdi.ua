﻿CREATE TABLE ICSC.SENSOR_LOCATION
(
  ID            NUMBER(15)                      NOT NULL,
  SENSOR_ID     NUMBER(15),
  DATA_FROM     DATE,
  DATA_TO       DATE,
  DATA_CREATED  DATE,
  STATUS        NVARCHAR2(50),
  LON           NUMBER(30,10),
  LAT           NUMBER(30,10),
  ASL           NUMBER(30,10)
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


CREATE UNIQUE INDEX ICSC.SENSOR_LOCATION_ID_PK ON ICSC.SENSOR_LOCATION
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


ALTER TABLE ICSC.SENSOR_LOCATION ADD (
  CONSTRAINT SENSORLOCATION_PK
 PRIMARY KEY
 (ID));
