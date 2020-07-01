﻿CREATE TABLE CALC.CALCTASKS_GN06_ARGS_DEF
(
  ID                               NUMBER(15)   NOT NULL,
  AZIMUTH_STEP_DEG                 NUMBER(22,8),
  ADDITIONAL_CONTOURS_BY_DISTANCE  NUMBER(1),
  INFOC_MEAS_RESULTS               BLOB,
  CONTURE_BY_FIELD_STRENGTH        NUMBER(1),
  FIELD_STRENGTH                   BLOB,
  SUBSCRIBERS_HEIGHT               NUMBER(9),
  PERCENTAGE_TIME                  NUMBER(30,10),
  USE_EFFECTIVE_HEIGHT             NUMBER(1),
  CALCULATION_TYPE_CODE            NUMBER(3)    NOT NULL,
  CALCULATION_TYPE_NAME            VARCHAR2(50 BYTE),
  BROADCASTING_EXTEND              CLOB
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


CREATE UNIQUE INDEX CALC.PK_CALCTASKS_GN06_ARGS_DEF ON CALC.CALCTASKS_GN06_ARGS_DEF
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

