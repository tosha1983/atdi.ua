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


if inttable_name ='CALCRESULTS_GN06' then
select CALC.CALCRESULTS_GN06_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CALCTASKS_GN06_ARGS_DEF' then
select CALC.CALCTASKS_GN06_ARGS_DEF_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='CALCTASKS_GN06_ARGS' then
select CALC.CALCTASKS_GN06_ARGS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='GN06_AFFECTED_ADM_RESULT' then
select CALC.GN06_AFFECTED_ADM_RESULT_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='GN06_CONTOURS_RESULT' then
select CALC.GN06_CONTOURS_RESULT_ID_SEQ.nextval
into n
from dual;
end if;


return(n);
end;
/
