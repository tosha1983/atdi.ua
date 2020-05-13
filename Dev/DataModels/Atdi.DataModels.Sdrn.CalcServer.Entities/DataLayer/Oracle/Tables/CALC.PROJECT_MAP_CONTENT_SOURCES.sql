CREATE TABLE CALC.PROJECT_MAP_CONTENT_SOURCES
(
  ID              NUMBER(15)                    NOT NULL,
  CONTEXT_ID      NUMBER(15)                    NOT NULL,
  INFOC_MAP_ID    NUMBER(15)                    NOT NULL,
  INFOC_MAP_NAME  NVARCHAR2(450)                NOT NULL,
  COVERAGE        NUMBER(22,8)                  NOT NULL,
  CRD_UPL_X       NUMBER(9)                     NOT NULL,
  CRD_UPL_Y       NUMBER(9)                     NOT NULL,
  CRD_LWR_X       NUMBER(9)                     NOT NULL,
  CRD_LWR_Y       NUMBER(9)                     NOT NULL,
  PRIORITY_CODE   NUMBER(3)                     NOT NULL,
  PRIORITY_NAME   NVARCHAR2(50)                 NOT NULL
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


CREATE UNIQUE INDEX CALC.PROJECT_MAP_CONTENT_SOURCES_PK ON CALC.PROJECT_MAP_CONTENT_SOURCES
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


ALTER TABLE CALC.PROJECT_MAP_CONTENT_SOURCES ADD (
  CONSTRAINT PROJECT_MAP_CONTENT_SOURCES_PK
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
