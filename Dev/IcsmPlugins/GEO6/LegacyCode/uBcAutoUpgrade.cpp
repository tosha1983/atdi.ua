//---------------------------------------------------------------------------


#pragma hdrstop

#include "uBcAutoUpgrade.h"
#include "uMainDm.h"
#include "LISBCTxServer_TLB.h"
#pragma package(smart_init)

#include <map>
#include <memory>
#include <DBClient.hpp>
#include <Dialogs.hpp>
//---------------------------------------------------------------------------

BcAutoUpgrade::BcAutoUpgrade(TIBDatabase *db, double version)
    :TDBAutoUpgrade(db, version, NULL)
{
}
//---------------------------------------------------------------------------

class TProgressForm : public TForm, TUpgradeProgress
{
    TLabel *lbl;
    TProgressBar* pb;
public:
    virtual AnsiString __fastcall GetLabelCaption() { return lbl->Caption; };
    virtual void __fastcall SetLabelCaption(AnsiString s) {lbl->Caption = s; lbl->Update(); };
    virtual int __fastcall GetMax() { return pb->Max; };
    virtual void __fastcall SetMax(int n) { pb->Max = n; pb->Update(); };
    virtual void __fastcall StepIt() { pb->StepIt(); pb->Update(); };
    virtual int __fastcall GetPos() { return pb->Position; };
    virtual void __fastcall SetPos(int n) { pb->Position = n; pb->Update(); };

    __fastcall TProgressForm(TComponent* owner, int dummy) : TForm(owner, dummy)
    {
        BorderStyle = bsDialog;
        Caption = "Progress Process";
        FormStyle = fsStayOnTop;
        Visible = false;
        Width = 400;

        lbl = new TLabel(this);
        lbl->Parent = this;
        lbl->Top = 8;
        lbl->Left= 12;
        lbl->Caption = "Progress Message";

        pb = new TProgressBar(this);
        pb->Left = 4;
        pb->Max = 100;
        pb->Min = 0;
        pb->Parent = this;
        pb->Step = 1;
        pb->Smooth = true;
        pb->Top = lbl->Top + lbl->Height + 10;
        pb->Width = this->ClientWidth - pb->Left * 2;
        pb->Height = 20;

        ClientHeight = pb->Top + pb->Height + 8;


        #ifdef _DEBUG
        Left = Screen->Width - Width - 5;
        Top = Screen->Height - Height - 5;
        HWND tbh = FindWindow("Shell_TrayWnd", "");
        if (tbh != NULL)
        {
            TRect tbRect; // taskbar rectangle
            if (GetWindowRect(tbh, &tbRect))
                Top = Top - (tbRect.Bottom - tbRect.Top);
        }
        #else
        Position = poScreenCenter;
        #endif                 
    }

};

void __fastcall BcAutoUpgrade::UpgradeDb()
{
    //непосредственно обновляем метаданные
    std::auto_ptr<TIBSQL> auxSQL(new TIBSQL(Application));
    auxSQL->ParamCheck = false;
    m_sql->ParamCheck = false;

    double tmp_version = GetDbVersion();
    if(tmp_version < m_ver)
    {
        TDBAutoUpgrade::UpgradeDb();

        int UPDATE_COUNT = 25; //общее количество обновлений
        std::auto_ptr<TProgressForm> progress (new TProgressForm(Application, 1));
        m_prgrs = (TUpgradeProgress*)(progress.get());
        progress->Caption = "Обновление БД. Дождитесь его завершения";
        progress->SetMax(UPDATE_COUNT);
        progress->Show();

        //START_UPD_VER(tmp_version, 0.0)
        //END_UPD_VER()
        // на всякий случай
        START_UPD_VER(tmp_version, 0.0)
        if (RunQuery("SELECT VERSION FROM SYSTEM_SCHEME WHERE NAME = 'SYSTEM';"))
        {
            RunQuery("INSERT INTO SYSTEM_SCHEME (NAME, VERSION) VALUES ('SYSTEM', 0);");
            m_tr->CommitRetaining();
        }
        END_UPD_VER()

        CreateOriginalSchema(tmp_version);

        START_UPD_VER(tmp_version, 20080116.1502)
            RunQuery("alter domain DM_ADDRESS type varchar(128)", uaUpdate, uoDomain, "DM_ADDRESS");
            Table ats("ANALOGTELESYSTEM", this);
            Table dts("DIGITALTELESYSTEM", this);
            Table ars("ANALOGRADIOSYSTEM", this);
            CreateDomain("DM_VARCHAR128", "varchar(128)", "");
            ats.AddField("DESCR", "DM_VARCHAR128", false, "");
            ars.AddField("DESCR", "DM_VARCHAR128", false, "");
            dts.AddField("DESCR", "DM_VARCHAR128", false, "");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080125.1557)
            Table tx("TRANSMITTERS", this);
            tx.AddField("EMC_CONCL_NUM", "DM_VARCHAR16", false, "");
            tx.AddField("EMC_CONCL_FROM", "DM_DATE", false, "");
            tx.AddField("EMC_CONCL_TO", "DM_DATE", false, "");
            tx.AddField("PERMUSE_OWNER_ID", "DM_INTEGER", false, "");
            tx.AddForeignKey("PERMUSE_OWNER_ID", "OWNER(ID)", true, false);
            tx.AddField("NR_REQ_NO", "DM_VARCHAR16", false, "");
            tx.AddField("NR_REQ_DATE", "DM_DATE", false, "");
            tx.AddField("NR_CONCL_NO", "DM_VARCHAR16", false, "");
            tx.AddField("NR_CONCL_DATE", "DM_DATE", false, "");
            tx.AddField("NR_APPL_NO", "DM_VARCHAR16", false, "");
            tx.AddField("NR_APPL_DATE", "DM_DATE", false, "");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080125.1722)
            Table lc_link("NR_LIC_LINK", this);
            lc_link.AddField("TX_ID", "DM_IDENTY_FK", false, "");
            lc_link.AddField("LIC_ID", "DM_IDENTY_FK", false, "");
            lc_link.AddForeignKey("TX_ID", "TRANSMITTERS(ID)", true, false);
            lc_link.AddForeignKey("LIC_ID", "LICENSE(ID)", true, false);
            lc_link.CreateTable();
            lc_link.GrantAll("PUBLIC");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080125.1838)
            Table lc_link("NR_LIC_LINK", this);
            lc_link.AddPrimaryKey("TX_ID,LIC_ID");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080126.1736)
            Table owner("OWNER", this);
            owner.AddField("AVB", "DM_BOOLEAN", false, "");
            owner.AddField("AAB", "DM_BOOLEAN", false, "");
            owner.AddField("DVB", "DM_BOOLEAN", false, "");
            owner.AddField("DAB", "DM_BOOLEAN", false, "");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080128.1326)                  //130-183
            Table lic("LICENSE", this);
            lic.AddField("CALLSIGN", "DM_VARCHAR32", false, "");
            lic.AddField("ANNUL", "DM_BOOLEAN", false, "");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080131.1533)                  //131-189
            Table tx("TRANSMITTERS", this);
            tx.AddField("POL_ISOL", "DM_DBELL", false, "");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080223.1348)                  //134-200
            CreateDomain("DM_EMISSION_CLASS", "CHAR(9)", "");
            CreateDomain("DM_DIRECTIVITY", "CHAR(2)", " check (value is null or value in ('D', 'ND'))");
            CreateDomain("DM_TYPE_OF_POWER", "CHAR(1)", " check (value is null or value in ('X', 'Y', 'Z'))");
            CreateDomain("DM_HEIGHTARRAY", "SMALLINT[36]", "");
            Table other("OTHER_PRIM_TERR", this);
            other.AddField("ID", "DM_IDENTY_PK", true, "");
            other.AddField("ADM", "DM_COUNTRYCODE", false, "");
            other.AddField("ADM_REF_ID", "DM_ADM_REF_ID30", false, "");
            other.AddField("CALL_SIGN", "DM_CALLSIGN", false, "");
            other.AddField("FREQ", "DM_MHERZ", false, "");
            other.AddField("REF_FREQ", "DM_MHERZ", false, "");
            other.AddField("DATE_INTO_USE", "DM_DATE", false, "");
            other.AddField("DATE_EXPIRE", "DM_DATE", false, "");
            other.AddField("TX_LOCATION", "DM_ADDRESS", false, "");
            other.AddField("TX_COUNTRY", "DM_COUNTRYCODE", false, "");
            other.AddField("TX_LAT", "DM_GEOPOINT", false, "");
            other.AddField("TX_LON", "DM_GEOPOINT", false, "");
            other.AddField("TX_RADIUS", "DM_KMETR", false, "");
            other.AddField("RX_LOCATION", "DM_ADDRESS", false, "");
            other.AddField("RX_COUNTRY", "DM_COUNTRYCODE", false, "");
            other.AddField("RX_LAT", "DM_GEOPOINT", false, "");
            other.AddField("RX_LON", "DM_GEOPOINT", false, "");
            other.AddField("RX_RADIUS", "DM_KMETR", false, "");
            other.AddField("STA_CLASS", "DM_VARCHAR4", false, "");
            other.AddField("SERVICE_NATURE", "DM_VARCHAR4", false, "");
            other.AddField("EMISSION_CLASS", "DM_EMISSION_CLASS", false, "");
            other.AddField("SYSTEM_TYPE", "DM_VARCHAR4", false, "");
            other.AddField("TYPE_OF_POWER", "DM_TYPE_OF_POWER", false, "");
            other.AddField("TX_OUT_POWER", "DM_DBELL", false, "");
            other.AddField("MAX_PWR_DENS", "DM_DBELL", false, "");
            other.AddField("ERP_MAX", "DM_DBELL", false, "");
            other.AddField("MAX_ANT_GAIN", "DM_DBELL", false, "");
            other.AddField("POL", "DM_POLARIZATION default 'U'", false, "");
            other.AddField("ANT_HEIGHT", "DM_HEIGHTMETR", false, "");
            other.AddField("DIR", "DM_DIRECTIVITY", false, "");
            other.AddField("MAIN_BEAM", "DM_ANGLE", false, "");
            other.AddField("GAIN", "DM_DBELL", false, "");
            other.AddField("SITE_ALT", "DM_HEIGHTMETR", false, "");
            other.AddField("MAX_EAH", "DM_HEIGHTMETR", false, "");
            other.AddField("EAH", "DM_HEIGHTARRAY", false, "");
            other.AddField("AZM", "DM_ANGLE", false, "");
            other.AddField("AZM_MAX", "DM_ANGLE", false, "");
            other.AddField("AZM_MIN", "DM_ANGLE", false, "");
            other.AddField("OP_HH_FR", "DM_TIME", false, "");
            other.AddField("OP_HH_TO", "DM_TIME", false, "");
            other.AddField("COORD_REQ", "DM_BOOLEAN", false, "");
            other.AddField("SIGNED_COMM", "DM_BOOLEAN", false, "");
            other.AddField("OPR_AGENCY", "DM_VARCHAR64", false, "");
            other.AddField("ADM_ADDRESS", "DM_ADDRESS", false, "");
            other.AddField("REMARKS", "DM_VARCHAR512", false, "");
            other.CreateTable();
            other.GrantAll();
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080223.1657)                  //134-200
            Table other("OTHER_PRIM_TERR", this);
            other.AddPrimaryKey("ID");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080402.1152)                  //210->211
            Table tx("TRANSMITTERS", this);
            tx.AddField("GND_COND", "DM_DOUBLEPRECISION", false, "");
            CreateDomain("DM_FLOAT", "FLOAT", "");
            CreateDomain("DM_GAINARRAY", "BLOB SUB_TYPE 0 SEGMENT SIZE 144", "");
            CreateDomain("DM_DAYNIGHT", "CHAR(2) NOT NULL", " check (value in ('HJ', 'HN'))");
            CreateDomain("DM_ANT_TYPE", "VARCHAR(1)", " check (value is null or value in ('A','B'))");
            CreateDomain("DM_PTRN_TYPE", "VARCHAR(1)", " check (value is null or value in ('T','E','M'))");
            CreateDomain("DM_TIMENUM", "NUMERIC(4,2)", "");

            Table lfmf("LFMF_OPER", this);
            lfmf.AddField("STA_ID", "DM_IDENTY_FK", true, "");      // station reference
            lfmf.AddField("DAYNIGHT", "DM_DAYNIGHT", false, "");    //Night or day
            lfmf.AddField("AZIMUTH", "DM_ANGLE", false, "");        //NUMBER(6,2) Azimuth of maximum radiation (Degrees) deg
            lfmf.AddField("ANGLE_ELEV",  "DM_ANGLE", false, "");    //Elevation of maximum radiation (Degrees) deg
            lfmf.AddField("SERV", "DM_BOOLEAN", false, "");         //Full service (Boolean) Bool
            lfmf.AddField("BDWDTH", "DM_KHERZ", false, "");         //7A1 - Necessary Bandwidth (kHz) BW/kHz
            lfmf.AddField("ADJ_RATIO", "DM_SMALLINT", false, "");   //NUMBER(2,0) //7B - Adjacent channel protection ratio (G75AdjRatio) eri_G75AdjRatio
            lfmf.AddField("PWR_KW", "DM_KWATT", false, "");         //8A - Power to antenna (kW) kW
            lfmf.AddField("AGL", "DM_HEIGHTMETR", false, "");       //9E - Antenna height above ground level (m) AGL
            lfmf.AddField("E_RMS", "DM_FLOAT",  false, "");         //NUMBER(6,2) //9I - R.M.S. value of radiation (mV/m) mV/m
            lfmf.AddField("E_MAX", "DM_DBELL", false, "");          //NUMBER(4,1) //9L - Maximum effective monopole radiated power (dB(kW)) dBkW
            lfmf.AddField("ANT_TYPE",  "DM_ANT_TYPE", false, "");   //VARCHAR(1) //9Q - Type of antenna (LfmfBroTTow) eri_LfmfBroTTow
            lfmf.AddField("PTRN_TYPE", "DM_PTRN_TYPE", false, "");  //VARCHAR(1) //9O - Type of pattern (Rj81Ptrn) eri_Rj81Ptrn
            lfmf.AddField("Q_FACT", "DM_FLOAT", false, "");         //NUMBER(4,1) //9P - Special Quadrature factor(9P) (mV/m) mV/m
            lfmf.AddField("START_TIME", "DM_TIMENUM", false, "");   //10B - Start time (UTC) Hour
            lfmf.AddField("STOP_TIME", "DM_TIMENUM", false, "");    //10B - Stop time (UTC) Hour
            lfmf.AddField("ERP", "DM_WATT", false, "");             //NUMBER(5,2) //Maximum radiated power (dBW) dBW
            lfmf.AddField("GAIN_AZM", "DM_GAINARRAY", false, "");   //GAIN_AZM (dB)
            lfmf.CreateTable();
            lfmf.AddPrimaryKey("STA_ID,DAYNIGHT");
            lfmf.AddForeignKey("STA_ID", "TRANSMITTERS(ID)", true, true);
            lfmf.GrantAll();
            //lfmf.AddField("HGT_ELEC", NUMBER(4,1)                 //9F - Electrical antenna height (Elec. Degrees) EDeg
            //lfmf.AddField("ATTN_H", VARCHAR(190)                  //9N - Horizontal attenuations

            RunQuery("ALTER DOMAIN DM_CLASSWAVE DROP CONSTRAINT");
            RunQuery("ALTER DOMAIN DM_CLASSWAVE ADD CHECK (VALUE IN ('VHF', 'UHF', 'LFMF'))");
            if (RunQuery("select ENUMVAL from SYSTEMCAST where ENUMVAL = 3"))
            try {
                RunQuery("insert into SYSTEMCAST(ID, CODE, DESCRIPTION, CLASSWAVE, FREQFROM, FREQTO, ENUMVAL, RELATIONNAME) "
                        "values ("+IntToStr(dmMain->getNewId())+", 'СХДХ', 'Мовлення в СХ/ДХ дiапазонах', 'LFMF', "
                        "0.03, 3.0, "+IntToStr(ttAM)+", 'LFMF_SYSTEM')",
                        uaInsert, uoTable, "SYSTEMCAST");
            } catch (...) {}

        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080403.1626)                  //210->211
            Table sc("SYSTEMCAST", this);
            sc.AddField("NOTUSED", "DM_BOOLEAN", true, "");

            Table lfmf_sys("LFMF_SYSTEM", this);
            lfmf_sys.AddField("ENUM", "DM_INTEGER", true, "");
            lfmf_sys.AddField("CODE", "DM_VARCHAR8", true, "");
            lfmf_sys.AddField("BW", "DM_KHERZ", true, "");
            lfmf_sys.AddPrimaryKey("ENUM");
            lfmf_sys.CreateTable();
            lfmf_sys.GrantAll("PUBLIC");

        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080404.1246)                  //210->211
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (1, 'AM', 4.5)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (2, 'DRM_A0', 4.5)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (3, 'DRM_A1', 5)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (4, 'DRM_A2', 9)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (5, 'DRM_A3', 10)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (6, 'DRM_B0', 4.5)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (7, 'DRM_B1', 5)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (8, 'DRM_B2', 9)");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE, BW) values (9, 'DRM_B3', 10)");
            /*
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (10, 'DRM_C0')");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (11, 'DRM_C1')");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (12, 'DRM_C2')");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (13, 'DRM_C3')");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (14, 'DRM_D0')");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (15, 'DRM_D1')");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (16, 'DRM_D2')");
            RunQuery("insert into LFMF_SYSTEM (ENUM, CODE) values (17, 'DRM_D3')");
            */                                    
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080415.1733)                  //210->211
            Table sc("SYSTEMCAST", this);
            sc.AddField("IS_USED", "DM_BOOLEAN", true, "");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080415.1735)                  //210->211
            RunQuery("UPDATE SYSTEMCAST set IS_USED = 1 where NOTUSED is null or NOTUSED = 0", uaUpdate, uoTable, "Системы вещания");
            RunQuery("UPDATE SYSTEMCAST set IS_USED = 0 where NOTUSED <> 0 ", uaUpdate, uoTable, "Системы вещания");
            RunQuery("alter table SYSTEMCAST drop NOTUSED", uaUpdate, uoTable, "Системы вещания");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080513.1823)                  //230->
            RunQuery("alter PROCEDURE SP_SET_CARRIER_AND_BW_3(\
        IN_TX_ID INTEGER,                          \
        IN_TX_VIDEO_CARRIER DOUBLE PRECISION,      \
        IN_TX_SOUND_CARRIER DOUBLE PRECISION,      \
        IN_TX_BLOCKCENTREFREQ DOUBLE PRECISION,    \
        IN_TX_SYSTEMCAST_ID INTEGER,               \
        IN_TX_TYPESYSTEM INTEGER,                  \
        IN_TX_CHANNEL_ID INTEGER)                  \
    RETURNS (                                      \
        OUT_CARRIER DOUBLE PRECISION,              \
        OUT_BANDWIDTH DOUBLE PRECISION)            \
    AS                                             \
    DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;        \
    DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
    DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
    DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
    DECLARE VARIABLE VAR_FREQ_BEG DOUBLE PRECISION;\
    DECLARE VARIABLE VAR_FREQ_END DOUBLE PRECISION;\
    DECLARE VARIABLE VAR_CHANNELBAND DOUBLE PRECISION;\
    DECLARE VARIABLE VAR_ARS_VAL INTEGER;\
    DECLARE VARIABLE VAR_LFMFBAND DOUBLE PRECISION;\
    begin                                 \
    /***********************************************/\n\
        select ENUMVAL from SYSTEMCAST where ID = :IN_TX_SYSTEMCAST_ID into :VAR_TX_SC_VAL;\n\
        select CHANNELBAND from ANALOGTELESYSTEM where ID = :IN_TX_TYPESYSTEM into :VAR_CHANNELBAND;\n\
        select ENUMVAL from ANALOGRADIOSYSTEM where ID = :IN_TX_TYPESYSTEM into :VAR_ARS_VAL;\n\
        select BW from LFMF_SYSTEM where ENUM = :IN_TX_TYPESYSTEM into :VAR_LFMFBAND;\
        select FREQFROM, FREQTO from CHANNELS where ID = :IN_TX_CHANNEL_ID into :VAR_FREQ_BEG, :VAR_FREQ_END;\n\
        VAR_VIDEO_CARRIER = IN_TX_VIDEO_CARRIER;\n\
        VAR_SOUND_CARRIER = IN_TX_SOUND_CARRIER;\n\
        VAR_BLOCKCENTREFREQ = IN_TX_BLOCKCENTREFREQ;\n\
        select t.CARRIER, t.BANDWIDTH from TRANSMITTERS t where ID = :IN_TX_ID into :OUT_CARRIER, :OUT_BANDWIDTH;\n\
   \n\
   /***********************************************/\n\
    /*\n\
        select SC.ENUMVAL, ATS.CHANNELBAND, ARS.ENUMVAL, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ, CH.FREQFROM, CH.FREQTO\n\
        from TRANSMITTERS TX\n\
            left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\n\
            left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\n\
            left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\n\
            left outer join CHANNELS CH on (TX.CHANNEL_ID = CH.ID)\n\
        where TX.ID = :IN_TX_ID\n\
        into :VAR_TX_SC_VAL, :VAR_CHANNELBAND, :VAR_ARS_VAL, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ, :VAR_FREQ_BEG, :VAR_FREQ_END;\n\
    */\n\
        if (VAR_TX_SC_VAL = 1) then begin\n\
            /* TVA */\n\
            OUT_CARRIER = VAR_VIDEO_CARRIER;\n\
            if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\n\
                OUT_BANDWIDTH = VAR_CHANNELBAND;\n\
            else\n\
                OUT_BANDWIDTH = 8.0;\n\
        end else if (VAR_TX_SC_VAL = 2) then begin\n\
            /* FM SB */\n\
            OUT_CARRIER = VAR_SOUND_CARRIER;\n\
            if (VAR_ARS_VAL = 0) then\n\
                /* system 1 */\n\
                OUT_BANDWIDTH = 0.18;\n\
            else if (VAR_ARS_VAL = 1) then\n\
                /* system 2 */\n\
                OUT_BANDWIDTH = 0.13;\n\
            else if (VAR_ARS_VAL = 2) then\n\
                /* system 3 */\n\
                OUT_BANDWIDTH = 0.18;\n\
            else if (VAR_ARS_VAL = 3) then\n\
                /* system 4 */\n\
                OUT_BANDWIDTH = 0.22;\n\
            else\n\
                /* system 5 */\n\
                OUT_BANDWIDTH = 0.3;\n\
        end else if (VAR_TX_SC_VAL = 3) then begin\n\
            OUT_CARRIER = VAR_SOUND_CARRIER;\n\
            if (OUT_BANDWIDTH is null or OUT_BANDWIDTH = 0.0) then \n\
                OUT_BANDWIDTH = VAR_LFMFBAND / 1000.0;\n\
        end else if (VAR_TX_SC_VAL = 4) then begin\n\
            /* DVB */\n\
            OUT_CARRIER = (VAR_FREQ_BEG + VAR_FREQ_END) / 2;\n\
            if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\n\
                OUT_BANDWIDTH = VAR_CHANNELBAND;\n\
            else\n\
                OUT_BANDWIDTH = 8.0;\n\
        end else if (VAR_TX_SC_VAL = 5) then begin\n\
            /* DAB */\n\
            OUT_BANDWIDTH = 1.8;\n\
            OUT_CARRIER = VAR_BLOCKCENTREFREQ;\n\
        end else begin\n\
            /* unknown system */\n\
            OUT_CARRIER = 0;\n\
            OUT_BANDWIDTH = 0;\n\
        end\n\
        if (OUT_CARRIER is null) then\n\
            OUT_CARRIER = 0;\n\
             \n\
        suspend;\n\
        \n\
    end\n\
    ");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080704.1318)                  //270->
            Table tt("STAND", this);
            tt.AddField("MAX_ANT_HGT", "DM_HEIGHTMETR", false);
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080706.1915)                  //276->
            Table tt("TRANSMITTERS", this);
            tt.AddField("MOD_TYPE", "DM_SMALLINT", false);
            tt.AddField("PROT_LEVL", "DM_SMALLINT", false);
            RunQuery("alter table LFMF_OPER alter ERP type DM_DBELL");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080715.2015)                  //285->
            Table tt("TRANSMITTERS", this);
            tt.AddField("NOISE_ZONE", "DM_SMALLINT", false);
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080731.1444)                  //285->
            Table lfmf("LFMF_OPER", this);
            lfmf.AddField("ETALON_ZONE", "DM_GAINARRAY", false, "");      // etalon zone
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080910.1227)
            String insertClause("INSERT INTO SYSTEMCAST (ID,CODE,DESCRIPTION,TYPESYSTEM,CLASSWAVE,FREQFROM,FREQTO,NUMDIAPASON,RELATIONNAME,ENUMVAL,IS_USED) VALUES (");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'FXM','Другие первичные',0,'VHF',174.,862.,1,'',-1,1)", uaInsert, uoTable, "Типы передатчиков");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20081006.2220)
			RunQuery("\n\
			ALTER PROCEDURE SP_SET_CARRIER_AND_BW_3 (\n\
			    IN_TX_ID INTEGER,\n\
			    IN_TX_VIDEO_CARRIER DOUBLE PRECISION,\n\
			    IN_TX_SOUND_CARRIER DOUBLE PRECISION,\n\
			    IN_TX_BLOCKCENTREFREQ DOUBLE PRECISION,\n\
			    IN_TX_SYSTEMCAST_ID INTEGER,\n\
			    IN_TX_TYPESYSTEM INTEGER,\n\
			    IN_TX_CHANNEL_ID INTEGER)\n\
			RETURNS (\n\
			    OUT_CARRIER DOUBLE PRECISION,\n\
			    OUT_BANDWIDTH DOUBLE PRECISION)\n\
			AS\n\
			DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;\n\
			DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\n\
			DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\n\
			DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\n\
			DECLARE VARIABLE VAR_FREQ_BEG DOUBLE PRECISION;\n\
			DECLARE VARIABLE VAR_FREQ_END DOUBLE PRECISION;\n\
			DECLARE VARIABLE VAR_CHANNELBAND DOUBLE PRECISION;\n\
			DECLARE VARIABLE VAR_BANDWIDTH DOUBLE PRECISION;\n\
			DECLARE VARIABLE VAR_ARS_VAL INTEGER;\n\
			begin\n\
			/*\n\
			    select SC.ENUMVAL, ATS.CHANNELBAND, ARS.ENUMVAL, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ, CH.FREQFROM, CH.FREQTO, TX.BANDWIDTH\n\
			    from TRANSMITTERS TX\n\
			        left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\n\
			        left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\n\
			        left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\n\
			        left outer join CHANNELS CH on (TX.CHANNEL_ID = CH.ID)\n\
			    where TX.ID = :IN_TX_ID\n\
			    into :VAR_TX_SC_VAL, :VAR_CHANNELBAND, :VAR_ARS_VAL, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ, :VAR_FREQ_BEG, :VAR_FREQ_END, :VAR_BANDWIDTH;\n\
			*/\n\
			    if (VAR_TX_SC_VAL = 1) then begin\n\
			        /* TVA */\n\
			        OUT_CARRIER = VAR_VIDEO_CARRIER;\n\
			        if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\n\
			            OUT_BANDWIDTH = VAR_CHANNELBAND;\n\
			        else\n\
			            OUT_BANDWIDTH = 8.0;\n\
			    end else if (VAR_TX_SC_VAL = 2) then begin\n\
			        /* FM SB */\n\
			        OUT_CARRIER = VAR_SOUND_CARRIER;\n\
			        if (VAR_ARS_VAL = 0) then\n\
			            /* system 1 */\n\
			            OUT_BANDWIDTH = 0.18;\n\
			        else if (VAR_ARS_VAL = 1) then\n\
			            /* system 2 */\n\
			            OUT_BANDWIDTH = 0.13;\n\
			        else if (VAR_ARS_VAL = 2) then\n\
			            /* system 3 */\n\
			            OUT_BANDWIDTH = 0.18;\n\
			        else if (VAR_ARS_VAL = 3) then\n\
			            /* system 4 */\n\
			            OUT_BANDWIDTH = 0.22;\n\
			        else\n\
			            /* system 5 */\n\
			            OUT_BANDWIDTH = 0.3;\n\
			    end else if (VAR_TX_SC_VAL = 4) then begin\n\
			        /* DVB */\n\
			        OUT_CARRIER = (VAR_FREQ_BEG + VAR_FREQ_END) / 2;\n\
			        if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\n\
			            OUT_BANDWIDTH = VAR_CHANNELBAND;\n\
			        else\n\
			            OUT_BANDWIDTH = 8.0;\n\
			    end else if (VAR_TX_SC_VAL = 5) then begin\n\
			        /* DAB */\n\
			        OUT_BANDWIDTH = 1.8;\n\
			        OUT_CARRIER = VAR_BLOCKCENTREFREQ;\n\
			    end else begin\n\
                    OUT_BANDWIDTH = OUT_BANDWIDTH;\n\
			        /* unknown system */\n\
                    /* do nothing */\n\
			    end\n\
			    suspend;\n\
			end\n\
			");

            CreateTrigger("TR_TRANS_BI_SET_CARR3","TRANSMITTERS","ACTIVE BEFORE INSERT POSITION 1 \n\
            as\n\
            declare variable var_carrier double precision;\n\
            declare variable var_bandwidth double precision;\n\
            begin\n\
                select out_carrier, out_bandwidth from SP_SET_CARRIER_AND_BW_3(new.id, new.video_carrier, new.sound_carrier_primary, new.blockcentrefreq, new.systemcast_id, new.typesystem, new.channel_id) into :var_carrier, :var_bandwidth;\n\
                if (var_carrier is not null and var_carrier <> 0) then\n\
                    new.carrier = :var_carrier;\n\
                if (var_bandwidth is not null and var_bandwidth <> 0) then\n\
                    new.bandwidth = :var_bandwidth;\n\
            end ");

            CreateTrigger("TR_TRANS_BU_SET_CARR3","TRANSMITTERS","ACTIVE BEFORE UPDATE POSITION 1\
            as\
            declare variable var_carrier double precision;\
            declare variable var_bandwidth double precision;\
            begin\n\
                if (\n\
                    (new.SYSTEMCAST_ID <> old.SYSTEMCAST_ID)\n\
                    or (old.SYSTEMCAST_ID is null and new.SYSTEMCAST_ID is null)\n\
                    or (new.TYPESYSTEM <> old.TYPESYSTEM)\n\
                    or (old.TYPESYSTEM is null and new.TYPESYSTEM is not null)\n\
                    or (new.VIDEO_CARRIER <> old.VIDEO_CARRIER)\n\
                    or (old.VIDEO_CARRIER is null and new.VIDEO_CARRIER is not null)\n\
                    or (new.SOUND_CARRIER_PRIMARY <> old.SOUND_CARRIER_PRIMARY)\n\
                    or (old.SOUND_CARRIER_PRIMARY is null and new.SOUND_CARRIER_PRIMARY is not null)\n\
                    or (new.CHANNEL_ID <> old.CHANNEL_ID)\n\
                    or (old.CHANNEL_ID is null and new.CHANNEL_ID is not null)\n\
                    or (new.BLOCKCENTREFREQ <> old.BLOCKCENTREFREQ)\n\
                    or (old.BLOCKCENTREFREQ is null and new.BLOCKCENTREFREQ is not null)\n\
                ) then\n\
                begin\n\
                    select out_carrier, out_bandwidth from SP_SET_CARRIER_AND_BW_3(new.id, new.video_carrier, new.sound_carrier_primary, new.blockcentrefreq, new.systemcast_id, new.typesystem, new.channel_id) into :var_carrier, :var_bandwidth;\n\
                    if (var_carrier is not null and var_carrier <> 0) then\n\
                        new.carrier = :var_carrier;\n\
                    if (var_bandwidth is not null and var_bandwidth <> 0) then\n\
                        new.bandwidth = :var_bandwidth;\n\
                end\n\
            END ");

        END_UPD_VER()

        START_UPD_VER(tmp_version, 20091109.1243)
            Table tt("ADMIT", this);
            tt.AddField("LASTNAME", "DM_VARCHAR128", false);
            tt.AddField("IDENT", "DM_VARCHAR64", false);
            tt.AddField("TEL", "DM_VARCHAR30", false);
            tt.AddField("POST", "DM_VARCHAR64", false);
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20091221.1415)
            Table tt("DOCUMENT", this);
            tt.AddField("DOCTYPE", "DM_INTEGER", false);
            tt.AddField("RTTYPE", "DM_INTEGER", false);
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20110723.1541)
            Table transmitters("TRANSMITTERS", this);
            transmitters.AddField("IS_DVB_T2","DM_BOOLEAN",false,"");
            transmitters.AddField("PILOT_PATTERN","DM_SMALLINT",false,"");
            transmitters.AddField("DIVERSITY","DM_SMALLINT",false,"");
            transmitters.AddField("ROTATED_CNSTLS","DM_BOOLEAN",false,"");
            transmitters.AddField("MODE_OF_EXTNTS","DM_BOOLEAN",false,"");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20110723.1542)
            RunQuery("update TRANSMITTERS set IS_DVB_T2=0, ROTATED_CNSTLS=0, MODE_OF_EXTNTS=0");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20111021.1332)
            Table transmitters("TRANSMITTERS", this);
            transmitters.AddField("MODULATION","DM_SMALLINT",false,"");
            transmitters.AddField("CODE_RATE","DM_SMALLINT",false,"");
            transmitters.AddField("FFT_SIZE","DM_SMALLINT",false,"");
            transmitters.AddField("GUARD_INTERVAL","DM_SMALLINT",false,"");
        END_UPD_VER()

        if(tmp_version != m_ver)
        {
            throw *(new Exception("Приложение не содержит последней схемы обновления базы данных. Обновите программное обеспечение."));
        }
    }
    return;
}
//---------------------------------------------------------------------------

