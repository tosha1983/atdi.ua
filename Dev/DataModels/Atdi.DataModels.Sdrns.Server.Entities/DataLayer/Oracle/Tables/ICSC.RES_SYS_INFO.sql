CREATE TABLE ICSC.RES_SYS_INFO
(
  ID                NUMBER(15)                  NOT NULL,
  BANDWIDTH         NUMBER(30,10),
  BASEID            NUMBER(9),
  BSIC              NUMBER(9),
  CHANNELNUMBER     NUMBER(9),
  CID               NUMBER(9),
  CODE              NUMBER(30,10),
  CTOI              NUMBER(22,8),
  ECI               NUMBER(9),
  ENODEBID          NUMBER(9),
  FREQ              NUMBER(30,10),
  ICIO              NUMBER(30,10),
  INBAND_POWER      NUMBER(30,10),
  ISCP              NUMBER(30,10),
  LAC               NUMBER(9),
  AGL               NUMBER(30,10),
  ASL               NUMBER(30,10),
  LAT               NUMBER(30,10),
  LON               NUMBER(30,10),
  MCC               NUMBER(9),
  MNC               NUMBER(9),
  NID               NUMBER(9),
  PCI               NUMBER(9),
  PN                NUMBER(9),
  POWER             NUMBER(30,10),
  PTOTAL            NUMBER(30,10),
  RNC               NUMBER(9),
  RSCP              NUMBER(30,10),
  RSRP              NUMBER(30,10),
  RSRQ              NUMBER(30,10),
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


CREATE INDEX ICSC.RES_SYS_INFO_GENERAL_ID_PK ON ICSC.RES_SYS_INFO
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


CREATE UNIQUE INDEX ICSC.RES_SYS_INFO_ID_PK ON ICSC.RES_SYS_INFO
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
  CONSTRAINT RESSYSINFO_PK
 PRIMARY KEY
 (ID));
