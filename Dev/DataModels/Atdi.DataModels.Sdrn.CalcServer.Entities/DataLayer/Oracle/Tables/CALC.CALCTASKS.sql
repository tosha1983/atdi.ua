CREATE TABLE CALC.CALCTASKS
(
  ID              NUMBER(15)                    NOT NULL,
  CONTEXT_ID      NUMBER(15)                    NOT NULL,
  TYPE_CODE       NUMBER(9)                     NOT NULL,
  TYPE_NAME       NVARCHAR2(250)                NOT NULL,
  STATUS_CODE     NUMBER(3)                     NOT NULL,
  STATUS_NAME     NVARCHAR2(50)                 NOT NULL,
  STATUS_NOTE     NCLOB,
  OWNER_INSTANCE  NVARCHAR2(250)                NOT NULL,
  OWNER_TASK_ID   VARCHAR2(36 BYTE)             NOT NULL,
  CREATED_DATE    TIMESTAMP(7) WITH TIME ZONE   DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  MAP_NAME        NVARCHAR2(250),
  NOTE            NVARCHAR2(250)
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
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX CALC.CALCTASKS_PK ON CALC.CALCTASKS
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


ALTER TABLE CALC.CALCTASKS ADD (
  CONSTRAINT CALCTASKS_PK
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
