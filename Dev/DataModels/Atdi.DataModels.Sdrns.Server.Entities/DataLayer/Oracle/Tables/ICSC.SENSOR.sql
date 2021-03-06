﻿CREATE TABLE ICSC.SENSOR
(
  ID                   NUMBER(15)               NOT NULL,
  SENSORIDENTIFIER_ID  NUMBER(15),
  STATUS               NVARCHAR2(50),
  TITLE				   NVARCHAR2(250),
  NAME                 NVARCHAR2(150),
  ADMINISTRATION       NVARCHAR2(50),
  NETWORKID            NVARCHAR2(150),
  REMARK               NVARCHAR2(250),
  BIUSEDATE            DATE,
  EOUSEDATE            DATE,
  AZIMUTH              NUMBER(30,10),
  ELEVATION            NUMBER(30,10),
  AGL                  NUMBER(30,10),
  IDSYSARGUS           NVARCHAR2(50),
  TYPESENSOR           NVARCHAR2(50),
  STEPMEASTIME         NUMBER(30,10),
  RXLOSS               NUMBER(30,10),
  OPHHFR               NUMBER(30,10),
  OPHHTO               NUMBER(30,10),
  OPDAYS               NVARCHAR2(50),
  CUSTTXT1             NVARCHAR2(50),
  CUSTDATA1            DATE,
  CUSTNBR1             NUMBER(30,10),
  DATECREATED          DATE,
  CREATEDBY            VARCHAR2(50 BYTE),
  APIVERSION           VARCHAR2(10 BYTE)        DEFAULT 'v1.0',
  TECHID               VARCHAR2(150 BYTE),
  LASTACTIVITY         DATE
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


CREATE UNIQUE INDEX ICSC.SENSOR_ID_PK ON ICSC.SENSOR
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


CREATE UNIQUE INDEX ICSC.UNIQUE_NAME_TECHID ON ICSC.SENSOR
(NAME, TECHID)
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


ALTER TABLE ICSC.SENSOR ADD (
  CONSTRAINT SENSOR_PK
 PRIMARY KEY
 (ID),
  CONSTRAINT UNIQUE_NAME_TECHID_F1UQ
 UNIQUE (NAME, TECHID));
