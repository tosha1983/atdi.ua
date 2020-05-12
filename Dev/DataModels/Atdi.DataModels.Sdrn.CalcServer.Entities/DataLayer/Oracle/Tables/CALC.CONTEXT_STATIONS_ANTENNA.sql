CREATE TABLE CALC.CONTEXT_STATIONS_ANTENNA
(
  ID                NUMBER(15)                  NOT NULL,
  GAIN_DB           NUMBER(30,10)               NOT NULL,
  TILT_DEG          NUMBER(22,8)                NOT NULL,
  AZIMUTH_DEG       NUMBER(22,8)                NOT NULL,
  XPD_DB            NUMBER(30,10)               NOT NULL,
  ITU_PATTERN_CODE  NUMBER(3)                   NOT NULL,
  ITU_PATTERN_NAME  NVARCHAR2(50)               NOT NULL
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


CREATE UNIQUE INDEX CALC.CONTEXT_STATIONS_ANTENNA_PK ON CALC.CONTEXT_STATIONS_ANTENNA
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


ALTER TABLE CALC.CONTEXT_STATIONS_ANTENNA ADD (
  CONSTRAINT CONTEXT_STATIONS_ANTENNA_PK
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
