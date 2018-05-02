//---------------------------------------------------------------------------

#ifndef uListMinFieldStrengthH
#define uListMinFieldStrengthH
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
#include <ImgList.hpp>
#include <ToolWin.hpp>
#include <IBSQL.hpp>
//---------------------------------------------------------------------------
class TfrmListMinFieldStrength : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TSmallintField *dstListTYPEAREA;
    TSmallintField *dstListTYPESERVICE;
    TFloatField *dstListMINFIELDSTENGTH;
    TIntegerField *dstListSYSTEMCAST_ID;
private:	// User declarations
    __fastcall TfrmListMinFieldStrength(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListMinFieldStrength(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListMinFieldStrength *frmListMinFieldStrength;
//---------------------------------------------------------------------------
#endif
