//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "uReliefFrm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "uProfileView"
#pragma resource "*.dfm"
//---------------------------------------------------------------------------
__fastcall TfrmRelief::TfrmRelief(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmRelief::FormDeactivate(TObject *Sender)
{
    Close();    
}
//---------------------------------------------------------------------------



