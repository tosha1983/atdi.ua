//---------------------------------------------------------------------------

#ifndef uBaseListTreeH
#define uBaseListTreeH
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
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <IBSQL.hpp>
#include <IBQuery.hpp>

#include "uBaseList.h"

#include <set>
//---------------------------------------------------------------------------
class TfrmBaseListTree : public TfrmBaseList
{
__published:	// IDE-managed Components
    TTreeView *trvList;
    TSplitter *splTree;
    TToolButton *tbtShowTree;
    TToolButton *tbtMoveTo;
    TAction *actShowTree;
    TAction *actMoveTo;
    TToolButton *tbtSeparator1;
    TIBSQL *sqlFindGrp;
    TIBSQL *sqlTree;
    TStaticText *stPath;
    TPanel *panTree;
    TStaticText *stxListQuantity;
    void __fastcall dstListNewRecord(TDataSet *DataSet);
    void __fastcall trvListDblClick(TObject *Sender);
    void __fastcall trvListKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall dstListAfterScroll(TDataSet *DataSet);
private:	// User declarations
protected:
    __fastcall TfrmBaseListTree(TComponent* Owner);

    virtual void __fastcall changeBranch(TTreeNode *newNode);
    void __fastcall checkWasInBase(std::set<int>& standIds);
    virtual void __fastcall fillNode(TTreeNode* node, int level);
    virtual void __fastcall fillTree();
    virtual void __fastcall Initialize();
    int __fastcall getAreaId();
    virtual __fastcall findBranch();
public:		// User declarations
    __fastcall TfrmBaseListTree(TComponent* Owner, HWND callerWin, int elementId);
    __fastcall TfrmBaseListTree(bool MDIChild);    
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmBaseListTree *frmBaseListTree;
//---------------------------------------------------------------------------
#endif
