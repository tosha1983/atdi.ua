//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uImportDigLayer.h"
//--------------------------------------------------------------------- 
#pragma resource "*.dfm"
TdlgImportDigLayer *dlgImportDigLayer;
//---------------------------------------------------------------------
__fastcall TdlgImportDigLayer::TdlgImportDigLayer(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgImportDigLayer::OKBtnClick(TObject *Sender)
{
    bool checked = false;
    for (int i = 0; i < lbxLayers->Items->Count && !checked; i++)
        checked = lbxLayers->Checked[i];

    if (!checked)
    {
        ModalResult = mrNone;
        lbxLayers->SetFocus();
        throw *(new Exception("������� ���� � ���� ���� ��� �������"));
    }

    if (!rbDab->Checked && !rbDvb->Checked)
    {
        ModalResult = mrNone;
        rbDvb->SetFocus();
        throw *(new Exception("������ ��� ���������� ������� - DAB �� DVB"));
    }

    if (cbxCountry->ItemIndex == -1)
    {
        ModalResult = mrNone;
        cbxCountry->SetFocus();
        throw *(new Exception("������ �����"));
    }
}
//---------------------------------------------------------------------------

