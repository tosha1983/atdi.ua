﻿CREATE TABLE ICSC.SENSOR_EQUIP
(
  ID              NUMBER(15)                    NOT NULL,
  SENSOR_ID       NUMBER(15),
  CODE            NVARCHAR2(50),
  MANUFACTURER    NVARCHAR2(50),
  NAME            NVARCHAR2(50),
  FAMILY          NVARCHAR2(50),
  TECHID          NVARCHAR2(200),
  VERSION         NVARCHAR2(50),
  LOWER_FREQ      NUMBER(30,10),
  UPPER_FREQ      NUMBER(30,10),
  RBW_MIN         NUMBER(30,10),
  RBW_MAX         NUMBER(30,10),
  VBW_MIN         NUMBER(30,10),
  VBW_MAX         NUMBER(30,10),
  MOBILITY        NUMBER(1),
  FFT_POINT_MAX   NUMBER(30,10),
  REF_LEVEL_DBM   NUMBER(30,10),
  OPERATION_MODE  NVARCHAR2(50),
  TYPE            NVARCHAR2(50),
  EQUIP_CLASS     NVARCHAR2(50),
  TUNING_STEP     NUMBER(30,10),
  USE_TYPE        NVARCHAR2(50),
  CATEGORY        NVARCHAR2(50),
  REMARK          NVARCHAR2(250),
  CUSTTXT1        NVARCHAR2(250),
  CUSTDATA1       DATE,
  CUSTNBR1        NUMBER(30,10)
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


CREATE UNIQUE INDEX ICSC.SENSOR_EQUIP_ID_PK ON ICSC.SENSOR_EQUIP
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


ALTER TABLE ICSC.SENSOR_EQUIP ADD (
  CONSTRAINT SENSOREQUIP_PK
 PRIMARY KEY
 (ID));
