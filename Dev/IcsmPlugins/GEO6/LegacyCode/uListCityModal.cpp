//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uListCityModal.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uListCity"
#pragma resource "*.dfm"
TfrmListCityModal *frmListCityModal;
//---------------------------------------------------------------------------
__fastcall TfrmListCityModal::TfrmListCityModal(TComponent* Owner)
    : TfrmListCity(Owner)
{
}
//---------------------------------------------------------------------------
__fastcall TfrmListCityModal::TfrmListCityModal(TComponent* Owner, HWND caller, int elementId)
    : TfrmListCity(Owner, caller, elementId)
{
}

void __fastcall TfrmListCityModal::FormDestroy(TObject *Sender)
{
    TfrmListCity::FormDestroy(Sender);
    frmListCityModal = NULL;
}
//---------------------------------------------------------------------------

