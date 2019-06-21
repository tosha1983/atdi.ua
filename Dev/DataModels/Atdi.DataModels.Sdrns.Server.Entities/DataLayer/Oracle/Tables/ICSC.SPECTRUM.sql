CREATE TABLE ICSC.SPECTRUM
(
  ID               NUMBER(15)                   NOT NULL,
  STARTFREQ_MHZ    NUMBER(22,8),
  STEPFREQ_KHZ     NUMBER(22,8),
  T1               NUMBER(9),
  T2               NUMBER(9),
  MARKER_INDEX     NUMBER(9),
  BW_KHZ           NUMBER(22,8),
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
LOB (LEVELS_DBM) STORE AS 
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


CREATE INDEX ICSC.XBS_SPECTRUM_EMITTING_ID_FK ON ICSC.SPECTRUM
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


CREATE UNIQUE INDEX ICSC.XBS_SPECTRUM_PK ON ICSC.SPECTRUM
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
  CONSTRAINT XBS_SPECTRUM_PK
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

ALTER TABLE ICSC.SPECTRUM ADD (
  CONSTRAINT FK_XBS_SPECTRUM_XBS_EMITTING 
 FOREIGN KEY (EMITTING_ID) 
 REFERENCES ICSC.EMITTING (ID));
