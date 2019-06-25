﻿CREATE TABLE ICSC.MEAS_SUB_TASK_STATION
(
  ID              NUMBER(15)                    NOT NULL,
  STATUS          NVARCHAR2(50),
  COUNT           NUMBER(9),
  TIME_NEXT_TASK  DATE,
  ID_SENSOR       NUMBER(15),
  ID_MEASSUBTASK  NUMBER(15)                    NOT NULL
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


CREATE UNIQUE INDEX ICSC.XBS_MEASSUBTASKSTA_PK ON ICSC.MEAS_SUB_TASK_STATION
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


ALTER TABLE ICSC.MEAS_SUB_TASK_STATION ADD (
  CONSTRAINT XBS_MEASSUBTASKSTA_PK
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

ALTER TABLE ICSC.MEAS_SUB_TASK_STATION ADD (
  CONSTRAINT FK_XBS_MEASSUBTASKSTA_XBS_MEAS 
 FOREIGN KEY (ID_MEASSUBTASK) 
 REFERENCES ICSC.MEAS_SUB_TASK (ID));