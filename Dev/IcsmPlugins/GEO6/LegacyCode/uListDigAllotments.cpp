//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListDigAllotments.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include "TxBroker.h"
#include "uImportDigLayer.h"
#include <memory>
#include "uDlgList.h"
#include "uItuImport.h"
#include "uItuExport.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListDigAllotments *frmListDigAllotments;
//---------------------------------------------------------------------------
__fastcall TfrmListDigAllotments::TfrmListDigAllotments(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListDigAllotments::TfrmListDigAllotments(bool MDIChild)
    : TfrmBaseListTree(MDIChild)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmListDigAllotments::TfrmListDigAllotments(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseListTree(Owner, callerWin, elementId)
{
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
        dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];
        dstList->Open();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::mniNewAllotmentClick(TObject *Sender)
{
    if (dstList->ParamByName("GRP_ID")->AsInteger == 0)
        throw *(new Exception("Виберіть країну в дереві"));

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dstList->Database;
    sql->Transaction = dstList->Transaction;
    sql->SQL->Text = "select id from SYSTEMCAST where ENUMVAL = " + IntToStr(ttAllot);
    sql->ExecQuery();
    if (sql->Eof)
        throw *(new Exception("Не могу найти тип объекта ВЫДЕЛЕНИЕ ("+IntToStr(ttAllot)+") в таблице SYSTEMCAST"));
    AnsiString scId = sql->Fields[0]->AsString;
    sql->Close();

    dstList->Append();
    int newId = dmMain->getNewId();
    dstList->FieldByName("ID")->AsInteger = newId;
    dstList->FieldByName("NOTICE_TYPE")->AsString = Sender == mniNewDVB ? "GT2" : "GS2";
    dstList->FieldByName("REF_PLAN_CFG")->AsString = Sender == mniNewDVB ? "RPC1" : "PRC4";
    int cntryId = dstList->ParamByName("GRP_ID")->AsInteger;
    dstList->FieldByName("ADM_ID")->AsInteger = cntryId;
    dstList->FieldByName("NB_SUB_AREAS")->AsInteger = 0;
    AnsiString adminCntry = dmMain->getCountryCode(cntryId);
    dstList->FieldByName("CTRY")->AsString = adminCntry;
    dstList->FieldByName("ALLOT_NAME")->AsString = "New "+adminCntry+" Allotment";
    dstList->FieldByName("PLAN_ENTRY")->AsString = "3";
    dstList->FieldByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];


    sql->SQL->Text = "insert into TRANSMITTERS (ID, LATITUDE, LONGITUDE, SYSTEMCAST_ID, STATUS) "
                                "VALUES (:ID, 0.0, 0.0, "+scId+","+dbSectIds[cbxDbSection->ItemIndex]+")";
    sql->ParamByName("ID")->AsInteger = newId;

    try {
        sql->ExecQuery();
        dstList->Post();
        dstList->Transaction->CommitRetaining();
    } __finally {
        dstList->Cancel();
        dstList->Transaction->RollbackRetaining();
    }

    actListEditExecute(Sender);
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDigAllotments::actListInsertExecute(
      TObject *Sender)
{
    TPoint pos = ClientToScreen(TPoint(tbtListIns->Left + tbrList->Left, tbtListIns->Top + tbtListIns->Height + tbrList->Top));
    pmnNewAllotment->Popup(pos.x, pos.y);    
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDigAllotments::EditElement()
{
    if (dstList->RecordCount) {
        FormProvider.ShowTx(txBroker.GetTx(dstListID->AsInteger, CLSID_LisBcDigAllot));
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::dstListCHANNELBLOCKGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (dstListNOTICE_TYPE->AsString == "GS2" || dstListNOTICE_TYPE->AsString == "DS2")
        Text = dstListB_NAME->AsString;
    else
        Text = dstListC_NAME->AsString;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::dstListPLAN_ENTRYGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (Sender->AsString.Length() > 0)
    {
        if (Sender->AsString == "1")
            Text = "1 - Присвоєння";
        else if (Sender->AsString == "2")
            Text = "2 - ОЧМ";
        else if (Sender->AsString == "3")
            Text = "3 - Виділення";
        else if (Sender->AsString == "4")
            Text = "4 - Виділення з Tx-ми і ОЧМ";
        else if (Sender->AsString == "5")
            Text = "5 - Виділення з 1 Tx, без ОЧМ";

    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::dgrListMouseMove(TObject *Sender,
      TShiftState Shift, int X, int Y)
{
    if (Shift == TShiftState() << ssLeft) {
        // высота заголовка
        TDBGrid *grd = dynamic_cast<TDBGrid*>(Sender);
        if (!grd)
            return;

        int h = grd->Canvas->TextHeight("Sy") + 4;
        if (Y > h)
            dgrList->BeginDrag(false, 3);
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::actImportDplanExecute(
      TObject *Sender)
{
    std::auto_ptr<TdlgImportDigLayer> dlg(new TdlgImportDigLayer(this));
    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dstList->Database;
    sql->Transaction = dstList->Transaction;
    sql->SQL->Text = "select l.id, l.name || ' (' || p.name || ', ' || count(*) || ' зон)' from dplan_layer l "
                     "left outer join dplan_plan p on (l.PLANID = p.id) "
                        "inner join DPLAN_ZONE z on (z.LAYER = l.id) "
                        "group by l.id, l.name, p.name "
                        "order by p.name ";
    dlg->lbxLayers->Items->Clear();
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
        dlg->lbxLayers->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);

    sql->Close();
    sql->SQL->Text = "select ID, NAME from COUNTRY order by NAME";
    dlg->cbxCountry->Items->Clear();
    for (sql->ExecQuery(); !sql->Eof; sql->Next())
        dlg->cbxCountry->Items->AddObject(sql->Fields[1]->AsString, (TObject*)sql->Fields[0]->AsInteger);
    sql->Close();

    int maxCount = Screen->Height / dlg->cbxCountry->ItemHeight / 3;
    if (dlg->cbxCountry->Items->Count > maxCount)
        dlg->cbxCountry->DropDownCount = maxCount;
    else
        dlg->cbxCountry->DropDownCount = dlg->cbxCountry->Items->Count;

    sql->Transaction->CommitRetaining();

    if (dlg->ShowModal() == mrOk)
    {
        sql->SQL->Text = "select id from SYSTEMCAST where ENUMVAL = " + IntToStr(ttAllot);
        sql->ExecQuery();
        if (sql->Eof)
            throw *(new Exception("Не могу найти тип объекта ВЫДЕЛЕНИЕ ("+IntToStr(ttAllot)+") в таблице SYSTEMCAST"));

        AnsiString scId = sql->Fields[0]->AsString;
        sql->Close();

        int countryId = (int)dlg->cbxCountry->Items->Objects[dlg->cbxCountry->ItemIndex];
        AnsiString adminCntry = dmMain->getCountryCode(countryId);

        sql->SQL->Text = "select z.id, z.name from DPLAN_ZONE z where z.LAYER = :L ";
        sql->Prepare();

        std::auto_ptr<TIBSQL> sqlGetCont(new TIBSQL(this));
        sqlGetCont->Database = dstList->Database;
        sqlGetCont->Transaction = dstList->Transaction;
        sqlGetCont->SQL->Text = "select c.LON, c.LAT from DPLAN_COORD c where c.ZONE = :Z order by c.NOM";
        sqlGetCont->Prepare();

        std::auto_ptr<TIBSQL> sqlPutCont(new TIBSQL(this));
        sqlPutCont->Database = dstList->Database;
        sqlPutCont->Transaction = dstList->Transaction;
        sqlPutCont->SQL->Text = "INSERT INTO DIG_SUBAREAPOINT (ALLOT_ID, CONT_NO, POINT_NO, LAT, LON) "
                                "VALUES (:ALLOT_ID, 1, :POINT_NO, :LAT, :LON)";
        sqlPutCont->Prepare();

        std::auto_ptr<TIBSQL> sqlPutAllot(new TIBSQL(this));
        sqlPutAllot->Database = dstList->Database;
        sqlPutAllot->Transaction = dstList->Transaction;
        sqlPutAllot->SQL->Text = dstList->InsertSQL->Text;
        sqlPutAllot->Prepare();

        std::auto_ptr<TIBSQL> sqlPutTx(new TIBSQL(this));
        sqlPutTx->Database = dstList->Database;
        sqlPutTx->Transaction = dstList->Transaction;
        sqlPutTx->SQL->Text = "insert into TRANSMITTERS (ID, LATITUDE, LONGITUDE, SYSTEMCAST_ID) "
                                "VALUES (:ID, :LAT, :LON, "+scId+")";
        sqlPutTx->Prepare();

        try {

          int contNo = 0;

          for (int i= 0; i < dlg->lbxLayers->Count; i++)
            if (dlg->lbxLayers->Checked[i])
            {
                sql->ParamByName("L")->AsInteger = (int)dlg->lbxLayers->Items->Objects[dlg->lbxLayers->ItemIndex];

                for (sql->ExecQuery(); !sql->Eof; sql->Next(), contNo++)
                {
                    int newId = dmMain->getNewId();
                    sqlPutAllot->ParamByName("ID")->AsInteger = newId;
                    sqlPutAllot->ParamByName("DAB_DVB")->AsString = dlg->rbDvb->Checked ? "V" : "A";
                    sqlPutAllot->ParamByName("RPC")->AsInteger = dlg->rbDvb->Checked ? rpc1 : rpc4;
                    sqlPutAllot->ParamByName("ADMIN_ID")->AsInteger = countryId;
                    sqlPutAllot->ParamByName("ADMINISTRATION")->AsString = adminCntry;
                    sqlPutAllot->ParamByName("COUNTRY")->AsString = adminCntry;
                    AnsiString name = sql->FieldByName("NAME")->AsString;
                    if (name.Length() > 32) name.SetLength(32);
                    sqlPutAllot->ParamByName("NAME")->AsString = name;
                    sqlPutAllot->ParamByName("RRC06_CODE")->AsString = "3";
                    sqlPutAllot->ExecQuery();
                    sqlPutAllot->Close();

                    sqlGetCont->ParamByName("Z")->AsInteger = sql->FieldByName("ID")->AsInteger;
                    int totalPoints = 0;
                    double totalLon = 0.0; double totalLat = 0.0;

                    for (sqlGetCont->ExecQuery(); !sqlGetCont->Eof && totalPoints < 99; sqlGetCont->Next())
                    {
                        double lon = sqlGetCont->FieldByName("LAT")->AsDouble;
                        double lat = sqlGetCont->FieldByName("LON")->AsDouble;

                        sqlPutCont->ParamByName("LON")->AsDouble = lon;
                        sqlPutCont->ParamByName("LAT")->AsDouble = lat;
                        sqlPutCont->ParamByName("ALLOT_ID")->AsInteger = newId;
                        totalPoints++;
                        sqlPutCont->ParamByName("POINT_NO")->AsInteger = totalPoints;
                        sqlPutCont->ExecQuery();
                        sqlPutCont->Close();

                        totalLon += lon;
                        totalLat += lat;
                    }
                    sqlGetCont->Close();

                    sqlPutTx->ParamByName("ID")->AsInteger = newId;
                    sqlPutTx->ParamByName("LAT")->AsDouble = (totalPoints == 0) ? 0 : totalLat / totalPoints;
                    sqlPutTx->ParamByName("LON")->AsDouble = (totalPoints == 0) ? 0 : totalLon / totalPoints;
                    sqlPutTx->ExecQuery();
                    sqlPutTx->Close();

                }

            }

            ShowMessage("Импортировано "+IntToStr(contNo)+" контуров");

            dstList->Transaction->CommitRetaining();
            dstList->Close();
            dstList->Open();

        } __finally {
            dstList->Transaction->RollbackRetaining();
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::actImportGt2Gs2Execute(
      TObject *Sender)
{
    dmMain->ImportRrc06(TfrmRrc06Import::imGs2Gt2);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::actExportGt2Gs2Execute(
      TObject *Sender)
{
    TfrmRrc06Export::ExportRrc006FromGrid(emGs2Gt2, dgrList);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::actListDeleteExecute(
      TObject *Sender)
{
    if (dstListDB_SECTION_ID->AsInteger != tsDraft)
        throw *(new Exception("Удалить выделение можно только в Предбазе"));

    //if (MessageBox(NULL, "Удалить выделение?", "Подтверждение", MB_ICONQUESTION | MB_YESNO) != IDYES)
    //    return;

    std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
    sql->Database = dstList->Database;
    sql->Transaction = dstList->Transaction;
    sql->SQL->Text = "delete from TRANSMITTERS where ID = " + dstListID->AsString;

    try {
        dstList->Delete();
        sql->ExecQuery();
        dstList->Transaction->CommitRetaining();
    } __finally {
        dstList->Transaction->RollbackRetaining();
        dstList->Refresh();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::cbxDbSectionChange(TObject *Sender)
{
    dstList->Close();
    if (cbxDbSection->ItemIndex > -1)
    {
        dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];
        dstList->Open();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListDigAllotments::actMoveToSectionExecute(
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
    dlg->lblComment->Caption = "Выберите секцию БД, в которую нужно переместить выделения";
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
            sql->SQL->Text = AnsiString("update ") +  "DIG_ALLOTMENT" + " set DB_SECTION_ID = "
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

void __fastcall TfrmListDigAllotments::actCopyToSectionExecute(
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
    dlg->lblComment->Caption = "Выберите секцию БД, в которую нужно скопировать выделения";
    if (dlg->ShowModal() == mrOk && dlg->lb->ItemIndex > -1)
    {
        //TBookmark bm =
    }
}
//---------------------------------------------------------------------------

