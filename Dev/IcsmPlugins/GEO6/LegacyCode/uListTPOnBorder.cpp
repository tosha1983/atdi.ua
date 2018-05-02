//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListTPOnBorder.h"
#include "uMainDm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma resource "*.dfm"
TfrmListTPOnBorder *frmListTPOnBorder;
//---------------------------------------------------------------------------
__fastcall TfrmListTPOnBorder::TfrmListTPOnBorder(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListTPOnBorder::TfrmListTPOnBorder(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseListTree(Owner, caller, elementId)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTPOnBorder::dstListLATITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTPOnBorder::dstListLONGITUDEGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->IsNull)
        Text = "";
    else
        Text = dmMain->coordToStr(Sender->AsFloat, 'X');
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTPOnBorder::dstListLATITUDESetText(TField *Sender,
      const AnsiString Text)
{
    Sender->AsFloat = dmMain->strToCoord(Text);
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTPOnBorder::dstListLONGITUDESetText(TField *Sender,
      const AnsiString Text)
{
    Sender->AsFloat = dmMain->strToCoord(Text);
}
//---------------------------------------------------------------------------
void __fastcall TfrmListTPOnBorder::changeBranch(TTreeNode* node)
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
            case 0: dstList->SelectSQL->Strings[1] = "left join COUNTRYPOINTS CN on (CD.NUMBOUND = CN.NUMBOUND) where CN.COUNTRY_ID = :GRP_ID";
                break;
            case 1: dstList->SelectSQL->Strings[1] = "where NUMBOUND = :GRP_ID";
                break;
        }
        dstList->ParamByName("GRP_ID")->AsInteger = (int)node->Data;
        dstList->Open();
    }
}
//---------------------------------------------------------------------------

void __fastcall TfrmListTPOnBorder::actShowOnMapExecute(TObject *Sender)
{
    double prevLon = 0, prevLat = 0;
    //if (!frmMap) {
        //frmMain->actShowMapExecute(this);
        //Show();
    //}

    TBookmark bm = dstList->GetBookmark();
    dstList->DisableControls();
    try {
        dstList->First();
        while (!dstList->Eof) {
            //frmMap->DrawPoint(dstListLONGITUDE->AsFloat, dstListLATITUDE->AsFloat, 5, clTeal, "", "");
            //if (prevLon && prevLat)
                //frmMap->DrawLine(prevLon, prevLat, dstListLONGITUDE->AsFloat, dstListLATITUDE->AsFloat, 1, clTeal);
            prevLon = dstListLONGITUDE->AsFloat, prevLat = dstListLATITUDE->AsFloat;
            dstList->Next();
        }
        dstList->GotoBookmark(bm);
    } __finally {
        dstList->EnableControls();
        dstList->FreeBookmark(bm);
    }
}
//---------------------------------------------------------------------------

