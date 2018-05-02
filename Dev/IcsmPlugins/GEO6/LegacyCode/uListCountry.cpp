//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListCountry.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uBaseList"
#pragma resource "*.dfm"
TfrmListCountry *frmListCountry;
//---------------------------------------------------------------------------
__fastcall TfrmListCountry::TfrmListCountry(TComponent* Owner)
    : TfrmBaseList(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListCountry::TfrmListCountry(TComponent* Owner, HWND caller, int elementId)
    : TfrmBaseList(Owner, caller, elementId)
{
    for (int i = 0; i < dgrList->Columns->Count; i++) {
        TColumn *column = dgrList->Columns->Items[i];
        if (column->Field == dstListDEF_COLOR) {
            column->PickList->Clear();
            column->PickList->Add("SECAM");
            column->PickList->Add("PAL");
            column->PickList->Add("NTSC");
            return;
        }
    }
}
//---------------------------------------------------------------------------


