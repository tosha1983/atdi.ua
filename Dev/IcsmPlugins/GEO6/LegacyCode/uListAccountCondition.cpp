//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListAccountCondition.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListAccountCondition *frmListAccountCondition;
//---------------------------------------------------------------------------
__fastcall TfrmListAccountCondition::TfrmListAccountCondition(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListAccountCondition::TfrmListAccountCondition(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseListTree(Owner, callerWin, elementId)
{
}
//---------------------------------------------------------------------------

void __fastcall TfrmListAccountCondition::fillTree()
{
    trvList->Items->Clear();
    TTreeNode* n = trvList->Items->AddChildObject(NULL, "Внутришні", (void*)0);
    n->ImageIndex = 20;
    n->SelectedIndex = 21;
    n = trvList->Items->AddChildObject(NULL, "Міжнародні", (void*)1);
    n->ImageIndex = 20;
    n->SelectedIndex = 21;
}
