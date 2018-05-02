//---------------------------------------------------------------------------

#ifndef uExplorerH
#define uExplorerH
//---------------------------------------------------------------------------
#include <ActnList.hpp>
#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <Controls.hpp>
#include <Forms.hpp>
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <Menus.hpp>
#include <StdCtrls.hpp>

#include "uMainDm.h"
//---------------------------------------------------------------------------
class TfrmExplorer : public TForm
{
__published:	// IDE-managed Components
    TActionList *aclExplorer;
    TImageList *imlList;
    TTreeView *trvExplorer;
    TAction *actNewObject;
    TAction *actDeleteObject;
    TPopupMenu *pmnExplorer;
    TMenuItem *actNewObject1;
    TMenuItem *actDeleteObject1;
    TIBSQL *sqlAddObject;
    TIBSQL *sqlDeleteObject;
    TStaticText *txtNoItems;
    TAction *actRefresh;
    TMenuItem *N1;
    TMenuItem *N2;
    TIBSQL *sqlSelectIDs;
    TIBSQL *sqlSelSelections;
    TAction *actDeleteSelection;
    TAction *actNewSelection;
    TIBSQL *sqlDeleteSelection;
    TMenuItem *N3;
    TMenuItem *N4;
    TMenuItem *N5;
    TIBSQL *sqlTPSelections;
    TIBSQL *sqlGetTxId;
    TAction *actEdit;
    TAction *actSaveTxTo;
    TAction *actPlan;
    TMenuItem *N6;
    TMenuItem *N7;
    TAction *actDeleteAll;
    TMenuItem *N8;
    TAction *actMoveUp;
    TAction *actMoveDown;
    TMenuItem *N9;
    TMenuItem *actMoveUp1;
    TMenuItem *actMoveDown1;
    TAction *actSelectionDeleteAll;
    TMenuItem *actSelectionDeleteAll1;
    TAction *actTestPointDeleteAll;
    TMenuItem *N10;
    TMenuItem *N11;
    TIBSQL *selMaxObjNo;
    TMenuItem *miIcst;
    TIBSQL *sqlSelIcst;
    void __fastcall actNewObjectExecute(TObject *Sender);
    void __fastcall actDeleteObjectExecute(TObject *Sender);
    void __fastcall actDeleteAllExecute(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall actRefreshExecute(TObject *Sender);
    void __fastcall trvExplorerDblClick(TObject *Sender);
    void __fastcall trvExplorerKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall trvExplorerDragOver(TObject *Sender, TObject *Source,
          int X, int Y, TDragState State, bool &Accept);
    void __fastcall trvExplorerDragDrop(TObject *Sender, TObject *Source,
          int X, int Y);
    void __fastcall trvExplorerExpanding(TObject *Sender, TTreeNode *Node,
          bool &AllowExpansion);
    void __fastcall actDeleteSelectionExecute(TObject *Sender);
    void __fastcall actNewSelectionExecute(TObject *Sender);
    void __fastcall actDeleteSelectionUpdate(TObject *Sender);
    void __fastcall FecthTx1Click(TObject *Sender);
    void __fastcall actPlanExecute(TObject *Sender);
        void __fastcall FormCreate(TObject *Sender);
        void __fastcall FormShow(TObject *Sender);
        void __fastcall FormResize(TObject *Sender);
    void __fastcall actMoveUp1Click(TObject *Sender);
    void __fastcall actMoveDown1Click(TObject *Sender);
    void __fastcall trvExplorerMouseDown(TObject *Sender,
          TMouseButton Button, TShiftState Shift, int X, int Y);
    void __fastcall actSelectionDeleteAllExecute(TObject *Sender);
    void __fastcall actTestPointDeleteAllExecute(TObject *Sender);
    void __fastcall trvExplorerDeletion(TObject *Sender, TTreeNode *Node);
    void __fastcall pmnExplorerPopup(TObject *Sender);
    void __fastcall miIcstClick(TObject *Sender);
private:	// User declarations
    std::map<int, StandRecord> stands;
    void __fastcall moveTreeItem(int Direction);//0 -- Up, 1 -- down
    void __fastcall DeleteSelection(int id);
public:		// User declarations
    int height;
    int width;
    bool ResizeStarted;
    __fastcall TfrmExplorer(TComponent* Owner);
    void __fastcall Refresh();
    void __fastcall AddTx(int TxId);
    String __fastcall GetTxBranchText(int txId);
protected:
    int UserId;
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_LIST_ELEMENT_SELECTED, Messages::TMessage, acceptSelection)
    END_MESSAGE_MAP(TForm)
    void __fastcall acceptSelection(Messages::TMessage& Message);
    void __fastcall AddTxToTree(int TxId);
    void __fastcall find_branch(TTreeNode* tn, TStringList* sl);
    void __fastcall GetSelectionBranches(TTreeNode* root);
    void __fastcall GetTestPointBranches(TTreeNode* root);
    bool __fastcall SelectedIsSelection();
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmExplorer *frmExplorer;
//---------------------------------------------------------------------------
#endif
