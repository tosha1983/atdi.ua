﻿CREATE TABLE ICSC.CLIENT_CONTEXTS_ABSORPTION
(
  MODEL_TYPE_CODE  NUMBER(3),
  MODEL_TYPE_NAME  NVARCHAR2(100),
  AVAILABLE        NUMBER(1),
  CONTEXT_ID       NUMBER(15),
  HYBRID           NUMBER(1)                    DEFAULT 0
)
TABLESPACE USERS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX ICSC.PK_CLIENT_CONTEXTS_ABSORPTION ON ICSC.CLIENT_CONTEXTS_ABSORPTION
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


ALTER TABLE ICSC.CLIENT_CONTEXTS_ABSORPTION ADD (
  CONSTRAINT PK_CLIENT_CONTEXTS_ABSORPTION
 PRIMARY KEY
 (CONTEXT_ID));
