CREATE TABLE ICSC.SPECTRUM
(
  ID               NUMBER(15)                   NOT NULL,
  STARTFREQ_MHZ    NUMBER(30,10),
  STEPFREQ_KHZ     NUMBER(30,10),
  T1               NUMBER(9),
  T2               NUMBER(9),
  MARKER_INDEX     NUMBER(9),
  BW_KHZ           NUMBER(30,10),
  CORRECT_ESTIM    NUMBER(1),
  TRACE_COUNT      NUMBER(9),
  SIGNALLEVEL_DBM  NUMBER(22,8),
  EMITTING_ID      NUMBER(15),
  CONTRAVENTION    NUMBER(1),
  LEVELS_DBM       BLOB
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
LOB (LEVELS_DBM) STORE AS SECUREFILE 
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


CREATE INDEX ICSC.SPECTRUM_EMITTING_ID_FK ON ICSC.SPECTRUM
(EMITTING_ID)
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


CREATE UNIQUE INDEX ICSC.SPECTRUM_ID_PK ON ICSC.SPECTRUM
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


ALTER TABLE ICSC.SPECTRUM ADD (
  CONSTRAINT SPECTRUM_PK
 PRIMARY KEY
 (ID));
