//---------------------------------------------------------------------------

#ifndef uListAreaH
#define uListAreaH
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
class TfrmListArea : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
    TIntegerField *dstListCOUNTRY_ID;
    TIBStringField *dstListCOUNTRY_NAME;
    TIBStringField *dstListNUMREGION;
private:	// User declarations
protected:
    __fastcall TfrmListArea(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListArea(TComponent* Owner, HWND callerWin, int elementId);
    __fastcall TfrmListArea(bool MDIChild);    
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListArea *frmListArea;
//---------------------------------------------------------------------------
#endif
