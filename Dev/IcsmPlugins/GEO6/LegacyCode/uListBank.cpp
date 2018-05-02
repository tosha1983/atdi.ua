//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListBank.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListBank *frmListBank;
//---------------------------------------------------------------------------
__fastcall TfrmListBank::TfrmListBank(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListBank::TfrmListBank(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmListBank::dstListBeforePost(TDataSet *DataSet)
{
    TfrmBaseList::dstListBeforePost(DataSet);
    if (!dstListMFO->IsNull)
    {
        if (!dmMain->RunQuery("select id from BANK where "+dstListMFO->FieldName+" = '"+dstListMFO->AsString+"' and ID <> "+dstListID->AsString))
            throw *(new Exception("Банк с таким МФО уже существует"));
    }
}
//---------------------------------------------------------------------------

