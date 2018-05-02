//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListMinFieldStrength.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListMinFieldStrength *frmListMinFieldStrength;
//---------------------------------------------------------------------------
__fastcall TfrmListMinFieldStrength::TfrmListMinFieldStrength(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListMinFieldStrength::TfrmListMinFieldStrength(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseListTree(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
