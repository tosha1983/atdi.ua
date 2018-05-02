//---------------------------------------------------------------------------

#ifndef uListBankH
#define uListBankH
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
class TfrmListBank : public TfrmBaseList
{
__published:	// IDE-managed Components
    TIntegerField *dstListID;
    TIBStringField *dstListNAME;
    TIBStringField *dstListMFO;
    TIBStringField *dstListADDRESS;
    void __fastcall dstListBeforePost(TDataSet *DataSet);
private:	// User declarations
protected:
    __fastcall TfrmListBank(TComponent* Owner);
public:		// User declarations
    __fastcall TfrmListBank(TComponent* Owner, HWND caller, int elementId);
};
//---------------------------------------------------------------------------
extern PACKAGE TfrmListBank *frmListBank;
//---------------------------------------------------------------------------
#endif
