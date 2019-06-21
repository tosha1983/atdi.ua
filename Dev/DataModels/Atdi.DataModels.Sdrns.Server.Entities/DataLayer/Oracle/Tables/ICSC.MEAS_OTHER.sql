﻿CREATE TABLE ICSC.MEAS_OTHER
(
  ID                        NUMBER(15)          NOT NULL,
  SW_NUMBER                 NUMBER(9),
  TYPE_SPECTRUM_SCAN        NVARCHAR2(50),
  TYPE_SPECTRUM_OCCUPATION  NVARCHAR2(50),
  LEVEL_MIN_OCCUP           NUMBER(22,8),
  NCHENAL                   NUMBER(9),
  ID_MEASTASK               NUMBER(15)
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


CREATE UNIQUE INDEX ICSC.XBS_MEASOTHER_PK ON ICSC.MEAS_OTHER
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


ALTER TABLE ICSC.MEAS_OTHER ADD (
  CONSTRAINT XBS_MEASOTHER_PK
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

ALTER TABLE ICSC.MEAS_OTHER ADD (
  CONSTRAINT FK_XBS_MEASOTHER_XBS_MEASTASK 
 FOREIGN KEY (ID_MEASTASK) 
 REFERENCES ICSC.MEAS_TASK (ID));
