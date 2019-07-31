﻿CREATE TABLE ICSC.VALIDATION_LOGS
(
  ID          NUMBER(15)                        NOT NULL,
  TABLE_NAME   NVARCHAR2(150),
  WHEN        DATE,                              
  MESSAGE_ID  NUMBER(15),	
  RES_MEAS_ID NUMBER(15),
  INFO        NCLOB
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


CREATE UNIQUE INDEX ICSC.VALIDATION_LOGS_ID_PK ON ICSC.VALIDATION_LOGS
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


ALTER TABLE ICSC.VALIDATION_LOGS ADD (
  CONSTRAINT PK_XBS_LOGS
 PRIMARY KEY
 (ID));