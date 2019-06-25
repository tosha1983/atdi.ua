﻿CREATE OR REPLACE FUNCTION ICSC.GetID (inttable_name IN varchar2 )
return number
is
n number(15);


BEGIN


if inttable_name ='AMQP_EVENTS' then
select ICSC.AMQP_EVENTS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='AMQP_MESSAGES' then
select ICSC.AMQP_MESSAGES_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='ANTENNA_PATTERN' then
select ICSC.ANTENNA_PATTERN_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='ENTITY' then
select ICSC.ENTITY_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='ENTITY_PART' then
select ICSC.ENTITY_PART_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_MEAS_STATION' then
select ICSC.LINK_MEAS_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_RES_SENSOR' then
select ICSC.LINK_RES_SENSOR_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_SECTOR_FREQ' then
select ICSC.LINK_SECTOR_FREQ_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_SECTOR_MASK' then
select ICSC.LINK_SECTOR_MASK_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_DT_PARAM' then
select ICSC.MEAS_DT_PARAM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_FREQ' then
select ICSC.MEAS_FREQ_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_FREQ_PARAM' then
select ICSC.MEAS_FREQ_PARAM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_LOCATION_PARAM' then
select ICSC.MEAS_LOCATION_PARAM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_OTHER' then
select ICSC.MEAS_OTHER_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_STATION' then
select ICSC.MEAS_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_SUB_TASK' then
select ICSC.MEAS_SUB_TASK_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_SUB_TASK_STATION' then
select ICSC.MEAS_SUB_TASK_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_TASK' then
select ICSC.MEAS_TASK_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='OWNER_DATA' then
select ICSC.OWNER_DATA_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_LEVELS' then
select ICSC.RES_LEVELS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_LEV_MEAS_ONLINE' then
select ICSC.RES_LEV_MEAS_ONLINE_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_LOCATION_SENSOR_MEAS' then
select ICSC.RES_LOCATION_SENSOR_MEAS_I_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_MEAS_STATION' then
select ICSC.RES_MEAS_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_MEAS' then
select ICSC.RES_MEAS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_STGENERAL' then
select ICSC.RES_STGENERAL_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_STLEVEL_CAR' then
select ICSC.RES_STLEVEL_CAR_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='RES_STMASKELM' then
select ICSC.RES_STMASKELM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_SYS_INFO' then
select ICSC.RES_SYS_INFO_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_SYS_INFO_BLOCKS' then
select ICSC.RES_SYS_INFO_BLOCKS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_ROUTES' then
select ICSC.RES_ROUTES_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SECTOR' then
select ICSC.SECTOR_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SECTOR_FREQ' then
select ICSC.SECTOR_FREQ_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SECTOR_MASK_ELEM' then
select ICSC.SECTOR_MASK_ELEM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR' then
select ICSC.SENSOR_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_ANTENNA' then
select ICSC.SENSOR_ANTENNA_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_EQUIP' then
select ICSC.SENSOR_EQUIP_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_EQUIP_SENSITIVITES' then
select ICSC.SENSOR_EQUIP_SENSITIVITES__SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_LOCATION' then
select ICSC.SENSOR_LOCATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_POLIG' then
select ICSC.SENSOR_POLIG_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='STATION' then
select ICSC.STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='STATION_SITE' then
select ICSC.STATION_SITE_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='LOGS' then
select ICSC.LOGS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='BEARING' then
select ICSC.BEARING_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='EMITTING' then
select ICSC.EMITTING_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='REFERENCE_LEVELS' then
select ICSC.REFERENCE_LEVELS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='REFERENCE_SIGNAL' then
select ICSC.REFERENCE_SIGNAL_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='SPECTRUM' then
select ICSC.SPECTRUM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='WORK_TIME' then
select ICSC.WORK_TIME_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='REFERENCE_SITUATION' then
select ICSC.REFERENCE_SITUATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_TASK_SIGNAL' then
select ICSC.MEAS_TASK_SIGNAL_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='FREQ_SAMPLE' then
select ICSC.FREQ_SAMPLE_ID_SEQ.nextval
into n
from dual;
end if;



return(n);
end;
/