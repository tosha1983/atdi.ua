CREATE TABLE CALC.PROJECT_MAPS
(
  ID                   NUMBER(15)               NOT NULL,
  PROJECT_ID           NUMBER(15)               NOT NULL,
  MAP_NAME             NVARCHAR2(250)           NOT NULL,
  MAP_NOTE             NCLOB,
  OWNER_INSTANCE       NVARCHAR2(250)           NOT NULL,
  OWNER_MAP_ID         VARCHAR2(36 BYTE)        NOT NULL,
  CREATED_DATE         TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  STATUS_CODE          NUMBER(3)                NOT NULL,
  STATUS_NAME          NVARCHAR2(50)            NOT NULL,
  STATUS_NOTE          NCLOB,
  STEP_UNIT            NVARCHAR2(10)            NOT NULL,
  OWNER_AXIS_X_NUMBER  NUMBER(9)                NOT NULL,
  OWNER_AXIS_X_STEP    NUMBER(9)                NOT NULL,
  OWNER_AXIS_Y_NUMBER  NUMBER(9)                NOT NULL,
  OWNER_AXIS_Y_STEP    NUMBER(9)                NOT NULL,
  OWNER_CRD_UPL_X      NUMBER(9)                NOT NULL,
  OWNER_CRD_UPL_Y      NUMBER(9)                NOT NULL,
  AXIS_X_NUMBER        NUMBER(9),
  AXIS_X_STEP          NUMBER(9),
  AXIS_Y_NUMBER        NUMBER(9),
  AXIS_Y_STEP          NUMBER(9),
  CRD_UPL_X            NUMBER(9),
  CRD_UPL_Y            NUMBER(9),
  CRD_LWR_X            NUMBER(9),
  CRD_LWR_Y            NUMBER(9),
  SOURCE_TYPE_NAME     NVARCHAR2(50),
  SOURCE_TYPE_CODE     NUMBER(3)
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
LOB (MAP_NOTE) STORE AS SECUREFILE 
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


CREATE UNIQUE INDEX CALC.PROJECT_MAPS_PK ON CALC.PROJECT_MAPS
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


ALTER TABLE CALC.PROJECT_MAPS ADD (
  CONSTRAINT PROJECT_MAPS_PK
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
