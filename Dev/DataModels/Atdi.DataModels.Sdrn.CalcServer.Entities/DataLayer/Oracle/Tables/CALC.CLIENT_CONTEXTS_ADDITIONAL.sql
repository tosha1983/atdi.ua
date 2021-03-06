﻿CREATE TABLE CALC.CLIENT_CONTEXTS_ADDITIONAL
(
  MODEL_TYPE_CODE  NUMBER(3)                    NOT NULL,
  MODEL_TYPE_NAME  NVARCHAR2(100)               NOT NULL,
  AVAILABLE        NUMBER(1)                    NOT NULL,
  CONTEXT_ID       NUMBER(15)                   NOT NULL
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


CREATE UNIQUE INDEX CALC.PK_CLIENT_CONTEXTS_ADDITIONAL ON CALC.CLIENT_CONTEXTS_ADDITIONAL
(CONTEXT_ID)
LOGGING
TABLESPACE USERS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;


ALTER TABLE CALC.CLIENT_CONTEXTS_ADDITIONAL ADD (
  CONSTRAINT PK_CLIENT_CONTEXTS_ADDITIONAL
 PRIMARY KEY
 (CONTEXT_ID));
