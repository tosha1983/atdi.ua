//---------------------------------------------------------------------------

#ifndef uListStandH
#define uListStandH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <IBQuery.hpp>

#include "uBaseListTree.h"

#include <set>

//---------------------------------------------------------------------------
class TfrmListStand : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TFloatField *dstListLATITUDE;
    TFloatField *dstListLONGITUDE;
    TIBStringField *dstListNAMESITE;
    TIntegerField *dstListAREA_ID;
    TIntegerField *dstListDISTRICT_ID;
    TIntegerField *dstListCITY_ID;
    TIntegerField *dstListSTREET_ID;
    TIBStringField *dstListADDRESS;
    TIntegerField *dstListHEIGHT_SEA;
    TFloatField *dstListMAX_OBST;
    TFloatField *dstListMAX_USE;
    TSmallintField *dstListVHF_IS;
    TIBStringField *dstListCN_NAME;
    TIBStringField *dstListA_NAME;
    TIBStringField *dstListD_NAME;
    TIBStringField *dstListC_NAME;
    TIBStringField *dstListST_NAME;
    TSplitter *splList;
    TDBGrid *grdTx;
    TDataSource *dsTx;
    TIBDataSet *dstTx;
    TIntegerField *dstTxTX_ID;
    TFloatField *dstTxTX_LAT;
    TFloatField *dstTxTX_LONG;
    TIBStringField *dstTxSCAST_CODE;
    TIBStringField *dstTxNAMECHANNEL;
    TIBStringField *dstTxACIN_CODE;
    TIBStringField *dstTxACOUT_CODE;
    TFloatField *dstTxSOUND_CARRIER;
    TSmallintField *dstTxVIDEO_OFFSET_LINE;
    TIntegerField *dstTxVIDEO_OFFSET_HERZ;
    TFloatField *dstTxEPR_VIDEO_MAX;
    TIntegerField *dstTxHEIGHT_EFF_MAX;
    TFloatField *dstTxPOWER_VIDEO;
    TIBSQL *sqlStreets;
    TIBSQL *sqlInsertStreet;
    TSmallintField *dstTxS_ENUMVAL;
    TFloatField *dstTxVIDEO_CARRIER;
    TIBStringField *dstTxADMINISTRATIONID;
    TIBStringField *dstListNAMESITE_ENG;
    TFloatField *dstTxEPR_SOUND_MAX_PRIMARY;
    TFloatField *dstTxBLOCKCENTREFREQ;
    TToolButton *ToolButton1;
    TToolButton *ToolButton2;
    TAction *actSearchCoord;
    TIBStringField *dstTxPOL;
    void __fastcall dstTxTX_LATGetText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall dstTxTX_LONGGetText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall dstTxNAMECHANNELGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstListCITY_IDChange(TField *Sender);
    void __fastcall dstListST_NAMEChange(TField *Sender);
    void __fastcall dstListAfterScroll(TDataSet *DataSet);
    void __fastcall dstListLATITUDEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstListLONGITUDEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall grdTxDblClick(TObject *Sender);
    void __fastcall grdTxKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall dstListLATITUDESetText(TField *Sender,
          const AnsiString Text);
    void __fastcall dstListLONGITUDESetText(TField *Sender,
          const AnsiString Text);
    void __fastcall dstTxEPR_VIDEO_MAXGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall grdTxMouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
    void __fastcall dstListAfterPost(TDataSet *DataSet);
    void __fastcall dstListNAMESITESetText(TField *Sender,
          const AnsiString Text);
    void __fastcall actMoveToExecute(TObject *Sender);
    void __fastcall dstListBeforeDelete(TDataSet *DataSet);
    void __fastcall actSearchCoordExecute(TObject *Sender);
private:
    void __fastcall moveRecords(std::set<int>& ids);
public:		// User declarations
    __fastcall TfrmListStand(TComponent* Owner, HWND caller, int elementId);
protected:
    __fastcall TfrmListStand(TComponent* Owner);
    virtual void __fastcall changeBranch(TTreeNode* node);
    virtual void __fastcall updateLookups();
    virtual void __fastcall Initialize();
    TColumn* __fastcall fillStreetPickList();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListStand *frmListStand;
//---------------------------------------------------------------------------
#endif
