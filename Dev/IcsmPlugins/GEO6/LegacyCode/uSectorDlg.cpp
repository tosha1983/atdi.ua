//---------------------------------------------------------------------
#include <vcl.h>
#pragma hdrstop

#include "uSectorDlg.h"
//--------------------------------------------------------------------- 
#pragma link "CSPIN"
#pragma resource "*.dfm"
TdlgSector *dlgSector;
//---------------------------------------------------------------------
__fastcall TdlgSector::TdlgSector(TComponent* AOwner)
	: TForm(AOwner)
{
}
//---------------------------------------------------------------------
void __fastcall TdlgSector::FormShow(TObject *Sender)
{
    cseRadEnd->SetFocus();    
}
//---------------------------------------------------------------------------

void __fastcall TdlgSector::FormCreate(TObject *Sender)
{
    lblTxName->Font->Color = clNavy;
    lblTxName->Font->Style = lblTxName->Font->Style << fsBold;
}
//---------------------------------------------------------------------------

