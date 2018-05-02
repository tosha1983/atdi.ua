//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListChannel.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListChannel *frmListChannel;
//---------------------------------------------------------------------------
__fastcall TfrmListChannel::TfrmListChannel(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListChannel::TfrmListChannel(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseListTree(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------

