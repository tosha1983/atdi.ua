CREATE TABLE ICSC.REF_SPECTRUM
(
  ID                    NUMBER(15)              NOT NULL,
  ID_NUM                NUMBER(9),
  TABLE_NAME            NVARCHAR2(50),
  TABLE_ID              NUMBER(9),
  SENSOR_ID             NUMBER(15),
  GLOBAL_SID            NVARCHAR2(50),
  FREQ_MHZ              NUMBER(30,10),
  LEVEL_DBM             NUMBER(30,10),
  DISPERSION_LOW        NUMBER(30,10),
  DISPERSION_UP         NUMBER(30,10),
  PERCENT               NUMBER(30,10),
  DATE_MEAS             DATE,
  HEAD_REF_SPECTRUM_ID  NUMBER(15),
  STATUS_MEAS           NVARCHAR2(4)
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


CREATE UNIQUE INDEX ICSC.REF_SPECTRUM_PK ON ICSC.REF_SPECTRUM
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


ALTER TABLE ICSC.REF_SPECTRUM ADD (
  CONSTRAINT REF_SPECTRUM_PK
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
