//---------------------------------------------------------------------------

#ifndef uPlanningH
#define uPlanningH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <Grids.hpp>
#include <ExtCtrls.hpp>
#include <DB.hpp>
#include <IBCustomDataSet.hpp>
#include <ToolWin.hpp>
#include <ActnList.hpp>
#include <ImgList.hpp>
#include <Menus.hpp>
#include "uMainDm.h"
#include <vector>
//---------------------------------------------------------------------------
class TfrmPlanning : public TForm
{
__published:	// IDE-managed Components
    TPageControl *pcPlanning;
    TTabSheet *tshPlan;
    TTabSheet *tshListUnwantSort;
    TTabSheet *tshListWantSort;
    TStringGrid *grdPlan;
    TStringGrid *grdWantedSort;
    TStringGrid *grdUnwantedSort;
    TToolBar *tbrPlan;
    TToolButton *tbtTx;
    TToolButton *tbtCopy;
    TLabel *lblChannel;
    TImageList *imlPlan;
    TActionList *aclList;
    TAction *actCh2Tx;
    TAction *actCh2Copy;
    TPopupMenu *pmnPlan;
    TMenuItem *N1;
    TMenuItem *N2;
    TToolButton *tbtSave;
    TToolButton *tbtReplan;
    TToolButton *tbtAnalyze;
    TToolButton *sep1;
    TAction *actSave;
    TAction *actReplan;
    TAction *actAnalyze;
    TMenuItem *N3;
    TMenuItem *N4;
    TMenuItem *N5;
    TAction *actShowUnwantedTx;
    TAction *actShowWantedTx;
    TToolButton *ToolButton2;
    TMenuItem *N6;
    TMenuItem *N7;
    TMenuItem *N8;
    TMenuItem *N9;
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall grdPlanDrawCell(TObject *Sender, int ACol, int ARow,
          TRect &Rect, TGridDrawState State);
    void __fastcall grdSortDrawCell(TObject *Sender, int ACol,
          int ARow, TRect &Rect, TGridDrawState State);
    void __fastcall pcPlanningChange(TObject *Sender);
    void __fastcall actCh2CopyExecute(TObject *Sender);
    void __fastcall actCh2TxExecute(TObject *Sender);
    void __fastcall actSaveExecute(TObject *Sender);
    void __fastcall actReplanExecute(TObject *Sender);
    void __fastcall actAnalyzeExecute(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall grdPlanMouseDown(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
    void __fastcall grdPlanKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall grdSortMouseDown(TObject *Sender,
          TMouseButton Button, TShiftState Shift, int X, int Y);
    void __fastcall grdSortKeyDown(TObject *Sender, WORD &Key,
          TShiftState Shift);
    void __fastcall actShowUnwantedTxExecute(TObject *Sender);
    void __fastcall actShowWantedTxExecute(TObject *Sender);
    void __fastcall tshPlanEnter(TObject *Sender);
    void __fastcall tshListUnwantSortEnter(TObject *Sender);
    void __fastcall tshListWantSortEnter(TObject *Sender);
    void __fastcall N7Click(TObject *Sender);
    void __fastcall N8Click(TObject *Sender);
    void __fastcall N9Click(TObject *Sender);
private:
    std::map<int, StandRecord> standRecords;
    void __fastcall DrawSortedList(TStringGrid *sgr, int vectorIdx);
public:		// User declarations
    __fastcall TfrmPlanning(TComponent* Owner);
    void __fastcall DrawPlan();
    void __fastcall CheckCurrentList();
protected:
    int __fastcall copyTx(int id);
    void __fastcall doMakeSelection(int txId);

    bool ctrlPressed; // используется в grdPlanDblClick
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmPlanning *frmPlanning;
//---------------------------------------------------------------------------
#endif

