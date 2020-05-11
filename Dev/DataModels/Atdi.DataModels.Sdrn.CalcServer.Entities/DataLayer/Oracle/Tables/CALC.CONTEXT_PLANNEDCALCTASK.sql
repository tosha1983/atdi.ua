CREATE TABLE CALC.CONTEXT_PLANNEDCALCTASK
(
  ID            NUMBER(15)                      NOT NULL,
  CONTEXT_ID    NUMBER(15)                      NOT NULL,
  TYPE_CODE     NUMBER(9)                       NOT NULL,
  TYPE_NAME     NVARCHAR2(250)                  NOT NULL,
  START_NUMBER  NUMBER(9)                       NOT NULL,
  MAP_NAME      NVARCHAR2(250),
  NOTE          NCLOB
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


CREATE UNIQUE INDEX CALC.CONTEXT_PLANNEDCALCTASK_PK ON CALC.CONTEXT_PLANNEDCALCTASK
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


ALTER TABLE CALC.CONTEXT_PLANNEDCALCTASK ADD (
  CONSTRAINT CONTEXT_PLANNEDCALCTASK_PK
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
