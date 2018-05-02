//---------------------------------------------------------------------------

#ifndef uListDigAllotmentsH
#define uListDigAllotmentsH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uBaseListTree.h"
#include <ActnList.hpp>
#include <ComCtrls.hpp>
#include <DB.hpp>
#include <DBGrids.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <IBQuery.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <Menus.hpp>
#include <vector>
//---------------------------------------------------------------------------
class TfrmListDigAllotments : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TPopupMenu *pmnNewAllotment;
    TMenuItem *mniNewDAB;
    TMenuItem *mniNewDVB;
    TToolButton *ToolButton1;
    TIBSQL *IBSQL1;
    TAction *actImportDplan;
    TAction *actImportGt2Gs2;
    TAction *actExportGt2Gs2;
    TToolButton *tbtItuImport;
    TToolButton *tbtItuExport;
    TIntegerField *dstListID;
    TIntegerField *dstListADM_ID;
    TIBStringField *dstListALLOT_NAME;
    TIBStringField *dstListADM_REF_ID;
    TIBStringField *dstListNOTICE_TYPE;
    TIntegerField *dstListPLAN_ENTRY;
    TIBStringField *dstListB_NAME;
    TIBStringField *dstListC_NAME;
    TIBStringField *dstListSYNHRONETID;
    TIBStringField *dstListCTRY;
    TIBStringField *dstListREF_PLAN_CFG;
    TIBStringField *dstListTYP_REF_NETWK;
    TToolButton *ToolButton2;
    TAction *actMoveToSection;
    TAction *actCopyToSection;
    TPanel *panDbSection;
    TLabel *lblDbSection;
    TComboBox *cbxDbSection;
    TToolButton *tbtMoveToSect;
    TToolButton *tbtCopyToSect;
    TSmallintField *dstListNB_SUB_AREAS;
    TIntegerField *dstListDB_SECTION_ID;
    TToolButton *tbttbtExpGt2Gs2;
    void __fastcall mniNewAllotmentClick(TObject *Sender);
    void __fastcall actListInsertExecute(TObject *Sender);
    void __fastcall dstListCHANNELBLOCKGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstListPLAN_ENTRYGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dgrListMouseMove(TObject *Sender, TShiftState Shift,
          int X, int Y);
    void __fastcall actImportDplanExecute(TObject *Sender);
    void __fastcall actImportGt2Gs2Execute(TObject *Sender);
    void __fastcall actExportGt2Gs2Execute(TObject *Sender);
    void __fastcall actListDeleteExecute(TObject *Sender);
    void __fastcall cbxDbSectionChange(TObject *Sender);
    void __fastcall actMoveToSectionExecute(TObject *Sender);
    void __fastcall actCopyToSectionExecute(TObject *Sender);
private:	// User declarations
protected:
    std::vector<int> dbSectIds;
    __fastcall TfrmListDigAllotments(TComponent* Owner);
public:		// User declarations
    virtual void __fastcall EditElement();
    __fastcall TfrmListDigAllotments(TComponent* Owner, HWND callerWin, int elementId);
    __fastcall TfrmListDigAllotments(bool MDIChild);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListDigAllotments *frmListDigAllotments;
//---------------------------------------------------------------------------
#endif
