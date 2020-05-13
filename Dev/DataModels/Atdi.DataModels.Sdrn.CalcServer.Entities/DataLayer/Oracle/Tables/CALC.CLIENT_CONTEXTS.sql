﻿CREATE TABLE CALC.CLIENT_CONTEXTS
(
  ID               NUMBER(15)                   NOT NULL,
  BASE_CONTEXT_ID  NUMBER(15),
  PROJECT_ID       NUMBER(15)                   NOT NULL,
  CONTEXT_NAME     NVARCHAR2(250)               NOT NULL,
  CONTEXT_NOTE     NCLOB,
  OWNER_INSTANCE   NVARCHAR2(250)               NOT NULL,
  OWNER_TASK_ID    VARCHAR2(36 BYTE)            NOT NULL,
  CREATED_DATE     TIMESTAMP(7) WITH TIME ZONE  DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  TYPE_CODE        NUMBER(3)                    NOT NULL,
  TYPE_NAME        NVARCHAR2(50)                NOT NULL,
  STATUS_CODE      NUMBER(3)                    NOT NULL,
  STATUS_NAME      NVARCHAR2(50)                NOT NULL,
  STATUS_NOTE      NCLOB
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


CREATE UNIQUE INDEX CALC.CLIENT_CONTEXTS_PK ON CALC.CLIENT_CONTEXTS
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


ALTER TABLE CALC.CLIENT_CONTEXTS ADD (
  CONSTRAINT CLIENT_CONTEXTS_PK
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
