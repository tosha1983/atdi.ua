CREATE OR REPLACE FUNCTION STATIONS.GetID (inttable_name IN varchar2)
return number
is
n number(15);


BEGIN


if inttable_name ='GLOBAL_IDENTITIES' then
select STATIONS.GLOBAL_IDENTITIES_ID_SEQ.nextval
into n
from dual;
end if;




return(n);
end;
/