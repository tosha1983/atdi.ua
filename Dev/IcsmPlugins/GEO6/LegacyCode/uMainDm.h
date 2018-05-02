//---------------------------------------------------------------------------

#ifndef uMainDmH
#define uMainDmH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <DB.hpp>
#include <IBDatabase.hpp>
#include <IBSQL.hpp>
#include <IBCustomDataSet.hpp>
#include <IBQuery.hpp>
#include <string>
#include <set>
#include <map>
#include <vector>
//---------------------------------------------------------------------------

#define WM_LIST_ELEMENT_SELECTED (WM_USER + 1)

typedef enum tagTxStatus {
    tsBase = 0,
    tsDraft = 1,
    tsDeleted = -1
} TxStatus;

struct DbSectionInfo {
    AnsiString name;
    bool allowChange;
    bool allowDelete;
    DbSectionInfo(){};
    DbSectionInfo(const DbSectionInfo& dbsi)
    {
        name = dbsi.name; allowChange = dbsi.allowChange; allowDelete = dbsi.allowDelete;
    }
    DbSectionInfo& operator= (const DbSectionInfo& dbsi)
    {
        name = dbsi.name; allowChange = dbsi.allowChange; allowDelete = dbsi.allowDelete;
        return *this;
    }
};

//  данные об опорах в формах выборок и планирования кэшируются в бинарное дерево
class StandRecord {
public:
    std::string siteName;
    std::string areaName;
    std::string cityName;
    std::string regionNum;
    int countryId;
    StandRecord() : countryId(0) {};
    StandRecord(const StandRecord& src) :
        siteName(src.siteName),
        areaName(src.areaName),
        cityName(src.cityName),
        regionNum(src.regionNum),
        countryId(src.countryId)
    {}
    StandRecord& operator = (const StandRecord& src)
    {
        siteName = src.siteName;
        areaName = src.areaName;
        cityName = src.cityName;
        regionNum = src.regionNum;
        countryId = src.countryId;
        return *this;
    }
};

typedef std::pair<int, double> VoltagePair;
typedef std::vector<VoltagePair> VoltageVector;
void __fastcall SortVoltageVector(VoltageVector& vv, const int lowerIdx, const int higherIdx);
typedef std::map<String, Variant> ParamList;

class TdmMain : public TDataModule
{
__published:	// IDE-managed Components
    TIBDatabase *dbMain;
    TIBTransaction *trMain;
    TIBSQL *sqlUserId;
    TIBSQL *sqlGetNewId;
    TIBSQL *sqlUserLog;
    TIBDataSet *ibdsCoordination;
    TIBDataSet *ibdsChannelList;
    TIBDataSet *ibdsAccCondList;
    TIBDataSet *ibdsScList;
    TIBDataSet *ibdsAtsList;
    TIBDataSet *ibdsArsList;
    TIntegerField *ibdsChannelListID;
    TIBStringField *ibdsChannelListNAMECHANNEL;
    TIntegerField *ibdsChannelListFREQUENCYGRID_ID;
    TIntegerField *ibdsAccCondListID;
    TIBStringField *ibdsAccCondListCODE;
    TIBStringField *ibdsAccCondListNAME;
    TSmallintField *ibdsAccCondListTYPECONDITION;
    TIntegerField *ibdsScListID;
    TIBStringField *ibdsScListCODE;
    TSmallintField *ibdsScListENUMVAL;
    TIntegerField *ibdsAtsListID;
    TIBStringField *ibdsAtsListNAMESYSTEM;
    TSmallintField *ibdsAtsListENUMVAL;
    TIntegerField *ibdsAtsListFREQUENCYGRID_ID;
    TIntegerField *ibdsArsListID;
    TIBStringField *ibdsArsListCODSYSTEM;
    TIntegerField *ibdsArsListENUMVAL;
    TIBDataSet *ibdsDabBlockName;
    TIntegerField *ibdsDabBlockNameID;
    TIBStringField *ibdsDabBlockNameNAME;
    TFloatField *ibdsDabBlockNameCENTREFREQ;
    TIBDataSet *ibdsDvbSystemList;
    TIntegerField *ibdsDvbSystemListID;
    TIBStringField *ibdsDvbSystemListNAMESYSTEM;
    TIntegerField *ibdsDvbSystemListENUMVAL;
    TIBDataSet *ibdsCoordDist;
    TIntegerField *ibdsCoordDistSYSTEMCAST_ID;
    TIntegerField *ibdsCoordDistEFFECTRADIATEPOWER;
    TIntegerField *ibdsCoordDistHEIGHTANTENNA;
    TSmallintField *ibdsCoordDistOVERLAND;
    TSmallintField *ibdsCoordDistOVERCOLDSEA;
    TSmallintField *ibdsCoordDistOVERWARMSEA;
    TSmallintField *ibdsCoordDistGENERALLYSEA20;
    TSmallintField *ibdsCoordDistGENERALLYSEA40;
    TSmallintField *ibdsCoordDistGENERALLYSEA60;
    TSmallintField *ibdsCoordDistGENERALLYSEA80;
    TSmallintField *ibdsCoordDistGENERALLYSEA100;
    TSmallintField *ibdsCoordDistMEDITERRANEANSEA20;
    TSmallintField *ibdsCoordDistMEDITERRANEANSEA40;
    TSmallintField *ibdsCoordDistMEDITERRANEANSEA60;
    TSmallintField *ibdsCoordDistMEDITERRANEANSEA80;
    TSmallintField *ibdsCoordDistMEDITERRANEANSEA100;
    TIntegerField *ibdsCoordDistID;
    TIBDataSet *ibdsCheckPoints;
    TIBDataSet *ibdsCountries;
    TFloatField *ibdsCheckPointsLATITUDE;
    TFloatField *ibdsCheckPointsLONGITUDE;
    TIntegerField *ibdsCheckPointsCOUNTRY_ID;
    TIntegerField *ibdsCountriesID;
    TIBStringField *ibdsCountriesNAME;
    TIBStringField *ibdsCountriesCODE;
    TIBQuery *qryTxList;
    TIntegerField *qryTxListID;
    TFloatField *qryTxListLATITUDE;
    TFloatField *qryTxListLONGITUDE;
    TIntegerField *qryTxListSYSTEMCAST_ID;
    TFloatField *qryTxListVIDEO_CARRIER;
    TSmallintField *qryTxListVIDEO_OFFSET_LINE;
    TIntegerField *qryTxListVIDEO_OFFSET_HERZ;
    TIBStringField *qryTxListTYPEOFFSET;
    TIBStringField *qryTxListSYSTEMCOLOUR;
    TFloatField *qryTxListPOWER_VIDEO;
    TFloatField *qryTxListEPR_VIDEO_MAX;
    TFloatField *qryTxListEPR_VIDEO_HOR;
    TFloatField *qryTxListEPR_VIDEO_VERT;
    TIntegerField *qryTxListIDENTIFIERSFN;
    TIntegerField *qryTxListRELATIVETIMINGSFN;
    TIntegerField *qryTxListCHANNEL_ID;
    TFloatField *qryTxListBLOCKCENTREFREQ;
    TFloatField *qryTxListSOUND_CARRIER_PRIMARY;
    TFloatField *qryTxListPOWER_SOUND_PRIMARY;
    TFloatField *qryTxListEPR_SOUND_MAX_PRIMARY;
    TFloatField *qryTxListEPR_SOUND_HOR_PRIMARY;
    TFloatField *qryTxListEPR_SOUND_VERT_PRIMARY;
    TFloatField *qryTxListV_SOUND_RATIO_PRIMARY;
    TSmallintField *qryTxListMONOSTEREO_PRIMARY;
    TFloatField *qryTxListSOUND_CARRIER_SECOND;
    TFloatField *qryTxListPOWER_SOUND_SECOND;
    TFloatField *qryTxListEPR_SOUND_MAX_SECOND;
    TFloatField *qryTxListEPR_SOUND_HOR_SECOND;
    TFloatField *qryTxListEPR_SOUND_VER_SECOND;
    TIBStringField *qryTxListSOUND_SYSTEM_SECOND;
    TFloatField *qryTxListV_SOUND_RATIO_SECOND;
    TIntegerField *qryTxListHEIGHTANTENNA;
    TIntegerField *qryTxListHEIGHT_EFF_MAX;
    TIBStringField *qryTxListPOLARIZATION;
    TIBStringField *qryTxListDIRECTION;
    TFloatField *qryTxListFIDERLOSS;
    TIntegerField *qryTxListFIDERLENGTH;
    TSmallintField *qryTxListANGLEELEVATION_HOR;
    TSmallintField *qryTxListANGLEELEVATION_VER;
    TFloatField *qryTxListANTENNAGAIN;
    TSmallintField *qryTxListTESTPOINTSIS;
    TSmallintField *qryTxListACCOUNTCONDITION_IN;
    TSmallintField *qryTxListACCOUNTCONDITION_OUT;
    TIBStringField *qryTxListADMINISTRATIONID;
    TIntegerField *qryTxListSTAND_ID;
    TBlobField *qryTxListEFFECTPOWERHOR;
    TBlobField *qryTxListEFFECTPOWERVER;
    TBlobField *qryTxListEFFECTHEIGHT;
    TBlobField *qryTxListANT_DIAG_H;
    TSmallintField *qryTxListSC_ENUMVAL;
    TSmallintField *qryTxListAT_ENUMVAL;
    TIntegerField *qryTxListDTS_ENUMVAL;
    TIntegerField *qryTxListARS_ENUMVAL;
    TSmallintField *qryTxListTYPESYSTEM;
    TIBDataSet *ibdsStands;
    TIntegerField *ibdsStandsID;
    TIBStringField *ibdsStandsNAMESITE;
    TIBStringField *ibdsStandsAREA_NAME;
    TIBStringField *ibdsStandsCITY_NAME;
    TIBStringField *ibdsStandsNUMREGION;
    TIntegerField *ibdsStandsCOUNTRY_ID;
    TIBStringField *qryTxListADM_RESPONSE;
    TIBStringField *qryTxListADM_SITED_IN;
    TSmallintField *qryTxListSTATUS_CODE;
    TIBStringField *qryTxListSTATION_NAME;
    TIntegerField *qryTxListSITE_HEIGHT;
    TIBStringField *qryTxListCHANNEL;
    TDateField *qryTxListDATE_OF_LAST_CHANGE;
    TIBStringField *qryTxListNUMREGION;
    TIntegerField *ibdsCheckPointsNUMBOUND;
    TIntegerField *qryTxListRPC;
    TIntegerField *qryTxListRX_MODE;
    TBlobField *qryTxListANT_DIAG_V;
    TIBSQL *sqlTxSysCast;
    TFloatField *qryTxListPOL_ISOL;
    TFloatField *qryTxListGND_COND;
    TIBDataSet *ibdsLfMfList;
    TIBStringField *ibdsLfMfListCODE;
    TIntegerField *ibdsLfMfListENUM;
    TIBQuery *ibqTxOuery;
    TBlobField *ibqTxOueryEFFECTPOWERHOR;
    TBlobField *ibqTxOueryEFFECTPOWERVER;
    TBlobField *ibqTxOueryEFFECTHEIGHT;
    TBlobField *ibqTxOueryANT_DIAG_H;
    TBlobField *ibqTxOueryANT_DIAG_V;
    TIntegerField *ibqTxOueryID;
    TIntegerField *ibqTxOuerySTAND_ID;
    TIBStringField *ibqTxOuerySTAND_NAMESITE;
    TIntegerField *ibqTxOuerySTAND_HEIGHT_SEA;
    TIBStringField *ibqTxOueryAREA_NAME;
    TIBStringField *ibqTxOueryAREA_NUMREGION;
    TIBStringField *ibqTxOueryCITY_NAME;
    TIBStringField *ibqTxOuerySTREET_NAME;
    TIBStringField *ibqTxOuerySTAND_ADDRESS;
    TDateField *ibqTxOueryDATECREATE;
    TDateField *ibqTxOueryDATECHANGE;
    TIntegerField *ibqTxOueryOWNER_ID;
    TIBStringField *ibqTxOueryOWNER_NAMEORGANIZATION;
    TIBStringField *ibqTxOueryOWNER_NUMIDENTYCOD;
    TIBStringField *ibqTxOueryOWNER_NUMNDS;
    TSmallintField *ibqTxOueryOWNER_TYPEFINANCE;
    TIBStringField *ibqTxOueryOWNER_ADDRESSJURE;
    TIBStringField *ibqTxOueryOWNER_ADDRESSPHYSICAL;
    TIBStringField *ibqTxOueryOWNER_NUMSETTLEMENTACCOUNT;
    TIBStringField *ibqTxOueryBANK_MFO;
    TIBStringField *ibqTxOueryOWNER_NUMIDENTYCODACCOUNT;
    TIBStringField *ibqTxOueryBANK_NAME;
    TIBStringField *ibqTxOueryNAMEBOSS;
    TIBStringField *ibqTxOueryOWNER_PHONE;
    TIBStringField *ibqTxOueryOWNER_FAX;
    TIBStringField *ibqTxOueryOWNER_MAIL;
    TIBStringField *ibqTxOueryNAMEBOSS2;
    TIBStringField *ibqTxOueryOWNER_PHONE2;
    TIBStringField *ibqTxOueryOWNER_FAX2;
    TIBStringField *ibqTxOueryOWNER_MAIL2;
    TIBStringField *ibqTxOueryOPERATOR_NAMEORGANIZATION;
    TIBStringField *ibqTxOueryOPERATOR_NUMIDENTYCOD;
    TIBStringField *ibqTxOueryOPERATOR_ADDRESSPHYSICAL;
    TIBStringField *ibqTxOueryOPERATOR_NAMEBOSS;
    TIBStringField *ibqTxOueryOPERATOR_PHONE;
    TIBStringField *ibqTxOueryOPERATOR_FAX;
    TIBStringField *ibqTxOueryOPERATOR_MAIL;
    TIBStringField *ibqTxOueryOPERATOR_NAMEBOSS2;
    TIBStringField *ibqTxOueryOPERATOR_PHONE2;
    TIBStringField *ibqTxOueryOPERATOR_FAX2;
    TIBStringField *ibqTxOueryOPERATOR_MAIL2;
    TIntegerField *ibqTxOueryLICENSE_CHANNEL_ID;
    TIBStringField *ibqTxOueryLICENSE_CH_NUMLICENSE;
    TDateField *ibqTxOueryLICENSE_CH_DATEFROM;
    TDateField *ibqTxOueryLICENSE_CH_DATETO;
    TIntegerField *ibqTxOueryLICENSE_RFR_ID;
    TIBStringField *ibqTxOueryLICENSE_RFR_NUMLICENSE;
    TDateField *ibqTxOueryLICENSE_RFR_DATEFROM;
    TIntegerField *ibqTxOueryLICENSE_SERVICE_ID;
    TIBStringField *ibqTxOueryLICENSE_SRV_NUMLICENSE;
    TDateField *ibqTxOueryLICENSE_SRV_DATEFROM;
    TIBStringField *ibqTxOueryNUMPERMBUILD;
    TDateField *ibqTxOueryDATEPERMBUILDFROM;
    TDateField *ibqTxOueryDATEPERMBUILDTO;
    TIBStringField *ibqTxOueryNUMPERMUSE;
    TDateField *ibqTxOueryDATEPERMUSEFROM;
    TDateField *ibqTxOueryDATEPERMUSETO;
    TIBStringField *ibqTxOueryREGIONALCOUNCIL;
    TIBStringField *ibqTxOueryNUMPERMREGCOUNCIL;
    TDateField *ibqTxOueryDATEPERMREGCOUNCIL;
    TBlobField *ibqTxOueryNOTICECOUNT;
    TIBStringField *ibqTxOueryNUMSTANDCERTIFICATE;
    TDateField *ibqTxOueryDATESTANDCERTIFICATE;
    TIBStringField *ibqTxOueryNUMFACTORY;
    TIBStringField *ibqTxOueryRESPONSIBLEADMIN;
    TIBStringField *ibqTxOueryADMINISTRATIONID;
    TSmallintField *ibqTxOueryREGIONALAGREEMENT;
    TDateField *ibqTxOueryDATEINTENDUSE;
    TBlobField *ibqTxOueryAREACOVERAGE;
    TIntegerField *ibqTxOuerySYSTEMCAST_ID;
    TIBStringField *ibqTxOuerySC_CODE;
    TIBStringField *ibqTxOueryCLASSWAVE;
    TSmallintField *ibqTxOueryACCOUNTCONDITION_IN;
    TIBStringField *ibqTxOueryACIN_CODE;
    TSmallintField *ibqTxOueryACCOUNTCONDITION_OUT;
    TIBStringField *ibqTxOueryACOUT_CODE;
    TIBStringField *ibqTxOueryTIMETRANSMIT;
    TSmallintField *ibqTxOueryTYPESYSTEM;
    TIBStringField *ibqTxOueryARS_CODSYSTEM;
    TFloatField *ibqTxOueryARS_DEVIATION;
    TIBStringField *ibqTxOueryATS_NAMESYSTEM;
    TIBStringField *ibqTxOueryATS_TYPE_NAMESYSTEM;
    TFloatField *ibqTxOueryATS_CHANNELBAND;
    TIntegerField *ibqTxOueryCHANNEL_ID;
    TIBStringField *ibqTxOueryNAMECHANNEL;
    TFloatField *ibqTxOueryCH_FREQTO;
    TFloatField *ibqTxOueryCH_FREQFROM;
    TFloatField *ibqTxOueryCH_DEVIATION;
    TFloatField *ibqTxOueryVIDEO_CARRIER;
    TSmallintField *ibqTxOueryVIDEO_OFFSET_LINE;
    TIntegerField *ibqTxOueryVIDEO_OFFSET_HERZ;
    TIBStringField *ibqTxOueryFREQSTABILITY;
    TIBStringField *ibqTxOueryTYPEOFFSET;
    TIBStringField *ibqTxOuerySYSTEMCOLOUR;
    TIBStringField *ibqTxOueryVIDEO_EMISSION;
    TFloatField *ibqTxOueryPOWER_VIDEO;
    TFloatField *ibqTxOueryEPR_VIDEO_MAX;
    TFloatField *ibqTxOueryEPR_VIDEO_HOR;
    TFloatField *ibqTxOueryEPR_VIDEO_VERT;
    TSmallintField *ibqTxOueryALLOTMENTBLOCKDAB_ID;
    TIBStringField *ibqTxOueryABD_NAME;
    TIBStringField *ibqTxOueryBD_NAME;
    TFloatField *ibqTxOueryBLOCKCENTREFREQ;
    TSmallintField *ibqTxOueryGUARDINTERVAL_ID;
    TIntegerField *ibqTxOueryCGI_FREQINTERVAL;
    TIntegerField *ibqTxOueryIDENTIFIERSFN;
    TIBStringField *ibqTxOuerySN_SYNHRONETID;
    TIntegerField *ibqTxOueryRELATIVETIMINGSFN;
    TFloatField *ibqTxOuerySOUND_CARRIER_PRIMARY;
    TIntegerField *ibqTxOuerySOUND_OFFSET_PRIMARY;
    TIBStringField *ibqTxOuerySOUND_EMISSION_PRIMARY;
    TFloatField *ibqTxOueryPOWER_SOUND_PRIMARY;
    TFloatField *ibqTxOueryEPR_SOUND_MAX_PRIMARY;
    TFloatField *ibqTxOueryEPR_SOUND_HOR_PRIMARY;
    TFloatField *ibqTxOueryEPR_SOUND_VERT_PRIMARY;
    TFloatField *ibqTxOueryV_SOUND_RATIO_PRIMARY;
    TSmallintField *ibqTxOueryMONOSTEREO_PRIMARY;
    TFloatField *ibqTxOuerySOUND_CARRIER_SECOND;
    TIntegerField *ibqTxOuerySOUND_OFFSET_SECOND;
    TIBStringField *ibqTxOuerySOUND_EMISSION_SECOND;
    TFloatField *ibqTxOueryPOWER_SOUND_SECOND;
    TFloatField *ibqTxOueryEPR_SOUND_MAX_SECOND;
    TFloatField *ibqTxOueryEPR_SOUND_HOR_SECOND;
    TFloatField *ibqTxOueryEPR_SOUND_VER_SECOND;
    TIBStringField *ibqTxOuerySOUND_SYSTEM_SECOND;
    TFloatField *ibqTxOueryV_SOUND_RATIO_SECOND;
    TIntegerField *ibqTxOueryHEIGHTANTENNA;
    TIntegerField *ibqTxOueryHEIGHT_EFF_MAX;
    TIBStringField *ibqTxOueryPOLARIZATION;
    TIBStringField *ibqTxOueryDIRECTION;
    TFloatField *ibqTxOueryFIDERLOSS;
    TIntegerField *ibqTxOueryFIDERLENGTH;
    TSmallintField *ibqTxOueryANGLEELEVATION_HOR;
    TSmallintField *ibqTxOueryANGLEELEVATION_VER;
    TFloatField *ibqTxOueryANTENNAGAIN;
    TSmallintField *ibqTxOueryTESTPOINTSIS;
    TIBStringField *ibqTxOueryNAMEPROGRAMM;
    TIntegerField *ibqTxOueryUSERID;
    TIBStringField *ibqTxOueryADMIT_NAME;
    TIntegerField *ibqTxOueryORIGINALID;
    TIBStringField *ibqTxOueryNUMREGISTRY;
    TIBStringField *ibqTxOueryTYPEREGISTRY;
    TIBStringField *ibqTxOueryREMARKS;
    TIntegerField *ibqTxOueryRELAYSTATION_ID;
    TIBStringField *ibqTxOueryTXRETR_ADMINISTRATIONID;
    TIBStringField *ibqTxOueryAREA_RETR_NAME;
    TIBStringField *ibqTxOueryCITY_RETR_NAME;
    TFloatField *ibqTxOueryTXRETR_SOUND_CARRIER_PRIMARY;
    TFloatField *ibqTxOueryTXRETR_VIDEO_CARRIER;
    TIntegerField *ibqTxOueryOPERATOR_ID;
    TIBStringField *ibqTxOueryOPER_MAME;
    TIntegerField *ibqTxOueryTYPERECEIVE_ID;
    TIBStringField *ibqTxOueryTREC_NAME;
    TIntegerField *ibqTxOueryLEVELSIDERADIATION;
    TIntegerField *ibqTxOueryFREQSHIFT;
    TSmallintField *ibqTxOuerySUMMATORPOWERS;
    TFloatField *ibqTxOueryAZIMUTHMAXRADIATION;
    TFloatField *ibqTxOuerySUMMATOFREQFROM;
    TFloatField *ibqTxOuerySUMMATORFREQTO;
    TFloatField *ibqTxOuerySUMMATORPOWERFROM;
    TFloatField *ibqTxOuerySUMMATORPOWERTO;
    TFloatField *ibqTxOuerySUMMATORMINFREQS;
    TFloatField *ibqTxOuerySUMMATORATTENUATION;
    TFloatField *ibqTxOueryARS_BAND_WIDTH;
    TStringField *ibqTxOueryLATITUDE_SYMB;
    TStringField *ibqTxOueryLONGITUTE_SYMB;
    TIBStringField *ibqTxOuerySTAND_NAMESITE_ENG;
    TIBStringField *ibqTxOueryCOUNTRY_CODE;
    TStringField *ibqTxOueryLATITUDE_SYMB2;
    TStringField *ibqTxOueryLONGITUDE_SYMB2;
    TIBStringField *d;
    TIBStringField *ibqTxOueryCGI_CODE;
    TIBStringField *ibqTxOueryPOLARIZATION2;
    TIBStringField *ibqTxOueryDIRECTION2;
    TIBStringField *ibqTxOueryDTS_NAMESYSTEM;
    TIBStringField *ibqTxOueryCOUNTRY_CODE2;
    TFloatField *ibqTxOueryEPR_SOUND_HOR_PRIMARY_DBW;
    TFloatField *ibqTxOueryEPR_SOUND_VERT_PRIMARY_DBW;
    TFloatField *ibqTxOueryEPR_VIDEO_MAX_DBW;
    TFloatField *ibqTxOueryEPR_VIDEO_HOR_DBW;
    TFloatField *ibqTxOueryEPR_VIDEO_VERT_DBW;
    TFloatField *ibqTxOueryEPR_SOUND_MAX_PRIMARY_DBW;
    TFloatField *ibqTxOueryCH_FREQCARRIERNICAM;
    TSmallintField *ibqTxOuerySC_ENUMVAL;
    TSmallintField *ibqTxOuerySTATUS;
    TFloatField *ibqTxOueryVIDEO_BAND;
    TFloatField *ibqTxOuerySOUND_BAND;
    TIBStringField *ibqTxOueryATS_DESCR;
    TIBStringField *ibqTxOueryARS_DESCR;
    TIBStringField *ibqTxOueryDTS_DESCR;
    TDateField *ibqTxOueryLICENSE_RFR_DATETO;
    TIBStringField *ibqTxOueryEMC_CONCL_NUM;
    TDateField *ibqTxOueryEMC_CONCL_FROM;
    TDateField *ibqTxOueryEMC_CONCL_TO;
    TIBStringField *ibqTxOueryNR_REQ_NO;
    TDateField *ibqTxOueryNR_REQ_DATE;
    TIBStringField *ibqTxOueryNR_CONCL_NO;
    TDateField *ibqTxOueryNR_CONCL_DATE;
    TIBStringField *ibqTxOueryNR_APPL_NO;
    TDateField *ibqTxOueryNR_APPL_DATE;
    TIBStringField *ibqTxOueryOP_AGCY;
    TIBStringField *ibqTxOueryADDR_CODE;
    TFloatField *ibqTxOueryGND_COND;
    TFloatField *ibqTxOueryMAX_COORD_DIST;
    TDateTimeField *ibqTxOueryDOC_CURRENT_TIME;
    TIBStringField *ibqTxOueryASSOCIATED_ADM_ALLOT_ID;
    TDateTimeField *ibqTxOueryDOC_CURRENT_DATE;
    TFloatField *ibqTxOueryLATITUDE;
    TFloatField *ibqTxOueryLONGITUDE2;
    TFloatField *ibqTxOueryLATITUDE2;
    TFloatField *ibqTxOueryLONGITUDE;
    TStringField *ibqTxOueryNAME;
    TStringField *ibqTxOueryLASTNAME;
    TStringField *ibqTxOueryTEL;
    TStringField *ibqTxOueryIDENT;
    TStringField *ibqTxOueryPOST;
    TStringField *ibqTxOueryNR_LICENSE_NUMLICENSE;
    TStringField *ibqTxOueryNR_LICENSE_DATEFROM;
    TStringField *ibqTxOueryNR_LICENSE_DATETO;
    void __fastcall dbMainAfterConnect(TObject *Sender);
    void __fastcall DataModuleDestroy(TObject *Sender);
    void __fastcall CoordFieldGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall CoordFieldSetText(TField *Sender,
          const AnsiString Text);
    void __fastcall OnGetFieldText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall OnSetFieldText(TField *Sender,
          const AnsiString Text);
    void __fastcall ibqTxOueryCalcFields(TDataSet *DataSet);
private:
    int m_userId;
    int __fastcall GetUserId();	// User declarations
    std::map<AnsiString, int> country_ids;
public:		// User declarations
    Tisc_array_get_slice isc_array_get_slice;
    Tisc_array_put_slice isc_array_put_slice;
    Tisc_array_lookup_bounds isc_array_lookup_bounds;
    Tisc_interprete isc_interprete;
    HANDLE ibLibrary;

    __fastcall TdmMain(TComponent* Owner);
    AnsiString __fastcall coordToStr(double coord, char direction);
    double __fastcall strToCoord(AnsiString& coord);
    int __fastcall getNewId();
    AnsiString __fastcall GetSystemCastName(int scId);
    AnsiString __fastcall getConditionName(int id);
    AnsiString __fastcall getChannelName(int id);
    AnsiString __fastcall getAtsName(int enumval);
    AnsiString __fastcall GetLfMfName(int enumval);
    AnsiString __fastcall getArsName(int enumval);
    AnsiString __fastcall getDabBlockName(double centerFreq);
    AnsiString __fastcall getPolarizationName(int id);
    AnsiString __fastcall getDvbSystemName(int id);
    AnsiString __fastcall getColorName(int colorId);
    AnsiString __fastcall getCountryCode(int cntryId);
    void __fastcall cacheSites(std::set<long>& standIdSet, std::map< int, StandRecord>& standRecords, bool clear = false);
    __property int UserId  = { read=GetUserId };
    int __fastcall RecordCopy(AnsiString, std::map<AnsiString, Variant>, std::map<AnsiString, Variant>);

    // declarations and definitions for state contour uset in digital allotment form
    struct BcCoord
    {
      double lon;
      double lat;
    };
    typedef std::vector<BcCoord> BordSector;
    typedef std::vector<BordSector> BordSectors;

    BordSectors bordSectors;
    double minlon;
    double minlat;
    double maxlon;
    double maxlat;

    void __fastcall loadBordSectors();
    int __fastcall getCountryId(char* code);
    void __fastcall ImportRrc06(int mode);

    std::map<int, DbSectionInfo> dbSectMap;
    DbSectionInfo __fastcall getDbSectionInfo(int id);
    void __fastcall GetList(AnsiString query, TStrings* sl);
    void __fastcall GetAllotZone(int zoneId, LPSAFEARRAY* sp);
    int __fastcall SaveAllotZone(int allotId, double eMin, AnsiString note, LPSAFEARRAY sa);
    void __fastcall DelAllotZones(int allotId);
    void __fastcall DelAllotZone(int zoneId);
    bool __fastcall RunQuery(AnsiString qry, ParamList pl = ParamList(), TIBTransaction *tr = NULL);
    CLSID& __fastcall GetObjClsid(int txType);
    int __fastcall GetSc(int txId);
    bool __fastcall GetIbArray(TIBTransaction *, TIBXSQLVAR *, void *, ISC_LONG len);
    bool __fastcall PutIbArray(TIBTransaction *, TIBXSQLVAR *, void *, ISC_LONG len);

    HRESULT __fastcall LoadObject(LPUNKNOWN obj);
    HRESULT __fastcall SaveObject(LPUNKNOWN obj);
    HRESULT __fastcall LoadDetails(LPUNKNOWN obj);
    String __fastcall GetTableName(int objType);
    String __fastcall GetObjQuery(int objType);
    void __fastcall Init();
    TDataSet* __fastcall GetObject(int objType, int id, TIBTransaction* tr = NULL);
    TDataSet* __fastcall GetDataSet(String qry, String updTable, TIBTransaction* tr = NULL);

    TFloatField *eff_pow_hor[36];
    TFloatField *eff_pow_vert[36];
    TFloatField *eff_height[36];
    TFloatField *eff_ant_gains[36];

    void __fastcall Create36FloatField(TFloatField** field, AnsiString Name, TDataSet *DataSet);
    void __fastcall ReadBlobIntoField(TField* blobField, TFloatField** calc_field);
    double __fastcall EmissionClassToBand(String);
    void __fastcall InitDbControls();
    void __fastcall SetSelectionIcst(int selId, bool setIcst);
protected:

};
//---------------------------------------------------------------------------
extern PACKAGE TdmMain *dmMain;
extern const char* selectTxClause;
extern const char* selectAllotClause;
extern const char* selectTxDetlClause;
extern const char* selectAllotDetlClause;
//---------------------------------------------------------------------------
#endif
