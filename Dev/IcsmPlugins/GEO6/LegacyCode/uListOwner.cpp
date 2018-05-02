//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListOwner.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
#include "uFormOwner.h"
#include <memory>

//---------------------------------------------------------------------------
__fastcall TfrmListOwner::TfrmListOwner(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListOwner::TfrmListOwner(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmListOwner::dstListAfterOpen(TDataSet *DataSet)
{
    TColumn* col;
    for (int i = 0; i < dgrList->Columns->Count; i++)
        if ((col = dgrList->Columns->Items[i])->Field == dstListB_NAME) {
            col->ButtonStyle = cbsEllipsis;
            break;
        }
}
//---------------------------------------------------------------------------
void __fastcall TfrmListOwner::updateLookups()
{
    TfrmBaseList::updateLookups();
    TIBSQL* sql = new TIBSQL(this);
    try {
        sql->Database = dstList->Database;
        //  Регіон
        sql->SQL->Text = AnsiString("select NAME from BANK where ID = ") + IntToStr(dstListBANK_ID->AsInteger);
        sql->ExecQuery();
        if (!sql->Eof)
            dstListB_NAME->AsString = sql->Fields[0]->AsString;
        else
            dstListB_NAME->AsString = "";
        sql->Close();
    } __finally {
        sql->Free();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListOwner::dgrListDblClick(TObject *Sender)
{
    if (!m_caller)
        actListEditExecute(Sender);
    else
        TfrmBaseList::dgrListDblClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListOwner::actListEditExecute(TObject *Sender)
{
    TfrmFormTRK *frmTRK = new TfrmFormTRK(this);
    frmTRK->dstObj->ParamByName("ID")->AsInteger = dstListID->AsInteger;
    frmTRK->dstObj->Open();
    frmTRK->Show();
    //TfrmBaseList::dgrListDblClick(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TfrmListOwner::dstListBeforePost(TDataSet *DataSet)
{
    TfrmBaseList::dstListBeforePost(DataSet);
    if (!dstListNUMIDENTYCODACCOUNT->IsNull)
    {
        if (!dmMain->RunQuery("select id from OWNER where "+dstListNUMIDENTYCODACCOUNT->FieldName+" = '"+dstListNUMIDENTYCODACCOUNT->AsString+"' and ID <> "+dstListID->AsString))
            throw *(new Exception("Организация с таким ЕГРПОУ уже существует"));
    }
    if (dstListAVB->IsNull)
        dstListAVB->AsInteger = 0;
    if (dstListAAB->IsNull)
        dstListAAB->AsInteger = 0;
    if (dstListDVB->IsNull)
        dstListDVB->AsInteger = 0;
    if (dstListDAB->IsNull)
        dstListDAB->AsInteger = 0;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListOwner::dstListCalcFields(TDataSet *DataSet)
{
    String servList;
    if (dstListAVB->AsInteger)
        servList += "TV,";
    if (dstListAAB->AsInteger)
        servList += "FM,";
    if (dstListDVB->AsInteger)
        servList += "DVB,";
    if (dstListDAB->AsInteger)
        servList += "DAB,";

    if (servList.Length() > 0)
        servList.Delete(servList.Length(), 1);

    dstListSERV_LIST->AsString = servList;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListOwner::actListInsertExecute(TObject *Sender)
{
    TfrmFormTRK *frmTRK = new TfrmFormTRK(this);
    int newId = dmMain->getNewId();
    frmTRK->dstObj->ParamByName("ID")->AsInteger = newId;
    frmTRK->dstObj->Open();
    frmTRK->dstObj->Insert();
    frmTRK->dstObjID->AsInteger = newId;
    frmTRK->Show();
}
//---------------------------------------------------------------------------

