//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListRadioService.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListRadioService *frmListRadioService;
//---------------------------------------------------------------------------
__fastcall TfrmListRadioService::TfrmListRadioService(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListRadioService::TfrmListRadioService(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
