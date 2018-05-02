//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListUncompatibleChannels.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListUncompatibleChannels *frmListUncompatibleChannels;
//---------------------------------------------------------------------------
__fastcall TfrmListUncompatibleChannels::TfrmListUncompatibleChannels(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListUncompatibleChannels::TfrmListUncompatibleChannels(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
