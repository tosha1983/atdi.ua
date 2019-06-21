CREATE OR REPLACE FUNCTION ICSR.GetID (inttable_name IN varchar2)
return number
is
n number(15);


BEGIN
if inttable_name ='AMQP_MESSAGES' then
select ICSR.AMQP_MESSAGES_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='ANTENNA_PATTERN' then
select ICSR.ANTENNA_PATTERN_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='ENTITY' then
select ICSR.ENTITY_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='ENTITY_PART' then
select ICSR.ENTITY_PART_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_MEAS_STATION' then
select ICSR.LINK_MEAS_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_RES_SENSOR' then
select ICSR.LINK_RES_SENSOR_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_SECTOR_FREQ' then
select ICSR.LINK_SECTOR_FREQ_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='LINK_SECTOR_MASK' then
select ICSR.LINK_SECTOR_MASK_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_DT_PARAM' then
select ICSR.MEAS_DT_PARAM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_FREQ' then
select ICSR.MEAS_FREQ_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_FREQ_PARAM' then
select ICSR.MEAS_FREQ_PARAM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_LOCATION_PARAM' then
select ICSR.MEAS_LOCATION_PARAM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_OTHER' then
select ICSR.MEAS_OTHER_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_STATION' then
select ICSR.MEAS_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_SUB_TASK' then
select ICSR.MEAS_SUB_TASK_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_SUB_TASK_STATION' then
select ICSR.MEAS_SUB_TASK_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_TASK' then
select ICSR.MEAS_TASK_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='OWNER_DATA' then
select ICSR.OWNER_DATA_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_LEVELS' then
select ICSR.RES_LEVELS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_LEV_MEAS_ONLINE' then
select ICSR.RES_LEV_MEAS_ONLINE_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_LOCATION_SENSOR_MEAS' then
select ICSR.RES_LOCATION_SENSOR_MEAS_I_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_MEAS_STATION' then
select ICSR.RES_MEAS_STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_MEAS' then
select ICSR.RES_MEAS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_STGENERAL' then
select ICSR.RES_STGENERAL_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_STLEVEL_CAR' then
select ICSR.RES_STLEVEL_CAR_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='RES_STMASKELM' then
select ICSR.RES_STMASKELM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_SYS_INFO' then
select ICSR.RES_SYS_INFO_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_SYS_INFO_BLOCKS' then
select ICSR.RES_SYS_INFO_BLOCKS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='RES_ROUTES' then
select ICSR.RES_ROUTES_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SECTOR' then
select ICSR.SECTOR_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SECTOR_FREQ' then
select ICSR.SECTOR_FREQ_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SECTOR_MASK_ELEM' then
select ICSR.SECTOR_MASK_ELEM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR' then
select ICSR.SENSOR_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_ANTENNA' then
select ICSR.SENSOR_ANTENNA_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_EQUIP' then
select ICSR.SENSOR_EQUIP_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_EQUIP_SENSITIVITES' then
select ICSR.SENSOR_EQUIP_SENSITIVITES__SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_LOCATION' then
select ICSR.SENSOR_LOCATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='SENSOR_POLIG' then
select ICSR.SENSOR_POLIG_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='STATION' then
select ICSR.STATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='STATION_SITE' then
select ICSR.STATION_SITE_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='LOGS' then
select ICSR.LOGS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='BEARING' then
select ICSR.BEARING_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='EMITTING' then
select ICSR.EMITTING_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='REFERENCE_LEVELS' then
select ICSR.REFERENCE_LEVELS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='REFERENCE_SIGNAL' then
select ICSR.REFERENCE_SIGNAL_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='SPECTRUM' then
select ICSR.SPECTRUM_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='WORK_TIME' then
select ICSR.WORK_TIME_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='REFERENCE_SITUATION' then
select ICSR.REFERENCE_SITUATION_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MEAS_TASK_SIGNAL' then
select ICSR.MEAS_TASK_SIGNAL_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='FREQ_SAMPLE' then
select ICSR.FREQ_SAMPLE_ID_SEQ.nextval
into n
from dual;
end if;



return(n);
end;
/
