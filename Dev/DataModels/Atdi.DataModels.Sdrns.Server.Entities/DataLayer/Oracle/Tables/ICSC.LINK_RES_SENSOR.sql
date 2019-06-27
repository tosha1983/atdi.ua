﻿CREATE TABLE ICSC.LINK_RES_SENSOR
(
  ID                   NUMBER(15)               NOT NULL,
  SENSOR_ID            NUMBER(15),
  RES_MEAS_STATION_ID  NUMBER(15)
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


CREATE INDEX ICSC.IDXBSRESMEASSTATIONK ON ICSC.LINK_RES_SENSOR
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


CREATE INDEX ICSC.XBS_ID_XBS_SENSORKEY ON ICSC.LINK_RES_SENSOR
(SENSOR_ID)
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


CREATE UNIQUE INDEX ICSC.XBS_LINKRESSENSOR_PK ON ICSC.LINK_RES_SENSOR
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


ALTER TABLE ICSC.LINK_RES_SENSOR ADD (
  CONSTRAINT XBS_LINKRESSENSOR_PK
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

ALTER TABLE ICSC.LINK_RES_SENSOR ADD (
  CONSTRAINT FK_XBS_LINKRESSENSOR_XBS_RESME 
 FOREIGN KEY (RES_MEAS_STATION_ID) 
 REFERENCES ICSC.RES_MEAS_STATION (ID),
  CONSTRAINT FK_XBS_LINKRESSENSOR_XBS_SENSO 
 FOREIGN KEY (SENSOR_ID) 
 REFERENCES ICSC.SENSOR (ID));
