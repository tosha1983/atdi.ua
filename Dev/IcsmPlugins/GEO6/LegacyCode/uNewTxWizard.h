//---------------------------------------------------------------------------

#ifndef uNewTxWizardH
#define uNewTxWizardH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <IBSQL.hpp>
#include <DBGrids.hpp>
#include <Grids.hpp>
#include <DBCtrls.hpp>
#include <Mask.hpp>
#include "uMainDm.h"
#include <IBEvents.hpp>
#include "CSPIN.h"
//---------------------------------------------------------------------------
class TAntennaDiagramPanel;
class TfrmNewTxWizard : public TForm
{
__published:	// IDE-managed Components
    TPanel *panType;
    TButton *btnPrev;
    TButton *btnNext;
    TButton *btnFinish;
    TButton *Cancel;
    TRadioGroup *rgrSystemCast;
    TIBSQL *sqlSystemcast;
    TIBDataSet *dstStand;
    TPanel *panCoord;
    TEdit *edtLat;
    TEdit *edtLon;
    TLabel *Label1;
    TLabel *Label2;
    TIBSQL *sqlGetCS_ID;
    TPanel *panStand;
    TRadioButton *rbtnExistingStand;
    TDBGrid *grdExistingStand;
    TRadioButton *rbtnNewStand;
    TIntegerField *dstStandID;
    TFloatField *dstStandLATITUDE;
    TFloatField *dstStandLONGITUDE;
    TIBStringField *dstStandNAMESITE;
    TIntegerField *dstStandAREA_ID;
    TIBStringField *dstStandA_NAME;
    TIntegerField *dstStandDISTRICT_ID;
    TIBStringField *dstStandD_NAME;
    TIntegerField *dstStandCITY_ID;
    TIBStringField *dstStandC_NAME;
    TIntegerField *dstStandSTREET_ID;
    TIBStringField *dstStandST_NAME;
    TIBStringField *dstStandADDRESS;
    TCheckBox *chbCheckInCoord;
    TLabel *Label3;
    TLabel *Label4;
    TLabel *Label5;
    TLabel *Label7;
    TLabel *Label8;
    TPanel *panSystem;
    TComboBox *cbxSystem;
    TLabel *Label9;
    TComboBox *cbxChannel;
    TLabel *Label10;
    TEdit *edtVideoCarrier;
    TLabel *Label11;
    TComboBox *cbxAllotment;
    TLabel *Label12;
    TEdit *edtSoundCarrier;
    TLabel *Label13;
    TPanel *panTech;
    TEdit *edtFeederLoss;
    TEdit *edtPower;
    TEdit *edtAntHeight;
    TLabel *Label14;
    TLabel *Label15;
    TLabel *Label16;
    TDataSource *dsExistingStand;
    TEdit *edtNewLat;
    TEdit *edtNewLon;
    TEdit *edtNewStandAreaNum;
    TComboBox *cbxNewStandArea;
    TComboBox *cbxNewStandCity;
    TComboBox *cbxNewStandStreet;
    TLabel *Label6;
    TEdit *edtNewStandAddress;
    TEdit *edtNewStandName;
    TLabel *Label17;
    TIBSQL *sqlArea;
    TIBSQL *sqlCity;
    TIBSQL *sqlStreet;
    TIBSQL *sqlAreaByNum;
    TButton *btnCity;
    TIBSQL *sqlAreaByCity;
    TIBSQL *sqlAreaById;
    TIBSQL *sqlNewStreet;
    TIBSQL *sqlSystem;
    TIBSQL *sqlChannel;
    TIBSQL *sqlAllotment;
    TIBSQL *sqlVideoCarrier;
    TIBSQL *sqlSoundBlock;
    TEdit *edtBlock;
    TLabel *Label18;
    TIBSQL *sqlNewStand;
    TIBSQL *sqlNewTx;
    TLabel *Label19;
    TEdit *edtSiteHeight;
    TPanel *panDVBParams;
    TComboBox *cbxDVBSystem;
    TComboBox *cbxDVBParams;
    TLabel *Label20;
    TLabel *Label21;
    TEdit *edtAntennaGain;
    TEdit *edtVideoSoundRatio;
    TLabel *Label22;
    TLabel *Label23;
    TLabel *lblID;
    TMaskEdit *edtAdminId;
    TIBSQL *sqlNewAdminId;
    TCheckBox *chbGenerateID;
    TButton *btnGetHeight;
    TIBSQL *sqlRadioSystem;
    TLabel *lblPolarization;
    TComboBox *cbxPolarization;
    TLabel *Label24;
    TEdit *edtFeederLen;
    TLabel *lblDirect;
    TComboBox *cbxDirect;
    TLabel *Label25;
    TStringGrid *sgrAntennaGain;
    TPanel *panAntGainGraph;
    TComboBox *cbxChannelGrid;
    TLabel *Label26;
    TIBSQL *sqlChannelGrid;
    TLabel *Label27;
    TIBStringField *dstStandA_NUMREGION;
    TIntegerField *dstStandS_HEIGHT_SEA;
    TCSpinEdit *seRadius;
    TLabel *lblRadius;
    TButton *btnRadius;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall CancelClick(TObject *Sender);
    void __fastcall FormCloseQuery(TObject *Sender, bool &CanClose);
    void __fastcall btnFinishClick(TObject *Sender);
    void __fastcall edtLatExit(TObject *Sender);
    void __fastcall edtLonExit(TObject *Sender);
    void __fastcall btnNextClick(TObject *Sender);
    void __fastcall btnPrevClick(TObject *Sender);
    void __fastcall grdExistingStandCellClick(TColumn *Column);
    void __fastcall dstStandLATITUDEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstStandLONGITUDEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall edtNewStandAreaNumExit(TObject *Sender);
    void __fastcall edtNewStandAreaNumKeyPress(TObject *Sender, char &Key);
    void __fastcall rbtnExistingStandClick(TObject *Sender);
    void __fastcall rbtnNewStandClick(TObject *Sender);
    void __fastcall edtNewLatExit(TObject *Sender);
    void __fastcall edtNewLonExit(TObject *Sender);
    void __fastcall edtNewLonKeyPress(TObject *Sender, char &Key);
    void __fastcall edtNewLatKeyPress(TObject *Sender, char &Key);
    void __fastcall cbxNewStandAreaChange(TObject *Sender);
    void __fastcall cbxNewStandCityChange(TObject *Sender);
    void __fastcall btnCityClick(TObject *Sender);
    void __fastcall cbxSystemChange(TObject *Sender);
    void __fastcall cbxChannelChange(TObject *Sender);
    void __fastcall cbxAllotmentChange(TObject *Sender);
    void __fastcall edtSoundCarrierExit(TObject *Sender);
    void __fastcall edtAntHeightExit(TObject *Sender);
    void __fastcall edtFeederLossExit(TObject *Sender);
    void __fastcall edtPowerExit(TObject *Sender);
    void __fastcall edtSiteHeightExit(TObject *Sender);
    void __fastcall edtAdminIdExit(TObject *Sender);
    void __fastcall chbGenerateIDClick(TObject *Sender);
    void __fastcall btnGetHeightClick(TObject *Sender);
    void __fastcall cbxDirectChange(TObject *Sender);
    void __fastcall cbxChannelGridChange(TObject *Sender);
    void __fastcall btnResetChannelClick(TObject *Sender);
    void __fastcall sgrAntennaGainSetEditText(TObject *Sender, int ACol,
          int ARow, const AnsiString Value);
    void __fastcall grdExistingStandEnter(TObject *Sender);
    void __fastcall dstStandAfterScroll(TDataSet *DataSet);
    void __fastcall cbxNewStandStreetChange(TObject *Sender);
    void __fastcall edtNewStandAddressExit(TObject *Sender);
    void __fastcall btnRadiusClick(TObject *Sender);
private:	// User declarations
    int systemcast, systemcast_id, system, block, channel, allotment_block;
    AnsiString sStandName, sNumRegion, sAddress;
    int numcbxRegion, numcbxCity, numcbxStreet;
    double lat, lon;
    double standLat, standLon;
    int stand, area, district, city, street;
    AnsiString address;
    double freq_video, freq_sound, freq_sound_sec;
    int sfn_no;
    int antenna_height;
    int h_eff_max;
    double power_video;
    double erp_video_max;
    double antenna_gain;
    int height_sea;
    double diagram[36];

    bool cancelled;
public:		// User declarations
    int new_id;
    __fastcall TfrmNewTxWizard(TComponent* Owner);
protected:
    TAntennaDiagramPanel *antennaDiagramPanel;
    int __fastcall findAreaByNum();
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, elementSelected)
    END_MESSAGE_MAP(TForm)
    void elementSelected(Messages::TMessage& msg);
    void __fastcall fillCombo(TComboBox* cbx, TIBSQL* sql, int parentId, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmNewTxWizard *frmNewTxWizard;


class TAntennaDiagramPanel : public TPanel {
private:
    double coverageZone[36];
//    double noiseLimitedZone[36];
//    double interfereLimitedZone[36];
    double norma; // maximum value
protected:
	virtual void __fastcall Paint(void);
    void __fastcall findNorma();
    void __fastcall drawZone(double* zone, TColor color = clBlack, int lineWeight = 1, int numOfPoints = 36);
    inline __fastcall int screenValue(double value) { return value/norma*(Width/2); };
public:
    __fastcall TAntennaDiagramPanel(TComponent* AOwner) : TPanel(AOwner) { clear(); };
    void __fastcall clear(void);
    void __fastcall setCoverage(double*);
};
//---------------------------------------------------------------------------


//---------------------------------------------------------------------------
#endif
