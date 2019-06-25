﻿CREATE TABLE ICSC.REFERENCE_SITUATION
(
  ID           NUMBER(15)                       NOT NULL,
  SENSOR_ID    NUMBER(15),
  MEASTASK_ID  NUMBER(15)
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


CREATE INDEX ICSC.XBS_REFSITUATION_MEASTASKID_FK ON ICSC.REFERENCE_SITUATION
(MEASTASK_ID)
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


CREATE UNIQUE INDEX ICSC.XBS_REFSITUATION_PK ON ICSC.REFERENCE_SITUATION
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


ALTER TABLE ICSC.REFERENCE_SITUATION ADD (
  CONSTRAINT XBS_REFSITUATION_PK
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

ALTER TABLE ICSC.REFERENCE_SITUATION ADD (
  CONSTRAINT FK_XBS_REFSITUATION_XBS_MEASTA 
 FOREIGN KEY (MEASTASK_ID) 
 REFERENCES ICSC.MEAS_TASK (ID),
  CONSTRAINT FK_XBS_REFSITUATION_XBS_SENSOR 
 FOREIGN KEY (SENSOR_ID) 
 REFERENCES ICSC.SENSOR (ID));