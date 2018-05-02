//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListSystemCast.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListSystemCast *frmListSystemCast;
//---------------------------------------------------------------------------
__fastcall TfrmListSystemCast::TfrmListSystemCast(TComponent* Owner)
    : TfrmBaseList(Owner)
{
    dstListIS_USED->ValidChars = dstListIS_USED->ValidChars << ' ';
}
//---------------------------------------------------------------------------
__fastcall TfrmListSystemCast::TfrmListSystemCast(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
    dstListIS_USED->ValidChars = dstListIS_USED->ValidChars << ' ';
}
//---------------------------------------------------------------------------
void __fastcall TfrmListSystemCast::dstListTYPESYSTEMGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (Sender->AsInteger == 0)
        Text = 'À';
    else
        Text = 'Ö';
}
//---------------------------------------------------------------------------


void __fastcall TfrmListSystemCast::dstListIS_USEDGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    Text = Sender->AsInteger == 0 ? "" : "+";
}
//---------------------------------------------------------------------------

void __fastcall TfrmListSystemCast::dstListIS_USEDSetText(TField *Sender,
      const AnsiString Text)
{
    Sender->AsInteger = (Text.IsEmpty() || Text == "0" || (Text == " " && Sender->AsInteger == 1)) ? 0 : 1;
}
//---------------------------------------------------------------------------

