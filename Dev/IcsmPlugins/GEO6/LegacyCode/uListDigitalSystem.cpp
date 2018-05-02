//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListDigitalSystem.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListDigitalSystem *frmListDigitalSystem;
//---------------------------------------------------------------------------
__fastcall TfrmListDigitalSystem::TfrmListDigitalSystem(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListDigitalSystem::TfrmListDigitalSystem(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
