//---------------------------------------------------------------------------

#ifndef uListTPOnBorderH
#define uListTPOnBorderH
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
#include <IBSQL.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <IBQuery.hpp>
//---------------------------------------------------------------------------
class TfrmListTPOnBorder : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIntegerField *dstListNUMBOUND;
    TFloatField *dstListLATITUDE;
    TFloatField *dstListLONGITUDE;
    TAction *actShowOnMap;
    TToolButton *tbtShowOnMap;
    void __fastcall dstListLATITUDEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstListLONGITUDEGetText(TField *Sender,
          AnsiString &Text, bool DisplayText);
    void __fastcall dstListLATITUDESetText(TField *Sender,
          const AnsiString Text);
    void __fastcall dstListLONGITUDESetText(TField *Sender,
          const AnsiString Text);
    void __fastcall actShowOnMapExecute(TObject *Sender);
private:	// User declarations
protected:
    __fastcall TfrmListTPOnBorder(TComponent* Owner);
    void __fastcall changeBranch(TTreeNode *newNode);
public:		// User declarations
    __fastcall TfrmListTPOnBorder(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListTPOnBorder *frmListTPOnBorder;
//---------------------------------------------------------------------------
#endif
