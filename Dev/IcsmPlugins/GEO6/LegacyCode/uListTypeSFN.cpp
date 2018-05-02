//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListTypeSFN.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListTypeSFN *frmListTypeSFN;
//---------------------------------------------------------------------------
__fastcall TfrmListTypeSFN::TfrmListTypeSFN(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmListTypeSFN::TfrmListTypeSFN(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}

