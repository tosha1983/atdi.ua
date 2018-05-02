//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListCarrierGuardInterval.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListCarrierGuardInterval *frmListCarrierGuardInterval;
//---------------------------------------------------------------------------
__fastcall TfrmListCarrierGuardInterval::TfrmListCarrierGuardInterval(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListCarrierGuardInterval::TfrmListCarrierGuardInterval(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
