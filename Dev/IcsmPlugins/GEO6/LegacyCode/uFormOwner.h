//---------------------------------------------------------------------------

#ifndef uFormOwnerH
#define uFormOwnerH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include "uListOwner.h"
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>

//---------------------------------------------------------------------------
class TfrmFormTRK : public TForm
{
__published:	// IDE-managed Components
    TLabel *Label1;
    TDBEdit *dbedtID;
    TLabel *Label2;
    TDBEdit *dbedtName;
    TLabel *Label3;
    TDBEdit *dbedtTelefon;
    TLabel *Label4;
    TDBEdit *dbedtFax;
    TLabel *Label5;
    TDBEdit *dbedtEDRPOU;
    TLabel *Label7;
    TDBEdit *dbedtBankName;
    TLabel *Label8;
    TDBEdit *dbedtNumTax;
    TLabel *Label9;
    TDBEdit *dbedtIPN;
    TLabel *Label10;
    TDBEdit *dbedtFin;
    TLabel *Label11;
    TDBEdit *dbedtUrAddress;
    TLabel *Label12;
    TDBEdit *dbedtFizAddress;
    TLabel *Label13;
    TDBEdit *dbedtNameBoss;
    TLabel *Label14;
    TDBEdit *dbedtEmale;
    TLabel *Label15;
    TDBEdit *dbedtNumRozr;
    TButton *btnSelectBank;
    TButton *btnOK;
    TDBCheckBox *servAtb;
    TDBCheckBox *servAab;
    TDBCheckBox *servDvb;
    TDBCheckBox *servDab;
    TButton *btmCancel;
    TGroupBox *gbxActivity;
    TIBDataSet *dstObj;
    TIntegerField *dstObjID;
    TIBStringField *dstObjNAMEORGANIZATION;
    TIBStringField *dstObjPHONE;
    TIBStringField *dstObjFAX;
    TStringField *dstObjSERV_LIST;
    TIBStringField *dstObjNUMIDENTYCODACCOUNT;
    TIntegerField *dstObjBANK_ID;
    TIBStringField *dstObjB_NAME;
    TIBStringField *dstObjNUMNDS;
    TIBStringField *dstObjNUMIDENTYCOD;
    TSmallintField *dstObjTYPEFINANCE;
    TIBStringField *dstObjADDRESSJURE;
    TIBStringField *dstObjADDRESSPHYSICAL;
    TIBStringField *dstObjNAMEBOSS;
    TIBStringField *dstObjMAIL;
    TIBStringField *dstObjNUMSETTLEMENTACCOUNT;
    TSmallintField *dstObjAVB;
    TSmallintField *dstObjAAB;
    TSmallintField *dstObjDVB;
    TSmallintField *dstObjDAB;
    TDataSource *dsrList;
    void __fastcall btnSelectBankClick(TObject *Sender);
    void __fastcall btnOKClick(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall btmCancelClick(TObject *Sender);
    void __fastcall dstObjBeforePost(TDataSet *DataSet);
private:	// User declarations
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, setElement)
    END_MESSAGE_MAP(TForm)
    void __fastcall setElement(Messages::TMessage &Message);
public:		// User declarations
    __fastcall TfrmFormTRK(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmFormTRK *frmFormTRK;
//---------------------------------------------------------------------------
#endif
