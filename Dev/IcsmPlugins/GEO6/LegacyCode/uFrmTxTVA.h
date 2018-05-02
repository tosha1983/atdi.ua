//---------------------------------------------------------------------------

#ifndef uFrmTxTVAH
#define uFrmTxTVAH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uFrmTxBaseAirAnalog.h"
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include <IBQuery.hpp>
#include <OleCtnrs.hpp>
#include <ActnList.hpp>
#include <math.h>
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
#include <vector>

//---------------------------------------------------------------------------
using namespace std;
class TfrmTxTVA : public TfrmTxBaseAirAnalog
{
__published:	// IDE-managed Components
    TPanel *pnlVideoOffsetHerz;
    TLabel *lblChannel;
    TComboBox *cbxChannel;
    TLabel *lblClassRadiation;
    TLabel *lblVideo;
    TLabel *lblAudio;
    TNumericEdit *edtFreqVideo;
    TNumericEdit *edtFreqAudio1;
    TLabel *lblFreq;
    TLabel *lblShift;
    TLabel *lblTip;
    TNumericEdit *edtEPRmaxVideo;
    TNumericEdit *edtEPRGVideo;
    TNumericEdit *edtEPRVVideo;
    TNumericEdit *edtPowerVideo;
    TLabel *Label2;
    TNumericEdit *edtFreqAudio2;
    TNumericEdit *edtEPRmaxAudio2;
    TNumericEdit *edtEPRGAudio2;
    TNumericEdit *edtEPRVAudio2;
    TNumericEdit *edtPowerAudio2;
    TComboBox *cbSystemcolor;
    TCheckBox *chbMonoStereo;
    TDataSource *dsTVA;
    TLabel *Label3;
    TComboBox *cbxVideoOffsetLine;
    TDBComboBox *dbcbTypeOffset;
    TLabel *Label4;
    TDBComboBox *cbxFreqStability;
    TNumericEdit *edtVSoundRatio1;
    TNumericEdit *edtVSoundRatio2;
    TLabel *lblVRatio;
    TIBQuery *ibqAnalogTeleSystemName;
    TLabel *lblSystemCast;
    TIBDataSet *ibdsTVA;
    TIBStringField *ibdsTVAVIDEO_EMISSION;
    TIBStringField *ibdsTVASOUND_EMISSION_PRIMARY;
    TIBStringField *ibdsTVASOUND_EMISSION_SECOND;
    TIBStringField *ibdsTVAFREQSTABILITY;
    TSmallintField *ibdsTVATYPESYSTEM;
    TIBStringField *ibdsTVATYPEOFFSET;
    TComboBox *cbxTypeSysName;
    TLabel *lblPowerVideo;
    TLabel *lblPowerSound1;
    TLabel *lblPowerSound2;
    TLabel *Label1;
    TComboBox *cbxVideoOffsetHerz;
    TButton *btnNullChannel;
    TEdit *edtClassRadiationVideo;
    TEdit *edtClassRadiationAudio1;
    TEdit *edtClassRadiationAudio2;
    TDBEdit *edtVideoEmission;
    TDBEdit *edtSoundEmissionPrimary;
    TDBEdit *edtSoundEmissionSecond;
    TBitBtn *btnVideoEmission;
    TBitBtn *btnSoundEmissionPrimary;
    TBitBtn *btnSoundEmissionSecond;
    void __fastcall chbMonoStereoClick(TObject *Sender);
    void __fastcall dbcbClassRadiationAudio2Exit(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall cbxTypeSysNameChange(TObject *Sender);
    void __fastcall cbSystemcolorChange(TObject *Sender);
    void __fastcall cbxChannelChange(TObject *Sender);
    void __fastcall btnTypeSystemClick(TObject *Sender);
    void __fastcall ibdsTVAAfterEdit(TDataSet *DataSet);
    void __fastcall ibdsTVATYPEOFFSETGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall ibdsTVATYPEOFFSETSetText(TField *Sender,
          const AnsiString Text);
    void __fastcall ibdsTVAFREQSTABILITYGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall ibdsTVAFREQSTABILITYSetText(TField *Sender,
          const AnsiString Text);
    void __fastcall cbxPolarizationChange(TObject *Sender);
    void __fastcall cbxVideoOffsetHerzChange(TObject *Sender);
    void __fastcall cbxVideoOffsetLineChange(TObject *Sender);
    void __fastcall actLoadExecute(TObject *Sender);
    void __fastcall btnNullChannelClick(TObject *Sender);
    void __fastcall edtEPRmaxVideoValueChange(TObject *Sender);
    void __fastcall edtEPRGVideoValueChange(TObject *Sender);
    void __fastcall edtEPRVVideoValueChange(TObject *Sender);
    void __fastcall edtPowerVideoValueChange(TObject *Sender);
    void __fastcall edtVSoundRatio1ValueChange(TObject *Sender);
        void __fastcall btnVideoEmissionClick(TObject *Sender);
        void __fastcall btnSoundEmissionPrimaryClick(TObject *Sender);
        void __fastcall btnSoundEmissionSecondClick(TObject *Sender);
private:	// User declarations
     bool mono_stereo_click;
public:		// User declarations
    __fastcall TfrmTxTVA(TComponent* Owner, ILISBCTx *in_Tx);
protected:
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);
    //  последние индекс выпадающих списков
    int offsetIdx;
    int colorIdx;
    int systemIdx;
    int channelIdx;

    virtual void __fastcall TxDataLoad();
    virtual  void __fastcall TxDataSave();
    virtual void __fastcall SetRadiationClass();
    virtual void _fastcall SetPowerInput(TPowerInput pi);
    virtual void _fastcall TxToForm();
    virtual void _fastcall FormToTx();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxTVA *frmTxTVA;
//---------------------------------------------------------------------------
#endif
