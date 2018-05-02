// COLISBCDIGALLOTIMPL : Implementation of TCoLisBcDigAllotImpl (CoClass: CoLisBcDigAllot, Interface: ILisBcDigAllot)

#include <vcl.h>
#pragma hdrstop

#include "LisBcDigAllotImpl.h"
#include <memory>
#include <math>
#include "LISBC_TLB.h"
/////////////////////////////////////////////////////////////////////////////
// TCoLisBcDigAllotImpl
#define EnsureIfObjectIsInitialized() \
    if (id == 0)                     \
        return Error("Объект (выделение) не инициализирован", IID_ILISBCTx);

#define EnsureIfObjectIsLoaded()      \
    if (id == 0)                     \
        return Error("Объект (выделение) не инициализирован", IID_ILisBcDigAllot);      \
    if (!isLoaded && id > 0)                  \
    {                               \
        HRESULT hr = reload();      \
        if (!SUCCEEDED(hr))         \
            return hr;              \
    }

#define EnsureIfSubzonesAreLoaded()      \
    if (!isSubAreasLoaded)               \
    {                                    \
        HRESULT hr = loadSubzones();     \
        if (!SUCCEEDED(hr))              \
            return hr;                   \
    }

inline int IsPointInsidePolygon(std::vector<BcCoord> *vi, int x, int y)
{
    std::vector<BcCoord>& p = *vi;
    int i1, i2, n, N, S, S1, S2, S3, flag;
    int Number = p.size();
    N = Number;
    for (n=0; n<N; n++)
    {
        flag = 0;
        i1 = n < N-1 ? n + 1 : 0;
        while (flag == 0)
        {
            i2 = i1 + 1;
            if (i2 >= N)
                i2 = 0;
            if (i2 == (n < N-1 ? n + 1 : 0))
                break;
            S = abs (p[i1].lon * (p[i2].lat - p[n ].lat) +
                     p[i2].lon * (p[n ].lat - p[i1].lat) +
                     p[n].lon  * (p[i1].lat - p[i2].lat));
            S1 = abs (p[i1].lon * (p[i2].lat - y) +
                      p[i2].lon * (y       - p[i1].lat) +
                      x       * (p[i1].lat - p[i2].lat));
            S2 = abs (p[n ].lon * (p[i2].lat - y) +
                      p[i2].lon * (y       - p[n ].lat) +
                      x       * (p[n ].lat - p[i2].lat));
            S3 = abs (p[i1].lon * (p[n ].lat - y) +
                      p[n ].lon * (y       - p[i1].lat) +
                      x       * (p[i1].lat - p[n ].lat));
            if (S == S1 + S2 + S3)
            {
                flag = 1;
                break;
            }
            i1 = i1 + 1;
            if (i1 >= N)
                i1 = 0;
        }
        if (flag == 0)
            break;
    }
    return flag;
}

void TLisBcDigAllotImpl::recalcCoord()
{
    double totLon = 0.0; double totLat = 0.0; double totMass;

    for (SubAreas::iterator szsi = subAreas.begin(); szsi != subAreas.end(); szsi++)
    {
        if (szsi->second.size() > 2)
        {
            //totalPoints += szsi->second.size();
            SubArea::iterator szi1 = szsi->second.begin();
            SubArea::iterator szi2 = szi1; szi2++;
            SubArea::iterator szi3 = szi2; szi3++;
            for (; szi3 < szsi->second.end(); szi2 = szi3, szi3++)
            {
                // center of gravity of triangle
                double xGrav = (szi1->lon + szi2->lon + szi3->lon) / 3;
                double yGrav = (szi1->lat + szi2->lat + szi3->lat) / 3;
                // check if COG belongs to poligon
                if (IsPointInsidePolygon(&szsi->second, xGrav, yGrav))
                {
                    double a = sqrt((szi1->lon - szi2->lon) * (szi1->lon - szi2->lon) + (szi1->lat - szi2->lat) * (szi1->lat - szi2->lat));
                    double b = sqrt((szi3->lon - szi2->lon) * (szi3->lon - szi2->lon) + (szi3->lat - szi2->lat) * (szi3->lat - szi2->lat));
                    double c = sqrt((szi1->lon - szi3->lon) * (szi1->lon - szi3->lon) + (szi1->lat - szi3->lat) * (szi1->lat - szi3->lat));
                    double p = (a + b + c) / 2;
                    double mass = sqrt(p*(p-a)*(p-b)*(p-c));

                    totLon += (xGrav * mass);
                    totLat += (yGrav * mass);
                    totMass += mass;

                } else {
                    // pass this triangle and move szi1 to next point
                    szi1++;
                }

            }

        } else {
            // хуйня, а не полигон. можно выкинуть.
        }
    }

    cenGravLat = (totMass == 0) ? 0 : totLat / totMass;
    cenGravLon = (totMass == 0) ? 0 : totLon / totMass;
}

STDMETHODIMP TLisBcDigAllotImpl::get_id(long* Value)
{
    *Value = id;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_pointsDs(long* Value)
{
    EnsureIfObjectIsLoaded();
    /* TODO: get_pointsDs */
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_subareasDs(long* Value)
{
    EnsureIfObjectIsLoaded();
    /* TODO: get_subareasDs */
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_station_name(BSTR* Value)
{
    return get_allot_name(Value);
}

STDMETHODIMP TLisBcDigAllotImpl::get_systemcast(TBCTxType* Value)
{
    *Value = ttAllot;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_data_changes(long* Value)
{
    EnsureIfObjectIsInitialized();
    *Value = isChanged;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_database(long* Value)
{
    EnsureIfObjectIsInitialized();
    //*Value = (long)db;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::init(long pdatabase, long load_id)
{
    try {
        if (load_id < 1)
            throw *(new Exception(AnsiString().sprintf("Недопустимый ID (%d) при инициализации выделения", load_id)));

        if (id > 0)
            return Error("Об'єкт проініціалізований", IID_ILISBCTx);

        if (pdatabase != NULL)
        {
            OleCheck(((IUnknown*)pdatabase)->QueryInterface(IID_ILisBcStorage, (void**)&storage));
            if (storage == NULL)
                return Error("Can't get Storage interface", IID_ILISBCTx);
        }

        id = load_id;

        if (load_id < 0)
            isLoaded = true;

        isChanged = false;

    }
    catch(Exception &e)
    {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::invalidate()
{
    isLoaded = false;
    subAreas.clear();
    subAreaIds.clear();
    subAreaTags.clear();
    isSubAreasLoaded = false;

    isChanged = false;
    
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::loadFromQuery(long query)
{
    EnsureIfObjectIsInitialized();
    try {

        static const String className("TIBSQL");
        if (className != ((TObject*)query)->ClassName())
        {
            /* TODO: log error*/
            throw *(new Exception("Сбой при вызове loadFromQuery(): это не TIBSQL"));
        }

        TIBSQL *sql = (TIBSQL*)query;
        //TIBXSQLVAR* fld = sql->FieldByName("ID");
        //if (fld->AsInteger != id)

        TIBXSQLDA *rec = sql->Current();
        //String fldNames(sql->Current()->Names.c_str());   // чото CodeGuard на деструктор ругается
        bool isDetails = false;//(fldNames.Pos("POINT_NO") > 0);
        for (int i = 0; i < rec->Count; i++ )
            if (strcmp("POINT_NO", rec->Vars[i]->AsXSQLVAR->sqlname) == 0)
                isDetails = true;

        db = sql->Database;

        if (!isDetails)
        {

            dab_dvb             = sql->FieldByName("NOTICE_TYPE")->AsString;
            adm_id              = sql->FieldByName("ADM_ID")->AsInteger;

            plan_entry          = sql->FieldByName("PLAN_ENTRY")->AsInteger;

            country             = sql->FieldByName("CTRY")->AsString;
            name                = sql->FieldByName("ALLOT_NAME")->AsString;
            uniq_country        = sql->FieldByName("GEO_AREA")->AsString;
            rpc                 = sql->FieldByName("REF_PLAN_CFG")->AsString;
            rn                  = sql->FieldByName("TYP_REF_NETWK")->AsString;
            freq                = sql->FieldByName("FREQ_ASSIGN")->AsDouble;
            if (dab_dvb == WideString(L"GS2") || dab_dvb == WideString(L"DS2"))
                channel_id      = sql->FieldByName("BLOCKDAB_ID")->AsInteger;
            else
                channel_id      = sql->FieldByName("CHANNEL_ID")->AsInteger;
            freqOffset          = sql->FieldByName("OFFSET")->AsDouble;

            AnsiString
            dummy               = sql->FieldByName("POLAR")->AsString;
            pol                 = dummy.IsEmpty() ? 0 : dummy[1];

            dummy               = sql->FieldByName("SPECT_MASK")->AsString;
            sm                  = dummy.IsEmpty() ? 0 : dummy[1];

            sfn_id              = sql->FieldByName("SFN_ID_FK")->AsInteger;
            remarks1            = sql->FieldByName("REMARKS1")->AsString;
            remarks2            = sql->FieldByName("REMARKS2")->AsString;
            remarks3            = sql->FieldByName("REMARKS3")->AsString;

            adm_ref_id          = sql->FieldByName("ADM_REF_ID")->AsString;

            //lon                 = sql->FieldByName("LONGITUDE")->AsDouble;
            //lat                 = sql->FieldByName("LATITUDE")->AsDouble;

            coord               = sql->FieldByName("COORD")->AsString;

            isLoaded = true;
            isChanged = false;

            subAreas.clear();
            subAreaIds.clear();
            subAreaTags.clear();
            isSubAreasLoaded = false;

        } else {
            //load details - subzones
            int currentCont = 0;
            int currentTag = 0;
            SubArea curSubarea;
            
            TIBXSQLVAR *allotIdFld = sql->FieldByName("ALLOT_ID");
            TIBXSQLVAR *lonFld = sql->FieldByName("LON");
            TIBXSQLVAR *latFld = sql->FieldByName("LAT");
            TIBXSQLVAR *contourIdFld = sql->FieldByName("CONTOUR_ID");
            TIBXSQLVAR *tagFld = sql->FieldByName("TAG");

            subAreas.clear();
            subAreaIds.clear();
            subAreaTags.clear();

            for (; !sql->Eof && allotIdFld->AsInteger == id; sql->Next())
            {
                int auxNo = contourIdFld->AsInteger;
                int auxTag = tagFld->AsInteger;
                if (currentCont != auxNo)
                {
                    if (!curSubarea.empty())
                    {
                        subAreas.insert(std::pair<int, SubArea>(currentCont, curSubarea));
                        subAreaIds.push_back(currentCont);
                        subAreaTags.push_back(currentTag);
                        curSubarea.clear();
                    }
                    currentCont = auxNo;
                    currentTag = auxTag;
                }

                BcCoord coord;
                coord.lon = lonFld->AsDouble; coord.lat = latFld->AsDouble;
                curSubarea.push_back(coord);
            }

            if (!curSubarea.empty())
            {
                subAreas.insert(std::pair<int, SubArea>(currentCont, curSubarea));
                subAreaIds.push_back(currentCont);
                subAreaTags.push_back(currentTag);
                curSubarea.clear();
            }

            isSubAreasLoaded = true;

            recalcCoord();
        }
    }
    catch(Exception &e)
    {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::reload()
{
    EnsureIfObjectIsInitialized();

    if (id <= 0)
        return S_FALSE;

    if (storage == NULL)
        return Error("Storage interface is NULL", IID_ILISBCTx);

    try
    {
        HRESULT hr = storage->LoadObject((IUnknown*)(ILISBCTx*)this);
        if (!SUCCEEDED(hr))
            return hr;
    }
    catch(Exception &e)
    {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;
}

HRESULT TLisBcDigAllotImpl::loadSubzones()
{
    if (id <= 0)
        return S_FALSE;

    if (storage == NULL)
        return Error("Storage interface is NULL", IID_ILISBCTx);

    try
    {
        HRESULT hr = storage->LoadDetails((IUnknown*)(ILISBCTx*)this);
        if (!SUCCEEDED(hr))
            return hr;
    }
    catch(Exception &e)
    {
        return Error(e.Message.c_str(), IID_ILISBCTx);
    }
    return S_OK;

}

STDMETHODIMP TLisBcDigAllotImpl::save()
{
  EnsureIfObjectIsInitialized();
  try
  {
    //if (storage == NULL)
    //    return Error("Storage interface is NULL", IID_ILISBCTx);

    //storage->SaveObject((IUnknown*)(ILISBCTx*)this);

    if (db == NULL)
        return Error("save(): database is not assigned", IID_ILISBCTx);

    std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
    sql->Database = db;
    sql->SQL->Text = "UPDATE DIG_ALLOTMENT SET "
                    "IS_PUB_REQ = :IS_PUB_REQ "
                    ",ADM_REF_ID = :ADM_REF_ID "
                    ",PLAN_ENTRY = :PLAN_ENTRY "
                    ",CTRY = :CTRY "
                    ",D_EXPIRY = :D_EXPIRY "
                    ",ALLOT_NAME = :ALLOT_NAME "
                    ",GEO_AREA = :GEO_AREA "
                    ",NB_SUB_AREAS = :NB_SUB_AREAS "
                    ",REF_PLAN_CFG = :REF_PLAN_CFG "
                    ",TYP_REF_NETWK = :TYP_REF_NETWK "
                    ",FREQ_ASSIGN = :FREQ_ASSIGN "
                    ",OFFSET = :OFFSET "
                    ",SFN_ID_FK = :SFN_ID_FK "
                    ",SPECT_MASK = :SPECT_MASK "
                    ",POLAR = :POLAR "
                    ",BLOCKDAB_ID = :BLOCKDAB_ID "
                    ",CHANNEL_ID = :CHANNEL_ID "
                    ",REMARKS1 = :REMARKS1 "
                    ",REMARKS2 = :REMARKS2 "
                    ",REMARKS3 = :REMARKS3 "
                    "WHERE ID = " + IntToStr(id);



    sql->ParamByName("IS_PUB_REQ")->Clear();
    sql->ParamByName("ADM_REF_ID")->AsString    = adm_ref_id          ;
    sql->ParamByName("PLAN_ENTRY")->AsInteger   = plan_entry          ;
    sql->ParamByName("CTRY")->AsString          = country             ;
    sql->ParamByName("ALLOT_NAME")->AsString    = name                ;
    sql->ParamByName("GEO_AREA")->AsString      = uniq_country        ;
    sql->ParamByName("REF_PLAN_CFG")->AsString  = rpc                 ;
    sql->ParamByName("TYP_REF_NETWK")->AsString = rn                  ;
    sql->ParamByName("FREQ_ASSIGN")->AsDouble   = freq                ;
    sql->ParamByName("OFFSET")->AsInteger       = freqOffset          ;
    if (channel_id == 0)
    {
        sql->ParamByName("CHANNEL_ID")->IsNull = true                   ;
        sql->ParamByName("BLOCKDAB_ID")->IsNull = true                  ;
    } else
        if (dab_dvb == WideString(L"GS2") || dab_dvb == WideString(L"DS2"))
        {
            sql->ParamByName("CHANNEL_ID")->IsNull = true               ;
            sql->ParamByName("BLOCKDAB_ID")->AsInteger= channel_id      ;
        } else {
            sql->ParamByName("CHANNEL_ID")->AsInteger = channel_id      ;
            sql->ParamByName("BLOCKDAB_ID")->IsNull = true              ;
        }
    sql->ParamByName("POLAR")->AsString           = AnsiString((const char*)&pol, 1) ;
    sql->ParamByName("SPECT_MASK")->AsString   = AnsiString((const char*)&sm, 1)  ;
    if (sfn_id == 0)
        sql->ParamByName("SFN_ID_FK")->Clear()                          ;
    else
        sql->ParamByName("SFN_ID_FK")->AsInteger     = sfn_id           ;
    sql->ParamByName("REMARKS1")->AsString        = remarks1            ;
    sql->ParamByName("REMARKS2")->AsString        = remarks2            ;
    sql->ParamByName("REMARKS3")->AsString        = remarks3            ;

    sql->ParamByName("NB_SUB_AREAS")->AsInteger   = subAreas.size()     ;

    sql->ExecQuery();


    sql->SQL->Text = "delete from DIG_ALLOT_CNTR_LNK where ALLOT_ID = " + IntToStr(id);
    sql->ExecQuery();

    if (uniq_country.Length() == 0 && !subAreas.empty())
    {
        sql->SQL->Text = "insert into DIG_ALLOT_CNTR_LNK (ALLOT_ID, CNTR_ID) "
                         "values (" + IntToStr(id) + ", :CNTR_ID)";
        sql->Prepare();
        for (SubAreas::iterator szsi = subAreas.begin(); szsi != subAreas.end(); szsi++)
        {
            sql->ParamByName("CNTR_ID")->AsInteger = szsi->first;
            sql->ExecQuery();
        }
    }

    // update central point
    sql->Close();
    sql->SQL->Text = "update transmitters set "
    //"LONGITUDE = :LON, LATITUDE = :LAT, "
    "COORD = :COORD where ID = :ID";
    sql->ParamByName("ID")->AsInteger = id;
    //sql->ParamByName("LAT")->AsDouble = lat;
    //sql->ParamByName("LON")->AsDouble = lon;
    sql->ParamByName("COORD")->AsString = AnsiString(coord);
    sql->ExecQuery();
    sql->Close();

    sql->Transaction->CommitRetaining();
  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  isChanged = false;
  return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::loadFromString(BSTR Source)
{
    return S_FALSE;
}

STDMETHODIMP TLisBcDigAllotImpl::saveToString(BSTR* Dest)
{
    return S_FALSE;
}

STDMETHODIMP TLisBcDigAllotImpl::get_identifiersfn(long* Value)
{
    return get_sfn_id(Value);
}

STDMETHODIMP TLisBcDigAllotImpl::get_is_fetched(VARIANT_BOOL* Value)
{
    EnsureIfObjectIsInitialized();
    *Value = isLoaded;
    return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_rpc(TBcRpc* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = (TBcRpc)-1;
    try { ((TBcRpc)*Value) = AnsiString(rpc)[3] - '0' - 1; } catch (...) {}
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_ref_plan_cfg(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString ws(rpc);
    *Value = ws.Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_rxMode(TBcRxMode* Value)
{
    EnsureIfObjectIsLoaded();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_channel_id(long Value)
{
    EnsureIfObjectIsLoaded();
    if (channel_id != Value)
    {
        channel_id = Value;
        isChanged = true;
    }
    return S_OK;
}
 
STDMETHODIMP TLisBcDigAllotImpl::set_rpc(TBcRpc Value)
{
    EnsureIfObjectIsLoaded();
    WideString newRpc(L"RPC");
    newRpc += IntToStr(Value + 1);
    if (rpc != newRpc)
    {
        rpc = newRpc;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_ref_plan_cfg(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    WideString newRpc(Value);
    if (rpc != newRpc)
    {
        rpc = newRpc;

        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_rxMode(TBcRxMode Value)
{
    EnsureIfObjectIsLoaded();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::AddPoint(long subzone, long num,
  BcCoord point)
{
    /*
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    if (subzone < 0 || subzone >= subAreas.size())
        return Error(AnsiString().sprintf("Индекс контура (%d) вне допустимых границ", subzone).c_str(),
                    IID_ILisBcDigAllot);
    if (num < -1 || num > subAreas[subzone].size())
        return Error(AnsiString().sprintf("Индекс точки (%d) вне допустимых границ", num).c_str(),
                    IID_ILisBcDigAllot);

    if (num == subAreas[subzone].size() || num == -1)
        subAreas[subzone].push_back(point);
    else
        subAreas[subzone].insert(&subAreas[subzone].at(num), point);

    isChanged = true;

    recalcCoord();
    */
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::AddSubarea(long id)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();

    SubAreas::iterator i = subAreas.find(id);
    if (i == subAreas.end())
    {
        SubArea area;
        try {
            /*
            std::auto_ptr<TIBSQL> sql(new TIBSQL(NULL));
            sql->Database = db;
            sql->SQL->Text = "select LON, LAT, c.CONTOUR_ID from DIG_SUBAREAPOINT p "
            "left outer join DIG_CONTOUR c on (c.ID = p.CONTOUR_ID) "
            "where ID = "+IntToStr(id)+" order by POINT_NO";
            for (sql->ExecQuery(); !sql->Eof; sql->Next())
            {
                BcCoord c; c.lon = sql->Fields[0]->AsDouble; c.lat = sql->Fields[1]->AsDouble;
                area.push_back(c);
            }
            */

            subAreas.insert(std::pair<int, SubArea>(id, area));
            subAreaIds.push_back(id);
            subAreaTags.push_back(0);//sql->Fields[2]->AsInteger);

        } catch (Exception &e)
        {
            return Error(AnsiString().sprintf("Неудалось добавить контур: %s", e.Message.c_str()).c_str(),
                    IID_ILisBcDigAllot);
        }

        isChanged = true;
        recalcCoord();
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::ClearPoints(long subzone)
{
    /*
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    if (subzone < 0 || subzone >= subAreas.size())
        return Error(AnsiString().sprintf("Индекс контура (%d) вне допустимых границ", subzone).c_str(),
                    IID_ILisBcDigAllot);
    subAreas[subzone].clear();

    isSubzonesChanged = true;
    isChanged = true;

    recalcCoord();
    */
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::ClearSubarea()
{
    /*
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    if (!subAreas.empty())
    {
        subAreas.clear();
        isSubzonesChanged = true;
        isChanged = true;
    }

    lat = 0.0; lon = 0.0;
    */
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::DelPoint(long subzone, long num)
{
    /*
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    if (subzone < 0 || subzone >= subAreas.size())
        return Error(AnsiString().sprintf("Индекс контура (%d) вне допустимых границ", subzone).c_str(),
                    IID_ILisBcDigAllot);
    if (num < 0 || num >= subAreas.at(subzone).size())
        return Error(AnsiString().sprintf("Индекс точки (%d) вне допустимых границ", num).c_str(),
                    IID_ILisBcDigAllot);

    subAreas.at(subzone).erase(&subAreas.at(subzone).at(num));

    isSubzonesChanged = true;
    isChanged = true;

    recalcCoord();
    */
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::DelSubarea(long id)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();

    SubAreas::iterator i = subAreas.find(id);
    if (i != subAreas.end())
    {
        subAreas.erase(i);
        std::vector<int>::iterator vi1;
        std::vector<int>::iterator vi2;
        for (vi1 = subAreaIds.begin(), vi2 = subAreaTags.begin(); vi1 < subAreaIds.end(); vi1++, vi2++)
        {
            if (*vi1 == id)
            {
                subAreaIds.erase(vi1);
                subAreaTags.erase(vi2);
                break;
            }
        }
        isChanged = true;
        recalcCoord();
    }                 

    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_nb_sub_areas(long* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    *Value = subAreas.size();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_points_num(long subareaId,
  long* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();

    *Value = 0;

    SubAreas::iterator i = subAreas.find(subareaId);
    if (i == subAreas.end())
        return Error(AnsiString().sprintf("Такого контура нет: Id = %d", subareaId).c_str(), IID_ILisBcDigAllot);

    *Value = i->second.size();
    return S_OK;
}
 
STDMETHODIMP TLisBcDigAllotImpl::get_point(long subareaId, long num,
  BcCoord* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    Value->lon = 0.0;
    Value->lat = 0.0;

    SubAreas::iterator i = subAreas.find(subareaId);
    if (i == subAreas.end())
        return Error(AnsiString().sprintf("Такого контура нет: Id = %d", subareaId).c_str(), IID_ILisBcDigAllot);

    *Value = i->second[num];

    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_adm_id(long* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = adm_id;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_ctry(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString retval = country;
    *Value = retval.Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_notice_type(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = WideString(dab_dvb).Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_freq(double* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = freq;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_offset(long* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = freqOffset;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_allot_name(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString retval = name;
    *Value = retval.Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_polar(unsigned_char* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = pol;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_typ_ref_netwk(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = WideString(rn).Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_plan_entry(long* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = plan_entry;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_sfn_id(long* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = sfn_id;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_spect_mask(unsigned_char* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = sm;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_channel_id(long* Value)
{
    EnsureIfObjectIsLoaded();
    *Value = channel_id;
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_geo_area(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString retval = uniq_country;
    *Value = retval.Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_ctry(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    if (country != WideString(Value))
    {
        country = Value;
        if (country.Length() > 3)
            country.SetLength(3);
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_freq(double Value)
{
    EnsureIfObjectIsLoaded();
    if (freq != Value)
    {
        freq = Value;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_offset(long Value)
{
    EnsureIfObjectIsLoaded();
    if (freqOffset != Value)
    {
        freqOffset = Value;
        isChanged = true;
    }
    return S_OK;
}


STDMETHODIMP TLisBcDigAllotImpl::set_allot_name(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    if (name != WideString(Value))
    {
        name = Value;
        if (name.Length() > 32)
            name.SetLength(32);
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_polar(unsigned_char Value)
{
    EnsureIfObjectIsLoaded();
    if (pol != Value)
    {
        pol = Value;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_typ_ref_netwk(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    WideString newVal(Value);
    if (rn != newVal)
    {
        rn = newVal;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_plan_entry(long Value)
{
    EnsureIfObjectIsLoaded();
    if (plan_entry != Value)
    {
        plan_entry = Value;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_sfn_id(long Value)
{
    EnsureIfObjectIsLoaded();
    if (sfn_id != Value)
    {
        sfn_id = Value;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_spect_mask(unsigned_char Value)
{
    EnsureIfObjectIsLoaded();
    if (sm != Value)
    {
        sm = Value;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::set_geo_area(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    WideString newval(Value);
    if (uniq_country != newval)
    {
        uniq_country = newval;
        if (uniq_country.Length() > 3)
            uniq_country.SetLength(3);
        subAreas.clear();
        subAreaIds.clear();
        subAreaTags.clear();
        isSubAreasLoaded = false;
        isChanged = true;
    }
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_attribsDs(long* Value)
{
    EnsureIfObjectIsLoaded();
    /* TODO: get_attribsDs*/
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_remarks1(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString retval = remarks1;
    *Value = retval.Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_remarks2(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString retval = remarks2;
    *Value = retval.Detach();
    return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_remarks3(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString retval = remarks3;
    *Value = retval.Detach();
    return S_OK;
}

#define MAX_REMARKS_LEN 512
 
STDMETHODIMP TLisBcDigAllotImpl::set_remarks1(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    if (remarks1 != WideString(Value))
    {
        remarks1 = Value;
        if (remarks1.Length() > MAX_REMARKS_LEN)
            remarks1.SetLength(MAX_REMARKS_LEN);
        isChanged = true;
    }
    return S_OK;
}


STDMETHODIMP TLisBcDigAllotImpl::set_remarks2(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    if (remarks2 != WideString(Value))
    {
        remarks2 = Value;
        if (remarks2.Length() > MAX_REMARKS_LEN)
            remarks2.SetLength(MAX_REMARKS_LEN);
        isChanged = true;
    }
    return S_OK;
}


STDMETHODIMP TLisBcDigAllotImpl::set_remarks3(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    if (remarks3 != WideString(Value))
    {
        remarks3 = Value;
        if (remarks3.Length() > MAX_REMARKS_LEN)
            remarks3.SetLength(MAX_REMARKS_LEN);
        isChanged = true;
    }
    return S_OK;

    // return Error(e.Message.c_str(), IID_ILisBcDigAllot);
}

STDMETHODIMP TLisBcDigAllotImpl::get_longitude(double* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    *Value = cenGravLon;
    return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_longitude(double Value)
{
     return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_latitude(double* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    *Value = cenGravLat;
    return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_latitude(double Value)
{
  return S_OK;
}




STDMETHODIMP TLisBcDigAllotImpl::get_acin_id(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_acout_id(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_adm_response(BSTR* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_adm_sited_in(BSTR* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_adminid(long* Value)
{
    *Value = 0;
    return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_analogtelesystem(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_angleelevation_hor(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_angleelevation_vert(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_antennagain(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_blockcentrefreq(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_channel(BSTR* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_date_of_last_change(DATE* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_direction(TBCDirection* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_dvb_system(TBCDVBSystem* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_effectantennagains(long idx,
  double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_effectheight(long idx, double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_effectpowerhor(long idx,
  double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_effectpowervert(long idx,
  double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_sound_hor_primary(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_sound_hor_second(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_sound_max_primary(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_sound_max_second(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_sound_vert_primary(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_sound_vert_second(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_video_hor(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_video_max(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_epr_video_vert(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_erp(long azimuth, double* power)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_fiderlenght(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_fiderloss(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_fm_system(TBCFMSystem* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_freq_carrier(double* freq)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_gaussianchannel(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_h_eff(long azimuth, long* height)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_height_eft_max(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_heightantenna(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_maxCoordDist(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_monostereo_primary(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_numregion(BSTR* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_polarization(TBCPolarization* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_power_sound_primary(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_power_sound_second(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_power_video(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_rayleighchannel(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_relativetimingsfn(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_riceanchannel(double* Value)
{
  return S_OK;
}

STDMETHODIMP TLisBcDigAllotImpl::get_site_height(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_sort_key_in(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_sort_key_out(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_sound_carrier_primary(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_sound_carrier_second(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_sound_offset_primary(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_sound_offset_second(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_stand_id(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_status_code(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_systemcolor(TBCTvStandards* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_testpointsis(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_typeoffset(TBCOffsetType* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_typesystem(TBCTvSystems* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_v_sound_ratio_primary(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_v_sound_ratio_second(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_video_carrier(double* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_video_offset_herz(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::get_video_offset_line(long* Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_acin_id(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_acout_id(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_adminid(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_analogtelesystem(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_angleelevation_hor(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_angleelevation_vert(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_antennagain(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_blockcentrefreq(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_channel_name(BSTR Value)
{
    return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_database(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_direction(TBCDirection Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_dvb_system(TBCDVBSystem Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_effectantennagains(long idx,
  double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_effectheight(long idx, double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_effectpowerhor(long idx, double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_effectpowervert(long idx,
  double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_sound_hor_primary(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_sound_hor_second(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_sound_max_primary(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_sound_max_second(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_sound_vert_primary(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_sound_vert_second(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_video_hor(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_video_max(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_epr_video_vert(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_fiderlenght(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_fiderloss(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_fm_system(TBCFMSystem Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_gaussianchannel(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_height_eft_max(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_heightantenna(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_identifiersfn(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_maxCoordDist(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_monostereo_primary(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_polarization(TBCPolarization Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_power_sound_primary(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_power_sound_second(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_power_video(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_rayleighchannel(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_relativetimingsfn(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_riceanchannel(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_sort_key_in(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_sort_key_out(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_sound_carrier_primary(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_sound_carrier_second(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_sound_offset_primary(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_sound_offset_second(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_stand_id(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_station_name(BSTR Value)
{
    return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_systemcast(TBCTxType Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_systemcolor(TBCTvStandards Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_testpointsis(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_typeoffset(TBCOffsetType Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_typesystem(TBCTvSystems Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_v_sound_ratio_primary(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_v_sound_ratio_second(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_video_carrier(double Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_video_offset_herz(long Value)
{
  return S_OK;
}
STDMETHODIMP TLisBcDigAllotImpl::set_video_offset_line(long Value)
{
  return S_OK;
}


STDMETHODIMP TLisBcDigAllotImpl::get_adm_ref_id(BSTR* Value)
{
    EnsureIfObjectIsLoaded()
    *Value = WideString (adm_ref_id).Detach();
    return S_OK;
};

STDMETHODIMP TLisBcDigAllotImpl::set_adm_ref_id(BSTR Value)
{
    EnsureIfObjectIsLoaded()
    WideString newval(Value);
    if (adm_ref_id != newval)
    {
        adm_ref_id = newval;
        isChanged = true;
    }
    return S_OK;
};


STDMETHODIMP TLisBcDigAllotImpl::get_db_sect(long* Value)
{
  return S_OK;
};

STDMETHODIMP TLisBcDigAllotImpl::set_db_sect(long Value)
{
  return S_OK;
};


STDMETHODIMP TLisBcDigAllotImpl::get_SubareaCount(long* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    *Value = subAreas.size();
    return S_OK;
};

STDMETHODIMP TLisBcDigAllotImpl::get_subareaId(long idx, long* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    *Value = 0;
    if (idx >= 0 && idx < subAreas.size())
        *Value = subAreaIds[idx];
    else
        return Error("Недопустимый индекс", IID_ILisBcDigAllot);
    return S_OK;
};

STDMETHODIMP TLisBcDigAllotImpl::get_subareaTag(long idx, long* Value)
{
    EnsureIfObjectIsLoaded();
    EnsureIfSubzonesAreLoaded();
    *Value = 0;
    if (idx >= 0 && idx < subAreas.size())
        *Value = subAreaTags[idx];
    else
        return Error("Недопустимый индекс", IID_ILisBcDigAllot);
    return S_OK;
};


STDMETHODIMP TLisBcDigAllotImpl::get_coord(BSTR* Value)
{
    EnsureIfObjectIsLoaded();
    WideString retval(coord);
    *Value = retval.Detach();
    return S_OK;
};


STDMETHODIMP TLisBcDigAllotImpl::set_coord(BSTR Value)
{
    EnsureIfObjectIsLoaded();
    WideString newVal(Value);
    if (coord != newVal)
    {
        coord = newVal;
        isChanged = true;
    }
    return S_OK;
};



STDMETHODIMP TLisBcDigAllotImpl::get_pol_isol(double* Value)
{
  try
  {

  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};


STDMETHODIMP TLisBcDigAllotImpl::set_pol_isol(double Value)
{
  try
  {

  }
  catch(Exception &e)
  {
    return Error(e.Message.c_str(), IID_ILISBCTx);
  }
  return S_OK;
};



