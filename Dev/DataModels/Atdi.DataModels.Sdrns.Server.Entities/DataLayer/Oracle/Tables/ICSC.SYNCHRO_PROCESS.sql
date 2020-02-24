CREATE TABLE ICSC.SYNCHRO_PROCESS
(
  ID                              NUMBER(15)    NOT NULL,
  CREATED_BY                      NVARCHAR2(50),
  CREATED_DATE                    DATE,
  STATUS                          NVARCHAR2(20),
  DATE_START                      DATE,
  DATE_END                        DATE,
  COUNT_RECORDS_IMPORTED          NUMBER(9),
  COUNT_RECORDS_OUT               NUMBER(9),
  COUNT_RECORDS_OUT_WITHOUT_EMIT  NUMBER(9)
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


CREATE UNIQUE INDEX ICSC.SYNCHRO_PROCESS_PK ON ICSC.SYNCHRO_PROCESS
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


ALTER TABLE ICSC.SYNCHRO_PROCESS ADD (
  CONSTRAINT SYNCHRO_PROCESS_PK
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
