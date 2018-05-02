//----------------------------------------------------------------------------
#ifndef uCalcParamsDlgH
#define uCalcParamsDlgH
//----------------------------------------------------------------------------
#include <vcl\System.hpp>
#include <vcl\Windows.hpp>
#include <vcl\SysUtils.hpp>
#include <vcl\Classes.hpp>
#include <vcl\Graphics.hpp>
#include <vcl\StdCtrls.hpp>
#include <vcl\Forms.hpp>
#include <vcl\Controls.hpp>
#include <vcl\Buttons.hpp>
#include <vcl\ExtCtrls.hpp>
#include <Dialogs.hpp>
#include <ComCtrls.hpp>
#include <IBSQL.hpp>
#include "CSPIN.h"
#include "NumericEdit.hpp"
#include "uParams.h"


//----------------------------------------------------------------------------
class TdlgCalcParams : public TForm
{
__published:
    TTabSheet *tshCalcParams;
    TGroupBox *gbxPropagModel;
    TComboBox *cbxPropagServer;
    TGroupBox *gbxReliefModel;
    TComboBox *cbxReliefServer;
    TLabel *Label1;
    TEdit *edtPathData;
    TButton *btnPath;
    TGroupBox *gbxPathParams;
    TCheckBox *chbHeff;
    TCheckBox *chbTxClearance;
    TCheckBox *chbRxClearance;
    TCheckBox *chbMorphology;
    TNumericEdit *edtStep;
    TLabel *Label2;
    TGroupBox *gbxCalcServer;
    TComboBox *cbxCalcServer;
    TButton *btnOk;
    TButton *btnCancel;
    TGroupBox *gbxCalcLog;
    TEdit *edtCalcLog;
    TButton *btnCalcLog;
    TOpenDialog *OpenDialog1;
    TRadioGroup *rgrEminMethod;
    TNumericEdit *edtMinSelInterf;
    TNumericEdit *edtHigherIntNum;
    TLabel *lblHigherIntNum;
    TPageControl *pcParams;
    TTabSheet *tshNewTx;
    TTabSheet *tshExpert;
    TCSpinEdit *seRadius;
    TLabel *lblRadius;
    TLabel *Label5;
    TEdit *edtNewStandAreaNum;
    TComboBox *cbxNewStandArea;
    TLabel *Label7;
    TComboBox *cbxNewStandCity;
    TIBSQL *sqlArea;
    TIBSQL *sqlCity;
    TCheckBox *chbMapAutoFit;
    TCheckBox *chbDuelAutoRecalc;
    TNumericEdit *edtFilesNum;
    TLabel *lblFilesNum;
    TCheckBox *chbGetCoordinatesFromBase;
    TCheckBox *cbxDisableReliefAtPlanning;
    TLabel *Label4;
    TLabel *Label6;
    TLabel *Label8;
    TCheckBox *cbxQuick_calc_duel_interf;
    TCheckBox *cbxQuick_calc_max_dist;
    TButton *btCoordDistLoc;
    TNumericEdit *edtEmin_dvb_200;
    TNumericEdit *edtEmin_dvb_500;
    TNumericEdit *edtEmin_dvb_700;
    TEdit *edtCoord_dist_ini_file;
    TTabSheet *tshCommon;
    TCheckBox *chbQueryOnMainormClose;
    TCheckBox *chbEarthCurveInRelief;
    TLabel *Label9;
    TTabSheet *tshLisBcCalc;
    TCheckBox *chbSelectionAutotruncation;
    TNumericEdit *edtBackLobeFmMono;
    TNumericEdit *edtBackLobeFmStereo;
    TNumericEdit *edtBackLobeTvBand2;
    TNumericEdit *edtPolarCorrectFm;
    TLabel *Label10;
    TLabel *Label11;
    TLabel *Label12;
    TLabel *Label13;
    TLabel *Label14;
    TComboBox *cbxDegreeStep;
    TCheckBox *chbShowCp;
    TNumericEdit *edtTreshVideo;
    TNumericEdit *edtTreshAudio;
    TLabel *Label15;
    TLabel *Label16;
    TCheckBox *chbTvSoundStereo;
    TLabel *Label17;
    TNumericEdit *edtStepCalcMaxDist;
    TCheckBox *chbShowTxNames;
    TNumericEdit *lineThicknessZoneCover;
    TNumericEdit *lineThicknessZoneNoise;
    TNumericEdit *lineThicknessZoneInterfere;
    TLabel *Label18;
    TLabel *Label19;
    TLabel *Label20;
    TColorDialog *colorDialog;
    TLabel *Label21;
    TLabel *Label22;
    TLabel *Label23;
    TLabel *Label24;
    TLabel *Label25;
    TColorBox *cbxLineColorZoneCover;
    TColorBox *cbxLineColorZoneNoise;
    TColorBox *cbxLineColorZoneInterfere;
    TColorBox *cbxCoordinationPointsInZoneColor;
    TColorBox *cbxCoordinationPointsOutZoneColor;
    TColorBox *cbxChangedTxColor;
    TLabel *Label26;
    TCheckBox *chbRequestForCoordDist;
    TColorBox *cbxLineColorZoneInterfere2;
    TLabel *Label27;
    TGroupBox *gbxPathTheo;
    TLabel *Label28;
    TCheckBox *chbHeffTheo;
    TCheckBox *chbTxClearanceTheo;
    TCheckBox *chbRxClearanceTheo;
    TCheckBox *chbMorphologyTheo;
    TNumericEdit *edtStepTheo;
    TCheckBox *chbTheoPathTheSame;
    TButton *btCalcAdd;
    TButton *btCalcRmv;
    TButton *btPpgAdd;
    TButton *btPpgRmv;
    TButton *btDtmAdd;
    TButton *btDtmRmv;
    TCheckBox *chRpcRxModeLink;
    TRadioGroup *rgDuelType;
    TButton *btCalcEdt;
    TButton *btPrgEdt;
    TButton *btDtmEdt;
    TGroupBox *gbxMapXProblems;
    TCheckBox *cbMapInitDelay;
    TCheckBox *cbMapInitInfo;
    TNumericEdit *edtMapInitDelay;
    TLabel *lbMapInitDelay;
    void __fastcall btnOkClick(TObject *Sender);
    void __fastcall btnPathClick(TObject *Sender);
    void __fastcall btnCancelClick(TObject *Sender);
    void __fastcall cbxReliefServerChange(TObject *Sender);
    void __fastcall edtStepExit(TObject *Sender);
    void __fastcall btnCalcLogClick(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall cbxNewStandAreaChange(TObject *Sender);
    void __fastcall edtFilesNumExit(TObject *Sender);
    void __fastcall btnDistPathClick(TObject *Sender);
    void __fastcall chbSelectionAutotruncationClick(TObject *Sender);
    void __fastcall edtTreshVideoExit(TObject *Sender);
    void __fastcall panelLineColorZoneCoverClick(TObject *Sender);
    void __fastcall cbxLineColorZoneCoverDblClick(TObject *Sender);
    void __fastcall chbRequestForCoordDistClick(TObject *Sender);
    void __fastcall chbTheoPathTheSameClick(TObject *Sender);
    void __fastcall btCalcAddClick(TObject *Sender);
    void __fastcall btPpgAddClick(TObject *Sender);
    void __fastcall btDtmAddClick(TObject *Sender);
    void __fastcall btCalcRmvClick(TObject *Sender);
    void __fastcall btPpgRmvClick(TObject *Sender);
    void __fastcall btDtmRmvClick(TObject *Sender);
    void __fastcall edtPathDataExit(TObject *Sender);
    void __fastcall btCalcEdtClick(TObject *Sender);
    void __fastcall btPrgEdtClick(TObject *Sender);
    void __fastcall btDtmEdtClick(TObject *Sender);
    void __fastcall edtMapInitDelayExit(TObject *Sender);
    void __fastcall cbMapInitDelayClick(TObject *Sender);
private:
    void __fastcall fillCombo(TComboBox *cbx, TIBSQL* sql, int parentId, int elementId);
    void __fastcall AddServer(TComboBox *cbx, ServParamsArray &arr, GUID& iid);
    void __fastcall RemoveServer(TComboBox *cbx, ServParamsArray &arr);
public:
	virtual __fastcall TdlgCalcParams(TComponent* AOwner);
    void __fastcall EditServer(TComboBox * cbx, ServParamsArray& arr);
};
//----------------------------------------------------------------------------
extern PACKAGE TdlgCalcParams *dlgCalcParams;
//----------------------------------------------------------------------------
#endif
