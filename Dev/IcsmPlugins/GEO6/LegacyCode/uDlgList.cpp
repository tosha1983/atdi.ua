//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uDlgList.h"
//--------------------------------------------------------------------- 
#pragma resource "*.dfm"
TdlgList *dlgList;
//---------------------------------------------------------------------
__fastcall TdlgList::TdlgList(TComponent* AOwner)
	: TForm(AOwner), lastIdx(-1)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgList::lbDblClick(TObject *Sender)
{
    btnOk->Click();
}
//---------------------------------------------------------------------------

void __fastcall TdlgList::btnOkClick(TObject *Sender)
{
    if (lb->ItemIndex == -1)
    {
        ModalResult = mrNone;
        throw *(new Exception("Выберите значение"));
    } else
        lastIdx = lb->ItemIndex;
}
//---------------------------------------------------------------------------

void __fastcall TdlgList::FormShow(TObject *Sender)
{
    if (lb->Items->Count > lastIdx)
        lb->ItemIndex = lastIdx;
}
//---------------------------------------------------------------------------

