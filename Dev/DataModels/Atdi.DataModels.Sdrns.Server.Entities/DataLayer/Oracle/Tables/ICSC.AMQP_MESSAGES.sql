CREATE TABLE ICSC.AMQP_MESSAGES
(
  ID                     NUMBER(15)             NOT NULL,
  STATUS_CODE            NUMBER(3)              NOT NULL,
  CREATED_DATE           TIMESTAMP(7) WITH TIME ZONE NOT NULL,
  THREAD_ID              NUMBER(15)             NOT NULL,
  PROCESSED_SDATE        TIMESTAMP(7) WITH TIME ZONE,
  PROCESSED_FDATE        TIMESTAMP(7) WITH TIME ZONE,
  PROP_EXCHANGE          NVARCHAR2(250),
  PROP_ROUTING_KEY       NVARCHAR2(250),
  PROP_DELIVERY_TAG      NVARCHAR2(250),
  PROP_CONSUMER_TAG      NVARCHAR2(250),
  PROP_APP_ID            NVARCHAR2(250),
  PROP_TYPE              NVARCHAR2(250),
  PROP_TIMESTAMP         NVARCHAR2(50),
  PROP_MESSAGE_ID        NVARCHAR2(250),
  PROP_CORRELATION_ID    NVARCHAR2(250),
  PROP_CONTENT_ENCODING  NVARCHAR2(250),
  PROP_CONTENT_TYPE      NVARCHAR2(250),
  HEADER_CREATED         NVARCHAR2(250),
  HEADER_SDRNSERVER      NVARCHAR2(250),
  HEADER_SENSORNAME      NVARCHAR2(250),
  HEADER_SENSORTECHID    NVARCHAR2(250),
  BODY_CONTENT_TYPE      NVARCHAR2(250),
  BODY_CONTENT_ENCODING  NVARCHAR2(250),
  BODY_CONTENT           BLOB,
  STATUS_NOTE            NCLOB
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
LOB (STATUS_NOTE) STORE AS SECUREFILE 
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
  LOB (BODY_CONTENT) STORE AS SECUREFILE 
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


CREATE UNIQUE INDEX ICSC.AMQP_MESSAGES_ID_PK ON ICSC.AMQP_MESSAGES
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


ALTER TABLE ICSC.AMQP_MESSAGES ADD (
  CONSTRAINT AMQP_MESSAGES_ID_PK
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
