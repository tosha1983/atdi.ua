//---------------------------------------------------------------------------

#ifndef uListAccountConditionH
#define uListAccountConditionH
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
#include <Grids.hpp>
#include <IBCustomDataSet.hpp>
#include <IBDatabase.hpp>
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include "uBaseList.h"
#include <ExtCtrls.hpp>
#include <IBSQL.hpp>
#include <IBQuery.hpp>
//---------------------------------------------------------------------------
class TfrmListAccountCondition : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListCODE;
    TIBStringField *dstListNAME;
    TSmallintField *dstListTYPECONDITION;
private:	// User declarations
protected:
    __fastcall TfrmListAccountCondition(TComponent* Owner);
    virtual void __fastcall fillTree();
public:		// User declarations
    __fastcall TfrmListAccountCondition(TComponent* Owner, HWND callerWin, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListAccountCondition *frmListAccountCondition;
//---------------------------------------------------------------------------
#endif
