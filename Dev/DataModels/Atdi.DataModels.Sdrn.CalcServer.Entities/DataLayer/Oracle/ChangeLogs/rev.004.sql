CREATE TABLE CALC.CHECKPOINTS
(
  ID            NUMBER(15),
  CREATED_DATE  TIMESTAMP(7) WITH TIME ZONE     DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  RESULT_ID     NUMBER(15)                      NOT NULL,
  STATUS_CODE   NUMBER(3)                       NOT NULL,
  STATUS_NAME   NVARCHAR2(50)                   NOT NULL,
  STATUS_NOTE   NCLOB,
  NAME          NVARCHAR2(250)                  NOT NULL
)
TABLESPACE USERS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX CALC.CHECKPOINTS_PK ON CALC.CHECKPOINTS
(ID)
LOGGING
TABLESPACE USERS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;


ALTER TABLE CALC.CHECKPOINTS ADD (
  CONSTRAINT CHECKPOINTS_PK
 PRIMARY KEY
 (ID));



 CREATE TABLE CALC.CHECKPOINTS_DATA
(
  ID             NUMBER(15),
  CREATED_DATE   TIMESTAMP(7) WITH TIME ZONE    DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  CHECKPOINT_ID  NUMBER(15)                     NOT NULL,
  DATA_CONTEXT   NVARCHAR2(250)                 NOT NULL,
  DATA_JSON      NCLOB
)
TABLESPACE USERS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX CALC.CHECKPOINTS_DATA_PK ON CALC.CHECKPOINTS_DATA
(ID)
LOGGING
TABLESPACE USERS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;


ALTER TABLE CALC.CHECKPOINTS_DATA ADD (
  CONSTRAINT CHECKPOINTS_DATA_PK
 PRIMARY KEY
 (ID));


 CREATE TABLE CALC.COMMANDS
(
  ID                 NUMBER(15),
  CREATED_DATE       TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT' NOT NULL,
  TYPE_CODE          NUMBER(15)                 NOT NULL,
  TYPE_NAME          NVARCHAR2(250)             NOT NULL,
  STATUS_CODE        NUMBER(3)                  NOT NULL,
  STATUS_NAME        NVARCHAR2(50)              NOT NULL,
  STATUS_NOTE        NCLOB,
  CALLER_INSTANCE    NVARCHAR2(250)             NOT NULL,
  CALLER_COMMAND_ID  VARCHAR2(36 BYTE)          NOT NULL,
  START_TIME         TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT',
  FINISH_TIME        TIMESTAMP(7) WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP AT TIME ZONE 'GMT',
  ARGS_JSON          NCLOB,
  RESULT_JSON        NCLOB
)
TABLESPACE USERS
PCTUSED    0
PCTFREE    10
INITRANS   1
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE UNIQUE INDEX CALC.COMMANDS_PK ON CALC.COMMANDS
(ID)
LOGGING
TABLESPACE USERS
PCTFREE    10
INITRANS   2
MAXTRANS   255
STORAGE    (
            PCTINCREASE      0
            BUFFER_POOL      DEFAULT
           )
NOPARALLEL;


ALTER TABLE CALC.COMMANDS ADD (
  CONSTRAINT COMMANDS_PK
 PRIMARY KEY
 (ID));



ALTER TABLE CALC.CALCTASKS
  ADD (
 NOTE NVARCHAR2(250)
);



CREATE SEQUENCE CALC.COMMANDS_ID_SEQ
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 0
  NOCYCLE
  CACHE 2
  NOORDER;
  
  CREATE SEQUENCE CALC.CHECKPOINTS_ID_SEQ
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 0
  NOCYCLE
  CACHE 2
  NOORDER;
  
    
  CREATE SEQUENCE CALC.CHECKPOINTS_DATA_ID_SEQ
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 0
  NOCYCLE
  CACHE 2
  NOORDER;



  CREATE OR REPLACE FUNCTION CALC.GetID (inttable_name IN varchar2)
return number
is
n number(15);


BEGIN


if inttable_name ='CALCRESULTS' then
select CALC.CALCRESULTS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCRESULTS_POINTFIELDSTRENGTH' then
select CALC.CALCRESULTS_POINTFIELDSTRENGTH_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='CALCRESULTS_STATION_CALIBRATION' then
select CALC.CALCRESULTS_STATION_CALIBRATION_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCRESULTS_STATION_CALIBRATION_DRIVE_TEST' then
select CALC.CALCRESULTS_STATION_CALIBRATION_DRIVE_TEST_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCRESULTS_STATION_CALIBRATION_STA' then
select CALC.CALCRESULTS_STATION_CALIBRATION_STA_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='CALCTASKS' then
select CALC.CALCTASKS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCTASKS_COVERAGEPROFILES' then
select CALC.CALCTASKS_COVERAGEPROFILES_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CALCTASKS_POINTFIELDSTRENGTH_ARGS' then
select CALC.CALCTASKS_POINTFIELDSTRENGTH_ARGS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF' then
select CALC.CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCTASKS_STATION_CALIBRATION_ARGS' then
select CALC.CALCTASKS_STATION_CALIBRATION_ARGS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CALCTASKS_STATION_CALIBRATION_ARGS_DEF' then
select CALC.CALCTASKS_STATION_CALIBRATION_ARGS_DEF_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLIENT_CONTEXTS' then
select CALC.CLIENT_CONTEXTS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_ABSORPTION' then
select CALC.CLIENT_CONTEXTS_ABSORPTION_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLIENT_CONTEXTS_ADDITIONAL' then
select CALC.CLIENT_CONTEXTS_ADDITIONAL_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_ATMOSPHERIC' then
select CALC.CLIENT_CONTEXTS_ATMOSPHERIC_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_CLUTTER' then
select CALC.CLIENT_CONTEXTS_CLUTTER_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLIENT_CONTEXTS_DIFFRACTION' then
select CALC.CLIENT_CONTEXTS_DIFFRACTION_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLIENT_CONTEXTS_DIFFRACTION' then
select CALC.CLIENT_CONTEXTS_DIFFRACTION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_DUCTING' then
select CALC.CLIENT_CONTEXTS_DUCTING_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLIENT_CONTEXTS_GLOBALPARAMS' then
select CALC.CLIENT_CONTEXTS_GLOBALPARAMS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_MAINBLOCK' then
select CALC.CLIENT_CONTEXTS_MAINBLOCK_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_REFLECTION' then
select CALC.CLIENT_CONTEXTS_REFLECTION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_SUBPATHDIFFRACTION' then
select CALC.CLIENT_CONTEXTS_SUBPATHDIFFRACTION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLIENT_CONTEXTS_TROPO' then
select CALC.CLIENT_CONTEXTS_TROPO_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLUTTERS_DESCS' then
select CALC.CLUTTERS_DESCS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLUTTERS_DESCS_CLUTTERS' then
select CALC.CLUTTERS_DESCS_CLUTTERS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLUTTERS_DESCS_FREQS' then
select CALC.CLUTTERS_DESCS_FREQS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CLUTTERS_DESCS_FREQS_CLUTTERS' then
select CALC.CLUTTERS_DESCS_FREQS_CLUTTERS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CONTEXT_PLANNEDCALCTASK' then
select CALC.CONTEXT_PLANNEDCALCTASK_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CONTEXT_STATION_COORDINATES' then
select CALC.CONTEXT_STATION_COORDINATES_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CONTEXT_STATION_PATTERNS' then
select CALC.CONTEXT_STATION_PATTERNS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CONTEXT_STATIONS' then
select CALC.CONTEXT_STATIONS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CONTEXT_STATIONS_ANTENNA' then
select CALC.CONTEXT_STATIONS_ANTENNA_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CONTEXT_STATIONS_RECEIVER' then
select CALC.CONTEXT_STATIONS_RECEIVER_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CONTEXT_STATIONS_SITE' then
select CALC.CONTEXT_STATIONS_SITE_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CONTEXT_STATIONS_TRANSMITTER' then
select CALC.CONTEXT_STATIONS_TRANSMITTER_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='PROJECT_MAP_CONTENT_SOURCES' then
select CALC.PROJECT_MAP_CONTENT_SOURCES_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='PROJECT_MAP_CONTENTS' then
select CALC.PROJECT_MAP_CONTENTS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='PROJECT_MAPS' then
select CALC.PROJECT_MAPS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='PROJECTS' then
select CALC.PROJECTS_ID_SEQ.nextval
into n
from dual;
end if;




if inttable_name ='CALCRESULT_EVENTS' then
select CALC.CALCRESULT_EVENTS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS' then
select CALC.CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS_DETAIL' then
select CALC.CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS_DETAIL_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='REF_SPECTRUM_BY_DRIVE_TESTS_ARGS' then
select CALC.REF_SPECTRUM_BY_DRIVE_TESTS_ARGS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='REF_SPECTRUM_BY_DRIVE_TESTS_ARGS_DEF' then
select CALC.REF_SPECTRUM_BY_DRIVE_TESTS_ARGS_DEF_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='COMMANDS' then
select CALC.COMMANDS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CHECKPOINTS' then
select CALC.CHECKPOINTS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CHECKPOINTS_DATA' then
select CALC.CHECKPOINTS_DATA_ID_SEQ.nextval
into n
from dual;
end if;


return(n);
end;
/
