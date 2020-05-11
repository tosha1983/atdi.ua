CREATE TABLE CALC.CONTEXT_STATION_COORDINATES
(
  STATION_ID  NUMBER(15)                        NOT NULL,
  ATDI_X      NUMBER(9)                         NOT NULL,
  ATDI_Y      NUMBER(9)                         NOT NULL,
  EPSG_X      NUMBER(22,8)                      NOT NULL,
  EPSG_Y      NUMBER(22,8)                      NOT NULL
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


CREATE UNIQUE INDEX CALC.CONTEXT_STATION_COORDINATES_PK ON CALC.CONTEXT_STATION_COORDINATES
(STATION_ID)
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


ALTER TABLE CALC.CONTEXT_STATION_COORDINATES ADD (
  CONSTRAINT CONTEXT_STATION_COORDINATES_PK
 PRIMARY KEY
 (STATION_ID)
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
