//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListCity.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListCity *frmListCity;
//---------------------------------------------------------------------------
__fastcall TfrmListCity::TfrmListCity(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListCity::TfrmListCity(TComponent* Owner, HWND callerWin, int elementId)
    : TfrmBaseListTree(Owner, callerWin, elementId)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmListCity::changeBranch(TTreeNode* node)
{
    dstList->Close();
    TIBXSQLVAR *p;
    if (node) {
        //  определим уровень - страна, регион, район
        int level = 0;
        TTreeNode* n = node;
        while (n = n->Parent)
            level++;
        switch (level) {
            case 0: dstList->SelectSQL->Strings[2] = "where COUNTRY_ID = :GRP_ID";
                break;
            case 1: dstList->SelectSQL->Strings[2] = "where AREA_ID = :GRP_ID";
                break;
            case 2: dstList->SelectSQL->Strings[2] = "where DISTRICT_ID = :GRP_ID";
                break;
        }
        dstList->ParamByName("GRP_ID")->AsInteger = (int)node->Data;
        dstList->Open();
    }
}

