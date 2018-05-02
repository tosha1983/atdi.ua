/* 17-02-2007 */

create table DIG_CONTOUR (ID DM_IDENTY_PK, ADM_ID DM_IDENTY_FK, CTRY DM_COUNTRYCODE, CONTOUR_ID DM_INTEGER not null, NB_TEST_PTS DM_INTEGER not null);
alter table DIG_CONTOUR

add constraint PK_DIG_CONTOUR
primary key (ID);
alter table DIG_CONTOUR

add constraint FK_DIG_CONTOUR
foreign key (ADM_ID)
references COUNTRY(ID)
on update CASCADE;



/* 18-02-2007 */


drop table DIG_SUBAREAPOINT;

CREATE TABLE DIG_SUBAREAPOINT (
    CONTOUR_ID   DM_SMALLINT NOT NULL,
    POINT_NO  DM_SMALLINT NOT NULL,
    LAT       DM_DOUBLEPRECISION NOT NULL,
    LON       DM_DOUBLEPRECISION NOT NULL
);


alter table DIG_SUBAREAPOINT
add constraint PK_DIG_SUBAREAPOINT
primary key (CONTOUR_ID,POINT_NO);


alter table DIG_SUBAREAPOINT
add constraint FK_DIG_SUBAREAPOINT01
foreign key (CONTOUR_ID)
references DIG_CONTOUR(ID)
on delete CASCADE
on update CASCADE;

create domain dm_notice_type char(3) not null check (value in ('GS2', 'GT2', 'GA1'));

drop table DIG_ALLOTMENT;

CREATE TABLE DIG_ALLOTMENT (
    ID             DM_IDENTY_PK,
    ADM_ID         DM_INTEGER,
    NOTICE_TYPE    DM_NOTICE_TYPE NOT NULL,
    IS_PUB_REQ     VARCHAR(5),
    ADM_REF_ID     DM_RRC06_ID,
    PLAN_ENTRY     DM_INTEGER NOT NULL,
    SFN_ID_FK      DM_INTEGER,
    FREQ_ASSIGN    DM_MHERZ NOT NULL,
    OFFSET         DM_INTEGER,
    D_EXPIRY       DATE,
    ALLOT_NAME     DM_VARCHAR32 NOT NULL,
    CTRY           DM_COUNTRYCODE NOT NULL,
    GEO_AREA       DM_COUNTRYCODE,
    NB_SUB_AREAS   DM_SMALLINT NOT NULL,
    REF_PLAN_CFG   CHAR(4) NOT NULL,
    TYP_REF_NETWK  CHAR(3),
    SPECT_MASK     DM_SPECTRUM_MASK,
    POLAR          DM_POLARIZATION NOT NULL,
    BLOCKDAB_ID    DM_INTEGER,
    CHANNEL_ID     DM_INTEGER,
    REMARKS1       DM_VARCHAR512,
    REMARKS2       DM_VARCHAR512,
    REMARKS3       DM_VARCHAR512
);



ALTER TABLE DIG_ALLOTMENT ADD CONSTRAINT PK_DIG_ALLOTMENT PRIMARY KEY (ID);


CREATE TABLE DIG_ALLOT_CNTR_LNK (
    ALLOT_ID  DM_IDENTY_FK,
    CNTR_ID   DM_IDENTY_FK
);


ALTER TABLE DIG_ALLOT_CNTR_LNK ADD CONSTRAINT FK_DIG_ALLOT_CNTR_LNK1 FOREIGN KEY (ALLOT_ID) REFERENCES DIG_ALLOTMENT (ID) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE DIG_ALLOT_CNTR_LNK ADD CONSTRAINT FK_DIG_ALLOT_CNTR_LNK2 FOREIGN KEY (CNTR_ID) REFERENCES DIG_CONTOUR (ID) ON DELETE CASCADE ON UPDATE CASCADE;


ALTER TABLE DIG_ALLOTMENT ADD CONSTRAINT FK_DIG_ALLOTMENT10 FOREIGN KEY (BLOCKDAB_ID) REFERENCES BLOCKDAB (ID) ON UPDATE CASCADE;
ALTER TABLE DIG_ALLOTMENT ADD CONSTRAINT FK_DIG_ALLOTMENT11 FOREIGN KEY (CHANNEL_ID) REFERENCES CHANNELS (ID) ON UPDATE CASCADE;
ALTER TABLE DIG_ALLOTMENT ADD CONSTRAINT FK_DIG_ALLOTMENT12 FOREIGN KEY (SFN_ID_FK) REFERENCES SYNHROFREQNET (ID) ON DELETE SET NULL ON UPDATE CASCADE;
ALTER TABLE DIG_ALLOTMENT ADD CONSTRAINT FK_DIG_ALLOTMENT13 FOREIGN KEY (ADM_ID) REFERENCES COUNTRY (ID) ON UPDATE CASCADE;


/* 19-02-2007 */

ALTER TABLE DIG_ALLOTMENT
ADD DB_SECTION_ID DM_IDENTY_FK
NOT NULL;

alter table DIG_ALLOTMENT
add constraint FK_DIG_ALLOTMENT14
foreign key (DB_SECTION_ID)
references DATABASESECTION(ID);

ALTER TABLE DIG_CONTOUR
ADD DB_SECTION_ID DM_IDENTY_FK
NOT NULL;

alter table DIG_CONTOUR
add constraint FK_DIG_CONTOUR1
foreign key (DB_SECTION_ID)
references DATABASESECTION(ID);
	

/* 28-02-2007 */

CREATE DOMAIN DM_ADM_REF_ID20 AS
VARCHAR(20);
CREATE DOMAIN DM_ADM_REF_ID30 AS
VARCHAR(30);
CREATE DOMAIN DM_ASSGN_CODE AS
CHAR(1) default 'S'
CHECK (value in ('L', 'C', 'S'));
CREATE DOMAIN DM_CALLSIGN AS
VARCHAR(10);

alter table TRANSMITTERS
add adm_id                      DM_INTEGER,
add is_pub_req                  DM_BOOLEAN,
add adm_ref_id                  DM_ADM_REF_ID20,
add plan_entry                  DM_INTEGER,
add assgn_code                  DM_ASSGN_CODE,
add associated_adm_allot_id     DM_ADM_REF_ID20,
add associated_allot_sfn_id     DM_ADM_REF_ID30,
add call_sign                   DM_CALLSIGN,
add d_expiry                    DM_DATE,
add op_agcy                     DM_VARCHAR64,
add addr_code                   DM_VARCHAR64,
add op_hh_fr                    DM_TIME,
add op_hh_to                    DM_TIME,
add is_resub                    DM_BOOLEAN,
add remark_conds_met            DM_BOOLEAN,
add signed_commitment           DM_BOOLEAN
;


/* 01-03-2007 */

ALTER TABLE TRANSMITTERS
ADD EFFECTANTENNAGAINSVER DM_BLOB;
ALTER TABLE TRANSMITTERS
ADD COORD DM_VARCHAR64
COLLATE WIN1251 ;


/* 02-03-2007 */

alter table TRANSMITTERS
add spect_mask  DM_SPECTRUM_MASK;

update TRANSMITTERS t
set
t.IS_PUB_REQ = 0,
t.ASSGN_CODE = 'S',
t.IS_RESUB = 0,
t.REMARK_CONDS_MET = 0,
t.SIGNED_COMMITMENT = 0;
commit;

ALTER DOMAIN DM_ASSGN_CODE
SET DEFAULT 'S';


/* 04-03-2007 */

ALTER TABLE TRANSMITTERS DROP CONSTRAINT FK_TRANSMITTERS;
alter table TRANSMITTERS
add constraint FK_TRANSMITTERS
foreign key (STATUS)
references DATABASESECTION(ID);
alter table "TRANSMITTERS" drop "DATABASESECTION_ID";

alter table "TRANSMITTERS" drop "RRC06_ID";

ALTER TABLE SYNHROFREQNET
    ALTER SYNHRONETID TYPE DM_VARCHAR30;


/* 21-03-2007 */
set term ^;

create trigger DIG_ALLOTMENT_AI FOR DIG_ALLOTMENT
active AFTER INSERT position 0
as
begin
  update TRANSMITTERS set STATUS = new.DB_SECTION_ID where ID = new.ID;
end^

create trigger DIG_ALLOTMENT_AU FOR DIG_ALLOTMENT
active AFTER UPDATE position 0
as
begin
    if (new.DB_SECTION_ID <> old.DB_SECTION_ID) then
        update TRANSMITTERS set STATUS = new.DB_SECTION_ID where ID = new.ID;
end^

set term ;^

/* 09-05-2007 */
/* update to procedure SP_TX_SELECTION3: */
/*===========================================================*/

	/* extract allotments  */

    OUT_AREA = '';

    for select a.id, a.DB_SECTION_ID, min(UDF_DISTANCE(p.LAT, p.LON, :VAR_TX_LAT, :VAR_TX_LON))
    from DIG_SUBAREAPOINT p
        left join DIG_ALLOT_CNTR_LNK l on (p.CONTOUR_ID = l.CNTR_ID)
        left join DIG_ALLOTMENT a on (l.ALLOT_ID = a.id)
    where
        (a.FREQ_ASSIGN = :VAR_CARRIER_WANT
          or
          (a.NOTICE_TYPE = 'GS2' or a.NOTICE_TYPE = 'DS2')
            and (
              a.FREQ_ASSIGN > :VAR_CARRIER_WANT and (a.FREQ_ASSIGN - :VAR_CARRIER_WANT) < (:VAR_BANDWIDTH_WANT / 2 + 0.9)
              or (a.FREQ_ASSIGN < :VAR_CARRIER_WANT and (:VAR_CARRIER_WANT - a.FREQ_ASSIGN) < (:VAR_BANDWIDTH_WANT / 2 + 0.9))
              )
          or (a.NOTICE_TYPE = 'GT2' or a.NOTICE_TYPE = 'DT2')
            and (
              a.FREQ_ASSIGN > :VAR_CARRIER_WANT and (a.FREQ_ASSIGN - :VAR_CARRIER_WANT) < (:VAR_BANDWIDTH_WANT / 2  + 3.99)
              or (a.FREQ_ASSIGN < :VAR_CARRIER_WANT and (:VAR_CARRIER_WANT - a.FREQ_ASSIGN) < (:VAR_BANDWIDTH_WANT / 2 + 3.99))
              )
        )
        and UDF_DISTANCE(p.LAT, p.LON, :VAR_TX_LAT, :VAR_TX_LON) <= :IN_RADIUS * 1000
    group by 1,2
    into :OUT_TX_ID, :OUT_STATUS, OUT_DISTANCE
    do
        suspend;



/* 30-05-2007 */
alter table DIG_ALLOT_CNTR_LNK
add constraint PK_DIG_ALLOT_CNTR_LNK
primary key (ALLOT_ID,CNTR_ID);


/* 20-07-2007 */
/* вертигальная диаграмма направленности антенны */

update TRANSMITTERS t set t.EFFECTANTENNAGAINSVER = t.EFFECTANTENNAGAINS;


SET TERM ^ ;

drop TRIGGER TR_TRANSMITTERS_AU_0^

ALTER TABLE TRANSMITTERS ALTER EFFECTANTENNAGAINS TO ANT_DIAG_H^

ALTER TABLE TRANSMITTERS ALTER EFFECTANTENNAGAINSVER TO ANT_DIAG_V^


CREATE TRIGGER TR_TRANSMITTERS_AU_0 FOR TRANSMITTERS
ACTIVE AFTER UPDATE POSITION 0
AS
declare variable var_userid integer;
declare variable var_namefield varchar(256);
declare variable var_currenttime Timestamp;
begin
  var_currenttime=current_timestamp;
  select ID from admit where LOGIN=user into :var_userid;
  if (new.STAND_ID<>old.STAND_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='STAND_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.LATITUDE<>old.LATITUDE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LATITUDE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.LONGITUDE<>old.LONGITUDE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LONGITUDE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.OWNER_ID<>old.OWNER_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='OWNER_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.LICENSE_CHANNEL_ID<>old.LICENSE_CHANNEL_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LICENSE_CHANNEL_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.LICENSE_RFR_ID<>old.LICENSE_RFR_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LICENSE_RFR_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.LICENSE_SERVICE_ID<>old.LICENSE_SERVICE_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LICENSE_SERVICE_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.NUMPERMBUILD<>old.NUMPERMBUILD) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMPERMBUILD' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DATEPERMBUILDFROM<>old.DATEPERMBUILDFROM) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMBUILDFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DATEPERMBUILDTO<>old.DATEPERMBUILDTO) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMBUILDTO' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.NUMPERMUSE<>old.NUMPERMUSE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMPERMUSE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DATEPERMUSEFROM<>old.DATEPERMUSEFROM) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMUSEFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DATEPERMUSETO<>old.DATEPERMUSETO) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMUSETO' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.REGIONALCOUNCIL<>old.REGIONALCOUNCIL) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='REGIONALCOUNCIL' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.NUMPERMREGCOUNCIL<>old.NUMPERMREGCOUNCIL) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMPERMREGCOUNCIL' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DATEPERMREGCOUNCIL<>old.DATEPERMREGCOUNCIL) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMREGCOUNCIL' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
/*  if (new.NOTICECOUNT<>old.NOTICECOUNT) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$description='NOTICECOUNT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end */
  if (new.NUMSTANDCERTIFICATE<>old.NUMSTANDCERTIFICATE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMSTANDCERTIFICATE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DATESTANDCERTIFICATE<>old.DATESTANDCERTIFICATE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATESTANDCERTIFICATE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.NUMFACTORY<>old.NUMFACTORY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMFACTORY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.RESPONSIBLEADMIN<>old.RESPONSIBLEADMIN) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='RESPONSIBLEADMIN' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ADMINISTRATIONID<>old.ADMINISTRATIONID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ADMINISTRATIONID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.REGIONALAGREEMENT<>old.REGIONALAGREEMENT) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='REGIONALAGREEMENT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DATEINTENDUSE<>old.DATEINTENDUSE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEINTENDUSE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.AREACOVERAGE<>old.AREACOVERAGE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='AREACOVERAGE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SYSTEMCAST_ID<>old.SYSTEMCAST_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SYSTEMCAST_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ACCOUNTCONDITION_IN<>old.ACCOUNTCONDITION_IN) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ACCOUNTCONDITION_IN' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ACCOUNTCONDITION_OUT<>old.ACCOUNTCONDITION_OUT) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ACCOUNTCONDITION_OUT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.TIMETRANSMIT<>old.TIMETRANSMIT) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TIMETRANSMIT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.CHANNEL_ID<>old.CHANNEL_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='CHANNEL_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
/*  if (new.VIDEO_CARRIER<>old.VIDEO_CARRIER) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_CARRIER' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.VIDEO_OFFSET_HERZ<>old.VIDEO_OFFSET_HERZ) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_OFFSET_HERZ' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end */
  if (new.VIDEO_OFFSET_LINE<>old.VIDEO_OFFSET_LINE) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_OFFSET_LINE' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.FREQSTABILITY<>old.FREQSTABILITY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FREQSTABILITY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.TYPEOFFSET<>old.TYPEOFFSET) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPEOFFSET' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SYSTEMCOLOUR<>old.SYSTEMCOLOUR) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SYSTEMCOLOUR' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.VIDEO_EMISSION<>old.VIDEO_EMISSION) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_EMISSION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.POWER_VIDEO<>old.POWER_VIDEO) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POWER_VIDEO' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.EPR_VIDEO_MAX<>old.EPR_VIDEO_MAX) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_VIDEO_MAX' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
/*  if (new.EPR_VIDEO_HOR<>old.EPR_VIDEO_HOR) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_VIDEO_HOR' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.EPR_VIDEO_VERT<>old.EPR_VIDEO_VERT) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_VIDEO_VERT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end */
  if (bf_compare_blob(new.EFFECTPOWERHOR, old.EFFECTPOWERHOR)=0) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EFFECTPOWERHOR' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (bf_compare_blob(new.EFFECTPOWERVER, old.EFFECTPOWERVER)=0) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EFFECTPOWERVER' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ALLOTMENTBLOCKDAB_ID<>old.ALLOTMENTBLOCKDAB_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ALLOTMENTBLOCKDAB_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.GUARDINTERVAL_ID<>old.GUARDINTERVAL_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='GUARDINTERVAL_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.IDENTIFIERSFN<>old.IDENTIFIERSFN) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='IDENTIFIERSFN' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.RELATIVETIMINGSFN<>old.RELATIVETIMINGSFN) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='RELATIVETIMINGSFN' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.BLOCKCENTREFREQ<>old.BLOCKCENTREFREQ) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='BLOCKCENTREFREQ' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SOUND_CARRIER_PRIMARY<>old.SOUND_CARRIER_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_CARRIER_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SOUND_OFFSET_PRIMARY<>old.SOUND_OFFSET_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_OFFSET_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SOUND_EMISSION_PRIMARY<>old.SOUND_EMISSION_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_EMISSION_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.POWER_SOUND_PRIMARY<>old.POWER_SOUND_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POWER_SOUND_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.EPR_SOUND_MAX_PRIMARY<>old.EPR_SOUND_MAX_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_MAX_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
/*  if (new.EPR_SOUND_HOR_PRIMARY<>old.EPR_SOUND_HOR_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_HOR_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.EPR_SOUND_VERT_PRIMARY<>old.EPR_SOUND_VERT_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_VERT_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end */
  if (new.V_SOUND_RATIO_PRIMARY<>old.V_SOUND_RATIO_PRIMARY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='V_SOUND_RATIO_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SOUND_EMISSION_SECOND<>old.SOUND_EMISSION_SECOND) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_EMISSION_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.POWER_SOUND_SECOND<>old.POWER_SOUND_SECOND) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POWER_SOUND_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.EPR_SOUND_MAX_SECOND<>old.EPR_SOUND_MAX_SECOND) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_MAX_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
/*  if (new.EPR_SOUND_HOR_SECOND<>old.EPR_SOUND_HOR_SECOND) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_HOR_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.EPR_SOUND_VER_SECOND<>old.EPR_SOUND_VER_SECOND) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_VER_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end */
  if (new.SOUND_SYSTEM_SECOND<>old.SOUND_SYSTEM_SECOND) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_SYSTEM_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.V_SOUND_RATIO_SECOND<>old.V_SOUND_RATIO_SECOND) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='V_SOUND_RATIO_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.HEIGHTANTENNA<>old.HEIGHTANTENNA) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='HEIGHTANTENNA' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
/*  if (new.HEIGHT_EFF_MAX<>old.HEIGHT_EFF_MAX) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='HEIGHT_EFF_MAX' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end */
  if (bf_compare_blob(new.EFFECTHEIGHT, old.EFFECTHEIGHT)=0) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EFFECTHEIGHT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.POLARIZATION<>old.POLARIZATION) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POLARIZATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.DIRECTION<>old.DIRECTION) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DIRECTION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.FIDERLOSS<>old.FIDERLOSS) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FIDERLOSS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.FIDERLENGTH<>old.FIDERLENGTH) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FIDERLENGTH' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ANGLEELEVATION_HOR<>old.ANGLEELEVATION_HOR) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANGLEELEVATION_HOR' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ANGLEELEVATION_VER<>old.ANGLEELEVATION_VER) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANGLEELEVATION_VER' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ANTENNAGAIN<>old.ANTENNAGAIN) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANTENNAGAIN' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (bf_compare_blob(new.ANT_DIAG_H, old.ANT_DIAG_H)=0) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANT_DIAG_H' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (bf_compare_blob(new.ANT_DIAG_V, old.ANT_DIAG_V)=0) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANT_DIAG_V' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.NAMEPROGRAMM<>old.NAMEPROGRAMM) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NAMEPROGRAMM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.ORIGINALID<>old.ORIGINALID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ORIGINALID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.NUMREGISTRY<>old.NUMREGISTRY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMREGISTRY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.TYPEREGISTRY<>old.TYPEREGISTRY) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPEREGISTRY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.REMARKS<>old.REMARKS) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='REMARKS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.RELAYSTATION_ID<>old.RELAYSTATION_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='RELAYSTATION_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.OPERATOR_ID<>old.OPERATOR_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='OPERATOR_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.TYPERECEIVE_ID<>old.TYPERECEIVE_ID) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPERECEIVE_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.LEVELSIDERADIATION<>old.LEVELSIDERADIATION) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LEVELSIDERADIATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.FREQSHIFT<>old.FREQSHIFT) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FREQSHIFT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SUMMATORPOWERS<>old.SUMMATORPOWERS) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORPOWERS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.AZIMUTHMAXRADIATION<>old.AZIMUTHMAXRADIATION) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='AZIMUTHMAXRADIATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SUMMATOFREQFROM<>old.SUMMATOFREQFROM) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATOFREQFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SUMMATORFREQTO<>old.SUMMATORFREQTO) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORFREQTO' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SUMMATORPOWERFROM<>old.SUMMATORPOWERFROM) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORPOWERFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SUMMATORPOWERTO<>old.SUMMATORPOWERTO) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORPOWERTO' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SUMMATORMINFREQS<>old.SUMMATORMINFREQS) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORMINFREQS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.SUMMATORATTENUATION<>old.SUMMATORATTENUATION) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORATTENUATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.STATUS<>old.STATUS) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='STATUS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.TYPESYSTEM<>old.TYPESYSTEM) then begin
    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPESYSTEM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;
    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,
                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,
                             :var_currenttime, 'Eciaiaiea', 'TRANSMITTERS', old.id, :var_namefield);
  end
  if (new.STATUS = 0 or (old.STATUS = 0)) then
    update transmitters set WAS_IN_BASE = 1 where id = new.ID and WAS_IN_BASE <> 1;
END

^

SET TERM ; ^


/* 25-07-2007 : script creating tables to store allotment interference
 * zones */

create table DIG_ALLOT_ZONE (
Id DM_IDENTY_PK,
Allot_id DM_IDENTY_FK,
Emin DM_DBELL,
Note DM_VARCHAR64 );

alter table dig_allot_zone add constraint pk_allot_zone primary key (id);
alter table dig_allot_zone add constraint fk_allot_zone foreign key (allot_id) references DIG_ALLOTMENT(id) on update cascade on delete cascade;


create table DIG_ALLOT_ZONE_POINT (
Zone_id DM_IDENTY_FK,
Point_no DM_INTEGER not null,
Lon DM_GEOPOINT not null,
Lat DM_GEOPOINT not null );

alter table dig_allot_zone_point add constraint pk_allot_zone_point primary key (zone_id, point_no);
alter table dig_allot_zone_point add constraint fk_allot_zone_point foreign key (zone_id) references dig_allot_zone(id) on update cascade on delete cascade;

