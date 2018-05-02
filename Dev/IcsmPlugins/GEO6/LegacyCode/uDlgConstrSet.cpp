//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uDlgConstrSet.h"
//---------------------------------------------------------------------
#pragma resource "*.dfm"
TdlgConstrSet *dlgConstrSet;
//---------------------------------------------------------------------
__fastcall TdlgConstrSet::TdlgConstrSet(TComponent* AOwner)
	: TForm(AOwner),
    minLon(-MaxDouble),
    minLat(-MaxDouble),
    maxLon(MaxDouble),
    maxLat(MaxDouble)

{
    cc = new TCoordinateConvertor(this);
}
//---------------------------------------------------------------------------

void __fastcall TdlgConstrSet::FormShow(TObject *Sender)
{
    ShowValues();
}
//---------------------------------------------------------------------

void __fastcall TdlgConstrSet::edtMinLatExit(TObject *Sender)
{
    GetValues();
    ShowValues();
}
//---------------------------------------------------------------------------

void __fastcall TdlgConstrSet::edtMinLonExit(TObject *Sender)
{
    GetValues();
    ShowValues();
}
//---------------------------------------------------------------------------

void __fastcall TdlgConstrSet::edtMaxLatExit(TObject *Sender)
{
    GetValues();
    ShowValues();
}
//---------------------------------------------------------------------------

void __fastcall TdlgConstrSet::edtMaxLonExit(TObject *Sender)
{
    GetValues();
    ShowValues();
}
//---------------------------------------------------------------------------

void __fastcall TdlgConstrSet::ShowValues()
{
    edtMinLat->Text = minLat > -MaxDouble ? cc->CoordToStr(minLat, 'Y') : AnsiString();
    edtMinLon->Text = minLon > -MaxDouble ? cc->CoordToStr(minLon, 'X') : AnsiString();
    edtMaxLat->Text = maxLat < MaxDouble ? cc->CoordToStr(maxLat, 'Y') : AnsiString();
    edtMaxLon->Text = maxLon < MaxDouble ? cc->CoordToStr(maxLon, 'X') : AnsiString();
}

void __fastcall TdlgConstrSet::GetValues()
{
    maxLon = !edtMaxLon->Text.IsEmpty() ? cc->StrToCoord(edtMaxLon->Text) : MaxDouble;
    maxLat = !edtMaxLat->Text.IsEmpty() ? cc->StrToCoord(edtMaxLat->Text) : MaxDouble;
    minLon = !edtMinLon->Text.IsEmpty() ? cc->StrToCoord(edtMinLon->Text) : -MaxDouble;
    minLat = !edtMinLat->Text.IsEmpty() ? cc->StrToCoord(edtMinLat->Text) : -MaxDouble;
}
