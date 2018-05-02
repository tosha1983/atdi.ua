//---------------------------------------------------------------------------

#ifndef uListDistrictH
#define uListDistrictH
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
class TfrmListDistrict : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
    TIntegerField *dstListAREA_ID;
private:	// User declarations
    __fastcall TfrmListDistrict(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListDistrict(TComponent* Owner, HWND caller, int elementId);
protected:
    virtual void __fastcall changeBranch(TTreeNode* node);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListDistrict *frmListDistrict;
//---------------------------------------------------------------------------
#endif
