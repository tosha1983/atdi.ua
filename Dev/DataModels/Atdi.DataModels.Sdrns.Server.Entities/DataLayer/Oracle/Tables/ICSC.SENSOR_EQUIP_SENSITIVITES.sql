﻿CREATE TABLE ICSC.SENSOR_EQUIP_SENSITIVITES
(
  ID              NUMBER(15)                    NOT NULL,
  SENSOREQUIP_ID  NUMBER(15),
  FREQ            NUMBER(22,8),
  KTBF            NUMBER(22,8),
  NOISEF          NUMBER(22,8),
  FREQ_STABILITY  NUMBER(22,8),
  ADDLOSS         NUMBER(22,8)
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


CREATE UNIQUE INDEX ICSC.XBS_SENSOREQUIPSENS_PK ON ICSC.SENSOR_EQUIP_SENSITIVITES
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


ALTER TABLE ICSC.SENSOR_EQUIP_SENSITIVITES ADD (
  CONSTRAINT XBS_SENSOREQUIPSENS_PK
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

ALTER TABLE ICSC.SENSOR_EQUIP_SENSITIVITES ADD (
  CONSTRAINT FK_XBS_SENSOREQUIPSENS_XBS_SEN 
 FOREIGN KEY (SENSOREQUIP_ID) 
 REFERENCES ICSC.SENSOR_EQUIP (ID));
