
CREATE TABLE CALC.CLUTTERS_DESCS_CLUTTERS
(
  CODE              NUMBER(3)                   NOT NULL,
  NAME              NVARCHAR2(150)              NOT NULL,
  NOTE              NCLOB,
  HEIGHT_M          NUMBER(3)                   NOT NULL,
  CLUTTERS_DESC_ID  NUMBER(15)                  NOT NULL
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
LOB (NOTE) STORE AS SECUREFILE 
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


CREATE UNIQUE INDEX CALC.PK_CLUTTERS_DESCS_CLUTTERS ON CALC.CLUTTERS_DESCS_CLUTTERS
(CLUTTERS_DESC_ID, CODE)
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


ALTER TABLE CALC.CLUTTERS_DESCS_CLUTTERS ADD (
  CONSTRAINT PK_CLUTTERS_DESCS_CLUTTERS
 PRIMARY KEY
 (CLUTTERS_DESC_ID, CODE)
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
