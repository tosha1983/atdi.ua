#ifndef uListTransmittersH
#define uListTransmittersH
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
#include "uMainDm.h"
#include <Buttons.hpp>
#include <Menus.hpp>
#include <IBQuery.hpp>

#include "uBaseListTree.h"

#include <set>

//---------------------------------------------------------------------------
class TfrmListTransmitters : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListTX_ID;
    TIBStringField *dstListADMINISTRATIONID;
    TFloatField *dstListTX_LAT;
    TFloatField *dstListTX_LONG;
    TFloatField *dstListVIDEO_CARRIER;
    TFloatField *dstListSOUND_CARRIER;
    TIntegerField *dstListVIDEO_OFFSET_HERZ;
    TFloatField *dstListEPR_VIDEO_MAX;
    TIntegerField *dstListHEIGHT_EFF_MAX;
    TFloatField *dstListPOWER_VIDEO;
    TIBStringField *dstListNAMESITE;
    TIBStringField *dstListAREA_NAME;
    TIBStringField *dstListCITY_NAME;
    TIBStringField *dstListNUMREGION;
    TIntegerField *dstListORIGINALID;
    TSmallintField *dstListSTATUS;
    TCheckBox *chbOnlyRoot;
    TIBStringField *dstListBD_NAME;
    TAction *actIntoBase;
    TAction *actIntoarchives;
    TAction *actIntoBeforeBase;
    TPopupMenu *pmIntoBeforeBase;
    TMenuItem *N1;
    TMenuItem *N2;
    TAction *actIntoProject;
    TToolButton *tbtnIntoBase;
    TToolButton *tbtnIntoBeforeBase;
    TToolButton *tbtnIntoArhiv;
    TToolButton *tbtnIntoProject;
    TToolButton *ToolButton5;
    TToolButton *ToolButton6;
    TIBStringField *dstListACIN_CODE;
    TIBStringField *dstListACOUT_CODE;
    TIBStringField *dstListSCAST_CODE;
    TIBStringField *dstListNAMECHANNEL;
    TSmallintField *dstListS_ENUMVAL;
    TFloatField *dstListEPR_SOUND_MAX_PRIMARY;
    TFloatField *dstListBLOCKCENTREFREQ;
    TSmallintField *dstListVIDEO_OFFSET_LINE;
    TSmallintField *dstListWAS_IN_BASE;
    TIntegerField *dstListUSER_DELETED;
    TDateTimeField *dstListDATE_DELETED;
    TIBDataSet *dstUsers;
    TIntegerField *dstUsersID;
    TIBStringField *dstUsersLOGIN;
    TIBStringField *dstUsersNAME;
    TStringField *dstListUSERDELNAME;
    TIntegerField *dstListTX_STAND_ID;
    TAction *actExport;
    TToolButton *tbtExport;
    TToolButton *tbtRecalcCoordDists;
    TAction *actRecalcTbtCoordDists;
    TAction *actMoveToSection;
    TAction *actCopyToSection;
    TPanel *panDbSection;
    TLabel *lblDbSection;
    TComboBox *cbxDbSection;
    TToolButton *tbtMoveToSect;
    TToolButton *tbtCopyToSect;
    TAction *actExpGt1Gs1;
    TToolButton *tbtExpGt1Gs1;
    TIBStringField *dstListPOL;
    void __fastcall dstListTX_LATGetText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall dstListTX_LONGGetText(TField *Sender, AnsiString &Text,
          bool DisplayText);
    void __fastcall dstListNAMECHANNELGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall actListEditExecute(TObject *Sender);
    void __fastcall dgrListDblClick(TObject *Sender);
    void __fastcall dgrListKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall dgrListMouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
    void __fastcall chbOnlyRootClick(TObject *Sender);
    void __fastcall dstListVIDEO_OFFSET_HERZGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall actIntoProjectExecute(TObject *Sender);
    void __fastcall actIntoBaseExecute(TObject *Sender);
    void __fastcall actIntoarchivesExecute(TObject *Sender);
    void __fastcall actIntoBeforeBaseExecute(TObject *Sender);
    void __fastcall N1Click(TObject *Sender);
    void __fastcall N2Click(TObject *Sender);
    void __fastcall actListDeleteExecute(TObject *Sender);
    void __fastcall actListCopyExecute(TObject *Sender);
    void __fastcall dstListEPR_VIDEO_MAXGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall actListInsertExecute(TObject *Sender);
    void __fastcall actListDeleteUpdate(TObject *Sender);
    void __fastcall actExportExecute(TObject *Sender);
    void __fastcall actRecalcTbtCoordDistsExecute(TObject *Sender);
    void __fastcall actMoveToExecute(TObject *Sender);
    void __fastcall cbxDbSectionChange(TObject *Sender);
    void __fastcall actMoveToSectionExecute(TObject *Sender);
    void __fastcall actCopyToSectionExecute(TObject *Sender);
    void __fastcall actExpGt1Gs1Execute(TObject *Sender);
private:	// User declarations
    TComponent* FOwner;
    HWND Fcaller;
    int FelementId;
    unsigned int FTxFlags;

    void __fastcall moveRecords(std::set<int>& standIds);    
protected:
    void __fastcall changeBranch(TTreeNode* node);
    __fastcall TfrmListTransmitters(TComponent* Owner);
    virtual void __fastcall setCaption();
    void __fastcall ChangeStatus(int id, int status, bool wasinbase);
    virtual __fastcall changeQueryCopy();
    virtual void __fastcall Initialize();
    bool change_systemcast;
    std::vector<int> dbSectIds;
public:		// User declarations
    __fastcall TfrmListTransmitters(TComponent* Owner, HWND caller, int elementId, unsigned int TxFlags);
    void __fastcall refresh();
    unsigned int list_flags;
    int TfrmListTransmitters::SelectIdRootTx(int id);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListTransmitters *frmListTransmitters;
//---------------------------------------------------------------------------
#endif
