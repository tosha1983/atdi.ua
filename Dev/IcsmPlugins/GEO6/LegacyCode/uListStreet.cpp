//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListStreet.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListStreet *frmListStreet;
//---------------------------------------------------------------------------
__fastcall TfrmListStreet::TfrmListStreet(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListStreet::TfrmListStreet(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseListTree(Owner, callerWin, elementId)
{
}
//---------------------------------------------------------------------------
