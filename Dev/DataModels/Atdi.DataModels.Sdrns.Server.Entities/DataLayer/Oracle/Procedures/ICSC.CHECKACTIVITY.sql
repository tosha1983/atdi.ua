CREATE OR REPLACE procedure ICSC.CHECKACTIVITY as
begin
update ICSC.sensor A set A.status = 'F' WHERE  (TO_CHAR(SYSDATE - A.lastactivity)*24*60)>5;
end;
/