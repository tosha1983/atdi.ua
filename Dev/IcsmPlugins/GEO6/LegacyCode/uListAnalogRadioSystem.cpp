//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListAnalogRadioSystem.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListAnalogRadioSystem *frmListAnalogRadioSystem;
//---------------------------------------------------------------------------
__fastcall TfrmListAnalogRadioSystem::TfrmListAnalogRadioSystem(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListAnalogRadioSystem::TfrmListAnalogRadioSystem(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
