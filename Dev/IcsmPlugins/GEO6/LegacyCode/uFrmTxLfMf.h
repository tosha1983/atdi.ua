//---------------------------------------------------------------------------

#ifndef uFrmTxLfMfH
#define uFrmTxLfMfH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "CustomMap.h"
#include "NumericEdit.hpp"
#include "uFrmTxBase.h"
#include "uLisObjectGrid.h"
#include "xlcClasses.hpp"
#include "xlEngine.hpp"
#include "xlReport.hpp"
#include <ActnList.hpp>
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBCtrls.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <IBQuery.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <Mask.hpp>
#include <Menus.hpp>
#include <OleCtnrs.hpp>
#include "uFrmTxBase.h"
//---------------------------------------------------------------------------
class TfrmTxLfMf : public TfrmTxBase
{
__published:	// IDE-managed Components
    TPanel *pnEmission;
    TLabel *lbFreq;
    TNumericEdit *edFreq;
    TLabel *lbSystem;
    TComboBox *cbSys;
    TLabel *lbEmissionClass;
    TDBEdit *edEmissionClass;
    TBitBtn *btEmissionClass;
    TNumericEdit *edBw;
    TLabel *lbBw;
    TPanel *pnDayNight;
    TCheckBox *chDay;
    TCheckBox *chNight;
    TRadioButton *rbDay;
    TRadioButton *rbNight;
    TLabel *lbOp;
    TLabel *lbMode;
    TLabel *lbAdjRat;
    TNumericEdit *edAdjRat;
    TGroupBox *gbEmission;
    TLabel *lbGndCond;
    TNumericEdit *edGndCond;
    TLabel *lbNoiseZone;
    TNumericEdit *edNoiseZone;
    TNumericEdit *edGndCondCalc;
    TButton *btSetGndCond;
    TComboBox *cbModType;
    TComboBox *cbProtLevl;
    TLabel *lbAntType;
    TComboBox *cbAntType;
    TNumericEdit *edPwrKw;
    TNumericEdit *edMaxGainH;
    TNumericEdit *edtAgl;
    TNumericEdit *edEmrp;
    TLabel *lbPwrKw;
    TLabel *lbAzmMax;
    TDBEdit *edAzmMax;
    TButton *btGainH;
    TButton *btClearGainH;
    TDBEdit *edOpFrom;
    TDBEdit *edOpTo;
    TLabel *lbGainH;
    TLabel *lbEmrp;
    TLabel *lbAgl;
    TLabel *Label6;
    TLabel *Label7;
    TLabel *lbModType;
    TLabel *lbProtLevl;
    TDataSource *dsLfMfOper;
    TIBDataSet *dstLfMfOper;
    TComboBox *cbMonoStereo;
    TLabel *lbMonoStereo;
    TNumericEdit *edNoiseZoneCalc;
    TButton *btSetNoiseZone;
    TButton *btErp;
    TPanel *pnLfmf;
    TGroupBox *gbPower;
    TGroupBox *gbAntenna;
    TLabel *lblOChS;
    TDBEdit *edtSfn;
    TButton *btnSetSfn;
    TButton *btnDropSfnId;
    TLabel *lblSynchr;
    TNumericEdit *edSynchro;
    TPanel *pnSfn;
    TPanel *pnOpTm;
    TButton *btSetEmrp;
    TIBBCDField *dstLfMfOperSTART_TIME;
    TIBBCDField *dstLfMfOperSTOP_TIME;
    TFloatField *dstLfMfOperAZIMUTH;
    TIntegerField *dstLfMfOperSTA_ID;
    TIBStringField *dstLfMfOperDAYNIGHT;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall cbSysChange(TObject *Sender);
    void __fastcall edFreqValueChange(TObject *Sender);
    void __fastcall edBwValueChange(TObject *Sender);
    void __fastcall chDayClick(TObject *Sender);
    void __fastcall chNightClick(TObject *Sender);
    void __fastcall rbDayNightClick(TObject *Sender);
    void __fastcall cbAntTypeChange(TObject *Sender);
    void __fastcall edAdjRatValueChange(TObject *Sender);
    void __fastcall edNoiseZoneValueChange(TObject *Sender);
    void __fastcall edGndCondValueChange(TObject *Sender);
    void __fastcall cbModTypeChange(TObject *Sender);
    void __fastcall cbProtLevlChange(TObject *Sender);
    void __fastcall edPwrKwValueChange(TObject *Sender);
    void __fastcall btGainHClick(TObject *Sender);
    void __fastcall btClearGainHClick(TObject *Sender);
    void __fastcall edEmrpValueChange(TObject *Sender);
    void __fastcall edtAglValueChange(TObject *Sender);
    void __fastcall cbMonoStereoChange(TObject *Sender);
    void __fastcall btSetGndCondClick(TObject *Sender);
    void __fastcall btEmissionClassClick(TObject *Sender);
    void __fastcall btSetNoiseZoneClick(TObject *Sender);
    void __fastcall actOkUpdate(TObject *Sender);
    void __fastcall btErpClick(TObject *Sender);
    void __fastcall btnSetSfnClick(TObject *Sender);
    void __fastcall btnDropSfnIdClick(TObject *Sender);
    void __fastcall edSynchroValueChange(TObject *Sender);
    void __fastcall btSetEmrpClick(TObject *Sender);
    void __fastcall dstLfMfOpeTIMEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstLfMfOperTIMESetText(TField *Sender,
          const AnsiString Text);
private:	// User declarations
public:		// User declarations
    __fastcall TfrmTxLfMf(TComponent* Owner, ILISBCTx *in_Tx);
    virtual void __fastcall SetRadiationClass();
    typedef TfrmTxBase Inherited;
    ILisBcLfMfPtr lfmf;
    void __fastcall ShowOperData();
    void __fastcall CheckControls();
    void __fastcall ShowSfn();
protected:
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);

    virtual void __fastcall TxDataLoad();
    virtual void __fastcall TxDataSave();
    void __fastcall CheckDayNight();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxLfMf *frmTxLfMf;
//---------------------------------------------------------------------------
#endif
