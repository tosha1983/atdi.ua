//---------------------------------------------------------------------------

#ifndef uFrmTxBaseAirDigitalH
#define uFrmTxBaseAirDigitalH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uFrmTxBaseAir.h"
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <IBQuery.hpp>
#include <OleCtnrs.hpp>
#include <ActnList.hpp>
#include <Menus.hpp>
#include <ImgList.hpp>
#include "xlcClasses.hpp"
#include "xlEngine.hpp"
#include "xlReport.hpp"
#include <IBSQL.hpp>
#include "NumericEdit.hpp"
#include <ToolWin.hpp>
#include "CustomMap.h"
#include "uLisObjectGrid.h"
#include <IBDatabase.hpp>
//---------------------------------------------------------------------------
class TfrmTxBaseAirDigital : public TfrmTxBaseAir
{
__published:	// IDE-managed Components
        TPanel *pnlForDigital;
        TLabel *lblOChS;
    TDBEdit *edtSfn;
        TLabel *lblSynchr;
    TNumericEdit *edtSynchronization;
    TButton *btnSetSfn;
    TIBDataSet *ibdsTxDigital;
    TIntegerField *ibdsTxDigitalIDENTIFIERSFN;
    TIBStringField *ibdsTxDigitalSYNHRONETID;
    TDataSource *dsTxDigital;
    TIntegerField *ibdsTxDigitalID;
    TLabel *lblRpc;
    TLabel *lblRxType;
    TComboBox *cbxRpc;
    TComboBox *cbxRxMode;
    TLabel *lblIdRrc06;
    TDBEdit *edtRrc06;
    TIBStringField *ibdsTxDigitalADM_REF_ID;
    TDBComboBox *cbPlanEntry;
    TDBComboBox *cbAssgnCode;
    TDBEdit *edAssocAllotId;
    TDBEdit *edAssocSfnId;
    TDBEdit *edCallSign;
    TLabel *lblPlanEntry;
    TLabel *lblAssgnCode;
    TLabel *lblAssAllotId;
    TLabel *lblAssSfnId;
    TLabel *lblCallSign;
    TNumericEdit *edtOffset;
    TLabel *lblOffset;
    TIntegerField *ibdsTxDigitalPLAN_ENTRY;
    TIBStringField *ibdsTxDigitalASSGN_CODE;
    TIBStringField *ibdsTxDigitalASSOCIATED_ADM_ALLOT_ID;
    TIBStringField *ibdsTxDigitalASSOCIATED_ALLOT_SFN_ID;
    TIBStringField *ibdsTxDigitalCALL_SIGN;
    TButton *btnDropSfnId;
    TDBEdit *edAssgnCode;
    TButton *btAssocAllot;
    TPopupMenu *mnShowAllotment;
    TMenuItem *mniShowAllotment;
    TSpeedButton *sbExpToGs1Gt1;
    TNumericEdit *edPolIsol;
    TCheckBox *chPolIsol;
    TLabel *lblPolIsol;
    TLabel *lblSm;
    TDBComboBox *cbSm;
    TIBStringField *ibdsTxDigitalSPECT_MASK;
    void __fastcall btnSetSfnClick(TObject *Sender);
    void __fastcall ibdsTxDigitalAfterEdit(TDataSet *DataSet);
    void __fastcall edtSynchronizationEnter(TObject *Sender);
    void __fastcall edtSynchronizationValueChange(TObject *Sender);
    void __fastcall cbxRpcChange(TObject *Sender);
    void __fastcall cbxRxModeChange(TObject *Sender);
    void __fastcall edtOffsetValueChange(TObject *Sender);
    void __fastcall btnDropSfnIdClick(TObject *Sender);
    void __fastcall dsTxDigitalDataChange(TObject *Sender, TField *Field);
    void __fastcall ibdsTxDigitalSYNHRONETIDChange(TField *Sender);
    void __fastcall ibdsTxDigitalASSOCIATED_ALLOT_SFN_IDChange(
          TField *Sender);
    void __fastcall ibdsTxDigitalIDENTIFIERSFNChange(TField *Sender);
    void __fastcall ibdsTxDigitalASSOCIATED_ADM_ALLOT_IDChange(
          TField *Sender);
    void __fastcall ibdsTxDigitalASSGN_CODEChange(TField *Sender);
    void __fastcall cbAssgnCodeChange(TObject *Sender);
    void __fastcall btAssocAllotClick(TObject *Sender);
    void __fastcall mniShowAllotmentClick(TObject *Sender);
    void __fastcall sbExpToGs1Gt1Click(TObject *Sender);
    void __fastcall chPolIsolClick(TObject *Sender);
    void __fastcall edPolIsolValueChange(TObject *Sender);
    void __fastcall cbxPolarizationChange(TObject *Sender);
private:
    void __fastcall CheckSfn();
    void __fastcall CheckAssgnCode();	// User declarations
    void __fastcall SetPlanEntry();
public:		// User declarations
    __fastcall TfrmTxBaseAirDigital(TComponent* Owner, ILISBCTx *in_Tx);
protected:
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);

    virtual void __fastcall TxDataLoad();
    virtual void __fastcall TxDataSave();
    virtual void _fastcall TxToForm();
    virtual void _fastcall FormToTx();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxBaseAirDigital *frmTxBaseAirDigital;
//---------------------------------------------------------------------------
#endif
