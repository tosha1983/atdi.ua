CREATE OR REPLACE FUNCTION INFOC.GetID (inttable_name IN varchar2)
return number
is
n number(15);


BEGIN


if inttable_name ='CLUTTERS_DESCS' then
select INFOC.CLUTTERS_DESCS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLUTTERS_DESCS_CLUTTERS' then
select INFOC.CLUTTERS_DESCS_CLUTTERS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLUTTERS_DESCS_FREQS' then
select INFOC.CLUTTERS_DESCS_FREQS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='CLUTTERS_DESCS_FREQS_CLUTTERS' then
select INFOC.CLUTTERS_DESCS_FREQS_CLUTTERS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='INTEGRATION_LOG' then
select INFOC.INTEGRATION_LOG_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='INTEGRATION_OBJECTS' then
select INFOC.INTEGRATION_OBJECTS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MAP_SECTORS' then
select INFOC.MAP_SECTORS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='MAPS' then
select INFOC.MAPS_ID_SEQ.nextval
into n
from dual;
end if;

return(n);
end;
