//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uCheckPositionForm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "CustomMap"
#pragma resource "*.dfm"
TfrmCheckPosition *frmCheckPosition;
//---------------------------------------------------------------------------
__fastcall TfrmCheckPosition::TfrmCheckPosition(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmCheckPosition::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action = caFree;    
}
//---------------------------------------------------------------------------

