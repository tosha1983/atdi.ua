CREATE TABLE ICSC.HEALTH_LOG
(
  ID                NUMBER(15)                  NOT NULL,
  SENDER_LOG_ID     NUMBER(15),
  SENDER_TYPE_CODE  NUMBER(3),
  SENDER_TYPE_NAME  NVARCHAR2(50),
  SENDER_INSTANCE   NVARCHAR2(250),
  SENDER_HOST       NVARCHAR2(250),
  SOURCE_TYPE_CODE  NUMBER(3)                   NOT NULL,
  SOURCE_TYPE_NAME  NVARCHAR2(50)               NOT NULL,
  SOURCE_INSTANCE   NVARCHAR2(250)              NOT NULL,
  SOURCE_TECHID     NVARCHAR2(250),
  SOURCE_HOST       NVARCHAR2(250),
  EVENT_CODE        NUMBER(3)                   NOT NULL,
  EVENT_NAME        NVARCHAR2(50)               NOT NULL,
  DISPATCH_TIME     TIMESTAMP(7) WITH TIME ZONE NOT NULL,
  RECEIVED_TIME     TIMESTAMP(7) WITH TIME ZONE NOT NULL,
  FORWARDED_TIME    TIMESTAMP(7) WITH TIME ZONE,
  EVENT_NOTE        NCLOB
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
LOB (EVENT_NOTE) STORE AS SECUREFILE 
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


CREATE UNIQUE INDEX ICSC.HEALTH_LOG_PK ON ICSC.HEALTH_LOG
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


ALTER TABLE ICSC.HEALTH_LOG ADD (
  CONSTRAINT HEALTH_LOG_PK
 PRIMARY KEY
 (ID));
