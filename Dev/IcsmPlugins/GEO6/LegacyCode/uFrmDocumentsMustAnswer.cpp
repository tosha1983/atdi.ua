//---------------------------------------------------------------------------
      
#include <vcl.h>
#pragma hdrstop

#include "uFrmDocumentsMustAnswer.h"
#include "uMainDm.h"
#include "FormProvider.h"
#include "TxBroker.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"        
TfrmDocumentsMustAnswer *frmDocumentsMustAnswer;
//---------------------------------------------------------------------------
__fastcall TfrmDocumentsMustAnswer::TfrmDocumentsMustAnswer(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TfrmDocumentsMustAnswer::FormShow(TObject *Sender)
{
    ibqDocMustAnswer->Close();
    ibqDocMustAnswer->ParamByName("DATE")->AsDateTime = Now() + 3.0;
    ibqDocMustAnswer->Open();
}
//---------------------------------------------------------------------------
void __fastcall TfrmDocumentsMustAnswer::dbgDocMausAnswerDblClick(TObject *Sender)
{
    FormProvider.ShowTx(txBroker.GetTx(ibqDocMustAnswerTR_ID->AsInteger,
                                        dmMain->GetObjClsid(ibqDocMustAnswerENUMVAL->AsInteger)));
}
//---------------------------------------------------------------------------
void __fastcall TfrmDocumentsMustAnswer::FormClose(TObject *Sender,
      TCloseAction &Action)
{
    Action =  caFree;
}
//---------------------------------------------------------------------------
void __fastcall TfrmDocumentsMustAnswer::ibqDocMustAnswerTYPELETTERGetText(
      TField *Sender, AnsiString &Text, bool DisplayText)
{
    if (ibqDocMustAnswerTYPELETTER->AsString == "")
        ;
    else if (ibqDocMustAnswerTYPELETTER->AsInteger)
        Text = "вхідне";
    else
        Text = "вихідне";
}
//---------------------------------------------------------------------------
void __fastcall TfrmDocumentsMustAnswer::FormCreate(TObject *Sender)
{
    Height = 280;
    Width =  680;
}
//---------------------------------------------------------------------------

