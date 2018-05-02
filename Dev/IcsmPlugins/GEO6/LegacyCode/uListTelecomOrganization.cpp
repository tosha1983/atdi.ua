//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListTelecomOrganization.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListTelecomOrganization *frmListTelecomOrganization;
//---------------------------------------------------------------------------
__fastcall TfrmListTelecomOrganization::TfrmListTelecomOrganization(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListTelecomOrganization::TfrmListTelecomOrganization(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTelecomOrganization::dstListCOORDDOCUMENTGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (dstListCOORDDOCUMENT->AsInteger)
        Text = "Так";
    else
        Text = "";
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTelecomOrganization::dstListCOORDDOCUMENTSetText(
      TField *Sender, const AnsiString Text)
{
    if (Text == "" || Text == "0")
        dstListCOORDDOCUMENT->AsInteger = 0;
    else
        dstListCOORDDOCUMENT->AsInteger = 1;
}
//---------------------------------------------------------------------------
