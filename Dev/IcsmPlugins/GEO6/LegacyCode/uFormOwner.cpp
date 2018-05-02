//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uFormOwner.h"
#include "uMainDm.h"
#include "FormProvider.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TfrmFormTRK *frmFormTRK;
//---------------------------------------------------------------------------
__fastcall TfrmFormTRK::TfrmFormTRK(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmFormTRK::btnSelectBankClick(TObject *Sender)
{
    FormProvider.ShowList(6, this->Handle, dstObjBANK_ID->AsInteger);
}

void __fastcall TfrmFormTRK::setElement(Messages::TMessage &Message)
{
    dstObj->Edit();
    dstObjBANK_ID->AsInteger = Message.LParam;
    TIBSQL* sql = new TIBSQL(this);
    try {
        sql->Database = dmMain->dbMain;
        //  Регіон
        sql->SQL->Text = AnsiString("select NAME from BANK where ID = ") + IntToStr(dstObjBANK_ID->AsInteger);
        sql->ExecQuery();
        dstObjB_NAME->AsString = sql->Fields[0]->AsString;
        sql->Close();
    } __finally {
        sql->Free();
    }
}

//---------------------------------------------------------------------------

void __fastcall TfrmFormTRK::btnOKClick(TObject *Sender)
{
    if (dstObj->State == dsEdit || dstObj->State == dsInsert)
    {
        dstObj->Post();
        dstObj->Transaction->CommitRetaining();
    }
    Close();
    TForm *mainForm = Application->MainForm;
    TfrmListOwner* listOwner;
    for (int i = 0; i < mainForm->MDIChildCount; i++)
        if (listOwner = dynamic_cast<TfrmListOwner*>(mainForm->MDIChildren[i]))
        {
            int id = listOwner->dstListID->AsInteger;
            listOwner->dstList->Close();
            listOwner->dstList->Transaction->CommitRetaining();
            listOwner->dstList->Open();
            listOwner->dstList->Locate("ID", id, TLocateOptions());
        }
}
//---------------------------------------------------------------------------


void __fastcall TfrmFormTRK::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;
}
//---------------------------------------------------------------------------

void __fastcall TfrmFormTRK::btmCancelClick(TObject *Sender)
{
    if (dstObj->State == dsEdit || dstObj->State == dsInsert)
        dstObj->Cancel();
    Close();
}
//---------------------------------------------------------------------------

void __fastcall TfrmFormTRK::dstObjBeforePost(TDataSet *DataSet)
{
    if (!dstObjNUMIDENTYCODACCOUNT->IsNull)
    {
        if (!dmMain->RunQuery("select id from OWNER where "+dstObjNUMIDENTYCODACCOUNT->FieldName+" = '"+dstObjNUMIDENTYCODACCOUNT->AsString+"' and ID <> "+dstObjID->AsString))
            throw *(new Exception("Организация с таким ЕГРПОУ уже существует"));
    }
    if (dstObjAVB->IsNull)
        dstObjAVB->AsInteger = 0;
    if (dstObjAAB->IsNull)
        dstObjAAB->AsInteger = 0;
    if (dstObjDVB->IsNull)
        dstObjDVB->AsInteger = 0;
    if (dstObjDAB->IsNull)
        dstObjDAB->AsInteger = 0;
}
//---------------------------------------------------------------------------

