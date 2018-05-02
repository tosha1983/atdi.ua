//---------------------------------------------------------------------------

#include <vcl.h>
#include <memory>
#pragma hdrstop

#include "FormProvider.h"
#include "Transliterator.hpp"
#include "TxBroker.h"

#include "uExplorer.h"
#include "uListStand.h"
#include "uMainDm.h"
#include "uMainForm.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListStand *frmListStand;
//---------------------------------------------------------------------------
__fastcall TfrmListStand::TfrmListStand(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
    actListDelete->Enabled = true;
    actListDelete->Visible = true;
}
//---------------------------------------------------------------------------
__fastcall TfrmListStand::TfrmListStand(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseListTree(Owner, caller, elementId)
{
    actListDelete->Enabled = true;
    actListDelete->Visible = true;
}
//---------------------------------------------------------------------------
void __fastcall TfrmListStand::changeBranch(TTreeNode* node)
{
    dstList->Close();
    TIBXSQLVAR *p;
    if (node) {
        //  определим уровень - страна, регион,
        int level = 0;
        TTreeNode* n = node;
        while (n = n->Parent)
            level++;
        switch (level) {
            case 0: dstList->SelectSQL->Strings[2] = "where A.COUNTRY_ID = :GRP_ID";
                break;
            case 1: dstList->SelectSQL->Strings[2] = "where S.AREA_ID = :GRP_ID";
                break;
        }
        dstList->ParamByName("GRP_ID")->AsInteger = (int)node->Data;
        dstList->Open();
    }
}
void __fastcall TfrmListStand::updateLookups()
{
    TfrmBaseListTree::updateLookups();
    TIBSQL* sql = new TIBSQL(this);
    try {
        sql->Database = dstList->Database;
        //  Регіон
        sql->SQL->Text = AnsiString("select NAME from AREA where ID = ") + IntToStr(dstListAREA_ID->AsInteger);
        sql->ExecQuery();
        if (!sql->Eof)
            dstListA_NAME->AsString = sql->Fields[0]->AsString;
        else
            dstListA_NAME->AsString = "";
        sql->Close();
        //  Район
        sql->SQL->Text = AnsiString("select NAME from DISTRICT where ID = ") + IntToStr(dstListDISTRICT_ID->AsInteger);
        sql->ExecQuery();
        if (!sql->Eof)
            dstListD_NAME->AsString = sql->Fields[0]->AsString;
        else
            dstListD_NAME->AsString = "";
        sql->Close();
        //  Населеий пункт
        sql->SQL->Text = AnsiString("select NAME from CITY where ID = ") + IntToStr(dstListCITY_ID->AsInteger);
        sql->ExecQuery();
        if (!sql->Eof)
            dstListC_NAME->AsString = sql->Fields[0]->AsString;
        else
            dstListC_NAME->AsString = "";
        sql->Close();
    } __finally {
        sql->Free();
    }
}

void __fastcall TfrmListStand::dstTxTX_LATGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstTxTX_LONGGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstTxNAMECHANNELGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    int val = dstTxS_ENUMVAL->AsInteger;
    switch (val) {
        case ttTV:
        case ttDVB:
            Text = dstTxNAMECHANNEL->AsString;
            break;
        case -1:
        case ttAM:
        case ttFM:
            Text = dstTxSOUND_CARRIER->AsString;
            break;
        case ttDAB:
            Text = dmMain->getDabBlockName(dstTxBLOCKCENTREFREQ->AsFloat);
            break;
        default: Text = " - ";
            break;
    }
}
//---------------------------------------------------------------------------
void __fastcall TfrmListStand::Initialize()
{
    TfrmBaseListTree::Initialize();
    dstTx->Open();
}
//---------------------------------------------------------------------------
void __fastcall TfrmListStand::dstListCITY_IDChange(TField *Sender)
{
    //  установить район и регион
    TIBSQL* sql = new TIBSQL(this);
    try {
        sql->Database = dstList->Database;
        //  Регіон
        sql->SQL->Text = AnsiString("select AREA_ID, DISTRICT_ID from CITY where ID = ") + IntToStr(dstListCITY_ID->AsInteger);
        sql->ExecQuery();
        if (!sql->Eof) {
            if (sql->Fields[0]->IsNull)
                dstListAREA_ID->SetData(NULL);
            else
                dstListAREA_ID->Value = sql->Fields[0]->Value;
            if (sql->Fields[1]->IsNull)
                dstListDISTRICT_ID->SetData(NULL);
            else
                dstListDISTRICT_ID->Value = sql->Fields[1]->Value;
        } else {
            dstListAREA_ID->SetData(NULL);
            dstListDISTRICT_ID->SetData(NULL);
        }
        sql->Close();
    } __finally {
        sql->Free();
    }

    //  список улиц - в пиклист
    TColumn* col = fillStreetPickList();
    //  если улица уже установлена, вывести её, если нет - очистить поле
    int idx = col->PickList->IndexOfObject((TObject*)dstListSTREET_ID->AsInteger);
    if (idx == -1)
        dstListST_NAME->AsString = "";
    else
        dstListST_NAME->AsString = col->PickList->Strings[idx];

}
//---------------------------------------------------------------------------


void __fastcall TfrmListStand::dstListST_NAMEChange(TField *Sender)
{
    //  установить правильный STREET_ID, если такого нет - добавить в базу
    // 1.найти колонку
    if (dstListST_NAME->AsString == "")
        return;

    TColumn* col = NULL;
    for (int i = 0; i < dgrList->Columns->Count; i++)
        if (dgrList->Columns->Items[i]->Field == dstListST_NAME) {
            col = dgrList->Columns->Items[i];
            break;
        }
    if (!col)
        return;

    // 2.индекс в пиклисте
    int idx = col->PickList->IndexOf(dstListST_NAME->AsString);
    // 3.если нет - добавить
    if (idx == -1) {
        if (Application->MessageBox(
        (AnsiString("Вулиці \'")+dstListST_NAME->AsString+"\' населеного пункту \'"+dstListC_NAME->AsString+"\' в базі немає. Додати?").c_str(),
        Application->Title.c_str(),
        MB_ICONQUESTION | MB_YESNO) != IDYES){
            dstListST_NAME->AsString = "";
            return;
        }
        sqlInsertStreet->Close();
        int newId = dmMain->getNewId();
        sqlInsertStreet->ParamByName("ID")->AsInteger = newId;
        sqlInsertStreet->ParamByName("NAME")->AsString = dstListST_NAME->AsString;
        sqlInsertStreet->ParamByName("CITY_ID")->AsInteger = dstListCITY_ID->AsInteger;
        sqlInsertStreet->ExecQuery();
        sqlInsertStreet->Transaction->CommitRetaining();
        sqlInsertStreet->Close();

        col->PickList->AddObject(dstListST_NAME->AsString, (TObject*)newId);
    }

    //  ещё раз индекс
    idx = col->PickList->IndexOf(dstListST_NAME->AsString);
    dstListSTREET_ID->AsInteger = (int)col->PickList->Objects[idx];
}
//---------------------------------------------------------------------------


TColumn* __fastcall TfrmListStand::fillStreetPickList()
{
    //  1.сначала найти колонку
    TColumn* col = NULL;
    for (int i = 0; i < dgrList->Columns->Count; i++)
        if (dgrList->Columns->Items[i]->Field == dstListST_NAME) {
            col = dgrList->Columns->Items[i];
            break;
        }
    if (!col)
        return col;

    //  2.заполнить пиклист
    col->PickList->Clear();
    sqlStreets->ParamByName("CITY_ID")->AsInteger = dstListCITY_ID->AsInteger;
    sqlStreets->ExecQuery();
    while(!sqlStreets->Eof) {
        col->PickList->AddObject(sqlStreets->Fields[1]->AsString, (TObject*)sqlStreets->Fields[0]->AsInteger);
        sqlStreets->Next();
    }
    sqlStreets->Close();

    return col;
}
void __fastcall TfrmListStand::dstListAfterScroll(TDataSet *DataSet)
{
    TfrmBaseListTree::dstListAfterScroll(DataSet);
    fillStreetPickList();
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstListLATITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'Y');
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstListLONGITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------


void __fastcall TfrmListStand::grdTxDblClick(TObject *Sender)
{
    if (dstTx->RecordCount) {
        FormProvider.ShowTx(txBroker.GetTx(dstTxTX_ID->AsInteger, dmMain->GetObjClsid(dstTxS_ENUMVAL->AsInteger)));
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::grdTxKeyDown(TObject *Sender, WORD &Key,
      TShiftState Shift)
{
    if (Key == 13 && Shift == TShiftState())
        grdTxDblClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstListLATITUDESetText(TField *Sender,
      const AnsiString Text)
{
    Sender->AsFloat = dmMain->strToCoord(Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstListLONGITUDESetText(TField *Sender,
      const AnsiString Text)
{
    Sender->AsFloat = dmMain->strToCoord(Text);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstTxEPR_VIDEO_MAXGetText(TField *Sender, AnsiString &Text, bool DisplayText)
{
    int val = dstTxS_ENUMVAL->AsInteger;
    switch (val) {
        case ttTV: Text = dstTxEPR_VIDEO_MAX->AsString;
            break;
        case ttDAB:
        case ttDVB:
        case ttAM:
        case ttFM: Text = dstTxEPR_SOUND_MAX_PRIMARY->AsString;
            break;
        default: Text = " - ";
            break;
    }
}
//---------------------------------------------------------------------------


void __fastcall TfrmListStand::grdTxMouseMove(TObject *Sender,
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
void __fastcall TfrmListStand::dstListAfterPost(TDataSet *DataSet)
{
     TfrmBaseList::dstListAfterPost(DataSet);
     FormProvider.UpdateStands(DataSet->Fields->FieldByName("ID")->AsInteger);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::dstListNAMESITESetText(TField *Sender,
      const AnsiString Text)
{
    if (dstListNAMESITE_ENG->AsString == RusToEng(dstListNAMESITE->AsString))
        dstListNAMESITE_ENG->AsString = RusToEng(Text);
    dstListNAMESITE->AsString = Text;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::actMoveToExecute(TObject *Sender)
{
    std::set<int> stands;

    TBookmark old_bookmark = dstList->GetBookmark();

    for ( int i=0; i < dgrList->SelectedRows->Count; i++ )
    {
        dstList->GotoBookmark((void *)dgrList->SelectedRows->Items[i].c_str());
        stands.insert(dstList->Fields->Fields[0]->AsInteger);
    }

    dstList->GotoBookmark(old_bookmark);

    if ( !stands.empty() )
        moveRecords(stands);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListStand::moveRecords(std::set<int>& standIds)
{
    if (Application->MessageBox("Перенести опору(и)?", "Перенесення.", MB_ICONWARNING | MB_YESNO) == IDYES )
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



void __fastcall TfrmListStand::dstListBeforeDelete(TDataSet *DataSet)
{
    if (dstTx->RecordCount > 0)
        throw *(new Exception("На опоре находятся передатчики - удаление невозможно"));
    if (Application->MessageBox("Удалить текущую опору?", Application->Title.c_str(), MB_ICONQUESTION | MB_YESNO) != IDYES)
        Abort();
}
//---------------------------------------------------------------------------


void __fastcall TfrmListStand::actSearchCoordExecute(TObject *Sender)
{
    MessageBox(NULL, "Будет завтра", "ЕщО нет", MB_ICONEXCLAMATION);    
}
//---------------------------------------------------------------------------

