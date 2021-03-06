﻿CREATE TABLE STATIONS.GLOBAL_IDENTITIES
(
  REGION_CODE   NVARCHAR2(50)                   NOT NULL,
  LICENSE_GSID  NVARCHAR2(50)                   NOT NULL,
  STANDARD      NVARCHAR2(50)                   NOT NULL,
  CREATED_DATE  TIMESTAMP(7) WITH TIME ZONE     NOT NULL,
  REAL_GSID     NVARCHAR2(50)
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


CREATE UNIQUE INDEX STATIONS.PK_GLOBAL_IDENTITIES_ ON STATIONS.GLOBAL_IDENTITIES
(REGION_CODE, LICENSE_GSID, STANDARD)
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


ALTER TABLE STATIONS.GLOBAL_IDENTITIES ADD (
  CONSTRAINT PK_GLOBAL_IDENTITIES_
 PRIMARY KEY
 (REGION_CODE, LICENSE_GSID, STANDARD)
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
