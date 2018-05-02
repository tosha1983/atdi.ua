//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListLicense.h"
#include "uMainDm.h"
#include "FormProvider.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseListTree"
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListLicense *frmListLicense;
//---------------------------------------------------------------------------
__fastcall TfrmListLicense::TfrmListLicense(TComponent* Owner)
    : TfrmBaseListTree(Owner)
{
}
//---------------------------------------------------------------------------

__fastcall TfrmListLicense::TfrmListLicense(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseListTree(Owner, caller, elementId)
{
    TColumn* col;
    for (int i = 0; i < dgrList->Columns->Count; i++)
        if ((col = dgrList->Columns->Items[i])->Field == dstListNAMEORGANIZATION) {
            col->ButtonStyle = cbsEllipsis;
            break;
        }
    dstListOWNER_ID->Tag = otOWNER;
    dstListNAMEORGANIZATION->Tag = dstListOWNER_ID->Index; 
}
//---------------------------------------------------------------------------

void __fastcall TfrmListLicense::fillTree()
{
    trvList->Items->Clear();
    TTreeNode* n = trvList->Items->AddChildObject(NULL, "ÊÌ (ÍÐ)", (void*)0);
    n->ImageIndex = 20;
    n->SelectedIndex = 21;
    n = trvList->Items->AddChildObject(NULL, "Ð×Ð", (void*)1);
    n->ImageIndex = 20;
    n->SelectedIndex = 21;
    n = trvList->Items->AddChildObject(NULL, "ÂÄ", (void*)2);
    n->ImageIndex = 20;
    n->SelectedIndex = 21;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListLicense::dstListBeforePost(TDataSet *DataSet)
{
    TfrmBaseListTree::dstListBeforePost(DataSet);
    if (dstListANNUL->IsNull)
        dstListANNUL->AsInteger = 0;
}
//---------------------------------------------------------------------------

void __fastcall TfrmListLicense::dstListANNULGetText(TField *Sender,
      AnsiString &Text, bool DisplayText)
{
    if (Sender->AsInteger == 0)
        Text = "";
    else
        Text = "Äà";
}
//---------------------------------------------------------------------------

void __fastcall TfrmListLicense::dstListANNULSetText(TField *Sender,
      const AnsiString Text)
{
    if (Text == "" || Text == "0")
        Sender->AsInteger = 0;
    else
        Sender->AsInteger = 1;
}
//---------------------------------------------------------------------------

