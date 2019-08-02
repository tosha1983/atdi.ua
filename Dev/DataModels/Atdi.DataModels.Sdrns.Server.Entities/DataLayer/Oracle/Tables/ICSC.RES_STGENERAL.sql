CREATE TABLE ICSC.RES_STGENERAL
(
  ID                      NUMBER(15)            NOT NULL,
  RES_MEAS_STATION_ID     NUMBER(15),
  CENTRAL_FREQUENCY       NUMBER(30,10),
  CENTRAL_FREQUENCY_MEAS  NUMBER(30,10),
  DURATION_MEAS           NUMBER(30,10),
  MARKER_INDEX            NUMBER(9),
  T1                      NUMBER(9),
  T2                      NUMBER(9),
  TIME_FINISH_MEAS        DATE,
  TIME_START_MEAS    DATE,
  OFFSET_FREQUENCY        NUMBER(30,10),
  SPECRUM_START_FREQ      NUMBER(22,8),
  SPECRUM_STEPS           NUMBER(22,8),
  CORRECTNESS_ESTIM       NUMBER(1),
  TRACE_COUNT             NUMBER(9),
  VBW                     NUMBER(30,10),
  RBW                     NUMBER(30,10),
  BW                      NUMBER(30,10),
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
LOB (LEVEL_SSPECTRUM_DBM) STORE AS SECUREFILE 
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
                    INITIAL          104K
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


CREATE UNIQUE INDEX ICSC.RES_STGENERAL_ID_PK ON ICSC.RES_STGENERAL
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
  CONSTRAINT RESSTGENERAL_PK
 PRIMARY KEY
 (ID));
