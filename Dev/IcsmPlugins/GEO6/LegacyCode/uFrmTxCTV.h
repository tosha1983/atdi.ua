//---------------------------------------------------------------------------

#ifndef uFrmTxCTVH
#define uFrmTxCTVH
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
class TfrmTxCTV : public TfrmTxBase
{
__published:	// IDE-managed Components
        TLabel *lblPermRegion;
        TLabel *lblRequestCTV;
        TLabel *lblDatePermRegion;
    TDBEdit *edtNumPermRegionalCouncil;
    TDBMemo *mmNoticeCount;
    TDBEdit *edtDatePermRegional;
        TLabel *lblOrganPerm;
    TDBEdit *edtRegionalCouncil;
        TPanel *PnlCTVChannel;
        TLabel *lblChannekCount;
    TNumericEdit *edtChannelTVCount;
        TLabel *lblChannelFMCount;
    TNumericEdit *edtChannelFMCount;
    TDBGrid *dbgChannelCTV;
        TLabel *lblTabl;
        TPanel *pnlCTVInfo;
    TIBDataSet *ibdsLicenseCTV;
    TDataSource *dsLicenseCTV;
    TIBStringField *ibdsLicenseCTVREGIONALCOUNCIL;
    TIBStringField *ibdsLicenseCTVNUMPERMREGCOUNCIL;
    TDateField *ibdsLicenseCTVDATEPERMREGCOUNCIL;
    TBlobField *ibdsLicenseCTVNOTICECOUNT;
    TIntegerField *ibdsLicenseCTVID;
    TButton *btnNoticeCount;
    TIBDataSet *ibdsChannelCtv;
    TDataSource *dsChannelCtv;
    TIntegerField *ibdsChannelCtvID;
    TIntegerField *ibdsChannelCtvTRANSMITTERS_ID;
    TSmallintField *ibdsChannelCtvTYPESYSTEM;
    TIntegerField *ibdsChannelCtvTYPERECEIVE_ID;
    TIBStringField *ibdsChannelCtvRX_CHANNEL;
    TFloatField *ibdsChannelCtvRX_FREQUENCY;
    TIBStringField *ibdsChannelCtvTX_CHANNEL;
    TFloatField *ibdsChannelCtvTX_FREQUENCY;
    TIBStringField *ibdsChannelCtvNAMEPROGRAMM;
    TIBQuery *ibqTypeRec;
    TIBStringField *ibdsChannelCtvTR_NAME;
    TDateTimePicker *dtpPermRegion;
    TPanel *Panel1;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall ibdsLicenseCTVAfterEdit(TDataSet *DataSet);
    void __fastcall ibdsChannelCtvBeforePost(TDataSet *DataSet);
    void __fastcall ibdsChannelCtvTYPESYSTEMGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall ibdsChannelCtvTYPESYSTEMSetText(TField *Sender,
          const AnsiString Text);
    void __fastcall ibdsChannelCtvBeforeDelete(TDataSet *DataSet);
    void __fastcall dbgChannelCTVEditButtonClick(TObject *Sender);
    void __fastcall dtpPermRegionChange(TObject *Sender);
private:	// User declarations
     void __fastcall acceptListElementSelection(Messages::TMessage &Message);
     int num_tv_channel;
     int num_radio_channel;
     enum RX_TX {rxtxRX, rxtxTX};
     RX_TX FLAG_RX_TX;
public:		// User declarations
    __fastcall TfrmTxCTV(TComponent* Owner, ILISBCTx *in_Tx);
protected:
    virtual void __fastcall TxDataLoad();
    virtual void __fastcall TxDataSave();
    virtual void __fastcall SetRadiationClass();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmTxCTV *frmTxCTV;
//---------------------------------------------------------------------------
#endif
