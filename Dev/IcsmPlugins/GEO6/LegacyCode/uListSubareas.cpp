//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListSubareas.h"
#include "uMainDm.h"
#include "uDlgList.h"
#include "uContourForm.h"
#include "uItuExport.h"
#include "tempcursor.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListDigSubareas *frmListDigSubareas;
//---------------------------------------------------------------------------
__fastcall TfrmListDigSubareas::TfrmListDigSubareas(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}

__fastcall TfrmListDigSubareas::TfrmListDigSubareas(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseListTree(Owner, callerWin, elementId)
{
    mapForm = new TForm(panMap);
    mapForm->Parent = panMap;
    mapForm->ParentWindow = panMap;
    mapForm->Align = alClient;
    mapForm->Visible = true;
    mapForm->BorderStyle = bsNone;
    mapForm->OnPaint = FormPaint;

    cd.SetWc(mapForm);
    cd.contours = &contours;

    oldIndex = -1;

    try {
        LoadChannels();
    } catch (Exception &e) { Application->ShowException(&e); };

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION ";
    dbSectIds.clear();
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        dbSectIds.push_back(sql->Fields[0]->AsInteger);
        cbxDbSection->Items->Add(sql->Fields[1]->AsString);
    }

    for (int i = 0; i < dbSectIds.size(); i++)
        if (dbSectIds[i] == 1)
            cbxDbSection->ItemIndex = i;

    if (cbxDbSection->ItemIndex > -1)
    {
        dstList->Close();
        //dstList->Transaction->CommitRetaining();
        dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];
        dstList->Open();
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDigSubareas::EditElement()
{
    std::auto_ptr<TfrmContour> frm(new TfrmContour(Application));
    frm->id = dstListID->AsInteger;
    frm->ShowModal();
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDigSubareas::dstListCONTOUR_IDGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (!Sender->IsNull)
        Text = AnsiString().sprintf("%04d", Sender->AsInteger);
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDigSubareas::cbxDbSectionChange(TObject *Sender)
{
    dstList->Close();
    if (cbxDbSection->ItemIndex > -1)
    {
        dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];
        dstList->Open();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::actMoveToSectionExecute(
      TObject *Sender)
{
    std::auto_ptr<TdlgList> dlg(new TdlgList(this));
    dlg->lb->Items->Clear();
    dlg->ids.clear();
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION";
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        dlg->lb->Items->Add(sql->Fields[1]->AsString);
        dlg->ids.push_back(sql->Fields[0]->AsInteger);
    }
    sql->Close();

    dlg->Caption = "Секция БД";
    dlg->lblComment->Caption = "Выберите секцию БД, в которую нужно переместить контуры";
    if (dlg->ShowModal() == mrOk && dlg->lb->ItemIndex > -1)
    {
        TBookmark bm = dstList->GetBookmark();
        TBookmarkList *bml = dgrList->SelectedRows;
        AnsiString idList;
        for (int i = 0; i < bml->Count; i++)
        {
            dstList->GotoBookmark((void *)bml->Items[i].c_str());
            if (idList.Length() > 0)
                idList = idList + ',';
            idList = idList + dstList->FieldByName("ID")->AsString;
        }
        if (idList.Length() > 0)
        {
            if (idList[idList.Length()] == ',')
                idList.SetLength(idList.Length() - 1);
            sql->SQL->Text = AnsiString("update ") +  "DIG_CONTOUR" + " set DB_SECTION_ID = "
                            + IntToStr(dlg->ids[dlg->lb->ItemIndex]) + " where ID in ("
                            + idList + ')';
            sql->ExecQuery();
            sql->Transaction->CommitRetaining();
        }
        dstList->FreeBookmark(bm);
        dstList->Close(); dstList->Open();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::actCopyToSectionExecute(
      TObject *Sender)
{
    std::auto_ptr<TdlgList> dlg(new TdlgList(this));
    dlg->lb->Items->Clear();
    dlg->ids.clear();
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION";
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        dlg->lb->Items->Add(sql->Fields[1]->AsString);
        dlg->ids.push_back(sql->Fields[0]->AsInteger);
    }

    dlg->Caption = "Секция БД";
    dlg->lblComment->Caption = "Выберите секцию БД, в которую нужно скопировать контуры";
    if (dlg->ShowModal() == mrOk && dlg->lb->ItemIndex > -1)
    {
        //TBookmark bm =
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::ReloadContours()
{
    contours.clear();
    idx.clear();
    oldIndex = -1;

    sqlContours->Close();
    sqlContours->SQL->Strings[sqlContours->SQL->Count - 2] = dstList->SelectSQL->Strings[dstList->SelectSQL->Count - 2];
    sqlContours->ParamByName("GRP_ID")->AsInteger = dstList->ParamByName("GRP_ID")->AsInteger;
    sqlContours->ParamByName("DB_SECTION_ID")->AsInteger = dstList->ParamByName("DB_SECTION_ID")->AsInteger;

    int currentCont = 0;
    AnsiString curName;
    DrawContourData curSubarea;
    for (sqlContours->ExecQuery(); !sqlContours->Eof; sqlContours->Next())
    {
        int auxNo = sqlContours->FieldByName("CONTOUR_ID")->AsInteger;
        if (currentCont != auxNo)
        {
            if (!curSubarea.contour.empty())
            {
                curSubarea.id = currentCont;
                curSubarea.name = curName;
                contours.push_back(curSubarea);
                idx[curSubarea.id] = contours.size() - 1;
                curSubarea.contour.clear();
            }
            currentCont = auxNo;
            curName = sqlContours->FieldByName("NAME")->AsString;
        }

        BcCoord coord; coord.lon = sqlContours->FieldByName("LON")->AsDouble; coord.lat = sqlContours->FieldByName("LAT")->AsDouble;
        curSubarea.contour.push_back(coord);
    }
    if (!curSubarea.contour.empty())
    {
        curSubarea.id = currentCont;
        curSubarea.name = curName;
        contours.push_back(curSubarea);
        curSubarea.contour.clear();
        idx.insert(std::pair<int,int>(curSubarea.id, contours.size() - 1));
        idx[curSubarea.id] = contours.size() - 1;
    }
}

void __fastcall TfrmListDigSubareas::FormPaint(TObject *Sender)
{
    cd.AjustScale();
    cd.DrawContours(dstListID->AsInteger);
    oldIndex = idx[dstListID->AsInteger];
}

void __fastcall TfrmListDigSubareas::dstListAfterOpen(TDataSet *DataSet)
{
    if (isInitialized)
    {
        ReloadContours();
        cd.FitContours();
        if (mapForm)
            mapForm->Invalidate();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::dstListAfterDelete(TDataSet *DataSet)
{
    TfrmBaseListTree::dstListAfterDelete(DataSet);
    ReloadContours();
    if (mapForm)
        mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::dstListAfterScroll(TDataSet *DataSet)
{
    TfrmBaseListTree::dstListAfterScroll(DataSet);
    if (mapForm) {
        if (oldIndex > -1 && oldIndex < contours.size())
            cd.DrawContour(contours[oldIndex], clBlue, clDkGray);
        int cntrId = dstListID->AsInteger;
        if (!dstList->Eof && contours.size() > 0 && cntrId < idx.size())
        {
            int index = idx[cntrId];
            if (index < contours.size())
                cd.DrawContour(contours[index], clLime, clWindowText);
            oldIndex = index;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::panMapResize(TObject *Sender)
{
    cd.AjustScale();
    if (mapForm)
        mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::actListInsertExecute(TObject *Sender)
{
    std::auto_ptr<TIBSQL> sql (new TIBSQL(this));
    sql->Database = dmMain->dbMain;
    sql->SQL->Text = " select max(CONTOUR_ID) from DIG_CONTOUR dc "
                    "where dc.ADM_ID = :GRP_ID and DB_SECTION_ID = :DB_SECTION_ID ";
    sql->ParamByName("GRP_ID")->AsInteger = dstList->ParamByName("GRP_ID")->AsInteger;
    sql->ParamByName("DB_SECTION_ID")->AsInteger = dstList->ParamByName("DB_SECTION_ID")->AsInteger;
    sql->ExecQuery();

    AnsiString ctry = dstList->FieldByName("CTRY")->AsString;
    dstList->Insert();
    dstList->FieldByName("ID")->AsInteger = dmMain->getNewId();
    dstList->FieldByName("ADM_ID")->AsInteger = dstList->ParamByName("GRP_ID")->AsInteger;
    dstList->FieldByName("DB_SECTION_ID")->AsInteger = dstList->ParamByName("DB_SECTION_ID")->AsInteger;
    dstList->FieldByName("CONTOUR_ID")->AsInteger = sql->Fields[0]->AsInteger + 1;
    dstList->FieldByName("NB_TEST_PTS")->AsInteger = 0;
    dstList->FieldByName("CTRY")->AsString = ctry;
    dstList->Post();
    
    actListEditExecute(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::actListDeleteExecute(TObject *Sender)
{
    if (dstListDB_SECTION_ID->AsInteger == tsDraft)
    {
        //if (MessageBox(NULL, "Удалить контур?", "Подтверждение", MB_ICONQUESTION | MB_YESNO) == IDYES)
            dstList->Delete();
    } else
        throw *(new Exception("Удалить контур можно только в Предбазе"));
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::tbtZoomInClick(TObject *Sender)
{
    cd.SetScaleX(cd.GetScaleX() / 1.5);
    cd.SetScaleY(cd.GetScaleY() / 1.5);
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::tbtZoomOutClick(TObject *Sender)
{
    cd.SetScaleX(cd.GetScaleX() * 1.5);
    cd.SetScaleY(cd.GetScaleY() * 1.5);
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::tbtLeftClick(TObject *Sender)
{
    double shift = cd.GetLon(mapForm->ClientWidth / 3) - cd.GetLon(0);
    cd.maxlon += shift;
    cd.minlon += shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::tbtRightClick(TObject *Sender)
{
    double shift = cd.GetLon(mapForm->ClientWidth / 3) - cd.GetLon(0);
    cd.maxlon -= shift;
    cd.minlon -= shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::tbtDownClick(TObject *Sender)
{
    double shift = cd.GetLat(0) - cd.GetLat(mapForm->ClientHeight / 3);
    cd.maxlat += shift;
    cd.minlat += shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::tbtUpClick(TObject *Sender)
{
    double shift = cd.GetLat(0) - cd.GetLat(mapForm->ClientHeight / 3);
    cd.maxlat -= shift;
    cd.minlat -= shift;
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::tbtFitClick(TObject *Sender)
{
    cd.FitContours();
    mapForm->Invalidate();
}
//---------------------------------------------------------------------------


void __fastcall TfrmListDigSubareas::LoadChannels()
{
    cbChannel->Items->Clear();
    channels.clear();

    cbChannel->Items->AddObject("<все>", (TObject*)0);
    channels.push_back();
    cbChannel->ItemIndex = 0;

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dstList->Database;
    sql->SQL->Text = "select ch.ID, ch.NAMECHANNEL || ' (' || fg.NAME || ')' "
                      "from CHANNELS ch left outer join  FREQUENCYGRID fg on (ch.FREQUENCYGRID_ID = fg.ID) "
                      "where fg.NAME <> 'DAB' "
                      "order by 2";
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        cbChannel->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        channels.push_back(ChannelInfo(1, sql->Fields[0]->AsInteger));
    }
    sql->Close();

    sql->SQL->Text = "select b.ID, b.NAME from BLOCKDAB b order by 2";
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
    {
        cbChannel->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
        channels.push_back(ChannelInfo(2, sql->Fields[0]->AsInteger));
    }
    sql->Close();

    cbChannel->DropDownCount = Screen->Height / cbChannel->ItemHeight / 3; 

    cbChannelChange(this);
    
}

void __fastcall TfrmListDigSubareas::cbChannelChange(TObject *Sender)
{
    int oldGrp = dstList->ParamByName("GRP_ID")->AsInteger;
    int oldDbSct = dstList->ParamByName("DB_SECTION_ID")->AsInteger;

    AnsiString criteria;
    if (cbChannel->ItemIndex == 0)
        criteria = "and dc.ID = dc.ID";
    else if (channels[cbChannel->ItemIndex].type == 1)
        criteria = "and dc.ID in (select CNTR_ID from DIG_ALLOT_CNTR_LNK l, DIG_ALLOTMENT a "
                                "where l.ALLOT_ID = a.ID and a.CHANNEL_ID = "
                                +IntToStr(channels[cbChannel->ItemIndex].id)+")";
    else // type = 2
        criteria = "and dc.ID in (select CNTR_ID from DIG_ALLOT_CNTR_LNK l, DIG_ALLOTMENT a "
                                "where l.ALLOT_ID = a.ID and a.BLOCKDAB_ID = "
                                +IntToStr(channels[cbChannel->ItemIndex].id)+")";

    int idx = -1;
    for (int i = 0; i < dstList->SelectSQL->Count; i++)
        if (dstList->SelectSQL->Strings[i].Pos("order by") > 0)
        {
            idx = i;
            break;
        }

    if (idx > 0)
    {
        bool reopen = dstList->Active;
        dstList->Close();
        if (dstList->SelectSQL->Strings[idx-1].Pos("and dc.ID ") > 0)
            dstList->SelectSQL->Strings[idx-1] = criteria;
        else
            dstList->SelectSQL->Insert(idx, criteria);
        dstList->ParamByName("GRP_ID")->AsInteger = oldGrp;
        dstList->ParamByName("DB_SECTION_ID")->AsInteger = oldDbSct;
        if (reopen)
        {
            dstList->Open();
            mapForm->Invalidate();
        }
    }
}
//---------------------------------------------------------------------------


AnsiString __fastcall TfrmListDigSubareas::GetChannelList(int id)
{
    AnsiString retVal;
    ChannelMap::iterator cmi = channelMap.find(id);
    
    if (cmi == channelMap.end())
    {
        static std::auto_ptr<TIBSQL> sql;
        if (sql.get() == NULL)
        {
            sql.reset(new TIBSQL(NULL)); // !!!! dont pass pointer to any owner - variable is global
            sql->Database = dstList->Database;
            sql->SQL->Text = "select distinct ch.NAMECHANNEL, bl.NAME from DIG_ALLOT_CNTR_LNK l "
                            "left outer join DIG_ALLOTMENT a on (l.ALLOT_ID = a.ID)"
                            "left outer join CHANNELS ch on (a.CHANNEL_ID = ch.ID) "
                            "left outer join BLOCKDAB bl on (a.BLOCKDAB_ID = bl.ID) "
                            "where l.CNTR_ID = :CNTR_ID order by a.BLOCKDAB_ID, a.CHANNEL_ID";
            sql->Prepare();
        }
        sql->Close();
        sql->Params->Vars[0]->AsInteger = id;
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
        {
            if (!retVal.IsEmpty()) retVal += ", ";
            if (!sql->Fields[0]->IsNull)
                retVal += sql->Fields[0]->AsString;
            else if (!sql->Fields[1]->IsNull)
                retVal += sql->Fields[1]->AsString;
        }
        sql->Close();
        channelMap[id] = retVal;

    } else

        retVal = cmi->second;

    return retVal;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::dstListCalcFields(TDataSet *DataSet)
{
    //dstListChannelList->AsString = GetChannelList(dstListID->AsInteger);
}
//---------------------------------------------------------------------------


void __fastcall TfrmListDigSubareas::dstListChannelListGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    Text = GetChannelList(dstListID->AsInteger);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::FormShow(TObject *Sender)
{
    dgrList->Invalidate(); // почему-то сразу не отрисовывается поле ChannelList        
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigSubareas::actExportExecute(TObject *Sender)
{
    TfrmRrc06Export::ExportRrc006FromGrid(emGa1, dgrList);
}
//---------------------------------------------------------------------------

