CREATE TABLE CALC.PROJECT_MAP_CONTENTS
(
  ID                NUMBER(15)                  NOT NULL,
  MAP_ID            NUMBER(15)                  NOT NULL,
  CREATED_DATE      TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  TYPE_CODE         NUMBER(3)                   NOT NULL,
  TYPE_NAME         NVARCHAR2(50)               NOT NULL,
  STEP_DATATYPE     NVARCHAR2(50)               NOT NULL,
  STEP_DATASIZE     NUMBER(3)                   NOT NULL,
  SOURCE_COUNT      NUMBER(9)                   NOT NULL,
  SOURCE_COVERAGE   NUMBER(22,8)                NOT NULL,
  CONTENT_SIZE      NUMBER(9)                   NOT NULL,
  CONTENT_TYPE      NVARCHAR2(250)              NOT NULL,
  CONTENT_ENCODING  NVARCHAR2(250)              NOT NULL,
  CONTENT           BLOB                        NOT NULL
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


CREATE UNIQUE INDEX CALC.PROJECT_MAP_CONTENTS_PK ON CALC.PROJECT_MAP_CONTENTS
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


ALTER TABLE CALC.PROJECT_MAP_CONTENTS ADD (
  CONSTRAINT PROJECT_MAP_CONTENTS_PK
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
