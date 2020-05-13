CREATE TABLE ICSC.HEALTH_LOG_DETAIL
(
  ID              NUMBER(15)                    NOT NULL,
  HEALTH_ID       NUMBER(15)                    NOT NULL,
  CREATED_DATE    TIMESTAMP(7) WITH TIME ZONE   NOT NULL,
  MESSAGE         NVARCHAR2(250)                NOT NULL,
  THREAD_ID       NUMBER(9)                     NOT NULL,
  SOURCE          NVARCHAR2(450),
  SITE_TYPE_CODE  NUMBER(3)                     NOT NULL,
  SITE_TYPE_NAME  NVARCHAR2(50)                 NOT NULL,
  SITE_INSTANCE   NVARCHAR2(250)                NOT NULL,
  SITE_HOST       NVARCHAR2(250),
  NOTE            NCLOB
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
LOB (NOTE) STORE AS SECUREFILE 
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


ALTER TABLE ICSC.HEALTH_LOG_DETAIL ADD (
  PRIMARY KEY
 (ID));
