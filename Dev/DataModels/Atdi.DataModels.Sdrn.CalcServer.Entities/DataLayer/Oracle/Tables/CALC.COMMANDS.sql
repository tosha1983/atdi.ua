﻿CREATE TABLE CALC.COMMANDS
(
  ID                 NUMBER(15),
  CREATED_DATE       TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  TYPE_CODE          NUMBER(15)                 NOT NULL,
  TYPE_NAME          NVARCHAR2(250)             NOT NULL,
  STATUS_CODE        NUMBER(3)                  NOT NULL,
  STATUS_NAME        NVARCHAR2(50)              NOT NULL,
  STATUS_NOTE        NCLOB,
  CALLER_INSTANCE    NVARCHAR2(250)             NOT NULL,
  CALLER_COMMAND_ID  VARCHAR2(36 BYTE)          NOT NULL,
  START_TIME         TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT',
  FINISH_TIME        TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT',
  ARGS_JSON          NCLOB,
  RESULT_JSON        NCLOB
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


CREATE UNIQUE INDEX CALC.COMMANDS_PK ON CALC.COMMANDS
(ID)
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


ALTER TABLE CALC.COMMANDS ADD (
  CONSTRAINT COMMANDS_PK
 PRIMARY KEY
 (ID));