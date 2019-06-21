
CREATE TABLE ICSC.RES_MEAS
(
  ID                        NUMBER(15)          NOT NULL,
  MEAS_TASK_ID              NVARCHAR2(150),
  SUB_MEAS_TASK_ID          NUMBER(15),
  SUB_MEAS_TASK_STATION_ID  NUMBER(15),
  SENSOR_ID                 NUMBER(15),
  ANTVAL                    NUMBER(22,8),
  TIME_MEAS                 DATE,
  DATA_RANK                 NUMBER(9),
  N                         NUMBER(9),
  STATUS                    NVARCHAR2(50),
  TYPE_MEASUREMENTS         NVARCHAR2(50),
  MEAS_SDR_RESULT_SID       NVARCHAR2(450),
  SYNCHRONIZED              NUMBER(1)           DEFAULT 0,
  START_TIME                DATE,
  STOP_TIME                 DATE,
  SCANS_NUMBER              NUMBER(10)
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


CREATE UNIQUE INDEX ICSC.XBS_RESSTATIONMEAS_PK ON ICSC.RES_MEAS
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


ALTER TABLE ICSC.RES_MEAS ADD (
  CONSTRAINT XBS_RESSTATIONMEAS_PK
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

ALTER TABLE ICSC.RES_MEAS ADD (
  CONSTRAINT FK_XBS_RESMEAS_XBS_SENSOR 
 FOREIGN KEY (SENSOR_ID) 
 REFERENCES ICSC.SENSOR (ID));
