//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uSelectStations.h"
//--------------------------------------------------------------------- 
#pragma resource "*.dfm"
TdgSelectStations *dgSelectStations;
//---------------------------------------------------------------------
__fastcall TdgSelectStations::TdgSelectStations(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdgSelectStations::btAllClick(TObject *Sender)
{
    for (int i = 0; i < lbList->Items->Count; i++)
        lbList->Checked[i] = true;
}
//---------------------------------------------------------------------------

void __fastcall TdgSelectStations::btNoneClick(TObject *Sender)
{
    for (int i = 0; i < lbList->Items->Count; i++)
        lbList->Checked[i] = false;
}
//---------------------------------------------------------------------------

