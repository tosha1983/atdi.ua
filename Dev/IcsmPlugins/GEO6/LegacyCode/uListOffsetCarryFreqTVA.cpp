//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListOffsetCarryFreqTVA.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListOffsetCarryFreqTVA *frmListOffsetCarryFreqTVA;
//---------------------------------------------------------------------------
__fastcall TfrmListOffsetCarryFreqTVA::TfrmListOffsetCarryFreqTVA(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListOffsetCarryFreqTVA::TfrmListOffsetCarryFreqTVA(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
