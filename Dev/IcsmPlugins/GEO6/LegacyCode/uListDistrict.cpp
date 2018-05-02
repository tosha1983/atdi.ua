//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListDistrict.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListDistrict *frmListDistrict;
//---------------------------------------------------------------------------
__fastcall TfrmListDistrict::TfrmListDistrict(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListDistrict::TfrmListDistrict(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseListTree(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmListDistrict::changeBranch(TTreeNode* node)
{
    dstList->Close();
    TIBXSQLVAR *p;
    if (node) {
        //  определим уровень - страна, регион,
        int level = 0;
        TTreeNode* n = node;
        while (n = n->Parent)
            level++;
        switch (level) {
            case 0: dstList->SelectSQL->Strings[2] = "left join AREA A on (A.ID = D.AREA_ID) where A.COUNTRY_ID = :GRP_ID";
                break;
            case 1: dstList->SelectSQL->Strings[2] = "where AREA_ID = :GRP_ID";
                break;
        }
        dstList->ParamByName("GRP_ID")->AsInteger = (int)node->Data;
        dstList->Open();
    }
}
