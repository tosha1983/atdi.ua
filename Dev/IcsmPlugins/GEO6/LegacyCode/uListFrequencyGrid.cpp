//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListFrequencyGrid.h"
#include "uNewChPlanDlg.h"
#include <memory>
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListFrequencyGrid *frmListFrequencyGrid;
//---------------------------------------------------------------------------
__fastcall TfrmListFrequencyGrid::TfrmListFrequencyGrid(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListFrequencyGrid::TfrmListFrequencyGrid(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmListFrequencyGrid::actListInsertExecute(TObject *Sender)
{
    std::auto_ptr<TdgCreateChannelPlan> dgCreateChannelPlan(new TdgCreateChannelPlan(Application));
    if (dgCreateChannelPlan->ShowModal() == mrOk)
        actRefreshExecute(Sender);
}
//---------------------------------------------------------------------------

