//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListSynhroNet.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListSynhroNet *frmListSynhroNet;
//---------------------------------------------------------------------------
__fastcall TfrmListSynhroNet::TfrmListSynhroNet(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmListSynhroNet::TfrmListSynhroNet(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}

