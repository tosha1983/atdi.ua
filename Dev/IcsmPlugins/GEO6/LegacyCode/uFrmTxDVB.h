//---------------------------------------------------------------------------

#ifndef uFrmTxDVBH
#define uFrmTxDVBH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uFrmTxBaseAirDigital.h"
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <math.h>
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
class TfrmTxDVB : public TfrmTxBaseAirDigital
{
__published:	// IDE-managed Components
    TPanel *pnlForDVB;
    TLabel *lblChannel;
    TNumericEdit *edtChannel;
    TLabel *lblFreqCentre;
    TNumericEdit *edtFreqCentre;
    TLabel *lblModulation;
    TComboBox *cbModulation;
    TLabel *lbCodeRate;
    TComboBox *cbCodeRate;
    TLabel *lbGiFftSize;
    TComboBox *cbGiFftSize;
    TComboBox *cbxTypeSysName;
    TLabel *lblSystemCast;
    TIBDataSet *ibdsDVB;
    TIntegerField *ibdsDVBID;
    TDataSource *dsDVB;
    TSmallintField *ibdsDVBGUARDINTERVAL_ID;
    TButton *btnChannel;
    TIBStringField *ibdsDVBNAMECHANNEL;
    TIntegerField *ibdsDVBCHANNEL_ID;
    TIBStringField *ibdsDVBNAMECURRIERGUARD;
    TSmallintField *ibdsDVBTYPESYSTEM;
    TButton *btnNullChannel;
    TIBQuery *ibqDigTSys;
    TIntegerField *ibqDigTSysID;
    TIBStringField *ibqDigTSysMODULATION;
    TIBStringField *ibqDigTSysCODERATE;
    TIBStringField *ibqDigTSysNAMESYSTEM;
    TIntegerField *ibqDigTSysENUMVAL;
    TDataSource *dsDigTSys;
    TLabel *lblClassRadiation;
    TEdit *edtClassRadiationVideo;
    TDBEdit *edtVideoEmission;
    TBitBtn *btnVideoEmission;
    TRadioButton *rbDvbt;
    TRadioButton *rbDvbt2;
    TLabel *lbPilotPattern;
    TComboBox *cbPilotPattern;
    TComboBox *cbDiversity;
    TLabel *lbDiversity;
    TCheckBox *chRotatedCnstls;
    TCheckBox *chModeOfExtnts;
    TPanel *pnDvbt;
    TPanel *pnDvbt2;
    TEdit *edDvbtSystemInfo;
    TEdit *edDvbtGiInfo;
    TComboBox *cbGuardInterval;
    TComboBox *cbFftSize;
    TLabel *lbGuardInterval;
    TLabel *lbFftSize;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall cbxTypeSysNameChange(TObject *Sender);
    void __fastcall cbGiFftSizeChange(TObject *Sender);
    void __fastcall btnChannelClick(TObject *Sender);
    void __fastcall ibdsDVBAfterEdit(TDataSet *DataSet);
    void __fastcall cbxTypeSysNameDropDown(TObject *Sender);
    void __fastcall btnNullChannelClick(TObject *Sender);
    void __fastcall btnVideoEmissionClick(TObject *Sender);
    void __fastcall cbxRpcChange(TObject *Sender);
    void __fastcall rbDvbtClick(TObject *Sender);
    void __fastcall rbDvbt2Click(TObject *Sender);
    void __fastcall cbPilotPatternChange(TObject *Sender);
    void __fastcall cbDiversityChange(TObject *Sender);
    void __fastcall chRotatedCnstlsClick(TObject *Sender);
    void __fastcall chModeOfExtntsClick(TObject *Sender);
    void __fastcall cbModulationChange(TObject *Sender);
    void __fastcall cbCodeRateChange(TObject *Sender);
    void __fastcall cbGuardIntervalChange(TObject *Sender);
    void __fastcall cbFftSizeChange(TObject *Sender);
private:	// User declarations
    int channel_id;
    int systemIdx;
    int giIdx;
public:		// User declarations
    __fastcall TfrmTxDVB(TComponent* Owner, ILISBCTx *in_Tx);
    ILisBcDvbt2* __fastcall GetDvbt2Intf() { return (ILisBcDvbt2 *)iDvbt2; }
    void InitControlsForDvbt();
    void InitControlsForDvbt2();
protected:
    ILisBcDvbt2Ptr iDvbt2;
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);
    virtual void __fastcall TxDataLoad();
    virtual  void __fastcall TfrmTxDVB::TxDataSave();
    virtual void __fastcall SetRadiationClass();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxDVB *frmTxDVB;
//---------------------------------------------------------------------------
#endif
