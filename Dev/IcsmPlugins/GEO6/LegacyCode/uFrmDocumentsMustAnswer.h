//---------------------------------------------------------------------------

#ifndef uFrmDocumentsMustAnswerH
#define uFrmDocumentsMustAnswerH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <IBQuery.hpp>
//---------------------------------------------------------------------------
class TfrmDocumentsMustAnswer : public TForm
{
__published:	// IDE-managed Components
    TDBGrid *dbgDocMausAnswer;
    TDataSource *dsDocMustAnswer;
    TIBQuery *ibqDocMustAnswer;
    TSmallintField *ibqDocMustAnswerTYPELETTER;
    TIntegerField *ibqDocMustAnswerTELECOMORGANIZATION_ID;
    TDateField *ibqDocMustAnswerCREATEDATEIN;
    TIntegerField *ibqDocMustAnswerNUMIN;
    TDateField *ibqDocMustAnswerCREATEDATEOUT;
    TIntegerField *ibqDocMustAnswerNUMOUT;
    TDateField *ibqDocMustAnswerANSWERDATE;
    TSmallintField *ibqDocMustAnswerANSWERIS;
    TStringField *ibqDocMustAnswerACC_NAME;
    TIntegerField *ibqDocMustAnswerACCOUNTCONDITION_ID;
    TIBStringField *ibqDocMustAnswerADMINISTRATIONID;
    TIBStringField *ibqDocMustAnswerCODE;
    TIntegerField *ibqDocMustAnswerTR_ID;
    TIntegerField *ibqDocMustAnswerENUMVAL;
    void __fastcall FormShow(TObject *Sender);
    void __fastcall dbgDocMausAnswerDblClick(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall ibqDocMustAnswerTYPELETTERGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall FormCreate(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmDocumentsMustAnswer(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmDocumentsMustAnswer *frmDocumentsMustAnswer;
//---------------------------------------------------------------------------
#endif
