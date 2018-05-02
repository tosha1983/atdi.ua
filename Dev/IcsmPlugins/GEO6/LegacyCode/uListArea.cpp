//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListArea.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListArea *frmListArea;
//---------------------------------------------------------------------------
__fastcall TfrmListArea::TfrmListArea(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmListArea::TfrmListArea(bool MDIChild)
    : TfrmBaseListTree(MDIChild)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmListArea::TfrmListArea(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseListTree(Owner, callerWin, elementId)
{
}
//---------------------------------------------------------------------------
