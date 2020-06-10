CREATE TABLE INFOC.MAP_SECTORS
(
  ID                NUMBER(15)                  NOT NULL,
  MAP_ID            NUMBER(15)                  NOT NULL,
  SECTOR_NAME       NVARCHAR2(250)              NOT NULL,
  CREATED_DATE      TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  AXIS_X_INDEX      NUMBER(9)                   NOT NULL,
  AXIS_Y_INDEX      NUMBER(9)                   NOT NULL,
  AXIS_X_NUMBER     NUMBER(9)                   NOT NULL,
  AXIS_Y_NUMBER     NUMBER(9)                   NOT NULL,
  CRD_UPL_X         NUMBER(9)                   NOT NULL,
  CRD_UPL_Y         NUMBER(9)                   NOT NULL,
  CRD_UPR_X         NUMBER(9)                   NOT NULL,
  CRD_UPR_Y         NUMBER(9)                   NOT NULL,
  CRD_LWL_X         NUMBER(9)                   NOT NULL,
  CRD_LWL_Y         NUMBER(9)                   NOT NULL,
  CRD_LWR_X         NUMBER(9)                   NOT NULL,
  CRD_LWR_Y         NUMBER(9)                   NOT NULL,
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
LOB (CONTENT) STORE AS SECUREFILE 
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


CREATE UNIQUE INDEX INFOC.MAP_SECTORS_PK ON INFOC.MAP_SECTORS
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


ALTER TABLE INFOC.MAP_SECTORS ADD (
  CONSTRAINT MAP_SECTORS_PK
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
