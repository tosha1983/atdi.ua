//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uOffsetRangeDlg.h"
//---------------------------------------------------------------------
#pragma resource "*.dfm"
TOffsetRangeDlg *OffsetRangeDlg;
//---------------------------------------------------------------------
__fastcall TOffsetRangeDlg::TOffsetRangeDlg(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TOffsetRangeDlg::OKBtnClick(TObject *Sender)
{
    try {
        edtDownRange->Text.ToInt();
    } catch (Exception &e) {
        edtDownRange->SetFocus();
        ModalResult = mrNone;
        e.Message = AnsiString("Помилка визначення нижньої межі: ") + e.Message;
        Application->ShowException(&e);
    }
    try {
        edtUpRange->Text.ToInt();
    } catch (Exception &e) {
        edtUpRange->SetFocus();
        ModalResult = mrNone;
        e.Message = AnsiString("Помилка визначення верхньої межі: ") + e.Message;
        Application->ShowException(&e);
    }
}
//---------------------------------------------------------------------------


