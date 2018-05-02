//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListAnalogTeleSystem.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListAnalogTeleSystem *frmListAnalogTeleSystem;
//---------------------------------------------------------------------------
__fastcall TfrmListAnalogTeleSystem::TfrmListAnalogTeleSystem(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListAnalogTeleSystem::TfrmListAnalogTeleSystem(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
