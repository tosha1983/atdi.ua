﻿CREATE TABLE ICSC.AMQP_MESSAGES_LOG
(
  ID            NUMBER(15)                      NOT NULL,
  MESSAGE_ID    NUMBER(15)                      NOT NULL,
  STATUS_CODE   NUMBER(3)                       NOT NULL,
  STATUS_NAME   NVARCHAR2(50),
  STATUS_NOTE   VARCHAR2(4000 BYTE),
  CREATED_DATE  TIMESTAMP(7) WITH TIME ZONE     NOT NULL,
  THREAD_ID     NUMBER(9),
  SOURCE        NVARCHAR2(450)
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


CREATE UNIQUE INDEX ICSC.AMQP_MESSAGES_LOG_PK ON ICSC.AMQP_MESSAGES_LOG
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


ALTER TABLE ICSC.AMQP_MESSAGES_LOG ADD (
  CONSTRAINT AMQP_MESSAGES_LOG_PK
 PRIMARY KEY
 (ID));