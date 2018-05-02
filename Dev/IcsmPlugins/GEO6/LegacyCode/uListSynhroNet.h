//---------------------------------------------------------------------------

#ifndef uListSynhroNetH
#define uListSynhroNetH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "uBaseList.h"
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
#include <IBQuery.hpp>
//---------------------------------------------------------------------------
class TfrmListSynhroNet : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListSYNHRONETID;
    TIntegerField *dstListTYPESYNHRONET_ID;
    TIBQuery *ibqTypeSFN;
    TIntegerField *ibqTypeSFNID;
    TIBStringField *ibqTypeSFNTYPENAME;
    TIBStringField *dstListTYPENAME;
private:	// User declarations
    __fastcall TfrmListSynhroNet(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListSynhroNet(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListSynhroNet *frmListSynhroNet;
//---------------------------------------------------------------------------
#endif
