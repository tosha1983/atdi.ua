//---------------------------------------------------------------------------
#ifndef uFrmTxBaseH
#define uFrmTxBaseH
//---------------------------------------------------------------------------
#include <ActnList.hpp>
#include <Buttons.hpp>
#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <Controls.hpp>
#include <DB.hpp>
#include <DBCtrls.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Forms.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBQuery.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include "LISBCTxServer_TLB.h"
#include <Mask.hpp>
#include <math.h>
#include <Menus.hpp>
#include <OleCtnrs.hpp>
#include <StdCtrls.hpp>
#include <ToolWin.hpp>
#include "xlcClasses.hpp"
#include "xlEngine.hpp"
#include "xlReport.hpp"

#include "uMainDm.h"
#include "CustomMap.h"
#include "uLisObjectGrid.h"
#include <IBDatabase.hpp>
//---------------------------------------------------------------------------
class TfrmTxBase : public TForm
{
__published:	// IDE-managed Components
        TPanel *pnlForAllTop;
        TLabel *lblTxType;
        TLabel *lblCode;
        TLabel *lblNumber;
    TDBEdit *edtID;
        TLabel *lblRegion;
        TLabel *lblPoint;
    TDBEdit *edtCityName;
        TLabel *lblHPoint;
    TDBEdit *edtHPoint;
        TLabel *lblLong;
        TLabel *lblLat;
        TEdit *edtLong;
        TEdit *edtLat;
    TPageControl *pcData;
    TTabSheet *tshCommon;
        TPanel *pnlMaster;
        TLabel *lblTRC;
    TDBEdit *edtTRK;
    TButton *btnTRK;
        TLabel *lblMaster;
    TDBEdit *edtOperator;
    TButton *btnOperator;
        TLabel *lblProgram;
    TDBComboBox *cbProgramm;
        TPanel *pnlForAllBottom;
    TTabSheet *tshCoordination;
    TTabSheet *tshEquipment;
        TLabel *lblStand;
    TDBEdit *edtStand;
    TButton *btnStand;
        TLabel *lblDateEnter;
    TDBText *lblUserEnter;
        TLabel *lblEdit;
    TDBText *lblUserEdit;
        TLabel *lblReestr;
    TDBText *lblDateChange;
    TDBText *lblDateCreate;
    TTabSheet *tshLicenses;
        TLabel *lblLicenseChannel;
        TLabel *lblPermUse;
    TDBEdit *edtPermUse;
        TLabel *lblDatePermUseBeg;
        TLabel *lblDatePermUseEnd;
        TLabel *lblNumStdCertification;
    TDBComboBox *cbxNumStdCertification;
        TLabel *lblDateCertification;
        TLabel *lblFactoryNum;
    TDBEdit *edtFactoryNum;
        TLabel *lblClassWave;
        TLabel *lblTimeTransmit;
    TDBEdit *edtTimeTransmit;
    TDBEdit *edtClassWave;
        TGroupBox *gbOrganizations;
        TDBGrid *dbgOrganizations;
        TGroupBox *gbDocuments;
        TDBGrid *dbgDocuments;
        TGroupBox *gbEquipment;
    TDBGrid *dbgEquipment;
    TIBDataSet *ibdsStantionsBase;
    TDataSource *dsStantionsBase;
    TDBText *lblTypeRegistry;
    TIBDataSet *ibdsLicenses;
    TDataSource *dsLicenses;
    TDBEdit *edtAreaName;
    TIntegerField *ibdsStantionsBaseID;
    TIntegerField *ibdsStantionsBaseSTAND_ID;
    TDateField *ibdsStantionsBaseDATECREATE;
    TDateField *ibdsStantionsBaseDATECHANGE;
    TIntegerField *ibdsStantionsBaseOWNER_ID;
    TIntegerField *ibdsStantionsBaseSYSTEMCAST_ID;
    TIBStringField *ibdsStantionsBaseTIMETRANSMIT;
    TIntegerField *ibdsStantionsBaseUSERID;
    TIntegerField *ibdsStantionsBaseORIGINALID;
    TIBStringField *ibdsStantionsBaseNUMREGISTRY;
    TIBStringField *ibdsStantionsBaseTYPEREGISTRY;
    TIBStringField *ibdsStantionsBaseREMARKS;
    TIBQuery *ibqAccCondNameIn;
    TIBQuery *ibqAccCondNameOut;
    TIntegerField *ibqAccCondNameInID;
    TIBStringField *ibqAccCondNameInNAME;
    TIntegerField *ibqAccCondNameOutID;
    TIBStringField *ibqAccCondNameOutNAME;
    TDataSource *dsStand;
    TIBQuery *ibqUserName;
    TStringField *ibdsStantionsBaseUSER_NAME;
    TBitBtn *btnSave;
    TBitBtn *btnLoad;
    TDBEdit *edtCode;
    TIBQuery *ibqTRKName;
    TStringField *ibdsStantionsBaseTRK_NAME;
    TIBStringField *ibdsStantionsBaseRESPONSIBLEADMIN;
    TIBStringField *ibdsStantionsBaseCLASSWAVE;
    TStringField *ibdsStantionsBaseSYSTEMCAST_NAME;
    TSmallintField *ibdsStantionsBaseACCOUNTCONDITION_IN;
    TDBEdit *edtNumRegion;
    TLabel *lblNumRegion;
    TIBQuery *ibqSystemCastName;
    TIntegerField *ibqUserNameID;
    TIBStringField *ibqUserNameNAME;
    TIntegerField *ibqTRKNameID;
    TIBStringField *ibqTRKNameNAMEORGANIZATION;
    TIntegerField *ibqSystemCastNameID;
    TIBStringField *ibqSystemCastNameCODE;
    TIBStringField *ibdsStantionsBaseADMINISTRATIONID;
    TIBDataSet *ibdsTelecomOrg;
    TDataSource *dsTelecomOrg;
    TDataSource *dsDocuments;
    TIBDataSet *ibdsDocuments;
    TIntegerField *ibdsDocumentsID;
    TIntegerField *ibdsDocumentsTELECOMORGANIZATION_ID;
    TIntegerField *ibdsDocumentsTRANSMITTERS_ID;
    TSmallintField *ibdsDocumentsTYPELETTER;
    TIntegerField *ibdsDocumentsACCOUNTCONDITION_ID;
    TDateField *ibdsDocumentsCREATEDATEIN;
    TDateField *ibdsDocumentsCREATEDATEOUT;
    TIntegerField *ibdsDocumentsNUMIN;
    TIntegerField *ibdsDocumentsNUMOUT;
    TDateField *ibdsDocumentsANSWERDATE;
    TSmallintField *ibdsDocumentsANSWERIS;
    TIntegerField *ibdsDocumentsLETTERS_ID;
    TIBQuery *ibqDocType;
    TIntegerField *ibqDocTypeID;
    TGroupBox *gbDoc;
    TButton *btnDocCreate;
    TButton *btnDocEdit;
    TButton *btnDocAnswer;
    TSmallintField *ibdsStantionsBaseACCOUNTCONDITION_OUT;
    TDataSource *dsAccCondNameOut;
    TDataSource *dsAccCondNameIn;
    TIBStringField *ibqDocTypeNAME;
    TIntegerField *ibdsDocumentsDOCUMENT_ID;
    TIBDataSet *ibdsEquipment;
    TDataSource *dsEquipment;
    TLabel *lblEditing;
    TLabel *Label5;
    TDBEdit *edtAdress;
    TBitBtn *btnOk;
    TBitBtn *btnCansel;
    TIntegerField *ibdsTelecomOrgID;
    TIntegerField *ibdsTelecomOrgTRANSMITTER_ID;
    TIntegerField *ibdsTelecomOrgTELECOMORG_ID;
    TIntegerField *ibdsTelecomOrgACCOUNTCONDITION_ID;
    TIBStringField *ibdsTelecomOrgCODE;
    TIBStringField *ibdsTelecomOrgNAME;
    TIBStringField *ibdsTelecomOrgAC_NAME;
    TButton *btnDocSave;
    TIntegerField *ibdsEquipmentEQUIPMENT_ID;
    TIntegerField *ibdsEquipmentTRANSMITTERS_ID;
    TDateField *ibdsEquipmentDATESTANDCERTIFICATE;
    TIBStringField *ibdsEquipmentNUMSTANDCERTIFICATE;
    TIBStringField *ibdsEquipmentNUMFACTORY;
    TIBStringField *ibdsEquipmentNAME;
    TIBStringField *ibdsEquipmentTYPEEQUIPMENT;
    TIBStringField *ibdsEquipmentMANUFACTURE;
    TIBStringField *ibdsLicensesNUMPERMUSE;
    TDateField *ibdsLicensesDATEPERMUSEFROM;
    TDateField *ibdsLicensesDATEPERMUSETO;
    TIBStringField *ibdsLicensesNUMSTANDCERTIFICATE;
    TIBStringField *ibdsLicensesNUMFACTORY;
    TDateField *ibdsLicensesDATEINTENDUSE;
    TDateField *ibdsLicensesDATESTANDCERTIFICATE;
    TIntegerField *ibdsLicensesID;
    TIntegerField *ibdsEquipmentID;
    TButton *btnGotoLast;
    TButton *btnGotoNext;
    TGroupBox *gbOrganization;
    TButton *btnNewTelecomOrg;
    TButton *btnDelOrg;
    TIntegerField *ibdsStantionsBaseOPERATOR_ID;
    TTabSheet *tshTestpoint;
    TGroupBox *gbTestpoints;
    TDBGrid *dbgTestpoints;
    TDataSource *dsTestpoints;
    TIBDataSet *ibdsTestpoint;
    TIntegerField *ibdsTestpointID;
    TIntegerField *ibdsTestpointTRANSMITTERS_ID;
    TIBStringField *ibdsTestpointNAME;
    TFloatField *ibdsTestpointLATITUDE;
    TFloatField *ibdsTestpointLONGITUDE;
    TSmallintField *ibdsTestpointTESTPOINT_TYPE;
    TFloatField *ibdsTestpointBEARING;
    TFloatField *ibdsTestpointDISTANCE;
    TFloatField *ibdsTestpointUSEBLEFIELD;
    TFloatField *ibdsTestpointPROTECTEDFIELD;
    TActionList *ActionList1;
    TAction *actOk;
    TAction *actClose;
    TAction *actApply;
    TAction *actLoad;
    TAction *actIntoProject;
    TAction *actIntoBase;
    TAction *actIntoarchives;
    TSmallintField *ibdsStantionsBaseSTATUS;
    TAction *actIntoBeforeBase;
    TPanel *pnlEquipButton;
    TButton *btnEqAdd;
    TButton *btnEqDel;
    TIBQuery *ibqNewTx;
    TPopupMenu *pmIntoBeforeBase;
    TMenuItem *mniCopyToDraftAnal;
    TMenuItem *mniMoveToDraft;
    TDateTimePicker *dtpPermUseBeg;
    TDateTimePicker *dtpPermUseEnd;
    TDateTimePicker *dtpDatCertificate;
    TDBEdit *edtPermUseBeg;
    TDBEdit *edtPermUseEnd;
    TDBEdit *edtDateCertificate;
    TDBEdit *edDateBegUse;
    TAction *actTxCopy;
    TAction *actIntoList;
    TAction *actIntoTree;
    TSpeedButton *sbIntoProject;
    TSpeedButton *sbIntoBase;
    TSpeedButton *sbIntoArhive;
    TSpeedButton *sbIntoBeforeBase;
    TSpeedButton *sbCopy;
    TSpeedButton *sbIntoList;
    TSpeedButton *sbIntoTree;
    TImageList *ImageList1;
    TLabel *lblOut;
    TDBLookupComboBox *cbStateOut;
    TLabel *lblIn;
    TDBLookupComboBox *cbStateIn;
    TIBStringField *ibqAccCondNameInCODE;
    TPageControl *pcRemark;
    TTabSheet *tshChangeLog;
    TDBGrid *dbgGhangeLog;
    TPanel *panList;
    TDBGrid *dgrList;
    TPanel *panSearch;
    TEdit *edtIncSearch;
    TDataSource *dsUserActLog;
    TIBDataSet *ibdsUserActLog;
    TIntegerField *ibdsUserActLogID;
    TDateTimeField *ibdsUserActLogDATECHANGE;
    TIBStringField *ibdsUserActLogLOGIN;
    TIBStringField *ibdsUserActLogTYPECHANGE;
    TIBStringField *ibdsUserActLogNAME_TABLE;
    TIBStringField *ibdsUserActLogNAME_FIELD;
    TIntegerField *ibdsUserActLogNUM_CHANGE;
    TIBStringField *ibqAccCondNameOutCODE;
    TDBLookupComboBox *cbStateInCode;
    TDBLookupComboBox *cbStateOutCode;
    TIBSQL *sqlNewAdminId;
    TIBQuery *ibqOwner;
    TStringField *ibdsStantionsBaseOPERATOR_NAME;
    TIBStringField *ibdsStantionsBaseNAMEPROGRAMM;
    TIBQuery *ibqStand;
    TIntegerField *ibqStandID;
    TIBStringField *ibqStandNAMESITE;
    TIntegerField *ibqStandCITY_ID;
    TIntegerField *ibqStandAREA_ID;
    TIntegerField *ibqStandHEIGHT_SEA;
    TIBStringField *ibqStandAREA_NAME;
    TIBStringField *ibqStandCITY_NAME;
    TIBStringField *ibqStandSTREET_NAME;
    TIBStringField *ibqStandADDRESS;
    TIBStringField *ibqStandNUMREG;
    TStringField *ibqStandFULL_ADDR;
    TButton *btnReplyRequired;
    TGroupBox *gbxCoordination;
    TDBMemo *DBMemo1;
    TTabSheet *tshMap;
    TImageList *imlMap;
    TSpeedButton *sbPlanning;
    TSpeedButton *sbExamination;
    TAction *actExamination;
    TAction *actPlanning;
    TDBEdit *dbedNumRegistry;
    TMenuItem *mniCopyToDraftDig;
    TTabSheet *tshPrivateNote;
    TTabSheet *tshNote;
    TDBMemo *DBMemo2;
    TIBStringField *ibdsStantionsBaseREMARKS_ADD;
    TCustomMapFrame *cmf;
    TLabel *lblNrReqNo;
    TDBEdit *edNrReqNo;
    TLabel *lblNrReqDate;
    TDBEdit *edNrReqDate;
    TDateTimePicker *dtpNrReq;
    TLabel *lblNrConclNo;
    TDBEdit *edNrConclNo;
    TLabel *lblNrConclDate;
    TDBEdit *edNrConclDate;
    TDateTimePicker *dtpNrConcl;
    TLabel *lblNrApplNo;
    TDBEdit *edNrApplNo;
    TLabel *lblNrApplDate;
    TDBEdit *edNrApplDate;
    TDateTimePicker *dtpNrAppl;
    TLabel *lblEmsNo;
    TDBEdit *edEmsNo;
    TLabel *lblEmsDateBeg;
    TDBEdit *edEmsDateBeg;
    TDateTimePicker *dtpEmsDateBeg;
    TLabel *lblEmsDateEnd;
    TDBEdit *edEmsDateEnd;
    TDateTimePicker *dtpEmsDateEnd;
    TBevel *bev1;
    TBevel *bev2;
    TLisObjectGrid *objGrdLic;
    TButton *btNrAppl;
    TLabel *lblJcsCond;
    TDBEdit *edJcsCond;
    TButton *btJcsCond;
    TLabel *lblPermOwner;
    TDBEdit *edPermOwner;
    TButton *btPermOwner;
    TLabel *lbllResLic;
    TDBEdit *edResLic;
    TButton *btResLic;
    TLabel *lbResLicDateBeg;
    TDBEdit *edResLicDateBeg;
    TDateTimePicker *dtpResLicDateBeg;
    TLabel *lblResLicDateEnd;
    TDBEdit *edResLicDateEnd;
    TDateTimePicker *dtpResLicDateEnd;
    TDBEdit *edlResLicOwner;
    TButton *btlResLicOwner;
    TLabel *lblResLicOwner;
    TButton *btnFieldList;
    TButton *btnAttach;
    TButton *btnDetach;
    TIntegerField *ibdsLicensesLICENSE_RFR_ID;
    TIBStringField *ibdsLicensesL_RFR_NUMLICENSE;
    TDateField *ibdsLicensesL_RFR_DATEFROM;
    TDateField *ibdsLicensesL_RFR_DATETO;
    TIBStringField *ibdsLicensesL_RFR_O_NAME;
    TIBStringField *ibdsLicensesEMC_CONCL_NUM;
    TDateField *ibdsLicensesEMC_CONCL_FROM;
    TDateField *ibdsLicensesEMC_CONCL_TO;
    TIntegerField *ibdsLicensesPERMUSE_OWNER_ID;
    TIBStringField *ibdsLicensesPERMUSE_O_NAME;
    TIBStringField *ibdsLicensesNR_REQ_NO;
    TDateField *ibdsLicensesNR_REQ_DATE;
    TIBStringField *ibdsLicensesNR_CONCL_NO;
    TDateField *ibdsLicensesNR_CONCL_DATE;
    TIBStringField *ibdsLicensesNR_APPL_NO;
    TDateField *ibdsLicensesNR_APPL_DATE;
    TIBTransaction *tr;
    TGroupBox *gbHoursOfOp;
    TLabel *lbOpEnd;
    TDBEdit *edOpStart;
    TDBEdit *edOpEnd;
    TLabel *lbOpStart;
    TTimeField *ibdsLicensesOP_HH_FR;
    TTimeField *ibdsLicensesOP_HH_TO;
    TGroupBox *gbUseDates;
    TLabel *lbIntoUse;
    TLabel *lbExpired;
    TDateTimePicker *dtIntoUse;
    TDateTimePicker *dtExpired;
    TDateField *ibdsLicensesD_EXPIRY;
    TSmallintField *ibdsLicensesREMARK_CONDS_MET;
    TSmallintField *ibdsLicensesIS_RESUB;
    TSmallintField *ibdsLicensesSIGNED_COMMITMENT;
    TDBEdit *edExpiry;
    TIBStringField *ibdsDocumentsAC_NAME;
    TIBStringField *ibdsDocumentsDT_NAME;
    void __fastcall btnOperatorClick(TObject *Sender);
    void __fastcall edtLongExit(TObject *Sender);
    void __fastcall edtLatExit(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall btnStandClick(TObject *Sender);
    void __fastcall btnTRKClick(TObject *Sender);
    void __fastcall btnDocCreateClick(TObject *Sender);
    void __fastcall btnNewTelecomOrgClick(TObject *Sender);
    void __fastcall dbgDocumentsEditButtonClick(TObject *Sender);

    void __fastcall dbgDocumentsDblClick(TObject *Sender);
    void __fastcall btnDocDelClick(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall ibdsDocumentsAfterScroll(TDataSet *DataSet);
    void __fastcall ibdsTelecomOrgBeforePost(TDataSet *DataSet);
    void __fastcall btnDocEditClick(TObject *Sender);
    void __fastcall btnDocAnswerClick(TObject *Sender);
    void __fastcall btnDocSaveClick(TObject *Sender);
    void __fastcall ibdsTelecomOrgAfterScroll(TDataSet *DataSet);
    void __fastcall tshEquipmentShow(TObject *Sender);
    void __fastcall btnEqAddClick(TObject *Sender);
    void __fastcall btnEqDelClick(TObject *Sender);
    void __fastcall ibdsEquipmentBeforePost(TDataSet *DataSet);
    void __fastcall btnGotoLastClick(TObject *Sender);
    void __fastcall btnGotoNextClick(TObject *Sender);
    void __fastcall ibdsDocumentsTYPELETTERGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall ibdsDocumentsANSWERISGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall FormCloseQuery(TObject *Sender, bool &CanClose);
    void __fastcall tshTestpointShow(TObject *Sender);
    void __fastcall ibdsTestpointAfterInsert(TDataSet *DataSet);
    void __fastcall ibdsTestpointBeforePost(TDataSet *DataSet);
    void __fastcall actOkUpdate(TObject *Sender);
    void __fastcall actOkExecute(TObject *Sender);
    void __fastcall actApplyExecute(TObject *Sender);
    void __fastcall actCloseExecute(TObject *Sender);
    void __fastcall actLoadExecute(TObject *Sender);
    void __fastcall ibdsStantionsBaseAfterEdit(TDataSet *DataSet);
    void __fastcall ibdsTelecomOrgAfterEdit(TDataSet *DataSet);
    void __fastcall ibdsTestpointAfterEdit(TDataSet *DataSet);
    void __fastcall ibdsLicensesAfterEdit(TDataSet *DataSet);
    void __fastcall ibdsEquipmentAfterEdit(TDataSet *DataSet);
    void __fastcall ibdsTestpointLATITUDEGetText(TField *Sender, AnsiString &Text, bool DisplayText);
    void __fastcall ibdsTestpointLONGITUDEGetText(TField *Sender, AnsiString &Text, bool DisplayText);
    void __fastcall ibdsTestpointLATITUDESetText(TField *Sender, const AnsiString Text);
    void __fastcall ibdsTestpointTESTPOINT_TYPESetText(TField *Sender, const AnsiString Text);
    void __fastcall ibdsTestpointTESTPOINT_TYPEGetText(TField *Sender, AnsiString &Text, bool DisplayText);
    void __fastcall ibdsTelecomOrgBeforeDelete(TDataSet *DataSet);
    void __fastcall btnDelOrgClick(TObject *Sender);
    void __fastcall actIntoProjectExecute(TObject *Sender);
    void __fastcall actIntoBaseExecute(TObject *Sender);
    void __fastcall actIntoarchivesExecute(TObject *Sender);
    void __fastcall actIntoBeforeBaseExecute(TObject *Sender);
    void __fastcall ibdsDocumentsDOCTYPE_NAMEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall mniCopyToDraftAnalClick(TObject *Sender);
    void __fastcall mniMoveToDraftClick(TObject *Sender);
    void __fastcall dtpPermUseBegChange(TObject *Sender);
    void __fastcall dtpPermUseEndChange(TObject *Sender);
    void __fastcall dtpDatCertificateChange(TObject *Sender);
    void __fastcall dbgEquipmentEnter(TObject *Sender);
    void __fastcall dbgTestpointsEnter(TObject *Sender);
    void __fastcall btnInListClick(TObject *Sender);
    void __fastcall actTxCopyExecute(TObject *Sender);
    void __fastcall actIntoListExecute(TObject *Sender);
    void __fastcall actIntoTreeExecute(TObject *Sender);
    void __fastcall ibdsTestpointDISTANCEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall pcDataChange(TObject *Sender);
    void __fastcall ibqStandCalcFields(TDataSet *DataSet);
    void __fastcall ibdsDocumentsFilterRecord(TDataSet *DataSet,
          bool &Accept);
    void __fastcall btnReplyRequiredClick(TObject *Sender);
    void __fastcall mniCopyToDraftDigClick(TObject *Sender);
    void __fastcall actExaminationExecute(TObject *Sender);
    void __fastcall actPlanningExecute(TObject *Sender);
    void __fastcall dtpNrReqChange(TObject *Sender);
    void __fastcall dtpNrConclChange(TObject *Sender);
    void __fastcall dtpNrApplChange(TObject *Sender);
    void __fastcall dtpEmsDateEndChange(TObject *Sender);
    void __fastcall dtpEmsDateBegChange(TObject *Sender);
    void __fastcall btResLicClick(TObject *Sender);
    void __fastcall dtpResLicDateBegChange(TObject *Sender);
    void __fastcall dtpResLicDateEndChange(TObject *Sender);
    void __fastcall btnFieldListClick(TObject *Sender);
    void __fastcall btnAttachClick(TObject *Sender);
    void __fastcall btnDetachClick(TObject *Sender);
    void __fastcall btPermOwnerClick(TObject *Sender);
    void __fastcall dtIntoUseChange(TObject *Sender);
    void __fastcall dtExpiredChange(TObject *Sender);
    
private:
    enum ACC_STATE { LAST_IN, LAST_OUT};
    ACC_STATE acc_in_out;
    enum FLAG_OWNER {foNON, foTRK, foOPERATOR};
    FLAG_OWNER flag_owner;
    int new_systemcast;

    TPopupMenu *popupMenu;

    bool __fastcall LastRecord(int id);
    void __fastcall MenuItem_OnClick(TObject *Sender);

public:		// User declarations
    TBCTxType type_form;
    ILISBCTx* __fastcall GetTx() { return (ILISBCTx *)Tx; }
    bool change_systemcast;
    int NewElementId;

    void __fastcall NewTx();
    AnsiString Passband2Str(double video_carrier);
    virtual void __fastcall SetRadiationClass() = 0;
    __fastcall TfrmTxBase(TComponent* Owner, ILISBCTx *in_Tx);
    void __fastcall UpdateDbSectLook(void);
    String __fastcall GetOwnerFilter();
    void __fastcall CreateDoc(TObject * Sender);//¡¿«¿ / œ–≈ƒ¡¿«¿ ...
protected:
    __fastcall TfrmTxBase(TComponent* Owner);
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptListElementSelection)
    END_MESSAGE_MAP(TForm)
    virtual void __fastcall acceptListElementSelection(Messages::TMessage &Message);
    virtual void __fastcall TxDataLoad();
    bool reply_required;
    ILISBCTxPtr Tx;
    long id;
    virtual void __fastcall TxDataSave();

    TControl *lastLicCaller;

    long isChanged;
};
//---------------------------------------------------------------------------
//extern PACKAGE TfrmTxBase *frmTxBase;
//---------------------------------------------------------------------------
#endif
