CREATE TABLE CALC.CLUTTERS_DESCS
(
  ID             NUMBER(15)                     NOT NULL,
  INFOC_DESC_ID  NUMBER(15)                     NOT NULL,
  MAP_ID         NUMBER(15)                     NOT NULL,
  CREATED_DATE   TIMESTAMP(7) WITH TIME ZONE    DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  NAME           NVARCHAR2(450)                 NOT NULL,
  NOTE           NCLOB
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


CREATE UNIQUE INDEX CALC.CLUTTERS_DESCS_PK ON CALC.CLUTTERS_DESCS
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


ALTER TABLE CALC.CLUTTERS_DESCS ADD (
  CONSTRAINT CLUTTERS_DESCS_PK
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
