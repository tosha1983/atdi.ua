DECLARE
  X NUMBER;
BEGIN
  SYS.DBMS_JOB.SUBMIT
  ( job       => X 
   ,what      => 'ICSC.CHECKACTIVITY;'
   ,next_date => to_date('20.06.2019 17:01:15','dd/mm/yyyy hh24:mi:ss')
   ,interval  => '(SYSDATE) + 1/24/60'
   ,no_parse  => FALSE
  );
  SYS.DBMS_OUTPUT.PUT_LINE('Job Number is: ' || to_char(x));
COMMIT;
END;
/