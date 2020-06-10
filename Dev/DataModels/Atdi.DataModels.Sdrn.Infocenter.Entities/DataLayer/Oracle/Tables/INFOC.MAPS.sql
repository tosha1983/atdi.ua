CREATE TABLE INFOC.MAPS
(
  ID               NUMBER(15)                   NOT NULL,
  STATUS_CODE      NUMBER(3)                    NOT NULL,
  STATUS_NAME      NVARCHAR2(50)                NOT NULL,
  STATUS_NOTE      NCLOB,
  CREATED_DATE     TIMESTAMP(7) WITH TIME ZONE  DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  TYPE_CODE        NUMBER(3)                    NOT NULL,
  TYPE_NAME        NVARCHAR2(50)                NOT NULL,
  PROJECTION       NVARCHAR2(50)                NOT NULL,
  STEP_UNIT        NVARCHAR2(10)                NOT NULL,
  STEP_DATATYPE    NVARCHAR2(50)                NOT NULL,
  STEP_DATASIZE    NUMBER(3)                    NOT NULL,
  AXIS_X_NUMBER    NUMBER(9)                    NOT NULL,
  AXIS_X_STEP      NUMBER(9)                    NOT NULL,
  AXIS_Y_NUMBER    NUMBER(9)                    NOT NULL,
  AXIS_Y_STEP      NUMBER(9)                    NOT NULL,
  CRD_UPL_X        NUMBER(9)                    NOT NULL,
  CRD_UPL_Y        NUMBER(9)                    NOT NULL,
  CRD_UPR_X        NUMBER(9)                    NOT NULL,
  CRD_UPR_Y        NUMBER(9)                    NOT NULL,
  CRD_LWL_X        NUMBER(9)                    NOT NULL,
  CRD_LWL_Y        NUMBER(9)                    NOT NULL,
  CRD_LWR_X        NUMBER(9)                    NOT NULL,
  CRD_LWR_Y        NUMBER(9)                    NOT NULL,
  CONTENT_SIZE     NUMBER(9)                    NOT NULL,
  CONTENT_SOURCE   NVARCHAR2(50)                NOT NULL,
  FILE_NAME        NVARCHAR2(250),
  FILE_SIZE        NUMBER(9),
  MAP_NAME         NVARCHAR2(450)               NOT NULL,
  MAP_NOTE         NCLOB,
  SECTORS_COUNT    NUMBER(9),
  SECTORS_X_COUNT  NUMBER(9),
  SECTORS_Y_COUNT  NUMBER(9)
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
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX INFOC.MAPS_PK ON INFOC.MAPS
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


ALTER TABLE INFOC.MAPS ADD (
  CONSTRAINT MAPS_PK
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
