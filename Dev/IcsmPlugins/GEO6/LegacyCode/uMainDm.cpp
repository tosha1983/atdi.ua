//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uMainDm.h"
#include <math.h>
#include <stdio.h>
#include <IniFiles.hpp>
#include <values.h>
#include <memory>
#include "uItuImport.h"
#include "uParams.h"
#include "FormProvider.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TdmMain *dmMain;
const char* selectTxClause = "select TX.ID, st.LATITUDE, st.LONGITUDE, tx.SYSTEMCAST_ID "
        ",tx.TYPESYSTEM, tx.VIDEO_CARRIER, tx.MAX_COORD_DIST "
        ",tx.VIDEO_OFFSET_LINE, tx.VIDEO_OFFSET_HERZ, tx.TYPEOFFSET "
        ",tx.SYSTEMCOLOUR,  tx.POWER_VIDEO, tx.EPR_VIDEO_MAX "
        ",tx.EPR_VIDEO_HOR, tx.EPR_VIDEO_VERT "
        ",tx.IDENTIFIERSFN, tx.RELATIVETIMINGSFN, tx.CHANNEL_ID "
        ",tx.BLOCKCENTREFREQ, tx.SOUND_CARRIER_PRIMARY   "
        ",tx.POWER_SOUND_PRIMARY, tx.EPR_SOUND_MAX_PRIMARY, tx.EPR_SOUND_HOR_PRIMARY "
        ",tx.EPR_SOUND_VERT_PRIMARY, tx.V_SOUND_RATIO_PRIMARY, tx.MONOSTEREO_PRIMARY "
        ",tx.SOUND_CARRIER_SECOND "
        ",tx.POWER_SOUND_SECOND, tx.EPR_SOUND_MAX_SECOND, tx.EPR_SOUND_HOR_SECOND "
        ",tx.EPR_SOUND_VER_SECOND, tx.SOUND_SYSTEM_SECOND, tx.V_SOUND_RATIO_SECOND "
        ",tx.HEIGHTANTENNA, tx.HEIGHT_EFF_MAX, tx.POLARIZATION, tx.DIRECTION "
        ",tx.FIDERLOSS, tx.FIDERLENGTH, tx.ANGLEELEVATION_HOR, tx.ANGLEELEVATION_VER, tx.ANTENNAGAIN "
        ",tx.TESTPOINTSIS, tx.ACCOUNTCONDITION_IN, tx.ACCOUNTCONDITION_OUT, tx.ADMINISTRATIONID, tx.STAND_ID"
        ",tx.EFFECTPOWERHOR, tx.EFFECTPOWERVER, tx.EFFECTHEIGHT, tx.ANT_DIAG_H, tx.ANT_DIAG_V, tx.MAX_COORD_DIST "
        ",CN.CODE ADM_RESPONSE, CN.CODE ADM_SITED_IN, TX.STATUS STATUS_CODE, ST.NAMESITE_ENG STATION_NAME "
        ",ST.HEIGHT_SEA SITE_HEIGHT, CH.NAMECHANNEL CHANNEL, TX.DATECHANGE DATE_OF_LAST_CHANGE "
        ",AR.NUMREGION NUMREGION "
        ",tx.RPC, tx.RX_MODE, tx.POL_ISOL, tx.GND_COND "
        ",SC.ENUMVAL SC_ENUMVAL "
        ",ATS.ENUMVAL AT_ENUMVAL, DTS.ENUMVAL DTS_ENUMVAL, ARS.ENUMVAL ARS_ENUMVAL "
        ",tx.BANDWIDTH, tx.MOD_TYPE, tx.PROT_LEVL "
        ",tx.NOISE_ZONE, tx.GND_COND, tx.BANDWIDTH, tx.ASSOCIATED_ADM_ALLOT_ID, cast('now' as TIMESTAMP) as DOC_CURRENT_TIME "
        ",tx.IS_DVB_T2, tx.PILOT_PATTERN, tx.DIVERSITY, tx.ROTATED_CNSTLS, tx.MODE_OF_EXTNTS"
        ",tx.MODULATION, tx.CODE_RATE, tx.FFT_SIZE, tx.GUARD_INTERVAL"
        "\n from TRANSMITTERS TX "
        " LEFT OUTER JOIN STAND ST on (TX.STAND_ID = ST.ID) "
        " LEFT OUTER JOIN AREA AR on (ST.AREA_ID = AR.ID) "
        " LEFT OUTER JOIN COUNTRY CN on (AR.COUNTRY_ID = CN.ID) "
        " LEFT OUTER JOIN CHANNELS CH on (TX.CHANNEL_ID = CH.ID) "
        " LEFT OUTER JOIN SYSTEMCAST SC on (SC.ID = TX.SYSTEMCAST_ID) "
        " LEFT OUTER JOIN ANALOGTELESYSTEM ATS on (ATS.ID = TX.TYPESYSTEM) "
        " LEFT OUTER JOIN DIGITALTELESYSTEM DTS on (DTS.ID = TX.TYPESYSTEM) "
        " LEFT OUTER JOIN ANALOGRADIOSYSTEM ARS on (ARS.ID = TX.TYPESYSTEM) "
        "where TX.ID in ";

const char* selectTxDetlClause = "select o.*, o.STA_ID OBJ_ID from LFMF_OPER o where STA_ID in ";

const char* selectAllotClause = "select A.*, TX.LATITUDE, TX.LONGITUDE, TX.COORD from DIG_ALLOTMENT A "
        " left outer join TRANSMITTERS TX "
        " on (A.ID = TX.ID) where A.ID in ";

const char* selectAllotDetlClause = "select s.CONTOUR_ID, POINT_NO, LAT, LON, c.CONTOUR_ID TAG, l.ALLOT_ID, l.ALLOT_ID OBJ_ID "
        " from DIG_SUBAREAPOINT s join DIG_ALLOT_CNTR_LNK l on (s.CONTOUR_ID = l.CNTR_ID)"
        " join DIG_CONTOUR c on (s.CONTOUR_ID = c.ID) where l.ALLOT_ID in ";

const char *selectStandClause = " SELECT STAND.ID, STAND.NAMESITE, AREA.NAME AREA_NAME, CITY.NAME CITY_NAME, "
        "AREA.NUMREGION, AREA.COUNTRY_ID"
        " FROM STAND "
        "   LEFT OUTER JOIN AREA ON (STAND.AREA_ID = AREA.ID) "
        "   LEFT OUTER JOIN CITY ON (STAND.CITY_ID = CITY.ID) "
        " where STAND.ID in ";

std::map<int, String> tblNames;
std::map<int, String> objQueries;

void __fastcall SortVoltageVector(VoltageVector& vv, const int lowerIdx, const int higherIdx)
{
    int lowerTrack, higherTrack;
    double middleVal;
    VoltagePair T;

    lowerTrack = lowerIdx;
    higherTrack = higherIdx;
    middleVal = vv[(lowerTrack + higherTrack) / 2].second;

    do {
        //  по убыванию
        while (vv[lowerTrack].second > middleVal)
            lowerTrack++;
        while (vv[higherTrack].second < middleVal)
            higherTrack--;
        if (lowerTrack <= higherTrack) {
            T = vv[lowerTrack]; vv[lowerTrack] = vv[higherTrack]; vv[higherTrack] = T;
            lowerTrack++; higherTrack--;
        }
    } while (lowerTrack <= higherTrack);

    if (higherTrack > lowerIdx)
        SortVoltageVector(vv, lowerIdx, higherTrack);
    if (lowerTrack < higherIdx)
        SortVoltageVector(vv, lowerTrack, higherIdx);
}

//---------------------------------------------------------------------------
__fastcall TdmMain::TdmMain(TComponent* Owner)
    : TDataModule(Owner), m_userId(0)
{
    qryTxList->SQL->Clear();
    qryTxList->SQL->Add(selectTxClause);

    // init funcs not loaded by IBX
    isc_array_get_slice = NULL;
    isc_array_put_slice = NULL;
    isc_array_lookup_bounds = NULL;
    isc_interprete = NULL;

    ibLibrary = LoadLibrary("gds32.dll");
    if (ibLibrary != NULL)
    {
        isc_array_get_slice = (Tisc_array_get_slice)GetProcAddress(ibLibrary, "isc_array_get_slice");
        isc_array_put_slice = (Tisc_array_put_slice)GetProcAddress(ibLibrary, "isc_array_put_slice");
        isc_array_lookup_bounds = (Tisc_array_lookup_desc)GetProcAddress(ibLibrary, "isc_array_lookup_bounds");
        isc_interprete = (Tisc_interprete)GetProcAddress(ibLibrary, "isc_interprete");
    }
    if (ibLibrary == NULL || isc_array_get_slice == NULL || isc_array_put_slice == NULL || isc_interprete == NULL)
        MessageBox(NULL, String().sprintf("Ошибка инициализации IB:\n"
                       "ibLibrary: %p\nisc_array_get_slice: %p\nisc_array_putt_slice: %p\nisc_array_lookup_bounds: %\n"
                        , ibLibrary, isc_array_get_slice, isc_array_put_slice, isc_array_lookup_bounds).c_str(),
                    NULL, MB_ICONEXCLAMATION);
    Init();
}
//---------------------------------------------------------------------------

AnsiString __fastcall TdmMain::coordToStr(double coord, char direction)
{
    int grad, min, sec;
    AnsiString Result;
    char modif;

    if (direction == 'Y')  //latitude
        if (coord > 0)
            modif = 'N';
        else
            modif = 'S';
    else if (direction == 'X') //longitude
        if (coord > 0)
            modif = 'E';
        else
            modif = 'W';
    else {
        return "";
    }

    coord = fabs(coord);
    grad = floor(coord + 1e-12);
    min =  floor((coord - grad) * 60 + 1e-12);
    sec = floor((coord - grad - min / 60.0) * 3600 + 0.5);

    Result = IntToStr(grad) + char(176) + modif + ' ' +
            (min < 10 ? "0" : "") + IntToStr(min) + "' " +
            (sec < 10 ? "0" : "") + IntToStr(sec) + "''";
//    .sprintf("%d%c 02%d' 02%d''", grad, modif, min, sec);
    return Result;

}

double __fastcall TdmMain::strToCoord(AnsiString& Text)
{
    AnsiString s;
    int digbeg, digend, len;
    int grad, min, sec, curval;
    double coord;
    bool negative;
    char curstr[20];

    s = Text;
    grad = 0; min = 0; sec = 0; negative = false;
    len = s.Length();
    digend = 0;
    for (int i = 1; i <= 3; i++) {  // three categories

        for (digbeg = digend; digbeg < len; digbeg++)
            if (isdigit(s.c_str()[digbeg]))
                break;
            else if ((i == 1) && (s.c_str()[digbeg] == '-'))
                negative = true;

        if (digbeg == len)
            break;

        // find the end of digital sequence
        for (digend = digbeg + 1; digend < len; digend++)
            if ((!isdigit(s.c_str()[digend]))
//            || ((i == 1) && (digend == digbeg + 3))
//            || ((i > 1) && (digend == digbeg + 2)))
            || (digend == digbeg + 2))
                break;

        strncpy(curstr, s.c_str()+digbeg, digend-digbeg);
        curstr[digend-digbeg] = '\0';
        sscanf(curstr, "%d", &curval);
//        curval = StrToInt(Copy(s, digbeg, digend-digbeg));
        switch (i) {
            case 1: grad = curval; break;
            case 2: min = curval; break;
            case 3: sec = curval; break;
        }

    }

    coord = grad + (min + sec/60.0) / 60.0;
    if (negative)
        coord = -coord;

    return coord;
}
//---------------------------------------------------------------------------

int __fastcall TdmMain::getNewId()
{
    int newId = 0;
    sqlGetNewId->Close();
    if (!sqlGetNewId->Transaction->InTransaction)
    {
        if (!sqlGetNewId->Transaction->DefaultDatabase->Connected)
            sqlGetNewId->Transaction->DefaultDatabase->Connected = true;
        sqlGetNewId->Transaction->StartTransaction();
    }
    sqlGetNewId->ExecQuery();
    if (!sqlGetNewId->Eof)
        newId = sqlGetNewId->Fields[0]->AsInteger;
    sqlGetNewId->Close();
    return newId;
}
//---------------------------------------------------------------------------

int __fastcall TdmMain::GetUserId()
{
    if (m_userId == 0) {
        sqlUserId->Close();
        sqlUserId->ExecQuery();
        m_userId = sqlUserId->Fields[0]->AsInteger;
    }
    return m_userId;
}
//---------------------------------------------------------------------------

void __fastcall TdmMain::dbMainAfterConnect(TObject *Sender)
{
    // login user
    if (!sqlUserLog->Transaction->Active)
        sqlUserLog->Transaction->StartTransaction();
    try {
        sqlUserLog->Params->Vars[0]->AsInteger = 1;
        sqlUserLog->ExecQuery();
    } catch(...) {}
    sqlUserLog->Close();
    sqlUserLog->Transaction->Commit();

    /*
    if (!qryTxList->Transaction->Active)
        qryTxList->Transaction->StartTransaction();
    qryTxList->SQL->Add(" (1)");
    qryTxList->Fields->Clear();
    qryTxList->Open();
    qryTxList->Fields->Fields[0]->DisplayLabel = "TX.ID";
    qryTxList->Close();
    #ifdef _DEBUG
    MessageBox(NULL, IntToStr(qryTxList->Fields->Count).c_str(), "Number of fields", MB_ICONINFORMATION);
    #endif
    qryTxList->Transaction->Commit();
    */
}
//---------------------------------------------------------------------------

AnsiString __fastcall TdmMain::GetSystemCastName(int scId)
{
    ibdsScList->Open();
    if (ibdsScList->Locate("ENUMVAL", scId, TLocateOptions()))
        return ibdsScListCODE->AsString;
    else
        return "Невідома система";
}

AnsiString __fastcall TdmMain::getConditionName(int id)
{
    ibdsAccCondList->Open();
    if (ibdsAccCondList->Locate("ID", id, TLocateOptions()))
        return ibdsAccCondListCODE->AsString;
    else
        return "";
}

AnsiString __fastcall TdmMain::getChannelName(int id)
{
    ibdsChannelList->Open();
    if (ibdsChannelList->Locate("ID", id, TLocateOptions()))
        return ibdsChannelListNAMECHANNEL->AsString;
    else
        return "";
}

AnsiString __fastcall TdmMain::getAtsName(int enumval)
{
    ibdsAtsList->Open();
    if (ibdsAtsList->Locate("ENUMVAL", enumval, TLocateOptions()))
        return ibdsAtsListNAMESYSTEM->AsString;
    else
        return "Невідома система";
}


AnsiString __fastcall TdmMain::GetLfMfName(int enumval)
{
    ibdsLfMfList->Open();
    if (ibdsLfMfList->Locate("ENUM", enumval, TLocateOptions()))
        return ibdsLfMfListCODE->AsString;
    else
        return "Невідома система";
}

AnsiString __fastcall TdmMain::getArsName(int enumval)
{
    ibdsArsList->Open();
    if (ibdsArsList->Locate("ENUMVAL", enumval, TLocateOptions()))
        return ibdsArsListCODSYSTEM->AsString;
    else
        return "Невідома система";
}

AnsiString __fastcall TdmMain::getDabBlockName(double centerFreq)
{
    ibdsDabBlockName->Open();
    if (ibdsDabBlockName->Locate("CENTREFREQ", centerFreq, TLocateOptions()))
        return ibdsDabBlockNameNAME->AsString;
    else
        return "";
}

AnsiString __fastcall TdmMain::getPolarizationName(int id)
{
    switch (id) {
        case 0/*plVER*/: return 'V'; 
        case 1/*plHOR*/: return 'H';
        case 2/*plMIX*/: return 'M';
        case 3/*plCIR*/: return 'C';
        default: return "";
    }
}

AnsiString __fastcall TdmMain::getDvbSystemName(int enumval)
{
    ibdsDvbSystemList->Open();
    if (ibdsDvbSystemList->Locate("ENUMVAL", enumval, TLocateOptions()))
        return ibdsDvbSystemListNAMESYSTEM->AsString;
    else
        return "";
}

AnsiString __fastcall TdmMain::getColorName(int colorId)
{
    switch (colorId) {
        case 1: return "SECAM";
        case 2: return "PAL";
        case 3: return "NTSC";
        default: return "";
    }
}

AnsiString __fastcall TdmMain::getCountryCode(int cntryId)
{
    ibdsCountries->Open();
    if (ibdsCountries->Locate("ID", cntryId, TLocateOptions()))
        return ibdsCountriesCODE->AsString;
    else
        return "";
}

void __fastcall TdmMain::cacheSites(std::set<long>& standIdSet, std::map<int, StandRecord>& standRecords, bool clear)
{
    if (clear && !standRecords.empty())
        standRecords.clear();

    int maxNo = 1499; //  for interbase          std::list
    std::set<long>::iterator si = standIdSet.begin();

    while (si != standIdSet.end()) {

        AnsiString asStandIds("(");

        int j = 0;
        while (j < maxNo && si != standIdSet.end()) {
            asStandIds = asStandIds  + *(si++) + ",";
            j++;
        }

        if (j > 0) {

            asStandIds[asStandIds.Length()] = ')';
            ibdsStands->Close();
            ibdsStands->SelectSQL->Clear();
            ibdsStands->SelectSQL->Add(selectStandClause);
            ibdsStands->SelectSQL->Add(asStandIds);
            ibdsStands->Open();
            ibdsStands->FetchAll();

            while (!ibdsStands->Eof) {
                StandRecord& sr = standRecords[ibdsStandsID->AsInteger];
                sr.siteName = ibdsStandsNAMESITE->AsString.c_str();
                sr.areaName = ibdsStandsAREA_NAME->AsString.c_str();
                sr.cityName = ibdsStandsCITY_NAME->AsString.c_str();
                sr.regionNum = ibdsStandsNUMREGION->AsString.c_str();
                sr.countryId = ibdsStandsCOUNTRY_ID->AsInteger;
                ibdsStands->Next();
            }
        }

        ibdsStands->Close();

    }
}

int __fastcall TdmMain::RecordCopy(AnsiString TableName, std::map<AnsiString, Variant> id, std::map<AnsiString, Variant> params)
{
    int new_id = dmMain->getNewId();
    int old_id;
    map<AnsiString ,Variant>::iterator iter;
    std::auto_ptr<TIBQuery> selectSql(new TIBQuery(this));
    selectSql->Database = dmMain->dbMain;
    std::auto_ptr<TIBQuery> typeSql(new TIBQuery(this));
    typeSql->Database = dmMain->dbMain;
    AnsiString str = "select * from " + TableName + " where ";
    AnsiString type ="select tx.id, s.enumval from transmitters tx left outer join systemcast s on (tx.systemcast_id = s.id) where tx.id = :id";
    int typeInt = 0;;
    int i = 0;
    for(iter = id.begin(); iter != id.end();++iter, ++i)
    {
      if(i != 0)
        str += " and ";
      str += iter->first + " = :" + iter->first;
      
    }
    iter = id.begin();
    if(i = 1 && iter->first == "ID")
    {
        
        typeSql->SQL->Text = type;
        typeSql->Params->ParamByName("id")->Value = iter->second;
        typeSql->Open();
        typeInt = typeSql->FieldByName("enumval")->AsInteger;
    }
    selectSql->SQL->Text = str;
    for(iter = id.begin(); iter != id.end();++iter)
    {
        selectSql->Params->ParamByName(iter->first)->Value = iter->second;
        
    }
    try {
        selectSql->Open();
    } catch(Exception &e) {
        Application->MessageBox(AnsiString("Помилка відкриття запросу підготовки копіювання" + e.Message).c_str(), Application->Title.c_str(), MB_ICONHAND);
        return 0;
    }
    if(!selectSql->IsEmpty()) {
        AnsiString InsertSQL = "insert into " + TableName + "(";
        AnsiString temp;
        Variant tempVar = "NULL";
        for(int i = 0; i < selectSql->FieldCount; i++) {
            InsertSQL += selectSql->Fields->Fields[i]->FieldName+",";
        }

        InsertSQL =InsertSQL.SubString(1,InsertSQL.Length()-1);
        InsertSQL += ") values (";
        for(int i = 0; i < selectSql->FieldCount; i++) {
            InsertSQL += (":"+selectSql->Fields->Fields[i]->FieldName)+",";
        }
        InsertSQL = InsertSQL.SubString(1,InsertSQL.Length()-1);
        InsertSQL += ")";
        std::auto_ptr<TIBQuery> insertSql(new TIBQuery(this));
        insertSql->Database = dmMain->dbMain;
        insertSql->Close();
        insertSql->SQL->Text = InsertSQL;
        insertSql->Prepare();
        for(int i = 0; i < insertSql->Params->Count; i++)
        {
            TParam* insParam = insertSql->Params->Items[i];
            TField* orgField = selectSql->Fields->FieldByName(insParam->Name);
            try {
                if (insParam->Name == "ID")
                {
                    old_id = selectSql->Fields->FieldByName("ID")->AsInteger;
                    insParam->AsInteger = new_id;
                }
                else if (!orgField->IsNull) {
                    if (orgField->DataType == ftBlob) {
                        TStream* blobStreamR = selectSql->CreateBlobStream(orgField, bmRead);
                        insParam->LoadFromStream(blobStreamR, ftBlob);
                    } else
                        {
                            iter = params.find(insParam->Name);
                            if (iter == params.end())
                                insParam->Value  = orgField->Value;
                            else if (!iter->second.IsNull())
                                insParam->Value  = iter->second;
                        }
                }
            } catch(Exception &e) {
                Application->MessageBox(AnsiString("Помилка копіювання поля " + e.Message).c_str(), Application->Title.c_str(), MB_ICONHAND);
            }
        }
        try {
            insertSql->Open();
            if(TableName.UpperCase() == "TRANSMITTERS" && typeInt == ttAM)
            {

                std::map<AnsiString, Variant> paramsLFMF;
                paramsLFMF["STA_ID"] = old_id;
                paramsLFMF["DAYNIGHT"] = "HJ";
                std::map<AnsiString, Variant> paramsLFMFIns;
                paramsLFMFIns["STA_ID"] = new_id;
                RecordCopy("LFMF_OPER", paramsLFMF, paramsLFMFIns);
                paramsLFMF["DAYNIGHT"] = "HN";
                RecordCopy("LFMF_OPER", paramsLFMF, paramsLFMFIns);
            }
        } catch(Exception &e) {
            Application->MessageBox(AnsiString("Помилка відкриття запросу копіювання даних " + e.Message).c_str(), Application->Title.c_str(), MB_ICONHAND);
            return 0;
        }
        return new_id;
    }
    else
        return 0;
}
void __fastcall TdmMain::DataModuleDestroy(TObject *Sender)
{
    // logout user
    if (dbMain->Connected)
    try {
        sqlUserLog->Params->Vars[0]->AsInteger = 0;
        sqlUserLog->ExecQuery();
        sqlUserLog->Transaction->Commit();
        sqlUserLog->Close();
        if (dbMain->DefaultTransaction->Active)
            dbMain->DefaultTransaction->Rollback();

        dbMain->Close();
    } catch(...){
    }

    if (ibLibrary != NULL)
        FreeLibrary(ibLibrary);
}
//---------------------------------------------------------------------------

void __fastcall TdmMain::loadBordSectors()
{
    minlon = 180.0;
    minlat = 90.0;
    maxlon = -180.0;
    maxlat = -90.0;

    bordSectors.clear();

    std::auto_ptr<TIniFile> ini (new TIniFile(ChangeFileExt(Application->ExeName, ".ini")));
    int id = ini->ReadInteger("Common", "DisplayCntryInAllotment", 1);
    ini->WriteInteger("Common", "DisplayCntryInAllotment", id);

    std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
    sql->Database = dbMain;
    sql->Transaction->CommitRetaining();
    sql->SQL->Text = "select cp.ID, cp.LATITUDE, cp.LONGITUDE, cp.NUMBOUND "
                    "from COORDPOINTS CP left outer join COUNTRYPOINTS PT on (pt.NUMBOUND = cp.NUMBOUND) ";
    if (id > 0)
        sql->SQL->Add("where pt.COUNTRY_ID = " + IntToStr(id));
    sql->SQL->Add(" order by cp.NUMBOUND, cp.ID");

    int currentCont = 0;
    BordSector curSubzone;
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        int auxNo = sql->FieldByName("NUMBOUND")->AsInteger;
        if (currentCont != auxNo)
        {
            if (!curSubzone.empty())
            {
                bordSectors.push_back(curSubzone);
                curSubzone.clear();
            }
            currentCont = auxNo;
        }

        BcCoord coord;
        coord.lon = sql->FieldByName("LONGITUDE")->AsDouble;
        coord.lat = sql->FieldByName("LATITUDE")->AsDouble;

        if (minlon > coord.lon)
            minlon = coord.lon;
        if (minlat > coord.lat)
            minlat = coord.lat;
        if (maxlon < coord.lon)
            maxlon = coord.lon;
        if (maxlat < coord.lat)
            maxlat = coord.lat;

        curSubzone.push_back(coord);
    }
    if (!curSubzone.empty())
    {
        bordSectors.push_back(curSubzone);
        curSubzone.clear();
    }
}

int __fastcall TdmMain::getCountryId(char* code)
{
    if (country_ids.empty())
    {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dbMain;
        sql->SQL->Text = "select CODE, ID from COUNTRY";
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
            country_ids[sql->Fields[0]->AsString] = sql->Fields[1]->AsInteger;
    }

    std::map<AnsiString, int>::iterator i = country_ids.find(AnsiString(code));
    if (i != country_ids.end())
        return i->second;
    else
        return -1;
}

DbSectionInfo __fastcall TdmMain::getDbSectionInfo(int id)
{
    if (dbSectMap.empty())
    {
        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dbMain;
        sql->SQL->Text = "SELECT ID, SECTIONNAME, CAN_MODIFY, CAN_DELETE FROM DATABASESECTION";
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
        {
            DbSectionInfo dbsi;
            dbsi.name = sql->Fields[1]->AsString;
            dbsi.allowChange = sql->Fields[2]->AsInteger;
            dbsi.allowDelete = sql->Fields[3]->AsInteger;                                                   
            dbSectMap[sql->Fields[0]->AsInteger] = dbsi;
        }
    }

    std::map<int, DbSectionInfo>::iterator i = dbSectMap.find(id);
    if (i != dbSectMap.end())
        return i->second;
    else
        throw *(new Exception("Неизвестный раздел БД"));
}

void __fastcall TdmMain::ImportRrc06(int mode)
{
    std::auto_ptr<TfrmRrc06Import> importForm(new TfrmRrc06Import(Application));
    importForm->importMode = (TfrmRrc06Import::ImportMode)mode;
    importForm->ShowModal();
}

void __fastcall TdmMain::GetList(AnsiString query, TStrings * sl)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dbMain;
    sql->SQL->Text = query;
    sl->Clear();
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
        sl->AddObject(sql->Fields[1]->AsString, reinterpret_cast<TObject*>(sql->Fields[0]->AsInteger));
}

void __fastcall TdmMain::GetAllotZone(int id, LPSAFEARRAY* sa/*, bool isSfn*/)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dbMain;
    sql->SQL->Text = AnsiString("select DIST from ") +
                    //isSfn ? "DIG_SFN_ZONE_POINT" :
                    "DIG_ALLOT_ZONE_POINT" +
                    " where ZONE_ID = "+IntToStr(id)+" order by POINT_NO";
    std::vector<double> zone;
    zone.reserve(36);
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
        zone.push_back(sql->Fields[0]->AsDouble);

    SAFEARRAYBOUND sab[1];
    sab[0].cElements = zone.size();
    sab[0].lLbound = 0;
    *sa = SafeArrayCreate(VT_R8, 1, sab);
    for (int i = 0; i < zone.size(); i++)
        ((double*)(*sa)->pvData)[i] = zone[i];
}

int __fastcall TdmMain::SaveAllotZone(int allotId, double eMin, AnsiString note, LPSAFEARRAY sa)
{
    int id = getNewId();

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dbMain;
    sql->SQL->Text = "insert into DIG_ALLOT_ZONE (id, allot_id, emin, note) values (:id, :allot_id, :emin, :note)";
    sql->ParamByName("id")->AsInteger = id;
    sql->ParamByName("allot_id")->AsInteger = allotId;
    sql->ParamByName("emin")->AsDouble = eMin;
    sql->ParamByName("note")->AsString = note;
    sql->ExecQuery();
    sql->Close();

    sql->SQL->Text = "insert into DIG_ALLOT_ZONE_POINT (zone_id, point_no, dist) values ("+IntToStr(id)+", :point_no, :dist)";
    sql->Prepare();
    for (int i = 0; i < sa->rgsabound[0].cElements; i++)
    {
        sql->ParamByName("point_no")->AsInteger = i;
        sql->ParamByName("dist")->AsDouble = ((double*)sa->pvData)[i];
        sql->ExecQuery();
        sql->Close();
    }

    sql->Transaction->CommitRetaining();

    return id;
}

void __fastcall TdmMain::DelAllotZones(int allotId)
{
    RunQuery("delete from DIG_ALLOT_ZONE where allot_id = "+ IntToStr(allotId));
    dbMain->DefaultTransaction->CommitRetaining();
}

void __fastcall TdmMain::DelAllotZone(int zoneId)
{
    RunQuery("delete from DIG_ALLOT_ZONE where id = "+ IntToStr(zoneId));
    dbMain->DefaultTransaction->CommitRetaining();
}

bool __fastcall TdmMain::RunQuery(AnsiString queryText, ParamList pl, TIBTransaction *tr)
{
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dbMain;
    if (tr != NULL)
        sql->Transaction = tr;
    sql->SQL->Text = queryText;
    if (pl.size())
    {
        for (ParamList::iterator pli = pl.begin(); pli != pl.end(); pli++)
            sql->ParamByName(pli->first)->AsVariant = pli->second;
    }

    sql->ExecQuery();
    return sql->Eof;
}

CLSID& __fastcall TdmMain::GetObjClsid(int txType)
{
    switch ((TBCTxType)txType)
    {
        case ttAllot: return GUID(CLSID_LisBcDigAllot);
        default: return GUID(CLSID_LISBCTx);
    }
}

int __fastcall TdmMain::GetSc(int txId)
{
    sqlTxSysCast->Close();
    if (!sqlTxSysCast->Prepared)
        sqlTxSysCast->Prepare();
    sqlTxSysCast->ParamByName("ID")->AsInteger = txId;
    sqlTxSysCast->ExecQuery();
    return sqlTxSysCast->Fields[0]->AsInteger;
}
//---------------------------------------------------------------------------

bool __fastcall TdmMain::GetIbArray(TIBTransaction *tr, TIBXSQLVAR *fld, void *arr, ISC_LONG len)
{
    if (isc_array_get_slice && !fld->IsNull)
    {
        ISC_STATUS status_vector[20];
        TGDS_QUAD arrayId = fld->AsQuad;
        TISC_DB_HANDLE hdb = tr->DefaultDatabase->Handle;
        TISC_TR_HANDLE htr = tr->Handle;
        TISC_ARRAY_DESC desc;
        isc_array_lookup_bounds(
            status_vector, &hdb, &htr,
            fld->Data->relname, fld->Data->sqlname, &desc
            );
        if (!(status_vector[0] == 1 && status_vector[1]))
            isc_array_get_slice(
                status_vector, &hdb, &htr,
                &arrayId, &desc, arr, &len
                );
        if (status_vector[0] == 1 && status_vector[1])
        {
            ISC_STATUS *p = status_vector;
            String msg;
            char buff[2048];
            int len;
            while (len = isc_interprete(buff, &p))
                msg = msg + '\n' + buff;
            MessageBox(NULL, (String("Ошибка при получении массива ")+
                                desc.array_desc_relation_name+"."+desc.array_desc_field_name+":"+msg).c_str(),
                        NULL, MB_ICONEXCLAMATION);
            return false;
        }
        return true;
    }
    else
        return false; // no such function
}

bool __fastcall TdmMain::PutIbArray(TIBTransaction *tr, TIBXSQLVAR *fld, void *arr, ISC_LONG len)
{
    if (isc_array_put_slice)
    {
        ISC_STATUS status_vector[20];
        TGDS_QUAD arrayId = {0,0};
        TISC_DB_HANDLE hdb = tr->DefaultDatabase->Handle;
        TISC_TR_HANDLE htr = tr->Handle;
        TISC_ARRAY_DESC desc;
        isc_array_lookup_bounds(
            status_vector, &hdb, &htr,
            fld->Data->relname, fld->Data->sqlname, &desc
            );
        if (!(status_vector[0] == 1 && status_vector[1]))
            isc_array_put_slice(
                status_vector, &hdb, &htr,
                &arrayId, &desc, arr, &len
                );
        if (!(status_vector[0] == 1 && status_vector[1]))
            fld->AsQuad = arrayId;
        if (status_vector[0] == 1 && status_vector[1])
        {
            ISC_STATUS *p = status_vector;
            String msg;
            char buff[2048];
            int len;
            while (len = isc_interprete(buff, &p))
                msg = msg + '\n' + buff;
            MessageBox(NULL, (String("Ошибка при записи массива ")+
                                desc.array_desc_relation_name+"."+desc.array_desc_field_name+":"+msg).c_str(),
                        NULL, MB_ICONEXCLAMATION);
            return false;
        }
        return true;
    }
    else
        return false; // no such function
}
//

void __fastcall TdmMain::CoordFieldGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    Text = Sender->IsNull ? String() :
                            Sender->FieldName.UpperCase().Pos("LAT") > 0 ?
                                coordToStr(Sender->AsFloat, 'Y') :
                                coordToStr(Sender->AsFloat, 'X');

}
//---------------------------------------------------------------------------

void __fastcall TdmMain::CoordFieldSetText(TField *Sender,
      const AnsiString Text)
{
    Sender->AsFloat = strToCoord(Text);
}
//---------------------------------------------------------------------------

HRESULT __fastcall TdmMain::LoadObject(LPUNKNOWN obj)
{
    ILISBCTxPtr tx;
    OleCheck(obj->QueryInterface(IID_ILISBCTx, (void**)&tx));

    long id;
    tx->get_id(&id);

    std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
    sql->Database = dbMain;

    ILisBcDigAllotPtr allot;
    obj->QueryInterface(IID_ILisBcDigAllot, (void**)&allot);
    String selectClause(allot.IsBound() ? selectAllotClause : selectTxClause);
    sql->SQL->Text = selectClause + "(" + IntToStr(id) + ")";
    sql->ExecQuery();
    //if (sql->Eof)
    //    return Error(AnsiString().sprintf("Объекта с ID = %d в БД нет", id).c_str(), IID_ILISBCTx);
    return tx->loadFromQuery((long)sql.get());
}

HRESULT __fastcall TdmMain::SaveObject(LPUNKNOWN obj)
{
    ILISBCTxPtr tx;
    OleCheck(obj->QueryInterface(IID_ILISBCTx, (void**)&tx));

    return S_OK;
}

HRESULT __fastcall TdmMain::LoadDetails(LPUNKNOWN obj)
{
    ILISBCTxPtr tx;
    OleCheck(obj->QueryInterface(IID_ILISBCTx, (void**)&tx));
    TBCTxType tt = ttUNKNOWN;
    tx->get_systemcast(&tt);
    long id;
    tx->get_id(&id);

    static std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
    sql->Close();
    sql->Database = dbMain;
    String qryText;

    if (tt == ttAM)
    {
        qryText = String(selectTxDetlClause) + "("+IntToStr(id)+") ";
    }
    else if (tt == ttAllot)
    {
        ILisBcDigAllotPtr allot;
        OleCheck(obj->QueryInterface(IID_ILisBcDigAllot, (void**)&allot));

        WideString uniq_country;
        allot->get_geo_area(&uniq_country); // here object will be loaded, if still not

        if (uniq_country.Length() > 0)
        {
            qryText = "select 0 as CONTOUR_ID, cp.ID as POINT_NO, "
                "cp.LATITUDE as LAT, cp.LONGITUDE as LON, 0 as TAG, " + IntToStr(id) + " as ALLOT_ID"
                " from COORDPOINTS CP join COUNTRYPOINTS PT on (pt.NUMBOUND = cp.NUMBOUND) "
                " join COUNTRY ctr on (pt.COUNTRY_ID = ctr.ID) "
                " where ctr.CODE = '" + AnsiString(uniq_country) +
                "' order by cp.NUMBOUND, cp.ID ";
        } else {
            qryText = String(selectAllotDetlClause) + "("+IntToStr(id)+") order by s.CONTOUR_ID, POINT_NO";
        }
    }

    if (!qryText.IsEmpty())
    {
        sql->SQL->Text = qryText;
        if (sql->Params->Names.Pos("OBJ_ID") > 0)
            sql->ParamByName("OBJ_ID")->AsInteger = id;
        sql->ExecQuery();
        return tx->loadFromQuery((long)sql.get());
    }
    return S_OK;
}

String __fastcall TdmMain::GetTableName(int objType)
{
    return tblNames[objType];
}

String __fastcall TdmMain::GetObjQuery(int objType)
{
    return objQueries[objType];
}

void __fastcall TdmMain::Init()
{
    tblNames[otDocument] = "LETTERS";
    tblNames[otDocTemplate] = "DOCUMENT";
    tblNames[otTx] = "TRANSMITTERS";
    tblNames[otTxSearch] = "TRANSMITTERS";
    objQueries[otDocument] = "select l.*, o.NAME OrgName, d.NAME TemplName, d.TTYPE, l.DOCUMENT_ID tempId, d.RTTYPE, d.DOCTYPE, "
        "acin.NAME State, t.ADM_REF_ID, s.NAMESITE, c.CODE, c.ENUMVAL, t.ADMINISTRATIONID "
        "from LETTERS l"
        " left join ACCOUNTCONDITION acin on l.ACCOUNTCONDITION_ID = acin.ID"
        " left join TELECOMORGANIZATION o on l.TELECOMORGANIZATION_ID = o.ID"
        " left join DOCUMENT d on l.DOCUMENT_ID = d.ID"
        " left join TRANSMITTERS t on l.TRANSMITTERS_ID = t.ID"
        " left join STAND s on t.STAND_ID = s.ID "
        " left join SYSTEMCAST c on t.SYSTEMCAST_ID = c.ID\n";
    objQueries[otDocTemplate] = "select * from DOCUMENT\n";
    objQueries[otTx] = "SELECT TX.ID TX_ID, ST.LATITUDE TX_LAT, ST.LONGITUDE TX_LONG, SC.CODE SC_CODE\n"
        ",SC.ENUMVAL S_ENUMVAL, CH.NAMECHANNEL, ACIN.CODE ACIN_CODE, ACOUT.CODE ACOUT_CODE"
        ",STAND.NAMESITE, AREA.NAME AREA_NAME, CITY.NAME CITY_NAME, AREA.NUMREGION"
        ",BD.NAME BD_NAME"", ADEL.NAME USERDELNAME"", DBSECT.SECTIONNAME "
        ",TX.STAND_ID TX_STAND_ID, tx.CARRIER CHANNELFREQBLOCK, 0 ERP_MAX, 0 POWER, VIDEO_OFFSET_HERZ/1000.0 OFFSET_KHZ"
        ",CN.CODE CN_CODE"
        ",TX.* "
        " FROM TRANSMITTERS TX"
        " LEFT OUTER JOIN DATABASESECTION DBSECT ON (TX.STATUS = DBSECT.ID)"
        " LEFT OUTER JOIN ACCOUNTCONDITION ACIN ON (TX.ACCOUNTCONDITION_IN = ACIN.ID)"
        " LEFT OUTER JOIN ACCOUNTCONDITION ACOUT ON (TX.ACCOUNTCONDITION_OUT = ACOUT.ID)"
        " LEFT OUTER JOIN SYSTEMCAST SC ON (TX.SYSTEMCAST_ID = SC.ID)"
        " LEFT OUTER JOIN CHANNELS CH ON (TX.CHANNEL_ID = CH.ID)"
        " LEFT OUTER JOIN STAND ST ON (TX.STAND_ID = ST.ID)"
        " LEFT OUTER JOIN AREA ON (ST.AREA_ID = AREA.ID)"
        " LEFT OUTER JOIN COUNTRY CN on (CN.ID = AREA.COUNTRY_ID)"
        " LEFT OUTER JOIN CITY ON (ST.CITY_ID = CITY.ID)"
        " LEFT OUTER JOIN BLOCKDAB BD ON (TX.ALLOTMENTBLOCKDAB_ID = BD.ID)"
        " LEFT OUTER JOIN ADMIT ADEL ON (TX.USER_DELETED = ADEL.ID)"
        " \nwhere \nAREA.COUNTRY_ID = :GRP_ID \nand TX.STATUS = :DB_SECTION_ID "
        " order by AREA.NUMREGION, SC.ENUMVAL, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, CITY.NAME, TX.ADMINISTRATIONID\n";
    objQueries[otTxSearch] = "SELECT TX.ID TX_ID, ST.LATITUDE TX_LAT, ST.LONGITUDE TX_LONG, SC.CODE SC_CODE\n"
        ",SC.ENUMVAL S_ENUMVAL, CH.NAMECHANNEL, ACIN.CODE ACIN_CODE, ACOUT.CODE ACOUT_CODE"
        ",STAND.NAMESITE, AREA.NAME AREA_NAME, CITY.NAME CITY_NAME, AREA.NUMREGION"
        ",BD.NAME BD_NAME"", ADEL.NAME USERDELNAME"", DBSECT.SECTIONNAME "
        ",TX.STAND_ID TX_STAND_ID, tx.CARRIER CHANNELFREQBLOCK, 0 ERP_MAX, 0 POWER, VIDEO_OFFSET_HERZ/1000.0 OFFSET_KHZ"
        ",CN.CODE CN_CODE"
        ",TX.* "
        " FROM TRANSMITTERS TX"
        " LEFT OUTER JOIN DATABASESECTION DBSECT ON (TX.STATUS = DBSECT.ID)"
        " LEFT OUTER JOIN ACCOUNTCONDITION ACIN ON (TX.ACCOUNTCONDITION_IN = ACIN.ID)"
        " LEFT OUTER JOIN ACCOUNTCONDITION ACOUT ON (TX.ACCOUNTCONDITION_OUT = ACOUT.ID)"
        " LEFT OUTER JOIN SYSTEMCAST SC ON (TX.SYSTEMCAST_ID = SC.ID)"
        " LEFT OUTER JOIN CHANNELS CH ON (TX.CHANNEL_ID = CH.ID)"
        " LEFT OUTER JOIN STAND ST ON (TX.STAND_ID = ST.ID)"
        " LEFT OUTER JOIN AREA ON (ST.AREA_ID = AREA.ID)"
        " LEFT OUTER JOIN COUNTRY CN on (CN.ID = AREA.COUNTRY_ID)"
        " LEFT OUTER JOIN CITY ON (ST.CITY_ID = CITY.ID)"
        " LEFT OUTER JOIN BLOCKDAB BD ON (TX.ALLOTMENTBLOCKDAB_ID = BD.ID)"
        " LEFT OUTER JOIN ADMIT ADEL ON (TX.USER_DELETED = ADEL.ID)"
        " \nwhere \nAREA.COUNTRY_ID = :GRP_ID \nand TX.STATUS = :DB_SECTION_ID "
        " order by AREA.NUMREGION, SC.ENUMVAL, TX.VIDEO_CARRIER, TX.SOUND_CARRIER_PRIMARY, CITY.NAME, TX.ADMINISTRATIONID\n";
}

TDataSet* __fastcall TdmMain::GetObject(int objType, int id, TIBTransaction* tr)
{
    static String funcId("TdmMain::GetObject()");

    String qry;
    try {
        qry = GetObjQuery(objType);
    } catch (Exception &e) {
        throw *(new Exception(funcId+": "+e.Message, objType));
    }

    String tableName = tblNames[objType];
    qry += " where "+tableName+".ID = " + IntToStr(id);

    TDataSet* ds = GetDataSet(qry, tableName, tr);

    if (ds->Eof)
    {
        if (id != 0)
        {
            delete ds;
            throw *(new Exception(funcId+": No object with id = "+IntToStr(id)+" in "+tableName+" table"));
        }
        ds->Insert();
        TField *f = ds->FindField("ID");
        if (f)
            f->AsInteger = getNewId();
        else
        {
            delete ds;
            throw *(new Exception(funcId+": No ID field in "+tableName+" table"));
        }
    }
    return ds;
}

TDataSet* __fastcall TdmMain::GetDataSet(String qry, String updTable, TIBTransaction* tr)
{
    TIBDataSet* ibds = new TIBDataSet(this);
    ibds->Database = dbMain;
    if (tr)
        ibds->Transaction = tr;
    ibds->SelectSQL->Text = qry;

    std::auto_ptr<TStringList> sl(new TStringList());
    if (!updTable.IsEmpty())
    {
        GetList("select 0, RDB$FIELD_NAME from RDB$RELATION_FIELDS where RDB$RELATION_NAME = UPPER('"+updTable+"')", sl.get());
        String insQry, insQryVal, updQry, whereCls, colon(":");
        bool noId = sl->IndexOf("ID") == -1;
        for (int i = 0; i < sl->Count; i++)
        {
            String fn = sl->Strings[i].Trim();
            insQry += (fn + ',');
            insQryVal += (colon + fn + ',');
            updQry += (fn + '='+':' + fn + ',');
            if (noId)
            {
                if (whereCls.Length() > 0)
                    whereCls += " and ";
                whereCls += (fn+" = :OLD_"+fn);
            }
        }
        if (noId)
        {
            String keyFields;
            GetList("select 0, s.RDB$FIELD_NAME from RDB$INDEX_SEGMENTS s "
                    "left join RDB$INDICES i on i.RDB$INDEX_NAME = s.RDB$INDEX_NAME "
                    "where i.RDB$RELATION_NAME = UPPER('"+updTable+"') and i.RDB$UNIQUE_FLAG = 1 "
                    "order by s.RDB$FIELD_POSITION ", sl.get());
            for (int i = 0; i < sl->Count; i++)
            {
                String fn = sl->Strings[i].Trim();
                if (keyFields.Length() > 0)
                    keyFields += " and ";
                keyFields += (fn+" = :OLD_"+fn);
            }
            if (keyFields.Length() > 0)
                whereCls = keyFields;
        }
        else
            whereCls = "ID = :OLD_ID";

        if (insQry.Length() > 0)
        {
            insQry.SetLength(insQry.Length() - 1);
            insQryVal.SetLength(insQryVal.Length() - 1);
            updQry.SetLength(updQry.Length() - 1);
            ibds->ModifySQL->Text = "update " + updTable + " set " + updQry + " where "+whereCls+" ";
            ibds->InsertSQL->Text = "insert into " + updTable + "(" + insQry + ")values("+insQryVal+')';
            ibds->DeleteSQL->Text = "delete from " + updTable + " where "+whereCls+" ";
        }
    }

    TDataSet* ds = ibds;

    ds->Open();
    for (int i = 0; i < ds->Fields->Count; i++)
    {
        ds->Fields->Fields[i]->OnSetText = OnSetFieldText;
        ds->Fields->Fields[i]->OnGetText = OnGetFieldText;
        if (ds->Fields->Fields[i]->FieldName != "ID")
            ds->Fields->Fields[i]->Required = false;
    }

    return ds;
}
//---------------------------------------------------------------------------

void __fastcall TdmMain::OnGetFieldText(TField *Sender, AnsiString &Text,
      bool DisplayText)
{
    if (Sender->IsNull)
        return;

    if (Sender->FieldName == "LON" || Sender->FieldName == "LONA" || Sender->FieldName == "LONB"
        || Sender->FieldName == "LAT" || Sender->FieldName == "LATA" || Sender->FieldName == "LATB")
        CoordFieldGetText(Sender, Text, DisplayText);
    else
        Text = Sender->AsString;
}
//---------------------------------------------------------------------------

void __fastcall TdmMain::OnSetFieldText(TField *Sender,
      const AnsiString Text)
{
    if (Text.IsEmpty())
    {
        Sender->SetData(NULL);
        return;
    }

    if (Sender->FieldName == "LON" || Sender->FieldName == "LAT")
        CoordFieldSetText(Sender, Text);
    else
        Sender->AsString = Text;
}
//---------------------------------------------------------------------------


void __fastcall TdmMain::ibqTxOueryCalcFields(TDataSet *DataSet)
{
    double DValue;
    try {
        ReadBlobIntoField(ibqTxOueryEFFECTPOWERHOR, eff_pow_hor);
        ReadBlobIntoField(ibqTxOueryEFFECTPOWERVER, eff_pow_vert);
        if (ibqTxOueryPOLARIZATION->AsString == "V")
            ReadBlobIntoField(ibqTxOueryANT_DIAG_V, eff_ant_gains);
        else
            ReadBlobIntoField(ibqTxOueryANT_DIAG_H, eff_ant_gains);
        ReadBlobIntoField(ibqTxOueryEFFECTHEIGHT, eff_height);
    } catch (Exception &e) {
        throw *(new Exception(AnsiString("Помилка визначення значень BLOB-полів БД: ")+e.Message));
    }

    AnsiString lat,lon;
    ibqTxOueryLATITUDE_SYMB->AsString = lat = dmMain->coordToStr(ibqTxOueryLATITUDE->AsFloat,'Y');
    ibqTxOueryLONGITUTE_SYMB->AsString = lon = "0" + dmMain->coordToStr(ibqTxOueryLONGITUDE->AsFloat, 'X');

    lat.Delete(lat.AnsiPos(AnsiString(char(176))),1);
    lat.Delete(lat.AnsiPos("' "),2);
    lat.Delete(lat.AnsiPos("''"),2);
    lat.Delete(lat.AnsiPos(" "),1);
    lon.Delete(lon.AnsiPos(AnsiString(char(176))),1);
    lon.Delete(lon.AnsiPos("' "),2);
    lon.Delete(lon.AnsiPos("''"),2);
    lon.Delete(lon.AnsiPos(" "),1);

    ibqTxOueryLATITUDE_SYMB2->AsString = lat;
    ibqTxOueryLONGITUDE_SYMB2->AsString = lon;

    ibqTxOueryVIDEO_BAND->AsFloat = EmissionClassToBand(ibqTxOueryVIDEO_EMISSION->AsString);
    ibqTxOuerySOUND_BAND->AsFloat = EmissionClassToBand(ibqTxOuerySOUND_EMISSION_PRIMARY->AsString);

    static std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
    sql->Close();
    sql->Database = dbMain;

    int id =  ibqTxOueryID->AsInteger;
    sql->SQL->Text = "select link.LIC_ID ID, link.TX_ID, lic.NUMLICENSE, lic.DATEFROM, lic.DATETO "
               "from NR_LIC_LINK link "
               "left outer join LICENSE lic on (lic.ID = link.LIC_ID) "
               "left outer join OWNER o on (lic.OWNER_ID = o.ID) "
               "where link.TX_ID = " + IntToStr(id);
    sql->ExecQuery();

    AnsiString nums, datesto, datesfrom;

    for(; !sql->Eof; )
    {
        nums += sql->FieldByName("NUMLICENSE")->AsString;
        datesto += sql->FieldByName("DATETO")->AsString;
        datesfrom += sql->FieldByName("DATEFROM")->AsString;

         sql->Next();

        nums += (sql->Eof ? "" : ", ");
        datesto += (sql->Eof ? "" : ", ");
        datesfrom += (sql->Eof ? "" : ", ");

    }

    ibqTxOueryNR_LICENSE_NUMLICENSE->AsString = nums;
    ibqTxOueryNR_LICENSE_DATEFROM->AsString = datesfrom;
    ibqTxOueryNR_LICENSE_DATETO->AsString = datesto;

    sql->Close();
    sql->SQL->Text = "select * from ADMIT where ID = :ID";
    sql->ParamByName("ID")->AsInteger = UserId;
    sql->ExecQuery();

    ibqTxOueryNAME->AsString = sql->FieldByName("NAME")->AsString;
    ibqTxOueryLASTNAME->AsString = sql->FieldByName("LASTNAME")->AsString;
    ibqTxOueryIDENT->AsString = sql->FieldByName("IDENT")->AsString;
    ibqTxOueryTEL->AsString = sql->FieldByName("TEL")->AsString;
    ibqTxOueryPOST->AsString = sql->FieldByName("POST")->AsString;

}
//---------------------------------------------------------------------------

void __fastcall TdmMain::ReadBlobIntoField(TField* blobField, TFloatField** calc_field)
{
    TStream *blobStream = blobField->DataSet->CreateBlobStream(blobField, bmRead);
    int stream_len = blobStream->Size / sizeof(double);
    double* dTemp = new double[stream_len];
    blobStream->ReadBuffer(dTemp, blobStream->Size);
    double* pos = dTemp;
    for (int i = 0 ; i < 36; i++)
       if ((stream_len--)>0) calc_field[i]->AsFloat = (*(pos++));
       else calc_field[i]->AsFloat = 0;
    delete[] dTemp;
}
//---------------------------------------------------------------------------

void __fastcall TdmMain::Create36FloatField(TFloatField** field, AnsiString Name, TDataSet *DataSet)
{
    for (int i=0; i < 36; i++) {
        field[i] = new TFloatField(DataSet);
        field[i]->FieldName = Name+AnsiString(i);
        field[i]->Index = DataSet->FieldCount;
        field[i]->Name = DataSet->Name + "_"+ field[i]->FieldName;
        field[i]->FieldKind = fkCalculated;
        field[i]->DataSet = DataSet;
        field[i]->DisplayFormat = "0.##";
        DataSet->FieldDefs->Update();
    }
}
//---------------------------------------------------------------------------

double __fastcall TdmMain::EmissionClassToBand(String emClass)
{
    emClass.Delete(4 + 1, emClass.Length() - 4);
    char factor = '\0';
    for (int i = 1; i <= emClass.Length(); i++)
        if (!isdigit(emClass[i]))
        {
            factor = emClass[i];
            emClass[i] = DecimalSeparator;
        }
    double band = 0.0;
    try {
        band = emClass.ToDouble();
    } catch (...) {
        return 0.0;
    }
    double mult = 0.0;
    switch (factor) {
        case 'H': case 'h': case 'Н': mult = 0.000001; break;
        case 'K': case 'k': case 'К': mult = 0.001; break;
        case 'M': case 'm': case 'М': mult = 1.0; break;
        case 'G': case 'g': mult = 1000.0; break;
        default : return 0.0;
    }
    return band * mult;
}
//---------------------------------------------------------------------------

void __fastcall TdmMain::InitDbControls()
{
    static bool init = true;
    if (init)
    {
        // this operation requires DB connected and transaction started. somehow.
        Create36FloatField(eff_pow_hor, "EFFPOWHOR", ibqTxOuery);
        Create36FloatField(eff_pow_vert, "EFFPOWVERT", ibqTxOuery);
        Create36FloatField(eff_height, "EFFHEIGHT", ibqTxOuery);
        Create36FloatField(eff_ant_gains, "EFFANTGAINS", ibqTxOuery);
        init = false;
    }
}

void __fastcall TdmMain::SetSelectionIcst(int selId, bool setIcst)
{
    String s1("update SELECTIONS set SELTYPE = 'E' where USERID = "); s1 += IntToStr(UserId);
    String s2("update SELECTIONS set SELTYPE = :SELTYPE where ID = :ID");
    ParamList pl; pl["ID"] = selId;
    if (setIcst)
    {
        RunQuery(s1);
        pl["SELTYPE"] = "I";
        RunQuery(s2, pl);
    } else {
        pl["SELTYPE"] = "E";
        RunQuery(s2, pl);
    }
    dbMain->DefaultTransaction->CommitRetaining();
}
