//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListEquipment.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListEquipment *frmListEquipment;
//---------------------------------------------------------------------------
__fastcall TfrmListEquipment::TfrmListEquipment(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListEquipment::TfrmListEquipment(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
