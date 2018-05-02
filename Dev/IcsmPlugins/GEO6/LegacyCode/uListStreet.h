//---------------------------------------------------------------------------

#ifndef uListStreetH
#define uListStreetH
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
class TfrmListStreet : public TfrmBaseListTree
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
    TIntegerField *dstListCITY_ID;
private:	// User declarations
protected:
    __fastcall TfrmListStreet(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListStreet(TComponent* Owner, HWND callerWin, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListStreet *frmListStreet;
//---------------------------------------------------------------------------
#endif
