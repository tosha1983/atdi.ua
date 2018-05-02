#include "uBcAutoUpgrade.h"
//#include "uProgress.h"
//#include <DBClient.hpp>
#include "uMainDm.h"

void __fastcall BcAutoUpgrade::CreateOriginalSchema(double &tmp_version)
{

        START_UPD_VER(tmp_version, 20080101.0000)
            
            CreateFunction("BF_BLOB_TO_STR","BLOB","CSTRING(1024) FREE_IT","BlobAsPChar","ToolFuncLib.dll");
            CreateFunction("BF_COMPARE_BLOB","BLOB, BLOB","INTEGER BY VALUE","CompareBlobsByValue","ToolFuncLib.dll");
            CreateFunction("UDF_DISTANCE","DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION","DOUBLE PRECISION BY VALUE","distance","geo_sphere");
            CreateFunction("UDF_ISINTERFERE","INTEGER, INTEGER, DOUBLE PRECISION, DOUBLE PRECISION, INTEGER, INTEGER, DOUBLE PRECISION, DOUBLE PRECISION","INTEGER BY VALUE","bcIsTxInterf","LISBCIsInterfere");

            CreateDomain("DM_ADDRESS", "VARCHAR(64)", "");
            CreateDomain("DM_ADM_REF_ID20", "VARCHAR(20)", "");
            CreateDomain("DM_ADM_REF_ID30", "VARCHAR(30)", "");
            CreateDomain("DM_ANGLE", "DOUBLE PRECISION", "");
            CreateDomain("DM_ASSGN_CODE", "CHAR(1) ", "DEFAULT 'S' CHECK (value in ('L', 'C', 'S'))");
            CreateDomain("DM_AUDIO_VIDEO", "CHAR(1) ", "NOT NULL CHECK (value in ('A', 'V'))");
            CreateDomain("DM_BLOB", "BLOB ", "SUB_TYPE 0 SEGMENT SIZE 80");
            CreateDomain("DM_BOOLEAN", "SMALLINT ", "DEFAULT 0 CHECK (VALUE IN (0, 1))");
            CreateDomain("DM_CALLSIGN", "VARCHAR(10)", "");
            CreateDomain("DM_CLASSWAVE", "VARCHAR(4) ", "CHECK (VALUE IN ('VHF', 'UHF', 'LFMF'))");
            CreateDomain("DM_COORDDOCUMENT", "SMALLINT", "CHECK (VALUE IN (0,1))");
            CreateDomain("DM_COUNTRYCODE", "VARCHAR(3)", "");
            CreateDomain("DM_DATE", "DATE", "");
            CreateDomain("DM_DATETIME", "TIMESTAMP", "");
            CreateDomain("DM_DBELL", "DOUBLE PRECISION", "");
            CreateDomain("DM_DBKVT", "DOUBLE PRECISION", "");
            CreateDomain("DM_DEGREE", "SMALLINT", "");
            CreateDomain("DM_DIRECTION", "CHAR(2)", "CHECK (VALUE IN ('D', 'ND'))");
            CreateDomain("DM_DISTANCEKMETR", "SMALLINT", "");
            CreateDomain("DM_DOUBLEPRECISION", "DOUBLE PRECISION", "");
            CreateDomain("DM_GEOPOINT", "DOUBLE PRECISION", "");
            CreateDomain("DM_HEIGHTMETR", "INTEGER", "");
            CreateDomain("DM_HERZ", "INTEGER", "");
            CreateDomain("DM_IDENTY_FK", "INTEGER", "NOT NULL");
            CreateDomain("DM_IDENTY_PK", "INTEGER", "NOT NULL");
            CreateDomain("DM_INTEGER", "INTEGER", "");
            CreateDomain("DM_INTEGER_DEFAULT_1", "INTEGER", "DEFAULT 0 NOT NULL");
            CreateDomain("DM_KHERZ", "DOUBLE PRECISION", "");
            CreateDomain("DM_KMETR", "DOUBLE PRECISION", "");
            CreateDomain("DM_KWATT", "DOUBLE PRECISION", "");
            CreateDomain("DM_LICENSE", "VARCHAR(4)", "NOT NULL CHECK (VALUE IN ('КМ', 'РЧР', 'ВД'))");
            CreateDomain("DM_LOGIN", "VARCHAR(16)", "");
            CreateDomain("DM_MBIT", "DOUBLE PRECISION", "");
            CreateDomain("DM_METR", "INTEGER", "");
            CreateDomain("DM_MHERZ", "DOUBLE PRECISION", "");
            CreateDomain("DM_MICROSEC", "INTEGER", "");
            CreateDomain("DM_MICROWATT", "INTEGER", "");
            CreateDomain("DM_MILLIWATT", "INTEGER", "");
            CreateDomain("DM_NAMECHANNEL", "VARCHAR(4)", "");
            CreateDomain("DM_NAMEPROGRAMM", "VARCHAR(16)", "");
            CreateDomain("DM_NOTICE_TYPE", "CHAR(3)", "NOT NULL CHECK (value in ('GS1','GT1','GS2', 'GT2', 'GA1', 'DT1', 'DT2', 'DS1', 'DS2', 'GA1'))");
            CreateDomain("DM_NULLFOREIGNKEY", "INTEGER", "");
            CreateDomain("DM_OFFSETLINE", "SMALLINT", "CHECK (VALUE >= -20 AND VALUE <= 20)");
            CreateDomain("DM_OFFSETTYPE", "VARCHAR(16)", "CHECK (VALUE IN ('Normal', 'Precision', 'Synchronised', 'Unspecified'))");
            CreateDomain("DM_PHONE", "VARCHAR(16)", "");
            CreateDomain("DM_POLARIZATION", "CHAR(1)", "CHECK (VALUE IN ('V', 'H', 'M', 'U'))");
            CreateDomain("DM_RRC06_CODE", "CHAR(1)", "");
            CreateDomain("DM_RRC06_ID", "CHAR(20)", "");
            CreateDomain("DM_SECONDSOUND", "VARCHAR(8)", "CHECK (VALUE IN ('FM', 'NICAM'))");
            CreateDomain("DM_SMALLINT", "SMALLINT", "");
            CreateDomain("DM_SPECTRUM_MASK", "CHAR(1)", "");
            CreateDomain("DM_STABILITY", "VARCHAR(16)", "CHECK (VALUE IN ('RELAXED', 'NORMAL', 'PRECISION'))");
            CreateDomain("DM_SYSTEMCOLOUR", "VARCHAR(8)", "CHECK (VALUE IN ('PAL', 'SECAM', 'NTSC'))");
            CreateDomain("DM_TIME", "TIME", "");
            CreateDomain("DM_VARCHAR16", "VARCHAR(16)", "");
            CreateDomain("DM_VARCHAR256", "VARCHAR(256)", "");
            CreateDomain("DM_VARCHAR30", "VARCHAR(30)", "");
            CreateDomain("DM_VARCHAR32", "VARCHAR(32)", "");
            CreateDomain("DM_VARCHAR4", "VARCHAR(4)", "");
            CreateDomain("DM_VARCHAR512", "VARCHAR(512)", "");
            CreateDomain("DM_VARCHAR64", "VARCHAR(64)", "");
            CreateDomain("DM_VARCHAR8", "VARCHAR(8)", "");
            CreateDomain("DM_VARCHARBIG", "VARCHAR(16384)", "");
            CreateDomain("DM_WATT", "INTEGER", "");

            RunQuery("CREATE GENERATOR GEN_ACTIVEVIEW_ID");
            RunQuery("SET GENERATOR GEN_ACTIVEVIEW_ID TO 478314");
            RunQuery("CREATE GENERATOR GEN_COMMON_ID");

            RunQuery("CREATE EXCEPTION E_WRONG_COORD_DISTANCE 'Не могу определить координационное расстояние'");

            RunQuery("CREATE PROCEDURE PR_ACTIVEVIEW_SETCHANGE (\
                IN_ADMITID INTEGER,\
                IN_TYPECHANGE VARCHAR(16),\
                IN_NAMETABLE VARCHAR(32),\
                IN_NUMCHANGE INTEGER)\
            RETURNS (\
            OUT_ID INTEGER)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure PR_ACTIVEVIEW_SETCHANGE to public");

            RunQuery("CREATE PROCEDURE SP_ACTIVE_USER (\
                IN_TYPEACTIVE INTEGER)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_ACTIVE_USER to public");

            RunQuery("CREATE PROCEDURE SP_CREATE_ALLOTMENT_TX\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_CREATE_ALLOTMENT_TX to public");

            RunQuery("CREATE PROCEDURE SP_CREATE_LICENSES\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_CREATE_LICENSES to public");

            RunQuery("CREATE PROCEDURE SP_CREATE_SELECTION (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_CREATE_SELECTION to public");

            RunQuery("CREATE PROCEDURE SP_DATEPERM (\
                TXID INTEGER)\
            RETURNS (\
                DATEPERMUSEFROM_OUT TIMESTAMP,\
                DATEPERMUSETO_OUT TIMESTAMP)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_DATEPERM to public");

            RunQuery("CREATE PROCEDURE SP_DELETE_DUPLICATES\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_DELETE_DUPLICATES to public");

            RunQuery("CREATE PROCEDURE SP_FILL_DM_CHANNELS\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_FILL_DM_CHANNELS to public");

            RunQuery("CREATE PROCEDURE SP_FILL_OFFSET\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_FILL_OFFSET to public");

            RunQuery("CREATE PROCEDURE SP_GEN_ID\
            RETURNS (\
                OUT_ID INTEGER)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_GEN_ID to public");

            RunQuery("CREATE PROCEDURE SP_GET_USERID\
            RETURNS (\
                OUT_ID INTEGER)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_GET_USERID to public");

            RunQuery("CREATE PROCEDURE SP_IS_INTERFERE (\
                IN_C1 DOUBLE PRECISION,\
                IN_C2 DOUBLE PRECISION,\
                IN_B1 DOUBLE PRECISION,\
                IN_B2 DOUBLE PRECISION,\
                IN_ADJ SMALLINT,\
                IN_IMG SMALLINT)\
            RETURNS (\
                OUT_RES SMALLINT)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_IS_INTERFERE to public");

            RunQuery("CREATE PROCEDURE SP_RECALC_CARRIER_AND_BW\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_RECALC_CARRIER_AND_BW to public");

            RunQuery("CREATE PROCEDURE SP_RECALC_CARRIER_AND_BW_3\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_RECALC_CARRIER_AND_BW_3 to public");

            RunQuery("CREATE PROCEDURE SP_RUS_NUMER\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_RUS_NUMER to public");

            RunQuery("CREATE PROCEDURE SP_SELECT_TX_DISTANCE (\
                IN_LAT DOUBLE PRECISION,\
                IN_LON DOUBLE PRECISION,\
                IN_DIF DOUBLE PRECISION)\
            RETURNS (\
                ID INTEGER,\
                LATITUDE DOUBLE PRECISION,\
                LONGITUDE DOUBLE PRECISION,\
                NAMESITE VARCHAR(32),\
                AREA_ID INTEGER,\
                A_NAME VARCHAR(32),\
                A_NUMREGION VARCHAR(4),\
                DISTRICT_ID INTEGER,\
                D_NAME VARCHAR(32),\
                CITY_ID INTEGER,\
                C_NAME VARCHAR(32),\
                STREET_ID INTEGER,\
                ST_NAME VARCHAR(32),\
                ADDRESS VARCHAR(64),\
                S_HEIGHT_SEA INTEGER,\
                DISTANCE DOUBLE PRECISION)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_SELECT_TX_DISTANCE to public");

            RunQuery("CREATE PROCEDURE SP_SET_CARRIER_AND_BW (\
                IN_TX_ID INTEGER)\
            RETURNS (\
                OUT_CARRIER DOUBLE PRECISION,\
                OUT_BANDWIDTH DOUBLE PRECISION)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_SET_CARRIER_AND_BW to public");

            RunQuery("CREATE PROCEDURE SP_SET_CARRIER_AND_BW_2 (\
                IN_TX_ID INTEGER)\
            RETURNS (\
                OUT_CARRIER DOUBLE PRECISION,\
                OUT_BANDWIDTH DOUBLE PRECISION)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_SET_CARRIER_AND_BW_2 to public");

            RunQuery("CREATE PROCEDURE SP_SET_CARRIER_AND_BW_3 (\
                IN_TX_ID INTEGER,\
                IN_TX_VIDEO_CARRIER DOUBLE PRECISION,\
                IN_TX_SOUND_CARRIER DOUBLE PRECISION,\
                IN_TX_BLOCKCENTREFREQ DOUBLE PRECISION,\
                IN_TX_SYSTEMCAST_ID INTEGER,\
                IN_TX_TYPESYSTEM INTEGER,\
                IN_TX_CHANNEL_ID INTEGER)\
            RETURNS (\
                OUT_CARRIER DOUBLE PRECISION,\
                OUT_BANDWIDTH DOUBLE PRECISION)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_SET_CARRIER_AND_BW_3 to public");

            RunQuery("CREATE PROCEDURE SP_SET_CARRIER_AND_BW_OLD (\
                IN_TX_ID INTEGER)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_SET_CARRIER_AND_BW_OLD to public");

            RunQuery("CREATE PROCEDURE SP_SITE (\
                I_ID INTEGER,\
                I_TYPE INTEGER)\
            RETURNS (\
                O_STRING VARCHAR(20))\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_SITE to public");

            RunQuery("CREATE PROCEDURE SP_TX_SELECTION (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_TX_SELECTION to public");

            RunQuery("CREATE PROCEDURE SP_TX_SELECTION2 (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION)\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_TX_SELECTION2 to public");

            RunQuery("CREATE PROCEDURE SP_TX_SELECTION3 (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION,\
                OUT_STATUS DOUBLE PRECISION,\
                OUT_AREA VARCHAR(4))\
            AS\
            BEGIN\
              EXIT;\
            END");
            RunQuery("grant execute on procedure SP_TX_SELECTION3 to public");


            Table accountcondition("ACCOUNTCONDITION", this);
            accountcondition.AddField("ID","DM_IDENTY_PK",true,"");
            accountcondition.AddField("TYPECONDITION","DM_BOOLEAN",false,"");
            accountcondition.AddField("NAME","DM_VARCHAR32",false,"");
            accountcondition.AddField("CODE","DM_VARCHAR4",false,"");
            accountcondition.CreateTable();
            accountcondition.GrantAll();

            Table activeview("ACTIVEVIEW", this);
            activeview.AddField("ID","DM_IDENTY_PK",true,"");
            activeview.AddField("ADMIT_ID","DM_IDENTY_FK",true,"");
            activeview.AddField("DATECHANGE","DM_DATETIME",false,"");
            activeview.AddField("TYPECHANGE","DM_VARCHAR16",false,"");
            activeview.AddField("NAME_TABLE","DM_VARCHAR32",false,"");
            activeview.AddField("NUM_CHANGE","DM_INTEGER",false,"");
            activeview.AddField("NAME_FIELD","DM_VARCHAR256",false,"");
            activeview.CreateTable();
            activeview.GrantAll();

            Table admit("ADMIT", this);
            admit.AddField("ID","DM_IDENTY_PK",true,"");
            admit.AddField("NAME","DM_VARCHAR64",false,"");
            admit.AddField("LOGIN","DM_LOGIN",false,"");
            admit.AddField("STATUS","DM_SMALLINT",false,"");
            admit.CreateTable();
            admit.GrantAll();

            Table allotmentblockdab("ALLOTMENTBLOCKDAB", this);
            allotmentblockdab.AddField("ID","DM_IDENTY_PK",true,"");
            allotmentblockdab.AddField("IDENTIFIER","DM_VARCHAR8",false,"");
            allotmentblockdab.AddField("NAME","DM_VARCHAR8",false,"");
            allotmentblockdab.AddField("BLOCKDAB_ID","DM_IDENTY_FK",true,"");
            allotmentblockdab.AddField("ASSIGNMENTIS","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("TYPEREFERENCENETWORK","DM_SMALLINT",false,"");
            allotmentblockdab.AddField("AGREEMENTNUMBER","DM_VARCHAR256",false,"");
            allotmentblockdab.AddField("COORDINATION1","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION2","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION3","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION4","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION5","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION6","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION7","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION8","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("COORDINATION9","DM_BOOLEAN",false,"");
            allotmentblockdab.AddField("REMARKS","DM_VARCHAR256",false,"");
            allotmentblockdab.CreateTable();
            allotmentblockdab.GrantAll();

            Table analogradiosystem("ANALOGRADIOSYSTEM", this);
            analogradiosystem.AddField("ID","DM_IDENTY_PK",true,"");
            analogradiosystem.AddField("CODSYSTEM","DM_VARCHAR8",false,"");
            analogradiosystem.AddField("TYPESYSTEM","DM_VARCHAR8",false,"");
            analogradiosystem.AddField("MODULATION","DM_VARCHAR8",false,"");
            analogradiosystem.AddField("DEVIATION","DM_KHERZ",false,"");
            analogradiosystem.AddField("ENUMVAL","INTEGER",true,"");
            analogradiosystem.CreateTable();
            analogradiosystem.GrantAll();

            Table analogtelesystem("ANALOGTELESYSTEM", this);
            analogtelesystem.AddField("ID","DM_IDENTY_PK",true,"");
            analogtelesystem.AddField("NAMESYSTEM","DM_VARCHAR4",false,"");
            analogtelesystem.AddField("NUMBERLINES","DM_SMALLINT",false,"");
            analogtelesystem.AddField("CHANNELBAND","DM_MHERZ",false,"");
            analogtelesystem.AddField("VIDEOBAND","DM_MHERZ",false,"");
            analogtelesystem.AddField("SEPARATEVIDEOSOUND1","DM_MHERZ",false,"");
            analogtelesystem.AddField("VESTIGIALBAND","DM_MHERZ",false,"");
            analogtelesystem.AddField("VIDEOMODULATION","DM_VARCHAR16",false,"");
            analogtelesystem.AddField("SOUND1MODULATION","DM_VARCHAR8",false,"");
            analogtelesystem.AddField("SOUND2SYSTEM","DM_VARCHAR8",false,"");
            analogtelesystem.AddField("SEPARATEVIDEOSOUND2","DM_MHERZ",false,"");
            analogtelesystem.AddField("ENUMVAL","SMALLINT",true,"");
            analogtelesystem.AddField("FREQUENCYGRID_ID","DM_IDENTY_FK",true,"");
            analogtelesystem.CreateTable();
            analogtelesystem.GrantAll();

            Table area("AREA", this);
            area.AddField("ID","DM_IDENTY_PK",true,"");
            area.AddField("COUNTRY_ID","DM_IDENTY_FK",true,"");
            area.AddField("NAME","DM_VARCHAR32",false,"");
            area.AddField("NUMREGION","DM_VARCHAR4",false,"");
            area.CreateTable();
            area.GrantAll();

            Table arhive("ARHIVE", this);
            arhive.AddField("ID","DM_IDENTY_PK",true,"");
            arhive.AddField("ACTIVEVIEW_ID","DM_IDENTY_FK",false,"");
            arhive.AddField("NAMEFIELD","DM_VARCHAR32",false,"");
            arhive.AddField("OLDDATA_CHAR","DM_VARCHAR256",false,"");
            arhive.AddField("OLDDATA_BLOB","DM_BLOB",false,"");
            arhive.CreateTable();
            arhive.GrantAll();

            Table bank("BANK", this);
            bank.AddField("ID","DM_IDENTY_PK",true,"");
            bank.AddField("NAME","DM_VARCHAR64",false,"");
            bank.AddField("MFO","DM_VARCHAR8",false,"");
            bank.AddField("ADDRESS","DM_VARCHAR64",false,"");
            bank.CreateTable();
            bank.GrantAll();

            Table blockdab("BLOCKDAB", this);
            blockdab.AddField("ID","DM_IDENTY_PK",true,"");
            blockdab.AddField("NAME","DM_VARCHAR4",false,"");
            blockdab.AddField("CENTREFREQ","DM_MHERZ",false,"");
            blockdab.AddField("FREQFROM","DM_MHERZ",false,"");
            blockdab.AddField("FREQTO","DM_MHERZ",false,"");
            blockdab.AddField("LOWERGUARDBAND","DM_KHERZ",false,"");
            blockdab.AddField("UPPERGUARDBAND","DM_KHERZ",false,"");
            blockdab.CreateTable();
            blockdab.GrantAll();

            Table carrierguardinterval("CARRIERGUARDINTERVAL", this);
            carrierguardinterval.AddField("ID","DM_IDENTY_PK",true,"");
            carrierguardinterval.AddField("CODE","DM_VARCHAR4",false,"");
            carrierguardinterval.AddField("NUMBERCARRIER","DM_SMALLINT",false,"");
            carrierguardinterval.AddField("TIMEUSEFULINTERVAL","DM_MICROSEC",false,"");
            carrierguardinterval.AddField("FREQINTERVAL","DM_HERZ",false,"");
            carrierguardinterval.AddField("FREQBOUNDINTERVAL","DM_MHERZ",false,"");
            carrierguardinterval.AddField("TIMECURRIERGUARD","DM_MICROSEC",false,"");
            carrierguardinterval.AddField("NAMECURRIERGUARD","DM_VARCHAR4",false,"");
            carrierguardinterval.CreateTable();
            carrierguardinterval.GrantAll();

            Table chanelsretranslate("CHANELSRETRANSLATE", this);
            chanelsretranslate.AddField("ID","DM_IDENTY_PK",true,"");
            chanelsretranslate.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",true,"");
            chanelsretranslate.AddField("TYPESYSTEM","DM_BOOLEAN",false,"");
            chanelsretranslate.AddField("RX_CHANNEL","DM_VARCHAR4",false,"");
            chanelsretranslate.AddField("TX_CHANNEL","DM_VARCHAR4",false,"");
            chanelsretranslate.AddField("RX_FREQUENCY","DM_MHERZ",false,"");
            chanelsretranslate.AddField("TX_FREQUENCY","DM_MHERZ",false,"");
            chanelsretranslate.AddField("TYPERECEIVE_ID","DM_IDENTY_FK",true,"");
            chanelsretranslate.AddField("NAMEPROGRAMM","DM_NAMEPROGRAMM",false,"");
            chanelsretranslate.CreateTable();
            chanelsretranslate.GrantAll();

            Table channels("CHANNELS", this);
            channels.AddField("ID","DM_IDENTY_PK",true,"");
            channels.AddField("FREQUENCYGRID_ID","DM_IDENTY_FK",true,"");
            channels.AddField("NAMECHANNEL","DM_VARCHAR4",false,"");
            channels.AddField("FREQFROM","DM_MHERZ",false,"");
            channels.AddField("FREQTO","DM_MHERZ",false,"");
            channels.AddField("FREQCARRIERVISION","DM_MHERZ",false,"");
            channels.AddField("FREQCARRIERSOUND","DM_MHERZ",false,"");
            channels.AddField("FMSOUNDCARRIERSECOND","DM_MHERZ",false,"");
            channels.AddField("FREQCARRIERNICAM","DM_MHERZ",false,"");
            channels.CreateTable();
            channels.GrantAll();

            Table city("CITY", this);
            city.AddField("ID","DM_IDENTY_PK",true,"");
            city.AddField("DISTRICT_ID","DM_IDENTY_FK",true,"");
            city.AddField("NAME","DM_VARCHAR32",false,"");
            city.AddField("AREA_ID","INTEGER",true,"");
            city.AddField("COUNTRY_ID","INTEGER",true,"");
            city.CreateTable();
            city.GrantAll();

            Table coorddistance("COORDDISTANCE", this);
            coorddistance.AddField("ID","DM_IDENTY_PK",true,"");
            coorddistance.AddField("SYSTEMCAST_ID","DM_IDENTY_FK",true,"");
            coorddistance.AddField("EFFECTRADIATEPOWER","DM_MILLIWATT",false,"");
            coorddistance.AddField("HEIGHTANTENNA","DM_HEIGHTMETR",false,"");
            coorddistance.AddField("OVERLAND","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("OVERCOLDSEA","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("OVERWARMSEA","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("GENERALLYSEA20","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("GENERALLYSEA40","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("GENERALLYSEA60","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("GENERALLYSEA80","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("GENERALLYSEA100","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("MEDITERRANEANSEA20","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("MEDITERRANEANSEA40","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("MEDITERRANEANSEA60","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("MEDITERRANEANSEA80","DM_DISTANCEKMETR",false,"");
            coorddistance.AddField("MEDITERRANEANSEA100","DM_DISTANCEKMETR",false,"");
            coorddistance.CreateTable();
            coorddistance.GrantAll();

            Table coordination("COORDINATION", this);
            coordination.AddField("ID","DM_IDENTY_FK",false,"");
            coordination.AddField("TRANSMITTER_ID","DM_IDENTY_FK",false,"");
            coordination.AddField("TELECOMORG_ID","DM_IDENTY_FK",false,"");
            coordination.AddField("ACCOUNTCONDITION_ID","DM_IDENTY_FK",false,"");
            coordination.CreateTable();
            coordination.GrantAll();

            Table coordpoints("COORDPOINTS", this);
            coordpoints.AddField("ID","DM_IDENTY_PK",true,"");
            coordpoints.AddField("NUMBOUND","DM_INTEGER",false,"");
            coordpoints.AddField("LATITUDE","DM_GEOPOINT",false,"");
            coordpoints.AddField("LONGITUDE","DM_GEOPOINT",false,"");
            coordpoints.CreateTable();
            coordpoints.GrantAll();

            Table country("COUNTRY", this);
            country.AddField("ID","DM_IDENTY_PK",true,"");
            country.AddField("NAME","DM_VARCHAR32",false,"");
            country.AddField("DESCRIPTION","DM_VARCHAR32",false,"");
            country.AddField("CODE","DM_VARCHAR4",false,"");
            country.AddField("DEF_TVA_SYS","DM_IDENTY_FK DEFAULT 0",true,"");
            country.AddField("DEF_COLOR","DM_SYSTEMCOLOUR DEFAULT 'SECAM'",false,"");
            country.AddField("DEF_FM_SYS","DM_IDENTY_FK DEFAULT 0",true,"");
            country.AddField("DEF_DVB_SYS","DM_IDENTY_FK DEFAULT 0",true,"");
            country.CreateTable();
            country.GrantAll();

            Table countrycoordination("COUNTRYCOORDINATION", this);
            countrycoordination.AddField("ID","DM_IDENTY_PK",true,"");
            countrycoordination.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",true,"");
            countrycoordination.AddField("COUNTRY_ID","DM_IDENTY_FK",true,"");
            countrycoordination.AddField("COORDINATIONIS","DM_BOOLEAN",false,"");
            countrycoordination.AddField("LETTERS_ID","DM_INTEGER",false,"");
            countrycoordination.CreateTable();
            countrycoordination.GrantAll();

            Table countrypoints("COUNTRYPOINTS", this);
            countrypoints.AddField("COUNTRY_ID","DM_IDENTY_FK",true,"");
            countrypoints.AddField("NUMBOUND","DM_INTEGER",true,"");
            countrypoints.AddField("CODE","DM_VARCHAR8",false,"");
            countrypoints.CreateTable();
            countrypoints.GrantAll();

            Table databasesection("DATABASESECTION", this);
            databasesection.AddField("ID","DM_IDENTY_PK",true,"");
            databasesection.AddField("SECTIONNAME","DM_VARCHAR16",false,"");
            databasesection.AddField("CAN_MODIFY","DM_BOOLEAN",true,"");
            databasesection.AddField("CAN_DELETE","DM_BOOLEAN",true,"");
            databasesection.CreateTable();
            databasesection.GrantAll();

            Table dig_allot_cntr_lnk("DIG_ALLOT_CNTR_LNK", this);
            dig_allot_cntr_lnk.AddField("ALLOT_ID","DM_IDENTY_FK",false,"");
            dig_allot_cntr_lnk.AddField("CNTR_ID","DM_IDENTY_FK",false,"");
            dig_allot_cntr_lnk.CreateTable();
            dig_allot_cntr_lnk.GrantAll();

            Table dig_allot_zone("DIG_ALLOT_ZONE", this);
            dig_allot_zone.AddField("ID","DM_IDENTY_PK",false,"");
            dig_allot_zone.AddField("ALLOT_ID","DM_IDENTY_FK",false,"");
            dig_allot_zone.AddField("EMIN","DM_DBELL",false,"");
            dig_allot_zone.AddField("NOTE","DM_VARCHAR64",false,"");
            dig_allot_zone.CreateTable();
            dig_allot_zone.GrantAll();

            Table dig_allot_zone_point("DIG_ALLOT_ZONE_POINT", this);
            dig_allot_zone_point.AddField("ZONE_ID","DM_IDENTY_FK",false,"");
            dig_allot_zone_point.AddField("POINT_NO","DM_INTEGER",true,"");
            dig_allot_zone_point.AddField("DIST","DM_DISTANCEKMETR",false,"");
            dig_allot_zone_point.CreateTable();
            dig_allot_zone_point.GrantAll();

            Table dig_allotment("DIG_ALLOTMENT", this);
            dig_allotment.AddField("ID","DM_IDENTY_PK",false,"");
            dig_allotment.AddField("ADM_ID","DM_INTEGER",false,"");
            dig_allotment.AddField("NOTICE_TYPE","DM_NOTICE_TYPE",true,"");
            dig_allotment.AddField("IS_PUB_REQ","VARCHAR(5)",false,"");
            dig_allotment.AddField("ADM_REF_ID","DM_RRC06_ID",false,"");
            dig_allotment.AddField("PLAN_ENTRY","DM_INTEGER",true,"");
            dig_allotment.AddField("SFN_ID_FK","DM_INTEGER",false,"");
            dig_allotment.AddField("FREQ_ASSIGN","DM_MHERZ",true,"");
            dig_allotment.AddField("OFFSET","DM_INTEGER",false,"");
            dig_allotment.AddField("D_EXPIRY","DATE",false,"");
            dig_allotment.AddField("ALLOT_NAME","DM_VARCHAR32",true,"");
            dig_allotment.AddField("CTRY","DM_COUNTRYCODE",true,"");
            dig_allotment.AddField("GEO_AREA","DM_COUNTRYCODE",false,"");
            dig_allotment.AddField("NB_SUB_AREAS","DM_SMALLINT",true,"");
            dig_allotment.AddField("REF_PLAN_CFG","CHAR(4)",true,"");
            dig_allotment.AddField("TYP_REF_NETWK","CHAR(3)",false,"");
            dig_allotment.AddField("SPECT_MASK","DM_SPECTRUM_MASK",false,"");
            dig_allotment.AddField("POLAR","DM_POLARIZATION",true,"");
            dig_allotment.AddField("BLOCKDAB_ID","DM_INTEGER",false,"");
            dig_allotment.AddField("CHANNEL_ID","DM_INTEGER",false,"");
            dig_allotment.AddField("REMARKS1","DM_VARCHAR512",false,"");
            dig_allotment.AddField("REMARKS2","DM_VARCHAR512",false,"");
            dig_allotment.AddField("REMARKS3","DM_VARCHAR512",false,"");
            dig_allotment.AddField("DB_SECTION_ID","DM_IDENTY_FK",true,"");
            dig_allotment.CreateTable();
            dig_allotment.GrantAll();

            Table dig_contour("DIG_CONTOUR", this);
            dig_contour.AddField("ID","DM_IDENTY_PK",false,"");
            dig_contour.AddField("ADM_ID","DM_IDENTY_FK",false,"");
            dig_contour.AddField("CTRY","DM_COUNTRYCODE",false,"");
            dig_contour.AddField("CONTOUR_ID","DM_INTEGER",true,"");
            dig_contour.AddField("NB_TEST_PTS","DM_INTEGER",true,"");
            dig_contour.AddField("DB_SECTION_ID","DM_IDENTY_FK",true,"");
            dig_contour.CreateTable();
            dig_contour.GrantAll();

            Table dig_subareapoint("DIG_SUBAREAPOINT", this);
            dig_subareapoint.AddField("CONTOUR_ID","DM_IDENTY_FK",true,"");
            dig_subareapoint.AddField("POINT_NO","DM_SMALLINT",true,"");
            dig_subareapoint.AddField("LAT","DM_DOUBLEPRECISION",true,"");
            dig_subareapoint.AddField("LON","DM_DOUBLEPRECISION",true,"");
            dig_subareapoint.CreateTable();
            dig_subareapoint.GrantAll();

            Table digitaltelesystem("DIGITALTELESYSTEM", this);
            digitaltelesystem.AddField("ID","DM_IDENTY_PK",true,"");
            digitaltelesystem.AddField("NAMESYSTEM","DM_VARCHAR4",false,"");
            digitaltelesystem.AddField("MODULATION","DM_VARCHAR8",false,"");
            digitaltelesystem.AddField("CODERATE","DM_VARCHAR4",false,"");
            digitaltelesystem.AddField("GAUSSIANCHANNEL","DM_DBELL",false,"");
            digitaltelesystem.AddField("RICEANCHANNEL","DM_DBELL",false,"");
            digitaltelesystem.AddField("RAYLEIGHCHANNEL","DM_DBELL",false,"");
            digitaltelesystem.AddField("NETBITRATEGUARD4","DM_MBIT",false,"");
            digitaltelesystem.AddField("NETBITRATEGUARD8","DM_MBIT",false,"");
            digitaltelesystem.AddField("NETBITRATEGUARD16","DM_MBIT",false,"");
            digitaltelesystem.AddField("NETBITRATEGUARD32","DM_MBIT",false,"");
            digitaltelesystem.AddField("ENUMVAL","INTEGER",true,"");
            digitaltelesystem.CreateTable();
            digitaltelesystem.GrantAll();

            Table district("DISTRICT", this);
            district.AddField("ID","DM_IDENTY_PK",true,"");
            district.AddField("AREA_ID","DM_IDENTY_FK",true,"");
            district.AddField("NAME","DM_VARCHAR32",false,"");
            district.CreateTable();
            district.GrantAll();

            Table document("DOCUMENT", this);
            document.AddField("ID","DM_IDENTY_PK",true,"");
            document.AddField("CODE","DM_VARCHAR4",false,"");
            document.AddField("NAME","DM_VARCHAR64",false,"");
            document.AddField("TEMPLATE","DM_BLOB",false,"");
            document.AddField("TEMPLATE2","DM_BLOB",false,"");
            document.AddField("TTYPE","DM_VARCHAR16",false,"");
            document.CreateTable();
            document.GrantAll();

            Table dplan_channel("DPLAN_CHANNEL", this);
            dplan_channel.AddField("PLANID","DM_IDENTY_FK",true,"");
            dplan_channel.AddField("CHANNEL","DM_IDENTY_FK",true,"");
            dplan_channel.AddField("COLOR","DM_INTEGER",false,"");
            dplan_channel.AddField("MAIN_SELECTION","DM_INTEGER",false,"");
            dplan_channel.CreateTable();
            dplan_channel.GrantAll();

            Table dplan_checkpoints("DPLAN_CHECKPOINTS", this);
            dplan_checkpoints.AddField("TXID","DM_IDENTY_FK",true,"");
            dplan_checkpoints.AddField("POINT_NO","DM_SMALLINT",true,"");
            dplan_checkpoints.AddField("LONGITUTE","DM_GEOPOINT",true,"");
            dplan_checkpoints.AddField("LATITUDE","DM_GEOPOINT",true,"");
            dplan_checkpoints.CreateTable();
            dplan_checkpoints.GrantAll();

            Table dplan_coord("DPLAN_COORD", this);
            dplan_coord.AddField("ZONE","DM_IDENTY_FK",true,"");
            dplan_coord.AddField("NOM","DM_INTEGER",true,"");
            dplan_coord.AddField("LON","DM_DOUBLEPRECISION",true,"");
            dplan_coord.AddField("LAT","DM_DOUBLEPRECISION",true,"");
            dplan_coord.CreateTable();
            dplan_coord.GrantAll();

            Table dplan_fg("DPLAN_FG", this);
            dplan_fg.AddField("PLANID","DM_IDENTY_FK",true,"");
            dplan_fg.AddField("FG","DM_IDENTY_FK",true,"");
            dplan_fg.CreateTable();
            dplan_fg.GrantAll();

            Table dplan_l_channel("DPLAN_L_CHANNEL", this);
            dplan_l_channel.AddField("LAYER","DM_IDENTY_FK",true,"");
            dplan_l_channel.AddField("CHANNEL","DM_IDENTY_FK",true,"");
            dplan_l_channel.CreateTable();
            dplan_l_channel.GrantAll();

            Table dplan_layer("DPLAN_LAYER", this);
            dplan_layer.AddField("ID","DM_IDENTY_PK",true,"");
            dplan_layer.AddField("NAME","DM_VARCHAR64",false,"");
            dplan_layer.AddField("PLANID","DM_IDENTY_FK",false,"");
            dplan_layer.AddField("DIAPASON","SMALLINT",false,"");
            dplan_layer.AddField("CHANNELBANDWIDTH","DM_SMALLINT",false,"");
            dplan_layer.CreateTable();
            dplan_layer.GrantAll();

            Table dplan_plan("DPLAN_PLAN", this);
            dplan_plan.AddField("ID","DM_IDENTY_PK",true,"");
            dplan_plan.AddField("NAME","DM_VARCHAR64",false,"");
            dplan_plan.AddField("SAMEZONES","DM_BOOLEAN",false,"");
            dplan_plan.AddField("COMMENT","DM_VARCHAR256",false,"");
            dplan_plan.AddField("USERID","DM_IDENTY_FK",false,"");
            dplan_plan.AddField("R_DIST","DM_INTEGER default 0",false,"");
            dplan_plan.CreateTable();
            dplan_plan.GrantAll();

            Table dplan_plan_selection("DPLAN_PLAN_SELECTION", this);
            dplan_plan_selection.AddField("SELECTION","DM_IDENTY_FK",false,"");
            dplan_plan_selection.AddField("CHANNEL","DM_IDENTY_FK",false,"");
            dplan_plan_selection.CreateTable();
            dplan_plan_selection.GrantAll();

            Table dplan_prohib_zones("DPLAN_PROHIB_ZONES", this);
            dplan_prohib_zones.AddField("ZONE","DM_IDENTY_FK",false,"");
            dplan_prohib_zones.AddField("PROHIB_ZONE","DM_IDENTY_FK",false,"");
            dplan_prohib_zones.AddField("DISTANCE","DM_INTEGER",false,"");
            dplan_prohib_zones.AddField("SEAPERCENT","SMALLINT",false,"");
            dplan_prohib_zones.AddField("X1","DM_DOUBLEPRECISION",false,"");
            dplan_prohib_zones.AddField("X2","DM_DOUBLEPRECISION",false,"");
            dplan_prohib_zones.AddField("Y1","DM_DOUBLEPRECISION",false,"");
            dplan_prohib_zones.AddField("Y2","DM_DOUBLEPRECISION",false,"");
            dplan_prohib_zones.CreateTable();
            dplan_prohib_zones.GrantAll();

            Table dplan_sel_trans("DPLAN_SEL_TRANS", this);
            dplan_sel_trans.AddField("SELECTION","DM_IDENTY_FK",false,"");
            dplan_sel_trans.AddField("TRANSMITTER","DM_IDENTY_FK",false,"");
            dplan_sel_trans.AddField("STATE","DM_SMALLINT",false,"");
            dplan_sel_trans.CreateTable();
            dplan_sel_trans.GrantAll();

            Table dplan_selection("DPLAN_SELECTION", this);
            dplan_selection.AddField("ID","DM_IDENTY_PK",true,"");
            dplan_selection.AddField("NAME","DM_VARCHAR64",false,"");
            dplan_selection.AddField("DESCRIPTION","DM_VARCHAR256",false,"");
            dplan_selection.AddField("PLANID","DM_IDENTY_FK",false,"");
            dplan_selection.AddField("CHANNEL","DM_IDENTY_FK",false,"");
            dplan_selection.AddField("TX_NUM","DM_INTEGER DEFAULT 0",false,"");
            dplan_selection.CreateTable();
            dplan_selection.GrantAll();

            Table dplan_tx_gr_link("DPLAN_TX_GR_LINK", this);
            dplan_tx_gr_link.AddField("TXID","DM_IDENTY_FK",false,"");
            dplan_tx_gr_link.AddField("GROUPID","DM_IDENTY_FK",false,"");
            dplan_tx_gr_link.AddField("ORDERNO","DM_IDENTY_PK DEFAULT 0",true,"");
            dplan_tx_gr_link.CreateTable();
            dplan_tx_gr_link.GrantAll();

            Table dplan_tx_group("DPLAN_TX_GROUP", this);
            dplan_tx_group.AddField("ID","DM_IDENTY_PK",true,"");
            dplan_tx_group.AddField("NAME","DM_VARCHAR16",true,"");
            dplan_tx_group.AddField("PARENTID","DM_INTEGER DEFAULT 0",true,"");
            dplan_tx_group.AddField("DESCRIPTION","DM_VARCHAR256",false,"");
            dplan_tx_group.AddField("PLANID","DM_IDENTY_FK",true,"");
            dplan_tx_group.CreateTable();
            dplan_tx_group.GrantAll();

            Table dplan_z_channel("DPLAN_Z_CHANNEL", this);
            dplan_z_channel.AddField("ZONE","DM_IDENTY_PK",false,"");
            dplan_z_channel.AddField("CHANNEL","DM_IDENTY_PK",false,"");
            dplan_z_channel.AddField("STATE","DM_SMALLINT",true,"");
            dplan_z_channel.CreateTable();
            dplan_z_channel.GrantAll();

            Table dplan_zone("DPLAN_ZONE", this);
            dplan_zone.AddField("ID","DM_IDENTY_PK",true,"");
            dplan_zone.AddField("NAME","DM_VARCHAR64",false,"");
            dplan_zone.AddField("LAYER","DM_IDENTY_FK",false,"");
            dplan_zone.AddField("CHANNEL","DM_IDENTY_FK",false,"");
            dplan_zone.AddField("R_DIST","DM_INTEGER default 0",false,"");
            dplan_zone.AddField("R_DIST_OVERIDDEN","DM_BOOLEAN",false,"");
            dplan_zone.AddField("NOTE","DM_VARCHAR256",false,"");
            dplan_zone.CreateTable();
            dplan_zone.GrantAll();

            Table equipment("EQUIPMENT", this);
            equipment.AddField("ID","DM_IDENTY_PK",true,"");
            equipment.AddField("TYPEEQUIPMENT","DM_VARCHAR64",false,"");
            equipment.AddField("NAME","DM_VARCHAR32",false,"");
            equipment.AddField("MANUFACTURE","DM_VARCHAR32",false,"");
            equipment.CreateTable();
            equipment.GrantAll();

            Table frequencygrid("FREQUENCYGRID", this);
            frequencygrid.AddField("ID","DM_IDENTY_PK",true,"");
            frequencygrid.AddField("NAME","DM_VARCHAR4",true,"");
            frequencygrid.AddField("DESCRIPTION","DM_VARCHAR256",false,"");
            frequencygrid.CreateTable();
            frequencygrid.GrantAll();

            Table imp_rests("IMP_RESTS", this);
            imp_rests.AddField("CODE","INTEGER",true,"");
            imp_rests.AddField("NAME","VARCHAR(34)",false,"");
            imp_rests.AddField("QUANTITY","NUMERIC(13,0) default 0",false,"");
            imp_rests.AddField("PRICE","NUMERIC(13,1) default 0",false,"");
            imp_rests.AddField("PROCESSED","SMALLINT DEFAULT 0",false,"");
            imp_rests.AddField("NOTFOUND","SMALLINT DEFAULT 0",false,"");
            imp_rests.CreateTable();
            imp_rests.GrantAll();

            Table letters("LETTERS", this);
            letters.AddField("ID","DM_IDENTY_PK",true,"");
            letters.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",true,"");
            letters.AddField("TYPELETTER","DM_BOOLEAN",false,"");
            letters.AddField("LETTERS_ID","DM_INTEGER",false,"");
            letters.AddField("ACCOUNTCONDITION_ID","DM_IDENTY_FK",true,"");
            letters.AddField("TELECOMORGANIZATION_ID","DM_IDENTY_FK",true,"");
            letters.AddField("DOCUMENT_ID","DM_IDENTY_FK",false,"");
            letters.AddField("CREATEDATEIN","DM_DATE",false,"");
            letters.AddField("NUMIN","DM_INTEGER",false,"");
            letters.AddField("CREATEDATEOUT","DM_DATE",false,"");
            letters.AddField("NUMOUT","DM_INTEGER",false,"");
            letters.AddField("ANSWERDATE","DM_DATE",false,"");
            letters.AddField("ANSWERIS","DM_BOOLEAN",false,"");
            letters.AddField("COPYLETTER","DM_BLOB",false,"");
            letters.CreateTable();
            letters.GrantAll();

            Table license("LICENSE", this);
            license.AddField("ID","DM_IDENTY_PK",true,"");
            license.AddField("OWNER_ID","DM_IDENTY_FK",true,"");
            license.AddField("CODE","DM_IDENTY_FK",false,"");
            license.AddField("NUMLICENSE","DM_VARCHAR64",false,"");
            license.AddField("DATEFROM","DM_DATE",false,"");
            license.AddField("DATETO","DM_DATE",false,"");
            license.AddField("TIMEFROM","DM_TIME",false,"");
            license.AddField("TIMETO","DM_TIME",false,"");
            license.CreateTable();
            license.GrantAll();

            Table minstrengthfield("MINSTRENGTHFIELD", this);
            minstrengthfield.AddField("ID","DM_IDENTY_PK",true,"");
            minstrengthfield.AddField("SYSTEMCAST_ID","DM_IDENTY_FK",false,"");
            minstrengthfield.AddField("TYPESERVICE","DM_SMALLINT",false,"");
            minstrengthfield.AddField("TYPEAREA","DM_SMALLINT",false,"");
            minstrengthfield.AddField("MINFIELDSTENGTH","DM_DBELL",false,"");
            minstrengthfield.CreateTable();
            minstrengthfield.GrantAll();

            Table offsetcarryfreqtva("OFFSETCARRYFREQTVA", this);
            offsetcarryfreqtva.AddField("ID","DM_IDENTY_PK",true,"");
            offsetcarryfreqtva.AddField("CODEOFFSET","DM_VARCHAR8",false,"");
            offsetcarryfreqtva.AddField("OFFSETLINES","DM_OFFSETLINE",false,"");
            offsetcarryfreqtva.AddField("OFFSET","DM_HERZ",false,"");
            offsetcarryfreqtva.CreateTable();
            offsetcarryfreqtva.GrantAll();

            Table owner("OWNER", this);
            owner.AddField("ID","DM_IDENTY_PK",true,"");
            owner.AddField("NAMEORGANIZATION","DM_VARCHAR256",false,"");
            owner.AddField("ADDRESSJURE","DM_ADDRESS",false,"");
            owner.AddField("ADDRESSPHYSICAL","DM_ADDRESS",false,"");
            owner.AddField("NUMIDENTYCOD","DM_VARCHAR16",false,"");
            owner.AddField("NUMNDS","DM_VARCHAR16",false,"");
            owner.AddField("TYPEFINANCE","DM_SMALLINT",false,"");
            owner.AddField("NAMEBOSS","DM_VARCHAR64",false,"");
            owner.AddField("PHONE","DM_PHONE",false,"");
            owner.AddField("FAX","DM_VARCHAR16",false,"");
            owner.AddField("MAIL","DM_VARCHAR32",false,"");
            owner.AddField("NUMSETTLEMENTACCOUNT","DM_VARCHAR16",false,"");
            owner.AddField("NUMIDENTYCODACCOUNT","DM_VARCHAR16",false,"");
            owner.AddField("BANK_ID","DM_IDENTY_FK",false,"");
            owner.CreateTable();
            owner.GrantAll();

            Table protectratioesdab("PROTECTRATIOESDAB", this);
            protectratioesdab.AddField("ID","DM_IDENTY_PK",true,"");
            protectratioesdab.AddField("RADIOSERVICE_ID","DM_IDENTY_FK",true,"");
            protectratioesdab.AddField("FREQSPACING","DM_MHERZ",false,"");
            protectratioesdab.AddField("PROTECTRATIO","DM_DBELL",false,"");
            protectratioesdab.CreateTable();
            protectratioesdab.GrantAll();

            Table protectratioesdvb("PROTECTRATIOESDVB", this);
            protectratioesdvb.AddField("ID","DM_IDENTY_PK",true,"");
            protectratioesdvb.AddField("TYPESYSTEM","DM_INTEGER",true,"");
            protectratioesdvb.AddField("BANDWIDTH","DM_MHERZ",false,"");
            protectratioesdvb.AddField("DIGITALTELESYSTEM_ID","DM_INTEGER",false,"");
            protectratioesdvb.AddField("CHANNEL","DM_SMALLINT",false,"");
            protectratioesdvb.AddField("FREQSPACING","DM_MHERZ",false,"");
            protectratioesdvb.AddField("ANALOGTELESYSTEM_ID","DM_INTEGER",false,"");
            protectratioesdvb.AddField("SYSTEMCOLOUR","DM_SYSTEMCOLOUR",true,"");
            protectratioesdvb.AddField("PROTECTRATIO","DM_DBELL",false,"");
            protectratioesdvb.CreateTable();
            protectratioesdvb.GrantAll();

            Table protectratioestva("PROTECTRATIOESTVA", this);
            protectratioestva.AddField("ID","DM_IDENTY_PK",true,"");
            protectratioestva.AddField("WANTEDSYSTEM","DM_INTEGER",false,"");
            protectratioestva.AddField("WANTEDCOLOUR","DM_SYSTEMCOLOUR",true,"");
            protectratioestva.AddField("UNWANTEDSYSTEM","DM_INTEGER",false,"");
            protectratioestva.AddField("UNWANTEDCOLOUR","DM_SYSTEMCOLOUR",false,"");
            protectratioestva.AddField("CHANNEL","DM_INTEGER",false,"");
            protectratioestva.AddField("FREQSPACING","DM_MHERZ",false,"");
            protectratioestva.AddField("BANDWIDTH","DM_MHERZ",false,"");
            protectratioestva.AddField("OFFSETLINE","DM_OFFSETLINE",false,"");
            protectratioestva.AddField("CLASSWAVE","DM_CLASSWAVE",false,"");
            protectratioestva.AddField("CONTININTERFERENCE","DM_DBELL",false,"");
            protectratioestva.AddField("TROPOSPHERINTERFERENCE","DM_DBELL",false,"");
            protectratioestva.CreateTable();
            protectratioestva.GrantAll();

            Table protectratioesvhfsb("PROTECTRATIOESVHFSB", this);
            protectratioesvhfsb.AddField("ID","DM_IDENTY_PK",false,"");
            protectratioesvhfsb.AddField("TYPESYSTEM","DM_INTEGER",false,"");
            protectratioesvhfsb.AddField("MONOSTEREO","DM_INTEGER",false,"");
            protectratioesvhfsb.AddField("FREQSPACING","DM_KHERZ",false,"");
            protectratioesvhfsb.AddField("DEVIATION1","DM_KHERZ",false,"");
            protectratioesvhfsb.AddField("DEVIATION2","DM_KHERZ",false,"");
            protectratioesvhfsb.AddField("STEADYINTERFERENCE","DM_DBELL",false,"");
            protectratioesvhfsb.AddField("TROPOSPHERINTERFERENCE","DM_DBELL",false,"");
            protectratioesvhfsb.CreateTable();
            protectratioesvhfsb.GrantAll();

            Table radioservice("RADIOSERVICE", this);
            radioservice.AddField("ID","DM_IDENTY_PK",true,"");
            radioservice.AddField("NAME","DM_VARCHAR64",false,"");
            radioservice.AddField("DESCRIPTION","DM_VARCHAR64",false,"");
            radioservice.AddField("CODE","DM_VARCHAR4",false,"");
            radioservice.AddField("FREQFROM","DM_MHERZ",false,"");
            radioservice.AddField("FREQTO","DM_MHERZ",false,"");
            radioservice.CreateTable();
            radioservice.GrantAll();

            Table sel_area("SEL_AREA", this);
            sel_area.AddField("SELECTION","DM_IDENTY_FK",false,"");
            sel_area.AddField("AREA","DM_IDENTY_FK",false,"");
            sel_area.CreateTable();
            sel_area.GrantAll();

            Table sel_condition("SEL_CONDITION", this);
            sel_condition.AddField("SELECTION","DM_IDENTY_FK",false,"");
            sel_condition.AddField("CONDITION","DM_IDENTY_FK",false,"");
            sel_condition.CreateTable();
            sel_condition.GrantAll();

            Table sel_frequency("SEL_FREQUENCY", this);
            sel_frequency.AddField("SELECTION","DM_IDENTY_FK",false,"");
            sel_frequency.AddField("FREQUENCY","DM_MHERZ",false,"");
            sel_frequency.CreateTable();
            sel_frequency.GrantAll();

            Table selectedtransmitters("SELECTEDTRANSMITTERS", this);
            selectedtransmitters.AddField("SELECTIONS_ID","DM_IDENTY_FK",false,"");
            selectedtransmitters.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",false,"");
            selectedtransmitters.AddField("USED_IN_CALC","DM_BOOLEAN",false,"");
            selectedtransmitters.AddField("DISTANCE","DM_KMETR",false,"");
            selectedtransmitters.AddField("SORTINDEX","INTEGER",false,"");
            selectedtransmitters.AddField("RESULT","BLOB SUB_TYPE 0 SEGMENT SIZE 80",false,"");
            selectedtransmitters.AddField("E_WANT","DM_DBELL",false,"");
            selectedtransmitters.AddField("E_UNWANT","DM_DBELL",false,"");
            selectedtransmitters.AddField("AZIMUTH","DM_ANGLE",false,"");
            selectedtransmitters.AddField("ZONE_OVERLAPPING","DM_BOOLEAN",false,"");
            selectedtransmitters.CreateTable();
            selectedtransmitters.GrantAll();

            Table selections("SELECTIONS", this);
            selections.AddField("ID","DM_IDENTY_PK",true,"");
            selections.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",true,"");
            selections.AddField("NAMEQUERIES","DM_VARCHAR64",false,"");
            selections.AddField("RESULT","DM_BLOB",false,"");
            selections.AddField("CREATEDATE","DM_DATE",false,"");
            selections.AddField("CHANGEDATE","DM_DATE",false,"");
            selections.AddField("NAMECALCMODEL","DM_VARCHAR64",false,"");
            selections.AddField("USERID","DM_INTEGER",true,"");
            selections.AddField("TYPERESULT","DM_VARCHAR64",false,"");
            selections.AddField("RX_ANT_HEIGHT","DM_METR",false,"");
            selections.AddField("FREQUENCY","DM_MHERZ",false,"");
            selections.AddField("SELTYPE","CHAR(1) DEFAULT 'E'",false,"");
            selections.AddField("CHANNEL_ID","DM_IDENTY_FK DEFAULT 0",true,"");
            selections.AddField("MAX_WANT_IDX","INTEGER",false,"");
            selections.AddField("MAX_UNWANT_IDX","INTEGER",false,"");
            selections.CreateTable();
            selections.GrantAll();

            Table stand("STAND", this);
            stand.AddField("ID","DM_IDENTY_PK",false,"");
            stand.AddField("NAMESITE","DM_VARCHAR32",false,"");
            stand.AddField("NAMESITE_ENG","DM_VARCHAR32",false,"");
            stand.AddField("LATITUDE","DM_GEOPOINT",false,"");
            stand.AddField("LONGITUDE","DM_GEOPOINT",false,"");
            stand.AddField("HEIGHT_SEA","DM_METR",false,"");
            stand.AddField("MAX_OBST","DM_DOUBLEPRECISION",false,"");
            stand.AddField("MAX_USE","DM_DOUBLEPRECISION",false,"");
            stand.AddField("VHF_IS","DM_BOOLEAN",false,"");
            stand.AddField("AREA_ID","DM_IDENTY_FK",false,"");
            stand.AddField("DISTRICT_ID","DM_IDENTY_FK",true,"");
            stand.AddField("CITY_ID","DM_IDENTY_FK",true,"");
            stand.AddField("ADDRESS","DM_ADDRESS",true,"");
            stand.AddField("STREET_ID","INTEGER",true,"");
            stand.CreateTable();
            stand.GrantAll();

            Table street("STREET", this);
            street.AddField("ID","DM_IDENTY_PK",true,"");
            street.AddField("CITY_ID","DM_IDENTY_FK",true,"");
            street.AddField("NAME","DM_VARCHAR32",false,"");
            street.CreateTable();
            street.GrantAll();

            Table synhrofreqnet("SYNHROFREQNET", this);
            synhrofreqnet.AddField("ID","DM_IDENTY_PK",true,"");
            synhrofreqnet.AddField("SYNHRONETID","DM_VARCHAR30",false,"");
            synhrofreqnet.AddField("TYPESYNHRONET_ID","DM_IDENTY_FK",false,"");
            synhrofreqnet.CreateTable();
            synhrofreqnet.GrantAll();

            Table systemcast("SYSTEMCAST", this);
            systemcast.AddField("ID","DM_IDENTY_PK",true,"");
            systemcast.AddField("CODE","DM_VARCHAR4",false,"");
            systemcast.AddField("DESCRIPTION","DM_VARCHAR32",false,"");
            systemcast.AddField("TYPESYSTEM","DM_SMALLINT",false,"");
            systemcast.AddField("CLASSWAVE","DM_CLASSWAVE",false,"");
            systemcast.AddField("FREQFROM","DM_MHERZ",false,"");
            systemcast.AddField("FREQTO","DM_MHERZ",false,"");
            systemcast.AddField("NUMDIAPASON","DM_SMALLINT",false,"");
            systemcast.AddField("RELATIONNAME","DM_VARCHAR64",true,"");
            systemcast.AddField("ENUMVAL","SMALLINT",true,"");
            systemcast.CreateTable();
            systemcast.GrantAll();

            Table telecomorganization("TELECOMORGANIZATION", this);
            telecomorganization.AddField("ID","DM_IDENTY_PK",true,"");
            telecomorganization.AddField("CODE","DM_VARCHAR4",false,"");
            telecomorganization.AddField("NAME","DM_VARCHAR32",false,"");
            telecomorganization.AddField("ADDRESS","DM_VARCHAR64",false,"");
            telecomorganization.AddField("MAIL","DM_VARCHAR32",false,"");
            telecomorganization.AddField("PHONE","DM_PHONE",false,"");
            telecomorganization.AddField("COUNTRY_ID","DM_IDENTY_FK",true,"");
            telecomorganization.AddField("COORDDOCUMENT","DM_COORDDOCUMENT",false,"");
            telecomorganization.CreateTable();
            telecomorganization.GrantAll();

            Table testpoints("TESTPOINTS", this);
            testpoints.AddField("ID","DM_IDENTY_PK",true,"");
            testpoints.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",true,"");
            testpoints.AddField("NAME","DM_VARCHAR16",false,"");
            testpoints.AddField("LATITUDE","DM_GEOPOINT",true,"");
            testpoints.AddField("LONGITUDE","DM_GEOPOINT",true,"");
            testpoints.AddField("BEARING","DM_ANGLE",false,"");
            testpoints.AddField("DISTANCE","DM_KMETR",false,"");
            testpoints.AddField("USEBLEFIELD","DM_DBELL",false,"");
            testpoints.AddField("PROTECTEDFIELD","DM_DBELL",false,"");
            testpoints.AddField("TESTPOINT_TYPE","DM_SMALLINT",false,"");
            testpoints.CreateTable();
            testpoints.GrantAll();

            Table testpointtransmitters("TESTPOINTTRANSMITTERS", this);
            testpointtransmitters.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",false,"");
            testpointtransmitters.AddField("TESTPOINTS_ID","DM_IDENTY_FK",false,"");
            testpointtransmitters.CreateTable();
            testpointtransmitters.GrantAll();

            Table transmitterequipment("TRANSMITTEREQUIPMENT", this);
            transmitterequipment.AddField("ID","DM_IDENTY_PK",true,"");
            transmitterequipment.AddField("TRANSMITTERS_ID","DM_IDENTY_FK",true,"");
            transmitterequipment.AddField("EQUIPMENT_ID","DM_IDENTY_FK",true,"");
            transmitterequipment.AddField("NUMFACTORY","DM_VARCHAR16",false,"");
            transmitterequipment.AddField("NUMSTANDCERTIFICATE","DM_VARCHAR16",false,"");
            transmitterequipment.AddField("DATESTANDCERTIFICATE","DM_DATE",false,"");
            transmitterequipment.CreateTable();
            transmitterequipment.GrantAll();

            Table transmitters("TRANSMITTERS", this);
            transmitters.AddField("ID","DM_IDENTY_PK",false,"");
            transmitters.AddField("STAND_ID","DM_INTEGER",false,"");
            transmitters.AddField("ADMINISTRATIONID","DM_VARCHAR4",false,"");
            transmitters.AddField("LATITUDE","DM_GEOPOINT",true,"");
            transmitters.AddField("LONGITUDE","DM_GEOPOINT",true,"");
            transmitters.AddField("SYSTEMCAST_ID","DM_IDENTY_FK",false,"");
            transmitters.AddField("TYPESYSTEM","DM_SMALLINT",false,"");
            transmitters.AddField("CHANNEL_ID","DM_INTEGER",false,"");
            transmitters.AddField("VIDEO_CARRIER","DM_MHERZ",false,"");
            transmitters.AddField("VIDEO_OFFSET_LINE","DM_OFFSETLINE",false,"");
            transmitters.AddField("VIDEO_OFFSET_HERZ","DM_HERZ",false,"");
            transmitters.AddField("TYPEOFFSET","DM_OFFSETTYPE",false,"");
            transmitters.AddField("SYSTEMCOLOUR","DM_SYSTEMCOLOUR",false,"");
            transmitters.AddField("POWER_VIDEO","DM_KWATT",false,"");
            transmitters.AddField("EPR_VIDEO_MAX","DM_KWATT",false,"");
            transmitters.AddField("EPR_VIDEO_HOR","DM_KWATT",false,"");
            transmitters.AddField("EPR_VIDEO_VERT","DM_KWATT",false,"");
            transmitters.AddField("EFFECTPOWERHOR","DM_BLOB",false,"");
            transmitters.AddField("EFFECTPOWERVER","DM_BLOB",false,"");
            transmitters.AddField("HEIGHTANTENNA","DM_HEIGHTMETR",false,"");
            transmitters.AddField("HEIGHT_EFF_MAX","DM_HEIGHTMETR",false,"");
            transmitters.AddField("EFFECTHEIGHT","DM_BLOB",false,"");
            transmitters.AddField("POLARIZATION","DM_POLARIZATION",false,"");
            transmitters.AddField("ANTENNAGAIN","DM_DBELL",false,"");
            transmitters.AddField("DIRECTION","DM_DIRECTION",false,"");
            transmitters.AddField("ANT_DIAG_H","DM_BLOB",false,"");
            transmitters.AddField("FIDERLOSS","DM_DBELL",false,"");
            transmitters.AddField("FIDERLENGTH","DM_METR",false,"");
            transmitters.AddField("ANGLEELEVATION_HOR","DM_DEGREE",false,"");
            transmitters.AddField("ANGLEELEVATION_VER","DM_DEGREE",false,"");
            transmitters.AddField("IDENTIFIERSFN","DM_INTEGER",false,"");
            transmitters.AddField("RELATIVETIMINGSFN","DM_MICROSEC",false,"");
            transmitters.AddField("GUARDINTERVAL_ID","DM_INTEGER",false,"");
            transmitters.AddField("ALLOTMENTBLOCKDAB_ID","DM_INTEGER",false,"");
            transmitters.AddField("BLOCKCENTREFREQ","DM_MHERZ",false,"");
            transmitters.AddField("ACCOUNTCONDITION_IN","DM_INTEGER",false,"");
            transmitters.AddField("ACCOUNTCONDITION_OUT","DM_INTEGER",false,"");
            transmitters.AddField("DATECREATE","DM_DATE",true,"");
            transmitters.AddField("DATECHANGE","DM_DATE",true,"");
            transmitters.AddField("LICENSE_CHANNEL_ID","DM_INTEGER",false,"");
            transmitters.AddField("LICENSE_RFR_ID","DM_INTEGER",false,"");
            transmitters.AddField("LICENSE_SERVICE_ID","DM_INTEGER",false,"");
            transmitters.AddField("NUMPERMBUILD","DM_VARCHAR64",false,"");
            transmitters.AddField("DATEPERMBUILDFROM","DM_DATE",false,"");
            transmitters.AddField("DATEPERMBUILDTO","DM_DATE",false,"");
            transmitters.AddField("NUMPERMUSE","DM_VARCHAR64",false,"");
            transmitters.AddField("DATEPERMUSEFROM","DM_DATE",false,"");
            transmitters.AddField("DATEPERMUSETO","DM_DATE",false,"");
            transmitters.AddField("REGIONALCOUNCIL","DM_VARCHAR64",false,"");
            transmitters.AddField("NUMPERMREGCOUNCIL","DM_VARCHAR64",false,"");
            transmitters.AddField("DATEPERMREGCOUNCIL","DM_DATE",false,"");
            transmitters.AddField("NOTICECOUNT","DM_BLOB",false,"");
            transmitters.AddField("NUMSTANDCERTIFICATE","DM_VARCHAR16",false,"");
            transmitters.AddField("DATESTANDCERTIFICATE","DM_DATE",false,"");
            transmitters.AddField("NUMFACTORY","DM_VARCHAR16",false,"");
            transmitters.AddField("RESPONSIBLEADMIN","DM_VARCHAR4",false,"");
            transmitters.AddField("REGIONALAGREEMENT","DM_SMALLINT",false,"");
            transmitters.AddField("DATEINTENDUSE","DM_DATE",false,"");
            transmitters.AddField("AREACOVERAGE","DM_BLOB",false,"");
            transmitters.AddField("CLASSWAVE","DM_CLASSWAVE",false,"");
            transmitters.AddField("TIMETRANSMIT","DM_VARCHAR256",false,"");
            transmitters.AddField("FREQSTABILITY","DM_STABILITY",false,"");
            transmitters.AddField("VIDEO_EMISSION","DM_VARCHAR16",false,"");
            transmitters.AddField("SOUND_CARRIER_PRIMARY","DM_MHERZ",false,"");
            transmitters.AddField("SOUND_OFFSET_PRIMARY","DM_HERZ",false,"");
            transmitters.AddField("SOUND_EMISSION_PRIMARY","DM_VARCHAR16",false,"");
            transmitters.AddField("POWER_SOUND_PRIMARY","DM_KWATT",false,"");
            transmitters.AddField("EPR_SOUND_MAX_PRIMARY","DM_DBKVT",false,"");
            transmitters.AddField("EPR_SOUND_HOR_PRIMARY","DM_DBKVT",false,"");
            transmitters.AddField("EPR_SOUND_VERT_PRIMARY","DM_DBKVT",false,"");
            transmitters.AddField("V_SOUND_RATIO_PRIMARY","DM_DBELL",false,"");
            transmitters.AddField("MONOSTEREO_PRIMARY","DM_BOOLEAN",false,"");
            transmitters.AddField("SOUND_CARRIER_SECOND","DM_MHERZ",false,"");
            transmitters.AddField("SOUND_OFFSET_SECOND","DM_HERZ",false,"");
            transmitters.AddField("SOUND_EMISSION_SECOND","DM_VARCHAR16",false,"");
            transmitters.AddField("POWER_SOUND_SECOND","DM_KWATT",false,"");
            transmitters.AddField("EPR_SOUND_MAX_SECOND","DM_KWATT",false,"");
            transmitters.AddField("EPR_SOUND_HOR_SECOND","DM_KWATT",false,"");
            transmitters.AddField("EPR_SOUND_VER_SECOND","DM_KWATT",false,"");
            transmitters.AddField("SOUND_SYSTEM_SECOND","DM_SECONDSOUND",false,"");
            transmitters.AddField("V_SOUND_RATIO_SECOND","DM_DBELL",false,"");
            transmitters.AddField("TESTPOINTSIS","DM_BOOLEAN",false,"");
            transmitters.AddField("NAMEPROGRAMM","DM_NAMEPROGRAMM",false,"");
            transmitters.AddField("USERID","DM_INTEGER",true,"");
            transmitters.AddField("NUMREGISTRY","DM_VARCHAR16",false,"");
            transmitters.AddField("TYPEREGISTRY","DM_VARCHAR16",false,"");
            transmitters.AddField("REMARKS","DM_VARCHAR512",false,"");
            transmitters.AddField("RELAYSTATION_ID","DM_INTEGER",false,"");
            transmitters.AddField("TYPERECEIVE_ID","DM_INTEGER",false,"");
            transmitters.AddField("OWNER_ID","DM_INTEGER",false,"");
            transmitters.AddField("OPERATOR_ID","DM_INTEGER",false,"");
            transmitters.AddField("LEVELSIDERADIATION","DM_MILLIWATT",false,"");
            transmitters.AddField("FREQSHIFT","DM_HERZ",false,"");
            transmitters.AddField("SUMMATORPOWERS","DM_BOOLEAN default 0",false,"");
            transmitters.AddField("AZIMUTHMAXRADIATION","DM_ANGLE",false,"");
            transmitters.AddField("SUMMATOFREQFROM","DM_MHERZ",false,"");
            transmitters.AddField("SUMMATORFREQTO","DM_MHERZ",false,"");
            transmitters.AddField("SUMMATORPOWERFROM","DM_KWATT",false,"");
            transmitters.AddField("SUMMATORPOWERTO","DM_KWATT",false,"");
            transmitters.AddField("SUMMATORMINFREQS","DM_MHERZ",false,"");
            transmitters.AddField("SUMMATORATTENUATION","DM_DBELL",false,"");
            transmitters.AddField("ORIGINALID","DM_INTEGER",false,"");
            transmitters.AddField("STATUS","DM_SMALLINT",false,"");
            transmitters.AddField("WAS_IN_BASE","DM_BOOLEAN",false,"");
            transmitters.AddField("CARRIER","DM_MHERZ",false,"");
            transmitters.AddField("BANDWIDTH","DM_MHERZ",false,"");
            transmitters.AddField("USER_DELETED","DM_NULLFOREIGNKEY",false,"");
            transmitters.AddField("DATE_DELETED","DM_DATETIME",false,"");
            transmitters.AddField("MAX_COORD_DIST","DM_KMETR default 1000",false,"");
            transmitters.AddField("REMARKS_ADD","DM_VARCHAR512",false,"");
            transmitters.AddField("LICENSE_1","DM_VARCHAR64",false,"");
            transmitters.AddField("LICENSE_1_DATE","DM_DATE",false,"");
            transmitters.AddField("LICENSE_2","DM_VARCHAR64",false,"");
            transmitters.AddField("LICENSE_2_DATE","DM_DATE",false,"");
            transmitters.AddField("RUSREG","DM_VARCHAR4",false,"");
            transmitters.AddField("RUSNUM","DM_VARCHAR4",false,"");
            transmitters.AddField("DEL","DM_SMALLINT",false,"");
            transmitters.AddField("RPC","DM_SMALLINT",false,"");
            transmitters.AddField("RX_MODE","DM_SMALLINT",false,"");
            transmitters.AddField("ADM_ID","DM_INTEGER",false,"");
            transmitters.AddField("IS_PUB_REQ","DM_BOOLEAN",false,"");
            transmitters.AddField("ADM_REF_ID","DM_ADM_REF_ID20",false,"");
            transmitters.AddField("PLAN_ENTRY","DM_INTEGER",false,"");
            transmitters.AddField("ASSGN_CODE","DM_ASSGN_CODE",false,"");
            transmitters.AddField("ASSOCIATED_ADM_ALLOT_ID","DM_ADM_REF_ID20",false,"");
            transmitters.AddField("ASSOCIATED_ALLOT_SFN_ID","DM_ADM_REF_ID30",false,"");
            transmitters.AddField("CALL_SIGN","DM_CALLSIGN",false,"");
            transmitters.AddField("D_EXPIRY","DM_DATE",false,"");
            transmitters.AddField("OP_AGCY","DM_VARCHAR64",false,"");
            transmitters.AddField("ADDR_CODE","DM_VARCHAR64",false,"");
            transmitters.AddField("OP_HH_FR","DM_TIME",false,"");
            transmitters.AddField("OP_HH_TO","DM_TIME",false,"");
            transmitters.AddField("IS_RESUB","DM_BOOLEAN",false,"");
            transmitters.AddField("REMARK_CONDS_MET","DM_BOOLEAN",false,"");
            transmitters.AddField("SIGNED_COMMITMENT","DM_BOOLEAN",false,"");
            transmitters.AddField("ANT_DIAG_V","DM_BLOB",false,"");
            transmitters.AddField("COORD","DM_VARCHAR64",false,"");
            transmitters.AddField("SPECT_MASK","DM_SPECTRUM_MASK",false,"");
            transmitters.CreateTable();
            transmitters.GrantAll();

            Table typereceive("TYPERECEIVE", this);
            typereceive.AddField("ID","DM_IDENTY_PK",true,"");
            typereceive.AddField("NAME","DM_VARCHAR16",false,"");
            typereceive.CreateTable();
            typereceive.GrantAll();

            Table typesynhronet("TYPESYNHRONET", this);
            typesynhronet.AddField("ID","DM_IDENTY_PK",true,"");
            typesynhronet.AddField("NUMTRANSMITTERS","DM_SMALLINT",false,"");
            typesynhronet.AddField("NETGEOMETRY","DM_VARCHAR16",false,"");
            typesynhronet.AddField("EFFHEIGHTANT_PERTRANS","DM_METR",false,"");
            typesynhronet.AddField("EFFHEIGHTANT_CENTRANS","DM_METR",false,"");
            typesynhronet.AddField("DISTANCE_ADJACENT_TRANS","DM_KMETR",false,"");
            typesynhronet.AddField("POWER_PERTRANS","DM_KWATT",false,"");
            typesynhronet.AddField("POWER_CENTRANS","DM_KWATT",false,"");
            typesynhronet.AddField("DIRECTIVITY_PERTRANS","DM_ANGLE",false,"");
            typesynhronet.AddField("DIRECTIVITY_CENTRANS","DM_ANGLE",false,"");
            typesynhronet.AddField("COVERAGEAREA","DM_KMETR",false,"");
            typesynhronet.AddField("TYPENAME","DM_VARCHAR32",false,"");
            typesynhronet.CreateTable();
            typesynhronet.GrantAll();

            Table uncompatiblechannels("UNCOMPATIBLECHANNELS", this);
            uncompatiblechannels.AddField("ID","DM_IDENTY_PK",true,"");
            uncompatiblechannels.AddField("COCHANNEL","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("LOWERADJACENT","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("UPPERADJACENT","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("HETERODYNEHARMONIC1","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("HETERODYNEHARMONIC2","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("HETERODYNEHARMONIC3","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("LOWERIMAGE1","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("LOWERIMAGE2","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("UPPERIMAGE1","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.AddField("UPPERIMAGE2","DM_NAMECHANNEL",false,"");
            uncompatiblechannels.CreateTable();
            uncompatiblechannels.GrantAll();

            Table userobjects("USEROBJECTS", this);
            userobjects.AddField("USERID","DM_IDENTY_FK",true,"");
            userobjects.AddField("OBJECTID","DM_IDENTY_FK",true,"");
            userobjects.AddField("OBJECTNO","INTEGER",false,"");
            userobjects.CreateTable();
            userobjects.GrantAll();

            // View: VW_AREA_TXCOUNT -/
            RunQuery("CREATE VIEW VW_AREA_TXCOUNT(\
                REGNUM,\
                TXCOUNT)\
            AS\
            select a.numregion, (select count(*) from transmitters t where t.stand_id in (select id from stand s where s.area_id=a.id)) from area a");
            RunQuery("grant all on VW_AREA_TXCOUNT to public");

            // View: VW_TX -/
            RunQuery("CREATE VIEW VW_TX(\
                ID,\
                DEL,\
                NUMREGION,\
                ADMINISTRATIONID,\
                NAMESITE,\
                AC_IN,\
                AC_OUT,\
                DATECREATE,\
                DATECHANGE,\
                STATUS)\
            AS\
            select t.id, t.del, a.numregion, t.administrationid, s.namesite,\
            (select ac1.code from accountcondition ac1 where ac1.id = t.accountcondition_in) ac_in,\
            (select ac2.code from accountcondition ac2 where ac2.id = t.accountcondition_out) ac_out,\
            t.datecreate, t.datechange, t.status\
            from transmitters t, stand s, area a\
            where\
            t.stand_id = s.id and\
            s.area_id = a.id");
            RunQuery("grant all on VW_TX to public");


            // View: VW_TXS -/
            RunQuery("CREATE VIEW VW_TXS(\
                ID,\
                ADMINISTRATIONID,\
                NUMREGION,\
                DATECHANGE)\
            AS\
            select tx.id, tx.administrationid, reg.numregion, tx.datechange from transmitters tx\
            join area reg on reg.id=(select area_id from stand where id = (select stand_id from transmitters tx2 where tx2.id = tx.id))");
            RunQuery("grant all on VW_TXS to public");

            RunQuery("ALTER TABLE OWNER ADD CONSTRAINT UNQ_OWNER UNIQUE (NUMIDENTYCOD)");

            accountcondition.AddPrimaryKey("ID");
            activeview.AddPrimaryKey("ID");
            admit.AddPrimaryKey("ID");
            allotmentblockdab.AddPrimaryKey("ID");
            analogradiosystem.AddPrimaryKey("ID");
            analogtelesystem.AddPrimaryKey("ID");
            area.AddPrimaryKey("ID");
            arhive.AddPrimaryKey("ID");
            bank.AddPrimaryKey("ID");
            blockdab.AddPrimaryKey("ID");
            carrierguardinterval.AddPrimaryKey("ID");
            chanelsretranslate.AddPrimaryKey("ID");
            channels.AddPrimaryKey("ID");
            city.AddPrimaryKey("ID");
            coorddistance.AddPrimaryKey("ID");
            coordination.AddPrimaryKey("ID");
            coordpoints.AddPrimaryKey("ID");
            country.AddPrimaryKey("ID");
            countrycoordination.AddPrimaryKey("ID");
            databasesection.AddPrimaryKey("ID");
            digitaltelesystem.AddPrimaryKey("ID");
            dig_allotment.AddPrimaryKey("ID");
            dig_allot_zone.AddPrimaryKey("ID");
            dig_allot_zone_point.AddPrimaryKey("ZONE_ID, POINT_NO");
            dig_contour.AddPrimaryKey("ID");
            dig_subareapoint.AddPrimaryKey("CONTOUR_ID, POINT_NO");
            district.AddPrimaryKey("ID");
            document.AddPrimaryKey("ID");
            dplan_channel.AddPrimaryKey("PLANID, CHANNEL");
            dplan_checkpoints.AddPrimaryKey("TXID, POINT_NO");
            dplan_coord.AddPrimaryKey("ZONE, NOM");
            dplan_layer.AddPrimaryKey("ID");
            dplan_plan.AddPrimaryKey("ID");
            dplan_prohib_zones.AddPrimaryKey("ZONE, PROHIB_ZONE");
            dplan_selection.AddPrimaryKey("ID");
            dplan_sel_trans.AddPrimaryKey("SELECTION, TRANSMITTER");
            dplan_tx_group.AddPrimaryKey("ID");
            dplan_tx_gr_link.AddPrimaryKey("TXID, GROUPID");
            dplan_zone.AddPrimaryKey("ID");
            equipment.AddPrimaryKey("ID");
            frequencygrid.AddPrimaryKey("ID");
            imp_rests.AddPrimaryKey("CODE");
            letters.AddPrimaryKey("ID");
            license.AddPrimaryKey("ID");
            minstrengthfield.AddPrimaryKey("ID");
            offsetcarryfreqtva.AddPrimaryKey("ID");
            owner.AddPrimaryKey("ID");
            protectratioesdab.AddPrimaryKey("ID");
            protectratioesdvb.AddPrimaryKey("ID");
            protectratioestva.AddPrimaryKey("ID");
            protectratioesvhfsb.AddPrimaryKey("ID");
            radioservice.AddPrimaryKey("ID");
            selections.AddPrimaryKey("ID");
            stand.AddPrimaryKey("ID");
            street.AddPrimaryKey("ID");
            synhrofreqnet.AddPrimaryKey("ID");
            systemcast.AddPrimaryKey("ID");
            telecomorganization.AddPrimaryKey("ID");
            testpoints.AddPrimaryKey("ID");
            testpointtransmitters.AddPrimaryKey("TESTPOINTS_ID, TRANSMITTERS_ID");
            transmitterequipment.AddPrimaryKey("ID");
            transmitters.AddPrimaryKey("ID");
            typereceive.AddPrimaryKey("ID");
            typesynhronet.AddPrimaryKey("ID");
            uncompatiblechannels.AddPrimaryKey("ID");
            userobjects.AddPrimaryKey("USERID, OBJECTID");

            activeview.AddForeignKey("ADMIT_ID","ADMIT(ID)",true,true);
            allotmentblockdab.AddForeignKey("BLOCKDAB_ID","BLOCKDAB (ID)",true,true);
            area.AddForeignKey("COUNTRY_ID","COUNTRY (ID)",true,true);
            arhive.AddForeignKey("ACTIVEVIEW_ID","ACTIVEVIEW (ID)",true,true);
            chanelsretranslate.AddForeignKey("TRANSMITTERS_ID","TRANSMITTERS (ID)",false,true);
            chanelsretranslate.AddForeignKey("TYPERECEIVE_ID","TYPERECEIVE (ID)",true,true);
            city.AddForeignKey("AREA_ID","AREA (ID)",true,true);
            city.AddForeignKey("COUNTRY_ID","COUNTRY (ID)",true,true);
            coorddistance.AddForeignKey("SYSTEMCAST_ID","SYSTEMCAST (ID)",false,false);
            coordination.AddForeignKey("ACCOUNTCONDITION_ID","ACCOUNTCONDITION (ID)",true,true);
            coordination.AddForeignKey("TELECOMORG_ID","TELECOMORGANIZATION (ID)",true,true);
            coordination.AddForeignKey("TRANSMITTER_ID","TRANSMITTERS (ID)",true,true);
            countrycoordination.AddForeignKey("COUNTRY_ID","COUNTRY (ID)",true,true);
            countrycoordination.AddForeignKey("TRANSMITTERS_ID","TRANSMITTERS (ID)",true,true);
            countrypoints.AddForeignKey("COUNTRY_ID","COUNTRY (ID)",true,true);
            dig_allotment.AddForeignKey("BLOCKDAB_ID","BLOCKDAB (ID)",true,false);
            dig_allotment.AddForeignKey("CHANNEL_ID","CHANNELS (ID)",true,false);
            dig_allotment.AddForeignKey("SFN_ID_FK","SYNHROFREQNET (ID) ON DELETE SET NULL ON UPDATE CASCADE",false,false);
            dig_allotment.AddForeignKey("ADM_ID","COUNTRY (ID)",true,false);
            dig_allotment.AddForeignKey("DB_SECTION_ID","DATABASESECTION (ID)",false,false);
            dig_allot_cntr_lnk.AddForeignKey("ALLOT_ID","DIG_ALLOTMENT (ID)",true,true);
            dig_allot_cntr_lnk.AddForeignKey("CNTR_ID","DIG_CONTOUR (ID)",true,true);
            dig_allot_zone.AddForeignKey("ALLOT_ID","DIG_ALLOTMENT (ID)",true,true);
            dig_allot_zone_point.AddForeignKey("ZONE_ID","DIG_ALLOT_ZONE (ID)",true,true);
            dig_contour.AddForeignKey("ADM_ID","COUNTRY (ID)",true,false);
            dig_contour.AddForeignKey("DB_SECTION_ID","DATABASESECTION (ID)",false,false);
            dig_subareapoint.AddForeignKey("CONTOUR_ID","DIG_CONTOUR (ID)",true,true);
            district.AddForeignKey("AREA_ID","AREA (ID)",true,true);
            dplan_channel.AddForeignKey("CHANNEL","CHANNELS (ID)",true,true);
            dplan_channel.AddForeignKey("MAIN_SELECTION","DPLAN_SELECTION (ID)",true,true);
            dplan_channel.AddForeignKey("PLANID","DPLAN_PLAN (ID)",true,true);
            dplan_checkpoints.AddForeignKey("TXID","TRANSMITTERS (ID)",true,true);
            dplan_coord.AddForeignKey("ZONE","DPLAN_ZONE (ID)",true,true);
            dplan_fg.AddForeignKey("FG","FREQUENCYGRID (ID)",true,true);
            dplan_fg.AddForeignKey("PLANID","DPLAN_PLAN (ID)",true,true);
            dplan_layer.AddForeignKey("PLANID","DPLAN_PLAN (ID)",true,true);
            dplan_l_channel.AddForeignKey("CHANNEL","CHANNELS (ID)",true,true);
            dplan_l_channel.AddForeignKey("LAYER","DPLAN_LAYER (ID)",true,true);
            dplan_plan_selection.AddForeignKey("SELECTION","DPLAN_SELECTION (ID)",true,true);
            dplan_plan_selection.AddForeignKey("CHANNEL","CHANNELS (ID)",true,true);
            dplan_prohib_zones.AddForeignKey("ZONE","DPLAN_ZONE (ID)",true,true);
            dplan_prohib_zones.AddForeignKey("PROHIB_ZONE","DPLAN_ZONE (ID)",true,true);
            dplan_selection.AddForeignKey("CHANNEL","CHANNELS (ID)",true,true);
            dplan_selection.AddForeignKey("PLANID","DPLAN_PLAN (ID)",true,true);
            dplan_sel_trans.AddForeignKey("SELECTION","DPLAN_SELECTION (ID)",true,true);
            dplan_sel_trans.AddForeignKey("TRANSMITTER","TRANSMITTERS (ID)",true,true);
            dplan_tx_group.AddForeignKey("PLANID","DPLAN_PLAN (ID)",true,true);
            dplan_tx_gr_link.AddForeignKey("TXID","TRANSMITTERS (ID)",true,true);
            dplan_tx_gr_link.AddForeignKey("GROUPID","DPLAN_TX_GROUP (ID)",true,true);
            dplan_zone.AddForeignKey("LAYER","DPLAN_LAYER (ID)",true,true);
            dplan_z_channel.AddForeignKey("CHANNEL","CHANNELS (ID)",true,true);
            dplan_z_channel.AddForeignKey("ZONE","DPLAN_ZONE (ID)",true,true);
            letters.AddForeignKey("DOCUMENT_ID","DOCUMENT (ID)",true,true);
            letters.AddForeignKey("TRANSMITTERS_ID","TRANSMITTERS (ID)",true,true);
            license.AddForeignKey("OWNER_ID","OWNER (ID)",true,true);
            minstrengthfield.AddForeignKey("SYSTEMCAST_ID","SYSTEMCAST (ID)",true,true);
            owner.AddForeignKey("BANK_ID","BANK (ID)",true,true);
            protectratioesdab.AddForeignKey("RADIOSERVICE_ID","RADIOSERVICE (ID)",true,true);
            selectedtransmitters.AddForeignKey("SELECTIONS_ID","SELECTIONS (ID)",true,true);
            selectedtransmitters.AddForeignKey("TRANSMITTERS_ID","TRANSMITTERS (ID)",true,true);
            selections.AddForeignKey("TRANSMITTERS_ID","TRANSMITTERS (ID)",false,true);
            sel_area.AddForeignKey("SELECTION","SELECTIONS (ID)",true,true);
            sel_area.AddForeignKey("AREA","AREA (ID)",true,true);
            sel_condition.AddForeignKey("SELECTION","SELECTIONS (ID)",true,true);
            sel_condition.AddForeignKey("CONDITION","ACCOUNTCONDITION (ID)",false,false);
            sel_frequency.AddForeignKey("SELECTION","SELECTIONS (ID)",true,true);
            stand.AddForeignKey("AREA_ID","AREA (ID)",true,true);
            stand.AddForeignKey("CITY_ID","CITY (ID)",true,true);
            stand.AddForeignKey("STREET_ID","STREET (ID)",true,true);
            street.AddForeignKey("CITY_ID","CITY (ID)",true,true);
            synhrofreqnet.AddForeignKey("TYPESYNHRONET_ID","TYPESYNHRONET (ID)",true,true);
            testpoints.AddForeignKey("TRANSMITTERS_ID","TRANSMITTERS (ID)",true,true);
            transmitterequipment.AddForeignKey("EQUIPMENT_ID","EQUIPMENT (ID)",true,true);
            transmitterequipment.AddForeignKey("TRANSMITTERS_ID","TRANSMITTERS (ID)",true,true);
            transmitters.AddForeignKey("SYSTEMCAST_ID","SYSTEMCAST (ID)",true,true);
            transmitters.AddForeignKey("STATUS","DATABASESECTION (ID)",false,false);
            transmitters.AddForeignKey("STAND_ID","STAND (ID)",true,false);

            area.AddIndex("AREA_IDX_NUMREG","NUMREGION","");
            dig_allotment.AddIndex("DIG_ALLOTMENT_IDX1","ADM_REF_ID","");
            stand.AddIndex("STAND_IDX_LAT","LATITUDE","");
            stand.AddIndex("STAND_IDX_LON","LONGITUDE","");
            stand.AddIndex("STAND_IDX_SITENAME","NAMESITE","");
            transmitters.AddIndex("TRANSMITTERS_IDX1","ASSOCIATED_ADM_ALLOT_ID","");
            transmitters.AddIndex("TRANSMITTERS_IDX_ADID","ADMINISTRATIONID","");

            // Trigger: AREA_BU0 -/
            CreateTrigger("AREA_BU0","AREA","ACTIVE BEFORE UPDATE POSITION 0 AS \
            begin \
                if (old.id = -1) then \
                begin \
                    new.country_id = old.country_id; \
                    new.name = old.name; \
                    new.numregion = old.numregion; \
                end \
            end ");

            // Trigger: BANK_BU0 -/
            CreateTrigger("BANK_BU0","BANK","ACTIVE BEFORE UPDATE POSITION 0 \
            AS \
            begin \
                if (old.id = -1) then \
                begin \
                    new.address = old.address; \
                    new.mfo = old.mfo; \
                    new.name = old.name; \
                end \
            end ");

            // Trigger: CITY_BI -/
            CreateTrigger("CITY_BI","CITY","ACTIVE BEFORE INSERT POSITION 0 \
            as \
            declare variable var_id integer; \
                begin \
                    if (new.DISTRICT_ID is null) then \
                        new.DISTRICT_ID = -1; \
                    if (new.AREA_ID is null) then begin \
                        select AREA_ID from DISTRICT where ID = new.DISTRICT_ID into :VAR_ID; \
                        new.AREA_ID = :VAR_ID; \
                    end \
                    if (new.COUNTRY_ID is null) then begin \
                        select COUNTRY_ID from AREA where ID = new.AREA_ID into :VAR_ID; \
                        new.COUNTRY_ID = :VAR_ID; \
                    end \
                end ");

            // Trigger: CITY_BU -/
            CreateTrigger("CITY_BU","CITY","ACTIVE BEFORE UPDATE POSITION 0 \
            as \
            declare variable VAR_ID integer; \
            begin \
                if (new.DISTRICT_ID is null) then \
                    new.DISTRICT_ID = -1; \
                if (new.AREA_ID is null) then begin \
                    select AREA_ID from DISTRICT where ID = new.DISTRICT_ID into :VAR_ID; \
                    new.AREA_ID = :VAR_ID; \
                end \
                if (new.COUNTRY_ID is null) then begin \
                    select COUNTRY_ID from AREA where ID = new.AREA_ID into :VAR_ID; \
                    new.COUNTRY_ID = :VAR_ID; \
                end \
            end ");

            // Trigger: CITY_BU1 -/
            CreateTrigger("CITY_BU1","CITY","ACTIVE BEFORE UPDATE POSITION 1 \
            AS \
            begin \
                if (old.id = -1) then \
                begin \
                    new.area_id = old.area_id; \
                    new.country_id = old.country_id; \
                    new.district_id = old.district_id; \
                    new.id = old.id; \
                    new.name = old.name; \
                end \
            end ");

            // Trigger: COUNTRY_BU0 -/
            CreateTrigger("COUNTRY_BU0","COUNTRY","ACTIVE BEFORE UPDATE POSITION 0 \
            AS \
            begin \
                if (old.id = -1) then \
                begin \
                    new.code = old.code; \
                    new.def_color = old.def_color; \
                    new.def_dvb_sys = old.def_dvb_sys; \
                    new.def_fm_sys = old.def_fm_sys; \
                    new.def_tva_sys = old.def_tva_sys; \
                    new.description = old.description; \
                    new.name = old.name; \
                    new.id = old.id; \
                end \
            end ");

            // Trigger: DIG_ALLOTMENT_AI -/
            CreateTrigger("DIG_ALLOTMENT_AI","DIG_ALLOTMENT","ACTIVE AFTER INSERT POSITION 0 \
            as \
            begin \
                update TRANSMITTERS set STATUS = new.DB_SECTION_ID where ID = new.ID; \
            end ");

            // Trigger: DIG_ALLOTMENT_AU -/
            CreateTrigger("DIG_ALLOTMENT_AU","DIG_ALLOTMENT","ACTIVE AFTER UPDATE POSITION 0 \
            as \
            begin \
                if (new.DB_SECTION_ID <> old.DB_SECTION_ID) then \
                update TRANSMITTERS set STATUS = new.DB_SECTION_ID where ID = new.ID; \
            end ");

            // Trigger: OWNER_BU0 -/
            CreateTrigger("OWNER_BU0","OWNER","ACTIVE BEFORE UPDATE POSITION 0 \
            AS \
            begin \
                if (old.id = -1) then \
                begin \
                    new.addressjure = old.addressjure; \
                    new.addressphysical = old.addressphysical; \
                    new.bank_id = old.bank_id; \
                    new.fax = old.fax; \
                    new.mail = old.mail; \
                    new.nameboss = old.nameboss; \
                    new.nameorganization = old.nameorganization; \
                    new.numidentycod = old.numidentycod; \
                    new.numidentycodaccount = old.numidentycodaccount; \
                    new.numnds = old.numnds; \
                    new.numsettlementaccount = old.numsettlementaccount; \
                    new.phone = old.phone; \
                end \
            end ");

            // Trigger: SELECTIONS_BI -/
            CreateTrigger("SELECTIONS_BI","SELECTIONS","ACTIVE BEFORE INSERT POSITION 0 \
            as \
            begin \
                select OUT_ID from SP_GET_USERID into new.USERID; \
                new.CREATEDATE = 'now'; \
            end ");



            // Trigger: STAND_BI -/
            CreateTrigger("STAND_BI","STAND","ACTIVE BEFORE INSERT POSITION 0 \
            as \
            declare variable VAR_ID integer; \
            declare variable VAR_ID2 integer; \
            begin \
                if (not (new.CITY_ID is null or new.CITY_ID <= 0)) then begin \
                select AREA_ID, DISTRICT_ID from CITY where ID = new.CITY_ID into :VAR_ID, :VAR_ID2; \
                    if (:VAR_ID is not null and :VAR_ID2 is not null) then begin \
                        new.AREA_ID = :VAR_ID; \
                        new.DISTRICT_ID = :VAR_ID2; \
                    end \
                end \
            end ");

            // Trigger: STAND_BU -/
            CreateTrigger("STAND_BU","STAND","ACTIVE BEFORE UPDATE POSITION 0 \
            as \
            declare variable VAR_ID integer; \
            declare variable VAR_ID2 integer; \
            begin \
                if (not (new.CITY_ID is null or new.CITY_ID <= 0)) then begin \
                    select AREA_ID, DISTRICT_ID from CITY where ID = new.CITY_ID into :VAR_ID, :VAR_ID2; \
                    if (:VAR_ID is not null and :VAR_ID2 is not null) then begin \
                        new.AREA_ID = :VAR_ID; \
                        new.DISTRICT_ID = :VAR_ID2; \
                    end \
                end \
            end ");

            // Trigger: TR_DPLAN_TX_GROUP_AD -/
            CreateTrigger("TR_DPLAN_TX_GROUP_AD","DPLAN_TX_GROUP","ACTIVE AFTER DELETE POSITION 0 \
            as \
            begin \
                DELETE FROM DPLAN_TX_GROUP WHERE PARENTID = OLD.ID; \
            end ");

            // Trigger: TR_TRANSMITTERS_AI_0 -/
            CreateTrigger("TR_TRANSMITTERS_AI_0","TRANSMITTERS","ACTIVE AFTER INSERT POSITION 0 \
            AS \
            declare variable var_userid integer; \
            declare variable var_currenttime Timestamp; \
            begin \
                var_currenttime=current_timestamp; \
                select ID from admit where LOGIN=user into :var_userid; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Добавление', 'TRANSMITTERS', new.id); \
            end ");

            // Trigger: TR_TRANSMITTERS_AU_0 -/
            CreateTrigger("TR_TRANSMITTERS_AU_0","TRANSMITTERS","ACTIVE AFTER UPDATE POSITION 0 \
            AS \
            declare variable var_userid integer; \
            declare variable var_namefield varchar(256); \
            declare variable var_currenttime Timestamp; \
            begin \
                var_currenttime=current_timestamp; \
                select ID from admit where LOGIN=user into :var_userid; \
              if (new.STAND_ID<>old.STAND_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='STAND_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.LATITUDE<>old.LATITUDE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LATITUDE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.LONGITUDE<>old.LONGITUDE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LONGITUDE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.OWNER_ID<>old.OWNER_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='OWNER_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.LICENSE_CHANNEL_ID<>old.LICENSE_CHANNEL_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LICENSE_CHANNEL_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.LICENSE_RFR_ID<>old.LICENSE_RFR_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LICENSE_RFR_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.LICENSE_SERVICE_ID<>old.LICENSE_SERVICE_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LICENSE_SERVICE_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.NUMPERMBUILD<>old.NUMPERMBUILD) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMPERMBUILD' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DATEPERMBUILDFROM<>old.DATEPERMBUILDFROM) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMBUILDFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DATEPERMBUILDTO<>old.DATEPERMBUILDTO) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMBUILDTO' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.NUMPERMUSE<>old.NUMPERMUSE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMPERMUSE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DATEPERMUSEFROM<>old.DATEPERMUSEFROM) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMUSEFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DATEPERMUSETO<>old.DATEPERMUSETO) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMUSETO' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.REGIONALCOUNCIL<>old.REGIONALCOUNCIL) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='REGIONALCOUNCIL' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.NUMPERMREGCOUNCIL<>old.NUMPERMREGCOUNCIL) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMPERMREGCOUNCIL' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DATEPERMREGCOUNCIL<>old.DATEPERMREGCOUNCIL) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEPERMREGCOUNCIL' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              /*  if (new.VIDEO_CARRIER<>old.VIDEO_CARRIER) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_CARRIER' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.VIDEO_OFFSET_HERZ<>old.VIDEO_OFFSET_HERZ) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_OFFSET_HERZ' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end */\
              if (new.NUMSTANDCERTIFICATE<>old.NUMSTANDCERTIFICATE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMSTANDCERTIFICATE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DATESTANDCERTIFICATE<>old.DATESTANDCERTIFICATE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATESTANDCERTIFICATE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.NUMFACTORY<>old.NUMFACTORY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMFACTORY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.RESPONSIBLEADMIN<>old.RESPONSIBLEADMIN) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='RESPONSIBLEADMIN' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end\
              if (new.ADMINISTRATIONID<>old.ADMINISTRATIONID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ADMINISTRATIONID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.REGIONALAGREEMENT<>old.REGIONALAGREEMENT) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='REGIONALAGREEMENT' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DATEINTENDUSE<>old.DATEINTENDUSE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DATEINTENDUSE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.AREACOVERAGE<>old.AREACOVERAGE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='AREACOVERAGE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.SYSTEMCAST_ID<>old.SYSTEMCAST_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SYSTEMCAST_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.ACCOUNTCONDITION_IN<>old.ACCOUNTCONDITION_IN) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ACCOUNTCONDITION_IN' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.ACCOUNTCONDITION_OUT<>old.ACCOUNTCONDITION_OUT) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ACCOUNTCONDITION_OUT' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.TIMETRANSMIT<>old.TIMETRANSMIT) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TIMETRANSMIT' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.CHANNEL_ID<>old.CHANNEL_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='CHANNEL_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              /*  if (new.VIDEO_CARRIER<>old.VIDEO_CARRIER) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_CARRIER' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.VIDEO_OFFSET_HERZ<>old.VIDEO_OFFSET_HERZ) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_OFFSET_HERZ' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end */\
              if (new.VIDEO_OFFSET_LINE<>old.VIDEO_OFFSET_LINE) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_OFFSET_LINE' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.FREQSTABILITY<>old.FREQSTABILITY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FREQSTABILITY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.TYPEOFFSET<>old.TYPEOFFSET) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPEOFFSET' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.SYSTEMCOLOUR<>old.SYSTEMCOLOUR) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SYSTEMCOLOUR' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.VIDEO_EMISSION<>old.VIDEO_EMISSION) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='VIDEO_EMISSION' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.POWER_VIDEO<>old.POWER_VIDEO) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POWER_VIDEO' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.EPR_VIDEO_MAX<>old.EPR_VIDEO_MAX) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_VIDEO_MAX' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              /*  if (new.EPR_VIDEO_HOR<>old.EPR_VIDEO_HOR) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_VIDEO_HOR' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.EPR_VIDEO_VERT<>old.EPR_VIDEO_VERT) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_VIDEO_VERT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end */\
              if (bf_compare_blob(new.EFFECTPOWERHOR, old.EFFECTPOWERHOR)=0) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EFFECTPOWERHOR' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (bf_compare_blob(new.EFFECTPOWERVER, old.EFFECTPOWERVER)=0) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EFFECTPOWERVER' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.ALLOTMENTBLOCKDAB_ID<>old.ALLOTMENTBLOCKDAB_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ALLOTMENTBLOCKDAB_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.GUARDINTERVAL_ID<>old.GUARDINTERVAL_ID) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='GUARDINTERVAL_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.IDENTIFIERSFN<>old.IDENTIFIERSFN) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='IDENTIFIERSFN' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.RELATIVETIMINGSFN<>old.RELATIVETIMINGSFN) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='RELATIVETIMINGSFN' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.BLOCKCENTREFREQ<>old.BLOCKCENTREFREQ) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='BLOCKCENTREFREQ' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.SOUND_CARRIER_PRIMARY<>old.SOUND_CARRIER_PRIMARY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_CARRIER_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.SOUND_OFFSET_PRIMARY<>old.SOUND_OFFSET_PRIMARY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_OFFSET_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.SOUND_EMISSION_PRIMARY<>old.SOUND_EMISSION_PRIMARY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_EMISSION_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.POWER_SOUND_PRIMARY<>old.POWER_SOUND_PRIMARY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POWER_SOUND_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.EPR_SOUND_MAX_PRIMARY<>old.EPR_SOUND_MAX_PRIMARY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_MAX_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              /*  if (new.EPR_SOUND_HOR_PRIMARY<>old.EPR_SOUND_HOR_PRIMARY) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_HOR_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                        :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.EPR_SOUND_VERT_PRIMARY<>old.EPR_SOUND_VERT_PRIMARY) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_VERT_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end */\
              if (new.V_SOUND_RATIO_PRIMARY<>old.V_SOUND_RATIO_PRIMARY) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='V_SOUND_RATIO_PRIMARY' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.SOUND_EMISSION_SECOND<>old.SOUND_EMISSION_SECOND) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_EMISSION_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.POWER_SOUND_SECOND<>old.POWER_SOUND_SECOND) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POWER_SOUND_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.EPR_SOUND_MAX_SECOND<>old.EPR_SOUND_MAX_SECOND) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_MAX_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              /*  if (new.EPR_SOUND_HOR_SECOND<>old.EPR_SOUND_HOR_SECOND) then begin\
                    select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_HOR_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                    insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                             NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                             :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.EPR_SOUND_VER_SECOND<>old.EPR_SOUND_VER_SECOND) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EPR_SOUND_VER_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end */\
              if (new.SOUND_SYSTEM_SECOND<>old.SOUND_SYSTEM_SECOND) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SOUND_SYSTEM_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.V_SOUND_RATIO_SECOND<>old.V_SOUND_RATIO_SECOND) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='V_SOUND_RATIO_SECOND' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.HEIGHTANTENNA<>old.HEIGHTANTENNA) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='HEIGHTANTENNA' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              /*  if (new.HEIGHT_EFF_MAX<>old.HEIGHT_EFF_MAX) then begin \
                  select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='HEIGHT_EFF_MAX' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                  insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                           NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                           :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end */\
              if (bf_compare_blob(new.EFFECTHEIGHT, old.EFFECTHEIGHT)=0) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='EFFECTHEIGHT' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.POLARIZATION<>old.POLARIZATION) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='POLARIZATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.DIRECTION<>old.DIRECTION) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='DIRECTION' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.FIDERLOSS<>old.FIDERLOSS) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FIDERLOSS' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.FIDERLENGTH<>old.FIDERLENGTH) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FIDERLENGTH' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.ANGLEELEVATION_HOR<>old.ANGLEELEVATION_HOR) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANGLEELEVATION_HOR' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.ANGLEELEVATION_VER<>old.ANGLEELEVATION_VER) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANGLEELEVATION_VER' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (new.ANTENNAGAIN<>old.ANTENNAGAIN) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANTENNAGAIN' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, \
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (bf_compare_blob(new.ANT_DIAG_H, old.ANT_DIAG_H)=0) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANT_DIAG_H' and rdb$relation_name='TRANSMITTERS' into :var_namefield; \
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid, \
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield); \
              end \
              if (bf_compare_blob(new.ANT_DIAG_V, old.ANT_DIAG_V)=0) then begin \
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ANT_DIAG_V' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.NAMEPROGRAMM<>old.NAMEPROGRAMM) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NAMEPROGRAMM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.ORIGINALID<>old.ORIGINALID) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='ORIGINALID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.NUMREGISTRY<>old.NUMREGISTRY) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='NUMREGISTRY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.TYPEREGISTRY<>old.TYPEREGISTRY) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPEREGISTRY' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.REMARKS<>old.REMARKS) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='REMARKS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.RELAYSTATION_ID<>old.RELAYSTATION_ID) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='RELAYSTATION_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.OPERATOR_ID<>old.OPERATOR_ID) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='OPERATOR_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.TYPERECEIVE_ID<>old.TYPERECEIVE_ID) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPERECEIVE_ID' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.LEVELSIDERADIATION<>old.LEVELSIDERADIATION) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='LEVELSIDERADIATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.FREQSHIFT<>old.FREQSHIFT) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='FREQSHIFT' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.SUMMATORPOWERS<>old.SUMMATORPOWERS) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORPOWERS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.AZIMUTHMAXRADIATION<>old.AZIMUTHMAXRADIATION) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='AZIMUTHMAXRADIATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.SUMMATOFREQFROM<>old.SUMMATOFREQFROM) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATOFREQFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.SUMMATORFREQTO<>old.SUMMATORFREQTO) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORFREQTO' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.SUMMATORPOWERFROM<>old.SUMMATORPOWERFROM) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORPOWERFROM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.SUMMATORPOWERTO<>old.SUMMATORPOWERTO) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORPOWERTO' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.SUMMATORMINFREQS<>old.SUMMATORMINFREQS) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORMINFREQS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.SUMMATORATTENUATION<>old.SUMMATORATTENUATION) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='SUMMATORATTENUATION' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.STATUS<>old.STATUS) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='STATUS' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.TYPESYSTEM<>old.TYPESYSTEM) then begin\
                select BF_BLOB_TO_STR(rdb$description) from rdb$relation_fields where rdb$field_name='TYPESYSTEM' and rdb$relation_name='TRANSMITTERS' into :var_namefield;\
                insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE,\
                                         NUM_CHANGE, NAME_FIELD) values (gen_id(gen_activeview_id, 1), :var_userid,\
                                         :var_currenttime, 'Изменение', 'TRANSMITTERS', old.id, :var_namefield);\
              end\
              if (new.STATUS = 0 or (old.STATUS = 0)) then\
                update transmitters set WAS_IN_BASE = 1 where id = new.ID and WAS_IN_BASE <> 1;\
            END ");

            // Trigger: TR_TRANSMITTERS_BI -/
            CreateTrigger("TR_TRANSMITTERS_BI","TRANSMITTERS","ACTIVE BEFORE INSERT POSITION 0\
            as\
            declare variable var_userid integer;\
            declare variable var_systemcast integer;\
            begin\
                /*  set default field values  */\
                if (new.DIRECTION is null) then\
                new.DIRECTION = 'ND';\
                if (new.POLARIZATION is null) then\
                    new.POLARIZATION = 'V';\
                if (new.SOUND_SYSTEM_SECOND is null) then\
                    new.SOUND_SYSTEM_SECOND = 'NICAM';\
                if (new.MONOSTEREO_PRIMARY is null) then\
                    new.MONOSTEREO_PRIMARY = 0;\
                if (new.TYPEOFFSET is null) then\
                    new.TYPEOFFSET = 'Unspecified';\
                if (new.SYSTEMCOLOUR is null) then\
                    new.SYSTEMCOLOUR = 'SECAM';\
                if (new.CLASSWAVE is null) then\
                    new.CLASSWAVE = 'VHF';\
                if (new.FREQSTABILITY is null) then\
                    new.FREQSTABILITY = 'NORMAL';\
                if (new.VIDEO_OFFSET_LINE is null) then\
                    new.VIDEO_OFFSET_LINE = 0;\
                if (new.ACCOUNTCONDITION_IN is null) then\
                    new.ACCOUNTCONDITION_IN = 0;\
                if (new.ACCOUNTCONDITION_OUT is null) then\
                    new.ACCOUNTCONDITION_OUT = 0;\
                if (new.NUMFACTORY is null) then\
                    new.NUMFACTORY = ' ';\
                if (new.STATUS is null) then\
                    new.STATUS =1; /* draft */\
                if (new.OWNER_ID is null) then\
                    new.OWNER_ID = 0; /* root */\
                    /*  set CLASSWAVE according to actual wavelength  */ \
                               select ENUMVAL from SYSTEMCAST where ID = new.SYSTEMCAST_ID into :var_systemcast;\
                if (var_systemcast = 1 or (var_systemcast = 4)) then begin\
                    /*  TVA & DVB-T  */ \
                    if (new.VIDEO_CARRIER > 300) then\
                        new.CLASSWAVE = 'UHF';\
                    else\
                        new.CLASSWAVE = 'VHF';\
                end else if (var_systemcast = 2) then begin\
                    /*  FM  */ \
                   if (new.SOUND_CARRIER_PRIMARY > 300) then\
                        new.CLASSWAVE = 'UHF';\
                    else\
                        new.CLASSWAVE = 'VHF';\
                end else if (var_systemcast = 5) then begin\
                    /*  T-DAB  */ \
                    if (new.BLOCKCENTREFREQ > 300) then\
                        new.CLASSWAVE = 'UHF';\
                    else\
                        new.CLASSWAVE = 'VHF';\
                end\
                new.TESTPOINTSIS = 0;\
                new.DATECREATE = 'now';\
                new.DATECHANGE = 'now';\
                select ID from ADMIT where LOGIN = user\
                into :var_userid;\
                if (var_userid is null) then begin\
                    select OUT_ID from SP_GEN_ID\
                    into :var_userid;\
                    insert into ADMIT\
                    (ID, LOGIN)\
                    values\
                    (:var_userid, user);\
                end\
                new.USERID = var_userid;\
            end ");

            // Trigger: TR_TRANSMITTERS_BI_SET_RX_MODE -/
            CreateTrigger("TR_TRANSMITTERS_BI_SET_RX_MODE","TRANSMITTERS","ACTIVE BEFORE INSERT POSITION 2 \
            AS\
            begin\
              if (new.systemcast_id=70) then\
              begin\
                if (new.rpc = 0) then new.rx_mode = 0;\
                if (new.rpc = 1) then new.rx_mode = 1;\
                if (new.rpc = 2) then new.rx_mode = 2;\
                if (new.rpc = 3) then new.rx_mode = 1;\
                if (new.rpc = 4) then new.rx_mode = 2;\
                if (new.rpc = 1) then new.typesystem = 243;\
              end\
            end ");

            // Trigger: TR_TRANSMITTERS_BU_0 -/
            CreateTrigger("TR_TRANSMITTERS_BU_0","TRANSMITTERS","ACTIVE BEFORE UPDATE POSITION 0\
            AS\
            begin\
              select ID from admit where LOGIN=user into new.userid;\
              if (new.STATUS = -1) then begin\
                  new.DATE_DELETED=current_timestamp;\
                  select ID from admit where LOGIN=user into new.USER_DELETED;\
              end\
              if (old.status = 1 and new.status = 0) then\
              begin\
                if ((old.administrationid is null) or (old.administrationid = '')) then\
                    begin\
                      new.datecreate = current_timestamp;\
                    end\
                else\
                begin\
                  new.datecreate = old.datecreate;\
                end\
                new.datechange = current_timestamp;\
              end\
            end ");

            // Trigger: TR_TRANS_BI_SET_CARR3 -/
            CreateTrigger("TR_TRANS_BI_SET_CARR3","TRANSMITTERS","ACTIVE BEFORE INSERT POSITION 1 \
            as\
            declare variable var_carrier double precision;\
            declare variable var_bandwidth double precision;\
            begin\
                /* select out_carrier, out_bandwidth from SP_SET_CARRIER_AND_BW(new.id) into :var_carrier, :var_bandwidth;*/ \
                select out_carrier, out_bandwidth from SP_SET_CARRIER_AND_BW_3(new.id, new.video_carrier, new.sound_carrier_primary, new.blockcentrefreq, new.systemcast_id, new.typesystem, new.channel_id) into :var_carrier, :var_bandwidth;\
                new.carrier = :var_carrier;\
                new.bandwidth = :var_bandwidth;\
            end ");

            // Trigger: TR_TRANS_BU_SET_CARR3 -/
            CreateTrigger("TR_TRANS_BU_SET_CARR3","TRANSMITTERS","ACTIVE BEFORE UPDATE POSITION 1\
            as\
            declare variable var_carrier double precision;\
            declare variable var_bandwidth double precision;\
            begin\
                if (\
                    (new.SYSTEMCAST_ID <> old.SYSTEMCAST_ID)\
                    or (old.SYSTEMCAST_ID is null and new.SYSTEMCAST_ID is null)\
                    or (new.TYPESYSTEM <> old.TYPESYSTEM)\
                    or (old.TYPESYSTEM is null and new.TYPESYSTEM is not null)\
                    or (new.VIDEO_CARRIER <> old.VIDEO_CARRIER)\
                    or (old.VIDEO_CARRIER is null and new.VIDEO_CARRIER is not null)\
                    or (new.SOUND_CARRIER_PRIMARY <> old.SOUND_CARRIER_PRIMARY)\
                    or (old.SOUND_CARRIER_PRIMARY is null and new.SOUND_CARRIER_PRIMARY is not null)\
                    or (new.CHANNEL_ID <> old.CHANNEL_ID)\
                    or (old.CHANNEL_ID is null and new.CHANNEL_ID is not null)\
                    or (new.BLOCKCENTREFREQ <> old.BLOCKCENTREFREQ)\
                    or (old.BLOCKCENTREFREQ is null and new.BLOCKCENTREFREQ is not null)\
                ) then\
                begin\
                    select out_carrier, out_bandwidth from SP_SET_CARRIER_AND_BW_3(new.id, new.video_carrier, new.sound_carrier_primary, new.blockcentrefreq, new.systemcast_id, new.typesystem, new.channel_id) into :var_carrier, :var_bandwidth;\
                    new.carrier = :var_carrier;\
                    new.bandwidth = :var_bandwidth;\
                end\
            END ");

            RunQuery("ALTER PROCEDURE PR_ACTIVEVIEW_SETCHANGE (\
                IN_ADMITID INTEGER,\
                IN_TYPECHANGE VARCHAR(16),\
                IN_NAMETABLE VARCHAR(32),\
                IN_NUMCHANGE INTEGER)\
            RETURNS (\
                OUT_ID INTEGER)\
            AS\
            BEGIN\
              out_id=GEN_ID(GEN_ACTIVEVIEW_ID, 1);\
              Insert into activeview (ID, ADMIT_ID, DATECHANGE, TYPECHANGE, NAME_TABLE, NUM_CHANGE)\
                     values (:out_id, :IN_ADMITID, 'NOW', :IN_TYPECHANGE, :IN_NAMETABLE, :IN_NUMCHANGE);\
            END");

            RunQuery("\
            \
            ALTER PROCEDURE SP_ACTIVE_USER (\
                IN_TYPEACTIVE INTEGER)\
            AS\
            DECLARE VARIABLE VAR_ACTIVETYPE VARCHAR(64);\
            DECLARE VARIABLE VAR_USERID INTEGER;\
            begin\
              if (IN_TYPEACTIVE=0) then begin\
                VAR_ACTIVETYPE='Выход из системы';\
              end else begin\
                VAR_ACTIVETYPE='Вход в систему';\
              end\
              select OUT_ID from SP_GET_USERID into :VAR_USERID;\
              insert into activeview  (ID, ADMIT_ID, DATECHANGE, TYPECHANGE)\
                                       values (gen_id(gen_activeview_id, 1), :VAR_USERID,\
                                       current_timestamp, :VAR_ACTIVETYPE);\
            end");

            RunQuery("\
            \
            ALTER PROCEDURE SP_CREATE_ALLOTMENT_TX\
            AS\
            DECLARE VARIABLE VAR_ID INTEGER;\
            DECLARE VARIABLE VAR_SYS_CAST_ID INTEGER;\
            begin\
              delete from transmitters where id in (select id from dig_allotment);\
            \
              select id from systemcast where code = 'ЦBТ'  into :var_sys_cast_id;\
            \
              for select id from dig_allotment\
              where not id in (select id from transmitters)\
              into :var_id do\
              begin\
                insert into transmitters (id, latitude, longitude, systemcast_id) values (:var_id, 0, 0, :var_sys_cast_id);\
              end\
              suspend;\
            end");

            RunQuery("\
            \
            ALTER PROCEDURE SP_CREATE_LICENSES\
            AS\
            DECLARE VARIABLE NEWID INTEGER;\
            DECLARE VARIABLE TXID INTEGER;\
            DECLARE VARIABLE L VARCHAR(64);\
            DECLARE VARIABLE D DATE;\
            begin\
                delete from license;\
                for select ID, LICENSE_1, LICENSE_1_DATE from TRANSMITTERS where LICENSE_1 is not null\
                into :TXID, :L, :D\
                do begin\
                  NEWID = null;\
                  select ID from license where CODE=0 and NUMLICENSE=:L and DATEFROM=:D into :NEWID;\
                  if (NEWID is null) then\
                  begin\
                    select out_id from sp_gen_id into :NEWID;\
                    insert into LICENSE (ID, OWNER_ID, CODE, NUMLICENSE, DATEFROM) values (:NEWID, -1, 0, :L, :D);\
                  end\
                  update transmitters set LICENSE_CHANNEL_ID = :NEWID where ID = :TXID;\
                end\
                for select ID, LICENSE_2, LICENSE_2_DATE from TRANSMITTERS where LICENSE_2 is not null\
                into :TXID, :L, :D\
                do begin\
                  NEWID = null;\
                  select ID from license where CODE=1 and NUMLICENSE=:L and DATEFROM=:D into :NEWID;\
                  if (NEWID is null) then\
                  begin\
                    select out_id from sp_gen_id into :NEWID;\
                    insert into LICENSE (ID, OWNER_ID, CODE, NUMLICENSE, DATEFROM) values (:NEWID, -1, 1, :L, :D);\
                  end\
                  update transmitters set LICENSE_RFR_ID = :NEWID where ID = :TXID;\
                end\
              /* Procedure Text */\
              suspend;\
            end");

            RunQuery("\
            \
            ALTER PROCEDURE SP_CREATE_SELECTION (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION)\
            AS\
            DECLARE VARIABLE VAR_TX_LAT INTEGER;\
            DECLARE VARIABLE VAR_TX_LON INTEGER;\
            DECLARE VARIABLE VAR_AREA INTEGER;\
            DECLARE VARIABLE VAR_ACCIN INTEGER;\
            DECLARE VARIABLE VAR_ACCOUT INTEGER;\
			DECLARE VARIABLE VAR_TX_ERP double precision;\
			DECLARE VARIABLE VAR_TX_ANTHEIGHT double precision;\
			DECLARE VARIABLE VAR_SC_ID INTEGER;\
			DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;\
			DECLARE VARIABLE VAR_TX_SC_VAL_UNWANT INTEGER;\
			DECLARE VARIABLE VAR_BANDWIDTH_WANT DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_BANDWIDTH_UNWANT DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CARRIER_CURR DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CARRIER_WANT DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CARRIER_UNWANT DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_DEVIATION DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_ISINTERFERES SMALLINT;\
			DECLARE VARIABLE VAR_ISINTF_AUX SMALLINT;\
			DECLARE VARIABLE VAR_RADIUS DOUBLE PRECISION; /*meters*/\
 			DECLARE VARIABLE VAR_USE_COND_IN SMALLINT;\
            DECLARE VARIABLE VAR_USE_COND_OUT SMALLINT;\
			begin\
			    if (in_use_conditions is null) then\
			        in_use_conditions = 0;\
			    if (in_use_areas is null) then\
			        in_use_areas = 0;\
			    if (in_use_adjanced is null) then\
			        in_use_adjanced = 1;\
			    if (in_use_image is null) then\
			        in_use_image = 1;\
			    if (in_only_root is null) then\
			        in_only_root = 0;\
			\
			    /* Tx parameters */\
			    select /*TX.LATITUDE, TX.LONGITUDE,*/   \
              TX.EPR_VIDEO_MAX, TX.HEIGHTANTENNA,  SC.ID, SC.ENUMVAL, ATS.CHANNELBAND, ARS.DEVIATION, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ\
			    from TRANSMITTERS TX\
			        left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
			        left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
			        left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\
			    where TX.ID = :IN_TX_ID\
			    into /*:VAR_TX_LAT_AUX, :VAR_TX_LON_AUX,*/ \
                :VAR_TX_ERP, :VAR_TX_ANTHEIGHT, :VAR_SC_ID, :VAR_TX_SC_VAL, :VAR_BANDWIDTH_WANT, :VAR_DEVIATION, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ;\
			\
			    if ((in_use_areas = 0) and (in_radius is null or (in_radius = 0))) then begin\
			        /* get value from coordination distances*/\
			        select min(cd.OVERWARMSEA)\
			        from COORDDISTANCE cd\
			        where cd.SYSTEMCAST_ID = :VAR_SC_ID\
			            and cd.EFFECTRADIATEPOWER/*mWt*/ >= (:VAR_TX_ERP/*kWt*/  * 1000000)\
			            and cd.HEIGHTANTENNA >= :VAR_TX_ANTHEIGHT\
			        into :in_radius;\
			    end\
			    if (in_radius is null or (in_radius = 0)) then\
			        exception E_WRONG_COORD_DISTANCE;\
			\
			    /*if (in_lon is null or (in_lon = 0)) then*/\
			    VAR_TX_LON = in_lon;\
			    /*if (in_lat is null or (in_lat = 0)) then*/\
  			    VAR_TX_LAT = in_lat;\
			\
			    /*  set CARRIER and BANDWIDTH according to Tx System  */\
			    if (VAR_TX_SC_VAL = 1) then begin\
			        /* TVA */\
			        /* VAR_BANDWIDTH = ;*/\
	  		        VAR_CARRIER_WANT = VAR_VIDEO_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 2) then begin\
			        /* FM SB */\
			        VAR_BANDWIDTH_WANT = VAR_DEVIATION * 2;\
			        VAR_CARRIER_WANT = VAR_SOUND_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 4) then begin\
			        /* DVB */\
		  	        VAR_BANDWIDTH_WANT = 8.0;\
			        VAR_CARRIER_WANT = VAR_VIDEO_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 5) then begin\
			        /* DAB */\
			        VAR_BANDWIDTH_WANT = 1.8;\
			        VAR_CARRIER_WANT = VAR_BLOCKCENTREFREQ;\
			    end else begin\
			        /* unknown system */\
  			        exit;\
			    end\
			\
			    /* при планировании частота задаётся вручную */\
			    if ((IN_CARRIER is not null) and (IN_CARRIER <> 0)) then\
			        VAR_CARRIER_WANT = IN_CARRIER;\
			\
			    if (in_use_conditions = 1) then begin\
			        if (exists (select sc.CONDITION\
			                        from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
			                        where sc.SELECTION = :IN_ID\
			                        and ac.TYPECONDITION = 0))\
			                        then\
			            VAR_USE_COND_IN = 1;\
			        else\
			            VAR_USE_COND_IN = 0;\
			        if (exists (select sc.CONDITION\
			                        from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
			                        where sc.SELECTION = :IN_ID\
			                        and ac.TYPECONDITION = 1))\
			                        then\
			            VAR_USE_COND_OUT = 1;\
			        else\
			            VAR_USE_COND_OUT = 0;\
			    end\
			\
			    VAR_RADIUS = :IN_RADIUS * 1000; /*to meters*/\
			    /* fill selection */\
	  		    for select TX.ID, SD.AREA_ID, TX.ACCOUNTCONDITION_IN, TX.ACCOUNTCONDITION_OUT, SC.ENUMVAL, ATS.CHANNELBAND, ARS.DEVIATION, TX.VIDEO_CARRIER\
			    , TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ\
			    , UDF_DISTANCE(TX.LATITUDE, TX.LONGITUDE, :VAR_TX_LAT, :VAR_TX_LON) DISTANCE\
			    from TRANSMITTERS TX\
			        left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
			        left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
			        left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\
			        left outer join STAND SD on (TX.STAND_ID = SD.ID)\
			    where TX.ID <> :IN_TX_ID\
			        and (:in_only_root = 0 or (:in_only_root = 1 and (tx.ORIGINALID = 0 or (tx.ORIGINALID is null))))\
			    into :OUT_TX_ID, :VAR_AREA, :VAR_ACCIN, :VAR_ACCOUT, :VAR_TX_SC_VAL_UNWANT, :VAR_BANDWIDTH_UNWANT, :VAR_DEVIATION, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ, :OUT_DISTANCE\
			    do begin\
			\
			        VAR_ISINTERFERES = 0;\
			        /*  либо по площади, либо по расстоянию */\
			        if (in_use_areas = 1) then begin\
			            if (exists (select sa.AREA from SEL_AREA sa where sa.SELECTION = :IN_ID and sa.AREA = :VAR_AREA)) then\
			                VAR_ISINTERFERES = 1;\
			        end else begin\
			            if (OUT_DISTANCE <= :VAR_RADIUS) then\
			                VAR_ISINTERFERES = 1;\
			        end\
			\
			        /*  если выбран, смотрим, нет ли фильтра по учётным состояниям  */\
  			        if (VAR_ISINTERFERES = 1) then begin\
			\
			            if (in_use_conditions = 1) then begin\
			                if (VAR_USE_COND_IN = 1) then\
			                    if (not exists (select sc.CONDITION\
			                                from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
			                                where sc.SELECTION = :IN_ID\
			                                and sc.CONDITION = :VAR_ACCIN))\
			                    then\
			                        VAR_ISINTERFERES = 0;\
			                if (VAR_USE_COND_OUT = 1) then\
			                    if (not exists (select sc.CONDITION\
			                                from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
			                                where sc.SELECTION = :IN_ID\
			                                and sc.CONDITION = :VAR_ACCOUT))\
			                    then\
			                        VAR_ISINTERFERES = 0;\
			            end\
			\
			        end\
			\
			        /*  если таки проходит, фильтруем по частоте  */\
   			        if (VAR_ISINTERFERES = 1) then begin\
			\
			            if (VAR_TX_SC_VAL_UNWANT = 1) then begin\
			                /* TVA */\
	 		                VAR_CARRIER_UNWANT = VAR_VIDEO_CARRIER;\
			            end else if (VAR_TX_SC_VAL_UNWANT = 2) then begin\
			                /* FM SB */\
	   		                VAR_BANDWIDTH_UNWANT = VAR_DEVIATION * 2;\
			                VAR_CARRIER_UNWANT = VAR_SOUND_CARRIER;\
			            end else if (VAR_TX_SC_VAL_UNWANT = 4) then begin\
			                /* DVB */\
		 	                VAR_BANDWIDTH_UNWANT = 8.0;\
			                VAR_CARRIER_UNWANT = VAR_VIDEO_CARRIER;\
			            end else if (VAR_TX_SC_VAL_UNWANT = 5) then begin\
			                /* DAB */\
		   	                VAR_BANDWIDTH_UNWANT = 1.8;\
			                VAR_CARRIER_UNWANT = VAR_BLOCKCENTREFREQ;\
			            end else begin\
			                /* unknown system */\
			                VAR_BANDWIDTH_UNWANT = 0;\
			                VAR_CARRIER_UNWANT = 0;\
			            end\
			\
			            /* сбрасываем флаг  */\
			            VAR_ISINTERFERES = 0;\
			            select OUT_RES from SP_IS_INTERFERE(:VAR_CARRIER_WANT, :VAR_CARRIER_UNWANT,\
			                                                        :VAR_BANDWIDTH_WANT, :VAR_BANDWIDTH_UNWANT,\
			                                                        :IN_USE_ADJANCED, :IN_USE_IMAGE)\
			            into :VAR_ISINTERFERES;\
			\
			            if (VAR_ISINTERFERES = 1) then\
			                suspend;\
			\
			        end\
			    end\
			end");

			RunQuery("\
			\
			ALTER PROCEDURE SP_DATEPERM (\
			    TXID INTEGER)\
			RETURNS (\
			    DATEPERMUSEFROM_OUT TIMESTAMP,\
			    DATEPERMUSETO_OUT TIMESTAMP)\
			AS\
			declare variable dfr date;\
			declare variable dto date;\
			begin\
			  /* Procedure Text */\
			  select t.datepermusefrom, t.datepermuseto\
			  from transmitters t where t.id = :txid\
			  into :dfr, :dto;\
			\
			  datepermusefrom_out = dfr;\
			  datepermuseto_out = dto;\
			  suspend;\
			end");
			
			RunQuery("\
			\
			ALTER PROCEDURE SP_DELETE_DUPLICATES\
			AS\
			declare variable var_id integer;\
			declare variable var_maxid integer;\
			declare variable var_count integer;\
			declare variable var_reg varchar(4);\
			declare variable var_num varchar(4);\
			\
			begin\
			  /* Procedure Text */\
			\
			  for select t.id, a.numregion, t.administrationid from transmitters t, stand s, area a where\
			    (select count (*) from transmitters tt\
			        join stand ss on ss.id=tt.stand_id\
			        join area aa on ss.area_id=aa.id\
			        where aa.numregion=a.numregion\
			        and tt.administrationid=t.administrationid) > 1\
			        and s.id = t.stand_id\
			        and a.id = s.area_id\
			        order by t.administrationid into :var_id, :var_reg, :var_num\
			   do begin\
			     select count(*) from transmitters t, stand s, area a\
			        where t.stand_id = s.id\
			        and s.area_id = a.id\
			        and a.numregion = :var_reg\
			        and t.administrationid = :var_num into :var_count;\
			      if (var_count = 2) then\
			      begin\
			             select max(t.id) from transmitters t, stand s, area a\
			                where t.stand_id = s.id\
			                and s.area_id = a.id\
			                and a.numregion = :var_reg\
			                and t.administrationid = :var_num into :var_maxid;\
			             update transmitters set del = 1 where id = :var_maxid;\
			      end\
			   end\
			  suspend;\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_FILL_DM_CHANNELS\
			AS\
			declare variable var_system_id integer;\
			declare variable var_system_name char(1);\
			declare variable var_ch_id integer;\
			declare variable var_ch_name integer;\
			declare variable var_b_low double precision;\
			declare variable var_b_hig double precision;\
			declare variable var_v_car double precision;\
			declare variable var_s_car double precision;\
			declare variable var_dualfm_car double precision;\
			declare variable var_nicam_car double precision;\
			\
			declare variable var_gh_sound double precision;\
			declare variable var_g_dualfm double precision;\
			declare variable var_ghlk_nicam double precision;\
			declare variable var_i_sound double precision;\
			declare variable var_kl_sound double precision;\
			declare variable var_i_nicam double precision;\
			\
			begin /*$$IBE$$ \
			    var_ch_name = 21;\
			    var_b_low = 470;\
			    var_b_hig = 478;\
			    var_v_car = 471.25;\
			\
			    var_gh_sound = 476.75;\
			    var_g_dualfm = 476.99;\
			    var_ghlk_nicam = 477.1;\
			    var_i_sound = 477.25;\
			    var_kl_sound = 477.75;\
			    var_i_nicam = 477.8;\
			\
			    while (var_ch_name <= 81) do begin\
			        for select ID, NAMESYSTEM\
			        from ANALOGTELESYSTEM\
			        where NAMESYSTEM in ('G', 'H', 'I', 'K', 'L')\
			        into :var_system_id, :var_system_name\
			        do begin\
			            var_s_car = 0;\
			            var_dualfm_car = 0;\
			            var_nicam_car = 0;\
			            if (var_system_name = 'G' or (var_system_name = 'H')) then\
			                var_s_car = var_gh_sound;\
			            if (var_system_name = 'G') then\
			                var_dualfm_car = var_g_dualfm;\
			            if (var_system_name = 'G' or (var_system_name = 'H') or (var_system_name = 'L') or (var_system_name = 'K')) then\
			                var_nicam_car = var_ghlk_nicam;\
			            if (var_system_name = 'I') then begin\
			                var_s_car = var_i_sound;\
			                var_nicam_car = var_i_nicam;\
			            end\
			            if (var_system_name = 'K' or (var_system_name = 'L')) then\
			                var_s_car = var_kl_sound;\
			\
			            select out_id from SP_GEN_ID into :var_ch_id;\
			\
			            insert into CHANNELS (\
			                ID,\
			                ANALOGTELESYSTEM_ID,\
			                NAMECHANNEL,\
			                FREQFROM,\
			                FREQTO,\
			                FREQCARRIERVISION,\
			                FREQCARRIERSOUND,\
			                FMSOUNDCARRIERSECOND,\
			                FREQCARRIERNICAM\
			            ) values (\
			                :var_ch_id,\
			                :var_system_id,\
			                cast(:var_ch_name as char(4)),\
			                :var_b_low,\
			                :var_b_hig,\
			                :var_v_car,\
			                :var_s_car,\
			                :var_dualfm_car,\
			                :var_nicam_car\
			            );\
			        end\
			\
			        var_b_low = var_b_low + 8;\
			        var_b_hig = var_b_hig + 8;\
			        var_v_car = var_v_car + 8;\
			\
			        var_gh_sound = var_gh_sound + 8;\
			        var_g_dualfm = var_g_dualfm + 8;\
			        var_ghlk_nicam = var_ghlk_nicam + 8;\
			        var_i_sound = var_i_sound + 8;\
			        var_kl_sound = var_kl_sound + 8;\
			        var_i_nicam = var_i_nicam + 8;\
			\
			        var_ch_name = var_ch_name + 1;\
			    end\
			\
			\
			 $$IBE$$*/ EXIT;\
			end");

			RunQuery("\
			\
			ALTER PROCEDURE SP_FILL_OFFSET\
			AS\
			declare variable var_id integer;\
			declare variable var_mult integer;\
			begin\
			    var_mult = 0;\
			  for select id from OFFSETCARRYFREQTVA into :var_id do begin\
			    update OFFSETCARRYFREQTVA\
			    set\
			        OFFSET = :var_mult * 1306,\
			        OFFSETLINES = :var_mult\
			    where id = :var_id;\
			    if (var_mult = 0) then begin\
			        var_mult = 1;\
			    end else if (var_mult > 0) then begin\
			        var_mult = var_mult * -1;\
			    end else /* var_mult < 0 */begin\
			        var_mult = var_mult * -1 + 1;\
			    end\
			  end\
			END\
			");
			RunQuery("\
			\
			ALTER PROCEDURE SP_GEN_ID\
			RETURNS (\
			    OUT_ID INTEGER)\
			AS\
			begin\
			  OUT_ID = gen_id(GEN_COMMON_ID, 1);\
			  suspend;\
			end\
			");
			RunQuery("\
			\
			ALTER PROCEDURE SP_GET_USERID\
			RETURNS (\
			    OUT_ID INTEGER)\
			AS\
			begin\
			    select ID from ADMIT where LOGIN = user into :OUT_ID;\
			    if (OUT_ID is null) then begin\
			        select OUT_ID from SP_GEN_ID into :OUT_ID;\
			        insert into ADMIT (ID, LOGIN) values (:OUT_ID, user);\
			    end\
			    suspend;\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_IS_INTERFERE (\
			    IN_C1 DOUBLE PRECISION,\
			    IN_C2 DOUBLE PRECISION,\
			    IN_B1 DOUBLE PRECISION,\
			    IN_B2 DOUBLE PRECISION,\
			    IN_ADJ SMALLINT,\
			    IN_IMG SMALLINT)\
			RETURNS (\
			    OUT_RES SMALLINT)\
			AS\
			declare variable VAR_C_DIFF double precision;\
			declare variable VAR_DELTA double precision;\
			begin\
			    OUT_RES = 0;\
			    VAR_C_DIFF = IN_C1 - IN_C2;\
			\
			    if (VAR_C_DIFF < 0) then\
			        VAR_C_DIFF = VAR_C_DIFF * -1;\
			\
			    /*  co-channel */\
			    /*  carriers difference must be at least 2/3 of the bandwidth  */\
			    if (VAR_C_DIFF <= (IN_B1 + IN_B2) / 3 ) then begin\
			        OUT_RES = 1;\
			        suspend;\
			    end\
			    else if (in_adj = 1) then\
			        /*adjanced channel  */\
			        /*  carriers difference must be at least 3/2 of the bandwidth  */\
			        if (VAR_C_DIFF <= (IN_B1 + IN_B2) * 3/4) then begin\
			            OUT_RES = 1;\
			            suspend;\
			    end\
			    else if (in_img = 1) then begin\
			        /*  image-channel */\
			        /* -8 */\
			        if ((IN_C1 - IN_C2) >= (IN_B1 * 8 - IN_B1/10) and (IN_C1 - IN_C2) <= (IN_B1 * 8 + IN_B1/10)) then\
			            OUT_RES = 1;\
			        /* +8 */\
			        if ((IN_C2 - IN_C1) >= (IN_B1 * 8 - IN_B1/10) and (IN_C2 - IN_C1) <= (IN_B1 * 8 + IN_B1/10)) then\
			            OUT_RES = 1;\
			        /* -9 */\
			        if ((IN_C1 - IN_C2) >= (IN_B1 * 9 - IN_B1/10) and (IN_C1 - IN_C2) <= (IN_B1 * 9 + IN_B1/10)) then\
			            OUT_RES = 1;\
			        /* +9 */\
			        if ((IN_C2 - IN_C1) >= (IN_B1 * 9 - IN_B1/10) and (IN_C2 - IN_C1) <= (IN_B1 * 9 + IN_B1/10)) then\
			            OUT_RES = 1;\
			        suspend;\
			    end\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_RECALC_CARRIER_AND_BW\
			AS\
			declare variable var_tx_id integer;\
			declare variable var_carrier double precision;\
			declare variable var_bandwidth double precision;\
			begin\
			    for select ID from TRANSMITTERS\
			    into :VAR_TX_ID do begin\
			        select out_carrier, out_bandwidth from SP_SET_CARRIER_AND_BW(:VAR_TX_ID) into :var_carrier, :var_bandwidth;\
			        update TRANSMITTERS set carrier = :var_carrier, bandwidth = :var_bandwidth where id = :VAR_TX_ID;\
			    end\
			END\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_RECALC_CARRIER_AND_BW_3\
			AS\
			DECLARE VARIABLE VAR_TX_ID INTEGER;\
			DECLARE VARIABLE VAR_TX_SYSTEMCAST_ID INTEGER;\
			DECLARE VARIABLE VAR_TX_TYPESYSTEM INTEGER;\
			DECLARE VARIABLE VAR_TX_CHANNEL_ID INTEGER;\
			DECLARE VARIABLE VAR_TX_VIDEO_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_TX_BLOCKCENTREFREQ DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_TX_SOUND_CARRIER_PRIMARY DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_BANDWIDTH DOUBLE PRECISION;\
			begin\
			    for select\
			      ID,\
			      VIDEO_CARRIER,\
			      SOUND_CARRIER_PRIMARY,\
			      BLOCKCENTREFREQ,\
			      SYSTEMCAST_ID,\
			      TYPESYSTEM,\
			      CHANNEL_ID from TRANSMITTERS\
			    into\
			      :VAR_TX_ID,\
			      :VAR_TX_VIDEO_CARRIER,\
			      :VAR_TX_SOUND_CARRIER_PRIMARY,\
			      :VAR_TX_BLOCKCENTREFREQ,\
			      :VAR_TX_SYSTEMCAST_ID,\
			      :VAR_TX_TYPESYSTEM,\
			      :VAR_TX_CHANNEL_ID do begin\
			      select out_carrier, out_bandwidth from SP_SET_CARRIER_AND_BW_3\
			        (:VAR_TX_ID,\
			         :VAR_TX_VIDEO_CARRIER,\
			         :VAR_TX_SOUND_CARRIER_PRIMARY,\
			         :VAR_TX_BLOCKCENTREFREQ,\
			         :VAR_TX_SYSTEMCAST_ID,\
			         :VAR_TX_TYPESYSTEM,\
			         :VAR_TX_CHANNEL_ID) into :var_carrier, :var_bandwidth;\
			      update TRANSMITTERS set carrier = :var_carrier, bandwidth = :var_bandwidth where id = :VAR_TX_ID;\
			    end\
			END\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_RUS_NUMER\
			AS\
			DECLARE VARIABLE VAR_ID INTEGER;\
			DECLARE VARIABLE VAR_N INTEGER;\
			DECLARE VARIABLE VAR_NS VARCHAR(4);\
			begin\
			    var_n = 1;\
			\
			    for select t.id from transmitters t where t.stand_id in\
			    (select id from stand s where s.area_id in\
			    (select id from area a where a.numregion like '11%')\
			    ) order by t.datecreate into :var_id\
			    do begin\
			      var_ns = var_n;\
			\
			      if (var_n < 10) then var_ns = '000' || var_ns;\
			      if ((var_n >= 10) and (var_n < 100)) then var_ns = '00' || var_ns;\
			      if ((var_n >= 100) and (var_n < 1000)) then var_ns = '0' || var_ns;\
			\
			      update transmitters t set t.administrationid = :var_ns where t.id = :var_id;\
			      var_n = var_n + 1;\
			    end\
			\
			  suspend;\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_SELECT_TX_DISTANCE (\
			    IN_LAT DOUBLE PRECISION,\
			    IN_LON DOUBLE PRECISION,\
			    IN_DIF DOUBLE PRECISION)\
			RETURNS (\
			    ID INTEGER,\
			    LATITUDE DOUBLE PRECISION,\
			    LONGITUDE DOUBLE PRECISION,\
			    NAMESITE VARCHAR(32),\
			    AREA_ID INTEGER,\
			    A_NAME VARCHAR(32),\
			    A_NUMREGION VARCHAR(4),\
			    DISTRICT_ID INTEGER,\
			    D_NAME VARCHAR(32),\
			    CITY_ID INTEGER,\
			    C_NAME VARCHAR(32),\
			    STREET_ID INTEGER,\
			    ST_NAME VARCHAR(32),\
			    ADDRESS VARCHAR(64),\
			    S_HEIGHT_SEA INTEGER,\
			    DISTANCE DOUBLE PRECISION)\
			AS\
			begin\
			  /* Procedure Text */\
			for select S.ID, S.LATITUDE, S.LONGITUDE, S.NAMESITE, S.AREA_ID, A.NAME A_NAME, A.NUMREGION A_NUMREGION, S.DISTRICT_ID,\
			        D.NAME D_NAME, S.CITY_ID, C.NAME C_NAME, S.STREET_ID, ST.NAME ST_NAME, S.ADDRESS, S.HEIGHT_SEA  S_HEIGHT_SEA,\
			        UDF_DISTANCE(S.LATITUDE, S.LONGITUDE, :IN_LAT, :IN_LON) DISTANCE\
			from STAND S\
			left join AREA A on (S.AREA_ID=A.ID)\
			left join DISTRICT D on (S.DISTRICT_ID=D.ID)\
			left join CITY C on (S.CITY_ID=C.ID)\
			left join STREET ST on (S.STREET_ID=ST.ID)\
			where (S.LATITUDE between (:IN_LAT - :IN_DIF) and (:IN_LAT + :IN_DIF)) and (S.LONGITUDE between (:IN_LON - :IN_DIF) and (:IN_LON + :IN_DIF))\
			into :ID, :LATITUDE, :LONGITUDE, :NAMESITE, :AREA_ID, :A_NAME, :A_NUMREGION, :DISTRICT_ID,\
			        :D_NAME, :CITY_ID, :C_NAME, :STREET_ID, :ST_NAME, :ADDRESS, :S_HEIGHT_SEA, :DISTANCE\
			do begin\
			  suspend;\
			end\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_SET_CARRIER_AND_BW (\
			    IN_TX_ID INTEGER)\
			RETURNS (\
			    OUT_CARRIER DOUBLE PRECISION,\
			    OUT_BANDWIDTH DOUBLE PRECISION)\
			AS\
			DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;\
			DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_FREQ_BEG DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_FREQ_END DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CHANNELBAND DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_ARS_VAL INTEGER;\
			begin\
			\
			    select SC.ENUMVAL, ATS.CHANNELBAND, ARS.ENUMVAL, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ, CH.FREQFROM, CH.FREQTO\
			    from TRANSMITTERS TX\
			        left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
			        left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
			        left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\
			        left outer join CHANNELS CH on (TX.CHANNEL_ID = CH.ID)\
			    where TX.ID = :IN_TX_ID\
			    into :VAR_TX_SC_VAL, :VAR_CHANNELBAND, :VAR_ARS_VAL, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ, :VAR_FREQ_BEG, :VAR_FREQ_END;\
			\
			    if (VAR_TX_SC_VAL = 1) then begin\
			        /* TVA */\
			        OUT_CARRIER = VAR_VIDEO_CARRIER;\
			        if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\
			            OUT_BANDWIDTH = VAR_CHANNELBAND;\
			        else\
			            OUT_BANDWIDTH = 8.0;\
			    end else if (VAR_TX_SC_VAL = 2) then begin\
			        /* FM SB */\
			        OUT_CARRIER = VAR_SOUND_CARRIER;\
			        if (VAR_ARS_VAL = 0) then\
			            /* system 1 */\
			            OUT_BANDWIDTH = 0.18;\
			        else if (VAR_ARS_VAL = 1) then\
			            /* system 2 */\
			            OUT_BANDWIDTH = 0.13;\
			        else if (VAR_ARS_VAL = 2) then\
			            /* system 3 */\
			            OUT_BANDWIDTH = 0.18;\
			        else if (VAR_ARS_VAL = 3) then\
			            /* system 4 */\
			            OUT_BANDWIDTH = 0.22;\
			        else\
			            /* system 5 */\
			            OUT_BANDWIDTH = 0.3;\
			    end else if (VAR_TX_SC_VAL = 4) then begin\
			        /* DVB */\
			        OUT_CARRIER = (VAR_FREQ_BEG + VAR_FREQ_END) / 2;\
			        if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\
			            OUT_BANDWIDTH = VAR_CHANNELBAND;\
			        else\
			            OUT_BANDWIDTH = 8.0;\
			    end else if (VAR_TX_SC_VAL = 5) then begin\
			        /* DAB */\
			        OUT_BANDWIDTH = 1.8;\
			        OUT_CARRIER = VAR_BLOCKCENTREFREQ;\
			    end else begin\
			        /* unknown system */\
			        OUT_CARRIER = 0;\
			        OUT_BANDWIDTH = 0;\
			    end\
			\
			    if (OUT_CARRIER is null) then\
			        OUT_CARRIER = 0;\
			\
			    suspend;\
			\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_SET_CARRIER_AND_BW_2 (\
			    IN_TX_ID INTEGER)\
			RETURNS (\
			    OUT_CARRIER DOUBLE PRECISION,\
			    OUT_BANDWIDTH DOUBLE PRECISION)\
			AS\
			DECLARE VARIABLE VAR_BANDWIDTH DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;\
			DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_DEVIATION DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
			begin\
			    select SC.ENUMVAL, ATS.CHANNELBAND, ARS.DEVIATION, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ\
			    from TRANSMITTERS TX\
			        left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
			        left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
			        left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\
			    where TX.ID = :IN_TX_ID\
			    into :VAR_TX_SC_VAL, :VAR_BANDWIDTH, :VAR_DEVIATION, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ;\
			\
			    if (VAR_TX_SC_VAL = 1) then begin\
			        /* TVA */\
			        VAR_CARRIER = VAR_VIDEO_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 2) then begin\
			        /* FM SB */\
			        VAR_BANDWIDTH = VAR_DEVIATION * 2 / 1000;\
			        VAR_CARRIER = VAR_SOUND_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 4) then begin\
			        /* DVB */\
			        VAR_BANDWIDTH = 8.0;\
			        VAR_CARRIER = VAR_VIDEO_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 5) then begin\
			        /* DAB */\
			        VAR_BANDWIDTH = 1.8;\
			        VAR_CARRIER = VAR_BLOCKCENTREFREQ;\
			    end else begin\
			        /* unknown system */\
			        exit;\
			    end\
			\
			    OUT_CARRIER = VAR_CARRIER;\
			    OUT_BANDWIDTH = VAR_BANDWIDTH;\
			\
			    SUSPEND;\
			\
			/*\
			    update TRANSMITTERS set CARRIER = :VAR_CARRIER, BANDWIDTH = :VAR_BANDWIDTH where ID = :IN_TX_ID;\
			*/\
			\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_SET_CARRIER_AND_BW_3 (\
			    IN_TX_ID INTEGER,\
			    IN_TX_VIDEO_CARRIER DOUBLE PRECISION,\
			    IN_TX_SOUND_CARRIER DOUBLE PRECISION,\
			    IN_TX_BLOCKCENTREFREQ DOUBLE PRECISION,\
			    IN_TX_SYSTEMCAST_ID INTEGER,\
			    IN_TX_TYPESYSTEM INTEGER,\
			    IN_TX_CHANNEL_ID INTEGER)\
			RETURNS (\
			    OUT_CARRIER DOUBLE PRECISION,\
			    OUT_BANDWIDTH DOUBLE PRECISION)\
			AS\
			DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;\
			DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_FREQ_BEG DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_FREQ_END DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CHANNELBAND DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_ARS_VAL INTEGER;\
			begin\
			/***********************************************/\
			    select ENUMVAL from SYSTEMCAST where ID = :IN_TX_SYSTEMCAST_ID into :VAR_TX_SC_VAL;\
			    select CHANNELBAND from ANALOGTELESYSTEM where ID = :IN_TX_TYPESYSTEM into :VAR_CHANNELBAND;\
			    select ENUMVAL from ANALOGRADIOSYSTEM where ID = :IN_TX_TYPESYSTEM into :VAR_ARS_VAL;\
			    select FREQFROM, FREQTO from CHANNELS where ID = :IN_TX_CHANNEL_ID into :VAR_FREQ_BEG, :VAR_FREQ_END;\
			    VAR_VIDEO_CARRIER = IN_TX_VIDEO_CARRIER;\
			    VAR_SOUND_CARRIER = IN_TX_SOUND_CARRIER;\
			    VAR_BLOCKCENTREFREQ = IN_TX_BLOCKCENTREFREQ;\
			\
			/***********************************************/\
			/*\
			    select SC.ENUMVAL, ATS.CHANNELBAND, ARS.ENUMVAL, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ, CH.FREQFROM, CH.FREQTO\
			    from TRANSMITTERS TX\
			        left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
			        left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
			        left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\
			        left outer join CHANNELS CH on (TX.CHANNEL_ID = CH.ID)\
			    where TX.ID = :IN_TX_ID\
			    into :VAR_TX_SC_VAL, :VAR_CHANNELBAND, :VAR_ARS_VAL, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ, :VAR_FREQ_BEG, :VAR_FREQ_END;\
			*/\
			    if (VAR_TX_SC_VAL = 1) then begin\
			        /* TVA */\
			        OUT_CARRIER = VAR_VIDEO_CARRIER;\
			        if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\
			            OUT_BANDWIDTH = VAR_CHANNELBAND;\
			        else\
			            OUT_BANDWIDTH = 8.0;\
			    end else if (VAR_TX_SC_VAL = 2) then begin\
			        /* FM SB */\
			        OUT_CARRIER = VAR_SOUND_CARRIER;\
			        if (VAR_ARS_VAL = 0) then\
			            /* system 1 */\
			            OUT_BANDWIDTH = 0.18;\
			        else if (VAR_ARS_VAL = 1) then\
			            /* system 2 */\
			            OUT_BANDWIDTH = 0.13;\
			        else if (VAR_ARS_VAL = 2) then\
			            /* system 3 */\
			            OUT_BANDWIDTH = 0.18;\
			        else if (VAR_ARS_VAL = 3) then\
			            /* system 4 */\
			            OUT_BANDWIDTH = 0.22;\
			        else\
			            /* system 5 */\
			            OUT_BANDWIDTH = 0.3;\
			    end else if (VAR_TX_SC_VAL = 4) then begin\
			        /* DVB */\
			        OUT_CARRIER = (VAR_FREQ_BEG + VAR_FREQ_END) / 2;\
			        if (VAR_CHANNELBAND = 7.0 or VAR_CHANNELBAND = 8.0) then\
			            OUT_BANDWIDTH = VAR_CHANNELBAND;\
			        else\
			            OUT_BANDWIDTH = 8.0;\
			    end else if (VAR_TX_SC_VAL = 5) then begin\
			        /* DAB */\
			        OUT_BANDWIDTH = 1.8;\
			        OUT_CARRIER = VAR_BLOCKCENTREFREQ;\
			    end else begin\
			        /* unknown system */\
                    /* do nothing */\
			    end\
			\
			    if (OUT_CARRIER is null) then\
			        OUT_CARRIER = 0;\
			\
			    suspend;\
			\
			end\
			");

			RunQuery("\
			\
			ALTER PROCEDURE SP_SET_CARRIER_AND_BW_OLD (\
			    IN_TX_ID INTEGER)\
			AS\
			DECLARE VARIABLE VAR_BANDWIDTH DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;\
			DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_DEVIATION DOUBLE PRECISION;\
			DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
			begin\
			    select SC.ENUMVAL, ATS.CHANNELBAND, ARS.DEVIATION, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, TX.BLOCKCENTREFREQ\
			    from TRANSMITTERS TX\
			        left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
			        left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
			        left outer join ANALOGRADIOSYSTEM ARS on (TX.TYPESYSTEM = ARS.ID)\
			    where TX.ID = :IN_TX_ID\
			    into :VAR_TX_SC_VAL, :VAR_BANDWIDTH, :VAR_DEVIATION, :VAR_VIDEO_CARRIER, :VAR_SOUND_CARRIER, :VAR_BLOCKCENTREFREQ;\
			    if (VAR_TX_SC_VAL = 1) then begin\
			        /* TVA */\
			        VAR_CARRIER = VAR_VIDEO_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 2) then begin\
			        /* FM SB */\
			        VAR_BANDWIDTH = VAR_DEVIATION * 2 / 1000;\
			        VAR_CARRIER = VAR_SOUND_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 4) then begin\
			        /* DVB */\
			        VAR_BANDWIDTH = 8.0;\
			        VAR_CARRIER = VAR_VIDEO_CARRIER;\
			    end else if (VAR_TX_SC_VAL = 5) then begin\
			        /* DAB */\
			        VAR_BANDWIDTH = 1.8;\
			        VAR_CARRIER = VAR_BLOCKCENTREFREQ;\
			    end else begin\
			        /* unknown system */        \
			        exit;\
			    end\
			    if (VAR_CARRIER is null) then\
			        VAR_CARRIER = 0;\
			    if (VAR_BANDWIDTH is null) then\
			        VAR_BANDWIDTH = 0;\
			    update TRANSMITTERS set CARRIER = :VAR_CARRIER, BANDWIDTH = :VAR_BANDWIDTH where ID = :IN_TX_ID;\
			end\
			");

            RunQuery("\
            \
            ALTER PROCEDURE SP_SITE (\
                I_ID INTEGER,\
                I_TYPE INTEGER)\
            RETURNS (\
                O_STRING VARCHAR(20))\
            AS\
            declare variable v_from numeric(5,2);\
            declare variable v_to numeric(5,2);\
            declare variable v_syscast integer;\
            declare variable v_string varchar(14);\
            begin\
              v_from=0;\
              v_to=0;\
              v_syscast=0;\
              v_string='';\
              o_string='';\
              /* Procedure Text */\
              select t.systemcast_id\
               from transmitters t\
               where t.id=:i_id\
               into :v_syscast;\
            \
              if (i_type=1) then\
                begin\
                 if ((v_syscast=1) or (v_syscast=71) ) then o_string='PM';\
                  else\
                   if ((v_syscast=2) or (v_syscast=70) ) then o_string='TM';\
                end\
               else\
                if (i_type=2) then\
                  begin\
                   if ((v_syscast=1) or (v_syscast=71) ) then\
                     begin\
                      select t.sound_carrier_primary\
                       from transmitters t\
                       where t.id=:i_id\
                       into :v_from;\
                       o_string=v_from;\
                     end\
                    else\
                     if ((v_syscast=2) or (v_syscast=70) ) then\
                       begin\
                        select ch.freqfrom, ch.freqto\
                         from transmitters t, channels ch\
                         where\
                          ch.id=t.channel_id and\
                          t.id=:i_id\
                         into :v_from, :v_to;\
                        o_string=v_from || '-' || :v_to;\
                        /*o_string=v_string;*/\
                       end\
                  end\
                 else\
                  if (i_type=3) then\
                    begin\
                     if ((v_syscast=1) or (v_syscast=71) ) then\
                       begin\
                        select t.power_sound_primary\
                         from transmitters t\
                         where t.id=:i_id\
                         into :v_to;\
                         o_string=v_to;\
                       end\
                      else\
                       if ((v_syscast=2) or (v_syscast=70) ) then\
                        begin\
                         select t.power_video\
                          from transmitters t\
                          where t.id=:i_id\
                          into :v_to;\
                          o_string=v_to;\
                        end\
                    end\
                   else\
                    if (i_type=4) then\
                      begin\
                       if ((v_syscast=1) or (v_syscast=71) ) then\
                         begin\
                          select t.sound_emission_primary\
                           from transmitters t\
                           where t.id=:i_id\
                           into :o_string;\
                         end\
                        else\
                         if ((v_syscast=2) or (v_syscast=70) ) then\
                           begin\
                            select t.video_emission\
                             from transmitters t\
                             where t.id=:i_id\
                             into :o_string;\
                           end\
                      end\
            \
              suspend;\
            end");

            RunQuery("\
            \
            ALTER PROCEDURE SP_TX_SELECTION (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION)\
            AS\
            DECLARE VARIABLE VAR_TX_LAT INTEGER;\
            DECLARE VARIABLE VAR_TX_LON INTEGER;\
            DECLARE VARIABLE VAR_AREA INTEGER;\
            DECLARE VARIABLE VAR_ACCIN INTEGER;\
            DECLARE VARIABLE VAR_ACCOUT INTEGER;\
            DECLARE VARIABLE VAR_TX_ERP double precision;\
            DECLARE VARIABLE VAR_TX_ANTHEIGHT double precision;\
            DECLARE VARIABLE VAR_SC_ID INTEGER;\
            DECLARE VARIABLE VAR_TX_SC_VAL INTEGER;\
            DECLARE VARIABLE VAR_TX_SC_VAL_UNWANT INTEGER;\
            DECLARE VARIABLE VAR_BANDWIDTH_WANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_BANDWIDTH_UNWANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_CURR DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_WANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_UNWANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_DEVIATION DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_ISINTERFERES SMALLINT;\
            DECLARE VARIABLE VAR_ISINTF_AUX SMALLINT;\
            DECLARE VARIABLE VAR_RADIUS DOUBLE PRECISION; /*meters*/\
            DECLARE VARIABLE VAR_USE_COND_IN SMALLINT;\
            DECLARE VARIABLE VAR_USE_COND_OUT SMALLINT;\
            begin\
                if (in_use_conditions is null) then\
                    in_use_conditions = 0;\
                if (in_use_areas is null) then\
                    in_use_areas = 0;\
                if (in_use_adjanced is null) then\
                    in_use_adjanced = 1;\
                if (in_use_image is null) then\
                    in_use_image = 1;\
                if (in_only_root is null) then\
                    in_only_root = 0;\
            \
                /* Tx parameters */\
                select TX.EPR_VIDEO_MAX, TX.HEIGHTANTENNA, TX.SYSTEMCAST_ID, TX.CARRIER, TX.BANDWIDTH\
                from TRANSMITTERS TX\
                where TX.ID = :IN_TX_ID\
                into :VAR_TX_ERP, :VAR_TX_ANTHEIGHT, :VAR_SC_ID, :VAR_CARRIER_WANT, :VAR_BANDWIDTH_WANT;\
            \
                if ((in_use_areas = 0) and (in_radius is null or (in_radius = 0))) then begin\
                    /* get value from coordination distances*/\
                    select min(cd.OVERWARMSEA)\
                    from COORDDISTANCE cd\
                    where cd.SYSTEMCAST_ID = :VAR_SC_ID\
                        and cd.EFFECTRADIATEPOWER/*mWt*/ >= (:VAR_TX_ERP/*kWt*/ * 1000000)\
                        and cd.HEIGHTANTENNA >= :VAR_TX_ANTHEIGHT\
                    into :in_radius;\
                end\
                if (in_radius is null or (in_radius = 0)) then\
                    exception E_WRONG_COORD_DISTANCE;\
            \
                /*if (in_lon is null or (in_lon = 0)) then*/\
                VAR_TX_LON = in_lon;\
                /*if (in_lat is null or (in_lat = 0)) then*/\
                VAR_TX_LAT = in_lat;\
            \
                /* ??? ???????????? ??????? ???????? ??????? */\
                if ((IN_CARRIER is not null) and (IN_CARRIER <> 0)) then\
                    VAR_CARRIER_WANT = IN_CARRIER;\
            \
                if (in_use_conditions = 1) then begin\
                    if (exists (select sc.CONDITION\
                                    from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                    where sc.SELECTION = :IN_ID\
                                    and ac.TYPECONDITION = 0))\
                                    then\
                        VAR_USE_COND_IN = 1;\
                    else\
                        VAR_USE_COND_IN = 0;\
                    if (exists (select sc.CONDITION\
                                    from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                    where sc.SELECTION = :IN_ID\
                                    and ac.TYPECONDITION = 1))\
                                    then\
                        VAR_USE_COND_OUT = 1;\
                    else\
                        VAR_USE_COND_OUT = 0;\
                end\
            \
                VAR_RADIUS = :IN_RADIUS * 1000; /*to meters*/\
                /* fill selection */\
                for select TX.ID, SD.AREA_ID, TX.ACCOUNTCONDITION_IN, TX.ACCOUNTCONDITION_OUT, TX.CARRIER, TX.BANDWIDTH\
                , UDF_DISTANCE(TX.LATITUDE, TX.LONGITUDE, :VAR_TX_LAT, :VAR_TX_LON) DISTANCE\
                from TRANSMITTERS TX\
                    left outer join STAND SD on (TX.STAND_ID = SD.ID)\
                where (:in_only_root = 0 or (:in_only_root = 1 and (tx.ORIGINALID = 0 or (tx.ORIGINALID is null))))\
                    and (TX.ID <> :IN_TX_ID)\
                into :OUT_TX_ID, :VAR_AREA, :VAR_ACCIN, :VAR_ACCOUT, :VAR_CARRIER_UNWANT, :VAR_BANDWIDTH_UNWANT\
                , :OUT_DISTANCE\
                do begin\
            \
                    VAR_ISINTERFERES = 0;\
                    /*  ???? ?? ???????, ???? ?? ?????????? */\
                    if (in_use_areas = 1) then begin\
                        if (exists (select sa.AREA from SEL_AREA sa where sa.SELECTION = :IN_ID and sa.AREA = :VAR_AREA)) then\
                            VAR_ISINTERFERES = 1;\
                    end else begin\
                        if (OUT_DISTANCE <= :VAR_RADIUS) then\
                                        VAR_ISINTERFERES = 1;\
                    end\
            \
                    /*  ???? ??????, ???????, ??? ?? ??????? ?? ??????? ??????????  */\
                    if (VAR_ISINTERFERES = 1) then begin\
            \
                        if (in_use_conditions = 1) then begin\
                            if (VAR_USE_COND_IN = 1) then\
                                if (not exists (select sc.CONDITION\
                                            from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                            where sc.SELECTION = :IN_ID\
                                            and sc.CONDITION = :VAR_ACCIN))\
                                then\
                                    VAR_ISINTERFERES = 0;\
                                        if (VAR_USE_COND_OUT = 1) then\
                                if (not exists (select sc.CONDITION\
                                            from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                            where sc.SELECTION = :IN_ID\
                                            and sc.CONDITION = :VAR_ACCOUT))\
                                then\
                                    VAR_ISINTERFERES = 0;\
                        end\
            \
                    end\
            \
                    /*  ???? ???? ????????, ????????? ?? ???????  */\
                    if (VAR_ISINTERFERES = 1) then begin\
            \
                        /* ?????????? ????  */\
                                    VAR_ISINTERFERES = 0;\
                        select OUT_RES from SP_IS_INTERFERE(:VAR_CARRIER_WANT, :VAR_CARRIER_UNWANT,\
                                                                    :VAR_BANDWIDTH_WANT, :VAR_BANDWIDTH_UNWANT,\
                                                                    :IN_USE_ADJANCED, :IN_USE_IMAGE)\
                        into :VAR_ISINTERFERES;\
            \
                        if (VAR_ISINTERFERES = 1) then\
                            suspend;\
            \
                    end\
                end\
            end");

            RunQuery("\
            \
            ALTER PROCEDURE SP_TX_SELECTION2 (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION)\
            AS\
            DECLARE VARIABLE VAR_TX_LAT INTEGER;\
            DECLARE VARIABLE VAR_TX_LON INTEGER;\
            DECLARE VARIABLE VAR_AREA INTEGER;\
            DECLARE VARIABLE VAR_ACCIN INTEGER;\
            DECLARE VARIABLE VAR_ACCOUT INTEGER;\
            DECLARE VARIABLE VAR_TX_ERP double precision;\
            DECLARE VARIABLE VAR_TX_ANTHEIGHT double precision;\
            DECLARE VARIABLE VAR_SC_ID INTEGER;\
            DECLARE VARIABLE VAR_TX_SC_VAL_WANT INTEGER;\
            DECLARE VARIABLE VAR_TX_SC_VAL_UNWANT INTEGER;\
            DECLARE VARIABLE VAR_ATS_ENUMVAL_WANT INTEGER;\
            DECLARE VARIABLE VAR_ATS_ENUMVAL_UNWANT INTEGER;\
            DECLARE VARIABLE VAR_BANDWIDTH_WANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_BANDWIDTH_UNWANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_CURR DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_WANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_UNWANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_DEVIATION DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_VIDEO_CARRIER DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_SOUND_CARRIER DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_BLOCKCENTREFREQ DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_ISINTERFERES SMALLINT;\
            DECLARE VARIABLE VAR_ISINTF_AUX SMALLINT;\
            DECLARE VARIABLE VAR_RADIUS DOUBLE PRECISION; /*meters*/\
            DECLARE VARIABLE VAR_USE_COND_IN SMALLINT;\
            DECLARE VARIABLE VAR_USE_COND_OUT SMALLINT;\
            DECLARE VARIABLE VAR_ISINTERF INTEGER;\
            begin\
                if (in_use_conditions is null) then\
                    in_use_conditions = 0;\
                if (in_use_areas is null) then\
                    in_use_areas = 0;\
                if (in_use_adjanced is null) then\
                    in_use_adjanced = 1;\
                if (in_use_image is null) then\
                    in_use_image = 1;\
                if (in_only_root is null) then\
                    in_only_root = 0;\
            \
                /* Tx parameters */\
                select TX.EPR_VIDEO_MAX, TX.HEIGHTANTENNA, TX.SYSTEMCAST_ID, TX.CARRIER, TX.BANDWIDTH, ATS.ENUMVAL, SC.ENUMVAL\
                from TRANSMITTERS TX\
                    left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
                    left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
                where TX.ID = :IN_TX_ID\
                into :VAR_TX_ERP, :VAR_TX_ANTHEIGHT, :VAR_SC_ID, :VAR_CARRIER_WANT, :VAR_BANDWIDTH_WANT, :VAR_ATS_ENUMVAL_WANT, :VAR_TX_SC_VAL_WANT;\
            \
            \
            \
                if ((in_use_areas = 0) and (in_radius is null or (in_radius = 0))) then begin\
                    /* get value from coordination distances*/\
                    select min(cd.OVERWARMSEA)\
                    from COORDDISTANCE cd\
                    where cd.SYSTEMCAST_ID = :VAR_SC_ID\
                        and cd.EFFECTRADIATEPOWER/*mWt*/ >= (:VAR_TX_ERP/*kWt*/ * 1000000)\
                        and cd.HEIGHTANTENNA >= :VAR_TX_ANTHEIGHT\
                    into :in_radius;\
                end\
                if (in_radius is null or (in_radius = 0)) then\
                    exception E_WRONG_COORD_DISTANCE;\
            \
                /*if (in_lon is null or (in_lon = 0)) then*/\
                VAR_TX_LON = in_lon;\
                /*if (in_lat is null or (in_lat = 0)) then*/\
                VAR_TX_LAT = in_lat;\
            \
                /* ??? ???????????? ??????? ???????? ??????? */\
                if ((IN_CARRIER is not null) and (IN_CARRIER <> 0)) then\
                    VAR_CARRIER_WANT = IN_CARRIER;\
            \
                if (in_use_conditions = 1) then begin\
                    if (exists (select sc.CONDITION\
                                    from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                    where sc.SELECTION = :IN_ID\
                                    and ac.TYPECONDITION = 0))\
                                    then\
                        VAR_USE_COND_IN = 1;\
                    else\
                        VAR_USE_COND_IN = 0;\
                    if (exists (select sc.CONDITION\
                                    from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                    where sc.SELECTION = :IN_ID\
                                    and ac.TYPECONDITION = 1))\
                                    then\
                        VAR_USE_COND_OUT = 1;\
                    else\
                        VAR_USE_COND_OUT = 0;\
                end\
            \
                VAR_RADIUS = :IN_RADIUS * 1000; /*to meters*/\
                /* fill selection */\
                for select TX.ID, SD.AREA_ID, TX.ACCOUNTCONDITION_IN, TX.ACCOUNTCONDITION_OUT, TX.CARRIER, TX.BANDWIDTH\
                    , UDF_DISTANCE(TX.LATITUDE, TX.LONGITUDE, :VAR_TX_LAT, :VAR_TX_LON) DISTANCE, ATS.ENUMVAL\
                from TRANSMITTERS TX\
                    left outer join STAND SD on (TX.STAND_ID = SD.ID)\
                    left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
                    left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
                where (:in_only_root = 0 or (:in_only_root = 1 and (tx.ORIGINALID = 0 or (tx.ORIGINALID is null))))\
                    and (TX.ID <> :IN_TX_ID)\
                    and (UDF_ISINTERFERE(:VAR_TX_SC_VAL_WANT, :VAR_ATS_ENUMVAL_WANT, :VAR_CARRIER_WANT, :VAR_BANDWIDTH_WANT,\
                               SC.ENUMVAL, ATS.ENUMVAL, TX.CARRIER, TX.BANDWIDTH) = 1)\
                into :OUT_TX_ID, :VAR_AREA, :VAR_ACCIN, :VAR_ACCOUT, :VAR_CARRIER_UNWANT, :VAR_BANDWIDTH_UNWANT\
                    , :OUT_DISTANCE, :VAR_ATS_ENUMVAL_UNWANT\
                do begin\
            \
                    VAR_ISINTERFERES = 0;\
                    /*  using areas */\
                    if (in_use_areas = 1) then begin\
                        if (exists (select sa.AREA from SEL_AREA sa where sa.SELECTION = :IN_ID and sa.AREA = :VAR_AREA)) then\
                            VAR_ISINTERFERES = 1;\
                    end else begin\
                        if (OUT_DISTANCE <= :VAR_RADIUS) then\
                            VAR_ISINTERFERES = 1;\
                    end\
            \
                    /*  using conditions  */\
                    if (VAR_ISINTERFERES = 1) then begin\
            \
                        if (in_use_conditions = 1) then begin\
                            if (VAR_USE_COND_IN = 1) then\
                                if (not exists (select sc.CONDITION\
                                            from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                            where sc.SELECTION = :IN_ID\
                                            and sc.CONDITION = :VAR_ACCIN))\
                                then\
                                    VAR_ISINTERFERES = 0;\
                            if (VAR_USE_COND_OUT = 1) then\
                                if (not exists (select sc.CONDITION\
                                            from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                            where sc.SELECTION = :IN_ID\
                                            and sc.CONDITION = :VAR_ACCOUT))\
                                then\
                                    VAR_ISINTERFERES = 0;\
                        end\
                    end\
            \
                    if (VAR_ISINTERFERES = 1) then\
                        suspend;\
            \
                end\
            END");

            RunQuery("\
            \
            ALTER PROCEDURE SP_TX_SELECTION3 (\
                IN_ID INTEGER,\
                IN_TX_ID INTEGER,\
                IN_RADIUS INTEGER,\
                IN_LON DOUBLE PRECISION,\
                IN_LAT DOUBLE PRECISION,\
                IN_USE_CONDITIONS SMALLINT,\
                IN_USE_AREAS SMALLINT,\
                IN_USE_ADJANCED SMALLINT,\
                IN_USE_IMAGE SMALLINT,\
                IN_ONLY_ROOT SMALLINT,\
                IN_CARRIER DOUBLE PRECISION)\
            RETURNS (\
                OUT_TX_ID INTEGER,\
                OUT_DISTANCE DOUBLE PRECISION,\
                OUT_STATUS DOUBLE PRECISION,\
                OUT_AREA VARCHAR(4))\
            AS\
            DECLARE VARIABLE VAR_TX_LAT INTEGER;\
            DECLARE VARIABLE VAR_TX_LON INTEGER;\
            DECLARE VARIABLE VAR_AREA INTEGER;\
            DECLARE VARIABLE VAR_ACCIN INTEGER;\
            DECLARE VARIABLE VAR_ACCOUT INTEGER;\
            DECLARE VARIABLE VAR_TX_ERP DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_TX_ANTHEIGHT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_SC_ID INTEGER;\
            DECLARE VARIABLE VAR_TX_SC_VAL_WANT INTEGER;\
            DECLARE VARIABLE VAR_ATS_ENUMVAL_WANT INTEGER;\
            DECLARE VARIABLE VAR_ATS_ENUMVAL_UNWANT INTEGER;\
            DECLARE VARIABLE VAR_BANDWIDTH_WANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_BANDWIDTH_UNWANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_WANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_CARRIER_UNWANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_ISINTERFERES SMALLINT;\
            DECLARE VARIABLE VAR_USE_COND_IN SMALLINT;\
            DECLARE VARIABLE VAR_USE_COND_OUT SMALLINT;\
            DECLARE VARIABLE VAR_COORD_DIST_WANT DOUBLE PRECISION;\
            DECLARE VARIABLE VAR_COORD_DIST_UNWANT DOUBLE PRECISION;\
            begin\
                if (in_use_conditions is null) then\
                    in_use_conditions = 0;\
                if (in_use_areas is null) then\
                    in_use_areas = 0;\
                if (in_use_adjanced is null) then\
                    in_use_adjanced = 1;\
                if (in_use_image is null) then\
                    in_use_image = 1;\
                if (in_only_root is null) then\
                    in_only_root = 0;\
            \
                /* Tx parameters */\
                select TX.EPR_VIDEO_MAX, TX.HEIGHTANTENNA, TX.SYSTEMCAST_ID, TX.CARRIER, TX.BANDWIDTH, ATS.ENUMVAL, SC.ENUMVAL, TX.MAX_COORD_DIST\
                from TRANSMITTERS TX\
                    left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
                    left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
                where TX.ID = :IN_TX_ID\
                into /*:VAR_TX_LAT_AUX, :VAR_TX_LON_AUX,*/ :VAR_TX_ERP, :VAR_TX_ANTHEIGHT, :VAR_SC_ID, :VAR_CARRIER_WANT, :VAR_BANDWIDTH_WANT, :VAR_ATS_ENUMVAL_WANT, :VAR_TX_SC_VAL_WANT, :VAR_COORD_DIST_WANT;\
            \
            \
                if (in_radius is null) then\
                    in_radius = 0;\
            \
                /* if ((in_use_areas = 0) and (in_radius = 0)) then begin */\
                    /* get value from coordination distances*/\
                    /*select min(cd.OVERWARMSEA)\
                    from COORDDISTANCE cd\
                    where cd.SYSTEMCAST_ID = :VAR_SC_ID\
                        and cd.EFFECTRADIATEPOWER */ /*mWt*/ /* >= (:VAR_TX_ERP */ /*kWt*/ /*  * 1000000)\
                        and cd.HEIGHTANTENNA >= :VAR_TX_ANTHEIGHT\
                    into :in_radius;\
                end\
                if (in_radius is null or (in_radius = 0)) then\
                    exception E_WRONG_COORD_DISTANCE; */\
            \
                /*if (in_lon is null or (in_lon = 0)) then*/\
                VAR_TX_LON = in_lon;\
                /*if (in_lat is null or (in_lat = 0)) then*/\
                VAR_TX_LAT = in_lat;\
            \
                /* implicit carrier for plaanning selections */\
                if ((IN_CARRIER is not null) and (IN_CARRIER <> 0)) then\
                    VAR_CARRIER_WANT = IN_CARRIER;\
            \
                if (in_use_conditions = 1) then begin\
                    if (exists (select sc.CONDITION\
                                    from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                    where sc.SELECTION = :IN_ID\
                                    and ac.TYPECONDITION = 0))\
                                    then\
                        VAR_USE_COND_IN = 1;\
                    else\
                        VAR_USE_COND_IN = 0;\
                    if (exists (select sc.CONDITION\
                                    from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                    where sc.SELECTION = :IN_ID\
                                    and ac.TYPECONDITION = 1))\
                                    then\
                        VAR_USE_COND_OUT = 1;\
                    else\
                        VAR_USE_COND_OUT = 0;\
                end\
            \
                /* VAR_RADIUS = :IN_RADIUS * 1000; */ /*to meters*/\
                /* fill selection */\
                for select TX.ID, SD.AREA_ID, TX.ACCOUNTCONDITION_IN, TX.ACCOUNTCONDITION_OUT, TX.CARRIER, TX.BANDWIDTH\
                    , UDF_DISTANCE(TX.LATITUDE, TX.LONGITUDE, :VAR_TX_LAT, :VAR_TX_LON) DISTANCE, ATS.ENUMVAL\
                    , TX.MAX_COORD_DIST, TX.STATUS\
                from TRANSMITTERS TX\
                    left outer join STAND SD on (TX.STAND_ID = SD.ID)\
                    left outer join ANALOGTELESYSTEM ATS on (TX.TYPESYSTEM = ATS.ID)\
                    left outer join SYSTEMCAST SC on (TX.SYSTEMCAST_ID = SC.ID)\
            where (:in_only_root = 0 or (:in_only_root = 1 and (tx.ORIGINALID = 0 or (tx.ORIGINALID is null))))\
               and (TX.ID <> :IN_TX_ID)\
               and (UDF_ISINTERFERE(:VAR_TX_SC_VAL_WANT, :VAR_ATS_ENUMVAL_WANT, :VAR_CARRIER_WANT,\
                    :VAR_BANDWIDTH_WANT, SC.ENUMVAL, ATS.ENUMVAL, TX.CARRIER, TX.BANDWIDTH) = 1\
                 or UDF_ISINTERFERE(SC.ENUMVAL, ATS.ENUMVAL, TX.CARRIER, TX.BANDWIDTH,\
                    :VAR_TX_SC_VAL_WANT, :VAR_ATS_ENUMVAL_WANT, :VAR_CARRIER_WANT, :VAR_BANDWIDTH_WANT) = 1)\
                into :OUT_TX_ID, :VAR_AREA, :VAR_ACCIN, :VAR_ACCOUT, :VAR_CARRIER_UNWANT, :VAR_BANDWIDTH_UNWANT\
                    , :OUT_DISTANCE, :VAR_ATS_ENUMVAL_UNWANT, :VAR_COORD_DIST_UNWANT, :OUT_STATUS\
                do begin\
            \
                    /* выбираем название региона чтобы можно было отсеить БРИФИКи*/\
            \
                    select area.numregion from area where area.id = :var_area into :out_area;\
            \
                    VAR_ISINTERFERES = 0;\
                    /*  using areas */\
                    if (in_use_areas = 1) then begin\
                        if (exists (select sa.AREA from SEL_AREA sa where sa.SELECTION = :IN_ID and sa.AREA = :VAR_AREA)) then\
                            VAR_ISINTERFERES = 1;\
                    end else begin\
                        /* distancies */\
                        if (OUT_DISTANCE <= VAR_COORD_DIST_WANT * 1000 or OUT_DISTANCE <= VAR_COORD_DIST_UNWANT * 1000) then begin\
                            if (IN_RADIUS > 0) then begin\
                                if (OUT_DISTANCE <= IN_RADIUS * 1000) then\
                                    VAR_ISINTERFERES = 1;\
                            end else\
                                VAR_ISINTERFERES = 1;\
                        end\
                    end\
            \
                    /*  using conditions  */\
                    if (VAR_ISINTERFERES = 1) then begin\
            \
                        if (in_use_conditions = 1) then begin\
                            if (VAR_USE_COND_IN = 1) then\
                                if (not exists (select sc.CONDITION\
                                            from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                            where sc.SELECTION = :IN_ID\
                                            and sc.CONDITION = :VAR_ACCIN))\
                                then\
                                    VAR_ISINTERFERES = 0;\
                            if (VAR_USE_COND_OUT = 1) then\
                                if (not exists (select sc.CONDITION\
                                            from SEL_CONDITION sc left outer join ACCOUNTCONDITION ac on (sc.CONDITION = ac.ID)\
                                            where sc.SELECTION = :IN_ID\
                                            and sc.CONDITION = :VAR_ACCOUT))\
                                then\
                                    VAR_ISINTERFERES = 0;\
                        end\
                    end\
            \
                    if (VAR_ISINTERFERES = 1) then\
                        suspend;\
            \
                end\
                   /* extract allotments  */\
                 OUT_AREA = '';\
            \
                 for select a.id, a.DB_SECTION_ID, min(UDF_DISTANCE(p.LAT, p.LON,\
            :VAR_TX_LAT, :VAR_TX_LON))\
                 from DIG_SUBAREAPOINT p\
                     left join DIG_ALLOT_CNTR_LNK l on (p.CONTOUR_ID = l.CNTR_ID)\
                     left join DIG_ALLOTMENT a on (l.ALLOT_ID = a.id)\
                 where\
                     (a.FREQ_ASSIGN = :VAR_CARRIER_WANT\
                       or\
                       (a.NOTICE_TYPE = 'GS2' or a.NOTICE_TYPE = 'DS2')\
                         and (\
                           a.FREQ_ASSIGN > :VAR_CARRIER_WANT and (a.FREQ_ASSIGN -\
            :VAR_CARRIER_WANT) <\
            (:VAR_BANDWIDTH_WANT / 2 + 0.9)\
                           or (a.FREQ_ASSIGN < :VAR_CARRIER_WANT and (:VAR_CARRIER_WANT -\
            a.FREQ_ASSIGN) <\
            (:VAR_BANDWIDTH_WANT / 2 + 0.9))\
                           )\
                       or (a.NOTICE_TYPE = 'GT2' or a.NOTICE_TYPE = 'DT2')\
                         and (\
                           a.FREQ_ASSIGN > :VAR_CARRIER_WANT and (a.FREQ_ASSIGN -\
            :VAR_CARRIER_WANT) <\
            (:VAR_BANDWIDTH_WANT / 2  + 3.99)\
                           or (a.FREQ_ASSIGN < :VAR_CARRIER_WANT and (:VAR_CARRIER_WANT -\
            a.FREQ_ASSIGN) <\
            (:VAR_BANDWIDTH_WANT / 2 + 3.99))\
                           )\
                     )\
                     and UDF_DISTANCE(p.LAT, p.LON, :VAR_TX_LAT, :VAR_TX_LON) <= :IN_RADIUS\
            * 1000\
                 group by a.id, a.DB_SECTION_ID     into :OUT_TX_ID, :OUT_STATUS, OUT_DISTANCE\
                 do\
                     suspend;\
            END");

            CreateDescription(uoTrigger,"TR_TRANSMITTERS_AI_0","Выход из системы");

            CreateDescription(uoDomain,"DM_ADDRESS","Адрес");
			CreateDescription(uoDomain,"DM_ANGLE","Угол");
			CreateDescription(uoDomain,"DM_BLOB","Тип Блоб");
			CreateDescription(uoDomain,"DM_BOOLEAN","Булевский тип");
			CreateDescription(uoDomain,"DM_CLASSWAVE","Класс волн");
			CreateDescription(uoDomain,"DM_COORDDOCUMENT","Обязательность координации");
			CreateDescription(uoDomain,"DM_DATE","Дата");
			CreateDescription(uoDomain,"DM_DATETIME","Дата и время");
			CreateDescription(uoDomain,"DM_DBELL","Децибелл");
			CreateDescription(uoDomain,"DM_DBKVT","Децибел/киловат");
			CreateDescription(uoDomain,"DM_DEGREE","Градус");
			CreateDescription(uoDomain,"DM_DIRECTION","Направленность антенны");
			CreateDescription(uoDomain,"DM_DISTANCEKMETR","Дистанция в километрах");
			CreateDescription(uoDomain,"DM_DOUBLEPRECISION","Тип Double");
			CreateDescription(uoDomain,"DM_GEOPOINT","Географические координаты");
			CreateDescription(uoDomain,"DM_HEIGHTMETR","Высота всего что измеряется в метрах");
			CreateDescription(uoDomain,"DM_HERZ","Герцы");
			CreateDescription(uoDomain,"DM_IDENTY_FK","Forign Key");
			CreateDescription(uoDomain,"DM_IDENTY_PK","Primary Key");
			CreateDescription(uoDomain,"DM_INTEGER","Тип Integer");
			CreateDescription(uoDomain,"DM_KHERZ","Килогерц");
			CreateDescription(uoDomain,"DM_KMETR","Километр");
			CreateDescription(uoDomain,"DM_KWATT","Кило/ватт");
			CreateDescription(uoDomain,"DM_LOGIN","Логин");
			CreateDescription(uoDomain,"DM_MBIT","МегаБит");
			CreateDescription(uoDomain,"DM_METR","Метр");
			CreateDescription(uoDomain,"DM_MHERZ","МегаГерц");
			CreateDescription(uoDomain,"DM_MICROSEC","Микро - секунды");
			CreateDescription(uoDomain,"DM_MICROWATT","микро ватт");
			CreateDescription(uoDomain,"DM_MILLIWATT","Милливатты");
			CreateDescription(uoDomain,"DM_NAMECHANNEL","Наименование канала");
			CreateDescription(uoDomain,"DM_NAMEPROGRAMM","Наименование программы");
			CreateDescription(uoDomain,"DM_OFFSETTYPE","Типы смещения");
			CreateDescription(uoDomain,"DM_PHONE","Номер телефона");
			CreateDescription(uoDomain,"DM_POLARIZATION","Поляризация антенны");
			CreateDescription(uoDomain,"DM_SECONDSOUND","Вторая система звука");
			CreateDescription(uoDomain,"DM_SMALLINT","Тип smallInt");
			CreateDescription(uoDomain,"DM_STABILITY","Тип нестабильности частоты");
			CreateDescription(uoDomain,"DM_SYSTEMCOLOUR","Система цвета");
			CreateDescription(uoDomain,"DM_TIME","Время");
			CreateDescription(uoDomain,"DM_VARCHAR16","Тип VARCHAR16");
			CreateDescription(uoDomain,"DM_VARCHAR256","Тип VARCHAR256");
			CreateDescription(uoDomain,"DM_VARCHAR30","Тип VARCHAR30");
			CreateDescription(uoDomain,"DM_VARCHAR32","Тип VARCHAR32");
			CreateDescription(uoDomain,"DM_VARCHAR4","Тип VARCHAR4");
			CreateDescription(uoDomain,"DM_VARCHAR64","Тип VARCHAR64");
			CreateDescription(uoDomain,"DM_VARCHAR8","Тип VARCHAR8");
			CreateDescription(uoDomain,"DM_VARCHARBIG","Тип VARCHAR16384");
			CreateDescription(uoDomain,"DM_WATT","Ватт");
			
			CreateDescription(uoTable,"ACCOUNTCONDITION","Учетное состояние (внутреннее, внешнее)");
			CreateDescription(uoTable,"ACTIVEVIEW","Журнал активности операторов");
			CreateDescription(uoTable,"ADMIT","Таблица прав пользователей");
			CreateDescription(uoTable,"ALLOTMENTBLOCKDAB","Частотные блоки распределения DAB");
			CreateDescription(uoTable,"ANALOGRADIOSYSTEM","Системы аналогового радиовещания");
			CreateDescription(uoTable,"ANALOGTELESYSTEM","Системы аналогового телевидения");
			CreateDescription(uoTable,"AREA","Области (Регионы)");
			CreateDescription(uoTable,"ARHIVE","Архивные данные");
			CreateDescription(uoTable,"BANK","Банки");
			CreateDescription(uoTable,"BLOCKDAB","Блоки цифровых вещательных присвоений");
			CreateDescription(uoTable,"CARRIERGUARDINTERVAL","Несущие частоты и защитные интервалы TVD");
			CreateDescription(uoTable,"CHANELSRETRANSLATE","Каналы ретрансляции");
			CreateDescription(uoTable,"CHANNELS","Каналы");
			CreateDescription(uoTable,"CITY","Города");
			CreateDescription(uoTable,"COORDDISTANCE","Координационные расстояния");
			CreateDescription(uoTable,"COORDINATION","координация присвоения с телекоморганизациями");
			CreateDescription(uoTable,"COORDPOINTS","Географические координаты границ государств (Сегменты)");
			CreateDescription(uoTable,"COUNTRY","Страны");
			CreateDescription(uoTable,"COUNTRYCOORDINATION","Координирующие страны");
			CreateDescription(uoTable,"COUNTRYPOINTS","Кросс таблица между COORDPOINTS и COUNTRY (сегменты связаны со странами)");
			CreateDescription(uoTable,"DIGITALTELESYSTEM","Системы модуляции цифрового телевидения");
			CreateDescription(uoTable,"DISTRICT","Районы");
			CreateDescription(uoTable,"DOCUMENT","Типы документов");
			CreateDescription(uoTable,"EQUIPMENT","Данные по оборудованию");
			CreateDescription(uoTable,"FREQUENCYGRID","частотные сетки каналов");
			CreateDescription(uoTable,"LETTERS","Исходящие и входящие письма");
			CreateDescription(uoTable,"LICENSE","Список лицензий");
			CreateDescription(uoTable,"MINSTRENGTHFIELD","Минимальные используемые напряженности поля");
			CreateDescription(uoTable,"OFFSETCARRYFREQTVA","Типы смещения несущей для аналоговых ТВ");
			CreateDescription(uoTable,"OWNER","Владельцы лицензий");
			CreateDescription(uoTable,"PROTECTRATIOESDAB","Защитные отношения для цифрового звука");
			CreateDescription(uoTable,"PROTECTRATIOESDVB","Защитные отношения для цифрового ТВ");
			CreateDescription(uoTable,"PROTECTRATIOESTVA","Защитные отношения для TVA");
			CreateDescription(uoTable,"PROTECTRATIOESVHFSB","Защитные отношения для аналогового звука");
			CreateDescription(uoTable,"RADIOSERVICE","Диапазоны радиослужб");
			CreateDescription(uoTable,"SELECTEDTRANSMITTERS","Таблица передатчиков участвующая в планировании");
			CreateDescription(uoTable,"SELECTIONS","Таблица выборок");
			CreateDescription(uoTable,"STAND","Опоры");
			CreateDescription(uoTable,"STREET","Улицы");
			CreateDescription(uoTable,"SYNHROFREQNET","Синхронные сети");
			CreateDescription(uoTable,"SYSTEMCAST","Системы вещания");
			CreateDescription(uoTable,"TELECOMORGANIZATION","Организации");
			CreateDescription(uoTable,"TESTPOINTS","Тестовые точки");
			CreateDescription(uoTable,"TESTPOINTTRANSMITTERS","Привязка передатчиков которые создают наибольшую помеху на контрольную точку");
			CreateDescription(uoTable,"TRANSMITTEREQUIPMENT","Список оборудования передатчика");
			CreateDescription(uoTable,"TRANSMITTERS","Передатчики");
			CreateDescription(uoTable,"TYPERECEIVE","Типы приема");
			CreateDescription(uoTable,"TYPESYNHRONET","Типы синхронных сетей");
			CreateDescription(uoTable,"UNCOMPATIBLECHANNELS","Несовместимые каналы для кабельного ТВ");
			
			CreateDescription(uoFunction,"BF_BLOB_TO_STR","Преобразование блоба в строку");
			CreateDescription(uoFunction,"BF_COMPARE_BLOB","сравнение двух блобов");
			CreateDescription(uoFunction,"UDF_DISTANCE","расстояние между двумя точками на сфере. координаты - широта и долгота");
			
			CreateDescription(uoStoredProc,"PR_ACTIVEVIEW_SETCHANGE","Измененные данные заносятся в архив");
			
			CreateDescription(uoTrigger,"TR_TRANSMITTERS_AI_0","Выход из системы");
			
			CreateDescription(uoTableField,"CODE","Код состояния","ACCOUNTCONDITION");
			CreateDescription(uoTableField,"NAME","Наименование состояния","ACCOUNTCONDITION");
			CreateDescription(uoTableField,"TYPECONDITION","Тип состояния (внутреннее - 0, международное - 1)","ACCOUNTCONDITION");
			CreateDescription(uoTableField,"ADMIT_ID","Идентификатор пользователя","ACTIVEVIEW");
			CreateDescription(uoTableField,"DATECHANGE","Дата и время изменения","ACTIVEVIEW");
			CreateDescription(uoTableField,"NAME_TABLE","Имя измененной таблицы","ACTIVEVIEW");
			CreateDescription(uoTableField,"NUM_CHANGE","Номер измененной записи","ACTIVEVIEW");
			CreateDescription(uoTableField,"TYPECHANGE","Тип изменений","ACTIVEVIEW");
			CreateDescription(uoTableField,"LOGIN","Логин пользователя","ADMIT");
			CreateDescription(uoTableField,"NAME","Имя пользователя","ADMIT");
			CreateDescription(uoTableField,"STATUS","Статус пользователя","ADMIT");
			CreateDescription(uoTableField,"AGREEMENTNUMBER","Номера соглашений между Администрациями","ALLOTMENTBLOCKDAB");
			CreateDescription(uoTableField,"ASSIGNMENTIS","Блок распределенный или присвоенный","ALLOTMENTBLOCKDAB");
			CreateDescription(uoTableField,"BLOCKDAB_ID","Идентификатор частотного блока (таблица BLOCKDAB)","ALLOTMENTBLOCKDAB");
			CreateDescription(uoTableField,"IDENTIFIER","Идентификатор присвоеного блока","ALLOTMENTBLOCKDAB");
			CreateDescription(uoTableField,"NAME","Наименование присвоеного блока","ALLOTMENTBLOCKDAB");
			CreateDescription(uoTableField,"REMARKS","Замечания","ALLOTMENTBLOCKDAB");
			CreateDescription(uoTableField,"TYPEREFERENCENETWORK","Тип сети","ALLOTMENTBLOCKDAB");
			CreateDescription(uoTableField,"DEVIATION","Девиация (+-50 или +-75)","ANALOGRADIOSYSTEM");
			CreateDescription(uoTableField,"MODULATION","Вид модуляции для стерео-систем (Pilot-ton, Polar)","ANALOGRADIOSYSTEM");
			CreateDescription(uoTableField,"TYPESYSTEM","Тип системы (моно, стерео)","ANALOGRADIOSYSTEM");
			CreateDescription(uoTableField,"CHANNELBAND","Полоса канала (мГц)","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"NAMESYSTEM","Наименование системы","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"NUMBERLINES","Количество линий","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"SEPARATEVIDEOSOUND1","Разница между частотой видео и звука Primary (мГц)","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"SEPARATEVIDEOSOUND2","Разница между частотой видео и звука Second (мГц)","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"SOUND1MODULATION","Тип модуляции частоты звука Primary","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"SOUND2SYSTEM","Система звука Second ","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"VESTIGIALBAND","Остаточная ширина","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"VIDEOBAND","Ширина полосы видео (мГц)","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"VIDEOMODULATION","Тип модуляции частоты видео ","ANALOGTELESYSTEM");
			CreateDescription(uoTableField,"COUNTRY_ID","Идентификатор страны","AREA");
			CreateDescription(uoTableField,"NAME","Название области","AREA");
			CreateDescription(uoTableField,"NUMREGION","Номер области (региона)- общеевропейский","AREA");
			CreateDescription(uoTableField,"ACTIVEVIEW_ID","Идентификатор изменяемой записи","ARHIVE");
			CreateDescription(uoTableField,"NAMEFIELD","Наименование изменяемого поля","ARHIVE");
			CreateDescription(uoTableField,"OLDDATA_BLOB","Старые данные для всех полей типа блоб","ARHIVE");
			CreateDescription(uoTableField,"OLDDATA_CHAR","Старые данные для всех полей кроме блоба","ARHIVE");
			CreateDescription(uoTableField,"ADDRESS","Адрес","BANK");
			CreateDescription(uoTableField,"MFO","МФО","BANK");
			CreateDescription(uoTableField,"NAME","Наименование","BANK");
			CreateDescription(uoTableField,"CENTREFREQ","Центральная частота","BLOCKDAB");
			CreateDescription(uoTableField,"FREQFROM","Диапазон блока (от)","BLOCKDAB");
			CreateDescription(uoTableField,"FREQTO","Диапазон блока (до)","BLOCKDAB");
			CreateDescription(uoTableField,"LOWERGUARDBAND","Нижняя защищенная граница","BLOCKDAB");
			CreateDescription(uoTableField,"NAME","Наименование блока","BLOCKDAB");
			CreateDescription(uoTableField,"UPPERGUARDBAND","Верхняя защищенная граница","BLOCKDAB");
			CreateDescription(uoTableField,"CODE","Код","CARRIERGUARDINTERVAL");
			CreateDescription(uoTableField,"FREQBOUNDINTERVAL","Интервал между крайними несущими","CARRIERGUARDINTERVAL");
			CreateDescription(uoTableField,"FREQINTERVAL","Интервал между несущими ","CARRIERGUARDINTERVAL");
			CreateDescription(uoTableField,"NAMECURRIERGUARD","Наименование защитного интервала","CARRIERGUARDINTERVAL");
			CreateDescription(uoTableField,"NUMBERCARRIER","количество несущих","CARRIERGUARDINTERVAL");
			CreateDescription(uoTableField,"TIMECURRIERGUARD","время защитного интервала","CARRIERGUARDINTERVAL");
			CreateDescription(uoTableField,"TIMEUSEFULINTERVAL","Время полезного интервала","CARRIERGUARDINTERVAL");
			CreateDescription(uoTableField,"NAMEPROGRAMM","Наименование программы","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"RX_CHANNEL","Канал приема","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"RX_FREQUENCY","Частота приема","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор ретранслятора (передатчика)","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"TX_CHANNEL","Канал передачи","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"TX_FREQUENCY","Частота передачи","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"TYPERECEIVE_ID","Тип приема","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"TYPESYSTEM","Тип системы (Частота - 0 , Канал - 1)","CHANELSRETRANSLATE");
			CreateDescription(uoTableField,"FMSOUNDCARRIERSECOND","Частота несущей (звук Second Dual FM)","CHANNELS");
			CreateDescription(uoTableField,"FREQCARRIERNICAM","Частота несущей (звук Second NICAM)","CHANNELS");
			CreateDescription(uoTableField,"FREQCARRIERSOUND","Частота несущей (звук Primary)","CHANNELS");
			CreateDescription(uoTableField,"FREQCARRIERVISION","Частота несущей (видео)","CHANNELS");
			CreateDescription(uoTableField,"FREQFROM","Диапазон канала (От)","CHANNELS");
			CreateDescription(uoTableField,"FREQTO","Диапазон канала (До)","CHANNELS");
			CreateDescription(uoTableField,"FREQUENCYGRID_ID","Идентификатор системы аналогового телевидения","CHANNELS");
			CreateDescription(uoTableField,"NAMECHANNEL","Название канала","CHANNELS");
			CreateDescription(uoTableField,"DISTRICT_ID","Идентификатор региона","CITY");
			CreateDescription(uoTableField,"NAME","Название города","CITY");
			CreateDescription(uoTableField,"EFFECTRADIATEPOWER","Эфективная излучаемая мощность","COORDDISTANCE");
			CreateDescription(uoTableField,"GENERALLYSEA20","Внутреннее море 20% пути","COORDDISTANCE");
			CreateDescription(uoTableField,"HEIGHTANTENNA","Высота антенны","COORDDISTANCE");
			CreateDescription(uoTableField,"MEDITERRANEANSEA20","Внешннее море 20% пути","COORDDISTANCE");
			CreateDescription(uoTableField,"OVERCOLDSEA","Над холодным морем","COORDDISTANCE");
			CreateDescription(uoTableField,"OVERLAND","Дистанция над землей","COORDDISTANCE");
			CreateDescription(uoTableField,"OVERWARMSEA","Над гарячим морем","COORDDISTANCE");
			CreateDescription(uoTableField,"SYSTEMCAST_ID","Идентификатор системы радиовещания","COORDDISTANCE");
			CreateDescription(uoTableField,"ACCOUNTCONDITION_ID","ссылка на учётное состояние","COORDINATION");
			CreateDescription(uoTableField,"TELECOMORG_ID","ссылка на организацию","COORDINATION");
			CreateDescription(uoTableField,"TRANSMITTER_ID","ссылка на передатчик","COORDINATION");
			CreateDescription(uoTableField,"LATITUDE","Широта точки границы","COORDPOINTS");
			CreateDescription(uoTableField,"LONGITUDE","Долгота точки границы","COORDPOINTS");
			CreateDescription(uoTableField,"NUMBOUND","Международный номер (сегмент) границы государства","COORDPOINTS");
			CreateDescription(uoTableField,"CODE","Международный стандарт (код)","COUNTRY");
			CreateDescription(uoTableField,"DESCRIPTION","Название страны (Eng.)","COUNTRY");
			CreateDescription(uoTableField,"NAME","Название страны (Укр.)","COUNTRY");
			CreateDescription(uoTableField,"COORDINATIONIS","Координация проведена ? (Да/нет)","COUNTRYCOORDINATION");
			CreateDescription(uoTableField,"COUNTRY_ID","Идентификатор страны","COUNTRYCOORDINATION");
			CreateDescription(uoTableField,"LETTERS_ID","Документ на основании которого была произведена координация (таблица LETTERS)","COUNTRYCOORDINATION");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор передатчика","COUNTRYCOORDINATION");
			CreateDescription(uoTableField,"COUNTRY_ID","Идентификатор  страны","COUNTRYPOINTS");
			CreateDescription(uoTableField,"NUMBOUND","Номер сегмента из таблицы COORDPOINTS","COUNTRYPOINTS");
			CreateDescription(uoTableField,"CODERATE","Скорость внутреннего кода","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"GAUSSIANCHANNEL","Гауссов канал","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"MODULATION","Тип модуляции","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"NAMESYSTEM","Наименование системы","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"NETBITRATEGUARD16","Скорость (Защитный интервал 1/16)","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"NETBITRATEGUARD32","Скорость (Защитный интервал 1/32)","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"NETBITRATEGUARD4","Скорость (Защитный интервал 1/4)","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"NETBITRATEGUARD8","Скорость (Защитный интервал 1/8)","DIGITALTELESYSTEM");
			CreateDescription(uoTableField,"AREA_ID","Идентификатор области","DISTRICT");
			CreateDescription(uoTableField,"NAME","Наименование области","DISTRICT");
			CreateDescription(uoTableField,"CODE","Код документа","DOCUMENT");
			CreateDescription(uoTableField,"NAME","Наименование документа","DOCUMENT");
			CreateDescription(uoTableField,"TEMPLATE","Шаблон документа","DOCUMENT");
			CreateDescription(uoTableField,"MANUFACTURE","Название производителя","EQUIPMENT");
			CreateDescription(uoTableField,"NAME","Наименование оборудования","EQUIPMENT");
			CreateDescription(uoTableField,"TYPEEQUIPMENT","Тип оборудования","EQUIPMENT");
			CreateDescription(uoTableField,"DESCRIPTION","описание сетки","FREQUENCYGRID");
			CreateDescription(uoTableField,"ID","первичный ключ","FREQUENCYGRID");
			CreateDescription(uoTableField,"NAME","код сетки","FREQUENCYGRID");
			CreateDescription(uoTableField,"ACCOUNTCONDITION_ID","Письмо пернадлежит к одному из учетных состояний (внутреннему и международному) передатчика","LETTERS");
			CreateDescription(uoTableField,"ANSWERDATE","Срок ответа","LETTERS");
			CreateDescription(uoTableField,"ANSWERIS","Ответ Да/Нет","LETTERS");
			CreateDescription(uoTableField,"COPYLETTER","Копия письма (в электронном или отсканированом виде)","LETTERS");
			CreateDescription(uoTableField,"CREATEDATEIN","Дата входящего (не заполняется для исходящих)","LETTERS");
			CreateDescription(uoTableField,"CREATEDATEOUT","Дата создания исходящего письма","LETTERS");
			CreateDescription(uoTableField,"DOCUMENT_ID","Тип документа (Идентификатор из таблицы DOCUMENT)","LETTERS");
			CreateDescription(uoTableField,"LETTERS_ID","Идентификатор родительского письма","LETTERS");
			CreateDescription(uoTableField,"NUMIN","Номер входящего (не заполняется для исходящих)","LETTERS");
			CreateDescription(uoTableField,"NUMOUT","Номер исходящего письма","LETTERS");
			CreateDescription(uoTableField,"TELECOMORGANIZATION_ID","код организации (Идентификатор из таблицы ORGANIZATION)","LETTERS");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор тех параметры передатчика (идентификатор из таблицы TECHPARAMTRANSMITER)","LETTERS");
			CreateDescription(uoTableField,"TYPELETTER","Тип письма (входящее - 1 исходящие - 0)","LETTERS");
			CreateDescription(uoTableField,"DATEFROM","Срок лицензии (с какой даты)","LICENSE");
			CreateDescription(uoTableField,"DATETO","Срок лицензии (по какую дату)","LICENSE");
			CreateDescription(uoTableField,"NUMLICENSE","Номер лицензии","LICENSE");
			CreateDescription(uoTableField,"OWNER_ID","Идентификатор владельца (идентификатор из таблицы OWNER)","LICENSE");
			CreateDescription(uoTableField,"TIMEFROM","Время начала выхода программы","LICENSE");
			CreateDescription(uoTableField,"TIMETO","Время конца выхода программы","LICENSE");
			CreateDescription(uoTableField,"MINFIELDSTENGTH","Минимальные напряжения","MINSTRENGTHFIELD");
			CreateDescription(uoTableField,"SYSTEMCAST_ID","Тип системы","MINSTRENGTHFIELD");
			CreateDescription(uoTableField,"TYPEAREA","Тип застроек при распространении волн (Село, город и т.д)","MINSTRENGTHFIELD");
			CreateDescription(uoTableField,"TYPESERVICE","Тип сервиса моно/стерео для аналогового радиовещания","MINSTRENGTHFIELD");
			CreateDescription(uoTableField,"CODEOFFSET","Код смещения","OFFSETCARRYFREQTVA");
			CreateDescription(uoTableField,"OFFSET","Код смещения (герц)","OFFSETCARRYFREQTVA");
			CreateDescription(uoTableField,"ADDRESSJURE","Юридическия адрес","OWNER");
			CreateDescription(uoTableField,"ADDRESSPHYSICAL","Физический адрес","OWNER");
			CreateDescription(uoTableField,"BANK_ID","Идентификатор банка","OWNER");
			CreateDescription(uoTableField,"FAX","Факс","OWNER");
			CreateDescription(uoTableField,"ID","Идентификатор","OWNER");
			CreateDescription(uoTableField,"MAIL","Электронный адрес","OWNER");
			CreateDescription(uoTableField,"NAMEBOSS","Руководитель","OWNER");
			CreateDescription(uoTableField,"NAMEORGANIZATION","Наименование","OWNER");
			CreateDescription(uoTableField,"NUMIDENTYCOD","_ндив_дуальний податковий номер","OWNER");
			CreateDescription(uoTableField,"NUMIDENTYCODACCOUNT","ЄДРПОУ","OWNER");
			CreateDescription(uoTableField,"NUMNDS","№ св_доцтва про реєстрац_ю платника ПДВ","OWNER");
			CreateDescription(uoTableField,"NUMSETTLEMENTACCOUNT","Расчетный счет","OWNER");
			CreateDescription(uoTableField,"PHONE","Телефон","OWNER");
			CreateDescription(uoTableField,"TYPEFINANCE","Вид ф_нансування  (0-Б/1-НБ)","OWNER");
			CreateDescription(uoTableField,"FREQSPACING","Разнос частот","PROTECTRATIOESDAB");
			CreateDescription(uoTableField,"PROTECTRATIO","Защитное отношение","PROTECTRATIOESDAB");
			CreateDescription(uoTableField,"RADIOSERVICE_ID","Идентификатор радиосервиса","PROTECTRATIOESDAB");
			CreateDescription(uoTableField,"ANALOGTELESYSTEM_ID","Система аналогово телевидения","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"BANDWIDTH","Ширина полосы","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"CHANNEL","Канал (0 - один канал  +1 - канал n+1  -1 - канал n-1  +9 - зеркальный канал  -9 - зеркальный канал  +8 - зеркальный канал)","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"DIGITALTELESYSTEM_ID","Модуляция и скорость обработки кода","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"FREQSPACING","Разнос частот для перекрывающих каналов","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"PROTECTRATIO","Защитное отношение","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"SYSTEMCOLOUR","Система цветности","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"TYPESYSTEM","Система вещания","PROTECTRATIOESDVB");
			CreateDescription(uoTableField,"BANDWIDTH","Ширина полосы цифрового ТВ","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"CHANNEL","Канал (0 - один канал  +1 - канал n+1  -1 - канал n-1  +9 - зеркальный канал  -9 - зеркальный канал  +8 - зеркальный канал)","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"CLASSWAVE","Класс волн","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"CONTININTERFERENCE","Длительная помеха","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"FREQSPACING","Разнос частот","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"OFFSETLINE","Смещение для видео сигнала","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"TROPOSPHERINTERFERENCE","Тропосферная помеха","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"UNWANTEDCOLOUR","Cистема цвета мешающего передатчика","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"UNWANTEDSYSTEM","Аналоговая система мешающего передатчика","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"WANTEDCOLOUR","Cистема цвета передатчика","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"WANTEDSYSTEM","Аналоговая система передатчика","PROTECTRATIOESTVA");
			CreateDescription(uoTableField,"DEVIATION1","Девиация излучаемого передатчика","PROTECTRATIOESVHFSB");
			CreateDescription(uoTableField,"DEVIATION2","Девиация мешающего передатчика","PROTECTRATIOESVHFSB");
			CreateDescription(uoTableField,"FREQSPACING","Разос частот","PROTECTRATIOESVHFSB");
			CreateDescription(uoTableField,"MONOSTEREO","Тип передачи моно/стерео","PROTECTRATIOESVHFSB");
			CreateDescription(uoTableField,"STEADYINTERFERENCE","Устойчивая помеха","PROTECTRATIOESVHFSB");
			CreateDescription(uoTableField,"TROPOSPHERINTERFERENCE","Тропосферная помеха","PROTECTRATIOESVHFSB");
			CreateDescription(uoTableField,"TYPESYSTEM","Система вещания","PROTECTRATIOESVHFSB");
			CreateDescription(uoTableField,"CODE","Международный стандарт (код)","RADIOSERVICE");
			CreateDescription(uoTableField,"DESCRIPTION","Название радиослужбы (Eng.)","RADIOSERVICE");
			CreateDescription(uoTableField,"NAME","Название радиослужбы (Укр.)","RADIOSERVICE");
			CreateDescription(uoTableField,"SELECTIONS_ID","Идентификатор планируемого передатчика","SELECTEDTRANSMITTERS");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор мешающего передатчика","SELECTEDTRANSMITTERS");
			CreateDescription(uoTableField,"USED_IN_CALC","Используется в вычислениях","SELECTEDTRANSMITTERS");
			CreateDescription(uoTableField,"CHANGEDATE","Дата изменения","SELECTIONS");
			CreateDescription(uoTableField,"CREATEDATE","Дата создания","SELECTIONS");
			CreateDescription(uoTableField,"FREQUENCY","Частота передатчика","SELECTIONS");
			CreateDescription(uoTableField,"NAMECALCMODEL","Наименование модели вычислений","SELECTIONS");
			CreateDescription(uoTableField,"NAMEQUERIES","Наименование выборки","SELECTIONS");
			CreateDescription(uoTableField,"RESULT","Результаты расчетов по выборке ","SELECTIONS");
			CreateDescription(uoTableField,"RX_ANT_HEIGHT","Высота приемной антенны","SELECTIONS");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор передатчика которому пренадлежит выборка","SELECTIONS");
			CreateDescription(uoTableField,"TYPERESULT","Тип результата","SELECTIONS");
			CreateDescription(uoTableField,"USERID","Наименование пользователя","SELECTIONS");
			CreateDescription(uoTableField,"ADDRESS","Идентификатор улицы","STAND");
			CreateDescription(uoTableField,"AREA_ID","Идентификатор области","STAND");
			CreateDescription(uoTableField,"CITY_ID","Идентификатор города","STAND");
			CreateDescription(uoTableField,"DISTRICT_ID","Идентификатор района","STAND");
			CreateDescription(uoTableField,"HEIGHT_SEA","Высота над уровнем моря","STAND");
			CreateDescription(uoTableField,"ID","Идентификатор","STAND");
			CreateDescription(uoTableField,"LATITUDE","Широта","STAND");
			CreateDescription(uoTableField,"LONGITUDE","Долгота","STAND");
			CreateDescription(uoTableField,"MAX_OBST","Максимальная зона помех","STAND");
			CreateDescription(uoTableField,"MAX_USE","Максимальная полезная зона","STAND");
			CreateDescription(uoTableField,"NAMESITE","Наименование пункта","STAND");
			CreateDescription(uoTableField,"NAMESITE_ENG","Название опоры для ТО1 и ТО2 (англ.)","STAND");
			CreateDescription(uoTableField,"VHF_IS","Есть ОВЧ на опоре","STAND");
			CreateDescription(uoTableField,"CITY_ID","Идентификатор города","STREET");
			CreateDescription(uoTableField,"NAME","Название улицы","STREET");
			CreateDescription(uoTableField,"SYNHRONETID","Идентификатор SFN ","SYNHROFREQNET");
			CreateDescription(uoTableField,"TYPESYNHRONET_ID","Тип сети из таблицы TYPESYNHRONET","SYNHROFREQNET");
			CreateDescription(uoTableField,"CLASSWAVE","Тип волн (UHF, VHF)","SYSTEMCAST");
			CreateDescription(uoTableField,"CODE","Код системы","SYSTEMCAST");
			CreateDescription(uoTableField,"DESCRIPTION","Дескриптор системы","SYSTEMCAST");
			CreateDescription(uoTableField,"FREQFROM","Диапазон частот (от)","SYSTEMCAST");
			CreateDescription(uoTableField,"FREQTO","Диапазон частот (До)","SYSTEMCAST");
			CreateDescription(uoTableField,"NUMDIAPASON","Номер диапазона","SYSTEMCAST");
			CreateDescription(uoTableField,"RELATIONNAME","Имя таблицы с описанием систем вещания","SYSTEMCAST");
			CreateDescription(uoTableField,"TYPESYSTEM","Тип системы(аналоговые - 0 , цыфровые - 1)","SYSTEMCAST");
			CreateDescription(uoTableField,"ADDRESS","Адрес организации","TELECOMORGANIZATION");
			CreateDescription(uoTableField,"CODE","Код организации","TELECOMORGANIZATION");
			CreateDescription(uoTableField,"COUNTRY_ID","Страна данной администрации","TELECOMORGANIZATION");
			CreateDescription(uoTableField,"MAIL","Электронный почтовый адрес","TELECOMORGANIZATION");
			CreateDescription(uoTableField,"NAME","Наименование организации","TELECOMORGANIZATION");
			CreateDescription(uoTableField,"PHONE","Телефон","TELECOMORGANIZATION");
			CreateDescription(uoTableField,"BEARING","Угол тестовой точки","TESTPOINTS");
			CreateDescription(uoTableField,"DISTANCE","Дистанция до тестовой точки","TESTPOINTS");
			CreateDescription(uoTableField,"LATITUDE","Широта","TESTPOINTS");
			CreateDescription(uoTableField,"LONGITUDE","Долгота","TESTPOINTS");
			CreateDescription(uoTableField,"NAME","Наименование тестовой точки","TESTPOINTS");
			CreateDescription(uoTableField,"PROTECTEDFIELD","Защищенное поле напряженности","TESTPOINTS");
			CreateDescription(uoTableField,"TESTPOINT_TYPE","0 - 36 значений, 1 - ретрансляторы, 2 - границы государств","TESTPOINTS");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор передатчика","TESTPOINTS");
			CreateDescription(uoTableField,"USEBLEFIELD","Используемое поле напряженности","TESTPOINTS");
			CreateDescription(uoTableField,"TESTPOINTS_ID","Идентификатор тестовой точки","TESTPOINTTRANSMITTERS");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор передатчика","TESTPOINTTRANSMITTERS");
			CreateDescription(uoTableField,"EQUIPMENT_ID","Идентификатор оборудования","TRANSMITTEREQUIPMENT");
			CreateDescription(uoTableField,"TRANSMITTERS_ID","Идентификатор передатчика","TRANSMITTEREQUIPMENT");
			CreateDescription(uoTableField,"ACCOUNTCONDITION_IN","Внутреннее состояние","TRANSMITTERS");
			CreateDescription(uoTableField,"ACCOUNTCONDITION_OUT","Международное состояние","TRANSMITTERS");
			CreateDescription(uoTableField,"ADMINISTRATIONID","Идентификатор установленный Администрацией","TRANSMITTERS");
			CreateDescription(uoTableField,"ALLOTMENTBLOCKDAB_ID","Идентификатор частотного распределенного блока (для цифрового звука)","TRANSMITTERS");
			CreateDescription(uoTableField,"ANGLEELEVATION_HOR","Угол возвышения антенны (Горизонтальная поляризация)передатчика","TRANSMITTERS");
			CreateDescription(uoTableField,"ANGLEELEVATION_VER","Угол возвышения (вертикальная поляризация)передатчика","TRANSMITTERS");
			CreateDescription(uoTableField,"ANTENNAGAIN","Коэффициент усиления антенны передатчика","TRANSMITTERS");
			CreateDescription(uoTableField,"ANT_DIAG_H","Коэффициент усиления антенны передатчика шаг 10 градусов","TRANSMITTERS");
			CreateDescription(uoTableField,"AREACOVERAGE","Зона покрытия (в основном для кабельного телевидения)","TRANSMITTERS");
			CreateDescription(uoTableField,"AZIMUTHMAXRADIATION","Азимут максимального випром_нювання              град.","TRANSMITTERS");
			CreateDescription(uoTableField,"BLOCKCENTREFREQ","Частота блока (только для цыфрового звука)","TRANSMITTERS");
			CreateDescription(uoTableField,"CHANNEL_ID","Идентификатор канала","TRANSMITTERS");
			CreateDescription(uoTableField,"CLASSWAVE","Класс волн (VHF, UHF)","TRANSMITTERS");
			CreateDescription(uoTableField,"DATECHANGE","Дата изменения","TRANSMITTERS");
			CreateDescription(uoTableField,"DATECREATE","Дата создания","TRANSMITTERS");
			CreateDescription(uoTableField,"DATEINTENDUSE","Дата когда администрация хочет начать использовать передатчик ","TRANSMITTERS");
			CreateDescription(uoTableField,"DATEPERMBUILDFROM","Дата разрешения на постройку (с какой даты)(не заполняется для кабельного)","TRANSMITTERS");
			CreateDescription(uoTableField,"DATEPERMBUILDTO","Дата разрешения на постройку (по какую дату)(не заполняется для кабельного)","TRANSMITTERS");
			CreateDescription(uoTableField,"DATEPERMREGCOUNCIL","Дата выдачи разрешения областным комитетом (Только для кабельного ТВ) ","TRANSMITTERS");
			CreateDescription(uoTableField,"DATEPERMUSEFROM","Дата разрешения на эксплуатацию (с какой даты)","TRANSMITTERS");
			CreateDescription(uoTableField,"DATEPERMUSETO","Дата разрешения на эксплуатацию (по какую дату)","TRANSMITTERS");
			CreateDescription(uoTableField,"DATESTANDCERTIFICATE","Дата выдачи стандарта сертификации","TRANSMITTERS");
			CreateDescription(uoTableField,"DEL","вспомогательное поле для удаления дубликатов (см проц. SP_DELETE_DUPLICATES)","TRANSMITTERS");
			CreateDescription(uoTableField,"DIRECTION","Направленность антенны (ND-не напрвленная, D-напрвленная) передатчика","TRANSMITTERS");
			CreateDescription(uoTableField,"EFFECTHEIGHT","Эффективные высоты антенны ","TRANSMITTERS");
			CreateDescription(uoTableField,"EFFECTPOWERHOR","Э.И.М. передатчика гориз. ","TRANSMITTERS");
			CreateDescription(uoTableField,"EFFECTPOWERVER","Э.И.М. передатчика вертик. ","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_SOUND_HOR_PRIMARY","Э.И.М. передатчика гориз. Primary(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_SOUND_HOR_SECOND","Э.И.М. передатчика гориз. Second(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_SOUND_MAX_PRIMARY","Э.И.М. передатчика max. Primary(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_SOUND_MAX_SECOND","Э.И.М. передатчика max. Second(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_SOUND_VERT_PRIMARY","Э.И.М. передатчика вертик. Primary(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_SOUND_VER_SECOND","Э.И.М. передатчика вертик. Second(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_VIDEO_HOR","Э.И.М. передатчика гориз. (видео)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_VIDEO_MAX","Э.И.М. передатчика (видео)","TRANSMITTERS");
			CreateDescription(uoTableField,"EPR_VIDEO_VERT","Э.И.М. передатчика вертик. (видео)","TRANSMITTERS");
			CreateDescription(uoTableField,"FIDERLENGTH","Длина фидера","TRANSMITTERS");
			CreateDescription(uoTableField,"FIDERLOSS","Потери в Фидере","TRANSMITTERS");
			CreateDescription(uoTableField,"FREQSHIFT","Вiдхилення частоти                              Гц","TRANSMITTERS");
			CreateDescription(uoTableField,"FREQSTABILITY","Тип нестабильности частоты","TRANSMITTERS");
			CreateDescription(uoTableField,"GUARDINTERVAL_ID","Несущая и защитный интервал (только для цыфрового ТВ)","TRANSMITTERS");
			CreateDescription(uoTableField,"HEIGHTANTENNA","Высота антенны","TRANSMITTERS");
			CreateDescription(uoTableField,"HEIGHT_EFF_MAX","Эффективная высота антенны","TRANSMITTERS");
			CreateDescription(uoTableField,"IDENTIFIERSFN","Идентификатор ОЧС (для цифровых)","TRANSMITTERS");
			CreateDescription(uoTableField,"LATITUDE","Широта","TRANSMITTERS");
			CreateDescription(uoTableField,"LEVELSIDERADIATION","Рiвень побiчних випромiнювань                   мВт","TRANSMITTERS");
			CreateDescription(uoTableField,"LICENSE_CHANNEL_ID","Идентификатор лицензии на канал мовлення","TRANSMITTERS");
			CreateDescription(uoTableField,"LICENSE_RFR_ID","Идентификатор лицензии на РЧР (Для кабельного не заполняется)","TRANSMITTERS");
			CreateDescription(uoTableField,"LICENSE_SERVICE_ID","Идентификатор лицензии на тех. обслуживание (заполняется после получения разрешения на эксплуатацию)","TRANSMITTERS");
			CreateDescription(uoTableField,"LONGITUDE","Долгота","TRANSMITTERS");
			CreateDescription(uoTableField,"MONOSTEREO_PRIMARY","тип моно/стерео Primary","TRANSMITTERS");
			CreateDescription(uoTableField,"NAMEPROGRAMM","Наименование программы","TRANSMITTERS");
			CreateDescription(uoTableField,"NOTICECOUNT","Заявка на выписку счета (Только для кабельного ТВ)","TRANSMITTERS");
			CreateDescription(uoTableField,"NUMFACTORY","Заводской номер","TRANSMITTERS");
			CreateDescription(uoTableField,"NUMPERMBUILD","Номер разрешения на постройку (не заполняется для кабельного)","TRANSMITTERS");
			CreateDescription(uoTableField,"NUMPERMREGCOUNCIL","Номер разрешения областного комитета (Только для кабельного ТВ) ","TRANSMITTERS");
			CreateDescription(uoTableField,"NUMPERMUSE","Номер разрешения на эксплуатацию","TRANSMITTERS");
			CreateDescription(uoTableField,"NUMREGISTRY","Номер регистрации","TRANSMITTERS");
			CreateDescription(uoTableField,"NUMSTANDCERTIFICATE","Номер стандарта сертификации","TRANSMITTERS");
			CreateDescription(uoTableField,"ORIGINALID","Идентификатор передатчика - оригинала для данной копии(null - для записи оригинала)","TRANSMITTERS");
			CreateDescription(uoTableField,"OWNER_ID","Владелец передатчика","TRANSMITTERS");
			CreateDescription(uoTableField,"POLARIZATION","Поляризация антенны передатчика","TRANSMITTERS");
			CreateDescription(uoTableField,"POWER_SOUND_PRIMARY","Мощность передатчика Primary(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"POWER_SOUND_SECOND","Мощность передатчика Second(звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"POWER_VIDEO","Мощность передатчика (видео)","TRANSMITTERS");
			CreateDescription(uoTableField,"REGIONALAGREEMENT","Региональное соглашение (Стокгольм, Женева и т.д)","TRANSMITTERS");
			CreateDescription(uoTableField,"REGIONALCOUNCIL","Наименование органа который выдал разрешения (Только для кабельного ТВ) ","TRANSMITTERS");
			CreateDescription(uoTableField,"RELATIVETIMINGSFN","В_дносна синхрон_зац_я передавача в ОЧС, мкс (для цифровых)","TRANSMITTERS");
			CreateDescription(uoTableField,"RELAYSTATION_ID","идентификатор станции-ретранслятора","TRANSMITTERS");
			CreateDescription(uoTableField,"REMARKS","Замечания","TRANSMITTERS");
			CreateDescription(uoTableField,"RESPONSIBLEADMIN","Ответственная организация","TRANSMITTERS");
			CreateDescription(uoTableField,"RPC","эталонная конфигурация планирования","TRANSMITTERS");
			CreateDescription(uoTableField,"RX_MODE","тип приема для ЦТВ","TRANSMITTERS");
			CreateDescription(uoTableField,"SOUND_CARRIER_PRIMARY","Частота несущей Primary (звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"SOUND_CARRIER_SECOND","Частота несущей Second (звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"SOUND_EMISSION_PRIMARY","Тип излучения несущей звука Primary","TRANSMITTERS");
			CreateDescription(uoTableField,"SOUND_EMISSION_SECOND","Тип излучения несущей звука Second","TRANSMITTERS");
			CreateDescription(uoTableField,"SOUND_OFFSET_PRIMARY","СНЧ несущей Primary  (звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"SOUND_OFFSET_SECOND","СНЧ несущей Second  (звук)","TRANSMITTERS");
			CreateDescription(uoTableField,"SOUND_SYSTEM_SECOND","Система звука Second","TRANSMITTERS");
			CreateDescription(uoTableField,"STAND_ID","Идентификатор опоры","TRANSMITTERS");
			CreateDescription(uoTableField,"STATUS","Статус передатчика (0-База, 1-предбаза, -1-Удаленный передатчик(т.е. сброшен в архив))","TRANSMITTERS");
			CreateDescription(uoTableField,"SUMMATOFREQFROM","Диапазон частот                         мГц''","TRANSMITTERS");
			CreateDescription(uoTableField,"SUMMATORATTENUATION","Затухання                                       дБ","TRANSMITTERS");
			CreateDescription(uoTableField,"SUMMATORFREQTO","Диапазон частот                         мГц''","TRANSMITTERS");
			CreateDescription(uoTableField,"SUMMATORMINFREQS","Минимальна ризниця частот                       Мгц","TRANSMITTERS");
			CreateDescription(uoTableField,"SUMMATORPOWERFROM","Диапазон потужностей            кВт","TRANSMITTERS");
			CreateDescription(uoTableField,"SUMMATORPOWERS","Суматора потужностей            Так/Н_","TRANSMITTERS");
			CreateDescription(uoTableField,"SUMMATORPOWERTO","Диапазон потужностей            кВт","TRANSMITTERS");
			CreateDescription(uoTableField,"SYSTEMCAST_ID","Система вещания","TRANSMITTERS");
			CreateDescription(uoTableField,"SYSTEMCOLOUR","Идентификатор системы цвета для аналоговых ТВ","TRANSMITTERS");
			CreateDescription(uoTableField,"TESTPOINTSIS","Проверка на наличие тестовых точек","TRANSMITTERS");
			CreateDescription(uoTableField,"TIMETRANSMIT","Время выхода в эфир","TRANSMITTERS");
			CreateDescription(uoTableField,"TYPEOFFSET","Тип СНЧ","TRANSMITTERS");
			CreateDescription(uoTableField,"TYPEREGISTRY","Тип регистрации (Раздел)","TRANSMITTERS");
			CreateDescription(uoTableField,"TYPESYSTEM","Тип системы : для аналогового звука табл. ANALOGRADIOSYSTEM для аналогового ТВ табл. ANALOGTELESYSTEM для цыфрового ТВ табл. DIGITALTELESYSTEM","TRANSMITTERS");
			CreateDescription(uoTableField,"USERID","Идентификатор пользователя который последним изменил запись","TRANSMITTERS");
			CreateDescription(uoTableField,"VIDEO_CARRIER","Частота несущей (видео)","TRANSMITTERS");
			CreateDescription(uoTableField,"VIDEO_EMISSION","Тип излучения несущей видео","TRANSMITTERS");
			CreateDescription(uoTableField,"VIDEO_OFFSET_HERZ","СНЧ несущей видео (Герцы)","TRANSMITTERS");
			CreateDescription(uoTableField,"VIDEO_OFFSET_LINE","СНЧ (линии)","TRANSMITTERS");
			CreateDescription(uoTableField,"V_SOUND_RATIO_PRIMARY","Отношения мощностей видео к звуку для Primary ","TRANSMITTERS");
			CreateDescription(uoTableField,"V_SOUND_RATIO_SECOND","Отношения мощностей видео к звуку для Second ","TRANSMITTERS");
			CreateDescription(uoTableField,"NAME","Тип приема","TYPERECEIVE");
			CreateDescription(uoTableField,"COVERAGEAREA","Зона покрытия","TYPESYNHRONET");
			CreateDescription(uoTableField,"DIRECTIVITY_CENTRANS","Направленность антенны центрального передатчика","TYPESYNHRONET");
			CreateDescription(uoTableField,"DIRECTIVITY_PERTRANS","Направленность антенны переферийных передатчиков","TYPESYNHRONET");
			CreateDescription(uoTableField,"DISTANCE_ADJACENT_TRANS","Дистанция между соседними передатчиками","TYPESYNHRONET");
			CreateDescription(uoTableField,"EFFHEIGHTANT_CENTRANS","Эффективная высота антенны центрального передатчика","TYPESYNHRONET");
			CreateDescription(uoTableField,"EFFHEIGHTANT_PERTRANS","Эффективная высота антенны переферийных передатчиков","TYPESYNHRONET");
			CreateDescription(uoTableField,"NETGEOMETRY","Геометрия расположения передатчиков","TYPESYNHRONET");
			CreateDescription(uoTableField,"NUMTRANSMITTERS","Количество передатчиков","TYPESYNHRONET");
			CreateDescription(uoTableField,"POWER_CENTRANS","Мощность центрального передатчика","TYPESYNHRONET");
			CreateDescription(uoTableField,"POWER_PERTRANS","Мощность переферийных передатчиков","TYPESYNHRONET");
			CreateDescription(uoTableField,"COCHANNEL","Со-канал","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"HETERODYNEHARMONIC1","Гетеродинный канал (1 гармоника)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"HETERODYNEHARMONIC2","Гетеродинный канал (2 гармоника)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"HETERODYNEHARMONIC3","Гетеродинный канал (3 гармоника)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"LOWERADJACENT","Канал (n-1)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"LOWERIMAGE1","Зеркальный канал 1 (-)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"LOWERIMAGE2","Зеркальный канал 2 (-)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"UPPERADJACENT","Канал (n+1)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"UPPERIMAGE1","Зеркальный канал 1 (+)","UNCOMPATIBLECHANNELS");
			CreateDescription(uoTableField,"UPPERIMAGE2","Зеркальный канал 2 (+)","UNCOMPATIBLECHANNELS");
			
			CreateDescription(uoProcParameter,"IN_ADMITID","Идентификатор пользователя","PR_ACTIVEVIEW_SETCHANGE");
			CreateDescription(uoProcParameter,"IN_NAMETABLE","Наименование таблицы","PR_ACTIVEVIEW_SETCHANGE");
			CreateDescription(uoProcParameter,"IN_NUMCHANGE","Номер измененной записи","PR_ACTIVEVIEW_SETCHANGE");
			CreateDescription(uoProcParameter,"IN_TYPECHANGE","Тип изменений","PR_ACTIVEVIEW_SETCHANGE");
        END_UPD_VER()

        START_UPD_VER(tmp_version, 20080102.0000)
            String insertClause("INSERT INTO SYSTEMCAST (ID,CODE,DESCRIPTION,TYPESYSTEM,CLASSWAVE,FREQFROM,FREQTO,NUMDIAPASON,RELATIONNAME,ENUMVAL) VALUES (");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'АТБ','Аналогове телебачення',0,'VHF',48.5,862,1,'ANALOGTELESYSTEM',1)", uaInsert, uoTable, "Типы передатчиков");

            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'АРМ','Аналогове радіомовлення',0,'UHF',66,107.9,1,'ANALOGRADIOSYSTEM',2)", uaInsert, uoTable, "Типы передатчиков");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'СХДХ','Мовлення в СХ/ДХ дiапазонах',NULL,'LFMF',0.03,3,NULL,'LFMF_SYSTEM',3)", uaInsert, uoTable, "Типы передатчиков");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'ЦТБ','Цифрове телебачення',1,'UHF',174,862,3,'DIGITALTELESYSTEM',4)", uaInsert, uoTable, "Типы передатчиков");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'ЦРМ','Цифрове радіомовлення',1,'UHF',47,1668,1,'<none>',5)", uaInsert, uoTable, "Типы передатчиков");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'КТБ','Системи кабельного телебачення',0,'UHF',48.5,862,1,'<none>',6)", uaInsert, uoTable, "Типы передатчиков");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'ВЫД','Цифровое выделение',1,'UHF',174,862,3,'<none>',7)", uaInsert, uoTable, "Типы передатчиков");

            insertClause = "INSERT INTO ANALOGRADIOSYSTEM (ID,CODSYSTEM,TYPESYSTEM,MODULATION,DEVIATION,ENUMVAL) VALUES (";
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'System 1','Mono',NULL,75,0)", uaInsert, uoTable, "Системы радио");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'System 2','Mono',NULL,50,1)", uaInsert, uoTable, "Системы радио");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'System 3','Stereo','polar',50,2)", uaInsert, uoTable, "Системы радио");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'System 4','Stereo','pilot',75,3)", uaInsert, uoTable, "Системы радио");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'System 5','Stereo','pilot',50,4)", uaInsert, uoTable, "Системы радио");

            insertClause = "INSERT INTO DIGITALTELESYSTEM (ID,NAMESYSTEM,MODULATION,CODERATE,ENUMVAL) VALUES (";
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'A1','QPSK','1/2',0)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'A2','QPSK','2/3',1)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'A3','QPSK','3/4',2)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'A5','QPSK','5/6',3)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'A7','QPSK','7/8',4)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'B1','16-QAM','1/2',5)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'B2','16-QAM','2/3',6)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'B3','16-QAM','3/4',7)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'B5','16-QAM','5/6',8)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'B7','16-QAM','7/8',9)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'C1','64-QAM','1/2',10)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'C2','64-QAM','2/3',11)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'C3','64-QAM','3/4',12)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'C5','64-QAM','5/6',13)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'C7','64-QAM','7/8',14)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'D1','QPSK','1/2',15)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'D2','QPSK','2/3',16)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'D3','QPSK','3/4',17)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'D5','QPSK','5/6',18)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'D7','QPSK','7/8',19)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'E1','16-QAM','1/2',20)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'F3','64-QAM','3/4',27)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'F5','64-QAM','5/6',28)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'E2','16-QAM','2/3',21)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'E3','16-QAM','3/4',22)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'E5','16-QAM','5/6',23)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'E7','16-QAM','7/8',24)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'F1','64-QAM','1/2',25)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'F2','64-QAM','2/3',26)", uaInsert, uoTable, "Системы ЦТВ");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'F7','64-QAM','7/8',29)", uaInsert, uoTable, "Системы ЦТВ");
                                               
            insertClause = "INSERT INTO COUNTRY (ID,NAME,CODE) VALUES (";
            RunQuery(insertClause+IntToStr(-1)+",'Невідома',NULL)",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",' Украина','UKR')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Росія','RUS')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Бєларусь','BLR')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Польща','POL')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Туреччина','TUR')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Болгарія','BUL')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Угорщина','HNG')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Румунія','ROU')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Грузія','GEO')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Німеччина','D')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Словакія','SVK')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Чехія','CZE')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Літва','LTU')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Латвія','LVA')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Естонія','EST')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Македонія','MKD')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Югославія','YUG')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Боснія','BIH')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Хорватія','HRV')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Словенія','SVN')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Швеція','S')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Швейцарія','SWL')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Молдова','MDA')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Австрія','AUT')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Арменія','ARM')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Албанія','ALB')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Азербайджан','AZE')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Греція','GRC')",uaInsert, uoTable, "Страны");
            RunQuery(insertClause+IntToStr(dmMain->getNewId())+",'Сербія та Черногорія','SCG')",uaInsert, uoTable, "Страны");

            RunQuery("insert into AREA(ID,COUNTRY_ID,NAME)values(-1,-1,'???')",uaInsert, uoTable, "Area");
            RunQuery("insert into CITY(ID,DISTRICT_ID,NAME,AREA_ID,COUNTRY_ID)values(-1,-1,'???',-1,-1)",uaInsert, uoTable, "City");
            RunQuery("insert into STREET(ID,CITY_ID,NAME)values(-1,-1,'???')",uaInsert, uoTable, "Street");

            insertClause = "INSERT INTO DATABASESECTION (ID,SECTIONNAME,CAN_MODIFY,CAN_DELETE) VALUES (";
            RunQuery(insertClause+"0,'БАЗА',0,0)",uaInsert, uoTable, "DATABASESECTION");
            RunQuery(insertClause+"1,'ПЕРЕДБАЗА',1,0)",uaInsert, uoTable, "DATABASESECTION");
            RunQuery(insertClause+"-1,'ВИДАЛЕНІ',0,1)",uaInsert, uoTable, "DATABASESECTION");
            RunQuery(insertClause+"100,'BR IFIC',0,0)",uaInsert, uoTable, "DATABASESECTION");

        END_UPD_VER()
}
