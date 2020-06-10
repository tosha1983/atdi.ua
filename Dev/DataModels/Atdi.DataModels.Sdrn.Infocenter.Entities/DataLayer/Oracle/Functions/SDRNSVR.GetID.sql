CREATE OR REPLACE FUNCTION SDRNSVR.GetID (inttable_name IN varchar2)
return number
is
n number(15);


BEGIN


if inttable_name ='DRIVE_ROUTES' then
select SDRNSVR.DRIVE_ROUTES_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='DRIVE_TESTS' then
select SDRNSVR.DRIVE_TESTS_ID_SEQ.nextval
into n
from dual;
end if;

if inttable_name ='DRIVE_TESTS_POINTS' then
select SDRNSVR.DRIVE_TESTS_POINTS_ID_SEQ.nextval
into n
from dual;
end if;



if inttable_name ='SM_MEAS_RESULTS' then
select SDRNSVR.SM_MEAS_RESULTS_ID_SEQ.nextval
into n
from dual;
end if;


if inttable_name ='SM_MEAS_RESULTS_STATS' then
select SDRNSVR.SM_MEAS_RESULTS_STATS_ID_SEQ.nextval
into n
from dual;
end if;

return(n);
end;
/