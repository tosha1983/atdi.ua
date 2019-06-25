﻿CREATE TABLE ICSC.RES_LEV_MEAS_ONLINE
(
  ID          NUMBER(15)                        NOT NULL,
  VALUE       NUMBER(22,8),
  RESMEAS_ID  NUMBER(15)
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


CREATE INDEX ICSC.XBS_ID_XBS_RESSTATIONMEAS_PK ON ICSC.RES_LEV_MEAS_ONLINE
(RESMEAS_ID)
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


CREATE UNIQUE INDEX ICSC.XBS_RESLEVMEASONLINE_PK ON ICSC.RES_LEV_MEAS_ONLINE
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


ALTER TABLE ICSC.RES_LEV_MEAS_ONLINE ADD (
  CONSTRAINT XBS_RESLEVMEASONLINE_PK
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

ALTER TABLE ICSC.RES_LEV_MEAS_ONLINE ADD (
  CONSTRAINT FK_XBS_RESLEVMEASONLINE_XBS_RE 
 FOREIGN KEY (RESMEAS_ID) 
 REFERENCES ICSC.RES_MEAS (ID));