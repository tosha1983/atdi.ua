//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListBlockDAB.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListBlockDAB *frmListBlockDAB;
//---------------------------------------------------------------------------
__fastcall TfrmListBlockDAB::TfrmListBlockDAB(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListBlockDAB::TfrmListBlockDAB(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------

