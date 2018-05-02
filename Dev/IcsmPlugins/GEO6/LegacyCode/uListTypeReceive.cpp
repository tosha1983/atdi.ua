//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListTypeReceive.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListTypeReceive *frmListTypeReceive;
//---------------------------------------------------------------------------
__fastcall TfrmListTypeReceive::TfrmListTypeReceive(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListTypeReceive::TfrmListTypeReceive(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
