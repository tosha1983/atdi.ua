//---------------------------------------------------------------------------

#include <vcl.h>

#include <IBQuery.hpp>

#include <vector>
#pragma hdrstop

#include "ExpServer_TLB.h"

#include "FormProvider.h"
#include "TxBroker.h"
#include "uAnalyzer.h"
#include "uExplorer.h"
#include "uExportDlg.h"
#include "uListTransmitters.h"
#include "uMainDm.h"
#include "uMainForm.h"
#include "uParams.h"
#include "uDlgList.h"
#include "uItuExport.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListTransmitters *frmListTransmitters;

const unsigned int txDraft         = 0x10000000;
const unsigned int txReg           = 0x20000000;
const unsigned int txDel           = 0x40000000;
const unsigned int txAnySystemCast = 0x0FFFFFFF;

//---------------------------------------------------------------------------
__fastcall TfrmListTransmitters::TfrmListTransmitters(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListTransmitters::TfrmListTransmitters(TComponent* Owner, HWND caller, int elementId, unsigned int TxFlags)
    : list_flags(TxFlags),
    FOwner(Owner),
    Fcaller(caller),
    FelementId(elementId),
    FTxFlags(TxFlags),

    TfrmBaseListTree(Owner, caller, elementId)
{

    try {
        Initialize(); // another time - members are initialized after constructors
        if (!dstList->Transaction->Active)
            dstList->Transaction->StartTransaction();
    } catch(Exception &e) {
        Application->MessageBox(e.Message.c_str(), Application->Title.c_str(), MB_ICONHAND);
    }
    setCaption();

    if (list_flags & txDel) {
        actIntoBase->Enabled = true;
        actIntoarchives->Enabled = false;
        actIntoBeforeBase->Enabled = true;
        actListDelete->Enabled = true;
        dstListDATE_DELETED->Visible = true;
        dstListUSERDELNAME->Visible = true;
    } else if (list_flags & txReg) {
        actIntoBase->Enabled = false;
        actIntoarchives->Enabled = true;
        actIntoBeforeBase->Enabled = true;
        actListDelete->Enabled = false;
    } else if (list_flags & txDraft) {
        actIntoBase->Enabled = true;
        actIntoarchives->Enabled = true;
        actIntoBeforeBase->Enabled = false;
        actListDelete->Enabled = false;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::Initialize()
{
  //if (list_flags) {
    try {

        AnsiString TypeTx;
        int dbSection = 0;

        if (list_flags & txDel)
            dbSection = -1;
        else if (list_flags & txDraft)
            dbSection = 1;

        if (list_flags & (1 << ttTV)) TypeTx = "1";
        else if (list_flags & (1 << ttFM)) TypeTx = "2";
        else if (list_flags & (1 << ttDVB)) TypeTx = "4";
        else if (list_flags & (1 << ttDAB)) TypeTx = "5";
        else if (list_flags & (1 << ttCTV)) TypeTx = "6";
        else TypeTx = "";

        AnsiString sc_id;

        if (TypeTx != "") {
            if (!dmMain->ibdsScList->Active)
                  dmMain->ibdsScList->Open();
            if (dmMain->ibdsScList->Locate("ENUMVAL", TypeTx, TLocateOptions()))
            sc_id =  " and SYSTEMCAST_ID = " + dmMain->ibdsScListID->AsString + " ";
        } else sc_id = "";

        std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
        sql->Database = dmMain->dbMain;
        sql->SQL->Text = "select ID, SECTIONNAME from DATABASESECTION ";
        dbSectIds.clear();
        cbxDbSection->Items->Clear();
        for (sql->ExecQuery(); !sql->Eof; sql->Next())
        {
            dbSectIds.push_back(sql->Fields[0]->AsInteger);
            cbxDbSection->Items->Add(sql->Fields[1]->AsString);
        }

        for (int i = 0; i < dbSectIds.size(); i++)
            if (dbSectIds[i] == dbSection)
                cbxDbSection->ItemIndex = i;

        for (int i = 0; i < dstList->SelectSQL->Count; i++)
            if (dstList->SelectSQL->Strings[i].LowerCase().Pos("and status =") > 0)
            {
                dstList->SelectSQL->Strings[i] = (" and STATUS = :DB_SECTION_ID " + sc_id);
                break;
            }

        if (cbxDbSection->ItemIndex > -1)
            dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];

        TfrmBaseListTree::Initialize();

    } catch(Exception &e) {
        Application->MessageBox(("Ошибка инициализации списка передатчиков:\n"+e.Message).c_str(),
                                Application->Title.c_str(), MB_ICONHAND);
    }
  //}
}


void __fastcall TfrmListTransmitters::changeBranch(TTreeNode* node)
{
    dstList->Close();
    TIBXSQLVAR *p;
    if (node) {
        //  определим уровень - страна, регион,
        int level = 0;
        TTreeNode* n = node;
        while (n = n->Parent)
            level++;
        for (int i = 0; i < dstList->SelectSQL->Count; i++)
            if (dstList->SelectSQL->Strings[i].LowerCase().Pos("where") > 0) {
                switch (level) {
                    case 0: dstList->SelectSQL->Strings[i] = "where area.country_id = :grp_id";
                        break;
                    case 1: dstList->SelectSQL->Strings[i] = "where stand.area_id = :grp_id";
                        break;
                }
                break;
            }
        dstList->ParamByName("GRP_ID")->AsInteger = (int)node->Data;
        if (cbxDbSection->ItemIndex > -1)
            dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];
        dstList->Open();
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTransmitters::dstListTX_LATGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::dstListTX_LONGGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::dstListNAMECHANNELGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    int val = dstListS_ENUMVAL->AsInteger;
    switch (val) {
        case ttTV:
        case ttDVB:
            Text = dstListNAMECHANNEL->AsString;
            break;
        case -1: 
        case ttAM:
        case ttFM:
            Text = dstListSOUND_CARRIER->AsString;
            break;
        case ttDAB:
            Text = dstListBD_NAME->AsString;
            break;
        default: Text = " - ";
            break;
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actListEditExecute(TObject *Sender)
{
    if (dstList->RecordCount) {
        FormProvider.ShowTx(txBroker.GetTx(dstListTX_ID->AsInteger));
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTransmitters::dgrListDblClick(TObject *Sender)
{
    if (m_caller)
        this->actOkExecute(this);
    else
        this->actListEditExecute(this);
}
//---------------------------------------------------------------------------

/*
    bool ac1 = true; bool ac2 = true;

    if (list_flags & (txDraft | txReg | txDel)) {
        ac1 = false;
        switch (dstListSTATUS->AsInteger) {
            case 0: // reg
                if (list_flags & txReg)
                    ac1 = true;
                break;
            case 1: // Draft
                if (list_flags & txDraft)
                    ac1 = true;
                break;
            case -1: // archive
                if (list_flags & txDel)
                    ac1 = true;
                break;
            default: // draft
                //  unknown
                break;
        }
    }

    if (list_flags & txAnySystemCast) {
        ac2 = false;
        switch (dstListS_ENUMVAL->AsInteger) {
            case ttTV:
                if (list_flags & (1 << ttTV))
                    ac2 = true;
                break;
            case ttFM:
                if (list_flags & (1 << ttFM))
                    ac2 = true;
                break;
            case ttDVB:
                if (list_flags & (1 << ttDVB))
                    ac2 = true;
                break;
            case ttDAB:
                if (list_flags & (1 << ttDAB))
                    ac2 = true;
                break;
            case ttCTV:
                if (list_flags & (1 << ttCTV))
                    ac2 = true;
                break;
        }
    }

    Accept = ac1 & ac2;
*/
//-------------------------------------------------------------------
void __fastcall TfrmListTransmitters::setCaption()
{
    AnsiString newCaption = "Передавачі";
    if (list_flags & (txDraft | txReg | txAnySystemCast)) {
        newCaption = newCaption + ":";
        if (cbxDbSection->ItemIndex > -1)
            newCaption = newCaption + ' ' + dmMain->getDbSectionInfo(dbSectIds[cbxDbSection->ItemIndex]).name + ',';
        if (list_flags & (1 << ttTV))
            newCaption = newCaption + " АТБ,";
        if (list_flags & (1 << ttFM))
            newCaption = newCaption + " АРМ,";
        if (list_flags & (1 << ttDVB))
            newCaption = newCaption + " ЦТБ,";
        if (list_flags & (1 << ttDAB))
            newCaption = newCaption + " ЦРМ,";
        if (list_flags & (1 << ttCTV))
            newCaption = newCaption + " КТБ,";
    }
    char lastChar = newCaption[newCaption.Length()];
    if (lastChar == ',' || lastChar == ':')
        newCaption.Delete(newCaption.Length(), 1);

    if (m_caller) {
        newCaption = newCaption + " (вибір)";
    } else {
        newCaption = newCaption + " (довідник)";
    }

    Caption = newCaption;

}

void __fastcall TfrmListTransmitters::dgrListKeyDown(TObject *Sender,
      WORD &Key, TShiftState Shift)
{
    if (m_caller) {
        if (Key == 13 && Shift == TShiftState())
            this->actOkExecute(this);
        if (Key == 27 && Shift == TShiftState())
            this->actCloseExecute(this);
    } else if (Key == 13 && Shift == TShiftState()) {
        this->actListEditExecute(this);
    } else {
        TfrmBaseListTree::dgrListKeyDown(Sender, Key, Shift);
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmListTransmitters::dgrListMouseMove(TObject *Sender,
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

int TfrmListTransmitters::SelectIdRootTx(int id)
{

  TIBSQL *sql = new TIBSQL(this);
  sql->Database = dmMain->dbMain;
  sql->SQL->Text = "select ORIGINALID from TRANSMITTERS where id = " + AnsiString(id);
  sql->ExecQuery();
  if (sql->Fields[0]->AsInteger == 0) return id;
  else if (sql->Fields[0]->AsInteger == id) return id;
  else return SelectIdRootTx(sql->Fields[0]->AsInteger);
}


void __fastcall TfrmListTransmitters::chbOnlyRootClick(TObject *Sender)
{
    int id;
    if (chbOnlyRoot->Checked) {
      if (dstList->FieldByName("ORIGINALID")->AsInteger)
         id = SelectIdRootTx(dstList->FieldByName("ORIGINALID")->AsInteger);
      else id = dstList->FieldByName("TX_ID")->AsInteger;
//      dstList->Close();
//      dsrList->Enabled = false;
//      dstList->Open();
      dstList->Filtered = false;
      dstList->Filtered = true;
      Set<TLocateOption,0,1> flags;
      dstList->Locate("TX_ID", AnsiString(id), flags);
      dsrList->Enabled = true;
    } else {
      id = dstList->FieldByName("TX_ID")->AsInteger;
//      dstList->Close();
//      dsrList->Enabled = false;
//      dstList->Open();
      dstList->Filtered = false;
      dstList->Filtered = true;
      Set<TLocateOption,0,1> flags;
      dstList->Locate("TX_ID", AnsiString(id), flags);
      dsrList->Enabled = true;
      }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::dstListVIDEO_OFFSET_HERZGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (!dstListVIDEO_OFFSET_HERZ->IsNull)
        Text = AnsiString(dstListVIDEO_OFFSET_HERZ->AsFloat / 1000);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actIntoProjectExecute(
      TObject *Sender)
{
 SendMessage(frmMain->showExplorer(), WM_LIST_ELEMENT_SELECTED, 39, dstListTX_ID->AsInteger);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::ChangeStatus(int id, int status, bool wasinbase)
{
 AnsiString ask;
 switch (status) {
 case -1 : ask = "Перенести  в архів?"; break;
 case 0 : ask = "Перенести в базу?"; break;
 case 1 : ask = "Перенести в передбазу?"; break;
 default : return;
 }
 if (Application->MessageBox(ask.c_str(), Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) != IDYES)
    return;

        TBookmark old_bookmark = dstList->GetBookmark();
        AnsiString IDs;
        for (int i=0; i < dgrList->SelectedRows->Count; i++) {
            dstList->GotoBookmark((void*)(dgrList->SelectedRows->Items[i].c_str()));
            IDs += dstListTX_ID->AsString + ",";
        }
        dstList->GotoBookmark(old_bookmark);
        IDs = IDs.SubString(1,IDs.Length()-1);


  TIBSQL *sql = new TIBSQL(this);
  sql->Database = dmMain->dbMain;
  AnsiString sqls = "update TRANSMITTERS set STATUS = " + AnsiString(status);
       if (wasinbase)
           sqls +=  " , WAS_IN_BASE = 1";
  sqls +=  " where ID in(" + AnsiString(IDs)+")";
  sql->SQL->Text = sqls;
  sql->ExecQuery();
  dmMain->trMain->CommitRetaining();

  sql->Close();
  frmMain->FormListTxRefresh();

  FormProvider.UpdateTransmitters(id);
}


void __fastcall TfrmListTransmitters::actIntoBaseExecute(TObject *Sender)
{
  ChangeStatus(dstListTX_ID->AsInteger, 0, true);
}
//---------------------------------------------------------------------------

__fastcall TfrmListTransmitters::changeQueryCopy()
{
   ibqInsertCopy->ParamByName("STATUS")->AsInteger = 1;
   ibqInsertCopy->ParamByName("WAS_IN_BASE")->AsInteger = 0;
   ibqInsertCopy->ParamByName("ADMINISTRATIONID")->AsString = "";

    if  (change_systemcast) {
        int new_systemcast;
	    int val = dstListS_ENUMVAL->AsInteger;
	    switch (val) {
	        case ttTV: new_systemcast = (int)ttDVB;
	            break;
	        case ttFM: new_systemcast = (int)ttDAB;
	            break;
	        default:   new_systemcast = 0;
	            break;
	    }
	    if (new_systemcast != 0) {
          if (!dmMain->ibdsScList->Active) dmMain->ibdsScList->Active = true;
	      dmMain->ibdsScList->Locate("ENUMVAL", new_systemcast, TLocateOptions());
	      ibqInsertCopy->ParamByName("SYSTEMCAST_ID")->AsInteger = dmMain->ibdsScListID->AsInteger;
	    }
	    change_systemcast = false;
  }

};

void __fastcall TfrmListTransmitters::actIntoarchivesExecute(
      TObject *Sender)
{
    ChangeStatus(dstListTX_ID->AsInteger, -1, true);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actIntoBeforeBaseExecute(
      TObject *Sender)
{
    ChangeStatus(dstListTX_ID->AsInteger, 1, true);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::N1Click(TObject *Sender)
{
    change_systemcast = false;
    TfrmBaseList::actListCopyExecute(Sender);
    frmMain->FormListTxRefresh();
    if (NewElementId)
       FormProvider.ShowTx(txBroker.GetTx(NewElementId));
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::N2Click(TObject *Sender)
{
    change_systemcast = true;
    TfrmBaseList::actListCopyExecute(Sender);
    frmMain->FormListTxRefresh();
    if (NewElementId)
       FormProvider.ShowTx(txBroker.GetTx(NewElementId));
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actListDeleteExecute(TObject *Sender)
{
    AnsiString IDs;

    std::vector<int> stands;//опоры с которых удаляются передатчики
    TBookmark old_bookmark = dstList->GetBookmark();
    if (dgrList->SelectedRows->Count > 0)
    {
        for (int i=0; i < dgrList->SelectedRows->Count; i++) {
            dstList->GotoBookmark((void*)(dgrList->SelectedRows->Items[i].c_str()));
            IDs += dstListTX_ID->AsString + ",";
            stands.push_back(dstListTX_STAND_ID->AsInteger);
        }
        dstList->GotoBookmark(old_bookmark);
        IDs = IDs.SubString(1,IDs.Length()-1);
    } else {
        IDs = dstListTX_ID->AsString;
    }

    if (IDs.Length() == 0)
        throw *(new Exception("Виберіть передавач(и) для видалення"));

    if (Application->MessageBox("Видалити передавач(и)? Відновлення не можливе!", Application->Title.c_str(), MB_ICONWARNING | MB_YESNO) != IDYES)
        return;

    TIBSQL *sql = new TIBSQL(this);
    sql->Database = dmMain->dbMain;
    //удаление передатчика
    sql->SQL->Text = "delete from TRANSMITTERS  where ( (ID in(" + AnsiString(IDs)+")) AND (WAS_IN_BASE<>1) )";
    sql->ExecQuery();
    //удаление передатчика из консоли
    sql->SQL->Text = "delete from USEROBJECTS where ( (USERID = " + IntToStr(dmMain->UserId) + ") and (OBJECTID in(" + AnsiString(IDs)+")) )";
    sql->ExecQuery();

    //обновить консоль
    if ( frmExplorer )
        frmExplorer->actRefreshExecute(NULL);

    dmMain->trMain->CommitRetaining();
    sql->Close();
    frmMain->FormListTxRefresh();

    //удаление опор без передатчиков
    std::auto_ptr<TIBSQL> sqlCount(new TIBSQL(this));
      sqlCount->Database = dmMain->dbMain;
      sqlCount->SQL->Text = "select Count(*) from TRANSMITTERS where STAND_ID = :STAND_ID";

    for(int i = 0; i < stands.size(); i++)
    {
        sqlCount->ParamByName("STAND_ID")->Value = stands[i];
        sqlCount->ExecQuery();
        if ( sqlCount->Fields[0]->AsInteger == 0 )
        {//удляем опору
            std::auto_ptr<TIBSQL> sqlDelete(new TIBSQL(this));
              sqlDelete->Database = dmMain->dbMain;
              sqlDelete->SQL->Text = "delete from STAND where ID = " + IntToStr(stands[i]);
            sqlDelete->ExecQuery();
            sqlDelete->Close();
        }
        sqlCount->Close();
    }                     
    dmMain->trMain->CommitRetaining();

}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actListCopyExecute(TObject *Sender)
{
    NewElementId = 0;
    change_systemcast = false;

    int val = dstListS_ENUMVAL->AsInteger;
    switch (val) {
        case ttTV:  pmIntoBeforeBase->Items->Items[0]->Caption = "Зробити копію в предбазі АТБ->АТБ";
                    pmIntoBeforeBase->Items->Items[1]->Caption = "Зробити копію в предбазі АТБ->ЦТБ";
                    pmIntoBeforeBase->Items->Items[1]->Visible = true;
            break;
        case ttFM:  pmIntoBeforeBase->Items->Items[0]->Caption = "Зробити копію в предбазі АРМ->АРМ";
                    pmIntoBeforeBase->Items->Items[1]->Caption = "Зробити копію в предбазі АРМ->ЦРМ";
                    pmIntoBeforeBase->Items->Items[1]->Visible = true;
            break;
        default:
                    pmIntoBeforeBase->Items->Items[1]->Visible = false;
            break;
    }

    pmIntoBeforeBase->Popup(tbtListCpy->ClientOrigin.x + tbtListCpy->Width/2,
                            tbtListCpy->ClientOrigin.y + tbtListCpy->Height/2);

}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::dstListEPR_VIDEO_MAXGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    int val = dstListS_ENUMVAL->AsInteger;
    switch (val) {
        case ttTV:
            Text = FormatFloat("0.##", dstListEPR_VIDEO_MAX->AsFloat);
            break;
        case ttFM:
        case ttDAB:
        case ttDVB:
            Text = FormatFloat("0.##", dstListEPR_SOUND_MAX_PRIMARY->AsFloat);
            break;
        default: Text = " - ";
            break;
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmListTransmitters::actListInsertExecute(TObject *Sender)
{
    frmMain->actNewTxExecute(this);
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTransmitters::refresh()
{
    dstList->Transaction->Active = false;
    TfrmListTransmitters(FOwner, Fcaller, FelementId, FTxFlags);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actListDeleteUpdate(TObject *Sender)
{
    if ( ( (list_flags & txDel) ||(list_flags & txDraft) )&& ( dstListWAS_IN_BASE->AsInteger != 1 ) )
        actListDelete->Enabled = true;
    else
        actListDelete->Enabled = false;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actExportExecute(TObject *Sender)
{
    if (dstList->RecordCount == 0) {
        Application->MessageBox("Список пустий.", Application->Title.c_str(), MB_ICONEXCLAMATION);
        return;
    }

    if (!dlgExport)
        dlgExport = new TdlgExport(Application);

    if (list_flags & (1 << ttTV)) {
        dlgExport->rgFormat->ItemIndex = 0;
        dlgExport->rgFormatClick(dlgExport);
    } else if (list_flags & (1 << ttDVB)) {
        dlgExport->rgFormat->ItemIndex = 1;
        dlgExport->rgFormatClick(dlgExport);
    }

    if (dlgExport->ShowModal() == mrOk)
        txExporter.exportTxGrid(dlgExport->rgFormat->ItemIndex, dlgExport->rgList->ItemIndex, dgrList, dlgExport->edtFilename->Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actRecalcTbtCoordDistsExecute(
      TObject *Sender)
{
    if (Application->MessageBox("Recalculate coordination distancies?", "Shure?", MB_ICONQUESTION | MB_YESNO) == IDYES) {
        TCOMILISBCTxList txList;
        txList.CreateInstance(CLSID_LISBCTxList);

        std::auto_ptr<TIBSQL> sqlIds(new TIBSQL(Application));
        sqlIds->Database = dmMain->dbMain;
        sqlIds->SQL->Text = "select t.ID, s.ENUMVAL from TRANSMITTERS t left outer join systemcast s on (t.systemcast_id = s.id)";

        frmMain->StatusBar1->SimplePanel = true;
        frmMain->StatusBar1->SimpleText = "Формирование списка...";
        TDateTime begTime = Time();

        frmMain->StatusBar1->Update();
        int cnt = 1;
        try {
            sqlIds->ExecQuery();
            while(!sqlIds->Eof) {
                txList.AddTx(txBroker.GetTx(sqlIds->Fields[0]->AsInteger, dmMain->GetObjClsid(sqlIds->Fields[1]->AsInteger)));
                sqlIds->Next();
                cnt++;
                if ((cnt << 7) >> 7 == cnt) {
                    frmMain->StatusBar1->SimpleText = "Формирование списка: " + AnsiString(cnt);
                    Application->Title = frmMain->StatusBar1->SimpleText;
                    frmMain->StatusBar1->Update();
                }
            }
        } __finally {
            frmMain->StatusBar1->SimplePanel = false;
        }

        TDateTime listTime = Time();

        Application->ProcessMessages();

        sqlIds->Transaction->CommitRetaining();
        sqlIds->Close();

        txBroker.EnsureList(txList, frmMain->pb);

        TDateTime extractTime = Time();

        Application->ProcessMessages();

        sqlIds->SQL->Text = "update TRANSMITTERS set MAX_COORD_DIST = :MAX_COORD_DIST where ID = :ID";
        sqlIds->Prepare();

        frmMain->StatusBar1->SimplePanel = true;
        frmMain->StatusBar1->SimpleText = "Расчёт координационных зон...";
        frmMain->pb->Min = 0;
        frmMain->pb->Position = 0;
        frmMain->pb->Visible = true;
        frmMain->pb->Max = txList.Size;
        frmMain->StatusBar1->Update();

        Screen->Cursor = crHourGlass;

        double zone[36];
        int packetCount = 0;

        try {
            for (int i = 0; i < txList.Size; i++) {
                setmem (zone, 36*sizeof(double), '\0');
                double maxCoordDist = 15;
                txAnalyzer.GetCoordinationZone(txList.get_Tx(i), zone);
                for (int i = 0; i < 36; i++) {
                    if (maxCoordDist < zone[i]) {
                        maxCoordDist = zone[i];
                    }
                }
                sqlIds->ParamByName("MAX_COORD_DIST")->AsDouble = maxCoordDist;
                int txId = txList.get_TxId(i);
                sqlIds->ParamByName("ID")->AsInteger = txId;
                sqlIds->ExecQuery();
                if (packetCount++ >= 1000) {
                    packetCount = 0;
                    sqlIds->Transaction->Commit();
                    sqlIds->Transaction->StartTransaction();
                } else {
                    sqlIds->Transaction->CommitRetaining();
                }
                frmMain->pb->StepIt();
                frmMain->StatusBar1->SimpleText = "Расчёт координационных зон: " + AnsiString(i+1) + " (ID " + AnsiString(txId) + ')';
                Application->Title = frmMain->StatusBar1->SimpleText;
                frmMain->StatusBar1->Update();
            }
        } __finally {
            frmMain->StatusBar1->SimplePanel = false;
            frmMain->pb->Visible = false;
            Screen->Cursor = crDefault;
        }

        TDateTime calcTime = Time();
        Application->MessageBoxA((AnsiString(
                                "List forming time: ") + FormatDateTime("hh:nn:ss.zzz", listTime - begTime) +
                                "\nExtract time: " + FormatDateTime("hh:nn:ss.zzz", extractTime - listTime) +
                                "\nCalculation/update time: " + FormatDateTime("hh:nn:ss.zzz", calcTime - extractTime) +
                                "\nTotal time: " + FormatDateTime("hh:nn:ss.zzz", calcTime - begTime)
                                ).c_str(),
                                "Completed", MB_ICONINFORMATION
                                );
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actMoveToExecute(TObject *Sender)
{
    std::set<int> stands;

    TBookmark old_bookmark = dstList->GetBookmark();

    //запомним опоры выбранных передатчиков
    for (int i=0; i < dgrList->SelectedRows->Count; i++)
    {
        dstList->GotoBookmark((void*)(dgrList->SelectedRows->Items[i].c_str()));
        stands.insert(dstListTX_STAND_ID->AsInteger);
    }

    dstList->GotoBookmark(old_bookmark);

    if ( !stands.empty() )
        moveRecords(stands);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::moveRecords(std::set<int>& standIds)
{
    if (Application->MessageBox("Перенести передавач(і)?", "Перенесення.", MB_ICONWARNING | MB_YESNO) == IDYES )
    {
        //тут выбрать регион...
        int areaId = getAreaId();

        if ( areaId > 0 )
        {
            checkWasInBase(standIds);

            if ( !standIds.empty() )
            {
                AnsiString ids;
                for ( std::set<int>::const_iterator id = standIds.begin(); id != standIds.end(); id++ )
                    ids += IntToStr(*id) + ", ";
                ids.Delete(ids.Length() - 1, 2);

                std::auto_ptr<TIBSQL> sql(new TIBSQL(this));
                  sql->Database = dmMain->dbMain;;
                  sql->GoToFirstRecordOnExecute = true;
                  sql->SQL->Text = "update Stand set area_id = " + IntToStr(areaId) + " where ( id in (" + ids + ") )";

                sql->ExecQuery();
                sql->Close();

                dmMain->trMain->CommitRetaining();
                frmMain->FormListStandRefresh();
                frmMain->FormListTxRefresh();

                //обновить консоль
                if ( frmExplorer )
                    frmExplorer->actRefreshExecute(NULL);
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::cbxDbSectionChange(TObject *Sender)
{
    dstList->Close();
    if (cbxDbSection->ItemIndex > -1)
    {
        dstList->ParamByName("DB_SECTION_ID")->AsInteger = dbSectIds[cbxDbSection->ItemIndex];
        dstList->Open();
    }
    setCaption();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTransmitters::actMoveToSectionExecute(
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
            sql->SQL->Text = AnsiString("update ") +  "TRANSMITTERS" + " set STATUS = "
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

void __fastcall TfrmListTransmitters::actCopyToSectionExecute(
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

void __fastcall TfrmListTransmitters::actExpGt1Gs1Execute(TObject *Sender)
{
    TfrmRrc06Export::ExportRrc006FromGrid(emGs1Gt1, dgrList);
}
//---------------------------------------------------------------------------

