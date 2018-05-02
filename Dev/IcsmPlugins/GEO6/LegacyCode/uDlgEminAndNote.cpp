//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uDlgEminAndNote.h"
//--------------------------------------------------------------------- 
#pragma resource "*.dfm"
TdlgEminAndNote *dlgEminAndNote;
//---------------------------------------------------------------------
__fastcall TdlgEminAndNote::TdlgEminAndNote(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgEminAndNote::btnOkClick(TObject *Sender)
{
    StrToFloat(edtEmin->Text);
    ModalResult = mrOk;    
}
//---------------------------------------------------------------------------

