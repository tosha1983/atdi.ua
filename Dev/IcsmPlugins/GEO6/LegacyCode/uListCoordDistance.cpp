//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListCoordDistance.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListCoordDistance *frmListCoordDistance;
//---------------------------------------------------------------------------
__fastcall TfrmListCoordDistance::TfrmListCoordDistance(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListCoordDistance::TfrmListCoordDistance(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseListTree(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
