//---------------------------------------------------------------------------

#ifndef uListCityH
#define uListCityH
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
//---------------------------------------------------------------------------
class TfrmListCity : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME2;
    TIntegerField *dstListDISTRICT_ID;
    TIntegerField *dstListAREA_ID;
private:	// User declarations
protected:
    __fastcall TfrmListCity(TComponent* Owner);
    virtual void __fastcall changeBranch(TTreeNode* node);
public:		// User declarations
    __fastcall TfrmListCity(TComponent* Owner, HWND callerWin, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListCity *frmListCity;
//---------------------------------------------------------------------------
#endif
