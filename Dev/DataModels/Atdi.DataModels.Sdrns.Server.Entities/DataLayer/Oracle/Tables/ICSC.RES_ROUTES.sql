CREATE TABLE ICSC.RES_ROUTES
(
  ID               NUMBER(15)                   NOT NULL,
  ROUTE_ID         NVARCHAR2(250),
  AGL              NUMBER(22,8),
  ASL              NUMBER(22,8),
  FINISH_TIME      DATE                         NOT NULL,
  LAT              NUMBER(22,8),
  LON              NUMBER(22,8),
  POINT_STAY_TYPE  NVARCHAR2(150),
  START_TIME       DATE,
  RES_MEAS_ID      NUMBER(15)                   NOT NULL
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


CREATE UNIQUE INDEX ICSC.XBS_RESROUTES_PK ON ICSC.RES_ROUTES
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


ALTER TABLE ICSC.RES_ROUTES ADD (
  CONSTRAINT XBS_RESROUTES_PK
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

ALTER TABLE ICSC.RES_ROUTES ADD (
  CONSTRAINT FK_XBS_RESROUTES_XBS_RESMEAS 
 FOREIGN KEY (RES_MEAS_ID) 
 REFERENCES ICSC.RES_MEAS (ID));
