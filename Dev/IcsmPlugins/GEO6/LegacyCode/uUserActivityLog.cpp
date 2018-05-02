//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uUserActivityLog.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmUserActivityLog *frmUserActivityLog;
//---------------------------------------------------------------------------
__fastcall TfrmUserActivityLog::TfrmUserActivityLog(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmUserActivityLog::TfrmUserActivityLog(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------


