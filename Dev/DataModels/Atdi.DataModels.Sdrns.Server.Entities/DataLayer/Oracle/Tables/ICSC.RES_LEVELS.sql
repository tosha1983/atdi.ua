CREATE TABLE ICSC.RES_LEVELS
(
  ID                  NUMBER(15)                NOT NULL,
  VALUE_LVL           NUMBER(22,8),
  STDDEV_LVL          NUMBER(30,10),
  VMIN_LVL            NUMBER(22,8),
  VMMAX_LVL           NUMBER(22,8),
  LIMIT_LVL           NUMBER(30,10),
  OCCUPANCY_LVL       NUMBER(30,10),
  PMIN_LVL            NUMBER(30,10),
  PMAX_LVL            NUMBER(30,10),
  PDIFF_LVL           NUMBER(30,10),
  FREQ_MEAS           NUMBER(22,8),
  VALUE_SPECT         NUMBER(22,8),
  STDDEV_SPECT        NUMBER(30,10),
  VMIN_SPECT          NUMBER(30,10),
  VMMAX_SPECT         NUMBER(30,10),
  LIMIT_SPECT         NUMBER(30,10),
  OCCUPANCY_SPECT     NUMBER(22,8),
  RES_MEAS_ID         NUMBER(15),
  LEVEL_MIN_ARR       NUMBER(22,8),
  SPECTRUM_OCCUP_ARR  BLOB
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
LOB (SPECTRUM_OCCUP_ARR) STORE AS SECUREFILE 
      ( TABLESPACE  USERS 
        ENABLE      STORAGE IN ROW
        CHUNK       8192
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


CREATE UNIQUE INDEX ICSC.RES_LEVELS_ID_PK ON ICSC.RES_LEVELS
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


CREATE INDEX ICSC.RES_LEVELS_RES_MEAS_ID_PK ON ICSC.RES_LEVELS
(RES_MEAS_ID)
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


ALTER TABLE ICSC.RES_LEVELS ADD (
  CONSTRAINT ID_RESLEVELS_PK
 PRIMARY KEY
 (ID));
