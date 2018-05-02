//---------------------------------------------------------------------------

#ifndef uFrmTxBaseAirH
#define uFrmTxBaseAirH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include "uFrmTxBase.h"
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <DB.hpp>
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
enum TPowerInput {piERP, piPOWER};

#define clDisabledEdit clBtnFace
#define clEnabledEdit clWindow



class TfrmTxBaseAir : public TfrmTxBase
{
__published:	// IDE-managed Components
        TPanel *pnlTech;
        TGroupBox *gbPower;
        TLabel *lblEBPmax;
        TLabel *lblEBPG;
        TLabel *lblEBPV;
        TLabel *lblPower;
        TGroupBox *gbAntenna;
        TLabel *lblDirect;
        TLabel *lblPolarization;
        TLabel *lblHeight;
        TLabel *lblAngle;
        TLabel *lblEmplif;
        TLabel *lblFiderLoss;
        TLabel *lblFiderLength;
    TNumericEdit *edtHeight;
    TNumericEdit *edtFiderLoss;
    TNumericEdit *edtFiderLength;
        TComboBox *cbxDirect;
        TComboBox *cbxPolarization;
    TNumericEdit *edtAngle;
    TNumericEdit *edtGain;
    TIBQuery *ibqTypeSystemName;
    TButton *btnAntPattH;
    TButton *btnEBPG1;
    TButton *btnEBPV1;
    TButton *btnHeff;
    TLabel *lblAbgl2;
    TNumericEdit *edtAngle2;
    TNumericEdit *edtPowerAudio1;
    TNumericEdit *edtEPRVAudio1;
    TNumericEdit *edtEPRGAudio1;
    TNumericEdit *edtEPRmaxAudio1;
    TNumericEdit *edtHeffMax;
    TLabel *lblHeffMax;
    TIBDataSet *ibdsAir;
    TDataSource *dsAir;
    TIBStringField *ibdsAirNUMLICENSE;
    TIBStringField *ibdsAirNUMPERMBUILD;
    TDateField *ibdsAirDATEPERMBUILDFROM;
    TDateField *ibdsAirDATEPERMBUILDTO;
    TIntegerField *ibdsAirLICENSE_RFR_ID;
    TIntegerField *ibdsAirID;
    TPanel *pnlSummator;
    TDBEdit *edtAzimuth;
    TSmallintField *ibdsAirSUMMATORPOWERS;
    TFloatField *ibdsAirSUMMATOFREQFROM;
    TFloatField *ibdsAirSUMMATORATTENUATION;
    TFloatField *ibdsAirSUMMATORFREQTO;
    TFloatField *ibdsAirSUMMATORMINFREQS;
    TFloatField *ibdsAirSUMMATORPOWERFROM;
    TFloatField *ibdsAirSUMMATORPOWERTO;
    TFloatField *ibdsAirAZIMUTHMAXRADIATION;
    TGroupBox *gbSummator;
    TDBEdit *edtSummFreqTo;
    TLabel *lblRange;
    TDBEdit *edtSummFreqFrom;
    TLabel *lblSummPow;
    TDBEdit *edtSummPowerFrom;
    TDBEdit *edtSummPowerTo;
    TLabel *lblMinFreq;
    TDBEdit *edtSumMinFreq;
    TLabel *lblLoss;
    TDBEdit *edtSummLoss;
    TCheckBox *chbSummator;
    TLabel *lblG;
    TPanel *pnlFreqShift;
    TLabel *lblSideLevel;
    TIntegerField *ibdsAirLEVELSIDERADIATION;
    TIntegerField *ibdsAirFREQSHIFT;
    TDBEdit *edtLevelSideRadiation;
    TLabel *lblFreqShift;
    TDBEdit *edtFreqShift;
    TBitBtn *btnCalcHeff;
    TButton *btnAntPattV;
        TCheckBox *CheckBoxERP;
        TCheckBox *CheckBoxPower;
    TLabel *lblAntDiscr;
        TStringField *ibdsStantionsBaseVIDEO_EMISSION;
        TStringField *ibdsStantionsBaseSOUND_EMISSION_PRIMARY;
        TStringField *ibdsStantionsBaseSOUND_EMISSION_SECOND;
    TLabel *lblSummAtten;
    TDBEdit *edtSummAtten;
    TDBEdit *edtCoord;
    TIBStringField *ibdsStantionsBaseCOORD;
    void __fastcall cbxDirectChange(TObject *Sender);
    void __fastcall cbxPolarizationChange(TObject *Sender);
    void __fastcall gbAntennaExit(TObject *Sender);
    void __fastcall btnEBPG1Click(TObject *Sender);
    void __fastcall btnEBPV1Click(TObject *Sender);
    void __fastcall btnHeffClick(TObject *Sender);
    void __fastcall ibdsAirAfterEdit(TDataSet *DataSet);
    void __fastcall chbSummatorClick(TObject *Sender);
    void __fastcall btnCalcHeffClick(TObject *Sender);
    void __fastcall btnTRKEnter(TObject *Sender);
    void __fastcall btnOperatorEnter(TObject *Sender);
    void __fastcall cbProgrammEnter(TObject *Sender);
    void __fastcall edtEPRmaxAudio1ValueChange(TObject *Sender);
    void __fastcall edtEPRGAudio1ValueChange(TObject *Sender);
    void __fastcall edtEPRVAudio1ValueChange(TObject *Sender);
    void __fastcall edtPowerAudio1ValueChange(TObject *Sender);
    void __fastcall edtHeightValueChange(TObject *Sender);
    void __fastcall edtHeffMaxValueChange(TObject *Sender);
    void __fastcall edtGainValueChange(TObject *Sender);
    void __fastcall edtAngle2ValueChange(TObject *Sender);
    void __fastcall edtAngleValueChange(TObject *Sender);
    void __fastcall edtFiderLossValueChange(TObject *Sender);
    void __fastcall edtFiderLengthValueChange(TObject *Sender);
        void __fastcall CheckBoxERPClick(TObject *Sender);
        void __fastcall CheckBoxPowerClick(TObject *Sender);
        void __fastcall FormCreate(TObject *Sender);
    void __fastcall ibdsAirSUMMATORATTENUATIONChange(TField *Sender);
    void __fastcall btnAntPattClick(TObject *Sender);
private:	// User declarations
    double data_36_old[36];
    double data_36[36];
    TPowerInput _power_input;
public:		// User declarations
    bool t36_flag_eprH;
    bool t36_flag_eprV;
    bool t36_flag_Heff;
    TForm *t36_flag_Gain_h;
    TForm *t36_flag_Discr_h;
    TForm *t36_flag_Gain_v;
    TForm *t36_flag_Discr_v;
    bool erp_cng;
    virtual void _fastcall FormToTx();
    virtual void _fastcall TxToForm();
    virtual void RefreshAll();
    const char* CalcVideoEmission();
    const char* CalcSoundEmission();
    void AdjustDirControls();
    __property TPowerInput PowerInput  = { read=_power_input, write=SetPowerInput };
    __fastcall TfrmTxBaseAir(TComponent* Owner, ILISBCTx *in_Tx);

protected:
    virtual void __fastcall TxDataLoad();
    virtual void __fastcall TxDataSave();

    void __fastcall acceptListElementSelection(Messages::TMessage &Message);
    virtual void _fastcall SetPowerInput(TPowerInput pi);
};
//---------------------------------------------------------------------------
#endif
