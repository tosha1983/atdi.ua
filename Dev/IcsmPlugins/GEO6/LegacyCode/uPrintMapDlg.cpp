//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uPrintMapDlg.h"
#include "uSelection.h"
#include "uMainForm.h"

//---------------------------------------------------------------------
#pragma resource "*.dfm"
//  блядская ATL
#define StrToInt(a) StrToInt(a)
TdlgPrintMap *dlgPrintMap;
//---------------------------------------------------------------------
__fastcall TdlgPrintMap::TdlgPrintMap(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgPrintMap::btnGetFileNameClick(TObject *Sender)
{
//    SaveDialog1->FileName = GetCurrentDir() + "\\default.bmp";
    SaveDialog1->Filter = "Файли BMP (*.bmp)|*.BMP";
    SaveDialog1->InitialDir = GetCurrentDir();
    if ( SaveDialog1->Execute() )
        edtFileName->Text = SaveDialog1->FileName;
}
//---------------------------------------------------------------------------


void __fastcall TdlgPrintMap::CancelBtnClick(TObject *Sender)
{
    //frmMap->Quality = 0;
    //frmMap->PrintFileName = "";
    Close();
}
//---------------------------------------------------------------------------

void __fastcall TdlgPrintMap::OKBtnClick(TObject *Sender)
{
    if (edtFileName->Text != "") {
        //frmMap->Quality = StrToInt(edtQuality->Text);
        //frmMap->PrintFileName = edtFileName->Text;
        Close();
    } else
        Application->MessageBox("Не введено ім'я файлу", Application->Title.c_str(), MB_ICONERROR | MB_OK);
}
//---------------------------------------------------------------------------

void __fastcall TdlgPrintMap::SaveDialog1SelectionChange(TObject *Sender)
{
    edtFileName->Text = SaveDialog1->FileName;
}
//---------------------------------------------------------------------------

void __fastcall TdlgPrintMap::FormShow(TObject *Sender)
{
    for (int i = 0; i < frmMain->MDIChildCount; i++) {
        TfrmSelection* frmSelection = dynamic_cast<TfrmSelection*>(frmMain->MDIChildren[i]);
        if (frmSelection) {
            // todo: do something
            //frmMap = frmSelection->frmMap;
            break;
        }
    }
}
//---------------------------------------------------------------------------

