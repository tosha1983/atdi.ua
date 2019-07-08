CREATE TABLE ICSC.RES_STGENERAL
(
  ID                      NUMBER(15)            NOT NULL,
  RES_MEAS_STATION_ID     NUMBER(15),
  CENTRAL_FREQUENCY       NUMBER(22,8),
  CENTRAL_FREQUENCY_MEAS  NUMBER(22,8),
  DURATION_MEAS           NUMBER(22,8),
  MARKER_INDEX            NUMBER(9),
  T1                      NUMBER(9),
  T2                      NUMBER(9),
  TIME_FINISH_MEAS        DATE,
  TIME_START_MEAS_DATE    DATE,
  OFFSET_FREQUENCY        NUMBER(22,8),
  SPECRUM_START_FREQ      NUMBER(30,10),
  SPECRUM_STEPS           NUMBER(30,10),
  CORRECTNESS_ESTIM       NUMBER(1),
  TRACE_COUNT             NUMBER(9),
  VBW                     NUMBER(22,8),
  RBW                     NUMBER(22,8),
  BW                      NUMBER(22,8),
  LEVEL_SSPECTRUM_DBM     BLOB
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
LOB (LEVEL_SSPECTRUM_DBM) STORE AS 
      ( TABLESPACE  USERS 
        ENABLE      STORAGE IN ROW
        CHUNK       8192
        RETENTION
        NOCACHE
        INDEX       (
          TABLESPACE USERS
          STORAGE    (
                      INITIAL          64K
                      NEXT             1
                      MINEXTENTS       1
                      MAXEXTENTS       UNLIMITED
                      PCTINCREASE      0
                      BUFFER_POOL      DEFAULT
                     ))
        STORAGE    (
                    INITIAL          64K
                    NEXT             1M
                    MINEXTENTS       1
                    MAXEXTENTS       UNLIMITED
                    PCTINCREASE      0
                    BUFFER_POOL      DEFAULT
                   )
      )
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX ICSC.XBS_RESSTGENERAL_PK ON ICSC.RES_STGENERAL
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


ALTER TABLE ICSC.RES_STGENERAL ADD (
  CONSTRAINT XBS_RESSTGENERAL_PK
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

ALTER TABLE ICSC.RES_STGENERAL ADD (
  CONSTRAINT FK_XBS_RESSTGENERAL_XBS_RESMEA 
 FOREIGN KEY (RES_MEAS_STATION_ID) 
 REFERENCES ICSC.RES_MEAS_STATION (ID));
