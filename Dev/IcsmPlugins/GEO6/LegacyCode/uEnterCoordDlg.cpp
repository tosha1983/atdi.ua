//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uEnterCoordDlg.h"
#include "uMainDm.h"
//---------------------------------------------------------------------
#pragma resource "*.dfm"
TdlgEnterCoord *dlgEnterCoord;
//---------------------------------------------------------------------
__fastcall TdlgEnterCoord::TdlgEnterCoord(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgEnterCoord::edtLonExit(TObject *Sender)
{
    edtLon->Text = dmMain->coordToStr(dmMain->strToCoord(edtLon->Text), 'X');
}
//---------------------------------------------------------------------------

void __fastcall TdlgEnterCoord::edtLatExit(TObject *Sender)
{
    edtLat->Text = dmMain->coordToStr(dmMain->strToCoord(edtLat->Text), 'Y');
}
//---------------------------------------------------------------------------

