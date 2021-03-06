﻿CREATE TABLE ICSC.AMQP_EVENTS
(
  ID            NUMBER(15)                      NOT NULL,
  PROP_TYPE     VARCHAR2(250 CHAR),
  CREATED_DATE  TIMESTAMP(7) WITH TIME ZONE     DEFAULT sysdate               NOT NULL
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
MONITORING
ENABLE ROW MOVEMENT;


CREATE UNIQUE INDEX ICSC.AMQP_EVENTS_ID_PK ON ICSC.AMQP_EVENTS
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


ALTER TABLE ICSC.AMQP_EVENTS ADD (
  CONSTRAINT AMQP_EVENTS_PK
 PRIMARY KEY
 (ID));
