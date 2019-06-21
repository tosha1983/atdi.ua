CREATE TABLE ICSC.RES_SYS_INFO
(
  ID                NUMBER(15)                  NOT NULL,
  BANDWIDTH         NUMBER(22,8),
  BASEID            NUMBER(9),
  BSIC              NUMBER(9),
  CHANNELNUMBER     NUMBER(9),
  CID               NUMBER(9),
  CODE              NUMBER(22,8),
  CTOI              NUMBER(22,8),
  ECI               NUMBER(9),
  ENODEBID          NUMBER(9),
  FREQ              NUMBER(22,8),
  ICIO              NUMBER(22,8),
  INBAND_POWER      NUMBER(22,8),
  ISCP              NUMBER(22,8),
  LAC               NUMBER(9),
  AGL               NUMBER(22,8),
  ASL               NUMBER(22,8),
  LAT               NUMBER(22,8),
  LON               NUMBER(22,8),
  MCC               NUMBER(9),
  MNC               NUMBER(9),
  NID               NUMBER(9),
  PCI               NUMBER(9),
  PN                NUMBER(9),
  POWER             NUMBER(22,8),
  PTOTAL            NUMBER(22,8),
  RNC               NUMBER(9),
  RSCP              NUMBER(22,8),
  RSRP              NUMBER(22,8),
  RSRQ              NUMBER(22,8),
  SC                NUMBER(9),
  SID               NUMBER(9),
  TAC               NUMBER(9),
  TYPECDMAEVDO      VARCHAR2(250 BYTE),
  UCID              NUMBER(9),
  RES_STGENERAL_ID  NUMBER(15)
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


CREATE INDEX ICSC.XBSRESSTGENERALIDINFO_PK ON ICSC.RES_SYS_INFO
(RES_STGENERAL_ID)
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


CREATE UNIQUE INDEX ICSC.XBS_RESSYSINFO_PK ON ICSC.RES_SYS_INFO
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


ALTER TABLE ICSC.RES_SYS_INFO ADD (
  CONSTRAINT XBS_RESSYSINFO_PK
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

ALTER TABLE ICSC.RES_SYS_INFO ADD (
  CONSTRAINT FK_XBS_RESSYSINFO_XBS_RESSTGEN 
 FOREIGN KEY (RES_STGENERAL_ID) 
 REFERENCES ICSC.RES_STGENERAL (ID));
